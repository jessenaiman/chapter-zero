// <copyright file="Enums.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace OmegaSpiral.Source.Scripts.Common
{
    /// <summary>
    /// Dreamweaver alignment type for narrative influence.
    /// </summary>
    public enum DreamweaverType
    {
        /// <summary>Hero thread - sacrifice and wisdom.</summary>
        Light,

        /// <summary>Ambition thread - chaos and transformation.</summary>
        Mischief,

        /// <summary>Shadow thread - power and consequence.</summary>
        Wrath,
    }

    /// <summary>
    /// Player's chosen Dreamweaver narrative thread.
    /// </summary>
    public enum DreamweaverThread
    {
        /// <summary>Light-aligned choices.</summary>
        Hero,

        /// <summary>Wrath-aligned choices.</summary>
        Shadow,

        /// <summary>Mischief-aligned choices.</summary>
        Ambition,
    }

    /// <summary>
    /// Character class types for party members.
    /// </summary>
    public enum CharacterClass
    {
        /// <summary>Warrior class.</summary>
        Fighter,

        /// <summary>Magic user class.</summary>
        Mage,

        /// <summary>Healing class.</summary>
        Priest,

        /// <summary>Stealth class.</summary>
        Thief,

        /// <summary>Support class.</summary>
        Bard,

        /// <summary>Holy warrior class.</summary>
        Paladin,

        /// <summary>Nature warrior class.</summary>
        Ranger,
    }

    /// <summary>
    /// Character race types for party members.
    /// </summary>
    public enum CharacterRace
    {
        /// <summary>Human race.</summary>
        Human,

        /// <summary>Elf race.</summary>
        Elf,

        /// <summary>Dwarf race.</summary>
        Dwarf,

        /// <summary>Gnome race.</summary>
        Gnome,

        /// <summary>Halfling race.</summary>
        Halfling,

        /// <summary>Half-Elf race.</summary>
        HalfElf,
    }

    /// <summary>
    /// Types of objects in ASCII dungeon.
    /// </summary>
    public enum ObjectType
    {
        /// <summary>Door object.</summary>
        Door,

        /// <summary>Monster object.</summary>
        Monster,

        /// <summary>Treasure chest object.</summary>
        Chest,
    }

    /// <summary>
    /// Types of tiles in tile-based dungeon.
    /// </summary>
    public enum TileType
    {
        /// <summary>Wall tile (impassable).</summary>
        Wall,

        /// <summary>Floor tile (walkable).</summary>
        Floor,

        /// <summary>Door tile.</summary>
        Door,

        /// <summary>Key tile.</summary>
        Key,

        /// <summary>Treasure tile.</summary>
        Treasure,

        /// <summary>Exit tile.</summary>
        Exit,
    }
}
