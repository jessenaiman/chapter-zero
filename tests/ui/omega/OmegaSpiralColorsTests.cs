// <copyright file="OmegaSpiralColorsTests.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

namespace OmegaSpiral.Tests.Unit.Common.Omega
{
    using GdUnit4;
    using OmegaSpiral.Source.Design;
    using OmegaSpiral.Source.Ui.Omega;
    using static GdUnit4.Assertions;

    /// <summary>
    /// Unit tests for OmegaSpiralColors design palette.
    /// Verifies that color constants match the official design specification from omega_spiral_colors.json.
    /// </summary>
    [TestSuite]
    [RequireGodotRuntime]
    public partial class OmegaSpiralColorsTests
{
    /// <summary>
    /// WarmAmber color matches design specification from design document.
    /// </summary>
    [TestCase]
    public void WarmAmber_MatchesDesignSpec()
    {
        var color = OmegaSpiralColors.WarmAmber;
        var designColor = DesignConfigService.GetColor("warm_amber");

        // Verify color exists and has full opacity
        AssertThat(color.A).IsEqual(1.0f);
        // Check RGB values match design document
        AssertThat(Math.Abs(color.R - designColor.R)).IsLess(OmegaSpiralColors.ColorTolerance);
        AssertThat(Math.Abs(color.G - designColor.G)).IsLess(OmegaSpiralColors.ColorTolerance);
        AssertThat(Math.Abs(color.B - designColor.B)).IsLess(OmegaSpiralColors.ColorTolerance);
    }

    /// <summary>
    /// PureWhite color matches design specification from design document.
    /// </summary>
    [TestCase]
    public void PureWhite_MatchesDesignSpec()
    {
        var color = OmegaSpiralColors.PureWhite;
        var designColor = DesignConfigService.GetColor("pure_white");

        AssertThat(color.R).IsEqual(designColor.R);
        AssertThat(color.G).IsEqual(designColor.G);
        AssertThat(color.B).IsEqual(designColor.B);
        AssertThat(color.A).IsEqual(designColor.A);
    }

    /// <summary>
    /// NeonMagenta color matches design specification from design document.
    /// </summary>
    [TestCase]
    public void NeonMagenta_MatchesDesignSpec()
    {
        var color = OmegaSpiralColors.NeonMagenta;
        var designColor = DesignConfigService.GetColor("neon_magenta");

        // Verify color exists and has full opacity
        AssertThat(color.A).IsEqual(1.0f);
        // Check RGB values match design document
        AssertThat(Math.Abs(color.R - designColor.R)).IsLess(OmegaSpiralColors.ColorTolerance);
        AssertThat(Math.Abs(color.G - designColor.G)).IsLess(OmegaSpiralColors.ColorTolerance);
        AssertThat(Math.Abs(color.B - designColor.B)).IsLess(OmegaSpiralColors.ColorTolerance);
    }

    /// <summary>
    /// DeepSpace color matches design specification from design document.
    /// </summary>
    [TestCase]
    public void DeepSpace_MatchesDesignSpec()
    {
        var color = OmegaSpiralColors.DeepSpace;
        var designColor = DesignConfigService.GetColor("deep_space");

        // Verify color exists and has full opacity
        AssertThat(color.A).IsEqual(1.0f);
        // Check RGB values match design document
        AssertThat(Math.Abs(color.R - designColor.R)).IsLess(OmegaSpiralColors.ColorTolerance);
        AssertThat(Math.Abs(color.G - designColor.G)).IsLess(OmegaSpiralColors.ColorTolerance);
        AssertThat(Math.Abs(color.B - designColor.B)).IsLess(OmegaSpiralColors.ColorTolerance);
    }

