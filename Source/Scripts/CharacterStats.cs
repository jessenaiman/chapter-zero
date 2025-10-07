using Godot;

namespace OmegaSpiral.Source.Scripts
{
    /// <summary>
    /// Character statistics following classic CRPG attribute system.
    /// </summary>
    public class CharacterStats
    {
        public int Strength { get; set; }
        public int Intelligence { get; set; }
        public int Wisdom { get; set; }
        public int Dexterity { get; set; }
        public int Constitution { get; set; }
        public int Charisma { get; set; }
        public int Luck { get; set; }

        /// <summary>
        /// Default constructor with zero values.
        /// </summary>
        public CharacterStats()
        {
            Strength = 0;
            Intelligence = 0;
            Wisdom = 0;
            Dexterity = 0;
            Constitution = 0;
            Charisma = 0;
            Luck = 0;
        }

        /// <summary>
        /// Constructor with specific stat values.
        /// </summary>
        public CharacterStats(int strength, int intelligence, int wisdom, int dexterity, int constitution, int charisma, int luck = 10)
        {
            Strength = strength;
            Intelligence = intelligence;
            Wisdom = wisdom;
            Dexterity = dexterity;
            Constitution = constitution;
            Charisma = charisma;
            Luck = luck;
        }

        /// <summary>
        /// Generate random stats using classic dice rolling method (3d6 for each stat).
        /// </summary>
        public static CharacterStats GenerateRandomStats()
        {
            var random = new Random();

            return new CharacterStats
            {
                Strength = Roll3d6(random),
                Intelligence = Roll3d6(random),
                Wisdom = Roll3d6(random),
                Dexterity = Roll3d6(random),
                Constitution = Roll3d6(random),
                Charisma = Roll3d6(random),
                Luck = Roll3d6(random)
            };
        }

        /// <summary>
        /// Apply racial modifiers to base stats.
        /// </summary>
        /// <param name="race">The character's race</param>
        public void ApplyRacialModifiers(CharacterRace race)
        {
            switch (race)
            {
                case CharacterRace.Human:
                    // Humans get +1 to all stats
                    Strength += 1;
                    Intelligence += 1;
                    Wisdom += 1;
                    Dexterity += 1;
                    Constitution += 1;
                    Charisma += 1;
                    break;

                case CharacterRace.Elf:
                    Dexterity += 2;
                    Intelligence += 1;
                    Constitution -= 1;
                    break;

                case CharacterRace.Dwarf:
                    Constitution += 2;
                    Strength += 1;
                    Charisma -= 1;
                    break;

                case CharacterRace.Gnome:
                    Intelligence += 2;
                    Wisdom += 1;
                    Strength -= 1;
                    break;

                case CharacterRace.Halfling:
                    Dexterity += 2;
                    Luck += 1;
                    Strength -= 1;
                    break;

                case CharacterRace.HalfElf:
                    Charisma += 1;
                    // Half-elves get +1 to two stats of player's choice, handled separately
                    break;
            }
        }

        /// <summary>
        /// Get the modifier for a given stat (used in calculations).
        /// </summary>
        /// <param name="stat">The stat to get modifier for</param>
        /// <returns>Modifier value (-5 to +5 typically)</returns>
        public int GetModifier(int stat)
        {
            return (stat - 10) / 2; // D&D style modifier calculation
        }

        /// <summary>
        /// Roll 3 six-sided dice for stat generation.
        /// </summary>
        private static int Roll3d6(Random random)
        {
            return random.Next(1, 7) + random.Next(1, 7) + random.Next(1, 7);
        }

        /// <summary>
        /// Convert stats to Godot dictionary for serialization.
        /// </summary>
        public Godot.Collections.Dictionary<string, int> ToDictionary()
        {
            return new Godot.Collections.Dictionary<string, int>
            {
                ["strength"] = Strength,
                ["intelligence"] = Intelligence,
                ["wisdom"] = Wisdom,
                ["dexterity"] = Dexterity,
                ["constitution"] = Constitution,
                ["charisma"] = Charisma,
                ["luck"] = Luck
            };
        }

        /// <summary>
        /// Load stats from Godot dictionary.
        /// </summary>
        public static CharacterStats FromDictionary(Godot.Collections.Dictionary<string, int> dict)
        {
            return new CharacterStats
            {
                Strength = dict.GetValueOrDefault("strength", 10),
                Intelligence = dict.GetValueOrDefault("intelligence", 10),
                Wisdom = dict.GetValueOrDefault("wisdom", 10),
                Dexterity = dict.GetValueOrDefault("dexterity", 10),
                Constitution = dict.GetValueOrDefault("constitution", 10),
                Charisma = dict.GetValueOrDefault("charisma", 10),
                Luck = dict.GetValueOrDefault("luck", 10)
            };
        }
    }
}