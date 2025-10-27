// <copyright file="OmegaThemedContainer.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using Godot;
using System.Threading.Tasks;

namespace OmegaSpiral.Source.Ui.Omega;

/// <summary>
/// Container with optional Omega visual theme (border, shaders, CRT effects).
/// Composes Omega components via factory - enable/disable via exports.
/// Used by narrative scenes that want the full Omega aesthetic.
/// </summary>
/// <remarks>
/// This class provides opt-in Omega theming:
/// - EnableOmegaBorder: Animated spiral border frame
/// - EnableCrtShaders: Phosphor/scanline/glitch effects
/// - EnableOmegaText: Themed text rendering
/// All components are optional and configurable via Godot inspector.
/// </remarks>
[GlobalClass]
public partial class OmegaThemedContainer : OmegaContainer
{
    /// <summary>
    /// Gets or sets a value indicating whether to enable the entire Omega visual theme.
    /// When false, disables all theming including border, shaders, and text styling.
    /// Useful for unit tests or minimal UI modes.
    /// Reference: https://docs.godotengine.org/en/stable/tutorials/best_practices/godot_notifications.html#ready-vs-enter-tree-vs-notification-parented
    /// </summary>
    [Export]
    public bool EnableOmegaTheme { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether to enable the animated Omega border frame.
    /// </summary>
    [Export]
    public bool EnableOmegaBorder { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether to enable CRT shader effects.
    /// </summary>
    [Export]
    public bool EnableCrtShaders { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether to enable Omega-themed text rendering.
    /// </summary>
    [Export]
    public bool EnableOmegaText { get; set; } = true;

    private OmegaBorderFrame? _BorderFrame;
    private ColorRect? _Background;
    private ColorRect? _PhosphorLayer;
    private ColorRect? _ScanlineLayer;
    private ColorRect? _GlitchLayer;
    private RichTextLabel? _TextDisplay;

    /// <inheritdoc/>
    protected override void CacheRequiredNodes()
    {
        base.CacheRequiredNodes();
        GD.Print($"[OmegaThemedContainer.CacheRequiredNodes] ChildCount before cache={GetChildCount()}");

        // List all children
        for (int i = 0; i < GetChildCount(); i++)
        {
            var child = GetChild(i);
            GD.Print($"  Child[{i}]: {child.Name} ({child.GetType().Name})");
        }

        // Cache optional background
        _Background = GetNodeOrNull<ColorRect>("Background");

        // Cache optional CRT shader layers
        _PhosphorLayer = GetNodeOrNull<ColorRect>("PhosphorLayer");
        _ScanlineLayer = GetNodeOrNull<ColorRect>("ScanlineLayer");
        _GlitchLayer = GetNodeOrNull<ColorRect>("GlitchLayer");

        // Cache optional text display
        _TextDisplay = GetNodeOrNull<RichTextLabel>("ContentContainer/TextDisplay");
    }

    /// <inheritdoc/>
    /// <inheritdoc/>
    protected override void CreateComponents()
    {
        base.CreateComponents();

        // Check if Omega theming is enabled
        if (!EnableOmegaTheme)
        {
            GD.Print("[OmegaThemedContainer] Omega theme disabled - skipping frame creation");
            return;
        }

        GD.Print($"[OmegaThemedContainer] Building complete Omega frame (EnableBorder={EnableOmegaBorder}, EnableCRT={EnableCrtShaders})");

        // Step 1: Build the complete Omega visual frame hierarchy
        BuildOmegaFrame();

        // Step 2: Find or create ContentContainer and reparent it inside the frame
        SetupContentContainer();

        // Step 3: Compose optional text renderer
        if (EnableOmegaText && _TextDisplay != null)
        {
            ComposeTextRenderer(_TextDisplay);
        }

        GD.Print($"[OmegaThemedContainer] Frame complete - ChildCount={GetChildCount()}");
    }

    /// <summary>
    /// Builds the complete Omega visual frame: Background → CRT layers → Border.
    /// This frame fills the entire viewport and provides the visual foundation.
    /// </summary>
    private void BuildOmegaFrame()
    {
        // Ensure this control fills the viewport
        AnchorLeft = 0;
        AnchorTop = 0;
        AnchorRight = 1;
        AnchorBottom = 1;
        OffsetLeft = 0;
        OffsetTop = 0;
        OffsetRight = 0;
        OffsetBottom = 0;
        GD.Print($"[OmegaThemedContainer] Root control anchored to fill viewport");

        // Create dark background at the back
        _Background = new ColorRect
        {
            Name = "Background",
            Color = OmegaSpiralColors.DeepSpace,
            MouseFilter = MouseFilterEnum.Ignore,
            AnchorLeft = 0,
            AnchorTop = 0,
            AnchorRight = 1,
            AnchorBottom = 1
        };
        AddChild(_Background);
        GD.Print($"[OmegaThemedContainer] Created Background - will fill viewport");

        // Create CRT shader layers if enabled
        if (EnableCrtShaders)
        {
            _PhosphorLayer = new ColorRect
            {
                Name = "PhosphorLayer",
                Color = OmegaSpiralColors.PhosphorGlow,
                MouseFilter = MouseFilterEnum.Ignore,
                AnchorLeft = 0,
                AnchorTop = 0,
                AnchorRight = 1,
                AnchorBottom = 1
            };
            AddChild(_PhosphorLayer);

            _ScanlineLayer = new ColorRect
            {
                Name = "ScanlineLayer",
                Color = OmegaSpiralColors.ScanlineOverlay,
                MouseFilter = MouseFilterEnum.Ignore,
                AnchorLeft = 0,
                AnchorTop = 0,
                AnchorRight = 1,
                AnchorBottom = 1
            };
            AddChild(_ScanlineLayer);

            _GlitchLayer = new ColorRect
            {
                Name = "GlitchLayer",
                Color = OmegaSpiralColors.GlitchDistortion,
                MouseFilter = MouseFilterEnum.Ignore,
                AnchorLeft = 0,
                AnchorTop = 0,
                AnchorRight = 1,
                AnchorBottom = 1
            };
            AddChild(_GlitchLayer);

            GD.Print($"[OmegaThemedContainer] Created CRT shader layers (Phosphor, Scanline, Glitch)");

            // Configure shader controller
            var shaderLayer = _PhosphorLayer;
            if (shaderLayer != null)
            {
                ComposeShaderController(shaderLayer);
            }
        }

        // Create border frame on top if enabled
        if (EnableOmegaBorder)
        {
            _BorderFrame = OmegaComponentFactory.CreateBorderFrame();
            AddChild(_BorderFrame);
            GD.Print($"[OmegaThemedContainer] Created BorderFrame on top");
        }
    }

    /// <summary>
    /// Finds the existing ContentContainer (from the scene) and ensures it's properly configured.
    /// Centers it on screen with appropriate bezel margins and proper size flags.
    /// If it doesn't exist, creates one with appropriate margins.
    /// </summary>
    private void SetupContentContainer()
    {
        var existingContent = GetNodeOrNull<Control>("ContentContainer");

        if (existingContent != null)
        {
            GD.Print($"[OmegaThemedContainer] Found existing ContentContainer - centering with bezel margins");

            // Center the container with 10% margins on all sides (bezel effect)
            existingContent.AnchorLeft = 0.05f;  // 5% margin from left
            existingContent.AnchorTop = 0.05f;   // 5% margin from top
            existingContent.AnchorRight = 0.95f; // 5% margin from right
            existingContent.AnchorBottom = 0.95f; // 5% margin from bottom
            existingContent.OffsetLeft = 0;
            existingContent.OffsetTop = 0;
            existingContent.OffsetRight = 0;
            existingContent.OffsetBottom = 0;
            existingContent.GrowHorizontal = Control.GrowDirection.Both;
            existingContent.GrowVertical = Control.GrowDirection.Both;

            // Reparent it to be the last child (on top of all Omega visual elements)
            MoveChild(existingContent, -1);
        }
        else
        {
            GD.Print($"[OmegaThemedContainer] No existing ContentContainer - creating one with margins");
            // Create a new content container with margins for bezel effect
            var container = new VBoxContainer
            {
                Name = "ContentContainer",
                AnchorLeft = 0.05f,
                AnchorTop = 0.05f,
                AnchorRight = 0.95f,
                AnchorBottom = 0.95f,
                GrowHorizontal = Control.GrowDirection.Both,
                GrowVertical = Control.GrowDirection.Both
            };
            AddChild(container);
        }

        GD.Print($"[OmegaThemedContainer] Final child order:");
        for (int i = 0; i < GetChildCount(); i++)
        {
            var child = GetChild(i);
            GD.Print($"  [{i}] {child.Name}");
        }
    }

    /// <summary>
    /// Gets the border frame if created.
    /// </summary>
    /// <returns>The OmegaBorderFrame instance, or null if not enabled/created.</returns>
    public OmegaBorderFrame? GetBorderFrame() => _BorderFrame;

    /// <summary>
    /// Gets the phosphor shader layer if available.
    /// </summary>
    /// <returns>The phosphor ColorRect, or null if not in scene.</returns>
    protected ColorRect? GetPhosphorLayer() => _PhosphorLayer;

    /// <summary>
    /// Gets the scanline shader layer if available.
    /// </summary>
    /// <returns>The scanline ColorRect, or null if not in scene.</returns>
    protected ColorRect? GetScanlineLayer() => _ScanlineLayer;

    /// <summary>
    /// Gets the glitch shader layer if available.
    /// </summary>
    /// <returns>The glitch ColorRect, or null if not in scene.</returns>
    protected ColorRect? GetGlitchLayer() => _GlitchLayer;

    // ==================== PUBLIC CONVENIENCE METHODS ====================

    /// <summary>
    /// Appends text to the main display with optional typing animation.
    /// </summary>
    /// <param name="text">Text to append.</param>
    /// <param name="typingSpeed">Characters per second for typing effect.</param>
    /// <param name="delayBeforeStart">Delay in seconds before starting to type.</param>
    public async Task AppendTextAsync(string text, float typingSpeed = 30f, float delayBeforeStart = 0f)
    {
        if (TextRenderer == null)
        {
            GD.PushWarning("[OmegaThemedContainer] Cannot append text - TextRenderer not initialized.");
            return;
        }

        await TextRenderer.AppendTextAsync(text, typingSpeed, delayBeforeStart).ConfigureAwait(false);
    }

    /// <summary>
    /// Clears all text from the main display.
    /// </summary>
    public void ClearText()
    {
        TextRenderer?.ClearText();
    }

    /// <summary>
    /// Applies a visual preset to the shader layers.
    /// </summary>
    /// <param name="presetName">Name of the preset to apply (e.g., "phosphor", "scanlines", "glitch").</param>
    public async Task ApplyVisualPresetAsync(string presetName)
    {
        if (ShaderController == null)
        {
            GD.PushWarning("[OmegaThemedContainer] Cannot apply visual preset - ShaderController not initialized.");
            return;
        }

        await ShaderController.ApplyVisualPresetAsync(presetName).ConfigureAwait(false);
    }

    /// <summary>
    /// Performs a pixel dissolve effect.
    /// </summary>
    /// <param name="durationSeconds">Duration of the dissolve effect in seconds.</param>
    public async Task PixelDissolveAsync(float durationSeconds = 2.5f)
    {
        if (ShaderController == null)
        {
            GD.PushWarning("[OmegaThemedContainer] Cannot perform dissolve - ShaderController not initialized.");
            return;
        }

        await ShaderController.PixelDissolveAsync(durationSeconds).ConfigureAwait(false);
    }

    /// <summary>
    /// Gets or sets the rotation speed of the spiral border animation.
    /// </summary>
    public float BorderRotationSpeed
    {
        get
        {
            var borderFrame = GetBorderFrame();
            return borderFrame?.GetRotationSpeed() ?? 0.05f;
        }
        set
        {
            var borderFrame = GetBorderFrame();
            if (borderFrame != null)
            {
                borderFrame.ConfigureAnimationSpeed(Mathf.Clamp(value, 0.0f, 2.0f), borderFrame.GetWaveSpeed());
            }
        }
    }

    /// <summary>
    /// Gets or sets the wave flow speed of the border animation.
    /// </summary>
    public float BorderWaveSpeed
    {
        get
        {
            var borderFrame = GetBorderFrame();
            return borderFrame?.GetWaveSpeed() ?? 0.8f;
        }
        set
        {
            var borderFrame = GetBorderFrame();
            if (borderFrame != null)
            {
                borderFrame.ConfigureAnimationSpeed(borderFrame.GetRotationSpeed(), Mathf.Clamp(value, 0.0f, 5.0f));
            }
        }
    }
}
