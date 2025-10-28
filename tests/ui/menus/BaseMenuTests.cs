// <copyright file="MenuUiTests.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using System.Reflection;
using GdUnit4;
using Godot;
using OmegaSpiral.Source.Stages.Stage0Start;
using OmegaSpiral.Source.Ui.Menus;
using OmegaSpiral.Source.Ui.Omega;
using OmegaSpiral.Tests.Shared;
using static GdUnit4.Assertions;

namespace OmegaSpiral.Tests.Ui;

/// <summary>
/// Integration tests for MenuUi base component.
/// Tests structure, button management, navigation, and API surface with real scenes.
/// </summary>
[TestSuite]
[RequireGodotRuntime]
public partial class MenuUiTests
{
    private ISceneRunner? _Runner;
    private MenuUi? _MenuUi;

    [Before]
    public async Task Setup()
    {
        // Load test fixture (TestMenuStub extends MenuUi) to avoid MainMenu initialization hangs
        // TestMenuStub stubs out PopulateMenuButtons to prevent manifest loading
        _Runner = ISceneRunner.Load("res://tests/fixtures/ui/menus/base_menu_ui_test.tscn");

        AssertThat(_Runner).IsNotNull();

        var baseMenuUi = _Runner.Scene() as MenuUi;
        AssertThat(baseMenuUi).IsNotNull();

        _MenuUi = AutoFree(baseMenuUi);

        // Wait for scene initialization
        await _Runner.SimulateFrames(10);
    }    [After]
    public void Cleanup()
    {
        _Runner?.Dispose();
    }    // ==================== INHERITANCE & API ====================

    /// <summary>
    /// MenuUi extends OmegaThemedContainer.
    /// </summary>
    [TestCase(Timeout = 2000)]
    public void MenuUi_ExtendsOmegaThemedContainer()
    {
        AssertThat(typeof(MenuUi).BaseType).IsEqual(typeof(OmegaThemedContainer));
        AssertThat(typeof(MenuUi).IsAssignableTo(typeof(Control))).IsTrue();
    }

    /// <summary>
    /// MenuUi exposes MenuMode enum with Standard value.
    /// </summary>
    [TestCase(Timeout = 2000)]
    public void MenuUi_HasMenuModeEnum()
    {
        var prop = typeof(MenuUi).GetProperty("Mode");
        AssertThat(prop).IsNotNull();

        var enumType = typeof(MenuUi).GetNestedType("MenuMode");
        AssertThat(enumType).IsNotNull();
    }

    /// <summary>
    /// MenuUi exposes protected helper methods for subclasses.
    /// </summary>
    [TestCase(Timeout = 2000)]
    public void MenuUi_ExposesProtectedHelpers()
    {
        var setMenuTitle = typeof(MenuUi).GetMethod("SetMenuTitle", BindingFlags.NonPublic | BindingFlags.Instance);
        var addButton = typeof(MenuUi).GetMethod("AddMenuButton", BindingFlags.NonPublic | BindingFlags.Instance);
        var clearButtons = typeof(MenuUi).GetMethod("ClearMenuButtons", BindingFlags.NonPublic | BindingFlags.Instance);

        AssertThat(setMenuTitle).IsNotNull();
        AssertThat(addButton).IsNotNull();
        AssertThat(clearButtons).IsNotNull();
    }

    // ==================== STRUCTURE & LAYOUT ====================

    /// <summary>
    /// ContentContainer exists and is correct type.
    /// </summary>
    [TestCase(Timeout = 3000)]
    public void ContentContainer_Exists()
    {
        AssertThat(_MenuUi).IsNotNull();
        if (_MenuUi == null) return;

        var container = _MenuUi.ContentContainer;
        AssertThat(container).IsNotNull();
        AssertThat(container).IsInstanceOf<Control>();
    }

    /// <summary>
    /// MenuTitle label exists with expected text.
    /// </summary>
    [TestCase(Timeout = 3000)]
    public void MenuTitle_Exists()
    {
        AssertThat(_MenuUi).IsNotNull();
        if (_MenuUi == null) return;

        var title = _MenuUi.MenuTitle;
        AssertThat(title).IsNotNull();
        // MainMenu scene has "Ωmega Spiral" as title text
        AssertThat(title?.Text).IsNotEmpty();
    }

