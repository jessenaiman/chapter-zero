# Omega Spiral: Chapter Zero Test Matrix

_All tests avoid asserting specific narrative text; they verify structure, sequencing, and side effects._

## Content Block Presentation

### CB-001: Content Block Wait State

- [ ] It should remain visible when waiting without user input for 10 seconds
- [ ] It should not auto-advance when no input is received
- [ ] It should wait indefinitely until player interaction

### CB-002: Input Method Support

- [ ] It should advance content block when keyboard select is pressed
- [ ] It should advance content block when gamepad confirm is pressed
- [ ] It should advance content block when mouse click is detected
- [ ] It should respond to all configured input methods consistently

### CB-003: CRT Visual Presentation

- [ ] It should center text within a 4:3 aspect ratio frame
- [ ] It should apply CRT blur shader effect to displayed text
- [ ] It should display visible scanline effects on content
- [ ] It should maintain visual consistency with reference overlay

### CB-004: Typewriter Animation

- [ ] It should reveal characters sequentially at consistent intervals
- [ ] It should play keystroke sound effects synchronized with character appearance
- [ ] It should loop typewriter sound until line completion
- [ ] It should maintain consistent timing throughout animation

### CB-005: Section Transition Effects

- [ ] It should trigger dissolve effect at YAML-defined section boundaries
- [ ] It should fade text using dissolve shader during transitions
- [ ] It should match configured dissolve duration from settings
- [ ] It should proceed to next content block after dissolve completes

### CB-006: Choice Selection Input

- [ ] It should allow keyboard navigation to select dialogue choices
- [ ] It should allow mouse click to select dialogue choices
- [ ] It should allow gamepad input to select dialogue choices
- [ ] It should advance narrative after any valid choice selection

## Dialogue Flow (Opening Scene)

### SC-001: Script Loading Fallback

- [ ] It should load valid script object when NobodyWho plugin is disabled
- [ ] It should return scene header matching expected structure
- [ ] It should use fallback script when LLM is unavailable

### SC-002: Omega Question Sequence

- [ ] It should present three mandatory questions in fixed order
- [ ] It should show OneStory prompt first
- [ ] It should show PlayerName prompt second
- [ ] It should show Secret prompt third

### SC-003: Dreamweaver Response Mapping

- [ ] It should provide exactly three responses for each prompt
- [ ] It should map one response to HERO affinity
- [ ] It should map one response to SHADOW affinity
- [ ] It should map one response to AMBITION affinity

### SC-004: One Story Interactive Blocks

- [ ] It should offer three distinct story blocks when selected
- [ ] It should require player interaction for each story block
- [ ] It should maintain story pool with more than 3 unique entries
- [ ] It should prevent duplicate story blocks in single playthrough

### SC-005: One Story Section Structure

- [ ] It should present three cryptic story sections sequentially
- [ ] It should require user interaction before advancing each section
- [ ] It should draw from validated story pool with sufficient uniqueness
- [ ] It should track section progression accurately

### SC-006: Name Collection Flow

- [ ] It should prompt user to enter player name
- [ ] It should present two cryptic meaning questions after name entry
- [ ] It should validate and store player name correctly
- [ ] It should transition to content block after question sequence

### SC-007: Secret Choice Mechanics

- [ ] It should present three choices mapping to Dreamweaver points
- [ ] It should correctly attribute choice to corresponding affinity
- [ ] It should trigger equation ghostwriting effect when choice selected
- [ ] It should display final cryptic message about the real game

## Dreamweaver Scoring & Interaction

### DW-001: Dreamweaver Dialogue

- [ ] It should display interstitial dialogue between Dreamweavers when player is idle
- [ ] It should show Dreamweavers referencing each other in conversation
- [ ] It should keep Omega silent during Dreamweaver conversations
- [ ] It should log correct speaker IDs for each dialogue line

### DW-002: Affinity Score Updates

- [ ] It should increment selected Dreamweaver affinity by configured value when choice made
- [ ] It should leave non-selected Dreamweaver affinities unchanged
- [ ] It should maintain affinity history array for audit trail
- [ ] It should apply correct point values based on choice weight

### DW-003: Cross-Scene Persistence

- [ ] It should maintain affinity array values when transitioning to Scene 2
- [ ] It should preserve cumulative scores from previous scenes
- [ ] It should not reset affinity state between scene transitions
- [ ] It should integrate with save system for state persistence

### DW-004: Reactive Commentary

- [ ] It should trigger non-selected Dreamweaver commentary after player choices
- [ ] It should reference affinity shifts in Dreamweaver dialogue
- [ ] It should prevent Omega from acknowledging Dreamweaver presence
- [ ] It should vary commentary based on current affinity standings

### DW-005: Scene-Level Score Tracking

