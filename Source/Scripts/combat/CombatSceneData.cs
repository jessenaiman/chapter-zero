// <copyright file="CombatSceneData.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace OmegaSpiral.Source.Scripts
{
    using System.Collections.Generic;
    using YamlDotNet.Serialization;

    /// <summary>
    /// Data model for turn-based pixel combat scenes.
    /// </summary>
    public class CombatSceneData
    {
        /// <summary>
        /// Gets or sets the scene type identifier.
        /// </summary>
        [YamlMember(Alias = "type")]
        public string Type { get; set; } = "pixel_combat";

        /// <summary>
        /// Gets or sets the path to the player sprite texture.
        /// </summary>
        [YamlMember(Alias = "playerSprite")]
        public string? PlayerSprite { get; set; }

        /// <summary>
        /// Gets or sets the enemy data for this combat encounter.
        /// </summary>
        [YamlMember(Alias = "enemy")]
        public CombatEnemy? Enemy { get; set; }

        /// <summary>
        /// Gets or sets the list of available combat actions.
        /// </summary>
        [YamlMember(Alias = "actions")]
        public List<string> Actions { get; set; } = new ();

        /// <summary>
        /// Gets or sets the background music for the combat scene.
        /// </summary>
        [YamlMember(Alias = "music")]
        public string? Music { get; set; }

        /// <summary>
        /// Gets or sets the text displayed when the player wins.
        /// </summary>
        [YamlMember(Alias = "victoryText")]
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
        [YamlMember(Alias = "name")]
        public string? Name { get; set; }

        /// <summary>
        /// Gets or sets the enemy's current hit points.
        /// </summary>
        [YamlMember(Alias = "hp")]
        public int HP { get; set; }

        /// <summary>
        /// Gets or sets the enemy's maximum hit points.
        /// </summary>
        [YamlMember(Alias = "maxHp")]
        public int MaxHP { get; set; }

        /// <summary>
        /// Gets or sets the enemy's attack power.
        /// </summary>
        [YamlMember(Alias = "attack")]
        public int Attack { get; set; }

        /// <summary>
        /// Gets or sets the enemy's defense value.
        /// </summary>
        [YamlMember(Alias = "defense")]
        public int Defense { get; set; }

        /// <summary>
        /// Gets or sets the path to the enemy sprite texture.
        /// </summary>
        [YamlMember(Alias = "sprite")]
        public string? Sprite { get; set; }

        /// <summary>
        /// Gets or sets the list of attack patterns this enemy can use.
        /// </summary>
        [YamlMember(Alias = "attackPatterns")]
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
