# GdUnit4 Refactoring Opportunities

## Executive Summary
The current test suite uses only basic GdUnit4 features. Significant improvements in test quality and coverage can be achieved by incorporating advanced GdUnit4 features that are already documented but unused.

---

## 1. **PARAMETERIZED TESTS** (Highest Priority)
**Currently:** Tests with similar logic repeat code multiple times
**Impact:** Reduce code duplication, improve maintainability, faster test feedback

### Examples in Current Code:
```csharp
// CURRENT: Three separate tests doing the same thing with different data
[TestCase]
public static void SelectchoiceWithkeyboardnavigationAllowschoiceselection() { ... }

[TestCase]
public static void SelectchoiceWithmouseclickAllowschoiceselection() { ... }

[TestCase]
public static void SelectchoiceWithgamepadinputAllowschoiceselection() { ... }
```

### REFACTORED: Single parameterized test
```csharp
[TestCase("keyboard", "Option A", 0, TestName = "KeyboardNavigation")]
[TestCase("mouse", "Option B", 1, TestName = "MouseClick")]
[TestCase("gamepad", "Option C", 2, TestName = "GamepadInput")]
public static void SelectChoice_WithVariousInputMethods_AllowsSelection(string inputMethod, string expectedChoice, int expectedIndex)
{
    // Single implementation handling all input methods
}
```

**Files to Refactor:**
- `ContentBlockTests.cs` - Many shader tests can be combined
- `NarrativeScriptFunctionalTests.cs` - Similar test patterns repeat
- `Stage1LoadingTests.cs` - Scene loading tests follow same pattern

---

## 2. **SIGNAL TESTING** (High Priority)
**Currently:** NOT IMPLEMENTED
**Impact:** Test event-driven behavior, verify signal emissions

### Use Cases:
- Verify content blocks emit "advanced" signal when complete
- Test choice selection emits "choice_selected" signal with correct data
- Verify narrative progression signals
- Test scene transition signals

### Example:
```csharp
[TestCase]
[RequireGodotRuntime]
public async Task ContentBlock_OnComplete_EmitsAdvancedSignal()
{
    // Arrange
    var runner = ISceneRunner.Load("res://source/stages/ghost/scenes/boot_sequence.tscn");

    // Act & Assert - Monitor signal with assertion
    var contentBlock = runner.FindChild<Control>("ContentBlock");
    await AssertSignal(contentBlock, "advanced")
        .IsEmitted()
        .WithinSeconds(5);
}
```

---

## 3. **ARGUMENT MATCHERS** (Medium Priority)
**Currently:** NOT IMPLEMENTED
**Impact:** More flexible assertions, match complex conditions

### Use Cases:
- Assert text contains patterns without exact match
- Verify numeric values within ranges
- Validate collection contents
- Complex state combinations

### Example:
```csharp
// Instead of:
AssertThat(textContent).IsEqual("full expected text");

// Use:
AssertThat(textContent).Contains("key phrase");
AssertThat(elapsedTime).IsGreaterThan(2.0);
AssertThat(choices.Count).IsInRange(1, 5);
```

---

## 4. **SCENE RUNNER ADVANCED FEATURES** (Medium Priority)
**Currently:** Basic load/instantiate only
**Impact:** Test visual, layout, and interaction aspects

### Missing Features:
- `await_idle_frame()` - Wait for rendering
- `await_signal()` - Wait for specific events
- `assert_scene_tree()` - Verify node hierarchy
- `find_child()` / `get_node()` - Proper node navigation
- `simulate_input_event()` - Direct event injection

### Example:
```csharp
[TestCase]
[RequireGodotRuntime]
public async Task TerminalScene_ShouldRenderTextWithProperAlignment()
{
    var runner = ISceneRunner.Load("res://source/stages/ghost/scenes/boot_sequence.tscn");

    // Find nodes in scene tree
    var textLabel = runner.FindChild<Label>("TextDisplay");
    var choiceContainer = runner.FindChild<VBoxContainer>("ChoiceContainer");

    // Assert layout
    AssertThat(textLabel.GlobalPosition.X)
        .IsCloseTo(runner.GetViewport().GetVisibleRect().GetCenter().X, 5);

    // Wait for render
    await runner.AwaitIdleFrame();

    // Verify children loaded
    AssertThat(choiceContainer.GetChildCount()).IsGreaterThan(0);
}
```

---

## 5. **MOQ MOCKING** (Medium Priority for C#)
**Currently:** Manual test doubles used everywhere
**Impact:** Easier mock setup, behavior verification

