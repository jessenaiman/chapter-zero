# Narrative System Architecture Review

## Current Architecture

### Narrative Folder (`source/narrative/`)
Provides **stage-agnostic** narrative framework:

**Core Classes:**
- `NarrativeScript` - Base YAML schema (title, speaker, moments[])
- `ContentBlock` - Individual narrative unit (types: narrative, question, composite)
- `ChoiceOption` - Player choice with id, text, scores
- `NarrativeElement` - Base for owner/affiliation tracking
- `NarrativeRenderer` - Basic text display with typewriter effect (handles INPUT)
- `NarrativeUi` - Extends `OmegaThemedContainer`, provides `PlayNarrativeSequenceAsync()` and `TransitionPersonaAsync()`
- `NarrativeScriptLoader` - Static YAML deserializer
- `NarrativeBeat` - Struct for sequential narrative moments

**Key Design:**
- NO input fields (philosophical requirement from design doc)
- Choice buttons only (mouse/keyboard)
- `NarrativeUi.ShaderController` - inherited from `OmegaThemedContainer` (manages visual effects)
- `NarrativeUi.TextRenderer` - inherited from base (handles typing animation)

### Ghost Stage (`source/stages/stage_1_ghost/`)
Currently **properly structured**:

**GhostUi:**
- ✅ Extends `NarrativeUi` (correct inheritance)
- ✅ Creates `GhostCinematicDirector()` to load `ghost.yaml`
- ✅ Implements `PresentNextMomentAsync()` which switches on `moment.Type`
- ✅ Handles "narrative", "question", "composite" blocks
- ✅ Tracks dreamweaver scores in `_DreamweaverScores` dict

**GhostCinematicDirector:**
- ✅ Extends `CinematicDirector<GhostTerminalCinematicPlan>` (proper caching)
- ✅ Returns YAML path: `"res://source/stages/stage_1_ghost/ghost.yaml"`
- ✅ Wraps `NarrativeScript` in stage-specific plan type

**ghost.yaml:**
- ✅ Fully populated with all narrative content from design doc
- ✅ Uses all ContentBlock fields correctly (visualPreset, options, scores, etc.)
- ✅ Includes secret fragment reveal moment with symbol text

## What Should Be Happening vs. What We See

### Expected Flow (from design doc + code structure):
1. GhostUi._Ready() calls base._Ready() → initializes OmegaThemedContainer (ShaderController, TextRenderer)
2. Loads ghost.yaml via GhostCinematicDirector
3. Calls StartGhostSequence() deferred
4. PresentNextMomentAsync() processes each moment:
   - Applies visual preset via ShaderController
   - Displays lines with typewriter effect via TextRenderer
   - Presents choices as buttons
5. Shader effects trigger (boot sequence glitch, phosphor glow, etc.)

### Actual Behavior (from screenshot):
- ✅ Scene is loading
- ❌ No narrative text appearing (lines not displayed)
- ❌ No visual effects (shader presets not applied)
- ❌ No choice buttons (UI not rendered)
- ❌ Frozen on boot-like state

## Diagnosis

**Ghost is correctly inheriting and structured.** The problem is likely:
1. **OmegaThemedContainer not initializing** - check if base class ready is being called properly
2. **ghost.yaml not deserializing** - YAML parsing error (check for syntax issues)
3. **Shader effects not implemented** - visual presets ("boot_sequence", "CODE_FRAGMENT_GLITCH_OVERLAY") might not exist
4. **Choice UI not rendering** - need to verify ChoiceContainer node in scene exists

## Next Steps (for user)
1. Check if `OmegaThemedContainer` is properly initialized
2. Verify ghost.yaml deserializes without errors
3. Check if shader presets are defined
4. Verify scene tree has ChoiceContainer node ready
