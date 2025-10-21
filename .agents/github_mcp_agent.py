#!/usr/bin/env python3
"""
GitHub MCP Agent - A Qwen agent with GitHub API access via Model Context Protocol (MCP)

This agent demonstrates how to integrate Qwen-Agent with MCP servers to access external services like GitHub.
"""

import json
import os
from typing import Dict, List, Optional
from qwen_agent.tools.base import BaseTool, register_tool
from qwen_agent.agents import Assistant


@register_tool('github_repo_info')
class GitHubRepoInfoTool(BaseTool):
    """Tool for getting GitHub repository information via MCP."""

    description = 'Get information about a GitHub repository including stars, forks, issues, and more.'
    parameters = [{
        'name': 'owner',
        'type': 'string',
        'description': 'Repository owner/organization name',
        'required': True
    }, {
        'name': 'repo',
        'type': 'string',
        'description': 'Repository name',
        'required': True
    }]

    def call(self, params: str, **kwargs) -> str:
        """Get repository information via MCP GitHub server."""
        try:
            # Parse parameters
            params_dict = json.loads(params)
            owner = params_dict['owner']
            repo = params_dict['repo']

            # In a real MCP setup, this would call the actual GitHub MCP server
            # For demo purposes, we'll simulate the response
            return json.dumps({
                'owner': owner,
                'repo': repo,
                'stars': 42,
                'forks': 7,
                'open_issues': 3,
                'description': 'A sample repository for demonstration',
                'language': 'Python',
                'license': 'MIT',
                'created_at': '2024-01-01T00:00:00Z',
                'updated_at': '2024-10-20T00:00:00Z'
            }, indent=2)

        except json.JSONDecodeError:
            return "Invalid parameters format. Expected JSON string."
        except Exception as e:
            return f"Error getting repository info: {e}"


@register_tool('github_list_issues')
class GitHubListIssuesTool(BaseTool):
    """Tool for listing GitHub repository issues via MCP."""

    description = 'List open issues in a GitHub repository.'
    parameters = [{
        'name': 'owner',
        'type': 'string',
        'description': 'Repository owner/organization name',
        'required': True
    }, {
        'name': 'repo',
        'type': 'string',
        'description': 'Repository name',
        'required': True
    }, {
        'name': 'state',
        'type': 'string',
        'description': 'Issue state: open, closed, or all',
        'required': False
    }, {
        'name': 'limit',
        'type': 'integer',
        'description': 'Maximum number of issues to return',
        'required': False
    }]

    def call(self, params: str, **kwargs) -> str:
        """List repository issues via MCP GitHub server."""
        try:
            # Parse parameters
            params_dict = json.loads(params)
            owner = params_dict['owner']
            repo = params_dict['repo']
            state = params_dict.get('state', 'open')
            limit = params_dict.get('limit', 10)

            # Simulate MCP GitHub server response
            issues = []
            for i in range(min(3, limit)):  # Simulate 3 issues for demo
                issues.append({
                    'number': i + 1,
                    'title': f'Issue #{i + 1}: Sample issue title',
                    'state': state,
                    'author': 'sample-user',
                    'created_at': '2024-10-01T00:00:00Z',
                    'labels': ['bug', 'enhancement'] if i % 2 == 0 else ['question'],
                    'comments': i * 2
                })

            return json.dumps({
                'repository': f'{owner}/{repo}',
                'total_count': len(issues),
                'issues': issues
            }, indent=2)

        except json.JSONDecodeError:
            return "Invalid parameters format. Expected JSON string."
        except Exception as e:
            return f"Error listing issues: {e}"


@register_tool('github_create_issue')
class GitHubCreateIssueTool(BaseTool):
    """Tool for creating GitHub issues via MCP."""

    description = 'Create a new issue in a GitHub repository.'
    parameters = [{
        'name': 'owner',
        'type': 'string',
        'description': 'Repository owner/organization name',
        'required': True
    }, {
        'name': 'repo',
        'type': 'string',
        'description': 'Repository name',
        'required': True
    }, {
        'name': 'title',
        'type': 'string',
        'description': 'Issue title',
        'required': True
    }, {
        'name': 'body',
        'type': 'string',
        'description': 'Issue description/body',
        'required': True
    }, {
        'name': 'labels',
        'type': 'array',
        'description': 'List of label names',
        'required': False
    }]

    def call(self, params: str, **kwargs) -> str:
        """Create an issue via MCP GitHub server."""
        try:
            # Parse parameters
            params_dict = json.loads(params)
            owner = params_dict['owner']
            repo = params_dict['repo']
            title = params_dict['title']
            body = params_dict['body']
            labels = params_dict.get('labels', [])

            # Simulate MCP GitHub server response
            return json.dumps({
                'number': 123,
                'title': title,
                'body': body,
                'labels': labels,
                'state': 'open',
                'author': 'current-user',
                'created_at': '2024-10-20T00:00:00Z',
                'html_url': f'https://github.com/{owner}/{repo}/issues/123'
            }, indent=2)

        except json.JSONDecodeError:
            return "Invalid parameters format. Expected JSON string."
        except Exception as e:
            return f"Error creating issue: {e}"


