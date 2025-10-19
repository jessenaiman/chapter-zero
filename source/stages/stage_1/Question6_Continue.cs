// <copyright file="Question6_Continue.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using Godot;
using System;
using System.Threading.Tasks;
using OmegaSpiral.Source.Scripts.Common;

namespace OmegaSpiral.Source.Scripts.Stages.Stage1;

/// <summary>
/// Final question scene: Continue confirmation and thread determination.
/// Determines the player's Dreamweaver thread and transitions to Stage 2.
/// </summary>
[GlobalClass]
public partial class Question6Continue : TerminalBase
{
    /// <inheritdoc/>
    public override async void _Ready()
    {
        base._Ready();

        // Determine and display thread
        await DetermineAndDisplayThreadAsync();
    }

    /// <summary>
    /// Determines the player's Dreamweaver thread and displays the result.
    /// </summary>
    /// <returns>A task that completes when thread is determined and displayed.</returns>
    private async Task DetermineAndDisplayThreadAsync()
    {
        GameState gameState = GetGameState();
        string dominantThread = gameState.GetDominantThread();
        string threadName = dominantThread.ToUpperInvariant();

        ApplyVisualPreset(dominantThread switch
        {
            "Light" => TerminalVisualPreset.ThreadLight,
            "Shadow" => TerminalVisualPreset.ThreadMischief,
            "Ambition" => TerminalVisualPreset.ThreadWrath,
            "Balance" => TerminalVisualPreset.ThreadBalance,
            _ => TerminalVisualPreset.StableBaseline,
        });

        string[] threadLines = dominantThread switch
        {
            "Light" => new[]
            {
                "> THREAD ANALYSIS COMPLETE",
                "> DOMINANT THREAD: LIGHT",
                "> NARRATIVE AFFINITY: Stories of conviction and moral certainty",
                "> DREAMWEAVER: The Guardian",
                "",
                "Your choices reveal a heart that seeks to protect and preserve.",
                "The Light thread calls to you—tales where one person's courage can illuminate the darkness."
            },
            "Shadow" => new[]
            {
                "> THREAD ANALYSIS COMPLETE",
                "> DOMINANT THREAD: SHADOW",
                "> NARRATIVE AFFINITY: Stories of observation and hidden truths",
                "> DREAMWEAVER: The Watcher",
                "",
                "Your choices reveal a mind that sees what others miss.",
                "The Shadow thread calls to you—tales where patience reveals the world's secret machinery."
            },
            "Ambition" => new[]
            {
                "> THREAD ANALYSIS COMPLETE",
                "> DOMINANT THREAD: AMBITION",
                "> NARRATIVE AFFINITY: Stories of transformation and self-mastery",
                "> DREAMWEAVER: The Weaver",
                "",
                "Your choices reveal a spirit that reshapes reality to its will.",
                "The Ambition thread calls to you—tales where every ending is just another beginning."
            },
            "Balance" => new[]
            {
                "> THREAD ANALYSIS COMPLETE",
                "> DOMINANT THREAD: BALANCE",
                "> NARRATIVE AFFINITY: Stories of complexity and philosophical depth",
                "> DREAMWEAVER: The Harmonizer",
                "",
                "Your choices reveal a soul that embraces the beautiful complexity of existence.",
                "The Balance thread calls to you—tales where no single path holds all the answers."
            },
            _ => new[]
            {
                "> THREAD ANALYSIS ERROR",
                "> UNABLE TO DETERMINE THREAD",
                "> DEFAULTING TO BALANCE"
            }
        };

        // Display thread determination with pauses
        foreach (string line in threadLines)
        {
            if (!string.IsNullOrEmpty(line))
            {
                await AppendTextAsync(line, useGhostEffect: true);
                await ToSignal(GetTree().CreateTimer(1.5f), SceneTreeTimer.SignalName.Timeout);
            }
            else
            {
                await ToSignal(GetTree().CreateTimer(0.8f), SceneTreeTimer.SignalName.Timeout);
            }
        }

        // Display score summary
        string scoreSummary = gameState.GetScoreSummary();
        await AppendTextAsync($"\n> SCORE SUMMARY: {scoreSummary}", useGhostEffect: true);

        GhostTerminalCinematicPlan plan = GhostTerminalCinematicDirector.GetPlan();

        foreach (string line in plan.Exit.Lines)
        {
            if (GhostTerminalNarrationHelper.TryParsePause(line, out double pauseSeconds))
            {
                await ToSignal(GetTree().CreateTimer(pauseSeconds), SceneTreeTimer.SignalName.Timeout);
                continue;
            }

            string resolved = line.Replace("{{THREAD_NAME}}", threadName, StringComparison.OrdinalIgnoreCase);
            await AppendTextAsync(resolved, useGhostEffect: true);
            await ToSignal(GetTree().CreateTimer(1.2f), SceneTreeTimer.SignalName.Timeout);
        }

        // Final transition
        await ToSignal(GetTree().CreateTimer(3.0f), SceneTreeTimer.SignalName.Timeout);

        // Store the determined thread for use in Stage 2
        // TODO: Save thread to game state/persistence system

        // Transition to Stage 2 (placeholder - will need to be updated when Stage 2 is implemented)
        GD.Print($"[Question6Continue] Determined thread: {dominantThread}");
        GD.Print($"[Question6Continue] Final scores: {gameState.GetScoreSummary()}");

        // Transition to Stage 2
        await ToSignal(GetTree().CreateTimer(5.0f), SceneTreeTimer.SignalName.Timeout);
        GetTree().ChangeSceneToFile("res://source/stages/stage_2/EchoHub.tscn");
    }
}
