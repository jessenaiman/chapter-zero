// <copyright file="IOmegaShaderControllerTests.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using GdUnit4;
using OmegaSpiral.Source.Ui.Omega;
using static GdUnit4.Assertions;

namespace OmegaSpiral.Tests.Unit.Common.Omega;

/// <summary>
/// Unit tests for IOmegaShaderController interface.
/// Verifies interface contract and implementation requirements.
/// </summary>
[TestSuite]
public partial class IOmegaShaderControllerTests
{
    /// <summary>
    /// Interface has ApplyVisualPresetAsync method signature.
    /// </summary>
    [TestCase]
    public void Interface_HasApplyVisualPresetAsyncMethod()
    {
        var interfaceType = typeof(IOmegaShaderController);
        var method = interfaceType.GetMethod("ApplyVisualPresetAsync");

        AssertThat(method).IsNotNull();
        AssertThat(method!.ReturnType.Name).Contains("Task");
    }

    /// <summary>
    /// Interface has ResetShaderEffects method.
    /// </summary>
    [TestCase]
    public void Interface_HasResetShaderEffectsMethod()
    {
        var interfaceType = typeof(IOmegaShaderController);
        var method = interfaceType.GetMethod("ResetShaderEffects");

        AssertThat(method).IsNotNull();
    }

    /// <summary>
    /// Interface has PixelDissolveAsync method.
    /// </summary>
    [TestCase]
    public void Interface_HasPixelDissolveAsyncMethod()
    {
        var interfaceType = typeof(IOmegaShaderController);
        var method = interfaceType.GetMethod("PixelDissolveAsync");

        AssertThat(method).IsNotNull();
        AssertThat(method!.ReturnType.Name).Contains("Task");
    }

    /// <summary>
    /// OmegaShaderController implements the interface.
    /// </summary>
    [TestCase]
    public void OmegaShaderController_ImplementsInterface()
    {
        var implementationType = typeof(OmegaShaderController);
        var interfaceType = typeof(IOmegaShaderController);

        AssertThat(interfaceType.IsAssignableFrom(implementationType)).IsTrue();
    }
}
