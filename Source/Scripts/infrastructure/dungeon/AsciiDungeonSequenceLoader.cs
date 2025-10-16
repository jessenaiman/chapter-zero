namespace OmegaSpiral.Source.Scripts.Infrastructure.Dungeon
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Godot;
    using Godot.Collections;
    using OmegaSpiral.Source.Scripts.Common;
    using OmegaSpiral.Source.Scripts.Domain.Dungeon;
    using OmegaSpiral.Source.Scripts.Domain.Dungeon.Models;

    /// <summary>
    /// Loads ASCII dungeon sequence definitions from JSON that conforms to the Scene 2 schema using Godot's native JSON capabilities.
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
            if (string.IsNullOrWhiteSpace(json))
            {
                throw new DungeonValidationException("Sequence JSON payload cannot be null or empty.");
            }

            // Parse the JSON using Godot's native JSON parser
            var parseResult = Json.ParseString(json);
            if (parseResult.VariantType == Variant.Type.Nil)
            {
                throw new DungeonValidationException("Failed to parse dungeon sequence JSON - parse result is null.");
            }

            if (parseResult.VariantType != Variant.Type.Dictionary)
            {
                throw new DungeonValidationException("Dungeon sequence JSON must contain an object at the root level.");
            }

            var data = (Godot.Collections.Dictionary<string, Variant>) parseResult;

            // Validate against the schema using JsonSchemaValidator
            var schemaPath = "res://Source/Data/schemas/dungeon_sequence_schema.json";
            if (!JsonSchemaValidator.ValidateSchema(data, schemaPath))
            {
                throw new DungeonValidationException("Dungeon sequence JSON does not conform to the required schema.");
            }

            // Extract and validate the type
            if (!data.TryGetValue("type", out var typeVariant) ||
                typeVariant.VariantType != Variant.Type.String ||
                typeVariant.As<string>() != "ascii_dungeon_sequence")
            {
                throw new DungeonValidationException("Invalid dungeon sequence type.");
            }

            // Extract and validate the dungeons array
            if (!data.TryGetValue("dungeons", out var dungeonsVariant) ||
                dungeonsVariant.VariantType != Variant.Type.Array)
            {
                throw new DungeonValidationException("Dungeon sequence must contain a 'dungeons' array.");
            }

            var dungeonsArray = (Godot.Collections.Array) dungeonsVariant;
            if (dungeonsArray.Count != 3)
            {
                throw new DungeonValidationException("Dungeon sequence must define exactly three stages.");
            }

            var definitions = new List<DungeonStageDefinition>();
            foreach (var dungeonVariant in dungeonsArray)
            {
                if (dungeonVariant.VariantType != Variant.Type.Dictionary)
                {
                    throw new DungeonValidationException("Each dungeon in the sequence must be an object.");
                }
                var dungeonDict = (Godot.Collections.Dictionary<string, Variant>) dungeonVariant;
                definitions.Add(ConvertDungeonDefinition(dungeonDict));
            }

            return AsciiDungeonSequence.Create(definitions);
        }

        private static DungeonStageDefinition ConvertDungeonDefinition(Godot.Collections.Dictionary<string, Variant> dungeon)
        {
            // Extract owner
            if (!dungeon.TryGetValue("owner", out var ownerVariant) ||
                ownerVariant.VariantType != Variant.Type.String)
            {
                throw new DungeonValidationException("Dungeon owner is required and must be a string.");
            }

            var ownerString = ownerVariant.As<string>();
            if (!Enum.TryParse<DreamweaverType>(ownerString, true, out var owner))
            {
                throw new DungeonValidationException($"Unsupported Dreamweaver owner '{ownerString}'.");
            }

            // Extract map
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

            // Extract and convert legend
            var legend = ConvertLegend(dungeon);

            // Extract and convert objects
            var objects = ConvertObjects(dungeon);

            return new DungeonStageDefinition(owner, map, legend, objects);
        }

        private static System.Collections.Generic.Dictionary<char, string> ConvertLegend(Godot.Collections.Dictionary<string, Variant> dungeon)
        {
            if (!dungeon.TryGetValue("legend", out var legendVariant) ||
                legendVariant.VariantType != Variant.Type.Dictionary)
            {
                throw new DungeonValidationException("Legend entries are required.");
            }

            var legendDict = (Godot.Collections.Dictionary<string, Variant>) legendVariant;
            if (legendDict.Count == 0)
            {
                throw new DungeonValidationException("Legend entries are required.");
            }

            var result = new System.Collections.Generic.Dictionary<char, string>();
            foreach (var entry in legendDict)
            {
                var key = entry.Key;
                if (key.Length != 1)
                {
                    throw new DungeonValidationException($"Legend key '{key}' must be a single character.");
                }
                result[key[0]] = entry.Value.As<string>();
            }

            return result;
        }

        private static System.Collections.Generic.Dictionary<char, DungeonObjectDefinition> ConvertObjects(Godot.Collections.Dictionary<string, Variant> dungeon)
        {
            if (!dungeon.TryGetValue("objects", out var objectsVariant) ||
                objectsVariant.VariantType != Variant.Type.Dictionary)
            {
                return new System.Collections.Generic.Dictionary<char, DungeonObjectDefinition>();
            }

            var objectsDict = (Godot.Collections.Dictionary<string, Variant>) objectsVariant;
            if (objectsDict.Count == 0)
            {
                return new System.Collections.Generic.Dictionary<char, DungeonObjectDefinition>();
            }

            var result = new System.Collections.Generic.Dictionary<char, DungeonObjectDefinition>(objectsDict.Count);

            foreach (var pair in objectsDict)
            {
                var key = pair.Key;
                if (key.Length != 1)
                {
                    throw new DungeonValidationException($"Object key '{key}' must be a single character.");
                }

                var keyChar = key[0];
                var documentVariant = pair.Value;

                if (documentVariant.VariantType != Variant.Type.Dictionary)
                {
                    throw new DungeonValidationException($"Interactive object '{key}' is invalid - must be an object.");
                }

                var documentDict = (Godot.Collections.Dictionary<string, Variant>) documentVariant;

                // Extract type
                if (!documentDict.TryGetValue("type", out var typeVariant) ||
                    typeVariant.VariantType != Variant.Type.String)
                {
                    throw new DungeonValidationException($"Object '{key}' must have a 'type' string property.");
                }

                var typeString = typeVariant.As<string>();
                if (!Enum.TryParse<DungeonObjectType>(typeString, true, out var objectType))
                {
                    throw new DungeonValidationException($"Unsupported dungeon object type '{typeString}' for object '{key}'.");
                }

                // Extract aligned_to
                if (!documentDict.TryGetValue("aligned_to", out var alignedToVariant) ||
                    alignedToVariant.VariantType != Variant.Type.String)
                {
                    throw new DungeonValidationException($"Object '{key}' must have an 'aligned_to' string property.");
                }

                var alignedToString = alignedToVariant.As<string>();
                if (!Enum.TryParse<DreamweaverType>(alignedToString, true, out var alignedTo))
                {
                    throw new DungeonValidationException($"Unsupported alignment '{alignedToString}' for object '{key}'.");
                }

                // Extract optional text
                var text = string.Empty;
                if (documentDict.TryGetValue("text", out var textVariant) &&
                    textVariant.VariantType == Variant.Type.String)
                {
                    text = textVariant.As<string>();
                }

                // Extract optional affinity_delta
                var affinityDelta = 0;
                if (documentDict.TryGetValue("affinity_delta", out var affinityDeltaVariant))
                {
                    if (affinityDeltaVariant.VariantType == Variant.Type.Int)
                    {
                        affinityDelta = (int) affinityDeltaVariant;
                    }
                    else if (affinityDeltaVariant.VariantType == Variant.Type.Float)
                    {
                        affinityDelta = (int) (float) affinityDeltaVariant;
                    }
                }

                result[keyChar] = new DungeonObjectDefinition(
                    objectType,
                    text,
                    alignedTo,
                    affinityDelta);
            }

            return result;
        }
    }
}
