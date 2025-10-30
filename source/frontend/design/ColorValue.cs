// <copyright file="ColorValue.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

namespace OmegaSpiral.Source.Data.Config;

using Newtonsoft.Json;

/// <summary>
/// Normalized RGBA color value (0.0-1.0 range) with hex reference.
/// </summary>
public class ColorValue
{
    /// <summary>
    /// Gets or sets red channel (0.0-1.0).
    /// </summary>
    [JsonProperty("r")]
    public float R { get; set; }

    /// <summary>
    /// Gets or sets green channel (0.0-1.0).
    /// </summary>
    [JsonProperty("g")]
    public float G { get; set; }

    /// <summary>
    /// Gets or sets blue channel (0.0-1.0).
    /// </summary>
    [JsonProperty("b")]
    public float B { get; set; }

    /// <summary>
    /// Gets or sets alpha channel (0.0-1.0).
    /// </summary>
    [JsonProperty("a")]
    public float A { get; set; }

    /// <summary>
    /// Gets or sets hexadecimal color representation.
    /// </summary>
    [JsonProperty("hex")]
    public string? Hex { get; set; }
}
