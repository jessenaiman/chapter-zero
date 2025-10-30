# Ωmega Spiral Architecture: Three-Layer Separation of Concerns

## Overview
Three distinct layers with Single Responsibility Principle. Each handles one concern.

```
GameManager (simple sequencer)
    ↓
CinematicDirector (orchestrator, pure C#)
    ↓
SceneManager (Godot node, execution)
```

## Layer 1: GameManager
**Responsibility**: Sequence stages, await completion signals

- Simple loop: create stage → await → move to next
- Does NOT know about scenes, narrative, or Godot specifics
- Just orchestrates CinematicDirector instances sequentially

## Layer 2: CinematicDirector (Pure C#, NOT a Node)
**Responsibility**: Orchestrate narrative flow through scenes

- Receives parsed narrative script (JSON already loaded, not its job)
- Loops through scenes array sequentially
- For each scene:
  - Query NarrativeEngine for requirements
  - Gather any other needed data
  - Create stage-specific SceneManager instance with scene data
  - Await completion signal from SceneManager
- When all scenes done → signal GameManager

**Key**: Does NOT inherit from Node. Pure business logic.

## Layer 3: SceneManager (Godot Node)
**Responsibility**: Execute a single scene in Godot

- Inherits from `SceneManager` (Node base)
- Takes data from CinematicDirector
- "Light, camera, action" - loads .tscn files
- Manages scene lifecycle (UI, narrative playback, signals)
- Loads NarrativeHandler UI (e.g., GhostUi)
- When scene completes → signals CinematicDirector

## Stage 1 (Ghost) Concrete Example

```
GameManager
  ├─ Creates GhostCinematicDirector
  ├─ Awaits its ExecuteAsync()
  └─ [Moves to Stage 2 when done]

GhostCinematicDirector (loops 8 scenes)
  ├─ Scene 1 → Create GhostSceneManager → Await signal
  ├─ Scene 2 → Create GhostSceneManager → Await signal
  ├─ Scene 3 → Create GhostSceneManager → Await signal
  ├─ ... (8 times)
  └─ All done → Signal GameManager

Each GhostSceneManager instance:
  ├─ Load ghost_terminal.tscn
  ├─ Attach GhostUi (INarrativeHandler)
  ├─ Play scene via NarrativeEngine.PlayAsync()
  └─ Signal CinematicDirector when complete
```

## Key Principles

1. **GameManager**: Simple, generic sequencing (works for any stage type)
2. **CinematicDirector**: No Godot dependency, pure orchestration logic
3. **SceneManager**: Where C# meets Godot (nodes, signals, scene trees)

**No StageBase**: Tests may reference it, but it's not needed for this architecture.

## What CinematicDirector Does NOT Do

- ❌ Parse JSON (a loader does that beforehand)
- ❌ Inherit from Node (pure C#)
- ❌ Manage Godot scene trees directly
- ❌ Implement INarrativeHandler (UI does that)

## What Each Layer Knows

| Layer | Knows About | Does NOT Know About |
|-------|-------------|-------------------|
| GameManager | CinematicDirector lifecycle | Scenes, narrative, Godot details |
| CinematicDirector | Scenes array, scene sequencing | Godot nodes, .tscn files, UI |
| SceneManager | Godot nodes, scene execution | Multi-scene orchestration |

This is the correct, workable architecture for Stage 1 and beyond.