### Current Approach:
```csharp
private sealed class TestContentBlock
{
    // ~200 lines of manual mock implementation
}
```

### Moq Approach:
```csharp
var mockContentBlock = new Mock<IContentBlock>();
mockContentBlock
    .Setup(cb => cb.DisplayText(It.IsAny<string>()))
    .Callback(() => /* verify */);
```

---

## 6. **ACTION ASSERTIONS** (Lower Priority)
**Currently:** NOT IMPLEMENTED
**Impact:** Test side effects and state changes

### Example:
```csharp
AssertThat(() => contentBlock.ReceiveInput())
    .CausesStateChange(
        () => contentBlock.Visible,
        from: true,
        to: false);
```

---

## 7. **FUZZING TESTS** (Optional - Advanced)
**Currently:** NOT IMPLEMENTED
**Impact:** Find edge cases with random/generated inputs

### Use Case Example:
```csharp
[TestCase]
[Fuzz]
public void TypewriterAnimation_HandlesArbitraryText_WithoutCrashing(string randomText)
{
    // Test will run with hundreds of random strings
    var contentBlock = new TestContentBlock();
    contentBlock.StartTypewriter(randomText, TimeSpan.FromMilliseconds(50), "typewriter");

    for (int i = 0; i < 1000; i++)
    {
        contentBlock.AdvanceTypewriter(TimeSpan.FromMilliseconds(10));
    }

    AssertThat(contentBlock.IsTypewriterActive).IsFalse();
}
```

---

## 8. **CUSTOM ASSERTIONS** (Lower Priority)
**Currently:** NOT IMPLEMENTED
**Impact:** Domain-specific assertion methods for clearer test code

### Example:
```csharp
// Instead of:
AssertThat(narrative.Content).IsNotNull();
AssertThat(narrative.Content).IsNotEqual("");
AssertThat(narrative.DialogueChoices.Count).IsGreaterThan(0);

// Create custom assertion:
AssertThat(narrative).HasValidContent();
AssertThat(narrative).ProvidesChoices();
```

---

## 9. **BATCH TESTING / TEST FIXTURES** (Lower Priority)
**Currently:** Manual setup in each test
**Impact:** Shared expensive setup (loading scenes, initializing state)

### Example:
```csharp
[TestFixture]
public class ContentBlockBatch
{
    private ISceneRunner _runner;

    [SetUpFixture]
    public async Task LoadSceneOnce()
    {
        _runner = ISceneRunner.Load("res://source/stages/ghost/scenes/boot_sequence.tscn");
    }

    [TestCase]
    public void Test1() { /* reuse _runner */ }

    [TestCase]
    public void Test2() { /* reuse _runner */ }
}
```

---

## Quick Wins (Easiest to Implement)

1. **Parameterized Tests** - Refactor 5-10 test suites with repeating patterns
2. **Argument Matchers** - Replace exact string/numeric comparisons with semantic assertions
3. **Scene Runner FindChild()** - Replace manual node lookups
4. **await_idle_frame()** - Add between scene loading and assertions

---

## Recommended Implementation Order

1. ✅ **Phase 1** (This Sprint)
   - Implement Parameterized Tests (reduce LOC by ~30%)
   - Add Argument Matchers for assertions (improve readability)

2. **Phase 2** (Next Sprint)
   - Add Signal Testing for event verification
   - Use Scene Runner advanced node lookup methods

3. **Phase 3** (Future)
   - Replace manual test doubles with Moq
   - Add Fuzzing for edge case discovery
   - Implement Test Fixtures for expensive setup

---

## File Size Reduction Potential

| Feature | Impact | Files Affected |
|---------|--------|-----------------|
| Parameterized Tests | -40% LOC | ContentBlockTests, Stage1Loading |
| Moq Mocking | -50% LOC | All test doubles |
| Scene Fixtures | -20% LOC | All [RequireGodotRuntime] tests |
| **Total** | **-50% LOC** | **All test files** |

---

## AAA Quality Metrics

Current state:
- ✅ Arrange-Act-Assert pattern
- ✅ Basic assertions
- ✅ Scene runner usage
- ✅ Async/await for input simulation
- ❌ Signal verification
- ❌ Parameterized test data
- ❌ State change assertions
- ❌ Edge case coverage (fuzzing)

Target state:
- ✅ All of above
- ✅ Signal verification
- ✅ Parameterized tests
- ✅ Complex assertions
- ✅ Fuzzing for robustness