    /// <summary>
    /// MenuButtonContainer exists and is VBoxContainer.
    /// </summary>
    [TestCase(Timeout = 3000)]
    public void ButtonContainer_Exists()
    {
        AssertThat(_MenuUi).IsNotNull();
        if (_MenuUi == null) return;

        var container = _MenuUi.MenuButtonContainer;
        AssertThat(container).IsNotNull();
        AssertThat(container).IsInstanceOf<VBoxContainer>();
    }

    /// <summary>
    /// MenuActionBar exists and is HBoxContainer.
    /// </summary>
    [TestCase(Timeout = 3000)]
    public void ActionBar_Exists()
    {
        AssertThat(_MenuUi).IsNotNull();
        if (_MenuUi == null) return;

        var actionBar = _MenuUi.MenuActionBar;
        AssertThat(actionBar).IsNotNull();
        AssertThat(actionBar).IsInstanceOf<HBoxContainer>();
    }

    // ==================== BUTTON MANAGEMENT ====================

    /// <summary>
    /// Can add buttons programmatically to button container.
    /// </summary>
    [TestCase(Timeout = 3000)]
    public void ButtonContainer_CanAddButtons()
    {
        AssertThat(_MenuUi).IsNotNull();
        if (_MenuUi == null) return;

        var container = _MenuUi.MenuButtonContainer;
        AssertThat(container).IsNotNull();
        if (container == null) return;

        var initialCount = container.GetChildCount();
        var testButton = AutoFree(new Button { Text = "Test Button" });
        container.AddChild(testButton);

        AssertInt(container.GetChildCount()).IsEqual(initialCount + 1);
    }

    /// <summary>
    /// MenuUi is enabled by default (mouse filter allows interaction).
    /// </summary>
    [TestCase(Timeout = 3000)]
    public void Menu_IsEnabledByDefault()
    {
        AssertThat(_MenuUi).IsNotNull();
        if (_MenuUi == null) return;

        var container = _MenuUi.MenuButtonContainer;
        AssertThat(container).IsNotNull();
        if (container == null) return;

        AssertObject(container.MouseFilter).IsEqual(Control.MouseFilterEnum.Pass);
        AssertObject(container.Modulate).IsEqual(Colors.White);
    }

    // ==================== NAVIGATION ====================

    /// <summary>
    /// FocusFirstButton() focuses the first button in the menu button container.
    /// </summary>
    [TestCase(Timeout = 3000)]
    public void FocusFirstButton_WorksCorrectly()
    {
        AssertThat(_MenuUi).IsNotNull();
        if (_MenuUi == null) return;

        var container = _MenuUi.MenuButtonContainer;
        AssertThat(container).IsNotNull();
        if (container == null) return;

        // Add test buttons
        var button1 = AutoFree(new Button { Text = "Button 1" });
        var button2 = AutoFree(new Button { Text = "Button 2" });
        container.AddChild(button1);
        container.AddChild(button2);

        // Act: Focus the first button
        _MenuUi.FocusFirstButton();

        // Assert: The first button should have focus
        AssertThat(button1).IsNotNull();
        if (button1 != null)
        {
            AssertThat(button1.HasFocus()).IsTrue();
        }
    }

    /// <summary>
    /// Can retrieve currently focused button via GetFocusedButton().
    /// </summary>
    [TestCase(Timeout = 3000)]
    public void GetFocusedButton_ReturnsCorrectButton()
    {
        AssertThat(_MenuUi).IsNotNull();
        if (_MenuUi == null) return;

        var container = _MenuUi.MenuButtonContainer;
        AssertThat(container).IsNotNull();
        if (container == null) return;

        var testButton = AutoFree(new Button { Text = "Test Button", Visible = true });
        container.AddChild(testButton);
        testButton!.GrabFocus();

        var focusedButton = _MenuUi.GetFocusedButton();
        AssertThat(focusedButton).IsNotNull();
        AssertThat(focusedButton).IsEqual(testButton);
    }

    // ==================== MODE & STATE ====================

    /// <summary>
    /// MenuMode defaults to Standard.
    /// </summary>
    [TestCase(Timeout = 2000)]
    public void MenuMode_DefaultsToStandard()
    {
        AssertThat(_MenuUi).IsNotNull();
        if (_MenuUi == null) return;

        AssertThat(_MenuUi.Mode).IsEqual(MenuUi.MenuMode.Standard);
    }
}
