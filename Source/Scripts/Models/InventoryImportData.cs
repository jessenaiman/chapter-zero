// <copyright file="InventoryImportData.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System.Collections.Generic;

namespace OmegaSpiral.Source.Scripts.Models;

/// <summary>
/// Data transfer object for importing inventory data.
/// </summary>
/// <remarks>
/// Used for deserializing inventory state from save files or network data.
/// </remarks>
public class InventoryImportData
{
    /// <summary>
    /// Gets or sets the list of item IDs in the inventory.
    /// </summary>
    public List<string> ItemIds { get; set; } = new List<string>();

    /// <summary>
    /// Gets or sets the list of item quantities.
    /// </summary>
    /// <remarks>
    /// Should match the length of <see cref="ItemIds"/>.
    /// </remarks>
    public List<int> ItemQuantities { get; set; } = new List<int>();

    /// <summary>
    /// Gets or sets the maximum capacity of the inventory.
    /// </summary>
    public int MaxCapacity { get; set; } = 20;

    /// <summary>
    /// Gets or sets the amount of gold.
    /// </summary>
    public int Gold { get; set; } = 0;
}
