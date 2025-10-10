using Godot;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

/// <summary>
/// A sample BattlerAction implementation that simulates a direct melee hit.
/// </summary>
public partial class AttackBattlerAction : BattlerAction
{
    private const float AttackDistance = 350.0f;

    /// <summary>
    /// A to-hit modifier for this attack that will be influenced by the target Battler's Evasion.
    /// </summary>
    [Export]
    public float HitChance { get; set; } = 100.0f;

    [Export]
    public int BaseDamage { get; set; } = 50;

    public override async void Execute(Battler source, List<Battler> targets = null)
    {
        if (targets == null) targets = new List<Battler>();

        if (targets.Count == 0)
        {
            GD.PrintErr("An attack action requires a target.");
            return;
        }

        var firstTarget = targets[0];

        // Wait for a short delay (equivalent to GDScript's await get_tree().create_timer(0.1).timeout)
        await Task.Delay(100);

        // Calculate where the acting Battler will move from and to.
        var origin = source.GlobalPosition;
        float attackNormal = Math.Sign(source.GlobalPosition.X - firstTarget.GlobalPosition.X);
        var destination = firstTarget.GlobalPosition + new Vector2(AttackDistance * attackNormal, 0);

        // Animate movement to attack position using Godot's Tween system
        var tween = source.CreateTween();
        tween.SetEase(Tween.EaseType.InOut).SetTrans(Tween.TransitionType.Cubic);
        tween.TweenProperty(source, "global_position", destination, 0.25f);

        // Wait for the tween to finish
        await ToSignal(tween, Tween.SignalName.Finished);

        // No attack animations yet, so wait for a short delay and then apply damage to the target.
        // Normally we would wait for an attack animation's "triggered" signal.
        await Task.Delay(100);

        foreach (var target in targets)
        {
            // Incorporate Battler attack and a random variation (10% +- potential damage) to damage.
            var modifiedDamage = BaseDamage + source.Stats.Attack;
            var damageDealt = modifiedDamage + (GD.Randf() - 0.5f) * 0.2f * modifiedDamage;

            // To hit is modified by a Battler's accuracy. That is, a Battler with 90 accuracy will have
            // 90% of the action's base to_hit chance.
            var toHit = HitChance * (source.Stats.HitChance / 100.0f);

            var hit = new BattlerHit((int)damageDealt, toHit);
            target.TakeHit(hit);

            // Wait a short time between hits
            await Task.Delay(100);
        }

        await Task.Delay(100);

        // Animate movement back to the attacker's original position.
        tween = source.CreateTween();
        tween.SetEase(Tween.EaseType.InOut).SetTrans(Tween.TransitionType.Cubic);
        tween.TweenProperty(source, "global_position", origin, 0.25f);

        // Wait for the tween to finish
        await ToSignal(tween, Tween.SignalName.Finished);

        await Task.Delay(100);
    }
}
