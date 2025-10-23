// <copyright file="DreamweaverPersona.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using System.Text;
using Godot;
using OmegaSpiral.Source.Scripts.Common;

namespace OmegaSpiral.Source.Narrative;
/// <summary>
/// Represents an individual Dreamweaver persona that can generate dynamic narrative
/// using the nobodywho LLM framework. Each persona uses JSON text as a foundation
/// for creating personalized dialogue and choices.
/// </summary>
public partial class DreamweaverPersona
{
    private PersonaConfig config;
    private GameState? gameState;
    private Random random = new();

    /// <summary>
    /// NobodyWho integration - references to scene nodes
    /// </summary>
    private GodotObject? llmChatNode;
    private bool llmInitialized;

    /// <summary>
    /// Initializes a new instance of the <see cref="DreamweaverPersona"/> class.
    /// </summary>
    /// <param name="personaId">The unique identifier for this persona.</param>
    /// <param name="config">The YAML configuration for this persona.</param>
    /// <param name="gameState">The global game state instance.</param>
    public DreamweaverPersona(string personaId, PersonaConfig config, GameState gameState)
    {
        ArgumentNullException.ThrowIfNull(personaId);
        ArgumentNullException.ThrowIfNull(config);
        ArgumentNullException.ThrowIfNull(gameState);

        this.PersonaId = personaId;
        this.config = config;
        this.gameState = gameState;
        this.llmInitialized = false;

        // Use persona ID as name and archetype if not specified in config
        this.Name = personaId.ToUpper(System.Globalization.CultureInfo.InvariantCulture);
        this.Archetype = personaId;
    }

    /// <summary>
    /// Gets the unique identifier for this persona.
    /// </summary>
    public string PersonaId { get; private set; }

    /// <summary>
    /// Gets or sets a value indicating whether this persona is currently active.
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Gets the display name of this persona.
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    /// Gets the archetype or personality type of this persona.
    /// </summary>
    public string Archetype { get; private set; }

    /// <summary>
    /// Gets the configuration for this persona.
    /// </summary>
    public PersonaConfig Config => this.config;

    /// <summary>
    /// Gets fallback choices when LLM generation fails.
    /// </summary>
    /// <summary>
    /// Generates dynamic narrative text using the JSON foundation as prompts.
    /// </summary>
    /// <param name="context">Additional context for narrative generation.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public async Task<string> GenerateNarrativeAsync(string context = "")
    {
        try
        {
            // Build prompt using JSON text as foundation
            var prompt = this.BuildNarrativePrompt(context);

            // Use LLM to generate response (placeholder for now)
            var generatedText = await this.GenerateWithLlmAsync(prompt).ConfigureAwait(false);

            // If LLM fails, fall back to JSON-based generation
            if (string.IsNullOrEmpty(generatedText))
            {
                generatedText = this.GenerateFromJsonFallback();
            }

            return generatedText;
        }
        catch (InvalidOperationException ex)
        {
            GD.PrintErr($"Narrative generation failed for {this.PersonaId}: {ex.Message}");
            return this.GenerateFromJsonFallback();
        }
        catch (ArgumentException ex)
        {
            GD.PrintErr($"JSON parsing failed for {this.PersonaId}: {ex.Message}");
            return this.GenerateFromJsonFallback();
        }
    }

    /// <summary>
    /// Gets a dynamic opening line, potentially enhanced by LLM.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public async Task<string> GetOpeningLineAsync()
    {
        try
        {
            if (this.config.OpeningLines.Count == 0)
            {
                return this.GetFallbackOpeningLine();
            }

            // Use LLM to potentially enhance or create new opening line
            var baseLine = this.config.OpeningLines[this.random.Next(this.config.OpeningLines.Count)];
            var prompt = $"Enhance this opening line while maintaining the {this.Archetype} theme: \"{baseLine}\"";

            var enhancedLine = await this.GenerateWithLlmAsync(prompt).ConfigureAwait(false);
            return string.IsNullOrEmpty(enhancedLine) ? baseLine : enhancedLine;
        }
        catch (InvalidOperationException ex)
        {
            GD.PrintErr($"Failed to get opening line for {this.PersonaId}: {ex.Message}");
            return this.GetFallbackOpeningLine();
        }
        catch (ArgumentOutOfRangeException ex)
        {
            GD.PrintErr($"Invalid config for {this.PersonaId}: {ex.Message}");
            return this.GetFallbackOpeningLine();
        }
    }

    /// <summary>
    /// Generates dynamic choices based on current game state and persona.
    /// </summary>
    /// <param name="context">Additional context for choice generation.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public async Task<List<ChoiceOption>> GenerateChoicesAsync(string context = "")
    {
        try
        {
            var baseChoices = this.GetBaseChoicesFromJson();
            if (baseChoices.Count == 0)
            {
                return GetFallbackChoices();
            }

            // Use LLM to potentially enhance or create new choices
            var prompt = this.BuildChoiceGenerationPrompt(context, baseChoices);
            var generatedChoices = await this.GenerateChoicesWithLlmAsync(prompt).ConfigureAwait(false);

            return generatedChoices.Count > 0 ? generatedChoices : baseChoices;
        }
        catch (InvalidOperationException ex)
        {
            GD.PrintErr($"Failed to generate choices for {this.PersonaId}: {ex.Message}");
            return this.GetBaseChoicesFromJson();
        }
        catch (ArgumentException ex)
        {
            GD.PrintErr($"JSON parsing failed for {this.PersonaId}: {ex.Message}");
            return this.GetBaseChoicesFromJson();
        }
    }

