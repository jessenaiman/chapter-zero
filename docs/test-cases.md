# Omega Spiral: Chapter Zero · Test Backlog

_Purpose: mirror every currently defined test case (except the Save/Load suite) while attaching concrete implementation details and repository references. Use this document to raise GitLab issues or track manual verification work._

## Content Block Presentation

### Issue: CB-001 · Content Block Wait State

- **Labels**: `area::narrative-ui`, `type::functional-test`
- **Description**: Keep the opening terminal content visible until an explicit advance input occurs.
- **Implementation Notes**: `NarrativeTerminal` maintains the wait state through the `awaitingInput` flag and `currentBlockIndex` guard (`Source/Scripts/field/narrative/NarrativeTerminal.cs:365-427`). The deterministic harness in `Tests/Narrative/ContentBlockTests.cs:20-76` already simulates elapsed time; extend it to assert that no call path resets `awaitingInput` without routed input.
- **Acceptance Criteria**:
  - [ ] It should remain visible when waiting without user input for 10 seconds
  - [ ] It should not auto-advance when no input is received
  - [ ] It should wait indefinitely until player interaction

### Issue: CB-002 · Input Method Support

- **Labels**: `area::narrative-ui`, `type::functional-test`
- **Description**: Support keyboard, mouse, and gamepad for advancing content and submitting answers.
- **Implementation Notes**: Introduce a `continue_dialogue` action in `project.godot`’s `[input]` section bound to `Key.Space`, `Key.Enter`, `MouseButton.Left`, and `JoypadButton.A`, and route all handling through `Input.IsActionJustPressed` inside `UIDialogue._Input` (`Source/Scripts/field/ui/UIDialogue.cs:266-288`). Choice selection must use the same action path by calling `NarrativeTerminal.HandleThreadSelection`/`HandleStoryChoice` (`Source/Scripts/field/narrative/NarrativeTerminal.cs:407-507`). Automate coverage via `Tests/Narrative/ContentBlockTests.cs:298-342` with keyboard, mouse, and gamepad `InputEvent` doubles.
- **Acceptance Criteria**:
  - [ ] It should advance content block when keyboard select is pressed
  - [ ] It should advance content block when gamepad confirm is pressed
  - [ ] It should advance content block when mouse click is detected
  - [ ] It should respond to all configured input methods consistently

### Issue: CB-003 · CRT Visual Presentation

- **Labels**: `area::narrative-effects`, `type::visual-test`
- **Description**: Display the opening CRT overlay with correct aspect ratio and shader treatment.
- **Implementation Notes**: Resize `TerminalPanel` plus `OutputLabel` in `Source/Scenes/Scene1Narrative.tscn:28-45` to a 640×480 (4:3) viewport and keep `ExtResource("4_crt")` (`Source/Shaders/crt_shader.gdshader`) assigned to the `ShaderMaterial`. Tests should load the scene via Godot headless to confirm `RectSize` ratio and that the shader uniforms (`scanline_intensity`, `blur_strength`) are non-zero.
- **Acceptance Criteria**:
  - [ ] It should center text within a 4:3 aspect ratio frame
  - [ ] It should apply CRT blur shader effect to displayed text
  - [ ] It should display visible scanline effects on content
  - [ ] It should maintain visual consistency with reference overlay

### Issue: CB-004 · Typewriter Animation

- **Labels**: `area::narrative-effects`, `type::functional-test`
- **Description**: Ensure character-by-character reveals stay in sync with SFX and timing configuration.
- **Implementation Notes**: `UIDialogue` stores `TypewriterSpeed` and runs a `Timer` that fires `OnTypewriterTimeout` (`Source/Scripts/field/ui/UIDialogue.cs:73-744`). Connect an `AudioStreamPlayer` child that plays the configured keystroke cue inside that timeout handler. Extend `Tests/Narrative/ContentBlockTests.cs:150-239` to cover timing jitter, continuous looping, and captured audio events.
- **Acceptance Criteria**:
  - [ ] It should reveal characters sequentially at consistent intervals
  - [ ] It should play keystroke sound effects synchronized with character appearance
  - [ ] It should loop typewriter sound until line completion
  - [ ] It should maintain consistent timing throughout animation

### Issue: CB-005 · Section Transition Effects

- **Labels**: `area::narrative-effects`, `type::functional-test`
- **Description**: Fade between json-defined sections with a dissolve shader and accurate timing.
- **Implementation Notes**: Implement a dissolve coroutine tied to Dialogic timeline callbacks in `Scene1Narrative` (`Source/Scripts/field/narrative/Scene1Narrative.cs:150-360`) by animating a shader parameter (e.g., `dissolve_amount`) over 1.5 seconds. Add configuration constants to `Scene1Narrative` so tests can assert durations through exported properties, and convert the placeholders in `Tests/Narrative/ContentBlockTests.cs:243-291` into proper assertions.
- **Acceptance Criteria**:
  - [ ] It should trigger dissolve effect at json-defined section boundaries
  - [ ] It should fade text using dissolve shader during transitions
  - [ ] It should match configured dissolve duration from settings
  - [ ] It should proceed to next content block after dissolve completes

### Issue: CB-006 · Choice Selection Input

- **Labels**: `area::narrative-ui`, `type::functional-test`
- **Description**: Provide multi-input choice selection flow across story blocks and prompts.
- **Implementation Notes**: Expose focusable UI buttons for `ChoiceOption` entries within `NarrativeTerminal.DisplayStoryBlockAsync` (`Source/Scripts/field/narrative/NarrativeTerminal.cs:457-507`) and pair them with navigation actions (`ui_up`, `ui_down`, `ui_accept`). Mirror the same behaviour in `Scene1Narrative.HandlePersonaChoice` (`Source/Scripts/field/narrative/Scene1Narrative.cs:360-418`) so keyboard, mouse, and controller work uniformly. Functional coverage belongs in `Tests/Narrative/ContentBlockTests.cs:298-343`.
- **Acceptance Criteria**:
  - [ ] It should allow keyboard navigation to select dialogue choices
  - [ ] It should allow mouse click to select dialogue choices
  - [ ] It should allow gamepad input to select dialogue choices
  - [ ] It should advance narrative after any valid choice selection

## Dialogue Flow (Opening Scene)

### Issue: SC-001 · Script Loading Fallback

- **Labels**: `area::narrative-runtime`, `type::functional-test`
- **Description**: Load deterministic scene data when NobodyWho is disabled or fails.
- **Implementation Notes**: Align the loader with the JSON assets in `Source/Data/stages/ghost-terminal/{hero,shadow,ambition,omega}.json` using Godot's native JSON parsing. Update `Tests/Narrative/DialogueFlowTests.cs:18-60` to instantiate the loader directly and assert `ScriptSource.Fallback`.
- **Acceptance Criteria**:
  - [x] It should load valid script (content currently called a scene) object when NobodyWho plugin is disabled
  - [x] It should return scene header matching expected structure
  - [x] It should use fallback script when LLM is unavailable

### Issue: SC-002 · Omega Question Sequence

