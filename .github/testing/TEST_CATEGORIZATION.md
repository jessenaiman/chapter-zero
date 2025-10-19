# Test Categorization Strategy for Omega Spiral

This document defines the test categories, traits, and filtering conventions used across the project to enable efficient test execution, CI/CD optimization, and developer workflow acceleration.

## Quick Reference

| Category | Runtime | Speed | Purpose |
|----------|---------|-------|---------|
| `Unit` | No Godot | < 100ms | Pure C# logic, single component |
| `Integration` | Mixed | 100ms-1s | Cross-component, multiple systems |
| `EndToEnd` | **Requires Godot** | > 2s | Full game flow, scene transitions |
| `Visual` | **Requires Godot** | > 1s | Scene rendering, UI correctness |

---

## Test Categories

Test categories divide tests by **execution layer** and **scope**. Every test **MUST** have exactly one category.

### Category: `Unit`

**Purpose:** Test pure C# logic in isolation with no Godot runtime dependency.

**Characteristics:**

- ✅ No `[RequireGodotRuntime]` attribute
- ✅ No Godot types or engine calls
- ✅ Execution time: < 100ms (typical: 1-10ms)
- ✅ Run in pure .NET environment (fastest)
- ✅ Safe for pre-commit hooks
- ✅ Parallelizable

**Location:** `Tests/Unit/**/*.cs`

**When to use:**

- Testing business logic (damage calculations, state machines)
- Testing data serialization/deserialization
- Testing algorithms and utility functions
- Testing domain entities and value objects

**Example:**

```csharp
namespace OmegaSpiral.Tests.Unit.Combat;

using static Assertions;

[TestSuite]
public class DamageCalculatorTests
{
    [Test]
    [Category("Unit")]
    [Trait("Layer", "Domain")]
    [Trait("Speed", "Fast")]
    [Trait("Owner", "Core")]
    public void Calculate_WithModifier_ReturnsCorrectValue()
    {
        // Arrange
        var calculator = new DamageCalculator();

        // Act
        var result = calculator.Calculate(baseDamage: 10, modifier: 1.5f);

        // Assert
        AssertThat(result).IsEqual(15);
    }

    [Test]
    [Category("Unit")]
    [Trait("Layer", "Domain")]
    [Trait("Speed", "Fast")]
    [Trait("Owner", "Core")]
    [DataPoint(nameof(DamageTestCases))]
    public void Calculate_VariousInputs_ReturnsExpected(
        int baseDamage,
        float modifier,
        int expected)
    {
        // Arrange
        var calculator = new DamageCalculator();

        // Act
        var result = calculator.Calculate(baseDamage, modifier);

        // Assert
        AssertThat(result).IsEqual(expected);
    }

    public static IEnumerable<object[]> DamageTestCases => new[]
    {
        new object[] { 10, 1.0f, 10 },
        new object[] { 10, 1.5f, 15 },
        new object[] { 5, 2.0f, 10 },
    };
}
```

---

### Category: `Integration`

**Purpose:** Test interactions between multiple components or systems working together.

**Characteristics:**

- ✅ May use real dependencies or test doubles
- ✅ Can have **optional** `[RequireGodotRuntime]` (depends on test)
- ✅ Execution time: 100ms - 1s
- ✅ Test cross-component communication
- ✅ Use `Trait("Runtime", "NoGodot")` or `Trait("Runtime", "RequireGodot")`
- ✅ Generally not parallelizable if using shared state

**Location:** `Tests/Integration/**/*.cs`

**When to use:**

- Testing GameState serialization + file I/O together
- Testing dialogue system reading narrative data from JSON
- Testing scene manager coordinating multiple subsystems
- Testing persistence layer (EF Core + SQLite)

**Example (No Godot):**

```csharp
namespace OmegaSpiral.Tests.Integration;

using static Assertions;

[TestSuite]
public class GameStatePersistenceTests
{
    [Test]
    [Category("Integration")]
    [Trait("Layer", "Infrastructure")]
    [Trait("Speed", "Slow")]
    [Trait("Runtime", "NoGodot")]
    [Trait("Owner", "Core")]
    public void SaveAndLoad_PreservesPlayerData()
    {
        // Arrange
        var gameState = new GameState { PlayerName = "Hero", Experience = 1000 };
        var repository = new InMemoryGameStateRepository();

        // Act
        repository.Save(gameState);
        var restored = repository.Load();

        // Assert
        AssertThat(restored.PlayerName).IsEqual("Hero");
        AssertThat(restored.Experience).IsEqual(1000);
    }
}
```

