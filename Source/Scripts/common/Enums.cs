// <copyright file="Enums.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

namespace OmegaSpiral.Source.Scripts.Common
{
    /// <summary>
    /// Dreamweaver alignment type for narrative influence.
    /// </summary>
    public enum DreamweaverType
    {
        /// <summary>Hero thread - sacrifice and wisdom.</summary>
        Light = 0,

        /// <summary>Ambition thread - chaos and transformation.</summary>
        Mischief = 1,

        /// <summary>Shadow thread - power and consequence.</summary>
        Wrath = 2,
    }

    /// <summary>
    /// Player's chosen Dreamweaver narrative thread.
    /// </summary>
    public enum DreamweaverThread
    {
        /// <summary>Light-aligned choices.</summary>
        Hero = 0,

        /// <summary>Wrath-aligned choices.</summary>
        Shadow = 1,

        /// <summary>Mischief-aligned choices.</summary>
        Ambition = 2,
    }

    /// <summary>
    /// Character class types for party members.
    /// </summary>
    public enum CharacterClass
    {
        /// <summary>Warrior class.</summary>
        Fighter = 0,

        /// <summary>Magic user class.</summary>
        Mage = 1,

        /// <summary>Healing class.</summary>
        Priest = 2,

        /// <summary>Stealth class.</summary>
        Thief = 3,

        /// <summary>Support class.</summary>
        Bard = 4,

        /// <summary>Holy warrior class.</summary>
        Paladin = 5,

        /// <summary>Nature warrior class.</summary>
        Ranger = 6,
    }

    /// <summary>
    /// Character race types for party members.
    /// </summary>
    public enum CharacterRace
    {
        /// <summary>Human race.</summary>
        Human = 0,

        /// <summary>Elf race.</summary>
        Elf = 1,

        /// <summary>Dwarf race.</summary>
        Dwarf = 2,

        /// <summary>Gnome race.</summary>
        Gnome = 3,

        /// <summary>Halfling race.</summary>
        Halfling = 4,

        /// <summary>Half-Elf race.</summary>
        HalfElf = 5,
    }

    /// <summary>
    /// Types of objects in ASCII dungeon.
    /// </summary>
    public enum ObjectType
    {
        /// <summary>Door object.</summary>
        Door = 0,

        /// <summary>Monster object.</summary>
        Monster = 1,

        /// <summary>Treasure chest object.</summary>
        Chest = 2,
    }

    /// <summary>
    /// Types of tiles in tile-based dungeon.
    /// </summary>
    public enum TileType
    {
        /// <summary>Wall tile (impassable).</summary>
        Wall = 0,

        /// <summary>Floor tile (walkable).</summary>
        Floor = 1,

        /// <summary>Door tile.</summary>
        Door = 2,

        /// <summary>Key tile.</summary>
        Key = 3,

        /// <summary>Treasure tile.</summary>
        Treasure = 4,

        /// <summary>Exit tile.</summary>
        Exit = 5,
    }
}
