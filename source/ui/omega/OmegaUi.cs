using Godot;
using System;
using System.Threading.Tasks;
using OmegaSpiral.Source.Ui.Omega;

namespace OmegaSpiral.Source.Ui.Omega
{
/// <summary>
/// Base Omega Ui orchestrator following SOLID principles.
/// Pure presentation layer - composes shader and text rendering components.
/// Single Responsibility: Component lifecycle and composition only.
/// </summary>
[GlobalClass]
public partial class OmegaUi : Control
{
    /// <summary>
    /// Emitted when the Ui has completed initialization successfully.
    /// All subclasses will automatically emit this signal when ready.
    /// Tests can listen for this signal to know when the Ui is ready for testing.
    /// </summary>
    [Signal]
    public delegate void InitializationCompletedEventHandler();

    /// <summary>
    /// Emitted when Ui initialization fails.
    /// Contains the error message describing what went wrong.
    /// Allows tests and systems to respond gracefully to initialization failures.
    /// </summary>
    [Signal]
    public delegate void InitializationFailedEventHandler(string errorMessage);

    /// <summary>
    /// Internal helper for unit tests to inject a mock shader controller.
    /// </summary>
    internal void SetShaderControllerForTest(IOmegaShaderController? controller)
    {
    _ShaderController = controller;
    }

    /// <summary>
    /// Internal helper for unit tests to inject a mock text renderer.
    /// </summary>
    internal void SetTextRendererForTest(IOmegaTextRenderer? renderer)
    {
    _TextRenderer = renderer;
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
    private readonly object _DisposeLock = new object();
        /// <inheritdoc/>
        public override void _Ready()
        {
            base._Ready();
            try
            {
                InitializeUi();
                EmitSignal(SignalName.InitializationCompleted);
            }
            catch (InvalidOperationException ex)
            {
                GD.PrintErr($"[OmegaUi] Ui initialization failed: {ex.Message}");
                EmitSignal(SignalName.InitializationFailed, ex.Message);
            }
            catch (Exception ex)
            {
                GD.PrintErr($"[OmegaUi] Unexpected error during initialization: {ex.Message}");
                EmitSignal(SignalName.InitializationFailed, ex.Message);
            }
        }

        /// <summary>
        /// Initializes the Ui by caching node references and creating components.
        /// </summary>
        private void InitializeUi()
        {
            CacheRequiredNodes();
            CreateComponents();
            InitializeComponentStates();
        }

    /// <summary>
    /// Finalizer for OmegaUi. Only needed if unmanaged resources are added in the future.
    /// Calls <see cref="Dispose(bool)"/> with <see langword="false"/>.
    /// </summary>
    ~OmegaUi()
    {
        Dispose(false);
    }
    /// <summary>
    /// Gets or sets the node path for the main text display control.
    /// This is the primary area for rendered text in the Ui theme.
    /// </summary>
    [Export]
    public NodePath? TextDisplayPath { get; set; } = default;

    /// <summary>
    /// Gets or sets the node path for the phosphor (glow) shader layer.
    /// Used for CRT-style visual effects in the Ui theme.
    /// </summary>
    [Export]
    public NodePath? PhosphorLayerPath { get; set; } = default;

    /// <summary>
    /// Gets or sets the node path for the scanline shader layer.
    /// Used for CRT-style scanline effects in the Ui theme.
    /// </summary>
    [Export]
    public NodePath? ScanlineLayerPath { get; set; } = default;

    /// <summary>
    /// Gets or sets the node path for the glitch shader layer.
    /// Used for CRT-style glitch effects in the Ui theme.
    /// </summary>
    [Export]
    public NodePath? GlitchLayerPath { get; set; } = default;

    // Component composition (Dependency Inversion Principle)
    private IOmegaShaderController? _ShaderController;
    private IOmegaTextRenderer? _TextRenderer;

    /// <summary>
    /// Gets the shader controller component for the Ui.
    /// Exposed for integration and unit testing.
    /// </summary>
    public IOmegaShaderController? ShaderController => _ShaderController;

    /// <summary>
    /// Gets the text renderer component for the Ui.
    /// Exposed for integration and unit testing.
    /// </summary>
    public IOmegaTextRenderer? TextRenderer => _TextRenderer;

    /// <summary>
    /// Gets the phosphor (glow) shader layer if available.
    /// Used for CRT-style glow effects in the Ui theme.
    /// </summary>
    protected ColorRect? PhosphorLayer => _PhosphorLayer;

    /// <summary>
    /// Gets the scanline shader layer if available.
    /// Used for CRT-style scanline effects in the Ui theme.
    /// </summary>
    protected ColorRect? ScanlineLayer => _ScanlineLayer;

    /// <summary>
    /// Gets the glitch shader layer if available.
    /// Used for CRT-style glitch effects in the Ui theme.
    /// </summary>
    protected ColorRect? GlitchLayer => _GlitchLayer;

    // Node references (cached for performance)
    private RichTextLabel? _TextDisplay;

    // CRT shader layers (universal visual style)
    private ColorRect? _PhosphorLayer;
    private ColorRect? _ScanlineLayer;
    private ColorRect? _GlitchLayer;

    // Button management (lazy-initialized)
    private VBoxContainer? _ButtonList;

    private bool _Disposed;

        /// Can be overridden by derived classes to customize node requirements.
        /// <summary>
        /// Caches references to required child nodes for the Ui theme.
        /// Can be overridden by derived classes to customize node requirements.
        /// PhosphorLayer and TextDisplay are optional - components may not exist for all Ui types.
        /// </summary>
        protected virtual void CacheRequiredNodes()
    {
        // Only use inspector-assigned NodePaths for node discovery
        _TextDisplay = TextDisplayPath != null && !string.IsNullOrEmpty(TextDisplayPath.ToString())
            ? GetNodeOrNull<RichTextLabel>(TextDisplayPath)
            : null;

        _PhosphorLayer = PhosphorLayerPath != null && !string.IsNullOrEmpty(PhosphorLayerPath.ToString())
            ? GetNodeOrNull<ColorRect>(PhosphorLayerPath)
            : null;

        _ScanlineLayer = ScanlineLayerPath != null && !string.IsNullOrEmpty(ScanlineLayerPath.ToString())
            ? GetNodeOrNull<ColorRect>(ScanlineLayerPath)
            : null;

        _GlitchLayer = GlitchLayerPath != null && !string.IsNullOrEmpty(GlitchLayerPath.ToString())
            ? GetNodeOrNull<ColorRect>(GlitchLayerPath)
            : null;
    }

    /// <summary>
    /// Creates component instances using factory methods (Open/Closed Principle).
    /// Used to compose shader and text rendering components for the Ui theme.
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
                _ShaderController = CreateShaderController(primaryShaderLayer);
                if (_ShaderController == null)
                    throw new InvalidOperationException("ShaderController creation returned null.");
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to create ShaderController: {ex.Message}", ex);
            }
        }

