// <copyright file="DungeonSequenceData.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using Godot;
using OmegaSpiral.Source.Scripts;
using OmegaSpiral.Source.Scripts.Common;
using YamlDotNet.Serialization;

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
    [YamlMember(Alias = "type")]
    public string Type { get; set; } = "ascii_dungeon_sequence";

    /// <summary>
    /// Gets the collection of dungeon rooms in this sequence.
    /// Each room represents a different area the player can explore.
    /// </summary>
    [YamlMember(Alias = "dungeons")]
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
    [YamlMember(Alias = "owner")]
    public DreamweaverType Owner { get; set; }

    /// <summary>
    /// Gets the ASCII map layout for this dungeon room.
    /// Each string represents a row of the dungeon grid.
    /// </summary>
    [YamlMember(Alias = "map")]
    public List<string> Map { get; init; } = new ();

    /// <summary>
    /// Gets the legend mapping characters to their descriptions.
    /// Used to determine tile types and walkability.
    /// </summary>
    [YamlMember(Alias = "legend")]
    public Dictionary<string, string> YamlLegend { get; init; } = new ();

    /// <summary>
    /// Gets the legend mapping characters to their descriptions.
    /// Used to determine tile types and walkability.
    /// </summary>
    public Dictionary<char, string> Legend
    {
        get
        {
            var result = new Dictionary<char, string>();
            foreach (var kvp in this.YamlLegend)
            {
                if (kvp.Key.Length == 1)
                {
                    result[kvp.Key[0]] = kvp.Value;
                }
            }

            return result;
        }
    }

    /// <summary>
    /// Gets the collection of interactive objects in this dungeon room.
    /// Maps character symbols to their corresponding dungeon objects.
    /// </summary>
    [YamlMember(Alias = "objects")]
    public Dictionary<string, DungeonObject> YamlObjects { get; init; } = new ();

    /// <summary>
    /// Gets the collection of interactive objects in this dungeon room.
    /// Maps character symbols to their corresponding dungeon objects.
    /// </summary>
    public Dictionary<char, DungeonObject> Objects
    {
        get
        {
            var result = new Dictionary<char, DungeonObject>();
            foreach (var kvp in this.YamlObjects)
            {
                if (kvp.Key.Length == 1)
                {
                    result[kvp.Key[0]] = kvp.Value;
                }
            }

            return result;
        }
    }

    /// <summary>
    /// Gets or sets the starting position for the player in this room.
    /// Coordinates are zero-based from the top-left corner of the map.
    /// </summary>
    [YamlMember(Alias = "playerStartPosition")]
    public List<int> YamlPlayerStartPosition { get; set; } = new () { 2, 2 };

    /// <summary>
    /// Gets or sets the starting position for the player in this room.
    /// Coordinates are zero-based from the top-left corner of the map.
    /// </summary>
    public Vector2I PlayerStartPosition
    {
        get => this.YamlPlayerStartPosition.Count >= 2
            ? new Vector2I(this.YamlPlayerStartPosition[0], this.YamlPlayerStartPosition[1])
            : new Vector2I(2, 2);
        set => this.YamlPlayerStartPosition = new List<int> { value.X, value.Y };
    }
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
    [YamlMember(Alias = "type")]
    public string YamlType { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the type category of this dungeon object.
    /// Determines behavior and interaction mechanics.
    /// </summary>
    public ObjectType Type
    {
        get => Enum.TryParse<ObjectType>(this.YamlType, out var type) ? type : ObjectType.Door;
        set => this.YamlType = value.ToString();
    }

    /// <summary>
    /// Gets or sets the descriptive text displayed when interacting with this object.
    /// Contains narrative content or gameplay instructions.
    /// </summary>
    [YamlMember(Alias = "text")]
    public string? Text { get; set; }

    /// <summary>
    /// Gets or sets the Dreamweaver alignment this object responds to.
    /// Affects scoring and narrative outcomes when collected.
    /// </summary>
    [YamlMember(Alias = "alignedTo")]
    public string YamlAlignedTo { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the Dreamweaver alignment this object responds to.
    /// Affects scoring and narrative outcomes when collected.
    /// </summary>
    public DreamweaverType AlignedTo
    {
        get => Enum.TryParse<DreamweaverType>(this.YamlAlignedTo, out var aligned) ? aligned : DreamweaverType.Light;
        set => this.YamlAlignedTo = value.ToString();
    }

    /// <summary>
    /// Gets or sets the grid position of this object within the dungeon room.
    /// Coordinates are zero-based from the top-left corner of the map.
    /// </summary>
    [YamlMember(Alias = "position")]
    public List<int> YamlPosition { get; set; } = new ();

    /// <summary>
    /// Gets or sets the grid position of this object within the dungeon room.
    /// Coordinates are zero-based from the top-left corner of the map.
    /// </summary>
    public Vector2I Position
    {
        get => this.YamlPosition.Count >= 2
            ? new Vector2I(this.YamlPosition[0], this.YamlPosition[1])
            : new Vector2I(0, 0);
        set => this.YamlPosition = new List<int> { value.X, value.Y };
    }
}
