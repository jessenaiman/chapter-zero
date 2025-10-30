// <copyright file="OmegaSpiralColors.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using System;
using Godot;

namespace OmegaSpiral.Source.Ui.Omega;

/// <summary>
/// Omega Spiral design color palette and animation parameters.
/// Based on the iconic Omega Spiral logo with its spiraling energy streams.
/// Loads colors from omega_spiral_colors.json for single source of truth.
/// All values are initialized from JSON at static construction time.
/// </summary>
public static class OmegaSpiralColors
{
    private static Godot.Collections.Dictionary<string, Variant> _DesignSystem = new();
    private static bool _IsInitialized;

        /// <summary>
        /// Initializes colors from the omega_spiral_colors.json configuration file.
        /// Called automatically on first property access.
        /// </summary>
        private static void EnsureInitialized()
        {
            if (_IsInitialized)
            {
                return;
            }

            try
            {
                string jsonContent = Godot.FileAccess.GetFileAsString("res://source/resources/omega_spiral_colors.json");
                var json = new Json();
                json.Parse(jsonContent);
                var config = (Godot.Collections.Dictionary<string, Variant>)json.Data;
                if (config.TryGetValue("design_system", out var designSystemVariant))
                {
                    _DesignSystem = (Godot.Collections.Dictionary<string, Variant>)designSystemVariant;
                }

                _IsInitialized = true;
                GD.Print("[OmegaSpiralColors] Initialized from JSON configuration");
            }
            catch (Exception ex)
            {
                GD.PrintErr($"[OmegaSpiralColors] Failed to load configuration: {ex.Message}");
                _IsInitialized = true; // Prevent retry loop, use defaults below
            }
        }    /// <summary>
    /// Helper method to extract Color from design_system dictionary.
    /// Falls back to default value if key not found or initialization fails.
    /// </summary>
    private static Color GetColorFromConfig(string key, Color defaultValue)
    {
        EnsureInitialized();

        if (_DesignSystem.TryGetValue(key, out var colorVariant))
        {
            try
            {
                var colorDict = (Godot.Collections.Dictionary<string, Variant>)colorVariant;
                if (colorDict.TryGetValue("r", out var r) &&
                    colorDict.TryGetValue("g", out var g) &&
                    colorDict.TryGetValue("b", out var b) &&
                    colorDict.TryGetValue("a", out var a))
                {
                    return new Color((float)r, (float)g, (float)b, (float)a);
                }
            }
            catch
            {
                // Fall through to default
            }
        }

        return defaultValue;
    }

    // ==================== COLOR PALETTE ====================

    /// <summary>
    /// Warm Amber - Primary text and glow color.
    /// The golden-orange energy stream from the logo's warm spiral.
    /// RGB(0.992157, 0.788235, 0.384314) = #FEC962
    /// </summary>
    public static Color WarmAmber => GetColorFromConfig("warm_amber", new(0.992157f, 0.788235f, 0.384314f, 1.0f));

    /// <summary>
    /// Pure White - Highlight and accent color.
    /// The bright center point and silver spiral threads from the logo.
    /// RGB(1.0, 1.0, 1.0) = #FFFFFF
    /// </summary>
    public static Color PureWhite => GetColorFromConfig("pure_white", new(1.0f, 1.0f, 1.0f, 1.0f));

    /// <summary>
    /// Neon Magenta - Border and glow accent (DEPRECATED - use thread colors).
    /// The crimson-pink energy stream from the logo's cool spiral.
    /// RGB(0.9, 0.15, 0.1) = #E62619 (now matches AmbitionThread)
    /// NOTE: For borders, use LightThread, ShadowThread, AmbitionThread for animated spiral.
    /// </summary>
    public static Color NeonMagenta => GetColorFromConfig("neon_magenta", new(0.9f, 0.15f, 0.1f, 1.0f));

    /// <summary>
    /// Deep Space - Background color.
    /// The void between the spiraling energy streams.
    /// RGB(0.054902, 0.0666667, 0.0862745) = #0E1116
    /// </summary>
    public static Color DeepSpace => GetColorFromConfig("deep_space", new(0.054902f, 0.0666667f, 0.0862745f, 1.0f));

    /// <summary>
    /// Dark Void - Alternative dark background color for UI elements.
    /// Very dark blue-gray, slightly lighter than pure black.
    /// RGB(0.0352941, 0.0352941, 0.0509804) = #090A0D
    /// </summary>
    public static Color DarkVoid => GetColorFromConfig("dark_void", new(0.0352941f, 0.0352941f, 0.0509804f, 1.0f));

    /// <summary>
    /// Pure Black - The overwhelming backdrop.
    /// The darkness that tries to consume everything, spilling between the energy streams.
    /// RGB(0.0, 0.0, 0.0) = #000000
    /// </summary>
    public static Color PureBlack => GetColorFromConfig("pure_black", new(0.0f, 0.0f, 0.0f, 1.0f));

