# Project Coding Rules (Non-Obvious Only)

- Always use Godot.Collections.Dictionary and Godot.Collections.Array for signal parameters, not System.Collections
- Use type aliases: `Range = Godot.Range`, `Timer = Godot.Timer` to resolve System/Godot conflicts
- SceneManager.TransitionToScene() must be used for all scene transitions (not direct Godot methods)
- GameState singleton manages all persistent game state and player progress
- Use GD.Print() for logging, never Console.WriteLine()
- File paths in Godot use `res://` scheme for project resources
- XML documentation required for all public members with `<exception>` tags for thrown exceptions
- Async methods must use 'Async' suffix and avoid .Wait(), .Result(), or .GetAwaiter().GetResult()
- Use Task.WhenAll() for parallel execution of multiple tasks
