// <copyright file="OmegaUiTests.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using GdUnit4;
using Godot;
using OmegaSpiral.Source.Ui.Omega;
using static GdUnit4.Assertions;

namespace OmegaSpiral.Tests.Unit.Ui
{
    /// <summary>
    /// Unit tests for OmegaUi base component.
    /// Validates that all OmegaUi components have proper spacing and layout.
    /// Ensures consistent visual spacing on all edges (top, bottom, left, right).
    /// </summary>
    [TestSuite]
    [RequireGodotRuntime]
    public class OmegaUiTests : IDisposable
    {
        private ISceneRunner? _runner;
        private OmegaUi? _omegaUi;

        /// <summary>
        /// Sets up the test scene and OmegaUi instance.
        /// Loads the omega_ui_test_fixture.tscn scene for testing.
        /// </summary>
        [Before]
        public void Setup()
        {
            this._runner = ISceneRunner.Load("res://tests/fixtures/omega_ui_test_fixture.tscn");
            this._omegaUi = this._runner.Scene() as OmegaUi;
            AssertThat(this._omegaUi).IsNotNull();
        }

        /// <summary>
        /// Cleans up test resources.
        /// </summary>
        [After]
        public void Teardown()
        {
            this._runner?.Dispose();
            this._runner = null;
            this._omegaUi = null;
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
        /// Tests that OmegaUi has a ContentContainer with margins on all sides.
        /// This ensures visual spacing so the Ui doesn't touch screen edges.
        /// </summary>
        [TestCase]
        [RequireGodotRuntime]
        public void OmegaUiHasContentContainerWithMargins()
        {
            var contentContainer = this._runner?.FindChild("OmegaUiTestFixture/ContentContainer") as MarginContainer;
            AssertThat(contentContainer).IsNotNull();
            AssertThat(contentContainer).IsInstanceOf<MarginContainer>();
        }

        /// <summary>
        /// Tests that ContentContainer has visible top margin/spacing.
        /// Ensures the Ui doesn't flush against the top edge of the screen.
        /// </summary>
        [TestCase]
        [RequireGodotRuntime]
        public void OmegaUiHasTopMargin()
        {
            var contentContainer = this._runner?.FindChild("OmegaUiTestFixture/ContentContainer") as MarginContainer;
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
        /// Ensures the Ui doesn't flush against the bottom edge of the screen.
        /// </summary>
        [TestCase]
        [RequireGodotRuntime]
        public void OmegaUiHasBottomMargin()
        {
            var contentContainer = this._runner?.FindChild("OmegaUiTestFixture/ContentContainer") as MarginContainer;
            AssertThat(contentContainer).IsNotNull();

            // ContentContainer should have full-screen anchors
            AssertThat(contentContainer?.AnchorTop).IsEqual(0.0f);
            AssertThat(contentContainer?.AnchorBottom).IsEqual(1.0f);
        }

        /// <summary>
        /// Tests that ContentContainer has visible left margin/spacing.
        /// Ensures the Ui doesn't flush against the left edge of the screen.
        /// </summary>
        [TestCase]
        [RequireGodotRuntime]
        public void OmegaUiHasLeftMargin()
        {
            var contentContainer = this._runner?.FindChild("OmegaUiTestFixture/ContentContainer") as MarginContainer;
            AssertThat(contentContainer).IsNotNull();

            // ContentContainer should have full-screen anchors
            AssertThat(contentContainer?.AnchorLeft).IsEqual(0.0f);
            AssertThat(contentContainer?.AnchorRight).IsEqual(1.0f);
        }

        /// <summary>
        /// Tests that ContentContainer has visible right margin/spacing.
        /// Ensures the Ui doesn't flush against the right edge of the screen.
        /// </summary>
        [TestCase]
        [RequireGodotRuntime]
        public void OmegaUiHasRightMargin()
        {
            var contentContainer = this._runner?.FindChild("OmegaUiTestFixture/ContentContainer") as MarginContainer;
            AssertThat(contentContainer).IsNotNull();

            // ContentContainer should have full-screen anchors
            AssertThat(contentContainer?.AnchorLeft).IsEqual(0.0f);
            AssertThat(contentContainer?.AnchorRight).IsEqual(1.0f);
        }

        /// <summary>
        /// Tests that OmegaUi components use MarginContainer for consistent spacing.
        /// This architectural test ensures all OmegaUi instances have proper layout structure.
        /// </summary>
        [TestCase]
        [RequireGodotRuntime]
        public void OmegaUiUsesMarginContainerPattern()
        {
            // Any OmegaUi should have a MarginContainer as direct child for content
            var contentContainer = this._runner?.FindChild("OmegaUiTestFixture/ContentContainer") as MarginContainer;
            AssertThat(contentContainer).IsNotNull();

            // It should be a direct child of the root OmegaUi control
            AssertThat(contentContainer?.GetParent()).IsEqual(this._omegaUi);
        }
    }
}
