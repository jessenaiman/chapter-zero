#!/usr/bin/env python3
"""
Git Checkin Agent - A simple AI-powered git automation tool using Qwen-Agent framework.

This agent can help with common git operations like:
- Checking git status
- Adding files to staging
- Creating commits with intelligent messages
- Pushing changes to remote repositories
"""

import subprocess
import json
import os
from typing import Dict, List, Optional, Union
from qwen_agent.tools.base import BaseTool, register_tool
from qwen_agent.agents import Assistant


@register_tool('git_status')
class GitStatusTool(BaseTool):
    """Tool for checking git repository status."""

    description = 'Check the current status of a git repository including staged, unstaged, and untracked files.'
    parameters = []

    def call(self, params: str, **kwargs) -> str:
        """Execute git status command and return formatted results."""
        try:
            # Get git status in porcelain format for easier parsing
            result = subprocess.run(
                ['git', 'status', '--porcelain'],
                capture_output=True,
                text=True,
                check=True
            )

            if not result.stdout.strip():
                return "Repository is clean. No changes to commit."

            # Parse the porcelain output
            lines = result.stdout.strip().split('\n')
            staged = []
            unstaged = []
            untracked = []

            for line in lines:
                if line.startswith('??'):
                    untracked.append(line[3:])  # Remove '?? ' prefix
                elif line.startswith('A'):
                    staged.append(f"Added: {line[3:]}")
                elif line.startswith('M'):
                    if line[1] == ' ':  # Staged modified
                        staged.append(f"Modified: {line[3:]}")
                    else:  # Unstaged modified
                        unstaged.append(f"Modified: {line[2:]}")
                elif line.startswith('D'):
                    if line[1] == ' ':  # Staged deleted
                        staged.append(f"Deleted: {line[3:]}")
                    else:  # Unstaged deleted
                        unstaged.append(f"Deleted: {line[2:]}")

            # Format the output
            output = "Git Status:\n"
            if staged:
                output += f"\nStaged files ({len(staged)}):\n" + '\n'.join(f"  • {f}" for f in staged)
            if unstaged:
                output += f"\nUnstaged files ({len(unstaged)}):\n" + '\n'.join(f"  • {f}" for f in unstaged)
            if untracked:
                output += f"\nUntracked files ({len(untracked)}):\n" + '\n'.join(f"  • {f}" for f in untracked)

            return output

        except subprocess.CalledProcessError as e:
            return f"Error checking git status: {e}"
        except FileNotFoundError:
            return "Git is not installed or not in PATH."
        except Exception as e:
            return f"Unexpected error: {e}"


@register_tool('git_add')
class GitAddTool(BaseTool):
    """Tool for adding files to git staging area."""

    description = 'Add files to git staging area. Can add specific files or all changes.'
    parameters = [{
        'name': 'files',
        'type': 'string',
        'description': 'Files to add. Use "." for all changes, or specific file names.',
        'required': True
    }]

    def call(self, params: str, **kwargs) -> str:
        """Execute git add command."""
        try:
            # Parse the files parameter
            files = json.loads(params)['files']

            # Execute git add
            if files == ".":
                result = subprocess.run(['git', 'add', '.'], capture_output=True, text=True, check=True)
            else:
                # Handle multiple files
                if isinstance(files, str):
                    files = [files]
                result = subprocess.run(['git', 'add'] + files, capture_output=True, text=True, check=True)

            return f"Successfully added files to staging area."

        except subprocess.CalledProcessError as e:
            return f"Error adding files: {e}"
        except json.JSONDecodeError:
            return "Invalid files parameter format. Expected JSON string."
        except Exception as e:
            return f"Unexpected error: {e}"


@register_tool('git_commit')
class GitCommitTool(BaseTool):
    """Tool for creating git commits."""

    description = 'Create a git commit with an intelligent message based on the changes.'
    parameters = [{
        'name': 'message',
        'type': 'string',
        'description': 'Commit message. If not provided, will generate based on changes.',
        'required': False
    }]

    def call(self, params: str, **kwargs) -> str:
        """Execute git commit command."""
        try:
            # Parse parameters
            params_dict = json.loads(params) if params else {}
            commit_message = params_dict.get('message', '')

            if not commit_message:
                # Generate commit message based on changes
                commit_message = self._generate_commit_message()

            # Execute git commit
            result = subprocess.run(
                ['git', 'commit', '-m', commit_message],
                capture_output=True,
                text=True,
                check=True
            )

            return f"Successfully created commit: '{commit_message}'"

        except subprocess.CalledProcessError as e:
            if "nothing to commit" in str(e):
                return "No changes to commit. Use git_add first to stage files."
            return f"Error creating commit: {e}"
        except Exception as e:
            return f"Unexpected error: {e}"

    def _generate_commit_message(self) -> str:
        """Generate a commit message based on the types of changes."""
        try:
            # Get diff stats
            result = subprocess.run(
                ['git', 'diff', '--cached', '--stat'],
                capture_output=True,
                text=True
            )

            if result.returncode != 0:
                return "Update code"

            output = result.stdout.strip()
            if not output:
                return "Update code"

            # Simple message generation based on file types and changes
            lines = output.split('\n')
            if len(lines) >= 2:
                # Extract file information from the stat line
                stat_line = lines[-1] if lines else ""
                if "files changed" in stat_line:
                    return "Update multiple files"
                elif "file changed" in stat_line:
                    return "Update file"

            return "Update code"

        except Exception:
            return "Update code"


