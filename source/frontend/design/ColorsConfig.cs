// <copyright file="ColorValue.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

namespace OmegaSpiral.Source.Design;

using System.Collections.Generic;
using Newtonsoft.Json;

/// <summary>
/// Root configuration for design colors and shader defaults loaded from colors_config.json.
/// </summary>
public sealed class ColorsConfig
{
    /// <summary>Named RGBA color palette.</summary>
    [JsonProperty("colors")]
    public Dictionary<string, ColorValue> Colors { get; set; } = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>Shader default parameter mappings keyed by shader identifier.</summary>
    [JsonProperty("shader_values")]
    public Dictionary<string, Dictionary<string, float>> ShaderValues { get; set; } = new(StringComparer.OrdinalIgnoreCase);
}

/// <summary>
/// Normalized RGBA color value (0.0-1.0 range).
/// </summary>
public sealed class ColorValue
{
    [JsonProperty("r")]
    public float R { get; set; }

    [JsonProperty("g")]
    public float G { get; set; }

    [JsonProperty("b")]
    public float B { get; set; }

    [JsonProperty("a")]
    public float A { get; set; } = 1.0f;

    [JsonProperty("hex")]
    public string? Hex { get; set; }
}
