// <copyright file="Question1_Name.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using Godot;
using System.Threading.Tasks;

namespace OmegaSpiral.Source.Scripts.Stages.Stage1;

/// <summary>
/// First question scene: Story type selection (HERO/SHADOW/AMBITION).
/// Records the choice in DreamweaverScore and transitions to the bridge question.
/// </summary>
[GlobalClass]
public partial class Question1Name : TerminalBase
{
    /// <inheritdoc/>
    public override async void _Ready()
    {
        base._Ready();

        // Present the story type question
        await PresentStoryTypeQuestionAsync();
    }

    /// <summary>
    /// Presents the story type question and handles the choice.
    /// </summary>
    /// <returns>A task that completes when choice is made and recorded.</returns>
    private async Task PresentStoryTypeQuestionAsync()
    {
        string question = "> QUERY: IF YOU COULD LIVE INSIDE ONE KIND OF STORY, WHICH WOULD IT BE?";

        string[] choices = new[]
        {
            "[ HERO ] - A tale where one choice can unmake a world",
            "[ SHADOW ] - A tale that hides its truth until you bleed for it",
            "[ AMBITION ] - A tale that changes every time you look away"
        };

        string selectedChoice = await PresentChoicesAsync(question, choices);

        // Record choice in DreamweaverScore
        DreamweaverScore score = GetDreamweaverScore();

        switch (selectedChoice)
        {
            case "[ HERO ] - A tale where one choice can unmake a world":
                score.RecordChoice("question1_name", "HERO", 2, 0, 0); // Light: 2
                break;
            case "[ SHADOW ] - A tale that hides its truth until you bleed for it":
                score.RecordChoice("question1_name", "SHADOW", 0, 2, 0); // Shadow: 2
                break;
            case "[ AMBITION ] - A tale that changes every time you look away":
                score.RecordChoice("question1_name", "AMBITION", 0, 0, 2); // Ambition: 2
                break;
        }

        // Display response and transition
        await DisplayResponseAndTransitionAsync(selectedChoice);
    }

    /// <summary>
    /// Displays the response to the choice and transitions to the next scene.
    /// </summary>
    /// <param name="selectedChoice">The choice that was selected.</param>
    /// <returns>A task that completes when transition begins.</returns>
    private async Task DisplayResponseAndTransitionAsync(string selectedChoice)
    {
        // Extract the story type from the choice
        string storyType = selectedChoice switch
        {
            "[ HERO ] - A tale where one choice can unmake a world" => "HERO",
            "[ SHADOW ] - A tale that hides its truth until you bleed for it" => "SHADOW",
            "[ AMBITION ] - A tale that changes every time you look away" => "AMBITION",
            _ => "UNKNOWN"
        };

        string[] responseLines = new[]
        {
            $"You chose **{storyType}**.",
            "That tells me something about you.",
            "Or perhaps… about me.",
            "Let me try to tell it back to you.",
            "In a city built on broken promises, a child stood at the edge of a bridge that led nowhere.",
            "They held a key made of glass—and everyone warned them: *\"Don't cross. The bridge isn't real.\"*",
            "But the child knew something no one else did…"
        };

        // Display response with pauses
        foreach (string line in responseLines)
        {
            await AppendTextAsync(line);
            await ToSignal(GetTree().CreateTimer(1.5f), SceneTreeTimer.SignalName.Timeout);
        }

        // Brief pause before transition
        await ToSignal(GetTree().CreateTimer(2.0f), SceneTreeTimer.SignalName.Timeout);

        // Transition to bridge question
        TransitionToScene("res://Source/Stages/Stage1/Question2_Bridge.tscn");
    }
}
