# AGENTS.md

This file provides guidance to agents when working with code in this repository.

## Technology Stack
- **Engine**: Godot 4.5.1 RC (/home/adam/Godot_v4.5.1-rc2_mono_linux_x86_64) with .NET/Mono support
- **Language**: C# 12 (using .NET 8.0) with preview language features
- **AI Integration**: NobodyWho plugin for local LLM inference
- **Testing**: NUnit + Godot test framework

## Build/Lint/Test Commands

### Build Commands
```bash
dotnet build  # Standard build
dotnet build --warnaserror  # Enforce warnings as errors
```

### Test Commands
```bash
dotnet test  # Run all tests
# Note: Some tests require Godot runtime and must be run through Godot's test runner or GdUnit4
```

### Lint/Format Commands
```bash
dotnet format  # Format code
dotnet format --verify-no-changes # Verify formatting without changes
dotnet build --warnaserror # Static analysis with Roslyn/StyleCop analyzers
```

### Pre-commit Checks (automatically enforced)
1. Code formatting: `dotnet format --verify-no-changes`
2. Static analysis: `dotnet build --warnaserror`
3. Tests: `dotnet test`
4. Security (optional): `trivy fs --exit-code 1 --severity HIGH,CRITICAL .`

## Code Style & Conventions

### Non-Obvious Patterns
- **Godot C# Integration**: Use `Godot.Collections.Dictionary` and `Godot.Collections.Array` for signal parameters, not System.Collections
- **Type Aliases**: `Range = Godot.Range`, `Timer = Godot.Timer` to resolve System/Godot conflicts
- **Singleton Autoloads**: SceneManager and GameState are autoloaded singletons defined in project.godot
- **Scene Transitions**: Use SceneManager.TransitionToScene() for all scene changes (not direct Godot methods)

### XML Documentation Required
- All public members must have XML documentation comments
- Use `<inheritdoc/>` for overriding methods when behavior is unchanged
- Document exceptions with `<exception>` tags

### Async Programming
- Use 'Async' suffix for all async methods
- Avoid `.Wait()`, `.Result`, or `.GetAwaiter().GetResult()` in async code
- Use `Task.WhenAll()` for parallel execution

### Godot-Specific Conventions
- Use PascalCase for Godot signals and node paths
- Access Godot built-in types through Godot namespace (not System)
- Use `GD.Print()` for logging, not Console.WriteLine()
- File paths in Godot use `res://` scheme for project resources
