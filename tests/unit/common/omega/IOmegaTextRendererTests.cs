// <copyright file="IOmegaTextRendererTests.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using GdUnit4;
using OmegaSpiral.Source.Ui.Omega;
using static GdUnit4.Assertions;

namespace OmegaSpiral.Tests.Unit.Common.Omega;

/// <summary>
/// Unit tests for IOmegaTextRenderer interface.
/// Verifies interface contract and implementation requirements.
/// </summary>
[TestSuite]
public partial class IOmegaTextRendererTests
{
    /// <summary>
    /// Interface has AppendTextAsync method signature.
    /// </summary>
    [TestCase]
    public void Interface_HasAppendTextAsyncMethod()
    {
        var interfaceType = typeof(IOmegaTextRenderer);
        var method = interfaceType.GetMethod("AppendTextAsync");

        AssertThat(method).IsNotNull();
        AssertThat(method!.ReturnType.Name).Contains("Task");
    }

    /// <summary>
    /// Interface has ClearText method.
    /// </summary>
    [TestCase]
    public void Interface_HasClearTextMethod()
    {
        var interfaceType = typeof(IOmegaTextRenderer);
        var method = interfaceType.GetMethod("ClearText");

        AssertThat(method).IsNotNull();
    }

    /// <summary>
    /// Interface has SetTextColor method.
    /// </summary>
    [TestCase]
    public void Interface_HasSetTextColorMethod()
    {
        var interfaceType = typeof(IOmegaTextRenderer);
        var method = interfaceType.GetMethod("SetTextColor");

        AssertThat(method).IsNotNull();
    }

    /// <summary>
    /// Interface has IsAnimating method.
    /// </summary>
    [TestCase]
    public void Interface_HasIsAnimatingMethod()
    {
        var interfaceType = typeof(IOmegaTextRenderer);
        var method = interfaceType.GetMethod("IsAnimating");

        AssertThat(method).IsNotNull();
    }

    /// <summary>
    /// OmegaTextRenderer implements the interface.
    /// </summary>
    [TestCase]
    public void OmegaTextRenderer_ImplementsInterface()
    {
        var implementationType = typeof(OmegaTextRenderer);
        var interfaceType = typeof(IOmegaTextRenderer);

        AssertThat(interfaceType.IsAssignableFrom(implementationType)).IsTrue();
    }
}
