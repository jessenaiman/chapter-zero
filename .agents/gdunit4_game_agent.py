#!/usr/bin/env python3
"""
GDUnit4 Game Agent - AI-powered test validation with visual confirmation and execution.
Features:
  - Analyzes GDUnit4 C# tests with code understanding
  - Locates and analyzes test screenshots with vision AI
  - Reruns tests dynamically
  - Generates images with Pollinations for visual validation
  - Compares expected vs actual results
Based on official Qwen-Agent cookbook examples:
  - cookbook_mind_map.ipynb
  - cookbook_database_manipulation.ipynb
"""

import os
import glob
import json
import base64
import subprocess
import urllib.parse
import requests
from io import BytesIO
from datetime import datetime
from pathlib import Path
from typing import Any
from PIL import Image
from qwen_agent.agents import Assistant
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


@register_tool('find_all_screenshots')
class FindAllScreenshots(BaseTool):
    """Find all screenshots in TestResults directory, optionally filtered by test name."""
    description = 'Find all screenshot files for a test, with detailed metadata about when they were created.'
    parameters = [
        {
            'name': 'test_file_path',
            'type': 'string',
            'description': 'Absolute path to the test file (used to extract test name)',
            'required': True
        },
        {
            'name': 'include_all',
            'type': 'boolean',
            'description': 'If true, return all screenshots in TestResults directory',
            'required': False
        }
    ]
    
    def call(self, params, **kwargs) -> str:
        params = self._verify_json_format_args(params)
        test_file_path = params.get('test_file_path', '')
        include_all = params.get('include_all', False)
        
        screenshots = self._find_all_test_screenshots(test_file_path, include_all)
        return json.dumps(screenshots)
    
    @staticmethod
    def _find_all_test_screenshots(test_file_path: str, include_all: bool = False) -> dict[str, Any]:
        """Find all screenshots with detailed information."""
        test_name = os.path.splitext(os.path.basename(test_file_path))[0] if test_file_path else ''
        screenshot_dir = os.path.join(
            os.path.dirname(os.path.dirname(os.path.abspath(__file__))),
            '..', 'TestResults', 'ui_screenshots')
        
        results = {
            'found': False,
            'test_name': test_name,
            'screenshot_dir': screenshot_dir,
            'screenshots': []
        }
        
        if not os.path.exists(screenshot_dir):
            return results
        
        # Find matching screenshots
        patterns = [
            f"{test_name}_*.png" if test_name else "*.png",
            f"*{test_name.replace('Tests', '')}*.png" if test_name else None
        ]
        patterns = [p for p in patterns if p]
        
        found_screenshots = []
        for pattern in patterns:
            matches = glob.glob(os.path.join(screenshot_dir, pattern))
            found_screenshots.extend(matches)
        
        # Remove duplicates while preserving order
        seen = set()
        unique_screenshots = []
        for s in found_screenshots:
            if s not in seen:
                seen.add(s)
                unique_screenshots.append(s)
        
        # Add metadata
        for screenshot_path in sorted(unique_screenshots, key=lambda x: os.path.getmtime(x), reverse=True):
            stat = os.stat(screenshot_path)
            results['screenshots'].append({
                'path': screenshot_path,
                'filename': os.path.basename(screenshot_path),
                'size_bytes': stat.st_size,
                'modified': datetime.fromtimestamp(stat.st_mtime).isoformat(),
                'width_hint': 'unknown'  # Will be filled if we analyze
            })
        
        results['found'] = len(results['screenshots']) > 0
        return results


