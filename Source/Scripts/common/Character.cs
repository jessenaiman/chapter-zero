// <copyright file="Character.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

namespace OmegaSpiral.Source.Scripts
{
    using Godot;
    using OmegaSpiral.Source.Scripts.Common;

    /// <summary>
    /// Represents a single character in the player's party with class, race, and stats.
    /// </summary>
    public class Character
    {
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
        /// <param name="name">The character's name.</param>
        /// <param name="characterClass">The character's class.</param>
        /// <param name="race">The character's race.</param>
        public Character(string name, CharacterClass characterClass, CharacterRace race)
        {
            this.Name = name;
            this.Class = characterClass;
            this.Race = race;
            this.Stats = CharacterStats.GenerateRandomStats();
            this.Stats.ApplyRacialModifiers(race);
        }

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
        /// Create character from Godot dictionary.
        /// </summary>
        /// <param name="dict">The dictionary containing character data.</param>
        /// <returns>A new character instance created from the dictionary data.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="dict"/> is <c>null</c>.</exception>
        public static Character FromDictionary(Godot.Collections.Dictionary<string, Variant> dict)
        {
            if (dict == null)
            {
                throw new System.ArgumentNullException(nameof(dict));
            }

            var character = new Character();

            if (dict.ContainsKey("name"))
            {
                character.Name = (string) dict["name"];
            }

            if (dict.ContainsKey("class"))
            {
                character.Class = (CharacterClass) System.Enum.Parse(typeof(CharacterClass), (string) dict["class"]);
            }

            if (dict.ContainsKey("race"))
            {
                character.Race = (CharacterRace) System.Enum.Parse(typeof(CharacterRace), (string) dict["race"]);
            }

            if (dict.ContainsKey("level"))
            {
                character.Level = (int) dict["level"];
            }

            if (dict.ContainsKey("experience"))
            {
                character.Experience = (int) dict["experience"];
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
        /// Get the character's hit points based on class and constitution.
        /// </summary>
        /// <returns>The character's total hit points.</returns>
        public int GetHitPoints()
        {
            int baseHp = this.Class switch
            {
                CharacterClass.Fighter => 10,
                CharacterClass.Paladin => 9,
                CharacterClass.Ranger => 8,
                CharacterClass.Thief => 6,
                CharacterClass.Bard => 7,
                CharacterClass.Mage => 4,
                CharacterClass.Priest => 5,
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
            if (this.Class == CharacterClass.Fighter || this.Class == CharacterClass.Thief || this.Class == CharacterClass.Ranger)
            {
                return 0; // Non-spellcasters have no MP
            }

            int baseMp = this.Class switch
            {
                CharacterClass.Mage => 4,
                CharacterClass.Priest => 3,
                CharacterClass.Bard => 2,
                CharacterClass.Paladin => 1,
                _ => 2
            };

            int spellStat = this.Class == CharacterClass.Priest ? this.Stats.Wisdom : this.Stats.Intelligence;
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
                ["class"] = this.Class.ToString(),
                ["race"] = this.Race.ToString(),
                ["level"] = this.Level,
                ["experience"] = this.Experience,
                ["stats"] = this.Stats.ToDictionary(),
            };
        }
    }
}
