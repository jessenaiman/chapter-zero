// <copyright file="TileDungeonData.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using OmegaSpiral.Source.Scripts;

public partial class TileDungeonData
{
    public string Type { get; set; } = "tile_dungeon";

    public List<string> Tilemap { get; set; } = new ();

    public Dictionary<char, TileDefinition> Legend { get; set; } = new ();

    public DungeonUI? UI { get; set; }

    public string? Controls { get; set; }

    public string? ExitCondition { get; set; }
}

public partial class TileDefinition
{
    public TileType Type { get; set; }

    public bool Walkable { get; set; }

    public bool Interactable { get; set; }

    public string? Description { get; set; }
}

public partial class DungeonUI
{
    public bool ShowInventory { get; set; }

    public bool ShowMap { get; set; }

    public List<string> ShowStats { get; set; } = new ();
}
