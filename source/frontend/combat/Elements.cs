// <copyright file="Elements.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

namespace OmegaSpiral.Source.Scripts.Combat;
/// <summary>
/// Enum representing different element types in combat.
/// </summary>
public enum Element
{
    /// <summary>
    /// No element type.
    /// </summary>
    None = 0,

    /// <summary>
    /// Bug element type.
    /// </summary>
    Bug = 1,

    /// <summary>
    /// Break element type.
    /// </summary>
    Break = 2,

    /// <summary>
    /// Seek element type.
    /// </summary>
    Seek = 3,
}

/// <summary>
/// Represents combat elements and their relationships in terms of advantages/disadvantages.
/// </summary>
public static class ElementAdvantages
{
    /// <summary>
    /// Elements may have an advantage against others (attack power, chance-to-hit, etc.). These
    /// relationships are stored in this dictionary.
    /// Dictionary values are the elements against which the dictionary key is strong. The weak elements
    /// are stored as a list. An empty list indicates that the element has no advantage against others.
    /// </summary>
    public static readonly Dictionary<Element, Element[]> Advantages = new Dictionary<Element, Element[]>
    {
        { Element.None, Array.Empty<Element>() },
        { Element.Bug, new Element[] { Element.Break } },
        { Element.Break, new Element[] { Element.None, Element.Seek } },
        { Element.Seek, new Element[] { Element.Bug } },
    };
}
