# Namespace Refactoring Mapping - frontend-refactor-code-review Branch

## Overview
This document maps all namespace changes required after moving the codebase structure from `source/services/`, `source/stages/`, `source/ui/` to the new `source/backend/` and `source/frontend/` architecture.

## Current Build Status
- **Errors:** 10 compilation errors
- **Root Cause:** Namespace mismatches after directory restructuring
- **Status:** Ready for Visual Studio batch namespace resolution

---

## Directory Structure

### New Layout
```
source/
├── backend/          ← Pure C# business logic (GameManager, CinematicDirector, schemas, etc.)
│   ├── CinematicDirector.cs
│   ├── GameManager.cs
│   ├── JournalSystem.cs
│   ├── common/
│   ├── narrative/
│   └── ...
└── frontend/         ← Godot nodes, UI, scenes
    ├── StageBase.cs
    ├── IStageBase.cs
    ├── stages/
    │   ├── stage_1_ghost/
    │   ├── stage_2_nethack/
    │   └── ...
    └── ui/
        ├── menus/
        ├── narrative/
        └── omega/
```

---

## Namespace Mapping Table

| Component | Old Namespace | New Namespace | Status |
|-----------|---------------|---------------|--------|
| GameManager | OmegaSpiral.Source.Infrastructure | OmegaSpiral.Source.Backend | ❌ Needs Update |
| StageBase | OmegaSpiral.Source.Infrastructure | OmegaSpiral.Source.Frontend | ✅ DONE |
| IStageBase | OmegaSpiral.Source.Infrastructure | OmegaSpiral.Source.Frontend | ✅ DONE |
| CinematicDirector | OmegaSpiral.Source.Scripts.Infrastructure | OmegaSpiral.Source.Backend | ❌ Needs Update |
| JournalSystem | OmegaSpiral.Source.Infrastructure | OmegaSpiral.Source.Backend | ❌ Needs Update |
| SceneManager | OmegaSpiral.Source.Infrastructure | OmegaSpiral.Source.Backend | ❌ Needs Update |
| ManifestLoader | OmegaSpiral.Source.Infrastructure | OmegaSpiral.Source.Backend | ❌ Needs Update |
| MainMenu | OmegaSpiral.Source.Stages.Stage0Start | OmegaSpiral.Source.Frontend.Ui.Menus | ❌ Needs Update |
| GhostCinematicDirector | OmegaSpiral.Source.Stages.Stage1 | OmegaSpiral.Source.Frontend.Stages.Stage1 | ❌ Needs Update |
| GhostDataLoader | OmegaSpiral.Source.Stages.Stage1 | OmegaSpiral.Source.Frontend.Stages.Stage1 | ❌ Needs Update |
| NethackCinematicDirector | OmegaSpiral.Source.Stages.Stage2 | OmegaSpiral.Source.Frontend.Stages.Stage2 | ❌ Needs Update |
| NethackDataLoader | OmegaSpiral.Source.Stages.Stage2 | OmegaSpiral.Source.Frontend.Stages.Stage2 | ❌ Needs Update |

---

## Compilation Errors to Fix

### 1. Missing Using Statements (Most Common)

Files needing `using OmegaSpiral.Source.Backend;`:
- `source/frontend/ui/menus/MainMenu.cs` - needs GameManager
- `source/frontend/stages/stage_*/` - need CinematicDirector, manifests

Files needing `using OmegaSpiral.Source.Frontend;`:
- `source/frontend/stages/stage_1_ghost/GhostCinematicDirector.cs` - needs StageBase
- `source/frontend/stages/stage_2_nethack/NethackCinematicDirector.cs` - needs StageBase
- Stage files needing IStageBase and StageCompleteDelegate

### 2. Missing CharacterData Reference

Files:
- `source/frontend/stages/stage_4_party_selection/MirrorSelectionController.cs`
- `source/frontend/stages/stage_4_party_selection/NeverGoAloneCombatController.cs`

**Solution:** Add `using OmegaSpiral.Source.Backend.Common;`

### 3. Plan Records Don't Implement IStageBase

Current Issue:
- `GhostTerminalCinematicPlan` cannot be used as `IStageBase` in `CinematicDirector<TPlan>`
- `NethackCinematicPlan` cannot be used as `IStageBase` in `CinematicDirector<TPlan>`

**Decision Point for Code Review:**
- Should plan records implement `IStageBase`? (Currently NO)
- Should we remove the constraint and use a simpler generic? (RECOMMENDED)

---

## Using Statement Organization

### Backend Files Should Have:
```csharp
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;  // Only if absolutely necessary
using OmegaSpiral.Source.Backend.Common;
```

### Frontend Files Should Have:
```csharp
using System;
using Godot;  // Always for frontend
using OmegaSpiral.Source.Backend;  // To use backend services
using OmegaSpiral.Source.Frontend;  // To use StageBase, IStageBase
```

---

## Files to Update (Priority Order)

### CRITICAL (Blocks Main Menu Loading)
1. ✅ `source/backend/GameManager.cs` - Update namespace to `OmegaSpiral.Source.Backend`
2. ✅ `source/backend/CinematicDirector.cs` - Update namespace to `OmegaSpiral.Source.Backend`
3. `source/frontend/ui/menus/MainMenu.cs` - Update namespace + add using statements

### HIGH (Blocks Stage 1-2 Loading)
4. `source/frontend/stages/stage_1_ghost/GhostCinematicDirector.cs` - Add using `OmegaSpiral.Source.Frontend`
5. `source/frontend/stages/stage_1_ghost/GhostDataLoader.cs` - Add using + fix IStageBase
6. `source/frontend/stages/stage_2_nethack/NethackCinematicDirector.cs` - Add using `OmegaSpiral.Source.Frontend`
7. `source/frontend/stages/stage_2_nethack/NethackDataLoader.cs` - Add using + fix IStageBase

### MEDIUM (Blocks Other Stages)
8. `source/frontend/stages/stage_4_party_selection/MirrorSelectionController.cs` - Add using CharacterData
9. `source/frontend/stages/stage_4_party_selection/NeverGoAloneCombatController.cs` - Add using CharacterData

---

## Architectural Decision Points for Code Review

### 1. Plan Records and IStageBase Constraint
**Question:** Should `GhostTerminalCinematicPlan` implement `IStageBase`?

**Current Approach:** NO - Plans are data containers
**Alternative Approach:** YES - Plans are also controllers

**Recommendation:** NO - Keep plans as data. Remove constraint from `CinematicDirector<TPlan>`.

### 2. Classes That Mix Godot and Logic
**Identified in backend/ that need moving to frontend/:**
- `ManifestAwareNode.cs` (extends Godot.Node)
- `ManifestAwareNodeGeneric.cs` (extends Godot.Node)
- `SceneBase.cs` (extends Godot.Control)
- `BaseNarrativeScene.cs` (extends Godot.Control)

**Decision:** Should these move to `frontend/` as they're Godot-aware?

---

## Next Steps

1. **Code Review:** Reviewers examine this mapping and architectural decisions
2. **Visual Studio Namespace Resolution:** Use VS's "Resolve" features to batch-update namespaces
3. **Build Verification:** Confirm 0 errors after updates
4. **Return to Stage 1:** After namespace cleanup, resume work on:
   - GhostStageController implementation
   - GhostAudioManager orphan node fixes
   - TDD test verification

---

## Notes

- This refactoring maintains all functionality; only structure and namespaces changed
- All tests should continue to pass once namespaces are corrected
- Three TDD test files remain in `tests/` directory (unchanged, still valid)
- Branch: `feature/frontend-refactor-code-review`
- Ready for code review before final namespace automation
