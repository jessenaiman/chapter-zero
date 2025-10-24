// <copyright file="Question6_Continue.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using Godot;
using System;
using System.Threading.Tasks;
using OmegaSpiral.Source.Scripts;
using OmegaSpiral.Source.Scripts.Common;
using OmegaSpiral.Source.Stages.Ghost;
using OmegaSpiral.Source.UI.Terminal;

namespace OmegaSpiral.Source.Scripts.Stages.Stage1;

/// <summary>
/// Final question scene: Continue confirmation and thread determination.
/// Determines the player's Dreamweaver thread and transitions to Stage 2.
/// </summary>
[GlobalClass]
public partial class Question6Continue : GhostTerminalUI
{
    /// <inheritdoc/>
    public override async void _Ready()
    {
        base._Ready();

        // Determine and display thread
        await DetermineAndDisplayThreadAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// Determines the player's Dreamweaver thread and displays the result.
    /// </summary>
    /// <returns>A task that completes when thread is determined and displayed.</returns>
    private async Task DetermineAndDisplayThreadAsync()
    {
        string dominantThread = DetermineDominantThread();
        await ApplyThreadVisualPresetAsync(dominantThread).ConfigureAwait(false);
        await DisplayThreadDeterminationAsync(dominantThread).ConfigureAwait(false);
        await DisplayScoreSummaryAsync().ConfigureAwait(false);
        await DisplayExitLinesAsync(dominantThread).ConfigureAwait(false);
        await HandleFinalTransitionAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// Determines the dominant thread from game state.
    /// </summary>
    /// <returns>The dominant thread name.</returns>
    private string DetermineDominantThread()
    {
        GameState gameState = GetGameState();
        return gameState.GetDominantThread();
    }

    /// <summary>
    /// Applies the appropriate visual preset for the determined thread.
    /// </summary>
    /// <param name="dominantThread">The dominant thread name.</param>
    private async Task ApplyThreadVisualPresetAsync(string dominantThread)
    {
        GhostTerminalVisualPreset preset = dominantThread switch
        {
            "Light" => GhostTerminalVisualPreset.ThreadLight,
            "Shadow" => GhostTerminalVisualPreset.ThreadMischief,
            "Ambition" => GhostTerminalVisualPreset.ThreadWrath,
            "Balance" => GhostTerminalVisualPreset.ThreadBalance,
            _ => GhostTerminalVisualPreset.StableBaseline,
        };

        await ApplyVisualPresetAsync(preset).ConfigureAwait(false);
    }

    /// <summary>
    /// Displays the thread determination results with appropriate narrative.
    /// </summary>
    /// <param name="dominantThread">The dominant thread name.</param>
    private async Task DisplayThreadDeterminationAsync(string dominantThread)
    {
        string threadName = dominantThread.ToUpperInvariant();
        string[] threadLines = GetThreadLines(dominantThread);

        foreach (string line in threadLines)
        {
            if (!string.IsNullOrEmpty(line))
            {
                await AppendTextAsync(line, useGhostEffect: true).ConfigureAwait(false);
                await ToSignal(GetTree().CreateTimer(1.5f), SceneTreeTimer.SignalName.Timeout);
            }
            else
            {
                await ToSignal(GetTree().CreateTimer(0.8f), SceneTreeTimer.SignalName.Timeout);
            }
        }
    }

    /// <summary>
    /// Gets the appropriate thread determination lines for the given thread.
    /// </summary>
    /// <param name="dominantThread">The dominant thread name.</param>
    /// <returns>Array of lines to display.</returns>
    private string[] GetThreadLines(string dominantThread)
    {
        var threadLines = new Dictionary<string, string[]>
        {
            ["Light"] = GetLightThreadLines(),
            ["Shadow"] = GetShadowThreadLines(),
            ["Ambition"] = GetAmbitionThreadLines(),
            ["Balance"] = GetBalanceThreadLines()
        };

        return threadLines.GetValueOrDefault(dominantThread, GetDefaultThreadLines());
    }

    private string[] GetLightThreadLines()
    {
        return new[]
        {
            "> THREAD ANALYSIS COMPLETE",
            "> DOMINANT THREAD: LIGHT",
            "> NARRATIVE AFFINITY: Stories of conviction and moral certainty",
            "> DREAMWEAVER: The Guardian",
            "",
            "Your choices reveal a heart that seeks to protect and preserve.",
            "The Light thread calls to you—tales where one person's courage can illuminate the darkness."
        };
    }

    private string[] GetShadowThreadLines()
    {
        return new[]
        {
            "> THREAD ANALYSIS COMPLETE",
            "> DOMINANT THREAD: SHADOW",
            "> NARRATIVE AFFINITY: Stories of observation and hidden truths",
            "> DREAMWEAVER: The Watcher",
            "",
            "Your choices reveal a mind that sees what others miss.",
            "The Shadow thread calls to you—tales where patience reveals the world's secret machinery."
        };
    }

    private string[] GetAmbitionThreadLines()
    {
        return new[]
        {
            "> THREAD ANALYSIS COMPLETE",
            "> DOMINANT THREAD: AMBITION",
            "> NARRATIVE AFFINITY: Stories of transformation and self-mastery",
            "> DREAMWEAVER: The Weaver",
            "",
            "Your choices reveal a spirit that reshapes reality to its will.",
            "The Ambition thread calls to you—tales where every ending is just another beginning."
        };
    }

    private string[] GetBalanceThreadLines()
    {
        return new[]
        {
            "> THREAD ANALYSIS COMPLETE",
            "> DOMINANT THREAD: BALANCE",
            "> NARRATIVE AFFINITY: Stories of complexity and philosophical depth",
            "> DREAMWEAVER: The Harmonizer",
            "",
            "Your choices reveal a soul that embraces the beautiful complexity of existence.",
            "The Balance thread calls to you—tales where no single path holds all the answers."
        };
    }

    private string[] GetDefaultThreadLines()
    {
        return new[]
        {
            "> THREAD ANALYSIS ERROR",
            "> UNABLE TO DETERMINE THREAD",
            "> DEFAULTING TO BALANCE"
        };
    }

    /// <summary>
    /// Displays the final score summary.
    /// </summary>
    private async Task DisplayScoreSummaryAsync()
    {
        GameState gameState = GetGameState();
        string scoreSummary = gameState.GetScoreSummary();
        await AppendTextAsync($"\n> SCORE SUMMARY: {scoreSummary}", useGhostEffect: true).ConfigureAwait(false);
    }

    /// <summary>
    /// Displays the exit lines from the cinematic plan.
    /// </summary>
    /// <param name="dominantThread">The dominant thread name for template replacement.</param>
    private async Task DisplayExitLinesAsync(string dominantThread)
    {
        string threadName = dominantThread.ToUpperInvariant();
        GhostTerminalCinematicPlan plan = GhostTerminalCinematicDirector.GetPlan();

        foreach (string line in plan.Exit.Lines)
        {
            if (GhostTerminalNarrationHelper.TryParsePause(line, out double pauseSeconds))
            {
                await ToSignal(GetTree().CreateTimer(pauseSeconds), SceneTreeTimer.SignalName.Timeout);
                continue;
            }

            string resolved = line.Replace("{{THREAD_NAME}}", threadName, StringComparison.OrdinalIgnoreCase);
            await AppendTextAsync(resolved, useGhostEffect: true).ConfigureAwait(false);
            await ToSignal(GetTree().CreateTimer(1.2f), SceneTreeTimer.SignalName.Timeout);
        }
    }

    /// <summary>
    /// Handles the final transition to Stage 2.
    /// </summary>
    private async Task HandleFinalTransitionAsync()
    {
        // Final transition
    await ToSignal(GetTree().CreateTimer(3.0f), SceneTreeTimer.SignalName.Timeout);

        // Store the determined thread for use in Stage 2
        // TODO: Save thread to game state/persistence system

        string dominantThread = DetermineDominantThread();
        GD.Print($"[Question6Continue] Determined thread: {dominantThread}");

        GameState gameState = GetGameState();
        GD.Print($"[Question6Continue] Final scores: {gameState.GetScoreSummary()}");
    await ToSignal(GetTree().CreateTimer(5.0f), SceneTreeTimer.SignalName.Timeout);


        // Transition to Stage 2 using the SceneManager autoload when available.
        SceneManager? sceneManager = GetTree().Root.GetNodeOrNull<SceneManager>("SceneManager");
        if (sceneManager != null)
        {
            sceneManager.TransitionToScene("Stage2Nethack", showLoadingScreen: false);
            return;
        }

        GD.PushWarning("[Question6Continue] SceneManager not found; falling back to direct scene change.");
        GetTree().ChangeSceneToFile("res://source/stages/echo_hub/echo_hub_main.tscn");
    }
}
