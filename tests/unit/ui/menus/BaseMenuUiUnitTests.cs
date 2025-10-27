// <copyright file="BaseMenuUiUnitTests.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using GdUnit4;
using static GdUnit4.Assertions;
using Godot;
using OmegaSpiral.Source.InputSystem;
using OmegaSpiral.Source.Ui.Menus;
using OmegaSpiral.Source.Ui.Omega;
using OmegaSpiral.Tests.Shared;

[TestSuite]
[RequireGodotRuntime]
public class BaseMenuUiUnitTests
{
    private BaseMenuUi? _BaseMenuUi;
    private Node? _TestSceneRoot;
    private OmegaInputRouter? _InputRouter;

    [Before]
    public void Setup()
    {
        var sceneTree = (SceneTree)Engine.GetMainLoop();

        // Ensure input router exists before BaseMenuUi enters the tree
        _InputRouter = sceneTree.Root.GetNodeOrNull<OmegaInputRouter>(OmegaInputRouter.DefaultNodeName);
        if (_InputRouter == null)
        {
            _InputRouter = AutoFree(new OmegaInputRouter())!;
            sceneTree.Root.AddChild(_InputRouter);
        }

        // Create root node and register for cleanup
        _TestSceneRoot = AutoFree(new Node())!;
        sceneTree.Root.AddChild(_TestSceneRoot);

        // Create BaseMenuUi and wrap with AutoFree for automatic cleanup
        _BaseMenuUi = AutoFree(new BaseMenuUi())!;
        _TestSceneRoot.AddChild(_BaseMenuUi);  // _Ready() fires automatically via Godot lifecycle

        // Validate background/theme using shared helper
        // If this fails, all subsequent tests will cascade fail
        OmegaUiTestHelper.ValidateBackgroundTheme(_BaseMenuUi, "BaseMenuUi");
    }

    [After]
    public void Teardown()
    {
        // AutoFree handles all cleanup automatically - no manual QueueFree needed
    }

    [TestCase]
    public void ContentContainer_Exists()
    {
        // The ContentContainer should be created during _Ready/CacheRequiredNodes
        var contentContainer = _BaseMenuUi?.GetNodeOrNull<Control>("ContentContainer");
        AssertThat(contentContainer).IsNotNull();
    }

    [TestCase]
    public void FocusFirstButton_WorksCorrectly()
    {
        // Add a button to the menu button container
        var button = new Button { Text = "Test Button" };
        var menuButtonContainer = _BaseMenuUi?.GetNode<VBoxContainer>("ContentContainer/MenuButtonContainer");
        menuButtonContainer?.AddChild(button);

        // Act: Focus the first button
        _BaseMenuUi?.FocusFirstButton();

        // Assert: The button should have focus
        AssertThat(button.HasFocus()).IsTrue();
    }

    [TestCase]
    public void GetFocusedButton_ReturnsCorrectButton()
    {
        // Add buttons to the menu button container
        var button1 = new Button { Text = "Button 1" };
        var button2 = new Button { Text = "Button 2" };
        var menuButtonContainer = _BaseMenuUi?.GetNode<VBoxContainer>("ContentContainer/MenuButtonContainer");
        menuButtonContainer?.AddChild(button1);
        menuButtonContainer?.AddChild(button2);

        // Set focus on the second button
        button2.GrabFocus();

        // Act: Get the focused button
        var focusedButton = _BaseMenuUi?.GetFocusedButton();

        // Assert: Should return the focused button
        AssertThat(focusedButton).IsEqual(button2);
    }

    [TestCase]
    public void GetFocusedButton_ReturnsNullWhenNoFocus()
    {
        // Add a button but don't focus it
        var button = new Button { Text = "Test Button" };
        var menuButtonContainer = _BaseMenuUi?.GetNode<VBoxContainer>("ContentContainer/MenuButtonContainer");
        menuButtonContainer?.AddChild(button);

        // Act: Get the focused button
        var focusedButton = _BaseMenuUi?.GetFocusedButton();

        // Assert: Should return null since no button has focus
        AssertThat(focusedButton).IsNull();
    }

    [TestCase]
    public void ConfirmRequested_ActivatesFocusedButton()
    {
        var menuButtonContainer = _BaseMenuUi?.GetNode<VBoxContainer>("ContentContainer/MenuButtonContainer");
        var button = new Button { Text = "Confirm" };
        bool pressed = false;
        button.Pressed += () => pressed = true;
        menuButtonContainer?.AddChild(button);
        button.GrabFocus();

        var inputEvent = new InputEventAction
        {
            Action = "ui_accept",
            Pressed = true
        };

        _InputRouter?._UnhandledInput(inputEvent);

        AssertThat(pressed).IsTrue();
    }

    [TestCase]
    public void NavigateDown_MovesFocusToNextButton()
    {
        var menuButtonContainer = _BaseMenuUi?.GetNode<VBoxContainer>("ContentContainer/MenuButtonContainer");
        var firstButton = new Button { Text = "One" };
        var secondButton = new Button { Text = "Two" };
        menuButtonContainer?.AddChild(firstButton);
        menuButtonContainer?.AddChild(secondButton);
        firstButton.GrabFocus();

        var inputEvent = new InputEventAction
        {
            Action = "ui_down",
            Pressed = true
        };

        _InputRouter?._UnhandledInput(inputEvent);

        AssertThat(secondButton.HasFocus()).IsTrue();
    }

    [TestCase]
    public void BackRequested_HidesMenu()
    {
        _BaseMenuUi!.Visible = true;

        var inputEvent = new InputEventAction
        {
            Action = "ui_cancel",
            Pressed = true
        };

        _InputRouter?._UnhandledInput(inputEvent);

        AssertThat(_BaseMenuUi.Visible).IsFalse();
    }
}