    /// <summary>
    /// Disabled Gray - Used for disabled UI elements.
    /// 50% opacity gray for indicating non-interactive states.
    /// RGB(0.5, 0.5, 0.5, 0.5)
    /// </summary>
    public static Color DisabledGray => GetColorFromConfig("disabled_gray", new(0.5f, 0.5f, 0.5f, 0.5f));

    /// <summary>
    /// Phosphor Green - Stage 1 CRT monochrome text color.
    /// Authentic 1980s terminal green (approx. #33FF33).
    /// </summary>
    public static Color PhosphorGreen => GetColorFromConfig("phosphor_green", new(0.2f, 1.0f, 0.2f, 1.0f));

    // ==================== DREAMWEAVER THREAD COLORS (FROM LOGO) ====================

    /// <summary>
    /// Light Thread - Silver/White Stream.
    /// The bright silver-white energy stream from the logo (left spiral).
    /// Represents illumination, clarity, and decisive action.
    /// Maps to "Light" Dreamweaver in ghost.yaml scoring system.
    /// RGB(0.95, 0.95, 1.0) = #F2F2FF (bright silver-white with slight blue tint)
    /// </summary>
    public static Color LightThread => GetColorFromConfig("light_thread", new(0.95f, 0.95f, 1.0f, 1.0f));

    /// <summary>
    /// Shadow Thread - Golden/Amber Stream.
    /// The warm golden-amber energy stream from the logo (middle spiral).
    /// Represents wisdom, patience, and measured action.
    /// Maps to "Shadow" Dreamweaver in ghost.yaml scoring system.
    /// RGB(1.0, 0.75, 0.2) = #FFBF33 (warm golden-amber)
    /// </summary>
    public static Color ShadowThread => GetColorFromConfig("shadow_thread", new(1.0f, 0.75f, 0.2f, 1.0f));

    /// <summary>
    /// Ambition Thread - Crimson/Red-Orange Stream.
    /// The deep crimson-red energy stream from the logo (right spiral).
    /// Represents transformation, pragmatism, and self-interest.
    /// Maps to "Ambition" Dreamweaver in ghost.yaml scoring system.
    /// RGB(0.9, 0.15, 0.1) = #E62619 (deep crimson-red)
    /// </summary>
    public static Color AmbitionThread => GetColorFromConfig("ambition_thread", new(0.9f, 0.15f, 0.1f, 1.0f));

    // ==================== SHADER LAYER OPACITY PRESETS ====================

    /// <summary>
    /// PhosphorGlow - White with 30% opacity for phosphor glow effect.
    /// Creates the glowing energy stream effect like the logo's center burst.
    /// Increased from 18% to 30% for stronger terminal aesthetic and better text visibility.
    /// </summary>
    public static Color PhosphorGlow => PureWhite * new Color(1, 1, 1, 0.30f);

    /// <summary>
    /// ScanlineOverlay - White with 18% opacity for CRT scanline pattern.
    /// Adds retro horizontal lines for authentic CRT aesthetic.
    /// Increased from 12% to 18% to create stronger scanline effect on terminal screens.
    /// </summary>
    public static Color ScanlineOverlay => PureWhite * new Color(1, 1, 1, 0.18f);

    /// <summary>
    /// GlitchDistortion - White with 8% opacity for glitch distortion effect.
    /// Subtle color separation and warping like spiral distortion in logo.
    /// </summary>
    public static Color GlitchDistortion => PureWhite * new Color(1, 1, 1, 0.08f);

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

    // ==================== BORDER WIDTH PRESETS ====================

    /// <summary>
    /// Thin border width for minimal UI framing (CRT style).
    /// Used for elegant, retro displays with subtle framing.
    /// Typical use: Credits, minimal dialogs.
    /// </summary>
    public const int BorderWidthThin = 2;

    /// <summary>
    /// Standard border width for primary UI elements.
    /// Used for main menus, quest dialogs, and core game screens.
    /// Provides strong visual presence without overwhelming content.
    /// This is the default/recommended border width.
    /// </summary>
    public const int BorderWidthStandard = 3;

    /// <summary>
    /// Thick border width for emphasis and critical UI.
    /// Used for important dialogs, warnings, and focal point screens.
    /// Creates strong visual separation and command attention.
    /// </summary>
    public const int BorderWidthThick = 4;

    /// <summary>
    /// Acceptable border widths for UI elements.
    /// Any border outside this range will fail design compliance tests.
    /// </summary>
    public static readonly (int Min, int Max) AcceptableBorderWidthRange = (BorderWidthThin, BorderWidthThick);

}
