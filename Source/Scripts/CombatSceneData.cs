using System.Collections.Generic;

namespace OmegaSpiral.Source.Scripts
{
    /// <summary>
    /// Data model for turn-based pixel combat scenes.
    /// </summary>
    public class CombatSceneData
    {
        public string Type { get; set; } = "pixel_combat";
        public string PlayerSprite { get; set; }
        public CombatEnemy Enemy { get; set; }
        public List<string> Actions { get; set; } = new();
        public string Music { get; set; }
        public string VictoryText { get; set; }
    }

    /// <summary>
    /// Represents an enemy in combat with stats and behavior.
    /// </summary>
    public class CombatEnemy
    {
        public string Name { get; set; }
        public int HP { get; set; }
        public int MaxHP { get; set; }
        public int Attack { get; set; }
        public int Defense { get; set; }
        public string Sprite { get; set; }
        public List<string> AttackPatterns { get; set; } = new();
    }

    /// <summary>
    /// Represents a combat action (FIGHT, MAGIC, ITEM, RUN).
    /// </summary>
    public class CombatAction
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public int Power { get; set; }
        public string Description { get; set; }
    }
}