// <copyright file="OmegaUi_IntegrationTests.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using GdUnit4;
using GdUnit4.Api;
using Godot;
using OmegaSpiral.Source.Ui.Omega;
using static GdUnit4.Assertions;
using System.Threading.Tasks;

namespace OmegaSpiral.Tests.Integration.Ui
{
    /// <summary>
    /// Integration tests for OmegaUi using scene runner.
    ///
    /// Following GdUnit4 best practices (GdUnit4Net-README.mdx, scene-runner.md):
    /// - Scene loaded ONCE per test in [BeforeTest]
    /// - All tests use same scene instance
    /// - Setup failure → all tests fail (expected behavior)
    /// - Tests focus ONLY on behavior, not initialization
    /// - Parameterized tests reduce code duplication
    /// </summary>
    [TestSuite]
    [RequireGodotRuntime]
    public partial class OmegaUi_IntegrationTests : Node
    {
        private ISceneRunner? _Runner;
        private OmegaUi? _OmegaUi;

        /// <summary>
        /// SETUP: Load scene once and verify components initialized.
        /// Following GdUnit4 lifecycle (gdunit4-tools.instructions.md):
        /// - [BeforeTest] runs before each test
        /// - ISceneRunner.Load() is synchronous - _Ready() called before return
        /// - Setup validates scene structure - if fails, suite fails (expected)
        /// </summary>
        [BeforeTest]
        public void Setup()
        {
            _Runner = ISceneRunner.Load("res://source/ui/omega/omega_ui.tscn");
            _OmegaUi = _Runner.Scene() as OmegaUi;

            AssertThat(_OmegaUi).IsNotNull();
            AssertThat(_OmegaUi!.TextRenderer).IsNotNull();
            AssertThat(_OmegaUi.ShaderController).IsNotNull();
        }

        [AfterTest]
        public void Teardown()
        {
            _Runner?.Dispose();
        }

        [TestCase]
        public void Initialization_ComponentsCreated()
        {
            // Verify components exist (validated in Setup)
            AssertThat(_OmegaUi!.TextRenderer).IsNotNull();
            AssertThat(_OmegaUi.ShaderController).IsNotNull();
        }

        [TestCase]
        public async Task AppendTextAsync_AppendsTextToRenderer()
        {
            await _OmegaUi!.AppendTextAsync("Test message", 50f, 0f);
            AssertThat(_OmegaUi.TextRenderer?.GetCurrentText()).Contains("Test message");
        }

        [TestCase("phosphor", TestName = "phosphor_preset")]
        [TestCase("scanlines", TestName = "scanlines_preset")]
        [TestCase("glitch", TestName = "glitch_preset")]
        public async Task ApplyVisualPresetAsync_AppliesPreset(string presetName)
        {
            await _OmegaUi!.ApplyVisualPresetAsync(presetName);
            AssertThat(_OmegaUi.ShaderController?.GetCurrentShaderMaterial()).IsNotNull();
        }

        [TestCase]
        public void Dispose_NullsReferences()
        {
            AssertThat(_OmegaUi!.TextRenderer).IsNotNull();
            AssertThat(_OmegaUi.ShaderController).IsNotNull();

            _OmegaUi.Dispose();

            AssertThat(_OmegaUi.TextRenderer).IsNull();
            AssertThat(_OmegaUi.ShaderController).IsNull();
        }

        [TestCase]
        public void Dispose_MultipleCalls_NoError()
        {
            _OmegaUi!.Dispose();
            _OmegaUi.Dispose();  // Should not throw

            // Should remain null
            AssertThat(_OmegaUi.TextRenderer).IsNull();
            AssertThat(_OmegaUi.ShaderController).IsNull();
        }
    }
}
