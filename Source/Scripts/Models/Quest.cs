namespace OmegaSpiral.Source.Scripts.Models
{
    using System;
    using System.Collections.Generic;
    using Godot;

    /// <summary>
    /// Represents a quest in the game.
    /// </summary>
    public partial class Quest : Resource
    {
        /// <summary>
        /// Gets or sets the unique identifier for this quest.
        /// </summary>
        [Export]
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the quest title.
        /// </summary>
        [Export]
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the quest description.
        /// </summary>
        [Export]
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the quest status.
        /// </summary>
        [Export]
        public string Status { get; set; } = "NotStarted";

        /// <summary>
        /// Gets or sets the quest objectives.
        /// </summary>
        public List<string> Objectives { get; set; } = new();

        /// <summary>
        /// Gets or sets the completed objectives.
        /// </summary>
        public List<string> CompletedObjectives { get; set; } = new();

        /// <summary>
        /// Gets or sets the quest rewards.
        /// </summary>
        public Dictionary<string, int> Rewards { get; set; } = new();

        /// <summary>
        /// Gets or sets the quest level requirement.
        /// </summary>
        [Export]
        public int RequiredLevel { get; set; }

        /// <summary>
        /// Gets or sets whether the quest is a main story quest.
        /// </summary>
        [Export]
        public bool IsMainQuest { get; set; }
    }
}
