# Project Documentation Rules (Non-Obvious Only)

- SceneManager and GameState are autoloaded singletons - they're available globally without instantiation
- NobodyWho plugin requires local LLM model files in the models/ directory for AI features
- Scene transitions must use SceneManager.TransitionToScene() method, not Godot's built-in scene switching
- DreamweaverSystem manages AI-driven narrative personas and is autoloaded
- NarratorEngine handles dynamic dialogue and story progression
- XML documentation is enforced via build process - all public members must be documented
- Godot.Collections.Dictionary and Godot.Collections.Array must be used for signals, not System.Collections
- The project uses C# 14 with preview features enabled
- File paths in Godot use `res://` scheme for project resources, not file system paths
- Async methods require 'Async' suffix for consistency
