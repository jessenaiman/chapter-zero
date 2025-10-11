# Development Commands for Ωmega Spiral

## Build Commands
- `dotnet build` - Build the C# solution
- `godot --build-solutions --no-window` - Build C# code within Godot
- `dotnet clean` - Clean build artifacts

## Run Commands
- `godot` - Launch Godot editor
- `godot --run` - Run the game directly
- `godot project.godot` - Run specific project

## Test Commands
- `dotnet test` - Run NUnit tests
- `dotnet test --filter "Category=Unit"` - Run specific test categories

## Export Commands
- Godot editor: Project → Export - Export to Windows/Linux executables

## Code Quality Commands
- `dotnet format` - Format C# code
- No specific linter configured yet

## Git Commands
- `git add .` - Stage all changes
- `git commit -m "message"` - Commit changes
- `git push` - Push to remote

## File System Commands
- `ls -la` - List files with details
- `find . -name "*.cs" -type f` - Find C# files
- `grep -r "pattern" Source/Scripts/` - Search in scripts

## Godot-Specific Commands
- `godot --export "Windows Desktop" bin/game.exe` - Export for Windows
- `godot --export "Linux/X11" bin/game.x86_64` - Export for Linux

## Development Workflow
1. Edit C# scripts in `Source/Scripts/`
2. Build with `dotnet build`
3. Run with `godot --run` to test
4. Use Godot editor for scene design
5. Run tests with `dotnet test`