**Example (With Godot):**

```csharp
namespace OmegaSpiral.Tests.Integration;

using static Assertions;

[TestSuite]
public class NarrativeDataLoadingTests
{
    [Test]
    [Category("Integration")]
    [RequireGodotRuntime]
    [Trait("Layer", "Infrastructure")]
    [Trait("Speed", "Slow")]
    [Trait("Runtime", "RequireGodot")]
    [Trait("Scene", "GhostTerminal")]
    [Trait("Owner", "Content")]
    public async Task LoadNarrativeData_FromJson_PopulatesSceneData()
    {
        // Arrange
        var loader = new NarrativeDataLoader();
        var filePath = "res://source/data/stages/ghost-terminal/scene_1.json";

        // Act
        var sceneData = await loader.LoadAsync(filePath);

        // Assert
        AssertThat(sceneData).IsNotNull();
        AssertThat(sceneData.Beats).HasLength(3);
    }
}
```

---

### Category: `EndToEnd`

**Purpose:** Test complete game flows simulating real player interactions and scene transitions.

**Characteristics:**

- ✅ **MUST** use `[RequireGodotRuntime]` attribute
- ✅ **MUST** use `Trait("Runtime", "RequireGodot")`
- ✅ Execution time: > 2s per test
- ✅ Test full workflows (e.g., "Act 1 complete to victory screen")
- ✅ Verify scene transitions, UI updates, state changes
- ✅ Run in CI only (optional in local development)
- ✅ Not parallelizable (requires exclusive engine access)

**Location:** `Tests/EndToEnd/**/*.cs`

**When to use:**

- Testing player navigates Act 1 and reaches Act 2
- Testing game save/load preserves full game state
- Testing all scene transitions in a sequence
- Testing game over and victory conditions

**Example:**

```csharp
namespace OmegaSpiral.Tests.EndToEnd.GameFlow;

using static Assertions;

[TestSuite]
public class Act1CompletionFlowTests
{
    [Test]
    [Category("EndToEnd")]
    [RequireGodotRuntime]
    [Trait("Scene", "GhostTerminal")]
    [Trait("Speed", "Slow")]
    [Trait("Runtime", "RequireGodot")]
    [Trait("Owner", "Content")]
    public async Task Act1_PlayerMakesChoice_TransitionsToAct2()
    {
        // Arrange
        var director = GetNode<GhostTerminalCinematicDirector>("/root/Director");
        await director.PlayNarrative("act_1_intro");

        // Act
        director.MakeChoice(0); // Select first narrative choice
        await Task.Delay(2000); // Wait for scene transition

        // Assert
        AssertThat(GetTree().CurrentScene.Name).IsEqual("Act2StartScene");
        AssertThat(GameState.CurrentAct).IsEqual(2);
    }

    [Test]
    [Category("EndToEnd")]
    [RequireGodotRuntime]
    [Trait("Scene", "GhostTerminal")]
    [Trait("Speed", "Slow")]
    [Trait("Runtime", "RequireGodot")]
    [Trait("Owner", "Core")]
    public async Task GameSaveLoad_PreservesAct1Progress()
    {
        // Arrange
        var director = GetNode<GhostTerminalCinematicDirector>("/root/Director");
        GameState.CurrentScene = "ghost_terminal";
        GameState.NarrativeProgress = 5;

        // Act
        await GameState.SaveAsync();
        GameState.Reset();
        await GameState.LoadAsync();

        // Assert
        AssertThat(GameState.NarrativeProgress).IsEqual(5);
        AssertThat(GameState.CurrentScene).IsEqual("ghost_terminal");
    }
}
```

---

### Category: `Visual`

**Purpose:** Test UI/scene rendering and visual correctness.

**Characteristics:**

