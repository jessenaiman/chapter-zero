# VS Code Test & Godot Task Setup TODOs (2025-10-14)

- [x] Review dotnet architecture instructions and other repo guidelines relevant to tooling updates.
- [x] Collect .NET 10 RC, C# 14, and Godot 4.5.1 RC2 references via Context7 for task alignment.
- [x] Inspect existing VS Code task configurations (.vscode directory) and current shell scripts.
- [x] Diagnose recent dotnet build failure (task log shows exit code 1) to ensure clean baseline before changes.
- [ ] Design required VS Code tasks: dotnet test, dotnet watch test, and Godot editor launch.
- [ ] Implement/Update VS Code configuration files to add the tasks with proper problem matchers and paths.
- [ ] Update documentation (README or dedicated tooling docs) to describe new tasks and usage for the team.
- [ ] Run dotnet build to confirm clean build after configuration changes.
- [ ] Run configured tasks (dotnet test, dotnet watch test, Godot editor) to validate behavior.
- [ ] Record outcomes and next steps in project memories for continuity.
