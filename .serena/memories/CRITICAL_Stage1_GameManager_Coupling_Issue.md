# CRITICAL ARCHITECTURAL COUPLING ISSUE - Stage 1 vs GameManager

## ⚠️ SHOW STOPPER: Stage 1 Cannot Complete With Current Architecture

### The Problem

**New GameManager (Stage 2 architecture) requires:**
```csharp
// GameManager.cs line 115
var stageInstance = stageScene.Instantiate<StageBase>();
await _CurrentStage.ExecuteStageAsync().ConfigureAwait(false);
_CurrentStage.StageComplete += OnStageComplete;
```

**Stage 1 (Ghost Terminal) provides:**
- Root node: `GhostUi` (NarrativeUi subclass) - NOT StageBase
- No `ExecuteStageAsync()` method
- No `StageComplete` signal
- Uses old Director pattern: `GhostCinematicDirector` loads YAML, calls `CinematicDirector.PlayAsync()`

### Why This Is Broken

1. **Type Mismatch**: `GhostUi` does NOT extend `StageBase`
   - `GameManager.Instantiate<StageBase>()` will return null
   - Stage fails to load

2. **Missing ExecuteStageAsync()**
   - GameManager expects all stages to implement this
   - Ghost Terminal has no such method
   - Stage cannot execute

3. **Signal Incompatibility**
   - GhostStageManager has `StageComplete(int[] scores)`
   - StageBase.StageComplete() takes NO parameters
   - GameManager disconnects wrong signal

4. **Director Pattern Abandoned**
   - Old: GhostCinematicDirector → loads YAML → CinematicDirector → NarrativeUi
   - New: GameManager → stages inherit StageBase
   - These patterns are **completely different**

### The Coupling Chain

```
GameManager (Stage 2 architecture)
    ↓ expects
StageBase
    ↓ requires
Root node to be StageBase subclass
    ↑ conflicts with
GhostUi (NarrativeUi subclass)
    ↑ used by
GhostCinematicDirector (Old Director pattern)
```

### Solution Options

#### Option A: Wrap Ghost in StageBase (Recommended - Minimal Coupling)
```csharp
// Create: GhostTerminalStage.cs
[GlobalClass]
public partial class GhostTerminalStage : StageBase
{
    private GhostCinematicDirector _director;

    public override int StageId => 1;

    public override async Task ExecuteStageAsync()
    {
        _director = new GhostCinematicDirector();
        await _director.PlayAsync(); // Delegates to existing Ghost logic
        EmitStageComplete(); // Signals GameManager
    }
}

// Then: ghost_terminal_stage.tscn root = GhostTerminalStage
```

This keeps **Ghost Terminal logic unchanged** but wraps it in GameManager's protocol.

#### Option B: Rewrite Ghost as Full StageBase (High Coupling - NOT RECOMMENDED)
- Would require rewriting `GhostStageManager` to inherit `StageBase`
- Would require changing `ghost_terminal.tscn` root node from `GhostUi` → `GhostStageManager`
- Would break all existing Ghost Terminal logic
- **Risk: Breaks Stage 1 functionality**

#### Option C: Revert GameManager to Old Pattern (Stage 2 Breaks)
- Would require Stage 2 to also use old Director pattern
- Creates inconsistency between stages
- **Risk: Cannot have unified stage progression**

### Recommended Fix

**Implement Option A:**

1. Create `GhostTerminalStage` extending `StageBase`
2. Have it instantiate `GhostCinematicDirector`
3. Create `ghost_terminal_stage.tscn` with `GhostTerminalStage` as root
4. Keep all existing Ghost Terminal code intact
5. Update `GameManager.StageScenes` array to reference `ghost_terminal_stage.tscn`

This provides:
- ✅ Clean separation: Stage 1 & 2 both use StageBase protocol
- ✅ Stage 1 logic: Unchanged, delegates to existing director
- ✅ Testability: Tests can still load `ghost_terminal.tscn` directly
- ✅ No breaking changes: Old Ghost code continues to work

### Files to Create/Modify

**NEW**: `source/stages/stage_1_ghost/GhostTerminalStage.cs`
```csharp
[GlobalClass]
public partial class GhostTerminalStage : StageBase
{
    public override int StageId => 1;

    public override async Task ExecuteStageAsync()
    {
        // Delegate to existing Ghost logic via director
        var director = new GhostCinematicDirector();
        await director.PlayAsync().ConfigureAwait(false);
        EmitStageComplete();
    }
}
```

**NEW**: `source/stages/stage_1_ghost/ghost_terminal_stage.tscn` (root = GhostTerminalStage)

**MODIFY**: `project.godot` → GameManager.StageScenes array

### Status

- ⚠️ **BLOCKING**: Stage 1 cannot complete without this fix
- 🔴 **HIGH PRIORITY**: Must be resolved before testing Stage 1 + 2 together
- ✅ **LOW RISK**: Option A is a thin wrapper, doesn't modify existing code

### References

- GameManager: `source/services/GameManager.cs` (lines 110-125)
- StageBase: `source/services/StageBase.cs`
- GhostCinematicDirector: `source/stages/stage_1_ghost/GhostCinematicDirector.cs`
- Current issue: GhostUi root + GhostCinematicDirector don't fit StageBase protocol