- **Labels**: `area::narrative-logic`, `type::functional-test`
- **Description**: Present the three mandatory Omega questions in the intended order.
- **Implementation Notes**: Normalise the opening question list when parsing json so `InitialChoice`, `NamePrompt`, and `SecretQuestion` map to fixed indices (`NarrativeTerminal.NormalizeNarrativeData`, `Source/Scripts/field/narrative/NarrativeTerminal.cs:190-226`). Mirror it in `Scene1Narrative` when the Dialogic timeline emits `dialogic_signal`. Tests belong in `Tests/Narrative/DialogueFlowTests.cs:64-111`.
- **Acceptance Criteria**:
  - [ ] It should present three mandatory questions in fixed order
  - [ ] It should show OneStory prompt first
  - [ ] It should show PlayerName prompt second
  - [ ] It should show Secret prompt third

### Issue: SC-003 · Dreamweaver Response Mapping

- **Labels**: `area::narrative-logic`, `type::functional-test`
- **Description**: Guarantee each prompt exposes three Dreamweaver-aligned responses.
- **Implementation Notes**: Keep the json `options` arrays intact when loading persona files and map them to `DreamweaverChoice.Thread` in `NormalizeNarrativeData` (`Source/Scripts/field/narrative/NarrativeTerminal.cs:198-216`). Tests already exercise this at `Tests/Narrative/DialogueFlowTests.cs:115-166`; extend them to validate label text.
- **Acceptance Criteria**:
  - [x] It should provide exactly three responses for each prompt
  - [x] It should map one response to HERO affinity
  - [x] It should map one response to SHADOW affinity
  - [x] It should map one response to AMBITION affinity

### Issue: SC-004 · One Story Interactive Blocks

- **Labels**: `area::narrative-content`, `type::functional-test`
- **Description**: Serve three unique story fragments, each requiring input before progressing.
- **Implementation Notes**: Populate `sceneData.StoryBlocks` from the persona json (`Source/Data/scenes/scene1_narrative/*.json`) and ensure `NarrativeTerminal.PresentStoryBlock` increments `currentBlockIndex` only after user confirmation (`Source/Scripts/field/narrative/NarrativeTerminal.cs:425-516`). Use fixtures in `Tests/Narrative/DialogueFlowTests.cs:170-220` to confirm no duplicates surface during one playthrough.
- **Acceptance Criteria**:
  - [ ] It should offer three distinct story blocks when selected
  - [ ] It should require player interaction for each story block
  - [ ] It should maintain story pool with more than 3 unique entries
  - [ ] It should prevent duplicate story blocks in single playthrough

### Issue: SC-005 · One Story Section Structure

- **Labels**: `area::narrative-content`, `type::functional-test`
- **Description**: Validate the internal sequencing and progression tracking in the One Story branch.
- **Implementation Notes**: Track section state within `Scene1Narrative` by storing the current `StoryBlock` id and updating it through Dialogic callbacks (`Source/Scripts/field/narrative/Scene1Narrative.cs:360-418`). Persist the same section index into `GameState.SceneData` so tests like `Tests/Narrative/DialogueFlowTests.cs:224-270` can recover the progress.
- **Acceptance Criteria**:
  - [ ] It should present three cryptic story sections sequentially
  - [ ] It should require user interaction before advancing each section
  - [ ] It should draw from validated story pool from NobodyWho gameplay, each with a dreamweaver personality
  - [ ] It should track section progression accurately

### Issue: SC-006 · Name Collection Flow

- **Labels**: `area::narrative-forms`, `type::functional-test`
- **Description**: Solicit, validate, and persist the player name while delivering two follow-up prompts.
- **Implementation Notes**: Reuse `NameValidationHarness` (`Source/Scripts/field/narrative/NameValidationHarness.cs`) inside `NarrativeTerminal.HandlePlayerName` (`Source/Scripts/field/narrative/NarrativeTerminal.cs:518-533`) and stash the accepted name via `SceneManager.SetPlayerName`. After validation, queue the additional questions through `Scene1Narrative.ShowNamePrompt` (`Source/Scripts/field/narrative/Scene1Narrative.cs:420-460`). Tests should live in `Tests/Narrative/DialogueFlowTests.cs:274-324`.
- **Acceptance Criteria**:
  - [ ] It should prompt user to enter player name
  - [ ] It should present two cryptic meaning questions after name entry
  - [ ] It should validate and store player name correctly
  - [ ] It should transition to content block after question sequence

### Issue: SC-007 · Secret Choice Mechanics

- **Labels**: `area::narrative-logic`, `type::functional-test`
- **Description**: Capture the secret response, map Dreamweaver points, and trigger the equation overlay.
- **Implementation Notes**: Implement the ghostwriting overlay as a `CanvasLayer` prefab referenced by `Scene1Narrative` and activated from `NarrativeTerminal.HandleSecret` (`Source/Scripts/field/narrative/NarrativeTerminal.cs:535-556`). Apply affinity deltas through `GameState.UpdateDreamweaverScore` (`Source/Scripts/common/GameState.cs:104-153`) and emit a log entry for the “game is real” finale. Expand `Tests/Narrative/DialogueFlowTests.cs:328-476` to validate scoring and freeze timing.
- **Acceptance Criteria**:
  - [ ] It should present three choices mapping to Dreamweaver points
  - [ ] It should correctly attribute choice to corresponding affinity
  - [ ] It should trigger equation ghostwriting effect when choice selected
  - [ ] It should display final cryptic message about the real game

## Dreamweaver Scoring & Interaction

### Issue: DW-001 · Dreamweaver Dialogue

- **Labels**: `area::dreamweaver`, `type::functional-test`
- **Description**: Deliver interstitial Dreamweaver conversations independent of Omega.
- **Implementation Notes**: Add a `DreamweaverDialogueController` node or extend `DreamweaverSystem` to emit dialogue events when `GameState.DreamweaverScores` change (`Source/Scripts/common/GameState.cs:104-153`). Persist speaker IDs into a backlog for verification. Build tests in `Tests/Narrative/DialogueFlowTests.cs` or a new suite to assert log entries.
- **Acceptance Criteria**:
  - [ ] It should display interstitial dialogue between Dreamweavers when player is idle
  - [ ] It should show Dreamweavers referencing each other in conversation
  - [ ] It should keep Omega silent during Dreamweaver conversations
  - [ ] It should log correct speaker IDs for each dialogue line

### Issue: DW-002 · Affinity Score Updates

- **Labels**: `area::dreamweaver`, `type::functional-test`
- **Description**: Keep Dreamweaver affinity scoring accurate and auditable.
- **Implementation Notes**: Continue using `GameState.UpdateDreamweaverScore` (`Source/Scripts/common/GameState.cs:104-133`) and add an affinity history collection on `GameState` for audit. Ensure narration paths call this helper inside `NarrativeTerminal.HandleSecret` and dungeon interactions. Tests exist in part under `Tests/Dungeon/NethackSceneTests.cs`; add coverage for increment, no-change, and history logging.
- **Acceptance Criteria**:
  - [ ] It should increment selected Dreamweaver affinity by configured value when choice made
  - [ ] It should leave non-selected Dreamweaver affinities unchanged
  - [ ] It should maintain affinity history array for audit trail
  - [ ] It should apply correct point values based on choice weight

