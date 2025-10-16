// <copyright file="OpeningSequence.cs" company="Ωmega Spiral">
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
/// Opening sequence for the Ghost Terminal stage.
/// Delegates to existing NarrativeTerminal logic for opening display and choice presentation.
/// </summary>
[GlobalClass]
public partial class OpeningSequence : NarrativeSequence
{
    /// <summary>
    /// Reference to the existing NarrativeTerminal for delegation.
    /// </summary>
    private NarrativeTerminal? narrativeTerminal;

    /// <summary>
    /// Container for thread choice buttons.
    /// </summary>
    private VBoxContainer? choiceContainer;

    /// <summary>
    /// Button for Hero thread choice.
    /// </summary>
    private Button? heroButton;

    /// <summary>
    /// Button for Shadow thread choice.
    /// </summary>
    private Button? shadowButton;

    /// <summary>
    /// Button for Ambition thread choice.
    /// </summary>
    private Button? ambitionButton;

    /// <inheritdoc/>
    protected override void OnSequenceReady()
    {
        base.OnSequenceReady();

        // Get scene nodes
        this.choiceContainer = this.GetNode<VBoxContainer>("ChoiceContainer");
        this.heroButton = this.GetNode<Button>("ChoiceContainer/HeroButton");
        this.shadowButton = this.GetNode<Button>("ChoiceContainer/ShadowButton");
        this.ambitionButton = this.GetNode<Button>("ChoiceContainer/AmbitionButton");

        // Connect button signals
        if (this.heroButton != null)
        {
            this.heroButton.Pressed += () => this.OnThreadChoiceSelected("hero");
        }

        if (this.shadowButton != null)
        {
            this.shadowButton.Pressed += () => this.OnThreadChoiceSelected("shadow");
        }

        if (this.ambitionButton != null)
        {
            this.ambitionButton.Pressed += () => this.OnThreadChoiceSelected("ambition");
        }

        // Find existing NarrativeTerminal
        this.narrativeTerminal = this.GetNodeOrNull<NarrativeTerminal>("/root/NarrativeTerminal");
        if (this.narrativeTerminal == null)
        {
            GD.PrintErr($"[{this.SequenceId}] NarrativeTerminal not found in scene tree");
        }
    }

    /// <inheritdoc/>
    public override async Task PlayAsync()
    {
        if (this.narrativeTerminal == null)
        {
            GD.PrintErr($"[{this.SequenceId}] Cannot play without NarrativeTerminal");
            this.CompleteSequence();
            return;
        }

        // Fade in
        await this.FadeFromBlackAsync(1.5f).ConfigureAwait(false);

        // Delegate opening display to existing NarrativeTerminal
        await this.DisplayOpeningUsingTerminal().ConfigureAwait(false);

        // Show choice buttons instead of text input
        this.ShowChoiceButtons();

        // Wait for user choice (handled by button signals)
    }

    /// <summary>
    /// Displays opening text using the existing NarrativeTerminal logic.
    /// </summary>
    private async Task DisplayOpeningUsingTerminal()
    {
        if (this.narrativeTerminal == null)
        {
            return;
        }

        // Access the existing DisplayOpeningAsync method via reflection or by calling it directly
        // Since it's private, we'll need to either make it public or duplicate the minimal logic

        // For now, duplicate the minimal logic from NarrativeTerminal.DisplayOpeningAsync
        var sceneData = this.GetSceneDataFromTerminal();
        if (sceneData?.OpeningLines != null)
        {
            foreach (string line in sceneData.OpeningLines)
            {
                await this.DisplayTextWithTypewriterAsync(line).ConfigureAwait(false);
                await Task.Delay(800).ConfigureAwait(false); // Brief pause between lines
            }
        }
    }

    /// <summary>
    /// Gets scene data from the terminal (temporary solution until we refactor the terminal).
    /// </summary>
    private NarrativeSceneData? GetSceneDataFromTerminal()
    {
        // This is a temporary solution. Ideally, we'd extract this logic to a shared service
        // or make NarrativeTerminal expose its sceneData publicly.

        try
        {
            string path = "res://Source/Data/stages/ghost-terminal/opening_scene.dtl";
            var configData = ConfigurationService.LoadConfiguration(path);
            return configData != null ? NarrativeSceneFactory.Create(configData) : null;
        }
        catch (Exception ex)
        {
            GD.PrintErr($"[{this.SequenceId}] Exception loading scene data: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// Shows the thread choice buttons after opening text.
    /// </summary>
    private void ShowChoiceButtons()
    {
        if (this.choiceContainer != null)
        {
            this.choiceContainer.Visible = true;
        }

        // Get choice text from scene data
        var sceneData = this.GetSceneDataFromTerminal();
        if (sceneData?.InitialChoice?.Options != null)
        {
            var options = sceneData.InitialChoice.Options;
            if (options.Count >= 3)
            {
                if (this.heroButton != null)
                {
                    this.heroButton.Text = options[0].Text ?? options[0].Id ?? "HERO";
                }

                if (this.shadowButton != null)
                {
                    this.shadowButton.Text = options[1].Text ?? options[1].Id ?? "SHADOW";
                }

                if (this.ambitionButton != null)
                {
                    this.ambitionButton.Text = options[2].Text ?? options[2].Id ?? "AMBITION";
                }
            }
        }
    }

    /// <summary>
    /// Handles thread choice selection.
    /// Updates game state and completes the sequence.
    /// </summary>
    /// <param name="threadId">The selected thread ID ("hero", "shadow", or "ambition").</param>
    private void OnThreadChoiceSelected(string threadId)
    {
        GD.Print($"[{this.SequenceId}] Thread choice selected: {threadId}");

        // Update game state with selected thread
        var gameState = this.GameState;
        if (gameState is not null)
        {
            if (Enum.TryParse(threadId, true, out DreamweaverThread thread))
            {
                gameState.DreamweaverThread = thread;
                GD.Print($"[{this.SequenceId}] Game state updated with thread: {thread}");
            }
        }

        // Hide buttons
        if (this.choiceContainer != null)
        {
            this.choiceContainer.Visible = false;
        }

        // Complete sequence with next sequence ID based on choice
        string nextSequenceId = $"thread_{threadId}";
        this.CompleteSequence(nextSequenceId);
    }
}
