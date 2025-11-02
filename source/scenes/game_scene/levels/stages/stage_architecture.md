# Ωmega Spiral Stage Runtime Architecture

Below is the consolidated runtime flow for a single stage (example: **Ghost Terminal**). The diagram highlights how narrative, design configuration, and Godot scenes interact while following the agreed three-layer separation of concerns.


┌─────────────────────────────────────────────────────────────┐
│                    GAMEMANAGER                              │
│  Simple: Load stage CinematicDirector → await signal       │
│  Repeat for next stage                                      │
└─────────────────┬───────────────────────────────────────────┘
                  │ Creates
                  ↓
┌─────────────────────────────────────────────────────────────┐
│              CINEMATIC DIRECTOR (per stage)                 │
│                                                              │
│  - Does NOT parse JSON (loader does that)                   │
│  - Iterates through scenes array from parsed script        │
│  - For each scene:                                          │
│    ├─ Query NarrativeEngine for what's needed               │
│    ├─ Gather any other data required                        │
│    └─ Create stage-specific SceneManager with data         │
│  - Awaits signal from SceneManager (scene complete)        │
│  - When narrative finished → signal GameManager             │
└─────────────────┬───────────────────────────────────────────┘
                  │ Creates for each scene
                  ↓
┌─────────────────────────────────────────────────────────────┐
│            SCENEMANAGER (per scene)                         │
│                                                              │
│  - Takes data from CinematicDirector                        │
│  - "Light, camera, action" - loads .tscn                   │
│  - Runs the scene (displays narrative, handles UI)         │
│  - When scene finishes → signals CinematicDirector          │
└─────────────────────────────────────────────────────────────┘
```

## Layer Responsibilities

- **GameManager** – Sequentially runs stage directors and listens for their completion.
- **CinematicDirector** – Pure C# coordinator. Retrieves the cached plan, iterates scenes, spawns `SceneManager` per scene, and aggregates Dreamweaver scores.
- **SceneManager** – Godot-side executor. Loads `[f_terminal.tscn`, instantiates a stage specific `OmegaUi`, applies stage design rules, and hosts the scene-specific signal loop.
- **OmegaUi** – Implements `INarrativeHandler`, driving CRT effects, text playback, choice presentation, and relaying choices to score tracking.
- **StorybookEngine** – Provide thread-safe JSON deserialization and hold the immutable script structure for the stage.
- **DesignConfigService** – Provides UI config (`source//design/ghost_config.json`) and supplies color palettes, shader presets, and typography to the UI.

## Naming & Asset Rules

1. Every stage owns a config file named `stage_name_config.json` under `source//design/` (e.g., `ghost_config.json`).
2. UI scenes follow the pattern `stage_name_ui.tscn` with the root script `StageNameUi.cs` (PascalCase) residing alongside stage code.
3. CinematicDirectors live under `source//stages/<stage>/` as pure C# files; no Godot inheritance.
4. SceneManagers inherit from the shared `SceneManager` base and remain the bridge between CinematicDirectors and Godot nodes.
5. Narrative scripts are JSON (`stage_name.json`) and loaded exclusively through the stage’s `*DataLoader` class – CinematicDirectors never parse raw files directly.
6. Shader and design assets are injected via `DesignConfigService` so multiple stages can reuse the same runtime pipeline while presenting distinct aesthetics.
