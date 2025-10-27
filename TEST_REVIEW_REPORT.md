# Omega Spiral - Chapter Zero: Test Review Report

**Date**: October 27, 2025  
**Reviewer**: GitHub Copilot  
**Purpose**: Comprehensive review of test structure, architecture, and issues for remediation

---

## Executive Summary

The test suite contains **60 total test cases across 10 test suites**, with the latest run showing **5 passing tests and 0 failures**. However, there are significant architectural problems that prevent a complete test run and mask deeper issues:

- ✅ **5 tests passing** (SceneLoadingAndViewportTests suite)
- ⚠️ **55 tests not executed** (disabled or not discovered)
- 🔴 **Major architectural issues** preventing full test execution
- ⚠️ **Orphan node warnings** (memory leak pattern)
- 🔴 **Multiple compile errors** in test files (null reference warnings, missing attributes)

---

## Critical Issues Found

### 1. **Missing `[RequireGodotRuntime]` Attribute** ⚠️ SEVERITY: HIGH

**Location**: `tests/unit/services/GameAppConfigTests.cs` (Lines 30-80)

**Problem**: Test methods use Godot functionality but are not annotated with `[RequireGodotRuntime]`.

**Affected Test Cases**:
- `TestUiLayoutConfigLoads()`
- `TestBezelMarginLoaded()`
- `TestStageNameWidthRatioLoaded()`
- `TestStatusLabelWidthRatioLoaded()`
- `TestButtonHeightLoaded()`
- `TestButtonPaddingHLoaded()`
- `TestButtonPaddingVLoaded()`
- `TestButtonSpacingLoaded()`
- `TestAllConfigValuesArePositive()`

**Root Cause**: The test class uses Godot's `CallDeferred()` in setup (`_AppConfig.CallDeferred(Godot.Node.MethodName._Ready)`) but lacks the runtime attribute.

**Impact**: Tests cannot execute properly without Godot runtime; GdUnit4 validation will fail.

**Remediation**: Add `[RequireGodotRuntime]` to all test methods that use Godot functionality.

---

### 2. **Null Reference Warning in GameAppConfigTests** ⚠️ SEVERITY: HIGH

**Location**: `tests/unit/services/GameAppConfigTests.cs` (Line 20)

**Problem**: 
```csharp
_AppConfig.CallDeferred(Godot.Node.MethodName._Ready);
```
Dereference of a possibly null reference.

**Root Cause**: `_AppConfig` is nullable (`GameAppConfig?`) but is used without null-checking after `AutoFree()`. The `AutoFree()` call may return null if memory allocation fails or if the object isn't properly initialized.

**Impact**: Runtime crash if `_AppConfig` is null; test fails unpredictably.

**Remediation**: 
- Add null check: `_AppConfig?.CallDeferred(...)`
- Or use `!` operator after confirming non-null: `_AppConfig!.CallDeferred(...)`

---

### 3. **Orphan Nodes Detected During Test Execution** ⚠️ SEVERITY: MEDIUM

**Pattern in Test Results**:
```
Warning: Detected <1> orphan nodes during test execution!  (CreateCenteredXControl)
Warning: Detected <24> orphan nodes during test execution! (StageSelectMenu_NeverRendersOffscreen)
Warning: Detected <72> orphan nodes during test execution! (Scene_LoadsConsistentlyAcrossMultipleInstances)
```

**Problem**: Nodes are being created but not properly freed after tests complete.

**Root Cause**: Scene loading tests in `SceneLoadingAndViewportTests` don't properly dispose of loaded scenes. Multiple node hierarchies accumulate during testing.

**Impact**: 
- Memory leak during test suite execution
- Godot warns about orphan nodes
- Potential for performance degradation in long test runs
- Indicates improper resource cleanup patterns in scene loading code

**Affected Code Area**: Likely in `source/` codebase where scenes are loaded during tests without proper cleanup.

**Remediation**: Ensure all loaded scenes use `QueueFree()` or are properly disposed in test teardown.

---

### 4. **Disabled Test Files** ⚠️ SEVERITY: CRITICAL

**Disabled Test Count**: 11 test files

**Disabled Tests**:
- `tests/common/SceneManagerTests.cs.disabled` - SceneManager functionality
- `tests/integration/stages/stage_2/NethackStageTests.cs.disabled` - ASCII Dungeon stage
- `tests/integration/stages/stage_3/CombatSystemTests.cs.disabled` - Combat system
- `tests/integration/stages/stage_3/NpcInteractionTests.cs.disabled` - NPC interactions
- `tests/integration/stages/stage_3/PlayerMovementTests.cs.disabled` - Player movement
- `tests/integration/stages/stage_3/Stage4InitializationTests.cs.disabled` - Stage 4 initialization

