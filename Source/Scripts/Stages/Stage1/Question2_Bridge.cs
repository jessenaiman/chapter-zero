// <copyright file="Question2_Bridge.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using Godot;
using System.Threading.Tasks;

namespace OmegaSpiral.Source.Scripts.Stages.Stage1;

/// <summary>
/// Second question scene: Bridge knowledge question.
/// Records the choice in DreamweaverScore and transitions to the voice question.
/// </summary>
[GlobalClass]
public partial class Question2Bridge : TerminalBase
{
    /// <inheritdoc/>
    public override async void _Ready()
    {
        base._Ready();

        // Present the bridge question
        await PresentBridgeQuestionAsync();
    }

    /// <summary>
    /// Presents the bridge question and handles the choice.
    /// </summary>
    /// <returns>A task that completes when choice is made and recorded.</returns>
    private async Task PresentBridgeQuestionAsync()
    {
        string question = "> WHAT DID THE CHILD KNOW?";

        string[] choices = new[]
        {
            "BRIDGE APPEARS - The bridge appears only when you stop believing in it.",
            "KEY WITHIN - The key wasn't for the bridge—it was for the lock inside them."
        };

        string selectedChoice = await PresentChoicesAsync(question, choices);

        // Record choice in DreamweaverScore
        DreamweaverScore score = GetDreamweaverScore();

        switch (selectedChoice)
        {
            case "BRIDGE APPEARS - The bridge appears only when you stop believing in it.":
                score.RecordChoice("question2_bridge", "BRIDGE APPEARS", 0, 3, 0); // Shadow: 3
                break;
            case "KEY WITHIN - The key wasn't for the bridge—it was for the lock inside them.":
                score.RecordChoice("question2_bridge", "KEY WITHIN", 0, 0, 3); // Ambition: 3
                break;
        }

        // Display response and transition
        await DisplayResponseAndTransitionAsync();
    }

    /// <summary>
    /// Displays the response to the bridge choice and transitions to the next scene.
    /// </summary>
    /// <returns>A task that completes when transition begins.</returns>
    private async Task DisplayResponseAndTransitionAsync()
    {
        string[] responseLines = new[]
        {
            "Ah. Yes. That's right.",
            "And so the child stepped forward—not onto stone, but onto possibility.",
            "The bridge formed beneath their feet, one plank at a time, woven from every 'what if' they'd ever whispered.",
            "But then… a voice called from below."
        };

        // Display response with pauses
        foreach (string line in responseLines)
        {
            await AppendTextAsync(line);
            await ToSignal(GetTree().CreateTimer(1.8f), SceneTreeTimer.SignalName.Timeout);
        }

        // Brief pause before transition
        await ToSignal(GetTree().CreateTimer(2.0f), SceneTreeTimer.SignalName.Timeout);

        // Transition to voice question
        TransitionToScene("res://Source/Stages/Stage1/Question3_Voice.tscn");
    }
}
