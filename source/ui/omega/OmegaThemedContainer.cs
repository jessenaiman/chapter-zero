// <copyright file="OmegaThemedContainer.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using Godot;

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
    private ColorRect? _PhosphorLayer;
    private ColorRect? _ScanlineLayer;
    private ColorRect? _GlitchLayer;
    private RichTextLabel? _TextDisplay;

    /// <inheritdoc/>
    protected override void CacheRequiredNodes()
    {
        base.CacheRequiredNodes();

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

        // Create border frame if enabled and not already in scene
        if (EnableOmegaBorder && GetNodeOrNull("BorderFrame") == null)
        {
            _BorderFrame = OmegaComponentFactory.CreateBorderFrame();
            AddChild(_BorderFrame);
        }

        // Create shader controller if CRT shaders enabled
        if (EnableCrtShaders)
        {
            var shaderLayer = _PhosphorLayer ?? _ScanlineLayer ?? _GlitchLayer;
            if (shaderLayer != null)
            {
                ComposeShaderController(shaderLayer);
            }
        }

        // Create text renderer if Omega text enabled
        if (EnableOmegaText && _TextDisplay != null)
        {
            ComposeTextRenderer(_TextDisplay);
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
}
