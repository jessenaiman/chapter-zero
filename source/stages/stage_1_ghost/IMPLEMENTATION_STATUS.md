# Stage 1 Implementation - Complete Structure

## Overview

Stage 1 opening sequence fully implemented with clean, consolidated structure.

## File Organization

### Source Code (10 C# Scripts)

Location: `res://source/stages/stage_1/`

1. **TerminalBase.cs** – Shared terminal presentation layer
   - Shader management (phosphor, scanline, glitch)
   - Text display with typewriter/ghostwriting effects
   - Choice presentation helpers and accessibility overlay
   - Centralised scene transition and audio playback control

2. **DreamweaverScore.cs** – Autoload scoring system
   - Tracks Light/Shadow/Ambition points recorded by Stage 1
   - Determines dominant thread with 60% balance checks
   - Maintains detailed choice history for later stages

3. **GhostTerminalCinematicDirector.cs** – Data translator
   - Loads `stage_1.json`, validates it against the schema, and builds deterministic beats
   - Provides a cached `GhostTerminalCinematicPlan` for the scene controllers

4. **GhostTerminalNarrationHelper.cs** – Rendering utilities
   - Parses inline timing tokens (e.g., `[PAUSE: 2s]`)
   - Offers helpers for glitch-style narration cues

5. **BootSequence.cs** – System initialization scene
   - Plays glitch-heavy boot lines and fades to a stable baseline
   - Auto-transitions to the opening monologue

6. **OpeningMonologue.cs** – Opening narrative
   - Streams the monologue from the cinematic plan
   - Transitions directly into the first philosophical question

7. **Question1_Name.cs** – Identity philosophy question
   - Presents “Do you have a name?” and records Dreamweaver scores
   - Sets up the story fragment handoff

8. **Question2_Bridge.cs** – Story fragment choice
   - Delivers the bridge parable and captures the philosophical pick
   - Leads into the continuation beat

9. **Question3_Voice.cs** – Story continuation bridge
   - Plays the continuation lines
   - Hands off to the secret question scene (legacy numbering keeps file order)

10. **Question5_Secret.cs** – Secret prompt and reveal
    - Presents “Can you keep a secret?”, records results, unlocks journal entries
    - Triggers the secret reveal visual preset and moves to the naming beat

11. **Question4_Name.cs** – Omega naming choice
    - Delivers the final naming setup and captures the story-alignment choice
    - Transitions into the Stage 1 wrap-up scene

12. **Question6_Continue.cs** – Thread determination and exit
    - Summarises scores, resolves `{{THREAD_NAME}}`, and prints finale lines
    - Hands control to Stage 2 via `SceneManager` with a direct fallback

### Godot Scenes (9 .tscn Files)

Location: `res://source/stages/stage_1/`

- **TerminalBase.tscn** - Base scene template
- **BootSequence.tscn** - Inherits TerminalBase
- **OpeningMonologue.tscn** - Inherits TerminalBase
- **Question1_Name.tscn** - Inherits TerminalBase
- **Question2_Bridge.tscn** - Inherits TerminalBase
- **Question3_Voice.tscn** - Inherits TerminalBase
- **Question5_Secret.tscn** - Inherits TerminalBase
- **Question4_Name.tscn** - Inherits TerminalBase (runs after the secret beat)
- **Question6_Continue.tscn** - Inherits TerminalBase

> Legacy variants of these scenes (with the original script paths) now live in
> `res://source/stages/ghost/legacy_opening/` so we can surface the alternate intro as bonus content
> without impacting the playable flow.

### Tests (11 Files)

Location: `tests/stages/stage_1/`

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

```mermaid
Opening.tscn (Entry Point)
    ↓
BootSequence
    ↓
OpeningMonologue
    ↓
Question1_Name (Story Type)
    ↓
Question2_Bridge (Bridge Knowledge)
```

## Scoring System

### Point Distribution

- Question 1 (Identity): 2 points
- Question 2 (Bridge): 3 points
- Continuation Beat (Voice): 3 points
- Secret Question: 4 points
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
DreamweaverScore="*res://source/stages/stage_1/DreamweaverScore.cs"
```

### Entry Point

```ini
run/main_scene="res://source/stages/main_menu/press_start_menu.tscn"
```

## Build Status

✅ Project builds successfully with 0 warnings, 0 errors
✅ Single OmegaSpiral.csproj (no solution file needed)
✅ Pre-commit hooks updated to use .csproj directly

## TODO

- [ ] Create actual Stage 1 integration tests (ContentBlockTests exist but need Stage 1-specific tests)
- [ ] Implement shader files (crt_phosphor.tres, crt_scanlines.tres, crt_glitch.tres)
- [ ] Add audio assets (typewriter, Ui sounds, ambient)
- [ ] Add transition animations
- [ ] Connect Question6 to Stage 2 entry point
- [ ] Implement DreamweaverScore persistence for Stage 2

## Testing

Current tests are for narrative/content systems. Need to add:

- DreamweaverScore unit tests
- Scene transition tests
- Choice recording tests
- Thread determination tests

```text
```