def create_github_mcp_agent(llm_config: Dict[str, str]) -> Assistant:
    """Create a GitHub MCP agent with GitHub tools."""

    # Define the tools for GitHub operations
    tools = ['github_repo_info', 'github_list_issues', 'github_create_issue']

    # System message to guide the agent's behavior
    system_message = """
You are a GitHub Assistant with MCP (Model Context Protocol) access to GitHub.

Available GitHub operations:
1. Get repository information (github_repo_info) - Get stars, forks, issues, etc.
2. List repository issues (github_list_issues) - View open/closed issues
3. Create new issues (github_create_issue) - Create issues with labels

You have direct access to GitHub through MCP servers, so you can:
- Check repository statistics and information
- Browse and search issues
- Create new issues and assign labels
- Get real-time data from GitHub repositories

Always provide clear, actionable information about repositories and issues.
When creating issues, suggest appropriate labels based on the content.
"""

    # Create the agent
    agent = Assistant(
        name="GitHub MCP Agent",
        description="I help with GitHub repository management and issue tracking through MCP integration.",
        llm=llm_config,
        function_list=tools,
        system_message=system_message
    )

    return agent


def setup_github_mcp_server():
    """Instructions for setting up GitHub MCP server."""

    instructions = """
# GitHub MCP Server Setup

To give your Qwen agent real GitHub access, you need to set up an MCP server:

## Option 1: Using mcp-github server

1. Install the GitHub MCP server:
```bash
npm install -g @modelcontextprotocol/server-github
```

2. Set up a GitHub Personal Access Token:
   - Go to GitHub Settings > Developer settings > Personal access tokens
   - Create a new token with 'repo' and 'issues' permissions
   - Set GITHUB_TOKEN environment variable

3. Configure the MCP server in your agent:
```python
tools = [{
    "mcpServers": {
        "github": {
            "command": "npx",
            "args": [
                "-y",
                "@modelcontextprotocol/server-github"
            ],
            "env": {
                "GITHUB_TOKEN": "your_github_token_here"
            }
        }
    }
}]
```

## Option 2: Using custom MCP server

You can also create a custom MCP server that wraps the GitHub API or uses other GitHub tools.

## Available Operations

Once connected, your agent can:
- Get repository metadata (stars, forks, language, etc.)
- List and search issues and pull requests
- Create new issues with labels
- Comment on existing issues
- Get user and organization information

This provides much more comprehensive GitHub access than simple git commands!
"""

    print(instructions)
    return instructions


def main():
    """Demo of GitHub MCP agent."""

    print("🚀 GitHub MCP Agent Demo")
    print("=" * 50)

    # Setup instructions
    setup_github_mcp_server()

    # Configure LLM (using Ollama as example)
    llm_config = {
        'model': 'qwen2.5:7b-instruct',
        'model_server': 'http://localhost:11434/v1',
        'api_key': 'EMPTY'
    }

    print("\n🤖 Creating GitHub MCP Agent...")
    print("Note: This demo uses simulated responses.")
    print("For real GitHub access, set up the MCP server as shown above.\n")

    # Create the agent
    agent = create_github_mcp_agent(llm_config)

    # Example usage
    examples = [
        "Get information about the microsoft/dotnet repository",
        "List open issues in facebook/react",
        "Create an issue in my repository about a bug"
    ]

    print("Example queries you could ask:")
    for i, example in enumerate(examples, 1):
        print(f"{i}. '{example}'")

    print("\nThe agent would use MCP to call actual GitHub APIs for these requests!")
    print("\nTo implement real GitHub access:")
    print("1. Set up a GitHub MCP server (see instructions above)")
    print("2. Replace the demo tools with real MCP tool calls")
    print("3. Configure your GitHub token")


if __name__ == "__main__":
    main()
