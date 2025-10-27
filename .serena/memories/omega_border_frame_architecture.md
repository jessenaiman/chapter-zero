# Omega UI Border Frame Architecture

## Current State
- BorderFrame creation logic is in OmegaUi.CreateBorderFrame() (60 lines)
- BUT: CreateBorderFrame() is NEVER CALLED in _Ready()
- It has complex shader setup, animation params, three thread colors from OmegaSpiralColors
- Tests expect BorderFrame to exist (many BorderFrame_* tests in OmegaUiTests.cs)

## Current Responsibilities in OmegaUi
1. CacheRequiredNodes() - caches TextDisplay, PhosphorLayer, ScanlineLayer, GlitchLayer
2. CreateComponents() - creates ShaderController, TextRenderer
3. InitializeComponentStates() - sets defaults on components
4. (Missing) CreateBorderFrame() - exists but never called!

## The Border Frame Problem
- 60+ lines of shader setup code crammed in OmegaUi
- Mixed concerns: node creation, shader loading, parameter setup
- Should be extracted to OmegaBorderFrame class
- CreateBorderFrame() would be replaced with: _BorderFrame = new OmegaBorderFrame()

## Should You Create OmegaBorderFrame.cs?

YES - Follows the system pattern:
- OmegaTextRenderer.cs - handles text rendering
- OmegaShaderController.cs - handles shader effects
- OmegaBorderFrame.cs - handles spiral border (MISSING)

Benefits:
1. Single Responsibility - border logic isolated
2. Testable - can unit test border frame setup separately
3. Consistent - follows OmegaTextRenderer/OmegaShaderController pattern
4. Reusable - any UI can use it

Suggested API:
```csharp
public class OmegaBorderFrame : ColorRect
{
    public OmegaBorderFrame();  // Sets up shader, colors, animations
    public void ConfigureAnimationSpeed(float rotationSpeed, float waveSpeed);
    public void UpdateThreadColors(Color light, Color shadow, Color ambition);
}
```

Then in OmegaUi._Ready():
```csharp
protected virtual void CreateBorderFrame()
{
    var existing = GetNodeOrNull<ColorRect>("BorderFrame");
    if (existing == null)
    {
        var frame = new OmegaBorderFrame();
        AddChild(frame);
    }
}
```

This would be called in CreateComponents() after line 276.
