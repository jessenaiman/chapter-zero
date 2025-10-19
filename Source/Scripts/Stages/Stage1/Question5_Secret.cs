// <copyright file="Question5_Secret.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using Godot;
using System.Threading.Tasks;

namespace OmegaSpiral.Source.Scripts.Stages.Stage1;

/// <summary>
/// Fifth question scene: Secret keeping question.
/// Records the choice in DreamweaverScore and transitions to the final continue question.
/// </summary>
[GlobalClass]
public partial class Question5Secret : TerminalBase
{
    /// <inheritdoc/>
    public override async void _Ready()
    {
        base._Ready();

        // Present the secret question
        await PresentSecretQuestionAsync();
    }

    /// <summary>
    /// Presents the secret question and handles the choice.
    /// </summary>
    /// <returns>A task that completes when choice is made and recorded.</returns>
    private async Task PresentSecretQuestionAsync()
    {
        string question = "> ΩMEGA ASKS: CAN YOU KEEP A SECRET?";

        string[] choices = new[]
        {
            "YES - Yes, I can keep a secret",
            "NO - No, I cannot keep secrets",
            "MUTUAL - Only if you keep one for me"
        };

        string selectedChoice = await PresentChoicesAsync(question, choices);

        // Record choice in DreamweaverScore
        DreamweaverScore score = GetDreamweaverScore();

        switch (selectedChoice)
        {
            case "YES - Yes, I can keep a secret":
                score.RecordChoice("question5_secret", "YES", 2, 2, 0); // Light: 2, Shadow: 2
                break;
            case "NO - No, I cannot keep secrets":
                score.RecordChoice("question5_secret", "NO", 0, 0, 4); // Ambition: 4
                break;
            case "MUTUAL - Only if you keep one for me":
                score.RecordChoice("question5_secret", "MUTUAL", 2, 0, 2); // Light: 2, Ambition: 2
                break;
        }

        // Display response and transition
        await DisplayResponseAndTransitionAsync();
    }

    /// <summary>
    /// Displays the response to the secret choice and transitions to the next scene.
    /// </summary>
    /// <returns>A task that completes when transition begins.</returns>
    private async Task DisplayResponseAndTransitionAsync()
    {
        string[] responseLines = new[]
        {
            "Good. The secret is this: Reality is a story that forgot it was being written. And we—are the ones remembering.",
            "",
            "> THE SPIRAL IS WATCHING..."
        };

        // Display response with pauses
        foreach (string line in responseLines)
        {
            if (!string.IsNullOrEmpty(line))
            {
                await AppendTextAsync(line);
                await ToSignal(GetTree().CreateTimer(2.0f), SceneTreeTimer.SignalName.Timeout);
            }
            else
            {
                await ToSignal(GetTree().CreateTimer(1.0f), SceneTreeTimer.SignalName.Timeout);
            }
        }

        // Brief pause before transition
        await ToSignal(GetTree().CreateTimer(3.0f), SceneTreeTimer.SignalName.Timeout);

        // Transition to final continue question
        TransitionToScene("res://Source/Stages/Stage1/Question6_Continue.tscn");
    }
}
