# GdUnit4Net v5.0+ Testing Patterns for Omega Spiral

## Overview
This memory contains critical patterns and rules for writing tests using GdUnit4Net v5.0+ framework in the Omega Spiral project. These patterns are derived from official documentation and existing test files in the codebase.

## Core Architecture Principles

### Smart Runtime Detection (v5.0+ Key Feature)
- **Default Behavior**: Tests run WITHOUT Godot runtime (10x faster for logic tests)
- **Opt-in Godot**: Use `[RequireGodotRuntime]` ONLY when test needs Godot features:
  - Creating Nodes
  - Using SceneRunner
  - Accessing Godot singletons
  - Testing shader materials, scenes, or resources

### Test Suite Structure
```csharp
[TestSuite]                    // Required on class
[RequireGodotRuntime]          // ONLY if tests use Godot nodes/features
public partial class MyTests   // 'partial' required for Godot
{
    [Before] public void Setup() { }      // Runs before each test
    [After] public void Cleanup() { }     // Runs after each test
    [TestCase] public void MyTest() { }   // Individual test method
}
```

## Memory Management Pattern

### AutoFree Pattern (Recommended)
```csharp
[Before]
public void Setup()
{
    _Node = AutoFree(new Node())!;  // AutoFree registers for automatic cleanup
    _Control = AutoFree(new Control())!;
}

[After]
public void Cleanup()
{
    // AutoFree handles cleanup automatically - NO manual QueueFree() needed
    // GdUnit4 will free objects when test completes
}
```

### Critical Rules
1. ✅ Use `AutoFree()` for ALL Godot objects created in tests
2. ❌ DO NOT call `QueueFree()` manually - AutoFree handles it
3. ❌ DO NOT call `Dispose()` on AutoFree'd objects - handled automatically
4. ✅ AutoFree objects scoped to: Before/BeforeTest/Test/AfterTest/After

## Assertion Patterns

### Standard Assertions
```csharp
using static GdUnit4.Assertions;

// Value assertions
AssertThat(value).IsEqual(expected);
AssertThat(value).IsNotEqual(unexpected);
AssertThat(number).IsGreaterThan(0);
AssertThat(number).IsLessThan(100);

// Object assertions
AssertThat(obj).IsNotNull();
AssertThat(obj).IsNull();
AssertThat(obj).IsInstanceOf<MyType>();
AssertThat(obj).IsNotInstanceOf<WrongType>();

// Exception assertions
AssertThrown(() => method()).IsInstanceOf<ArgumentNullException>();

// String assertions
AssertThat(str).IsEqual("expected");
AssertThat(str).Contains("substring");

// Collection assertions
AssertThat(list).IsEmpty();
AssertThat(list).IsNotEmpty();
AssertThat(list).HasSize(5);

// Custom failure messages
AssertThat(value).IsEqual(expected)
    .OverrideFailureMessage("Custom explanation of what went wrong");
```

## Scene Runner Pattern (Integration Tests)

### Basic Usage
```csharp
[TestCase]
[RequireGodotRuntime]  // Scene runner REQUIRES Godot runtime
public async Task TestScene()
{
    // Load scene
    ISceneRunner runner = ISceneRunner.Load("res://my_scene.tscn");
    
    // Simulate frames
    await runner.SimulateFrames(60);
    
    // Simulate input
    runner.SimulateKeyPressed(KeyList.Enter);
    await runner.AwaitInputProcessed();
    
    // Access nodes
    var node = runner.FindChild("NodeName");
    
    // Set properties
    runner.SetProperty("_myField", value);
    
    // Get properties
    var result = runner.GetProperty<int>("_myField");
    
    // Wait for signals
    await runner.AwaitSignal("my_signal").WithTimeout(1000);
    
    // Cleanup is automatic
}
```

### Scene Runner Key Functions
- `SimulateFrames(uint frames)` - Process scene for N frames
- `SimulateKeyPressed(KeyList key)` - Simulate key press
- `SimulateMouseButtonPressed(MouseButton button)` - Simulate mouse
- `AwaitInputProcessed()` - Wait for input events to process
- `FindChild(string name)` - Find node in scene tree
- `SetProperty(string name, Variant value)` - Set scene property
- `GetProperty<T>(string name)` - Get scene property
- `AwaitSignal(string signal)` - Wait for signal emission