            // Create text renderer if text display exists
            if (_TextDisplay != null)
        {
            try
            {
                _TextRenderer = CreateTextRenderer(_TextDisplay);
                if (_TextRenderer == null)
                    throw new InvalidOperationException("TextRenderer creation returned null.");
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to create TextRenderer: {ex.Message}", ex);
            }
        }
    }

    /// <summary>
    /// Initializes default states for Ui components in the Ui theme.
    /// Can be overridden by derived classes to customize initial state.
    /// </summary>
    protected virtual void InitializeComponentStates()
    {
        if (_TextDisplay != null)
        {
                _TextDisplay.Text = string.Empty;
            _TextDisplay.Modulate = Colors.White;
        }
    }


    /// <summary>
    /// Factory method for creating the shader controller for the Ui theme.
    /// Override in derived classes for custom implementations.
    /// </summary>
    /// <param name="display">The primary shader display layer.</param>
    /// <returns>An instance of IOmegaShaderController.</returns>
    protected virtual IOmegaShaderController CreateShaderController(ColorRect display)
    {
        return new OmegaShaderController(display);
    }

    /// <summary>
    /// Factory method for creating the text renderer for the Ui theme.
    /// Override in derived classes for custom implementations.
    /// </summary>
    /// <param name="textDisplay">The text display node.</param>
    /// <returns>An instance of IOmegaTextRenderer.</returns>
    protected virtual IOmegaTextRenderer CreateTextRenderer(RichTextLabel textDisplay)
    {
        return new OmegaTextRenderer(textDisplay);
    }

    /// <summary>
    /// Appends text to the main display with optional typing animation.
    /// Delegates to the <see cref="TextRenderer"/> component.
    /// </summary>
    /// <param name="text">The text to append.</param>
    /// <param name="typingSpeed">Characters per second for typing animation.</param>
    /// <param name="delayBeforeStart">Delay in seconds before starting.</param>
    /// <returns>A task representing the async operation.</returns>
    public async Task AppendTextAsync(string text, float typingSpeed = 30f, float delayBeforeStart = 0f)
    {
    if (_TextRenderer == null)
        {
            GD.PushWarning("[OmegaUi] Cannot append text - TextRenderer not initialized.");
            return;
        }

    await _TextRenderer.AppendTextAsync(text, typingSpeed, delayBeforeStart);
    }

    /// <summary>
    /// Clears all text from the main display.
    /// Delegates to the <see cref="TextRenderer"/> component.
    /// </summary>
    public void ClearText()
    {
        if (_TextRenderer != null)
        {
            _TextRenderer.ClearText();
        }
    }

    /// <summary>
    /// Applies a visual preset to the shader layers in the Ui theme.
    /// Delegates to the <see cref="ShaderController"/> component.
    /// </summary>
    /// <param name="presetName">The name of the preset to apply.</param>
    /// <returns>A task representing the async operation.</returns>
    public async Task ApplyVisualPresetAsync(string presetName)
    {
        if (_ShaderController == null)
        {
            GD.PushWarning("[OmegaUi] Cannot apply visual preset - ShaderController not initialized.");
            return;
        }

    await _ShaderController.ApplyVisualPresetAsync(presetName);
    }

    /// <summary>
    /// Performs a pixel dissolve effect on the Ui theme.
    /// Delegates to the <see cref="ShaderController"/> component.
    /// </summary>
    /// <param name="durationSeconds">The duration of the effect.</param>
    /// <returns>A task representing the async operation.</returns>
    public async Task PixelDissolveAsync(float durationSeconds = 2.5f)
    {
        if (_ShaderController == null)
        {
            GD.PushWarning("[OmegaUi] Cannot perform dissolve - ShaderController not initialized.");
            return;
        }

    await _ShaderController.PixelDissolveAsync(durationSeconds);
    }

    /// <summary>
    /// Factory method for creating buttons within this UI.
    /// OmegaUI controls button lifecycle and layout integration.
    /// Subclasses can override to customize button styling and behavior.
    /// </summary>
    /// <param name="buttonName">The name to assign to the button node.</param>
    /// <param name="buttonText">The display text for the button.</param>
    /// <returns>A new Button node ready for configuration by the caller.</returns>
    protected virtual Button CreateButton(string buttonName, string buttonText = "")
    {
        var button = new Button
        {
            Name = buttonName,
            Text = buttonText,
            CustomMinimumSize = new Vector2(0, 40) // Consistent button height
        };

        return button;
    }

    /// <summary>
    /// Gets or creates a managed button list container.
    /// OmegaUI owns and manages the button collection lifecycle.
    /// Subclasses request this list and add buttons to it without managing the container itself.
    /// The button list is lazily created on first request.
    /// </summary>
    /// <returns>A VBoxContainer for holding buttons, managed by OmegaUI.</returns>
    protected VBoxContainer GetOrCreateButtonList()
    {
        if (_ButtonList == null)
        {
            _ButtonList = new VBoxContainer
            {
                Name = "ButtonList",
                SizeFlagsHorizontal = Control.SizeFlags.ExpandFill,
                SizeFlagsVertical = Control.SizeFlags.ExpandFill
            };
            AddChild(_ButtonList);
        }

        return _ButtonList;
    }

    /// <summary>
    /// Called when the node is about to be removed from the scene tree.
    /// Ensures proper cleanup of resources and components for the Ui theme.
    /// </summary>
    public override void _ExitTree()
    {
        base._ExitTree();
        Dispose();
    }

    /// <inheritdoc/>
    /// <summary>
    /// Disposes of resources used by the OmegaUi and its components.
    /// Implements the IDisposable pattern correctly for the Ui theme.
    /// </summary>
    /// <param name="disposing">True if called from Dispose(), false if called from finalizer.</param>
    protected override void Dispose(bool disposing)
    {
        if (_Disposed)
        {
            base.Dispose(disposing);
            return;
        }

        if (disposing)
        {
            // Dispose managed components (NOT scene tree nodes - Godot handles those)
            // Only dispose if they implement IDisposable and are NOT part of scene tree
            if (_ShaderController is IDisposable shaderControllerDisposable && _ShaderController is not Node)
            {
                shaderControllerDisposable.Dispose();
            }

            if (_TextRenderer is IDisposable textRendererDisposable && _TextRenderer is not Node)
            {
                textRendererDisposable.Dispose();
            }

            // DO NOT call QueueFree on child nodes - Godot automatically frees children
            // when parent is freed. Manually freeing causes orphan node warnings.
        }

        // Clear references
        _ShaderController = null;
        _TextRenderer = null;
        _TextDisplay = null;
        _PhosphorLayer = null;
        _ScanlineLayer = null;
        _GlitchLayer = null;
        _ButtonList = null;
        _Disposed = true;

        base.Dispose(disposing);
    }

    /// <summary>
    /// Resolves the primary shader layer used to drive shader effects in the Ui theme.
    /// Returns the first available CRT overlay.
    /// </summary>
    /// <returns>The primary shader layer, or <see langword="null"/> if none available.</returns>
    private ColorRect? ResolvePrimaryShaderLayer()
    {
        if (_PhosphorLayer != null)
        {
            return _PhosphorLayer;
        }
        if (_ScanlineLayer != null)
        {
            return _ScanlineLayer;
        }
        if (_GlitchLayer != null)
        {
            return _GlitchLayer;
        }
        return null;
    }
}
}
