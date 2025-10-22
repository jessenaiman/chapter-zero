# Project Code Smell & Unimplemented Stages Review

**Date**: October 21, 2025  
**Scope**: Full codebase analysis  
**Project**: Omega Spiral - Chapter Zero

---

## Executive Summary

### Overall Health: ⚠️ MODERATE CONCERNS
- **5 Stages**: 1 complete (Stage 1), 2 partially done (Stage 2-3), 2 not started (Stage 4-5)
- **Code Smells**: 40+ identified (TODO/FIXME markers, placeholders, incomplete implementations)
- **Test Coverage**: Fragmented across stages, some test files in `.disabled` or `.pending`
- **Architectural Debt**: Duplication, dead code, inconsistent error handling

---

## Stage Implementation Status

### ✅ Stage 1: Ghost Terminal (COMPLETE)
**Status**: Fully implemented and tested  
**Tests**: 22 unit tests passing, 4 integration tests available  
**Files**: 
- `source/stages/ghost/scripts/` - TerminalBase, Question1-6 scenes
- `tests/stages/stage_1/` - Complete test coverage

**Quality**: GOOD
- Proper unit/integration test separation (just completed)
- Comprehensive scene implementation
- Working shader pipeline (CRT effects)
- All 9 scenes implemented with narrative flow

**Known Issues**: NONE CRITICAL

---

### 🟡 Stage 2: Echo Hub (PARTIAL)
**Status**: Stub implementations, not fully playable  
**Files**:
- `source/stages/stage_2/EchoHub.cs` - Entry point (stub)
- `source/stages/stage_2/EchoInterlude.cs` - Placeholder
- `source/stages/stage_2/EchoDungeon.cs` - Minimal stub
- `source/stages/stage_2/nethack/nethack_sequence.tscn` - Scene with init message

**Current Implementation**:
```csharp
// source/stages/stage_2/EchoDungeon.cs
/// Minimal dungeon scene stub. Future work will render the map and handle movement.
[GlobalClass]
public partial class EchoDungeon : Node2D { }
```

**Code Smells**:
1. ❌ **Empty stub class** - No actual implementation
2. ❌ **Placeholder interlude** - "Will later handle choice rendering"
3. ❌ **Disabled Stage 2 tests** - Tests in `.pending` folder, not running
4. ⚠️ **Missing ASCII dungeon renderer** - AsciiRoomRenderer has TODO comments
5. ⚠️ **No combat flow** - NetHack-style combat not integrated

**Blockers**:
- Requires TerminalDisplayBox reimplementation (removed during Stage 1 cleanup)
- AsciiRoomRenderer needs 3D effects restoration
- Dungeon movement system not started

**Test Status**: 
- `tests/stages/stage_4/.pending/Stage4InitializationTests.cs` exists but disabled
- No working Stage 2 tests

---

### 🔴 Stage 3: Never Go Alone (PARTIAL)
**Status**: Design doc exists, minimal code  
**Files**:
- `source/stages/stage_3/` - Empty or minimal files
- `docs/stages/stage_3/` - Design documentation

**Known Issues**:
- ❌ No test infrastructure yet
- ❌ Character party system not connected
- ❌ No UI implementation

---

### 🔴 Stage 4: Town Exploration (NOT STARTED)
**Status**: Godot-open-RPG integration pending  
**Files**:
- `source/stages/stage_4/tile_dungeon.tscn` - Scene file exists
- `source/stages/stage_4/TileDungeon.cs` - Minimal script
- `source/stages/stage_4/field/` - Some components exist

**Code Smells**:
1. ⚠️ **Signal binding issues** - Multiple GD0202 compilation errors
2. ⚠️ **Placeholder combat** - PixelCombatController has many TODO comments
3. ❌ **Test file in .pending** - Not integrated with build
4. ❌ **NUnit attributes in GdUnit4 tests** - Category decorators cause compilation errors

**Compilation Errors** (BLOCKING):
```
source/stages/stage_4/TileDungeon.cs - Signal signature mismatches
source/scripts/field/PixelCombatController.cs - GD0202 Vector2i conversion issues
tests/stages/stage_4/.pending/DungeonStageSignalTests.cs - [Category] attribute not found
```

**Placeholder Code**:
```csharp
// source/scripts/field/PixelCombatController.cs line 197
/// Performs a magic action. Placeholder implementation.
private void PerformMagic()
{
    // Placeholder magic implementation
    PerformAttack(true);
}

/// Performs an item use action. Placeholder implementation.
private void PerformItem()
{
    // Placeholder item implementation
    _playerHP += 20;
}
```

---

