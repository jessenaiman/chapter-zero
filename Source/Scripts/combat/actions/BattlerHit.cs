// <copyright file="BattlerHit.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using System;
using Godot;

/// <summary>
/// Represents a damage-dealing hit to be applied to a target Battler.
/// Encapsulates calculations for how hits are applied based on some properties.
/// </summary>
public partial class BattlerHit : RefCounted
{
    /// <summary>
    /// Gets or sets the amount of damage this hit deals.
    /// </summary>
    public int Damage { get; set; }

    /// <summary>
    /// Gets or sets the chance (0-100) that this hit will successfully land.
    /// </summary>
    public float HitChance { get; set; } = 100.0f;

    /// <summary>
    /// Initializes a new instance of the <see cref="BattlerHit"/> class with the specified damage and hit chance.
    /// </summary>
    /// <param name="dmg">The amount of damage to deal.</param>
    /// <param name="toHit">The chance to hit (default 100.0f).</param>
    public BattlerHit(int dmg, float toHit = 100.0f)
    {
        this.Damage = dmg;
        this.HitChance = toHit;
    }

    /// <summary>
    /// Determines whether this hit is successful based on the hit chance.
    /// </summary>
    /// <returns><see langword="true"/> if the hit lands; otherwise, <see langword="false"/>.</returns>
    public bool IsSuccessful()
    {
        return GD.Randf() * 100.0f < this.HitChance;
    }
}