- ✅ **MUST** use `[RequireGodotRuntime]` attribute
- ✅ **MUST** use `Trait("Runtime", "RequireGodot")`
- ✅ Execution time: > 1s per test
- ✅ Test scene composition and visual hierarchy
- ✅ Verify UI elements render and layout correctly
- ✅ May include screenshot comparison (future)
- ✅ Run in CI optional (usually for critical UI)
- ✅ Not parallelizable

**Location:** `Tests/Visual/**/*.cs`

**When to use:**

- Testing dialogue UI wraps long text correctly
- Testing UI buttons are clickable/positioned correctly
- Testing scene composition loads all required nodes
- Testing theme/style application

**Example:**

```csharp
namespace OmegaSpiral.Tests.Visual;

using static Assertions;

[TestSuite]
public class DialogueUITests
{
    [Test]
    [Category("Visual")]
    [RequireGodotRuntime]
    [Trait("Scene", "GhostTerminal")]
    [Trait("Speed", "Slow")]
    [Trait("Runtime", "RequireGodot")]
    [Trait("Owner", "Content")]
    public async Task DialogueUI_WithLongText_WrapsCorrectly()
    {
        // Arrange
        var dialogueUI = GetNode<DialogueUIPanel>("/root/DialoguePanel");
        var longText = "Lorem ipsum dolor sit amet, consectetur adipiscing " +
                       "elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.";

        // Act
        await dialogueUI.ShowDialogueAsync(longText);

        // Assert
        var textLabel = dialogueUI.GetNode<Label>("TextLabel");
        AssertThat(textLabel.Text).IsEqual(longText);
        AssertThat(textLabel.CustomMinimumSize.X).IsGreaterThan(0);

        // Verify text wraps (height increased due to word wrap)
        var estimatedLines = Mathf.Ceil(textLabel.GetContentHeight() /
                            textLabel.GetThemeFontSize("font_size"));
        AssertThat(estimatedLines).IsGreaterThan(1);
    }
}
```

---

## Trait System

Traits are **cross-cutting concerns** (orthogonal attributes) that enable powerful test filtering. Multiple traits can be applied to a single test.

### Trait: `Layer` (Architectural Layer)

**Values:** `Domain` | `Infrastructure` | `Presentation`

Identifies which architectural layer the test exercises:

- **Domain** (Business Logic)
  - Game rules, calculations, state machines
  - Examples: damage calculation, turn determination, narrative branching
  - Expected in: Unit, Integration tests

- **Infrastructure** (I/O, Persistence)
  - File operations, database, resource loading, networking
  - Examples: save/load, JSON parsing, texture loading
  - Expected in: Integration tests

- **Presentation** (UI, Rendering)
  - Dialogue boxes, menus, scene composition, animations
  - Examples: text wrapping, button positioning, fade transitions
  - Expected in: Visual, Integration tests

**Applied to all tests. No test should lack this trait.**

**Filter Examples:**
```bash
# Run all domain logic tests (usually fast)
dotnet test --filter "Trait=Layer&Trait=Domain"

# Run all infrastructure tests (slower, I/O bound)
dotnet test --filter "Trait=Layer&Trait=Infrastructure"

# Run all presentation tests (requires Godot)
dotnet test --filter "Trait=Layer&Trait=Presentation"
```

---

### Trait: `Speed` (Execution Time)

**Values:** `Fast` | `Slow`

Indicates expected execution time:

- **Fast** (< 100ms)
  - Pure C#, no I/O, no Godot runtime
  - Safe for local pre-commit checks
  - Can run 100+ tests/second
  - Example: Unit tests, simple validations

- **Slow** (> 1s)
  - Requires I/O, Godot runtime, or complex operations
  - Run in CI/CD pipeline
  - Not suitable for local pre-commit
  - Example: EndToEnd, Visual, Integration with I/O

**Applied to all tests. No test should lack this trait.**

**Filter Examples:**
```bash
# Pre-commit: only fast tests (< 10s total)
dotnet test --filter "Trait=Speed&Trait=Fast&Trait=Runtime&Trait=NoGodot"

# Local development: unit tests (< 30s)
dotnet test --filter "Category=Unit"

# CI pipeline stage 1: fast feedback (< 30s)
dotnet test --filter "Trait=Speed&Trait=Fast"

# CI pipeline stage 2: everything except Visual (< 2 min)
dotnet test --filter "Category!=Visual"
```

---

