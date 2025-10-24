using Godot;
using System;
using OmegaSpiral.Source.UI.Omega;

namespace OmegaSpiral.Source.UI.Omega
{
/// <summary>
/// Base Omega UI orchestrator following SOLID principles.
/// Pure presentation layer - composes shader and text rendering components.
/// Single Responsibility: Component lifecycle and composition only.
/// </summary>
[GlobalClass]
public partial class OmegaUI : Control, IDisposable
{
    /// <summary>
    /// Internal helper for unit tests to inject a mock shader controller.
    /// </summary>
    internal void SetShaderControllerForTest(IOmegaShaderController? controller)
    {
        _shaderController = controller;
    }

    /// <summary>
    /// Internal helper for unit tests to inject a mock text renderer.
    /// </summary>
    internal void SetTextRendererForTest(IOmegaTextRenderer? renderer)
    {
        _textRenderer = renderer;
    }
    /// <summary>
    /// Handles Godot notifications for node lifecycle events.
    /// Ensures disposal is called on NotificationPredelete for robust cleanup.
    /// </summary>
    /// <param name="what">The notification type.</param>
    public override void _Notification(int what)
    {
        base._Notification(what);
        if (what == NotificationPredelete)
        {
            Dispose();
        }
    }
    // Thread safety for Dispose pattern
    private readonly object _disposeLock = new object();
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
        private void InitializeUI()
        {
            CacheRequiredNodes();
            CreateComponents();
            InitializeComponentStates();
        }

    /// <summary>
    /// <summary>
    /// Finalizer for OmegaUI. Only needed if unmanaged resources are added in the future.
    /// Calls <see cref="Dispose(bool)"/> with <see langword="false"/>.
    /// </summary>
    ~OmegaUI()
    {
        Dispose(false);
    }
    /// <summary>
    /// <summary>
    /// Gets or sets the node path for the main text display control.
    /// This is the primary area for rendered text in the UI theme.
    /// </summary>
    [Export]
    public NodePath? TextDisplayPath { get; set; } = default;

    /// <summary>
    /// <summary>
    /// Gets or sets the node path for the phosphor (glow) shader layer.
    /// Used for CRT-style visual effects in the UI theme.
    /// </summary>
    [Export]
    public NodePath? PhosphorLayerPath { get; set; } = default;

    /// <summary>
    /// <summary>
    /// Gets or sets the node path for the scanline shader layer.
    /// Used for CRT-style scanline effects in the UI theme.
    /// </summary>
    [Export]
    public NodePath? ScanlineLayerPath { get; set; } = default;

    /// <summary>
    /// <summary>
    /// Gets or sets the node path for the glitch shader layer.
    /// Used for CRT-style glitch effects in the UI theme.
    /// </summary>
    [Export]
    public NodePath? GlitchLayerPath { get; set; } = default;

    // Component composition (Dependency Inversion Principle)
    private IOmegaShaderController? _shaderController;
    private IOmegaTextRenderer? _textRenderer;

    /// <summary>
    /// Gets the shader controller component for the UI.
    /// Exposed for integration and unit testing.
    /// </summary>
    public IOmegaShaderController? ShaderController => _shaderController;

    /// <summary>
    /// Gets the text renderer component for the UI.
    /// Exposed for integration and unit testing.
    /// </summary>
    public IOmegaTextRenderer? TextRenderer => _textRenderer;

    /// <summary>
    /// <summary>
    /// Gets the phosphor (glow) shader layer if available.
    /// Used for CRT-style glow effects in the UI theme.
    /// </summary>
    protected ColorRect? PhosphorLayer => _phosphorLayer;

    /// <summary>
    /// <summary>
    /// Gets the scanline shader layer if available.
    /// Used for CRT-style scanline effects in the UI theme.
    /// </summary>
    protected ColorRect? ScanlineLayer => _scanlineLayer;

    /// <summary>
    /// <summary>
    /// Gets the glitch shader layer if available.
    /// Used for CRT-style glitch effects in the UI theme.
    /// </summary>
    protected ColorRect? GlitchLayer => _glitchLayer;

    // Node references (cached for performance)
    private RichTextLabel? _textDisplay;

    // CRT shader layers (universal visual style)
    private ColorRect? _phosphorLayer;
    private ColorRect? _scanlineLayer;
    private ColorRect? _glitchLayer;

    private bool _disposed;

    /// Can be overridden by derived classes to customize node requirements.
    /// <summary>
    /// Caches references to required child nodes for the UI theme.
    /// Can be overridden by derived classes to customize node requirements.
    /// PhosphorLayer and TextDisplay are optional - components may not exist for all UI types.
    /// </summary>
    protected virtual void CacheRequiredNodes()
    {
        // Only use inspector-assigned NodePaths for node discovery
        _textDisplay = TextDisplayPath != null && !string.IsNullOrEmpty(TextDisplayPath.ToString())
            ? GetNodeOrNull<RichTextLabel>(TextDisplayPath)
            : null;

        _phosphorLayer = PhosphorLayerPath != null && !string.IsNullOrEmpty(PhosphorLayerPath.ToString())
            ? GetNodeOrNull<ColorRect>(PhosphorLayerPath)
            : null;

        _scanlineLayer = ScanlineLayerPath != null && !string.IsNullOrEmpty(ScanlineLayerPath.ToString())
            ? GetNodeOrNull<ColorRect>(ScanlineLayerPath)
            : null;

        _glitchLayer = GlitchLayerPath != null && !string.IsNullOrEmpty(GlitchLayerPath.ToString())
            ? GetNodeOrNull<ColorRect>(GlitchLayerPath)
            : null;
    }

