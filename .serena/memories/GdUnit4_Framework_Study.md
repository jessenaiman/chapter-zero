# GdUnit4 C# Framework - Comprehensive Study

## Document Purpose
Complete understanding of GdUnit4 test framework as it applies to Ωmega Spiral's failing tests.
Following: gdUnit4Net-README.mdx, csharp-setup.md, scene-runner.md, gdunit4-tools.instructions.md, signals.instructions.md

## 1. CORE FRAMEWORK CONCEPTS

### 1.1 Test Structure
**From gdUnit4Net-API.mdx:**
- Test suites marked with `[TestSuite]` attribute
- Test cases marked with `[TestCase]` attribute
- Setup/Teardown: `[Before]`, `[After]` for class-level; `[BeforeTest]`, `[AfterTest]` for test-level
- Tests automatically registered for cleanup via `AutoFree<T>()`

**Key Point:** Godot requires `[RequireGodotRuntime]` attribute on tests that use Godot features (nodes, signals, etc.)

### 1.2 Smart Runtime Detection (GdUnit4Net v5.0+)
**From gdUnit4Net-README.mdx:**
- Tests run in **lightweight mode by default** - NO Godot runtime
- Only when `[RequireGodotRuntime]` is used does Godot startup occur
- This is a BREAKING CHANGE from v4.x

**Implication for us:** Scene tests MUST have `[RequireGodotRuntime]` or they won't work at all.

---

## 2. THE SCENERUNNER - CRITICAL FOR UI TESTS

### 2.1 What ISceneRunner Does
**From scene-runner.md:**
- Loads a scene file into a Godot runtime context
- Manages the full lifecycle: Load → _Ready() → _Process() → Cleanup
- Provides methods to simulate input, inspect nodes, wait for signals
- Automatically disposed after test

**Usage Pattern:**
```csharp
[TestCase, RequireGodotRuntime]
public async Task MyTest()
{
    ISceneRunner runner = ISceneRunner.Load("res://my_scene.tscn");
    // ... test code ...
    // runner automatically disposed, scene nodes freed
}
```

### 2.2 CRITICAL: Scene Loading & Initialization
**From scene-runner.md:**
When `ISceneRunner.Load()` is called:
1. Scene file is deserialized
2. Root node is instantiated
3. Scene tree is built
4. **All `_Ready()` methods are called sequentially from root to leaves**
5. Scene is now "active" and ready for simulation

**This happens INSIDE Load()** - you can immediately access nodes after Load returns.

### 2.3 Node Access After Loading
**From accessors.md:**
```csharp
// After ISceneRunner.Load()
var node = runner.FindChild("NodeName", recursive: true, owned: false) as MyNodeType;
// or
var property = runner.GetProperty<T>("_fieldName");
// or
var nodeFromScene = runner.Scene().GetNode<MyNodeType>("path/to/node");
```

**IMPORTANT:** `FindChild()` searches the ACTIVE scene tree. If nodes aren't there, they're NULL.

---

## 3. AUTOMATIC OBJECT CLEANUP - AutoFree()

### 3.1 Purpose
**From gdunit4-tools.instructions.md:**
- Registers objects for automatic cleanup after test/suite
- Objects live only in their scope: suite setup → test setup → test → test teardown → suite teardown
- When scope exits, objects are freed

### 3.2 Cleanup Lifecycle
```
[Before]         ← Objects created here freed after before_test
  ↓
[BeforeTest]     ← Objects created here freed after test
  ↓
[TestCase]       ← Objects created here freed after test
  ↓
[AfterTest]      ← Objects still from before_test live; new objects freed after suite
  ↓
[After]
```

### 3.3 Manual Disposal
**From gdunit4-tools.instructions.md:**
"References keep an internal reference counter so that they are automatically released when no longer in use."
- BUT: Classes inheriting from `Object` must be manually freed OR wrapped in `AutoFree<T>()`
- Godot nodes inherit from Object, so they need special handling

---

## 4. SIGNAL TESTING