- [ ] It should accumulate points correctly across multiple scenes
- [ ] It should apply appropriate point values (1 or 2) per choice
- [ ] It should persist affinity array between scene transitions
- [ ] It should maintain accurate tallies throughout playthrough

## LLM / NobodyWho Integration

### NL-001: Live LLM Generation

- [ ] It should compile scene in single pass when LLM is available
- [ ] It should cache generated script output after initial request
- [ ] It should avoid per-line LLM calls during scene execution
- [ ] It should request fresh generation before scene start

### NL-002: Fallback Dataset Usage

- [ ] It should load stored dataset when LLM is unavailable
- [ ] It should note fallback mode in scene metadata
- [ ] It should use dataset from recorded real gameplay
- [ ] It should maintain scene functionality without LLM connection

### NL-003: Transcript Replay Mode

- [ ] It should replay scene using saved NobodyWho transcript
- [ ] It should avoid live LLM calls when replay flag is set
- [ ] It should log cached transcript usage for debugging
- [ ] It should validate storage path permissions before loading

## Dungeon Stage Sequence (Scene 2)

### STG-001: Dungeon Schema Loading

- [ ] It should load three distinct DungeonStage instances from valid schema
- [ ] It should assign unique owner values matching schema definition
- [ ] It should validate map dimensions against schema specifications
- [ ] It should correctly align object glyphs with legend entries

### STG-002: Owner Duplication Prevention

- [ ] It should throw domain exception when duplicate owners detected
- [ ] It should prevent aggregate creation with invalid owner sequence
- [ ] It should avoid persisting stages with duplicate owners
- [ ] It should maintain balanced affinity distribution across Dreamweavers

### STG-003: Stage Entry Event Publishing

- [ ] It should publish DungeonStageEntered event when stage begins
- [ ] It should include owner identifier in event payload
- [ ] It should include stage index in event payload
- [ ] It should include ASCII map metadata for rendering in event payload

### STG-004: Object Interaction Affinity

- [ ] It should increment affinity when interacting with owner-aligned object
- [ ] It should apply configured point value to correct Dreamweaver
- [ ] It should record interaction in affinity history
- [ ] It should leave affinity unchanged for non-aligned object interactions

### STG-005: Deterministic Stage Progression

- [ ] It should advance to next stage when current stage is completed
- [ ] It should maintain consistent sequence order with same seed
- [ ] It should generate identical layouts across runs with same seed
- [ ] It should reach third stage without skipping when progressing

## Save/Load & Persistence

### SL-001: Content Block Position Save

- [ ] It should resume at exact content block position after save and load
- [ ] It should preserve text display state when saving mid-block
- [ ] It should prevent content skipping after loading saved game
- [ ] It should prevent content duplication after loading saved game

### SL-002: Dialogue Choice Preservation

- [ ] It should maintain available choices when saving before selection
- [ ] It should restore choice screen state exactly after loading
- [ ] It should allow player to make choice after loading save
- [ ] It should preserve all Dreamweaver affinity choice options

### SL-003: Affinity State Restoration

- [ ] It should restore affinity array values exactly matching saved state
- [ ] It should preserve affinity history after save/load cycle
- [ ] It should serialize DreamweaverAffinityState correctly
- [ ] It should deserialize affinity state without data loss

### SL-004: Auto-Save Functionality

- [ ] It should create auto-save file when transitioning between scenes
- [ ] It should timestamp auto-save files for identification
- [ ] It should keep manual saves separate from auto-saves
- [ ] It should not overwrite manual save slots with auto-saves

### SL-005: Multi-Slot Independence

- [ ] It should maintain independent game state for each save slot
- [ ] It should load correct progress when switching between slots
- [ ] It should prevent cross-contamination of affinity between slots
- [ ] It should display accurate metadata for each save slot

## Error Handling & Edge Cases

### ERR-001: Name Input Validation

- [ ] It should reject empty string when player name is submitted
- [ ] It should reject special characters outside allowed set
- [ ] It should reject names exceeding maximum length
- [ ] It should display clear error message when validation fails

### ERR-002: Input Spam Protection

- [ ] It should complete content normally when input buttons are mashed rapidly
- [ ] It should maintain consistent state machine after 50+ rapid inputs
- [ ] It should prevent content skipping during typewriter effect
- [ ] It should avoid crashes when processing input spam

### ERR-003: Window Focus Handling

- [ ] It should pause typewriter effect when window loses focus
- [ ] It should resume typewriter correctly when window regains focus
- [ ] It should sync audio playback after alt-tab return
- [ ] It should handle pause menu during typewriter animation

### ERR-004: Transition Recovery

