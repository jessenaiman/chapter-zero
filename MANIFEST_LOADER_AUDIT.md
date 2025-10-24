# Manifest Loader Architecture Audit

**Date**: October 23, 2025
**Branch**: feature/issue-2-ascii-room-renderer

---

## CURRENT STATE

### Manifest Loading Files

| File | Type | Purpose | Status |
|------|------|---------|--------|
| `source/infrastructure/BaseManifestLoader.cs` | Abstract Base | Generic JSON→C# loader for any manifest type | ✅ **IN USE** |
| `source/infrastructure/ManifestLoader.cs` | Class | Loads `IReadOnlyList<ManifestStage>` (top-level game manifest) | ✅ **IN USE** |
| `source/infrastructure/StageManifestLoader.cs` | Class | Loads `StageManifest` (per-stage scene structure) | ✅ **IN USE** |
| `source/infrastructure/NarrativeDataLoader.cs` | Class | Loads narrative JSON using System.Text.Json (NOT manifest-based) | ✅ **IN USE** |
| `source/infrastructure/SceneFlowLoader.cs` | Class | Loads `StageSceneFlow` (alternative scene sequencing) | ⚠️ **UNCLEAR** |

### Data Model Files

| File | Type | Purpose | Used By |
|------|------|---------|---------|
| `source/infrastructure/StageManifest.cs` | Data Class | Defines stage structure (scenes, order, transitions) | `StageManifestLoader`, `StageController` |
| `source/infrastructure/ManifestLoader.cs` | Data Class | Defines `ManifestStage` (stage metadata) | `ManifestLoader`, `MainMenu.cs` |
| `source/infrastructure/SceneFlowLoader.cs` | Data Class | Defines `SceneFlowEntry`, `StageSceneFlow` | `SceneFlowLoader` (unclear usage) |

### Base Classes & Interfaces

| File | Type | Purpose | Used By |
|------|------|---------|---------|
| `source/infrastructure/StageController.cs` | Abstract Base | Orchestrates stage scene flow, affinity tracking | All stage controllers |
| `source/infrastructure/SceneBase.cs` | Abstract Base | Base for beat/scene scripts | All stage beat/scene classes |
| `source/infrastructure/StageManagement/IStageManager.cs` | Interface | Defines stage transition contract | Stage managers (1-6) |
| `source/infrastructure/StageManagement/StageManagerBase.cs` | Abstract Base | Implements `IStageManager`, uses manifest loading | All stage managers (1-6) |

---

## CURRENT USAGE PATTERNS

### Pattern 1: StageController (2 usages)
**File**: `source/infrastructure/StageController.cs` (line 296)
```csharp
var loader = new StageManifestLoader();
StageManifest = loader.LoadManifest(StageManifestPath);
```
**Impact**: All stage controllers (Stage1, Stage2, Stage4) inherit this, so loaders are instantiated once per stage in `_Ready()`.

### Pattern 2: StageManagerBase (1 usage)
**File**: `source/infrastructure/StageManagement/StageManagerBase.cs` (line 84)
```csharp
var loader = new StageManifestLoader();
StageManifest? manifest = loader.LoadManifest(StageManifestPath);
```
**Impact**: All stage managers (1-6) inherit this. Loaders instantiated when transitioning between stages.

### Pattern 3: MainMenu (1 usage)
**File**: `source/stages/stage_0_start/MainMenu.cs` (lines 32, 66, 206)
```csharp
private readonly ManifestLoader _manifestLoader = new();
var stages = _manifestLoader.LoadManifest(StageManifestPath);
var stage = _manifestLoader.GetStage(stageId);
```
**Impact**: Main menu instantiates and keeps reference to loader for querying stages.

### Pattern 4: Narrative Data (2 usages - DIFFERENT PURPOSE)
**Files**:
- `source/stages/stage_2/NethackInterludeSequence.cs` (line 61)
- `source/stages/stage_2/NethackExploration.cs` (line 56)

