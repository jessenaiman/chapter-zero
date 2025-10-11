using Godot;

/// <summary>
/// Represents a hit that can be applied to a battler, typically dealing damage or healing.
/// </summary>
public partial class BattlerHit : Resource
{
    /// <summary>
    /// The amount of damage dealt by this hit. Positive values indicate damage, negative values indicate healing.
    /// </summary>
    public int Damage { get; set; }

    /// <summary>
    /// The chance that this hit will successfully connect with the target, as a percentage (0-100).
    /// </summary>
    public float ToHit { get; set; }

    /// <summary>
    /// Constructor for BattlerHit
    /// </summary>
    /// <param name="damage">The amount of damage dealt</param>
    /// <param name="toHit">The chance that this hit will successfully connect with the target</param>
    public BattlerHit(int damage, float toHit)
    {
        Damage = damage;
        ToHit = toHit;
    }

    /// <summary>
    /// Determines if the hit is successful based on the ToHit chance and the target's evasion.
    /// </summary>
    /// <param name="target">The battler being targeted by this hit</param>
    /// <returns>True if the hit is successful, false otherwise</returns>
    public bool IsSuccessful(Battler? target = null)
    {
        // Generate a random value between 0 and 100
        float randomValue = GD.Randf() * 100;

        // If no target is provided, just use the to-hit chance directly
        if (target == null)
        {
            return randomValue <= ToHit;
        }

        // Adjust the hit chance based on the target's evasion
        float adjustedHitChance = ToHit * (1.0f - (target.Stats.Evasion / 100.0f));

        return randomValue <= adjustedHitChance;
    }
}
