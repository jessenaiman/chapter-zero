using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Text;

namespace OmegaSpiral.Source.Scripts;

/// <summary>
/// Represents an individual Dreamweaver persona that can generate dynamic narrative
/// using the nobodywho LLM framework. Each persona uses JSON text as a foundation
/// for creating personalized dialogue and choices.
/// </summary>
public partial class DreamweaverPersona
{
    public string PersonaId { get; private set; }
    public bool IsActive { get; set; }
    public string Name { get; private set; }
    public string Archetype { get; private set; }

    private JsonElement _config;
        private GameState? _gameState = null;
    private Random _random = new();

    // NobodyWho integration
    private GodotObject? _llmModel;

    public DreamweaverPersona(string personaId, JsonElement config, GameState gameState)
    {
        PersonaId = personaId;
        _config = config;
        _gameState = gameState;

        // Extract persona metadata from config
        Name = GetConfigString("name", personaId.ToUpper());
        Archetype = GetConfigString("archetype", personaId);

        // Initialize LLM model
        InitializeLlmModel();
    }

    private void InitializeLlmModel()
    {
        try
        {
            // Initialize NobodyWho model for this persona
            _llmModel = (GodotObject)ClassDB.Instantiate("NobodyWhoModel");

            // Load the Qwen3-4B model
            var modelPath = "res://models/qwen3-4b-instruct-2507-q4_k_m.gguf";
            _llmModel.Call("load_model", modelPath);

            // Configure model parameters for narrative generation
            _llmModel.Set("temperature", 0.8f);  // Creative but not too random
            _llmModel.Set("max_tokens", 200);    // Keep responses concise
            _llmModel.Set("top_p", 0.9f);        // Balanced sampling

            GD.Print($"Initialized NobodyWho LLM model for persona: {PersonaId}");
        }
        catch (Exception ex)
        {
            GD.PrintErr($"Failed to initialize NobodyWho LLM model for {PersonaId}: {ex.Message}");
            _llmModel = null;
        }
    }

    /// <summary>
    /// Generates dynamic narrative text using the JSON foundation as prompts.
    /// </summary>
    public async Task<string> GenerateNarrativeAsync(string context = "")
    {
        try
        {
            // Build prompt using JSON text as foundation
            var prompt = BuildNarrativePrompt(context);

            // Use LLM to generate response (placeholder for now)
            var generatedText = await GenerateWithLlmAsync(prompt);

            // If LLM fails, fall back to JSON-based generation
            if (string.IsNullOrEmpty(generatedText))
            {
                generatedText = GenerateFromJsonFallback(context);
            }

            return generatedText;
        }
        catch (Exception ex)
        {
            GD.PrintErr($"Narrative generation failed for {PersonaId}: {ex.Message}");
            return GenerateFromJsonFallback(context);
        }
    }

    /// <summary>
    /// Gets a dynamic opening line, potentially enhanced by LLM.
    /// </summary>
    public async Task<string> GetOpeningLineAsync()
    {
        try
        {
            var openingLines = GetConfigArray("openingLines");
            if (openingLines.Count == 0)
            {
                return GetFallbackOpeningLine();
            }

            // Use LLM to potentially enhance or create new opening line
            var baseLine = openingLines[_random.Next(openingLines.Count)];
            var prompt = $"Enhance this opening line while maintaining the {Archetype} theme: \"{baseLine}\"";

            var enhancedLine = await GenerateWithLlmAsync(prompt);
            return string.IsNullOrEmpty(enhancedLine) ? baseLine : enhancedLine;
        }
        catch (Exception ex)
        {
            GD.PrintErr($"Failed to get opening line for {PersonaId}: {ex.Message}");
            return GetFallbackOpeningLine();
        }
    }

    /// <summary>
    /// Generates dynamic choices based on current game state and persona.
    /// </summary>
    public async Task<List<ChoiceOption>> GenerateChoicesAsync(string context = "")
    {
        try
        {
            var baseChoices = GetBaseChoicesFromJson();
            if (baseChoices.Count == 0)
            {
                return GetFallbackChoices();
            }

            // Use LLM to potentially enhance or create new choices
            var prompt = BuildChoiceGenerationPrompt(context, baseChoices);
            var generatedChoices = await GenerateChoicesWithLlmAsync(prompt);

            return generatedChoices.Count > 0 ? generatedChoices : baseChoices;
        }
        catch (Exception ex)
        {
            GD.PrintErr($"Failed to generate choices for {PersonaId}: {ex.Message}");
            return GetBaseChoicesFromJson();
        }
    }