### Trait: `Runtime` (Godot Dependency)

**Values:** `NoGodot` | `RequireGodot`

Indicates whether test needs Godot engine:

- **NoGodot**
  - Pure C#, no Godot types or calls
  - Run without `[RequireGodotRuntime]` attribute
  - Faster (10x speedup vs. Godot runtime)
  - Safer (fewer environmental dependencies)
  - Example: Unit tests, Infrastructure with mocks

- **RequireGodot**
  - Needs `[RequireGodotRuntime]` attribute
  - Requires Godot engine and project setup
  - Slower (overhead of Godot startup)
  - Can access nodes, scenes, signals
  - Example: EndToEnd, Visual, Integration with real Godot types

**Applied to all tests. No test should lack this trait.**

**Attribute Requirement:**
- `Trait("Runtime", "NoGodot")` → **NO** `[RequireGodotRuntime]`
- `Trait("Runtime", "RequireGodot")` → **YES** `[RequireGodotRuntime]`

**Filter Examples:**
```bash
# Run all tests that DON'T need Godot (fast CI path)
dotnet test --filter "Trait=Runtime&Trait=NoGodot"

# Run only Godot-dependent tests
dotnet test --filter "Trait=Runtime&Trait=RequireGodot"

# Run fast tests without Godot locally
dotnet test --filter "Trait=Speed&Trait=Fast&Trait=Runtime&Trait=NoGodot"
```

---

### Trait: `Scene` (Act/Scene Context)

**Values:** `GhostTerminal` | `Nethack` | `NeverGoAlone` | `TileDungeon` | `FieldCombat`

Identifies which Act/Scene namespace the test belongs to:

- **GhostTerminal** - Act 1
  - Narrative-driven dialogue system
  - JSON-based branching storylines
  - Tests: NarrativeSceneData, dialogue flow, narrative choices

- **Nethack** - Act 2
  - Terminal-based dungeon sequences
  - ASCII roguelike experience
  - Tests: sequence generation, terminal rendering, dungeon logic

- **NeverGoAlone** - Act 3
  - Turn-based tactical combat
  - Squad-based mechanics
  - Tests: combat turns, squad AI, tactical positioning

- **TileDungeon** - Act 4
  - Tile-based dungeon exploration
  - Grid-based movement and combat
  - Tests: tile pathfinding, grid rendering, exploration state

- **FieldCombat** - Act 5
  - Real-time field combat system
  - Action-oriented gameplay
  - Tests: real-time combat, camera control, action input

**Applied to Act-specific tests only. Optional for generic infrastructure tests.**

**Filter Examples:**
```bash
# Run all Act 1 (GhostTerminal) tests
dotnet test --filter "Trait=Scene&Trait=GhostTerminal"

# Run all Act 2-3 tests
dotnet test --filter "Trait=Scene&Trait=Nethack OR Trait=Scene&Trait=NeverGoAlone"

# Run only combat-related acts (Act 3, 4, 5)
dotnet test --filter "Trait=Scene&Trait=NeverGoAlone OR Trait=Scene&Trait=TileDungeon OR Trait=Scene&Trait=FieldCombat"
```

---

### Trait: `Owner` (Ownership Domain)

**Values:** `Core` | `Content`

Indicates ownership and responsibility:

- **Core**
  - Game framework, reusable systems, infrastructure
  - Stable, foundational code
  - Low change frequency
  - Owned by: Engine team
  - Examples: GameState, SceneManager, input handling, persistence

- **Content**
  - Act-specific narrative, dialogue, scene data
  - Story and world building
  - Frequent changes (story iterations)
  - Owned by: Content/Narrative team
  - Examples: Dialogue choices, narrative beats, scene progression

**Applied to all tests. No test should lack this trait.**

**Filter Examples:**
```bash
# Run all core framework tests (safe refactoring scope)
dotnet test --filter "Trait=Owner&Trait=Core"

# Run all content/story tests (for narrative review)
dotnet test --filter "Trait=Owner&Trait=Content"

# Run infrastructure tests owned by core
dotnet test --filter "Trait=Layer&Trait=Infrastructure&Trait=Owner&Trait=Core"
```

---

## Common Filter Patterns

### 🚀 Local Development Workflow

