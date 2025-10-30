# CinematicDirector Architecture - Standardized Patterns

## Overview
The `CinematicDirector<TPlan>` base class implements a standardized pattern for all 5 game stages. It supports two architectural patterns: **Pure Narrative** and **Hybrid Stages**.

## Core Architecture

### Non-Generic Interface
```csharp
public interface ICinematicDirector
{
    Task RunStageAsync();
}
```
- Purpose: Allows `GameManager` to work with any stage director regardless of generic `TPlan` type
- Solves type covariance issues when storing directors in collections

### Generic Base Class
```csharp
public abstract class CinematicDirector<TPlan> : ICinematicDirector where TPlan : StoryPlan
{
    protected TPlan? Plan { get; private set; }

    // Virtual method - allows stage-specific overrides
    public virtual async Task RunStageAsync() { ... }

    // Helper methods
    protected async Task RunStageWithGameplayAsync(string gameplayScenePath) { ... }
    protected virtual async Task RunGameplaySceneAsync(string scenePath) { ... }
}
```

## Pattern 1: Pure Narrative Stages

### When to Use
Stages that display only narrative sequences without additional gameplay/exploration phases.

**Examples:** Stage 1 (Ghost), Stage 2 (Nethack)

### Implementation Pattern
```csharp
public sealed class GhostCinematicDirector : CinematicDirector<GhostCinematicPlan>
{
    /// <summary>
    /// This is a PURE NARRATIVE stage.
    /// Uses base RunStageAsync() - no override needed.
    /// </summary>

    protected override string GetDataPath()
    {
        return "res://source/frontend/stages/stage_1_ghost/ghost.json";
    }

    protected override GhostCinematicPlan BuildPlan(StoryScriptRoot script)
    {
        return new GhostCinematicPlan(script);
    }

    protected override SceneManager CreateSceneManager(StoryScriptElement scene, object data)
    {
        return new SceneManager(scene, data);
    }
    // No RunStageAsync override - uses base implementation
}
```

### How It Works
1. Base `RunStageAsync()` loads JSON from `GetDataPath()`
2. Builds plan using `BuildPlan()`
3. Iterates through scenes and displays each sequentially
4. Uses `CreateSceneManager()` for scene-specific UI/behavior
5. Stage completes when all scenes are done

## Pattern 2: Hybrid Stages

### When to Use
Stages that combine narrative sequences with interactive gameplay/exploration phases.

**Examples:** Stage 3 (Town), Stage 4 (Party Selection), Stage 5 (Escape)

### Implementation Pattern
```csharp
public sealed class TownCinematicDirector : CinematicDirector<TownCinematicPlan>
{
    /// <summary>
    /// This is a HYBRID stage combining narrative + gameplay.
    /// OVERRIDE RunStageAsync() to use two-phase pattern.
    /// </summary>

    public override async Task RunStageAsync()
    {
        // HYBRID PATTERN: Run narrative first, then gameplay
        await this.RunStageWithGameplayAsync("res://source/frontend/stages/stage_3_town/town_main_start.tscn");
    }

    protected override string GetDataPath()
    {
        return "res://source/frontend/stages/stage_3_town/town_stage.json";
    }

    protected override TownCinematicPlan BuildPlan(StoryScriptRoot script)
    {
        return new TownCinematicPlan(script);
    }

    protected override SceneManager CreateSceneManager(StoryScriptElement scene, object data)
    {
        return new SceneManager(scene, data);
    }
}
```

### How It Works
1. Override `RunStageAsync()` to call `RunStageWithGameplayAsync(scenePath)`
2. Phase 1 (Narrative): Runs base narrative logic (load JSON, iterate scenes)
3. Phase 2 (Gameplay): Calls `RunGameplaySceneAsync()` to load gameplay scene
4. Gameplay scene loaded as child of root node
5. Stage completes when both phases finish

## Key Methods

### Abstract Methods (Required in all directors)
- `protected abstract string GetDataPath()` - Returns path to JSON story file
- `protected abstract TPlan BuildPlan(StoryScriptRoot script)` - Builds stage plan
- `protected abstract SceneManager CreateSceneManager(...)` - Creates scene manager

