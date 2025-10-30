// <copyright file="DesignSystem.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

namespace OmegaSpiral.Source.Data.Config;

using Newtonsoft.Json;

/// <summary>
/// Core color palette and design tokens shared across UI elements and shaders.
/// </summary>
public class DesignSystem
{
    /// <summary>
    /// Gets or sets primary background void color.
    /// </summary>
    [JsonProperty("deep_space")]
    public ColorValue? DeepSpace { get; set; }

    /// <summary>
    /// Gets or sets secondary background for layered panels.
    /// </summary>
    [JsonProperty("dark_void")]
    public ColorValue? DarkVoid { get; set; }

    /// <summary>
    /// Gets or sets Omega accent glow (primary narrative text).
    /// </summary>
    [JsonProperty("warm_amber")]
    public ColorValue? WarmAmber { get; set; }

    /// <summary>
    /// Gets or sets highlight and interface focus state color.
    /// </summary>
    [JsonProperty("pure_white")]
    public ColorValue? PureWhite { get; set; }

    /// <summary>
    /// Gets or sets absolute black for masks.
    /// </summary>
    [JsonProperty("pure_black")]
    public ColorValue? PureBlack { get; set; }

    /// <summary>
    /// Gets or sets color for disabled UI elements (50% opacity).
    /// </summary>
    [JsonProperty("disabled_gray")]
    public ColorValue? DisabledGray { get; set; }

    /// <summary>
    /// Gets or sets phosphor overlay layer tint.
    /// </summary>
    [JsonProperty("phosphor_glow")]
    public ColorValue? PhosphorGlow { get; set; }

    /// <summary>
    /// Gets or sets scanline overlay tint.
    /// </summary>
    [JsonProperty("scanline_overlay")]
    public ColorValue? ScanlineOverlay { get; set; }

    /// <summary>
    /// Gets or sets glitch overlay tint.
    /// </summary>
    [JsonProperty("glitch_distortion")]
    public ColorValue? GlitchDistortion { get; set; }

    /// <summary>
    /// Gets or sets Dreamweaver Light thread color (Silver).
    /// </summary>
    [JsonProperty("light_thread")]
    public ColorValue? LightThread { get; set; }

    /// <summary>
    /// Gets or sets Dreamweaver Shadow thread color (Golden Amber).
    /// </summary>
    [JsonProperty("shadow_thread")]
    public ColorValue? ShadowThread { get; set; }

    /// <summary>
    /// Gets or sets Dreamweaver Ambition thread color (Crimson).
    /// </summary>
    [JsonProperty("ambition_thread")]
    public ColorValue? AmbitionThread { get; set; }
}