    private string BuildNarrativePrompt(string context)
    {
        var sb = new StringBuilder();

        // Add persona identity and theme
        sb.AppendLine($"You are {Name}, the {Archetype.ToUpper()} Dreamweaver.");
        sb.AppendLine($"Your personality: {GetConfigString("personality", "mysterious and profound")}");

        // Add current game state context
        if (_gameState != null)
        {
            sb.AppendLine($"Current game state: Party size {_gameState.PlayerParty.Members.Count}, Dreamweaver thread {_gameState.DreamweaverThread}");
        }

        // Add specific context if provided
        if (!string.IsNullOrEmpty(context))
        {
            sb.AppendLine($"Context: {context}");
        }

        // Add JSON foundation text
        var storyBlocks = GetConfigArray("storyBlocks");
        if (storyBlocks.Count > 0)
        {
            sb.AppendLine("Foundation narrative elements:");
            foreach (var block in storyBlocks.Take(3)) // Limit to prevent prompt bloat
            {
                sb.AppendLine($"- {block}");
            }
        }

        sb.AppendLine("\nGenerate a compelling narrative continuation that fits this persona's voice and theme.");
        sb.AppendLine("Keep it concise but impactful, under 200 words.");

        return sb.ToString();
    }

    private string BuildChoiceGenerationPrompt(string context, List<ChoiceOption> baseChoices)
    {
        var sb = new StringBuilder();

        sb.AppendLine($"You are {Name}, the {Archetype.ToUpper()} Dreamweaver.");
        sb.AppendLine("Generate 3 meaningful choices for the player that fit your persona and current situation.");

        if (!string.IsNullOrEmpty(context))
        {
            sb.AppendLine($"Context: {context}");
        }

        sb.AppendLine("Each choice should have:");
        sb.AppendLine("- A short label (2-4 words)");
        sb.AppendLine("- A brief description (under 20 words)");
        sb.AppendLine("- An ID for identification");

        sb.AppendLine("Base choices for inspiration:");
        foreach (var choice in baseChoices)
        {
            sb.AppendLine($"- {choice.Text}: {choice.Description}");
        }

        return sb.ToString();
    }

    private async Task<string> GenerateWithLlmAsync(string prompt)
    {
        if (_llmModel == null)
        {
            GD.PrintErr("LLM model not initialized");
            return "";
        }

        try
        {
            // Create a chat interface for structured conversation
            var chat = (GodotObject)ClassDB.Instantiate("NobodyWhoChat");
            chat.Call("set_model", _llmModel);

            // Add system message to establish persona
            var systemMessage = $"You are {Name}, the {Archetype.ToUpper()} Dreamweaver. Respond in character.";
            chat.Call("add_message", "system", systemMessage);

            // Add user prompt
            chat.Call("add_message", "user", prompt);

            // Generate response (synchronous call for now - Godot GDExtension async is complex)
            var result = (string)chat.Call("generate");

            // Clean up chat instance
            chat.Call("queue_free");

            return result;
        }
        catch (Exception ex)
        {
            GD.PrintErr($"LLM generation failed: {ex.Message}");
            return "";
        }
    }

    private async Task<List<ChoiceOption>> GenerateChoicesWithLlmAsync(string prompt)
    {
        if (_llmModel == null)
        {
            return new List<ChoiceOption>();
        }

        try
        {
            // Create a chat interface for structured conversation
            var chat = (GodotObject)ClassDB.Instantiate("NobodyWhoChat");
            chat.Call("set_model", _llmModel);

            // Add system message for structured output
            var systemMessage = @"You are a narrative choice generator. Always respond with exactly 3 choices in this JSON format:
[
  {""id"": ""choice1"", ""text"": ""CHOICE LABEL"", ""description"": ""Brief description""},
  {""id"": ""choice2"", ""text"": ""CHOICE LABEL"", ""description"": ""Brief description""},
  {""id"": ""choice3"", ""text"": ""CHOICE LABEL"", ""description"": ""Brief description""}
]";
            chat.Call("add_message", "system", systemMessage);

            // Add user prompt
            chat.Call("add_message", "user", prompt);

            // Generate response (synchronous call for now - Godot GDExtension async is complex)
            var result = (string)chat.Call("generate");

            // Parse JSON response
            var choices = ParseChoicesFromJson(result);

            // Clean up chat instance
            chat.Call("queue_free");

            return choices;
        }
        catch (Exception ex)
        {
            GD.PrintErr($"LLM choice generation failed: {ex.Message}");
            return new List<ChoiceOption>();
        }
    }

