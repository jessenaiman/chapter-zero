// <copyright file="OmegaSpiralConfig.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

namespace OmegaSpiral.Source.Design;

using System.Collections.Generic;
using Newtonsoft.Json;

/// <summary>
/// Root configuration for design colors and shader defaults loaded from OmegaSpiralConfig.json.
/// </summary>
public sealed class OmegaSpiralConfig
{
    /// <summary>Documentation for the config file.</summary>
    [JsonProperty("_doc")]
    public string? Doc { get; set; }

    /// <summary>Core color palette.</summary>
    [JsonProperty("design_system")]
    public Dictionary<string, ColorValue> DesignSystem { get; set; } = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>Shader presets with parameters.</summary>
    [JsonProperty("shader_presets")]
    public Dictionary<string, ShaderPreset> ShaderPresets { get; set; } = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>Additional notes.</summary>
    [JsonProperty("notes")]
    public List<string> Notes { get; set; } = new();
}
