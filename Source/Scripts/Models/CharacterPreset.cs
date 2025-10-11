// <copyright file="CharacterPreset.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace OmegaSpiral.Source.Scripts.Models;

/// <summary>
/// Represents a preset character configuration for quick character creation.
/// </summary>
/// <remarks>
/// Presets allow players to quickly create characters with predefined
/// class, race, stats, equipment, and appearance settings.
/// </remarks>
public class CharacterPreset
{
    /// <summary>
    /// Gets or sets the name of the character preset.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the description of this preset.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the character class for this preset.
    /// </summary>
    public CharacterClass Class { get; set; } = CharacterClass.Fighter;

    /// <summary>
    /// Gets or sets the character race for this preset.
    /// </summary>
    public CharacterRace Race { get; set; } = CharacterRace.Human;

    /// <summary>
    /// Gets or sets the character name for this preset.
    /// </summary>
    public string CharacterName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the starting level for this preset.
    /// </summary>
    public int Level { get; set; } = 1;

    /// <summary>
    /// Gets or sets the character appearance configuration.
    /// </summary>
    public CharacterAppearance? Appearance { get; set; }

    /// <summary>
    /// Gets or sets the starting inventory preset.
    /// </summary>
    public InventoryPreset? StartingInventory { get; set; }

    /// <summary>
    /// Gets or sets the starting equipment preset.
    /// </summary>
    public Equipment? StartingEquipment { get; set; }

    /// <summary>
    /// Gets or sets the starting skills.
    /// </summary>
    public Skills? StartingSkills { get; set; }
}
