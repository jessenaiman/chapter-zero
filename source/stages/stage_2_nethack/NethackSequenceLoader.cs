using Godot;
using System.Text.Json;
using System.Text.Json.Serialization;
using OmegaSpiral.Source.Scripts.Common;
using OmegaSpiral.Source.Scripts.domain.Dungeon;
using OmegaSpiral.Source.Scripts.domain.Dungeon.Models;

namespace OmegaSpiral.Source.Scripts.Infrastructure.Dungeon
{
    /// <summary>
    /// JSON model for dungeon sequence root object.
    /// </summary>
    internal sealed class DungeonSequenceJson
    {
        /// <summary>
        /// Gets or sets the sequence type.
        /// </summary>
        [JsonPropertyName("type")]
        public string Type { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the array of dungeons.
        /// </summary>
        [JsonPropertyName("dungeons")]
        public List<DungeonJson> Dungeons { get; set; } = new();
    }

    /// <summary>
    /// JSON model for individual dungeon objects.
    /// </summary>
    internal sealed class DungeonJson
    {
        /// <summary>
        /// Gets or sets the dungeon ID.
        /// </summary>
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        /// <summary>
        /// Gets or sets the Dreamweaver owner.
        /// </summary>
        [JsonPropertyName("owner")]
        public string Owner { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the ASCII map rows.
        /// </summary>
        [JsonPropertyName("map")]
        public List<string> Map { get; set; } = new();

        /// <summary>
        /// Gets or sets the legend mapping characters to descriptions.
        /// </summary>
        [JsonPropertyName("legend")]
        public Dictionary<string, string> Legend { get; set; } = new();

        /// <summary>
        /// Gets or sets the interactive objects.
        /// </summary>
        [JsonPropertyName("objects")]
        public Dictionary<string, DungeonObjectJson> Objects { get; set; } = new();
    }

    /// <summary>
    /// JSON model for dungeon objects.
    /// </summary>
    internal sealed class DungeonObjectJson
    {
        /// <summary>
        /// Gets or sets the object type.
        /// </summary>
        [JsonPropertyName("type")]
        public string Type { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the narrative text.
        /// </summary>
        [JsonPropertyName("text")]
        public string Text { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the Dreamweaver alignment.
        /// </summary>
        [JsonPropertyName("aligned_to")]
        public string AlignedTo { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the affinity delta.
        /// </summary>
        [JsonPropertyName("affinity_delta")]
        public int AffinityDelta { get; set; }
    }

    /// <summary>
    /// Loads ASCII dungeon sequence definitions from JSON that conforms to the Scene 2 schema using System.Text.Json.
    /// </summary>
    public sealed class AsciiDungeonSequenceLoader
    {
        /// <summary>
        /// Loads a dungeon sequence from JSON text.
        /// </summary>
        /// <param name="json">The JSON payload.</param>
        /// <returns>An aggregate representing the dungeon sequence.</returns>
        /// <exception cref="DungeonValidationException">Thrown when the sequence payload is invalid or cannot be parsed.</exception>
        public static AsciiDungeonSequence LoadFromJson(string json)
        {
            ValidateJsonInput(json);
            var sequenceData = ParseAndValidateJson(json);
            ValidateSequenceType(sequenceData);
            var definitions = ConvertDungeonsToDefinitions(sequenceData.Dungeons);

            return AsciiDungeonSequence.Create(definitions);
        }

        private static void ValidateJsonInput(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                throw new DungeonValidationException("Sequence JSON payload cannot be null or empty.");
            }
        }

        private static DungeonSequenceJson ParseAndValidateJson(string json)
        {
            try
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    ReadCommentHandling = JsonCommentHandling.Skip,
                    AllowTrailingCommas = true,
                };

                var sequenceData = JsonSerializer.Deserialize<DungeonSequenceJson>(json, options);

                if (sequenceData == null)
                {
                    throw new DungeonValidationException("Failed to deserialize dungeon sequence JSON - result is null.");
                }

                return sequenceData;
            }
            catch (JsonException ex)
            {
                throw new DungeonValidationException($"Failed to parse dungeon sequence JSON: {ex.Message}");
            }
        }

        private static void ValidateSequenceType(DungeonSequenceJson data)
        {
            if (string.IsNullOrEmpty(data.Type) || data.Type != "ascii_dungeon_sequence")
            {
                throw new DungeonValidationException("Invalid dungeon sequence type.");
            }
        }