### Issue: DW-003 · Cross-Scene Persistence

- **Labels**: `area::dreamweaver`, `type::functional-test`
- **Description**: Persist affinity data between scenes without Save/Load mechanics.
- **Implementation Notes**: At the end of Scene 1, call `SceneManager.UpdateCurrentScene` and verify `GameState.DreamweaverScores` remain unchanged when `SceneManager.TransitionToScene` triggers (`Source/Scripts/SceneManager.cs:34-83`). Unit tests can stub the scene manager to cover cross-scene transitions.
- **Acceptance Criteria**:
  - [ ] It should maintain affinity array values when transitioning to Scene 2
  - [ ] It should preserve cumulative scores from previous scenes
  - [ ] It should not reset affinity state between scene transitions
  - [ ] It should integrate with save system for state persistence

### Issue: DW-004 · Reactive Commentary

- **Labels**: `area::dreamweaver`, `type::functional-test`
- **Description**: Produce responsive Dreamweaver commentary after each player decision.
- **Implementation Notes**: Hook commentary triggers to `DreamweaverSystem.GenerateNarrativeAsync` results and `GameState.UpdateDreamweaverScore`. Maintain thresholds for reaction tiers in a json config (e.g., `res://Source/Data/narrative/dreamweaver_commentary.json`) and assert output through integration tests.
- **Acceptance Criteria**:
  - [ ] It should trigger non-selected Dreamweaver commentary after player choices
  - [ ] It should reference affinity shifts in Dreamweaver dialogue
  - [ ] It should prevent Omega from acknowledging Dreamweaver presence
  - [ ] It should vary commentary based on current affinity standings

### Issue: DW-005 · Scene-Level Score Tracking

- **Labels**: `area::dreamweaver`, `type::functional-test`
- **Description**: Track per-scene and cumulative Dreamweaver points without drift.
- **Implementation Notes**: Extend `GameState` with a per-scene tally (e.g., `Dictionary<int, Dictionary<DreamweaverType,int>>`) and expose helpers for aggregation. Unit tests should emulate multi-scene runs to ensure tallies persist and sums match expectations.
- **Acceptance Criteria**:
  - [ ] It should accumulate points correctly across multiple scenes
  - [ ] It should apply appropriate point values (1 or 2) per choice
  - [ ] It should persist affinity array between scene transitions
  - [ ] It should maintain accurate tallies throughout playthrough

### Issue: DW-006 · Contextual Commentary

- **Labels**: `area::dreamweaver`, `type::narrative-test`
- **Description**: Deliver Dreamweaver remarks at major story beats, weighted by affinity.
- **Implementation Notes**: Map scene events to commentary triggers in `DreamweaverSystem` by subscribing to cues from `SceneManager` and `NarrativeTerminal`. Use a rule table to pick lines from the persona json files and log emission timestamps for verification.
- **Acceptance Criteria**:
  - [ ] It should trigger Dreamweaver commentary at appropriate story beats
  - [ ] It should show comments from dominant or affected Dreamweaver after choice
  - [ ] It should time commentary naturally without interrupting flow
  - [ ] It should use event triggers tied to scene progression

### Issue: DW-007 · Threshold-Based Content

- **Labels**: `area::dreamweaver`, `type::narrative-test`
- **Description**: Unlock exclusive content when affinity thresholds are met.
- **Implementation Notes**: Define explicit threshold values per Dreamweaver in a config file and gate lines/quests accordingly. Expose the thresholds through `GameState` for tests to assert unlocking/hiding logic.
- **Acceptance Criteria**:
  - [ ] It should unlock unique dialogue when affinity threshold is reached
  - [ ] It should provide special content only to players with high affinity
  - [ ] It should hide threshold content from players with low affinity
  - [ ] It should track and document affinity threshold values

### Issue: DW-008 · Playthrough Variation

- **Labels**: `area::dreamweaver`, `type::narrative-test`
- **Description**: Ensure each Dreamweaver path produces distinct narrative outcomes.
- **Implementation Notes**: Seed a deterministic fixture that plays through HERO, SHADOW, and AMBITION choices and confirm `GameState.SelectedDreamweaver` drives varied content in Scene 2. Capture output transcripts to compare.
- **Acceptance Criteria**:
  - [ ] It should produce different dominant Dreamweaver in HERO playthrough
  - [ ] It should produce different dominant Dreamweaver in SHADOW playthrough
  - [ ] It should produce different dominant Dreamweaver in AMBITION playthrough
  - [ ] It should reflect choice consequences in narrative branches

### Issue: DW-009 · Affinity Tie-Breaking

- **Labels**: `area::dreamweaver`, `type::functional-test`
- **Description**: Resolve equal affinity scores deterministically.
- **Implementation Notes**: Implement tie-breaking logic in `GameState.GetHighestScoringDreamweaver` (`Source/Scripts/common/GameState.cs:121-153`) using a configured priority list and document it. Tests should cover multiple tie scenarios.
- **Acceptance Criteria**:
  - [ ] It should apply consistent tie-breaker when affinity values are equal
  - [ ] It should use configured priority for tie resolution
  - [ ] It should prevent undefined state when multiple Dreamweavers tied
  - [ ] It should document tie-breaker logic in design documentation

### Issue: DW-010 · Identity Consistency

- **Labels**: `area::dreamweaver`, `type::visual-test`
- **Description**: Maintain each Dreamweaver’s branding across UI, audio, and VFX.
- **Implementation Notes**: Store identity data (palette, glyphs, leitmotifs) in a central resource and ensure it’s applied in HUDs and dialogue overlays. Visual regression tests should confirm color palettes and audio cues remain consistent.
- **Acceptance Criteria**:
  - [ ] It should maintain consistent visual identity for each Dreamweaver across scenes
  - [ ] It should maintain consistent color scheme for each Dreamweaver
  - [ ] It should maintain consistent audio theme for each Dreamweaver
  - [ ] It should match Dreamweaver presentation to style guide specifications

## LLM / NobodyWho Integration

### Issue: NL-001 · Live LLM Generation

- **Labels**: `area::nobodywho`, `type::integration-test`
- **Description**: Generate narrative in a single pass when NobodyWho is active.
- **Implementation Notes**: `DreamweaverPersona.GenerateNarrativeAsync` (`Source/Scripts/field/narrative/DreamweaverPersona.cs:320-422`) should batch prompts and cache transcripts per scene. Add unit tests that stub the NobodyWho model to assert single-pass generation and cache reuse.
- **Acceptance Criteria**:
  - [ ] It should compile scene in single pass when LLM is available
  - [ ] It should cache generated script output after initial request
  - [ ] It should avoid per-line LLM calls during scene execution
  - [ ] It should request fresh generation before scene start

### Issue: NL-002 · Fallback Dataset Usage

