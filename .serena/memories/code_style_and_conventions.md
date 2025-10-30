# Code Style and Conventions for Ωmega Spiral

## Language and Framework
- **Language**: C# 14 (preview)
- **Framework**: .NET 10 RC2
- **Engine**: Godot 4.6-dev2 with .NET/Mono
- **Testing**: GdUnit4 for Godot C# testing

## Naming Conventions
### Files/Folders
- **Folders**: snake_case (e.g., `my_scenes/`, `player_sprite.png`)
- **C# Script Files**: PascalCase matching class name (e.g., `PlayerCharacter.cs`)
- **Node Names in Scenes**: PascalCase (e.g., `Player`, `Camera3D`)
- **Signals**: snake_case (e.g., `door_opened`, `player_moved`)

### C# Code
- **Classes**: PascalCase (e.g., `PlayerCharacter`, `GameState`)
- **Methods**: PascalCase (e.g., `UpdateHealth()`, `CalculateDamage()`)
- **Properties**: PascalCase (e.g., `CurrentHealth`, `MaxSpeed`)
- **Constants**: PascalCase (e.g., `DefaultSpeed`, `MaxPlayers`)
- **Variables**: camelCase (e.g., `playerName`, `currentSpeed`)
- **Private Fields**: camelCase with underscore prefix (e.g., `_playerName`, `_currentSpeed`)

## Formatting
- **Indentation**: 4 spaces (soft tabs)
- **Line Endings**: LF (Unix)
- **Encoding**: UTF-8 without BOM
- **Max Line Length**: 100 characters
- **Brace Style**: Allman style (braces on new line)
- **Final Newline**: Yes for .cs files

## XML Documentation
- **Public Members**: Must be documented with XML comments
- **Internal Members**: Encouraged for complex logic
- **Tags**: `<summary>`, `<param>`, `<returns>`, `<exception>`, `<typeparam>`, `<typeparamref>`, `<cref>`, `<see langword>`, `<paramref>`, `<c>`, `<code>`, `<example>`, `<inheritdoc/>`

## Code Quality Rules
- **Warnings as Errors**: Enabled in builds
- **Nullable**: Enabled
- **Implicit Usings**: Enabled
- **Async/Await**: Use ConfigureAwait(false) for library code
- **Security**: High and critical vulnerabilities must be fixed
- **Performance**: Avoid unnecessary allocations, use efficient patterns

## Godot-Specific Conventions
- **Node Lifetime**: Godot manages disposal, not traditional Dispose pattern
- **Inheritance Chains**: Accept CA1501 warnings for Godot's deep hierarchies
- **Generated Code**: Suppress warnings for .godot/ generated files
- **Unsafe Blocks**: Allowed when necessary for performance

## Testing Conventions
- **Framework**: GdUnit4 with NUnit compatibility
- **Scene Runner**: Use for UI/integration tests
- **AutoFree**: Register objects for automatic cleanup
- **Mocks/Spies**: Use for dependencies and external calls
- **Input Simulation**: Use runner for UI tests
- **Assertions**: GdUnit4's AssertThat for all checks