    /// <summary>
    /// PhosphorGlow has opacity as specified in design document.
    /// </summary>
    [TestCase]
    public void PhosphorGlow_HasCorrectOpacity()
    {
        var color = OmegaSpiralColors.PhosphorGlow;
        var designColor = DesignConfigService.GetColor("phosphor_glow");

        AssertThat(color.R).IsEqual(designColor.R);
        AssertThat(color.G).IsEqual(designColor.G);
        AssertThat(color.B).IsEqual(designColor.B);
        // Check opacity matches design document
        AssertThat(Math.Abs(color.A - designColor.A)).IsLess(OmegaSpiralColors.OpacityTolerance);
    }

    /// <summary>
    /// ScanlineOverlay has opacity as specified in design document.
    /// </summary>
    [TestCase]
    public void ScanlineOverlay_HasCorrectOpacity()
    {
        var color = OmegaSpiralColors.ScanlineOverlay;
        var designColor = DesignConfigService.GetColor("scanline_overlay");

        AssertThat(color.R).IsEqual(designColor.R);
        AssertThat(color.G).IsEqual(designColor.G);
        AssertThat(color.B).IsEqual(designColor.B);
        // Check opacity matches design document
        AssertThat(Math.Abs(color.A - designColor.A)).IsLess(OmegaSpiralColors.OpacityTolerance);
    }

    /// <summary>
    /// GlitchDistortion has opacity as specified in design document.
    /// </summary>
    [TestCase]
    public void GlitchDistortion_HasCorrectOpacity()
    {
        var color = OmegaSpiralColors.GlitchDistortion;
        var designColor = DesignConfigService.GetColor("glitch_distortion");

        AssertThat(color.R).IsEqual(designColor.R);
        AssertThat(color.G).IsEqual(designColor.G);
        AssertThat(color.B).IsEqual(designColor.B);
        // Check opacity matches design document
        AssertThat(Math.Abs(color.A - designColor.A)).IsLess(OmegaSpiralColors.OpacityTolerance);
    }

    // ==================== THREAD COLORS ====================

    /// <summary>
    /// LightThread color matches design specification.
    /// </summary>
    [TestCase]
    public void LightThread_MatchesDesignSpec()
    {
        var color = OmegaSpiralColors.LightThread;
        var designColor = DesignConfigService.GetColor("light_thread");

        AssertThat(Math.Abs(color.R - designColor.R)).IsLess(OmegaSpiralColors.ColorTolerance);
        AssertThat(Math.Abs(color.G - designColor.G)).IsLess(OmegaSpiralColors.ColorTolerance);
        AssertThat(Math.Abs(color.B - designColor.B)).IsLess(OmegaSpiralColors.ColorTolerance);
        AssertThat(color.A).IsEqual(1.0f);
    }

    /// <summary>
    /// ShadowThread color matches design specification.
    /// </summary>
    [TestCase]
    public void ShadowThread_MatchesDesignSpec()
    {
        var color = OmegaSpiralColors.ShadowThread;
        var designColor = DesignConfigService.GetColor("shadow_thread");

        AssertThat(Math.Abs(color.R - designColor.R)).IsLess(OmegaSpiralColors.ColorTolerance);
        AssertThat(Math.Abs(color.G - designColor.G)).IsLess(OmegaSpiralColors.ColorTolerance);
        AssertThat(Math.Abs(color.B - designColor.B)).IsLess(OmegaSpiralColors.ColorTolerance);
        AssertThat(color.A).IsEqual(1.0f);
    }

    /// <summary>
    /// AmbitionThread color matches design specification.
    /// </summary>
    [TestCase]
    public void AmbitionThread_MatchesDesignSpec()
    {
        var color = OmegaSpiralColors.AmbitionThread;
        var designColor = DesignConfigService.GetColor("ambition_thread");

        AssertThat(Math.Abs(color.R - designColor.R)).IsLess(OmegaSpiralColors.ColorTolerance);
        AssertThat(Math.Abs(color.G - designColor.G)).IsLess(OmegaSpiralColors.ColorTolerance);
        AssertThat(Math.Abs(color.B - designColor.B)).IsLess(OmegaSpiralColors.ColorTolerance);
        AssertThat(color.A).IsEqual(1.0f);
    }

