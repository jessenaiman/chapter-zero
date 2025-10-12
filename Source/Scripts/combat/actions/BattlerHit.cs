// <copyright file="BattlerHit.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System;
using Godot;

/// <summary>
/// Represents a damage-dealing hit to be applied to a target Battler.
/// Encapsulates calculations for how hits are applied based on some properties.
/// </summary>
public partial class BattlerHit : RefCounted
{
    public int Damage { get; set; }

    public float HitChance { get; set; } = 100.0f;

    public BattlerHit(int dmg, float toHit = 100.0f)
    {
        this.Damage = dmg;
        this.HitChance = toHit;
    }

    public bool IsSuccessful()
    {
        return GD.Randf() * 100.0f < this.HitChance;
    }
}
