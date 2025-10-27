// <copyright file="MainMenu_IntegrationTests.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using GdUnit4;
using Godot;
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
    public void Setup()
    {
        // Load the actual scene file used in the game
        _Runner = ISceneRunner.Load("res://source/ui/menus/main_menu.tscn");
        _MainMenu = (MainMenu)_Runner.Scene();

        AssertThat(_MainMenu).IsNotNull()
            .OverrideFailureMessage("MainMenu scene failed to load - check scene file exists and is valid");
    }

    [After]
    public void Cleanup()
    {
        _Runner?.Dispose();
    }

    // NOTE: Visual component tests (BorderFrame, PhosphorLayer, etc.) moved to OmegaThemedContainer_Tests
    // Those are Omega framework concerns, not MainMenu-specific concerns

    // ==================== MENU STRUCTURE ====================

    /// <summary>
    /// Tests that all required menu nodes exist (smoke test for scene integrity).
    /// </summary>
    [TestCase]
    public void MainMenu_HasRequiredNodes()
    {
        // Core structure
        AssertThat(_MainMenu!.GetNodeOrNull("ContentContainer")).IsNotNull()
            .OverrideFailureMessage("ContentContainer missing");
        AssertThat(_MainMenu.GetNodeOrNull("ContentContainer/MenuTitle")).IsNotNull()
            .OverrideFailureMessage("MenuTitle missing");
        AssertThat(_MainMenu.GetNodeOrNull("ContentContainer/MenuButtonContainer")).IsNotNull()
            .OverrideFailureMessage("MenuButtonContainer missing");
        AssertThat(_MainMenu.GetNodeOrNull("ContentContainer/MenuActionBar")).IsNotNull()
            .OverrideFailureMessage("MenuActionBar missing");
    }

    /// <summary>
    /// Tests that MainMenu extends BaseMenuUi which extends OmegaThemedContainer.
    /// </summary>
    [TestCase]
    public void MainMenu_InheritsOmegaTheming()
    {
        AssertThat(_MainMenu).IsInstanceOf<OmegaThemedContainer>()
            .OverrideFailureMessage("MainMenu should inherit from OmegaThemedContainer through BaseMenuUi");
    }
}
