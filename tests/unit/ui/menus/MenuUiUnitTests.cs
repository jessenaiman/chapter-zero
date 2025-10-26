// <copyright file="MenuUiUnitTests.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using GdUnit4;
using static GdUnit4.Assertions;
using Godot;
using OmegaSpiral.Source.Ui.Menus;

[TestSuite]
[RequireGodotRuntime]
public class MenuUiUnitTests
{
    private MenuUi? _MenuUi;
    private Node? _TestSceneRoot;

    [Before]
    public void Setup()
    {
        _MenuUi = new MenuUi();
        _TestSceneRoot = new Node();
        _TestSceneRoot.AddChild(_MenuUi);

        // Simulate _Ready to initialize the menu structure
        _MenuUi._Ready();
    }

    [After]
    public void Teardown()
    {
        _TestSceneRoot?.QueueFree();
        _MenuUi?.QueueFree();
    }

    [TestCase]
    public void ContentContainer_Exists()
    {
        // The ContentContainer should be created during _Ready/CacheRequiredNodes
        var contentContainer = _MenuUi?.GetNodeOrNull<Control>("ContentContainer");
        AssertThat(contentContainer).IsNotNull();
    }

    [TestCase]
    public void FocusFirstButton_WorksCorrectly()
    {
        // Add a button to the menu button container
        var button = new Button { Text = "Test Button" };
        var menuButtonContainer = _MenuUi?.GetNode<VBoxContainer>("ContentContainer/MenuButtonContainer");
        menuButtonContainer?.AddChild(button);

        // Act: Focus the first button
        _MenuUi?.FocusFirstButton();

        // Assert: The button should have focus
        AssertThat(button.HasFocus()).IsTrue();
    }

    [TestCase]
    public void GetFocusedButton_ReturnsCorrectButton()
    {
        // Add buttons to the menu button container
        var button1 = new Button { Text = "Button 1" };
        var button2 = new Button { Text = "Button 2" };
        var menuButtonContainer = _MenuUi?.GetNode<VBoxContainer>("ContentContainer/MenuButtonContainer");
        menuButtonContainer?.AddChild(button1);
        menuButtonContainer?.AddChild(button2);

        // Set focus on the second button
        button2.GrabFocus();

        // Act: Get the focused button
        var focusedButton = _MenuUi?.GetFocusedButton();

        // Assert: Should return the focused button
        AssertThat(focusedButton).IsEqual(button2);
    }

    [TestCase]
    public void GetFocusedButton_ReturnsNullWhenNoFocus()
    {
        // Add a button but don't focus it
        var button = new Button { Text = "Test Button" };
        var menuButtonContainer = _MenuUi?.GetNode<VBoxContainer>("ContentContainer/MenuButtonContainer");
        menuButtonContainer?.AddChild(button);

        // Act: Get the focused button
        var focusedButton = _MenuUi?.GetFocusedButton();

        // Assert: Should return null since no button has focus
        AssertThat(focusedButton).IsNull();
    }
}