### Virtual Methods (Optional)
- `public virtual async Task RunStageAsync()` - Override in hybrid stages only
- `protected virtual async Task RunGameplaySceneAsync(string scenePath)` - Can be overridden for custom loading

### Helper Methods (Use in hybrid stage RunStageAsync overrides)
- `protected async Task RunStageWithGameplayAsync(string gameplayScenePath)` - Combines narrative + gameplay

## Stage Implementations

### Stage 1: Ghost Terminal (Pure Narrative)
- Director: `GhostCinematicDirector`
- Plan: `GhostCinematicPlan`
- JSON: `ghost.json`
- Pattern: Pure narrative only
- Does NOT override `RunStageAsync()`

### Stage 2: Nethack (Pure Narrative)
- Director: `NethackCinematicDirector`
- Plan: `NethackCinematicPlan`
- JSON: `nethack.json`
- Pattern: Pure narrative only
- Does NOT override `RunStageAsync()`

### Stage 3: Town (Hybrid)
- Director: `TownCinematicDirector`
- Plan: `TownCinematicPlan`
- JSON: `town_stage.json`
- Gameplay Scene: `town_main_start.tscn`
- Pattern: Narrative intro → Town exploration
- OVERRIDES `RunStageAsync()` to use hybrid pattern

### Stage 4: Party Selection (Hybrid)
- Director: `PartySelectionCinematicDirector`
- Plan: `PartySelectionCinematicPlan`
- JSON: `stage4.json`
- Gameplay Scene: `party_selection_ui.tscn` (TODO: implement)
- Pattern: Narrative intro → Character creation UI
- OVERRIDES `RunStageAsync()` to use hybrid pattern

### Stage 5: Escape (Hybrid)
- Director: `EscapeCinematicDirector` (File: `EscapeCinematicController.cs`)
- Plan: `EscapeCinematicPlan`
- JSON: `stage_5.json`
- Gameplay Scene: `escape_hub_start.tscn`
- Pattern: Narrative sequences → Endgame scene
- OVERRIDES `RunStageAsync()` to use hybrid pattern

## Usage in GameManager

```csharp
private ICinematicDirector? GetDirectorForStage(string stageId)
{
    return stageId switch
    {
        "stage_1_ghost" => new GhostCinematicDirector(),
        "stage_2_nethack" => new NethackCinematicDirector(),
        "stage_3_town" => new TownCinematicDirector(),
        "stage_4_party_selection" => new PartySelectionCinematicDirector(),
        "stage_5_escape" => new EscapeCinematicDirector(),
        _ => null,
    };
}

// In game loop:
var director = GetDirectorForStage(currentStageId);
await director.RunStageAsync(); // Automatically handles both patterns
```

## Adding Future Stages

### For Pure Narrative Stages:
1. Create new director inheriting `CinematicDirector<YourPlan>`
2. Implement required abstract methods
3. Do NOT override `RunStageAsync()`
4. Follow Ghost/Nethack pattern

### For Hybrid Stages:
1. Create new director inheriting `CinematicDirector<YourPlan>`
2. Implement required abstract methods
3. Override `RunStageAsync()` to call `RunStageWithGameplayAsync(scenePath)`
4. Follow Town/PartySelection/Escape pattern

## Architecture Decisions

### Why Method Overloading Instead of Inheritance?
- **Avoid class explosion**: No need for `HybridCinematicDirector` base class
- **Clear intent**: Overriding `RunStageAsync()` immediately signals "this is a hybrid stage"
- **Flexible**: Each stage controls its own narrative-to-gameplay transition
- **Maintainable**: All stages use same base class, same pattern

### Why Non-Generic Interface?
- `ICinematicDirector` solves type covariance problems in `GameManager`
- Allows storing different director types in same collection
- `GameManager` doesn't need to know about `TPlan` generics

### Why Virtual RunStageAsync()?
- Pure narrative stages use base implementation
- Hybrid stages override with minimal code (one method call)
- Clear separation of concerns