```bash
# 1. Pre-commit check (< 10 seconds)
# Run only: Unit tests + fast tests + no Godot
dotnet test --filter "Trait=Speed&Trait=Fast&Trait=Runtime&Trait=NoGodot"

# 2. Before pushing to remote (< 30 seconds)
# Run only: All Unit category tests
dotnet test --filter "Category=Unit"

# 3. Working on specific Act (e.g., Act 1)
# Run: All GhostTerminal tests
dotnet test --filter "Trait=Scene&Trait=GhostTerminal"

# 4. After refactoring domain logic
# Run: All domain layer tests
dotnet test --filter "Trait=Layer&Trait=Domain"

# 5. Before submitting content (dialogues, narrative)
# Run: Content owner tests
dotnet test --filter "Trait=Owner&Trait=Content"

# 6. Local full test (if you have time)
# Run: Everything except Visual tests
dotnet test --filter "Category!=Visual"
```

### 🔄 CI/CD Pipeline Stages

```bash
# Stage 1: Fast Feedback (< 30 seconds)
# Gate: Syntax, style, fast logic
dotnet test --filter "Trait=Speed&Trait=Fast" --settings .runsettings

# Stage 2: Unit + Fast Integration (< 2 minutes)
# Gate: Logic correctness, component interactions
dotnet test --filter "Category=Unit OR (Category=Integration&Trait=Speed&Trait=Fast&Trait=Runtime&Trait=NoGodot)" --settings .runsettings

# Stage 3: All Non-Visual (< 5 minutes)
# Gate: Full game flow, scene management
dotnet test --filter "Category!=Visual" --settings .runsettings

# Stage 4: Full Suite (< 10 minutes, optional)
# Gate: Complete verification including visual tests
dotnet test --settings .runsettings
```

### 🎯 Specific Scenarios

```bash
# Run all tests for a specific developer
dotnet test --filter "Trait=Owner&Trait=Core" # Framework tests

# Run all tests that need Godot (to verify setup)
dotnet test --filter "Trait=Runtime&Trait=RequireGodot" --settings .runsettings

# Run all slow tests locally (before committing Godot changes)
dotnet test --filter "Trait=Speed&Trait=Slow" --settings .runsettings

# Run all tests EXCEPT slow ones (quick verification)
dotnet test --filter "Trait=Speed&Trait=Fast"

# Run all infrastructure tests (verify persistence layer)
dotnet test --filter "Trait=Layer&Trait=Infrastructure"

# Run all presentation tests (UI review)
dotnet test --filter "Trait=Layer&Trait=Presentation" --settings .runsettings
```

---

## Test Attribute Template

**Use this template when creating new tests:**

```csharp
namespace OmegaSpiral.Tests.{Category}.{Feature};

using static Assertions;

/// <summary>
/// Tests for <see cref="YourClass"/>.
/// </summary>
[TestSuite]
public class YourClassTests
{
    /// <summary>
    /// Tests <see cref="YourClass.MethodName"/> under typical conditions.
    /// </summary>
    [Test]
    [Category("Unit")]                        // ← Required: execution layer
    [Trait("Layer", "Domain")]                // ← Required: architectural layer
    [Trait("Speed", "Fast")]                  // ← Required: expected execution time
    [Trait("Owner", "Core")]                  // ← Required: ownership domain
    public void MethodName_GivenCondition_ExpectedBehavior()
    {
        // Arrange
        var subject = new YourClass();

        // Act
        var result = subject.MethodName();

        // Assert
        AssertThat(result).IsEqualTo(expectedValue);
    }

    /// <summary>
    /// Tests <see cref="YourClass.MethodName"/> with integration with persistence.
    /// </summary>
    [Test]
    [Category("Integration")]
    [Trait("Layer", "Infrastructure")]
    [Trait("Speed", "Slow")]
    [Trait("Runtime", "NoGodot")]
    [Trait("Owner", "Core")]
    public void MethodName_IntegrationScenario_ExpectedResult()
    {
        // Arrange
        var repository = new InMemoryRepository();
        var subject = new YourClass(repository);

        // Act
        subject.SaveData("key", "value");
        var restored = subject.LoadData("key");

        // Assert
        AssertThat(restored).IsEqualTo("value");
    }

    /// <summary>
    /// Tests full Act 1 narrative flow from start to completion.
    /// </summary>
    [Test]
    [Category("EndToEnd")]
    [RequireGodotRuntime]
    [Trait("Scene", "GhostTerminal")]
    [Trait("Speed", "Slow")]
    [Trait("Runtime", "RequireGodot")]
    [Trait("Owner", "Content")]
    public async Task NarrativeFlow_PlayerCompletes_TransitionsToAct2()
    {
        // Arrange
        var director = GetNode<GhostTerminalCinematicDirector>("/root/Director");

        // Act
        await director.PlayNarrativeAsync("act_1_complete");
        director.MakeChoice(0);

        // Assert
        AssertThat(GameState.CurrentAct).IsEqualTo(2);
    }
}
```

