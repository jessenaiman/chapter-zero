#!/usr/bin/env python3
"""
Script to parse the chat export JSON file and extract scene-related information.
This script addresses the issue encountered when trying to parse the large JSON
file in the interactive Python environment.
"""

import json
import re
from typing import Dict, List, Any, Tuple

def load_chat_export(file_path: str) -> Dict[str, Any]:
    """
    Load the chat export JSON file.
    
    Args:
        file_path: Path to the chat export JSON file
        
    Returns:
        Parsed JSON data as a dictionary
    """
    with open(file_path, 'r', encoding='utf-8') as f:
        data = json.load(f)
    return data

def extract_scene_messages(messages: Dict[str, Any]) -> List[Tuple[str, Dict[str, Any]]]:
    """
    Extract messages that contain scene-related information.
    
    Args:
        messages: Dictionary of messages from the chat history
        
    Returns:
        List of tuples containing (message_id, message_data) for scene-related messages
    """
    scene_related = []
    
    for msg_id, message in messages.items():
        content = message.get('content', '')
        
        # Check if message contains scene-related keywords
        if any(keyword in content.lower() for keyword in 
               ['scene', 'godot', 'dungeon', 'crt', 'terminal', 'typewriter', 
                'dreamweaver', 'opening', 'narrative', 'choice', 'echo labyrinth']):
            scene_related.append((msg_id, message))
    
    return scene_related

def extract_assistant_responses(messages: Dict[str, Any]) -> List[Tuple[str, Dict[str, Any]]]:
    """
    Extract responses from the AI assistant that contain scene specifications.
    
    Args:
        messages: Dictionary of messages from the chat history
        
    Returns:
        List of tuples containing (message_id, message_data) for assistant responses
    """
    assistant_responses = []
    
    for msg_id, message in messages.items():
        if message.get('role') == 'assistant':
            content = message.get('content', '')
            
            # Check if assistant response contains scene-related information
            if any(keyword in content.lower() for keyword in 
                   ['scene', 'godot', 'dungeon', 'crt', 'terminal', 'typewriter', 
                    'dreamweaver', 'json', 'gdscript', 'code', 'specification']):
                assistant_responses.append((msg_id, message))
    
    return assistant_responses

def extract_code_blocks(text: str) -> List[str]:
    """
    Extract code blocks from text content.
    
    Args:
        text: Text content that may contain code blocks
        
    Returns:
        List of code blocks found in the text
    """
    # Match both ``` and ~~~ code blocks
    pattern = r'```(?:\w+)?\n(.*?)```|~~~(?:\w+)?\n(.*?)~~~'
    matches = re.findall(pattern, text, re.DOTALL)
    
    code_blocks = []
    for match in matches:
        # Each match is a tuple with (content1, content2), we take the one that's not empty
        content = match[0] if match[0] else match[1]
        if content:
            code_blocks.append(content.strip())
    
    return code_blocks

def extract_json_from_text(text: str) -> List[Dict[str, Any]]:
    """
    Extract JSON objects from text content.
    
    Args:
        text: Text content that may contain JSON objects
        
    Returns:
        List of JSON objects found in the text
    """
    json_objects = []
    
    # Find text that looks like JSON objects
    # This is a simple approach - for more complex JSON parsing we might need something more robust
    pattern = r'\{(?:[^{}]|(?R))*\}'
    
    # For now, use a simpler approach to find JSON-like structures
    start_indices = [i for i, char in enumerate(text) if char == '{']
    
    for start_idx in start_indices:
        brace_count = 0
        json_str = ''
        
        for i in range(start_idx, len(text)):
            char = text[i]
            if char == '{':
                brace_count += 1
                json_str += char
            elif char == '}':
                brace_count -= 1
                json_str += char
                if brace_count == 0:
                    try:
                        # Try to parse the JSON string
                        json_obj = json.loads(json_str)
                        if isinstance(json_obj, dict) and 'scene' in str(json_obj).lower():
                            json_objects.append(json_obj)
                        break
                    except json.JSONDecodeError:
                        # If it's not valid JSON, continue looking
                        continue
            elif brace_count > 0:
                json_str += char
    
    return json_objects

def main():
    """
    Main function to parse the chat export and extract relevant information.
    """
    file_path = '/home/adam/omega-spiral/chapter-zero/chat-export-1759854349966.json'
    
    print("Loading chat export file...")
    try:
        data = load_chat_export(file_path)
        print(f"Successfully loaded JSON with {len(data)} top-level items")
    except Exception as e:
        print(f"Error loading file: {e}")
        return
    
    # The JSON structure appears to have a single chat object
    if len(data) > 0:
        chat_data = data[0]['chat']
        
        # Get the messages from history
        messages = chat_data['history']['messages']
        print(f"Found {len(messages)} messages in chat history")
        
        # Extract scene-related messages
        scene_msgs = extract_scene_messages(messages)
        print(f"Found {len(scene_msgs)} scene-related messages")
        
        # Extract assistant responses
        assistant_msgs = extract_assistant_responses(messages)
        print(f"Found {len(assistant_msgs)} assistant responses with scene info")
        
        # Process the messages to extract relevant information
        print("\n" + "="*60)
        print("SCENE-RELATED MESSAGES")
        print("="*60)
        
        for msg_id, msg in scene_msgs:
            role = msg.get('role', 'unknown')
            content = msg.get('content', '')
            print(f"\nMessage ID: {msg_id}")
            print(f"Role: {role}")
            print(f"Content preview: {content[:200]}{'...' if len(content) > 200 else ''}")
            
            # Extract and display code blocks if present
            code_blocks = extract_code_blocks(content)
            if code_blocks:
                print(f"Found {len(code_blocks)} code blocks")
                
            # Extract and display JSON objects if present
            json_objs = extract_json_from_text(content)
            if json_objs:
                print(f"Found {len(json_objs)} JSON objects")
    
        print("\n" + "="*60)
        print("ASSISTANT RESPONSES WITH SPECIFICATIONS")
        print("="*60)
        
        for msg_id, msg in assistant_msgs:
            role = msg.get('role', 'unknown')
            content = msg.get('content', '')
            print(f"\nMessage ID: {msg_id}")
            print(f"Content preview: {content[:300]}{'...' if len(content) > 300 else ''}")
            
            # Extract and display code blocks from assistant responses
            code_blocks = extract_code_blocks(content)
            if code_blocks:
                print(f"\nCode blocks in this response:")
                for i, code_block in enumerate(code_blocks):
                    print(f"  Code block {i+1}:")
                    lines = code_block.split('\n')
                    for j, line in enumerate(lines[:10]):  # Show first 10 lines of each block
                        print(f"    {line}")
                    if len(lines) > 10:
                        print(f"    ... and {len(lines) - 10} more lines")
    
    print("\n" + "="*60)
    print("EXTRACTION COMPLETE")
    print("="*60)
    print("This script identified relevant messages from the chat export.")
    print("It extracted scene-related information, code blocks, and JSON structures.")
    print("Use this information to enhance the scene documentation as needed.")

if __name__ == "__main__":
    main()