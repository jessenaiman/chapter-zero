# Suggested Commands for Ωmega Spiral Development

## Build Commands
- `dotnet build --no-restore` - Build the C# project
- `godot --path . --verbose` - Run the game from command line

## Test Commands
- `dotnet test --no-build --settings .runsettings` - Run all tests with GdUnit4
- `dotnet test --settings .runsettings --filter "Class=YourTestClass"` - Run specific test class

## Code Quality Commands
- `dotnet format --verify-no-changes` - Check code formatting
- `dotnet build --warnaserror --no-restore` - Build with warnings as errors

## Development Workflow
- `git checkout -b feature/your-feature` - Create feature branch
- `dotnet test` - Run tests before commit
- `dotnet build` - Ensure build passes

## Utility Commands (Linux)
- `ls -la` - List files with details
- `find . -name "*.cs" -type f` - Find C# files
- `grep -r "pattern" source/` - Search for patterns in source
- `cd /home/adam/Dev/omega-spiral/chapter-zero` - Navigate to project root

## Godot Specific
- Open Godot editor and press F5 to play
- Enable plugins in Project → Project Settings → Plugins
