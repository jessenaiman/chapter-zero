# Project Architecture Rules (Non-Obvious Only)

- SceneManager and GameState are autoloaded singletons that manage scene transitions and persistent state
- All scene transitions must route through SceneManager.TransitionToScene() to maintain game state consistency
- GameState singleton handles save/load functionality and persists across scene changes
- Godot Collections (not System.Collections) are required for signal parameters to maintain Godot compatibility
- The five main stages (1-6) represent distinct game phases with shared state management
- Async operations must use 'Async' suffix and avoid blocking calls like .Wait() or .Result()
- XML documentation is enforced at build time for all public members
- Type aliases (Range = Godot.Range, Timer = Godot.Timer) resolve System/Godot namespace conflicts