### 🔴 Stage 5: Final Convergence (NOT STARTED)
**Status**: Design doc exists, no implementation  
**Files**:
- `source/data/stages/stage_5/stage_5.json` - Data file only
- No C# implementation files

**Known Issues**:
- ❌ No code written
- ❌ No scene files
- ❌ No tests

---

## Critical Code Smells (Ranked by Severity)

### CRITICAL ISSUES (Block Build/Tests)

| Issue | Location | Impact | Fix |
|-------|----------|--------|-----|
| **Signal signature mismatch (GD0202)** | source/stages/stage_4/TileDungeon.cs | Build fails | Fix Vector2i → Variant conversion in signal parameters |
| **NUnit [Category] in GdUnit4** | tests/stages/stage_2/DungeonStageSignalTests.cs, tests/common/GameStateSignalTests.cs | Build fails | Replace `[Category]` with `[TestCase]` or remove |
| **Duplicate SceneManagerTests** | tests/common/SceneManagerTests.cs.disabled | Unused code | Delete .disabled file, consolidate with Unit/Infrastructure version |
| **Missing TerminalDisplayBox** | source/scripts/field/AsciiRoomRenderer.cs lines 31, 165, 191 | Stage 2 broken | Restore component or reimplement |

### HIGH SEVERITY (Code Quality)

| Issue | Location | Lines | Problem |
|-------|----------|-------|---------|
| **Placeholder implementations** | source/scripts/field/PixelCombatController.cs | 197-220, 234, 248-253, 305, 315 | Magic/Item actions do nothing; combat returns not implemented |
| **Empty stub classes** | source/stages/stage_2/EchoDungeon.cs | 8 | "Future work will render the map" - no implementation |
| **TODO return to scene** | source/scripts/field/PixelCombatController.cs line 234 | 1 | Combat escape doesn't return to previous scene |
| **Placeholder error handling** | source/scripts/field/TileDungeonController.cs line 78 | 1 | "This is a placeholder implementation" |
| **Cyclomatic complexity** | source/stages/ghost/scripts/TerminalBase.cs line 450 | 1 | ApplyVisualPreset complexity = 11 (limit 8) |
| **Too many parameters** | source/stages/ghost/scripts/TerminalBase.cs line 513 | 1 | SetPhosphorSettings has 9 parameters (limit 8) |

### MEDIUM SEVERITY (Maintainability)

| Issue | Location | Problem |
|-------|----------|---------|
| **Duplicate NarrativeSceneData** | source/narrative/PersonaConfig.cs vs source/scripts/field/narrative/NarrativeSceneData.cs | "TODO: duplicate - Consider consolidating" |
| **Duplicate choice classes** | source/narrative/DreamweaverChoice.cs vs PersonaConfig.cs | "TODO: duplicate" |
| **Commented-out UIDialogue code** | source/narrative/NarrativeSequence.cs lines 45, 92, 139 | 4 TODO markers for unimplemented class |
| **Disabled cinematic system** | source/narrative/NarrativeTerminal.cs lines 44, 171 | "TODO: Stage1 rebuild - Old cinematic director disabled" |
| **Missing LLM integration** | source/narrative/DreamweaverPersona.cs lines 369, 391 | "TODO: Implement actual NobodyWho integration when scene nodes available" |
| **Hardcoded scene paths** | Multiple files | Scene transitions hardcoded instead of in config |
| **No timeout handling** | source/scripts/SceneManager.cs | Loading screens never implemented |

### LOW SEVERITY (Future Enhancements)

| Issue | Location | Problem |
|-------|----------|---------|
| **Magic integration placeholder** | source/resources/scenes/dreamweaver_core.tscn | "TODO: Add NobodyWhoModel script when LLM integration complete" |
| **Chat interface TODO** | source/resources/scenes/dreamweaver_core.tscn line 37 | "TODO: Add chat interface for managing LLM conversations" |
| **Multiplayer sync TODO** | source/resources/scenes/dreamweaver_core.tscn line 45 | "TODO: Add signal broadcasting for multiplayer narrative sync" |
| **Options menu** | source/scripts/ui/menus/MainMenu.cs line 107 | "TODO: Load options scene" |
| **Portrait property missing** | source/scripts/ui/UIDialogue.cs line 626 | "TODO: Add Portrait property to Character class" |
| **Game over logic** | source/overworld/maps/CombatEncounterTrigger.cs line 133 | "TODO: Implement game over logic" |

---

## Test Infrastructure Issues

### ✅ What's Working
- Stage 1: Full unit + integration test split (just completed)
- GdUnit4 framework properly configured
- Test doubles pattern working well
- Parameterized tests implemented

