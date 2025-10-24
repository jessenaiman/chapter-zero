# Post-Refactoring Test Failure Report
**Date:** October 24, 2025  
**Branch:** main  
**Status:** CRITICAL - Massive test failures detected

## Executive Summary

A major architectural refactoring was completed to simplify the narrative system. While the build succeeds, **the test suite is catastrophically failing**. The VSCode task runner is suppressing error output, making failures invisible in the IDE. Running the full test suite will crash the machine due to resource exhaustion.

## What Was Refactored

### 1. **Narrative Architecture Simplification**
- Created base `NarrativeScript.cs` schema (3 block types: narrative, question, composite)
- Created `Stage1Script.cs` extending base with dreamweaver scoring
- Converted `ghost.json` → `ghost.yaml` with clean sequential structure
- Created `NarrativeRenderer.cs` for simple text/choice display (no game logic)
- Created `Stage1Manager.cs` to load YAML and execute blocks sequentially

### 2. **Infrastructure Changes**
- Created `GameManager.cs` singleton for stage orchestration
- Created `StageManager.cs` abstract base class
- Added GameManager to project.godot autoload (removed NarratorEngine)
- Moved old `SceneManager` pattern to new GameManager/StageManager pattern

### 3. **Files Moved to .duplicates/ (15+ files)**
**Narrative:**
- NarrativeUI.cs
- NarratorEngine.cs
- NarrativeDataNormalizer.cs
- NarrativeInputResolver.cs
- DialogueChoice.cs
- NameValidationHarness.cs
- NarrativeSequence.cs
- BaseDialogue.cs
- asset_management/ folder
- audio/ folder

**Infrastructure:**
- StageManagement/ folder (entire old system with 10 files)
- StageController.cs (old base class)

**Stage Controllers:**
- stage_1_ghost/GhostController.cs
- stage_2_nethack/NethackHub.cs
- stage_4_party_selection/Stage4Controller.cs

## Test Failure Analysis

### **Critical Issues:**

1. **Orphan Node Epidemic**
   - Every test shows 28-44+ orphan nodes
   - Indicates improper cleanup in new architecture
   - Resources not being freed after tests complete

2. **Test Execution Problems**
   ```
   The argument /home/adam/Dev/omega-spiral/chapter-zero/.godot/mono/temp/bin/Debug/chapter-zero.dll is invalid.
   ```
   - Test runner itself may be broken
   - VSCode task shows "succeeded" but tests actually fail
   - Running full suite crashes the system (resource exhaustion)

3. **UI Test Failures (MainMenuTests.cs)**
   - `StageButtonsPopulatedFromManifest` - Expecting > 0 buttons, got 0
   - `TextAppearsHorizontally` - Expecting alignment '1', got 'Center'
   - Multiple tests failing with orphan node warnings
   - Tests expect old manifest-driven system that was removed

### **Test Result File:**
- Location: `TestResults/TestResults/test-result.trx`
- Size: 4,301 lines (massive)
- Multiple failures with orphan node warnings throughout

## What Likely Broke

### 1. **Stage Manifest System**
**Old System:**
- Stages used JSON manifests with scene lists
- `StageController.TransitionToSceneAsync()` for scene management
- MainMenu populated stage buttons from manifest

**New System:**
- No manifests - stages use YAML scripts with content blocks
- `StageManager.ExecuteStageAsync()` runs sequentially through blocks
- MainMenu likely still expects manifest data

**Impact:**
- MainMenu tests fail (no stage buttons populated)
- Stage transition tests likely broken
- Scene loading tests may fail

### 2. **Autoload Dependencies**
**Removed from autoload:**
- `NarratorEngine` (moved to .duplicates)

**Added to autoload:**
- `GameManager`

**Still in autoload (potentially broken):**
- `SceneManager` - May conflict with new GameManager
- `DreamweaverSystem` - References may be broken

**Impact:**
- Tests expecting NarratorEngine will fail
- Tests using SceneManager may have conflicts
- Orphan nodes suggest cleanup isn't happening in new singletons

### 3. **Narrative Rendering**
**Old System:**
- `NarrativeTerminal.cs` with complex logic
- `NarrativeDataNormalizer.Normalize()`
- `NarrativeInputResolver.ResolveThreadChoice()`
- Integration with DreamweaverSystem

**New System:**
- `NarrativeRenderer.cs` - simple display only
- No normalization/resolution logic
- NarrativeTerminal still exists but errors ignored

**Impact:**
- Tests using NarrativeTerminal broken (DreamweaverSystem not found errors)
- Rendering tests may fail
- Choice selection tests broken

### 4. **Data Model Changes**
**Old:**
- `DreamweaverChoice` class in narrative layer
- Complex `NarrativeSceneData` with multiple subsections
- Stage-specific data mixed with narrative data

**New:**
- Base `ChoiceOption` in narrative layer
- `Stage1ChoiceOption` extends with dreamweaver fields
- Clean separation of concerns

**Impact:**
- Tests casting to `DreamweaverChoice` will fail
- Tests expecting old data structure broken
- JSON deserialization tests broken

## Files With Known Errors (But Build Succeeds)

### **Compile Errors (Suppressed by Analyzer):**

1. **source/narrative/NarrativeTerminal.cs**
   - Missing: `DreamweaverSystem` type
   - Missing: `NarrativeDataNormalizer.Normalize()`
   - Missing: `NarrativeInputResolver.ResolveThreadChoice()`
   - Type conversion errors: `DreamweaverChoice` vs `ChoiceOption`

