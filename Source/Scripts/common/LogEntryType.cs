/// <summary>
/// Types of log entries for the combat log.
/// </summary>
public enum LogEntryType
{
    /// <summary>
    /// General information entry.
    /// </summary>
    Info,

    /// <summary>
    /// Damage dealt entry.
    /// </summary>
    Damage,

    /// <summary>
    /// Healing received entry.
    /// </summary>
    Heal,

    /// <summary>
    /// Attack missed entry.
    /// </summary>
    Miss,

    /// <summary>
    /// Action performed entry.
    /// </summary>
    Action,

    /// <summary>
    /// Status effect applied entry.
    /// </summary>
    StatusEffect,

    /// <summary>
    /// Battler defeated entry.
    /// </summary>
    Defeated,

    /// <summary>
    /// Milestone achieved entry.
    /// </summary>
    Milestone,

    /// <summary>
    /// Breakthrough achieved entry.
    /// </summary>
    Breakthrough,

    /// <summary>
    /// System message entry.
    /// </summary>
    System
}
