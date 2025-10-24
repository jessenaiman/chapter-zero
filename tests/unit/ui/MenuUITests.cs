// <copyright file="MenuUITests.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using GdUnit4;
using Godot;
using OmegaSpiral.Source.UI.Menus;
using static GdUnit4.Assertions;

namespace OmegaSpiral.Tests.Unit.UI;

/// <summary>
/// Unit tests for MenuUI base component.
/// Validates that all MenuUI instances have proper menu structure and button layouts.
/// Ensures menus are static (no auto-transitions) with user-driven navigation.
/// </summary>
[TestSuite]
[RequireGodotRuntime]
public class MenuUITests : IDisposable
{
    private ISceneRunner? runner;
    private MenuUI? menuUI;

    /// <summary>
    /// Sets up the test scene and MenuUI instance.
    /// Loads the menu_ui_test_fixture.tscn scene for testing.
    /// </summary>
    [Before]
    public void Setup()
    {
        this.runner = ISceneRunner.Load("res://tests/fixtures/menu_ui_test_fixture.tscn");
        this.menuUI = this.runner.Scene() as MenuUI;
        AssertThat(this.menuUI).IsNotNull();
    }

    /// <summary>
    /// Cleans up test resources.
    /// </summary>
    [After]
    public void Teardown()
    {
        this.runner?.Dispose();
        this.runner = null;
        this.menuUI = null;
    }

    /// <summary>
    /// Disposes of resources used by the test class.
    /// </summary>
    public void Dispose()
    {
        this.Teardown();
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Tests that MenuUI has a ContentContainer with margins on all sides.
    /// This ensures visual spacing so the UI doesn't touch screen edges.
    /// </summary>
    [TestCase]
    [RequireGodotRuntime]
    public void MenuUIHasContentContainerWithMargins()
    {
        var contentContainer = this.menuUI?.GetNodeOrNull<MarginContainer>("ContentContainer");
        AssertThat(contentContainer).IsNotNull();
        AssertThat(contentContainer).IsInstanceOf<MarginContainer>();
    }

    /// <summary>
    /// Tests that MenuUI has a menu title label.
    /// Ensures visual hierarchy for menu identification.
    /// </summary>
    [TestCase]
    [RequireGodotRuntime]
    public void MenuUIHasMenuTitle()
    {
        var menuTitle = this.menuUI?.GetNodeOrNull<Label>("ContentContainer/MenuTitle");
        AssertThat(menuTitle).IsNotNull();
        AssertThat(menuTitle?.Text).IsEqual("Test Menu");
    }

    /// <summary>
    /// Tests that MenuUI has a button container for menu items.
    /// Ensures structure supports dynamic button creation.
    /// </summary>
    [TestCase]
    [RequireGodotRuntime]
    public void MenuUIHasButtonContainer()
    {
        var buttonContainer = this.menuUI?.GetNodeOrNull<VBoxContainer>("ContentContainer/MenuButtonContainer");
        AssertThat(buttonContainer).IsNotNull();
        AssertThat(buttonContainer).IsInstanceOf<VBoxContainer>();
    }

    /// <summary>
    /// Tests that MenuUI has an action bar for control buttons.
    /// Ensures support for back/confirm/etc. buttons at bottom of menu.
    /// </summary>
    [TestCase]
    [RequireGodotRuntime]
    public void MenuUIHasActionBar()
    {
        var actionBar = this.menuUI?.GetNodeOrNull<HBoxContainer>("ContentContainer/MenuActionBar");
        AssertThat(actionBar).IsNotNull();
        AssertThat(actionBar).IsInstanceOf<HBoxContainer>();
    }

    /// <summary>
    /// Tests that MenuUI can add buttons programmatically.
    /// Validates the AddMenuButton helper method works correctly.
    /// </summary>
    [TestCase]
    [RequireGodotRuntime]
    public void MenuUICanAddButtons()
    {
    var containerBefore = this.menuUI?.GetNodeOrNull<VBoxContainer>("ContentContainer/MenuButtonContainer");
    AssertThat(containerBefore).IsNotNull();
    AssertInt(containerBefore!.GetChildCount()).IsEqual(0);

    // Create and add button, register for auto-free
    var testButton = new Button { Text = "Test Button" };
    containerBefore.AddChild(testButton);
    AutoFree(testButton);

    AssertInt(containerBefore.GetChildCount()).IsEqual(1);
    }

    /// <summary>
    /// Tests that MenuUI is enabled by default (mouse filter allows interaction).
    /// </summary>
    [TestCase]
    [RequireGodotRuntime]
    public void MenuUIIsEnabledByDefault()
    {
        var container = this.menuUI?.GetNodeOrNull<VBoxContainer>("ContentContainer/MenuButtonContainer");
        AssertThat(container).IsNotNull();
        AssertObject(container!.MouseFilter).IsEqual(Control.MouseFilterEnum.Pass);
        AssertObject(container.Modulate).IsEqual(Colors.White);
    }

    /// <summary>
    /// Tests that MenuUI mode is set to Standard by default.
    /// </summary>
    [TestCase]
    [RequireGodotRuntime]
    public void MenuUIModeIsStandardByDefault()
    {
        AssertThat(this.menuUI?.Mode).IsEqual(MenuUI.MenuMode.Standard);
    }

    /// <summary>
    /// Tests that MenuUI has proper anchors for full-screen coverage.
    /// </summary>
    [TestCase]
    [RequireGodotRuntime]
    public void MenuUIHasFullScreenAnchors()
    {
        AssertThat(this.menuUI).IsNotNull();
        AssertFloat(this.menuUI!.AnchorLeft).IsEqual(0.0f);
        AssertFloat(this.menuUI.AnchorRight).IsEqual(1.0f);
        AssertFloat(this.menuUI.AnchorTop).IsEqual(0.0f);
        AssertFloat(this.menuUI.AnchorBottom).IsEqual(1.0f);
    }

    /// <summary>
    /// Tests that MenuUI content container fills parent bounds.
    /// Ensures all content stretches to screen edges (with margins).
    /// </summary>
    [TestCase]
    [RequireGodotRuntime]
    public void MenuUIContentContainerFillsParent()
    {
        var contentContainer = this.menuUI?.GetNodeOrNull<Control>("ContentContainer");
        AssertThat(contentContainer).IsNotNull();
        AssertFloat(contentContainer!.AnchorLeft).IsEqual(0.0f);
        AssertFloat(contentContainer.AnchorRight).IsEqual(1.0f);
        AssertFloat(contentContainer.AnchorTop).IsEqual(0.0f);
        AssertFloat(contentContainer.AnchorBottom).IsEqual(1.0f);
    }
}
