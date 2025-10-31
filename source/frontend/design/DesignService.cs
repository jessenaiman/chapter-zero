// <copyright file="DesignService.cs" company="Omega Spiral">
// Copyright (c) Omega Spiral. All rights reserved.
// </copyright>

namespace OmegaSpiral.Source.Frontend.Design;

using System;
using System.Collections.Generic;
using Godot;
using Newtonsoft.Json;

/// <summary>
/// Provides a static façade for retrieving UI design data (colors and shader presets) from the
/// <c>omega_spiral_colors_config.json</c> configuration file.
/// The service lazily loads the JSON once per application domain and caches the parsed
/// <see cref="ColorsConfig"/> instance for fast subsequent look‑ups.
/// </summary>
public static class DesignService
{
    private const string _ConfigPath = "res://omega_spiral_colors_config.json";
    private static readonly Lazy<ColorsConfig> _Config = new(LoadConfig, System.Threading.LazyThreadSafetyMode.ExecutionAndPublication);

    /// <summary>
    /// Retrieves a <see cref="Godot.Color"/> from the design palette.
    /// If the supplied <paramref name="name"/> is null, empty, or whitespace the method returns
    /// <see cref="Godot.Colors.White"/>. When the key does not exist in the configuration a warning
    /// is emitted and <see cref="Godot.Colors.White"/> is returned as a safe fallback.
    /// </summary>
    /// <param name="name">The palette key, e.g. <c>warm_amber</c>. Case‑insensitive.</param>
    /// <returns>A <see cref="Godot.Color"/> representing the requested palette entry.</returns>
    public static Color GetColor(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return Colors.White;
        }

        var config = _Config.Value;
        if (config.Colors.TryGetValue(name, out var value))
        {
            return new Color(value.R, value.G, value.B, value.A);
        }

        GD.PushWarning($"[DesignService] Palette key '{name}' not found; falling back to white.");
        return Colors.White;
    }

    /// <summary>
    /// Attempts to obtain the full <see cref="ShaderPreset"/> definition for the supplied
    /// <paramref name="presetKey"/>.
    /// </summary>
    /// <param name="presetKey">Key within the <c>shader_presets</c> dictionary (e.g. <c>spiral_border_base</c>).</param>
    /// <param name="preset">When the method returns <c>true</c>, this out parameter contains the
    /// resolved <see cref="ShaderPreset"/>; otherwise it is <c>null</c>.</param>
    /// <returns><c>true</c> if a matching preset was found; otherwise <c>false</c>.</returns>
    public static bool TryGetShaderPreset(string presetKey, out ShaderPreset? preset)
    {
        preset = null;

        if (string.IsNullOrWhiteSpace(presetKey))
        {
            return false;
        }

        var config = _Config.Value;
        if (config.ShaderPresets.TryGetValue(presetKey, out var shaderPreset))
        {
            preset = shaderPreset;
            return true;
        }

        GD.PushWarning($"[DesignService] Shader preset '{presetKey}' not found.");
        return false;
    }

    /// <summary>
    /// Retrieves the default parameter map for a shader preset.
    /// The returned dictionary is read‑only and contains values of type <c>object</c> because
    /// shader parameters can be floats, vectors, colors, etc.
    /// </summary>
    /// <param name="presetKey">Key within the <c>shader_presets</c> collection.</param>
    /// <param name="parameters">On success, receives an <see cref="IReadOnlyDictionary{TKey,TValue}"/>
    /// where <c>TKey</c> is <c>string</c> and <c>TValue</c> is <c>object</c>.</param>
    /// <returns><c>true</c> if the preset exists and contains a parameter dictionary; otherwise <c>false</c>.</returns>
    public static bool TryGetShaderDefaults(string presetKey, out IReadOnlyDictionary<string, object> parameters)
    {
        parameters = new Dictionary<string, object>();

        if (string.IsNullOrWhiteSpace(presetKey))
        {
            return false;
        }

        var config = _Config.Value;
        if (config.ShaderPresets.TryGetValue(presetKey, out var preset) && preset?.Parameters != null)
        {
            parameters = preset.Parameters;
            return true;
        }

        return false;
    }

    private static ColorsConfig LoadConfig()
    {
        try
        {
            if (!FileAccess.FileExists(_ConfigPath))
            {
                GD.PrintErr($"[DesignService] Configuration file not found at {_ConfigPath}. Using default configuration.");
                return CreateDefaultConfig();
            }

            string json;
            try
            {
                json = FileAccess.GetFileAsString(_ConfigPath);
            }
            catch (Exception ex)
            {
                GD.PrintErr($"[DesignService] Failed to read configuration file: {ex.Message}");
                return CreateDefaultConfig();
            }

            if (string.IsNullOrWhiteSpace(json))
            {
                GD.PrintErr("[DesignService] Configuration file is empty. Using default configuration.");
                return CreateDefaultConfig();
            }

            OmegaSpiralConfig omegaConfig;
            try
            {
                omegaConfig = JsonConvert.DeserializeObject<OmegaSpiralConfig>(json) ?? new OmegaSpiralConfig();
            }
            catch (Newtonsoft.Json.JsonException ex)
            {
                GD.PrintErr($"[DesignService] Failed to parse JSON configuration: {ex.Message}. Using default configuration.");
                return CreateDefaultConfig();
            }

            if (omegaConfig == null)
            {
                GD.PrintErr("[DesignService] Deserialized configuration is null. Using default configuration.");
                return CreateDefaultConfig();
            }

            var config = new ColorsConfig
            {
                Colors = omegaConfig.DesignSystem ?? new Dictionary<string, ColorValue>(),
                ShaderPresets = omegaConfig.ShaderPresets ?? new Dictionary<string, ShaderPreset>(),
            };

            EnsureRequiredColors(config);
            GD.Print($"[DesignService] Successfully loaded {config.Colors.Count} colors and {config.ShaderPresets.Count} shader presets.");
            return config;
        }
        catch (Exception ex)
        {
            GD.PrintErr($"[DesignService] Unexpected error loading configuration: {ex.Message}. Using default configuration.");
            return CreateDefaultConfig();
        }
    }

    private static ColorsConfig CreateDefaultConfig()
    {
        var config = new ColorsConfig
        {
            Colors = new Dictionary<string, ColorValue>(),
            ShaderPresets = new Dictionary<string, ShaderPreset>(),
        };
        EnsureRequiredColors(config);
        return config;
    }

    private static void EnsureRequiredColors(ColorsConfig config)
    {
        string[] required = { "warm_amber", "pure_white", "light_thread", "shadow_thread", "ambition_thread" };
        foreach (var key in required)
        {
            if (!config.Colors.ContainsKey(key))
            {
                GD.PushWarning($"[DesignService] Missing palette color '{key}', using white fallback.");
                config.Colors[key] = new ColorValue { R = 1f, G = 1f, B = 1f, A = 1f };
            }
        }
    }
}
