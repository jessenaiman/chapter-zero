using Godot;

namespace OmegaSpiral.Source.Scripts
{
    /// <summary>
    /// Represents a single character in the player's party with class, race, and stats.
    /// </summary>
    public class Character
    {
        public string Name { get; set; } = "";
        public CharacterClass Class { get; set; } = CharacterClass.Fighter;
        public CharacterRace Race { get; set; } = CharacterRace.Human;
        public CharacterStats Stats { get; set; } = new CharacterStats();
        public int Level { get; set; } = 1;
        public int Experience { get; set; } = 0;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public Character()
        {
        }

        /// <summary>
        /// Constructor with basic character information.
        /// </summary>
        public Character(string name, CharacterClass characterClass, CharacterRace race)
        {
            Name = name;
            Class = characterClass;
            Race = race;
            Stats = CharacterStats.GenerateRandomStats();
            Stats.ApplyRacialModifiers(race);
        }

        /// <summary>
        /// Get the character's hit points based on class and constitution.
        /// </summary>
        public int GetHitPoints()
        {
            int baseHp = Class switch
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

            int conModifier = Stats.GetModifier(Stats.Constitution);
            return (baseHp + conModifier) * Level;
        }

        /// <summary>
        /// Get the character's magic points based on class and intelligence/wisdom.
        /// </summary>
        public int GetMagicPoints()
        {
            if (Class == CharacterClass.Fighter || Class == CharacterClass.Thief || Class == CharacterClass.Ranger)
                return 0; // Non-spellcasters have no MP

            int baseMp = Class switch
            {
                CharacterClass.Mage => 4,
                CharacterClass.Priest => 3,
                CharacterClass.Bard => 2,
                CharacterClass.Paladin => 1,
                _ => 2
            };

            int spellStat = Class == CharacterClass.Priest ? Stats.Wisdom : Stats.Intelligence;
            int spellModifier = Stats.GetModifier(spellStat);
            return (baseMp + spellModifier) * Level;
        }

        /// <summary>
        /// Convert character to Godot dictionary for serialization.
        /// </summary>
        public Godot.Collections.Dictionary<string, Variant> ToDictionary()
        {
            return new Godot.Collections.Dictionary<string, Variant>
            {
                ["name"] = Name,
                ["class"] = Class.ToString(),
                ["race"] = Race.ToString(),
                ["level"] = Level,
                ["experience"] = Experience,
                ["stats"] = Stats.ToDictionary()
            };
        }

        /// <summary>
        /// Create character from Godot dictionary.
        /// </summary>
        public static Character FromDictionary(Godot.Collections.Dictionary<string, Variant> dict)
        {
            var character = new Character();

            if (dict.ContainsKey("name"))
                character.Name = (string)dict["name"];

            if (dict.ContainsKey("class"))
                character.Class = (CharacterClass)System.Enum.Parse(typeof(CharacterClass), (string)dict["class"]);

            if (dict.ContainsKey("race"))
                character.Race = (CharacterRace)System.Enum.Parse(typeof(CharacterRace), (string)dict["race"]);

            if (dict.ContainsKey("level"))
                character.Level = (int)dict["level"];

            if (dict.ContainsKey("experience"))
                character.Experience = (int)dict["experience"];

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
    }
}