    /// <summary>
    /// Gets fallback choices when LLM generation fails.
    /// </summary>
    /// <returns>A list of fallback choice options.</returns>
    private static List<ChoiceOption> GetFallbackChoices()
    {
        return new List<ChoiceOption>
        {
            new ChoiceOption { Id = "continue", Label = "CONTINUE", Description = "Continue the journey" },
            new ChoiceOption { Id = "reflect", Label = "REFLECT", Description = "Take a moment to reflect" },
            new ChoiceOption { Id = "question", Label = "QUESTION", Description = "Ask a question" },
        };
    }

    /// <summary>
    /// Parses choices from JSON response using Godot native JSON parsing.
    /// </summary>
    /// <param name="jsonResponse">The JSON response string to parse.</param>
    /// <returns>A list of parsed choice options.</returns>
    /// <exception cref="ArgumentException">Thrown when JSON parsing fails.</exception>
    private static List<ChoiceOption> ParseChoicesFromJson(string jsonResponse)
    {
        var choices = new List<ChoiceOption>();

        try
        {
            // Clean up the response to extract JSON
            var jsonStart = jsonResponse.IndexOf('[');
            var jsonEnd = jsonResponse.LastIndexOf(']') + 1;
            if (jsonStart >= 0 && jsonEnd > jsonStart)
            {
                var jsonText = jsonResponse.Substring(jsonStart, jsonEnd - jsonStart);

                // Use Godot native JSON parsing
                var json = new Json();
                var parseError = json.Parse(jsonText);

                if (parseError != Error.Ok)
                {
                    GD.PrintErr($"Failed to parse choices JSON: {json.GetErrorMessage()}");
                    return choices;
                }

                var data = json.Data;

                // Validate data is an array
                if (data.VariantType != Variant.Type.Array)
                {
                    GD.PrintErr("Choices JSON is not an array");
                    return choices;
                }

                var choicesArray = data.AsGodotArray();

                // Map each choice from Godot variant to ChoiceOption
                foreach (var choiceVariant in choicesArray)
                {
                    if (choiceVariant.VariantType != Variant.Type.Dictionary)
                    {
                        continue;
                    }

                    var choiceDict = choiceVariant.AsGodotDictionary();

                    var choice = new ChoiceOption
                    {
                        Id = choiceDict.ContainsKey("id") ? choiceDict["id"].AsString() : string.Empty,
                        Label = choiceDict.ContainsKey("text") ? choiceDict["text"].AsString() :
                                choiceDict.ContainsKey("label") ? choiceDict["label"].AsString() : string.Empty,
                        Description = choiceDict.ContainsKey("description") ? choiceDict["description"].AsString() : string.Empty,
                    };

                    choices.Add(choice);
                }
            }
        }
        catch (ArgumentOutOfRangeException ex)
        {
            GD.PrintErr($"Invalid JSON response format: {ex.Message}");
        }
        catch (ArgumentException ex)
        {
            GD.PrintErr($"Failed to parse choices JSON: {ex.Message}");
        }

        return choices;
    }

    /// <summary>
    /// Initializes the LLM chat node for this persona by setting up system prompts.
    /// Call this after the scene is ready to ensure NobodyWho nodes are available.
    /// </summary>
    public void InitializeLlmAsync()
    {
        // This will be called when DreamweaverSystem initializes personas in the scene
        // For now, we'll rely on fallback narrative generation
        this.llmInitialized = true;
        GD.Print($"Persona {this.PersonaId} LLM initialized (fallback mode active)");
    }

    /// <summary>
    /// Sets the chat node reference for LLM communication.
    /// </summary>
    public void SetChatNode(GodotObject chatNode)
    {
        this.llmChatNode = chatNode;
        GD.Print($"Chat node set for persona {this.PersonaId}");
    }

    /// <summary>
    /// Private helper methods
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    private string BuildNarrativePrompt(string context)
    {
        var sb = new StringBuilder();

        // Add persona identity and theme
        sb.AppendLine(FormattableString.Invariant($"You are {this.Name}, the {this.Archetype.ToUpper(System.Globalization.CultureInfo.InvariantCulture)} Dreamweaver."));
        sb.AppendLine("Your personality: mysterious and profound");

        // Add current game state context
        if (this.gameState != null)
        {
            sb.AppendLine(FormattableString.Invariant($"Current game state: Party size {this.gameState.PlayerParty.Members.Count}, Dreamweaver thread {this.gameState.DreamweaverThread}"));
        }

        // Add specific context if provided
        if (!string.IsNullOrEmpty(context))
        {
            sb.AppendLine(FormattableString.Invariant($"Context: {context}"));
        }

        // Add story block narrative elements
        if (this.config.StoryBlocks.Count > 0)
        {
            sb.AppendLine("Foundation narrative elements:");
            foreach (var block in this.config.StoryBlocks.Take(3))
            {
                foreach (var paragraph in block.Paragraphs)
                {
                    sb.AppendLine(FormattableString.Invariant($"- {paragraph}"));
                }
            }
        }

        sb.AppendLine("\nGenerate a compelling narrative continuation that fits this persona's voice and theme.");
        sb.AppendLine("Keep it concise but impactful, under 20 words.");

        return sb.ToString();
    }

