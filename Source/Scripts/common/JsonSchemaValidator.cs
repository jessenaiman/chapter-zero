namespace OmegaSpiral.Source.Scripts.Common;

// <copyright file="JsonSchemaValidator.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using System;
using Godot;
using Godot.Collections;

/// <summary>
/// Provides functionality to validate JSON data against a specified schema using Godot's native JSON capabilities.
/// </summary>
[GlobalClass]
public partial class JsonSchemaValidator : Node
{
    /// <summary>
    /// Validates the provided JSON data against the schema at the specified path.
    /// </summary>
    /// <param name="jsonData">The JSON data to validate as a Godot Dictionary.</param>
    /// <param name="schemaPath">The file path to the JSON schema.</param>
    /// <returns>True if the data is valid according to the schema, false otherwise.</returns>
    public static bool ValidateSchema(Dictionary<string, Variant> jsonData, string schemaPath)
    {
        try
        {
            // Load the schema from the specified path using Godot's FileAccess
            string schemaText;
            try
            {
                schemaText = Godot.FileAccess.GetFileAsString(schemaPath);
            }
            catch (Exception ex)
            {
                GD.PrintErr($"Failed to read schema file '{schemaPath}': {ex.Message}");
                return false;
            }

            // Parse the schema using Godot's native JSON parser
            var schemaParseResult = Json.ParseString(schemaText);
            if (schemaParseResult.VariantType == Variant.Type.Nil)
            {
                GD.PrintErr($"Failed to parse schema JSON from '{schemaPath}'. Parse result is null.");
                return false;
            }

            if (schemaParseResult.VariantType != Variant.Type.Dictionary)
            {
                GD.PrintErr($"Schema file '{schemaPath}' does not contain a valid JSON object.");
                return false;
            }

            var schema = (Dictionary<string, Variant>) schemaParseResult;

            // Perform basic validation - check if required fields exist based on schema
            if (schema.TryGetValue("type", out var schemaTypeVariant) &&
                schemaTypeVariant.VariantType == Variant.Type.String)
            {
                var schemaType = schemaTypeVariant.As<string>();
                if (jsonData.TryGetValue("type", out var dataTypeVariant) &&
                    dataTypeVariant.VariantType == Variant.Type.String)
                {
                    var dataType = dataTypeVariant.As<string>();
                    if (schemaType != dataType)
                    {
                        GD.PrintErr($"Schema validation error: Expected type '{schemaType}', got '{dataType}'");
                        return false;
                    }
                }
            }

            // Validate required properties if specified in schema
            if (schema.TryGetValue("required", out var requiredVariant) &&
                requiredVariant.VariantType == Variant.Type.Array)
            {
                var requiredArray = (Godot.Collections.Array) requiredVariant;
                foreach (var prop in requiredArray)
                {
                    if (prop.VariantType == Variant.Type.String)
                    {
                        var propName = prop.As<string>();
                        if (!jsonData.ContainsKey(propName))
                        {
                            GD.PrintErr($"Schema validation error: Required property '{propName}' is missing");
                            return false;
                        }
                    }
                }
            }

            // Validate properties structure if specified in schema
            if (schema.TryGetValue("properties", out var propertiesVariant) &&
                propertiesVariant.VariantType == Variant.Type.Dictionary)
            {
                var properties = (Dictionary<string, Variant>) propertiesVariant;
                if (!ValidateProperties(jsonData, properties))
                {
                    return false;
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

    /// <summary>
    /// Validates the provided JSON data against a schema dictionary.
    /// </summary>
    /// <param name="jsonData">The JSON data to validate as a Godot Dictionary.</param>
    /// <param name="schema">The schema dictionary to validate against.</param>
    /// <returns>True if the data is valid according to the schema, false otherwise.</returns>
    public static bool ValidateSchema(Dictionary<string, Variant> jsonData, Dictionary<string, Variant> schema)
    {
        try
        {
            // Perform basic validation - check if required fields exist based on schema
            if (schema.TryGetValue("type", out var schemaTypeVariant) &&
                schemaTypeVariant.VariantType == Variant.Type.String)
            {
                var schemaType = schemaTypeVariant.As<string>();
                if (jsonData.TryGetValue("type", out var dataTypeVariant) &&
                    dataTypeVariant.VariantType == Variant.Type.String)
                {
                    var dataType = dataTypeVariant.As<string>();
                    if (schemaType != dataType)
                    {
                        GD.PrintErr($"Schema validation error: Expected type '{schemaType}', got '{dataType}'");
                        return false;
                    }
                }
            }

            // Validate required properties if specified in schema
            if (schema.TryGetValue("required", out var requiredVariant) &&
                requiredVariant.VariantType == Variant.Type.Array)
            {
                var requiredArray = (Godot.Collections.Array) requiredVariant;
                foreach (var prop in requiredArray)
                {
                    if (prop.VariantType == Variant.Type.String)
                    {
                        var propName = prop.As<string>();
                        if (!jsonData.ContainsKey(propName))
                        {
                            GD.PrintErr($"Schema validation error: Required property '{propName}' is missing");
                            return false;
                        }
                    }
                }
            }

            // Validate properties structure if specified in schema
            if (schema.TryGetValue("properties", out var propertiesVariant) &&
                propertiesVariant.VariantType == Variant.Type.Dictionary)
            {
                var properties = (Dictionary<string, Variant>) propertiesVariant;
                if (!ValidateProperties(jsonData, properties))
                {
                    return false;
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

    /// <summary>
    /// Validates properties against their type definitions in the schema.
    /// </summary>
    /// <param name="data">The data dictionary to validate.</param>
    /// <param name="properties">The properties schema definition.</param>
    /// <returns>True if all properties are valid, false otherwise.</returns>
    private static bool ValidateProperties(Dictionary<string, Variant> data, Dictionary<string, Variant> properties)
    {
        foreach (var property in properties)
        {
            var propName = property.Key;
            if (data.TryGetValue(propName, out var propValue))
            {
                var propSchema = (Dictionary<string, Variant>) property.Value;

                if (propSchema.TryGetValue("type", out var expectedTypeVariant) &&
                    expectedTypeVariant.VariantType == Variant.Type.String)
                {
                    var expectedType = expectedTypeVariant.As<string>();
                    if (!ValidatePropertyType(propValue, expectedType))
                    {
                        GD.PrintErr($"Schema validation error: Property '{propName}' has incorrect type. Expected '{expectedType}'.");
                        return false;
                    }
                }
            }
        }
        return true;
    }

    /// <summary>
    /// Validates a property value against an expected type string.
    /// </summary>
    /// <param name="value">The value to validate.</param>
    /// <param name="expectedType">The expected type string (e.g., "string", "number", "object", "array").</param>
    /// <returns>True if the type matches, false otherwise.</returns>
    private static bool ValidatePropertyType(Variant value, string expectedType)
    {
        return expectedType.ToLowerInvariant() switch
        {
            "string" => value.VariantType == Variant.Type.String,
            "number" => value.VariantType == Variant.Type.Int || value.VariantType == Variant.Type.Float,
            "integer" => value.VariantType == Variant.Type.Int,
            "boolean" => value.VariantType == Variant.Type.Bool,
            "object" => value.VariantType == Variant.Type.Dictionary,
            "array" => value.VariantType == Variant.Type.Array,
            "null" => value.VariantType == Variant.Type.Nil,
            _ => true // Unknown type, consider valid
        };
    }

    /// <summary>
    /// Gets the last error message from the validation process.
    /// </summary>
    /// <returns>A string containing the last error message.</returns>
    public static string GetLastErrorMessage()
    {
        // This is a basic implementation - in a more complete version, we'd store the actual error
        return "Validation error occurred. Please check schema and data format.";
    }
}
