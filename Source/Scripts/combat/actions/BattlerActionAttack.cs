// <copyright file="BattlerActionAttack.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System;
using System.Threading.Tasks;
using Godot;

/// <summary>
/// A sample <see cref="BattlerAction"/> implementation that simulates a direct melee hit.
/// </summary>
public partial class AttackBattlerAction : BattlerAction
{
    private const float AttackDistance = 350.0f;

    /// <summary>
    /// Gets or sets a to-hit modifier for this attack that will be influenced by the target Battler's
    /// <see cref="BattlerStats.Evasion"/>.
    /// </summary>
    [Export]
    public float HitChance { get; set; } = 100.0f;

    /// <summary>
    /// Gets or sets the base damage value for this attack action.
    /// </summary>
    [Export]
    public int BaseDamage { get; set; } = 50;

    /// <summary>
    /// Executes the attack action, moving the battler to attack position and applying damage.
    /// </summary>
    /// <param name="source">The battler performing the attack.</param>
    /// <param name="targets">The targets of the attack.</param>
    /// <returns>A task representing the asynchronous attack execution.</returns>
    /// <inheritdoc/>
    public override async Task Execute(Battler source, Battler[] targets = null!)
    {
        if (source == null)
        {
            throw new ArgumentNullException(nameof(source), "Source battler cannot be null.");
        }

        if (targets == null)
        {
            targets = Array.Empty<Battler>();
        }

        if (targets.Length == 0)
        {
            GD.PrintErr("An attack action requires a target.");
            return;
        }

        Battler firstTarget = targets[0];

        var timer = source.GetTree().CreateTimer(0.1f);
        await source.ToSignal(timer, SceneTreeTimer.SignalName.Timeout);

        // Calculate where the acting Battler will move from and to.
        Vector2 origin = source.Position;
        float attackNormal = Math.Sign(source.Position.X - firstTarget.Position.X);
        Vector2 destination = firstTarget.Position + new Vector2(AttackDistance * attackNormal, 0);

        // Animate movement to attack position.
        Tween tween = source.CreateTween().SetEase(Tween.EaseType.InOut).SetTrans(Tween.TransitionType.Cubic);
        tween.TweenProperty(source, "position", destination, 0.25f);
        await source.ToSignal(tween, Tween.SignalName.Finished);

        // No attack animations yet, so wait for a short delay and then apply damage to the target.
        // Normally we would wait for an attack animation's "triggered" signal.
        timer = source.GetTree().CreateTimer(0.1f);
        await source.ToSignal(timer, SceneTreeTimer.SignalName.Timeout);

        foreach (Battler target in targets)
        {
            // Incorporate Battler attack and a random variation (10% +- potential damage) to damage.
            int modifiedDamage = this.BaseDamage + source.Stats?.Attack ?? 0;
            double damageDealt = modifiedDamage + ((GD.Randf() - 0.5) * 0.2 * modifiedDamage);

            // To hit is modified by a Battler's accuracy. That is, a Battler with 90 accuracy will have
            // 90% of the action's base to_hit chance.
            float toHit = this.HitChance * (source.Stats?.HitChance ?? 100.0f / 100.0f);

            BattlerHit hit = new BattlerHit((int)damageDealt, toHit);
            target.TakeHit(hit);
            timer = source.GetTree().CreateTimer(0.1f);
            await source.ToSignal(timer, SceneTreeTimer.SignalName.Timeout);
        }

        timer = source.GetTree().CreateTimer(0.1f);
        await source.ToSignal(timer, SceneTreeTimer.SignalName.Timeout);

        // Animate movement back to the attacker's original position.
        tween = source.CreateTween().SetEase(Tween.EaseType.InOut).SetTrans(Tween.TransitionType.Cubic);
        tween.TweenProperty(source, "position", origin, 0.25f);
        await source.ToSignal(tween, Tween.SignalName.Finished);

        timer = source.GetTree().CreateTimer(0.1f);
        await source.ToSignal(timer, SceneTreeTimer.SignalName.Timeout);
    }
}
