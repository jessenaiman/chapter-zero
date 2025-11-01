// <copyright file="OmegaSpiralConfig.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

namespace OmegaSpiral.Source.Frontend.Design;

using System.Collections.Generic;
using Newtonsoft.Json;

/// <summary>
/// Root configuration for design colors and shader defaults loaded from OmegaSpiralConfig.json.
/// </summary>
public sealed class OmegaSpiralConfig
{
    /// <summary>
    /// Optional documentation string for the configuration file itself. This mirrors the
    /// <c>_doc</c> field present in the JSON source and can be used by tooling to surface
    /// a human‑readable description of the design system.
    /// </summary>
    /// <remarks>
    /// The property is nullable because the JSON may omit the field. When present, the value
    /// is typically a short paragraph describing the purpose of the palette and shader presets.
    /// </remarks>
    [JsonProperty("_doc")]
    public string? Doc { get; set; }

    /// <summary>
    /// The core colour palette mapping logical colour names to <see cref="ColorValue"/> instances.
    /// Keys are case‑insensitive and correspond to entries used throughout the UI via
    /// <see cref="OmegaSpiral.Source.Design.DesignService.GetColor(string)"/>.
    /// </summary>
    [JsonProperty("design_system")]
    public Dictionary<string, ColorValue> DesignSystem { get; set; } = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Collection of shader preset definitions. Each entry provides a shader resource path and a
    /// dictionary of parameters that can be applied to a <see cref="Godot.ShaderMaterial"/>.
    /// </summary>
    [JsonProperty("shader_presets")]
    public Dictionary<string, ShaderPreset> ShaderPresets { get; set; } = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Arbitrary free‑form notes that may be used by designers or tooling. The list is not
    /// interpreted by the engine; it exists solely for documentation purposes.
    /// </summary>
    [JsonProperty("notes")]
    public List<string> Notes { get; set; } = new();
}
