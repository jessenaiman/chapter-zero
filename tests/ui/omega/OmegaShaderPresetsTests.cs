// <copyright file="OmegaShaderPresetsTests.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using GdUnit4;
using OmegaSpiral.Source.Ui.Omega;
using static GdUnit4.Assertions;

namespace OmegaSpiral.Tests.Unit.Common.Omega;

/// <summary>
/// Unit tests for OmegaShaderPresets static provider.
/// Verifies preset configurations and availability without requiring Godot runtime.
/// </summary>
[TestSuite]
[RequireGodotRuntime]
public partial class OmegaShaderPresetsTests
{
    /// <summary>
    /// GetPreset returns correct preset for phosphor.
    /// </summary>
    [TestCase]
    public void GetPreset_ReturnsPhosphorPreset()
    {
        var preset = OmegaShaderPresets.GetPreset("phosphor");

        AssertThat(preset).IsNotNull();
        AssertThat(preset!.ShaderPath).IsEqual("res://source/shaders/crt_phosphor.tres");
        AssertThat(preset.Parameters).IsNotNull();
        AssertThat(preset.Parameters).ContainsKeys(
            new string[] { "phosphor_color", "phosphor_intensity", "scanline_intensity", "glow_strength" });
    }

    /// <summary>
    /// GetPreset returns correct preset for scanlines.
    /// </summary>
    [TestCase]
    public void GetPreset_ReturnsScanlinesPreset()
    {
        var preset = OmegaShaderPresets.GetPreset("scanlines");

        AssertThat(preset).IsNotNull();
        AssertThat(preset!.ShaderPath).IsEqual("res://source/shaders/crt_scanlines.tres");
        AssertThat(preset.Parameters).IsNotNull();
        AssertThat(preset.Parameters).ContainsKeys(
            new string[] { "scanline_color", "scanline_opacity", "scanline_spacing", "brightness" });
    }

    /// <summary>
    /// GetPreset returns correct preset for glitch.
    /// </summary>
    [TestCase]
    public void GetPreset_ReturnsGlitchPreset()
    {
        var preset = OmegaShaderPresets.GetPreset("glitch");

        AssertThat(preset).IsNotNull();
        AssertThat(preset!.ShaderPath).IsEqual("res://source/shaders/crt_glitch.tres");
        AssertThat(preset.Parameters).IsNotNull();
        AssertThat(preset.Parameters).ContainsKeys(
            new string[] { "glitch_intensity", "noise_amount", "rgb_shift", "scanline_jitter" });
    }

    /// <summary>
    /// GetPreset returns correct preset for CRT.
    /// </summary>
    [TestCase]
    public void GetPreset_ReturnsCrtPreset()
    {
        var preset = OmegaShaderPresets.GetPreset("crt");

        AssertThat(preset).IsNotNull();
        AssertThat(preset!.ShaderPath).IsEqual("res://source/shaders/crt_combined.tres");
        AssertThat(preset.Parameters).IsNotNull();
        AssertThat(preset.Parameters).ContainsKeys(
            new string[] { "phosphor_color", "phosphor_intensity", "scanline_intensity", "curvature", "brightness" });
    }

    /// <summary>
    /// GetPreset returns terminal preset with no shader.
    /// </summary>
    [TestCase]
    public void GetPreset_ReturnsTerminalPresetWithNoShader()
    {
        var preset = OmegaShaderPresets.GetPreset("terminal");

        AssertThat(preset).IsNotNull();
        AssertThat(preset!.ShaderPath).IsNull();
        AssertThat(preset.Parameters).IsNotNull();
        AssertThat(preset.Parameters).IsEmpty();
    }

    /// <summary>
    /// GetPreset is case-insensitive.
    /// </summary>
    [TestCase]
    public void GetPreset_IsCaseInsensitive()
    {
        var presetLower = OmegaShaderPresets.GetPreset("phosphor");
        var presetUpper = OmegaShaderPresets.GetPreset("PHOSPHOR");
        var presetMixed = OmegaShaderPresets.GetPreset("PhOsPhOr");

        AssertThat(presetLower).IsNotNull();
        AssertThat(presetUpper).IsNotNull();
        AssertThat(presetMixed).IsNotNull();
        // TODO: Compare shader paths when non-null comparison is resolved
    }

    /// <summary>
    /// GetPreset returns null for unknown preset.
    /// </summary>
    [TestCase]
    public void GetPreset_ReturnsNullForUnknownPreset()
    {
        var preset = OmegaShaderPresets.GetPreset("unknown");

        AssertThat(preset).IsNull();
    }

    /// <summary>
    /// GetAvailablePresets returns all preset names.
    /// </summary>
    [TestCase]
    public void GetAvailablePresets_ReturnsAllPresetNames()
    {
        var presets = OmegaShaderPresets.GetAvailablePresets();

        AssertThat(presets).IsNotNull();
        AssertThat(presets.Count).IsGreaterEqual(7);
        AssertThat(presets).Contains("phosphor", "scanlines", "glitch", "crt", "terminal", "boot_sequence", "code_fragment_glitch_overlay");
    }

    /// <summary>
    /// All available presets can be retrieved by name.
    /// </summary>
    [TestCase]
    public void AvailablePresets_AllCanBeRetrieved()
    {
        var presetNames = OmegaShaderPresets.GetAvailablePresets();

        foreach (var presetName in presetNames)
        {
            var preset = OmegaShaderPresets.GetPreset(presetName);
            AssertThat(preset).IsNotNull()
                .OverrideFailureMessage($"Preset '{presetName}' should be retrievable");
        }
    }

    /// <summary>
    /// Phosphor preset has correct green color value.
    /// </summary>
    [TestCase]
    public void PhosphorPreset_HasCorrectGreenColor()
    {
        var preset = OmegaShaderPresets.GetPreset("phosphor");

        AssertThat(preset).IsNotNull();
        AssertThat(preset!.Parameters).ContainsKeys(new string[] { "phosphor_color" });

        var color = preset.Parameters["phosphor_color"].AsColor();
        AssertThat(color.R).IsEqual(0.0f);
        AssertThat(color.G).IsEqual(1.0f);
        AssertThat(color.B).IsEqual(0.0f);
    }

    /// <summary>
    /// Static preset references are consistent.
    /// </summary>
    [TestCase]
    public void StaticPresets_AreConsistent()
    {
        var phosphor1 = OmegaShaderPresets.PhosphorPreset;
        var phosphor2 = OmegaShaderPresets.PhosphorPreset;

        AssertThat(phosphor1).IsSame(phosphor2);
        // TODO: Compare shader paths when non-null comparison is resolved
    }
}