## Test Organization Patterns

### Unit Tests (`tests/integration/ui/omega/`)
- Test **single components** in isolation
- Use `AutoFree()` for node creation
- Focus on initialization, configuration, state changes
- Example: `OmegaBorderFrame_UnitTests.cs`, `OmegaTextRendererTests.cs`

### Integration Tests (`tests/integration/ui/`)
- Test **component interactions** and scene behavior
- Use **SceneRunner** for scene loading
- Test user interactions (input, navigation, signals)
- Example: `BaseMenuTests.cs`, `MainMenuTests.cs`

## Test Naming Convention

```csharp
MethodName_StateUnderTest_ExpectedBehavior()

// Examples:
Constructor_WithValidInput_InitializesCorrectly()
Constructor_WithNullInput_ThrowsArgumentNullException()
ApplyPreset_PhosphorPreset_AppliesCorrectShader()
ButtonClick_WhenDisabled_DoesNotTriggerAction()
Layout_PropertiesAreCorrect()
Anchors_FillParentBounds()
```

## Test Data Constants

```csharp
// Use constants instead of magic values
private const string _TestText = "Hello World";
private const float _ToleranceMs = 10f;
private const int _AnimationDelay = 50;
private const float _TypingSpeed = 50f;
```

## Async/Await Pattern

```csharp
[TestCase]
[RequireGodotRuntime]
public async Task AsyncMethod_CompletesSuccessfully()
{
    // ConfigureAwait(true) to maintain Godot context
    await _Renderer.AppendTextAsync("text").ConfigureAwait(true);
    
    AssertThat(_Renderer.GetCurrentText()).IsEqual("text");
}
```

## XML Documentation Requirements

Every test MUST have XML documentation:

```csharp
/// <summary>
/// Tests that [component] [performs action] when [condition].
/// Example: Tests that OmegaBorderFrame initializes with correct shader parameters.
/// </summary>
[TestCase]
public void Test() { }
```

## Test Structure Pattern

```csharp
[TestSuite]
[RequireGodotRuntime]
public partial class MyComponentTests
{
    private MyComponent? _Component;
    
    // Constants
    private const string _TestValue = "test";
    
    [Before]
    public void Setup()
    {
        _Component = AutoFree(new MyComponent())!;
    }
    
    [After]
    public void Cleanup()
    {
        // AutoFree handles cleanup
    }
    
    // ==================== INITIALIZATION ====================
    
    /// <summary>
    /// Tests that component initializes without errors.
    /// </summary>
    [TestCase]
    public void Constructor_InitializesSuccessfully()
    {
        AssertThat(_Component).IsNotNull();
    }
    
    // ==================== CONFIGURATION ====================
    
    /// <summary>
    /// Tests that configuration applies correctly.
    /// </summary>
    [TestCase]
    public void Configure_WithValidParameters_AppliesSettings()
    {
        // Test implementation
    }
    
    // ==================== ERROR HANDLING ====================
    
    /// <summary>
    /// Tests that null input throws exception.
    /// </summary>
    [TestCase]
    public void Method_WithNullInput_ThrowsException()
    {
        AssertThrown(() => _Component!.Method(null!))
            .IsInstanceOf<ArgumentNullException>();
    }
}
```

## Parameterized Tests

### Using TestCase Attributes
```csharp
[TestCase(1, 2, 3)]
[TestCase(5, 7, 12)]
[TestCase(10, 20, 30)]
public void Add_WithVariousInputs_ReturnsCorrectSum(int a, int b, int expected)
{
    AssertThat(a + b).IsEqual(expected);
}

// With custom test names
[TestCase(1, 2, 3, TestName = "SmallNumbers")]
[TestCase(100, 200, 300, TestName = "LargeNumbers")]
public void Add_Parameterized(int a, int b, int expected)
{
    AssertThat(a + b).IsEqual(expected);
}
```