        private static List<DungeonStageDefinition> ConvertDungeonsToDefinitions(List<DungeonJson> dungeons)
        {
            if (dungeons.Count != 3)
            {
                throw new DungeonValidationException("Dungeon sequence must define exactly three stages.");
            }

            var definitions = new List<DungeonStageDefinition>();
            foreach (var dungeon in dungeons)
            {
                definitions.Add(ConvertDungeonJson(dungeon));
            }
            return definitions;
        }

        private static DungeonStageDefinition ConvertDungeonJson(DungeonJson dungeon)
        {
            var id = dungeon.Id ?? "stage";
            var owner = ParseDreamweaverType(dungeon.Owner, "owner");
            var map = dungeon.Map;
            var legend = ConvertLegend(dungeon.Legend);
            var objects = ConvertObjects(dungeon.Objects);

            return new DungeonStageDefinition(id, owner, map, legend, objects);
        }

        private static DreamweaverType ParseDreamweaverType(string value, string fieldName)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new DungeonValidationException($"Dungeon {fieldName} is required and must be a string.");
            }

            if (!Enum.TryParse<DreamweaverType>(value, true, out var result))
            {
                throw new DungeonValidationException($"Unsupported Dreamweaver {fieldName} '{value}'.");
            }

            return result;
        }

        private static List<string> ExtractAndValidateMap(Godot.Collections.Dictionary<string, Variant> dungeon)
        {
            if (!dungeon.TryGetValue("map", out var mapVariant) ||
                mapVariant.VariantType != Variant.Type.Array)
            {
                throw new DungeonValidationException("Dungeon map is required and must be an array of strings.");
            }

            var mapArray = (Godot.Collections.Array) mapVariant;
            var map = new List<string>();
            foreach (var mapItem in mapArray)
            {
                if (mapItem.VariantType != Variant.Type.String)
                {
                    throw new DungeonValidationException("Map entries must be strings.");
                }
                map.Add(mapItem.As<string>());
            }

            if (map.Count == 0)
            {
                throw new DungeonValidationException("Dungeon map cannot be empty.");
            }

            return map;
        }

        private static System.Collections.Generic.Dictionary<char, string> ConvertLegend(Dictionary<string, string> legend)
        {
            if (legend.Count == 0)
            {
                throw new DungeonValidationException("Legend entries are required.");
            }

            var result = new System.Collections.Generic.Dictionary<char, string>();
            foreach (var entry in legend)
            {
                var key = entry.Key;
                if (key.Length != 1)
                {
                    throw new DungeonValidationException($"Legend key '{key}' must be a single character.");
                }
                result[key[0]] = entry.Value;
            }

            return result;
        }

        private static System.Collections.Generic.Dictionary<char, DungeonObjectDefinition> ConvertObjects(Dictionary<string, DungeonObjectJson> objects)
        {
            if (objects.Count == 0)
            {
                return new System.Collections.Generic.Dictionary<char, DungeonObjectDefinition>();
            }

            var result = new System.Collections.Generic.Dictionary<char, DungeonObjectDefinition>(objects.Count);

            foreach (var pair in objects)
            {
                var keyChar = ValidateObjectKey(pair.Key);
                var objectDefinition = ConvertDungeonObjectJson(pair.Key, pair.Value);
                result[keyChar] = objectDefinition;
            }

            return result;
        }

        private static char ValidateObjectKey(string key)
        {
            if (key.Length != 1)
            {
                throw new DungeonValidationException($"Object key '{key}' must be a single character.");
            }
            return key[0];
        }

        private static DungeonObjectDefinition ConvertDungeonObjectJson(string key, DungeonObjectJson obj)
        {
            var objectType = ParseDungeonObjectType(obj.Type, key);
            var alignedTo = ParseDreamweaverType(obj.AlignedTo, $"alignment for object '{key}'");
            var text = obj.Text;
            var affinityDelta = obj.AffinityDelta;

            return new DungeonObjectDefinition(objectType, text, alignedTo, affinityDelta);
        }

        private static DungeonObjectType ParseDungeonObjectType(string value, string key)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new DungeonValidationException($"Object '{key}' must have a 'type' string property.");
            }

            if (!Enum.TryParse<DungeonObjectType>(value, true, out var objectType))
            {
                throw new DungeonValidationException($"Unsupported dungeon object type '{value}' for object '{key}'.");
            }

            return objectType;
        }
    }
}
