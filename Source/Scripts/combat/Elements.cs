using Godot;
using System;
using System.Collections.Generic;

/// <summary>
/// Represents combat elements and their relationships in terms of advantages/disadvantages.
/// </summary>
public partial class Elements : RefCounted
{
    /// <summary>
    /// Enum representing different element types in combat.
    /// </summary>
    public enum Types
    {
        None,
        Bug,
        Break,
        Seek
    }

    /// <summary>
    /// Elements may have an advantage against others (attack power, chance-to-hit, etc.). These
    /// relationships are stored in this dictionary.
    /// Dictionary values are the elements against which the dictionary key is strong. The weak elements
    /// are stored as a list. An empty list indicates that the element has no advantage against others.
    /// </summary>
    public static readonly Dictionary<Types, Types[]> Advantages = new Dictionary<Types, Types[]>
    {
        { Types.None, new Types[0] },
        { Types.Bug, new Types[] { Types.Break } },
        { Types.Break, new Types[] { Types.None, Types.Seek } },
        { Types.Seek, new Types[] { Types.Bug } }
    };
}