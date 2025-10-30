// <copyright file="DungeonSequenceData.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using Godot;

namespace OmegaSpiral.Source.Scripts.Field
{
    /// <summary>
    /// Contains data for a sequence of ASCII-based dungeon rooms.
    /// Used by the NetHack-style scene to provide multiple dungeon layouts.
    /// Each dungeon belongs to a specific Dreamweaver type for narrative alignment.
    /// </summary>
    public partial class DungeonSequenceData : Resource
    {
        /// <summary>
        /// Gets or sets the type identifier for this dungeon sequence.
        /// Used to distinguish between different dungeon sequence formats.
        /// </summary>
        [Export]
        public string Type { get; set; } = "ascii_dungeon_sequence";

        /// <summary>
        /// Gets the collection of dungeon rooms in this sequence.
        /// Each room represents a different area the player can explore.
        /// </summary>
        [Export]
        public Godot.Collections.Array<DungeonRoom> Dungeons { get; set; } = new();
    }
}
