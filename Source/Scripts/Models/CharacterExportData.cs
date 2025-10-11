// <copyright file="CharacterExportData.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System.Collections.Generic;

namespace OmegaSpiral.Source.Scripts.Models;

/// <summary>
/// Data transfer object for exporting character data.
/// </summary>
/// <remarks>
/// Used for serializing character state to save files or network data.
/// </remarks>
public class CharacterExportData
{
    /// <summary>
    /// Gets or sets the character's name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the character's level.
    /// </summary>
    public int Level { get; set; } = 1;

    /// <summary>
    /// Gets or sets the character's class.
    /// </summary>
    public string Class { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the character's race.
    /// </summary>
    public string Race { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the character's current hit points.
    /// </summary>
    public int CurrentHp { get; set; } = 0;

    /// <summary>
    /// Gets or sets the character's maximum hit points.
    /// </summary>
    public int MaxHp { get; set; } = 0;

    /// <summary>
    /// Gets or sets the character's current experience points.
    /// </summary>
    public int Experience { get; set; } = 0;

    /// <summary>
    /// Gets or sets the character's attribute values.
    /// </summary>
    /// <remarks>
    /// Dictionary mapping attribute name to value (e.g., "Strength" -> 15).
    /// </remarks>
    public Dictionary<string, int> Attributes { get; set; } = new Dictionary<string, int>();

    /// <summary>
    /// Gets or sets the inventory export data.
    /// </summary>
    public InventoryExportData? Inventory { get; set; }

    /// <summary>
    /// Gets or sets the equipment export data.
    /// </summary>
    public Dictionary<string, string> EquippedItems { get; set; } = new Dictionary<string, string>();

    /// <summary>
    /// Gets or sets the character's skills.
    /// </summary>
    public Dictionary<string, int> Skills { get; set; } = new Dictionary<string, int>();

    /// <summary>
    /// Gets or sets the timestamp when this data was exported.
    /// </summary>
    public string ExportTimestamp { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the save file version.
    /// </summary>
    public string SaveVersion { get; set; } = "1.0";
}
