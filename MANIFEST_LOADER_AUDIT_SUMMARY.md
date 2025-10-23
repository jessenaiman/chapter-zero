# Manifest Loader Architecture - Pre-Refactor Review Summary

## 📋 QUICK REFERENCE

### ✅ Files In Use (DO NOT DELETE)
- `src/infrastructure/BaseManifestLoader.cs` - Core generic loader
- `src/infrastructure/StageManifest.cs` - Stage data model
- `src/infrastructure/NarrativeDataLoader.cs` - Narrative content (different concern)
- `src/infrastructure/SceneBase.cs` - Beat/scene base class
- `src/infrastructure/SceneFlowLoader.cs` - **NEEDS CLARIFICATION**

### ⚠️ Files To Delete (After Refactor)
1. `src/infrastructure/ManifestLoader.cs` (129 lines)
   - Used by: `MainMenu.cs` line 32, 66, 206
   - Replaced by: `MainMenu : ManifestAwareNode<IReadOnlyList<ManifestStage>>`

2. `src/infrastructure/StageManifestLoader.cs` (70 lines)
   - Used by: 
     - `StageController.cs` line 296 in `_Ready()`
     - `StageManagerBase.cs` line 84 in `ResolveManifestFirstScene()`
   - Replaced by: `ManifestAwareNode<StageManifest>` pattern

---

## 🎯 CURRENT INSTANTIATION POINTS (WILL ELIMINATE)

### 1. StageController (line 296)
```csharp
// CURRENT
var loader = new StageManifestLoader();
StageManifest = loader.LoadManifest(StageManifestPath);

// AFTER
// Call inherited LoadManifest() from ManifestAwareNode<StageManifest>
// Access via: this.LoadedData or protected property
```
**Affected**: All stage controllers (Stage1, Stage2, Stage4)

### 2. StageManagerBase (line 84)
```csharp
// CURRENT
var loader = new StageManifestLoader();
StageManifest? manifest = loader.LoadManifest(StageManifestPath);

// AFTER
// Option A: Inherit from ManifestAwareNode<StageManifest>
// Option B: Create protected helper method that uses ManifestAwareNode internally
// Option C: Dependency inject a shared loader
```
**Affected**: All stage managers (Stage1-6Manager)

### 3. MainMenu (lines 32, 66, 206)
```csharp
// CURRENT
private readonly ManifestLoader _manifestLoader = new();
var stages = _manifestLoader.LoadManifest(StageManifestPath);
var stage = _manifestLoader.GetStage(stageId);

// AFTER
// Inherit from ManifestAwareNode<IReadOnlyList<ManifestStage>>
// Access via: this.LoadedData (the stages list)
// Remove GetStage() method, access directly from LoadedData
```
**Affected**: `MainMenu.cs` line 32, 66, 206

---

## 🔍 DUPLICATE PATTERNS FOUND

### Pattern A: Loader in _Ready()
```csharp
// StageController.cs line 296
public override async void _Ready()
{
    try
    {
        var loader = new StageManifestLoader();  // ← DUPLICATE PATTERN
        StageManifest = loader.LoadManifest(StageManifestPath);
        // ...
    }
}
```

### Pattern B: Loader as Field
```csharp
// MainMenu.cs line 32
private readonly ManifestLoader _manifestLoader = new();  // ← DUPLICATE PATTERN

public override void _Ready()
{
    var stages = _manifestLoader.LoadManifest(StageManifestPath);  // ← DUPLICATE PATTERN
}
```

### Pattern C: Loader in Method
```csharp
// StageManagerBase.cs line 84
protected string? ResolveManifestFirstScene()
{
    var loader = new StageManifestLoader();  // ← DUPLICATE PATTERN
    StageManifest? manifest = loader.LoadManifest(StageManifestPath);
    return manifest?.GetFirstScene()?.SceneFile;
}
```

**ALL THREE PATTERNS WILL BE UNIFIED** to use `ManifestAwareNode<T>`.

---

## 🎪 INHERITANCE AFTER REFACTOR

```
NEW INTERFACES/BASES
├── IManifestAware [NEW INTERFACE]
│   ├── ManifestAwareNode<T> : Node [NEW BASE CLASS]
│   └── SceneBase (will implement) [REFACTORED]
│
EXISTING CLASSES THAT INHERIT
├── StageController : ManifestAwareNode<StageManifest> [REFACTORED]
│   ├── Stage1Controller
│   ├── NethackHub (Stage2)
│   └── Stage4Controller
│
├── MainMenu : ManifestAwareNode<IReadOnlyList<ManifestStage>> [REFACTORED]
│
├── SceneBase : Control, IManifestAware [REFACTORED]
│   ├── NethackInterludeSequence
│   ├── NethackExploration
│   ├── BootSequence
│   └── (all other beat/scene classes)
│
EXISTING MANAGERS (NEED DISCUSSION)
└── StageManagerBase ← Currently instantiates StageManifestLoader in method
    ├── Stage1Manager
    ├── Stage2Manager
    ├── Stage3Manager
    ├── Stage4Manager
    ├── Stage5Manager
    └── Stage6Manager
```

**QUESTION FOR DISCUSSION**: Should `StageManagerBase` inherit from `ManifestAwareNode<StageManifest>`, or should we extract manifest loading to a shared helper method?

---

## ⚠️ POTENTIAL EDGE CASES

1. **SceneFlowLoader.cs** - Exists but unclear usage. May conflict with `StageManifest`.
   - **Question**: Is this still used, or is it legacy?
   - **Decision**: User to clarify

2. **NarrativeDataLoader.cs** - Uses `System.Text.Json`, not Godot JSON
   - **Status**: ✅ Correctly separate (different concern)
   - **Future**: Could be refactored to `ManifestAwareNode<T>` pattern for consistency, but not required

3. **MainMenu.cs Methods** - Uses `_manifestLoader.GetStage(stageId)` helper
   - **Change Required**: Remove this method; access stages directly from `LoadedData`
   - **Impact**: Lines 206 need update

4. **Manifest Path Properties** - Each controller defines its own string constant
   - **Change Required**: Ensure all follow same pattern
   - **Example**: `"res://source/stages/stage_X/stage_manifest.json"`

---

## 🚦 GO/NO-GO CHECKLIST

- ✅ **BaseManifestLoader.cs works**: Already in production
- ✅ **No circular dependencies**: All loaders are standalone
- ✅ **Affinity tracking separate**: Won't be affected by refactor
- ✅ **Scene transitions separate**: Won't be affected by refactor
- ⚠️ **StageManagerBase.cs**: Uses loader in method; needs discussion on inheritance
- ❓ **SceneFlowLoader.cs**: Unclear if active; needs user clarification
- ✅ **NarrativeDataLoader.cs**: Correctly separate; keep as-is

---

## 📝 FINAL NOTES

**Zero blocker issues found.**

The current architecture is solid. The refactor will:
1. Eliminate code duplication (3 different instantiation patterns → 1 interface)
2. Enforce consistency (all manifest-aware classes use same pattern)
3. Enable debugging (SceneBase can load manifests for standalone testing)
4. Reduce confusion (clear class hierarchy: IManifestAware → ManifestAwareNode<T>)

**Ready to proceed with confidence.**