    /// <summary>
    /// <summary>
    /// Creates component instances using factory methods (Open/Closed Principle).
    /// Used to compose shader and text rendering components for the UI theme.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when component creation fails.</exception>
    private void CreateComponents()
    {
        // Create shader controller if shader layer exists
        var primaryShaderLayer = ResolvePrimaryShaderLayer();
        if (primaryShaderLayer != null)
        {
            try
            {
                _shaderController = CreateShaderController(primaryShaderLayer);
                if (_shaderController == null)
                    throw new InvalidOperationException("ShaderController creation returned null.");
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to create ShaderController: {ex.Message}", ex);
            }
        }

        // Create text renderer if text display exists
        if (_textDisplay != null)
        {
            try
            {
                _textRenderer = CreateTextRenderer(_textDisplay);
                if (_textRenderer == null)
                    throw new InvalidOperationException("TextRenderer creation returned null.");
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to create TextRenderer: {ex.Message}", ex);
            }
        }
    }

    /// <summary>
    /// <summary>
    /// Initializes default states for UI components in the UI theme.
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
    /// <summary>
    /// Factory method for creating the shader controller for the UI theme.
    /// Override in derived classes for custom implementations.
    /// </summary>
    /// <param name="display">The primary shader display layer.</param>
    /// <returns>An instance of IOmegaShaderController.</returns>
    protected virtual IOmegaShaderController CreateShaderController(ColorRect display)
    {
        return new OmegaShaderController(display);
    }

    /// <summary>
    /// <summary>
    /// Factory method for creating the text renderer for the UI theme.
    /// Override in derived classes for custom implementations.
    /// </summary>
    /// <param name="textDisplay">The text display node.</param>
    /// <returns>An instance of IOmegaTextRenderer.</returns>
    protected virtual IOmegaTextRenderer CreateTextRenderer(RichTextLabel textDisplay)
    {
        return new OmegaTextRenderer(textDisplay);
    }

    /// <summary>
    /// <summary>
    /// Appends text to the main display with optional typing animation.
    /// Delegates to the <see cref="TextRenderer"/> component.
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
    /// <summary>
    /// Clears all text from the main display.
    /// Delegates to the <see cref="TextRenderer"/> component.
    /// </summary>
    public void ClearText()
    {
        if (_textRenderer != null)
        {
            _textRenderer.ClearText();
        }
    }

    /// <summary>
    /// <summary>
    /// Applies a visual preset to the shader layers in the UI theme.
    /// Delegates to the <see cref="ShaderController"/> component.
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
    /// <summary>
    /// Performs a pixel dissolve effect on the UI theme.
    /// Delegates to the <see cref="ShaderController"/> component.
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

    /// <summary>
    /// <summary>
    /// Called when the node is about to be removed from the scene tree.
    /// Ensures proper cleanup of resources and components for the UI theme.
    /// </summary>
    public override void _ExitTree()
    {
        base._ExitTree();
        Dispose();
    }

    /// <inheritdoc/>
    /// <summary>
    /// Disposes of resources used by the OmegaUI and its components.
    /// Implements the IDisposable pattern correctly for the UI theme.
    /// </summary>
    /// <param name="disposing">True if called from Dispose(), false if called from finalizer.</param>
    protected override void Dispose(bool disposing)
    {
        if (_disposed)
        {
            base.Dispose(disposing);
            return;
        }

        if (disposing)
        {
            // Dispose or free shader controller
            if (_shaderController is Node shaderControllerNode)
            {
                shaderControllerNode.QueueFree();
            }
            else if (_shaderController is IDisposable shaderControllerDisposable)
            {
                shaderControllerDisposable.Dispose();
            }

            // Dispose or free text renderer
            if (_textRenderer is Node textRendererNode)
            {
                textRendererNode.QueueFree();
            }
            else if (_textRenderer is IDisposable textRendererDisposable)
            {
                textRendererDisposable.Dispose();
            }

            // Queue Godot nodes for freeing (idiomatic Godot pattern)
            _phosphorLayer?.QueueFree();
            _scanlineLayer?.QueueFree();
            _glitchLayer?.QueueFree();
            _textDisplay?.QueueFree();
        }

        _shaderController = null;
        _textRenderer = null;
        _textDisplay = null;
        _phosphorLayer = null;
        _scanlineLayer = null;
        _glitchLayer = null;
        _disposed = true;

        base.Dispose(disposing);
    }

    /// <summary>
    /// Resolves the primary shader layer used to drive shader effects in the UI theme.
    /// Returns the first available CRT overlay.
    /// </summary>
    /// <returns>The primary shader layer, or <see langword="null"/> if none available.</returns>
    private ColorRect? ResolvePrimaryShaderLayer()
    {
        if (_phosphorLayer != null)
        {
            return _phosphorLayer;
        }
        if (_scanlineLayer != null)
        {
            return _scanlineLayer;
        }
        if (_glitchLayer != null)
        {
            return _glitchLayer;
        }
        return null;
    }
}
}
