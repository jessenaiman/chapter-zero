// <copyright file="DreamweaverSystem.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Godot;
using OmegaSpiral.Scripts;

namespace OmegaSpiral.Source.Scripts;

/// <summary>
/// Main orchestrator for the Dreamweaver LLM system.
/// Manages three personas (Hero, Shadow, Ambition) and coordinates
/// dynamic narrative generation using the nobodywho framework.
/// </summary>
public partial class DreamweaverSystem : Node
{
    [Signal]
    public delegate void NarrativeGeneratedEventHandler(string personaId, string generatedText);

    [Signal]
    public delegate void PersonaActivatedEventHandler(string personaId);

    [Signal]
    public delegate void GenerationErrorEventHandler(string personaId, string errorMessage);

    private Dictionary<string, DreamweaverPersona> personas = new ();
    private GameState? gameState;
    private NarrativeCache? narrativeCache;
    private SystemPromptBuilder? promptBuilder;
    private CreativeMemoryRAG? creativeRAG;
    private DreamweaverChoiceTracker? choiceTracker;

    // Observer references
    private OmegaNarrator? omegaNarrator;
    private HeroObserver? heroObserver;
    private ShadowObserver? shadowObserver;
    private AmbitionObserver? ambitionObserver;

    /// <inheritdoc/>
    public override void _Ready()
    {
        this.gameState = this.GetNode<GameState>("/root/GameState");

        // Get infrastructure nodes
        this.narrativeCache = this.GetNodeOrNull<NarrativeCache>("/root/NarrativeCache");
        this.promptBuilder = this.GetNodeOrNull<SystemPromptBuilder>("/root/SystemPromptBuilder");
        this.creativeRAG = this.GetNodeOrNull<CreativeMemoryRAG>("/root/CreativeMemoryRAG");
        this.choiceTracker = this.GetNodeOrNull<DreamweaverChoiceTracker>("/root/DreamweaverChoiceTracker");

        // Get persona nodes
        this.omegaNarrator = this.GetNodeOrNull<OmegaNarrator>("/root/OmegaNarrator");
        this.heroObserver = this.GetNodeOrNull<HeroObserver>("/root/HeroObserver");
        this.shadowObserver = this.GetNodeOrNull<ShadowObserver>("/root/ShadowObserver");
        this.ambitionObserver = this.GetNodeOrNull<AmbitionObserver>("/root/AmbitionObserver");

        // Initialize legacy persona system (for backward compatibility)
        this.InitializePersonas();

        GD.Print("DreamweaverSystem initialized with orchestration");
    }

    private void InitializePersonas()
    {
        // Load persona configurations from JSON files (legacy system)
        var heroConfig = LoadPersonaConfig("hero");
        var shadowConfig = LoadPersonaConfig("shadow");
        var ambitionConfig = LoadPersonaConfig("ambition");

        if (this.gameState == null)
        {
            GD.PrintErr("GameState not found, cannot initialize personas");
            return;
        }

        if (heroConfig != null)
        {
            this.personas["hero"] = new DreamweaverPersona("hero", heroConfig.Value, this.gameState);
        }

        if (shadowConfig != null)
        {
            this.personas["shadow"] = new DreamweaverPersona("shadow", shadowConfig.Value, this.gameState);
        }

        if (ambitionConfig != null)
        {
            this.personas["ambition"] = new DreamweaverPersona("ambition", ambitionConfig.Value, this.gameState);
        }
    }

    private static JsonElement? LoadPersonaConfig(string personaId)
    {
        try
        {
            var configPath = $"res://Source/Data/scenes/scene1_narrative_yaml/{personaId}.yaml";
            if (!Godot.FileAccess.FileExists(configPath))
            {
                GD.PrintErr($"Persona config not found: {configPath}");
                return null;
            }

            Godot.FileAccess file = Godot.FileAccess.Open(configPath, Godot.FileAccess.ModeFlags.Read);
            var yamlText = file.GetAsText();
            file.Close();

            // For now, convert YAML to JSON for parsing
            // TODO: Implement proper YAML parsing
            var jsonText = ConvertYamlToJson(yamlText);

            var options = new JsonSerializerOptions
            {
                AllowTrailingCommas = true,
                ReadCommentHandling = JsonCommentHandling.Skip,
            };

            var doc = JsonDocument.Parse(jsonText);
            return doc.RootElement;
        }
        catch (Exception ex)
        {
            GD.PrintErr($"Failed to load persona config for {personaId}: {ex.Message}");
            return null;
        }
    }