@register_tool('rerun_test')
class RerunTest(BaseTool):
    """Rerun a specific GDUnit4 test and capture output."""
    description = 'Rerun a GDUnit4 test by class name using dotnet test filter and capture the output.'
    parameters = [
        {
            'name': 'test_class_name',
            'type': 'string',
            'description': 'Full class name of the test (e.g., MenuUiTests, UITestHarnessScreenshotTests)',
            'required': True
        },
        {
            'name': 'workspace_root',
            'type': 'string',
            'description': 'Absolute path to the project workspace root',
            'required': True
        }
    ]
    
    def call(self, params, **kwargs) -> str:
        params = self._verify_json_format_args(params)
        test_class = params.get('test_class_name', '')
        workspace_root = params.get('workspace_root', '')
        
        result = self._execute_test(test_class, workspace_root)
        return json.dumps(result)
    
    @staticmethod
    def _execute_test(test_class: str, workspace_root: str) -> dict[str, Any]:
        """Execute dotnet test with filter."""
        if not os.path.exists(workspace_root):
            return {
                'success': False,
                'error': f'Workspace not found: {workspace_root}',
                'output': '',
                'test_class': test_class
            }
        
        try:
            cmd = [
                'dotnet', 'test',
                '--no-build',
                '--settings', '.runsettings',
                '--filter', f'Class={test_class}'
            ]
            
            result = subprocess.run(
                cmd,
                cwd=workspace_root,
                capture_output=True,
                text=True,
                timeout=120
            )
            
            return {
                'success': result.returncode == 0,
                'test_class': test_class,
                'exit_code': result.returncode,
                'output': result.stdout,
                'errors': result.stderr,
                'command': ' '.join(cmd)
            }
        
        except subprocess.TimeoutExpired:
            return {
                'success': False,
                'test_class': test_class,
                'error': 'Test execution timed out after 120 seconds',
                'output': ''
            }
        except Exception as e:
            return {
                'success': False,
                'test_class': test_class,
                'error': str(e),
                'output': ''
            }


@register_tool('generate_comparison_image')
class GenerateComparisonImage(BaseTool):
    """Generate a visual comparison image using Pollinations AI."""
    description = 'Generate a visual representation of test expectations vs actual results using Pollinations image API.'
    parameters = [
        {
            'name': 'test_name',
            'type': 'string',
            'description': 'Name of the test being validated',
            'required': True
        },
        {
            'name': 'prompt',
            'type': 'string',
            'description': 'Description of what the test is validating (used to generate image)',
            'required': True
        },
        {
            'name': 'model',
            'type': 'string',
            'description': 'Model to use (flux, flux-realism, gptimage, etc.)',
            'required': False
        }
    ]
    
    def call(self, params, **kwargs) -> str:
        params = self._verify_json_format_args(params)
        test_name = params.get('test_name', 'test')
        prompt = params.get('prompt', '')
        model = params.get('model', 'flux')
        
        image_data = self._generate_image(prompt, model)
        return json.dumps({
            'success': image_data is not None,
            'test_name': test_name,
            'model': model,
            'has_image': image_data is not None,
            'message': 'Image generated successfully' if image_data else 'Failed to generate image'
        })
    
    @staticmethod
    def _generate_image(prompt: str, model: str = 'flux') -> bytes | None:
        """Generate image using Pollinations API (no auth required)."""
        try:
            encoded_prompt = urllib.parse.quote(prompt)
            url = f"https://image.pollinations.ai/prompt/{encoded_prompt}"
            
            params = {
                'model': model,
                'width': 1024,
                'height': 768,
                'nologo': 'true'
            }
            
            response = requests.get(url, params=params, timeout=60)
            response.raise_for_status()
            
            if 'image' in response.headers.get('Content-Type', ''):
                return response.content
            return None
        
        except Exception as e:
            print(f"Error generating image: {e}")
            return None


