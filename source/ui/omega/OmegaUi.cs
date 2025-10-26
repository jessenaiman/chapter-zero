using Godot;
using System;
using System.Threading.Tasks;
using OmegaSpiral.Source.Ui.Omega;

namespace OmegaSpiral.Source.Ui.Omega
{
/// <summary>
/// Tracks the initialization phase of OmegaUi.
/// Allows tests and external systems to verify component readiness.
/// </summary>
public enum OmegaUiInitializationState
{
    /// <summary>OmegaUi created but not initialized.</summary>
    Uninitialized,

    /// <summary>Required nodes have been cached (or created if missing).</summary>
    NodesCached,

    /// <summary>Border frame has been created and configured.</summary>
    BorderFrameCreated,

    /// <summary>Shader and text components have been created.</summary>
    ComponentsCreated,

    /// <summary>Component states have been initialized.</summary>
    Initialized,

    /// <summary>Initialization failed - see InitializationError for details.</summary>
    Failed
}

/// <summary>
/// Base Omega Ui orchestrator following SOLID principles.
/// Pure presentation layer - composes shader and text rendering components.
/// Single Responsibility: Component lifecycle and composition only.
///
/// ARCHITECTURE:
/// - Explicit initialization via Initialize() instead of _Ready() magic
/// - Testable via InitializationState tracking
/// - Node creation via virtual factory methods (overridable for testing)
/// - _Ready() calls Initialize() for production use only
/// </summary>
[GlobalClass]
public partial class OmegaUi : Control
{
    /// <summary>
    /// Emitted when the Ui has completed initialization successfully.
    /// All subclasses will automatically emit this signal when ready.
    /// </summary>
    [Signal]
    public delegate void InitializationCompletedEventHandler();

    /// <summary>
    /// Emitted when Ui initialization fails.
    /// Contains the error message describing what went wrong.
    /// Allows systems to respond gracefully to initialization failures.
    /// </summary>
    [Signal]
    public delegate void InitializationFailedEventHandler(string errorMessage);

    /// <summary>
    /// Current initialization phase. Tests and external systems can inspect this.
    /// </summary>
    public OmegaUiInitializationState InitializationState { get; private set; } = OmegaUiInitializationState.Uninitialized;

    /// <summary>
    /// If initialization failed, contains the error message. Null otherwise.
    /// </summary>
    public string? InitializationError { get; private set; }

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

