// <copyright file="DungeonSequenceTestData.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

namespace OmegaSpiral.Tests.Unit.Dungeon
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.Json;
    using System.Text.Json.Serialization;
    using OmegaSpiral.Source.Scripts.Common;
    using OmegaSpiral.Source.Scripts.Domain.Dungeon.Models;

    /// <summary>
    /// Helper factory methods for constructing dungeon stage definitions in unit tests.
    /// </summary>
    internal static class DungeonSequenceTestData
    {
        /// <summary>
        /// Creates a stage definition owned by the specified Dreamweaver.
        /// </summary>
        /// <param name="owner">The owning Dreamweaver.</param>
        /// <returns>A fully populated stage definition.</returns>
        public static DungeonStageDefinition CreateStage(DreamweaverType owner)
        {
            return new DungeonStageDefinition(
                owner,
                new[]
                {
                    "##########",
                    "#..C....D#",
                    "#..M....##",
                    "#........#",
                    "##########",
                },
                new Dictionary<char, string>
                {
                    ['#'] = "Wall",
                    ['.'] = "Floor",
                    ['C'] = "Chest",
                    ['D'] = "Door",
                    ['M'] = "Monster",
                },
                new Dictionary<char, DungeonObjectDefinition>
                {
                    ['C'] = new DungeonObjectDefinition(DungeonObjectType.Chest, "A shimmering chest.", owner, 1),
                    ['D'] = new DungeonObjectDefinition(DungeonObjectType.Door, "An ancient door.", owner, 0),
                    ['M'] = new DungeonObjectDefinition(DungeonObjectType.Monster, "A lurking shadow.", owner, 0),
                });
        }

        /// <summary>
        /// Creates a JSON payload matching the schema representation for the supplied definitions.
        /// </summary>
        /// <param name="definitions">The stage definitions.</param>
        /// <returns>A JSON string representing the sequence.</returns>
        public static string CreateSequenceJson(IEnumerable<DungeonStageDefinition> definitions)
        {
            var document = new SequenceDocument
            {
                Dungeons = definitions.Select(CreateDungeonDocument).ToList(),
            };

            return JsonSerializer.Serialize(document, SerializerOptions);
        }

        private static DungeonDocument CreateDungeonDocument(DungeonStageDefinition definition)
        {
            return new DungeonDocument
            {
                Owner = definition.Owner.ToString().ToLowerInvariant(),
                Map = definition.Map.ToList(),
                Legend = definition.Legend.ToDictionary(entry => entry.Key.ToString(), entry => entry.Value),
                Objects = definition.Objects.ToDictionary(
                    entry => entry.Key.ToString(),
                    entry => new DungeonObjectDocument
                    {
                        Type = entry.Value.Type.ToString().ToLowerInvariant(),
                        Text = entry.Value.Text,
                        AlignedTo = entry.Value.AlignedTo.ToString().ToLowerInvariant(),
                        AffinityDelta = entry.Value.AffinityDelta,
                    }),
            };
        }

        private sealed class SequenceDocument
        {
            [JsonPropertyName("type")]
            public string Type { get; set; } = "ascii_dungeon_sequence";

            [JsonPropertyName("dungeons")]
            public List<DungeonDocument> Dungeons { get; set; } = new();
        }

        private sealed class DungeonDocument
        {
            [JsonPropertyName("owner")]
            public string Owner { get; set; } = string.Empty;

            [JsonPropertyName("map")]
            public List<string> Map { get; set; } = new();

            [JsonPropertyName("legend")]
            public Dictionary<string, string> Legend { get; set; } = new();

            [JsonPropertyName("objects")]
            public Dictionary<string, DungeonObjectDocument> Objects { get; set; } = new();
        }

        private sealed class DungeonObjectDocument
        {
            [JsonPropertyName("type")]
            public string Type { get; set; } = string.Empty;

            [JsonPropertyName("text")]
            public string Text { get; set; } = string.Empty;

            [JsonPropertyName("aligned_to")]
            public string AlignedTo { get; set; } = string.Empty;

            [JsonPropertyName("affinity_delta")]
            public int AffinityDelta { get; set; }
        }

        private static readonly JsonSerializerOptions SerializerOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false,
        };
    }
}
