using Godot;
using System;

/// <summary>
/// Represents a damage-dealing hit to be applied to a target Battler.
/// Encapsulates calculations for how hits are applied based on some properties.
/// </summary>
public partial class BattlerHit : RefCounted
{
    public int Damage { get; set; } = 0;
    public float HitChance { get; set; } = 100.0f;

    public BattlerHit(int dmg, float toHit = 100.0f)
    {
        Damage = dmg;
        HitChance = toHit;
    }

    public bool IsSuccessful()
    {
        return GD.Randf() * 100.0f < HitChance;
    }
}