    /// <summary>
    /// Called by Godot when the node enters the scene tree.
    /// Performs synchronous initialization following Godot's standard lifecycle.
    /// </summary>
    public override void _Ready()
    {
        base._Ready();

        if (InitializationState != OmegaUiInitializationState.Uninitialized)
        {
            GD.PushWarning($"[OmegaUi] Already initialized, skipping _Ready initialization (state: {InitializationState})");
            return;
        }

        GD.Print("=== [OmegaUi] Starting initialization ===");

        try
        {
            // Phase 1: Cache or create nodes
            GD.Print("[OmegaUi] Phase 1: Caching required nodes");
            CacheRequiredNodes();
            InitializationState = OmegaUiInitializationState.NodesCached;
            GD.Print("[OmegaUi] Phase 1 complete: NodesCached");

            // Phase 2: Create border frame
            GD.Print("[OmegaUi] Phase 2: Creating BorderFrame");
            CreateBorderFrame();
            InitializationState = OmegaUiInitializationState.BorderFrameCreated;
            GD.Print("[OmegaUi] Phase 2 complete: BorderFrameCreated");

            // Phase 3: Create components (ShaderController, TextRenderer)
            GD.Print("[OmegaUi] Phase 3: Creating components");
            CreateComponents();
            InitializationState = OmegaUiInitializationState.ComponentsCreated;
            GD.Print("[OmegaUi] Phase 3 complete: ComponentsCreated");

            // Phase 4: Initialize component states
            GD.Print("[OmegaUi] Phase 4: Initializing component states");
            InitializeComponentStates();
            InitializationState = OmegaUiInitializationState.Initialized;
            GD.Print("[OmegaUi] Phase 4 complete: Initialized");

            GD.Print("=== [OmegaUi] Initialization SUCCESS ===");
            EmitSignal(SignalName.InitializationCompleted);
        }
        catch (InvalidOperationException ex)
        {
            InitializationState = OmegaUiInitializationState.Failed;
            InitializationError = ex.Message;
            GD.PushError($"[OmegaUi] Initialization FAILED at state {InitializationState}: {ex.Message}");
            GD.PushError($"[OmegaUi] Stack trace: {ex.StackTrace}");
            EmitSignal(SignalName.InitializationFailed, ex.Message);
            throw;
        }
        catch (Exception ex)
        {
            InitializationState = OmegaUiInitializationState.Failed;
            InitializationError = ex.Message;
            GD.PushError($"[OmegaUi] Unexpected error during initialization at state {InitializationState}: {ex.Message}");
            GD.PushError($"[OmegaUi] Stack trace: {ex.StackTrace}");
            EmitSignal(SignalName.InitializationFailed, ex.Message);
            throw new InvalidOperationException($"Initialization failed: {ex.Message}", ex);
        }
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
    /// Defaults to "ContentContainer/TextDisplay" but can be overridden in derived classes.
    /// </summary>
    [Export]
    public NodePath? TextDisplayPath { get; set; } = "ContentContainer/TextDisplay";

    /// <summary>
    /// Gets or sets the node path for the phosphor (glow) shader layer.
    /// Used for CRT-style visual effects in the Ui theme.
    /// Defaults to "PhosphorLayer" but can be overridden in derived classes.
    /// </summary>
    [Export]
    public NodePath? PhosphorLayerPath { get; set; } = "PhosphorLayer";

    /// <summary>
    /// Gets or sets the node path for the scanline shader layer.
    /// Used for CRT-style scanline effects in the Ui theme.
    /// Defaults to "ScanlineLayer" but can be overridden in derived classes.
    /// </summary>
    [Export]
    public NodePath? ScanlineLayerPath { get; set; } = "ScanlineLayer";

    /// <summary>
    /// Gets or sets the node path for the glitch shader layer.
    /// Used for CRT-style glitch effects in the Ui theme.
    /// Defaults to "GlitchLayer" but can be overridden in derived classes.
    /// </summary>
    [Export]
    public NodePath? GlitchLayerPath { get; set; } = "GlitchLayer";

    // Component composition (Dependency Inversion Principle)
    private IOmegaShaderController? _ShaderController;
    private IOmegaTextRenderer? _TextRenderer;

    /// <summary>
    /// Gets the shader controller component for the Ui.
    /// </summary>
    public IOmegaShaderController? ShaderController => _ShaderController;

    /// <summary>
    /// Gets the text renderer component for the Ui.
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

    /// <summary>
    /// Gets or sets the rotation speed of the spiral border animation.
    /// Range: 0.0 (static) to 2.0 (fast). Default: 0.05 (very slow).
    /// A value of 0.0 stops the spiral rotation completely.
    /// </summary>
    public float BorderRotationSpeed
    {
        get
        {
            var borderFrame = GetNodeOrNull<ColorRect>("BorderFrame");
            if (borderFrame?.Material is ShaderMaterial shaderMaterial)
            {
                return (float)shaderMaterial.GetShaderParameter("rotation_speed");
            }
            return 0.05f; // Default value
        }
        set
        {
            var borderFrame = GetNodeOrNull<ColorRect>("BorderFrame");
            if (borderFrame?.Material is ShaderMaterial shaderMaterial)
            {
                shaderMaterial.SetShaderParameter("rotation_speed", Mathf.Clamp(value, 0.0f, 2.0f));
            }
        }
    }

    /// <summary>
    /// Gets or sets the wave flow speed of the border animation.
    /// Range: 0.0 (static) to 5.0 (fast). Default: 0.8 (gentle flow).
    /// Controls how fast the wavelike particles flow along the energy streams.
    /// </summary>
    public float BorderWaveSpeed
    {
        get
        {
            var borderFrame = GetNodeOrNull<ColorRect>("BorderFrame");
            if (borderFrame?.Material is ShaderMaterial shaderMaterial)
            {
                return (float)shaderMaterial.GetShaderParameter("wave_speed");
            }
            return 0.8f; // Default value
        }
        set
        {
            var borderFrame = GetNodeOrNull<ColorRect>("BorderFrame");
            if (borderFrame?.Material is ShaderMaterial shaderMaterial)
            {
                shaderMaterial.SetShaderParameter("wave_speed", Mathf.Clamp(value, 0.0f, 5.0f));
            }
        }
    }

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
        GD.Print("[OmegaUi] Caching required nodes...");

        // Create nodes if they don't exist (for testing or when not using scene files)
        EnsureRequiredNodesExist();

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

        // Log what was found
        GD.Print($"[OmegaUi] Node cache results: TextDisplay={_TextDisplay != null}, " +
                 $"Phosphor={_PhosphorLayer != null}, Scanline={_ScanlineLayer != null}, Glitch={_GlitchLayer != null}");

        // Also log Background color
        var background = GetNodeOrNull<ColorRect>("Background");
        if (background != null)
        {
            GD.Print($"[OmegaUi] Background color: {background.Color}");
        }

        // Wire in design system colors to shader layers from OmegaSpiralColors
        if (_PhosphorLayer != null)
        {
            _PhosphorLayer.Color = OmegaSpiralColors.PhosphorGlow;
            GD.Print($"[OmegaUi] PhosphorLayer color set to {OmegaSpiralColors.PhosphorGlow}");
        }

        if (_ScanlineLayer != null)
        {
            _ScanlineLayer.Color = OmegaSpiralColors.ScanlineOverlay;
            GD.Print($"[OmegaUi] ScanlineLayer color set to {OmegaSpiralColors.ScanlineOverlay}");
        }

        if (_GlitchLayer != null)
        {
            _GlitchLayer.Color = OmegaSpiralColors.GlitchDistortion;
            GD.Print($"[OmegaUi] GlitchLayer color set to {OmegaSpiralColors.GlitchDistortion}");
        }

        GD.Print("[OmegaUi] Node caching complete");
    }

