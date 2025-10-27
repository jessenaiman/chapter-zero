// <copyright file="BaseMenuUiTests.cs" company="Ωmega Spiral">
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

namespace OmegaSpiral.Tests.Integration.Ui;

/// <summary>
/// Integration tests for BaseMenuUi base component.
/// Tests structure, button management, navigation, and API surface with real scenes.
/// </summary>
[TestSuite]
[RequireGodotRuntime]
public partial class BaseMenuUiTests
{
    private ISceneRunner? _Runner;
    private BaseMenuUi? _BaseMenuUi;

    [Before]
    public async Task Setup()
    {
        // Load MainMenu scene (extends BaseMenuUi) to test BaseMenuUi functionality with real scene structure
        _Runner = ISceneRunner.Load("res://source/ui/menus/main_menu.tscn");
        var mainMenu = (MainMenu)_Runner.Scene();
        _BaseMenuUi = mainMenu;

        // Wait for scene initialization
        await _Runner.SimulateFrames(10);

        // Validate background/theme using shared helper
        // If this fails, all subsequent tests will cascade fail
        OmegaUiTestHelper.ValidateBackgroundTheme(_BaseMenuUi, "BaseMenuUi");
    }

    [After]
    public void Cleanup()
    {
        _Runner?.Dispose();
    }    // ==================== INHERITANCE & API ====================

    /// <summary>
    /// BaseMenuUi extends OmegaThemedContainer.
    /// </summary>
    [TestCase]
    public void BaseMenuUi_ExtendsOmegaThemedContainer()
    {
        AssertThat(typeof(BaseMenuUi).BaseType).IsEqual(typeof(OmegaThemedContainer));
        AssertThat(typeof(BaseMenuUi).IsAssignableTo(typeof(Control))).IsTrue();
    }

    /// <summary>
    /// BaseMenuUi exposes MenuMode enum with Standard value.
    /// </summary>
    [TestCase]
    public void BaseMenuUi_HasMenuModeEnum()
    {
        var prop = typeof(BaseMenuUi).GetProperty("Mode");
        AssertThat(prop).IsNotNull();

        var enumType = typeof(BaseMenuUi).GetNestedType("MenuMode");
        AssertThat(enumType).IsNotNull();
    }

    /// <summary>
    /// BaseMenuUi exposes protected helper methods for subclasses.
    /// </summary>
    [TestCase]
    public void BaseMenuUi_ExposesProtectedHelpers()
    {
        var setMenuTitle = typeof(BaseMenuUi).GetMethod("SetMenuTitle", BindingFlags.NonPublic | BindingFlags.Instance);
        var addButton = typeof(BaseMenuUi).GetMethod("AddMenuButton", BindingFlags.NonPublic | BindingFlags.Instance);
        var clearButtons = typeof(BaseMenuUi).GetMethod("ClearMenuButtons", BindingFlags.NonPublic | BindingFlags.Instance);

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
        var container = _BaseMenuUi?.ContentContainer;
        AssertThat(container).IsNotNull();
        AssertThat(container).IsInstanceOf<Control>();
    }

    /// <summary>
    /// MenuTitle label exists with expected text.
    /// </summary>
    [TestCase]
    public void MenuTitle_Exists()
    {
        var title = _BaseMenuUi?.MenuTitle;
        AssertThat(title).IsNotNull();
        // MainMenu scene has "Ωmega Spiral" as title text
        AssertThat(title?.Text).IsNotEmpty();
    }

    /// <summary>
    /// MenuButtonContainer exists and is VBoxContainer.
    /// </summary>
    [TestCase]
    public void ButtonContainer_Exists()
    {
        var container = _BaseMenuUi?.MenuButtonContainer;
        AssertThat(container).IsNotNull();
        AssertThat(container).IsInstanceOf<VBoxContainer>();
    }

    /// <summary>
    /// MenuActionBar exists and is HBoxContainer.
    /// </summary>
    [TestCase]
    public void ActionBar_Exists()
    {
        var actionBar = _BaseMenuUi?.MenuActionBar;
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
        var container = _BaseMenuUi?.MenuButtonContainer;
        AssertThat(container).IsNotNull();

        var initialCount = container!.GetChildCount();
        var testButton = AutoFree(new Button { Text = "Test Button" });
        container.AddChild(testButton);

        AssertInt(container.GetChildCount()).IsEqual(initialCount + 1);
    }

    /// <summary>
    /// BaseMenuUi is enabled by default (mouse filter allows interaction).
    /// </summary>
    [TestCase]
    public void Menu_IsEnabledByDefault()
    {
        var container = _BaseMenuUi?.MenuButtonContainer;
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
        var container = _BaseMenuUi?.MenuButtonContainer;
        AssertThat(container).IsNotNull();

        // Add test buttons
        var button1 = AutoFree(new Button { Text = "Button 1" });
        var button2 = AutoFree(new Button { Text = "Button 2" });
        container!.AddChild(button1);
        container.AddChild(button2);

        // Act: Focus the first button
        _BaseMenuUi?.FocusFirstButton();

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
    [TestCase]
    public void GetFocusedButton_ReturnsCorrectButton()
    {
        var container = _BaseMenuUi?.MenuButtonContainer;
        AssertThat(container).IsNotNull();

        var testButton = AutoFree(new Button { Text = "Test Button", Visible = true });
        container!.AddChild(testButton);
        testButton!.GrabFocus();

        var focusedButton = _BaseMenuUi!.GetFocusedButton();
        AssertThat(focusedButton).IsNotNull();
        AssertThat(focusedButton).IsEqual(testButton);
    }

    // ==================== MODE & STATE ====================

    /// <summary>
    /// MenuMode defaults to Standard.
    /// </summary>
    [TestCase]
    public void MenuMode_DefaultsToStandard()
    {
        AssertThat(_BaseMenuUi?.Mode).IsEqual(BaseMenuUi.MenuMode.Standard);
    }
}
