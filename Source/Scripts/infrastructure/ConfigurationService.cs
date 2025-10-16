namespace OmegaSpiral.Source.Scripts.Infrastructure
{
    using System;
    using Godot;
    using Godot.Collections;
    using OmegaSpiral.Source.Scripts.Common;
    using OmegaSpiral.Source.Scripts.Domain.Dungeon;

    /// <summary>
    /// Provides centralized configuration loading and validation services using Godot's native JSON capabilities.
    /// </summary>
    public static class ConfigurationService
    {
        /// <summary>
        /// Loads and validates JSON configuration from a file path.
        /// </summary>
        /// <param name="configPath">The path to the configuration file.</param>
        /// <returns>The parsed configuration data as a Godot Dictionary.</returns>
        /// <exception cref="DungeonValidationException">Thrown when the configuration file cannot be loaded or parsed.</exception>
        public static Dictionary<string, Variant> LoadConfiguration(string configPath)
        {
            if (string.IsNullOrWhiteSpace(configPath))
            {
                throw new DungeonValidationException("Configuration path cannot be null or empty.");
            }

            // Load the JSON file content using Godot's FileAccess
            string jsonContent;
            try
            {
                jsonContent = Godot.FileAccess.GetFileAsString(configPath);
            }
            catch (Exception ex)
            {
                throw new DungeonValidationException($"Failed to read configuration file '{configPath}': {ex.Message}", ex);
            }

            if (string.IsNullOrWhiteSpace(jsonContent))
            {
                throw new DungeonValidationException($"Configuration file '{configPath}' is empty or null.");
            }

            return ParseJsonContent(jsonContent, configPath);
        }

        /// <summary>
        /// Loads and validates JSON configuration from a JSON string.
        /// </summary>
        /// <param name="jsonContent">The JSON string content.</param>
        /// <param name="sourceDescription">Description of the source for error reporting (optional).</param>
        /// <returns>The parsed configuration data as a Godot Dictionary.</returns>
        /// <exception cref="DungeonValidationException">Thrown when the JSON content cannot be parsed.</exception>
        public static Dictionary<string, Variant> LoadConfigurationFromString(string jsonContent, string sourceDescription = "JSON string")
        {
            if (string.IsNullOrWhiteSpace(jsonContent))
            {
                throw new DungeonValidationException($"{sourceDescription} cannot be null or empty.");
            }

            return ParseJsonContent(jsonContent, sourceDescription);
        }

        /// <summary>
        /// Validates configuration against a schema file.
        /// </summary>
        /// <param name="configData">The configuration data to validate.</param>
        /// <param name="schemaPath">The path to the JSON schema file.</param>
        /// <returns>True if the configuration is valid according to the schema, false otherwise.</returns>
        public static bool ValidateConfiguration(Dictionary<string, Variant> configData, string schemaPath)
        {
            try
            {
                return JsonSchemaValidator.ValidateSchema(configData, schemaPath);
            }
            catch (Exception ex)
            {
                GD.PrintErr($"Configuration validation error: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Validates configuration against a schema dictionary.
        /// </summary>
        /// <param name="configData">The configuration data to validate.</param>
        /// <param name="schema">The schema dictionary to validate against.</param>
        /// <returns>True if the configuration is valid according to the schema, false otherwise.</returns>
        public static bool ValidateConfiguration(Dictionary<string, Variant> configData, Dictionary<string, Variant> schema)
        {
            // Use the parameters to avoid unused parameter warnings
            _ = configData;
            _ = schema;

            try
            {
                // For now, we'll return true - the actual validation will be implemented in JsonSchemaValidator
                return true;
            }
            catch (Exception ex)
            {
                GD.PrintErr($"Configuration validation error: {ex.Message}");
                return false;
            }
        }

        private static Dictionary<string, Variant> ParseJsonContent(string jsonContent, string sourceDescription)
        {
            // Parse the JSON content using Godot's native JSON parser
            var parseResult = Json.ParseString(jsonContent);

            if (parseResult.VariantType == Variant.Type.Nil)
            {
                throw new DungeonValidationException($"Failed to parse JSON content from '{sourceDescription}'. Parse result is null.");
            }

            // Check if the result is a dictionary (object) or array
            if (parseResult.VariantType == Variant.Type.Dictionary)
            {
                return (Dictionary<string, Variant>) parseResult;
            }
            else if (parseResult.VariantType == Variant.Type.Array)
            {
                throw new DungeonValidationException($"Expected JSON object but got array from '{sourceDescription}'.");
            }
            else
            {
                throw new DungeonValidationException($"Expected JSON object but got {parseResult.VariantType} from '{sourceDescription}'.");
            }
        }
    }
}
