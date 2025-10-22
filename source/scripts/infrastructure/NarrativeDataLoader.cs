// <copyright file="NarrativeDataLoader.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using Godot;
using System.Text.Json;

namespace OmegaSpiral.Source.Scripts.Infrastructure;

/// <summary>
/// Generic loader for narrative JSON files.
/// Parses JSON into strongly-typed C# objects for stage-specific narrative content.
/// </summary>
public class NarrativeDataLoader
{
    /// <summary>
    /// Loads and deserializes a narrative JSON file.
    /// </summary>
    /// <typeparam name="T">The type to deserialize into (e.g., Stage1NarrativeData).</typeparam>
    /// <param name="jsonPath">Path to the JSON file (e.g., "res://source/stages/stage_1/opening.json").</param>
    /// <returns>Deserialized narrative data, or null if loading failed.</returns>
    public T? LoadNarrativeData<T>(string jsonPath) where T : class
    {
        if (!ResourceLoader.Exists(jsonPath))
        {
            GD.PrintErr($"[NarrativeDataLoader] Narrative file not found: {jsonPath}");
            return null;
        }

        try
        {
            var jsonText = Godot.FileAccess.GetFileAsString(jsonPath);
            
            if (string.IsNullOrWhiteSpace(jsonText))
            {
                GD.PrintErr($"[NarrativeDataLoader] Narrative file is empty: {jsonPath}");
                return null;
            }

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                ReadCommentHandling = JsonCommentHandling.Skip,
                AllowTrailingCommas = true,
            };

            var data = JsonSerializer.Deserialize<T>(jsonText, options);

            if (data == null)
            {
                GD.PrintErr($"[NarrativeDataLoader] Failed to deserialize narrative data from: {jsonPath}");
                return null;
            }

            GD.Print($"[NarrativeDataLoader] Successfully loaded narrative data from: {jsonPath}");
            return data;
        }
        catch (JsonException ex)
        {
            GD.PrintErr($"[NarrativeDataLoader] JSON parsing error in {jsonPath}: {ex.Message}");
            return null;
        }
        catch (System.Exception ex)
        {
            GD.PrintErr($"[NarrativeDataLoader] Unexpected error loading {jsonPath}: {ex.Message}");
            return null;
        }
    }
}