- **Labels**: `area::nobodywho`, `type::integration-test`
- **Description**: Revert to recorded datasets when the LLM is offline.
- **Implementation Notes**: Store fallback transcripts under `res://Source/Data/narrative/fallback_datasets/` and have `NarrativeTerminal.TryLoadSceneData` detect offline state and load them. Tests should disable the NobodyWho client and verify metadata flags indicate fallback mode.
- **Acceptance Criteria**:
  - [ ] It should load stored dataset when LLM is unavailable
  - [ ] It should note fallback mode in scene metadata
  - [ ] It should use dataset from recorded real gameplay
  - [ ] It should maintain scene functionality without LLM connection

### Issue: NL-003 · Transcript Replay Mode

- **Labels**: `area::nobodywho`, `type::integration-test`
- **Description**: Replay a stored NobodyWho transcript on demand.
- **Implementation Notes**: Add a replay flag and file picker to `NarrativeCompiler` so it loads transcripts from `res://Records/NobodyWho/` without contacting the model. Tests should validate logging and permission handling.
- **Acceptance Criteria**:
  - [ ] It should replay scene using saved NobodyWho transcript
  - [ ] It should avoid live LLM calls when replay flag is set
  - [ ] It should log cached transcript usage for debugging
  - [ ] It should validate storage path permissions before loading

## Dungeon Stage Sequence (Scene 2)

### Issue: STG-001 · Dungeon Schema Loading

- **Labels**: `area::dungeon`, `type::functional-test`
- **Description**: Load Scene 2 dungeon stages from the json schema and validate layout integrity.
- **Implementation Notes**: Point `AsciiDungeonSequenceLoader` at `Source/Data/stages/nethack/dungeon_sequence.json` using Godot's native JSON parsing. Convert results into `DungeonStageDefinition` before passing to `AsciiDungeonSequence.Create`. Tests live in `Tests/Dungeon/NethackSceneTests.cs:27-174`.
- **Acceptance Criteria**:
  - [ ] It should load three distinct DungeonStage instances from valid schema
  - [ ] It should assign unique owner values matching schema definition
  - [ ] It should validate map dimensions against schema specifications
  - [ ] It should correctly align object glyphs with legend entries

### Issue: STG-002 · Owner Duplication Prevention

- **Labels**: `area::dungeon`, `type::functional-test`
- **Description**: Block duplicate Dreamweaver owners in the sequence definition.
- **Implementation Notes**: `AsciiDungeonSequence.Create` already throws when duplicates appear (`Source/Scripts/domain/dungeon/AsciiDungeonSequence.cs:19-48`); extend tests in `Tests/Dungeon/NethackSceneTests.cs` to cover validation via json fixtures.
- **Acceptance Criteria**:
  - [ ] It should throw domain exception when duplicate owners detected
  - [ ] It should prevent aggregate creation with invalid owner sequence
  - [ ] It should avoid persisting stages with duplicate owners
  - [ ] It should maintain balanced affinity distribution across Dreamweavers

### Issue: STG-003 · Stage Entry Event Publishing

- **Labels**: `area::dungeon`, `type::integration-test`
- **Description**: Publish dungeon stage events with full metadata every time a new stage is entered.
- **Implementation Notes**: `AsciiDungeonSequenceRunner.PublishStageEntered` (`Source/Scripts/field/narrative/AsciiDungeonSequenceRunner.cs:74-111`) sends `DungeonStageEnteredEvent`; ensure the event publisher writes owner, index, and ASCII map to the bus, and replace the TODO assertions in `Tests/Dungeon/NethackSceneTests.cs:176-214`.
- **Acceptance Criteria**:
  - [ ] It should publish DungeonStageEntered event when stage begins
  - [ ] It should include owner identifier in event payload
  - [ ] It should include stage index in event payload
  - [ ] It should include ASCII map metadata for rendering in event payload

### Issue: STG-004 · Object Interaction Affinity

- **Labels**: `area::dungeon`, `type::functional-test`
- **Description**: Adjust Dreamweaver affinity when interacting with objects inside ASCII dungeons.
- **Implementation Notes**: Use `DungeonStage.ResolveInteraction` to map glyphs to `DreamweaverAffinityChange` and call `GameState.UpdateDreamweaverScore`. Existing tests in `Tests/Dungeon/NethackSceneTests.cs:216-268` should confirm the deltas and neutrality cases.
- **Acceptance Criteria**:
  - [ ] It should increment affinity when interacting with owner-aligned object
  - [ ] It should apply configured point value to correct Dreamweaver
  - [ ] It should record interaction in affinity history
  - [ ] It should leave affinity unchanged for non-aligned object interactions

### Issue: STG-005 · Deterministic Stage Progression

- **Labels**: `area::dungeon`, `type::functional-test`
- **Description**: Ensure stage progression follows a deterministic order given the same seed.
- **Implementation Notes**: Add an explicit seed field to the dungeon loader and pass it into any procedural generation step. Tests should run sequences twice to prove identical ordering and layout.
- **Acceptance Criteria**:
  - [ ] It should advance to next stage when current stage is completed
  - [ ] It should maintain consistent sequence order with same seed
  - [ ] It should generate identical layouts across runs with same seed
  - [ ] It should reach third stage without skipping when progressing

## Error Handling & Edge Cases

### Issue: ERR-001 · Name Input Validation

- **Labels**: `area::narrative-forms`, `type::functional-test`
- **Description**: Reject invalid player names and show friendly errors.
- **Implementation Notes**: `NameValidationHarness` (`Source/Scripts/field/narrative/NameValidationHarness.cs`) already covers the validation rules and is exercised by `Tests/Narrative/ErrorHandlingTests.cs:18-78`. Integrate the harness into UI submission paths so behaviour matches the tests.
- **Acceptance Criteria**:
  - [x] It should reject empty string when player name is submitted
  - [x] It should reject special characters outside allowed set
  - [x] It should reject names exceeding maximum length
  - [x] It should display clear error message when validation fails

### Issue: ERR-002 · Input Spam Protection

- **Labels**: `area::narrative-ui`, `type::stability-test`
- **Description**: Prevent rapid input from breaking dialogue state.
- **Implementation Notes**: Add input debouncing around `UIDialogue.OnContinueInput` and `NarrativeTerminal.Handle*` methods and update the TODO placeholders in `Tests/Narrative/ErrorHandlingTests.cs:82-118` with real stress tests that send 50+ inputs.
- **Acceptance Criteria**:
  - [x] It should complete content normally when input buttons are mashed rapidly
  - [x] It should maintain consistent state machine after 50+ rapid inputs
  - [x] It should prevent content skipping during typewriter effect
  - [x] It should avoid crashes when processing input spam

### Issue: ERR-003 · Window Focus Handling

- **Labels**: `area::narrative-ui`, `type::stability-test`
- **Description**: Handle focus loss during typewriter playback gracefully.
- **Implementation Notes**: Override `_Notification` in `UIDialogue` and `Scene1Narrative` to pause timers and audio when receiving `MainLoop.NotificationWmFocusOut`, resuming on focus in. Add tests using Godot’s headless runner to emit focus notifications.
- **Acceptance Criteria**:
  - [ ] It should pause typewriter effect when window loses focus
  - [ ] It should resume typewriter correctly when window regains focus
  - [ ] It should sync audio playback after alt-tab return
  - [ ] It should handle pause menu during typewriter animation

