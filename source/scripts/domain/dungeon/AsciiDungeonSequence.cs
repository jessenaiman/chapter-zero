using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using OmegaSpiral.Source.Scripts.Common;
using OmegaSpiral.Source.Scripts.domain.Dungeon.Models;

namespace OmegaSpiral.Source.Scripts.domain.Dungeon
{
    /// <summary>
    /// Represents the aggregate responsible for orchestrating the ASCII dungeon stages encountered in Scene 2.
    /// </summary>
    public sealed class AsciiDungeonSequence
    {
        private AsciiDungeonSequence(IReadOnlyList<DungeonStage> stages)
        {
            this.Stages = stages;
        }

        /// <summary>
        /// Gets the ordered collection of dungeon stages that compose the sequence.
        /// </summary>
        public IReadOnlyList<DungeonStage> Stages { get; }

        /// <summary>
        /// Creates a new dungeon sequence from validated stage definitions.
        /// </summary>
        /// <param name="definitions">The stage definitions provided by the schema loader.</param>
        /// <returns>An initialized <see cref="AsciiDungeonSequence"/> instance.</returns>
        /// <exception cref="DungeonValidationException">Thrown when validation fails.</exception>
        public static AsciiDungeonSequence Create(IReadOnlyList<DungeonStageDefinition> definitions)
        {
            ArgumentNullException.ThrowIfNull(definitions);

            if (definitions.Count != 3)
            {
                throw new DungeonValidationException("ASCII dungeon sequences must contain exactly three stages.");
            }

            var owners = new HashSet<DreamweaverType>();
            var stages = new List<DungeonStage>(definitions.Count);

            foreach (var definition in definitions)
            {
                if (!owners.Add(definition.Owner))
                {
                    throw new DungeonValidationException("Each dungeon stage must belong to a unique Dreamweaver owner.");
                }

                var stage = DungeonStage.Create(definition);
                stages.Add(stage);
            }

            return new AsciiDungeonSequence(new ReadOnlyCollection<DungeonStage>(stages));
        }
    }
}
