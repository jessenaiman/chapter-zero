// <copyright file="DungeonRoom.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using Godot;
using OmegaSpiral.Source.Scripts.Common;
using YamlDotNet.Serialization;

namespace OmegaSpiral.Source.Scripts.Field
{
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
        public List<string> Map { get; init; } = new();

        /// <summary>
        /// Gets the legend mapping characters to their descriptions.
        /// Used to determine tile types and walkability.
        /// </summary>
        [YamlMember(Alias = "legend")]
        public Dictionary<string, string> YamlLegend { get; init; } = new();

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
        public Dictionary<string, DungeonObject> YamlObjects { get; init; } = new();

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
        /// Gets the starting position for the player in this room.
        /// Coordinates are zero-based from the top-left corner of the map.
        /// </summary>
        [YamlMember(Alias = "playerStartPosition")]
        public List<int> YamlPlayerStartPosition { get; init; } = new() { 2, 2 };

        /// <summary>
        /// Gets the starting position for the player in this room.
        /// Coordinates are zero-based from the top-left corner of the map.
        /// </summary>
        public Vector2I PlayerStartPosition
        {
            get => this.YamlPlayerStartPosition.Count >= 2
                ? new Vector2I(this.YamlPlayerStartPosition[0], this.YamlPlayerStartPosition[1])
                : new Vector2I(2, 2);
        }
    }
}
