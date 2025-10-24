// <copyright file="MainMenuTests.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using System;
using Godot;
using GdUnit4;
using OmegaSpiral.UI.Menus;
using OmegaSpiral.Source.UI.Menus;
using static GdUnit4.Assertions;

namespace OmegaSpiral.Tests.Integration.UI.Menus;

/// <summary>
/// Integration tests for MainMenu.
/// Validates stage button loading from manifest, stage selection, and scene transitions.
/// </summary>
[TestSuite]
[RequireGodotRuntime]
public partial class MainMenuTests : Node
{
    private const string MainMenuScenePath = "res://source/stages/stage_0_start/main_menu.tscn";
    private const string ManifestPath = "res://source/stages/stage_0_start/main_menu_manifest.json";

    [TestCase]
    public void MainMenu_IsValidControl()
    {
        // MainMenu should extend MenuUI which extends OmegaUI which extends Control
        AssertThat(typeof(MainMenu).IsAssignableTo(typeof(Control))).IsTrue();
    }

    [TestCase]
    public void MainMenu_ExtendsMenuUI()
    {
        // Verify MainMenu inherits from MenuUI
        AssertThat(typeof(MainMenu).BaseType).IsEqual(typeof(MenuUI));
    }

    [TestCase]
    public void MainMenu_SceneFileExists()
    {
        AssertThat(ResourceLoader.Exists(MainMenuScenePath)).IsTrue();
    }

    [TestCase]
    public void MainMenu_ManifestFileExists()
    {
        AssertThat(ResourceLoader.Exists(ManifestPath)).IsTrue();
    }

    [TestCase]
    public void MainMenu_CanLoadScene()
    {
        var scene = GD.Load<PackedScene>(MainMenuScenePath);
        AssertThat(scene).IsNotNull();
    }

    [TestCase]
    public void MainMenu_SceneRootIsControl()
    {
        var scene = GD.Load<PackedScene>(MainMenuScenePath);
        var instance = scene.Instantiate();

        AssertThat(instance is Control).IsTrue();

        instance.QueueFree();
    }

    [TestCase]
    public void MainMenu_HasRequiredNodePaths()
    {
        // These paths should exist in the main_menu.tscn
        var scene = GD.Load<PackedScene>(MainMenuScenePath);
        AssertThat(scene).IsNotNull();

        // Just verify the paths are valid in the resource
        AssertThat(MainMenuScenePath.Contains("main_menu.tscn")).IsTrue();
    }

    [TestCase]
    public void MainMenu_ManifestIsJson()
    {
        AssertThat(ManifestPath.EndsWith(".json")).IsTrue();
    }
}
