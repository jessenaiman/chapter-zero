// <copyright file="CharacterClass.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace OmegaSpiral.Source.Scripts
{
    using Godot;

    /// <summary>
    /// Represents a character class with associated stat bonuses and abilities.
    /// </summary>
    public class CharacterClass
    {
        /// <summary>
        /// Gets or sets the name of the character class.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the stat bonuses provided by this character class.
        /// </summary>
        public CharacterStats StatBonuses { get; set; } = new CharacterStats();

        /// <summary>
        /// Initializes a new instance of the <see cref="CharacterClass"/> class.
        /// </summary>
        public CharacterClass()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CharacterClass"/> class.
        /// </summary>
        /// <param name="name">The name of the character class.</param>
        /// <param name="statBonuses">The stat bonuses for this class.</param>
        public CharacterClass(string name, CharacterStats statBonuses)
        {
            this.Name = name;
            this.StatBonuses = statBonuses;
        }

        // Static class instances for common character classes

        /// <summary>
        /// Gets the Fighter character class.
        /// </summary>
        public static CharacterClass Fighter { get; } = new CharacterClass(
            "Fighter",
            new CharacterStats(strength: 2, intelligence: 0, wisdom: 0, dexterity: 1, agility: 1, constitution: 2, vitality: 2, charisma: 0));

        /// <summary>
        /// Gets the Paladin character class.
        /// </summary>
        public static CharacterClass Paladin { get; } = new CharacterClass(
            "Paladin",
            new CharacterStats(strength: 2, intelligence: 0, wisdom: 1, dexterity: 0, agility: 0, constitution: 2, vitality: 2, charisma: 1));

        /// <summary>
        /// Gets the Ranger character class.
        /// </summary>
        public static CharacterClass Ranger { get; } = new CharacterClass(
            "Ranger",
            new CharacterStats(strength: 1, intelligence: 0, wisdom: 1, dexterity: 2, agility: 2, constitution: 1, vitality: 1, charisma: 0));

        /// <summary>
        /// Gets the Thief character class.
        /// </summary>
        public static CharacterClass Thief { get; } = new CharacterClass(
            "Thief",
            new CharacterStats(strength: 0, intelligence: 1, wisdom: 0, dexterity: 2, agility: 2, constitution: 0, vitality: 0, charisma: 1));

        /// <summary>
        /// Gets the Bard character class.
        /// </summary>
        public static CharacterClass Bard { get; } = new CharacterClass(
            "Bard",
            new CharacterStats(strength: 0, intelligence: 1, wisdom: 0, dexterity: 1, agility: 1, constitution: 0, vitality: 0, charisma: 2));

        /// <summary>
        /// Gets the Mage character class.
        /// </summary>
        public static CharacterClass Mage { get; } = new CharacterClass(
            "Mage",
            new CharacterStats(strength: 0, intelligence: 2, wisdom: 1, dexterity: 0, agility: 0, constitution: 0, vitality: 0, charisma: 1));

        /// <summary>
        /// Gets the Priest character class.
        /// </summary>
        public static CharacterClass Priest { get; } = new CharacterClass(
            "Priest",
            new CharacterStats(strength: 0, intelligence: 1, wisdom: 2, dexterity: 0, agility: 0, constitution: 1, vitality: 1, charisma: 1));
    }
}
