# Scene Loading Architecture Analysis

## Current Flow (3-Layer System)

### Layer 1: GameManager (C#, Orchestrator)
- **Role**: Top-level game state machine
- **Entry Point**: `MainMenu` calls `GameManager.StartGame(0)` (fire-and-forget, non-blocking)
- **Flow**: 
  - Iterates through 5 stages (stage_1_ghost through stage_5_escape)
  - For each stage, instantiates a `CinematicDirector` subclass
  - Awaits `RunStageAsync()` completion
  - Tracks dreamweaver points across all stages
- **Current**: Fire-and-forget async pattern (good for GDScript interop)

### Layer 2: CinematicDirector (C#, Stage Orchestrator)
- **Three Execution Patterns**:
  1. **Pure Narrative** (Ghost, Nethack): Runs JSON story script scenes sequentially
  2. **Narrative + UI** (Ghost): Loads a .tscn scene first, then runs narrative
  3. **Hybrid** (Town, PartySelection, Escape): Narrative sequences + interactive gameplay scenes

- **Per-Stage Pattern**:
  - Loads JSON story script from `source/backend/data/[stage_name]/script.json`
  - Builds a plan (cached)
  - For each scene in plan:
    - Creates `SceneManager` instance
    - Awaits `RunSceneAsync()` completion
    - Collects results

### Layer 3: SceneManager (C#, Scene Runner)
- **Role**: Loads .tscn, finds/creates UI handler, runs scene to completion
- **Flow**:
  1. Gets main SceneTree
  2. Searches for handler node (prefers: NarrativeUi > GhostUi > creates default)
  3. Calls `IStoryHandler.DisplaySceneAsync(sceneData, additionalData)`
  4. Returns SceneResult when handler signals done

### Layer 4 (Infrastructure): SceneLoader (GDScript Autoload)
- **Role**: Background scene loading utility
- **Used By**: Optional - for smooth transitions with loading screens
- **Current**: 
  - Configured to use `res://source/frontend/ui/menu/scenes/loading_screen/loading_screen.tscn`
  - Async-loads .tscn files in background
  - Can display loading screen while loading
  - NOT currently wired to GameManager/CinematicDirector

---

## Current Problems

1. **UI Scene Loading Not Wired**: CinematicDirector has `LoadUiSceneAsync()` method but stages don't use it
2. **SceneLoader Not Used**: Maaacks menu has SceneLoader but GameManager bypasses it
3. **Duplicate Loading Logic**: Menu system has its own scene loader, game has separate path
4. **Hard to Maintain**: Three different systems managing scene transitions

---

## Simplification Options

### Option A: Embrace CinematicDirector (Recommended)
- Keep GameManager → CinematicDirector → SceneManager chain
- Remove SceneLoader from maaacks menu system (only use for menu transitions)
- Stages handle their own scene loading via CinematicDirector
- **Pros**: Single unified pattern, clear ownership, async/await native
- **Cons**: Minor refactor to wire up stage scenes to CinematicDirector

### Option B: Unify Via SceneLoader
- Make GameManager use SceneLoader for all scene transitions
- Wire CinematicDirector to use SceneLoader instead of direct instantiation
- **Pros**: Single API, loading screen support everywhere
- **Cons**: GDScript/C# interop complexity, C# async/GDScript signals mismatch

### Option C: Hybrid (Best Practical)
- Menu system uses SceneLoader (already has it)
- Game stages use CinematicDirector (already designed for this)
- SceneLoader stays GDScript utility for menu transitions
- CinematicDirector stays C# for stage orchestration
- NO cross-contamination
- **Pros**: Respects two different systems, clear separation
- **Cons**: Two different loading patterns (but intentional)

---

## Recommendation: Option C (Hybrid Approach)

**Why?**
1. **Minimal Changes**: No refactor needed
2. **Respects Design**: Menu system (GDScript) stays simple, Game (C#) stays sophisticated
3. **Clear Intent**: SceneLoader for UI transitions, CinematicDirector for stage gameplay
4. **Easy to Understand**: Developers know "if GDScript, use SceneLoader; if C#, CinematicDirector does it"

**Implementation**:
- SceneLoader: Menu opening → Main menu → Stage1 scene entry
- CinematicDirector: Inside Stage1, orchestrates narrative sequences + gameplay
- NO crossover

**Cost**: Near zero. Current system already follows this pattern—just need to know it's intentional.

---

## Architecture Diagram

```
MainMenu (GDScript)
    ↓
    [SceneLoader] → Loading Screen
    ↓
Stage1 Scene (.tscn)
    ↓
GameManager.StartGame(0)
    ↓
CinematicDirector (Stage1Director)
    ↓
    └─→ SceneManager (per narrative scene)
        └─→ IStoryHandler (NarrativeUi, etc.)
```

This is already your current design. The only thing missing is documentation.
