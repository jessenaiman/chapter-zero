// <copyright file="OmegaUiViewportConfig.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Linq;

namespace OmegaSpiral.Source.Ui.Omega;

/// <summary>
/// Configuration for Omega UI viewport sizing and responsive margins.
/// Provides standardized viewport sizes and margin calculations for consistent UI layout.
/// </summary>
public static class OmegaUiViewportConfig
{
    // ==================== MARGIN PERCENTAGES ====================

    /// <summary>
    /// Minimum margin percentage for compact layouts.
    /// 5% of viewport dimension provides minimal breathing room.
    /// </summary>
    public const float MinimumMarginPercent = 0.05f;

    /// <summary>
    /// Standard margin percentage for balanced layouts.
    /// 8% of viewport dimension provides comfortable spacing.
    /// </summary>
    public const float StandardMarginPercent = 0.08f;

    /// <summary>
    /// Maximum margin percentage for spacious layouts.
    /// 12% of viewport dimension provides generous margins.
    /// </summary>
    public const float MaximumMarginPercent = 0.12f;

    // ==================== VIEWPORT SIZE DEFINITION ====================

    /// <summary>
    /// Represents a supported viewport size with dimensions and aspect ratio.
    /// </summary>
    public readonly struct ViewportSize
    {
        /// <summary>
        /// Width of the viewport in pixels.
        /// </summary>
        public int Width { get; }

        /// <summary>
        /// Height of the viewport in pixels.
        /// </summary>
        public int Height { get; }

        /// <summary>
        /// Aspect ratio as a string (e.g., "16:9", "4:3").
        /// </summary>
        public string AspectRatio { get; }

        /// <summary>
        /// Creates a new viewport size.
        /// </summary>
        /// <param name="width">Width in pixels.</param>
        /// <param name="height">Height in pixels.</param>
        /// <param name="aspectRatio">Aspect ratio string.</param>
        public ViewportSize(int width, int height, string aspectRatio)
        {
            Width = width;
            Height = height;
            AspectRatio = aspectRatio;
        }
    }

    // ==================== SUPPORTED VIEWPORTS ====================

    /// <summary>
    /// Collection of supported viewport sizes for responsive UI testing.
    /// Includes common gaming resolutions and aspect ratios.
    /// </summary>
    public static IEnumerable<ViewportSize> SupportedViewports => new[]
    {
        // 16:9 aspect ratio - most common
        new ViewportSize(1920, 1080, "16:9"),  // Full HD
        new ViewportSize(1280, 720, "16:9"),   // HD
        new ViewportSize(2560, 1440, "16:9"),  // QHD

        // 16:10 aspect ratio - common for monitors
        new ViewportSize(1920, 1200, "16:10"), // WUXGA
        new ViewportSize(1280, 800, "16:10"),  // WXGA

        // 4:3 aspect ratio - classic gaming
        new ViewportSize(1024, 768, "4:3"),    // XGA
        new ViewportSize(800, 600, "4:3"),     // SVGA

        // 21:9 aspect ratio - ultrawide
        new ViewportSize(3440, 1440, "21:9"),  // Ultrawide QHD

        // Mobile/tablet aspect ratios
        new ViewportSize(1080, 1920, "9:16"),  // Mobile portrait
        new ViewportSize(2160, 1080, "2:1"),   // Mobile landscape
    };

    // ==================== MARGIN CALCULATION ====================

    /// <summary>
    /// Calculates the minimum horizontal margin for a given viewport width.
    /// Uses the minimum margin percentage to ensure content doesn't touch edges.
    /// </summary>
    /// <param name="viewportWidth">Width of the viewport in pixels.</param>
    /// <returns>Minimum horizontal margin in pixels.</returns>
    public static int CalculateHorizontalMargin(int viewportWidth)
    {
        return (int)(viewportWidth * MinimumMarginPercent);
    }

    /// <summary>
    /// Calculates the minimum vertical margin for a given viewport height.
    /// Uses the minimum margin percentage to ensure content doesn't touch edges.
    /// </summary>
    /// <param name="viewportHeight">Height of the viewport in pixels.</param>
    /// <returns>Minimum vertical margin in pixels.</returns>
    public static int CalculateVerticalMargin(int viewportHeight)
    {
        return (int)(viewportHeight * MinimumMarginPercent);
    }
}
