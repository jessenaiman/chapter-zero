Here’s the updated plan for the GDUnit4GameAgent, now specifying Pollinations API usage:

---

## Project: GDUnit4GameAgent for Godot (Guido)

**Type:**
- AI-powered expert agent for video game testing
- Integrated with Godot 4.6 and GDUnit4
- Exposed as an MCP tool for automated queries and advice

**Agent Name:**
- `GDUnit4GameAgent` (file: `.agents/gdunit4_game_agent.py`)

**Purpose:**
- Acts as a testing advisor for Godot-based video games
- Validates GDUnit4 test files for correctness and best practices
- Reviews Ui test screenshots for coverage and anomalies
- Parses and analyzes test result logs for errors and actionable advice
- Uses RAG-like memory to reference all GDUnit4 documentation and project standards
- **Uses Pollinations API for any image generation or related tasks (not Periscope API)**

**Workflow:**
1. User queries the agent about a test file, Ui screenshot, test result, or requests image generation.
2. Agent loads all relevant GDUnit4 instructions and documentation.
3. Agent validates the test file, reviews screenshots, analyzes test results, and uses Pollinations API for image tasks.
4. Agent responds with clear, actionable feedback tailored to the video game project.

**Naming Conventions:**
- Agent file: `gdunit4_game_agent.py`
- MCP tool name: `gdunit4_game_agent`
- All responses and advice are contextualized for Godot video game development.

**Project Context:**
- The agent is designed for the Omega Spiral game project, written in Godot 4.6 with C# and GDUnit4 for testing.
- It helps developers quickly resolve test issues, improve coverage, and maintain high-quality game code.
- All image generation or manipulation is handled via Pollinations API for global compatibility.

---

Let me know if you want any further adjustments before implementation!
