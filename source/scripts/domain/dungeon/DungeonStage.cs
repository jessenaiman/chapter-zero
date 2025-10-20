using System.Collections.ObjectModel;
using OmegaSpiral.Source.Scripts.Common;
using OmegaSpiral.Source.Scripts.domain.Dungeon.Models;

namespace OmegaSpiral.Source.Scripts.domain.Dungeon
{
    /// <summary>
    /// Represents a single Dreamweaver-owned ASCII dungeon stage.
    /// </summary>
    public sealed class DungeonStage
    {
        private readonly ReadOnlyCollection<string> map;
        private readonly ReadOnlyDictionary<char, string> legend;
        private readonly ReadOnlyDictionary<char, DungeonObjectDefinition> interactiveObjects;

        private DungeonStage(
            string id,
            DreamweaverType owner,
            ReadOnlyCollection<string> map,
            ReadOnlyDictionary<char, string> legend,
            ReadOnlyDictionary<char, DungeonObjectDefinition> interactiveObjects)
        {
            this.Id = id;
            this.Owner = owner;
            this.map = map;
            this.legend = legend;
            this.interactiveObjects = interactiveObjects;
        }

        /// <summary>
        /// Gets the unique identifier for this dungeon stage.
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// Gets the Dreamweaver that owns this dungeon stage.
        /// </summary>
        public DreamweaverType Owner { get; }

        /// <summary>
        /// Gets the ASCII map rows associated with this stage.
        /// </summary>
        public IReadOnlyList<string> Map => this.map;

        /// <summary>
        /// Gets the legend entries for glyph descriptions.
        /// </summary>
        public IReadOnlyDictionary<char, string> Legend => this.legend;

        /// <summary>
        /// Creates a <see cref="DungeonStage"/> from the supplied definition.
        /// </summary>
        /// <param name="definition">The source definition.</param>
        /// <returns>An initialized dungeon stage.</returns>
        /// <exception cref="DungeonValidationException">Thrown when validation fails.</exception>
        public static DungeonStage Create(DungeonStageDefinition definition)
        {
            ArgumentNullException.ThrowIfNull(definition);

            if (definition.Map is null || definition.Map.Count == 0)
            {
                throw new DungeonValidationException("Dungeon maps must contain at least one row.");
            }

            var normalizedMap = NormalizeMap(definition.Map);
            var legend = BuildLegend(definition.Legend);
            var interactiveObjects = BuildObjects(definition.Objects, legend);

            return new DungeonStage(
                definition.Id,
                definition.Owner,
                new ReadOnlyCollection<string>(normalizedMap),
                new ReadOnlyDictionary<char, string>(legend),
                new ReadOnlyDictionary<char, DungeonObjectDefinition>(interactiveObjects));
        }

        /// <summary>
        /// Resolves an interaction for the provided glyph, returning the resulting affinity change.
        /// </summary>
        /// <param name="glyph">The glyph that the player interacted with.</param>
        /// <returns>A <see cref="DungeonInteractionResult"/> describing the outcome.</returns>
        public DungeonInteractionResult ResolveInteraction(char glyph)
        {
            return this.interactiveObjects.TryGetValue(glyph, out var definition)
                ? DungeonInteractionResult.FromDefinition(definition)
                : DungeonInteractionResult.CreateNeutral(this.Owner);
        }

        private static List<string> NormalizeMap(IReadOnlyList<string> mapRows)
        {
            var result = new List<string>(mapRows.Count);
            var expectedWidth = mapRows[0].Length;

            foreach (var row in mapRows)
            {
                if (string.IsNullOrWhiteSpace(row))
                {
                    throw new DungeonValidationException("Map rows cannot be null, empty, or whitespace.");
                }

                if (row.Length != expectedWidth)
                {
                    throw new DungeonValidationException("All map rows must have equal width.");
                }

                result.Add(row);
            }

            return result;
        }

        private static Dictionary<char, string> BuildLegend(IReadOnlyDictionary<char, string> legend)
        {
            if (legend is null || legend.Count == 0)
            {
                throw new DungeonValidationException("Legend definitions are required for dungeon stages.");
            }

            return legend.ToDictionary(entry => entry.Key, entry => entry.Value);
        }

        private static Dictionary<char, DungeonObjectDefinition> BuildObjects(
            IReadOnlyDictionary<char, DungeonObjectDefinition> objects,
            Dictionary<char, string> legend)
        {
            if (objects is null || objects.Count == 0)
            {
                return new Dictionary<char, DungeonObjectDefinition>();
            }

            var result = new Dictionary<char, DungeonObjectDefinition>(objects.Count);

            foreach (var kvp in objects)
            {
                if (!legend.ContainsKey(kvp.Key))
                {
                    throw new DungeonValidationException($"Interactive object '{kvp.Key}' is missing a legend entry.");
                }

                result[kvp.Key] = kvp.Value;
            }

            return result;
        }
    }
}
