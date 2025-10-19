// <copyright file="Question3_Voice.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using Godot;
using System.Threading.Tasks;

namespace OmegaSpiral.Source.Scripts.Stages.Stage1;

/// <summary>
/// Story continuation scene that bridges the bridge choice to the secret question.
/// </summary>
[GlobalClass]
public partial class Question3Voice : TerminalBase
{
    /// <inheritdoc/>
    public override async void _Ready()
    {
        base._Ready();

        // Present the continuation beat leading into the secret
        await PlayContinuationAsync();
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

            await AppendTextAsync(line, useGhostEffect: true);
            await ToSignal(GetTree().CreateTimer(1.6f), SceneTreeTimer.SignalName.Timeout);
        }

        // Brief pause before transition
        await ToSignal(GetTree().CreateTimer(2.0f), SceneTreeTimer.SignalName.Timeout);

        // Transition to secret question
        TransitionToScene("res://Source/Stages/Stage1/question_4_name.tscn");
    }
}
