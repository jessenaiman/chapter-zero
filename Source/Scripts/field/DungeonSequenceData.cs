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
