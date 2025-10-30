// <copyright file="DesignConfigService.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

namespace OmegaSpiral.Source.Config;

using System;
using Godot;
using Newtonsoft.Json;

/// <summary>
/// Service for loading and accessing design-system configuration.
/// Loads individual config files and provides unified access to colors, UI tokens, shaders, and stage presets.
/// </summary>
public static class DesignConfigService
{
    // Config file paths
    private const string DesignSystemPath = "res://source/config/colors_design_system.json";
    private const string UiTokensPath = "res://source/config/ui_tokens_config.json";
    private const string StagePresetsPath = "res://source/config/stage_presets_config.json";

    // Shader config paths
    private const string SpiralBorderPath = "res://source/config/shader_spiral_border_config.json";
    private const string CrtPhosphorPath = "res://source/config/shader_crt_phosphor_config.json";
    private const string CrtScanlinesPath = "res://source/config/shader_crt_scanlines_config.json";
    private const string CrtGlitchPath = "res://source/config/shader_crt_glitch_config.json";
    private const string PulsingBackgroundPath = "res://source/config/shader_pulsing_background_config.json";
    private const string CrtUnifiedPath = "res://source/config/shader_crt_unified_config.json";
    private const string TitleGradientPath = "res://source/config/shader_title_gradient_config.json";

    // Cached configurations
    private static DesignSystemConfig? _designSystem;
    private static UiTokens? _uiTokens;
    private static StagePreset? _stagePresets;
    private static ShaderSpiralBorderConfig? _spiralBorderConfig;
    private static ShaderCrtPhosphorConfig? _crtPhosphorConfig;
    private static ShaderCrtScanlinesConfig? _crtScanlinesConfig;
    private static ShaderCrtGlitchConfig? _crtGlitchConfig;
    private static ShaderPulsingBackgroundConfig? _pulsingBackgroundConfig;
    private static ShaderCrtUnifiedConfig? _crtUnifiedConfig;
    private static ShaderTitleGradientConfig? _titleGradientConfig;

    /// <summary>
    /// Gets the design system configuration (colors).
    /// </summary>
    public static DesignSystemConfig DesignSystem => _designSystem ??= LoadConfig<DesignSystemConfig>(DesignSystemPath);

    /// <summary>
    /// Gets the UI tokens configuration.
    /// </summary>
    public static UiTokens UiTokens => _uiTokens ??= LoadConfig<UiTokens>(UiTokensPath);

    /// <summary>
    /// Gets the stage presets configuration.
    /// </summary>
    public static StagePreset StagePresets => _stagePresets ??= LoadConfig<StagePreset>(StagePresetsPath);

    /// <summary>
    /// Gets the spiral border shader configuration.
    /// </summary>
    public static ShaderSpiralBorderConfig SpiralBorderConfig => _spiralBorderConfig ??= LoadConfig<ShaderSpiralBorderConfig>(SpiralBorderPath);

    /// <summary>
    /// Gets the CRT phosphor shader configuration.
    /// </summary>
    public static ShaderCrtPhosphorConfig CrtPhosphorConfig => _crtPhosphorConfig ??= LoadConfig<ShaderCrtPhosphorConfig>(CrtPhosphorPath);

    /// <summary>
    /// Gets the CRT scanlines shader configuration.
    /// </summary>
    public static ShaderCrtScanlinesConfig CrtScanlinesConfig => _crtScanlinesConfig ??= LoadConfig<ShaderCrtScanlinesConfig>(CrtScanlinesPath);

    /// <summary>
    /// Gets the CRT glitch shader configuration.
    /// </summary>
    public static ShaderCrtGlitchConfig CrtGlitchConfig => _crtGlitchConfig ??= LoadConfig<ShaderCrtGlitchConfig>(CrtGlitchPath);

    /// <summary>
    /// Gets the pulsing background shader configuration.
    /// </summary>
    public static ShaderPulsingBackgroundConfig PulsingBackgroundConfig => _pulsingBackgroundConfig ??= LoadConfig<ShaderPulsingBackgroundConfig>(PulsingBackgroundPath);

    /// <summary>
    /// Gets the CRT unified shader configuration.
    /// </summary>
    public static ShaderCrtUnifiedConfig CrtUnifiedConfig => _crtUnifiedConfig ??= LoadConfig<ShaderCrtUnifiedConfig>(CrtUnifiedPath);

    /// <summary>
    /// Gets the title gradient shader configuration.
    /// </summary>
    public static ShaderTitleGradientConfig TitleGradientConfig => _titleGradientConfig ??= LoadConfig<ShaderTitleGradientConfig>(TitleGradientPath);

