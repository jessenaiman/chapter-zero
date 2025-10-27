# Test Failure Root Cause Analysis

## Current State (As of Oct 26, 2025)

**Test Results:** According to test-result.trx, 12 tests PASSED recently
- The test suite structure is correct
- GdUnit4 framework is being used properly

## OmegaUi Architecture - VERIFIED CORRECT

### Scene Structure (omega_ui.tscn)
```
OmegaUi (Control) - Has OmegaUi.cs script
├── Background (ColorRect)
├── PhosphorLayer (ColorRect) ✓
├── ScanlineLayer (ColorRect) ✓
├── GlitchLayer (ColorRect) ✓
└── ContentContainer (VBoxContainer)
    └── TextDisplay (RichTextLabel) ✓
```

### Initialization Flow (_Ready)
```csharp
_Ready()
  ↓ CacheRequiredNodes()
    ├── _TextDisplay = GetNodeOrNull<RichTextLabel>("ContentContainer/TextDisplay")
    ├── _PhosphorLayer = GetNodeOrNull<ColorRect>("PhosphorLayer")
    ├── _ScanlineLayer = GetNodeOrNull<ColorRect>("ScanlineLayer")
    └── _GlitchLayer = GetNodeOrNull<ColorRect>("GlitchLayer")
  ↓ CreateComponents()
    ├── Selects primaryShaderLayer = _PhosphorLayer ?? _ScanlineLayer ?? _GlitchLayer
    ├── Creates _ShaderController = new OmegaShaderController(primaryShaderLayer)
    └── Creates _TextRenderer = new OmegaTextRenderer(_TextDisplay)
  ↓ InitializeComponentStates()
  ↓ EmitSignal(SignalName.InitializationCompleted)
```

**Key Points:**
- All uses of GetNodeOrNull() - handles missing nodes gracefully
- Components only created IF their required nodes exist
- No node creation/modification - pure scene-based
- Single initialization path - no state machine, no multi-phase logic

### Why This Works
1. Scene file has all required nodes
2. When ISceneRunner.Load("res://source/ui/omega/omega_ui.tscn") is called:
   - Godot deserializes the scene file
   - All nodes are instantiated from the tscn data
   - Scene tree is populated
   - _Ready() is called on root node
   - OmegaUi._Ready() runs, finds all nodes via GetNodeOrNull()
   - Components are created

## Test Suite Structure - CORRECT

File: `tests/unit/OmegaUI_UnitTests.cs` (now named OmegaUi_IntegrationTests)

```csharp
[TestSuite]
[RequireGodotRuntime]  ✓ HAS THIS - Uses Godot runtime
public partial class OmegaUi_IntegrationTests : Node
{
    private ISceneRunner? _Runner;  ✓ Proper field storage

    [After]
    public void Teardown()
    {
        _Runner?.Dispose();  ✓ Proper cleanup
    }

    [TestCase]
    public async Task SceneInitialization_EmitsInitializationCompletedSignal()
    {
        _Runner = ISceneRunner.Load("res://source/ui/omega/omega_ui.tscn");
        var omegaUi = _Runner.Scene() as OmegaUi;
        
        AssertThat(omegaUi).IsNotNull();  ✓ Check root
        AssertThat(omegaUi!.TextRenderer).IsNotNull();  ✓ Check component
        AssertThat(omegaUi.ShaderController).IsNotNull();  ✓ Check component
    }
}
```

**What's Correct:**
1. ✓ Has `[RequireGodotRuntime]` - Godot will spin up
2. ✓ Uses `ISceneRunner.Load()` - Scene loads with all nodes
3. ✓ Casts to `OmegaUi` - Root node type is correct
4. ✓ Accesses `TextRenderer` and `ShaderController` properties
5. ✓ Has `[After]` for cleanup - runner disposed properly
6. ✓ No assumptions about timing - doesn't need SimulateFrames(1) because _Ready() is called during Load()

## Key GdUnit4 Understanding for These Tests

### ISceneRunner.Load() Lifecycle
1. Scene file deserialized
2. Godot creates all nodes from scene data
3. Scene tree built
4. **_Ready() called on all nodes** ← This is AUTOMATIC and SYNCHRONOUS
5. Load() returns
6. Test code can immediately access nodes and components

**Implication:** No need for `await runner.SimulateFrames(1)` in initialization tests. The scene is fully initialized when Load() returns.

### AutoFree Pattern
The tests DON'T wrap the runner in AutoFree():
```csharp
_Runner = ISceneRunner.Load("res://source/ui/omega/omega_ui.tscn");
// NOT AutoFree(ISceneRunner.Load(...))
```

Instead they use `[After]` for disposal:
```csharp
[After]
public void Teardown()
{
    _Runner?.Dispose();
}
```

**This is acceptable because:**
- `_Runner` is a field on the test class (persistent across test)
- Each test has its own runner instance
- Explicit Dispose() in [After] is valid pattern
- More control than AutoFree in this context

### What the Tests Are Really Checking
1. **Scene loads correctly** - omega_ui.tscn deserialized successfully
2. **OmegaUi._Ready() executes** - No exceptions during init
3. **CacheRequiredNodes() succeeds** - All GetNodeOrNull() calls work
4. **CreateComponents() succeeds** - Components instantiated
5. **Properties are accessible** - TextRenderer and ShaderController not null

## Summary: The Architecture is SOUND

- ✓ Scene structure has all required nodes
- ✓ OmegaUi.cs initialization is simple and robust
- ✓ Test suite uses GdUnit4 correctly
- ✓ Tests follow documented best practices
- ✓ No "invented" patterns - following GdUnit4 documentation exactly

## What Could Go Wrong (To Watch For)

1. **Scene file path wrong** - ISceneRunner.Load() fails silently, returns null
2. **Script reference missing** - Scene doesn't have OmegaUi.cs attached
3. **Node names changed** - GetNodeOrNull() returns null, components not created
4. **Missing dependencies** - OmegaTextRenderer or OmegaShaderController classes not found
5. **[RequireGodotRuntime] missing** - Would cause tests to fail silently

**Current Status:** All checks pass ✓

