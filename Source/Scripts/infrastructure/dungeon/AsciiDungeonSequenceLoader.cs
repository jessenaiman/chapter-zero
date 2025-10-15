namespace OmegaSpiral.Source.Scripts.Infrastructure.Dungeon
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.Json;
    using System.Text.Json.Serialization;
    using OmegaSpiral.Source.Scripts.Common;
    using OmegaSpiral.Source.Scripts.Domain.Dungeon;
    using OmegaSpiral.Source.Scripts.Domain.Dungeon.Models;

    /// <summary>
    /// Loads ASCII dungeon sequence definitions from JSON that conforms to the Scene 2 schema.
    /// </summary>
    public sealed class AsciiDungeonSequenceLoader
    {
        private static readonly JsonSerializerOptions SerializerOptions = new()
        {
            PropertyNameCaseInsensitive = true,
        };

        /// <summary>
        /// Loads a dungeon sequence from JSON text.
        /// </summary>
        /// <param name="json">The JSON payload.</param>
        /// <returns>An aggregate representing the dungeon sequence.</returns>
        /// <exception cref="DungeonValidationException">Thrown when the sequence payload is invalid or cannot be parsed.</exception>
        public AsciiDungeonSequence LoadFromJson(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                throw new DungeonValidationException("Sequence JSON payload cannot be null or empty.");
            }

            SequenceDocument? document;
            try
            {
                document = JsonSerializer.Deserialize<SequenceDocument>(json, SerializerOptions);
            }
            catch (JsonException ex)
            {
                throw new DungeonValidationException("Failed to parse dungeon sequence JSON.", ex);
            }

            if (document is null || document.Type != "ascii_dungeon_sequence")
            {
                throw new DungeonValidationException("Invalid dungeon sequence type.");
            }

            if (document.Dungeons is null || document.Dungeons.Count != 3)
            {
                throw new DungeonValidationException("Dungeon sequence must define exactly three stages.");
            }

            var definitions = document.Dungeons.Select(ConvertDungeonDefinition).ToList();
            return AsciiDungeonSequence.Create(definitions);
        }

        private static DungeonStageDefinition ConvertDungeonDefinition(DungeonDocument dungeon)
        {
            ArgumentNullException.ThrowIfNull(dungeon);

            if (!Enum.TryParse<DreamweaverType>(dungeon.Owner, true, out var owner))
            {
                throw new DungeonValidationException($"Unsupported Dreamweaver owner '{dungeon.Owner}'.");
            }

            var map = dungeon.Map ?? throw new DungeonValidationException("Dungeon map is required.");
            var legend = ConvertLegend(dungeon.Legend);
            var objects = ConvertObjects(dungeon.Objects);

            return new DungeonStageDefinition(owner, map, legend, objects);
        }

        private static IReadOnlyDictionary<char, string> ConvertLegend(Dictionary<string, string>? legend)
        {
            if (legend is null || legend.Count == 0)
            {
                throw new DungeonValidationException("Legend entries are required.");
            }

            return legend.ToDictionary(
                entry => entry.Key.Single(),
                entry => entry.Value);
        }

        private static IReadOnlyDictionary<char, DungeonObjectDefinition> ConvertObjects(
            Dictionary<string, DungeonObjectDocument>? objects)
        {
            if (objects is null || objects.Count == 0)
            {
                return new Dictionary<char, DungeonObjectDefinition>();
            }

            var result = new Dictionary<char, DungeonObjectDefinition>(objects.Count);

            foreach (var pair in objects)
            {
                var key = pair.Key.Single();
                var document = pair.Value ?? throw new DungeonValidationException($"Interactive object '{pair.Key}' is invalid.");

                if (!Enum.TryParse<DungeonObjectType>(document.Type, true, out var objectType))
                {
                    throw new DungeonValidationException($"Unsupported dungeon object type '{document.Type}'.");
                }

                if (!Enum.TryParse<DreamweaverType>(document.AlignedTo, true, out var alignedTo))
                {
                    throw new DungeonValidationException($"Unsupported alignment '{document.AlignedTo}' for object '{pair.Key}'.");
                }

                result[key] = new DungeonObjectDefinition(
                    objectType,
                    document.Text ?? string.Empty,
                    alignedTo,
                    document.AffinityDelta ?? 0);
            }

            return result;
        }

        private sealed class SequenceDocument
        {
            [JsonPropertyName("type")]
            public string Type { get; set; } = string.Empty;

            [JsonPropertyName("dungeons")]
            public List<DungeonDocument>? Dungeons { get; set; }
        }

        private sealed class DungeonDocument
        {
            [JsonPropertyName("owner")]
            public string Owner { get; set; } = string.Empty;

            [JsonPropertyName("map")]
            public List<string>? Map { get; set; }

            [JsonPropertyName("legend")]
            public Dictionary<string, string>? Legend { get; set; }

            [JsonPropertyName("objects")]
            public Dictionary<string, DungeonObjectDocument>? Objects { get; set; }
        }

        private sealed class DungeonObjectDocument
        {
            [JsonPropertyName("type")]
            public string Type { get; set; } = string.Empty;

            [JsonPropertyName("text")]
            public string? Text { get; set; }

            [JsonPropertyName("aligned_to")]
            public string AlignedTo { get; set; } = string.Empty;

            [JsonPropertyName("affinity_delta")]
            public int? AffinityDelta { get; set; }
        }
    }
}