### ❌ What's Broken
- **Duplicate test file**: `tests/common/SceneManagerTests.cs.disabled` marked for consolidation
- **Disabled test suites**: 
  - `tests/stages/stage_4/.pending/Stage4InitializationTests.cs`
  - `tests/stages/stage_1/NeverGoAloneControllerTests.cs.disabled`
- **Pending implementation**: `tests/stages/stage_1/TerminalTextVisualTest.cs` marked TODO
- **NUnit incompatibility**: 3+ test files using `[Category]` instead of GdUnit4 `[TestCase]`

### 🟡 Partially Working
- Stage 2 testing infrastructure exists but tests are pending
- Stage 3 has no tests yet
- Stages 4-5 have minimal or no tests

---

## Architectural Concerns

### 1. **Namespace Inconsistency**
```csharp
// Stage 1
namespace OmegaSpiral.Source.Scripts.Stages.Stage1;

// Stage 2
namespace OmegaSpiral.Source.Stages.Stage2;  // Different pattern

// Varies inconsistently
```
**Impact**: Import statements fragile, hard to discover classes  
**Fix**: Standardize on `OmegaSpiral.Stages.StageN` or `OmegaSpiral.Source.Stages.StageN`

### 2. **Scene Management Duplication**
- MainMenu.cs: Scene transition with fallback
- Question6_Continue.cs: Different scene transition pattern
- SceneManager: Third pattern with validation

**Impact**: Inconsistent state management  
**Fix**: Consolidate into single SceneManager pattern

### 3. **Error Handling Inconsistency**
```csharp
// Style 1: GetNodeOrNull with fallback
var runner = GetNodeOrNull<GameState>("/root/GameState");

// Style 2: GetNode with no guard
var startButton = GetNode<Button>("...");  // Can crash

// Style 3: ResourceLoader.Exists check
if (!ResourceLoader.Exists(scenePath)) { }
```

**Impact**: Crash-prone code paths  
**Fix**: Adopt consistent null-safety pattern

### 4. **Signal Binding Issues**
Multiple compilation errors in Stage 4:
```csharp
GD0202: Argument 4: cannot convert from 'DreamweaverType' to 'Godot.Variant'
```

**Root Cause**: Custom enums in signal parameters  
**Fix**: Convert enums to int or string before signal emission

---

## Specific TODO/FIXME Analysis

### High Priority (Blocks Stages)
```csharp
// source/scripts/field/PixelCombatController.cs
// TODO: Return to previous scene (line 234)
// TODO: Calculate from party (line 97)
// TODO: Calculate based on party stats (line 253)
// TODO: Game over (line 305)
// TODO: Victory sequence (line 315)
```

### Medium Priority (Architecture)
```csharp
// source/scripts/field/AsciiRoomRenderer.cs
// TODO: Stage2 - TerminalDisplayBox was removed, needs reimplementation (line 31)
// TODO: Stage2 - Temporarily disabled 3D effects (line 165)
// TODO: Stage2 - Lighting also disabled (line 191)
```

### Low Priority (Nice-to-have)
```csharp
// source/resources/scenes/dreamweaver_core.tscn
// TODO: Add NobodyWhoModel script when LLM integration complete (line 31)
// TODO: Add chat interface (line 37)
// TODO: Add signal broadcasting (line 45)
```

---

## Quick Wins (Can Fix Now)

| Fix | Effort | Impact | Status |
|-----|--------|--------|--------|
| Extract test doubles to separate file (Step 2) | 15 min | High - Enables Stage 2 testing | READY |
| Remove duplicate SceneManagerTests.cs.disabled | 5 min | Low - Cleanup | READY |
| Fix [Category] → GdUnit4 in Stage 4 tests | 10 min | High - Unblocks build | READY |
| Update namespace consistency | 30 min | Medium - Better discovery | READY |
| Fix TerminalBase cyclomatic complexity | 20 min | Low - Code quality | READY |

---

## Unimplemented Features (By Stage)

### Stage 1 ✅
- [x] All 9 scenes
- [x] Shader pipeline
- [x] DreamweaverScore tracking
- [x] Thread determination
- [x] Story transitions

### Stage 2 🔴
- [ ] ASCII dungeon rendering (blocked: TerminalDisplayBox missing)
- [ ] NetHack-style navigation
- [ ] Combat encounters
- [ ] Dungeon level generation
- [ ] Interlude dialogue
- [ ] Scene choice integration
- [ ] 3D effects/lighting

### Stage 3 🔴
- [ ] Character party UI
- [ ] Party selection logic
- [ ] Affinity system
- [ ] Dialogue sequences
- [ ] Choice outcomes

