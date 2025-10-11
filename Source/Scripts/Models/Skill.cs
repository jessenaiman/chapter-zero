namespace OmegaSpiral.Source.Scripts.Models
{
    using Godot;

    /// <summary>
    /// Represents a single skill with a name, level, and experience.
    /// </summary>
    public class Skill
    {
        /// <summary>
        /// Gets or sets the unique identifier for the skill.
        /// </summary>
        [Export]
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the display name of the skill.
        /// </summary>
        [Export]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the description of the skill.
        /// </summary>
        [Export]
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the current level of the skill.
        /// </summary>
        [Export]
        public int Level { get; set; } = 1;

        /// <summary>
        /// Gets or sets the current experience points for the skill.
        /// </summary>
        [Export]
        public int Experience { get; set; }

        /// <summary>
        /// Gets or sets the maximum level for the skill.
        /// </summary>
        [Export]
        public int MaxLevel { get; set; } = 10;

        /// <summary>
        /// Gets or sets the category of the skill (e.g., "Combat", "Magic", "Crafting").
        /// </summary>
        [Export]
        public string Category { get; set; } = string.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="Skill"/> class.
        /// </summary>
        public Skill()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Skill"/> class with a name and category.
        /// </summary>
        /// <param name="name">The name of the skill.</param>
        /// <param name="category">The category of the skill.</param>
        public Skill(string name, string category)
        {
            Name = name;
            Category = category;
            Id = name.ToLower().Replace(" ", "_");
        }
    }
}
