// <copyright file="CombatSceneData.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using Godot;

namespace OmegaSpiral.Source.Scripts
{
    /// <summary>
    /// Data model for turn-based pixel combat scenes.
    /// </summary>
    public partial class CombatSceneData : Resource
    {
        /// <summary>
        /// Gets or sets the scene type identifier.
        /// </summary>
        [Export]
        public string Type { get; set; } = "pixel_combat";

        /// <summary>
        /// Gets or sets the path to the player sprite texture.
        /// </summary>
        [Export]
        public string? PlayerSprite { get; set; }

        /// <summary>
        /// Gets or sets the enemy data for this combat encounter.
        /// </summary>
        [Export]
        public CombatEnemy? Enemy { get; set; }

        /// <summary>
        /// Gets or sets the list of available combat actions.
        /// </summary>
        [Export]
        public Godot.Collections.Array<string> Actions { get; set; } = new();

        /// <summary>
        /// Gets or sets the background music for the combat scene.
        /// </summary>
        [Export]
        public string? Music { get; set; }

        /// <summary>
        /// Gets or sets the text displayed when the player wins.
        /// </summary>
        [Export]
        public string? VictoryText { get; set; }
    }

    /// <summary>
    /// Represents an enemy in combat with stats and behavior.
    /// </summary>
    public partial class CombatEnemy : Resource
    {
        /// <summary>
        /// Gets or sets the enemy's name.
        /// </summary>
        [Export]
        public string? Name { get; set; }

        /// <summary>
        /// Gets or sets the enemy's current hit points.
        /// </summary>
        [Export]
        public int HP { get; set; }

        /// <summary>
        /// Gets or sets the enemy's maximum hit points.
        /// </summary>
        [Export]
        public int MaxHP { get; set; }

        /// <summary>
        /// Gets or sets the enemy's attack power.
        /// </summary>
        [Export]
        public int Attack { get; set; }

        /// <summary>
        /// Gets or sets the enemy's defense value.
        /// </summary>
        [Export]
        public int Defense { get; set; }

        /// <summary>
        /// Gets or sets the path to the enemy sprite texture.
        /// </summary>
        [Export]
        public string? Sprite { get; set; }

        /// <summary>
        /// Gets or sets the list of attack patterns this enemy can use.
        /// </summary>
        [Export]
        public Godot.Collections.Array<string> AttackPatterns { get; set; } = new();
    }

    /// <summary>
    /// Represents a combat action (FIGHT, MAGIC, ITEM, RUN).
    /// </summary>
    public partial class CombatAction : Resource
    {
        /// <summary>
        /// Gets or sets the name of the combat action.
        /// </summary>
        [Export]
        public string? Name { get; set; }

        /// <summary>
        /// Gets or sets the type of the combat action.
        /// </summary>
        [Export]
        public string? Type { get; set; }

        /// <summary>
        /// Gets or sets the power level of the combat action.
        /// </summary>
        [Export]
        public int Power { get; set; }

        /// <summary>
        /// Gets or sets the description of the combat action.
        /// </summary>
        [Export]
        public string? Description { get; set; }
    }
}