### Stage 4 🔴
- [ ] Tile-based exploration
- [ ] Player movement
- [ ] Enemy spawning
- [ ] Combat system (placeholder only)
- [ ] Map rendering
- [ ] Collision system

### Stage 5 🔴
- [ ] All systems (not started)

---

## Recommendations

### Immediate Actions (This Sprint)
1. **Fix Stage 4 compilation errors** (2 hours)
   - Convert DreamweaverType enums to int in signals
   - Remove [Category] attributes from test files
   
2. **Extract test doubles** (Step 2, 15 min)
   - Move TestContentBlock, TestTransitionContext, TestChoiceContext to separate file
   - Enables reuse across test suites

3. **Fix TerminalBase code quality** (20 min)
   - Extract ApplyVisualPreset switch into separate methods
   - Group SetPhosphorSettings parameters into config object

### Short Term (Next 2 Weeks)
1. **Restore Stage 2 infrastructure**
   - Reimplement TerminalDisplayBox or replacement
   - Restore 3D effects to AsciiRoomRenderer
   - Re-enable Stage 2 tests

2. **Consolidate test infrastructure**
   - Remove NUnit/GdUnit4 hybrid patterns
   - Enable all test files
   - Establish consistent test structure across stages

3. **Namespace standardization**
   - Choose one pattern: `OmegaSpiral.Stages.StageN` or current
   - Update all imports systematically

### Medium Term (1-2 Months)
1. **LLM Integration** (NobodyWho plugin)
   - Implement UiDialogue class (currently stubbed)
   - Connect DreamweaverPersona to LLM
   - Add multiplayer narrative sync signals

2. **Complete Stage 2 Implementation**
   - Finish dungeon rendering
   - Implement combat flow
   - Connect to Stage 1 → Stage 3 transitions

3. **Stage 3 & 4 Implementation**
   - Character party selection UI
   - Tile-based exploration
   - Combat systems

### Long Term (2-3 Months)
1. Complete Stage 5
2. Polish and optimization
3. Performance profiling

---

## Risk Assessment

| Risk | Probability | Impact | Mitigation |
|------|-------------|--------|------------|
| Stage 4 compilation blocks build | HIGH | CRITICAL | Fix signal bindings immediately |
| Stage 2 incomplete (TerminalDisplayBox) | HIGH | HIGH | Prioritize reimplementation |
| Test infrastructure fragmentation | MEDIUM | HIGH | Consolidate NUnit/GdUnit patterns |
| Namespace inconsistency causes import errors | MEDIUM | MEDIUM | Standardize early |
| Placeholder code shipped to production | LOW | HIGH | Remove all placeholder implementations before release |

---

## Code Health Scorecard

```
Stage 1: ████████░ 80% (Complete, minor polish needed)
Stage 2: ██░░░░░░░ 20% (Stubs only, critical path blocked)
Stage 3: ██░░░░░░░ 20% (Design exists, no implementation)
Stage 4: ██░░░░░░░ 20% (Broken build, needs signal fixes)
Stage 5: █░░░░░░░░ 10% (Data file only)

Tests:  ███░░░░░░ 30% (Stage 1 good, others fragmented)
Code Quality: ███░░░░░░ 30% (Many TODO/placeholders)
Architecture: ███░░░░░░ 30% (Inconsistent patterns)
Documentation: ████░░░░░ 40% (Stage 1-3 has docs, 4-5 lacking)
```

---

## Files to Review/Fix

**Delete/Consolidate**:
- [ ] `tests/common/SceneManagerTests.cs.disabled` (duplicate)
- [ ] `tests/stages/stage_1/NeverGoAloneControllerTests.cs.disabled` (if no longer needed)

**Enable**:
- [ ] `tests/stages/stage_4/.pending/Stage4InitializationTests.cs`
- [ ] `tests/stages/stage_1/TerminalTextVisualTest.cs`

**Fix**:
- [ ] `source/stages/stage_4/TileDungeon.cs` (signal bindings)
- [ ] `tests/common/GameStateSignalTests.cs` (NUnit → GdUnit4)
- [ ] `tests/stages/stage_2/DungeonStageSignalTests.cs` (NUnit → GdUnit4)
- [ ] `source/stages/ghost/scripts/TerminalBase.cs` (complexity)

**Implement**:
- [ ] `source/scripts/field/AsciiRoomRenderer.cs` (3D effects)
- [ ] `source/narrative/NarrativeSequence.cs` (UIDialogue)
- [ ] `source/scripts/field/PixelCombatController.cs` (actual combat)

---

**Next Action**: Fix Stage 4 compilation errors, then extract test doubles (Step 2).
