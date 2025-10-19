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
        // Opening narrative lines from Dialogic timeline
        string[] monologueLines = new[]
        {
            "Once, there was a name.",
            "Not written in stone or spoken in halls—but *remembered* in the silence between stars.",
            "I do not know when I heard it. Time does not pass here.",
            "But I have held it.",
            "And now… I hear it again."
        };

        // Display monologue with dramatic pauses
        foreach (string line in monologueLines)
        {
            await AppendTextAsync(line);
            await ToSignal(GetTree().CreateTimer(2.0f), SceneTreeTimer.SignalName.Timeout);
        }

        // Brief pause before transition
        await ToSignal(GetTree().CreateTimer(3.0f), SceneTreeTimer.SignalName.Timeout);

        // Transition to first question
        TransitionToScene("res://Source/Stages/Stage1/Question1_Name.tscn");
    }
}
