// <copyright file="NarrativeScriptJsonLoader.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

namespace OmegaSpiral.Source.Backend.Narrative;

using System;
using Godot;
using Newtonsoft.Json;

/// <summary>
/// Narrative script loader that deserializes JSON files into <see cref="StoryScriptRoot"/> instances.
/// </summary>
public static class StoryLoader
{
    /// <summary>
    /// Loads a JSON narrative script from the specified file path.
    /// </summary>
    /// <param name="jsonFilePath">The Godot resource path to the JSON file (e.g., "res://source/frontend/stages/stage_1_ghost/ghost.json").</param>
    /// <returns>A deserialized <see cref="StoryScriptRoot"/> instance.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the file cannot be opened or read, or deserialization fails.</exception>
    public static StoryScriptRoot LoadJsonScript(string jsonFilePath)
    {
        using var file = FileAccess.Open(jsonFilePath, FileAccess.ModeFlags.Read);
        if (file == null)
        {
            throw new InvalidOperationException($"Failed to open {jsonFilePath}");
        }

        var jsonContent = file.GetAsText();

        try
        {
            var script = JsonConvert.DeserializeObject<StoryScriptRoot>(jsonContent);
            if (script == null)
            {
                throw new InvalidOperationException($"Failed to deserialize JSON from {jsonFilePath}: result was null");
            }

            if (string.IsNullOrWhiteSpace(script.Title))
            {
                GD.PrintErr($"[StoryLoader] Warning: 'title' is missing or empty in {jsonFilePath}");
            }

            if (script.Scenes == null || script.Scenes.Count == 0)
            {
                GD.PrintErr($"[StoryLoader] Warning: 'scenes' is empty or missing in {jsonFilePath}");
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
    /// <typeparam name="T">The stage-specific script type that extends <see cref="StoryScriptRoot"/>.</typeparam>
    /// <param name="jsonFilePath">The Godot resource path to the JSON file.</param>
    /// <returns>A deserialized instance of the stage-specific script type.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the file cannot be opened or read.</exception>
    public static T LoadJsonScript<T>(string jsonFilePath)
        where T : StoryScriptRoot
    {
        using var file = FileAccess.Open(jsonFilePath, FileAccess.ModeFlags.Read);
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
