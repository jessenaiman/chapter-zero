// <copyright file="EquipmentStats.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace OmegaSpiral.Source.Scripts.Models;

/// <summary>
/// Represents statistical modifiers provided by equipment items.
/// </summary>
/// <remarks>
/// Used to calculate bonuses or penalties applied to character stats
/// when equipment is equipped.
/// </remarks>
public class EquipmentStats
{
    /// <summary>
    /// Gets or sets the strength modifier.
    /// </summary>
    public int Strength { get; set; } = 0;

    /// <summary>
    /// Gets or sets the dexterity modifier.
    /// </summary>
    public int Dexterity { get; set; } = 0;

    /// <summary>
    /// Gets or sets the constitution modifier.
    /// </summary>
    public int Constitution { get; set; } = 0;

    /// <summary>
    /// Gets or sets the intelligence modifier.
    /// </summary>
    public int Intelligence { get; set; } = 0;

    /// <summary>
    /// Gets or sets the wisdom modifier.
    /// </summary>
    public int Wisdom { get; set; } = 0;

    /// <summary>
    /// Gets or sets the charisma modifier.
    /// </summary>
    public int Charisma { get; set; } = 0;

    /// <summary>
    /// Gets or sets the armor class modifier.
    /// </summary>
    public int ArmorClass { get; set; } = 0;

    /// <summary>
    /// Gets or sets the attack bonus modifier.
    /// </summary>
    public int AttackBonus { get; set; } = 0;

    /// <summary>
    /// Gets or sets the damage bonus modifier.
    /// </summary>
    public int DamageBonus { get; set; } = 0;

    /// <summary>
    /// Gets or sets the hit point bonus modifier.
    /// </summary>
    public int HitPoints { get; set; } = 0;

    /// <summary>
    /// Gets or sets the movement speed modifier.
    /// </summary>
    public int MovementSpeed { get; set; } = 0;
}
