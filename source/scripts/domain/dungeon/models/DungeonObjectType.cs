namespace OmegaSpiral.Source.Scripts.domain.Dungeon.Models
{
    /// <summary>
    /// Defines the supported interactive object types within dungeon stages.
    /// </summary>
    public enum DungeonObjectType
    {
        /// <summary>
        /// Represents a locked or narrative door.
        /// </summary>
        Door,

        /// <summary>
        /// Represents an enemy encounter.
        /// </summary>
        Monster,

        /// <summary>
        /// Represents a treasure chest or reward container.
        /// </summary>
        Chest,
    }
}
