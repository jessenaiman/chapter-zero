// <copyright file="DungeonObject.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using Godot;
using OmegaSpiral.Source.Scripts.Common;
using YamlDotNet.Serialization;

namespace OmegaSpiral.Source.Scripts.Field
{
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
        /// Gets the grid position of this object within the dungeon room.
        /// Coordinates are zero-based from the top-left corner of the map.
        /// </summary>
        [YamlMember(Alias = "position")]
        public List<int> YamlPosition { get; init; } = new();

        /// <summary>
        /// Gets the grid position of this object within the dungeon room.
        /// Coordinates are zero-based from the top-left corner of the map.
        /// </summary>
        public Vector2I Position
        {
            get => this.YamlPosition.Count >= 2
                ? new Vector2I(this.YamlPosition[0], this.YamlPosition[1])
                : new Vector2I(0, 0);
        }
    }
}
