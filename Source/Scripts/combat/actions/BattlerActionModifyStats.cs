using Godot;
using System;
using System.Threading.Tasks;

/// <summary>
/// A sample <see cref="BattlerAction"/> implementation that simulates a ranged attack, such as a fireball.
/// </summary>
public partial class StatsBattlerAction : BattlerAction
{
    private const float JumpDistance = 250.0f;

    /// <summary>
    /// A to-hit modifier for this attack that will be influenced by the target Battler's
    /// <see cref="BattlerStats.Evasion"/>.
    /// </summary>
    [Export]
    public int AddedValue { get; set; } = 10;

    public override async Task Execute(Battler source, Battler[] targets = null!)
    {
        if (targets == null) targets = new Battler[0];

        if (targets.Length == 0)
        {
            GD.PrintErr("A ranged attack action requires a target.");
            return;
        }

        var timer = source.GetTree().CreateTimer(0.1f);
        await source.ToSignal(timer, SceneTreeTimer.SignalName.Timeout);

        // Animate a little jump from the source Battler to add some movement to the action.
        Vector2 origin = source.Position;

        Tween tween = source.CreateTween().SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Quad);
        tween.TweenProperty(source, "position", origin + new Vector2(0, -JumpDistance), 0.15f);
        await source.ToSignal(tween, Tween.SignalName.Finished);
        timer = source.GetTree().CreateTimer(0.1f);
        await source.ToSignal(timer, SceneTreeTimer.SignalName.Timeout);
        tween = source.CreateTween().SetEase(Tween.EaseType.In).SetTrans(Tween.TransitionType.Quad);
        tween.TweenProperty(source, "position", origin, 0.15f);
        await source.ToSignal(tween, Tween.SignalName.Finished);

        // No attack animations yet, so wait for a short delay and then apply damage to the target.
        // Normally we would wait for an attack animation's "triggered" signal and then spawn a
        // projectile, waiting for impact.
        timer = source.GetTree().CreateTimer(0.1f);
        await source.ToSignal(timer, SceneTreeTimer.SignalName.Timeout);
        foreach (Battler target in targets)
        {
            target.Stats.AddModifier("attack", AddedValue);
            target.Stats.AddModifier("hit_chance", AddedValue);
        }

        timer = source.GetTree().CreateTimer(0.1f);
        await source.ToSignal(timer, SceneTreeTimer.SignalName.Timeout);
    }
}