    /// <summary>
    /// Creates the animated spiral border with three intermingling energy streams.
    /// Uses shader-based rendering for wavelike particle trails in Silver, Golden, and Crimson.
    /// Border animation speed and appearance can be controlled via shader parameters.
    /// If BorderFrame already exists in scene, configures its shader properties.
    ///
    /// Virtual so tests can override to provide a mock BorderFrame if needed.
    /// BorderFrame is OPTIONAL - if shader cannot be loaded, logs error but continues.
    /// </summary>
    protected virtual void CreateBorderFrame()
    {
        GD.Print("[OmegaUi] Phase 2: CreateBorderFrame starting");

        // Check if BorderFrame already exists in the scene (loaded from .tscn)
        var existingBorderFrame = GetNodeOrNull<ColorRect>("BorderFrame");
        if (existingBorderFrame != null)
        {
            GD.Print("[OmegaUi] BorderFrame found in scene - configuring shader");
            try
            {
                ConfigureBorderShader(existingBorderFrame);
                GD.Print("[OmegaUi] BorderFrame shader configured successfully");
            }
            catch (Exception ex)
            {
                GD.PushError($"[OmegaUi] Failed to configure existing BorderFrame: {ex.Message}");
            }
            return;
        }

        GD.Print("[OmegaUi] Creating BorderFrame programmatically");

        try
        {
            // Create BorderFrame programmatically with shader
            var borderFrame = new ColorRect
            {
                Name = "BorderFrame",
                SizeFlagsHorizontal = Control.SizeFlags.ExpandFill,
                SizeFlagsVertical = Control.SizeFlags.ExpandFill,
                ZIndex = 10, // Render above background layers but below content
                MouseFilter = MouseFilterEnum.Ignore // Don't intercept mouse events
            };

            // Anchor to fill entire parent control
            borderFrame.AnchorLeft = 0;
            borderFrame.AnchorTop = 0;
            borderFrame.AnchorRight = 1;
            borderFrame.AnchorBottom = 1;

            // Load and configure the spiral border shader
            var shader = GD.Load<Shader>("res://source/shaders/spiral_border.gdshader");
            if (shader == null)
            {
                GD.PushError("[OmegaUi] WARNING: spiral_border.gdshader not found at res://source/shaders/spiral_border.gdshader - BorderFrame will not render");
                return;
            }

            var shaderMaterial = new ShaderMaterial { Shader = shader };

            // Set three thread colors from design system
            shaderMaterial.SetShaderParameter("light_thread", OmegaSpiralColors.LightThread);
            shaderMaterial.SetShaderParameter("shadow_thread", OmegaSpiralColors.ShadowThread);
            shaderMaterial.SetShaderParameter("ambition_thread", OmegaSpiralColors.AmbitionThread);

            // Set animation parameters (slow, subtle animation by default)
            shaderMaterial.SetShaderParameter("rotation_speed", 0.05f); // Very slow spiral
            shaderMaterial.SetShaderParameter("wave_speed", 0.8f);      // Gentle wave flow
            shaderMaterial.SetShaderParameter("wave_frequency", 8.0f);  // Moderate wavelength density
            shaderMaterial.SetShaderParameter("wave_amplitude", 0.25f); // Subtle wave height
            shaderMaterial.SetShaderParameter("border_width", 0.015f);  // ~1.5% of viewport
            shaderMaterial.SetShaderParameter("glow_intensity", 1.2f);  // Moderate glow

            borderFrame.Material = shaderMaterial;

            // Add BorderFrame as sibling to background layers (visual overlay only)
            AddChild(borderFrame);
            MoveChild(borderFrame, 4); // After Background, Phosphor, Scanline, Glitch layers

            GD.Print("[OmegaUi] BorderFrame created successfully");
        }
        catch (Exception ex)
        {
            GD.PushError($"[OmegaUi] Failed to create BorderFrame: {ex.Message}");
            // Continue anyway - BorderFrame is optional
        }
    }

