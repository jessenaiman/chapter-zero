using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

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

    private Dictionary<string, DreamweaverPersona> _personas = new();
    private GameState _gameState;
    private Random _random = new();

    public override void _Ready()
    {
        _gameState = GetNode<GameState>("/root/GameState");

        // Initialize the three Dreamweaver personas
        InitializePersonas();

        GD.Print("Dreamweaver System initialized with 3 personas");
    }

    private void InitializePersonas()
    {
        // Load persona configurations from JSON files
        var heroConfig = LoadPersonaConfig("hero");
        var shadowConfig = LoadPersonaConfig("shadow");
        var ambitionConfig = LoadPersonaConfig("ambition");

        if (heroConfig != null)
        {
            _personas["hero"] = new DreamweaverPersona("hero", heroConfig.Value, _gameState);
        }

        if (shadowConfig != null)
        {
            _personas["shadow"] = new DreamweaverPersona("shadow", shadowConfig.Value, _gameState);
        }

        if (ambitionConfig != null)
        {
            _personas["ambition"] = new DreamweaverPersona("ambition", ambitionConfig.Value, _gameState);
        }
    }

    private JsonElement? LoadPersonaConfig(string personaId)
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
                ReadCommentHandling = JsonCommentHandling.Skip
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

    private string ConvertYamlToJson(string yamlText)
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
                    itemBuilder.Append("{");
                    itemBuilder.Append($"\"{parts[0]}\": \"{parts[1]}\"");
                    itemBuilder.Append("}");
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
                    value = value.Trim('\"');
                }
                jsonBuilder.AppendLine($"\"{key}\": \"{value}\",");
                previousIndent = indent;
            }
        }

        ClosePendingArrays(0);
        jsonBuilder.AppendLine("}");

        var result = jsonBuilder.ToString();
    result = System.Text.RegularExpressions.Regex.Replace(result, ",\\n(?=[\\\\}\\]])", "\\n");
        result = result.Replace("}\n}\n", "}\n}");

        return result;
    }

    /// <summary>
    /// Generates dynamic narrative for a specific persona using LLM.
    /// Uses the JSON text as a foundation for the prompt.
    /// </summary>
    public async Task<string> GenerateNarrativeAsync(string personaId, string context = "")
    {
        if (!_personas.ContainsKey(personaId))
        {
            GD.PrintErr($"Unknown persona: {personaId}");
            EmitSignal(SignalName.GenerationError, personaId, "Unknown persona");
            return GetFallbackNarrative(personaId);
        }

        try
        {
            var persona = _personas[personaId];
            var generatedText = await persona.GenerateNarrativeAsync(context);

            EmitSignal(SignalName.NarrativeGenerated, personaId, generatedText);
            return generatedText;
        }
        catch (Exception ex)
        {
            GD.PrintErr($"Failed to generate narrative for {personaId}: {ex.Message}");
            EmitSignal(SignalName.GenerationError, personaId, ex.Message);
            return GetFallbackNarrative(personaId);
        }
    }

    /// <summary>
    /// Gets a random opening line for a persona, enhanced by LLM if available.
    /// </summary>
    public async Task<string> GetOpeningLineAsync(string personaId)
    {
        if (!_personas.ContainsKey(personaId))
        {
            return GetFallbackOpeningLine(personaId);
        }

        try
        {
            var persona = _personas[personaId];
            return await persona.GetOpeningLineAsync();
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
    public async Task<List<ChoiceOption>> GenerateChoicesAsync(string personaId, string context = "")
    {
        if (!_personas.ContainsKey(personaId))
        {
            return GetFallbackChoices(personaId);
        }

        try
        {
            var persona = _personas[personaId];
            return await persona.GenerateChoicesAsync(context);
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
        if (!_personas.ContainsKey(personaId))
        {
            GD.PrintErr($"Cannot activate unknown persona: {personaId}");
            return;
        }

        // Deactivate all other personas
        foreach (var kvp in _personas)
        {
            kvp.Value.IsActive = (kvp.Key == personaId);
        }

        EmitSignal(SignalName.PersonaActivated, personaId);
        GD.Print($"Activated Dreamweaver persona: {personaId}");
    }

    /// <summary>
    /// Gets the currently active persona.
    /// </summary>
    public DreamweaverPersona GetActivePersona()
    {
        return _personas.Values.FirstOrDefault(p => p.IsActive);
    }

    // Fallback methods for when LLM generation fails
    private string GetFallbackNarrative(string personaId)
    {
        var fallbacks = new Dictionary<string, string>
        {
            ["hero"] = "The hero's path calls to you, filled with light and shadow.",
            ["shadow"] = "The shadows whisper secrets that only you can hear.",
            ["ambition"] = "Ambition drives you forward, carving new paths through reality."
        };

        return fallbacks.GetValueOrDefault(personaId, "The narrative continues...");
    }

    private string GetFallbackOpeningLine(string personaId)
    {
        var fallbacks = new Dictionary<string, string>
        {
            ["hero"] = "A hero emerges from the darkness.",
            ["shadow"] = "The shadows remember what you forget.",
            ["ambition"] = "Ambition flows upward, defying gravity."
        };

        return fallbacks.GetValueOrDefault(personaId, "Welcome to the spiral.");
    }

    private List<ChoiceOption> GetFallbackChoices(string personaId)
    {
        return new List<ChoiceOption>
        {
            new ChoiceOption { Id = "continue", Text = "CONTINUE", Description = "Continue the journey" },
            new ChoiceOption { Id = "reflect", Text = "REFLECT", Description = "Take a moment to reflect" },
            new ChoiceOption { Id = "question", Text = "QUESTION", Description = "Ask a question" }
        };
    }
}

/// <summary>
/// Represents a choice option in the narrative.
/// </summary>
public class ChoiceOption
{
    public string Id { get; set; } = "";
    public string Text { get; set; } = "";
    public string Description { get; set; } = "";
}