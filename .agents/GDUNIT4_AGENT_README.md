# GDUnit4 Game Agent - AI Test Validator

An advanced AI-powered test validation and execution agent for GDUnit4 tests in Godot 4.6 C# projects. Features vision analysis, screenshot comparison, and test execution automation.

---

## Features

### ✨ Core Capabilities

- **Code Analysis** - Deep understanding of C# test code with GDUnit4 best practices
- **Screenshot Vision** - Analyzes UI test screenshots with AI vision capabilities
- **Test Execution** - Reruns tests dynamically and captures results
- **Visual Generation** - Creates comparison images via Pollinations API (no auth required)
- **Comprehensive Reports** - Professional assessment with actionable recommendations
- **Multi-Tool Integration** - Works with 5+ specialized tools for different aspects

### 🛠️ Built-in Tools

1. **read_test_file** - Read and analyze C# test source code
2. **find_all_screenshots** - Locate all test screenshots with metadata
3. **rerun_test** - Execute dotnet test with filtering
4. **generate_comparison_image** - Create visual reference images
5. **find_all_screenshots** - Advanced screenshot discovery and matching

---

## Quick Start

### Basic Analysis
```bash
python3 .agents/gdunit4_game_agent.py tests/integration/ui/MenuUiTests.cs "Does this test follow GDUnit4 best practices?"
```

### Analysis with Test Rerun
```bash
python3 .agents/gdunit4_game_agent.py --rerun tests/integration/ui/MenuUiTests.cs "Verify tests pass and analyze results"
```

### Interactive Mode
```bash
python3 .agents/gdunit4_game_agent.py
# Then provide paths and queries when prompted
```

---

## Output Format

The agent returns a JSON response with detailed information:

```json
{
  "success": true,
  "findings": "Comprehensive analysis report...",
  "screenshots_found": ["MenuUiTests_001.png", "MenuUiTests_002.png"],
  "screenshot_count": 2,
  "test_file": "/absolute/path/to/test/file.cs",
  "test_class": "MenuUiTests",
  "test_execution": {
    "success": true,
    "exit_code": 0,
    "output": "Test results..."
  },
  "rerun_performed": true
}
```

---

## Use Cases

### 1. Quick Quality Check
```bash
python3 .agents/gdunit4_game_agent.py tests/MyTest.cs "Is this test well-structured and comprehensive?"
```

### 2. Screenshot Validation
```bash
python3 .agents/gdunit4_game_agent.py tests/UITest.cs "Analyze the screenshots - do they show correct UI layout?"
```

### 3. Test Execution + Analysis
```bash
python3 .agents/gdunit4_game_agent.py --rerun tests/MenuUiTests.cs "Rerun and verify all tests pass with visual confirmation"
```

### 4. Documentation Compliance
```bash
python3 .agents/gdunit4_game_agent.py tests/MyTest.cs "Verify XML documentation and follow C# style guide"
```

### 5. Best Practices Audit
```bash
python3 .agents/gdunit4_game_agent.py tests/GameTest.cs "Check for GDUnit4 best practices: scene runner usage, AutoFree, mocking, proper disposal"
```

---

## Advanced Usage

### Using with Pollinations API Guide

The agent has access to the **POLLINATIONS_API_GUIDE.md** for reference when recommending:
- Visual validation approaches
- Image comparison techniques
- Reference image generation for UI tests

### Screenshot Analysis

The agent can:
- ✅ Find all screenshots for a test
- ✅ Extract creation timestamps and metadata
- ✅ Compress screenshots for efficient LLM analysis
- ✅ Compare multiple screenshots to identify regression
- ✅ Suggest visual validation improvements

### Test Execution Integration

When using `--rerun` flag:
- Executes `dotnet test --filter Class={TestName}`
- Captures stdout and stderr
- Analyzes exit codes and error messages
- Integrates results into comprehensive report
- Generates new screenshots automatically

---

## Integration Examples

### MCP Server

Use via the MCP server for GitHub Copilot integration:

```bash
npx @pollinations/model-context-protocol
```

Then invoke with:
```json
{
  "tool": "validate_gdunit4_test",
  "arguments": {
    "test_file_path": "/absolute/path/to/test.cs",
    "query": "Your validation question",
    "rerun_test": false
  }
}
```

### Python Script

```python
from gdunit4_game_agent import GDUnit4TestValidator

validator = GDUnit4TestValidator()
result = validator.analyze_test(
    test_file_path="/path/to/MenuUiTests.cs",
    query="Does this test validate the MenuUi component comprehensively?",
    rerun_test=True
)

print(result['findings'])
```

### CI/CD Pipeline

