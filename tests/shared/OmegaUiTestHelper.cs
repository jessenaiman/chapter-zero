// <copyright file="MenuUiTestHelper.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using Godot;
using OmegaSpiral.Source.Ui.Omega;
using static GdUnit4.Assertions;

namespace OmegaSpiral.Tests.Shared;

/// <summary>
/// Shared test helper for validating MenuUI components.
/// Provides centralized background/theme validation to ensure all menu tests
/// properly detect when UI is broken (white background instead of themed).
/// </summary>
public static class OmegaUiTestHelper
{
    /// <summary>
    /// Validates that a UI component has proper background layers with correct theming.
    /// This validation ensures the UI is not showing a white background, which indicates
    /// broken theme/styling. For OmegaThemedContainer with EnableOmegaBorder=true, validates
    /// that visual layers exist. For OmegaContainer and other classes, this is a no-op.
    /// If any validation fails, all subsequent tests in the suite will cascade fail.
    /// </summary>
    /// <param name="uiComponent">The UI component to validate (OmegaThemedContainer, BaseMenuUi, etc.).</param>
    /// <param name="componentName">Name of the component for error messages (e.g., "BaseMenuUi", "PauseMenu").</param>
    /// <exception cref="AssertionException">Thrown when validation fails, causing all tests to fail.</exception>
    public static void ValidateBackgroundTheme(Control uiComponent, string componentName = "UI Component")
    {
        // CRITICAL: Validate UI component exists
        AssertThat(uiComponent).IsNotNull()
            .OverrideFailureMessage($"{componentName} failed to instantiate - all tests will fail");

        // Only validate visual layers if this is an OmegaThemedContainer
        // OmegaContainer (base class for BaseMenuUi) has no visual nodes - that's by design
        // This check allows the new architecture to coexist with old visual tests
        if (uiComponent is not OmegaThemedContainer themedContainer)
        {
            return; // Pure OmegaContainer (no visuals) - validation complete
        }

        // For OmegaThemedContainer, only validate if Omega border is enabled
        // (visual layers are only created when EnableOmegaBorder=true)
        if (!themedContainer.EnableOmegaBorder)
        {
            return; // Visual layers disabled - validation complete
        }

        // Validate dark background exists (required base for overlay layers)
        var background = uiComponent.GetNodeOrNull<ColorRect>("Background");
        AssertThat(background).IsNotNull()
            .OverrideFailureMessage($"{componentName}: Background missing - UI has no dark base for overlays - all tests will fail");

        // Validate background layers exist and are properly colored
        // Access layers via GetNode since they're protected in OmegaThemedContainer
        var phosphorLayer = uiComponent.GetNodeOrNull<ColorRect>("PhosphorLayer");
        AssertThat(phosphorLayer).IsNotNull()
            .OverrideFailureMessage($"{componentName}: PhosphorLayer missing - UI has no background theme - all tests will fail");

        var scanlineLayer = uiComponent.GetNodeOrNull<ColorRect>("ScanlineLayer");
        AssertThat(scanlineLayer).IsNotNull()
            .OverrideFailureMessage($"{componentName}: ScanlineLayer missing - UI has no background theme - all tests will fail");

        var glitchLayer = uiComponent.GetNodeOrNull<ColorRect>("GlitchLayer");
        AssertThat(glitchLayer).IsNotNull()
            .OverrideFailureMessage($"{componentName}: GlitchLayer missing - UI has no background theme - all tests will fail");

        // Validate layers are NOT fully opaque white (which is the broken state)
        // The layers should be semi-transparent white overlays with specific alpha values from OmegaSpiralColors
        // PhosphorGlow: 18% alpha, ScanlineOverlay: 12% alpha, GlitchDistortion: 8% alpha
        GD.Print($"[OmegaUiTestHelper] Checking {componentName} background layers:");
        GD.Print($"  PhosphorLayer exists: {phosphorLayer != null}, Color: {phosphorLayer?.Color}");
        GD.Print($"  ScanlineLayer exists: {scanlineLayer != null}, Color: {scanlineLayer?.Color}");
        GD.Print($"  GlitchLayer exists: {glitchLayer != null}, Color: {glitchLayer?.Color}");

        // Check that layers are NOT fully opaque white (Alpha = 1.0) which indicates broken/uninitialized state
        // Layers should have the correct semi-transparent overlay colors from OmegaSpiralColors
        bool isPhosphorBroken = phosphorLayer != null && IsBrokenWhite(phosphorLayer.Color);
        bool isScanlineBroken = scanlineLayer != null && IsBrokenWhite(scanlineLayer.Color);
        bool isGlitchBroken = glitchLayer != null && IsBrokenWhite(glitchLayer.Color);

        AssertThat(isPhosphorBroken).IsFalse()
            .OverrideFailureMessage($"{componentName}: PhosphorLayer is fully opaque WHITE ({phosphorLayer?.Color}) - not themed overlay - UI background is broken - all tests will fail");

        AssertThat(isScanlineBroken).IsFalse()
            .OverrideFailureMessage($"{componentName}: ScanlineLayer is fully opaque WHITE ({scanlineLayer?.Color}) - not themed overlay - UI background is broken - all tests will fail");

        AssertThat(isGlitchBroken).IsFalse()
            .OverrideFailureMessage($"{componentName}: GlitchLayer is fully opaque WHITE ({glitchLayer?.Color}) - not themed overlay - UI background is broken - all tests will fail");

        // Validate expected colors from OmegaSpiralColors (overlay colors with specific alpha)
        if (phosphorLayer != null && !isPhosphorBroken)
        {
            AssertThat(ColorApproximatelyEqual(phosphorLayer.Color, OmegaSpiralColors.PhosphorGlow)).IsTrue()
                .OverrideFailureMessage($"{componentName}: PhosphorLayer has wrong color. Expected: {OmegaSpiralColors.PhosphorGlow}, Got: {phosphorLayer.Color}");
        }

        if (scanlineLayer != null && !isScanlineBroken)
        {
            AssertThat(ColorApproximatelyEqual(scanlineLayer.Color, OmegaSpiralColors.ScanlineOverlay)).IsTrue()
                .OverrideFailureMessage($"{componentName}: ScanlineLayer has wrong color. Expected: {OmegaSpiralColors.ScanlineOverlay}, Got: {scanlineLayer.Color}");
        }

        if (glitchLayer != null && !isGlitchBroken)
        {
            AssertThat(ColorApproximatelyEqual(glitchLayer.Color, OmegaSpiralColors.GlitchDistortion)).IsTrue()
                .OverrideFailureMessage($"{componentName}: GlitchLayer has wrong color. Expected: {OmegaSpiralColors.GlitchDistortion}, Got: {glitchLayer.Color}");
        }
    }

