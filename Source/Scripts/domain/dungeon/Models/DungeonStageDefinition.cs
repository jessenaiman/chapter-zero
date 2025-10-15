namespace OmegaSpiral.Source.Scripts.Domain.Dungeon.Models
{
    using System.Collections.Generic;
    using OmegaSpiral.Source.Scripts.Common;

    /// <summary>
    /// Immutable definition describing a Dreamweaver-owned ASCII dungeon stage.
    /// </summary>
    /// <param name="Owner">The Dreamweaver owner.</param>
    /// <param name="Map">The ASCII map rows.</param>
    /// <param name="Legend">Legend entries describing glyphs.</param>
    /// <param name="Objects">Interactive objects embedded within the map.</param>
    public sealed record DungeonStageDefinition(
        DreamweaverType Owner,
        IReadOnlyList<string> Map,
        IReadOnlyDictionary<char, string> Legend,
        IReadOnlyDictionary<char, DungeonObjectDefinition> Objects);
}
