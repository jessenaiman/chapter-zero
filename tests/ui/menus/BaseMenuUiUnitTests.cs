// <copyright file="BaseMenuUiUnitTests.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using GdUnit4;
using static GdUnit4.Assertions;
using Godot;
using OmegaSpiral.Source.Stages.Stage0Start;
using OmegaSpiral.Source.Ui.Menus;
using OmegaSpiral.Source.Ui.Omega;
using OmegaSpiral.Tests.Shared;

namespace OmegaSpiral.Tests.Ui;

/// <summary>
/// Unit tests for BaseMenuUi using scene-based instantiation.
/// Tests BaseMenuUi functionality by loading MainMenu scene (which extends BaseMenuUi).
/// </summary>
[TestSuite]
[RequireGodotRuntime]
public class BaseMenuUiUnitTests
{
    private ISceneRunner? _Runner;
    private BaseMenuUi? _BaseMenuUi;

    [Before]
    public void Setup()
    {
        // Load minimal test fixture (TestMenuStub) that extends BaseMenuUi
        // This avoids hanging on MainMenu initialization (manifest loading, etc.)
        _Runner = ISceneRunner.Load("res://tests/fixtures/ui/menus/base_menu_ui_test.tscn");
        var scene = _Runner.Scene();
        _BaseMenuUi = AutoFree((BaseMenuUi)scene)!;


        AssertThat(_BaseMenuUi).IsNotNull()
            .OverrideFailureMessage("Test fixture scene failed to load for BaseMenuUi testing");
    }

    [After]
    public void Cleanup()
    {
        _Runner?.Dispose();
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
        // MainMenu scene already has buttons in MenuButtonContainer
        var menuButtonContainer = _BaseMenuUi?.GetNode<VBoxContainer>("ContentContainer/MenuButtonContainer");
        AssertThat(menuButtonContainer).IsNotNull();

        // Add a test button to ensure we're testing the method, not just scene structure
        var testButton = new Button { Text = "Test Button" };
        menuButtonContainer!.AddChild(testButton);

        // Act: Focus the first button
        _BaseMenuUi?.FocusFirstButton();

        // Assert: Some button should have focus (either existing or our test button)
        var focusedButton = _BaseMenuUi?.GetFocusedButton();
        AssertThat(focusedButton).IsNotNull();
    }

    [TestCase]
    public void GetFocusedButton_ReturnsCorrectButton()
    {
        var menuButtonContainer = _BaseMenuUi?.GetNode<VBoxContainer>("ContentContainer/MenuButtonContainer");
        AssertThat(menuButtonContainer).IsNotNull();

        // Add test buttons
        var button1 = new Button { Text = "Button 1" };
        var button2 = new Button { Text = "Button 2" };
        menuButtonContainer!.AddChild(button1);
        menuButtonContainer.AddChild(button2);

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
        var menuButtonContainer = _BaseMenuUi?.GetNode<VBoxContainer>("ContentContainer/MenuButtonContainer");
        AssertThat(menuButtonContainer).IsNotNull();

        // Add a button but don't focus it
        var button = new Button { Text = "Test Button" };
        menuButtonContainer!.AddChild(button);

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
        var focusedButton = _BaseMenuUi?.GetFocusedButton();

        // Assert: Should return null
        AssertThat(focusedButton).IsNull();
    }


}
