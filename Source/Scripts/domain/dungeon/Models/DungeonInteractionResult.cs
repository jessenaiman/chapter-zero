namespace OmegaSpiral.Source.Scripts.Domain.Dungeon.Models
{
    using System;
    using OmegaSpiral.Source.Scripts.Common;

    /// <summary>
    /// Captures the outcome of interacting with a dungeon glyph.
    /// </summary>
    /// <param name="AlignedTo">The Dreamweaver alignment influenced by the interaction.</param>
    /// <param name="ObjectDefinition">The underlying object definition, if any.</param>
    /// <param name="Change">The affinity change applied.</param>
    public sealed record DungeonInteractionResult(
        DreamweaverType AlignedTo,
        DungeonObjectDefinition? ObjectDefinition,
        DreamweaverAffinityChange Change)
    {
        /// <summary>
        /// Creates a result based on an interactive object definition.
        /// </summary>
        /// <param name="definition">The interactive object definition.</param>
        /// <returns>A populated interaction result.</returns>
        public static DungeonInteractionResult FromDefinition(DungeonObjectDefinition definition)
        {
            ArgumentNullException.ThrowIfNull(definition);
            return new DungeonInteractionResult(
                definition.AlignedTo,
                definition,
                DreamweaverAffinityChange.FromDelta(definition.AffinityDelta));
        }

        /// <summary>
        /// Builds a neutral result when no interactive object is present.
        /// </summary>
        /// <param name="owner">The owning Dreamweaver.</param>
        /// <returns>A neutral interaction result.</returns>
        public static DungeonInteractionResult CreateNeutral(DreamweaverType owner)
        {
            return new DungeonInteractionResult(owner, null, DreamweaverAffinityChange.Neutral());
        }
    }
}
