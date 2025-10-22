# GDUnit4 Game Agent

An AI-powered testing expert for Godot 4.6 game projects using GDUnit4.

## Overview

The **GDUnit4GameAgent** is a Qwen-Agent-based advisor that provides expert guidance on testing your Omega Spiral game project. It:

- **Validates** GDUnit4 test files for correctness and best practices
- **Reviews** UI test screenshots for coverage and anomalies
- **Analyzes** test result files (HTML, TRX) for errors and provides actionable advice
- **References** all GDUnit4 documentation (RAG-style memory)
- **Integrates** with Pollinations.AI API for image generation (no signup, globally available)

## Features

### 1. RAG-Style Memory
Loads all GDUnit4 documentation from `.github/instructions/testing/` and embeds it in the agent's system prompt. This gives the agent deep knowledge of:
- GDUnit4 testing patterns and best practices
- Scene runner usage
- Mocking and signaling
- Parametrized tests
- Test adapters

### 2. Simple, Focused Design
- **No complex custom tools** - uses Qwen-Agent's built-in reasoning
- **Just HTTP calls** - integrates with Pollinations.AI via simple requests
- **Easy to extend** - add new capabilities through system prompt modifications

### 3. Pollinations.AI Integration
Uses the **free, no-signup** Pollinations API:
```python
# Image generation
https://image.pollinations.ai/prompt/YOUR_PROMPT

# Text-to-speech
https://text.pollinations.ai/YOUR_TEXT?model=openai-audio&voice=nova
```

## Usage

### As Interactive Chat
```bash
cd /path/to/.agents
python gdunit4_game_agent.py
```

Then ask questions:
```
🎮 You: Is tests/ui/terminal/TerminalBorderDiagnosticsTests.cs a valid GDUnit4 test?

🤖 Agent: [Analysis of the test file...]

🎮 You: Review the screenshots in TestResults/ui_screenshots

🤖 Agent: [Screenshot analysis...]
```

### Programmatic Usage
```python
from gdunit4_game_agent import create_gdunit4_game_agent

# Create the agent
agent = create_gdunit4_game_agent()

# Use it
messages = [
    {'role': 'user', 'content': 'Validate tests/ui/terminal/TerminalBorderDiagnosticsTests.cs'}
]

for response in agent.run(messages=messages):
    print(response['content'])
```

### MCP Integration
Register this agent as an MCP tool for use with Claude Desktop or other MCP clients:

```bash
# Install as MCP server
npx @pollinations/model-context-protocol

# Use via MCP
# The agent becomes available as a tool in your MCP environment
```

## Architecture

```
gdunit4_game_agent.py
├── load_gdunit4_docs()
│   └── Loads all .md/.mdx files from .github/instructions/testing/
├── _build_system_instruction()
│   └── Embeds documentation into system prompt
└── create_gdunit4_game_agent()
    └── Instantiates Qwen-Agent with RAG memory and system prompt
```

## Configuration

### LLM Backend
```python
llm_cfg = {
    'model': 'qwen-max-latest',
    'model_type': 'qwen_dashscope',
    # API key from DASHSCOPE_API_KEY environment variable
}

agent = create_gdunit4_game_agent(llm_cfg=llm_cfg)
```

### Alternative LLM Services
```python
# OpenAI-compatible API (vLLM, Ollama, etc.)
llm_cfg = {
    'model': 'qwen-max',
    'model_server': 'http://localhost:8000/v1',
    'api_key': 'EMPTY',
}
```

## Requirements

- Python 3.8+
- `qwen-agent` package
- API key for LLM service (DashScope, OpenAI, etc.)

## Install

```bash
# Install Qwen-Agent with all optional features
pip install "qwen-agent[gui,rag,code_interpreter,mcp]"
```

## Example Queries

1. **Validate a test file**
   ```
   "Is /path/to/tests/ui/terminal/TerminalBorderDiagnosticsTests.cs a valid GDUnit4 test?"
   ```

2. **Review screenshots**
   ```
   "Review the UI screenshots in TestResults/ui_screenshots and check for:
   Background_FillsViewport.png, Background_HasDarkGrayBezelColor.png"
   ```

3. **Analyze test results**
   ```
   "Analyze the test results in TestResults/TestResults/test-result.html
   What's failing and how do I fix it?"
   ```

4. **Ask for best practices**
   ```
   "How should I properly structure a parametrized GDUnit4 test for Godot?"
   ```

5. **Debug a failing test**
   ```
   "My test Test1_BorderWidthsAreSetOnAllSides is failing.
   Here's the test file: [paste code]
   And here's the error: [paste error]
   What's wrong?"
   ```

## Pollinations.AI Features

The agent can guide you on using Pollinations.AI for:

- **Image Generation**: `https://image.pollinations.ai/prompt/...`
- **Text Generation**: `https://text.pollinations.ai/...`
- **Text-to-Speech**: `https://text.pollinations.ai/...?model=openai-audio&voice=nova`
- **Vision/Image Analysis**: `https://text.pollinations.ai/openai` (POST with vision)
- **Function Calling**: Call external tools via Pollinations

All APIs are **free, no-signup, globally available**.

## For Omega Spiral

This agent is tailored for the Omega Spiral project:

- **Engine**: Godot 4.6
- **Language**: C#
- **Test Framework**: GDUnit4
- **Project Type**: Turn-based RPG with AI-driven narrative
- **Architecture**: SOLID principles, clean separation of concerns

The agent references Omega Spiral conventions and best practices throughout.

## Next Steps

1. ✅ **Agent Created** - `gdunit4_game_agent.py` ready to use
2. 🔜 **MCP Registration** - Expose as MCP tool for Claude Desktop
3. 🔜 **Copilot Integration** - Register with GitHub Copilot
4. 🔜 **CI/CD Integration** - Use in GitHub Actions for automated test validation

## License

Part of the Omega Spiral project. All rights reserved.
