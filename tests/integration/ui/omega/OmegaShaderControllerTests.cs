// <copyright file="OmegaShaderControllerTests.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using GdUnit4;
using Godot;
using static GdUnit4.Assertions;
using OmegaSpiral.Source.Ui.Omega;

namespace OmegaSpiral.Tests.Unit.Common.Omega
{
    /// <summary>
    /// Unit tests for OmegaShaderController component.
    /// Tests shader effect application, preset management, and visual transitions.
    /// </summary>
    [TestSuite]
    [RequireGodotRuntime]
    public class OmegaShaderControllerTests
    {
        private OmegaShaderController _Controller = null!;
        private ColorRect _MockDisplay = null!;

        /// <summary>
        /// Sets up test fixtures before each test.
        /// </summary>
        [Before]
        public void Setup()
        {
            _MockDisplay = AutoFree(new ColorRect())!;
            _Controller = new OmegaShaderController(_MockDisplay);
        }

        /// <summary>
        /// Cleans up test fixtures after each test.
        /// </summary>
        [After]
        public void Cleanup()
        {
            _Controller?.Dispose();
            // Cleanup is handled by AutoFree for _mockDisplay
        }

        /// <summary>
        /// Tests that constructor initializes with valid display node.
        /// </summary>
        [TestCase]
        public void Constructor_WithValidDisplay_InitializesCorrectly()
        {
            // Arrange & Act
            var controller = new OmegaShaderController(_MockDisplay);

            // Assert
            AssertThat(controller).IsNotNull();
            AssertThat(controller.GetCurrentShaderMaterial()).IsNull();
        }

        /// <summary>
        /// Tests that constructor throws with null display.
        /// </summary>
        [TestCase]
        public void Constructor_WithNullDisplay_ThrowsArgumentNullException()
        {
            // Act & Assert
            AssertThrown(() => new OmegaShaderController(null!))
                .IsInstanceOf<ArgumentNullException>();
        }

        /// <summary>
        /// Tests applying phosphor preset applies correct shader and parameters.
        /// </summary>
        [TestCase]
        public async Task ApplyVisualPresetAsync_PhosphorPreset_AppliesCorrectShader()
        {
            // Act
            await _Controller.ApplyVisualPresetAsync("phosphor").ConfigureAwait(true);

            // Assert
            var material = _Controller.GetCurrentShaderMaterial();
            AssertThat(material).IsNotNull();
            AssertThat(material!.Shader!.ResourcePath).Contains("crt_phosphor");
        }

        /// <summary>
        /// Tests applying scanlines preset applies correct shader and parameters.
        /// </summary>
        [TestCase]
        public async Task ApplyVisualPresetAsync_ScanlinesPreset_AppliesCorrectShader()
        {
            // Act
            await _Controller.ApplyVisualPresetAsync("scanlines").ConfigureAwait(true);

            // Assert
            var material = _Controller.GetCurrentShaderMaterial();
            AssertThat(material).IsNotNull();
            AssertThat(material!.Shader!.ResourcePath).Contains("crt_scanlines");
        }

        /// <summary>
        /// Tests applying glitch preset applies correct shader and parameters.
        /// </summary>
        [TestCase]
        public async Task ApplyVisualPresetAsync_GlitchPreset_AppliesCorrectShader()
        {
            // Act
            await _Controller.ApplyVisualPresetAsync("glitch").ConfigureAwait(true);

            // Assert
            var material = _Controller.GetCurrentShaderMaterial();
            AssertThat(material).IsNotNull();
            AssertThat(material!.Shader!.ResourcePath).Contains("crt_glitch");
        }

        /// <summary>
        /// Tests applying invalid preset throws exception.
        /// </summary>
        [TestCase]
        public void ApplyVisualPresetAsync_InvalidPreset_ThrowsArgumentException()
        {
            // Act & Assert
            AssertThrown(() => ApplyInvalidPresetSync())
                .IsInstanceOf<ArgumentException>();
        }

        private void ApplyInvalidPresetSync()
        {
            _Controller.ApplyVisualPresetAsync("invalid").Wait();
        }

        /// <summary>
        /// Tests pixel dissolve effect completes within expected time.
        /// </summary>
        [TestCase]
        public async Task PixelDissolveAsync_DefaultDuration_CompletesSuccessfully()
        {
            // Arrange
            var startTime = Time.GetTicksMsec();

            // Act
            await _Controller.PixelDissolveAsync().ConfigureAwait(true);

            // Assert
            var elapsed = Time.GetTicksMsec() - startTime;
            AssertThat(elapsed).IsGreater(900); // Should take at least 0.9 seconds
            AssertThat(elapsed).IsLess(1100); // Should take at most 1.1 seconds
        }

        /// <summary>
        /// Tests pixel dissolve with custom duration.
        /// </summary>
        [TestCase]
        public async Task PixelDissolveAsync_CustomDuration_CompletesInCorrectTime()
        {
            // Arrange
            var duration = 0.5f;
            var startTime = Time.GetTicksMsec();

            // Act
            await _Controller.PixelDissolveAsync(duration).ConfigureAwait(true);

            // Assert
            var elapsed = Time.GetTicksMsec() - startTime;
            AssertThat(elapsed).IsGreater(400); // Should take at least 0.4 seconds
            AssertThat(elapsed).IsLess(700); // Should take at most 0.7 seconds (allowing for timing variance)
        }

        /// <summary>
        /// Tests resetting shader effects removes material.
        /// </summary>
        [TestCase]
        public async Task ResetShaderEffects_AfterApplyingPreset_RemovesMaterial()
        {
            // Arrange
            await _Controller.ApplyVisualPresetAsync("phosphor").ConfigureAwait(true);
            AssertThat(_Controller.GetCurrentShaderMaterial()).IsNotNull();

            // Act
            _Controller.ResetShaderEffects();

            // Assert
            AssertThat(_Controller.GetCurrentShaderMaterial()).IsNull();
        }

        /// <summary>
        /// Tests getting current shader material when none applied returns null.
        /// </summary>
        [TestCase]
        public void GetCurrentShaderMaterial_NoShaderApplied_ReturnsNull()
        {
            // Act & Assert
            AssertThat(_Controller.GetCurrentShaderMaterial()).IsNull();
        }

        /// <summary>
        /// Tests applying terminal preset (clean display) removes shader.
        /// </summary>
        [TestCase]
        public async Task ApplyVisualPresetAsync_TerminalPreset_RemovesShader()
        {
            // Arrange
            await _Controller.ApplyVisualPresetAsync("phosphor").ConfigureAwait(true);
            AssertThat(_Controller.GetCurrentShaderMaterial()).IsNotNull();

            // Act
            await _Controller.ApplyVisualPresetAsync("terminal").ConfigureAwait(true);

            // Assert
            AssertThat(_Controller.GetCurrentShaderMaterial()).IsNull();
        }
    }
}
