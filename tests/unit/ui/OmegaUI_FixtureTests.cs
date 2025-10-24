// <copyright file="OmegaUI_FixtureTests.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using GdUnit4;
using Godot;
using OmegaSpiral.Source.UI.Omega;
using static GdUnit4.Assertions;

namespace OmegaSpiral.Tests.Unit.UI
{
    /// <summary>
    /// Validates the scene structure and integrity of the omega_ui_test_fixture.tscn prefab.
    /// This ensures that all required nodes exist and are configured with the correct layout properties.
    /// </summary>
    [TestSuite]
    [RequireGodotRuntime]
    public partial class OmegaUI_FixtureTests : Node
    {
        // Scene nodes to be validated, populated once in Setup.
        private ISceneRunner? _runner;
        private OmegaUI? _omegaUI;
        private MarginContainer? _contentContainer;
        private ColorRect? _phosphorLayer;
        private RichTextLabel? _textDisplay;

        /// <summary>
        /// Sets up the test fixture by loading the scene and validating all required nodes exist.
        /// All assertions here serve as gating conditions for the actual tests.
        /// If setup fails, all tests are skipped, which is the correct behavior.
        /// </summary>
        [Before]
        public void Setup()
        {
            // Arrange: Load the scene fixture
            _runner = ISceneRunner.Load("res://tests/fixtures/omega_ui_test_fixture.tscn");
            _omegaUI = _runner?.Scene() as OmegaUI;

            // Assert: Ensure the root node exists.
            AssertThat(_omegaUI).IsNotNull();

            // Assert: Locate and validate all required child nodes. If any are missing, setup fails immediately.
            _contentContainer = _omegaUI!.GetNodeOrNull<MarginContainer>("ContentContainer");
            AssertThat(_contentContainer).IsNotNull();

            _phosphorLayer = _contentContainer!.GetNodeOrNull<ColorRect>("PhosphorLayer");
            AssertThat(_phosphorLayer).IsNotNull("The scene must have a 'PhosphorLayer' inside the container.");

            _textDisplay = _contentContainer.GetNodeOrNull<RichTextLabel>("TextDisplay");
            AssertThat(_textDisplay).IsNotNull("The scene must have a 'TextDisplay' inside the container.");
        }

        /// <summary>
        /// Cleans up resources after each test.
        /// GdUnit4's ISceneRunner handles proper disposal of scenes and all child nodes.
        /// </summary>
        [After]
        public void Teardown()
        {
            _runner?.Dispose();
        }

        /// <summary>
        /// Validates that the UI fixture has the correct node hierarchy, layout structure, and anchor properties.
        /// This single test validates the entire fixture cohesively, ensuring:
        /// - The ContentContainer is a direct child of the root OmegaUI
        /// - The container uses full-screen anchors (0, 0, 1, 1)
        /// - All required shader and text display nodes are properly typed
        /// </summary>
        [TestCase]
        public void Fixture_HasCorrectLayoutAndStructure()
        {
            // Assert 1: The container should be a direct child of the root.
            AssertThat(_contentContainer!.GetParent()).IsEqual(_omegaUI);

            // Assert 2: The container should be anchored to fill the entire screen (0, 0, 1, 1).
            AssertThat(_contentContainer.AnchorLeft).IsEqual(0.0f);
            AssertThat(_contentContainer.AnchorTop).IsEqual(0.0f);
            AssertThat(_contentContainer.AnchorRight).IsEqual(1.0f);
            AssertThat(_contentContainer.AnchorBottom).IsEqual(1.0f);

            // Assert 3: Verify that all shader and text display nodes are properly typed.
            AssertThat(_phosphorLayer).IsInstanceOf<ColorRect>();
            AssertThat(_textDisplay).IsInstanceOf<RichTextLabel>();
        }
    }
}
