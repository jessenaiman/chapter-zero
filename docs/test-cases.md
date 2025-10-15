# Omega Spiral: Chapter Zero Test Cases

## Content Blocks

1. **Text blocks read content correctly** - When loading a text block from YAML, the system should parse and display all text content without data loss
2. **Text waits for user interaction** - When a text block appears, it should remain on screen until user input is received, then advance to next sequential block
3. **Text dissolves at YAML sections** - When reaching a YAML-defined section boundary, text should dissolve with visual effect similar to matrix-style transitions
4. **Text appears with typewriter effect** - Each character should appear sequentially with timing delay, creating typewriter animation
5. **Typewriter effect plays typing sounds** - Each character appearance should trigger typing sound effect synchronized with visual animation
6. **Text centers in 4:3 CRT rectangle** - Text should be centered within 4:3 aspect ratio rectangle matching old CRT monitor proportions
7. **Text has CRT blur effect** - Text should have animated blur and distortion effects simulating old failing CRT screen
8. **Text options accept keyboard, mouse, or gamepad input** - Dialogue options should respond to all three input methods for selection

## Script Tests

### Opening Scene Tests

1. **Omega script works without NobodyWho** - When NobodyWho is disabled, scene should load fallback script content successfully
2. **Omega asks three specific questions** - Scene should present exactly three questions: "One Story", "What is My/Your Name", "Can you keep a secret"

#### Sections

- **One Story Section**
  - **Three cryptic story sections exist** - Database should contain at least 3 unique cryptic story sections for this part
  - **User interacts with each section** - Each story section should require user interaction before proceeding
  - **Section exits to content block** - After completing story sections, flow should continue to regular text content block

- **What is My/Your Name Section**
  - **Omega asks for player name** - System should prompt user to enter their name
  - **Omega asks two cryptic meaning questions** - After name entry, two follow-up questions about name meaning should appear
 - **Player can answer both questions** - Both questions should accept player input and store responses
  - **Section exits to content block** - After answering questions, flow continues to text content block

- **Can you keep a secret Section**
  - **Omega asks about keeping secrets** - System should present secret question to player
  - **Player gets 3 Dreamweaver point choices** - Three options should be available, each mapping to different Dreamweaver point
  - **Equation ghostwrites with freeze** - Scientific equation should appear as ghostwritten text with screen freezing partway
  - **Omega regains control message appears** - After freeze, text should show Omega regaining system control
  - **Final cryptic message about game reality** - Final message should indicate game is real but choice already made

### Omega Tests

1. **Omega is BBG in first scene only** - Omega should be primary character only in first scene, not in other scenes
2. **Omega initiates Dreamweaver program** - Omega should start the system that creates dreamweavers
3. **Omega does not acknowledge Dreamweavers** - During gameplay, Omega should not respond to or acknowledge dreamweaver comments
4. **Omega asks questions to player in first scene only** - Direct player questions should only come from Omega in first scene
5. **Omega is not a Dreamweaver** - Omega should function as NPC for scene programming, not as one of the three dreamweavers

### Dreamweaver Tests

1. **Dreamweavers talk to each other** - During chapter-zero scenes, dreamweavers should have dialogue directed to other dreamweavers
2. **Dreamweavers react to player choices** - Dreamweavers should comment on and respond to player choices and interactions
3. **Only 3 Dreamweavers exist** - System should maintain exactly 3 dreamweaver entities, no more or less
4. **Scene tallies Dreamweaver points** - Each scene should maintain simple point system tracking question results (1 or 2 points) in array updated after each scene

## LLM and NobodyWho Tests

1. **Local LLM provides new text** - When NobodyWho is enabled, local LLM should generate fresh script content for scenes
2. **Game defaults to real gameplay datasets** - When LLM unavailable, game should use datasets from real previous gameplay sessions
3. **Script processes in one pass** - Instead of line-by-line LLM calls, entire script should be processed in single pass before scene starts
