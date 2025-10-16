// <copyright file="NameInputSequence.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

namespace OmegaSpiral.Source.Scripts.Field.Narrative.Sequences;

using System;
using System.Threading.Tasks;
using Godot;
using OmegaSpiral.Source.Scripts.Field.Narrative;
using OmegaSpiral.Source.Scripts.Common;
using OmegaSpiral.Source.Scripts.Infrastructure;

/// <summary>
/// Sequence for collecting the player's name.
/// Displays a prompt and accepts text input, then transitions to the secret question.
/// </summary>
[GlobalClass]
public partial class NameInputSequence : NarrativeSequence
{
    /// <summary>
    /// Scene data loaded from the current thread configuration.
    /// </summary>
    private NarrativeSceneData? sceneData;

    /// <summary>
    /// Text input field for name entry.
    /// </summary>
    private LineEdit? nameInput;

    /// <summary>
    /// Button to confirm name entry.
    /// </summary>
    private Button? confirmButton;

    /// <summary>
    /// Label displaying the name prompt.
    /// </summary>
    private Label? promptLabel;

    /// <summary>
    /// The player's entered name.
    /// </summary>
    private string playerName = string.Empty;

    /// <inheritdoc/>
    protected override void OnSequenceReady()
    {
        base.OnSequenceReady();

        // Get scene nodes
        this.promptLabel = this.GetNode<Label>("PromptLabel");
        this.nameInput = this.GetNode<LineEdit>("NameInput");
        this.confirmButton = this.GetNode<Button>("ConfirmButton");

        // Connect signals
        if (this.confirmButton != null)
        {
            this.confirmButton.Pressed += this.OnConfirmPressed;
        }

        if (this.nameInput != null)
        {
            this.nameInput.TextSubmitted += this.OnNameSubmitted;
        }

        // Load scene data to get the name prompt
        this.LoadSceneData();
    }

    /// <inheritdoc/>
    public override async Task PlayAsync()
    {
        // Fade in
        await this.FadeFromBlackAsync(1.5f).ConfigureAwait(false);

        // Display the name prompt
        await this.DisplayNamePromptAsync().ConfigureAwait(false);

        // Wait for user input (handled by signals)
    }

    /// <summary>
    /// Loads scene data from the current thread to get the name prompt.
    /// </summary>
    private void LoadSceneData()
    {
        try
        {
            // Get the current thread from the director or game state
            // For now, we'll use a default path - this should be passed from the director
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
    /// This should ideally be passed from the director based on the player's choice.
    /// </summary>
    private static string GetCurrentThreadPath()
    {
        // TODO: Get this from director state or game state
        // For now, default to hero thread
        return "res://Source/Data/stages/ghost-terminal/hero.yaml";
    }

    /// <summary>
    /// Displays the name prompt with typewriter effect.
    /// </summary>
    private async Task DisplayNamePromptAsync()
    {
        string prompt = this.sceneData?.NamePrompt ?? "What name should the terminal record?";

        if (this.promptLabel != null)
        {
            await this.DisplayTextWithTypewriterAsync(prompt).ConfigureAwait(false);
        }

        // Enable input
        if (this.nameInput != null)
        {
            this.nameInput.Editable = true;
            this.nameInput.GrabFocus();
        }

        if (this.confirmButton != null)
        {
            this.confirmButton.Disabled = false;
        }
    }

    /// <summary>
    /// Handles the confirm button press.
    /// </summary>
    private void OnConfirmPressed()
    {
        if (this.nameInput != null && !string.IsNullOrWhiteSpace(this.nameInput.Text))
        {
            this.playerName = this.nameInput.Text.Trim();
            this.OnNameEntered();
        }
    }

    /// <summary>
    /// Handles name submission via Enter key.
    /// </summary>
    /// <param name="newText">The submitted text.</param>
    private void OnNameSubmitted(string newText)
    {
        if (!string.IsNullOrWhiteSpace(newText))
        {
            this.playerName = newText.Trim();
            this.OnNameEntered();
        }
    }

    /// <summary>
    /// Called when a valid name has been entered.
    /// Stores the name and transitions to the secret question.
    /// </summary>
    private void OnNameEntered()
    {
        GD.Print($"[{this.SequenceId}] Player name entered: {this.playerName}");

        // TODO: Store the name in game state or director state
        // For now, we'll transition to the secret question

        this.CompleteSequence("secret_question");
    }
}
