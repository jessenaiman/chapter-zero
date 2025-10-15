# Project Debug Rules (Non-Obvious Only)

- Use GD.Print() for logging instead of Console.WriteLine() - Godot's logging system is integrated with the editor
- Debugger must be attached through Godot editor, not external IDE for full Godot object inspection
- Scene transitions must use SceneManager.TransitionToScene() - direct Godot scene changes bypass game state management
- GameState singleton persists across scene changes and must be considered when debugging state issues
- NobodyWho plugin logs are visible in Godot output panel, not system console
- File paths use `res://` scheme - local file system paths won't work for Godot resources
- Memory leaks often occur from disconnected signals - check SignalManager for proper cleanup
