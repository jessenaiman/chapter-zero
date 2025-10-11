using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Godot;

namespace OmegaSpiral;

/// <summary>
/// Builds system prompts for LLM personas by combining persona configuration,
/// scene schema context, RAG-retrieved creative content, and game state.
/// </summary>
public partial class SystemPromptBuilder : Node
{
    private GameState? gameState;
    private CreativeMemoryRAG? creativeRAG;

    /// <inheritdoc/>
    public override void _Ready()
    {
        this.gameState = this.GetNode<GameState>("/root/GameState");
        this.creativeRAG = this.GetNode<CreativeMemoryRAG>("/root/CreativeMemoryRAG");
        GD.Print("SystemPromptBuilder initialized");
    }

    /// <summary>
    /// Builds a system prompt for OmegaNarrator based on current context.
    /// </summary>
    /// <param name="basePrompt">The base system prompt from OmegaNarrator's configuration.</param>
    /// <param name="sceneContext">Schema lines from the current step (creative direction).</param>
    /// <param name="stepId">The current step ID for RAG retrieval.</param>
    /// <returns>Complete system prompt with context.</returns>
    public async System.Threading.Tasks.Task<string> BuildOmegaNarratorPromptAsync(
        string basePrompt,
        string[] sceneContext,
        string stepId)
    {
        var promptBuilder = new StringBuilder();

        // 1. Base persona prompt
        promptBuilder.AppendLine(basePrompt);
        promptBuilder.AppendLine();

        // 2. Game state context
        if (this.gameState != null)
        {
            promptBuilder.AppendLine("## Current Game State");
            promptBuilder.AppendLine($"- Player Name: {this.gameState.PlayerName ?? "Unknown"}");
            promptBuilder.AppendLine($"- Selected Thread: {this.gameState.SelectedThread ?? "None"}");
            promptBuilder.AppendLine($"- Game Seed: {this.gameState.GameSeed}");
            promptBuilder.AppendLine();
        }

        // 3. Creative direction from schema
        promptBuilder.AppendLine("## Creative Direction (from scene schema)");
        promptBuilder.AppendLine("The creative team has provided the following narrative beats for this moment:");
        foreach (var line in sceneContext)
        {
            promptBuilder.AppendLine($"- {line}");
        }

        promptBuilder.AppendLine();
        promptBuilder.AppendLine("Use these beats as creative direction. Expand on them with Omega's cold, systematic voice.");
        promptBuilder.AppendLine("Maintain the same emotional arc and narrative progression.");
        promptBuilder.AppendLine();

        // 4. RAG context (related narrative beats)
        if (this.creativeRAG != null)
        {
            var ragResults = await this.creativeRAG.FindRelatedBeatsAsync(stepId, topK: 3).ConfigureAwait(false);
            if (ragResults.Count > 0)
            {
                promptBuilder.AppendLine("## Related Narrative Context");
                promptBuilder.AppendLine("These related moments may inform your narration:");
                foreach (var result in ragResults)
                {
                    promptBuilder.AppendLine($"- Step '{result.StepId}': {result.Content}");
                }

                promptBuilder.AppendLine();
            }
        }

        // 5. Output format instructions
        promptBuilder.AppendLine("## Output Format");
        promptBuilder.AppendLine("Generate Omega's narration as a single continuous text block.");
        promptBuilder.AppendLine("Do NOT add labels like 'Omega:' or 'Narrator:' - just the raw text.");
        promptBuilder.AppendLine("Keep the tone cold, mechanical, and observational.");

        return promptBuilder.ToString();
    }

