using OmegaSpiral.Source.Scripts.Common;

namespace OmegaSpiral.Source.Scripts.domain.Dungeon.Models
{
    /// <summary>
    /// Immutable definition for an interactive object in a dungeon stage.
    /// </summary>
    /// <param name="Type">The object type.</param>
    /// <param name="Text">Narrative text displayed when interacting.</param>
    /// <param name="AlignedTo">The Dreamweaver alignment associated with the object.</param>
    /// <param name="AffinityDelta">The affinity score delta applied when interacting.</param>
    public sealed record DungeonObjectDefinition(
        DungeonObjectType Type,
        string Text,
        DreamweaverType AlignedTo,
        int AffinityDelta);
}
