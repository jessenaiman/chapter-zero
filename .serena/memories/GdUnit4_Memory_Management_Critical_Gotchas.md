# 🚨 GdUnit4 C# Testing - Critical Memory Management & Orphan Node Prevention

**PURPOSE:** Ωmega Spiral tests had persistent orphan node issues. This memory documents the EXACT patterns to prevent them.

---

## THE CORE PROBLEM: Godot Uses Reference Counting, Not C# GC

```csharp
// ❌ WRONG - Orphans accumulate
private Node _cachedNode;  // Holds reference forever
private GhostStageManager _manager;  // Persists between tests

[Before]
public void Setup()
{
    var runner = ISceneRunner.Load("res://scene.tscn");
    _cachedNode = runner.Scene().GetChild(0);  // Stores reference
    _manager = runner.Scene() as GhostStageManager;  // Persists
}

// ✅ CORRECT - No orphans
private ISceneRunner _Runner;  // Store ONLY the runner

[Before]
public void Setup()
{
    _Runner = ISceneRunner.Load("res://scene.tscn");
}

[After]
public void Teardown()
{
    _Runner?.Dispose();  // ← CRITICAL: Frees ALL scene nodes from Godot tree
}

[TestCase]
public void SomeTest()
{
    var node = _Runner!.Scene().GetChild(0);  // Local variable - scoped!
    var manager = _Runner!.Scene() as GhostStageManager;  // Local - scoped!
    // Both go out of scope when test ends
}
```

**Why:** C# garbage collection doesn't free Godot nodes. Only `ISceneRunner.Dispose()` tells Godot to remove nodes from the scene tree.

---

## CRITICAL RULE #1: ONLY Store ISceneRunner, Nothing Else

```csharp
[TestSuite]
[RequireGodotRuntime]
public class GhostTerminalSceneLoadTests
{
    // ✅ GOOD - Only this
    private ISceneRunner? _Runner;

    // ❌ BAD - Never store these
    // private Node? _CachedNode;
    // private GhostStageManager? _Manager;
    // private Terminal? _Terminal;

    [Before]
    public void Setup()
    {
        _Runner = ISceneRunner.Load("res://source/stages/stage_1_ghost/ghost_terminal.tscn");
    }

    [After]
    public void Teardown()
    {
        _Runner?.Dispose();  // ← Frees ALL nodes at once
    }

    [TestCase]
    public void TestSomething()
    {
        // Use local variables for node references
        var root = _Runner!.Scene();  // Local
        var terminal = root.GetNodeOrNull("Path/To/Terminal");  // Local
        AssertThat(terminal).IsNotNull();
        // Both locals go out of scope here - no orphans
    }
}
```

---

## CRITICAL RULE #2: [Before] and [After] Run ONCE Per Test Method

Each test method gets:
1. Fresh `[Before]` → new scene loaded
2. Test method runs
3. `[After]` → scene disposed
4. Repeat for next test

**This is why duplicate [Before]/[After] breaks everything:**

```csharp
// ❌ WRONG - GdUnit4 only sees FIRST [Before] and [After]
[Before]
public void Setup1() { /* This runs */ }

[Before]
public void Setup2() { /* GdUnit4 ignores this! */ }

[After]
public void Teardown1() { /* This runs */ }

[After]
public void Teardown2() { /* GdUnit4 ignores this! */ }

// Result: Setup2 is never called, Teardown2 is never called
// Scene disposes but Setup2's init never happens = broken state

// ✅ CORRECT - Only one [Before], one [After]
[Before]
public void Setup() { /* All setup here */ }

[After]
public void Teardown() { /* All cleanup here */ }
```

---

## CRITICAL RULE #3: Local Variables for Node References

```csharp
// ❌ WRONG - Storing node references
[TestCase]
public void TestTerminal()
{
    var terminal = _Runner!.Scene().GetNodeOrNull("Path/Terminal");
    _CachedTerminal = terminal;  // ← Stores reference - persists after test!

    // Test runs, but reference stays in memory
    // Teardown disposes runner, but _CachedTerminal still references freed memory
}

// ✅ CORRECT - Local variables
[TestCase]
public void TestTerminal()
{
    var terminal = _Runner!.Scene().GetNodeOrNull("Path/Terminal");  // Local
    AssertThat(terminal).IsNotNull();
    // terminal goes out of scope when method ends
}

[TestCase]
public void AnotherTest()
{
    // New fresh _Runner from [Before]
    var terminal = _Runner!.Scene().GetNodeOrNull("Path/Terminal");  // Fresh node
    AssertThat(terminal).IsNotNull();
}
```

---

## CRITICAL RULE #4: AutoFree() Only for Objects You Create, Not Scene Nodes

```csharp
// ✅ CORRECT - Use AutoFree for custom objects
[TestCase]
public void TestCustomObject()
{
    var customNode = AutoFree(new Node());  // ← I created this
    customNode.Name = "MyNode";
    AssertThat(customNode).IsNotNull();
    // AutoFree handles cleanup
}

// ❌ WRONG - Don't AutoFree scene nodes
[TestCase]
public void TestSceneNode()
{
    var sceneNode = AutoFree(_Runner!.Scene());  // ← Scene runner manages this!
    // Double-free issue: _Runner.Dispose() + AutoFree both try to free same object
}

// ✅ CORRECT - Scene runner manages everything
[TestCase]
public void TestSceneNode()
{
    var sceneNode = _Runner!.Scene();  // ← Managed by _Runner
    AssertThat(sceneNode).IsNotNull();
    // _Runner.Dispose() in [After] handles cleanup
}
```

---

