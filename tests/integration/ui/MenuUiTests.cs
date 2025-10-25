// <copyright file="MenuUiTests.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using System.Reflection;
using GdUnit4;
using Godot;
using OmegaSpiral.Source.Ui.Menus;
using OmegaSpiral.Source.Ui.Omega;
using static GdUnit4.Assertions;

namespace OmegaSpiral.Tests.Integration.Ui;

/// <summary>
/// Integration tests for MenuUi base component.
/// Tests structure, button management, navigation, and API surface with real scenes.
/// </summary>
[TestSuite]
[RequireGodotRuntime]
public partial class MenuUiTests : Node
{
    private ISceneRunner _Runner = null!;
    private MenuUi _MenuUi = null!;

    [Before]
    public void Setup()
    {
        // Load scene and let GdUnit4 manage its lifecycle - no manual Dispose needed
        _Runner = ISceneRunner.Load("res://tests/fixtures/menu_ui_test_fixture.tscn");
        _Runner.SimulateFrames(5);
        _MenuUi = (MenuUi)_Runner.Scene();
        AssertThat(_MenuUi).IsNotNull();
    }

    // ==================== INHERITANCE & API ====================

    /// <summary>
    /// MenuUi extends OmegaUi.
    /// </summary>
    [TestCase]
    public void MenuUi_ExtendsOmegaUi()
    {
        AssertThat(typeof(MenuUi).BaseType).IsEqual(typeof(OmegaUi));
        AssertThat(typeof(MenuUi).IsAssignableTo(typeof(Control))).IsTrue();
    }

    /// <summary>
    /// MenuUi exposes MenuMode enum with Standard value.
    /// </summary>
    [TestCase]
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
    [TestCase]
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
    [TestCase]
    public void ContentContainer_Exists()
    {
        var container = _MenuUi?.GetNodeOrNull<MarginContainer>("ContentContainer");
        AssertThat(container).IsNotNull();
        AssertThat(container).IsInstanceOf<MarginContainer>();
    }

    /// <summary>
    /// MenuTitle label exists with expected text.
    /// </summary>
    [TestCase]
    public void MenuTitle_Exists()
    {
        var title = _MenuUi?.GetNodeOrNull<Label>("ContentContainer/MenuTitle");
        AssertThat(title).IsNotNull();
        AssertThat(title?.Text).IsEqual("Test Menu");
    }

    /// <summary>
    /// MenuButtonContainer exists and is VBoxContainer.
    /// </summary>
    [TestCase]
    public void ButtonContainer_Exists()
    {
        var container = _MenuUi?.GetNodeOrNull<VBoxContainer>("ContentContainer/MenuButtonContainer");
        AssertThat(container).IsNotNull();
        AssertThat(container).IsInstanceOf<VBoxContainer>();
    }

    /// <summary>
    /// MenuActionBar exists and is HBoxContainer.
    /// </summary>
    [TestCase]
    public void ActionBar_Exists()
    {
        var actionBar = _MenuUi?.GetNodeOrNull<HBoxContainer>("ContentContainer/MenuActionBar");
        AssertThat(actionBar).IsNotNull();
        AssertThat(actionBar).IsInstanceOf<HBoxContainer>();
    }

    // ==================== BUTTON MANAGEMENT ====================

    /// <summary>
    /// Can add buttons programmatically to button container.
    /// </summary>
    [TestCase]
    public void ButtonContainer_CanAddButtons()
    {
        var container = _MenuUi?.GetNodeOrNull<VBoxContainer>("ContentContainer/MenuButtonContainer");
        AssertThat(container).IsNotNull();
        AssertInt(container!.GetChildCount()).IsEqual(0);

        var testButton = AutoFree(new Button { Text = "Test Button" });
        container.AddChild(testButton);

        AssertInt(container.GetChildCount()).IsEqual(1);
    }

    /// <summary>
    /// MenuUi is enabled by default (mouse filter allows interaction).
    /// </summary>
    [TestCase]
    public void Menu_IsEnabledByDefault()
    {
        var container = _MenuUi?.GetNodeOrNull<VBoxContainer>("ContentContainer/MenuButtonContainer");
        AssertThat(container).IsNotNull();
        AssertObject(container!.MouseFilter).IsEqual(Control.MouseFilterEnum.Pass);
        AssertObject(container.Modulate).IsEqual(Colors.White);
    }

    // ==================== NAVIGATION ====================

    /// <summary>
    /// Can focus first button via FocusFirstButton().
    /// </summary>
    [TestCase]
    public void FocusFirstButton_WorksCorrectly()
    {
        var container = _MenuUi?.GetNodeOrNull<VBoxContainer>("ContentContainer/MenuButtonContainer");
        AssertThat(container).IsNotNull();

        var button1 = AutoFree(new Button { Text = "Button 1" });
        var button2 = AutoFree(new Button { Text = "Button 2" });
        container!.AddChild(button1);
        container.AddChild(button2);

        _MenuUi!.FocusFirstButton();

        AssertThat(button1!.HasFocus()).IsTrue();
    }

    /// <summary>
    /// Can retrieve currently focused button via GetFocusedButton().
    /// </summary>
    [TestCase]
    public void GetFocusedButton_ReturnsCorrectButton()
    {
        var container = _MenuUi?.GetNodeOrNull<VBoxContainer>("ContentContainer/MenuButtonContainer");
        AssertThat(container).IsNotNull();

        var testButton = AutoFree(new Button { Text = "Test Button" });
        container!.AddChild(testButton);
        testButton!.GrabFocus();

        var focusedButton = _MenuUi!.GetFocusedButton();
        AssertThat(focusedButton).IsEqual(testButton);
    }

    // ==================== MODE & STATE ====================

    /// <summary>
    /// MenuMode defaults to Standard.
    /// </summary>
    [TestCase]
    public void MenuMode_DefaultsToStandard()
    {
        AssertThat(_MenuUi?.Mode).IsEqual(MenuUi.MenuMode.Standard);
    }
}
