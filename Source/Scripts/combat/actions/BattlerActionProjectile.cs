// <copyright file="BattlerActionProjectile.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System;
using System.Threading.Tasks;
using Godot;

/// <summary>
/// A sample <see cref="BattlerAction"/> implementation that simulates a ranged attack, such as a fireball.
/// </summary>
public partial class RangedBattlerAction : BattlerAction
{
    [Export]
    public float AttackDistance { get; set; } = 350.0f;

    [Export]
    public float AttackTime { get; set; } = 0.25f;

    [Export]
    public float ReturnTime { get; set; } = 0.25f;

    /// <summary>
    /// Gets or sets a to-hit modifier for this attack that will be influenced by the target Battler's
    /// <see cref="BattlerStats.Evasion"/>.
    /// </summary>
    [Export]
    public float HitChance { get; set; } = 100.0f;

    [Export]
    public int BaseDamage { get; set; } = 50;

    /// <inheritdoc/>
    public override async Task Execute(Battler source, Battler[] targets = null!)
    {
        if (targets == null)
        {
            targets = Array.Empty<Battler>();
        }

        if (targets.Length == 0)
        {
            GD.PrintErr("A ranged attack action requires a target.");
            return;
        }

        Battler firstTarget = targets[0];

        var timer = source.GetTree().CreateTimer(0.1f);
        await source.ToSignal(timer, SceneTreeTimer.SignalName.Timeout);

        // Calculate where the acting Battler will move from and to.
        Vector2 origin = source.Position;
        float attackDirection = Math.Sign(firstTarget.Position.X - source.Position.X);
        Vector2 destination = origin + new Vector2(this.AttackDistance * attackDirection, 0);

        // Quickly animate the attacker to the attack position, pretending to lob a fireball or smth.
        Tween tween = source.CreateTween().SetEase(Tween.EaseType.InOut).SetTrans(Tween.TransitionType.Back);
        tween.TweenProperty(source, "position", destination, this.AttackTime);
        await source.ToSignal(tween, Tween.SignalName.Finished);

        // No attack animations yet, so wait for a short delay and then apply damage to the target.
        // Normally we would wait for an attack animation's "triggered" signal and then spawn a
        // projectile, waiting for impact.
        timer = source.GetTree().CreateTimer(0.1f);
        await source.ToSignal(timer, SceneTreeTimer.SignalName.Timeout);
        foreach (Battler target in targets)
        {
            BattlerHit hit = new BattlerHit(this.BaseDamage, this.HitChance);
            target.TakeHit(hit);
            timer = source.GetTree().CreateTimer(0.1f);
            await source.ToSignal(timer, SceneTreeTimer.SignalName.Timeout);
        }

        timer = source.GetTree().CreateTimer(0.4f);
        await source.ToSignal(timer, SceneTreeTimer.SignalName.Timeout);

        // Animate movement back to the attacker's original position.
        tween = source.CreateTween().SetEase(Tween.EaseType.InOut).SetTrans(Tween.TransitionType.Cubic);
        tween.TweenProperty(source, "position", origin, this.ReturnTime);
        await source.ToSignal(tween, Tween.SignalName.Finished);

        timer = source.GetTree().CreateTimer(0.1f);
        await source.ToSignal(timer, SceneTreeTimer.SignalName.Timeout);
    }
}