**Reason for Disabling**: Unknown (files have `.disabled` extension)

**Impact**: 
- Core functionality not being tested (scene management, stage transitions, combat, player control)
- ~30+ test cases not running
- No visibility into whether core systems work

**Remediation**: 
1. Investigate why tests were disabled
2. Fix underlying issues in test code or source code
3. Re-enable tests with proper fixes

---

### 5. **Broken Tests in Quarantine** ⚠️ SEVERITY: HIGH

**Broken Test Directory**: `tests/integration/stages/stage_2/.broken/`

**Broken Tests**:
1. `SceneTransitionEndToEndTest.cs` - Scene transition tests
2. `AsciiDungeonEndToEndTest.cs` - ASCII dungeon end-to-end tests

**Problem**: Tests exist but don't run (files in `.broken` directory)

**Impact**: 
- Scene transition logic not validated
- ASCII dungeon functionality not tested
- End-to-end workflow broken between Stage 1 and Stage 2

**Sample Issue** (from SceneTransitionEndToEndTest):
```csharp
sceneManager.TransitionToScene("Scene2NethackSequence");
// Issue: No verification that scene actually changed
// Issue: No assertions after transition
// Issue: Tests don't use [RequireGodotRuntime]
```

**Remediation**: 
1. Move tests out of `.broken/` directory
2. Add `[RequireGodotRuntime]` attribute
3. Add proper scene loading verification
4. Add assertions to verify state after transitions

---

### 6. **Pending Tests** ⚠️ SEVERITY: MEDIUM

**Pending Test Directory**: `tests/integration/stages/stage_3/.pending/`

**Pending Tests** (11 files, ~40+ test cases):
- `CombatSystemTests.cs`
- `HouseSceneTests.cs`
- `UiConsistencyTests.cs`
- `BattleSceneTests.cs`
- `FieldCombatStageTests.cs`
- `Stage4InitializationTests.cs`
- `NpcInteractionTests.cs`
- `TownSceneTests.cs`
- `PerformanceTests.cs`
- `PlayerMovementTests.cs`
- `TownExplorationTests.cs`

**Problem**: Tests exist but are quarantined and not running.

**Impact**: 
- Stage 3 (Field Combat Stage / Town exploration) functionality untested
- ~40+ potential test cases not running
- No visibility into Stage 3/4 correctness

**Remediation**: 
1. Review pending tests
2. Fix issues preventing execution
3. Move to main test directory with proper fixes

---

### 7. **Static Test Classes** ⚠️ SEVERITY: MEDIUM

**Affected Tests**:
- `ContentBlockIntegrationTests.cs` - uses `public static class`
- `ContentBlockTests.cs` - uses `public static class`
- `GhostCinematicDirectorTests.cs` - uses `public static class`
- `NarrativeScriptFunctionalTests.cs` - uses `public static class`

**Problem**: 
```csharp
[TestSuite]
[RequireGodotRuntime]
public static class ContentBlockIntegrationTests  // ← Static class
{
    [TestCase]
    public static async Task BootSequenceSceneLoadsSuccessfully()  // ← Static method
```

**Issue**: GdUnit4 best practices recommend non-static test classes for proper lifecycle management, setup/teardown, and resource disposal.

**Impact**: 
- May cause issues with test discovery
- Prevents proper setup/teardown execution
- Makes resource cleanup difficult
- Violates GdUnit4 recommended patterns

**Remediation**: Convert static test classes to instance classes:
```csharp
[TestSuite]
[RequireGodotRuntime]
public class ContentBlockIntegrationTests  // ← Instance class
{
    [TestCase]
    public async Task BootSequenceSceneLoadsSuccessfully()  // ← Instance method
```

---

### 8. **Inconsistent Setup/Teardown Patterns** ⚠️ SEVERITY: MEDIUM

**Pattern 1** (GameAppConfigTests.cs - Good):
```csharp
[Before]
public void Setup() { ... }

[After]
public void Teardown() { ... }
```

**Pattern 2** (Many integration tests - Missing):
```csharp
// No [Before] / [After] methods
// No resource cleanup
// Relies on automatic disposal only
```

