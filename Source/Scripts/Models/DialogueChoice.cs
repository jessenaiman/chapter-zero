namespace OmegaSpiral.Source.Scripts.Models
{
    using Godot;

    /// <summary>
    /// Represents a dialogue choice option presented to the player.
    /// </summary>
    public class DialogueChoice
    {
        /// <summary>
        /// Gets or sets the unique identifier for the choice.
        /// </summary>
        [Export]
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the text displayed for this choice.
        /// </summary>
        [Export]
        public string Text { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the ID of the next dialogue node to transition to if this choice is selected.
        /// </summary>
        [Export]
        public string NextNodeId { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether this choice is available to the player.
        /// </summary>
        [Export]
        public bool IsAvailable { get; set; } = true;

        /// <summary>
        /// Gets or sets the requirement condition for this choice to be available.
        /// </summary>
        [Export]
        public string Requirement { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the consequence of selecting this choice.
        /// </summary>
        [Export]
        public string Consequence { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether this choice has been selected before.
        /// </summary>
        public bool HasBeenSelected { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DialogueChoice"/> class.
        /// </summary>
        public DialogueChoice()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DialogueChoice"/> class with text and next node ID.
        /// </summary>
        /// <param name="text">The text for the choice.</param>
        /// <param name="nextNodeId">The next node ID to transition to.</param>
        public DialogueChoice(string text, string nextNodeId)
        {
            Text = text;
            NextNodeId = nextNodeId;
            Id = text.ToLower().Replace(" ", "_").Substring(0, System.Math.Min(20, text.Length));
        }
    }
}
