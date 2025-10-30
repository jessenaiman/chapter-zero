// <copyright file="MainMenu_IntegrationTests.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using GdUnit4;
using OmegaSpiral.Source.Stages.Stage0Start;
using OmegaSpiral.Source.Ui.Omega;
using static GdUnit4.Assertions;

namespace OmegaSpiral.Tests.Ui;

/// <summary>
/// Integration tests for MainMenu with real scene instantiation.
/// Tests that Omega visual theme (border, CRT effects, centering) works in actual gameplay.
///
/// RESPONSIBILITY: Verify MainMenu loads correctly with all Omega visual components
/// present and functional - no mocking, tests real scene behavior.
/// </summary>
[TestSuite]
[RequireGodotRuntime]
public partial class MainMenu_IntegrationTests
{
    private ISceneRunner? _Runner;
    private MainMenu? _MainMenu;

    [Before]
    public async Task Setup()
    {
        // Load the actual scene file used in the game
        _Runner = ISceneRunner.Load("res://source/ui/menus/main_menu.tscn");

        AssertThat(_Runner).IsNotNull();

        var scene = _Runner.Scene();
        AssertThat(scene).IsNotNull();

        _MainMenu = scene as MainMenu;
        AssertThat(_MainMenu).IsNotNull();

        // Wait for scene initialization
        await _Runner.SimulateFrames(10);
    }

    [After]
    public void Cleanup()
    {
        _Runner?.Dispose();
    }

    // NOTE: Visual component tests (BorderFrame, PhosphorLayer, etc.) moved to OmegaContainer_Tests
    // Those are Omega framework concerns, not MainMenu-specific concerns

    // ==================== MENU STRUCTURE ====================

    /// <summary>
    /// Tests that all required menu nodes exist (smoke test for scene integrity).
    /// </summary>
    [TestCase(Timeout = 5000)]
    public void MainMenu_HasRequiredNodes()
    {
        AssertThat(_MainMenu).IsNotNull();
        if (_MainMenu == null) return;

        // Core structure
        AssertThat(_MainMenu.GetNodeOrNull("ContentContainer")).IsNotNull();
        AssertThat(_MainMenu.GetNodeOrNull("ContentContainer/MenuTitle")).IsNotNull();
        AssertThat(_MainMenu.GetNodeOrNull("ContentContainer/MenuButtonContainer")).IsNotNull();
        AssertThat(_MainMenu.GetNodeOrNull("ContentContainer/MenuActionBar")).IsNotNull();
    }

    /// <summary>
    /// Tests that MainMenu extends MenuUi which extends OmegaContainer.
    /// </summary>
    [TestCase(Timeout = 2000)]
    public void MainMenu_InheritsOmegaTheming()
    {
        AssertThat(_MainMenu).IsInstanceOf<OmegaContainer>();
    }
}