    /// <summary>
    /// Checks if a color is fully opaque white (RGB=1,1,1 and Alpha=1.0).
    /// This indicates an uninitialized/broken state. Properly themed layers should have
    /// semi-transparent white overlays from OmegaSpiralColors (18%, 12%, 8% alpha respectively).
    /// </summary>
    private static bool IsBrokenWhite(Color color)
    {
        const float tolerance = 0.01f;
        // Broken = fully opaque white (RGB close to 1.0 AND Alpha close to 1.0)
        return Mathf.Abs(color.R - 1.0f) < tolerance &&
               Mathf.Abs(color.G - 1.0f) < tolerance &&
               Mathf.Abs(color.B - 1.0f) < tolerance &&
               Mathf.Abs(color.A - 1.0f) < tolerance;
    }

    /// <summary>
    /// Compares two colors with a tolerance for floating point precision issues.
    /// </summary>
    /// <param name="color1">First color to compare</param>
    /// <param name="color2">Second color to compare</param>
    /// <param name="tolerance">Tolerance for comparison (default 0.01)</param>
    /// <returns>True if colors are approximately equal</returns>
    private static bool ColorApproximatelyEqual(Color color1, Color color2, float tolerance = 0.01f)
    {
        return Mathf.Abs(color1.R - color2.R) < tolerance &&
               Mathf.Abs(color1.G - color2.G) < tolerance &&
               Mathf.Abs(color1.B - color2.B) < tolerance &&
               Mathf.Abs(color1.A - color2.A) < tolerance;
    }
}
