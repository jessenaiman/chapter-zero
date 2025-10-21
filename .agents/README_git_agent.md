# Git Checkin Agent

A simple AI-powered git automation tool built with the Qwen-Agent framework. This agent helps automate common git operations like checking status, staging files, committing changes, and pushing to remote repositories.

## Features

- **Git Status Checking**: View current repository status including staged, unstaged, and untracked files
- **Smart File Staging**: Add specific files or all changes to staging area
- **Intelligent Commits**: Create commits with auto-generated or custom messages
- **Remote Pushing**: Push committed changes to remote repositories
- **Interactive Chat Interface**: Natural language interaction for git operations

## Prerequisites

- Python 3.8+
- Git installed and available in PATH
- Qwen-Agent framework
- API key for your preferred LLM service (DashScope, OpenAI, etc.)

## Installation

1. **Install Qwen-Agent**:
```bash
pip install "qwen-agent[gui,rag,code_interpreter,mcp]"
```

2. **Set up API Key**:
```bash
# For DashScope (Qwen models)
export DASHSCOPE_API_KEY="your_api_key_here"

# Or set it in your script configuration
```

## Usage

### Basic Usage

```python
from git_checkin_agent import create_git_checkin_agent

# Configure your LLM
llm_config = {
    'model': 'qwen3-32b',
    'model_server': 'dashscope',
    'api_key': 'your_api_key_here'
}

# Create the agent
agent = create_git_checkin_agent(llm_config)

# Use the agent
messages = [{'role': 'user', 'content': 'check git status'}]
for response in agent.run(messages):
    print(response)
```

### Command Line Interface

Run the interactive chat interface:

```bash
python git_checkin_agent.py
```

Available commands:
- `'check status'` or `'git status'` - Check repository status
- `'add all files'` or `'stage all changes'` - Stage all changes
- `'commit with message "your message"'` - Create commit with custom message
- `'push changes'` or `'push to remote'` - Push to remote repository

### Example Workflow

```
Your request: check status

Agent response: Git Status:

Staged files (0):

Unstaged files (2):
  • Modified: git_checkin_agent.py
  • Modified: README_git_agent.md

Untracked files (1):
  • git_checkin_agent.py

Your request: add all files

Agent response: Successfully added files to staging area.

Your request: commit with message "Add git checkin agent"

Agent response: Successfully created commit: 'Add git checkin agent'

Your request: push changes

Agent response: Successfully pushed changes to remote repository.
```

## Configuration

### LLM Configuration Options

#### DashScope (Qwen Models)
```python
llm_config = {
    'model': 'qwen3-32b',  # or qwen-max, qwen-turbo, etc.
    'model_server': 'dashscope',
    'api_key': 'your_dashscope_api_key'
}
```

#### OpenAI Compatible (vLLM, Ollama, etc.)
```python
llm_config = {
    'model': 'qwen2.5-7b-instruct',
    'model_server': 'http://localhost:8000/v1',
    'api_key': 'EMPTY'  # or your API key
}
```

#### Advanced Configuration
```python
llm_config = {
    'model': 'qwen3-32b',
    'model_server': 'dashscope',
    'api_key': 'your_api_key',
    'generate_cfg': {
        'top_p': 0.8,
        'temperature': 0.7
    }
}
```

## Agent Capabilities

### Git Status Tool
- Shows staged, unstaged, and untracked files
- Provides clear formatting for easy reading
- Handles empty repositories gracefully

### Git Add Tool
- Add specific files by name
- Add all changes with "."
- Supports JSON parameter format

### Git Commit Tool
- Auto-generates commit messages based on changes
- Accepts custom commit messages
- Validates that there are changes to commit

### Git Push Tool
- Checks for unpushed commits before pushing
- Handles remote repository errors gracefully
- Provides clear feedback on push status

## Architecture

The agent is built using the Qwen-Agent framework with custom tools:

- **GitStatusTool**: Checks repository status
- **GitAddTool**: Stages files for commit
- **GitCommitTool**: Creates commits with intelligent messages
- **GitPushTool**: Pushes changes to remote

Each tool follows the Qwen-Agent `BaseTool` interface and can be used independently or as part of the complete agent system.

## Error Handling

The agent includes comprehensive error handling for:
- Git command failures
- Missing git installation
- Network/remote issues
- Invalid parameters
- Repository state issues

## Security Notes

- API keys are handled securely through environment variables or configuration
- Git operations use subprocess with proper error handling
- No sensitive data is logged or stored

## Contributing

To extend the agent:

1. Create new tools by inheriting from `BaseTool`
2. Register tools with `@register_tool('tool_name')`
3. Add tools to the `function_list` when creating the agent
4. Update the system message to include new capabilities

## License

This project is open source and available under the MIT License.
