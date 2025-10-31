# Godot Testing Reference (GdUnit4)

## Overview
This reference consolidates the essential guidelines for writing automated tests in Godot using **GdUnit4** (C# and GDScript). It covers test project setup, test case structure, scene‑runner usage, input simulation, assertions, utilities, and advanced features such as parameterized tests and fuzzing.

---

## 1. Project Setup
- **Godot‑Mono 4.3+** with .NET 8/9 (see `docs/code-guides/testing/csharp-setup.md`).
- Add NuGet packages:
  ```xml
  <PackageReference Include="gdUnit4.api" Version="5.0.0" />
  <PackageReference Include="gdUnit4.test.adapter" Version="3.0.0" />
  <PackageReference Include="gdUnit4.analyzers" Version="1.0.0" />
  ```
- Ensure `GODOT_BIN` environment variable points to the Godot executable (or set it in `.runsettings`).
- Create a `.runsettings` file to configure Godot runtime, environment variables, and GdUnit4 options.
- Build the solution (`dotnet build`) before running tests.

---

## 2. Test Suite & TestCase
- Mark a class with `[TestSuite]` (C#) or extend `GdUnitTestSuite` (GDScript).
- Individual tests are methods annotated with `[TestCase]` (C#) or named `test_*` (GDScript).
- Use **hooks** for setup/teardown:
  - `[Before]` / `before()` – runs before each test.
  - `[After]` / `after()` – runs after each test.
  - `[BeforeAll]` / `before_test()` – runs once per suite.
  - `[AfterAll]` / `after_test()` – runs once after suite.
- Keep tests **fast**; avoid heavy Godot runtime unless required. Use `[RequireGodotRuntime]` only when you need scene processing, signals, or input simulation.

---

## 3. Scene Runner (Integration Tests)
```csharp
ISceneRunner runner = ISceneRunner.Load("res://path/to/scene.tscn");
```
- **Processing**: `await runner.SimulateFrames(uint frames, uint deltaPeerFrame = 0);`
- **Time factor**: `runner.SetTimeFactor(float factor);`
- **Auto‑free**: `var obj = AutoFree(new Node());`
- **Cleanup**: `runner.Free();` (handled automatically when using `AutoFree`).

### Common Pattern
```csharp
[Before]
public void Setup()
{
    _runner = AutoFree(ISceneRunner.Load("res://my_scene.tscn"));
}

[TestCase]
public async Task MyButton_is_visible()
{
    var btn = _runner.FindChild("MyButton");
    AssertObject(btn).IsNotNull();
    AssertThat(btn.Visible).IsTrue();
}
```

---

## 4. Accessors (Scene Interaction)
- `GetProperty<T>(string name)` – read a property from the root node.
- `SetProperty<T>(string name, Variant value)` – set a property.
- `FindChild(string name, bool recursive = true, bool owned = false)` – retrieve any node.
- `Invoke<T>(string methodName, params Variant[] args)` – call a method and get the result.

---

## 5. Input Simulation
### Actions (InputEventAction)
```csharp
runner.SimulateActionPressed("ui_accept");
await runner.AwaitInputProcessed();
```
- `SimulateActionPress` – hold action.
- `SimulateActionRelease` – release action.

### Keys (InputEventKey)
```csharp
runner.SimulateKeyPressed(KeyList.Enter);
await runner.AwaitInputProcessed();
```
- `SimulateKeyPress` – hold key.
- `SimulateKeyRelease` – release key.

### Mouse (InputEventMouse/MouseButton)
```csharp
runner.SimulateMouseButtonPressed(MouseButton.Left, position);
await runner.AwaitInputProcessed();
```
- `SetMousePos`, `GetMousePosition` for cursor control.
- `SimulateMouseMove`, `SimulateMouseMoveRelative`, `SimulateMouseMoveAbsolute`.

### Synchronize Inputs
Always call `await runner.AwaitInputProcessed();` after any simulated input to flush the event queue and ensure the scene processes the input before assertions.

---

## 6. Waiting for Results
- **Functions**: `await runner.AwaitMethod<T>("methodName").IsEqual(expected).WithTimeout(ms);`
- **Signals**: `await runner.AwaitSignal(node).IsEmitted("signalName");`

---

## 7. Assertions (Fluent API)
```csharp
AssertThat(value).IsEqual(expected);
AssertObject(node).IsNotNull();
AssertThat(collection).ContainsExactly(...);
```
- Use `static Assertions;` for concise syntax.
- Chain multiple assertions for readability.

---

## 8. Utilities
- **auto_free()** – automatically frees objects after the test (C#: `AutoFree<T>(obj)`).
- **CreateTempDir / CreateTempFile** – temporary filesystem for file‑based tests.
- **Resource helpers** (`resource_as_array`, `resource_as_string`).

---

## 9. Parameterized & Data‑Driven Tests
### Parameterized TestCases (C#)
```csharp
[TestCase(1,2,3,6)]
[TestCase(3,4,5,12)]
public void Sum(int a,int b,int c,int expect)
{
    AssertThat(a+b+c).IsEqual(expect);
}
```
- Use `TestName` to give each case a readable name.

### DataPoint (C# only)
- Define a static property/method returning `IEnumerable<object[]>` and annotate the test with `[DataPoint(nameof(MyData))]`.
- Supports async generation, lazy `yield return`, and external sources.
- Provides richer data sets than static `TestCase`.

---

## 10. Fuzzing (Advanced)
- Add `[Fuzzer]` attribute to a test method and specify a fuzzing strategy.
- Use `Fuzzed<T>` to retrieve generated values inside the test.
- Combine with `fuzzer_iterations` to control run count.

---

## 11. Best‑Practice Checklist
- ✅ Keep tests deterministic; seed random generators if needed.
- ✅ Use **SceneRunner** only for integration tests; unit tests should avoid Godot runtime.
- ✅ Simulate input then **await input processed** before assertions.
- ✅ Clean up with `AutoFree` or `runner.Free()`.
- ✅ Name tests descriptively; use `TestName` for parameterized cases.
- ✅ Limit the number of frames simulated; use `SimulateFrames(1)` when possible.
- ✅ Prefer `GetProperty`/`SetProperty` over direct node traversal when testing internal state.
- ✅ Run the full test suite via `dotnet test --settings .runsettings`.
- ✅ Review test results in the IDE’s Test Explorer (VS, VS Code, Rider).

---

*This reference is stored in memory as `godot_testing_best_practices.md` for quick lookup.*