    private string BuildChoiceGenerationPrompt(string context, List<ChoiceOption> baseChoices)
    {
        var sb = new StringBuilder();

        sb.AppendLine(FormattableString.Invariant($"You are {this.Name}, the {this.Archetype.ToUpper(System.Globalization.CultureInfo.InvariantCulture)} Dreamweaver."));
        sb.AppendLine("Generate 3 meaningful choices for the player that fit your persona and current situation.");

        if (!string.IsNullOrEmpty(context))
        {
            sb.AppendLine(FormattableString.Invariant($"Context: {context}"));
        }

        sb.AppendLine("Each choice should have:");
        sb.AppendLine("- A short label (2-4 words)");
        sb.AppendLine("- A brief description (under 20 words)");
        sb.AppendLine("- An ID for identification");

        sb.AppendLine("Base choices for inspiration:");
        foreach (var choice in baseChoices)
        {
            sb.AppendLine(FormattableString.Invariant($"- {choice.Label}: {choice.Description}"));
        }

        return sb.ToString();
    }

    private async Task<string> GenerateWithLlmAsync(string prompt)
    {
        await Task.Yield(); // Make this properly async for future LLM integration

        if (!this.llmInitialized || this.llmChatNode == null)
        {
            GD.Print($"LLM not initialized for {this.PersonaId}, using fallback");
            return string.Empty;
        }

        try
        {
            // For now, use fallback generation
            // TODO: Implement actual NobodyWho integration when scene nodes are available
            return string.Empty;
        }
        catch (InvalidOperationException ex)
        {
            GD.PrintErr($"LLM generation failed: {ex.Message}");
            return string.Empty;
        }
    }

    private async Task<List<ChoiceOption>> GenerateChoicesWithLlmAsync(string prompt)
    {
        await Task.Yield(); // Make this properly async for future LLM integration

        if (!this.llmInitialized || this.llmChatNode == null)
        {
            return new List<ChoiceOption>();
        }

        try
        {
            // For now, use fallback generation
            // TODO: Implement actual NobodyWho integration when scene nodes are available
            return new List<ChoiceOption>();
        }
        catch (InvalidOperationException ex)
        {
            GD.PrintErr($"LLM choice generation failed: {ex.Message}");
            return new List<ChoiceOption>();
        }
    }

    private string GenerateFromJsonFallback()
    {
        if (this.config.StoryBlocks.Count > 0)
        {
            var block = this.config.StoryBlocks[this.random.Next(this.config.StoryBlocks.Count)];
            return string.Join(" ", block.Paragraphs);
        }

        return this.GetFallbackNarrative();
    }

    private List<ChoiceOption> GetBaseChoicesFromJson()
    {
        var choices = new List<ChoiceOption>();

        try
        {
            if (this.config.InitialChoice != null && this.config.InitialChoice.Options.Count > 0)
            {
                foreach (var option in this.config.InitialChoice.Options)
                {
                    choices.Add(new ChoiceOption
                    {
                        Id = option.Id,
                        Label = option.Label,
                        Description = option.Description,
                    });
                }
            }
        }
        catch (InvalidOperationException ex)
        {
            GD.PrintErr($"Invalid config structure for {this.PersonaId}: {ex.Message}");
        }
        catch (ArgumentNullException ex)
        {
            GD.PrintErr($"Failed to parse choices from config for {this.PersonaId}: {ex.Message}");
        }

        return choices;
    }

    /// <summary>
    /// Fallback methods
    /// </summary>
    /// <returns></returns>
    private string GetFallbackOpeningLine()
    {
        var fallbacks = new Dictionary<string, string>
        {
            ["hero"] = "A hero emerges from the darkness.",
            ["shadow"] = "The shadows remember what you forget.",
            ["ambition"] = "Ambition flows upward, defying gravity.",
        };
        return fallbacks.GetValueOrDefault(this.PersonaId, "Welcome to the spiral.");
    }

    private string GetFallbackNarrative()
    {
        var fallbacks = new Dictionary<string, string>
        {
            ["hero"] = "The hero's journey continues, filled with challenges and revelations.",
            ["shadow"] = "The shadows deepen, revealing hidden truths and forgotten memories.",
            ["ambition"] = "Ambition drives progress, pushing boundaries and achieving the impossible.",
        };
        return fallbacks.GetValueOrDefault(this.PersonaId, "The narrative unfolds...");
    }
}
