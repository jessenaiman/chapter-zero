// <copyright file="CurrencyReward.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace OmegaSpiral.Source.Scripts.Models;

/// <summary>
/// Represents a currency reward that can be granted to the player.
/// </summary>
public class CurrencyReward
{
    /// <summary>
    /// Gets or sets the type of currency being rewarded.
    /// </summary>
    public string CurrencyType { get; set; } = "Gold";

    /// <summary>
    /// Gets or sets the amount of currency being rewarded.
    /// </summary>
    public int Amount { get; set; } = 0;
}
