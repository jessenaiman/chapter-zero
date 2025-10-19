// <copyright file="Question4_Name.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using Godot;
using System.Threading.Tasks;

namespace OmegaSpiral.Source.Scripts.Stages.Stage1;

/// <summary>
/// Fourth question scene: Player name input.
/// Stores the player name and transitions to the secret question.
/// </summary>
[GlobalClass]
public partial class Question4Name : TerminalBase
{
    /// <summary>
    /// The player's entered name.
    /// </summary>
    public static string PlayerName { get; private set; } = string.Empty;

    /// <inheritdoc/>
    public override async void _Ready()
    {
        base._Ready();

        // Get player name input
        await GetPlayerNameAsync();
    }

    /// <summary>
    /// Prompts for and stores the player's name.
    /// </summary>
    /// <returns>A task that completes when name is entered.</returns>
    private async Task GetPlayerNameAsync()
    {
        string prompt = "> WHAT IS YOUR NAME?";

        PlayerName = await GetTextInputAsync(prompt, "Enter your name...");

        // Display response and transition
        await DisplayResponseAndTransitionAsync();
    }

    /// <summary>
    /// Displays the response to the name input and transitions to the next scene.
    /// </summary>
    /// <returns>A task that completes when transition begins.</returns>
    private async Task DisplayResponseAndTransitionAsync()
    {
        string[] responseLines = new[]
        {
            $"Of course. {PlayerName}. I didn't hear it in a book. I heard it… in a voice that sounded like mine.",
            "Are you sure you're the reader? Or am I the one playing you?"
        };

        // Display response with pauses
        foreach (string line in responseLines)
        {
            await AppendTextAsync(line);
            await ToSignal(GetTree().CreateTimer(2.0f), SceneTreeTimer.SignalName.Timeout);
        }

        // Brief pause before transition
        await ToSignal(GetTree().CreateTimer(2.0f), SceneTreeTimer.SignalName.Timeout);

        // Transition to secret question
        TransitionToScene("res://Source/Stages/Stage1/Question5_Secret.tscn");
    }
}
