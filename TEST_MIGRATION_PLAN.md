# Test Migration Plan: Source-Aligned Structure

## Executive Summary
Restructure the Tests directory to mirror Source directory organization with clear separation between Unit tests (logic-only) and Gameplay tests (Godot runtime-dependent).

## Current State Analysis

### Existing Structure Issues
- Mixed organizational patterns (category-based + feature-based)
- Unit tests scattered across `Tests/Unit/` and mixed with gameplay tests
- No clear separation between pure logic and Godot runtime tests
- Difficult to navigate and maintain

### Current Categories
```
Tests/
├── Common/                  (mixed unit + gameplay)
├── Dungeon/                 (mixed unit + gameplay)
├── EndToEnd/                (gameplay)
├── Field/                   (gameplay)
├── Integration/             (gameplay)
├── Unit/                    (pure unit)
├── Stages/                  (feature-based, mixed)
└── Visual/                  (gameplay)
```

## Target Structure

### Source Mirror Organization
```
Tests/
├── Data/
│   └── stages/
│       ├── nethack/
│       │   ├── Unit/
│       │   │   └── NethackDungeonSequenceLoaderTests.cs
│       │   └── NethackStageGameplayTests.cs
│       ├── character-selection/
│       ├── combat-dialog/
│       └── ghost-terminal/
├── Scripts/
│   ├── combat/
│   │   ├── Unit/
│   │   │   └── CombatLogicTests.cs
│   │   └── CombatGameplayTests.cs
│   ├── common/
│   │   ├── Unit/
│   │   │   └── GameStateTests.cs
│   │   └── CommonGameplayTests.cs
│   ├── field/
│   │   ├── Unit/
│   │   │   └── FieldLogicTests.cs
│   │   └── FieldGameplayTests.cs
│   ├── infrastructure/
│   │   ├── dungeon/
│   │   │   ├── Unit/
│   │   │   │   ├── AsciiDungeonSequenceLoaderTests.cs
│   │   │   │   └── DreamweaverAffinityServiceTests.cs
│   │   │   └── DungeonInfrastructureGameplayTests.cs
│   │   └── Unit/
│   │       ├── SceneManagerTests.cs
│   │       ├── ConfigurationServiceTests.cs
│   │       └── SceneLoaderServiceTests.cs
│   ├── persistence/
│   │   ├── Unit/
│   │   │   └── SaveLoadManagerTests.cs
│   │   └── PersistenceGameplayTests.cs
│   └── UI/
│       ├── Unit/
│       └── UIGameplayTests.cs
├── Stages/
│   ├── Stage-1/
│   │   ├── Unit/
│   │   │   ├── NarrativeContentTests.cs
│   │   │   └── TerminalLogicTests.cs
│   │   └── Stage1GameplayTests.cs
│   ├── Stage-2/
│   │   ├── Unit/
│   │   │   └── NethackSequenceLogicTests.cs
│   │   └── Stage2GameplayTests.cs
│   ├── Stage-3/
│   │   ├── Unit/
│   │   └── Stage3GameplayTests.cs
│   └── Stage-4/
│       ├── Unit/
│       └── Stage4GameplayTests.cs
└── Integration/
    └── CrossStageIntegrationTests.cs
```

## Test Classification Rules

### Unit Tests (in `/Unit` folders)
- **No Godot Runtime**: Logic-only tests using `[TestCase]`
- **No Scene Dependencies**: No Nodes, Resources, or scene manipulation
- **Fast Execution**: Tests complete in milliseconds
- **Examples**:
  - JSON parsing and validation
  - Data structure integrity
  - Algorithm correctness
  - Service logic (mocked dependencies)
  - State calculations

### Gameplay Tests (in main feature folders)
- **Godot Runtime Required**: Use `[RequireGodotRuntime]` or `[TestCase]` with scene setup
- **Scene/Node Interactions**: Tests that require Godot objects, signals, physics, etc.
- **Integration Focus**: Tests that validate gameplay experience
- **Slower Execution**: Tests may take seconds
- **Examples**:
  - Dungeon stage progression
  - Object interaction events
  - Scene transitions
  - Event publisher validation
  - Affinity scoring with Godot objects

## Migration Phases

### Phase 1: Create New Directory Structure
1. Create `Tests/Data/stages/` directories
2. Create `Tests/Scripts/` with subdirectories matching Source structure
3. Create `Tests/Stages/` Stage-specific directories
4. Create `Tests/Integration/` (consolidate existing end-to-end)

### Phase 2: Split and Reorganize Tests

#### 2.1 NethackStageTests.cs Split
**Current Location**: `Tests/Stages/Stage-2/nethack/NethackStage/NethackStageTests.cs`

**Unit Tests → `Tests/Data/stages/nethack/Unit/NethackDungeonSequenceLoaderTests.cs`**
- `LoadDungeonSequence_WithValidJson_LoadsCorrectly()`
- `LoadDungeonSequence_WithValidJson_HasUniqueOwners()`
- `LoadDungeonSequence_WithValidJson_HasValidMaps()`
- `LoadDungeonSequence_WithObjectsProperty_ValidatesCorrectly()`

