# Stage 1 Ghost Terminal YAML Intricacies

## Custom Stage-Specific Elements in ghost.yaml

### 1. **fadeToStable Field** (Moment 0)
- Custom field: `fadeToStable: true`
- Applied to first boot sequence moment
- **GhostUi Handling**: Applies `boot_sequence` shader preset before narrative
- **Purpose**: Initiates visual boot sequence before terminal stabilizes
- **Not in base ContentBlock**: This is Stage 1 specific

### 2. **timing Field** (Moment 1)
- Custom field: `timing: slow_burn`
- Applied to second moment ("Once, there was a name...")
- **Purpose**: Controls narrative delivery pace (slowness for philosophical intro)
- **Not in base ContentBlock**: Stage 1 specific pacing marker

### 3. **owner Field** (Present on multiple moments)
- Custom field: `owner: system|omega|light|shadow|ambition`
- Identifies speaker for narrative moments and options
- **GhostUi Handling**: Used for Dreamweaver thread tracking (which persona is speaking)
- **Purpose**: Tracks who is speaking for narrative continuity and thread transitions
- **Not in base ContentBlock**: Stage 1 uses this for persona differentiation

### 4. **pause Field** (Moments 1, 5, 8)
- Field: `pause: 2.5|3.0|2.0` (seconds)
- Applied to narrative and question moments
- **GhostUi Handling**: `ConvertToNarrativeBeats()` creates final beat with `DelaySeconds`
- **Purpose**: Natural pacing between narrative and choices

### 5. **visualPreset Field** (Moment 6)
- Field: `visualPreset: CODE_FRAGMENT_GLITCH_OVERLAY`
- Applied to secret/code fragment moment
- **GhostUi Handling**: Special handling in `PresentSecretRevealCeremonyAsync()`
- **Purpose**: Triggers 4-second orchestrated audio + symbol-by-symbol reveal
- **Not standard narrative**: Stage 1 has special ceremony for this moment

### 6. **context Field** (Moment 3)
- Custom field: `context: "(Not YOUR name. The question is: do names matter?)"`
- Appears on composite moment
- **Purpose**: Provides meta-narrative context before a choice
- **Not in base ContentBlock**: Stage 1 uses for philosophical framing

### 7. **setup Field** (Moments 3, 4, 5, 8)
- Field: Array of strings for narrative setup before choice/question
- **GhostUi Handling**: Converted to NarrativeBeat and played before choices
- **Purpose**: Contextual narrative that frames the upcoming question

### 8. **continuation Field** (Moment 4)
- Field: Array of strings that play AFTER choice selection
- **GhostUi Handling**: Converted to NarrativeBeat after `PresentChoicesAsync()`
- **Purpose**: Narrative consequence/epilogue after player choice
- **Unique**: Creates choice -> continuation flow

### 9. **response Field** (Moment 5 options)
- Custom field: `response: "Good. Because this is all you will ever have of it."`
- Present on each option in question type
- **Purpose**: Individual narrative response for each choice
- **GhostUi Handling**: NOT currently used - may need implementation

### 10. **dreamweaver Field** (Moment 8 options)
- Custom field: `dreamweaver: LIGHT|SHADOW|AMBITION`
- Applied to final question options
- **Purpose**: Explicit thread assignment for final question
- **GhostUi Handling**: Used for final Dreamweaver selection

### 11. **journalEntry Field** (Moment 6)
- Custom field: `journalEntry: OMEGA_CODE_FRAGMENT_1`
- Applied to secret reveal moment
- **Purpose**: Marks this as a journal-persistent story fragment
- **GhostUi Handling**: Should be recorded in JournalSystem after presentation

### 12. **persistent Field** (Moment 6)
- Custom field: `persistent: true`
- Marks a moment as stage-persistent (carries over to next stages)
- **Purpose**: Marks code fragments that are permanent discoveries
- **GhostUi Handling**: Should trigger JournalSystem.RecordStage() with persistent flag

### 13. **philosophical Field** (Moment 4 options)
- Custom field: `philosophical: faith_through_doubt|self_discovery|truth_beneath_lies`
- Applied to composite moment options
- **Purpose**: Tags choice with philosophical archetype for Dreamweaver tracking
- **GhostUi Handling**: Could be used for advanced scoring or metadata

### 14. **scores Field** (Moment 4, 5, 8 options)
- Standard field: `scores: {light: 0|1|2, shadow: 0|1|2, ambition: 0|1|2}`
- Applied to all choice options
- **GhostUi Handling**: `TrackDreamweaverScores()` accumulates these
- **Purpose**: Tracks which Dreamweaver thread player aligns with

### 15. **id Field** (All options)
- Standard field: `id: light|shadow|ambition|yes|no|trade|etc`
- Applied to all options
- **Purpose**: Unique identifier for choice tracking and analytics
- **GhostUi Handling**: Used to match option to scoring

### 16. **Template Variables** (Moment 9)
- In text: `{{THREAD_NAME}}`
- **Purpose**: Placeholder for Dreamweaver thread name to be inserted at runtime
- **GhostUi Handling**: Should replace with actual determined thread before display
- **Not implemented yet**: Needs string interpolation in `CompleteGhostSequenceAsync()`

## Summary of Stage-Specific Intricacies

**Custom Fields That Base NarrativeUi Doesn't Handle:**
1. `fadeToStable` - Boot sequence triggering
2. `timing` - Narrative pacing (slow_burn)
3. `owner` - Speaker tracking for personas
4. `context` - Meta-narrative framing
5. `response` - Per-choice narrative response
6. `dreamweaver` - Explicit thread assignment
7. `journalEntry` - Persistent story fragments
8. `persistent` - Stage persistence flag
9. `philosophical` - Philosophical archetype tags
10. `{{THREAD_NAME}}` - Template variable for runtime substitution

**Why GhostUi Must Inherit and Override:**
- Base `NarrativeUi` handles text rendering, choices, and basic narrative beats
- Stage 1 needs:
  - Custom boot sequence (not standard boot)
  - Persona/thread tracking via `owner` field
  - Per-choice response narratives
  - Secret reveal ceremony (special case)
  - Journal persistence tracking
  - Template variable substitution
  - Timing/pacing customization

**These Cannot Be Generic** because:
- Each stage has different narrative mechanics
- Stage 2 (Nethack) will have different `owner` personas
- Stage 3 (Town) will have different choice mechanics
- Stage 4+ will have different persistence and thread models

## Testing Strategy

Tests should verify:
1. All 9 moments load correctly
2. `fadeToStable` triggers boot sequence
3. `pause` fields apply correct delays
4. All choices have valid `scores` fields
5. Secret reveal moment identified by `visualPreset: CODE_FRAGMENT_GLITCH_OVERLAY`
6. `journalEntry` field properly recorded
7. Template variables like `{{THREAD_NAME}}` are replaced
8. Final Dreamweaver is determined correctly from accumulated scores