2. **source/narrative/dialogue/DreamweaverDialogue.cs**
   - Missing: `BaseDialogueData` (moved to .duplicates)
   - Missing: `BaseDialogueParser`
   - Missing: `IDialogueChoice`, `INarrativeBlock` interfaces

3. **source/stages/stage_3_town/TownStage.cs**
   - Missing: `DreamweaverSystem` type

4. **source/stages/stage_3_town/dialogue/NpcDialogueData.cs**
   - Missing: `BaseDialogueData`
   - Missing: `BaseDialogueChoice`, `BaseNarrativeBlock`

5. **source/field/dialogue/NpcDialogue.cs**
   - Potentially broken references (needs verification)

6. **source/stages/stage_1_ghost/ghost.json**
   - Duplicate "question" keys (JSON invalid)
   - Should be deleted (replaced by ghost.yaml)

## Recommended Actions for Next Agent

### **Immediate Priority:**

1. **Fix Test Runner**
   - Investigate why VSCode task shows success when tests fail
   - Ensure test output is visible
   - Prevent system crashes from full test runs

2. **Archive or Delete Broken Files**
   - Move NarrativeTerminal.cs to .duplicates (or refactor heavily)
   - Move DreamweaverDialogue.cs to .duplicates
   - Move NpcDialogueData.cs to .duplicates
   - Delete ghost.json (replaced by ghost.yaml)
   - Move TownStage.cs to .duplicates (Stage 3 not implemented yet)

3. **Fix Orphan Node Issues**
   - Review GameManager cleanup logic
   - Review StageManager cleanup logic
   - Ensure all nodes are freed in `_ExitTree()`
   - Fix test teardown to properly free resources

### **Secondary Priority:**

4. **Update MainMenu**
   - Remove manifest-based stage button population
   - Wire to new GameManager.StageScenes array
   - Update tests to match new architecture

5. **Fix Stage Transitions**
   - Remove old SceneManager from autoload (conflicts with GameManager)
   - Update any code using SceneManager to use GameManager
   - Fix scene transition tests

6. **Review DreamweaverSystem Integration**
   - Check if DreamweaverSystem autoload is still needed
   - Fix or remove references to it
   - Update dreamweaver scoring to use new Stage1Manager logic

7. **Clean Up Dialogue System**
   - Decide if BaseDialogue system should be restored or deleted
   - Move remaining dialogue files using old system to .duplicates
   - Update tests or remove broken dialogue tests

### **Testing Strategy:**

1. **Don't run full test suite** - Will crash system
2. **Run tests in small batches** - By directory or test class
3. **Fix orphan nodes first** - Critical for all tests
4. **Fix one subsystem at a time** - MainMenu → Stage transitions → Dialogue

## New Architecture Overview (For Reference)

```
GameManager (autoload singleton)
  ├─ StageScenes: PackedScene[]
  ├─ AdvanceToNextStageAsync()
  └─ Listens for: StageComplete signal
  
StageManager (abstract base)
  ├─ StageId: int (abstract)
  ├─ ExecuteStageAsync() (abstract)
  └─ Emits: StageComplete signal
  
Stage1Manager : StageManager
  ├─ Loads: ghost.yaml
  ├─ Uses: NarrativeRenderer
  ├─ Tracks: Dreamweaver scores
  └─ Iterates: Content blocks sequentially

NarrativeRenderer
  ├─ DisplayLinesAsync(string[])
  ├─ ShowChoicesAsync(ChoiceOption[])
  └─ Returns: Selected choice ID (no logic)
  
NarrativeScript (base)
  └─ Blocks: ContentBlock[]
      ├─ Type: narrative | question | composite
      ├─ Lines, Prompt, Options
      └─ Extension point for stages
      
Stage1Script : NarrativeScript
  └─ Stage1ChoiceOption : ChoiceOption
      └─ Adds: Dreamweaver, Scores, Philosophical
```

## Critical Questions for Review

1. Should we restore the old manifest system temporarily?
2. Should SceneManager autoload be removed entirely?
3. Should NarrativeTerminal be deleted or refactored?
4. Is the dialogue system (BaseDialogue) still needed anywhere?
5. Should ghost.json be kept for backward compatibility?
6. Are Stages 2-6 using the old architecture? (Will all break)
7. Should DreamweaverSystem integration be simplified?

## Files Changed (Git Status Summary)

**New Files:**
- source/infrastructure/GameManager.cs
- source/infrastructure/StageManager.cs
- source/narrative/NarrativeScript.cs
- source/narrative/NarrativeRenderer.cs
- source/stages/stage_1_ghost/Stage1Script.cs
- source/stages/stage_1_ghost/Stage1Manager.cs
- source/stages/stage_1_ghost/ghost.yaml

**Modified Files:**
- project.godot (autoload changes)

**Moved to .duplicates:**
- 15+ files (see section above)

**Broken But Not Moved:**
- source/narrative/NarrativeTerminal.cs
- source/narrative/dialogue/DreamweaverDialogue.cs
- source/stages/stage_3_town/TownStage.cs
- source/stages/stage_3_town/dialogue/NpcDialogueData.cs
- source/field/dialogue/NpcDialogue.cs

## Conclusion

This refactoring successfully simplified the narrative architecture **in theory**, but **broke the entire test suite in practice**. The build succeeds because the analyzer is suppressing errors from old files. The test failures are hidden by the VSCode task runner.

**The system is not production-ready and needs significant remediation work before it can run tests safely.**

Next agent should focus on:
1. Making tests visible and safe to run
2. Fixing orphan node cleanup
3. Deciding what to restore vs. what to remove
4. One subsystem at a time (don't try to fix everything at once)
