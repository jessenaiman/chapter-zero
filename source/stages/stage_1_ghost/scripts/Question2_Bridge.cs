// <copyright file="Question2_Bridge.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using Godot;
using System.Collections.Generic;
using System.Threading.Tasks;
using OmegaSpiral.Source.Scripts.Common;
using OmegaSpiral.Source.Stages.Ghost;

namespace OmegaSpiral.Source.Scripts.Stages.Stage1;

/// <summary>
/// Second question scene: "What bridges the gap between thought and action?"
/// </summary>
[GlobalClass]
public partial class Question2Bridge : GhostTerminalUI
{
    /// <inheritdoc/>
    public override async void _Ready()
    {
        base._Ready();

        // Present the bridge question
        await PresentBridgeQuestionAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// Presents the bridge question and handles the choice.
    /// </summary>
    /// <returns>A task that completes when choice is made and recorded.</returns>
    private async Task PresentBridgeQuestionAsync()
    {
        GhostTerminalCinematicPlan plan = GhostTerminalCinematicDirector.GetPlan();
        GhostTerminalNarrationBeat introBeat = plan.StoryIntro;

        foreach (string line in introBeat.Lines)
        {
            if (GhostTerminalNarrationHelper.TryParsePause(line, out double pauseSeconds))
            {
                await ToSignal(GetTree().CreateTimer(pauseSeconds), SceneTreeTimer.SignalName.Timeout);
                continue;
            }

            await AppendTextAsync(line, useGhostEffect: true).ConfigureAwait(false);
            await ToSignal(GetTree().CreateTimer(1.2f), SceneTreeTimer.SignalName.Timeout);
        }

        GhostTerminalChoiceBeat choiceBeat = plan.StoryChoice;
        GhostTerminalChoicePrompt prompt = choiceBeat.Prompt;

        string[] optionTexts = prompt.Options.Select(option => option.Text).ToArray();

        string selectedText = await PresentChoicesAsync(prompt.Prompt, optionTexts).ConfigureAwait(false);
        GhostTerminalChoiceOption selectedOption = prompt.Options.First(option => option.Text == selectedText);

        RecordChoice("question2_bridge", selectedOption);

        if (!string.IsNullOrWhiteSpace(selectedOption.Response))
        {
            await AppendTextAsync(selectedOption.Response, useGhostEffect: true).ConfigureAwait(false);
        }
    await ToSignal(GetTree().CreateTimer(1.4f), SceneTreeTimer.SignalName.Timeout);

        // Transition to continuation scene
        TransitionToScene("res://source/stages/stage_1/scenes/question_3_voice.tscn");
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
