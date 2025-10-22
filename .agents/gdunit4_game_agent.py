#!/usr/bin/env python3
"""
GDUnit4 Game Agent - Vision-capable test validator for Godot 4.6 C# projects.
Based on official Qwen-Agent cookbook examples:
  - cookbook_mind_map.ipynb
  - cookbook_database_manipulation.ipynb
"""

import os
import glob
import base64
from io import BytesIO
from PIL import Image
from qwen_agent.agents import Assistant
from qwen_agent.utils.output_beautify import typewriter_print
from qwen_agent.tools.base import BaseTool, register_tool


@register_tool('read_test_file')
class ReadTestFile(BaseTool):
    """Read a C# test file from the filesystem to analyze test code."""
    description = 'Read a C# test file from the filesystem to analyze test code and expectations.'
    parameters = [{
        'name': 'file_path',
        'type': 'string',
        'description': 'Absolute path to the test file to read',
        'required': True
    }]
    
    def call(self, params, **kwargs) -> str:
        params = self._verify_json_format_args(params)
        file_path = params['file_path']
        try:
            with open(file_path, 'r', encoding='utf-8') as f:
                content = f.read()
            return content
        except Exception as e:
            return f"Error reading file: {str(e)}"


def get_gdunit4_docs():
    """Load GDUnit4 documentation files for RAG context."""
    workspace_root = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
    docs_dir = os.path.join(workspace_root, '.github', 'instructions', 'testing')
    doc_files = [
        'gdunit4-tools.instructions.md',
        'gdUnit4Net-API.mdx',
        'gdUnit4Net-README.mdx',
        'gdUnit4Net-TestAdapter.md',
        'scene-runner.instructions.md',
        'mock.instructions.md',
        'signals.instructions.md',
    ]
    
    return [os.path.join(docs_dir, f) for f in doc_files if os.path.exists(os.path.join(docs_dir, f))]


def find_test_screenshot(test_file_path):
    """Find screenshot for a given test file by searching TestResults/ui_screenshots."""
    test_name = os.path.splitext(os.path.basename(test_file_path))[0]
    screenshot_dir = os.path.join(
        os.path.dirname(os.path.dirname(os.path.abspath(__file__))),
        'TestResults', 'ui_screenshots')
    
    if not os.path.exists(screenshot_dir):
        return None
    
    patterns = [
        f"{test_name}_*.png",
        f"*{test_name.replace('Tests', '')}*.png"
    ]
    
    for pattern in patterns:
        matches = glob.glob(os.path.join(screenshot_dir, pattern))
        if matches:
            return matches[0]
    
    return None


def compress_screenshot(image_path, max_size=512):
    """Compress and resize screenshot to reduce data size."""
    img = Image.open(image_path)
    img.thumbnail((max_size, max_size), Image.Resampling.LANCZOS)
    
    buffer = BytesIO()
    img.save(buffer, format='PNG', optimize=True, quality=50)
    img_data = base64.b64encode(buffer.getvalue()).decode()
    
    return f"data:image/png;base64,{img_data}"


def test(test_file_path: str, query: str):
    """Analyze a test file based on a user query using the official Qwen-Agent pattern."""
    
    # Check test file exists
    if not os.path.exists(test_file_path):
        print(f"Error: Test file not found: {test_file_path}")
        return
    
    # Configure LLM (following cookbook pattern)
    llm_cfg = {
        'model': 'qwen-max',
        # Use an OpenAI-compatible model service:
        # 'model': 'Qwen2.5-7B-Instruct',
        # 'model_server': 'http://localhost:8000/v1',
        # 'api_key': 'EMPTY',
    }
    
    # System instruction (following cookbook pattern)
    system_instruction = '''You are a GDUnit4 testing expert for Godot 4.6 C# projects.
When analyzing tests:
1. First read the test file to understand what the test expects
2. Check assertions, comments, and attributes
3. If a screenshot is provided, analyze if it matches test expectations
4. Report findings clearly and concisely'''
    
    # Create agent (following cookbook_mind_map.ipynb and cookbook_database_manipulation.ipynb pattern)
    bot = Assistant(
        llm=llm_cfg,
        system_message=system_instruction,
        function_list=['read_test_file'],  # Register the read_test_file tool
        files=get_gdunit4_docs()  # Add GDUnit4 docs for RAG context
    )
    
    # Build message (following cookbook pattern)
    full_path = os.path.abspath(test_file_path)
    message_content = [
        {'text': f"Test file: {full_path}\n\nQuestion: {query}"}
    ]
    
    # Add screenshot if available
    screenshot_path = find_test_screenshot(test_file_path)
    if screenshot_path and os.path.exists(screenshot_path):
        compressed_image = compress_screenshot(screenshot_path)
        message_content.append({'image': compressed_image})
    
    messages = [{'role': 'user', 'content': message_content}]
    
    # Run agent and stream output (following cookbook pattern)
    response_plain_text = ''
    print('Agent response:')
    for ret_messages in bot.run(messages=messages):
        response_plain_text = typewriter_print(ret_messages, response_plain_text)


if __name__ == '__main__':
    import sys
    if len(sys.argv) > 2:
        test_file = sys.argv[1]
        query = ' '.join(sys.argv[2:])
    else:
        test_file = input("Enter test file path: ").strip()
        query = input("Your question: ").strip()
    
    if test_file and query:
        test(test_file, query)
    else:
        print("\nUsage: python3 gdunit4_game_agent.py <test_file> <question>")
        print("\nExamples:")
        print("  python3 gdunit4_game_agent.py tests/ui/MyTest.cs 'Is this test passing correctly?'")
        print("  python3 gdunit4_game_agent.py tests/ui/MyTest.cs 'Does the screenshot match expectations?'")
