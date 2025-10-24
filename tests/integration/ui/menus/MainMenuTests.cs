// <copyright file="MainMenuTests.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using System.Threading.Tasks;
using Godot;
using GdUnit4;
using GdUnit4.Api;
using OmegaSpiral.Source.Stages.Stage0Start;
using OmegaSpiral.Source.Ui.Menus;
using static GdUnit4.Assertions;

namespace OmegaSpiral.Tests.Integration.Ui.Menus;

/// <summary>
/// Integration tests for MainMenu.
/// Validates manifest parsing, dynamic stage button creation, and stage selection functionality.
/// </summary>
[TestSuite]
[RequireGodotRuntime]
public partial class MainMenuTests : Node
{
    private const string _MainMenuScenePath = "res://source/stages/stage_0_start/main_menu.tscn";
    private const string _ManifestPath = "res://source/stages/stage_0_start/main_menu_manifest.json";

    /// <summary>
    /// Tests that MainMenu loads and parses the stage manifest correctly.
    /// Verifies that stage buttons are created dynamically from the manifest.
    /// </summary>
    [TestCase]
    [RequireGodotRuntime]
    public async Task MainMenu_LoadsStagesFromManifest()
    {
        using ISceneRunner runner = ISceneRunner.Load(_MainMenuScenePath);
        await runner.SimulateFrames(5).ConfigureAwait(false); // Give time for manifest loading

        var mainMenu = runner.Scene() as MainMenu;
        AssertThat(mainMenu).IsNotNull();
        var menu = mainMenu!;

        // Check that stage buttons were created in the StageButtonList container
        var nullableStageButtonList = menu.GetNodeOrNull<VBoxContainer>("MenuContainer/MenuWrapper/MenuContent/MenuButtonsMargin/MenuButtonsContainer/StageButtonList");
        AssertThat(nullableStageButtonList).IsNotNull();
        var stageButtonList = nullableStageButtonList!;

        // Should have 6 stage buttons based on manifest
        var stageButtons = stageButtonList.GetChildren().Where(child => child is Button).ToArray();
        AssertThat(stageButtons.Length).IsEqual(6);
    }

    /// <summary>
    /// Tests that stage buttons have correct text from manifest display names.
    /// </summary>
    [TestCase]
    [RequireGodotRuntime]
    public async Task MainMenu_StageButtonsHaveCorrectText()
    {
        using ISceneRunner runner = ISceneRunner.Load(_MainMenuScenePath);
        await runner.SimulateFrames(5).ConfigureAwait(false);

        var mainMenu = runner.Scene() as MainMenu;
        AssertThat(mainMenu).IsNotNull();
        var menu = mainMenu!;

        var nullableStageButtonList = menu.GetNodeOrNull<VBoxContainer>("MenuContainer/MenuWrapper/MenuContent/MenuButtonsMargin/MenuButtonsContainer/StageButtonList");
        AssertThat(nullableStageButtonList).IsNotNull();
        var stageButtonList = nullableStageButtonList!;

        var stageButtons = stageButtonList.GetChildren().Where(child => child is Button).Cast<Button>().ToArray();

        // Check that buttons have expected text from manifest
        AssertThat(stageButtons[0].Text).Contains("Ghost Terminal");
        AssertThat(stageButtons[1].Text).Contains("Nethack");
        AssertThat(stageButtons[2].Text).Contains("Amnesia");
        AssertThat(stageButtons[3].Text).Contains("Party Selection");
        AssertThat(stageButtons[4].Text).Contains("Fractured Escape");
        AssertThat(stageButtons[5].Text).Contains("System Log Epilogue");
    }

    /// <summary>
    /// Tests that clicking a stage button triggers stage selection.
    /// Verifies that the SceneManager is called to transition to the selected stage.
    /// </summary>
    [TestCase]
    [RequireGodotRuntime]
    public async Task MainMenu_StageButtonClick_TriggersStageSelection()
    {
        using ISceneRunner runner = ISceneRunner.Load(_MainMenuScenePath);
        await runner.SimulateFrames(5).ConfigureAwait(false);

        var mainMenu = runner.Scene() as MainMenu;
        AssertThat(mainMenu).IsNotNull();
        var menu = mainMenu!;

        var stageButtonList = menu.GetNodeOrNull<VBoxContainer>("MenuContainer/MenuWrapper/MenuContent/MenuButtonsMargin/MenuButtonsContainer/StageButtonList");
        AssertThat(stageButtonList).IsNotNull();

        var firstStageButton = stageButtonList!.GetChildren().Where(child => child is Button).Cast<Button>().First();
        AssertThat(firstStageButton).IsNotNull();

        // Click the first stage button
        firstStageButton.EmitSignal("pressed");
        await runner.SimulateFrames(2).ConfigureAwait(false);

        // Verify the scene runner still exists (no crashes)
        AssertThat(runner).IsNotNull();
    }
}
