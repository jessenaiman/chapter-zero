// <copyright file="Stage1NarrativeScript.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Godot;
using OmegaSpiral.Source.Narrative;

namespace OmegaSpiral.Source.Stages.Stage1;

/// <summary>
/// Sequential narrative script for Stage 1.
/// Contains an ordered list of content blocks to be rendered sequentially.
/// </summary>
public class Stage1NarrativeScript
{
    /// <summary>
    /// Gets or sets the script type identifier.
    /// </summary>
    [JsonPropertyName("type")]
    public string Type { get; set; } = "narrative_script";

    /// <summary>
    /// Gets or sets the script metadata.
    /// </summary>
    [JsonPropertyName("metadata")]
    public ScriptMetadata Metadata { get; set; } = new();

    /// <summary>
    /// Gets or sets the sequential list of content blocks.
    /// These are rendered in order from index 0 to N.
    /// </summary>
    [JsonPropertyName("blocks")]
    public List<ContentBlock> Blocks { get; set; } = new();

    /// <summary>
    /// Loads a narrative script from a JSON file.
    /// </summary>
    /// <param name="jsonPath">Path to the JSON file (e.g., "res://source/stages/stage_1_ghost/ghost.json").</param>
    /// <returns>The loaded narrative script.</returns>
    public static async Task<Stage1NarrativeScript?> LoadFromJsonAsync(string jsonPath)
    {
        using var file = FileAccess.Open(jsonPath, FileAccess.ModeFlags.Read);
        if (file == null)
        {
            GD.PrintErr($"[Stage1NarrativeScript] Failed to open JSON file: {jsonPath}");
            return null;
        }

        var jsonText = file.GetAsText();
        file.Close();

        try
        {
            return JsonSerializer.Deserialize<Stage1NarrativeScript>(jsonText);
        }
        catch (JsonException ex)
        {
            GD.PrintErr($"[Stage1NarrativeScript] JSON parsing error: {ex.Message}");
            return null;
        }
    }
}

/// <summary>
/// Metadata about the narrative script.
/// </summary>
public class ScriptMetadata
{
    [JsonPropertyName("iteration")]
    public string Iteration { get; set; } = string.Empty;

    [JsonPropertyName("iterationFallback")]
    public int IterationFallback { get; set; }

    [JsonPropertyName("previousAttempt")]
    public string PreviousAttempt { get; set; } = string.Empty;

    [JsonPropertyName("interface")]
    public string Interface { get; set; } = string.Empty;

    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;
}

/// <summary>
/// A single content block in the narrative script.
/// Can be a narrative, choice, glitch effect, or other block type.
/// </summary>
public class ContentBlock
{
    /// <summary>
    /// Gets or sets the block type (e.g., "narrative", "choice", "glitch").
    /// </summary>
    [JsonPropertyName("blockType")]
    public string BlockType { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets narrative lines (for narrative/monologue blocks).
    /// </summary>
    [JsonPropertyName("lines")]
    public List<string>? Lines { get; set; }

    /// <summary>
    /// Gets or sets the question prompt (for choice blocks).
    /// </summary>
    [JsonPropertyName("prompt")]
    public string? Prompt { get; set; }

    /// <summary>
    /// Gets or sets the question context/clarification (for choice blocks).
    /// </summary>
    [JsonPropertyName("context")]
    public string? Context { get; set; }

    /// <summary>
    /// Gets or sets the choice options (for choice blocks).
    /// </summary>
    [JsonPropertyName("options")]
    public List<ChoiceOption>? Options { get; set; }

    /// <summary>
    /// Gets or sets cinematic timing hint (e.g., "slow_burn", "rapid").
    /// </summary>
    [JsonPropertyName("timing")]
    public string? Timing { get; set; }

    /// <summary>
    /// Gets or sets whether to fade to stable after this block (for glitch blocks).
    /// </summary>
    [JsonPropertyName("fadeToStable")]
    public bool? FadeToStable { get; set; }

    /// <summary>
    /// Gets or sets visual preset to apply (e.g., "boot_sequence", "secret_reveal").
    /// </summary>
    [JsonPropertyName("visualPreset")]
    public string? VisualPreset { get; set; }
}
