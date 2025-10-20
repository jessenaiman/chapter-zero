// <copyright file="Question1_Name.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using Godot;
using System.Linq;
using System.Threading.Tasks;
using OmegaSpiral.Source.Scripts.Common;

namespace OmegaSpiral.Source.Scripts.Stages.Stage1;

/// <summary>
/// First question scene: Identity philosophy selection sourced from the cinematic data.
/// Records the choice in <see cref="GameState"/> and transitions to the bridge question.
/// </summary>
[GlobalClass]
public partial class Question1Name : TerminalBase
{
    /// <inheritdoc/>
    public override async void _Ready()
    {
        base._Ready();

        // Present the identity question
        await PresentIdentityQuestionAsync();
    }

    /// <summary>
    /// Presents the identity-focused question and handles the choice.
    /// </summary>
    /// <returns>A task that completes when choice is made and recorded.</returns>
    private async Task PresentIdentityQuestionAsync()
    {
        GhostTerminalCinematicPlan plan = GhostTerminalCinematicDirector.GetPlan();
        GhostTerminalChoiceBeat firstChoice = plan.FirstChoice;

        foreach (string line in firstChoice.SetupLines)
        {
            if (GhostTerminalNarrationHelper.TryParsePause(line, out double pauseSeconds))
            {
                await ToSignal(GetTree().CreateTimer(pauseSeconds), SceneTreeTimer.SignalName.Timeout);
                continue;
            }

            await AppendTextAsync(line, useGhostEffect: true);
            await ToSignal(GetTree().CreateTimer(1.0f), SceneTreeTimer.SignalName.Timeout);
        }

        GhostTerminalChoicePrompt prompt = firstChoice.Prompt;

        if (!string.IsNullOrWhiteSpace(prompt.Context))
        {
            await AppendTextAsync(prompt.Context, useGhostEffect: true);
            await ToSignal(GetTree().CreateTimer(1.0f), SceneTreeTimer.SignalName.Timeout);
        }

        string[] optionTexts = prompt.Options.Select(option => option.Text).ToArray();

        string selectedText = await PresentChoicesAsync(prompt.Prompt, optionTexts, ghostPrompt: true);
        GhostTerminalChoiceOption selectedOption = prompt.Options.First(option => option.Text == selectedText);

        RecordChoice("question1_name", selectedOption);

        await ToSignal(GetTree().CreateTimer(1.2f), SceneTreeTimer.SignalName.Timeout);

        // Transition to bridge question
        TransitionToScene("res://source/stages/ghost/scenes/question_2_bridge.tscn");
    }

    private void RecordChoice(string questionId, GhostTerminalChoiceOption option)
    {
        GameState gameState = GetGameState();

        gameState.RecordChoice(
            questionId,
            option.Text,
            option.Scores.Light,
            option.Scores.Shadow,
            option.Scores.Ambition);
    }
}
