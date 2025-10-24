// <copyright file="OmegaUi_UnitTests.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using GdUnit4;
using OmegaSpiral.Source.Ui.Omega;
using Moq;
using static GdUnit4.Assertions;
using System.Threading.Tasks;

namespace OmegaSpiral.Tests.Unit.Ui
{
    /// <summary>
    /// Unit tests for OmegaUi behavioral logic using GdUnit4 mocks.
    /// Validates API delegation, graceful failure, and dispose pattern.
    /// </summary>
    [TestSuite]
    public partial class OmegaUi_UnitTests
    {
        /// <summary>
        /// UT-API-01: API Delegation (AppendText)
        /// Ensures AppendTextAsync delegates to IOmegaTextRenderer.
        /// </summary>
        [TestCase]
        [RequireGodotRuntime]
        public async Task AppendTextAsync_DelegatesToTextRenderer()
        {
            var mockRenderer = new Mock<IOmegaTextRenderer>();
            var omegaUi = new OmegaUi();
            omegaUi.SetTextRendererForTest(mockRenderer.Object);
            await omegaUi.AppendTextAsync("Hello", 42f, 1f);
            mockRenderer.Verify(m => m.AppendTextAsync("Hello", 42f, 1f), Times.Once);
        }

        /// <summary>
        /// UT-API-02: API Graceful Failure (AppendText)
        /// Ensures no exception is thrown and warning is logged if TextRenderer is null.
        /// </summary>
        [TestCase]
        [RequireGodotRuntime]
        public async Task AppendTextAsync_GracefulFailureWhenRendererNull()
        {
            var omegaUi = new OmegaUi();
            await omegaUi.AppendTextAsync("test");
            // No exception should be thrown, warning should be logged (cannot verify GD.PushWarning directly)
        }

        /// <summary>
        /// UT-API-03: API Delegation (ApplyVisualPreset)
        /// Ensures ApplyVisualPresetAsync delegates to IOmegaShaderController.
        /// </summary>
        [TestCase]
        [RequireGodotRuntime]
        public async Task ApplyVisualPresetAsync_DelegatesToShaderController()
        {
            var mockShader = new Mock<IOmegaShaderController>();
            var omegaUi = new OmegaUi();
            omegaUi.SetShaderControllerForTest(mockShader.Object);
            await omegaUi.ApplyVisualPresetAsync("CRT");
            mockShader.Verify(m => m.ApplyVisualPresetAsync("CRT"), Times.Once);
        }

        /// <summary>
        /// UT-API-04: API Graceful Failure (ApplyVisualPreset)
        /// Ensures no exception is thrown and warning is logged if ShaderController is null.
        /// </summary>
        [TestCase]
        [RequireGodotRuntime]
        public async Task ApplyVisualPresetAsync_GracefulFailureWhenControllerNull()
        {
            var omegaUi = new OmegaUi();
            await omegaUi.ApplyVisualPresetAsync("CRT");
            // No exception should be thrown, warning should be logged
        }

        /// <summary>
        /// UT-DISP-01: Dispose Pattern
        /// Ensures Dispose calls Dispose on both components and nulls references.
        /// </summary>
        [TestCase]
        [RequireGodotRuntime]
        public void Dispose_CallsDisposeOnComponentsAndNullsReferences()
        {
            var mockShader = new Mock<IOmegaShaderController>();
            mockShader.As<System.IDisposable>();
            var mockRenderer = new Mock<IOmegaTextRenderer>();
            mockRenderer.As<System.IDisposable>();
            var omegaUi = new OmegaUi();
            omegaUi.SetShaderControllerForTest(mockShader.Object);
            omegaUi.SetTextRendererForTest(mockRenderer.Object);
            omegaUi.Dispose();
            mockShader.As<System.IDisposable>().Verify(m => m.Dispose(), Times.Once);
            mockRenderer.As<System.IDisposable>().Verify(m => m.Dispose(), Times.Once);
            AssertThat(omegaUi.ShaderController).IsNull();
            AssertThat(omegaUi.TextRenderer).IsNull();
        }

        /// <summary>
        /// UT-DISP-02: Multiple Disposals
        /// Ensures Dispose logic is only executed once.
        /// </summary>
        [TestCase]
        [RequireGodotRuntime]
        public void Dispose_CalledTwice_OnlyExecutesOnce()
        {
            var mockShader = new Mock<IOmegaShaderController>();
            mockShader.As<System.IDisposable>();
            var mockRenderer = new Mock<IOmegaTextRenderer>();
            mockRenderer.As<System.IDisposable>();
            var omegaUi = new OmegaUi();
            omegaUi.SetShaderControllerForTest(mockShader.Object);
            omegaUi.SetTextRendererForTest(mockRenderer.Object);
            omegaUi.Dispose();
            omegaUi.Dispose(); // Should not call Dispose again
            mockShader.As<System.IDisposable>().Verify(m => m.Dispose(), Times.Once);
            mockRenderer.As<System.IDisposable>().Verify(m => m.Dispose(), Times.Once);
        }
    }
}