- [ ] It should recover gracefully when game crashes during scene transition
- [ ] It should load last stable save after force quit during transition
- [ ] It should offer scene selection if recovery fails
- [ ] It should prevent corrupted save state after interrupted transition

## Accessibility & UX

### ACC-001: Text Speed Configuration

- [ ] It should adjust typewriter speed when setting is changed
- [ ] It should reflect speed changes in real-time during gameplay
- [ ] It should support range from instant reveal to very slow
- [ ] It should provide at least 5 preset speed options

### ACC-002: Font Size Options

- [ ] It should display text at configured size (Small/Medium/Large)
- [ ] It should fit all text within CRT boundary at any size
- [ ] It should prevent overflow or clipping with longest dialogue lines
- [ ] It should scale CRT effect appropriately with font size changes

### ACC-003: Seen Dialogue Skipping

- [ ] It should advance to next unseen content when skip button pressed
- [ ] It should stop skipping at next player choice or unseen dialogue
- [ ] It should log skipped content for tracking
- [ ] It should display visual indicator distinguishing seen vs unseen content

### ACC-004: Dialogue History Access

- [ ] It should display at least 20 previous dialogue blocks in backlog
- [ ] It should allow scrolling through dialogue history
- [ ] It should return to current position when backlog is closed
- [ ] It should persist history for current game session

### ACC-005: Colorblind Accessibility

- [ ] It should use patterns and icons in addition to colors for Dreamweaver UI
- [ ] It should support Protanopia colorblind mode
- [ ] It should support Deuteranopia colorblind mode
- [ ] It should support Tritanopia colorblind mode

### ACC-006: Audio Volume Control

- [ ] It should adjust SFX volume independently from other audio
- [ ] It should adjust typing sounds volume independently
- [ ] It should adjust music volume independently
- [ ] It should provide mute option for each audio layer

## Dungeon Gameplay (Scene 2 Expansion)

### DNG-001: Movement Input Methods

- [ ] It should move player correctly with WASD keys
- [ ] It should move player correctly with arrow keys
- [ ] It should move player correctly with gamepad D-pad or stick
- [ ] It should move player correctly with mouse clicks if supported

### DNG-002: Collision System

- [ ] It should block player movement when attempting to walk through walls
- [ ] It should block player movement when attempting to walk through solid objects
- [ ] It should allow grid-based precise movement along valid paths
- [ ] It should prevent clipping through collision boundaries

### DNG-003: Invalid Move Feedback

- [ ] It should play bump sound when invalid move is attempted into wall
- [ ] It should trigger screen shake or bounce animation on collision
- [ ] It should maintain game state unchanged after invalid move
- [ ] It should provide clear visual feedback for boundary detection

### DNG-004: Fog of War System

- [ ] It should keep explored areas visible after player leaves them
- [ ] It should hide unexplored areas until player approaches
- [ ] It should persist revealed state across save and reload
- [ ] It should maintain consistent vision radius for exploration

### DNG-005: Object Interaction Indicators

- [ ] It should highlight objects when player is adjacent to them
- [ ] It should show tooltip or icon indicating object is interactive
- [ ] It should remove highlight when player moves away from object
- [ ] It should use consistent visual effect for all interactive objects

### DNG-006: Objective Communication

- [ ] It should display stage objective on HUD at stage start
- [ ] It should update objective progress as player advances
- [ ] It should trigger completion fanfare when objective is met
- [ ] It should provide clear message upon stage completion

### DNG-007: Failure and Respawn

- [ ] It should display game over screen when failure condition is triggered
- [ ] It should respawn player at stage start or last checkpoint
- [ ] It should preserve affinity state after respawn
- [ ] It should track retry count for difficulty adjustment

### DNG-008: Inventory Management

- [ ] It should store picked up items in player inventory
- [ ] It should display item details in inventory UI
- [ ] It should apply item effects when used or equipped
- [ ] It should allow player to drop items from inventory

## Dreamweaver Narrative Integration

### DW-006: Contextual Commentary

- [ ] It should trigger Dreamweaver commentary at appropriate story beats
- [ ] It should show comments from dominant or affected Dreamweaver after choice
- [ ] It should time commentary naturally without interrupting flow
- [ ] It should use event triggers tied to scene progression

### DW-007: Threshold-Based Content

- [ ] It should unlock unique dialogue when affinity threshold is reached
- [ ] It should provide special content only to players with high affinity
- [ ] It should hide threshold content from players with low affinity
- [ ] It should track and document affinity threshold values

### DW-008: Playthrough Variation

- [ ] It should produce different dominant Dreamweaver in HERO playthrough
- [ ] It should produce different dominant Dreamweaver in SHADOW playthrough
- [ ] It should produce different dominant Dreamweaver in AMBITION playthrough
- [ ] It should reflect choice consequences in narrative branches

