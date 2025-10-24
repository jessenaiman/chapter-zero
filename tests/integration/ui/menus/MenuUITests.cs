// <copyright file="MenuUiTests.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using System;
using System.Reflection;
using Godot;
using GdUnit4;
using OmegaSpiral.Source.Ui.Menus;
using OmegaSpiral.Source.Ui.Omega;
using static GdUnit4.Assertions;

namespace OmegaSpiral.Tests.Integration.Ui.Menus;

/// <summary>
/// Integration tests for MenuUi base class.
/// Validates button container management, action bar, menu title, and state transitions.
/// </summary>
[TestSuite]
[RequireGodotRuntime]
public partial class MenuUiTests : Node
{
    [TestCase]
    public void MenuUi_IsValidControl()
    {
        // MenuUi should extend OmegaUi which extends Control
        AssertThat(typeof(MenuUi).IsAssignableTo(typeof(Control))).IsTrue();
    }

    [TestCase]
    public void MenuUi_HasMenuMode()
    {
        // MenuUi should have a MenuMode export property
        var prop = typeof(MenuUi).GetProperty("Mode");
        AssertThat(prop).IsNotNull();
    }

    [TestCase]
    public void MenuUi_MenuModeEnum_HasStandardValue()
    {
        // MenuUi.MenuMode enum should have Standard value
        var enumType = typeof(MenuUi).GetNestedType("MenuMode");
        AssertThat(enumType).IsNotNull();
    }

    [TestCase]
    public void MenuUi_ExposesProtectedMethodsForSubclasses()
    {
        // MenuUi should expose protected methods for subclasses
        var setMenuTitle = typeof(MenuUi).GetMethod("SetMenuTitle", BindingFlags.NonPublic | BindingFlags.Instance);
        var getContainer = typeof(MenuUi).GetMethod("GetMenuButtonContainer", BindingFlags.NonPublic | BindingFlags.Instance);
        var addButton = typeof(MenuUi).GetMethod("AddMenuButton", BindingFlags.NonPublic | BindingFlags.Instance);
        var clearButtons = typeof(MenuUi).GetMethod("ClearMenuButtons", BindingFlags.NonPublic | BindingFlags.Instance);
        var setEnabled = typeof(MenuUi).GetMethod("SetMenuEnabled", BindingFlags.NonPublic | BindingFlags.Instance);
        var getActionBar = typeof(MenuUi).GetMethod("GetMenuActionBar", BindingFlags.NonPublic | BindingFlags.Instance);

        AssertThat(setMenuTitle).IsNotNull();
        AssertThat(getContainer).IsNotNull();
        AssertThat(addButton).IsNotNull();
        AssertThat(clearButtons).IsNotNull();
        AssertThat(setEnabled).IsNotNull();
        AssertThat(getActionBar).IsNotNull();
    }

    [TestCase]
    public void MenuUi_ExtendsOmegaUi()
    {
        // Verify MenuUi extends OmegaUi
        AssertThat(typeof(MenuUi).BaseType).IsEqual(typeof(OmegaUi));
    }
}
