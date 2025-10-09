using Godot;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Nodes;

public partial class SceneLoader : Node
{
    private JsonSchemaValidator _schemaValidator;
    
    public override void _Ready()
    {
        _schemaValidator = new JsonSchemaValidator();
    }
    
    public JsonNode LoadSceneData(string scenePath)
    {
        try
        {
            string dataPath = $"res://Source/Data/scenes/{scenePath}/data.json";
            if (!File.Exists(dataPath))
            {
                GD.PrintErr($"Scene data file does not exist: {dataPath}");
                return null;
            }
            
            string jsonText = File.ReadAllText(dataPath);
            JsonNode sceneData = JsonNode.Parse(jsonText);
            
            // Validate against schema
            string schemaPath = $"res://Source/Data/scenes/{scenePath}/schema.json";
            if (!File.Exists(schemaPath))
            {
                GD.PrintErr($"Schema file does not exist: {schemaPath}");
                return null;
            }
            
            bool isValid = _schemaValidator.ValidateSchema(sceneData, schemaPath);
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
    
    public PackedScene LoadScene(string scenePath)
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
    
    public bool ValidateSceneData(JsonNode sceneData, string scenePath)
    {
        try
        {
            string schemaPath = $"res://Source/Data/scenes/{scenePath}/schema.json";
            if (!File.Exists(schemaPath))
            {
                GD.PrintErr($"Schema file does not exist: {schemaPath}");
                return false;
            }
            
            return _schemaValidator.ValidateSchema(sceneData, schemaPath);
        }
        catch (Exception ex)
        {
            GD.PrintErr($"Error validating scene data: {ex.Message}");
            return false;
        }
    }
}