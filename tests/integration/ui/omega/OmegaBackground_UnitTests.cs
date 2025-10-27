using GdUnit4;
using Godot;
using OmegaSpiral.Source.Ui.Omega;
using static GdUnit4.Assertions;

namespace OmegaSpiral.Tests.Integration.Ui.Omega
{
    /// <summary>
    /// Integration tests for OmegaBackground component.
    /// Tests the background color configuration from design system.
    ///
    /// RESPONSIBILITY: Verify OmegaBackground configures ColorRect with design system colors.
    /// Color application, design system integration, and fallback behavior all tested here.
    ///
    /// Keeps these tests OUT of OmegaUiTests.cs and MainMenuTests.cs to avoid cluttering
    /// those files with background-specific concerns.
    /// </summary>
    [TestSuite]
    [RequireGodotRuntime]
    public partial class OmegaBackground_UnitTests
    {
        private ColorRect? _BackgroundNode;

        /// <summary>
        /// Sets up test fixtures before each test.
        /// Creates a mock Background ColorRect.
        /// </summary>
        [Before]
        public void Setup()
        {
            _BackgroundNode = AutoFree(new ColorRect())!;
        }

        /// <summary>
        /// Cleans up test fixtures after each test.
        /// </summary>
        [After]
        public void Cleanup()
        {
            // AutoFree handles cleanup automatically
        }

        // ==================== CONFIGURATION ====================

        /// <summary>
        /// ConfigureBackground() sets color from design system.
        /// Background color matches OmegaSpiralColors.DeepSpace.
        /// </summary>
        [TestCase]
        public void ConfigureBackground_AppliesDesignSystemColor()
        {
            OmegaBackground.ConfigureBackground(_BackgroundNode!);

            AssertThat(_BackgroundNode!.Color).IsEqual(OmegaSpiralColors.DeepSpace)
                .OverrideFailureMessage("Background color must match design system (DeepSpace)");
        }

        /// <summary>
        /// ConfigureBackground() throws on null node.
        /// Safety check to prevent null reference errors.
        /// </summary>
        [TestCase]
        public void ConfigureBackground_ThrowsOnNull()
        {
            AssertThrown(() => OmegaBackground.ConfigureBackground(null!))
                .OverrideFailureMessage("ConfigureBackground must throw ArgumentNullException for null node");
        }

        /// <summary>
        /// GetBackgroundColor() returns current color or default.
        /// </summary>
        [TestCase]
        public void GetBackgroundColor_ReturnsCurrent()
        {
            _BackgroundNode!.Color = Colors.Red;

            var color = OmegaBackground.GetBackgroundColor(_BackgroundNode);
            AssertThat(color).IsEqual(Colors.Red);
        }

        /// <summary>
        /// GetBackgroundColor() returns design system default for null node.
        /// </summary>
        [TestCase]
        public void GetBackgroundColor_ReturnsDefaultForNull()
        {
            var color = OmegaBackground.GetBackgroundColor(null);
            AssertThat(color).IsEqual(OmegaSpiralColors.DeepSpace);
        }

        // ==================== COLOR PROPERTIES ====================

        /// <summary>
        /// DeepSpace color has correct RGB values.
        /// Verifies the color loaded from JSON config is correct.
        /// </summary>
        [TestCase]
        public void DeepSpace_HasCorrectRGBValues()
        {
            var deepSpace = OmegaSpiralColors.DeepSpace;

            // Deep space is a dark color (0.054902, 0.0666667, 0.0862745)
            AssertThat(deepSpace.R).IsGreater(0.0f).IsLess(0.1f)
                .OverrideFailureMessage("DeepSpace R channel should be very low (dark)");
            AssertThat(deepSpace.G).IsGreater(0.0f).IsLess(0.1f)
                .OverrideFailureMessage("DeepSpace G channel should be very low (dark)");
            AssertThat(deepSpace.B).IsGreater(0.0f).IsLess(0.15f)
                .OverrideFailureMessage("DeepSpace B channel should be very low (dark)");
            AssertThat(deepSpace.A).IsEqual(1.0f)
                .OverrideFailureMessage("DeepSpace alpha should be fully opaque");
        }

        // ==================== RESET BEHAVIOR ====================

        /// <summary>
        /// ResetToDesignSystemColor() reverts custom colors to design system.
        /// </summary>
        [TestCase]
        public void ResetToDesignSystemColor_Reverts()
        {
            // Set custom color
            _BackgroundNode!.Color = Colors.Red;
            AssertThat(_BackgroundNode.Color).IsEqual(Colors.Red);

            // Reset to design system
            OmegaBackground.ResetToDesignSystemColor(_BackgroundNode);
            AssertThat(_BackgroundNode.Color).IsEqual(OmegaSpiralColors.DeepSpace);
        }

        /// <summary>
        /// ResetToDesignSystemColor() handles null node gracefully.
        /// No exception should be thrown.
        /// </summary>
        [TestCase]
        public void ResetToDesignSystemColor_HandlesNull()
        {
            // Should not throw
            OmegaBackground.ResetToDesignSystemColor(null);
            AssertThat(true).IsTrue(); // If we got here, no exception was thrown
        }


    }
}
