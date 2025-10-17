// <copyright file="OpeningSequence.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;
using OmegaSpiral.Source.Scripts.Common;
using OmegaSpiral.Source.Scripts.Common.ScreenTransitions;
using OmegaSpiral.Source.Scripts.Field.Narrative;
using OmegaSpiral.Source.Scripts.Field.Narrative.Audio;
using OmegaSpiral.Source.Scripts.Infrastructure;

namespace OmegaSpiral.Source.Scripts.Field.Narrative.Sequences;
/// <summary>
/// Opening sequence for the Ghost Terminal stage.
/// Presents the initial narrative and thread choice options with enhanced audio and visual effects.
/// </summary>
[GlobalClass]
public partial class OpeningSequence : NarrativeSequence
{
    /// <summary>
    /// Reference to the existing NarrativeTerminal for loading scene data.
    /// </summary>
    private NarrativeSceneData? sceneData;

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

    /// <summary>
    /// Narrative audio manager for the sequence.
    /// </summary>
    private NarrativeAudioManager? audioManager;

    /// <summary>
    /// Flag to prevent multiple selections.
    /// </summary>
    private bool choiceMade;

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

        // Initialize audio manager
        this.InitializeAudioManager();

        // Load scene data
        this.LoadSceneData();
    }

    /// <summary>
    /// Initializes the narrative audio manager.
    /// </summary>
    private void InitializeAudioManager()
    {
        // Create and add audio manager as a child node
        this.audioManager = new NarrativeAudioManager();
        this.AddChild(this.audioManager);
    }

    /// <inheritdoc/>
    public override async Task PlayAsync()
    {
        if (this.sceneData == null)
        {
            GD.PrintErr($"[{this.SequenceId}] Failed to load scene data");
            this.CompleteSequence();
            return;
        }

        // Fade in from black
        await this.FadeFromBlackAsync(1.0f).ConfigureAwait(false);

        // Display opening lines with enhanced typewriter effect
        await this.DisplayOpeningLinesAsync().ConfigureAwait(false);

        // Present thread choice options
        await this.PresentThreadChoiceAsync().ConfigureAwait(false);

        // Wait for user choice (handled by button signals)
    }

    /// <summary>
    /// Loads scene data from the current thread configuration.
    /// </summary>
    private void LoadSceneData()
    {
        try
        {
            string basePath = "res://Source/Data/stages/ghost-terminal";
            string threadKey = this.GameState?.DreamweaverThread.ToString().ToLowerInvariant() ?? "hero";
            var candidates = new[] { threadKey, "hero", "shadow", "ambition" };

            foreach (string candidate in candidates)
            {
                string path = $"{basePath}/{candidate}.json";
                if (!Godot.FileAccess.FileExists(path))
                {
                    continue;
                }

                var configData = ConfigurationService.LoadConfiguration(path);
                this.sceneData = configData != null ? NarrativeSceneFactory.Create(configData) : null;
                break;
            }

            if (this.sceneData == null)
            {
                GD.PrintErr($"[{this.SequenceId}] No valid scene data found, using fallback");
                this.sceneData = new NarrativeSceneData();
            }
        }
        catch (Exception ex)
        {
            GD.PrintErr($"[{this.SequenceId}] Exception loading scene data: {ex.Message}");
        }
    }

    /// <summary>
    /// Displays the opening lines with enhanced typewriter effect and audio.
    /// </summary>
    private async Task DisplayOpeningLinesAsync()
    {
        if (this.sceneData?.OpeningLines == null)
        {
            return;
        }

        foreach (string line in this.sceneData.OpeningLines)
        {
            // Play typewriter sound during text display
            await this.DisplayTextWithEnhancedTypewriterAsync(line).ConfigureAwait(false);

            // Brief pause between lines
            await Task.Delay(800).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Displays text with enhanced typewriter effect including audio feedback.
    /// </summary>
    /// <param name="text">The text to display.</param>
    private async Task DisplayTextWithEnhancedTypewriterAsync(string text)
    {
        if (this.Dialogue != null)
        {
            // Use UIDialogue's enhanced typewriter with audio
            await this.Dialogue.StartDialogueAsync(text).ConfigureAwait(false);
        }
        else
        {
            // Fallback to basic typewriter effect with audio
            await this.BasicTypewriterWithAudioAsync(text).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Basic typewriter effect with audio support when UIDialogue is not available.
    /// </summary>
    /// <param name="text">The text to display.</param>
    private async Task BasicTypewriterWithAudioAsync(string text)
    {
        if (this.audioManager == null || !this.audioManager.IsInitialized)
        {
            // Fallback to simple text display if audio is not available
            var outputLabelNode = this.GetNodeOrNull<RichTextLabel>("%OutputLabel");
            if (outputLabelNode != null)
            {
                outputLabelNode.AppendText(text + "\n");
            }
            else
            {
                GD.Print(text);
            }

            return;
        }

        // Display text character by character with audio
        var outputLabel = this.GetNodeOrNull<RichTextLabel>("%OutputLabel");
        if (outputLabel != null)
        {
            outputLabel.AppendText("\n");

            foreach (char character in text)
            {
                outputLabel.AppendText(character.ToString());

                // Play typewriter sound for each character using the audio manager
                this.audioManager.PlayTypewriterSound();

                await this.ToSignal(this.GetTree().CreateTimer(0.025f), Godot.Timer.SignalName.Timeout);
            }

            outputLabel.AppendText("\n");
        }
    }

    /// <summary>
    /// Presents the thread choice options with visual and audio feedback.
    /// </summary>
    private async Task PresentThreadChoiceAsync()
    {
        if (this.sceneData?.InitialChoice == null)
        {
            GD.PrintErr($"[{this.SequenceId}] No initial choice data found");
            this.CompleteSequence();
            return;
        }

        // Display the choice prompt
        await this.DisplayTextWithEnhancedTypewriterAsync($"[b]{this.sceneData.InitialChoice.Prompt}[/b]").ConfigureAwait(false);

        // Show choice buttons with animations
        await this.ShowChoiceButtonsAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// Shows the thread choice buttons with animations and visual feedback.
    /// </summary>
    private async Task ShowChoiceButtonsAsync()
    {
        if (this.choiceContainer == null)
        {
            return;
        }

        // Make container visible
        this.choiceContainer.Visible = true;

        // Get choice options from scene data
        if (this.sceneData?.InitialChoice?.Options != null)
        {
            var options = this.sceneData.InitialChoice.Options;
            if (options.Count >= 3)
            {
                // Update button texts and show them with animations
                if (this.heroButton != null)
                {
                    this.heroButton.Text = options[0].Text ?? options[0].Id ?? "HERO";
                    this.AnimateButtonAppearance(this.heroButton);
                }

                if (this.shadowButton != null)
                {
                    this.shadowButton.Text = options[1].Text ?? options[1].Id ?? "SHADOW";
                    this.AnimateButtonAppearance(this.shadowButton);
                }

                if (this.ambitionButton != null)
                {
                    this.ambitionButton.Text = options[2].Text ?? options[2].Id ?? "AMBITION";
                    this.AnimateButtonAppearance(this.ambitionButton);
                }
            }
        }

        // Add a small delay for visual effect
        await Task.Delay(500).ConfigureAwait(false);
    }

    /// <summary>
    /// Animates button appearance for visual feedback.
    /// </summary>
    /// <param name="button">The button to animate.</param>
    private void AnimateButtonAppearance(Button button)
    {
        // Create a simple scale animation for button appearance
        if (this.AnimationPlayer != null && this.AnimationPlayer.HasAnimation("button_appear"))
        {
            this.AnimationPlayer.Play("button_appear");
        }
        else
        {
            // Fallback: simple visual effect
            var originalScale = button.Scale;
            button.Scale = new Vector2(0.8f, 0.8f);

            // Animate scale to normal
            var tween = this.CreateTween();
            tween.TweenProperty(button, "scale", originalScale, 0.3f);
        }
    }

    /// <summary>
    /// Handles thread choice selection with audio feedback.
    /// Updates game state and completes the sequence.
    /// </summary>
    /// <param name="threadId">The selected thread ID ("hero", "shadow", or "ambition").</param>
    private void OnThreadChoiceSelected(string threadId)
    {
        if (this.choiceMade)
        {
            return; // Prevent multiple selections
        }

        this.choiceMade = true;

        GD.Print($"[{this.SequenceId}] Thread choice selected: {threadId}");

        // Play selection sound using audio manager
        this.audioManager?.PlaySelectionSound();

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
