// <copyright file="NarrativeUiTests.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

namespace OmegaSpiral.TestsUi.Narrative;

using GdUnit4;
using System.Threading.Tasks;
using Godot;
using OmegaSpiral.Source.Narrative;
using OmegaSpiral.Source.Ui.Omega;
using static GdUnit4.Assertions;

/// <summary>
/// Unit tests for NarrativeUi class.
/// Validates narrative presentation, shader timing, and choice handling.
/// </summary>
[TestSuite]
public class NarrativeUiTests
{
    /// <summary>
    /// NarrativeBeat should initialize with default values.
    /// </summary>
    [TestCase]
    public void NarrativeBeat_InitializesWithDefaults()
    {
        // Arrange & Act
        var beat = new NarrativeUi.NarrativeBeat();

        // Assert
        AssertThat(beat.Text).IsEqual(string.Empty);
        AssertThat(beat.VisualPreset).IsNull();
        AssertThat(beat.DelaySeconds).IsEqual(0f);
        AssertThat(beat.TypingSpeed).IsEqual(30f);
    }

    /// <summary>
    /// NarrativeBeat should support custom values.
    /// </summary>
    [TestCase]
    public void NarrativeBeat_SupportsCustomValues()
    {
        // Arrange & Act
        var beat = new NarrativeUi.NarrativeBeat
        {
            Text = "Custom text",
            VisualPreset = "glitch_preset",
            DelaySeconds = 1.5f,
            TypingSpeed = 45f
        };

        // Assert
        AssertThat(beat.Text).IsEqual("Custom text");
        AssertThat(beat.VisualPreset).IsEqual("glitch_preset");
        AssertThat(beat.DelaySeconds).IsEqual(1.5f);
        AssertThat(beat.TypingSpeed).IsEqual(45f);
    }

    /// <summary>
    /// NarrativeBeat should support null visual preset.
    /// </summary>
    [TestCase]
    public void NarrativeBeat_SupportsNullVisualPreset()
    {
        // Arrange & Act
        var beat = new NarrativeUi.NarrativeBeat
        {
            Text = "Text without effects",
            VisualPreset = null
        };

        // Assert
        AssertThat(beat.Text).IsEqual("Text without effects");
        AssertThat(beat.VisualPreset).IsNull();
    }

    /// <summary>
    /// NarrativeBeat should support zero delay.
    /// </summary>
    [TestCase]
    public void NarrativeBeat_SupportsZeroDelay()
    {
        // Arrange & Act
        var beat = new NarrativeUi.NarrativeBeat
        {
            Text = "Immediate text",
            DelaySeconds = 0f
        };

        // Assert
        AssertThat(beat.Text).IsEqual("Immediate text");
        AssertThat(beat.DelaySeconds).IsEqual(0f);
    }

    /// <summary>
    /// NarrativeBeat should support custom typing speed.
    /// </summary>
    [TestCase]
    public void NarrativeBeat_SupportsCustomTypingSpeed()
    {
        // Arrange & Act
        var beat = new NarrativeUi.NarrativeBeat
        {
            Text = "Fast text",
            TypingSpeed = 60f
        };

        // Assert
        AssertThat(beat.Text).IsEqual("Fast text");
        AssertThat(beat.TypingSpeed).IsEqual(60f);
    }

    /// <summary>
    /// NarrativeBeat should support negative delay (for special effects).
    /// </summary>
    [TestCase]
    public void NarrativeBeat_SupportsNegativeDelay()
    {
        // Arrange & Act
        var beat = new NarrativeUi.NarrativeBeat
        {
            Text = "Pre-delayed text",
            DelaySeconds = -0.5f
        };

        // Assert
        AssertThat(beat.Text).IsEqual("Pre-delayed text");
        AssertThat(beat.DelaySeconds).IsEqual(-0.5f);
    }

    /// <summary>
    /// NarrativeBeat should support very slow typing speed.
    /// </summary>
    [TestCase]
    public void NarrativeBeat_SupportsVerySlowTypingSpeed()
    {
        // Arrange & Act
        var beat = new NarrativeUi.NarrativeBeat
        {
            Text = "Very slow text",
            TypingSpeed = 10f
        };

        // Assert
        AssertThat(beat.Text).IsEqual("Very slow text");
        AssertThat(beat.TypingSpeed).IsEqual(10f);
    }

    /// <summary>
    /// NarrativeBeat should support empty text with effects.
    /// </summary>
    [TestCase]
    public void NarrativeBeat_SupportsEmptyTextWithEffects()
    {
        // Arrange & Act
        var beat = new NarrativeUi.NarrativeBeat
        {
            Text = string.Empty,
            VisualPreset = "screen_flash",
            DelaySeconds = 0.2f
        };

        // Assert
        AssertThat(beat.Text).IsEqual(string.Empty);
        AssertThat(beat.VisualPreset).IsEqual("screen_flash");
        AssertThat(beat.DelaySeconds).IsEqual(0.2f);
    }

    /// <summary>
    /// NarrativeBeat should support very fast typing speed.
    /// </summary>
    [TestCase]
    public void NarrativeBeat_SupportsVeryFastTypingSpeed()
    {
        // Arrange & Act
        var beat = new NarrativeUi.NarrativeBeat
        {
            Text = "Instant text",
            TypingSpeed = 120f
        };

        // Assert
        AssertThat(beat.Text).IsEqual("Instant text");
        AssertThat(beat.TypingSpeed).IsEqual(120f);
    }

    /// <summary>
    /// NarrativeBeat should support long delay for dramatic effect.
    /// </summary>
    [TestCase]
    public void NarrativeBeat_SupportsLongDelay()
    {
        // Arrange & Act
        var beat = new NarrativeUi.NarrativeBeat
        {
            Text = "Dramatic text",
            DelaySeconds = 5.0f
        };

        // Assert
        AssertThat(beat.Text).IsEqual("Dramatic text");
        AssertThat(beat.DelaySeconds).IsEqual(5.0f);
    }
}