---

## Requirements Checklist

When creating or modifying tests, ensure:

- [ ] **Category**: Exactly one of `Unit`, `Integration`, `EndToEnd`, `Visual`
- [ ] **Layer Trait**: One of `Domain`, `Infrastructure`, `Presentation`
- [ ] **Speed Trait**: One of `Fast`, `Slow`
- [ ] **Runtime Trait**: One of `NoGodot`, `RequireGodot`
- [ ] **Owner Trait**: One of `Core`, `Content`
- [ ] **Scene Trait** (optional): One of `GhostTerminal`, `Nethack`, `NeverGoAlone`, `TileDungeon`, `FieldCombat` if Act-specific
- [ ] **Attribute Match**: `Trait("Runtime", "RequireGodot")` **requires** `[RequireGodotRuntime]`
- [ ] **Attribute Match**: `Trait("Runtime", "NoGodot")` **must not** have `[RequireGodotRuntime]`
- [ ] **XML Documentation**: All tests must have `<summary>` explaining what is tested
- [ ] **Naming Convention**: `MethodUnderTest_Condition_ExpectedBehavior`

---

## IDE Integration

### Visual Studio Code (C# Dev Kit)

```json
// .vscode/settings.json
{
    "dotnet-test-explorer.testProjectPath": "Tests",
    "dotnet-test-explorer.runSettingsPath": ".runsettings",
    "dotnet-test-explorer.formatOutput": true
}
```

In Test Explorer, apply filters:
```
Category:Unit
Trait:Layer=Domain&Category:Unit
Trait:Scene=GhostTerminal&Category:EndToEnd
```

### Visual Studio (2022+)

- **Test** → **Configure Run Settings** → Select `.runsettings`
- **Test Explorer** → **Add Traits Filter**
- Or use **Search** box: `@Category=Unit @Trait:Layer=Domain`

### JetBrains Rider (2024.2+)

- **Run** → **Edit Configurations** → Select `.runsettings`
- **Run** tests with **Filter** configuration
- Or use inline gutter icons to run specific test

---

## Troubleshooting

### "Test not discovered"
- Verify `[TestSuite]` and `[Test]` attributes are present
- Verify namespace follows `OmegaSpiral.Tests.*` pattern
- Verify file name ends with `Tests.cs`

### "Trait filter doesn't work"
- Verify trait names match exactly (case-sensitive)
- Verify filter syntax: `Trait=Name&Trait=Value` (ampersand between traits)
- Try: `dotnet test --filter "Trait=Layer&Trait=Domain"`

### "Godot runtime error in non-[RequireGodotRuntime] test"
- **ERROR**: Test has `Trait("Runtime", "NoGodot")` but uses Godot types
- **FIX**: Remove Godot references OR add `[RequireGodotRuntime]` + change to `Trait("Runtime", "RequireGodot")`

### "Test runs too slowly locally"
- Verify you're not running `Trait=Speed&Trait=Slow` locally
- Use: `dotnet test --filter "Trait=Speed&Trait=Fast&Trait=Runtime&Trait=NoGodot"`
- Reserve slow tests for CI pipeline

---

## References

- **GdUnit4Net Documentation**: https://mikeschulze.github.io/gdUnit4/
- **VSTest Filtering Guide**: https://github.com/microsoft/vstest/wiki/Filter-Option
- **Omega Spiral Architecture**: See `.github/instructions/code-standards.instructions.md`
- **.runsettings Configuration**: See `.runsettings` at solution root
