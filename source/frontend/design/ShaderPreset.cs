// <copyright file="ShaderPreset.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

namespace OmegaSpiral.Source.Design;

using System.Collections.Generic;
using Newtonsoft.Json;

/// <summary>
/// Shader preset configuration.
/// </summary>
public sealed class ShaderPreset
{
    /// <summary>Documentation for the preset.</summary>
    [JsonProperty("_doc")]
    public string? Doc { get; set; }

    /// <summary>Path to the shader resource.</summary>
    [JsonProperty("shader")]
    public string Shader { get; set; } = string.Empty;

    /// <summary>Shader parameters (mixed types).</summary>
    [JsonProperty("parameters")]
    public Dictionary<string, object> Parameters { get; set; } = new(StringComparer.OrdinalIgnoreCase);
}
