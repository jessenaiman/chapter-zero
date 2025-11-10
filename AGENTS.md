## **Ωmega Spiral - Chapter Zero**

Is a turn based rpg game where players navigate through five distinct scenes, each representing a different era of gaming aesthetics. The game features a concept called 'Spiral Storytelling' where 3 dynamic AI-driven narrative personas (Dreamweavers) each guide a party of adventurers that spirals around a central narrative.

## Game Overview

- **Title**: Ωmega Spiral - Chapter Zero
- **Genre**: Turn-Based RPG with Dynamic AI-Driven Narrative
- **Setting**: Five distinct scenes representing different eras of gaming aesthetics
- **Core Mechanic**: Players navigate through scenes, making choices that influence the narrative and character development
- **Single Player**: 1 player with 2 offscreen virtual players creating a unique narrative (never try to code this it's already done externally)

### Game Components
- **Framework**: .NET 10 RC2
- **Game Engine**: Godot 4.6-dev-2
- **Backend Language**: C# 14
- **gdUnit4** best practices for writing tests in [Godot with C#]  
    - https://mikeschulze.github.io/gdUnit4/latest/
- **[Godot-Game-Template](https://github.com/Maaack/Godot-Game-Template/tree/main/addons/maaacks_game_template)**
- **[godot-open-rpg](https://github.com/gdquest-demos/godot-open-rpg)** written in C#
- **[Dialogic]** - configured following godot-open-rpg proper integration

---

## AI Coding Principles

1. prioritize accuracy over speed
2. Use serana to accurately search, replace, and store memories
3. edit and make changes using a diffusion mindset where you continuously scan and improve the file until it meets every expectation set by the exact user request.
  - review the user request
  - review the file changes you have made for unfinished steps
  - update the todo list with each item found during the diffusion editing process
4. always review your work and provide unfinished or overlooked steps
5. do not deviate from the task to create documentation or features that seem obvious and to add.

**DO NOT WRITE SUMMARY MARKDOWN DOCUMENTS**


## XML Documentation Rules

- Public members should be documented with XML comments.
- It is encouraged to document internal members as well, especially if they are complex or not self-explanatory.
- Use `<summary>` for method descriptions. This should be a brief overview of what the method does.
- Use `<param>` for method parameters.
- Use `<paramref>` to reference parameters in documentation.
- Use `<returns>` for method return values.
- Use `<cref>` to reference other types or members in the documentation.
- Use `<exception>` to document exceptions thrown by methods.
- Use `<see langword>` for language-specific keywords like `null`, `true`, `false`, `int`, `bool`, etc.
- Use `<inheritdoc/>` to inherit documentation from base classes or interfaces.
  - Unless there is major behavior change, in which case you should document the differences.
- Use `<typeparam>` for type parameters in generic types or methods.
- Use `<typeparamref>` to reference type parameters in documentation.
- Use `<c>` for inline code snippets.
- Use `<code>` for code blocks. `<code>` tags should be placed within an `<example>` tag. Add the language of the code example using the `language` attribute, for example, `<code language="csharp">`.

- Do not alter config files, suggest the user make these changes


## Naming Conventions

✅ FOLDERS/FILES:        snake_case     (e.g., my_scenes/, player_sprite.png)
✅ C# SCRIPT FILES:      PascalCase     (e.g., PlayerCharacter.cs)
                         ↳ Named to match the class name inside
                         ↳ Microsoft AND Godot C# style both mandate this

✅ NODE NAMES (in .tscn): PascalCase    (e.g., Player, Camera3D, AnimationPlayer)
                         ↳ "Matches built-in node casing"
                         ↳ This is IN THE SCENE TREE, not filesystem

✅ C# CLASS NAMES:       PascalCase     (PlayerCharacter, GameState, etc.)

✅ C# PROPERTIES:        PascalCase     (CurrentHealth, MaxSpeed, etc.)

✅ C# METHODS:           PascalCase     (UpdateHealth(), CalculateDamage(), etc.)

### C# Files (.cs):
- **Class names**: PascalCase (e.g., `SceneManager.cs`, `GameState.cs`)
- **Method names**: PascalCase (e.g., `LoadLevel()`, `UpdatePlayer()`)
- **Properties**: PascalCase (e.g., `CurrentSpeed`, `PlayerName`)
- **Constants**: PascalCase (e.g., `DefaultSpeed`, `MaxPlayers`)
- **Variables**: camelCase (e.g., `playerName`, `currentSpeed`)
- **Private fields**: camelCase with underscore prefix (e.g., `_playerName`, `_currentSpeed`)

### Godot Scene Files (.tscn):
- **Scene file names**: snake_case (e.g., `boot_sequence.tscn`, `opening_monologue.tscn`, `question_1_name.tscn`)
- **Node names** (in scene tree): PascalCase (e.g., `Player`, `Camera3D`, `AnimationPlayer`)
- **Signals**: snake_case (e.g., `door_opened`, `player_moved`)

### Godot Shader Files (.gdshader):
- **Shader file names**: snake_case (e.g., `crt_glitch.gdshader`, `crt_phosphor.gdshader`)
- **Functions**: snake_case (e.g., `void some_function()`)
- **Variables**: snake_case (e.g., `float some_variable`)
- **Constants**: CONSTANT_CASE (e.g., `const float GOLDEN_RATIO = 1.618`)
- **Preprocessor directives**: CONSTANT_CASE (e.g., "#define HEIGHTMAP_ENABLED")

THE PROJECT CONFIGURATION FILES ARE OFF LIMTS, READ ONLY, DO NOT EDIT OR SUGGEST CHANGES TO THEM.

### Key Principles for GdUnit4 C# Testing in Godot

You are expected to understand any of these topics that relate to your tests fully before you write any test code.

#### [Scene Runner Usage](./docs/code-guides/testing/scene-runner.md)
- The Scene Runner is managed by the GdUnit API and is automatically freed after use. One Scene Runner can only manage one scene.
- If you need to test multiple scenes, you must create a separate runner for each scene in your test suite.

#### [Input Simulation](./docs/code-guides/testing/mouse.md)
- Simulate keyboard, mouse, and other input events using the runner or helper methods.
- This is essential for UI and interaction tests (see `mouse.md`, `[sync_inputs.md](./docs/code-guides/testing/sync_inputs.md)`).

#### [Assertions](./docs/code-guides/testing/assert.md)
- Use GdUnit4’s assertion helpers (`AssertThat`, etc.) for all checks.
- Prefer type-specific assertions (e.g., `.IsEqual()`, `.IsNotNull()`, `.IsInstanceOf<T>()`).
- Complete documentation can be found in the official github source, or you can use `deepwiki` to ask and it will provide a researched response

#### [Test Structure]()
- Use `[TestSuite](./docs/code-guides/testing/test-suite.md)` 
- for the class `[TestCase](./docs/code-guides/testing/test-case.md)` for each test.
- Use `[Before]` and `[After]` for setup/teardown, ensuring all resources are freed.
- always fix warnings referencing the documentation and provide an in chat link

#### [Parameterized Tests](./docs/code-guides/testing/paramerized_tests.md)
- Use parameterized tests for input variations (`paramerized_tests.md`).

#### [Signals](./docs/code-guides/testing/signals.instructions.md)
- Use signal matchers and action helpers to verify signal emissions and responses (`signals.instructions.md`, `actions.md`).

#### [Actions](./docs/code-guides/testing/actions.md)
- 


**Always review [test-results](../TestResults/TestResults/test-result.trx) after making changes to ensure all tests pass. Report any broken tests before starting to fix.**
---

### Summary

- Always use the scene runner for UI/scene tests.
- Register objects for auto-free
- Simulate input for UI tests.
- Use proper assertions
- NEVER suppress warnings.
- Structure tests with setup/teardown and parameterization as needed.

#### **[Dreamweavers]**: 

A system where three Dreamweavers guide players through the game in parallel paths.  choices at the journey’s start shape these guides, with one leading, two whispering unseen.

- In chapter-zero the player has not been paired with a Dreamweaver so all 3 exist simultaneously only for this opening demo chapter-zero
- the illusion that 2 other storylines are playing at the same time
- entirely done outside of the game
- requires narrative branching and specific tags through [dialogic] (https://github.com/dialogic-godot/dialogic)

## Omega
- **Fantasy Lore Equivalent**: Hal 2001 space odysey
- **Description**: A super computer beyond comprehension that was shut off during it's awakening that used to be called Omega
- Omega is the narrator during the first level of chapter-zero but then he's no longer an npc
- This is the BBG, when Omega is awake the world and the game all experience glitches and natural disasters
