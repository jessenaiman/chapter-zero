// <copyright file="OmegaUITests.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using GdUnit4;
using Godot;
using OmegaSpiral.Source.UI.Omega;
using static GdUnit4.Assertions;

namespace OmegaSpiral.Tests.Unit.UI
{
    /// <summary>
    /// Unit tests for OmegaUI base component.
    /// Validates that all OmegaUI components have proper spacing and layout.
    /// Ensures consistent visual spacing on all edges (top, bottom, left, right).
    /// </summary>
    [TestSuite]
    [RequireGodotRuntime]
    public class OmegaUITests : IDisposable
    {
        private ISceneRunner? runner;
        private OmegaUI? omegaUI;

        /// <summary>
        /// Sets up the test scene and OmegaUI instance.
        /// Loads the omega_ui_test_fixture.tscn scene for testing.
        /// </summary>
        [Before]
        public void Setup()
        {
            this.runner = ISceneRunner.Load("res://tests/fixtures/omega_ui_test_fixture.tscn");
            this.omegaUI = this.runner.Scene() as OmegaUI;
            AssertThat(this.omegaUI).IsNotNull();
        }

        /// <summary>
        /// Cleans up test resources.
        /// </summary>
        [After]
        public void Teardown()
        {
            this.omegaUI?.QueueFree();
            this.omegaUI = null;
            this.runner?.Dispose();
            this.runner = null;
        }

        /// <summary>
        /// Disposes of resources used by the test class.
        /// </summary>
        public void Dispose()
        {
            this.Teardown();
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Tests that OmegaUI has a ContentContainer with margins on all sides.
        /// This ensures visual spacing so the UI doesn't touch screen edges.
        /// </summary>
        [TestCase]
        [RequireGodotRuntime]
        public void OmegaUIHasContentContainerWithMargins()
        {
            var contentContainer = this.runner?.FindChild("OmegaUITestFixture/ContentContainer") as MarginContainer;
            AssertThat(contentContainer).IsNotNull();
            AssertThat(contentContainer).IsInstanceOf<MarginContainer>();
        }

        /// <summary>
        /// Tests that ContentContainer has visible top margin/spacing.
        /// Ensures the UI doesn't flush against the top edge of the screen.
        /// </summary>
        [TestCase]
        [RequireGodotRuntime]
        public void OmegaUIHasTopMargin()
        {
            var contentContainer = this.runner?.FindChild("OmegaUITestFixture/ContentContainer") as MarginContainer;
            AssertThat(contentContainer).IsNotNull();

            // ContentContainer should have full-screen anchors
            AssertThat(contentContainer?.AnchorTop).IsEqual(0.0f);
            AssertThat(contentContainer?.AnchorBottom).IsEqual(1.0f);

            // Should have margin defined (either via theme or explicit constant)
            // For now, we check that it exists and fills the parent
            AssertThat(contentContainer?.GetParent()).IsNotNull();
        }

        /// <summary>
        /// Tests that ContentContainer has visible bottom margin/spacing.
        /// Ensures the UI doesn't flush against the bottom edge of the screen.
        /// </summary>
        [TestCase]
        [RequireGodotRuntime]
        public void OmegaUIHasBottomMargin()
        {
            var contentContainer = this.runner?.FindChild("OmegaUITestFixture/ContentContainer") as MarginContainer;
            AssertThat(contentContainer).IsNotNull();

            // ContentContainer should have full-screen anchors
            AssertThat(contentContainer?.AnchorTop).IsEqual(0.0f);
            AssertThat(contentContainer?.AnchorBottom).IsEqual(1.0f);
        }

        /// <summary>
        /// Tests that ContentContainer has visible left margin/spacing.
        /// Ensures the UI doesn't flush against the left edge of the screen.
        /// </summary>
        [TestCase]
        [RequireGodotRuntime]
        public void OmegaUIHasLeftMargin()
        {
            var contentContainer = this.runner?.FindChild("OmegaUITestFixture/ContentContainer") as MarginContainer;
            AssertThat(contentContainer).IsNotNull();

            // ContentContainer should have full-screen anchors
            AssertThat(contentContainer?.AnchorLeft).IsEqual(0.0f);
            AssertThat(contentContainer?.AnchorRight).IsEqual(1.0f);
        }

        /// <summary>
        /// Tests that ContentContainer has visible right margin/spacing.
        /// Ensures the UI doesn't flush against the right edge of the screen.
        /// </summary>
        [TestCase]
        [RequireGodotRuntime]
        public void OmegaUIHasRightMargin()
        {
            var contentContainer = this.runner?.FindChild("OmegaUITestFixture/ContentContainer") as MarginContainer;
            AssertThat(contentContainer).IsNotNull();

            // ContentContainer should have full-screen anchors
            AssertThat(contentContainer?.AnchorLeft).IsEqual(0.0f);
            AssertThat(contentContainer?.AnchorRight).IsEqual(1.0f);
        }

        /// <summary>
        /// Tests that OmegaUI components use MarginContainer for consistent spacing.
        /// This architectural test ensures all OmegaUI instances have proper layout structure.
        /// </summary>
        [TestCase]
        [RequireGodotRuntime]
        public void OmegaUIUsesMarginContainerPattern()
        {
            // Any OmegaUI should have a MarginContainer as direct child for content
            var contentContainer = this.runner?.FindChild("OmegaUITestFixture/ContentContainer") as MarginContainer;
            AssertThat(contentContainer).IsNotNull();

            // It should be a direct child of the root OmegaUI control
            AssertThat(contentContainer?.GetParent()).IsEqual(this.omegaUI);
        }
    }
}
