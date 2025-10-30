// <copyright file="CharacterStats.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using Godot;
using OmegaSpiral.Source.Backend.Common;

namespace OmegaSpiral.Source.Backend;

/// <summary>
/// Character statistics following classic CRPG attribute system.
/// </summary>
public class CharacterStats
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CharacterStats"/> class.
    /// Default constructor with zero values.
    /// </summary>
    public CharacterStats()
    {
        this.Strength = 0;
        this.Intelligence = 0;
        this.Wisdom = 0;
        this.Dexterity = 0;
        this.Constitution = 0;
        this.Charisma = 0;
        this.Luck = 0;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CharacterStats"/> class.
    /// Constructor with specific stat values.
    /// </summary>
    /// <param name="strength">The strength stat value.</param>
    /// <param name="intelligence">The intelligence stat value.</param>
    /// <param name="wisdom">The wisdom stat value.</param>
    /// <param name="dexterity">The dexterity stat value.</param>
    /// <param name="constitution">The constitution stat value.</param>
    /// <param name="charisma">The charisma stat value.</param>
    /// <param name="luck">The luck stat value (default is 10).</param>
    public CharacterStats(int strength, int intelligence, int wisdom, int dexterity, int constitution, int charisma, int luck = 10)
    {
        this.Strength = strength;
        this.Intelligence = intelligence;
        this.Wisdom = wisdom;
        this.Dexterity = dexterity;
        this.Constitution = constitution;
        this.Charisma = charisma;
        this.Luck = luck;
    }

    /// <summary>
    /// Gets or sets the strength stat, which affects melee combat and physical tasks.
    /// </summary>
    public int Strength { get; set; }

    /// <summary>
    /// Gets or sets the intelligence stat, which affects spellcasting and knowledge skills.
    /// </summary>
    public int Intelligence { get; set; }

    /// <summary>
    /// Gets or sets the wisdom stat, which affects divine spellcasting and perception.
    /// </summary>
    public int Wisdom { get; set; }

    /// <summary>
    /// Gets or sets the dexterity stat, which affects ranged combat and agility.
    /// </summary>
    public int Dexterity { get; set; }

    /// <summary>
    /// Gets or sets the constitution stat, which affects hit points and endurance.
    /// </summary>
    public int Constitution { get; set; }

    /// <summary>
    /// Gets or sets the charisma stat, which affects social interactions and leadership.
    /// </summary>
    public int Charisma { get; set; }

    /// <summary>
    /// Gets or sets the luck stat, which affects random events and critical hits.
    /// </summary>
    public int Luck { get; set; }

    /// <summary>
    /// Generate random stats using classic dice rolling method (3d6 for each stat).
    /// </summary>
    /// <returns>A new CharacterStats instance with randomly generated values.</returns>
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
            Luck = Roll3d6(random),
        };
    }

    /// <summary>
    /// Get the modifier for a given stat (used in calculations).
    /// </summary>
    /// <param name="stat">The stat to get modifier for.</param>
    /// <returns>Modifier value (-5 to +5 typically).</returns>
    public static int GetModifier(int stat)
    {
        return (stat - 10) / 2; // D&D style modifier calculation
    }

    /// <summary>
    /// Load stats from Godot dictionary.
    /// </summary>
    /// <param name="dict">The dictionary containing stat values.</param>
    /// <returns>A new CharacterStats instance created from the dictionary data.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="dict"/> is <c>null</c>.</exception>
    public static CharacterStats FromDictionary(Godot.Collections.Dictionary<string, Variant> dict)
    {
        ArgumentNullException.ThrowIfNull(dict);

        return new CharacterStats
        {
            Strength = dict.TryGetValue("strength", out Variant strengthValue) ? strengthValue.AsInt32() : 10,
            Intelligence = dict.TryGetValue("intelligence", out Variant intelligenceValue) ? intelligenceValue.AsInt32() : 10,
            Wisdom = dict.TryGetValue("wisdom", out Variant wisdomValue) ? wisdomValue.AsInt32() : 10,
            Dexterity = dict.TryGetValue("dexterity", out Variant dexterityValue) ? dexterityValue.AsInt32() : 10,
            Constitution = dict.TryGetValue("constitution", out Variant constitutionValue) ? constitutionValue.AsInt32() : 10,
            Charisma = dict.TryGetValue("charisma", out Variant charismaValue) ? charismaValue.AsInt32() : 10,
            Luck = dict.TryGetValue("luck", out Variant luckValue) ? luckValue.AsInt32() : 10,
        };
    }

    /// <summary>
    /// Apply racial modifiers to base stats.
    /// </summary>
    /// <param name="race">The character's race.</param>
    public void ApplyRacialModifiers(CharacterRace race)
    {
        switch (race)
        {
            case CharacterRace.Human:
                // Humans get +1 to all stats
                this.Strength += 1;
                this.Intelligence += 1;
                this.Wisdom += 1;
                this.Dexterity += 1;
                this.Constitution += 1;
                this.Charisma += 1;
                break;

            case CharacterRace.Elf:
                this.Dexterity += 2;
                this.Intelligence += 1;
                this.Constitution -= 1;
                break;

            case CharacterRace.Dwarf:
                this.Constitution += 2;
                this.Strength += 1;
                this.Charisma -= 1;
                break;

            case CharacterRace.Gnome:
                this.Intelligence += 2;
                this.Wisdom += 1;
                this.Strength -= 1;
                break;

            case CharacterRace.Halfling:
                this.Dexterity += 2;
                this.Luck += 1;
                this.Strength -= 1;
                break;

            case CharacterRace.HalfElf:
                this.Charisma += 1;

                // Half-elves get +1 to two stats of player's choice, handled separately
                break;
        }
    }

    /// <summary>
    /// Convert stats to Godot dictionary for serialization.
    /// </summary>
    /// <returns>A dictionary containing all stat values.</returns>
    public Godot.Collections.Dictionary<string, int> ToDictionary()
    {
        return new Godot.Collections.Dictionary<string, int>
        {
            ["strength"] = this.Strength,
            ["intelligence"] = this.Intelligence,
            ["wisdom"] = this.Wisdom,
            ["dexterity"] = this.Dexterity,
            ["constitution"] = this.Constitution,
            ["charisma"] = this.Charisma,
            ["luck"] = this.Luck,
        };
    }

    /// <summary>
    /// Roll 3 six-sided dice for stat generation.
    /// </summary>
    /// <param name="random">The random number generator used for dice rolls.</param>
    private static int Roll3d6(Random random)
    {
        return random.Next(1, 7) + random.Next(1, 7) + random.Next(1, 7);
    }
}
