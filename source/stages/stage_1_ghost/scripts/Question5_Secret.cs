// <copyright file="Question5_Secret.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using Godot;
using System.Linq;
using System.Threading.Tasks;
using OmegaSpiral.Source.Scripts.Common;
using OmegaSpiral.Source.Stages.Ghost;
using OmegaSpiral.Source.UI.Terminal;

namespace OmegaSpiral.Source.Scripts.Stages.Stage1;

/// <summary>
/// Fifth question scene: Secret keeping question.
/// Records the choice in DreamweaverScore and transitions to the naming question.
/// </summary>
[GlobalClass]
public partial class Question5Secret : GhostTerminalUI
{
    /// <inheritdoc/>
    public override async void _Ready()
    {
        base._Ready();

        // Present the secret question
        await PresentSecretQuestionAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// Presents the secret question and handles the choice.
    /// </summary>
    /// <returns>A task that completes when choice is made and recorded.</returns>
    private async Task PresentSecretQuestionAsync()
    {
        GhostTerminalCinematicPlan plan = GhostTerminalCinematicDirector.GetPlan();

        foreach (string line in plan.SecretSetup.Lines)
        {
            if (GhostTerminalNarrationHelper.TryParsePause(line, out double pauseSeconds))
            {
                await ToSignal(GetTree().CreateTimer(pauseSeconds), SceneTreeTimer.SignalName.Timeout);
            }
            else
            {
                await AppendTextAsync(line, useGhostEffect: true);
                await ToSignal(GetTree().CreateTimer(1.2f), SceneTreeTimer.SignalName.Timeout);
            }
        }

        GhostTerminalSecretChoiceBeat secretBeat = plan.SecretChoice;
        GhostTerminalChoicePrompt prompt = secretBeat.Prompt;

        string[] optionTexts = prompt.Options.Select(option => option.Text).ToArray();
        string selectedText = await PresentChoicesAsync(prompt.Prompt, optionTexts, ghostPrompt: true).ConfigureAwait(false);
        GhostTerminalChoiceOption selectedOption = prompt.Options.First(option => option.Text == selectedText);

        RecordChoice("secret_question", selectedOption);

        if (!string.IsNullOrWhiteSpace(selectedOption.Response))
        {
            await AppendTextAsync(selectedOption.Response, useGhostEffect: true);
            await ToSignal(GetTree().CreateTimer(1.4f), SceneTreeTimer.SignalName.Timeout);
        }

        await PlaySecretRevealAsync(secretBeat.Reveal);

        await ToSignal(GetTree().CreateTimer(1.8f), SceneTreeTimer.SignalName.Timeout);
        TransitionToScene("res://source/stages/stage_1/scenes/question_4_name.tscn");
    }

    private async Task PlaySecretRevealAsync(GhostTerminalSecretRevealPlan reveal)
    {
        ApplyVisualPreset(GhostTerminalVisualPreset.SecretReveal);

        foreach (string line in reveal.Lines)
        {
            if (GhostTerminalNarrationHelper.TryParsePause(line, out double pauseSeconds))
            {
                await ToSignal(GetTree().CreateTimer(pauseSeconds), SceneTreeTimer.SignalName.Timeout);
                continue;
            }

            await AppendTextAsync(line, useGhostEffect: true, charDelaySeconds: 0.04f);
            await ToSignal(GetTree().CreateTimer(1.0f), SceneTreeTimer.SignalName.Timeout);
        }

        if (!string.IsNullOrWhiteSpace(reveal.JournalEntry))
        {
            GameState gameState = GetGameState();
            if (!gameState.Shards.Contains(reveal.JournalEntry))
            {
                gameState.Shards.Add(reveal.JournalEntry);
            }
        }
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