    private static string ConvertYamlToJson(string yamlText)
    {
        // Minimal YAML parser tailored to persona files. Replace with proper YAML parsing when available.
        var lines = yamlText.Split('\n');
        var jsonBuilder = new System.Text.StringBuilder();
        jsonBuilder.AppendLine("{");

        var stack = new Stack<string>();
        var pendingArrayItems = new List<string>();
        int previousIndent = 0;

        void ClosePendingArrays(int currentIndent)
        {
            while (stack.Count > 0 && previousIndent >= currentIndent)
            {
                if (pendingArrayItems.Count > 0)
                {
                    jsonBuilder.AppendLine(string.Join(",\n", pendingArrayItems));
                    pendingArrayItems.Clear();
                }

                jsonBuilder.AppendLine("],");
                stack.Pop();
                previousIndent -= 2;
            }
        }

        for (int i = 0; i < lines.Length; i++)
        {
            var line = lines[i].TrimEnd();
            if (string.IsNullOrWhiteSpace(line) || line.TrimStart().StartsWith("#"))
            {
                continue;
            }

            var indent = line.Length - line.TrimStart().Length;
            var trimmedLine = line.Trim();

            if (trimmedLine.StartsWith("- "))
            {
                var value = trimmedLine.Substring(2).Trim();
                if (value.Contains(": "))
                {
                    var parts = value.Split(new[] { ": " }, 2, StringSplitOptions.None);
                    var itemBuilder = new System.Text.StringBuilder();
                    itemBuilder.Append('{');
                    itemBuilder.Append($"\"{parts[0]}\": \"{parts[1]}\"");
                    itemBuilder.Append('}');
                    pendingArrayItems.Add(itemBuilder.ToString());
                }
                else
                {
                    pendingArrayItems.Add($"\"{value}\"");
                }

                previousIndent = indent;
                continue;
            }

            if (trimmedLine.EndsWith(":"))
            {
                ClosePendingArrays(indent);

                var key = trimmedLine.TrimEnd(':');
                bool startsArray = false;
                if (i + 1 < lines.Length)
                {
                    var nextLine = lines[i + 1].TrimStart();
                    startsArray = nextLine.StartsWith("- ");
                }

                if (startsArray)
                {
                    jsonBuilder.AppendLine($"\"{key}\": [");
                    stack.Push(key);
                }
                else
                {
                    jsonBuilder.AppendLine($"\"{key}\": {{");
                }

                previousIndent = indent;
                continue;
            }

            if (trimmedLine.Contains(": "))
            {
                ClosePendingArrays(indent);

                var parts = trimmedLine.Split(new[] { ": " }, 2, StringSplitOptions.None);
                var key = parts[0];
                var value = parts[1].Trim();
                if (value.StartsWith("\"") && value.EndsWith("\""))
                {
                    value = value.Trim('"');
                }

                jsonBuilder.AppendLine($"\"{key}\": \"{value}\",");
                previousIndent = indent;
            }
        }

        ClosePendingArrays(0);
        jsonBuilder.AppendLine("}");

        var result = jsonBuilder.ToString();
        result = System.Text.RegularExpressions.Regex.Replace(result, ",\\n(?=[\\}\\]])", "\n");
        result = result.Replace("}\n}\n", "}\n}");

        return result;
    }

