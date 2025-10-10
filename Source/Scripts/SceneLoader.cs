// <copyright file="SceneLoader.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Nodes;
using Godot;

public partial class SceneLoader : Node
{
    private JsonSchemaValidator? schemaValidator;

    /// <inheritdoc/>
    public override void _Ready()
    {
        this.schemaValidator = new JsonSchemaValidator();
    }

    public static JsonNode? LoadSceneData(string scenePath)
    {
        try
        {
            string dataPath = $"res://Source/Data/scenes/{scenePath}/data.json";
            if (!Godot.FileAccess.FileExists(dataPath))
            {
                GD.PrintErr($"Scene data file does not exist: {dataPath}");
                return null;
            }

            string jsonText = File.ReadAllText(dataPath);
            JsonNode? sceneData = JsonNode.Parse(jsonText);
            if (sceneData == null)
            {
                GD.PrintErr($"Failed to parse JSON data from: {dataPath}");
                return null;
            }

            // Validate against schema
            string schemaPath = $"res://Source/Data/scenes/{scenePath}/schema.json";
            if (!Godot.FileAccess.FileExists(schemaPath))
            {
                // Try the specs contracts directory as fallback
                schemaPath = $"res://specs/004-implement-omega-spiral/contracts/{scenePath}_schema.json";
                if (!Godot.FileAccess.FileExists(schemaPath))
                {
                    GD.PrintErr($"Schema file does not exist: {schemaPath}");
                    return null;
                }
            }

            bool isValid = JsonSchemaValidator.ValidateSchema(sceneData, schemaPath);
            if (!isValid)
            {
                GD.PrintErr($"Scene data validation failed for: {dataPath}");
                return null;
            }

            return sceneData;
        }
        catch (Exception ex)
        {
            GD.PrintErr($"Error loading scene data: {ex.Message}");
            return null;
        }
    }

    public static PackedScene? LoadScene(string scenePath)
    {
        try
        {
            string resourcePath = $"res://Source/Scenes/{scenePath}.tscn";
            Resource sceneResource = ResourceLoader.Load(resourcePath);

            if (sceneResource is PackedScene packedScene)
            {
                return packedScene;
            }
            else
            {
                GD.PrintErr($"Failed to load scene: {resourcePath}");
                return null;
            }
        }
        catch (Exception ex)
        {
            GD.PrintErr($"Error loading scene: {ex.Message}");
            return null;
        }
    }

    public static bool ValidateSceneData(JsonNode sceneData, string scenePath)
    {
        try
        {
            string schemaPath = $"res://Source/Data/scenes/{scenePath}/schema.json";
            if (!Godot.FileAccess.FileExists(schemaPath))
            {
                // Try the specs contracts directory as fallback
                schemaPath = $"res://specs/004-implement-omega-spiral/contracts/{scenePath}_schema.json";
                if (!Godot.FileAccess.FileExists(schemaPath))
                {
                    GD.PrintErr($"Schema file does not exist: {schemaPath}");
                    return false;
                }
            }

            return JsonSchemaValidator.ValidateSchema(sceneData, schemaPath);
        }
        catch (Exception ex)
        {
            GD.PrintErr($"Error validating scene data: {ex.Message}");
            return false;
        }
    }
}