    /// <summary>
    /// Attempts to retrieve a shader preset from the configuration.
    /// Maps preset names to individual shader configurations.
    /// </summary>
    /// <param name="presetName">Name of the shader preset.</param>
    /// <param name="preset">The resolved preset configuration.</param>
    /// <returns>True if the preset exists; otherwise false.</returns>
    public static bool TryGetShaderPreset(string presetName, out ShaderPreset? preset)
    {
        preset = null;

        if (string.IsNullOrWhiteSpace(presetName))
        {
            return false;
        }

        // Map preset names to individual shader configs
        switch (presetName.ToLowerInvariant())
        {
            case "spiral_border_base":
                preset = new ShaderPreset
                {
                    Shader = SpiralBorderConfig.Shader,
                    Parameters = SpiralBorderConfig.Parameters?.ToDictionary(p => p.Key, p => p.Value) ?? new Dictionary<string, object>()
                };
                return true;

            case "crt_phosphor_base":
                preset = new ShaderPreset
                {
                    Shader = CrtPhosphorConfig.Shader,
                    Parameters = CrtPhosphorConfig.Parameters?.ToDictionary(p => p.Key, p => p.Value) ?? new Dictionary<string, object>()
                };
                return true;

            case "crt_scanlines_base":
                preset = new ShaderPreset
                {
                    Shader = CrtScanlinesConfig.Shader,
                    Parameters = CrtScanlinesConfig.Parameters?.ToDictionary(p => p.Key, p => p.Value) ?? new Dictionary<string, object>()
                };
                return true;

            case "crt_glitch_base":
                preset = new ShaderPreset
                {
                    Shader = CrtGlitchConfig.Shader,
                    Parameters = CrtGlitchConfig.Parameters?.ToDictionary(p => p.Key, p => p.Value) ?? new Dictionary<string, object>()
                };
                return true;

            case "pulsing_background_base":
                preset = new ShaderPreset
                {
                    Shader = PulsingBackgroundConfig.Shader,
                    Parameters = PulsingBackgroundConfig.Parameters?.ToDictionary(p => p.Key, p => p.Value) ?? new Dictionary<string, object>()
                };
                return true;

            case "crt_unified":
                preset = new ShaderPreset
                {
                    Shader = CrtUnifiedConfig.Shader,
                    Parameters = CrtUnifiedConfig.Parameters?.ToDictionary(p => p.Key, p => p.Value) ?? new Dictionary<string, object>()
                };
                return true;

            case "title_gradient":
                preset = new ShaderPreset
                {
                    Shader = TitleGradientConfig.Shader,
                    Parameters = TitleGradientConfig.Parameters?.ToDictionary(p => p.Key, p => p.Value) ?? new Dictionary<string, object>()
                };
                return true;

            // Legacy presets that need to be mapped
            case "phosphor":
                preset = new ShaderPreset
                {
                    Shader = CrtPhosphorConfig.Shader,
                    Parameters = new Dictionary<string, object>
                    {
                        ["phosphor_color"] = new { r = 0.0f, g = 1.0f, b = 0.0f, a = 1.0f },
                        ["phosphor_intensity"] = 1.2f,
                        ["scanline_intensity"] = 0.3f,
                        ["glow_strength"] = 0.8f
                    }
                };
                return true;

            case "scanlines":
                preset = new ShaderPreset
                {
                    Shader = CrtScanlinesConfig.Shader,
                    Parameters = new Dictionary<string, object>
                    {
                        ["scanline_color"] = new { r = 0.0f, g = 0.0f, b = 0.0f, a = 1.0f },
                        ["scanline_opacity"] = 0.4f,
                        ["scanline_spacing"] = 2.0f,
                        ["brightness"] = 1.1f
                    }
                };
                return true;

            case "glitch":
                preset = new ShaderPreset
                {
                    Shader = CrtGlitchConfig.Shader,
                    Parameters = new Dictionary<string, object>
                    {
                        ["glitch_intensity"] = 0.6f,
                        ["noise_amount"] = 0.3f,
                        ["rgb_shift"] = 0.02f,
                        ["scanline_jitter"] = 0.1f
                    }
                };
                return true;

            case "crt":
                preset = new ShaderPreset
                {
                    Shader = CrtUnifiedConfig.Shader,
                    Parameters = new Dictionary<string, object>
                    {
                        ["phosphor_color"] = new { r = 0.0f, g = 1.0f, b = 0.0f, a = 1.0f },
                        ["phosphor_intensity"] = 1.0f,
                        ["scanline_intensity"] = 0.5f,
                        ["curvature"] = 0.1f,
                        ["brightness"] = 1.0f
                    }
                };
                return true;

            case "terminal":
                // Clean terminal - no shader
                preset = new ShaderPreset
                {
                    Shader = "",
                    Parameters = new Dictionary<string, object>()
                };
                return true;

            case "boot_sequence":
                preset = new ShaderPreset
                {
                    Shader = CrtGlitchConfig.Shader,
                    Parameters = new Dictionary<string, object>
                    {
                        ["glitch_intensity"] = 1.0f,
                        ["scanline_speed"] = 3.0f,
                        ["rgb_split"] = 7.0f,
                        ["symbol_bleed"] = 0.8f,
                        ["noise_amount"] = 0.6f
                    }
                };
                return true;

            case "stable_baseline_glitch_spike":
                preset = new ShaderPreset
                {
                    Shader = CrtGlitchConfig.Shader,
                    Parameters = new Dictionary<string, object>
                    {
                        ["glitch_intensity"] = 0.5f,
                        ["chromatic_offset"] = 2.0f,
                        ["noise_amount"] = 0.2f
                    }
                };
                return true;

            case "code_fragment_glitch_overlay":
                preset = new ShaderPreset
                {
                    Shader = CrtGlitchConfig.Shader,
                    Parameters = new Dictionary<string, object>
                    {
                        ["glitch_intensity"] = 1.0f,
                        ["chromatic_offset"] = 2.0f,
                        ["noise_amount"] = 0.8f,
                        ["interference_speed"] = 15.0f,
                        ["block_size"] = 32.0f
                    }
                };
                return true;

            case "dreamweaver_selection_light":
                preset = new ShaderPreset
                {
                    Shader = CrtPhosphorConfig.Shader,
                    Parameters = new Dictionary<string, object>
                    {
                        ["phosphor_tint"] = new { r = 0.95f, g = 0.95f, b = 1.0f, a = 1.0f },
                        ["scanline_intensity"] = 0.25f,
                        ["flicker_frequency"] = 0.8f,
                        ["vignette_strength"] = 0.15f,
                        ["glow_spread"] = 1.4f
                    }
                };
                return true;

            case "dreamweaver_selection_shadow":
                preset = new ShaderPreset
                {
                    Shader = CrtPhosphorConfig.Shader,
                    Parameters = new Dictionary<string, object>
                    {
                        ["phosphor_tint"] = new { r = 0.6f, g = 0.3f, b = 0.8f, a = 1.0f },
                        ["scanline_intensity"] = 0.45f,
                        ["rgb_split"] = 1.5f,
                        ["flicker_frequency"] = 2.0f,
                        ["vignette_strength"] = 0.5f
                    }
                };
                return true;

            case "dreamweaver_selection_ambition":
                preset = new ShaderPreset
                {
                    Shader = CrtPhosphorConfig.Shader,
                    Parameters = new Dictionary<string, object>
                    {
                        ["phosphor_tint"] = new { r = 0.9f, g = 0.15f, b = 0.1f, a = 1.0f },
                        ["scanline_intensity"] = 0.55f,
                        ["flicker_frequency"] = 3.5f,
                        ["vignette_strength"] = 0.45f,
                        ["glow_spread"] = 0.8f
                    }
                };
                return true;

            default:
                return false;
        }
    }

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
            if (parts.Length != 2 || DesignSystem == null)
            {
                return false;
            }

            var section = parts[0];
            var propertyName = parts[1];

            if (!section.Equals("design_system", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            var designSystem = DesignSystem;
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
                color = new Color(colorValue.R, colorValue.G, colorValue.B, colorValue.A);
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
    /// Loads a configuration from JSON file and deserializes it.
    /// </summary>
    /// <typeparam name="T">The type to deserialize to.</typeparam>
    /// <param name="path">The resource path to the JSON file.</param>
    /// <returns>The deserialized configuration object.</returns>
    private static T LoadConfig<T>(string path) where T : new()
    {
        try
        {
            if (!FileAccess.FileExists(path))
            {
                GD.PrintErr($"[DesignConfigService] Configuration file not found at {path}");
                return new T();
            }

            var jsonContent = FileAccess.GetFileAsString(path);
            var config = JsonConvert.DeserializeObject<T>(jsonContent);

            if (config == null)
            {
                GD.PrintErr($"[DesignConfigService] Failed to deserialize configuration from {path}");
                return new T();
            }

            GD.Print($"[DesignConfigService] Loaded configuration from {path}");
            return config;
        }
        catch (Exception ex)
        {
            GD.PrintErr($"[DesignConfigService] Error loading configuration from {path}: {ex.Message}");
            return new T();
        }
    }
}
