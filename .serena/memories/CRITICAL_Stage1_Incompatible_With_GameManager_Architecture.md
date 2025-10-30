# ✅ RESOLVED: Stage 1 Ghost Terminal Architecture - Correct Pattern Identified

## The Correct Pattern (Inheritance + Composition)

```
GameManager
    ↓
    Instantiates → StageBase (abstract)
                       ↑
                       |
                  GhostStageController : StageBase
                       |
                       ├─→ Uses: GhostDataLoader : CinematicDirector<GhostTerminalCinematicPlan>
                       |   Purpose: Load ghost.yaml + cache plan
                       |
                       └─→ Instantiates: ghost_terminal.tscn
                           Root: NarrativeUi
                           Contains: GhostStageManager (coordinates narrative playback)
```

**KEY INSIGHT:** `CinematicDirector` is NOT an inheritance relationship for the stage controller. Instead:
- `CinematicDirector<TPlan>` = **Utility class** for YAML loading + caching
- `GhostDataLoader : CinematicDirector` = **Concrete YAML loader**
- `GhostStageController : StageBase` = **Stage controller** that USES GhostDataLoader

This matches exactly what NethackStageController does (which is the correct reference implementation).

---

## Current State vs Required State

| Aspect | Current (WRONG) | Required (RIGHT) |
|--------|-----------------|-----------------|
| Stage root node | `NarrativeUi` | `GhostStageController : StageBase` |
| Stage controller | `GhostStageManager : Node` | `GhostStageController : StageBase` |
| YAML loading | Direct in scene | `GhostDataLoader : CinematicDirector` |
| Method for GameManager | (none - not compatible) | `ExecuteStageAsync()` |
| Scene instantiation | Root of scene tree | Child of controller |
| Completion signal | Custom with scores | `StageBase.StageComplete()` |

---

## Required Changes (Minimal Refactoring)

### Step 1: Create GhostStageController
**File:** `source/stages/stage_1_ghost/GhostStageController.cs`

Following exact NethackStageController pattern:

```csharp
public sealed partial class GhostStageController : StageBase
{
    private readonly GhostDataLoader _DataLoader = new();
    private Node? _StageScene;
    private TaskCompletionSource<bool>? _StageCompletion;

    public override int StageId => 1;

    public override async Task ExecuteStageAsync()
    {
        try
        {
            // 1. Load the YAML narrative plan
            var plan = _DataLoader.GetPlan();
            GD.Print($"[GhostStageController] Loaded: '{plan.Script.Title}'");

            // 2. Load and instantiate the scene
            var scenePath = "res://source/stages/stage_1_ghost/ghost_terminal.tscn";
            var scene = GD.Load<PackedScene>(scenePath);
            _StageScene = scene.Instantiate();

            // 3. Setup completion tracking
            _StageCompletion = new TaskCompletionSource<bool>();
            _StageScene.TreeExiting += OnSceneExiting;

            // 4. Add to tree (triggers _Ready)
            AddChild(_StageScene);
            GD.Print("[GhostStageController] Scene instantiated and added to tree");

            // 5. Wait for completion
            await _StageCompletion.Task.ConfigureAwait(false);
            GD.Print("[GhostStageController] Stage completed");

            // 6. Signal completion to GameManager
            EmitStageComplete();
        }
        catch (Exception ex)
        {
            GD.PrintErr($"[GhostStageController] Error: {ex.Message}\n{ex.StackTrace}");
            EmitStageComplete();
        }
    }

    private void OnSceneExiting()
    {
        GD.Print("[GhostStageController] Stage scene is exiting scene tree");
        _StageCompletion?.TrySetResult(true);
    }
}
```

### Step 2: No Changes Needed To:
- ✅ `ghost_terminal.tscn` - Root stays as `NarrativeUi`
- ✅ `GhostStageManager` - Stays as child of `NarrativeUi`, coordinates scene
- ✅ `GhostDataLoader` - Already correctly extends `CinematicDirector`
- ✅ `NarrativeEngine` - Already correctly orchestrates playback
- ✅ All narrative UI/audio components - No changes

### Step 3: Update Project Configuration
In `project.godot` or wherever stages are registered:
```
GameManager.StageScenes[0] = res://source/stages/stage_1_ghost/GhostStageController.tscn
                             (NEW scene with GhostStageController as root)
```

---

## TDD Tests Impact

✅ **NO CHANGES NEEDED to tests!**

Our 3 TDD tests load `ghost_terminal.tscn` directly:
```csharp
_Runner = ISceneRunner.Load("res://source/stages/stage_1_ghost/ghost_terminal.tscn");
```

They don't depend on GameManager or GhostStageController, so they still work perfectly.

---

## Implementation Plan

1. Create `GhostStageController.cs` extending `StageBase`
2. Create `GhostStageController.tscn` scene with controller as root node
3. Update project configuration to load controller scene as Stage 1
4. Verify: `dotnet build` succeeds
5. Verify: TDD tests still pass
6. Verify: GameManager can load Stage 1 correctly
7. Verify: Can progress Stage 1 → Stage 2 (GameManager flow)

---

## Why This Works

- **Separation of concerns:** Controller manages lifecycle, Scene manages presentation
- **Composition over inheritance:** Controller USES DataLoader, doesn't extend it
- **GameManager compatibility:** Controller provides required `ExecuteStageAsync()`
- **Scene independence:** Scene can still be tested directly without controller
- **Consistent pattern:** Matches NethackStageController exactly

---

## Confirmation

This is the CORRECT architecture. No undo needed. Just implement GhostStageController following the NethackStageController template.
