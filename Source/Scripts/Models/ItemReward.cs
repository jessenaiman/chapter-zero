// <copyright file="ItemReward.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace OmegaSpiral.Source.Scripts.Models;

/// <summary>
/// Represents an item reward that can be granted to the player.
/// </summary>
public class ItemReward
{
    /// <summary>
    /// Gets or sets the item being rewarded.
    /// </summary>
    public Item Item { get; set; } = default!;

    /// <summary>
    /// Gets or sets the quantity of the item being rewarded.
    /// </summary>
    public int Quantity { get; set; } = 1;
}
