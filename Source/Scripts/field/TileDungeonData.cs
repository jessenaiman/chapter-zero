// <copyright file="TileDungeonData.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using OmegaSpiral.Source.Scripts;
using OmegaSpiral.Source.Scripts.Common;
using YamlDotNet.Serialization;

public partial class TileDungeonData
{
    /// <summary>
    /// Gets or sets the scene type identifier.
    /// </summary>
    [YamlMember(Alias = "type")]
    public string Type { get; set; } = "tile_dungeon";

    /// <summary>
    /// Gets or sets the tilemap as a list of strings.
    /// </summary>
    [YamlMember(Alias = "tilemap")]
    public List<string> Tilemap { get; set; } = new ();

    /// <summary>
    /// Gets or sets the legend mapping characters to tile definitions.
    /// </summary>
    [YamlIgnore]
    public Dictionary<char, TileDefinition> Legend { get; set; } = new ();

    /// <summary>
    /// Gets or sets the legend as a dictionary with string keys for YAML serialization.
    /// </summary>
    [YamlMember(Alias = "legend")]
    public Dictionary<string, TileDefinition> YamlLegend
    {
        get
        {
            var result = new Dictionary<string, TileDefinition>();
            foreach (var kvp in this.Legend)
            {
                result[kvp.Key.ToString()] = kvp.Value;
            }

            return result;
        }

        set
        {
            this.Legend.Clear();
            foreach (var kvp in value)
            {
                if (kvp.Key.Length == 1)
                {
                    this.Legend[kvp.Key[0]] = kvp.Value;
                }
            }
        }
    }

    /// <summary>
    /// Gets or sets the UI configuration for the dungeon.
    /// </summary>
    [YamlMember(Alias = "ui")]
    public DungeonUI? UI { get; set; }

    /// <summary>
    /// Gets or sets the control instructions for the player.
    /// </summary>
    [YamlMember(Alias = "controls")]
    public string? Controls { get; set; }

    /// <summary>
    /// Gets or sets the condition required to exit the dungeon.
    /// </summary>
    [YamlMember(Alias = "exitCondition")]
    public string? ExitCondition { get; set; }
}

public partial class TileDefinition
{
    /// <summary>
    /// Gets or sets the type of tile.
    /// </summary>
    [YamlMember(Alias = "type")]
    public TileType Type { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the tile can be walked on.
    /// </summary>
    [YamlMember(Alias = "walkable")]
    public bool Walkable { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the tile can be interacted with.
    /// </summary>
    [YamlMember(Alias = "interactable")]
    public bool Interactable { get; set; }

    /// <summary>
    /// Gets or sets the description of the tile.
    /// </summary>
    [YamlMember(Alias = "description")]
    public string? Description { get; set; }
}

public partial class DungeonUI
{
    /// <summary>
    /// Gets or sets a value indicating whether to show the inventory UI.
    /// </summary>
    [YamlMember(Alias = "showInventory")]
    public bool ShowInventory { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to show the map UI.
    /// </summary>
    [YamlMember(Alias = "showMap")]
    public bool ShowMap { get; set; }

    /// <summary>
    /// Gets or sets the list of stats to display in the UI.
    /// </summary>
    [YamlMember(Alias = "showStats")]
    public List<string> ShowStats { get; set; } = new ();
}
