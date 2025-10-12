# Scene1 Integration Tests

## Overview

Automated integration tests for Scene1Narrative that simulate complete playthrough scenarios with user choices. These tests validate the entire narrative flow from opening sequence to scene transition using simulated Dialogic signals.

## Test Structure

### Test Files

- **`DialogicTestHelper.cs`**: Core helper class that simulates Dialogic signals and timeline execution
- **`Scene1TestHelper.cs`**: Scene1-specific helper with persona data and validation methods
- **`Scene1PlaythroughTests.cs`**: Complete integration tests for all narrative paths

### Test Coverage

#### Complete Playthrough Tests
- ✅ HERO persona path with all story choices
- ✅ SHADOW persona path with all story choices
- ✅ AMBITION persona path with all story choices

#### Component Tests
- ✅ Opening sequence displays all lines in order
- ✅ Persona selection updates game state for all personas
- ✅ Story block progression captures choices
- ✅ Name input validation and storage
- ✅ Secret question response recording
- ✅ Timeline properly ends after completion
- ✅ Multiple sequential playthroughs work correctly

## Running the Tests

### From VS Code

1. Open the Testing panel (flask icon in sidebar)
2. Navigate to Scene1PlaythroughTests
3. Click "Run All Tests" or run individual tests

### From Command Line

```bash
# Run all tests
dotnet test

# Run only Scene1 integration tests
dotnet test --filter "FullyQualifiedName~Scene1PlaythroughTests"

# Run a specific test
dotnet test --filter "FullyQualifiedName~CompletePlaythrough_HeroPersona_CompletesSuccessfully"
```

### From Godot Editor

If you have GdUnit4 addon installed:

1. Open Godot Editor
2. Go to bottom panel → GdUnit4
3. Select Scene1PlaythroughTests
4. Click "Run Tests"

## Test Scenarios

### HERO Persona Path

```csharp
// Example: Simulate HERO playthrough
sceneHelper.SimulateCompletePlaythrough(
    personaIndex: 1,              // HERO
    playerName: "Lumina",
    storyChoices: new Dictionary<int, int> { { 0, 0 }, { 1, 1 } },
    secretResponseIndex: 1        // "yes"
);
```

### SHADOW Persona Path

```csharp
// Example: Simulate SHADOW playthrough
sceneHelper.SimulateCompletePlaythrough(
    personaIndex: 2,              // SHADOW
    playerName: "Umbra",
    storyChoices: new Dictionary<int, int> { { 0, 1 }, { 1, 0 } },
    secretResponseIndex: 2        // "no"
);
```

### AMBITION Persona Path

```csharp
// Example: Simulate AMBITION playthrough
sceneHelper.SimulateCompletePlaythrough(
    personaIndex: 3,              // AMBITION
    playerName: "Caelus",
    storyChoices: new Dictionary<int, int> { { 0, 0 }, { 1, 0 } },
    secretResponseIndex: 3        // "only if you keep one for me"
);
```

## Test Data

### Persona Choices

| Index | ID        | Name      | Description                              |
|-------|-----------|-----------|------------------------------------------|
| 1     | hero      | HERO      | A tale where one choice can unmake a world |
| 2     | shadow    | SHADOW    | A tale that hides its truth until you bleed for it |
| 3     | ambition  | AMBITION  | A tale that changes every time you look away |

### Secret Question Responses

| Index | Response                        |
|-------|---------------------------------|
| 1     | yes                             |
| 2     | no                              |
| 3     | only if you keep one for me     |

### Expected Opening Lines

```
Once, there was a name.
Not written in stone or spoken in halls—but remembered in the silence between stars.
I do not know when I heard it. Time does not pass here.
But I have held it.
And now… I hear it again.
```

## How It Works

### Signal Simulation

The `DialogicTestHelper` simulates Dialogic signals without requiring the actual Dialogic plugin to be active during tests:

```csharp
// Simulate timeline start
dialogicHelper.SimulateStartTimeline("res://path/to/timeline.dtl");

// Simulate text display
dialogicHelper.SimulateTextSignal("Opening line text");

// Simulate player choice
dialogicHelper.SimulateChoice(choiceIndex: 0, choiceText: "HERO");

// Simulate text input
dialogicHelper.SimulateTextInput("PlayerName");

// Simulate timeline end
dialogicHelper.SimulateTimelineEnd();
```

### State Validation

Tests validate that game state is correctly updated:

```csharp
bool isValid = sceneHelper.ValidateGameState(
    expectedPersona: DreamweaverThread.Hero,
    expectedName: "Lumina",
    expectedSecret: "yes"
);
```

### Signal Capture

All Dialogic signals are captured for assertion:

```csharp
// Check for specific signals
Assert.That(
    dialogicHelper.CapturedSignals,
    Has.Some.Matches<string>(s => s.Contains("HERO")),
    "Should have HERO selection signal"
);
```

## Repeatability

All tests are designed to be:

- ✅ **Repeatable**: Can be run multiple times with consistent results
- ✅ **Isolated**: Each test has its own setup/teardown
- ✅ **Independent**: Tests don't depend on execution order
- ✅ **Fast**: Tests run in seconds without UI rendering

## Extending the Tests

### Adding New Story Blocks

```csharp
// Add new story block choices to your test
var storyChoices = new Dictionary<int, int>
{
    { 0, 0 }, // Block 0, choice 0
    { 1, 1 }, // Block 1, choice 1
    { 2, 0 }, // Block 2, choice 0  <- NEW
};
```

### Testing Custom Signals

```csharp
// Simulate custom Dialogic signals
dialogicHelper.SimulateCustomSignal("custom_event", new Dictionary<string, object>
{
    { "param1", "value1" },
    { "param2", 42 }
});
```

### Adding New Validation

```csharp
// Add custom validation logic
var customValue = dialogicHelper.GetVariable("custom_var");
Assert.That(customValue, Is.EqualTo(expectedValue));
```

## Troubleshooting

### Tests Fail with "GameState not found"

This is expected in unit test environments where autoload singletons aren't available. The tests use fallback logic to continue validation without crashing.

### Timeline signals not captured

Ensure you're calling `SimulateStartTimeline()` before other simulation methods:

```csharp
sceneHelper.SimulateOpeningSequence(); // This calls SimulateStartTimeline internally
```

### Build errors after adding tests

Ensure GdUnit4 packages are installed:

```bash
dotnet restore
dotnet build
```

## Dependencies

- **NUnit 4.4.0**: Test framework
- **GdUnit4.api 5.0.0**: Godot testing API
- **GdUnit4.test.adapter 3.0.0**: Test adapter for .NET
- **Godot.NET.Sdk 4.5.0**: Godot C# SDK

## Contributing

When adding new tests:

1. Follow the naming pattern: `MethodName_Condition_ExpectedResult()`
2. Add XML documentation comments to all test methods
3. Use the test helpers for common operations
4. Validate with `dotnet test` before committing
5. Run Codacy analysis on new test files

## Related Documentation

- [Scene1Narrative Implementation](../../Source/Scripts/field/narrative/Scene1Narrative.cs)
- [GameState Documentation](../../Source/Scripts/GameState.cs)
- [DreamweaverSystem Documentation](../../Source/Scripts/DreamweaverSystem.cs)
- [Testing Guidelines](../../docs/TESTING.md)