    private List<ChoiceOption> ParseChoicesFromJson(string jsonResponse)
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
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var parsedChoices = JsonSerializer.Deserialize<List<ChoiceOption>>(jsonText, options);

                if (parsedChoices != null)
                {
                    choices = parsedChoices;
                }
            }
        }
        catch (Exception ex)
        {
            GD.PrintErr($"Failed to parse choices JSON: {ex.Message}");
        }

        return choices;
    }

    private string GenerateFromJsonFallback(string context)
    {
        var storyBlocks = GetConfigArray("storyBlocks");
        if (storyBlocks.Count > 0)
        {
            return storyBlocks[_random.Next(storyBlocks.Count)];
        }

        return GetFallbackNarrative();
    }

    private List<ChoiceOption> GetBaseChoicesFromJson()
    {
        var choices = new List<ChoiceOption>();

        try
        {
            if (_config.TryGetProperty("initialChoice", out var initialChoice) &&
                initialChoice.TryGetProperty("options", out var options))
            {
                foreach (var option in options.EnumerateArray())
                {
                    choices.Add(new ChoiceOption
                    {
                        Id = GetJsonString(option, "id"),
                        Text = GetJsonString(option, "label"),
                        Description = GetJsonString(option, "description")
                    });
                }
            }
        }
        catch (Exception ex)
        {
            GD.PrintErr($"Failed to parse choices from JSON for {PersonaId}: {ex.Message}");
        }

        return choices;
    }

    // Helper methods for JSON parsing
    private string GetConfigString(string property, string defaultValue = "")
    {
        if (_config.TryGetProperty(property, out var element))
        {
            return element.GetString() ?? defaultValue;
        }
        return defaultValue;
    }

    private List<string> GetConfigArray(string property)
    {
        var list = new List<string>();
        if (_config.TryGetProperty(property, out var element) && element.ValueKind == JsonValueKind.Array)
        {
            foreach (var item in element.EnumerateArray())
            {
                list.Add(item.GetString() ?? "");
            }
        }
        return list;
    }

    private string GetJsonString(JsonElement element, string property, string defaultValue = "")
    {
        if (element.TryGetProperty(property, out var prop))
        {
            return prop.GetString() ?? defaultValue;
        }
        return defaultValue;
    }

    // Fallback methods
    private string GetFallbackOpeningLine()
    {
        var fallbacks = new Dictionary<string, string>
        {
            ["hero"] = "A hero emerges from the darkness.",
            ["shadow"] = "The shadows remember what you forget.",
            ["ambition"] = "Ambition flows upward, defying gravity."
        };
        return fallbacks.GetValueOrDefault(PersonaId, "Welcome to the spiral.");
    }

    private string GetFallbackNarrative()
    {
        var fallbacks = new Dictionary<string, string>
        {
            ["hero"] = "The hero's journey continues, filled with challenges and revelations.",
            ["shadow"] = "The shadows deepen, revealing hidden truths and forgotten memories.",
            ["ambition"] = "Ambition drives progress, pushing boundaries and achieving the impossible."
        };
        return fallbacks.GetValueOrDefault(PersonaId, "The narrative unfolds...");
    }

    private List<ChoiceOption> GetFallbackChoices()
    {
        return new List<ChoiceOption>
        {
            new ChoiceOption { Id = "continue", Text = "CONTINUE", Description = "Continue the journey" },
            new ChoiceOption { Id = "reflect", Text = "REFLECT", Description = "Take a moment to reflect" },
            new ChoiceOption { Id = "question", Text = "QUESTION", Description = "Ask a question" }
        };
    }
}