// <copyright file="OmegaSpiralColors.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using Godot;

namespace OmegaSpiral.Source.Ui.Omega;

/// <summary>
/// Omega Spiral design color palette and animation parameters.
/// Based on the iconic Omega Spiral logo with its spiraling energy streams.
/// Provides single source of truth for visual design compliance testing.
/// </summary>
public static class OmegaSpiralColors
{
    // ==================== COLOR PALETTE ====================

    /// <summary>
    /// Warm Amber - Primary text and glow color.
    /// The golden-orange energy stream from the logo's warm spiral.
    /// RGB(0.992157, 0.788235, 0.384314) = #FEC962
    /// </summary>
    public static readonly Color WarmAmber = new(0.992157f, 0.788235f, 0.384314f, 1.0f);

    /// <summary>
    /// Pure White - Highlight and accent color.
    /// The bright center point and silver spiral threads from the logo.
    /// RGB(1.0, 1.0, 1.0) = #FFFFFF
    /// </summary>
    public static readonly Color PureWhite = new(1.0f, 1.0f, 1.0f, 1.0f);

    /// <summary>
    /// Neon Magenta - Border and glow accent.
    /// The crimson-pink energy stream from the logo's cool spiral.
    /// RGB(0.933333, 0.278431, 0.458824) = #EE4775
    /// </summary>
    public static readonly Color NeonMagenta = new(0.933333f, 0.278431f, 0.458824f, 1.0f);

    /// <summary>
    /// Deep Space - Background color.
    /// The void between the spiraling energy streams.
    /// RGB(0.054902, 0.0666667, 0.0862745) = #0E1116
    /// </summary>
    public static readonly Color DeepSpace = new(0.054902f, 0.0666667f, 0.0862745f, 1.0f);

    // ==================== SHADER LAYER OPACITY PRESETS ====================

    /// <summary>
    /// PhosphorGlow - White with 18% opacity for phosphor glow effect.
    /// Creates the glowing energy stream effect like the logo's center burst.
    /// </summary>
    public static readonly Color PhosphorGlow = PureWhite * new Color(1, 1, 1, 0.18f);

    /// <summary>
    /// ScanlineOverlay - White with 12% opacity for CRT scanline pattern.
    /// Adds retro horizontal lines for authentic CRT aesthetic.
    /// </summary>
    public static readonly Color ScanlineOverlay = PureWhite * new Color(1, 1, 1, 0.12f);

    /// <summary>
    /// GlitchDistortion - White with 8% opacity for glitch distortion effect.
    /// Subtle color separation and warping like spiral distortion in logo.
    /// </summary>
    public static readonly Color GlitchDistortion = PureWhite * new Color(1, 1, 1, 0.08f);

    // ==================== ANIMATION PARAMETERS ====================

    /// <summary>
    /// Spiral rotation speed in radians per second.
    /// Positive = clockwise, Negative = counter-clockwise.
    /// Default: π/4 (45 degrees per second) clockwise.
    /// </summary>
    public const float SpiralRotationSpeed = 0.785398f; // π/4

    /// <summary>
    /// Color cycle duration in seconds.
    /// How long it takes for colors to complete one full rotation around the UI border.
    /// Default: 8 seconds for smooth, meditative motion like the logo's gentle spiral.
    /// </summary>
    public const float ColorCycleDuration = 8.0f;

    /// <summary>
    /// Phosphor pulse frequency in Hz.
    /// How many times per second the glow intensity oscillates.
    /// Default: 0.5Hz (gentle breathing effect, 2 seconds per cycle).
    /// </summary>
    public const float PhosphorPulseFrequency = 0.5f;

    /// <summary>
    /// Phosphor pulse intensity range.
    /// Min/Max opacity values during pulse animation.
    /// Creates breathing effect: 12% (dim) → 24% (bright) → 12% (dim).
    /// </summary>
    public static readonly (float Min, float Max) PhosphorPulseRange = (0.12f, 0.24f);

    // ==================== COLOR TOLERANCE ====================

    /// <summary>
    /// Acceptable color tolerance for float precision in tests.
    /// Allows ±0.01 per RGB channel due to floating-point arithmetic.
    /// </summary>
    public const float ColorTolerance = 0.01f;

    /// <summary>
    /// Acceptable opacity tolerance for shader layer tests.
    /// Allows ±0.02 alpha channel variance.
    /// </summary>
    public const float OpacityTolerance = 0.02f;
}
