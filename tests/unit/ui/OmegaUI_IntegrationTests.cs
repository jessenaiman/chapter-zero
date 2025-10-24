// <copyright file="OmegaUi_IntegrationTests.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using GdUnit4;
using Godot;
using OmegaSpiral.Source.Ui.Omega;
using static GdUnit4.Assertions;

namespace OmegaSpiral.Tests.Unit.Ui
{
    /// <summary>
    /// Integration tests for OmegaUi initialization logic.
    /// Validates component creation and handling of missing node paths.
    /// </summary>
    [TestSuite]
    [RequireGodotRuntime]
    public partial class OmegaUi_IntegrationTests : Node
    {
        private ISceneRunner? runner;
        private OmegaUi? omegaUi;

        [Before]
        public void Setup()
        {
            runner = ISceneRunner.Load("res://tests/fixtures/omega_ui_test_fixture.tscn");
            omegaUi = runner?.Scene() as OmegaUi;
            AssertThat(omegaUi).IsNotNull();
        }

        [After]
        public void Teardown()
        {
            runner?.Dispose();
            runner = null;
            omegaUi = null;
        }

        /// <summary>
        /// UT-INIT-01: Component Creation (Full)
        /// Asserts that both ShaderController and TextRenderer are created when valid node paths exist.
        /// </summary>
        [TestCase]
        public void Initialization_WithValidPaths_CreatesComponents()
        {
            AssertThat(omegaUi!.ShaderController).IsNotNull();
            AssertThat(omegaUi!.TextRenderer).IsNotNull();
        }

        /// <summary>
        /// UT-INIT-02: Component Creation (Partial)
        /// Asserts that only TextRenderer is created when only text display node exists.
        /// </summary>
        [TestCase]
        public void Initialization_WithMissingShaderPath_DoesNotCreateShaderController()
        {
            using var runnerPartial = ISceneRunner.Load("res://tests/fixtures/omega_ui_missing_shader.tscn");
            var ui = runnerPartial.Scene() as OmegaUi;
            AssertThat(ui).IsNotNull();
            AssertThat(ui!.TextRenderer).IsNotNull();
            AssertThat(ui!.ShaderController).IsNull();
        }

        // Factory failure test omitted: requires custom subclass or fixture to override CreateShaderController.
    }
}
