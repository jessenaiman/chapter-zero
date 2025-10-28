# Ghost Terminal Narrative Architecture Analysis

## Current State

### YAML Structure (ghost.yaml)
The narrative is organized into **moments** with 4 main types:

1. **narrative** - Simple text display with optional visual presets
   - Lines displayed sequentially
   - Optional pause duration
   - Optional visual preset (shader effect)
   - Auto-advances to next moment

2. **question** - Multiple choice with scoring
   - Setup text
   - Choices with Dreamweaver scoring
   - Each choice tracks points for light/shadow/ambition threads
   - Selection advances to next moment

3. **composite** - Setup + Question + Continuation
   - Setup narrative text
   - Question with choices (like "question" type)
   - Continuation narrative AFTER choice is selected
   - More complex interaction pattern

4. **journalEntry** - Secret reveals with special presentation
   - Displays journal entries with visual preset (CODE_FRAGMENT_GLITCH_OVERLAY)
   - Symbol-by-symbol reveal with audio sync
   - Marked as persistent (for data retention)
   - Handled by `PresentSecretRevealCeremonyAsync()`

### GhostUi Implementation

**Current Moment Types Being Handled:**
- ✅ `narrative` - Basic text display
- ✅ `question` - Choice selection with scoring
- ✅ `composite` - Multi-part choice sequences
- ✅ `journalEntry` (as narrative with CODE_FRAGMENT_GLITCH_OVERLAY preset)

**Moment Progression Flow:**
```
PresentNextMomentAsync()
  → Switch on moment.Type
    → Narrative: PresentNarrativeMomentAsync() → PlayNarrativeSequenceAsync()
    → Question: PresentQuestionMomentAsync() → PresentChoicesAsync() + TrackDreamweaverScores()
    → Composite: PresentCompositeMomentAsync() → Setup + Choices + Continuation
    → Unknown: Error & skip
  → Auto-advance via PresentNextMomentAsync() recursion
```

**Dreamweaver Scoring:**
- Tracks light/shadow/ambition points from choices
- `TrackDreamweaverScores()` accumulates scores from ChoiceOption.Scores dict
- Final score determines dominant thread via `GetDominantDreamweaver()`

**Special Features:**
- Secret Reveal Ceremony: `PresentSecretRevealCeremonyAsync()`
  - 4-second audio buildup via GhostAudioManager
  - Symbol-by-symbol reveal with overtone sync
  - Waits for user input via `WaitForAnyKeyAsync()`
- Shader Effects: Applied via visual presets
  - boot_sequence (applied at startup)
  - terminal (default)
  - CODE_FRAGMENT_GLITCH_OVERLAY (secret reveals)

## Issues & Gaps Identified

### 1. **Boot Sequence Refactored** ✅
- Removed `visualPreset: boot_sequence` from first YAML moment
- Now handled by NarrativeUi.PlayBootSequenceAsync() automatically
- GhostUi.DisplayBootTextAsync() overrides to use first moment as boot text
- First moment is skipped in main sequence (_CurrentMomentIndex = 1)

### 2. **Missing Moment Type Handling**
- YAML has `journalEntry` type but code doesn't explicitly handle it
- Currently processed as `narrative` with special visual preset detection
- **Should add explicit `case "journalEntry"` to switch statement for clarity**

### 3. **Pause Handling**
- `pause` field in YAML is read but only applied as end-of-sequence delay
- Individual line pauses not supported (all lines display immediately)
- `timing: slow_burn` field in YAML is never used

### 4. **Text Formatting & Templating**
- YAML has placeholder: `[SYSTEM: Dreamweaver thread selected - {{THREAD_NAME}}]`
- No template substitution implemented
- Last narrative moment should replace {{THREAD_NAME}} with selected Dreamweaver

### 5. **Branching/Conditional Content**
- All moments play linearly regardless of choices
- Choices affect scoring but not narrative path
- No way to show different content based on Dreamweaver scores mid-stage

### 6. **Audio Integration** ⚠️
- GhostAudioManager exists but methods may not be fully implemented:
  - `EnterSecretRevealAsync()` - 4-second buildup
  - `PlaySymbolOvertoneAsync(int index)` - Symbol-specific tones
- Integration with narrative moments not full tested

### 7. **Final Stage Transition**
- `CompleteGhostSequenceAsync()` determines dominant thread
- Gets Dreamweaver and prints scores
- **Actual stage transition logic not shown** (likely incomplete)

## Recommended Fixes (Priority Order)

### HIGH PRIORITY
1. Add explicit `case "journalEntry"` handling in `PresentNextMomentAsync()`
2. Implement template substitution for `{{THREAD_NAME}}` in final narrative
3. Verify `CompleteGhostSequenceAsync()` actually transitions to next stage
4. Test boot sequence auto-play via NarrativeUi._Ready()

### MEDIUM PRIORITY
5. Support per-line pause timing (`pause` field on individual lines)
6. Implement `timing: slow_burn` slow text reveal effect
7. Add branching logic based on accumulated scores
8. Full audio manager integration testing

### LOW PRIORITY
9. Add narrative line auto-advance via click (instead of showing all at once)
10. Implement visual preset transitions between moments
11. Add choice highlight/focus states

## Test Checklist

- [ ] Boot sequence plays with first moment text + glitch shader
- [ ] Boot moment is skipped in main sequence
- [ ] Narrative moments display and auto-advance
- [ ] Question moments accept choices and track scores
- [ ] Composite moments show setup → choices → continuation
- [ ] Secret reveal ceremony plays with audio + symbol reveal
- [ ] Final moment shows correct Dreamweaver name
- [ ] Stage transitions to next scene after completion
- [ ] Dreamweaver scores persist correctly