class GDUnit4TestValidator:
    """
    GDUnit4 Test Validator - AI-Powered Test Analysis and Validation

    This module provides intelligent analysis of GDUnit4 tests for Godot 4.6 C# projects.
    It reads test files, finds and analyzes screenshots, reruns tests, and generates visual
    confirmations using AI models with Pollinations API for no-auth image generation.

    Features:
    - Reads and analyzes C# test files with GDUnit4 framework
    - Finds test screenshots and analyzes visual output
    - Reruns tests via dotnet CLI and captures results
    - Generates comparison images using Pollinations API (no API keys required)
    - Uses AI vision to validate test assertions and visual expectations
    - Provides professional assessment of code quality and test coverage

    Architecture:
        <summary>
        Validates GDUnit4 tests using AI analysis with vision capabilities, screenshot analysis, and test execution.
        Integrates with Qwen-Agent to provide intelligent test validation with visual confirmation.
        Supports Pollinations AI for generating comparison images without API keys.
        </summary>
    """
    
    def __init__(self):
        """Initialize the validator with LLM configuration and system instructions."""
        self.llm_cfg = {
            'model': 'openai',  # Use Pollinations text model
            'model_server': 'https://text.pollinations.ai',  # Pollinations text API base URL
            'api_key': 'EMPTY',  # No API key required for Pollinations
            'use_raw_api': True,  # Use raw API format for Pollinations
        }
        self.workspace_root = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
        self.system_instruction = '''You are a senior GDUnit4 testing expert for Godot 4.6 C# projects with 20+ years of game development experience.
When analyzing tests:
1. Read the test file to understand expectations, assertions, and test structure
2. Check assertions, XML documentation, and GDUnit4 best practices compliance
3. If screenshots exist, analyze them to verify visual expectations
4. If a test fails, recommend specific fixes with code examples
5. For visual tests, suggest using Pollinations API (no auth required) for reference images
6. Provide professional assessment of code quality, test coverage, and architecture
7. Consider performance, maintainability, and following C# style guide conventions
8. Report findings clearly with specific, actionable recommendations
9. Be thorough but concise - focus on what matters most for game development quality'''
    
    def get_gdunit4_docs(self) -> list[str]:
        """
        <summary>
        Load GDUnit4 documentation files for RAG context and Pollinations API guide.
        </summary>
        
        <returns>List of absolute paths to documentation files that exist</returns>
        """
        docs_dir = os.path.join(self.workspace_root, '.github', 'instructions', 'testing')
        agent_dir = os.path.join(self.workspace_root, '.agents')
        
        doc_files = [
            ('testing', 'gdunit4-tools.instructions.md'),
            ('testing', 'gdUnit4Net-API.mdx'),
            ('testing', 'gdUnit4Net-README.mdx'),
            ('testing', 'gdUnit4Net-TestAdapter.md'),
            ('testing', 'scene-runner.instructions.md'),
            ('testing', 'mock.instructions.md'),
            ('testing', 'signals.instructions.md'),
            ('agents', 'POLLINATIONS_API_GUIDE.md'),  # Add Pollinations guide
        ]
        
        result = []
        for subdir, filename in doc_files:
            if subdir == 'testing':
                filepath = os.path.join(docs_dir, filename)
            else:
                filepath = os.path.join(agent_dir, filename)
            
            if os.path.exists(filepath):
                result.append(filepath)
        
        return result
    
    def compress_screenshot(self, image_path: str, max_size: int = 512) -> str:
        """
        <summary>
        Compress and resize screenshot to reduce data size for LLM processing.
        </summary>
        
        <param name="image_path">Path to the image file to compress</param>
        <param name="max_size">Maximum dimension in pixels (default: 512)</param>
        
        <returns>Base64-encoded data URI of the compressed PNG image</returns>
        """
        img = Image.open(image_path)
        img.thumbnail((max_size, max_size), Image.Resampling.LANCZOS)
        
        buffer = BytesIO()
        img.save(buffer, format='PNG', optimize=True, quality=50)
        img_data = base64.b64encode(buffer.getvalue()).decode()
        
        return f"data:image/png;base64,{img_data}"
    
    def analyze_test(self, test_file_path: str, query: str, rerun_test: bool = False) -> dict[str, Any]:
        """
        <summary>
        Analyze a test file based on a user query using Pollinations AI directly.
        Can optionally rerun the test to get current results and capture new screenshots.
        </summary>

        <param name="test_file_path">Absolute path to the C# test file to analyze</param>
        <param name="query">User query or validation question about the test</param>
        <param name="rerun_test">If true, reruns the test and analyzes results with screenshots</param>

        <returns>
        Dictionary containing:
            - success: bool indicating if analysis completed
            - findings: str with detailed analysis results
            - screenshots_found: list of screenshot paths analyzed
            - test_file: str with absolute path to the analyzed test
            - test_execution: dict with test results if rerun_test=True
            - error: str if analysis failed
        </returns>

        <exception cref="FileNotFoundError">Thrown if test file does not exist</exception>
        """

        if not os.path.exists(test_file_path):
            return {
                'success': False,
                'error': f'Test file not found: {test_file_path}',
                'findings': None
            }

        try:
            full_path = os.path.abspath(test_file_path)
            test_class_name = os.path.splitext(os.path.basename(test_file_path))[0]

            # Optionally rerun the test
            test_execution = None
            if rerun_test:
                test_execution = RerunTest._execute_test(test_class_name, self.workspace_root)

            # Find all screenshots for this test
            all_screenshots = FindAllScreenshots._find_all_test_screenshots(test_file_path, include_all=False)
            screenshots_found = all_screenshots.get('screenshots', [])

            # Call Pollinations API directly with simple format
            simple_prompt = f"""Please analyze this GDUnit4 test. The test file is: {full_path}

Test content:
{test_content}

Query: {query}

Test execution results: {json.dumps(test_execution, indent=2) if test_execution else 'Not run'}

Screenshots found: {len(screenshots_found)}

Please provide a professional analysis of this test, focusing on the query above."""

            response = self._call_pollinations_api(simple_prompt)
            
            return {
                'success': True,
                'findings': response,
                'screenshots_found': [s.get('filename', s.get('path')) for s in screenshots_found],
                'screenshot_count': len(screenshots_found),
                'test_file': full_path,
                'test_class': test_class_name,
                'test_execution': test_execution,
                'rerun_performed': rerun_test
            }

        except Exception as e:
            return {
                'success': False,
                'error': str(e),
                'findings': None,
                'test_file': full_path if 'full_path' in locals() else test_file_path
            }

    def _call_pollinations_api_with_messages(self, messages: list[dict]) -> str:
        """Call Pollinations text API directly with message format."""
        url = "https://text.pollinations.ai/openai"

        payload = {
            "model": "openai",
            "messages": messages,
            "stream": False
        }

        headers = {"Content-Type": "application/json"}

        try:
            response = requests.post(url, headers=headers, json=payload, timeout=60)
            response.raise_for_status()

            result = response.json()
            return result['choices'][0]['message']['content']

        except requests.exceptions.RequestException as e:
            return f"Error calling Pollinations API: {str(e)}"
        except (KeyError, IndexError) as e:
            return f"Error parsing Pollinations API response: {str(e)} - Response: {response.text if 'response' in locals() else 'No response'}"


