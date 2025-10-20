using OmegaSpiral.Source.Scripts.Common;

namespace OmegaSpiral.Source.Scripts.domain.Dungeon.Models
{
    /// <summary>
    /// Immutable definition describing a Dreamweaver-owned ASCII dungeon stage.
    /// </summary>
    /// <param name="Id">The unique identifier for this stage.</param>
    /// <param name="Owner">The Dreamweaver owner.</param>
    /// <param name="Map">The ASCII map rows.</param>
    /// <param name="Legend">Legend entries describing glyphs.</param>
    /// <param name="Objects">Interactive objects embedded within the map.</param>
    public sealed record DungeonStageDefinition(
        string Id,
        DreamweaverType Owner,
        IReadOnlyList<string> Map,
        IReadOnlyDictionary<char, string> Legend,
        IReadOnlyDictionary<char, DungeonObjectDefinition> Objects);
}
