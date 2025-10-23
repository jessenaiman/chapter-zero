using Godot;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace OmegaSpiral.Source.UI.Terminal;

/// <summary>
/// Static provider for terminal shader preset configurations.
/// Contains predefined shader settings for different visual effects.
/// </summary>
public static class TerminalPresetProvider
{
    /// <summary>
    /// Gets a shader preset configuration by name.
    /// </summary>
    /// <param name="presetName">The name of the preset to retrieve.</param>
    /// <returns>The preset configuration, or null if not found.</returns>
    public static ShaderPresetConfig? GetPreset(string presetName)
    {
        return presetName.ToLower() switch
        {
            "phosphor" => PhosphorPreset,
            "scanlines" => ScanlinesPreset,
            "glitch" => GlitchPreset,
            "crt" => CrtPreset,
            "terminal" => TerminalPreset,
            _ => null
        };
    }

    /// <summary>
    /// Gets all available preset names.
    /// </summary>
    /// <returns>A collection of available preset names.</returns>
    public static Collection<string> GetAvailablePresets()
    {
        return new Collection<string> { "phosphor", "scanlines", "glitch", "crt", "terminal" };
    }

    /// <summary>
    /// Phosphor green CRT effect preset.
    /// </summary>
    public static readonly ShaderPresetConfig PhosphorPreset = new()
    {
        ShaderPath = "res://source/shaders/crt_phosphor.tres",
        Parameters = new Dictionary<string, Variant>
        {
            ["phosphor_color"] = new Color(0.0f, 1.0f, 0.0f), // Green
            ["phosphor_intensity"] = 1.2f,
            ["scanline_intensity"] = 0.3f,
            ["glow_strength"] = 0.8f
        }
    };

    /// <summary>
    /// Classic CRT scanlines preset.
    /// </summary>
    public static readonly ShaderPresetConfig ScanlinesPreset = new()
    {
        ShaderPath = "res://source/shaders/crt_scanlines.tres",
        Parameters = new Dictionary<string, Variant>
        {
            ["scanline_color"] = new Color(0.0f, 0.0f, 0.0f), // Black
            ["scanline_opacity"] = 0.4f,
            ["scanline_spacing"] = 2.0f,
            ["brightness"] = 1.1f
        }
    };

    /// <summary>
    /// Glitch effect preset for corrupted terminal display.
    /// </summary>
    public static readonly ShaderPresetConfig GlitchPreset = new()
    {
        ShaderPath = "res://source/shaders/crt_glitch.tres",
        Parameters = new Dictionary<string, Variant>
        {
            ["glitch_intensity"] = 0.6f,
            ["noise_amount"] = 0.3f,
            ["rgb_shift"] = 0.02f,
            ["scanline_jitter"] = 0.1f
        }
    };

    /// <summary>
    /// Combined CRT effect preset.
    /// </summary>
    public static readonly ShaderPresetConfig CrtPreset = new()
    {
        ShaderPath = "res://source/shaders/crt_combined.tres",
        Parameters = new Dictionary<string, Variant>
        {
            ["phosphor_color"] = new Color(0.0f, 1.0f, 0.0f),
            ["phosphor_intensity"] = 1.0f,
            ["scanline_intensity"] = 0.5f,
            ["curvature"] = 0.1f,
            ["brightness"] = 1.0f
        }
    };

    /// <summary>
    /// Clean terminal preset for normal text display.
    /// </summary>
    public static readonly ShaderPresetConfig TerminalPreset = new()
    {
        ShaderPath = null, // No shader for clean terminal
        Parameters = new Dictionary<string, Variant>()
    };
}

/// <summary>
/// Configuration for a shader preset.
/// </summary>
public class ShaderPresetConfig
{
    /// <summary>
    /// Path to the shader resource file, or null for no shader.
    /// </summary>
    public string? ShaderPath { get; set; }

    /// <summary>
    /// Shader parameters to apply.
    /// </summary>
    public Dictionary<string, Variant> Parameters { get; set; } = new();
}
