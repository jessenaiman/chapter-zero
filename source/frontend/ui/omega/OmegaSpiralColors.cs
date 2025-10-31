using Godot;
using OmegaSpiral.Source.Frontend.Design;

namespace OmegaSpiral.Source.Ui.Omega;

/// <summary>
/// Lightweight palette helper that surfaces commonly used Ωmega Spiral colors and constants.
/// Internally delegates to <see cref="DesignService"/> so tests and legacy callers remain stable
/// while the new single-source JSON configuration drives actual values.
/// </summary>
public static class OmegaSpiralColors
{
    /// <summary>Numeric tolerance for comparing RGB channels in tests.</summary>
    public const float ColorTolerance = 0.01f;

    /// <summary>Numeric tolerance for comparing alpha channels in tests.</summary>
    public const float OpacityTolerance = 0.02f;

    /// <summary>Default spiral rotation speed in radians per second.</summary>
    public const float SpiralRotationSpeed = 0.785398f; // π/4

    /// <summary>Phosphor pulse frequency in Hz.</summary>
    public const float PhosphorPulseFrequency = 0.5f;

    /// <summary>Color cycle duration in seconds.</summary>
    public const float ColorCycleDuration = 8.0f;

    /// <summary>
    /// Retrieves the "warm_amber" palette color.
    /// </summary>
    public static Color WarmAmber => Get("warm_amber");

    /// <summary>
    /// Retrieves the "pure_white" palette color.
    /// </summary>
    public static Color PureWhite => Get("pure_white");

    /// <summary>
    /// Retrieves the "neon_magenta" palette color.
    /// </summary>
    public static Color NeonMagenta => Get("neon_magenta");

    /// <summary>
    /// Retrieves the "deep_space" palette color.
    /// </summary>
    public static Color DeepSpace => Get("deep_space");

    /// <summary>
    /// Retrieves the "glitch_distortion" palette color.
    /// </summary>
    public static Color GlitchDistortion => Get("glitch_distortion");

    /// <summary>
    /// Retrieves the "scanline_overlay" palette color.
    /// </summary>
    public static Color ScanlineOverlay => Get("scanline_overlay");

    /// <summary>
    /// Retrieves the "phosphor_glow" palette color.
    /// </summary>
    public static Color PhosphorGlow => Get("phosphor_glow");

    /// <summary>
    /// Retrieves the "light_thread" palette color.
    /// </summary>
    public static Color LightThread => Get("light_thread");

    /// <summary>
    /// Retrieves the "shadow_thread" palette color.
    /// </summary>
    public static Color ShadowThread => Get("shadow_thread");

    /// <summary>
    /// Retrieves the "ambition_thread" palette color.
    /// </summary>
    public static Color AmbitionThread => Get("ambition_thread");

    public static PulseRange PhosphorPulseRange => new(0.12f, 0.24f);

    private static Color Get(string key)
    {
        return DesignService.GetColor(key);
    }

    /// <summary>
    /// Simple immutable range structure for shader pulse animations.
    /// </summary>
    public readonly struct PulseRange
    {
        public PulseRange(float min, float max)
        {
            Min = min;
            Max = max;
        }

        /// <summary>Minimum pulse intensity.</summary>
        public float Min { get; }

        /// <summary>Maximum pulse intensity.</summary>
        public float Max { get; }
    }
}