@register_tool('git_push')
class GitPushTool(BaseTool):
    """Tool for pushing commits to remote repository."""

    description = 'Push committed changes to the remote repository.'
    parameters = []

    def call(self, params: str, **kwargs) -> str:
        """Execute git push command."""
        try:
            # Check if there are unpushed commits first
            result = subprocess.run(
                ['git', 'log', '--oneline', 'origin/HEAD..HEAD'],
                capture_output=True,
                text=True
            )

            if result.returncode != 0 or not result.stdout.strip():
                return "No commits to push. All changes are up to date."

            # Execute git push
            result = subprocess.run(['git', 'push'], capture_output=True, text=True, check=True)

            return "Successfully pushed changes to remote repository."

        except subprocess.CalledProcessError as e:
            if "origin" in str(e) and "does not appear to be a git repository" in str(e):
                return "No remote repository configured. Set up remote with 'git remote add origin <url>'"
            return f"Error pushing changes: {e}"
        except Exception as e:
            return f"Unexpected error: {e}"


def create_git_checkin_agent(llm_config: Dict[str, str]) -> Assistant:
    """Create a git checkin agent with all git tools."""

    # Define the tools for git operations
    tools = ['git_status', 'git_add', 'git_commit', 'git_push']

    # System message to guide the agent's behavior
    system_message = """
You are a Git Checkin Agent. You help users with common git operations.

Available operations:
1. Check repository status (git_status)
2. Add files to staging (git_add)
3. Create commits (git_commit)
4. Push changes to remote (git_push)

Always check status first to understand what changes exist.
Guide users through the typical workflow: status → add → commit → push.
Provide clear explanations of what each operation does.
If users ask for something unclear, ask for clarification.
"""

    # Create the agent
    agent = Assistant(
        name="Git Checkin Agent",
        description="I help with git operations like checking status, staging files, committing, and pushing changes.",
        llm=llm_config,
        function_list=tools,
        system_message=system_message
    )

    return agent


def main():
    """Example usage of the git checkin agent."""

    # Multiple LLM configuration options:

    # Option 1: Local Ollama (Free, open source)
    # First install Ollama from https://ollama.com/
    # Then run: ollama pull qwen2.5:7b-instruct
    llm_config = {
        'model': 'qwen2.5:7b-instruct',
        'model_server': 'http://localhost:11434/v1',
        'api_key': 'EMPTY'  # Ollama doesn't need an API key
    }

    # Option 2: Local vLLM server (Free, open source)
    # First install and run vLLM server with a Qwen model
    # llm_config = {
    #     'model': 'Qwen/Qwen2.5-7B-Instruct',
    #     'model_server': 'http://localhost:8000/v1',
    #     'api_key': 'EMPTY'  # vLLM doesn't need an API key when no auth
    # }

    # Option 3: DashScope (requires API key from https://dashscope.aliyun.com/)
    # llm_config = {
    #     'model': 'qwen3-32b',
    #     'model_server': 'dashscope',
    #     'api_key': os.getenv('DASHSCOPE_API_KEY', '')
    # }

    # Option 4: OpenAI API (requires paid API key)
    # llm_config = {
    #     'model': 'gpt-3.5-turbo',
    #     'model_server': 'https://api.openai.com/v1',
    #     'api_key': os.getenv('OPENAI_API_KEY', '')
    # }

    print("Using Ollama configuration (free local model)")
    print("Make sure Ollama is running: ollama pull qwen2.5:7b-instruct")

    # Create the agent
    agent = create_git_checkin_agent(llm_config)

    # Example conversation
    print("Git Checkin Agent Ready!")
    print("Available commands:")
    print("- 'check status' or 'git status'")
    print("- 'add all files' or 'stage all changes'")
    print("- 'commit with message' or 'create commit'")
    print("- 'push changes' or 'push to remote'")
    print("- 'help' for more information")
    print()

    # Simple chat loop example
    while True:
        try:
            user_input = input("Your request: ").strip()
            if not user_input:
                continue

            if user_input.lower() in ['exit', 'quit', 'bye']:
                print("Goodbye!")
                break

            # Send message to agent
            messages = [{'role': 'user', 'content': user_input}]

            print(f"\nAgent response:")
            response_text = ""
            for response in agent.run(messages):
                response_text = response[-1]['content'] if response else ""
                print(response_text)

            print()

        except KeyboardInterrupt:
            print("\nGoodbye!")
            break
        except Exception as e:
            print(f"Error: {e}")


if __name__ == "__main__":
    main()
