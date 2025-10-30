using System;
using System.Collections.Generic;
using Godot;
using Newtonsoft.Json;

namespace OmegaSpiral.Source.Design;

/// <summary>
/// Provides access to design colors and shader defaults defined in <c>colors_config.json</c>.
/// </summary>
public static class DesignConfigService
{
    private const string ConfigPath = "res://source/frontend/design/colors_config.json";
    private static readonly Lazy<ColorsConfig> _config = new(LoadConfig, System.Threading.LazyThreadSafetyMode.ExecutionAndPublication);

    /// <summary>
    /// Gets a color by name from the palette; returns white if missing.
    /// </summary>
    /// <param name="name">Palette key, e.g., <c>warm_amber</c>.</param>
    public static Color GetColor(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return Colors.White;
        }

        var config = _config.Value;
        if (config.Colors.TryGetValue(name, out var value))
        {
            return new Color(value.R, value.G, value.B, value.A);
        }

        GD.PushWarning($"[DesignConfigService] Palette key '{name}' not found; falling back to white.");
        return Colors.White;
    }

    /// <summary>
    /// Attempts to retrieve default shader parameters for a given shader key.
    /// </summary>
    /// <param name="shaderKey">Key within <c>shader_values</c> (e.g., <c>spiral_border</c>).</param>
    /// <param name="parameters">Resolved parameter map on success.</param>
    public static bool TryGetShaderDefaults(string shaderKey, out IReadOnlyDictionary<string, float> parameters)
    {
        parameters = new Dictionary<string, float>();

        if (string.IsNullOrWhiteSpace(shaderKey))
        {
            return false;
        }

        var config = _config.Value;
        if (config.ShaderValues.TryGetValue(shaderKey, out var map))
        {
            parameters = map;
            return true;
        }

        return false;
    }

    private static ColorsConfig LoadConfig()
    {
        try
        {
            if (!FileAccess.FileExists(ConfigPath))
            {
                GD.PrintErr($"[DesignConfigService] Config not found at {ConfigPath}");
                return new ColorsConfig();
            }

            var json = FileAccess.GetFileAsString(ConfigPath);
            var config = JsonConvert.DeserializeObject<ColorsConfig>(json) ?? new ColorsConfig();

            EnsureRequiredColors(config);
            GD.Print($"[DesignConfigService] Loaded {config.Colors.Count} colors and {config.ShaderValues.Count} shader defaults.");
            return config;
        }
        catch (Exception ex)
        {
            GD.PrintErr($"[DesignConfigService] Failed to load configuration: {ex.Message}");
            return new ColorsConfig();
        }
    }

    private static void EnsureRequiredColors(ColorsConfig config)
    {
        string[] required = { "warm_amber", "pure_white", "light_thread", "shadow_thread", "ambition_thread" };
        foreach (var key in required)
        {
            if (!config.Colors.ContainsKey(key))
            {
                GD.PushWarning($"[DesignConfigService] Missing palette color '{key}', using white fallback.");
                config.Colors[key] = new ColorValue { R = 1f, G = 1f, B = 1f, A = 1f };
            }
        }
    }
}
