// <copyright file="ThreadBranch_Ambition.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;
using OmegaSpiral.Source.Scripts.Field.Narrative;
using OmegaSpiral.Source.Scripts.Common;
using OmegaSpiral.Source.Scripts.Infrastructure;

namespace OmegaSpiral.Source.Scripts.Field.Narrative.Sequences;
/// <summary>
/// Thread branch sequence for Ambition-aligned narrative continuation.
/// Presents Ambition-specific story content after the opening choice.
/// </summary>
[GlobalClass]
public partial class ThreadBranchAmbition : NarrativeSequence
{
    /// <summary>
    /// Scene data loaded from the Ambition thread configuration.
    /// </summary>
    private NarrativeSceneData? sceneData;

    /// <summary>
    /// Current story block index being presented.
    /// </summary>
    private int currentBlockIndex;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    protected override void OnSequenceReady()
    {
        base.OnSequenceReady();

        // Load Ambition thread data
        this.LoadAmbitionSceneData();
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public override async Task PlayAsync()
    {
        if (this.sceneData == null)
        {
            GD.PrintErr($"[{this.SequenceId}] Failed to load Ambition scene data");
            this.CompleteSequence();
            return;
        }

        // Fade in
        await this.FadeFromBlackAsync(1.5f).ConfigureAwait(false);

        // Display opening lines
        await this.DisplayOpeningLinesAsync().ConfigureAwait(false);

        // Present story blocks
        await this.PresentStoryBlocksAsync().ConfigureAwait(false);

        // Transition to name input sequence
        this.CompleteSequence("name_input");
    }

    /// <summary>
    /// Loads the Ambition thread scene data from configuration.
    /// </summary>
    private void LoadAmbitionSceneData()
    {
        try
        {
            string path = "res://Source/Data/stages/ghost-terminal/ambition.yaml";
            var configData = ConfigurationService.LoadConfiguration(path);
            this.sceneData = configData != null ? NarrativeSceneFactory.Create(configData) : null;

            if (this.sceneData != null)
            {
                GD.Print($"[{this.SequenceId}] Loaded Ambition scene data with {this.sceneData.StoryBlocks.Count} story blocks");
            }
        }
        catch (Exception ex)
        {
            GD.PrintErr($"[{this.SequenceId}] Exception loading Ambition scene data: {ex.Message}");
        }
    }

    /// <summary>
    /// Displays the opening lines for the Ambition thread.
    /// </summary>
    private async Task DisplayOpeningLinesAsync()
    {
        if (this.sceneData?.OpeningLines == null)
        {
            return;
        }

        foreach (string line in this.sceneData.OpeningLines)
        {
            await this.DisplayTextWithTypewriterAsync(line).ConfigureAwait(false);
            await Task.Delay(1000).ConfigureAwait(false); // Pause between lines
        }
    }

    /// <summary>
    /// Presents all story blocks in sequence.
    /// </summary>
    private async Task PresentStoryBlocksAsync()
    {
        if (this.sceneData?.StoryBlocks == null)
        {
            return;
        }

        for (this.currentBlockIndex = 0; this.currentBlockIndex < this.sceneData.StoryBlocks.Count; this.currentBlockIndex++)
        {
            await this.DisplayStoryBlockAsync(this.sceneData.StoryBlocks[this.currentBlockIndex]).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Displays a single story block with paragraphs and choices.
    /// </summary>
    /// <param name="block">The story block to display.</param>
    private async Task DisplayStoryBlockAsync(StoryBlock block)
    {
        // Display paragraphs
        foreach (string paragraph in block.Paragraphs)
        {
            await this.DisplayTextWithTypewriterAsync(paragraph).ConfigureAwait(false);
            await Task.Delay(800).ConfigureAwait(false);
        }

        // Display question and choices if present
        if (!string.IsNullOrEmpty(block.Question))
        {
            await this.DisplayTextWithTypewriterAsync($"[b]{block.Question}[/b]").ConfigureAwait(false);

            if (block.Choices.Count > 0)
            {
                for (int i = 0; i < block.Choices.Count; i++)
                {
                    await this.DisplayTextWithTypewriterAsync($"  {i + 1}. {block.Choices[i].Text}").ConfigureAwait(false);
                }

                // For now, auto-advance after showing choices (simplified implementation)
                // In a full implementation, this would wait for user input
                await Task.Delay(2000).ConfigureAwait(false);
            }
        }
    }

    /// <summary>
    /// Prompts the player for their name.
    /// </summary>
    private async Task PromptForNameAsync()
    {
        string prompt = string.IsNullOrWhiteSpace(this.sceneData?.NamePrompt)
            ? "What name should the terminal record?"
            : this.sceneData.NamePrompt;

        await this.DisplayTextWithTypewriterAsync($"[b]{prompt}[/b]").ConfigureAwait(false);

        // For now, use a default name (simplified implementation)
        // In a full implementation, this would wait for user input
        await Task.Delay(1500).ConfigureAwait(false);
    }

    /// <summary>
    /// Presents the secret question with options.
    /// </summary>
    private async Task PresentSecretQuestionAsync()
    {
        if (this.sceneData?.SecretQuestion == null)
        {
            return;
        }

        await this.DisplayTextWithTypewriterAsync($"[b]{this.sceneData.SecretQuestion.Prompt}[/b]").ConfigureAwait(false);

        if (this.sceneData.SecretQuestion.Options.Count > 0)
        {
            for (int i = 0; i < this.sceneData.SecretQuestion.Options.Count; i++)
            {
                await this.DisplayTextWithTypewriterAsync($"  {i + 1}. {this.sceneData.SecretQuestion.Options[i]}").ConfigureAwait(false);
            }
        }

        // For now, auto-advance after showing options (simplified implementation)
        await Task.Delay(2000).ConfigureAwait(false);
    }

    /// <summary>
    /// Displays the exit line to conclude the Ambition thread.
    /// </summary>
    private async Task DisplayExitLineAsync()
    {
        if (!string.IsNullOrEmpty(this.sceneData?.ExitLine))
        {
            await this.DisplayTextWithTypewriterAsync(this.sceneData.ExitLine).ConfigureAwait(false);
            await Task.Delay(2000).ConfigureAwait(false);
        }
    }
}