## CRITICAL RULE #5: Never Store Scene Runners Across Tests

```csharp
// ❌ WRONG - Runner persists between tests
private ISceneRunner _GlobalRunner;

[TestSuite]
public class Tests
{
    [Before]
    public void Setup()
    {
        // First test loads
        _GlobalRunner = ISceneRunner.Load("res://scene1.tscn");
    }

    [TestCase]
    public void Test1() { /* ... */ }

    [TestCase]
    public void Test2()
    {
        // Test1's scene is STILL loaded!
        // Now loading Test2's scene
        _GlobalRunner = ISceneRunner.Load("res://scene2.tscn");  // Scene1 + Scene2 both loaded!
    }
}

// ✅ CORRECT - Fresh runner per test
private ISceneRunner? _Runner;

[TestSuite]
public class Tests
{
    [Before]
    public void Setup()
    {
        _Runner = ISceneRunner.Load("res://scene.tscn");
    }

    [After]
    public void Teardown()
    {
        _Runner?.Dispose();  // Scene freed before next test
    }

    [TestCase]
    public void Test1() { /* ... */ }

    [TestCase]
    public void Test2()
    {
        // Test1's scene was disposed by Teardown()
        // _Runner now points to fresh scene2
    }
}
```

---

## CRITICAL RULE #6: Dispose() MUST Be Called in [After]

```csharp
// ❌ WRONG - No Teardown
[TestSuite]
public class Tests
{
    private ISceneRunner? _Runner;

    [Before]
    public void Setup()
    {
        _Runner = ISceneRunner.Load("res://scene.tscn");
    }
    // ← No [After]! Scene NEVER disposed!
    // After 100 tests: 100 scene copies loaded in memory

    [TestCase]
    public void Test1() { }
}

// ✅ CORRECT - Always have [After]
[TestSuite]
public class Tests
{
    private ISceneRunner? _Runner;

    [Before]
    public void Setup()
    {
        _Runner = ISceneRunner.Load("res://scene.tscn");
    }

    [After]
    public void Teardown()
    {
        _Runner?.Dispose();  // ← REQUIRED
    }

    [TestCase]
    public void Test1() { }
}
```

---

## CRITICAL RULE #7: [RequireGodotRuntime] Required for Scene Tests

```csharp
// ❌ WRONG - Missing attribute
[TestSuite]
public class Tests
{
    [TestCase]
    public void TestScene()
    {
        var runner = ISceneRunner.Load("res://scene.tscn");  // Fails silently
        var node = runner.Scene();  // NULL - no Godot runtime
    }
}

// ✅ CORRECT - Add attribute
[TestSuite]
[RequireGodotRuntime]
public class Tests
{
    [TestCase]
    public void TestScene()
    {
        var runner = ISceneRunner.Load("res://scene.tscn");  // Works
        var node = runner.Scene();  // Valid scene root
    }
}
```

---

## CHECKLIST: Preventing Orphan Nodes

- [ ] Only ONE `[Before]` method - consolidate all setup
- [ ] Only ONE `[After]` method - consolidate all teardown
- [ ] `[After]` calls `_Runner?.Dispose()`
- [ ] Class stores ONLY `private ISceneRunner? _Runner`
- [ ] All node references are LOCAL variables in test methods
- [ ] No fields like `private Node? _CachedNode` or `private GhostStageManager? _Manager`
- [ ] `[RequireGodotRuntime]` on test class or methods
- [ ] `AutoFree()` only used for objects YOU create, not scene nodes
- [ ] Each test gets fresh scene via [Before]/[After] cycle

---

## Example: CORRECT Pattern for Your Ghost Terminal Tests

```csharp
[TestSuite]
[RequireGodotRuntime]
public class GhostTerminalSceneLoadTests
{
    // ✅ ONLY store runner
    private ISceneRunner? _Runner;

    // ✅ Consolidate setup
    [Before]
    public void Setup()
    {
        _Runner = ISceneRunner.Load("res://source/stages/stage_1_ghost/ghost_terminal.tscn");
    }

    // ✅ Consolidate teardown
    [After]
    public void Teardown()
    {
        _Runner?.Dispose();
    }

    [TestCase]
    public void GhostTerminal_Loads_Successfully()
    {
        // ✅ Local variables
        var root = _Runner!.Scene();
        AssertThat(root).IsNotNull();
        AssertThat(root).IsInstanceOf<Control>();
    }

    [TestCase]
    public void GhostTerminal_HasGhostStageManagerScript()
    {
        // ✅ Fresh scene, local variables
        var root = _Runner!.Scene();
        var stageManager = root as GhostStageManager;
        AssertThat(stageManager).IsNotNull();
    }
}
// Each test: Setup() → Test() → Teardown()
// No orphans because _Runner.Dispose() called after each test
```

---

## Why Ωmega Spiral Had Orphan Nodes

1. ❌ Stored node references in class fields
2. ❌ Multiple `[Before]/[After]` methods (only first/last ran)
3. ❌ Missing `[After]` method entirely
4. ❌ Forgot to call `_Runner?.Dispose()`
5. ❌ No `[RequireGodotRuntime]` attribute

**Result:** Scene runners accumulated in memory, nodes never freed from Godot tree.

---

## Key Insight

**Godot Scene Tree ≠ C# Objects**

- C# GC collects `ISceneRunner` reference → doesn't matter for Godot
- Godot keeps nodes alive via **reference counting**, independent of C#
- ONLY `ISceneRunner.Dispose()` tells Godot: "Remove this scene from the tree"
- Without it, scenes accumulate FOREVER

Follow the checklist. You'll never have orphan nodes again.