    /// <summary>
    /// Configures the spiral border shader with design system colors and animation parameters.
    /// Allows runtime adjustment of border appearance and animation speed.
    /// </summary>
    /// <param name="borderFrame">The ColorRect containing the spiral border shader.</param>
    /// <exception cref="InvalidOperationException">Thrown when BorderFrame doesn't have a ShaderMaterial.</exception>
    private void ConfigureBorderShader(ColorRect borderFrame)
    {
        if (borderFrame.Material is not ShaderMaterial shaderMaterial)
        {
            GD.PushError("[OmegaUi] WARNING: BorderFrame does not have a ShaderMaterial - cannot configure shader.");
            return;
        }

        GD.Print("[OmegaUi] Configuring BorderFrame shader with design system colors");

        // Update thread colors from design system
        shaderMaterial.SetShaderParameter("light_thread", OmegaSpiralColors.LightThread);
        shaderMaterial.SetShaderParameter("shadow_thread", OmegaSpiralColors.ShadowThread);
        shaderMaterial.SetShaderParameter("ambition_thread", OmegaSpiralColors.AmbitionThread);

        GD.Print("[OmegaUi] BorderFrame shader configured successfully");
    }

    /// <summary>
    /// Creates component instances using factory methods (Open/Closed Principle).
    /// Used to compose shader and text rendering components for the Ui theme.
    ///
    /// Virtual so tests can override CreateShaderController() and CreateTextRenderer()
    /// to inject mock components.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when component creation fails.</exception>
    protected virtual void CreateComponents()
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
    ///
    /// Tests can verify this phase was completed by checking InitializationState == Initialized.
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

    await _TextRenderer.AppendTextAsync(text, typingSpeed, delayBeforeStart).ConfigureAwait(false);
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

    await _ShaderController.ApplyVisualPresetAsync(presetName).ConfigureAwait(false);
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

    await _ShaderController.PixelDissolveAsync(durationSeconds).ConfigureAwait(false);
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

    /// <summary>
    /// Ensures that all required nodes exist, creating them if necessary.
    /// Virtual so tests can override to inject mock nodes instead of creating real ones.
    /// This allows the class to work both with scene files and direct instantiation in tests.
    /// </summary>
    protected virtual void EnsureRequiredNodesExist()
    {
        // Create Background if it doesn't exist (dark base for overlay layers)
        if (GetNodeOrNull<ColorRect>("Background") == null)
        {
            var background = new ColorRect
            {
                Name = "Background",
                SizeFlagsHorizontal = Control.SizeFlags.ExpandFill,
                SizeFlagsVertical = Control.SizeFlags.ExpandFill,
                Color = OmegaSpiralColors.DeepSpace,
                ZIndex = -2
            };
            AddChild(background);
            GD.Print($"[OmegaUi] Background created with DeepSpace color: {OmegaSpiralColors.DeepSpace}");
        }
        else
        {
            GD.Print("[OmegaUi] Background already exists in scene");
        }

        // Create ContentContainer if it doesn't exist
        if (GetNodeOrNull<Control>("ContentContainer") == null)
        {
            var contentContainer = new Control
            {
                Name = "ContentContainer",
                SizeFlagsHorizontal = Control.SizeFlags.ExpandFill,
                SizeFlagsVertical = Control.SizeFlags.ExpandFill
            };
            AddChild(contentContainer);
        }

        // Create PhosphorLayer if it doesn't exist
        if (GetNodeOrNull<ColorRect>("PhosphorLayer") == null)
        {
            var phosphorLayer = new ColorRect
            {
                Name = "PhosphorLayer",
                SizeFlagsHorizontal = Control.SizeFlags.ExpandFill,
                SizeFlagsVertical = Control.SizeFlags.ExpandFill,
                ZIndex = -1
            };
            AddChild(phosphorLayer);
        }

        // Create ScanlineLayer if it doesn't exist
        if (GetNodeOrNull<ColorRect>("ScanlineLayer") == null)
        {
            var scanlineLayer = new ColorRect
            {
                Name = "ScanlineLayer",
                SizeFlagsHorizontal = Control.SizeFlags.ExpandFill,
                SizeFlagsVertical = Control.SizeFlags.ExpandFill,
                ZIndex = -1
            };
            AddChild(scanlineLayer);
        }

        // Create GlitchLayer if it doesn't exist
        if (GetNodeOrNull<ColorRect>("GlitchLayer") == null)
        {
            var glitchLayer = new ColorRect
            {
                Name = "GlitchLayer",
                SizeFlagsHorizontal = Control.SizeFlags.ExpandFill,
                SizeFlagsVertical = Control.SizeFlags.ExpandFill,
                ZIndex = -1
            };
            AddChild(glitchLayer);
        }
    }
}
}
