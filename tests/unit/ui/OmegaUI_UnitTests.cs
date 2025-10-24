// <copyright file="OmegaUI_UnitTests.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using GdUnit4;
using OmegaSpiral.Source.UI.Omega;
using static GdUnit4.Assertions;
using System.Threading.Tasks;

namespace OmegaSpiral.Tests.Unit.UI
{
    /// <summary>
    /// Unit tests for OmegaUI behavioral logic using GdUnit4 mocks.
    /// Validates API delegation, graceful failure, and dispose pattern.
    /// </summary>
    [TestSuite]
    public partial class OmegaUI_UnitTests
    {
        /// <summary>
        /// UT-API-01: API Delegation (AppendText)
        /// Ensures AppendTextAsync delegates to IOmegaTextRenderer.
        /// </summary>
        [TestCase]
        public async Task AppendTextAsync_DelegatesToTextRenderer()
        {
            var mockRenderer = GdUnit4.Mock.Of<IOmegaTextRenderer>();
            var omegaUI = new OmegaUI();
            omegaUI.SetTextRendererForTest(mockRenderer);
            await omegaUI.AppendTextAsync("Hello", 42f, 1f);
            GdUnit4.Mock.Verify(mockRenderer).AppendTextAsync("Hello", 42f, 1f);
        }

        /// <summary>
        /// UT-API-02: API Graceful Failure (AppendText)
        /// Ensures no exception is thrown and warning is logged if TextRenderer is null.
        /// </summary>
        [TestCase]
        public async Task AppendTextAsync_GracefulFailureWhenRendererNull()
        {
            var omegaUI = new OmegaUI();
            await omegaUI.AppendTextAsync("test");
            // No exception should be thrown, warning should be logged (cannot verify GD.PushWarning directly)
        }

        /// <summary>
        /// UT-API-03: API Delegation (ApplyVisualPreset)
        /// Ensures ApplyVisualPresetAsync delegates to IOmegaShaderController.
        /// </summary>
        [TestCase]
        public async Task ApplyVisualPresetAsync_DelegatesToShaderController()
        {
            var mockShader = GdUnit4.Mock.Of<IOmegaShaderController>();
            var omegaUI = new OmegaUI();
            omegaUI.SetShaderControllerForTest(mockShader);
            await omegaUI.ApplyVisualPresetAsync("CRT");
            GdUnit4.Mock.Verify(mockShader).ApplyVisualPresetAsync("CRT");
        }

        /// <summary>
        /// UT-API-04: API Graceful Failure (ApplyVisualPreset)
        /// Ensures no exception is thrown and warning is logged if ShaderController is null.
        /// </summary>
        [TestCase]
        public async Task ApplyVisualPresetAsync_GracefulFailureWhenControllerNull()
        {
            var omegaUI = new OmegaUI();
            await omegaUI.ApplyVisualPresetAsync("CRT");
            // No exception should be thrown, warning should be logged
        }

        /// <summary>
        /// UT-DISP-01: Dispose Pattern
        /// Ensures Dispose calls Dispose on both components and nulls references.
        /// </summary>
        [TestCase]
        public void Dispose_CallsDisposeOnComponentsAndNullsReferences()
        {
            var mockShader = GdUnit4.Mock.Of<IOmegaShaderController, System.IDisposable>();
            var mockRenderer = GdUnit4.Mock.Of<IOmegaTextRenderer, System.IDisposable>();
            var omegaUI = new OmegaUI();
            omegaUI.SetShaderControllerForTest(mockShader);
            omegaUI.SetTextRendererForTest(mockRenderer);
            omegaUI.Dispose();
            GdUnit4.Mock.Verify(mockShader).Dispose();
            GdUnit4.Mock.Verify(mockRenderer).Dispose();
            AssertThat(omegaUI.ShaderController).IsNull();
            AssertThat(omegaUI.TextRenderer).IsNull();
        }

        /// <summary>
        /// UT-DISP-02: Multiple Disposals
        /// Ensures Dispose logic is only executed once.
        /// </summary>
        [TestCase]
        public void Dispose_CalledTwice_OnlyExecutesOnce()
        {
            var mockShader = GdUnit4.Mock.Of<IOmegaShaderController, System.IDisposable>();
            var mockRenderer = GdUnit4.Mock.Of<IOmegaTextRenderer, System.IDisposable>();
            var omegaUI = new OmegaUI();
            omegaUI.SetShaderControllerForTest(mockShader);
            omegaUI.SetTextRendererForTest(mockRenderer);
            omegaUI.Dispose();
            omegaUI.Dispose(); // Should not call Dispose again
            GdUnit4.Mock.Verify(mockShader).Dispose();
            GdUnit4.Mock.Verify(mockRenderer).Dispose();
        }
    }
}