    /// <summary>
    /// Generates dynamic narrative using OmegaNarrator with full orchestration.
    /// Checks cache first, builds prompt with RAG context, generates via LLM, then caches result.
    /// </summary>
    /// <param name="stepId">The step ID from the scene schema.</param>
    /// <param name="contextLines">Schema lines to use as creative direction.</param>
    /// <returns>Generated narrative text.</returns>
    public async Task<string> GenerateNarrativeAsync(string stepId, string[] contextLines)
    {
        if (this.omegaNarrator == null)
        {
            GD.PrintErr("OmegaNarrator not available");
            return GetFallbackNarrative("omega");
        }

        try
        {
            // 1. Check cache first
            if (this.narrativeCache != null)
            {
                var cached = await this.narrativeCache.LoadCachedNarrativeAsync(stepId, "omega", contextLines).ConfigureAwait(false);
                if (cached != null)
                {
                    return cached;
                }
            }

            // 2. Build prompt with RAG context
            string systemPrompt;
            if (this.promptBuilder != null)
            {
                systemPrompt = await this.promptBuilder.BuildOmegaNarratorPromptAsync(
                    this.omegaNarrator.GetSystemPrompt(),
                    contextLines,
                    stepId).ConfigureAwait(false);
            }
            else
            {
                // Fallback: simple prompt without RAG
                systemPrompt = this.omegaNarrator.GetSystemPrompt();
                systemPrompt += "\n\nCreative direction:\n" + string.Join("\n", contextLines);
            }

            // 3. Generate via OmegaNarrator
            var userPrompt = "Narrate this moment in Omega's voice, expanding on the creative direction provided.";
            var generatedText = await this.omegaNarrator.GenerateResponseAsync(systemPrompt, userPrompt).ConfigureAwait(false);

            // 4. Cache the result
            if (this.narrativeCache != null && !string.IsNullOrWhiteSpace(generatedText))
            {
                await this.narrativeCache.CacheNarrativeAsync(stepId, "omega", generatedText, contextLines).ConfigureAwait(false);
            }

            this.EmitSignal(SignalName.NarrativeGenerated, "omega", generatedText);
            return generatedText;
        }
        catch (Exception ex)
        {
            GD.PrintErr($"Failed to generate narrative: {ex.Message}");
            this.EmitSignal(SignalName.GenerationError, "omega", ex.Message);
            return GetFallbackNarrative("omega");
        }
    }

    /// <summary>
    /// Generates hidden observer commentary for all three Dreamweavers in parallel.
    /// Used after player makes a choice to track alignment with each thread.
    /// </summary>
    /// <param name="stepId">The step ID where choice was made.</param>
    /// <param name="choiceText">The choice the player selected.</param>
    /// <returns>Dictionary mapping observer type to commentary text.</returns>
    public async Task<Dictionary<string, string>> GenerateObserverCommentaryAsync(string stepId, string choiceText)
    {
        var results = new Dictionary<string, string>();

        if (this.heroObserver == null || this.shadowObserver == null || this.ambitionObserver == null)
        {
            GD.PrintErr("One or more observers not available");
            return results;
        }

        try
        {
            // Generate commentary from all three observers in parallel
            var heroTask = this.GenerateSingleObserverCommentaryAsync("hero", this.heroObserver, stepId, choiceText);
            var shadowTask = this.GenerateSingleObserverCommentaryAsync("shadow", this.shadowObserver, stepId, choiceText);
            var ambitionTask = this.GenerateSingleObserverCommentaryAsync("ambition", this.ambitionObserver, stepId, choiceText);

            await Task.WhenAll(heroTask, shadowTask, ambitionTask).ConfigureAwait(false);

            results["hero"] = await heroTask;
            results["shadow"] = await shadowTask;
            results["ambition"] = await ambitionTask;

            // Update choice tracker with sentiment scores
            if (this.choiceTracker != null)
            {
                this.choiceTracker.RecordChoice(
                    stepId,
                    choiceText,
                    results["hero"],
                    results["shadow"],
                    results["ambition"]);
            }

            return results;
        }
        catch (Exception ex)
        {
            GD.PrintErr($"Failed to generate observer commentary: {ex.Message}");
            return results;
        }
    }