### Using DataPoint (C# Only)
```csharp
[TestCase]
[DataPoint(nameof(TestData))]
public void Test_WithDataPoints(int a, int b, int expected)
{
    AssertThat(a + b).IsEqual(expected);
}

public static IEnumerable<object[]> TestData => new[]
{
    new object[] { 1, 2, 3 },
    new object[] { 5, 7, 12 }
};
```

## Critical Rules Checklist

1. ✅ **Use `[RequireGodotRuntime]` on class AND methods** that create Godot nodes
2. ✅ **Use `AutoFree()`** for all Godot objects - no manual `QueueFree()`
3. ✅ **Use `async Task`** for methods that await (with `.ConfigureAwait(true)`)
4. ✅ **Test one concern per test method** - keep tests focused
5. ✅ **Group tests with comments** - use `// ======== SECTION ========`
6. ✅ **Follow naming convention** - `MethodName_StateUnderTest_ExpectedBehavior`
7. ✅ **Add XML doc to every test** - explain what's being tested
8. ✅ **Use constants** for test data instead of magic values
9. ✅ **Test both success and failure paths** - include exception tests
10. ✅ **Follow existing patterns** - match style in existing test files

## Common Test Sections

Organize tests into logical sections with comment separators:

```csharp
// ==================== INITIALIZATION ====================
// ==================== CONFIGURATION ====================
// ==================== SHADER SETUP ====================
// ==================== ANIMATION PARAMETERS ====================
// ==================== LAYOUT ====================
// ==================== BUTTON MANAGEMENT ====================
// ==================== NAVIGATION ====================
// ==================== ERROR HANDLING ====================
// ==================== DISPOSAL ====================
```

## Example Test File Structure

See existing test files for reference:
- `tests/integration/ui/omega/OmegaBorderFrame_UnitTests.cs` - Component unit tests
- `tests/integration/ui/omega/OmegaTextRendererTests.cs` - Async component tests
- `tests/integration/ui/BaseMenuTests.cs` - Integration tests with scene tree
- `tests/integration/ui/omega/OmegaShaderControllerTests.cs` - Shader tests

## VSTest Integration

### Test Filtering
```bash
# Run specific class
dotnet test --filter "Class=OmegaBorderFrame_UnitTests"

# Run specific category
dotnet test --filter "TestCategory=UnitTest"

# Combine filters
dotnet test --filter "(TestCategory=UnitTest)&(Class~Omega)"
```

### .runsettings Configuration
```xml
<?xml version="1.0" encoding="utf-8"?>
<RunSettings>
    <RunConfiguration>
        <EnvironmentVariables>
            <GODOT_BIN>/path/to/godot</GODOT_BIN>
        </EnvironmentVariables>
    </RunConfiguration>
</RunSettings>
```

## Performance Tips

1. **Use `[RequireGodotRuntime]` sparingly** - only when absolutely needed
2. **Prefer unit tests over integration tests** - faster execution
3. **Use constants** - avoid repeated allocations
4. **Group related assertions** - reduce setup/teardown overhead
5. **Use AutoFree** - prevents memory leaks, automatic cleanup

## Anti-Patterns to Avoid

❌ **DON'T** add `[RequireGodotRuntime]` to every test - only use when needed
❌ **DON'T** manually call `QueueFree()` on AutoFree'd objects
❌ **DON'T** forget `.ConfigureAwait(true)` on awaited Godot operations
❌ **DON'T** use magic numbers/strings - use constants
❌ **DON'T** test multiple concerns in one test method
❌ **DON'T** forget XML documentation on test methods
❌ **DON'T** suppress warnings with `#pragma` - fix the issue
❌ **DON'T** create memory leaks - always use AutoFree for Godot objects

## References

- Official Docs: https://mikeschulze.github.io/gdUnit4/
- GdUnit4Net GitHub: https://github.com/MikeSchulze/gdUnit4Net
- Examples Repo: https://github.com/MikeSchulze/gdUnit4NetExamples
- VSTest Docs: https://github.com/microsoft/vstest

---

**Last Updated**: October 26, 2025
**Framework Version**: GdUnit4Net v5.0.0+
**Godot Version**: 4.6-dev2
