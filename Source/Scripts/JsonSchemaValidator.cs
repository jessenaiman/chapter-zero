using Godot;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;

namespace OmegaSpiral.Source.Scripts
{
    /// <summary>
    /// JSON schema validation system that validates scene data against predefined schemas.
    /// Provides detailed error messages to help developers debug content issues during demo development.
    /// </summary>
    public class JsonSchemaValidator : Node
    {
        /// <summary>
        /// Validates a JSON object against a schema file.
        /// </summary>
        /// <param name="jsonData">The JSON data to validate</param>
        /// <param name="schemaPath">Path to the schema file</param>
        /// <param name="errorMessage">Output error message if validation fails</param>
        /// <returns>True if validation passes, false otherwise</returns>
        public bool Validate(JObject jsonData, string schemaPath, out string errorMessage)
        {
            errorMessage = string.Empty;

            try
            {
                // Check if schema file exists
                if (!File.Exists(schemaPath))
                {
                    errorMessage = $"Schema file not found: {schemaPath}";
                    GD.PrintErr(errorMessage);
                    return false;
                }

                // Load schema
                string schemaJson = File.ReadAllText(schemaPath);
                JObject schema = JObject.Parse(schemaJson);

                // Perform basic validation
                if (!ValidateAgainstSchema(jsonData, schema, out errorMessage))
                {
                    GD.PrintErr($"JSON validation failed for {schemaPath}: {errorMessage}");
                    return false;
                }

                GD.Print($"JSON validation passed for {schemaPath}");
                return true;
            }
            catch (Exception ex)
            {
                errorMessage = $"Error validating JSON: {ex.Message}";
                GD.PrintErr(errorMessage);
                return false;
            }
        }

        /// <summary>
        /// Validates JSON data against a schema.
        /// </summary>
        /// <param name="data">JSON data to validate</param>
        /// <param name="schema">Schema to validate against</param>
        /// <param name="errorMessage">Output error message if validation fails</param>
        /// <returns>True if validation passes, false otherwise</returns>
        private bool ValidateAgainstSchema(JObject data, JObject schema, out string errorMessage)
        {
            errorMessage = string.Empty;

            // Check required properties
            if (schema.ContainsKey("required"))
            {
                JArray requiredProps = (JArray)schema["required"];
                foreach (string prop in requiredProps)
                {
                    if (!data.ContainsKey(prop))
                    {
                        errorMessage = $"Missing required property: {prop}";
                        return false;
                    }
                }
            }

            // Check properties
            if (schema.ContainsKey("properties") && data.ContainsKey("properties"))
            {
                JObject schemaProps = (JObject)schema["properties"];
                JObject dataProps = (JObject)data["properties"];

                foreach (var prop in schemaProps)
                {
                    string propName = prop.Key;
                    JObject propSchema = (JObject)prop.Value;

                    // Check if property exists in data
                    if (dataProps.ContainsKey(propName))
                    {
                        JToken dataValue = dataProps[propName];

                        // Validate property type
                        if (propSchema.ContainsKey("type"))
                        {
                            string expectedType = (string)propSchema["type"];
                            if (!ValidateType(dataValue, expectedType, out string typeError))
                            {
                                errorMessage = $"Property '{propName}' validation failed: {typeError}";
                                return false;
                            }
                        }
                    }
                    else if (propSchema.ContainsKey("required") && (bool)propSchema["required"])
                    {
                        errorMessage = $"Required property '{propName}' is missing";
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Validates that a JSON token matches the expected type.
        /// </summary>
        /// <param name="token">JSON token to validate</param>
        /// <param name="expectedType">Expected type as string</param>
        /// <param name="errorMessage">Output error message if validation fails</param>
        /// <returns>True if type matches, false otherwise</returns>
        private bool ValidateType(JToken token, string expectedType, out string errorMessage)
        {
            errorMessage = string.Empty;

            switch (expectedType.ToLower())
            {
                case "string":
                    if (token.Type != JTokenType.String)
                    {
                        errorMessage = $"Expected string, got {token.Type}";
                        return false;
                    }
                    break;

                case "number":
                    if (token.Type != JTokenType.Float && token.Type != JTokenType.Integer)
                    {
                        errorMessage = $"Expected number, got {token.Type}";
                        return false;
                    }
                    break;

                case "integer":
                    if (token.Type != JTokenType.Integer)
                    {
                        errorMessage = $"Expected integer, got {token.Type}";
                        return false;
                    }
                    break;

                case "boolean":
                    if (token.Type != JTokenType.Boolean)
                    {
                        errorMessage = $"Expected boolean, got {token.Type}";
                        return false;
                    }
                    break;

                case "object":
                    if (token.Type != JTokenType.Object)
                    {
                        errorMessage = $"Expected object, got {token.Type}";
                        return false;
                    }
                    break;

                case "array":
                    if (token.Type != JTokenType.Array)
                    {
                        errorMessage = $"Expected array, got {token.Type}";
                        return false;
                    }
                    break;

                default:
                    errorMessage = $"Unknown type: {expectedType}";
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Gets detailed error information for JSON validation failures.
        /// </summary>
        /// <param name="jsonData">The JSON data that failed validation</param>
        /// <param name="schemaPath">Path to the schema file</param>
        /// <returns>Detailed error information</returns>
        public string GetDetailedErrorInfo(JObject jsonData, string schemaPath)
        {
            try
            {
                string schemaJson = File.ReadAllText(schemaPath);
                JObject schema = JObject.Parse(schemaJson);

                var errors = new List<string>();

                // Check for missing required properties
                if (schema.ContainsKey("required"))
                {
                    JArray requiredProps = (JArray)schema["required"];
                    foreach (string prop in requiredProps)
                    {
                        if (!jsonData.ContainsKey(prop))
                        {
                            errors.Add($"Missing required property: {prop}");
                        }
                    }
                }

                // Check property types
                if (schema.ContainsKey("properties"))
                {
                    JObject schemaProps = (JObject)schema["properties"];
                    
                    foreach (var prop in schemaProps)
                    {
                        string propName = prop.Key;
                        JObject propSchema = (JObject)prop.Value;

                        if (jsonData.ContainsKey(propName))
                        {
                            JToken dataValue = jsonData[propName];
                            
                            if (propSchema.ContainsKey("type"))
                            {
                                string expectedType = (string)propSchema["type"];
                                if (!ValidateType(dataValue, expectedType, out string typeError))
                                {
                                    errors.Add($"Property '{propName}': {typeError}");
                                }
                            }
                        }
                    }
                }

                return string.Join("\n", errors);
            }
            catch (Exception ex)
            {
                return $"Error getting detailed error info: {ex.Message}";
            }
        }
    }
}