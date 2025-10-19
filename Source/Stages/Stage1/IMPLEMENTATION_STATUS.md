# Stage 1 Implementation - Complete Structure

## Overview

Stage 1 opening sequence fully implemented with clean, consolidated structure.

## File Organization

### Source Code (10 C# Scripts)

Location: `Source/Scripts/Stages/Stage1/`

1. **TerminalBase.cs** - Foundation class for all terminal scenes
   - Shader management (phosphor, scanline, glitch)
   - Text display with typewriter effect
   - Choice presentation system
   - Text input handling
   - Scene transition management
   - Audio playback control

2. **DreamweaverScore.cs** - Scoring system (Autoload singleton)
   - Tracks Light/Shadow/Ambition points
   - Determines dominant thread
   - Choice history tracking
   - Balance ending logic (60% threshold)

3. **BootSequence.cs** - System initialization scene
   - Boot messages display
   - Auto-transitions to OpeningMonologue

4. **OpeningMonologue.cs** - Opening narrative
   - Displays introduction text
   - Auto-transitions to Question1

5. **Question1Name.cs** - Story type selection
   - HERO/SHADOW/AMBITION choice
   - Records 2 points to chosen thread

6. **Question2Bridge.cs** - Bridge knowledge question
   - BRIDGE APPEARS (Shadow: 3) / KEY WITHIN (Ambition: 3)
   - Transitions to Question3

7. **Question3Voice.cs** - Voice from below question
   - DON'T BELONG (Shadow: 3) / WAITING (Light: 3)
   - Transitions to Question4

8. **Question4Name.cs** - Player name input
   - Text input field
   - Stores player name statically
   - Transitions to Question5

9. **Question5Secret.cs** - Secret keeping question
   - YES (Light: 2, Shadow: 2)
   - NO (Ambition: 4)
   - MUTUAL (Light: 2, Ambition: 2)
   - Transitions to Question6

10. **Question6Continue.cs** - Final continue and thread determination
    - Calculates dominant thread
    - Displays thread affinity message
    - Shows score summary
    - Prepares for Stage 2 (TODO)

### Godot Scenes (9 .tscn Files)

Location: `Source/Stages/Stage1/`

- **TerminalBase.tscn** - Base scene template
- **BootSequence.tscn** - Inherits TerminalBase
- **OpeningMonologue.tscn** - Inherits TerminalBase
- **Question1_Name.tscn** - Inherits TerminalBase
- **Question2_Bridge.tscn** - Inherits TerminalBase
- **Question3_Voice.tscn** - Inherits TerminalBase
- **Question4_Name.tscn** - Inherits TerminalBase
- **Question5_Secret.tscn** - Inherits TerminalBase
- **Question6_Continue.tscn** - Inherits TerminalBase

### Tests (11 Files)

Location: `Tests/Stages/Stage1/`

**Test Files:**

- ContentBlockTests.cs
- ErrorHandlingTests.cs
- NarrativeScriptFunctionalTests.cs
- NeverGoAloneControllerTests.cs

**Test Helpers:**

- GamepadInput.cs
- KeyInput.cs
- MouseInput.cs
- GamepadNavigation.cs
- KeyboardNavigation.cs
- InputMethodType.cs
- TestInputSpamHarness.cs

## Sequence Flow

```
Opening.tscn (Entry Point)
    ↓
BootSequence
    ↓
OpeningMonologue
    ↓
Question1_Name (Story Type)
    ↓
Question2_Bridge (Bridge Knowledge)
    ↓
Question3_Voice (Voice from Below)
    ↓
Question4_Name (Player Name Input)
    ↓
Question5_Secret (Secret Keeping)
    ↓
Question6_Continue (Thread Determination)
    ↓
Stage 2 (TODO)
```

## Scoring System

### Point Distribution

- Question 1 (Story Type): 2 points
- Question 2 (Bridge): 3 points
- Question 3 (Voice): 3 points
- Question 5 (Secret): 4 points
- **Total Maximum**: 12 points (or 8 if secret skipped)

### Thread Determination

- **Light**: Moral certainty, conviction, protection
- **Shadow**: Observation, patience, hidden truths
- **Ambition**: Self-empowerment, transformation, risk
- **Balance**: No thread reaches 60% (philosophical complexity)

### Tiebreaker Order

If scores are equal: Light → Shadow → Ambition

## Project Configuration

### Autoloads (project.godot)

```
DreamweaverScore="*res://Source/Scripts/Stages/Stage1/DreamweaverScore.cs"
```

### Entry Point

```
run/main_scene="res://Source/Stages/MainMenu/PressStartMenu.tscn"
```

## Build Status

✅ Project builds successfully with 0 warnings, 0 errors
✅ Single OmegaSpiral.csproj (no solution file needed)
✅ Pre-commit hooks updated to use .csproj directly

## TODO

- [ ] Create actual Stage 1 integration tests (ContentBlockTests exist but need Stage 1-specific tests)
- [ ] Implement shader files (crt_phosphor.tres, crt_scanlines.tres, crt_glitch.tres)
- [ ] Add audio assets (typewriter, UI sounds, ambient)
- [ ] Add transition animations
- [ ] Connect Question6 to Stage 2 entry point
- [ ] Implement DreamweaverScore persistence for Stage 2

## Testing

Current tests are for narrative/content systems. Need to add:

- DreamweaverScore unit tests
- Scene transition tests
- Choice recording tests
- Thread determination tests
