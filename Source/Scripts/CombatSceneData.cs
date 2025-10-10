// <copyright file="CombatSceneData.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace OmegaSpiral.Source.Scripts
{
    using System.Collections.Generic;

    /// <summary>
    /// Data model for turn-based pixel combat scenes.
    /// </summary>
    public class CombatSceneData
    {
        /// <summary>
        /// Gets or sets the scene type identifier.
        /// </summary>
        public string Type { get; set; } = "pixel_combat";

        /// <summary>
        /// Gets or sets the path to the player sprite texture.
        /// </summary>
        public string? PlayerSprite { get; set; }

        /// <summary>
        /// Gets or sets the enemy data for this combat encounter.
        /// </summary>
        public CombatEnemy? Enemy { get; set; }

        /// <summary>
        /// Gets or sets the list of available combat actions.
        /// </summary>
        public List<string> Actions { get; set; } = new ();

        /// <summary>
        /// Gets or sets the background music for the combat scene.
        /// </summary>
        public string? Music { get; set; }

        /// <summary>
        /// Gets or sets the text displayed when the player wins.
        /// </summary>
        public string? VictoryText { get; set; }
    }

    /// <summary>
    /// Represents an enemy in combat with stats and behavior.
    /// </summary>
    public class CombatEnemy
    {
        /// <summary>
        /// Gets or sets the enemy's name.
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Gets or sets the enemy's current hit points.
        /// </summary>
        public int HP { get; set; }

        /// <summary>
        /// Gets or sets the enemy's maximum hit points.
        /// </summary>
        public int MaxHP { get; set; }

        /// <summary>
        /// Gets or sets the enemy's attack power.
        /// </summary>
        public int Attack { get; set; }

        /// <summary>
        /// Gets or sets the enemy's defense value.
        /// </summary>
        public int Defense { get; set; }

        /// <summary>
        /// Gets or sets the path to the enemy sprite texture.
        /// </summary>
        public string? Sprite { get; set; }

        /// <summary>
        /// Gets or sets the list of attack patterns this enemy can use.
        /// </summary>
        public List<string> AttackPatterns { get; set; } = new ();
    }

    /// <summary>
    /// Represents a combat action (FIGHT, MAGIC, ITEM, RUN).
    /// </summary>
    public class CombatAction
    {
        /// <summary>
        /// Gets or sets the name of the combat action.
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Gets or sets the type of the combat action.
        /// </summary>
        public string? Type { get; set; }

        /// <summary>
        /// Gets or sets the power level of the combat action.
        /// </summary>
        public int Power { get; set; }

        /// <summary>
        /// Gets or sets the description of the combat action.
        /// </summary>
        public string? Description { get; set; }
    }
}
