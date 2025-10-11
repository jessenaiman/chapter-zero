// <copyright file="Character.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace OmegaSpiral.Source.Scripts
{
    using Godot;
    using OmegaSpiral.Source.Scripts.Models;

    /// <summary>
    /// Represents a single character in the player's party with class, race, and stats.
    /// </summary>
    public class Character
    {
        /// <summary>
        /// Gets or sets the character's name.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the character's class.
        /// </summary>
        public CharacterClass Class { get; set; } = CharacterClass.Fighter;

        /// <summary>
        /// Gets or sets the character's race.
        /// </summary>
        public CharacterRace Race { get; set; } = CharacterRace.Human;

        /// <summary>
        /// Gets or sets the character's stats.
        /// </summary>
        public CharacterStats Stats { get; set; } = new CharacterStats();

        /// <summary>
        /// Gets or sets the character's level.
        /// </summary>
        public int Level { get; set; } = 1;

        /// <summary>
        /// Gets or sets the character's experience points.
        /// </summary>
        public int Experience { get; set; }

        /// <summary>
        /// Gets or sets the character's equipment.
        /// </summary>
        public Equipment Equipment { get; set; } = new Equipment();

        /// <summary>
        /// Gets or sets the character's skills.
        /// </summary>
        public List<string> Skills { get; set; } = new List<string>();

        /// <summary>
        /// Gets or sets the character's portrait image path.
        /// </summary>
        public string Portrait { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the character's appearance data.
        /// </summary>
        public CharacterAppearance Appearance { get; set; } = new CharacterAppearance();

        /// <summary>
        /// Initializes a new instance of the <see cref="Character"/> class.
        /// Default constructor.
        /// </summary>
        public Character()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Character"/> class.
        /// Constructor with basic character information.
        /// </summary>
        public Character(string name, CharacterClass characterClass, CharacterRace race)
        {
            this.Name = name;
            this.Class = characterClass;
            this.Race = race;
            this.Stats = CharacterStats.GenerateRandomStats();
            this.Stats.ApplyRacialModifiers(race);
        }

        /// <summary>
        /// Get the character's hit points based on class and constitution.
        /// </summary>
        /// <returns>The character's total hit points.</returns>
        public int GetHitPoints()
        {
            int baseHp = this.Class.Name switch
            {
                "Fighter" => 10,
                "Paladin" => 9,
                "Ranger" => 8,
                "Thief" => 6,
                "Bard" => 7,
                "Mage" => 4,
                "Priest" => 5,
                _ => 8
            };

            int conModifier = CharacterStats.GetModifier(this.Stats.Constitution);
            return (baseHp + conModifier) * this.Level;
        }

        /// <summary>
        /// Get the character's magic points based on class and intelligence/wisdom.
        /// </summary>
        /// <returns>The character's total magic points.</returns>
        public int GetMagicPoints()
        {
            if (this.Class.Name == "Fighter" || this.Class.Name == "Thief" || this.Class.Name == "Ranger")
            {
                return 0; // Non-spellcasters have no MP
            }

            int baseMp = this.Class.Name switch
            {
                "Mage" => 4,
                "Priest" => 3,
                "Bard" => 2,
                "Paladin" => 1,
                _ => 2
            };

            int spellStat = this.Class.Name == "Priest" ? this.Stats.Wisdom : this.Stats.Intelligence;
            int spellModifier = CharacterStats.GetModifier(spellStat);
            return (baseMp + spellModifier) * this.Level;
        }

        /// <summary>
        /// Convert character to Godot dictionary for serialization.
        /// </summary>
        /// <returns>A dictionary containing the character's data.</returns>
        public Godot.Collections.Dictionary<string, Variant> ToDictionary()
        {
            return new Godot.Collections.Dictionary<string, Variant>
            {
                ["name"] = this.Name,
                ["class"] = this.Class.Name,
                ["race"] = this.Race.ToString(),
                ["level"] = this.Level,
                ["experience"] = this.Experience,
                ["stats"] = this.Stats.ToDictionary(),
            };
        }

        /// <summary>
        /// Create character from Godot dictionary.
        /// </summary>
        /// <param name="dict">The dictionary containing character data.</param>
        /// <returns>A new character instance created from the dictionary data.</returns>
        public static Character FromDictionary(Godot.Collections.Dictionary<string, Variant> dict)
        {
            var character = new Character();

            if (dict.ContainsKey("name"))
            {
                character.Name = (string)dict["name"];
            }

            if (dict.ContainsKey("class"))
            {
                string className = (string)dict["class"];
                character.Class = GetCharacterClassByName(className);
            }

            if (dict.ContainsKey("race"))
            {
                character.Race = (CharacterRace)System.Enum.Parse(typeof(CharacterRace), (string)dict["race"]);
            }

            if (dict.ContainsKey("level"))
            {
                character.Level = (int)dict["level"];
            }

            if (dict.ContainsKey("experience"))
            {
                character.Experience = (int)dict["experience"];
            }

            if (dict.ContainsKey("stats"))
            {
                var statsVar = dict["stats"];
                if (statsVar.VariantType == Variant.Type.Dictionary)
                {
                    var statsDict = statsVar.AsGodotDictionary<string, Variant>();
                    character.Stats = CharacterStats.FromDictionary(statsDict);
                }
            }

            return character;
        }

        /// <summary>
        /// Get a character class by name.
        /// </summary>
        /// <param name="className">The name of the character class.</param>
        /// <returns>The character class instance, or Fighter as default.</returns>
        private static CharacterClass GetCharacterClassByName(string className)
        {
            return className switch
            {
                "Fighter" => CharacterClass.Fighter,
                "Paladin" => CharacterClass.Paladin,
                "Ranger" => CharacterClass.Ranger,
                "Thief" => CharacterClass.Thief,
                "Bard" => CharacterClass.Bard,
                "Mage" => CharacterClass.Mage,
                "Priest" => CharacterClass.Priest,
                _ => CharacterClass.Fighter
            };
        }
    }
}
