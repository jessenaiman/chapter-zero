namespace OmegaSpiral.Source.Scripts.Services
{
    using System;
    using System.Collections.Generic;
    using System.Text.Json;
    using Godot;
    using OmegaSpiral.Source.Scripts.Interfaces;
    using OmegaSpiral.Source.Scripts.Models;

    /// <summary>
    /// Default implementation of the narrative scene service.
    /// Manages JSON schema loading, step navigation, and variable context.
    /// </summary>
    public class NarrativeSceneService : INarrativeSceneService
    {
        private readonly JsonSerializerOptions jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            ReadCommentHandling = JsonCommentHandling.Skip,
        };

        private readonly Dictionary<string, string> variables = new();
        private SceneSchema? sceneSchema;
        private int currentStepIndex;

        /// <inheritdoc/>
        public SceneSchema? CurrentSchema => sceneSchema;

        /// <inheritdoc/>
        public SceneStep? CurrentStep =>
            sceneSchema != null && currentStepIndex >= 0 && currentStepIndex < sceneSchema.Steps.Count
                ? sceneSchema.Steps[currentStepIndex]
                : null;

        /// <inheritdoc/>
        public int CurrentStepIndex => currentStepIndex;

        /// <inheritdoc/>
        public bool LoadSceneSchema(string schemaPath)
        {
            if (!FileAccess.FileExists(schemaPath))
            {
                GD.PrintErr($"Scene schema not found: {schemaPath}");
                return false;
            }

            try
            {
                string json = FileAccess.GetFileAsString(schemaPath);
                SceneSchema? schema = JsonSerializer.Deserialize<SceneSchema>(json, jsonOptions);

                if (schema == null)
                {
                    GD.PrintErr("Scene schema parsed as null.");
                    return false;
                }

                sceneSchema = schema;
                currentStepIndex = 0;
                variables.Clear();

                return true;
            }
            catch (Exception ex)
            {
                GD.PrintErr($"Failed to parse scene schema: {ex.Message}");
                return false;
            }
        }

        /// <inheritdoc/>
        public bool AdvanceToNextStep()
        {
            if (sceneSchema == null || !HasMoreSteps())
            {
                return false;
            }

            currentStepIndex++;
            return true;
        }

        /// <inheritdoc/>
        public void ResetToStart()
        {
            currentStepIndex = 0;
            variables.Clear();
        }

        /// <inheritdoc/>
        public bool HasMoreSteps()
        {
            return sceneSchema != null && currentStepIndex < sceneSchema.Steps.Count - 1;
        }

        /// <inheritdoc/>
        public string? EvaluateVariable(string variableName)
        {
            return variables.TryGetValue(variableName, out string? value) ? value : null;
        }

        /// <inheritdoc/>
        public void SetVariable(string variableName, string value)
        {
            variables[variableName] = value;
        }
    }
}