**Impact**: 
- Inconsistent resource cleanup
- Some tests may leave resources allocated
- Contributing to orphan node warnings

**Remediation**: Add setup/teardown to all test classes that load scenes or create Godot objects.

---

### 9. **Missing Null-Safety Patterns** ⚠️ SEVERITY: MEDIUM

**Example Issues**:

In GameAppConfigTests.cs:
```csharp
public void TestBezelMarginLoaded()
{
    AssertThat(_AppConfig!.BezelMargin).IsEqual(0.05f);  // ← Forced null-coalesce operator
}
```

**Problem**: 
- Uses `!` (null-forgiving operator) instead of null-checking
- If `_AppConfig` is null, will throw at runtime
- Indicates underlying null reference issue

**Remediation**: Fix root cause (ensure setup properly initializes) rather than suppressing warnings.

---

### 10. **Test Organization Issues** ⚠️ SEVERITY: LOW

**Current Structure**:
```
tests/
├── common/                          # Old location
├── unit/                            # New location
├── integration/                     # New location
├── shared/                          # Helpers
└── integration/stages/
    ├── stage_1_ghost/
    ├── stage_2/
    │   └── .broken/                 # Broken tests
    └── stage_3/
        └── .pending/                # Pending tests
```

**Issues**:
- Tests split across `tests/common/` and `tests/unit/`
- `.broken/` and `.pending/` directories not part of main suite
- Inconsistent naming (stage_1_ghost vs stage_2 vs stage_3)
- SceneManagerTests in old location, others in new location

**Remediation**: Consolidate to single location with clear naming:
```
tests/
├── unit/
├── integration/
├── shared/
└── _disabled/  (for disabled tests, clearly marked)
```

---

## Test Execution Summary

### Tests That Run (5/60 - 8.3%):
- ✅ `SceneLoadingAndViewportTests.CreateCenteredXControl`
- ✅ `SceneLoadingAndViewportTests.StageSelectMenu_NeverRendersOffscreen`
- ✅ `SceneLoadingAndViewportTests.AnyScene_LoadsWithinViewportBounds`
- ✅ `SceneLoadingAndViewportTests.Scene_LoadsConsistentlyAcrossMultipleInstances`
- ✅ `SceneLoadingAndViewportTests.Scene_HasValidDimensions`

### Tests Not Running (55/60 - 91.7%):
- ⏸️ Tests in disabled files (11 files)
- ⏸️ Tests in `.broken/` directory (2 files)
- ⏸️ Tests in `.pending/` directory (11 files)
- ⏸️ Tests with compilation errors (9 methods in GameAppConfigTests)
- ⏸️ Other functional/narrative tests (not discovered in this run)

---

## Architectural Pattern Issues

### Issue A: Scene Loading Without Proper Cleanup

**Symptoms**: Orphan node warnings

**Root Cause Pattern**:
```csharp
// In test code
var scene = ResourceLoader.Load<PackedScene>("res://path.tscn");
var instance = scene.Instantiate();
// Missing: instance.QueueFree();
```

**Affects**: `SceneLoadingAndViewportTests` and all scene-loading integration tests

**Code Area to Review**: 
- `source/services/` - Scene loading implementation
- `source/infrastructure/` - Resource management
- Test setup/teardown methods

---

### Issue B: Godot Lifecycle Management

**Symptoms**: Tests not using `[RequireGodotRuntime]` properly

**Root Cause**: Test code calls Godot APIs (`CallDeferred`, scene loading) without declaring dependency

**Affects**: 
- `GameAppConfigTests.cs`
- Various integration tests
- UI tests

**Code Pattern to Fix**:
```csharp
// BEFORE (broken)
[TestSuite]
public class MyTest {
    void Setup() { 
        _obj.CallDeferred(...);  // ← Godot API without runtime
    }
}

// AFTER (correct)
[TestSuite]
[RequireGodotRuntime]
public class MyTest {
    void Setup() { 
        _obj.CallDeferred(...);  // ← Godot API with runtime declared
    }
}
```

---

### Issue C: Static Test Classes

**Symptoms**: Tests with `public static class` and `[TestCase] public static async Task`

**Root Cause**: Misunderstanding of GdUnit4 patterns

**Affects**:
- `ContentBlockIntegrationTests.cs`
- `ContentBlockTests.cs`
- `GhostCinematicDirectorTests.cs`
- `NarrativeScriptFunctionalTests.cs`

---

## Recommendations for the Remediation Team

