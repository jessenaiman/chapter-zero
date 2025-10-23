namespace OmegaSpiral.Source.Scripts.domain.Dungeon.Models
{
    /// <summary>
    /// Represents the change applied to Dreamweaver affinity following an interaction.
    /// </summary>
    /// <param name="Type">The type of change.</param>
    /// <param name="Amount">The absolute amount of the change.</param>
    public sealed record DreamweaverAffinityChange(
        DreamweaverAffinityChangeType Type,
        int Amount)
    {
        /// <summary>
        /// Creates a neutral affinity change.
        /// </summary>
        /// <returns>A neutral change instance.</returns>
        public static DreamweaverAffinityChange Neutral() => new(DreamweaverAffinityChangeType.Neutral, 0);

        /// <summary>
        /// Derives an affinity change from a signed delta.
        /// </summary>
        /// <param name="delta">The signed delta value.</param>
        /// <returns>A normalized affinity change.</returns>
        public static DreamweaverAffinityChange FromDelta(int delta)
        {
            if (delta > 0)
            {
                return new DreamweaverAffinityChange(DreamweaverAffinityChangeType.Increase, delta);
            }

            if (delta < 0)
            {
                return new DreamweaverAffinityChange(DreamweaverAffinityChangeType.Decrease, -delta);
            }

            return Neutral();
        }
    }
}
