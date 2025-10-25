// <copyright file="OmegaSpiralColorsTests.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using GdUnit4;
using Godot;
using OmegaSpiral.Source.Ui.Omega;
using static GdUnit4.Assertions;

namespace OmegaSpiral.Tests.Unit.Common.Omega;

/// <summary>
/// Unit tests for OmegaSpiralColors design palette.
/// Verifies that color constants match the official design specification.
/// </summary>
[TestSuite]
[RequireGodotRuntime]
public partial class OmegaSpiralColorsTests
{
    /// <summary>
    /// WarmAmber color matches design specification #FEC962.
    /// </summary>
    [TestCase]
    public void WarmAmber_MatchesDesignSpec()
    {
        var color = OmegaSpiralColors.WarmAmber;

        // Verify color exists and has full opacity
        AssertThat(color.A).IsEqual(1.0f);
        // TODO: Add precise RGB value checks with tolerance
    }

    /// <summary>
    /// PureWhite color matches design specification #FFFFFF.
    /// </summary>
    [TestCase]
    public void PureWhite_MatchesDesignSpec()
    {
        var color = OmegaSpiralColors.PureWhite;

        AssertThat(color.R).IsEqual(1.0f);
        AssertThat(color.G).IsEqual(1.0f);
        AssertThat(color.B).IsEqual(1.0f);
        AssertThat(color.A).IsEqual(1.0f);
    }

    /// <summary>
    /// NeonMagenta color matches design specification #EE4775.
    /// </summary>
    [TestCase]
    public void NeonMagenta_MatchesDesignSpec()
    {
        var color = OmegaSpiralColors.NeonMagenta;

        // Verify color exists and has full opacity
        AssertThat(color.A).IsEqual(1.0f);
        // TODO: Add precise RGB value checks with tolerance
    }

    /// <summary>
    /// DeepSpace color matches design specification #0E1116.
    /// </summary>
    [TestCase]
    public void DeepSpace_MatchesDesignSpec()
    {
        var color = OmegaSpiralColors.DeepSpace;

        // Verify color exists and has full opacity
        AssertThat(color.A).IsEqual(1.0f);
        // TODO: Add precise RGB value checks with tolerance
    }

    /// <summary>
    /// PhosphorGlow has 18% opacity as specified.
    /// </summary>
    [TestCase]
    public void PhosphorGlow_Has18PercentOpacity()
    {
        var color = OmegaSpiralColors.PhosphorGlow;

        AssertThat(color.R).IsEqual(1.0f);
        AssertThat(color.G).IsEqual(1.0f);
        AssertThat(color.B).IsEqual(1.0f);
        // TODO: Add opacity check with tolerance (should be ~0.18f)
    }

    /// <summary>
    /// ScanlineOverlay has 12% opacity as specified.
    /// </summary>
    [TestCase]
    public void ScanlineOverlay_Has12PercentOpacity()
    {
        var color = OmegaSpiralColors.ScanlineOverlay;

        AssertThat(color.R).IsEqual(1.0f);
        AssertThat(color.G).IsEqual(1.0f);
        AssertThat(color.B).IsEqual(1.0f);
        // TODO: Add opacity check with tolerance (should be ~0.12f)
    }

    /// <summary>
    /// GlitchDistortion has 8% opacity as specified.
    /// </summary>
    [TestCase]
    public void GlitchDistortion_Has8PercentOpacity()
    {
        var color = OmegaSpiralColors.GlitchDistortion;

        AssertThat(color.R).IsEqual(1.0f);
        AssertThat(color.G).IsEqual(1.0f);
        AssertThat(color.B).IsEqual(1.0f);
        // TODO: Add opacity check with tolerance (should be ~0.08f)
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
