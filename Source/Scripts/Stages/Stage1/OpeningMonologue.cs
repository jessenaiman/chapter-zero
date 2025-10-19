// <copyright file="OpeningMonologue.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using Godot;
using System.Threading.Tasks;

namespace OmegaSpiral.Source.Scripts.Stages.Stage1;

/// <summary>
/// Opening monologue scene for Stage 1.
/// Displays the initial narrative text and transitions to the first question.
/// </summary>
[GlobalClass]
public partial class OpeningMonologue : TerminalBase
{
    /// <inheritdoc/>
    public override async void _Ready()
    {
        base._Ready();

        // Start the opening monologue
        await RunOpeningMonologueAsync();
    }

    /// <summary>
    /// Runs the opening monologue sequence with narrative text.
    /// </summary>
    /// <returns>A task that completes when monologue finishes.</returns>
    private async Task RunOpeningMonologueAsync()
    {
        GhostTerminalCinematicPlan plan = GhostTerminalCinematicDirector.GetPlan();
        GhostTerminalNarrationBeat monologue = plan.OpeningMonologue;

        foreach (string line in monologue.Lines)
        {
            if (GhostTerminalNarrationHelper.TryParsePause(line, out double pauseSeconds))
            {
                await ToSignal(GetTree().CreateTimer(pauseSeconds), SceneTreeTimer.SignalName.Timeout);
                continue;
            }

            await AppendTextAsync(line);
            await ToSignal(GetTree().CreateTimer(1.6f), SceneTreeTimer.SignalName.Timeout);
        }

        // Brief pause, then transition to first question
        await ToSignal(GetTree().CreateTimer(2.0f), SceneTreeTimer.SignalName.Timeout);

        TransitionToScene("res://Source/Stages/Stage1/question1_name.tscn");
    }
}