    /// <summary>
    /// Loads cached narrative if available.
    /// </summary>
    /// <param name="stepId">The step ID to load cache for.</param>
    /// <param name="personaId">The persona that generated the narrative.</param>
    /// <param name="contextLines">The schema lines used as context (for validation).</param>
    /// <returns>Cached narrative text, or null if not found.</returns>
    public async Task<string?> LoadCachedNarrativeAsync(string stepId, string personaId, string[] contextLines)
    {
        if (this.narrativeCache == null)
        {
            return null;
        }

        return await this.narrativeCache.LoadCachedNarrativeAsync(stepId, personaId, contextLines).ConfigureAwait(false);
    }

    /// <summary>
    /// Caches generated narrative for future use.
    /// </summary>
    /// <param name="stepId">The step ID to cache for.</param>
    /// <param name="personaId">The persona that generated the narrative.</param>
    /// <param name="generatedText">The generated text to cache.</param>
    /// <param name="contextLines">The schema lines used as context.</param>
    public async Task CacheNarrativeAsync(string stepId, string personaId, string generatedText, string[] contextLines)
    {
        if (this.narrativeCache == null)
        {
            return;
        }

        await this.narrativeCache.CacheNarrativeAsync(stepId, personaId, generatedText, contextLines).ConfigureAwait(false);
    }

    /// <summary>
    /// Indexes a scene schema for RAG retrieval.
    /// Should be called once per scene during initialization.
    /// </summary>
    /// <param name="schemaPath">Path to the scene schema JSON file.</param>
    /// <returns>True if indexing succeeded.</returns>
    public async Task<bool> IndexSceneSchemaAsync(string schemaPath)
    {
        if (this.creativeRAG == null)
        {
            GD.PrintErr("CreativeMemoryRAG not available");
            return false;
        }

        return await this.creativeRAG.IndexSchemaAsync(schemaPath).ConfigureAwait(false);
    }

    /// <summary>
    /// Gets statistics about the system's components.
    /// </summary>
    /// <returns>Dictionary with system statistics.</returns>
    public Dictionary<string, object> GetSystemStats()
    {
        var stats = new Dictionary<string, object>
        {
            ["OmegaNarrator"] = this.omegaNarrator != null,
            ["HeroObserver"] = this.heroObserver != null,
            ["ShadowObserver"] = this.shadowObserver != null,
            ["AmbitionObserver"] = this.ambitionObserver != null,
            ["NarrativeCache"] = this.narrativeCache != null,
            ["PromptBuilder"] = this.promptBuilder != null,
            ["CreativeRAG"] = this.creativeRAG != null,
            ["ChoiceTracker"] = this.choiceTracker != null,
        };

        if (this.narrativeCache != null)
        {
            stats["CacheStats"] = this.narrativeCache.GetCacheStats();
        }

        if (this.creativeRAG != null)
        {
            stats["RAGStats"] = this.creativeRAG.GetStats();
        }

        if (this.choiceTracker != null)
        {
            stats["ChoiceStats"] = this.choiceTracker.GetStats();
        }

        return stats;
    }

    private async Task<string> GenerateSingleObserverCommentaryAsync(
        string observerType,
        DreamweaverObserver observer,
        string stepId,
        string choiceText)
    {
        try
        {
            // 1. Check cache first
            var cacheKey = $"{stepId}_{observerType}_commentary";
            if (this.narrativeCache != null)
            {
                var cached = await this.narrativeCache.LoadCachedNarrativeAsync(
                    cacheKey,
                    observerType,
                    new[] { choiceText }).ConfigureAwait(false);
                if (cached != null)
                {
                    return cached;
                }
            }

            // 2. Build prompt with RAG context
            string systemPrompt;
            if (this.promptBuilder != null)
            {
                systemPrompt = await this.promptBuilder.BuildObserverPromptAsync(
                    observer.GetSystemPrompt(),
                    observerType,
                    choiceText,
                    stepId).ConfigureAwait(false);
            }
            else
            {
                // Fallback: simple prompt
                systemPrompt = observer.GetSystemPrompt();
                systemPrompt += $"\n\nPlayer's choice: {choiceText}";
            }

            // 3. Generate commentary
            var userPrompt = "Provide your hidden commentary on this choice (1-2 sentences).";
            var commentary = await observer.GenerateCommentaryAsync(systemPrompt, userPrompt).ConfigureAwait(false);

            // 4. Cache the result
            if (this.narrativeCache != null && !string.IsNullOrWhiteSpace(commentary))
            {
                await this.narrativeCache.CacheNarrativeAsync(
                    cacheKey,
                    observerType,
                    commentary,
                    new[] { choiceText }).ConfigureAwait(false);
            }

            return commentary;
        }
        catch (Exception ex)
        {
            GD.PrintErr($"Failed to generate {observerType} commentary: {ex.Message}");
            return string.Empty;
        }
    }