### Issue: ERR-004 · Transition Recovery

- **Labels**: `area::infrastructure`, `type::stability-test`
- **Description**: Recover from mid-transition failures without corrupting state.
- **Implementation Notes**: Guard `SceneManager.TransitionToScene` with try/catch and store recovery checkpoints in `GameState.SceneData`. Implement automated recovery tests that simulate crashes by throwing inside transition callbacks.
- **Acceptance Criteria**:
  - [ ] It should recover gracefully when game crashes during scene transition
  - [ ] It should load last stable save after force quit during transition
  - [ ] It should offer scene selection if recovery fails
  - [ ] It should prevent corrupted save state after interrupted transition

## Accessibility & UX

### Issue: ACC-001 · Text Speed Configuration

- **Labels**: `area::narrative-ui`, `type::accessibility-test`
- **Description**: Allow players to configure and persist typewriter speed.
- **Implementation Notes**: Expose a settings menu hook to `UIDialogue.SetTypewriterSpeed` (`Source/Scripts/field/ui/UIDialogue.cs:464-473`) and persist the value in a user config file. Write tests that adjust speed mid-play and verify timing changes.
- **Acceptance Criteria**:
  - [ ] It should adjust typewriter speed when setting is changed
  - [ ] It should reflect speed changes in real-time during gameplay
  - [ ] It should support range from instant reveal to very slow
  - [ ] It should provide at least 5 preset speed options

### Issue: ACC-002 · Font Size Options

- **Labels**: `area::narrative-ui`, `type::accessibility-test`
- **Description**: Provide adjustable font sizes without breaking the CRT layout.
- **Implementation Notes**: Use theme overrides on `OutputLabel` (`Source/Scenes/Scene1Narrative.tscn:34-45`) to set small/medium/large presets and adjust bounding boxes accordingly. Tests should render each size and assert no clipping.
- **Acceptance Criteria**:
  - [ ] It should display text at configured size (Small/Medium/Large)
  - [ ] It should fit all text within CRT boundary at any size
  - [ ] It should prevent overflow or clipping with longest dialogue lines
  - [ ] It should scale CRT effect appropriately with font size changes

### Issue: ACC-003 · Seen Dialogue Skipping

- **Labels**: `area::narrative-ui`, `type::ux-test`
- **Description**: Allow skipping previously viewed dialogue safely.
- **Implementation Notes**: Track seen dialogue IDs in `GameState.NarratorQueue` and offer a skip action that fast-forwards to unseen content while halting at branching choices. Add tests to confirm logs of skipped content.
- **Acceptance Criteria**:
  - [ ] It should advance to next unseen content when skip button pressed
  - [ ] It should stop skipping at next player choice or unseen dialogue
  - [ ] It should log skipped content for tracking
  - [ ] It should display visual indicator distinguishing seen vs unseen content

### Issue: ACC-004 · Dialogue History Access

- **Labels**: `area::narrative-ui`, `type::ux-test`
- **Description**: Provide a backlog UI with scrollable history.
- **Implementation Notes**: Implement a backlog window that records at least 20 recent entries from `NarrativeTerminal.DisplayImmediate`. Integration tests should ensure closing the backlog returns focus to the current dialogue.
- **Acceptance Criteria**:
  - [ ] It should display at least 20 previous dialogue blocks in backlog
  - [ ] It should allow scrolling through dialogue history
  - [ ] It should return to current position when backlog is closed
  - [ ] It should persist history for current game session

### Issue: ACC-005 · Colorblind Accessibility

- **Labels**: `area::ux`, `type::accessibility-test`
- **Description**: Make Dreamweaver cues colorblind-friendly.
- **Implementation Notes**: Define alternate palettes and iconography for Protanopia, Deuteranopia, and Tritanopia modes and ensure the HUD applies them. Tests should verify palette switches and icon usage.
- **Acceptance Criteria**:
  - [ ] It should use patterns and icons in addition to colors for Dreamweaver UI
  - [ ] It should support Protanopia colorblind mode
  - [ ] It should support Deuteranopia colorblind mode
  - [ ] It should support Tritanopia colorblind mode

### Issue: ACC-006 · Audio Volume Control

- **Labels**: `area::audio`, `type::ux-test`
- **Description**: Provide independent volume sliders for SFX, typing, and music.
- **Implementation Notes**: Expose mixer buses for SFX, typing, and music in the Godot audio bus layout and connect settings sliders to those buses. Tests should adjust each slider and confirm the corresponding bus volume changes.
- **Acceptance Criteria**:
  - [ ] It should adjust SFX volume independently from other audio
  - [ ] It should adjust typing sounds volume independently
  - [ ] It should adjust music volume independently
  - [ ] It should provide mute option for each audio layer

## Dungeon Gameplay (Scene 2 Expansion)

### Issue: DNG-001 · Movement Input Methods

- **Labels**: `area::dungeon`, `type::functional-test`
- **Description**: Support multiple control schemes for tile movement.
- **Implementation Notes**: Refactor `TileDungeonController._Input` (`Source/Scripts/field/TileDungeonController.cs:96-133`) to use `InputMap` actions (`move_up`, `move_down`, etc.) with keyboard, controller, and optional mouse bindings. Tests should simulate each input type and confirm player coordinates.
- **Acceptance Criteria**:
  - [ ] It should move player correctly with WASD keys
  - [ ] It should move player correctly with arrow keys
  - [ ] It should move player correctly with gamepad D-pad or stick
  - [ ] It should move player correctly with mouse clicks if supported

### Issue: DNG-002 · Collision System

- **Labels**: `area::dungeon`, `type::functional-test`
- **Description**: Prevent invalid movement through walls or solid objects.
- **Implementation Notes**: Retain the `IsWalkable` check in `TileDungeonController` but extend it to include static body collisions from the TileMap. Tests should cover walking into walls, objects, and valid paths.
- **Acceptance Criteria**:
  - [ ] It should block player movement when attempting to walk through walls
  - [ ] It should block player movement when attempting to walk through solid objects
  - [ ] It should allow grid-based precise movement along valid paths
  - [ ] It should prevent clipping through collision boundaries

### Issue: DNG-003 · Invalid Move Feedback

- **Labels**: `area::dungeon`, `type::ux-test`
- **Description**: Provide audio/visual feedback when movement is blocked.
- **Implementation Notes**: Trigger a bump sound and brief tween on invalid moves inside `TileDungeonController.TryMovePlayer`. Tests can assert that feedback nodes fire when movement fails.
- **Acceptance Criteria**:
  - [ ] It should play bump sound when invalid move is attempted into wall
  - [ ] It should trigger screen shake or bounce animation on collision
  - [ ] It should maintain game state unchanged after invalid move
  - [ ] It should provide clear visual feedback for boundary detection

### Issue: DNG-004 · Fog of War System