### DW-009: Affinity Tie-Breaking

- [ ] It should apply consistent tie-breaker when affinity values are equal
- [ ] It should use configured priority for tie resolution
- [ ] It should prevent undefined state when multiple Dreamweavers tied
- [ ] It should document tie-breaker logic in design documentation

### DW-010: Identity Consistency

- [ ] It should maintain consistent visual identity for each Dreamweaver across scenes
- [ ] It should maintain consistent color scheme for each Dreamweaver
- [ ] It should maintain consistent audio theme for each Dreamweaver
- [ ] It should match Dreamweaver presentation to style guide specifications

## Audio/Visual Polish

### AV-001: Music Transitions

- [ ] It should fade out music smoothly when transitioning between sections
- [ ] It should fade in or crossfade new music smoothly at transition
- [ ] It should avoid abrupt music stops during scene changes
- [ ] It should prevent volume spikes during transitions

### AV-002: Sound Effect Mixing

- [ ] It should mix multiple SFX without distortion when triggered simultaneously
- [ ] It should prevent audio clipping with priority system
- [ ] It should limit simultaneous SFX instances per frame
- [ ] It should maintain clear audio when typewriter and button SFX overlap

### AV-003: Narrative Visual Effects

- [ ] It should trigger CRT glitch effect at equation ghostwrite moment
- [ ] It should synchronize screen glitch/static with narrative beat
- [ ] It should enhance immersion with appropriate effect intensity
- [ ] It should use shader animation triggered by scene events

### AV-004: Motion Sensitivity

- [ ] It should apply screen shake effects that are noticeable but not excessive
- [ ] It should provide option to reduce motion effects in settings
- [ ] It should provide option to disable motion effects completely
- [ ] It should avoid causing nausea or motion sickness in sensitive players

### AV-005: Dissolve Timing

- [ ] It should complete dissolve effect within 1-2 seconds
- [ ] It should feel smooth and natural during transition
- [ ] It should maintain player engagement without impatience
- [ ] It should allow time to appreciate visual effect

## Localization/Text

### LOC-001: Text Wrapping

- [ ] It should wrap long text at word boundaries within CRT frame
- [ ] It should prevent overflow beyond 4:3 CRT boundary
- [ ] It should remain readable at all configured font sizes
- [ ] It should calculate appropriate max characters per line for each size

### LOC-002: Special Character Rendering

- [ ] It should render unicode characters correctly (é, ñ, 中)
- [ ] It should render mathematical equations correctly
- [ ] It should use font with extended unicode character support
- [ ] It should avoid displaying boxes or question marks for unsupported characters

### LOC-003: Container Overflow Prevention

- [ ] It should fit all text within UI containers without clipping
- [ ] It should auto-resize containers when possible for text
- [ ] It should truncate text gracefully with ellipsis when needed
- [ ] It should test all UI screens with maximum-length strings

### LOC-004: Variable Length Handling

- [ ] It should maintain appropriate pacing for short text lines (5 words)
- [ ] It should allow skipping of long text lines (50+ words)
- [ ] It should maintain consistent per-character timing regardless of length
- [ ] It should support instant-complete on input during typewriter

## Integration & Flow

### INT-001: End-to-End Playthrough

- [ ] It should complete Scene 1 without errors, crashes, or warnings
- [ ] It should transition smoothly from Scene 1 to Scene 2
- [ ] It should carry forward game state correctly across scene boundary
- [ ] It should maintain console error-free throughout full playthrough

### INT-002: Cross-Scene Affinity Effects

- [ ] It should reflect HERO affinity choices from Scene 1 in Scene 2 content
- [ ] It should reflect SHADOW affinity choices from Scene 1 in Scene 2 content
- [ ] It should reflect AMBITION affinity choices from Scene 1 in Scene 2 content
- [ ] It should adapt Scene 2 dialogue, stage order, or commentary based on affinity

### INT-003: Name Persistence

- [ ] It should display player name in Scene 2 dialogue after capture in Scene 1
- [ ] It should display player name in UI elements across all scenes
- [ ] It should store player name in save files
- [ ] It should avoid reverting to default name after initial entry

### INT-004: Secret Choice Continuity

- [ ] It should reference Scene 1 secret choice in Scene 2 narrative
- [ ] It should show consequences of Scene 1 secret in Scene 2 events
- [ ] It should maintain narrative continuity through callbacks and hints
- [ ] It should enhance immersion with meaningful choice persistence

## Retired / Duplicate Items

- "Sound of typing should accompany the typewriter effect" → Covered by CB-004
- "Omega is the BBG … only first scene" → Narrative note; not testable, contextual to SC-002
- "Dreamweavers comment… only 3 exist" → Covered by DW-001/DW-002
