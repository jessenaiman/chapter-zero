# Initialization

Omega Spiral Stage Reorganization Task

- Objective: Clean up and reorganize the existing project structure for Omega Spiral, a Godot 4.5.1 C# RPG with five distinct scenes/stages.
- Requirements:
  - Organize files under correct stage directories, matching docs/stages structure.
  - Reorganize scripts and assets to be RPG-appropriate; move misplaced Audio.
  - Mark suspected duplicate code (DialogueChoice, ChoiceOption) with TODOs.
  - Unify narrative/ghost-terminal scenes and code to follow @ghost.md script.
  - Unify nethack gameplay mechanics per docs/stages/stage-2-nethack/nethack-scene.md.
  - Unify never-go-alone character selection per docs/stages/stage-3-never-go-alone/never-go-alone-script.json.
  - Wire town exploration system in Godot, referencing gdquest-demos/godot-open-rpg.
  - Build end credits montage with RPG references for Stage 5.
  - Review each stage for accuracy, fix duplication, update tests, validate execution, and ensure file organization matches stage groupings.
- Constraints:
  - Enforce XML documentation for public members.
  - Follow C# style guide and project coding conventions.
  - Use Godot-specific patterns (SceneManager, GameState, GD.Print, res:// paths).
  - Use GDUnit4 for tests, with [RequireGodotRuntime] for Godot-dependent tests.
  - Do not add features beyond requirements; do not hide errors or write false positive tests.
  - Use TODO and FIX tags for attention areas.
