 // <copyright file="DungeonSequenceData.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using Godot;
using OmegaSpiral.Source.Scripts;

/// <summary>
/// Contains data for a sequence of ASCII-based dungeon rooms.
/// Used by the NetHack-style scene to provide multiple dungeon layouts.
/// Each dungeon belongs to a specific Dreamweaver type for narrative alignment.
/// </summary>
public partial class DungeonSequenceData
{
    /// <summary>
    /// Gets or sets the type identifier for this dungeon sequence.
    /// Used to distinguish between different dungeon sequence formats.
    /// </summary>
    public string Type { get; set; } = "ascii_dungeon_sequence";

    /// <summary>
    /// Gets the collection of dungeon rooms in this sequence.
    /// Each room represents a different area the player can explore.
    /// </summary>
    public List<DungeonRoom> Dungeons { get; init; } = new ();
}

/// <summary>
/// Represents a single ASCII-based dungeon room with map layout, objects, and navigation.
/// Each room belongs to a specific Dreamweaver type and contains interactive elements.
/// Used by the ASCII renderer to display explorable dungeon environments.
/// </summary>
public partial class DungeonRoom
{
    /// <summary>
    /// Gets or sets the Dreamweaver type that owns this dungeon room.
    /// Determines the narrative alignment and object behaviors in this room.
    /// </summary>
    public DreamweaverType Owner { get; set; }

    /// <summary>
    /// Gets the ASCII map layout for this dungeon room.
    /// Each string represents a row of the dungeon grid.
    /// </summary>
    public List<string> Map { get; init; } = new ();

    /// <summary>
    /// Gets the legend mapping characters to their descriptions.
    /// Used to determine tile types and walkability.
    /// </summary>
    public Dictionary<char, string> Legend { get; init; } = new ();

    /// <summary>
    /// Gets the collection of interactive objects in this dungeon room.
    /// Maps character symbols to their corresponding dungeon objects.
    /// </summary>
    public Dictionary<char, DungeonObject> Objects { get; init; } = new ();

    /// <summary>
    /// Gets or sets the starting position for the player in this room.
    /// Coordinates are zero-based from the top-left corner of the map.
    /// </summary>
    public Vector2I PlayerStartPosition { get; set; } = new (2, 2);
}

/// <summary>
/// Represents an interactive object within a dungeon room.
/// Objects have types, descriptions, and alignments that affect gameplay.
/// Can be collected or interacted with to progress through the dungeon.
/// </summary>
public partial class DungeonObject
{
    /// <summary>
    /// Gets or sets the type category of this dungeon object.
    /// Determines behavior and interaction mechanics.
    /// </summary>
    public ObjectType Type { get; set; }

    /// <summary>
    /// Gets or sets the descriptive text displayed when interacting with this object.
    /// Contains narrative content or gameplay instructions.
    /// </summary>
    public string? Text { get; set; }

    /// <summary>
    /// Gets or sets the Dreamweaver alignment this object responds to.
    /// Affects scoring and narrative outcomes when collected.
    /// </summary>
    public DreamweaverType AlignedTo { get; set; }

    /// <summary>
    /// Gets or sets the grid position of this object within the dungeon room.
    /// Coordinates are zero-based from the top-left corner of the map.
    /// </summary>
    public Vector2I Position { get; set; }
}
