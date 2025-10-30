// <copyright file="DesignSystemConfig.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

namespace OmegaSpiral.Source.Config;

using Newtonsoft.Json;

/// <summary>
/// Configuration for Omega Spiral design system colors.
/// Contains the core color palette used across UI elements, shaders, and narrative threads.
/// </summary>
public class DesignSystemConfig
{
    /// <summary>
    /// Gets or sets the primary background void color.
    /// </summary>
    [JsonProperty("deep_space")]
    public ColorValue? DeepSpace { get; set; }

    /// <summary>
    /// Gets or sets the secondary background color for layered panels.
    /// </summary>
    [JsonProperty("dark_void")]
    public ColorValue? DarkVoid { get; set; }

    /// <summary>
    /// Gets or sets the Omega accent glow color (primary narrative text).
    /// </summary>
    [JsonProperty("warm_amber")]
    public ColorValue? WarmAmber { get; set; }

    /// <summary>
    /// Gets or sets the pure white color for highlights and interface focus states.
    /// </summary>
    [JsonProperty("pure_white")]
    public ColorValue? PureWhite { get; set; }

    /// <summary>
    /// Gets or sets the absolute black color for masks.
    /// </summary>
    [JsonProperty("pure_black")]
    public ColorValue? PureBlack { get; set; }

    /// <summary>
    /// Gets or sets the disabled gray color for UI elements (50% opacity).
    /// </summary>
    [JsonProperty("disabled_gray")]
    public ColorValue? DisabledGray { get; set; }

    /// <summary>
    /// Gets or sets the phosphor overlay layer tint.
    /// </summary>
    [JsonProperty("phosphor_glow")]
    public ColorValue? PhosphorGlow { get; set; }

    /// <summary>
    /// Gets or sets the scanline overlay tint.
    /// </summary>
    [JsonProperty("scanline_overlay")]
    public ColorValue? ScanlineOverlay { get; set; }

    /// <summary>
    /// Gets or sets the glitch overlay tint.
    /// </summary>
    [JsonProperty("glitch_distortion")]
    public ColorValue? GlitchDistortion { get; set; }

    /// <summary>
    /// Gets or sets the Dreamweaver Light thread color (Silver).
    /// </summary>
    [JsonProperty("light_thread")]
    public ColorValue? LightThread { get; set; }

    /// <summary>
    /// Gets or sets the Dreamweaver Shadow thread color (Golden Amber).
    /// </summary>
    [JsonProperty("shadow_thread")]
    public ColorValue? ShadowThread { get; set; }

    /// <summary>
    /// Gets or sets the Dreamweaver Ambition thread color (Crimson).
    /// </summary>
    [JsonProperty("ambition_thread")]
    public ColorValue? AmbitionThread { get; set; }
}
