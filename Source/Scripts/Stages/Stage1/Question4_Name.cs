// <copyright file="Question4_Name.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using Godot;
using System.Linq;
using System.Threading.Tasks;
using OmegaSpiral.Source.Scripts.Common;

namespace OmegaSpiral.Source.Scripts.Stages.Stage1;

/// <summary>
/// Fourth question scene: Omega naming choice derived from narrative data.
/// </summary>
[GlobalClass]
public partial class Question4Name : TerminalBase
{
    /// <inheritdoc/>
    public override async void _Ready()
    {
        base._Ready();

        // Present the naming question
        await PresentNameQuestionAsync();
    }

    /// <summary>
    /// Presents Omega's naming question and records the chosen dreamweaver alignment.
    /// </summary>
    /// <returns>A task that completes when the choice is made.</returns>
    private async Task PresentNameQuestionAsync()
    {
        GhostTerminalCinematicPlan plan = GhostTerminalCinematicDirector.GetPlan();
        GhostTerminalNarrationBeat setupBeat = plan.NameSetup;

        foreach (string line in setupBeat.Lines)
        {
            if (GhostTerminalNarrationHelper.TryParsePause(line, out double pauseSeconds))
            {
                await ToSignal(GetTree().CreateTimer(pauseSeconds), SceneTreeTimer.SignalName.Timeout);
                continue;
            }

            await AppendTextAsync(line, useGhostEffect: true);
            await ToSignal(GetTree().CreateTimer(1.2f), SceneTreeTimer.SignalName.Timeout);
        }

        GhostTerminalChoiceBeat nameBeat = plan.NameChoice;
        GhostTerminalChoicePrompt prompt = nameBeat.Prompt;
        string[] optionTexts = prompt.Options.Select(option => option.Text).ToArray();

        string selectedText = await PresentChoicesAsync(prompt.Prompt, optionTexts, ghostPrompt: true);
        GhostTerminalChoiceOption selectedOption = prompt.Options.First(option => option.Text == selectedText);

        RecordChoice("question4_name", selectedOption);

        await ToSignal(GetTree().CreateTimer(1.4f), SceneTreeTimer.SignalName.Timeout);

        // Transition to final continue scene
        TransitionToScene("res://Source/Stages/Stage1/question5_secret.tscn");
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
