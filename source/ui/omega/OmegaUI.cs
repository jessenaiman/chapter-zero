using Godot;
using System;
using OmegaSpiral.Source.UI.Omega;

namespace OmegaSpiral.Source.UI.Omega;

/// <summary>
/// Base Omega UI orchestrator following SOLID principles.
/// Pure presentation layer - composes shader and text rendering components.
/// Single Responsibility: Component lifecycle and composition only.
/// </summary>
[GlobalClass]
public partial class OmegaUI : Control
{
    // Component composition (Dependency Inversion Principle)
    private IOmegaShaderController? _shaderController;
    private IOmegaTextRenderer? _textRenderer;

    // Protected accessors for derived classes (Open/Closed Principle)
    protected IOmegaShaderController? ShaderController => _shaderController;
    protected IOmegaTextRenderer? TextRenderer => _textRenderer;

    // Node references (cached for performance)
    private ColorRect? _primaryShaderLayer;
    private RichTextLabel? _textDisplay;

    /// <inheritdoc/>
    public override void _Ready()
    {
        base._Ready();

        try
        {
            InitializeUI();
        }
        catch (InvalidOperationException ex)
        {
            GD.PrintErr($"[OmegaUI] UI initialization failed: {ex.Message}");
            throw;
        }
        catch (Exception ex)
        {
            GD.PrintErr($"[OmegaUI] Unexpected error during initialization: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Initializes the UI by caching node references and creating components.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when required nodes are missing.</exception>
    private void InitializeUI()
    {
        CacheRequiredNodes();
        CreateComponents();
        InitializeComponentStates();
    }

    /// <summary>
    /// Caches references to required child nodes.
    /// Can be overridden by derived classes to customize node requirements.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when required nodes are missing.</exception>
    protected virtual void CacheRequiredNodes()
    {
        // Required nodes for base Omega UI
        _primaryShaderLayer = GetNodeOrNull<ColorRect>("PhosphorLayer");
        if (_primaryShaderLayer == null)
            throw new InvalidOperationException("PhosphorLayer node is required but not found.");

        _textDisplay = GetNodeOrNull<RichTextLabel>("TextDisplay");
        if (_textDisplay == null)
            throw new InvalidOperationException("TextDisplay node is required but not found.");
    }

    /// <summary>
    /// Creates component instances using factory methods (Open/Closed Principle).
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when component creation fails.</exception>
    private void CreateComponents()
    {
        try
        {
            _shaderController = CreateShaderController(_primaryShaderLayer!);
            if (_shaderController == null)
                throw new InvalidOperationException("ShaderController creation returned null.");
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to create ShaderController: {ex.Message}", ex);
        }

        try
        {
            _textRenderer = CreateTextRenderer(_textDisplay!);
            if (_textRenderer == null)
                throw new InvalidOperationException("TextRenderer creation returned null.");
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to create TextRenderer: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Initializes default states for UI components.
    /// Can be overridden by derived classes to customize initial state.
    /// </summary>
    protected virtual void InitializeComponentStates()
    {
        if (_textDisplay != null)
        {
            _textDisplay.Text = string.Empty;
            _textDisplay.Modulate = Colors.White;
        }
    }

    /// <summary>
    /// Factory method for creating shader controller (Open/Closed Principle).
    /// Override in derived classes for custom implementations.
    /// </summary>
    /// <param name="display">The primary shader display layer.</param>
    /// <returns>An instance of IOmegaShaderController.</returns>
    protected virtual IOmegaShaderController CreateShaderController(ColorRect display)
    {
        return new OmegaShaderController(display);
    }

    /// <summary>
    /// Factory method for creating text renderer (Open/Closed Principle).
    /// Override in derived classes for custom implementations.
    /// </summary>
    /// <param name="textDisplay">The text display node.</param>
    /// <returns>An instance of IOmegaTextRenderer.</returns>
    protected virtual IOmegaTextRenderer CreateTextRenderer(RichTextLabel textDisplay)
    {
        return new OmegaTextRenderer(textDisplay);
    }

    /// <summary>
    /// Appends text to the display with optional typing animation.
    /// Delegates to TextRenderer component (Single Responsibility).
    /// </summary>
    /// <param name="text">The text to append.</param>
    /// <param name="typingSpeed">Characters per second for typing animation.</param>
    /// <param name="delayBeforeStart">Delay in seconds before starting.</param>
    /// <returns>A task representing the async operation.</returns>
    public async System.Threading.Tasks.Task AppendTextAsync(string text, float typingSpeed = 30f, float delayBeforeStart = 0f)
    {
        if (_textRenderer == null)
        {
            GD.PushWarning("[OmegaUI] Cannot append text - TextRenderer not initialized.");
            return;
        }

        await _textRenderer.AppendTextAsync(text, typingSpeed, delayBeforeStart);
    }

    /// <summary>
    /// Clears all text from the display.
    /// Delegates to TextRenderer component (Single Responsibility).
    /// </summary>
    public void ClearText()
    {
        if (_textRenderer != null)
        {
            _textRenderer.ClearText();
        }
    }

    /// <summary>
    /// Applies a visual preset to the shaders.
    /// Delegates to ShaderController component (Single Responsibility).
    /// </summary>
    /// <param name="presetName">The name of the preset to apply.</param>
    /// <returns>A task representing the async operation.</returns>
    public async System.Threading.Tasks.Task ApplyVisualPresetAsync(string presetName)
    {
        if (_shaderController == null)
        {
            GD.PushWarning("[OmegaUI] Cannot apply visual preset - ShaderController not initialized.");
            return;
        }

        await _shaderController.ApplyVisualPresetAsync(presetName);
    }

    /// <summary>
    /// Performs a pixel dissolve effect.
    /// Delegates to ShaderController component (Single Responsibility).
    /// </summary>
    /// <param name="durationSeconds">The duration of the effect.</param>
    /// <returns>A task representing the async operation.</returns>
    public async System.Threading.Tasks.Task PixelDissolveAsync(float durationSeconds = 2.5f)
    {
        if (_shaderController == null)
        {
            GD.PushWarning("[OmegaUI] Cannot perform dissolve - ShaderController not initialized.");
            return;
        }

        await _shaderController.PixelDissolveAsync(durationSeconds);
    }
}