### 4.1 Basic Signal Assertion
**From signals.instructions.md:**
```csharp
// Wait for signal to emit
await AssertSignal(emitter).IsEmitted("signal_name");

// Wait for signal with parameters
await AssertSignal(emitter).IsEmitted("signal_name", [expected_arg]);

// Verify signal doesn't emit (with timeout)
await AssertSignal(emitter).IsNotEmitted("signal_name").WithTimeout(50);
```

### 4.2 Monitoring Signals
```csharp
// Start monitoring BEFORE emissions occur
AssertSignal(emitter).StartMonitoring();

// Then check for emissions
await AssertSignal(emitter).IsEmitted("signal_name");
```

**CRITICAL:** Must call `StartMonitoring()` BEFORE the signal is emitted, or it won't be captured.

---

## 5. INPUT SIMULATION

### 5.1 Synchronizing Input Events
**From sync_inputs.md:**
```csharp
runner.SimulateKeyPressed(KeyList.Enter);
await runner.AwaitInputProcessed();  // ← MANDATORY after input
```

**Why it matters:**
- Input events are queued
- Without `AwaitInputProcessed()`, test continues before input is processed
- Can cause "input has no effect" bugs in tests

### 5.2 Simulating Frames
**From scene-runner.md:**
```csharp
await runner.SimulateFrames(60);  // Process 60 frames
await runner.SimulateFrames(60, 100);  // 60 frames with 100ms delta
```

**Gotcha:** This is SLOW. Each frame is processed through full Godot lifecycle.

---

## 6. TEST LIFECYCLE IN DETAIL

### 6.1 Complete Flow for Scene Test
```csharp
[TestSuite]
public class MyUITest
{
    private ISceneRunner runner;

    [Before]  // ← Called ONCE before ALL tests
    public void Setup()
    {
        // Heavy setup here
    }

    [BeforeTest]  // ← Called BEFORE EACH TEST
    public void BeforeEachTest()
    {
        runner = AutoFree(ISceneRunner.Load("res://my_scene.tscn"));
        // Scene is NOW loaded, _Ready() has been called
    }

    [TestCase]
    public async Task TestOne()
    {
        // runner.Scene() is ready to use
        var node = runner.FindChild("MyNode");
        AssertThat(node).IsNotNull();
    }

    [AfterTest]  // ← Called AFTER EACH TEST, BEFORE scene cleanup
    public void AfterEachTest()
    {
        // runner NOT yet disposed here - still have access
        // runner.Dispose() happens after this method returns
    }

    [After]  // ← Called ONCE after ALL tests
    public void Teardown()
    {
        // Final cleanup
    }
}
```

### 6.2 Key Insight: Scene Lifecycle
- `AutoFree(ISceneRunner.Load())` - Scene loads, `_Ready()` called
- Test code runs - can access all scene nodes via runner
- `[AfterTest]` runs - runner still valid
- `[AfterTest]` ends - runner is Disposed, scene tree destroyed
- `AutoFree()` cleanup happens - all registered objects freed

---

## 7. ORPHAN NODES WARNING

### 7.1 What Causes It
**From gdunit4-tools.instructions.md and scene-runner.md:**
- Scene runner manages scene tree during test
- If test fails or doesn't properly clean up nodes added during test, they remain
- Godot tracks "owned" nodes - nodes created at runtime that weren't in the scene file
- If these aren't freed before runner.Dispose(), warning is generated

### 7.2 Prevention
1. Only access nodes that are in the .tscn file (already owned by scene)
2. If you create nodes programmatically in test, free them explicitly or wrap in AutoFree
3. Ensure Dispose() on runner completes fully - don't create long-lived references

---

## 8. REQUIREGODOTRUNTIME ATTRIBUTE

### 8.1 When to Use
**From csharp-setup.md and gdUnit4Net-README.mdx:**
- Use `[RequireGodotRuntime]` on ANY test that:
  - Uses `ISceneRunner.Load()`
  - Accesses Godot nodes, signals, or properties
  - Calls Godot engine methods

