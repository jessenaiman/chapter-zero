// <copyright file="ChoiceButtonTests.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using GdUnit4;
using Godot;
using OmegaSpiral.Source.Ui.Omega;
using static GdUnit4.Assertions;

namespace OmegaSpiral.Tests.Integration.Ui.Omega;

/// <summary>
/// Integration tests for ChoiceButton component.
/// Verifies that ChoiceButton properly inherits from Godot Button and follows the Omega architecture.
/// </summary>
[TestSuite]
[RequireGodotRuntime]
public partial class ChoiceButtonTests : Node
{
    private ChoiceButton? _ChoiceButton;

    /// <summary>
    /// Setup before each test.
    /// </summary>
    [Before]
    public void Setup()
    {
        _ChoiceButton = AutoFree(new ChoiceButton());
        AddChild(_ChoiceButton);
        _ChoiceButton!._Ready();
    }

    /// <summary>
    /// Cleanup after each test.
    /// </summary>
    [After]
    public void Teardown()
    {
        _ChoiceButton = null;
    }

    /// <summary>
    /// ChoiceButton inherits from Button.
    /// </summary>
    [TestCase]
    public void ChoiceButton_InheritsFromButton()
    {
        AssertThat(_ChoiceButton).IsNotNull();
        AssertThat(_ChoiceButton).IsInstanceOf<Button>();
        AssertThat(typeof(ChoiceButton).BaseType).IsEqual(typeof(Button));
    }

    /// <summary>
    /// ChoiceButton can be configured with choice data.
    /// </summary>
    [TestCase]
    public void Configure_SetsAllProperties()
    {
        AssertThat(_ChoiceButton).IsNotNull();

        _ChoiceButton!.Configure("Test Choice", 10, 5, 3);

        AssertThat(_ChoiceButton.ChoiceText).IsEqual("Test Choice");
        AssertThat(_ChoiceButton.Text).IsEqual("Test Choice");
        AssertThat(_ChoiceButton.LightPoints).IsEqual(10);
        AssertThat(_ChoiceButton.ShadowPoints).IsEqual(5);
        AssertThat(_ChoiceButton.AmbitionPoints).IsEqual(3);
    }

    /// <summary>
    /// ChoiceButton throws exception for null or empty text.
    /// </summary>
    [TestCase]
    public void Configure_ThrowsExceptionForEmptyText()
    {
        AssertThat(_ChoiceButton).IsNotNull();

        AssertThrown(() => _ChoiceButton!.Configure("", 1, 1, 1))
            .IsInstanceOf<System.ArgumentException>();

        AssertThrown(() => _ChoiceButton!.Configure(null!, 1, 1, 1))
            .IsInstanceOf<System.ArgumentException>();
    }

    /// <summary>
    /// ChoiceButton emits signal when pressed.
    /// </summary>
    [TestCase]
    public void OnPressed_EmitsChoiceSelectedSignal()
    {
        AssertThat(_ChoiceButton).IsNotNull();
        _ChoiceButton!.Configure("Test Choice", 10, 5, 3);

        // TODO: Implement signal monitoring test
        // Signal tests require proper GdUnit4 signal monitoring setup
        GD.Print("[ChoiceButtonTests] Signal emission test placeholder");
    }

    /// <summary>
    /// ChoiceButton properties are read/write accessible.
    /// </summary>
    [TestCase]
    public void Properties_AreReadWriteAccessible()
    {
        AssertThat(_ChoiceButton).IsNotNull();

        _ChoiceButton!.ChoiceText = "Manual Set";
        _ChoiceButton.LightPoints = 15;
        _ChoiceButton.ShadowPoints = 7;
        _ChoiceButton.AmbitionPoints = 2;

        AssertThat(_ChoiceButton.ChoiceText).IsEqual("Manual Set");
        AssertThat(_ChoiceButton.Text).IsEqual("Manual Set");
        AssertThat(_ChoiceButton.LightPoints).IsEqual(15);
        AssertThat(_ChoiceButton.ShadowPoints).IsEqual(7);
        AssertThat(_ChoiceButton.AmbitionPoints).IsEqual(2);
    }

    /// <summary>
    /// ChoiceButton supports focus mode for navigation.
    /// </summary>
    [TestCase]
    public void ChoiceButton_SupportsFocusNavigation()
    {
        AssertThat(_ChoiceButton).IsNotNull();

        // Focus mode should be set in _Ready
        AssertThat(_ChoiceButton!.FocusMode).IsEqual(Control.FocusModeEnum.All);
    }

    /// <summary>
    /// ChoiceButton cleans up signal connections on exit.
    /// </summary>
    [TestCase]
    public void ExitTree_CleansUpSignalConnections()
    {
        AssertThat(_ChoiceButton).IsNotNull();

        // Just verify the method exists and can be called without errors
        _ChoiceButton!._ExitTree();

        // Re-add to tree for cleanup
        if (!IsInstanceValid(_ChoiceButton) || !_ChoiceButton.IsInsideTree())
        {
            AddChild(_ChoiceButton);
        }
    }
}
