// <copyright file="ColorsConfig.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

namespace OmegaSpiral.Source.Design;

using System.Collections.Generic;

/// <summary>
/// Internal configuration wrapper for design colors and shader presets loaded from JSON.
/// Provides type-safe access to the design system.
/// </summary>
internal sealed class ColorsConfig
{
    /// <summary>
    /// Gets or sets the color palette dictionary (key -> ColorValue).
    /// Maps color names like "warm_amber" to their RGBA values.
    /// </summary>
    public Dictionary<string, ColorValue> Colors { get; set; } = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Gets or sets the shader presets dictionary (key -> ShaderPreset).
    /// Maps preset names like "spiral_border_base" to shader paths and parameters.
    /// </summary>
    public Dictionary<string, ShaderPreset> ShaderPresets { get; set; } = new(StringComparer.OrdinalIgnoreCase);
}
