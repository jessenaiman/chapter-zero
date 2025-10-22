// <copyright file="Question3_Voice.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using Godot;
using System.Threading.Tasks;
using OmegaSpiral.Source.Scripts.Common;
using OmegaSpiral.Source.Stages.Ghost;

namespace OmegaSpiral.Source.Scripts.Stages.Stage1;

/// <summary>
/// Third question scene: asking about voice preferences.
/// </summary>
[GlobalClass]
public partial class Question3Voice : GhostTerminalUI
{
    /// <inheritdoc/>
    public override async void _Ready()
    {
        base._Ready();

        // Present the continuation beat leading into the secret
        await PlayContinuationAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// Presents the continuation of the bridge story and transitions to the secret question.
    /// </summary>
    /// <returns>A task that completes when the continuation finishes.</returns>
    private async Task PlayContinuationAsync()
    {
        GhostTerminalCinematicPlan plan = GhostTerminalCinematicDirector.GetPlan();
        GhostTerminalNarrationBeat continuation = plan.StoryContinuation;

        foreach (string line in continuation.Lines)
        {
            if (GhostTerminalNarrationHelper.TryParsePause(line, out double pauseSeconds))
            {
                await ToSignal(GetTree().CreateTimer(pauseSeconds), SceneTreeTimer.SignalName.Timeout);
                continue;
            }

            await AppendTextAsync(line, useGhostEffect: true).ConfigureAwait(false);
            await ToSignal(GetTree().CreateTimer(1.6f), SceneTreeTimer.SignalName.Timeout);
        }

        // Brief pause before transition
        await ToSignal(GetTree().CreateTimer(2.0f), SceneTreeTimer.SignalName.Timeout);

        // Transition to secret question (legacy scene numbering keeps file name)
        TransitionToScene("res://source/stages/ghost/scenes/question_5_secret.tscn");
    }
}
