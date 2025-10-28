# Ghost Stage Cleanup Analysis

## What Ghost IS Properly Inheriting ✅

### From NarrativeUi:
- `PlayNarrativeSequenceAsync()` - Sequential narrative beat display
- `TransitionPersonaAsync()` - Dreamweaver thread color transitions
- `PresentChoicesAsync()` - Choice button rendering
- `ClearNarrative()` - UI state reset
- `GetThreadColor()` - Thread-specific colors
- `OmegaChoicePresenter` integration - Choice UI handling

### From OmegaContainer (via NarrativeUi):
- `ShaderController` - Visual effects management
- `TextRenderer` - Text typing animation
- `AppendTextAsync()` - Append text to display
- `ClearText()` - Clear display
- `ApplyVisualPresetAsync()` - Apply shader presets
- CRT layers: `_PhosphorLayer`, `_ScanlineLayer`, `_GlitchLayer`
- `BorderFrame`, `BuildOmegaFrame()` - Omega aesthetic

## What Ghost SHOULD Be Using But Isn't ❌

### Not Using from NarrativeUi:
1. **`PlayNarrativeSequenceAsync()`** - Ghost reimplements via `PresentNarrativeMomentAsync()`
   - NarrativeUi has `NarrativeBeat` helper for exactly this
   - Ghost could use this for simpler moment handling

2. **`TransitionPersonaAsync()`** - Ghost doesn't call this at all
   - No persona color transitions implemented
   - Design doc specifies phosphor_tint crossfade during final selection
   - Ghost needs this for State 4: Dreamweaver Selection

3. **`ClearNarrative()`** - Not called between moments
   - Ghost doesn't reset ChoiceContainer visibility
   - Could cause UI state bugs

### Not Properly Using from OmegaContainer:
- `ApplyVisualPresetAsync()` calls exist but shader presets likely not defined
- No usage of `PixelDissolveAsync()` for transitions

## What Ghost Has That SHOULD Be Removed ❌

### Dead Code / Unused Classes:

1. **GhostNarrativeData.cs** - COMPLETELY UNUSED
   - Old JSON-based structure (before YAML conversion)
   - Defines `Stage1NarrativeData`, `BootSequenceData`, etc.
   - Also defines `DreamweaverChoice` and `DreamweaverScores` that don't match current architecture
   - **Action: DELETE** - ghost.yaml has replaced this entirely

2. **GhostNarrator.cs** - MINIMAL USE
   - Only has `TryParsePause()` utility
   - Ghost doesn't call it anywhere in the provided code
   - NarrativeRenderer already handles `[PAUSE: X]` parsing
   - **Action: DELETE** - duplicate functionality already in base

### Reimplemented Code That Could Be Simplified:

3. **GhostAudioManager.cs** - MASSIVE CLASS (700+ lines)
   - Reimplements a lot that could be simpler
   - Good news: Audio architecture IS needed (not in NarrativeUi)
   - BUT: Could be significantly simplified:
     - No need to manage individual AudioStreamPlayer nodes - use buses
     - Threading/Timer logic could be moved to GhostUi
     - Still KEEP, but REFACTOR for clarity

4. **GhostUi.cs Methods That Duplicate Base:**
   - `PresentNarrativeMomentAsync()` - Could use `PlayNarrativeSequenceAsync()` from base
   - `PresentQuestionMomentAsync()` - Could call `PresentChoicesAsync()` directly
   - `PresentCompositeMomentAsync()` - Combination of above
   - **Action: REFACTOR** - simplify by delegating to base class

## Architecture Mismatch Summary

### Current Ghost Structure (Over-Engineered):
```
GhostUi
  ├─ LoadGhostScript() → GhostCinematicDirector → ghost.yaml ✅
  ├─ PresentNextMomentAsync() ✅
  ├─ PresentNarrativeMomentAsync() ← Could delegate to base
  ├─ PresentQuestionMomentAsync() ← Could delegate to base
  ├─ PresentCompositeMomentAsync() ← Could delegate to base
  ├─ PresentSecretRevealCeremonyAsync() ✅ (Stage-specific)
  ├─ TrackDreamweaverScores() ✅ (Stage-specific)
  ├─ CompleteGhostSequenceAsync() ✅ (Stage-specific)
  ├─ GetDominantDreamweaver() ✅ (Stage-specific)
  ├─ GhostAudioManager ← Needs refactoring (too complex)
  ├─ GhostCinematicDirector ✅ (Good)
  ├─ GhostNarrativeData ← DELETE (unused)
  ├─ GhostNarrator ← DELETE (duplicate functionality)
  └─ Missing: TransitionPersonaAsync() calls for Dreamweaver lock-in
```

### What It Should Be (Simplified):
```
GhostUi extends NarrativeUi
  ├─ _Ready() ✅
  ├─ LoadGhostScript() → GhostCinematicDirector ✅
  ├─ StartGhostSequence() ✅
  ├─ PresentNextMomentAsync() ✅
  ├─ PresentNarrativeMomentAsync() → call base.PlayNarrativeSequenceAsync()
  ├─ PresentQuestionMomentAsync() → call base.PresentChoicesAsync()
  ├─ PresentCompositeMomentAsync() → combine above two
  ├─ PresentSecretRevealCeremonyAsync() ✅ (special case)
  ├─ TrackDreamweaverScores() ✅
  ├─ CompleteGhostSequenceAsync() ✅ + ADD TransitionPersonaAsync()
  ├─ GetDominantDreamweaver() ✅
  ├─ GhostAudioManager REFACTORED (simpler, but keep)
  └─ GhostCinematicDirector ✅
```

## Recommended Actions (Priority Order)

1. **DELETE GhostNarrativeData.cs** - Zero functionality, replaced by ghost.yaml
2. **DELETE GhostNarrator.cs** - Duplicate of base class functionality
3. **REFACTOR GhostAudioManager.cs** - Remove unnecessary complexity
4. **SIMPLIFY PresentNarrativeMomentAsync()** - Delegate to base class
5. **SIMPLIFY PresentQuestionMomentAsync()** - Use PresentChoicesAsync()
6. **ADD TransitionPersonaAsync() call** in CompleteGhostSequenceAsync()
7. **VERIFY shader presets** are defined ("boot_sequence", "CODE_FRAGMENT_GLITCH_OVERLAY", "terminal")

## Why Ghost Wasn't Using These

The Ghost code was written **before NarrativeUi had proper abstraction**. You can see:
- GhostNarrativeData is JSON-based (older pattern)
- Audio manager is self-contained (designed before Ghost became part of larger system)
- Moment presenter logic is hand-coded instead of using base class helpers

This is **excellent** - shows growth! Now we just need to refactor it to use the new architecture.