```csharp
var loader = new NarrativeDataLoader();
_narrativeData = loader.LoadNarrativeData<Stage2NarrativeData>(NarrativeJsonPath);
```
**Impact**: Loads narrative content (not manifests). Uses `System.Text.Json`, not Godot JSON. ✅ **DIFFERENT CONCERN - KEEP SEPARATE**

---

## ISSUES IDENTIFIED

### 🔴 Issue 1: Repeated Instantiation (Inefficient)
- **Location**: `StageController._Ready()` (line 296) and `StageManagerBase.ResolveManifestFirstScene()` (line 84)
- **Problem**: New `StageManifestLoader` instance created every time a stage loads
- **Impact**: Minor (loaders are lightweight), but violates DRY principle
- **Solution**: Extract to base class or use static/dependency injection

### 🟡 Issue 2: No Interface Contract
- **Location**: `ManifestLoader`, `StageManifestLoader` are standalone classes with no shared interface
- **Problem**: Hard to enforce consistent manifest loading behavior across codebase
- **Impact**: Each loader has its own error handling, logging, and patterns
- **Solution**: Create `IManifestAware` interface that all manifest-loading nodes implement

### 🟡 Issue 3: MainMenu Duplicates Loader Logic
- **Location**: `source/stages/stage_0_start/MainMenu.cs` (line 32)
- **Problem**: MainMenu creates `new ManifestLoader()` inline, doesn't inherit from base
- **Impact**: MainMenu logic is separated from stage controllers; hard to debug consistent behavior
- **Solution**: Make MainMenu inherit from `ManifestAwareNode<IReadOnlyList<ManifestStage>>`

### 🟡 Issue 4: SceneBase Has No Manifest Loading
- **Location**: `source/infrastructure/SceneBase.cs`
- **Problem**: Beat/scene scripts have `StageManifestPath` property but no way to load manifests
- **Impact**: Can't debug individual scenes in isolation; must run through full stage
- **Solution**: Implement `IManifestAware` in `SceneBase`, add `LoadManifest()` method

### 🟡 Issue 5: SceneFlowLoader Purpose Unclear
- **Location**: `source/infrastructure/SceneFlowLoader.cs`
- **Problem**: Exists but unclear if it's in use; similar to `StageManifest` but different structure
- **Impact**: Potential dead code or competing manifest standards
- **Action**: User to clarify if this should be consolidated with `StageManifest`

### 🟢 Non-Issue: NarrativeDataLoader Separate
- **Location**: `source/infrastructure/NarrativeDataLoader.cs`
- **Status**: ✅ Correctly separate (uses `System.Text.Json`, not Godot JSON)
- **Reason**: Loads narrative scene content, not stage manifests
- **Recommendation**: Keep separate, but could be refactored to `ManifestAwareNode<T>` pattern for consistency

---

## FILES READY FOR DELETION

After refactor, these will be **REDUNDANT**:

1. **`source/infrastructure/ManifestLoader.cs`** (129 lines)
   - Replaced by: `ManifestAwareNode<IReadOnlyList<ManifestStage>>`
   - Used by: `MainMenu.cs`
   - Action: DELETE after MainMenu refactor

2. **`source/infrastructure/StageManifestLoader.cs`** (70 lines)
   - Replaced by: `ManifestAwareNode<StageManifest>`
   - Used by: `StageController._Ready()` (line 296), `StageManagerBase.ResolveManifestFirstScene()` (line 84)
   - Action: DELETE after refactor

---

## FILES REQUIRING CHANGES

### Primary Changes
1. **Create**: `source/infrastructure/IManifestAware.cs` (new interface)
2. **Create**: `source/infrastructure/ManifestAwareNode.cs` (new generic base class)
3. **Refactor**: `source/infrastructure/StageController.cs` (inherit from `ManifestAwareNode<StageManifest>`)
4. **Refactor**: `source/infrastructure/SceneBase.cs` (implement `IManifestAware`)
5. **Refactor**: `source/stages/stage_0_start/MainMenu.cs` (inherit from `ManifestAwareNode<IReadOnlyList<ManifestStage>>`)

