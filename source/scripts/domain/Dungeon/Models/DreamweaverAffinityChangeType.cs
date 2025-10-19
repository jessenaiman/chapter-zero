namespace OmegaSpiral.Source.Scripts.domain.Dungeon.Models
{
    /// <summary>
    /// Defines how a Dreamweaver's affinity is affected by an interaction.
    /// </summary>
    public enum DreamweaverAffinityChangeType
    {
        /// <summary>
        /// No change to affinity.
        /// </summary>
        Neutral,

        /// <summary>
        /// Affinity increases.
        /// </summary>
        Increase,

        /// <summary>
        /// Affinity decreases.
        /// </summary>
        Decrease,
    }
}
