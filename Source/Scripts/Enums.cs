// <copyright file="Enums.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace OmegaSpiral.Source.Scripts
{
    using Godot;

    /// <summary>
    /// Core alignment mechanic that influences narrative and gameplay throughout the game.
    /// </summary>
    public enum DreamweaverType
    {
        /// <summary>
        /// Hero thread - represents sacrifice, wisdom, and positive transformation.
        /// </summary>
        Light,

        /// <summary>
        /// Ambition thread - represents chaos, transformation, and unpredictable change.
        /// </summary>
        Mischief,

        /// <summary>
        /// Shadow thread - represents power, consequence, and darker impulses.
        /// </summary>
        Wrath,
    }

    /// <summary>
    /// Player's chosen narrative alignment thread that determines story variations.
    /// </summary>
    public enum DreamweaverThread
    {
        /// <summary>
        /// Light-aligned choices focusing on heroism and positive outcomes.
        /// </summary>
        Hero,

        /// <summary>
        /// Wrath-aligned choices focusing on power and darker consequences.
        /// </summary>
        Shadow,

        /// <summary>
        /// Mischief-aligned choices focusing on chaos and transformation.
        /// </summary>
        Ambition,
    }

    /// <summary>
    /// Types of interactive objects in ASCII dungeons.
    /// </summary>
    public enum ObjectType
    {
        /// <summary>
        /// A door that can be opened or locked.
        /// </summary>
        Door,

        /// <summary>
        /// A hostile creature that attacks the player.
        /// </summary>
        Monster,

        /// <summary>
        /// A container that may hold items or treasure.
        /// </summary>
        Chest,
    }

    /// <summary>
    /// Types of tiles in the 2D tile dungeon.
    /// </summary>
    public enum TileType
    {
        /// <summary>
        /// Impassable wall tile.
        /// </summary>
        Wall,

        /// <summary>
        /// Walkable floor tile.
        /// </summary>
        Floor,

        /// <summary>
        /// Door tile that can be opened.
        /// </summary>
        Door,

        /// <summary>
        /// Key item tile.
        /// </summary>
        Key,

        /// <summary>
        /// Treasure chest tile.
        /// </summary>
        Treasure,

        /// <summary>
        /// Exit tile that completes the dungeon.
        /// </summary>
        Exit,
    }
}