### Secondary Changes (Automatic)
- `source/infrastructure/StageManagement/StageManagerBase.cs` - Will use base class manifest loading if extracted to shared method
- All stage controllers (`Stage1Controller`, `NethackHub`, `Stage4Controller`) - Inherit new behavior from `StageController`
- All beat/scene classes - Can optionally use `SceneBase.LoadManifest()` for debugging

---

## DEPENDENCY CHAIN

```
BaseManifestLoader<T> (abstract)
    ↓
    ├─→ ManifestLoader (for IReadOnlyList<ManifestStage>)
    │   └─→ MainMenu.cs [WILL BE REMOVED]
    │
    └─→ StageManifestLoader (for StageManifest)
        ├─→ StageController._Ready() [WILL BE REMOVED]
        └─→ StageManagerBase.ResolveManifestFirstScene() [WILL BE REMOVED]

NEW ARCHITECTURE:
ManifestAwareNode<T> : Node, IManifestAware
    ↓
    ├─→ StageController : ManifestAwareNode<StageManifest>
    │   ├─→ Stage1Controller
    │   ├─→ NethackHub (Stage2)
    │   └─→ Stage4Controller
    │
    └─→ MainMenu : ManifestAwareNode<IReadOnlyList<ManifestStage>>

SceneBase : Control, IManifestAware [NEW]
    ├─→ All beat/scene classes
    └─→ Can load stage manifest for standalone debugging
```

---

## IMPLEMENTATION SAFETY CHECKLIST

- [ ] Verify `BaseManifestLoader<T>` is working (already in use)
- [ ] Create `IManifestAware` interface with standard contract
- [ ] Create `ManifestAwareNode<T>` generic base using `BaseManifestLoader<T>`
- [ ] Refactor `StageController` to inherit `ManifestAwareNode<StageManifest>`
- [ ] Refactor `SceneBase` to implement `IManifestAware`
- [ ] Refactor `MainMenu` to inherit `ManifestAwareNode<IReadOnlyList<ManifestStage>>`
- [ ] Run `dotnet build` to verify no compilation errors
- [ ] Run `dotnet test` to verify all tests pass
- [ ] Delete `ManifestLoader.cs`
- [ ] Delete `StageManifestLoader.cs`
- [ ] KEEP `NarrativeDataLoader.cs` (different concern)
- [ ] CLARIFY: `SceneFlowLoader.cs` - is this still used?

---

## RECOMMENDATIONS

1. **Extract Stage Manifest Loading to Base**: Both `StageController` and `StageManagerBase` instantiate `StageManifestLoader()`. Extract to a static method or make loaders singleton.

2. **Enforce Interface Contract**: All manifest-aware code must implement `IManifestAware`. This makes it obvious where loading happens and ensures consistent error handling.

3. **Enable Scene Debugging**: Implement `IManifestAware` in `SceneBase` so beat/scene scripts can load their stage manifest for offline testing.

4. **Clarify SceneFlowLoader**: Determine if `SceneFlowLoader.cs` is active code or legacy. If active, consider consolidating with `StageManifest`.

5. **Document Manifest Path Strategy**: Ensure all stage controllers define `StageManifestPath` consistently (e.g., `"res://source/stages/stage_X/stage_manifest.json"`).

---

## AUDIT COMPLETE ✅

**No show-stoppers found.** Architecture is sound; refactor is safe to proceed.

Files to review: `BaseManifestLoader.cs`, `ManifestLoader.cs`, `StageManifestLoader.cs`
Files to clarify: `SceneFlowLoader.cs`
Files to keep: `NarrativeDataLoader.cs`