**Gameplay Tests → `Tests/Data/stages/nethack/NethackStageGameplayTests.cs`**
- `DungeonStage_ObjectInteractions_ReturnCorrectAlignment()` [RequireGodotRuntime]
- `DungeonStage_OwnerAlignedInteraction_GivesTwoPoints()` [RequireGodotRuntime]
- `DungeonStage_CrossAlignedInteraction_GivesOnePoint()` [RequireGodotRuntime]
- `DungeonSequenceRunner_ProcessesStagesCorrectly()`
- `DungeonSequenceRunner_CompletesAllStages()`

#### 2.2 Existing Unit Tests Migration
**Move from `Tests/Unit/` to feature-specific locations:**
- `Tests/Unit/Dungeon/AsciiDungeonSequenceRunnerTests.cs` → `Tests/Scripts/infrastructure/dungeon/Unit/`
- `Tests/Unit/Infrastructure/Dungeon/DreamweaverAffinityServiceTests.cs` → `Tests/Scripts/infrastructure/dungeon/Unit/`
- `Tests/Unit/Persistence/SaveLoadManagerTests.cs` → `Tests/Scripts/persistence/Unit/`
- `Tests/Unit/Infrastructure/SceneManagerTests.cs` → `Tests/Scripts/infrastructure/Unit/`

#### 2.3 Gameplay Tests Consolidation
**Move from mixed locations to main feature folders:**
- `Tests/Common/GameStateTests.cs` → `Tests/Scripts/common/Unit/`
- `Tests/Field/*.cs` → `Tests/Scripts/field/` (gameplay)
- `Tests/Dungeon/*.cs` → `Tests/Data/stages/nethack/` or keep as integration
- `Tests/EndToEnd/` → `Tests/Integration/`

### Phase 3: Namespace and Reference Updates
1. Update namespaces to reflect new locations
2. Update import statements
3. Verify test discovery in IDE
4. Update `.runsettings` if test paths are hardcoded

### Phase 4: Validation
1. Run all tests to verify functionality
2. Check test discovery count
3. Verify proper categorization with traits
4. Performance baseline comparison

## Detailed File Movements

| Current Path | New Path | Type | Action |
|---|---|---|---|
| `Tests/Stages/Stage-2/nethack/NethackStage/NethackStageTests.cs` | `Tests/Data/stages/nethack/Unit/NethackDungeonSequenceLoaderTests.cs` | Unit | Split & Move |
| | `Tests/Data/stages/nethack/NethackStageGameplayTests.cs` | Gameplay | Split & Move |
| `Tests/Unit/Dungeon/AsciiDungeonSequenceRunnerTests.cs` | `Tests/Scripts/infrastructure/dungeon/Unit/` | Unit | Move |
| `Tests/Unit/Infrastructure/Dungeon/DreamweaverAffinityServiceTests.cs` | `Tests/Scripts/infrastructure/dungeon/Unit/` | Unit | Move |
| `Tests/Common/GameStateTests.cs` | `Tests/Scripts/common/Unit/GameStateTests.cs` | Unit | Move |
| `Tests/Common/SceneManagerTests.cs` | `Tests/Scripts/infrastructure/Unit/SceneManagerTests.cs` | Unit | Move |
| `Tests/Field/*` | `Tests/Scripts/field/FieldGameplayTests.cs` | Gameplay | Consolidate |
| `Tests/Dungeon/*` | `Tests/Integration/` or feature-specific | Mixed | Review & Move |
| `Tests/EndToEnd/*` | `Tests/Integration/` | Gameplay | Move |
| `Tests/Unit/Persistence/*` | `Tests/Scripts/persistence/Unit/` | Unit | Move |
| `Tests/Visual/*` | `Tests/Visual/` (keep) | Gameplay | Keep |

## Namespace Updates

### Example: NethackStageTests Split

**Unit Test Namespace:**
```csharp
namespace OmegaSpiral.Tests.Data.Stages.Nethack.Unit
{
    // Pure logic tests, no Godot runtime
}
```

**Gameplay Test Namespace:**
```csharp
namespace OmegaSpiral.Tests.Data.Stages.Nethack
{
    // Godot runtime tests
}
```

## Benefits

✅ **Clarity**: Clear separation of unit vs. gameplay tests  
✅ **Performance**: Fast test runs for logic-only tests  
✅ **Navigation**: Easy to find tests for any feature  
✅ **Maintainability**: Tests located near source code  
✅ **Scalability**: Extensible to new features  
✅ **CI/CD**: Can run unit tests fast, gameplay tests separately  

## Implementation Timeline

| Phase | Tasks | Estimated Time |
|---|---|---|
| 1 | Directory structure creation | 15 min |
| 2.1 | NethackStageTests split | 30 min |
| 2.2 | Move existing unit tests | 20 min |
| 2.3 | Consolidate gameplay tests | 30 min |
| 3 | Namespace & reference updates | 45 min |
| 4 | Validation & testing | 30 min |
| **Total** | | **2.5 hours** |

## Rollback Plan

If issues arise:
1. Commit current changes to a branch
2. Keep original file backups
3. Test directory structure in isolation
4. Revert using git if needed

## Notes

- All test names should remain unchanged to preserve CI/CD references
- Categories and traits should be updated in test attributes
- Keep test data fixtures in a central `Tests/Common/` location
- Consider adding a `Tests/README.md` documenting the structure