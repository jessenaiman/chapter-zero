namespace OmegaSpiral.Source.Scripts.Models
{
    using System.Collections.Generic;

    /// <summary>
    /// Schema for narrative scene definitions loaded from JSON.
    /// </summary>
    public partial class SceneSchema
    {
        /// <summary>
        /// Gets or sets the unique identifier for the scene referenced by save data and transitions.
        /// </summary>
        public string SceneId { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the human-readable title of the scene.
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the ordered steps executed by the terminal flow.
        /// </summary>
        public List<SceneStep> Steps { get; set; } = new();
    }

    /// <summary>
    /// Individual scene step description defining a single narrative action.
    /// </summary>
    public partial class SceneStep
    {
        /// <summary>
        /// Gets or sets the unique identifier of the step used in branching.
        /// </summary>
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the step type (dialogue, choice, input, or effect).
        /// </summary>
        public string Type { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the text lines rendered during dialogue steps.
        /// </summary>
        public List<string> Lines { get; set; } = new();

        /// <summary>
        /// Gets or sets the prompt shown above choice or input widgets.
        /// </summary>
        public string Prompt { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the list of available choice options for choice steps.
        /// </summary>
        public List<ChoiceOption> Options { get; set; } = new();

        /// <summary>
        /// Gets or sets the effect identifier for effect steps.
        /// </summary>
        public string Effect { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the effect duration in milliseconds.
        /// </summary>
        public int Duration { get; set; }

        /// <summary>
        /// Gets or sets the delay in milliseconds applied after the step completes.
        /// </summary>
        public int Delay { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the next step to execute, or <see langword="null"/> to advance sequentially.
        /// </summary>
        public string? NextStep { get; set; }
    }

    /// <summary>
    /// Choice data used within scene steps to represent player decisions.
    /// </summary>
    public partial class ChoiceOption
    {
        /// <summary>
        /// Gets or sets the unique identifier for the choice.
        /// </summary>
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the label shown to the player for this choice.
        /// </summary>
        public string? Text { get; set; }

        /// <summary>
        /// Gets or sets optional descriptive text displayed beneath the label.
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the legacy story block index associated with the choice.
        /// </summary>
        public int NextBlock { get; set; }

        /// <summary>
        /// Gets or sets the next step identifier to jump to when this option is chosen, or <see langword="null"/> to follow the step default.
        /// </summary>
        public string? NextStep { get; set; }
    }
}
