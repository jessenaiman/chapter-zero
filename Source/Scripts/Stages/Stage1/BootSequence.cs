// <copyright file="BootSequence.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using Godot;
using System.Threading.Tasks;

namespace OmegaSpiral.Source.Scripts.Stages.Stage1;

/// <summary>
/// Initial boot sequence scene for Stage 1 opening.
/// Displays system initialization messages and transitions to the opening monologue.
/// </summary>
[GlobalClass]
public partial class BootSequence : TerminalBase
{
    /// <inheritdoc/>
    public override async void _Ready()
    {
        base._Ready();

        // Start boot sequence
        await RunBootSequenceAsync();
    }

    /// <summary>
    /// Runs the complete boot sequence with system messages.
    /// </summary>
    /// <returns>A task that completes when boot sequence finishes.</returns>
    private async Task RunBootSequenceAsync()
    {
        // System initialization messages
        string[] bootMessages = new[]
        {
            "> ΩMEGA SPIRAL v2.7.13",
            "> INITIALIZING NEURAL INTERFACE...",
            "> LOADING DREAMWEAVER PROTOCOLS...",
            "> ESTABLISHING CONNECTION TO THE VOID...",
            "> MEMORY FRAGMENTS DETECTED",
            "> PREPARING NARRATIVE ENGINES...",
            "> SYSTEM READY",
            "> WELCOME TO THE SPIRAL"
        };

        // Display boot messages with delays
        foreach (string message in bootMessages)
        {
            await AppendTextAsync(message);
            await ToSignal(GetTree().CreateTimer(0.8f), SceneTreeTimer.SignalName.Timeout);
        }

        // Brief pause before transition
        await ToSignal(GetTree().CreateTimer(2.0f), SceneTreeTimer.SignalName.Timeout);

        // Transition to opening monologue
        TransitionToScene("res://Source/Stages/Stage1/OpeningMonologue.tscn");
    }
}