    /// <summary>
    /// SpiralRotationSpeed is π/4 radians per second.
    /// </summary>
    [TestCase]
    public void SpiralRotationSpeed_IsPiOver4()
    {
        var speed = OmegaSpiralColors.SpiralRotationSpeed;

        // Verify constant exists
        AssertThat(speed).IsEqual(0.785398f);
        // TODO: Add comparison to Mathf.Pi / 4 with tolerance
    }

    /// <summary>
    /// ColorCycleDuration is 8 seconds.
    /// </summary>
    [TestCase]
    public void ColorCycleDuration_Is8Seconds()
    {
        AssertThat(OmegaSpiralColors.ColorCycleDuration).IsEqual(8.0f);
    }

    /// <summary>
    /// PhosphorPulseFrequency is 0.5 Hz.
    /// </summary>
    [TestCase]
    public void PhosphorPulseFrequency_IsHalfHz()
    {
        AssertThat(OmegaSpiralColors.PhosphorPulseFrequency).IsEqual(0.5f);
    }

    /// <summary>
    /// PhosphorPulseRange has correct min/max values.
    /// </summary>
    [TestCase]
    public void PhosphorPulseRange_HasCorrectMinMax()
    {
        var range = OmegaSpiralColors.PhosphorPulseRange;

        AssertThat(range.Min).IsEqual(0.12f);
        AssertThat(range.Max).IsEqual(0.24f);
    }

    /// <summary>
    /// ColorTolerance is acceptable for float precision.
    /// </summary>
    [TestCase]
    public void ColorTolerance_IsAppropriate()
    {
        AssertThat(OmegaSpiralColors.ColorTolerance).IsEqual(0.01f);
    }

    /// <summary>
    /// OpacityTolerance is acceptable for shader layer tests.
    /// </summary>
    [TestCase]
    public void OpacityTolerance_IsAppropriate()
    {
        AssertThat(OmegaSpiralColors.OpacityTolerance).IsEqual(0.02f);
    }

    /// <summary>
    /// All primary colors have full opacity.
    /// </summary>
    [TestCase]
    public void PrimaryColors_HaveFullOpacity()
    {
        AssertThat(OmegaSpiralColors.WarmAmber.A).IsEqual(1.0f);
        AssertThat(OmegaSpiralColors.PureWhite.A).IsEqual(1.0f);
        AssertThat(OmegaSpiralColors.NeonMagenta.A).IsEqual(1.0f);
        AssertThat(OmegaSpiralColors.DeepSpace.A).IsEqual(1.0f);
    }

    /// <summary>
    /// Shader layer colors are all white with varying opacity.
    /// </summary>
    [TestCase]
    public void ShaderLayerColors_AreWhiteWithVaryingOpacity()
    {
        // PhosphorGlow
        AssertThat(OmegaSpiralColors.PhosphorGlow.R).IsEqual(1.0f);
        AssertThat(OmegaSpiralColors.PhosphorGlow.G).IsEqual(1.0f);
        AssertThat(OmegaSpiralColors.PhosphorGlow.B).IsEqual(1.0f);

        // ScanlineOverlay
        AssertThat(OmegaSpiralColors.ScanlineOverlay.R).IsEqual(1.0f);
        AssertThat(OmegaSpiralColors.ScanlineOverlay.G).IsEqual(1.0f);
        AssertThat(OmegaSpiralColors.ScanlineOverlay.B).IsEqual(1.0f);

        // GlitchDistortion
        AssertThat(OmegaSpiralColors.GlitchDistortion.R).IsEqual(1.0f);
        AssertThat(OmegaSpiralColors.GlitchDistortion.G).IsEqual(1.0f);
        AssertThat(OmegaSpiralColors.GlitchDistortion.B).IsEqual(1.0f);
    }
}
}
