#!/usr/bin/env python3
"""
MCP GitHub Setup - Real implementation of GitHub MCP integration for Qwen-Agent

This script shows how to set up actual MCP servers for GitHub access.
"""

import os
import json
import subprocess
from qwen_agent.agents import Assistant


def check_prerequisites():
    """Check if required tools are installed."""
    prerequisites = {
        'node': 'Node.js (for MCP servers)',
        'npm': 'npm package manager',
        'git': 'Git version control'
    }

    missing = []
    for cmd, description in prerequisites.items():
        try:
            subprocess.run([cmd, '--version'], capture_output=True, check=True)
            print(f"✅ {description}")
        except (subprocess.CalledProcessError, FileNotFoundError):
            print(f"❌ {description} - NOT FOUND")
            missing.append(cmd)

    if missing:
        print(f"\nMissing prerequisites: {', '.join(missing)}")
        return False

    print("\n🎉 All prerequisites installed!")
    return True


def setup_github_token():
    """Guide user through GitHub token setup."""
    print("\n🔑 GitHub Token Setup")
    print("=" * 50)

    token = os.getenv('GITHUB_TOKEN')
    if token:
        print("✅ GitHub token found in environment variable")
        return token

    print("❌ No GitHub token found!")
    print("\nTo create a GitHub Personal Access Token:")
    print("1. Go to: https://github.com/settings/tokens")
    print("2. Click 'Generate new token (classic)'")
    print("3. Select scopes: 'repo' (full repository access)")
    print("4. Copy the generated token")
    print("5. Set environment variable: export GITHUB_TOKEN=your_token_here")

    # For demo purposes, you can hardcode a token here
    # token = "ghp_your_token_here"

    return token


def create_real_github_mcp_agent():
    """Create an agent with real MCP GitHub integration."""

    # Step 1: Check prerequisites
    if not check_prerequisites():
        return None

    # Step 2: Setup GitHub token
    github_token = setup_github_token()
    if not github_token:
        print("\n⚠️  Skipping real MCP setup - no GitHub token provided")
        print("The agent will work with simulated responses only.")
        return create_demo_agent()

    # Step 3: Configure MCP servers for real GitHub access
    mcp_tools = [{
        "mcpServers": {
            "github": {
                "command": "npx",
                "args": [
                    "-y",
                    "@modelcontextprotocol/server-github"
                ],
                "env": {
                    "GITHUB_TOKEN": github_token
                }
            }
        }
    }]

    # Step 4: Configure LLM
    llm_config = {
        'model': 'qwen2.5:7b-instruct',
        'model_server': 'http://localhost:11434/v1',
        'api_key': 'EMPTY'
    }

    # Step 5: Create agent with MCP integration
    system_message = """
You are a GitHub Assistant with real MCP (Model Context Protocol) access to GitHub APIs.

You can perform these real GitHub operations:
1. Get repository information (stars, forks, issues, contributors, etc.)
2. List and search repository issues and pull requests
3. Create new issues with labels and assignments
4. Comment on existing issues and PRs
5. Get user and organization information
6. Check repository statistics and trends

Always use the MCP GitHub server for accurate, real-time data.
Be helpful in managing repositories and tracking project progress.
"""

    try:
        agent = Assistant(
            name="GitHub MCP Agent",
            description="I have real-time access to GitHub through MCP servers.",
            llm=llm_config,
            function_list=mcp_tools,
            system_message=system_message
        )

        print("\n🚀 Real GitHub MCP Agent created successfully!")
        print("This agent can now access real GitHub data through MCP servers.")

        return agent

    except Exception as e:
        print(f"\n❌ Error creating real MCP agent: {e}")
        print("Falling back to demo agent...")
        return create_demo_agent()


def create_demo_agent():
    """Create a demo agent with simulated GitHub responses."""

    # Use the custom tools we created earlier
    from github_mcp_agent import create_github_mcp_agent

    llm_config = {
        'model': 'qwen2.5:7b-instruct',
        'model_server': 'http://localhost:11434/v1',
        'api_key': 'EMPTY'
    }

    return create_github_mcp_agent(llm_config)


def main():
    """Main setup and demo function."""
    print("🔧 MCP GitHub Agent Setup")
    print("=" * 50)
    print("This script will help you set up a Qwen agent with GitHub MCP access.")

    # Create the agent (real or demo)
    agent = create_real_github_mcp_agent()

    if agent:
        print("\n💡 Example usage:")
        print("• 'What are the most starred Python repositories?'")
        print("• 'Show me open issues in facebook/react'")
        print("• 'Create an issue in my repository about a bug'")
        print("• 'How many contributors does microsoft/vscode have?'")

        print("\n🔗 Next steps:")
        print("1. Install Ollama: https://ollama.com/")
        print("2. Run: ollama pull qwen2.5:7b-instruct")
        print("3. Run: ollama serve")
        print("4. Set up GitHub MCP server as shown above")
        print("5. Run the agent and ask it about GitHub repositories!")

    return agent


if __name__ == "__main__":
    agent = main()
