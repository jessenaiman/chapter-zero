using Godot;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Nodes;

public partial class JsonSchemaValidator : Node
{
    public bool ValidateSchema(JsonNode jsonData, string schemaPath)
    {
        try
        {
            // Load the schema from the specified path
            string schemaText = File.ReadAllText(schemaPath);
            JsonNode schemaNode = JsonNode.Parse(schemaText);
            
            // Perform basic validation - check if required fields exist based on schema
            if (schemaNode?["type"]?.ToString() is string schemaType)
            {
                if (jsonData?["type"]?.ToString() is string dataType)
                {
                    if (schemaType != dataType)
                    {
                        GD.PrintErr($"Schema validation error: Expected type '{schemaType}', got '{dataType}'");
                        return false;
                    }
                }
            }
            
            // Validate required properties if specified in schema
            if (schemaNode?["required"] != null)
            {
                JsonArray requiredProps = (JsonArray)schemaNode["required"];
                foreach (var prop in requiredProps)
                {
                    string propName = prop.ToString();
                    if (jsonData?[propName] == null)
                    {
                        GD.PrintErr($"Schema validation error: Required property '{propName}' is missing");
                        return false;
                    }
                }
            }
            
            return true;
        }
        catch (Exception ex)
        {
            GD.PrintErr($"Schema validation error: {ex.Message}");
            return false;
        }
    }
    
    public string GetLastErrorMessage()
    {
        // This is a basic implementation - in a more complete version, we'd store the actual error
        return "Validation error occurred. Please check schema and data format.";
    }
}