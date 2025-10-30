// <copyright file="DesignConfigService.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

namespace OmegaSpiral.Source.Backend;

using System;
using Godot;
using Newtonsoft.Json;
using OmegaSpiral.Source.Backend.Configuration;

/// <summary>
/// Service for loading and accessing design-system configuration (colors, shader presets).
/// Deserializes omega_spiral_colors.json into strongly-typed C# objects using Newtonsoft.Json.
/// Provides methods to retrieve colors and shader preset parameters at runtime.
/// </summary>
public static class DesignConfigService
{
    private const string ConfigPath = "res://source/data/resources/omega_spiral_colors.json";

    private static DesignConfiguration? CachedConfiguration;

    /// <summary>
    /// Gets the fully deserialized design configuration.
    /// Lazy-loaded on first access.
    /// </summary>
    public static DesignConfiguration Configuration => CachedConfiguration ??= LoadConfiguration();

    /// <summary>
    /// Gets a design color using a dotted path (e.g., "design_system.deep_space").
    /// </summary>
    /// <param name="colorPath">Dotted path to a color property.</param>
    /// <returns>The resolved color, or white if not found.</returns>
    public static Color GetDesignColor(string colorPath)
    {
        if (TryGetDesignColor(colorPath, out var color))
        {
            return color;
        }

        return Colors.White;
    }

    /// <summary>
    /// Attempts to retrieve a design color from a dotted path.
    /// </summary>
    /// <param name="colorPath">Dotted path (e.g., "design_system.deep_space").</param>
    /// <param name="color">The resolved color value.</param>
    /// <returns>True if the color exists; otherwise false.</returns>
    public static bool TryGetDesignColor(string colorPath, out Color color)
    {
        color = Colors.White;

        try
        {
            var parts = colorPath.Split('.', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length != 2 || Configuration.DesignSystem == null)
            {
                return false;
            }

            var section = parts[0];
            var propertyName = parts[1];

            if (!section.Equals("design_system", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            var designSystem = Configuration.DesignSystem;
            var colorValue = propertyName.ToLowerInvariant() switch
            {
                "deep_space" => designSystem.DeepSpace,
                "dark_void" => designSystem.DarkVoid,
                "warm_amber" => designSystem.WarmAmber,
                "pure_white" => designSystem.PureWhite,
                "pure_black" => designSystem.PureBlack,
                "disabled_gray" => designSystem.DisabledGray,
                "phosphor_glow" => designSystem.PhosphorGlow,
                "scanline_overlay" => designSystem.ScanlineOverlay,
                "glitch_distortion" => designSystem.GlitchDistortion,
                "light_thread" => designSystem.LightThread,
                "shadow_thread" => designSystem.ShadowThread,
                "ambition_thread" => designSystem.AmbitionThread,
                _ => null,
            };

            if (colorValue != null)
            {
                color = new Color(colorValue.Red, colorValue.Green, colorValue.Blue, colorValue.Alpha);
                return true;
            }
        }
        catch (Exception ex)
        {
            GD.PrintErr($"[DesignConfigService] Error resolving color '{colorPath}': {ex.Message}");
        }

        return false;
    }

    /// <summary>
    /// Loads the configuration from JSON file and deserializes it.
    /// </summary>
    /// <returns>The deserialized configuration object.</returns>
    private static DesignConfiguration LoadConfiguration()
    {
        try
        {
            if (!FileAccess.FileExists(ConfigPath))
            {
                GD.PrintErr($"[DesignConfigService] Configuration file not found at {ConfigPath}");
                return new DesignConfiguration();
            }

            var jsonContent = FileAccess.GetFileAsString(ConfigPath);
            var config = JsonConvert.DeserializeObject<DesignConfiguration>(jsonContent);

            if (config == null)
            {
                GD.PrintErr($"[DesignConfigService] Failed to deserialize configuration from {ConfigPath}");
                return new DesignConfiguration();
            }

            GD.Print("[DesignConfigService] Configuration loaded successfully.");
            return config;
        }
        catch (Exception ex)
        {
            GD.PrintErr($"[DesignConfigService] Error loading configuration: {ex.Message}");
            return new DesignConfiguration();
        }
    }
}
