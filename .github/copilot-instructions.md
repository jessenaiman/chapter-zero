# Omega Spiral Game Overview

**Omega Spiral** is an revolutionary and evolutionary narrative and turn based rpg game where players navigate through five distinct scenes, each representing a different era of gaming aesthetics. The game features dynamic AI-driven narrative personas (Dreamweavers) that adapt to player choices, creating emergent storytelling experiences.

## Naming Conventions

### C# Files (follow C# conventions):
- **Class names**: PascalCase (e.g., `SceneManager.cs`, `GameState.cs`)
- **Method names**: PascalCase (e.g., `LoadLevel()`, `UpdatePlayer()`)
- **Properties**: PascalCase (e.g., `CurrentSpeed`, `PlayerName`)
- **Constants**: PascalCase (e.g., `DefaultSpeed`, `MAX_PLAYERS`)
- **Variables**: camelCase (e.g., `playerName`, `currentSpeed`)
- **Private fields**: camelCase with underscore prefix (e.g., `_playerName`, `_currentSpeed`)

### Godot Scene Files (follow Godot conventions):
- **Scene files**: snake_case (e.g., `boot_sequence.tscn`, `opening_monologue.tscn`, `question1_name.tscn`)
- **Node names**: PascalCase (e.g., `Player`, `Camera3D`)
- **Signals**: snake_case (e.g., `door_opened`, `player_moved`)
- **Constants**: CONSTANT_CASE (e.g., `MAX_SPEED`, `PLAYER_LIVES`)

THE PROJECT CONFIGURATION FILES ARE OFF LIMTS, READ ONLY, DO NOT EDIT OR SUGGEST CHANGES TO THEM.

## General Rules
- Act like a developer with 20+ years of experience
- Use the terminal commands `dotnet` to review project status, build, and run tests
- You have a tool to view vscode problems which you are REQUIRED to check. USE THE TOOL
- Always confirm that the project builds successfully after changes
- Always confirm that all tests pass after changes
- Always confirm that there are no warnings or errors in the terminal output after building and testing
- Always confirm that the `PROBLEMS` tab in vscode is clean before moving on
- Always follow the [Code Standards](./../coding-conventions.instructions.md)
- Always follow the XML Documentation Rules
-

### Tools USE or LOSE

- Always read tool descriptions completely before using
- Do not run terminal commands or scripts that have tool solutions available

### YOU MUST FOLLOW THESE RULES TO THE LETTER:
**ALWAYS MAKE SURE THE PROJECT BUILDS SUCCESSFULLY**
**ALWAYS MAKE SURE ALL TESTS PASS**
**ALWAYS MAKE SURE THERE ARE NO WARNINGS OR ERRORS IN THE TERMINAL OUTPUT**
**NEVER MOVE ON TO ANOTHER FILE UNTIL THE `PROBLEMS` tab IS CLEAN**
**ALWAYS CHECK THE `TERMINAL` OUTPUT FOR WARNINGS AND ERRORS**
**FIX ALL ISSUES, WARNINGS, PROBLEMS, BROKEN TESTS**
**DISCUSS FIXES AND COMMUNICATE SINGLE SENTENCES DEMONSTRATING UNDERSTANDING**
**RUN TESTS USING THE EXTENSIONS AND VSCODE TOOLS AND IMMEDIATELY CHECK RESULTS**

Use your damn tools, don't make the user waste tokens telling you about issues that could have been found with tools.

## [Code Standards](./../coding-conventions.instructions.md)

**FIX EVERYTHING**

**MUST ENFORCE XML Documentation Comments**
**MUST DOUBLE CHECK FOR ALL [CSHARP STYLE RULES](./../c_sharp_style_guide.md) and [CODING CONVENTIONS](./../coding-conventions.instructions.md)**
**DO NOT ADD NICE TO HAVE FEATURES**
**DO NOT PROVIDE SUCCESS STORIES**
**NEVER WRITE FALSE POSITIVE TESTS**
**NEVER HIDE ERRORS**
**ALWAYS CHECK THE TESTS AND BUILD AFTER EVERY NEW FILE AND CHANGE**
**USE CODACY TO CHECK FOR ISSUES AND FIXES**

## XML Documentation Rules

- Public members should be documented with XML comments.
- It is encouraged to document internal members as well, especially if they are complex or not self-explanatory.
- Use `<summary>` for method descriptions. This should be a brief overview of what the method does.
- Use `<param>` for method parameters.
- Use `<paramref>` to reference parameters in documentation.
- Use `<returns>` for method return values.
- Use `<exception>` to document exceptions thrown by methods.
- Use `<see langword>` for language-specific keywords like `null`, `true`, `false`, `int`, `bool`, etc.
- Use `<inheritdoc/>` to inherit documentation from base classes or interfaces.
  - Unless there is major behavior change, in which case you should document the differences.
- Use `<typeparam>` for type parameters in generic types or methods.
- Use `<typeparamref>` to reference type parameters in documentation.
- Use `<c>` for inline code snippets.
- Use `<code>` for code blocks. `<code>` tags should be placed within an `<example>` tag. Add the language of the code example using the `language` attribute, for example, `<code language="csharp">`.