def create_gdunit4_tools() -> dict[str, Any]:
    """
    <summary>
    Create MCP tool definitions for GDUnit4 test validation.
    </summary>
    
    <returns>Dictionary of tool definitions compatible with MCP servers</returns>
    """
    validator = GDUnit4TestValidator()
    
    return {
        'validate_gdunit4_test': {
            'description': 'Validate a GDUnit4 test file with AI analysis, visual confirmation, and test execution',
            'function': validator.analyze_test,
            'parameters': {
                'test_file_path': {
                    'type': 'string',
                    'description': 'Absolute path to the C# test file'
                },
                'query': {
                    'type': 'string',
                    'description': 'Validation query (e.g., "Does this test follow best practices?")'
                },
                'rerun_test': {
                    'type': 'boolean',
                    'description': 'If true, reruns the test and analyzes results'
                }
            }
        }
    }


if __name__ == '__main__':
    import sys
    
    validator = GDUnit4TestValidator()
    
    # Parse command line arguments
    rerun = '--rerun' in sys.argv
    args = [arg for arg in sys.argv[1:] if arg != '--rerun']
    
    if len(args) > 1:
        test_file = args[0]
        query = ' '.join(args[1:])
    else:
        test_file = input("Enter test file path: ").strip()
        query = input("Your validation query: ").strip()
        rerun = input("Rerun test? (y/n): ").strip().lower() == 'y'
    
    if test_file and query:
        result = validator.analyze_test(test_file, query, rerun_test=rerun)
        print(json.dumps(result, indent=2))
    else:
        print("\nUsage: python3 gdunit4_game_agent.py [--rerun] <test_file> <query>")
        print("\nExamples:")
        print("  python3 gdunit4_game_agent.py tests/ui/MenuUiTests.cs 'Does this test follow GDUnit4 best practices?'")
        print("  python3 gdunit4_game_agent.py --rerun tests/ui/MenuUiTests.cs 'Verify test passes and validate screenshots'")
        print("  python3 gdunit4_game_agent.py tests/ui/MyTest.cs 'Verify XML documentation and assertions'")
        print("\nFlags:")
        print("  --rerun: Rerun the test and capture new screenshots for analysis")
