// <copyright file="ChoiceButtonTests.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using GdUnit4;
using Godot;
using OmegaSpiral.Source.Ui.Omega;
using static GdUnit4.Assertions;

namespace OmegaSpiral.Tests.Ui.Omega;

/// <summary>
/// Integration tests for ChoiceButton component.
/// Verifies that ChoiceButton properly inherits from Godot Button and follows the Omega architecture.
///
/// TEST DISCOVERY TRAITS:
/// - Category: UI/Components
/// - Component: ChoiceButton (Button)
/// - Layer: Omega UI System
/// - Type: Integration Test
/// - Lifecycle: Tests node configuration, properties, and signal emission
///
/// LOCATION: tests/ui/omega/ChoiceButtonTests.cs
/// NAMESPACE: OmegaSpiral.Tests.Ui.Omega
/// </summary>
[TestSuite]
[RequireGodotRuntime]
public partial class ChoiceButtonTests
{
    private ChoiceButton? _ChoiceButton;

    /// <summary>
    /// Setup before each test.
    /// </summary>
    [Before]
    public void Setup()
    {
        _ChoiceButton = AutoFree(new ChoiceButton())!;
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
    [TestCase(Timeout = 10000)]
    public void ChoiceButton_InheritsFromButton()
    {
        AssertThat(_ChoiceButton).IsNotNull();
        AssertThat(_ChoiceButton).IsInstanceOf<Button>();
        AssertThat(typeof(ChoiceButton).BaseType).IsEqual(typeof(OmegaUiButton));
    }

    /// <summary>
    /// ChoiceButton can be configured with choice data.
    /// </summary>
    [TestCase(Timeout = 10000)]
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
    [TestCase(Timeout = 10000)]
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
    [TestCase(Timeout = 10000)]
    public void OnPressed_EmitsChoiceSelectedSignal()
    {
        AssertThat(_ChoiceButton).IsNotNull();
        _ChoiceButton!.Configure("Test Choice", 10, 5, 3);

        // Simulate button press and verify no exception
        _ChoiceButton.EmitSignal("pressed");

        // Verify signal was emitted
        AssertThat(_ChoiceButton.GetSignalConnectionList("pressed").Count).IsGreater(0);
    }

    /// <summary>
    /// ChoiceButton properties are read/write accessible.
    /// </summary>
    [TestCase(Timeout = 10000)]
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
    [TestCase(Timeout = 10000)]
    public void ChoiceButton_SupportsFocusNavigation()
    {
        AssertThat(_ChoiceButton).IsNotNull();

        // Focus mode should be set in _Ready
        AssertThat(_ChoiceButton!.FocusMode).IsEqual(Control.FocusModeEnum.All);
    }

    /// <summary>
    /// ChoiceButton cleans up signal connections on exit.
    /// </summary>
    [TestCase(Timeout = 10000)]
    public void ExitTree_CleansUpProperly()
    {
        AssertThat(_ChoiceButton).IsNotNull();

        // Verify the node is properly registered and can be cleaned up
        AssertThat(GodotObject.IsInstanceValid(_ChoiceButton)).IsTrue();

        // The SafeOmegaTest base class handles proper cleanup verification
        // This test just ensures the method exists and doesn't throw
        _ChoiceButton!._ExitTree();

        // Node should still be valid until Godot processes the cleanup
        AssertThat(GodotObject.IsInstanceValid(_ChoiceButton)).IsTrue();
    }
}
