// <copyright file="NarrativeScriptJsonLoader.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

namespace OmegaSpiral.Source.Backend.Narrative;

using Godot;
using Newtonsoft.Json;

/// <summary>
/// Generic JSON narrative script loader.
/// Provides stage-agnostic deserialization of JSON narrative files into <see cref="NarrativeScriptRoot"/> instances.
/// Replaces YamlDotNet-based loading with Newtonsoft.Json for unified JSON support.
/// </summary>
public static class NarrativeScriptJsonLoader
{
    /// <summary>
    /// Loads a JSON narrative script from the specified file path.
    /// </summary>
    /// <param name="jsonFilePath">The Godot resource path to the JSON file (e.g., "res://source/frontend/stages/stage_1_ghost/ghost.json").</param>
    /// <returns>A deserialized <see cref="NarrativeScriptRoot"/> instance.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the file cannot be opened or read, or deserialization fails.</exception>
    public static NarrativeScriptRoot LoadJsonScript(string jsonFilePath)
    {
        using var file = Godot.FileAccess.Open(jsonFilePath, Godot.FileAccess.ModeFlags.Read);
        if (file == null)
        {
            throw new InvalidOperationException($"Failed to open {jsonFilePath}");
        }

        var jsonContent = file.GetAsText();

        try
        {
            var script = JsonConvert.DeserializeObject<NarrativeScriptRoot>(jsonContent);
            if (script == null)
            {
                throw new InvalidOperationException($"Failed to deserialize JSON from {jsonFilePath}: result was null");
            }

            // Validate required fields
            if (string.IsNullOrWhiteSpace(script.Title))
            {
                GD.PrintErr($"[NarrativeScriptJsonLoader] Warning: 'title' is missing or empty in {jsonFilePath}");
            }

            if (script.Scenes == null || script.Scenes.Count == 0)
            {
                GD.PrintErr($"[NarrativeScriptJsonLoader] Warning: 'scenes' is empty or missing in {jsonFilePath}");
            }

            return script;
        }
        catch (JsonException ex)
        {
            GD.PrintErr($"JSON deserialization failed for {jsonFilePath}: {ex.Message}");
            throw new InvalidOperationException($"Failed to deserialize JSON from {jsonFilePath}", ex);
        }
        catch (Exception ex)
        {
            GD.PrintErr($"Unexpected error loading JSON from {jsonFilePath}: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Loads a JSON narrative script with stage-specific extensions.
    /// </summary>
    /// <typeparam name="T">The stage-specific script type that extends <see cref="NarrativeScriptRoot"/>.</typeparam>
    /// <param name="jsonFilePath">The Godot resource path to the JSON file.</param>
    /// <returns>A deserialized instance of the stage-specific script type.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the file cannot be opened or read.</exception>
    public static T LoadJsonScript<T>(string jsonFilePath)
        where T : NarrativeScriptRoot
    {
        using var file = Godot.FileAccess.Open(jsonFilePath, Godot.FileAccess.ModeFlags.Read);
        if (file == null)
        {
            throw new InvalidOperationException($"Failed to open {jsonFilePath}");
        }

        var jsonContent = file.GetAsText();

        try
        {
            var script = JsonConvert.DeserializeObject<T>(jsonContent);
            if (script == null)
            {
                throw new InvalidOperationException($"Failed to deserialize JSON from {jsonFilePath}: result was null");
            }

            return script;
        }
        catch (JsonException ex)
        {
            GD.PrintErr($"JSON deserialization failed for {jsonFilePath}: {ex.Message}");
            throw new InvalidOperationException($"Failed to deserialize JSON from {jsonFilePath}", ex);
        }
    }
}