    /// <summary>
    /// Builds a system prompt for a DreamweaverObserver based on current context.
    /// </summary>
    /// <param name="basePrompt">The base system prompt from the observer's configuration.</param>
    /// <param name="observerType">The observer type (hero/shadow/ambition).</param>
    /// <param name="choiceContext">The player's choice that triggered commentary.</param>
    /// <param name="stepId">The current step ID for RAG retrieval.</param>
    /// <returns>Complete system prompt with context.</returns>
    public async System.Threading.Tasks.Task<string> BuildObserverPromptAsync(
        string basePrompt,
        string observerType,
        string choiceContext,
        string stepId)
    {
        var promptBuilder = new StringBuilder();

        // 1. Base persona prompt
        promptBuilder.AppendLine(basePrompt);
        promptBuilder.AppendLine();

        // 2. Observer role reminder
        promptBuilder.AppendLine("## Your Role");
        promptBuilder.AppendLine("You are a hidden observer speaking to the other Dreamweavers (Hero, Shadow, Ambition).");
        promptBuilder.AppendLine("The player does NOT hear your commentary - only your fellow observers do.");
        promptBuilder.AppendLine("Analyze the player's choice through your unique lens.");
        promptBuilder.AppendLine();

        // 3. Player choice context
        promptBuilder.AppendLine("## Player's Choice");
        promptBuilder.AppendLine(choiceContext);
        promptBuilder.AppendLine();

        // 4. Game state context
        if (this.gameState != null)
        {
            promptBuilder.AppendLine("## Game State");
            promptBuilder.AppendLine($"- Player Name: {this.gameState.PlayerName ?? "Unknown"}");
            promptBuilder.AppendLine($"- Selected Thread: {this.gameState.SelectedThread ?? "None"}");
            promptBuilder.AppendLine($"- Current Scene: Scene 1 (Narrative Terminal)");
            promptBuilder.AppendLine();
        }

        // 5. RAG context (related narrative beats)
        if (this.creativeRAG != null)
        {
            var ragResults = await this.creativeRAG.FindRelatedBeatsAsync(stepId, topK: 2).ConfigureAwait(false);
            if (ragResults.Count > 0)
            {
                promptBuilder.AppendLine("## Related Story Moments");
                foreach (var result in ragResults)
                {
                    promptBuilder.AppendLine($"- {result.Content}");
                }

                promptBuilder.AppendLine();
            }
        }

        // 6. Output format instructions
        promptBuilder.AppendLine("## Output Format");
        promptBuilder.AppendLine("Provide your hidden commentary as 1-2 sentences.");
        promptBuilder.AppendLine("Focus on one key insight about this choice.");
        promptBuilder.AppendLine($"Speak in your {observerType} voice, but remember: the player won't hear this.");

        return promptBuilder.ToString();
    }

    /// <summary>
    /// Builds a simplified prompt for opening lines or one-shot generation.
    /// </summary>
    /// <param name="basePrompt">The base system prompt.</param>
    /// <param name="context">Additional context for the generation.</param>
    /// <returns>Complete system prompt.</returns>
    public string BuildSimplePrompt(string basePrompt, string context = "")
    {
        var promptBuilder = new StringBuilder();
        promptBuilder.AppendLine(basePrompt);

        if (!string.IsNullOrEmpty(context))
        {
            promptBuilder.AppendLine();
            promptBuilder.AppendLine("## Context");
            promptBuilder.AppendLine(context);
        }

        if (this.gameState != null && !string.IsNullOrEmpty(this.gameState.PlayerName))
        {
            promptBuilder.AppendLine();
            promptBuilder.AppendLine($"Player Name: {this.gameState.PlayerName}");
        }

        return promptBuilder.ToString();
    }

    /// <summary>
    /// Processes variable substitution in prompts ({playerName}, {selectedThread}, etc.).
    /// </summary>
    /// <param name="promptText">The prompt text with variables.</param>
    /// <returns>Prompt text with variables replaced.</returns>
    public string ProcessVariables(string promptText)
    {
        if (this.gameState == null)
        {
            return promptText;
        }

        var processed = promptText
            .Replace("{playerName}", this.gameState.PlayerName ?? "Unknown")
            .Replace("{selectedThread}", this.gameState.SelectedThread ?? "None")
            .Replace("{gameSeed}", this.gameState.GameSeed.ToString());

        return processed;
    }

    /// <summary>
    /// Builds a prompt for dynamic choice generation.
    /// </summary>
    /// <param name="basePrompt">The base system prompt.</param>
    /// <param name="context">The context for choice generation.</param>
    /// <param name="numChoices">Number of choices to generate.</param>
    /// <returns>Complete system prompt for choice generation.</returns>
    public string BuildChoiceGenerationPrompt(string basePrompt, string context, int numChoices = 3)
    {
        var promptBuilder = new StringBuilder();
        promptBuilder.AppendLine(basePrompt);
        promptBuilder.AppendLine();
        promptBuilder.AppendLine("## Task");
        promptBuilder.AppendLine($"Generate {numChoices} narrative choices for the player.");
        promptBuilder.AppendLine();

        if (!string.IsNullOrEmpty(context))
        {
            promptBuilder.AppendLine("## Context");
            promptBuilder.AppendLine(context);
            promptBuilder.AppendLine();
        }

        promptBuilder.AppendLine("## Output Format");
        promptBuilder.AppendLine("Return choices as JSON array:");
        promptBuilder.AppendLine("[");
        promptBuilder.AppendLine("  { \"id\": \"choice1\", \"text\": \"CHOICE TEXT\", \"description\": \"Brief description\" },");
        promptBuilder.AppendLine("  ...");
        promptBuilder.AppendLine("]");

        return promptBuilder.ToString();
    }

    /// <summary>
    /// Gets a summary of available context for debugging.
    /// </summary>
    /// <returns>Dictionary with context availability.</returns>
    public Dictionary<string, bool> GetContextAvailability()
    {
        return new Dictionary<string, bool>
        {
            ["GameState"] = this.gameState != null,
            ["CreativeRAG"] = this.creativeRAG != null,
            ["PlayerName"] = !string.IsNullOrEmpty(this.gameState?.PlayerName),
            ["SelectedThread"] = !string.IsNullOrEmpty(this.gameState?.SelectedThread),
        };
    }
}
