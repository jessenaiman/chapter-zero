// <copyright file="Question3_Voice.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using Godot;
using System.Threading.Tasks;

namespace OmegaSpiral.Source.Scripts.Stages.Stage1;

/// <summary>
/// Third question scene: Voice from below question.
/// Records the choice in DreamweaverScore and transitions to name input.
/// </summary>
[GlobalClass]
public partial class Question3Voice : TerminalBase
{
    /// <inheritdoc/>
    public override async void _Ready()
    {
        base._Ready();

        // Present the voice question
        await PresentVoiceQuestionAsync();
    }

    /// <summary>
    /// Presents the voice question and handles the choice.
    /// </summary>
    /// <returns>A task that completes when choice is made and recorded.</returns>
    private async Task PresentVoiceQuestionAsync()
    {
        string question = "> WHAT DID THE VOICE SAY?";

        string[] choices = new[]
        {
            "DON'T BELONG - You don't belong here.",
            "WAITING - I've been waiting for you."
        };

        string selectedChoice = await PresentChoicesAsync(question, choices);

        // Record choice in DreamweaverScore
        DreamweaverScore score = GetDreamweaverScore();

        switch (selectedChoice)
        {
            case "DON'T BELONG - You don't belong here.":
                score.RecordChoice("question3_voice", "DON'T BELONG", 0, 3, 0); // Shadow: 3
                break;
            case "WAITING - I've been waiting for you.":
                score.RecordChoice("question3_voice", "WAITING", 3, 0, 0); // Light: 3
                break;
        }

        // Display response and transition
        await DisplayResponseAndTransitionAsync();
    }

    /// <summary>
    /// Displays the response to the voice choice and transitions to the next scene.
    /// </summary>
    /// <returns>A task that completes when transition begins.</returns>
    private async Task DisplayResponseAndTransitionAsync()
    {
        string[] responseLines = new[]
        {
            "…I see. That changes everything.",
            "And now—here you are. Not in the story. But at the place where the story begins again."
        };

        // Display response with pauses
        foreach (string line in responseLines)
        {
            await AppendTextAsync(line);
            await ToSignal(GetTree().CreateTimer(2.0f), SceneTreeTimer.SignalName.Timeout);
        }

        // Brief pause before transition
        await ToSignal(GetTree().CreateTimer(2.0f), SceneTreeTimer.SignalName.Timeout);

        // Transition to name input
        TransitionToScene("res://Source/Stages/Stage1/Question4_Name.tscn");
    }
}