```bash
#!/bin/bash
# Validate all tests during build
for test_file in tests/integration/ui/*.cs; do
    echo "Validating $test_file..."
    python3 .agents/gdunit4_game_agent.py --rerun "$test_file" "Run and validate test"
done
```

---

## System Instruction

The agent operates with this professional directive:

> "You are a senior GDUnit4 testing expert for Godot 4.6 C# projects with 20+ years of game development experience. When analyzing tests, read test files, check assertions and XML documentation, analyze screenshots if available, recommend specific fixes, suggest using Pollinations API for visual validation, and provide professional assessments of code quality, test coverage, and architecture."

This ensures:
- ✅ Professional, experienced-level analysis
- ✅ Game development context awareness
- ✅ Specific, actionable recommendations
- ✅ Best practices alignment
- ✅ Performance and maintainability focus

---

## Documentation & References

The agent has access to:

- `gdunit4-tools.instructions.md` - GDUnit4 testing tools reference
- `gdUnit4Net-API.mdx` - Complete API documentation
- `gdUnit4Net-README.mdx` - Testing framework guide
- `gdUnit4Net-TestAdapter.md` - Test adapter configuration
- `scene-runner.instructions.md` - Scene runner usage patterns
- `mock.instructions.md` - Mocking and spy techniques
- `signals.instructions.md` - Signal testing patterns
- `POLLINATIONS_API_GUIDE.md` - Vision and generation APIs

---

## Configuration

### Environment Variables

```bash
# Optional: Specify custom workspace root
export GDUNIT4_WORKSPACE=/path/to/project

# Optional: Set test timeout (seconds)
export GDUNIT4_TEST_TIMEOUT=120
```

### Qwen Model Selection

Edit `gdunit4_game_agent.py` to change model:

```python
self.llm_cfg = {
    'model': 'qwen-max',  # Change to: qwen-turbo, qwen-plus, etc.
}
```

---

## Troubleshooting

### Agent Requires DashScope API Key

**Issue**: `"No api key provided. You can set by dashscope.api_key = your_api_key"`

**Solution**: Set DashScope API key for Qwen models:
```bash
export DASHSCOPE_API_KEY=your_key_here
python3 .agents/gdunit4_game_agent.py ...
```

**Alternative**: Consider using Pollinations API for visual generation (no key required).

### Screenshots Not Found

**Issue**: Agent reports `screenshots_found: []` but tests created screenshots

**Cause**: Screenshots are stored in `TestResults/ui_screenshots/` directory

**Solution**: Verify directory exists and contains `.png` files matching test names

### Test Execution Timeout

**Issue**: `"Test execution timed out after 120 seconds"`

**Cause**: Test takes longer than default timeout

**Solution**: Modify timeout in `RerunTest._execute_test()` method

---

## Best Practices

### 1. Use Specific Queries

```bash
# ❌ Too vague
"Is this test good?"

# ✅ Specific
"Does this test validate all button states and follow GDUnit4 best practices with proper setup/teardown?"
```

### 2. Leverage Screenshot Analysis

```bash
# ✅ Include visual validation
"Analyze the screenshot - does the UI match the test's visual expectations?"
```

### 3. Combine with Test Execution

```bash
# ✅ Get current status
python3 .agents/gdunit4_game_agent.py --rerun test.cs "Verify tests pass and validate structure"
```

### 4. Use for CI/CD Validation

```bash
# ✅ Integrate into pipeline
if ! python3 .agents/gdunit4_game_agent.py --rerun "$test" "verify"; then
    echo "Test validation failed"
    exit 1
fi
```

---

## Performance Tips

- **Screenshot Compression**: Automatically reduces images to 512px for efficient analysis
- **Parallel Analysis**: Can analyze multiple tests concurrently
- **Cached Docs**: GDUnit4 documentation loaded once per session
- **Streaming Response**: Agent streams response chunks for real-time feedback

---

## Dependencies

- `qwen_agent` - Alibaba Qwen agent framework
- `pillow` - Image processing for screenshot compression
- `requests` - HTTP client for Pollinations API
- `base64` - Image encoding for vision analysis

Install with:
```bash
pip install qwen_agent pillow requests
```

---

## Related Files

- `.agents/gdunit4_game_agent.py` - Main agent implementation
- `.agents/gdunit4_mcp_server.py` - MCP server wrapper
- `.agents/POLLINATIONS_API_GUIDE.md` - Vision/generation API reference
- `.agents/MCP_RESEARCH_SUMMARY.md` - MCP protocol documentation

---

## License & Attribution

Part of the **Omega Spiral - Chapter Zero** game project. Uses Qwen-Agent framework from Alibaba and Pollinations API for image generation.