- **Labels**: `area::dungeon`, `type::functional-test`
- **Description**: Hide unexplored tiles until the player discovers them.
- **Implementation Notes**: Maintain a visibility map per stage and render it via a shader or overlay. Persist revealed tiles to `GameState.SceneData` so they survive scene reloads. Tests should verify reveal persistence.
- **Acceptance Criteria**:
  - [ ] It should keep explored areas visible after player leaves them
  - [ ] It should hide unexplored areas until player approaches
  - [ ] It should persist revealed state across save and reload
  - [ ] It should maintain consistent vision radius for exploration

### Issue: DNG-005 · Object Interaction Indicators

- **Labels**: `area::dungeon`, `type::ux-test`
- **Description**: Signal interactive objects when the player is nearby.
- **Implementation Notes**: Add highlight sprites or UI prompts adjacent to interactive glyphs and remove them when the player departs. Tests should check indicator visibility toggles.
- **Acceptance Criteria**:
  - [ ] It should highlight objects when player is adjacent to them
  - [ ] It should show tooltip or icon indicating object is interactive
  - [ ] It should remove highlight when player moves away from object
  - [ ] It should use consistent visual effect for all interactive objects

### Issue: DNG-006 · Objective Communication

- **Labels**: `area::dungeon`, `type::functional-test`
- **Description**: Communicate stage objectives clearly during dungeon play.
- **Implementation Notes**: Populate HUD labels with objectives pulled from the stage definition and update them as progress occurs. Integration tests should verify the HUD reflects completion state.
- **Acceptance Criteria**:
  - [ ] It should display stage objective on HUD at stage start
  - [ ] It should update objective progress as player advances
  - [ ] It should trigger completion fanfare when objective is met
  - [ ] It should provide clear message upon stage completion

### Issue: DNG-007 · Failure and Respawn

- **Labels**: `area::dungeon`, `type::stability-test`
- **Description**: Provide a consistent respawn flow when the player fails.
- **Implementation Notes**: Detect failure conditions, show a game over screen, and reload the stage while preserving affinity scores. Tests should cover respawn from checkpoints and track retry counts.
- **Acceptance Criteria**:
  - [ ] It should display game over screen when failure condition is triggered
  - [ ] It should respawn player at stage start or last checkpoint
  - [ ] It should preserve affinity state after respawn
  - [ ] It should track retry count for difficulty adjustment

### Issue: DNG-008 · Inventory Management

- **Labels**: `area::dungeon`, `type::functional-test`
- **Description**: Manage dungeon items within the shared inventory system.
- **Implementation Notes**: Reuse `Inventory` (`Source/Scripts/common/Inventory.cs`) and `PartyData.Inventory` to persist pickups, display details in UI (`Source/Scripts/field/ui/inventory`), and support using or dropping items. Tests should cover add/remove/use flows.
- **Acceptance Criteria**:
  - [ ] It should store picked up items in player inventory
  - [ ] It should display item details in inventory UI
  - [ ] It should apply item effects when used or equipped
  - [ ] It should allow player to drop items from inventory

## Audio/Visual Polish

### Issue: AV-001 · Music Transitions

- **Labels**: `area::audio`, `type::integration-test`
- **Description**: Smoothly transition music between narrative segments.
- **Implementation Notes**: Use `Music.CrossfadeTo` (`Source/Scripts/common/Music.cs:159-207`) when entering or exiting the terminal scenes. Tests should confirm fade durations and absence of clipping.
- **Acceptance Criteria**:
  - [ ] It should fade out music smoothly when transitioning between sections
  - [ ] It should fade in or crossfade new music smoothly at transition
  - [ ] It should avoid abrupt music stops during scene changes
  - [ ] It should prevent volume spikes during transitions

### Issue: AV-002 · Sound Effect Mixing

- **Labels**: `area::audio`, `type::functional-test`
- **Description**: Mix multiple sound effects without distortion.
- **Implementation Notes**: Limit simultaneous instances per SFX by pooling `AudioStreamPlayer`s and set priorities on the audio bus. Use automated tests to trigger overlapping sounds and assert no clipping occurs.
- **Acceptance Criteria**:
  - [ ] It should mix multiple SFX without distortion when triggered simultaneously
  - [ ] It should prevent audio clipping with priority system
  - [ ] It should limit simultaneous SFX instances per frame
  - [ ] It should maintain clear audio when typewriter and button SFX overlap

### Issue: AV-003 · Narrative Visual Effects

- **Labels**: `area::narrative-effects`, `type::visual-test`
- **Description**: Trigger cinematic effects during key narrative beats.
- **Implementation Notes**: Drive the CRT glitch and equation overlay from `NarrativeTerminal.HandleSecret` and animation resources to ensure consistent timings. Tests should confirm shader parameters change as expected.
- **Acceptance Criteria**:
  - [ ] It should trigger CRT glitch effect at equation ghostwrite moment
  - [ ] It should synchronize screen glitch/static with narrative beat
  - [ ] It should enhance immersion with appropriate effect intensity
  - [ ] It should use shader animation triggered by scene events

### Issue: AV-004 · Motion Sensitivity

- **Labels**: `area::ux`, `type::accessibility-test`
- **Description**: Provide configurable motion effects to accommodate sensitive players.
- **Implementation Notes**: Gate screen shake and motion effects behind settings toggles and apply them via tween nodes. Automated tests should toggle settings and verify effect suppression.
- **Acceptance Criteria**:
  - [ ] It should apply screen shake effects that are noticeable but not excessive
  - [ ] It should provide option to reduce motion effects in settings
  - [ ] It should provide option to disable motion effects completely
  - [ ] It should avoid causing nausea or motion sickness in sensitive players

### Issue: AV-005 · Dissolve Timing

- **Labels**: `area::narrative-effects`, `type::visual-test`
- **Description**: Keep transition dissolve timing consistent and engaging.
- **Implementation Notes**: Reuse the dissolve animation introduced for CB-005 and add timing assertions to ensure the effect lasts between one and two seconds. Tests should verify consistent durations.
- **Acceptance Criteria**:
  - [ ] It should complete dissolve effect within 1-2 seconds
  - [ ] It should feel smooth and natural during transition
  - [ ] It should maintain player engagement without impatience
  - [ ] It should allow time to appreciate visual effect

## Localization / Text

### Issue: LOC-001 · Text Wrapping

- **Labels**: `area::localization`, `type::functional-test`
- **Description**: Wrap text safely inside the CRT frame across languages.
- **Implementation Notes**: Configure `RichTextLabel.AutowrapMode` (already set in `Scene1Narrative.tscn`) and compute maximum characters per line based on current font metrics. Tests should load lengthy strings to ensure no overflow.
- **Acceptance Criteria**:
  - [ ] It should wrap long text at word boundaries within CRT frame
  - [ ] It should prevent overflow beyond 4:3 CRT boundary
  - [ ] It should remain readable at all configured font sizes
  - [ ] It should calculate appropriate max characters per line for each size

### Issue: LOC-002 · Special Character Rendering

