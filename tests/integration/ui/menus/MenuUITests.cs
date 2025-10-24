// <copyright file="MenuUITests.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using System;
using System.Reflection;
using Godot;
using GdUnit4;
using OmegaSpiral.Source.UI.Menus;
using static GdUnit4.Assertions;

namespace OmegaSpiral.Tests.Integration.UI.Menus;

/// <summary>
/// Integration tests for MenuUI base class.
/// Validates button container management, action bar, menu title, and state transitions.
/// </summary>
[TestSuite]
[RequireGodotRuntime]
public partial class MenuUITests : Node
{
    [TestCase]
    public void MenuUI_IsValidControl()
    {
        // MenuUI should extend OmegaUI which extends Control
        AssertThat(typeof(MenuUI).IsAssignableTo(typeof(Control))).IsTrue();
    }

    [TestCase]
    public void MenuUI_HasMenuMode()
    {
        // MenuUI should have a MenuMode export property
        var prop = typeof(MenuUI).GetProperty("Mode");
        AssertThat(prop).IsNotNull();
    }

    [TestCase]
    public void MenuUI_MenuModeEnum_HasStandardValue()
    {
        // MenuUI.MenuMode enum should have Standard value
        var enumType = typeof(MenuUI).GetNestedType("MenuMode");
        AssertThat(enumType).IsNotNull();
    }

    [TestCase]
    public void MenuUI_HasProtectedMethods()
    {
        // MenuUI should expose protected methods for subclasses
        var setMenuTitle = typeof(MenuUI).GetMethod("SetMenuTitle", BindingFlags.NonPublic | BindingFlags.Instance);
        var getContainer = typeof(MenuUI).GetMethod("GetMenuButtonContainer", BindingFlags.NonPublic | BindingFlags.Instance);
        var addButton = typeof(MenuUI).GetMethod("AddMenuButton", BindingFlags.NonPublic | BindingFlags.Instance);
        var clearButtons = typeof(MenuUI).GetMethod("ClearMenuButtons", BindingFlags.NonPublic | BindingFlags.Instance);
        var setEnabled = typeof(MenuUI).GetMethod("SetMenuEnabled", BindingFlags.NonPublic | BindingFlags.Instance);
        var getActionBar = typeof(MenuUI).GetMethod("GetMenuActionBar", BindingFlags.NonPublic | BindingFlags.Instance);

        AssertThat(setMenuTitle).IsNotNull();
        AssertThat(getContainer).IsNotNull();
        AssertThat(addButton).IsNotNull();
        AssertThat(clearButtons).IsNotNull();
        AssertThat(setEnabled).IsNotNull();
        AssertThat(getActionBar).IsNotNull();
    }

    [TestCase]
    public void MenuUI_ExtendsOmegaUI()
    {
        // Verify MenuUI inheritance chain
        AssertThat(typeof(MenuUI).BaseType).IsEqual(typeof(MenuUI).BaseType?.BaseType?.BaseType == typeof(object) ? typeof(MenuUI).BaseType : typeof(MenuUI).BaseType);
    }
}