### Phase 1: Fix Compilation Errors (HIGH PRIORITY)
1. **Add `[RequireGodotRuntime]`** to `GameAppConfigTests` - 9 test methods
   - Estimated effort: 10 minutes
   - Impact: Unlocks GameAppConfig tests

2. **Fix null reference** in `GameAppConfigTests.Setup()` line 20
   - Estimated effort: 15 minutes
   - Impact: Prevents runtime crashes

### Phase 2: Fix Test Architecture (HIGH PRIORITY)
1. **Convert static test classes to instance classes** (4 files in stage_1_ghost)
   - Estimated effort: 30 minutes
   - Impact: Proper GdUnit4 lifecycle management

2. **Add setup/teardown to all integration tests**
   - Estimated effort: 1-2 hours
   - Impact: Proper resource cleanup, reduced orphan nodes

### Phase 3: Re-enable and Fix Disabled Tests (MEDIUM PRIORITY)
1. **Review and re-enable disabled tests** (11 files)
   - `SceneManagerTests.cs`
   - Stage tests (stage_2, stage_3)
   - Estimated effort: 2-3 hours per file
   - Impact: Full test coverage on core systems

2. **Move broken tests out of quarantine** (2 files in `.broken/`)
   - Add proper runtime attributes
   - Add scene transition verification
   - Estimated effort: 1-2 hours

### Phase 4: Resolve Pending Tests (MEDIUM PRIORITY)
1. **Migrate pending tests** from `.pending/` directory
   - Fix compilation/runtime issues
   - Add proper setup/teardown
   - Estimated effort: 4-6 hours
   - Impact: Stage 3/4 coverage

### Phase 5: Fix Orphan Node Warnings (MEDIUM PRIORITY)
1. **Investigate scene loading code** in source/
   - Find all instances where scenes are loaded without cleanup
   - Add `QueueFree()` calls or proper disposal
   - Estimated effort: 2-3 hours

2. **Update test teardown** to ensure scene cleanup
   - Estimated effort: 1 hour

### Phase 6: Organize Test Structure (LOW PRIORITY)
1. **Consolidate test directories**
   - Move `tests/common/` tests to appropriate `tests/unit/` or `tests/integration/`
   - Create `tests/_disabled/` for disabled tests
   - Estimated effort: 30 minutes

---

## Key Metrics

| Metric | Value |
|--------|-------|
| Total Test Suites | 10 |
| Total Test Cases | 60 |
| Currently Running | 5 |
| Coverage % | 8.3% |
| Disabled Tests | 11+ files |
| Broken Tests | 2 files |
| Pending Tests | 11 files |
| Compilation Errors | 9 methods |
| Orphan Node Warnings | Present (memory leak) |

---

## Next Steps

1. **Assign Phase 1 and Phase 2** to remediation team (highest ROI for effort)
2. **Create issue tracking** for each disabled/broken/pending test
3. **Establish code review process** for test fixes
4. **Run tests after each fix** to verify improvement
5. **Target**: Get test coverage back to 60+ running tests (100%)

---

## Appendix: Test Files Status

### ✅ Running Tests
- `tests/integration/common/SceneLoadingAndViewportTests.cs` (5 test cases)

### 🔴 Compilation Errors (Blocked)
- `tests/unit/services/GameAppConfigTests.cs` (9 test methods - missing `[RequireGodotRuntime]`)

### ⏸️ Disabled Files (11)
- `tests/common/SceneManagerTests.cs.disabled`
- `tests/integration/stages/stage_2/NethackStageTests.cs.disabled`
- `tests/integration/stages/stage_3/CombatSystemTests.cs.disabled`
- `tests/integration/stages/stage_3/NpcInteractionTests.cs.disabled`
- `tests/integration/stages/stage_3/PlayerMovementTests.cs.disabled`
- `tests/integration/stages/stage_3/Stage4InitializationTests.cs.disabled`
- (plus 5 more with .uid.disabled)

### 🚫 Broken Files (2 in `.broken/`)
- `tests/integration/stages/stage_2/.broken/SceneTransitionEndToEndTest.cs`
- `tests/integration/stages/stage_2/.broken/AsciiDungeonEndToEndTest.cs`

### ⏳ Pending Files (11 in `.pending/`)
- All in `tests/integration/stages/stage_3/.pending/`
- 40+ test cases

### 🔄 Other Test Files
- Many integration UI tests, narrative tests, etc. (discovered but status unclear)

