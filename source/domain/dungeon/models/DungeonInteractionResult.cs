using OmegaSpiral.Source.Scripts.Common;

namespace OmegaSpiral.Source.Scripts.domain.Dungeon.Models
{
    /// <summary>
    /// Internal log of a dungeon interaction outcome. Used for debug reports and analytics.
    /// Tracks WHAT happened (glyph clicked, object type, alignment) for later review.
    /// Does NOT handle scoring - that's done externally.
    /// </summary>
    /// <param name="Glyph">The character glyph that was interacted with.</param>
    /// <param name="ObjectType">The type of object clicked (Door/Monster/Chest), or null if empty space.</param>
    /// <param name="AlignedTo">Which Dreamweaver this object is aligned to, or null if none.</param>
    /// <param name="NarrativeText">The flavor text shown to the player.</param>
    public sealed record DungeonInteractionResult(
        char Glyph,
        DungeonObjectType? ObjectType,
        DreamweaverType? AlignedTo,
        string NarrativeText)
    {
        /// <summary>
        /// Creates a result based on an interactive object definition.
        /// </summary>
        /// <param name="glyph">The glyph interacted with.</param>
        /// <param name="definition">The interactive object definition.</param>
        /// <returns>A populated interaction result.</returns>
        public static DungeonInteractionResult FromDefinition(char glyph, DungeonObjectDefinition definition)
        {
            ArgumentNullException.ThrowIfNull(definition);
            return new DungeonInteractionResult(
                glyph,
                definition.Type,
                definition.AlignedTo,
                definition.Text);
        }

        /// <summary>
        /// Builds a neutral result when no interactive object is present.
        /// </summary>
        /// <param name="glyph">The glyph that was clicked.</param>
        /// <returns>A neutral interaction result with no object.</returns>
        public static DungeonInteractionResult CreateNeutral(char glyph)
        {
            return new DungeonInteractionResult(
                glyph,
                null,
                null,
                "Nothing happens.");
        }
    }
}
