// <copyright file="NarrativeContentLoader.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using Godot;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OmegaSpiral.Source.Stages.Stage1;

/// <summary>
/// Loads Stage 1 narrative content from opening.json.
/// Deserializes JSON into strongly-typed C# objects for data-driven beat rendering.
/// </summary>
public class NarrativeContentLoader
{
    /// <summary>
    /// Loads the opening.json file and returns the parsed narrative document.
    /// </summary>
    /// <param name="jsonPath">Path to opening.json (e.g., "res://source/stages/stage_1/opening.json")</param>
    /// <returns>Parsed NarrativeDocument, or null if loading failed.</returns>
    public NarrativeDocument? LoadNarrativeContent(string jsonPath)
    {
        if (!ResourceLoader.Exists(jsonPath))
        {
            GD.PrintErr($"[NarrativeContentLoader] File not found: {jsonPath}");
            return null;
        }

        try
        {
            var jsonText = Godot.FileAccess.GetFileAsString(jsonPath);

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                WriteIndented = true,
            };

            var document = JsonSerializer.Deserialize<NarrativeDocument>(jsonText, options);

            if (document == null)
            {
                GD.PrintErr($"[NarrativeContentLoader] Failed to deserialize {jsonPath}");
                return null;
            }

            GD.Print($"[NarrativeContentLoader] Successfully loaded narrative content from {jsonPath}");
            return document;
        }
        catch (JsonException jsonEx)
        {
            GD.PrintErr($"[NarrativeContentLoader] JSON parsing error in {jsonPath}: {jsonEx.Message}");
            return null;
        }
        catch (System.Exception ex)
        {
            GD.PrintErr($"[NarrativeContentLoader] Error loading {jsonPath}: {ex.Message}");
            return null;
        }
    }
}
