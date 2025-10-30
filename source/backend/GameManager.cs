// <copyright file="GameManager.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

namespace OmegaSpiral.Source.Backend;

using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;
using OmegaSpiral.Source.Backend.Narrative;
using OmegaSpiral.Source.Stages.Stage3;
using OmegaSpiral.Source.Stages.Stage4;
using OmegaSpiral.Source.Stages.Stage5;

/// <summary>
/// Simple game manager that loads stage CinematicDirectors and runs them sequentially.
/// Manages game state and stage progression.
/// </summary>
[GlobalClass]
public partial class GameManager : Node
{
    /// <summary>
    /// Gets a value indicating whether the game is currently running.
    /// </summary>
    public bool IsRunning { get; private set; }

    private List<string> StageIds { get; } = new()
    {
        "stage_1_ghost",
        "stage_2_nethack",
        "stage_3_town",
        "stage_4_party_selection",
        "stage_5_escape",
    };

    private int CurrentStageIndex { get; set; }

    /// <summary>
    /// Called when the node enters the scene tree.
    /// Does not auto-start; MainMenu should call StartGameAsync().
    /// </summary>
    public override void _Ready()
    {
        GD.Print("[GameManager] Ready. Awaiting start command from MainMenu.");
    }

    /// <summary>
    /// Starts the game from the beginning or from a specific stage.
    /// </summary>
    /// <param name="startStageIndex">The stage index to start from (default: 0).</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task StartGameAsync(int startStageIndex = 0)
    {
        if (this.IsRunning)
        {
            GD.PrintErr("[GameManager] Game is already running!");
            return;
        }

        this.IsRunning = true;
        this.CurrentStageIndex = startStageIndex;

        GD.Print($"[GameManager] Starting game from stage {startStageIndex}");

        await this.RunGameLoopAsync().ConfigureAwait(false);
    }

    private async Task RunGameLoopAsync()
    {
        while (this.CurrentStageIndex < this.StageIds.Count)
        {
            var stageId = this.StageIds[this.CurrentStageIndex];
            GD.Print($"[GameManager] === Starting Stage {this.CurrentStageIndex + 1}: {stageId} ===");

            var director = this.CreateCinematicDirector(stageId);
            if (director != null)
            {
                await director.RunStageAsync().ConfigureAwait(false);
                GD.Print($"[GameManager] Stage {this.CurrentStageIndex + 1} completed.");
            }
            else
            {
                GD.PrintErr($"[GameManager] Failed to create director for {stageId}");
            }

            this.CurrentStageIndex++;
        }

        GD.Print("[GameManager] All stages completed!");
        this.IsRunning = false;
    }

    private ICinematicDirector? CreateCinematicDirector(string stageId)
    {
        // Factory method to create the appropriate director for each stage
        return stageId switch
        {
            "stage_1_ghost" => new Stages.Stage1.GhostCinematicDirector(),
            "stage_2_nethack" => new Stages.Stage2.NethackCinematicDirector(),
            "stage_3_town" => new OmegaSpiral.Source.Stages.Stage3.TownCinematicDirector(),
            "stage_4_party_selection" => new OmegaSpiral.Source.Stages.Stage4.PartySelectionCinematicDirector(),
            "stage_5_escape" => new OmegaSpiral.Source.Stages.Stage5.EscapeCinematicDirector(),
            _ => null,
        };
    }
}