- **Labels**: `area::localization`, `type::functional-test`
- **Description**: Render extended Unicode characters without corruption.
- **Implementation Notes**: Confirm the font (`Source/assets/gui/font/SourceCodePro-Bold.ttf`) supports extended glyphs and add fallback fonts if required. Tests should supply multilingual lines and equations.
- **Acceptance Criteria**:
  - [ ] It should render unicode characters correctly (é, ñ, 中)
  - [ ] It should render mathematical equations correctly
  - [ ] It should use font with extended unicode character support
  - [ ] It should avoid displaying boxes or question marks for unsupported characters

### Issue: LOC-003 · Container Overflow Prevention

- **Labels**: `area::localization`, `type::functional-test`
- **Description**: Prevent UI containers from clipping localized text.
- **Implementation Notes**: Add layout tests across UI scenes to detect overflow, expand panels dynamically, and apply ellipsis where sizing cannot change. Automated tests should load max-length strings.
- **Acceptance Criteria**:
  - [ ] It should fit all text within UI containers without clipping
  - [ ] It should auto-resize containers when possible for text
  - [ ] It should truncate text gracefully with ellipsis when needed
  - [ ] It should test all UI screens with maximum-length strings

### Issue: LOC-004 · Variable Length Handling

- **Labels**: `area::localization`, `type::functional-test`
- **Description**: Maintain pacing and skip options across varying line lengths.
- **Implementation Notes**: Ensure typewriter timing scales with `TypewriterSpeed` while `SkipTypewriter` remains responsive for long lines. Tests should cover short and long strings, confirming timing and instant completion work consistently.
- **Acceptance Criteria**:
  - [ ] It should maintain appropriate pacing for short text lines (5 words)
  - [ ] It should allow skipping of long text lines (50+ words)
  - [ ] It should maintain consistent per-character timing regardless of length
  - [ ] It should support instant-complete on input during typewriter

## Integration & Flow

### Issue: INT-001 · End-to-End Playthrough

- **Labels**: `area::integration`, `type::playtest`
- **Description**: Verify a full Scene 1 playthrough transitions cleanly into Scene 2.
- **Implementation Notes**: Drive an automated playthrough that exercises all major paths, capturing console logs and ensuring `SceneManager.TransitionToScene("Scene2NethackSequence")` completes without errors. Store the run as a regression fixture.
- **Acceptance Criteria**:
  - [ ] It should complete Scene 1 without errors, crashes, or warnings
  - [ ] It should transition smoothly from Scene 1 to Scene 2
  - [ ] It should carry forward game state correctly across scene boundary
  - [ ] It should maintain console error-free throughout full playthrough

### Issue: INT-002 · Cross-Scene Affinity Effects

- **Labels**: `area::integration`, `type::functional-test`
- **Description**: Reflect Scene 1 choices in Scene 2 content.
- **Implementation Notes**: Use the affinity data stored in `GameState.DreamweaverScores` to adjust dungeon layout, dialogue, or commentary in Scene 2. Automated tests should validate that HERO, SHADOW, and AMBITION paths produce distinct behaviour.
- **Acceptance Criteria**:
  - [ ] It should reflect HERO affinity choices from Scene 1 in Scene 2 content
  - [ ] It should reflect SHADOW affinity choices from Scene 1 in Scene 2 content
  - [ ] It should reflect AMBITION affinity choices from Scene 1 in Scene 2 content
  - [ ] It should adapt Scene 2 dialogue, stage order, or commentary based on affinity

### Issue: INT-003 · Name Persistence

- **Labels**: `area::integration`, `type::functional-test`
- **Description**: Ensure the captured player name persists across systems.
- **Implementation Notes**: Once the name is stored via `SceneManager.SetPlayerName`, display it in Scene 2 UI and dialogue, and serialize it with other state fields. Tests should assert the name flows through the entire playthrough loop.
- **Acceptance Criteria**:
  - [ ] It should display player name in Scene 2 dialogue after capture in Scene 1
  - [ ] It should display player name in UI elements across all scenes
  - [ ] It should store player name in save files
  - [ ] It should avoid reverting to default name after initial entry

### Issue: INT-004 · Secret Choice Continuity

- **Labels**: `area::integration`, `type::functional-test`
- **Description**: Carry forward the Scene 1 secret choice into later content.
- **Implementation Notes**: Record the secret choice in `GameState.SceneData` and surface callbacks in Scene 2 narrative, dungeon commentary, or Dreamweaver reactions. Tests should verify references appear and match the stored choice.
- **Acceptance Criteria**:
  - [ ] It should reference Scene 1 secret choice in Scene 2 narrative
  - [ ] It should show consequences of Scene 1 secret in Scene 2 events
  - [ ] It should maintain narrative continuity through callbacks and hints
  - [ ] It should enhance immersion with meaningful choice persistence

## Gameplay Simulation Tests

### Issue: SIM-001 · Full Opening Sequence Playthrough
- **Labels**: `area::integration`, `type::playtest`
- **Description**: Simulate a complete playthrough of the opening sequence with all player choices.
- **Implementation Notes**: Create an automated test that drives through the entire Scene1Narrative sequence including name input, persona selection, and all three story blocks. Use Godot's input simulation to trigger UI events and verify state progression. Tests should capture the full flow from initial loading to transition to Scene 2.
- **Acceptance Criteria**:
  - [ ] It should complete full opening sequence without errors or crashes
  - [ ] It should handle all player input scenarios (keyboard, mouse, gamepad)
  - [ ] It should validate all choice selections lead to correct narrative paths
  - [ ] It should verify final transition to Scene 2 occurs correctly
  - [ ] It should maintain consistent game state throughout entire sequence

### Issue: SIM-002 · Hero Path Gameplay Flow
- **Labels**: `area::integration`, `type::playtest`
- **Description**: Test the complete HERO persona gameplay experience from start to finish.
- **Implementation Notes**: Simulate a player choosing the HERO path, entering "Garrett" as name, and progressing through all HERO-aligned story blocks. Verify affinity scores are updated correctly and the final transition reflects HERO choice. Test should assert proper dialogue flow and Dreamweaver responses.
- **Acceptance Criteria**:
  - [ ] It should select HERO persona when player chooses first option
  - [ ] It should update HERO affinity score during story interactions
  - [ ] It should display HERO-appropriate dialogue and narrative content
  - [ ] It should transition to Scene 2 with HERO path active
  - [ ] It should carry forward HERO state to subsequent scenes

### Issue: SIM-003 · Shadow Path Gameplay Flow
- **Labels**: `area::integration`, `type::playtest`
- **Description**: Test the complete SHADOW persona gameplay experience from start to finish.
- **Implementation Notes**: Simulate a player choosing the SHADOW path, entering "Shadow" as name, and progressing through all SHADOW-aligned story blocks. Verify affinity scores are updated correctly and the final transition reflects SHADOW choice. Test should assert proper dialogue flow and Dreamweaver responses.
- **Acceptance Criteria**:
  - [ ] It should select SHADOW persona when player chooses second option
  - [ ] It should update SHADOW affinity score during story interactions
  - [ ] It should display SHADOW-appropriate dialogue and narrative content
  - [ ] It should transition to Scene 2 with SHADOW path active
  - [ ] It should carry forward SHADOW state to subsequent scenes

