using Godot;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using OmegaSpiral.Source.Scripts.Infrastructure;

namespace OmegaSpiral.Source.Ui.Omega;

/// <summary>
/// Static provider for Omega UI shader preset configurations.
/// Presets are read from <see cref="DesignConfigService"/> to keep runtime visuals aligned with design docs.
/// </summary>
public static class OmegaShaderPresets
{
    private static readonly object _CacheLock = new();
    private static Dictionary<string, ShaderPresetConfig>? _PresetCache;

    /// <summary>
    /// Gets the legacy phosphor preset (green CRT) for compatibility tests.
    /// </summary>
    public static ShaderPresetConfig PhosphorPreset => GetPresetOrThrow("phosphor");

    /// <summary>
    /// Gets the legacy scanlines preset for compatibility tests.
    /// </summary>
    public static ShaderPresetConfig ScanlinesPreset => GetPresetOrThrow("scanlines");

    /// <summary>
    /// Gets the legacy glitch preset for compatibility tests.
    /// </summary>
    public static ShaderPresetConfig GlitchPreset => GetPresetOrThrow("glitch");

    /// <summary>
    /// Gets the combined CRT preset for compatibility tests.
    /// </summary>
    public static ShaderPresetConfig CrtPreset => GetPresetOrThrow("crt");

    /// <summary>
    /// Gets the terminal preset (no shader).
    /// </summary>
    public static ShaderPresetConfig TerminalPreset => GetPresetOrThrow("terminal");

    /// <summary>
    /// Gets the boot sequence preset defined by the design doc.
    /// </summary>
    public static ShaderPresetConfig BootSequencePreset => GetPresetOrThrow("boot_sequence");

    /// <summary>
    /// Gets the code fragment reveal glitch preset.
    /// </summary>
    public static ShaderPresetConfig CodeFragmentGlitchOverlayPreset => GetPresetOrThrow("code_fragment_glitch_overlay");

    /// <summary>
    /// Gets a shader preset configuration by name.
    /// </summary>
    /// <param name="presetName">The name of the preset to retrieve.</param>
    /// <returns>The preset configuration, or null if not found.</returns>
    public static ShaderPresetConfig? GetPreset(string presetName)
    {
        if (string.IsNullOrWhiteSpace(presetName))
        {
            return null;
        }

        var presets = EnsureCache();
        return presets.TryGetValue(presetName, out var preset) ? preset : null;
    }

    /// <summary>
    /// Gets all available preset names.
    /// </summary>
    /// <returns>A collection of available preset names.</returns>
    public static Collection<string> GetAvailablePresets()
    {
        var presets = EnsureCache();
        return new Collection<string>(presets.Keys.ToList());
    }

    private static Dictionary<string, ShaderPresetConfig> EnsureCache()
    {
        lock (_CacheLock)
        {
            return _PresetCache ??= BuildCache();
        }
    }

    private static Dictionary<string, ShaderPresetConfig> BuildCache()
    {
        var cache = new Dictionary<string, ShaderPresetConfig>(StringComparer.OrdinalIgnoreCase);

        if (!DesignConfigService.DesignConfig.TryGetValue("shader_presets", out var presetsVariant) ||
            presetsVariant.Obj is not Godot.Collections.Dictionary<string, Variant> presetsDict)
        {
            return cache;
        }

        foreach (var keyObj in presetsDict.Keys)
        {
            if (keyObj is not string presetName)
            {
                continue;
            }

            if (!DesignConfigService.TryGetShaderPreset(presetName, out var preset))
            {
                continue;
            }

            cache[presetName] = new ShaderPresetConfig(preset.ShaderPath, preset.Parameters);
        }

        return cache;
    }

    private static ShaderPresetConfig GetPresetOrThrow(string presetName)
    {
        var preset = GetPreset(presetName);
        if (preset == null)
        {
            throw new InvalidOperationException($"Shader preset '{presetName}' was not found in design configuration.");
        }

        return preset;
    }
}

/// <summary>
/// Configuration for a shader preset.
/// </summary>
public sealed class ShaderPresetConfig
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ShaderPresetConfig"/> class.
    /// </summary>
    /// <param name="shaderPath">Shader resource path or <see langword="null"/> for pass-through.</param>
    /// <param name="parameters">Resolved shader parameter dictionary.</param>
    public ShaderPresetConfig(string? shaderPath, Dictionary<string, Variant> parameters)
    {
        ShaderPath = shaderPath;
        Parameters = parameters != null
            ? new Dictionary<string, Variant>(parameters, StringComparer.OrdinalIgnoreCase)
            : new Dictionary<string, Variant>(StringComparer.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Gets the shader resource path, or <see langword="null"/> for a preset that clears the current shader.
    /// </summary>
    public string? ShaderPath { get; }

    /// <summary>
    /// Gets the shader parameters to apply.
    /// </summary>
    public Dictionary<string, Variant> Parameters { get; }
}