    /// <summary>
    /// Gets a random opening line for a persona, enhanced by LLM if available.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public async Task<string> GetOpeningLineAsync(string personaId)
    {
        if (!this.personas.ContainsKey(personaId))
        {
            return GetFallbackOpeningLine(personaId);
        }

        try
        {
            var persona = this.personas[personaId];
            return await persona.GetOpeningLineAsync().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            GD.PrintErr($"Failed to get opening line for {personaId}: {ex.Message}");
            return GetFallbackOpeningLine(personaId);
        }
    }

    /// <summary>
    /// Generates dynamic choices for a persona based on current game state.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public async Task<List<ChoiceOption>> GenerateChoicesAsync(string personaId, string context = "")
    {
        if (!this.personas.ContainsKey(personaId))
        {
            return GetFallbackChoices(personaId);
        }

        try
        {
            var persona = this.personas[personaId];
            return await persona.GenerateChoicesAsync(context).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            GD.PrintErr($"Failed to generate choices for {personaId}: {ex.Message}");
            return GetFallbackChoices(personaId);
        }
    }

    /// <summary>
    /// Activates a persona, making it the primary narrative voice.
    /// </summary>
    public void ActivatePersona(string personaId)
    {
        if (!this.personas.ContainsKey(personaId))
        {
            GD.PrintErr($"Cannot activate unknown persona: {personaId}");
            return;
        }

        // Deactivate all other personas
        foreach (var kvp in this.personas)
        {
            kvp.Value.IsActive = kvp.Key == personaId;
        }

        this.EmitSignal(SignalName.PersonaActivated, personaId);
        GD.Print($"Activated Dreamweaver persona: {personaId}");
    }

    /// <summary>
    /// Gets the currently active persona.
    /// </summary>
    /// <returns>The currently active persona, or null if none is active.</returns>
    public DreamweaverPersona? GetActivePersona()
    {
        return this.personas.Values.FirstOrDefault(p => p.IsActive);
    }

    // Fallback methods for when LLM generation fails
    private static string GetFallbackNarrative(string personaId)
    {
        var fallbacks = new Dictionary<string, string>
        {
            ["hero"] = "The hero's path calls to you, filled with light and shadow.",
            ["shadow"] = "The shadows whisper secrets that only you can hear.",
            ["ambition"] = "Ambition drives you forward, carving new paths through reality.",
            ["omega"] = "The system observes. The pattern continues. You are part of the spiral.",
        };

        return fallbacks.GetValueOrDefault(personaId, "The narrative continues...");
    }

    private static string GetFallbackOpeningLine(string personaId)
    {
        var fallbacks = new Dictionary<string, string>
        {
            ["hero"] = "A hero emerges from the darkness.",
            ["shadow"] = "The shadows remember what you forget.",
            ["ambition"] = "Ambition flows upward, defying gravity.",
        };

        return fallbacks.GetValueOrDefault(personaId, "Welcome to the spiral.");
    }

    private static List<ChoiceOption> GetFallbackChoices(string personaId)
    {
        return new List<ChoiceOption>
        {
            new ChoiceOption { Id = "continue", Text = "CONTINUE", Description = "Continue the journey" },
            new ChoiceOption { Id = "reflect", Text = "REFLECT", Description = "Take a moment to reflect" },
            new ChoiceOption { Id = "question", Text = "QUESTION", Description = "Ask a question" },
        };
    }
}
