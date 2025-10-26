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
        _MenuUi = AutoFree(new MenuUi())!;
        AddChild(_MenuUi);
        _MenuUi.Initialize(); // Use synchronous initialization for tests
        AssertThat(_MenuUi).IsNotNull();
    }

    [After]
    public void Cleanup()
    {
        // Explicitly free the MenuUi and all its children
        _MenuUi?.QueueFree();
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
        var container = _MenuUi?.ContentContainer;
        AssertThat(container).IsNotNull();
        AssertThat(container).IsInstanceOf<Control>();
    }

    /// <summary>
    /// MenuTitle label exists with expected text.
    /// </summary>
    [TestCase]
    public void MenuTitle_Exists()
    {
        var title = _MenuUi?.MenuTitle;
        AssertThat(title).IsNotNull();
        AssertThat(title?.Text).IsEqual("");
    }

    /// <summary>
    /// MenuButtonContainer exists and is VBoxContainer.
    /// </summary>
    [TestCase]
    public void ButtonContainer_Exists()
    {
        var container = _MenuUi?.MenuButtonContainer;
        AssertThat(container).IsNotNull();
        AssertThat(container).IsInstanceOf<VBoxContainer>();
    }

    /// <summary>
    /// MenuActionBar exists and is HBoxContainer.
    /// </summary>
    [TestCase]
    public void ActionBar_Exists()
    {
        var actionBar = _MenuUi?.MenuActionBar;
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
        var container = _MenuUi?.MenuButtonContainer;
        AssertThat(container).IsNotNull();

        var initialCount = container!.GetChildCount();
        var testButton = AutoFree(new Button { Text = "Test Button" });
        container.AddChild(testButton);

        AssertInt(container.GetChildCount()).IsEqual(initialCount + 1);
    }

    /// <summary>
    /// MenuUi is enabled by default (mouse filter allows interaction).
    /// </summary>
    [TestCase]
    public void Menu_IsEnabledByDefault()
    {
        var container = _MenuUi?.MenuButtonContainer;
        AssertThat(container).IsNotNull();
        AssertObject(container!.MouseFilter).IsEqual(Control.MouseFilterEnum.Pass);
        AssertObject(container.Modulate).IsEqual(Colors.White);
    }

    // ==================== NAVIGATION ====================

    /// <summary>
    /// FocusFirstButton() focuses the first button in the menu button container.
    /// </summary>
    [TestCase]
    public void FocusFirstButton_WorksCorrectly()
    {
        var container = _MenuUi?.MenuButtonContainer;
        AssertThat(container).IsNotNull();

        // Add test buttons
        var button1 = AutoFree(new Button { Text = "Button 1" });
        var button2 = AutoFree(new Button { Text = "Button 2" });
        container!.AddChild(button1);
        container.AddChild(button2);

        // Act: Focus the first button
        _MenuUi?.FocusFirstButton();

        // Assert: The first button should have focus
        AssertThat(button1?.HasFocus()).IsTrue();
    }

    /// <summary>
    /// Can retrieve currently focused button via GetFocusedButton().
    /// </summary>
    [TestCase]
    public void GetFocusedButton_ReturnsCorrectButton()
    {
        var container = _MenuUi?.MenuButtonContainer;
        AssertThat(container).IsNotNull();

        var testButton = AutoFree(new Button { Text = "Test Button", Visible = true });
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
