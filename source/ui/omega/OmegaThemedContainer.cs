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
    protected override void CreateComponents()
    {
        base.CreateComponents();
        GD.Print($"[OmegaThemedContainer.CreateComponents] Starting - EnableOmegaBorder={EnableOmegaBorder}, EnableCrtShaders={EnableCrtShaders}");

        // Create background if not already in scene
        if (GetNodeOrNull("Background") == null)
        {
            _Background = new ColorRect
            {
                Name = "Background",
                Color = OmegaSpiralColors.DeepSpace,
                MouseFilter = MouseFilterEnum.Ignore,
                AnchorLeft = 0,
                AnchorTop = 0,
                AnchorRight = 1,
                AnchorBottom = 1,
                OffsetLeft = 0,
                OffsetTop = 0,
                OffsetRight = 0,
                OffsetBottom = 0
            };
            AddChild(_Background);
            MoveChild(_Background, 0); // Send to back
            GD.Print($"[OmegaThemedContainer] Created Background - Visible={_Background.Visible}, Color={_Background.Color}, ZIndex={_Background.ZIndex}");
        }

        // Create CRT shader layers if enabled and not already in scene
        if (EnableCrtShaders)
        {
            // Create phosphor layer if not present
            if (GetNodeOrNull("PhosphorLayer") == null)
            {
                _PhosphorLayer = new ColorRect
                {
                    Name = "PhosphorLayer",
                    Color = OmegaSpiralColors.PhosphorGlow, // White with 18% opacity for phosphor effect
                    MouseFilter = MouseFilterEnum.Ignore,
                    AnchorLeft = 0,
                    AnchorTop = 0,
                    AnchorRight = 1,
                    AnchorBottom = 1
                };
                AddChild(_PhosphorLayer);
                GD.Print($"[OmegaThemedContainer] Created PhosphorLayer - Visible={_PhosphorLayer.Visible}, Color={_PhosphorLayer.Color}");
            }

            // Create scanline layer if not present
            if (GetNodeOrNull("ScanlineLayer") == null)
            {
                _ScanlineLayer = new ColorRect
                {
                    Name = "ScanlineLayer",
                    Color = OmegaSpiralColors.ScanlineOverlay, // White with 12% opacity for scanlines
                    MouseFilter = MouseFilterEnum.Ignore,
                    AnchorLeft = 0,
                    AnchorTop = 0,
                    AnchorRight = 1,
                    AnchorBottom = 1
                };
                AddChild(_ScanlineLayer);
                GD.Print($"[OmegaThemedContainer] Created ScanlineLayer - Visible={_ScanlineLayer.Visible}, Color={_ScanlineLayer.Color}");
            }

            // Create glitch layer if not present
            if (GetNodeOrNull("GlitchLayer") == null)
            {
                _GlitchLayer = new ColorRect
                {
                    Name = "GlitchLayer",
                    Color = OmegaSpiralColors.GlitchDistortion, // White with 8% opacity for glitch effect
                    MouseFilter = MouseFilterEnum.Ignore,
                    AnchorLeft = 0,
                    AnchorTop = 0,
                    AnchorRight = 1,
                    AnchorBottom = 1
                };
                AddChild(_GlitchLayer);
                GD.Print($"[OmegaThemedContainer] Created GlitchLayer - Visible={_GlitchLayer.Visible}, Color={_GlitchLayer.Color}");
            }

            // Reorder deterministically so CRT layers sit between Background and ContentContainer
            var contentContainer = GetNodeOrNull("ContentContainer");
            var borderFrameNode = GetNodeOrNull("BorderFrame");

            // Build the desired order: Background, Phosphor, Scanline, Glitch, Content, Border
            var desiredOrder = new Node?[] { _Background, _PhosphorLayer, _ScanlineLayer, _GlitchLayer, contentContainer, borderFrameNode };

            // Remove and re-add each node in order to guarantee exact stacking
            foreach (var node in desiredOrder)
            {
                if (node == null)
                    continue;

                // Only operate on nodes that are direct children of this container
                if (node.GetParent() == this)
                {
                    RemoveChild(node);
                }

                AddChild(node);
            }

            GD.Print($"[OmegaThemedContainer] Reordered children deterministically");

            // Configure shader controller with any shader layer (they're all equivalent)
            var shaderLayer = _PhosphorLayer ?? _ScanlineLayer ?? _GlitchLayer;
            if (shaderLayer != null)
            {
                ComposeShaderController(shaderLayer);
            }
        }

        // Create border frame if enabled and not already in scene
        if (EnableOmegaBorder && GetNodeOrNull("BorderFrame") == null)
        {
            _BorderFrame = OmegaComponentFactory.CreateBorderFrame();
            AddChild(_BorderFrame);
            MoveChild(_BorderFrame, -1); // Bring to front
            GD.Print($"[OmegaThemedContainer] Created BorderFrame - Visible={_BorderFrame.Visible}, ZIndex={_BorderFrame.ZIndex}");
        }

        // Create text renderer if Omega text enabled
        if (EnableOmegaText && _TextDisplay != null)
        {
            ComposeTextRenderer(_TextDisplay);
        }

        GD.Print($"[OmegaThemedContainer.CreateComponents] Complete - ChildCount={GetChildCount()}");
        for (int i = 0; i < GetChildCount(); i++)
        {
            var child = GetChild(i);
            var visibleStr = child is CanvasItem ci ? $"Visible={ci.Visible}" : "N/A";
            GD.Print($"  Final Child[{i}]: {child.Name} ({child.GetType().Name}) {visibleStr}");
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