### Issue: SIM-004 · Ambition Path Gameplay Flow
- **Labels**: `area::integration`, `type::playtest`
- **Description**: Test the complete AMBITION persona gameplay experience from start to finish.
- **Implementation Notes**: Simulate a player choosing the AMBITION path, entering "Ambition" as name, and progressing through all AMBITION-aligned story blocks. Verify affinity scores are updated correctly and the final transition reflects AMBITION choice. Test should assert proper dialogue flow and Dreamweaver responses.
- **Acceptance Criteria**:
  - [ ] It should select AMBITION persona when player chooses third option
  - [ ] It should update AMBITION affinity score during story interactions
  - [ ] It should display AMBITION-appropriate dialogue and narrative content
  - [ ] It should transition to Scene 2 with AMBITION path active
  - [ ] It should carry forward AMBITION state to subsequent scenes

### Issue: SIM-005 · Rapid Input Stress Test
- **Labels**: `area::stability`, `type::stress-test`
- **Description**: Test system stability under rapid player input during narrative sequences.
- **Implementation Notes**: Simulate rapid keyboard/mouse input during typewriter text display and choice selection phases. Send 100+ rapid inputs and verify the system handles them gracefully without crashing or corrupting state. Test should cover both valid and invalid inputs.
- **Acceptance Criteria**:
  - [ ] It should handle 100+ rapid keyboard inputs without crashing
  - [ ] It should handle 100+ rapid mouse clicks without state corruption
  - [ ] It should maintain narrative flow despite input spamming
  - [ ] It should not skip or corrupt text during rapid input sequences
  - [ ] It should recover gracefully from input overflow situations

### Issue: SIM-006 · Multi-Session Gameplay Continuity
- **Labels**: `area::integration`, `type::functional-test`
- **Description**: Test gameplay continuity across multiple play sessions with save/load.
- **Implementation Notes**: Simulate a player starting a game, making choices, saving progress, quitting, then reloading and continuing. Verify that all choices, affinity scores, and narrative state are preserved correctly. Test should cover both automatic and manual saves.
- **Acceptance Criteria**:
  - [ ] It should preserve player name across save/load cycles
  - [ ] It should maintain persona selection after game restart
  - [ ] It should carry forward affinity scores between sessions
  - [ ] It should resume at correct narrative point after loading
  - [ ] It should handle save corruption gracefully with fallback options

### Issue: SIM-007 · Choice Consequence Verification
- **Labels**: `area::narrative-logic`, `type::functional-test`
- **Description**: Verify that player choices have meaningful consequences in subsequent gameplay.
- **Implementation Notes**: Test that each persona choice leads to different dialogue options, story branches, and Dreamweaver interactions in Scene 2. Simulate the same scenario with different initial choices and verify divergent outcomes.
- **Acceptance Criteria**:
  - [ ] It should show different dialogue trees based on initial persona choice
  - [ ] It should trigger different Dreamweaver commentary for each path
  - [ ] It should influence combat encounters based on affinity scores
  - [ ] It should affect NPC reactions in later scenes
  - [ ] It should create meaningful narrative divergence from early choices

### Issue: SIM-008 · Accessibility Gameplay Test
- **Labels**: `area::accessibility`, `type::ux-test`
- **Description**: Test gameplay with accessibility features enabled (text speed, font size, etc.).
- **Implementation Notes**: Simulate gameplay with various accessibility settings enabled (slow text speed, large fonts, colorblind modes). Verify that all gameplay elements remain accessible and functional with these settings.
- **Acceptance Criteria**:
  - [ ] It should complete gameplay with slowest text speed setting
  - [ ] It should display all UI elements correctly with largest font size
  - [ ] It should function properly with colorblind mode enabled
  - [ ] It should maintain gameplay integrity with accessibility options active
  - [ ] It should provide equivalent experience regardless of accessibility settings

## Comprehensive Integration Tests

### Issue: INT-005 · Full Game Loop Integration
- **Labels**: `area::integration`, `type::playtest`
- **Description**: Test the complete game loop from Scene 1 through all subsequent scenes.
- **Implementation Notes**: Create an automated test that drives through the entire game flow: Scene 1 (Ghost Terminal) → Scene 2 (Nethack Sequence) → Scene 3 (Never Go Alone) → Scene 4 (Tile Dungeon) → Scene 5 (Field Combat). Verify state persistence, affinity tracking, and smooth transitions between all scenes. Test should capture console output and ensure no errors occur throughout the full loop.
- **Acceptance Criteria**:
  - [ ] It should complete full game loop without crashes or errors
  - [ ] It should maintain affinity scores across all scene transitions
  - [ ] It should preserve player name and choices throughout gameplay
  - [ ] It should transition smoothly between all five main scenes
  - [ ] It should carry forward narrative state and consequences

### Issue: INT-006 · Cross-Platform Gameplay Consistency
- **Labels**: `area::integration`, `type::compatibility-test`
- **Description**: Ensure gameplay consistency across different platforms and hardware configurations.
- **Implementation Notes**: Test gameplay on different resolutions, aspect ratios, and hardware capabilities. Verify that narrative pacing, input responsiveness, and visual effects remain consistent. Tests should cover windowed/fullscreen modes and different input devices (keyboard, mouse, gamepad).
- **Acceptance Criteria**:
  - [ ] It should maintain consistent gameplay across 16:9, 16:10, and 4:3 aspect ratios
  - [ ] It should function correctly at resolutions from 1024x768 to 4K
  - [ ] It should handle input consistently across keyboard, mouse, and gamepad
  - [ ] It should maintain performance across different hardware configurations
  - [ ] It should preserve visual fidelity across platform variations

### Issue: INT-007 · Save/Load Game State Integrity
- **Labels**: `area::integration`, `type::functional-test`
- **Description**: Verify complete game state preservation during save and load operations.
- **Implementation Notes**: Test saving at various points throughout gameplay and loading back into the exact same state. Verify that all player data, affinity scores, inventory items, and narrative choices are preserved. Tests should cover both manual saves and auto-saves.
- **Acceptance Criteria**:
  - [ ] It should save complete game state including player name and choices
  - [ ] It should load saved game state without data loss or corruption
  - [ ] It should preserve affinity scores and narrative consequences
  - [ ] It should maintain inventory contents and character equipment
  - [ ] It should resume gameplay at exact point where save was triggered

### Issue: INT-008 · Multi-Player Session Isolation
- **Labels**: `area::integration`, `type::functional-test`
- **Description**: Ensure separate player sessions do not interfere with each other.
- **Implementation Notes**: Test multiple save slots and verify that starting a new game doesn't affect existing saves. Test concurrent access scenarios and ensure proper isolation between different player profiles. Verify that each session maintains its own state independently.
- **Acceptance Criteria**:
  - [ ] It should maintain separate state for each save slot
  - [ ] It should prevent cross-contamination between player sessions
  - [ ] It should allow multiple independent save files
  - [ ] It should isolate game state between concurrent sessions
  - [ ] It should handle session switching without data leakage