- Logic-only tests (pure C# calculations) don't need it - they run faster

### 8.2 Performance Impact
- Without `[RequireGodotRuntime]`: Test runs in lightweight mode (~1-5ms)
- With `[RequireGodotRuntime]`: Godot runtime spins up (~100-500ms)
- Only use when necessary

---

## 9. CONFIGURATION: .runsettings

### 9.1 Critical Setting
**From gdUnit4Net-TestAdapter.md:**
```xml
<?xml version="1.0" encoding="utf-8"?>
<RunSettings>
    <RunConfiguration>
        <EnvironmentVariables>
            <EnvironmentVariable name="GODOT_BIN" value="/path/to/godot/executable"/>
        </EnvironmentVariables>
    </RunConfiguration>
    <GdUnit4>
        <TestAdapterSettings>
            <Parameters>--headless</Parameters>
        </TestAdapterSettings>
    </GdUnit4>
</RunSettings>
```

**Mandatory:** GODOT_BIN must point to correct Godot binary, or tests fail silently.

---

## 10. COMMON MISTAKES & FIXES

### Mistake 1: Missing [RequireGodotRuntime]
```csharp
// ❌ WRONG - Will run without Godot, nodes will be NULL
[TestCase]
public async Task TestUI()
{
    var runner = ISceneRunner.Load("res://ui.tscn");  // Fails!
    var node = runner.FindChild("Label");  // Always NULL
}

// ✅ CORRECT
[TestCase]
[RequireGodotRuntime]
public async Task TestUI()
{
    var runner = ISceneRunner.Load("res://ui.tscn");  // Works
    var node = runner.FindChild("Label");  // Found
}
```

### Mistake 2: Not Wrapping in AutoFree
```csharp
// ❌ WRONG - Scene runner not freed
[BeforeTest]
public void Setup()
{
    var runner = ISceneRunner.Load("res://scene.tscn");  // Memory leak!
}

// ✅ CORRECT
[BeforeTest]
public void Setup()
{
    runner = AutoFree(ISceneRunner.Load("res://scene.tscn"));  // Auto-freed
}
```

### Mistake 3: Forgetting AwaitInputProcessed
```csharp
// ❌ WRONG - Input not processed yet
runner.SimulateKeyPressed(KeyList.Enter);
var button = runner.FindChild("MyButton") as Button;
AssertThat(button.Pressed).IsTrue();  // Fails - press not processed

// ✅ CORRECT
runner.SimulateKeyPressed(KeyList.Enter);
await runner.AwaitInputProcessed();  // Now input is processed
var button = runner.FindChild("MyButton") as Button;
AssertThat(button.Pressed).IsTrue();  // Passes
```

### Mistake 4: Checking Signals Without StartMonitoring
```csharp
// ❌ WRONG - Signal emitted before monitoring started
var emitter = AutoFree(new MyEmitter());
emitter.DoEmitSignal();
await AssertSignal(emitter).IsEmitted("my_signal");  // Fails - missed it

// ✅ CORRECT - Option A: StartMonitoring before emit
var emitter = AutoFree(new MyEmitter());
AssertSignal(emitter).StartMonitoring();
emitter.DoEmitSignal();
await AssertSignal(emitter).IsEmitted("my_signal");  // Passes

// ✅ CORRECT - Option B: Create runner scene which emits during _Ready
var runner = ISceneRunner.Load("res://emitter_scene.tscn");
await AssertSignal(runner.Scene()).IsEmitted("my_signal");  // Passes
```

---

## SUMMARY: What Tests Need to Pass

1. **Must have `[RequireGodotRuntime]`** on any test using ISceneRunner or Godot objects
2. **Must wrap scene runner in `AutoFree()`** for automatic cleanup
3. **Must use `[BeforeTest]/[AfterTest]`** for per-test setup/teardown, NOT [Before]/[After]
4. **Must call `AwaitInputProcessed()`** after every input simulation
5. **Must call `StartMonitoring()`** on signals BEFORE they can be emitted
6. **Scene tree must be properly structured** in .tscn file - _Ready() expects nodes to exist
7. **Must not create assumptions about timing** - use async/await patterns
8. **Must dispose runners properly** to avoid orphan nodes
