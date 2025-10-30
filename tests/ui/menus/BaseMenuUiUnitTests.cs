// <copyright file="MenuUiUnitTests.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using GdUnit4;
using static GdUnit4.Assertions;
using Godot;
using OmegaSpiral.Source.Ui.Menus;

namespace OmegaSpiral.Tests.Ui;

/// <summary>
/// Unit tests for MenuUi using scene-based instantiation.
/// Tests MenuUi functionality by loading MainMenu scene (which extends MenuUi).
/// </summary>
[TestSuite]
[RequireGodotRuntime]
public class MenuUiUnitTests
{
    private ISceneRunner? _Runner;
    private MenuUi? _MenuUi;

    [Before]
    public async Task Setup()
    {
        // Load minimal test fixture (TestMenuStub) that extends MenuUi
        // This avoids hanging on MainMenu initialization (manifest loading, etc.)
        _Runner = ISceneRunner.Load("res://tests/fixtures/ui/menus/base_menu_ui_test.tscn");

        AssertThat(_Runner).IsNotNull();

        var scene = _Runner.Scene();
        AssertThat(scene).IsNotNull();

        _MenuUi = AutoFree((MenuUi)scene)!;

        AssertThat(_MenuUi).IsNotNull();

        // Wait for scene initialization
        await _Runner.SimulateFrames(10);
    }    [After]
    public void Cleanup()
    {
        _Runner?.Dispose();
    }

    [TestCase(Timeout = 3000)]
    public void ContentContainer_Exists()
    {
        AssertThat(_MenuUi).IsNotNull();
        if (_MenuUi == null) return;

        // The ContentContainer should be created during _Ready/CacheRequiredNodes
        var contentContainer = _MenuUi.GetNodeOrNull<Control>("ContentContainer");
        AssertThat(contentContainer).IsNotNull();
    }

    [TestCase(Timeout = 3000)]
    public void FocusFirstButton_WorksCorrectly()
    {
        AssertThat(_MenuUi).IsNotNull();
        if (_MenuUi == null) return;

        // MainMenu scene already has buttons in MenuButtonContainer
        var menuButtonContainer = _MenuUi.GetNodeOrNull<VBoxContainer>("ContentContainer/MenuButtonContainer");
        AssertThat(menuButtonContainer).IsNotNull();
        if (menuButtonContainer == null) return;

        // Add a test button to ensure we're testing the method, not just scene structure
        var testButton = new Button { Text = "Test Button" };
        menuButtonContainer.AddChild(testButton);

        // Act: Focus the first button
        _MenuUi.FocusFirstButton();

        // Assert: Some button should have focus (either existing or our test button)
        var focusedButton = _MenuUi.GetFocusedButton();
        AssertThat(focusedButton).IsNotNull();
    }

    [TestCase(Timeout = 3000)]
    public void GetFocusedButton_ReturnsCorrectButton()
    {
        AssertThat(_MenuUi).IsNotNull();
        if (_MenuUi == null) return;

        var menuButtonContainer = _MenuUi.GetNodeOrNull<VBoxContainer>("ContentContainer/MenuButtonContainer");
        AssertThat(menuButtonContainer).IsNotNull();
        if (menuButtonContainer == null) return;

        // Add test buttons
        var button1 = new Button { Text = "Button 1" };
        var button2 = new Button { Text = "Button 2" };
        menuButtonContainer.AddChild(button1);
        menuButtonContainer.AddChild(button2);

        // Set focus on the second button
        button2.GrabFocus();

        // Act: Get the focused button
        var focusedButton = _MenuUi.GetFocusedButton();

        // Assert: Should return the focused button
        AssertThat(focusedButton).IsEqual(button2);
    }

    [TestCase(Timeout = 3000)]
    public void GetFocusedButton_ReturnsNullWhenNoFocus()
    {
        AssertThat(_MenuUi).IsNotNull();
        if (_MenuUi == null) return;

        var menuButtonContainer = _MenuUi.GetNodeOrNull<VBoxContainer>("ContentContainer/MenuButtonContainer");
        AssertThat(menuButtonContainer).IsNotNull();
        if (menuButtonContainer == null) return;

        // Add a button but don't focus it
        var button = new Button { Text = "Test Button" };
        menuButtonContainer.AddChild(button);

        // Ensure no button has focus
        if (button.HasFocus())
        {
            button.ReleaseFocus();
        }

        // Clear focus from any existing buttons in the scene
        foreach (var child in menuButtonContainer.GetChildren())
        {
            if (child is Button btn && btn.HasFocus())
            {
                btn.ReleaseFocus();
            }
        }

        // Act: Get the focused button when none is focused
        var focusedButton = _MenuUi.GetFocusedButton();

        // Assert: Should return null
        AssertThat(focusedButton).IsNull();
    }


}
