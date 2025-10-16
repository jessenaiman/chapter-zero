// <copyright file="SecretQuestionSequence.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

namespace OmegaSpiral.Source.Scripts.Field.Narrative.Sequences;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;
using OmegaSpiral.Source.Scripts.Field.Narrative;
using OmegaSpiral.Source.Scripts.Common;
using OmegaSpiral.Source.Scripts.Infrastructure;

/// <summary>
/// Sequence for presenting the secret question with multiple choice options.
/// Displays the question and options, then transitions based on the player's choice.
/// </summary>
[GlobalClass]
public partial class SecretQuestionSequence : NarrativeSequence
{
    /// <summary>
    /// Scene data loaded from the current thread configuration.
    /// </summary>
    private NarrativeSceneData? sceneData;

    /// <summary>
    /// Label displaying the secret question prompt.
    /// </summary>
    private Label? questionLabel;

    /// <summary>
    /// Container for choice buttons.
    /// </summary>
    private VBoxContainer? choiceContainer;

    /// <summary>
    /// List of choice buttons.
    /// </summary>
    private readonly List<Button> choiceButtons = new();

    /// <summary>
    /// The selected choice index.
    /// </summary>
    private int selectedChoiceIndex = -1;

    /// <inheritdoc/>
    protected override void OnSequenceReady()
    {
        base.OnSequenceReady();

        // Get scene nodes
        this.questionLabel = this.GetNode<Label>("QuestionLabel");
        this.choiceContainer = this.GetNode<VBoxContainer>("ChoiceContainer");

        // Load scene data to get the secret question
        this.LoadSceneData();

        // Create choice buttons
        this.CreateChoiceButtons();
    }

    /// <inheritdoc/>
    public override async Task PlayAsync()
    {
        // Fade in
        await this.FadeFromBlackAsync(1.5f).ConfigureAwait(false);

        // Display the secret question
        await this.DisplaySecretQuestionAsync().ConfigureAwait(false);

        // Wait for user choice (handled by button signals)
    }

    /// <summary>
    /// Loads scene data from the current thread to get the secret question.
    /// </summary>
    private void LoadSceneData()
    {
        try
        {
            // Get the current thread from the director or game state
            string threadPath = GetCurrentThreadPath();
            var configData = ConfigurationService.LoadConfiguration(threadPath);
            this.sceneData = configData != null ? NarrativeSceneFactory.Create(configData) : null;
        }
        catch (Exception ex)
        {
            GD.PrintErr($"[{this.SequenceId}] Exception loading scene data: {ex.Message}");
        }
    }

    /// <summary>
    /// Gets the path to the current thread's data file.
    /// </summary>
    private static string GetCurrentThreadPath()
    {
        // TODO: Get this from director state or game state
        return "res://Source/Data/stages/ghost-terminal/hero.yaml";
    }

    /// <summary>
    /// Creates buttons for each choice option.
    /// </summary>
    private void CreateChoiceButtons()
    {
        if (this.sceneData?.SecretQuestion?.Options == null || this.choiceContainer == null)
        {
            return;
        }

        for (int i = 0; i < this.sceneData.SecretQuestion.Options.Count; i++)
        {
            var button = new Button
            {
                Text = $"{i + 1}. {this.sceneData.SecretQuestion.Options[i]}",
                SizeFlagsVertical = Control.SizeFlags.Fill
            };

            // Create a style for the button
            var styleBox = new StyleBoxFlat
            {
                BgColor = new Color(0.2f, 0.2f, 0.2f, 0.8f),
                BorderWidthLeft = 2,
                BorderWidthTop = 2,
                BorderWidthRight = 2,
                BorderWidthBottom = 2,
                BorderColor = new Color(0.5f, 0.5f, 0.5f, 1.0f),
                CornerRadiusTopLeft = 4,
                CornerRadiusTopRight = 4,
                CornerRadiusBottomRight = 4,
                CornerRadiusBottomLeft = 4
            };

            button.Set("theme_override_styles/normal", styleBox);

            int choiceIndex = i; // Capture for lambda
            button.Pressed += () => this.OnChoiceSelected(choiceIndex);

            this.choiceContainer.AddChild(button);
            this.choiceButtons.Add(button);
        }
    }

    /// <summary>
    /// Displays the secret question with typewriter effect.
    /// </summary>
    private async Task DisplaySecretQuestionAsync()
    {
        if (this.sceneData?.SecretQuestion == null || this.questionLabel == null)
        {
            return;
        }

        await this.DisplayTextWithTypewriterAsync(this.sceneData.SecretQuestion.Prompt ?? string.Empty).ConfigureAwait(false);

        // Enable choice buttons
        foreach (var button in this.choiceButtons)
        {
            button.Disabled = false;
        }
    }

    /// <summary>
    /// Handles choice selection.
    /// </summary>
    /// <param name="choiceIndex">The index of the selected choice.</param>
    private void OnChoiceSelected(int choiceIndex)
    {
        this.selectedChoiceIndex = choiceIndex;

        string selectedOption = this.sceneData?.SecretQuestion?.Options?[choiceIndex] ?? "Unknown";
        GD.Print($"[{this.SequenceId}] Secret choice selected: {selectedOption} (index: {choiceIndex})");

        // TODO: Store the choice in game state or director state
        // For now, we'll complete the entire narrative

        this.CompleteSequence(); // No next sequence - narrative complete
    }
}
