namespace OmegaSpiral.Source.Scripts.Interfaces
{
    using System.Threading.Tasks;
    using OmegaSpiral.Source.Scripts.Models;

    /// <summary>
    /// Service interface for handling narrative scene loading, schema parsing, and step execution.
    /// Abstracts the complexity of JSON schema management and step-based narrative flow.
    /// </summary>
    public interface INarrativeSceneService
    {
        /// <summary>
        /// Loads a narrative scene schema from the specified file path.
        /// </summary>
        /// <param name="schemaPath">The path to the JSON schema file.</param>
        /// <returns><see langword="true"/> if the schema was successfully loaded; otherwise, <see langword="false"/>.</returns>
        bool LoadSceneSchema(string schemaPath);

        /// <summary>
        /// Gets the current scene schema.
        /// </summary>
        SceneSchema? CurrentSchema { get; }

        /// <summary>
        /// Gets the current step in the narrative sequence.
        /// </summary>
        SceneStep? CurrentStep { get; }

        /// <summary>
        /// Gets the index of the current step.
        /// </summary>
        int CurrentStepIndex { get; }

        /// <summary>
        /// Advances to the next step in the narrative sequence.
        /// </summary>
        /// <returns><see langword="true"/> if advanced to next step; <see langword="false"/> if sequence is complete.</returns>
        bool AdvanceToNextStep();

        /// <summary>
        /// Resets the narrative sequence to the beginning.
        /// </summary>
        void ResetToStart();

        /// <summary>
        /// Checks if the narrative sequence has more steps to execute.
        /// </summary>
        /// <returns><see langword="true"/> if there are more steps; otherwise, <see langword="false"/>.</returns>
        bool HasMoreSteps();

        /// <summary>
        /// Evaluates a variable from the scene context.
        /// </summary>
        /// <param name="variableName">The name of the variable to evaluate.</param>
        /// <returns>The variable value, or <see langword="null"/> if not found.</returns>
        string? EvaluateVariable(string variableName);

        /// <summary>
        /// Sets a variable in the scene context.
        /// </summary>
        /// <param name="variableName">The name of the variable to set.</param>
        /// <param name="value">The value to assign.</param>
        void SetVariable(string variableName, string value);
    }
}
