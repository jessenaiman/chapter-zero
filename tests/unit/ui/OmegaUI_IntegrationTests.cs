// <copyright file="OmegaUI_IntegrationTests.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using GdUnit4;
using Godot;
using OmegaSpiral.Source.UI.Omega;
using static GdUnit4.Assertions;

namespace OmegaSpiral.Tests.Unit.UI
{
    /// <summary>
    /// Integration tests for OmegaUI initialization logic.
    /// Validates component creation and handling of missing node paths.
    /// </summary>
    [TestSuite]
    [RequireGodotRuntime]
    public partial class OmegaUI_IntegrationTests : Node
    {
        private ISceneRunner? runner;
        private OmegaUI? omegaUI;

        [Before]
        public void Setup()
        {
            runner = ISceneRunner.Load("res://tests/fixtures/omega_ui_test_fixture.tscn");
            omegaUI = runner?.Scene() as OmegaUI;
            AssertThat(omegaUI).IsNotNull();
        }

        [After]
        public void Teardown()
        {
            runner?.Dispose();
            runner = null;
            omegaUI = null;
        }

        /// <summary>
        /// UT-INIT-01: Component Creation (Full)
        /// Asserts that both ShaderController and TextRenderer are created when valid node paths exist.
        /// </summary>
        [TestCase]
        public void Initialization_WithValidPaths_CreatesComponents()
        {
            AssertThat(omegaUI!.ShaderController).IsNotNull();
            AssertThat(omegaUI!.TextRenderer).IsNotNull();
        }

        /// <summary>
        /// UT-INIT-02: Component Creation (Partial)
        /// Asserts that only TextRenderer is created when only text display node exists.
        /// </summary>
        [TestCase]
        public void Initialization_WithMissingShaderPath_DoesNotCreateShaderController()
        {
            using var runnerPartial = ISceneRunner.Load("res://tests/fixtures/omega_ui_missing_shader.tscn");
            var ui = runnerPartial.Scene() as OmegaUI;
            AssertThat(ui).IsNotNull();
            AssertThat(ui!.TextRenderer).IsNotNull();
            AssertThat(ui!.ShaderController).IsNull();
        }

        // Factory failure test omitted: requires custom subclass or fixture to override CreateShaderController.
    }
}
