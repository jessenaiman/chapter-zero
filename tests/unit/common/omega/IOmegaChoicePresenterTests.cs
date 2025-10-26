// <copyright file="IOmegaChoicePresenterTests.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using GdUnit4;
using OmegaSpiral.Source.Ui.Omega;
using static GdUnit4.Assertions;
using System.Linq;

namespace OmegaSpiral.Tests.Unit.Common.Omega;

/// <summary>
/// Unit tests for IOmegaChoicePresenter interface.
/// Verifies interface contract and implementation requirements.
/// </summary>
[TestSuite]
public partial class IOmegaChoicePresenterTests
{
    /// <summary>
    /// Interface has PresentChoicesAsync method signature.
    /// </summary>
    [TestCase]
    public void Interface_HasPresentChoicesAsyncMethod()
    {
        var interfaceType = typeof(IOmegaChoicePresenter);
        var methods = interfaceType.GetMethods();
        var presentChoicesAsyncMethods = methods.Where(m => m.Name == "PresentChoicesAsync").ToList();

        // Multiple overloads exist, just verify at least one is a Task
        AssertThat(presentChoicesAsyncMethods.Count).IsGreater(1);
        AssertThat(presentChoicesAsyncMethods.Any(m => m.ReturnType.GetGenericTypeDefinition() == typeof(Task<>))).IsTrue();
    }

    /// <summary>
    /// Interface has HideChoices method.
    /// </summary>
    [TestCase]
    public void Interface_HasHideChoicesMethod()
    {
        var interfaceType = typeof(IOmegaChoicePresenter);
        var method = interfaceType.GetMethod("HideChoices");

        AssertThat(method).IsNotNull();
    }

    /// <summary>
    /// OmegaChoicePresenter implements the interface.
    /// </summary>
    [TestCase]
    public void OmegaChoicePresenter_ImplementsInterface()
    {
        var implementationType = typeof(OmegaChoicePresenter);
        var interfaceType = typeof(IOmegaChoicePresenter);

        AssertThat(interfaceType.IsAssignableFrom(implementationType)).IsTrue();
    }
}
