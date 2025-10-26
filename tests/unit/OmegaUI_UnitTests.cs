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
    /// Validates initialization, component creation, API behavior, and dispose pattern.
    /// </summary>
    [TestSuite]
    [RequireGodotRuntime]
    public partial class OmegaUi_IntegrationTests : Node
    {
        private ISceneRunner? _Runner;

        [After]
        public void Teardown()
        {
            _Runner?.Dispose();
        }

        /// <summary>
        /// INT-INIT-01: Scene Initialization
        /// Ensures OmegaUi scene loads and initializes successfully.
        /// </summary>
        [TestCase]
        public async Task SceneInitialization_EmitsInitializationCompletedSignal()
        {
            _Runner = ISceneRunner.Load("res://source/ui/omega/omega_ui.tscn");
            var omegaUi = _Runner.Scene() as OmegaUi;
            AssertThat(omegaUi).IsNotNull();

            // Allow _Ready to execute and async initialization to complete
            await _Runner.SimulateFrames(2);

            // Verify initialization completed successfully
            AssertThat(omegaUi!.InitializationState).IsEqual(OmegaUiInitializationState.Initialized);
            AssertThat(omegaUi.InitializationError).IsNull();

            // Verify components were created
            AssertThat(omegaUi.TextRenderer).IsNotNull();
            AssertThat(omegaUi.ShaderController).IsNotNull();
        }

        /// <summary>
        /// INT-INIT-02: Component Creation
        /// Ensures TextRenderer and ShaderController are created during initialization.
        /// </summary>
        [TestCase]
        public async Task ComponentCreation_CreatesTextRendererAndShaderController()
        {
            _Runner = ISceneRunner.Load("res://source/ui/omega/omega_ui.tscn");
            var omegaUi = _Runner.Scene() as OmegaUi;
            AssertThat(omegaUi).IsNotNull();

            await _Runner.SimulateFrames(1); // Allow _Ready to execute

            AssertThat(omegaUi!.TextRenderer).IsNotNull();
            AssertThat(omegaUi.ShaderController).IsNotNull();
        }

        /// <summary>
        /// INT-API-01: AppendText Behavior
        /// Ensures AppendTextAsync works when TextRenderer is initialized.
        /// </summary>
        [TestCase]
        public async Task AppendTextAsync_WorksWithInitializedRenderer()
        {
            _Runner = ISceneRunner.Load("res://source/ui/omega/omega_ui.tscn");
            var omegaUi = _Runner.Scene() as OmegaUi;
            AssertThat(omegaUi).IsNotNull();

            await _Runner.SimulateFrames(1);

            // Should not throw - TextRenderer is initialized from scene
            await omegaUi!.AppendTextAsync("Test message", 50f, 0f);

            // Verify text was appended (check via TextRenderer)
            AssertThat(omegaUi.TextRenderer?.GetCurrentText()).Contains("Test message");
        }

        /// <summary>
        /// INT-API-02: AppendText Graceful Failure
        /// Ensures no exception when TextRenderer is null (scene without TextDisplay node).
        /// </summary>
        [TestCase]
        public async Task AppendTextAsync_GracefulFailureWhenRendererNull()
        {
            // Create OmegaUi programmatically without scene nodes
            var omegaUi = AutoFree(new OmegaUi())!;
            AddChild(omegaUi);
            await ToSignal(omegaUi, Node.SignalName.Ready);

            // Should not throw - gracefully handles null renderer
            await omegaUi.AppendTextAsync("Test");

            // Verify renderer is null (no TextDisplay node exists)
            AssertThat(omegaUi.TextRenderer).IsNull();
        }

        /// <summary>
        /// INT-API-03: ApplyVisualPreset Behavior
        /// Ensures ApplyVisualPresetAsync works when ShaderController is initialized.
        /// </summary>
        [TestCase]
        public async Task ApplyVisualPresetAsync_WorksWithInitializedController()
        {
            _Runner = ISceneRunner.Load("res://source/ui/omega/omega_ui.tscn");
            var omegaUi = _Runner.Scene() as OmegaUi;
            AssertThat(omegaUi).IsNotNull();

            await _Runner.SimulateFrames(1);

            // Should not throw - ShaderController is initialized from scene
            await omegaUi!.ApplyVisualPresetAsync("phosphor");

            // Verify shader controller has material
            AssertThat(omegaUi.ShaderController?.GetCurrentShaderMaterial()).IsNotNull();
        }

        /// <summary>
        /// INT-API-04: ApplyVisualPreset Graceful Failure
        /// Ensures no exception when ShaderController is null.
        /// </summary>
        [TestCase]
        public async Task ApplyVisualPresetAsync_GracefulFailureWhenControllerNull()
        {
            // Create OmegaUi programmatically without shader layers
            var omegaUi = AutoFree(new OmegaUi())!;
            AddChild(omegaUi);
            await ToSignal(omegaUi, Node.SignalName.Ready);

            // Should not throw - gracefully handles null controller
            await omegaUi.ApplyVisualPresetAsync("phosphor");

            // Verify controller is null (no shader layer nodes exist)
            AssertThat(omegaUi.ShaderController).IsNull();
        }

        /// <summary>
        /// INT-DISP-01: Dispose Pattern
        /// Ensures Dispose nulls component references.
        /// </summary>
        [TestCase]
        public async Task Dispose_NullsComponentReferences()
        {
            _Runner = ISceneRunner.Load("res://source/ui/omega/omega_ui.tscn");
            var omegaUi = _Runner.Scene() as OmegaUi;
            AssertThat(omegaUi).IsNotNull();

            await _Runner.SimulateFrames(1);

            // Verify components exist before disposal
            AssertThat(omegaUi!.TextRenderer).IsNotNull();
            AssertThat(omegaUi.ShaderController).IsNotNull();

            omegaUi.Dispose();

            // Verify components are nulled after disposal
            AssertThat(omegaUi.TextRenderer).IsNull();
            AssertThat(omegaUi.ShaderController).IsNull();
        }

        /// <summary>
        /// INT-DISP-02: Multiple Disposals
        /// Ensures Dispose can be called multiple times without error.
        /// </summary>
        [TestCase]
        public async Task Dispose_CalledTwice_NoError()
        {
            _Runner = ISceneRunner.Load("res://source/ui/omega/omega_ui.tscn");
            var omegaUi = _Runner.Scene() as OmegaUi;
            AssertThat(omegaUi).IsNotNull();

            await _Runner.SimulateFrames(1);

            omegaUi!.Dispose();
            omegaUi.Dispose(); // Should not throw or cause issues

            AssertThat(omegaUi.TextRenderer).IsNull();
            AssertThat(omegaUi.ShaderController).IsNull();
        }
    }
}
