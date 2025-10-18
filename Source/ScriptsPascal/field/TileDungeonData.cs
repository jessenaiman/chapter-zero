// <copyright file="TileDungeonData.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using Godot;
using OmegaSpiral.Source.Scripts.Common;

namespace OmegaSpiral.Source.Scripts.Field
{
    /// <summary>
    /// Represents the data structure for a tile-based dungeon, including tilemap, legend, UI, and controls.
    /// </summary>
    public partial class TileDungeonData : Resource
    {
        /// <summary>
        /// Gets or sets the scene type identifier.
        /// </summary>
        [Export]
        public string Type { get; set; } = "tile_dungeon";

        /// <summary>
        /// Gets or sets the tilemap as a list of strings.
        /// </summary>
        [Export]
        public Godot.Collections.Array<string> Tilemap { get; set; } = new();

        /// <summary>
        /// Gets or sets the legend mapping characters to tile definitions.
        /// </summary>
        [Export]
        public Godot.Collections.Dictionary<string, TileDefinition> LegendForJson { get; set; } = new();

        /// <summary>
        /// Gets or sets the UI configuration for the dungeon.
        /// </summary>
        [Export]
        public DungeonUI? UI { get; set; }

        /// <summary>
        /// Gets or sets the control instructions for the player.
        /// </summary>
        [Export]
        public string? Controls { get; set; }

        /// <summary>
        /// Gets or sets the condition required to exit the dungeon.
        /// </summary>
        [Export]
        public string? ExitCondition { get; set; }

        /// <summary>
        /// Gets the legend mapping characters to tile definitions.
        /// </summary>
        public Dictionary<char, TileDefinition> Legend
        {
            get
            {
                var result = new Dictionary<char, TileDefinition>();
                foreach (var kvp in this.LegendForJson)
                {
                    if (kvp.Key.Length == 1)
                    {
                        result[kvp.Key[0]] = kvp.Value;
                    }
                }
                return result;
            }
            set
            {
                this.LegendForJson.Clear();
                foreach (var kvp in value)
                {
                    this.LegendForJson[kvp.Key.ToString()] = kvp.Value;
                }
            }
        }
    }

    /// <summary>
    /// Represents the definition of a tile in the dungeon, including its type, walkability, and interactability.
    /// </summary>
    public partial class TileDefinition : Resource
    {
        /// <summary>
        /// Gets or sets the type of tile.
        /// </summary>
        [Export]
        public TileType Type { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the tile can be walked on.
        /// </summary>
        [Export]
        public bool Walkable { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the tile can be interacted with.
        /// </summary>
        [Export]
        public bool Interactable { get; set; }

        /// <summary>
        /// Gets or sets the description of the tile.
        /// </summary>
        [Export]
        public string? Description { get; set; }
    }

    /// <summary>
    /// Represents the UI configuration for the dungeon, including visibility of inventory, map, and stats.
    /// </summary>
    public partial class DungeonUI : Resource
    {
        /// <summary>
        /// Gets or sets a value indicating whether to show the inventory UI.
        /// </summary>
        [Export]
        public bool ShowInventory { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to show the map UI.
        /// </summary>
        [Export]
        public bool ShowMap { get; set; }

        /// <summary>
        /// Gets or sets the list of stats to display in the UI.
        /// </summary>
        [Export]
        public Godot.Collections.Array<string> ShowStats { get; set; } = new();
    }
}
