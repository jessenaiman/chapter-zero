using Godot;
using System;
using System.Threading.Tasks;

/// <summary>
/// A sample <see cref="BattlerAction"/> implementation that simulates a direct melee hit.
/// </summary>
public partial class HealBattlerAction : BattlerAction
{
    private const float JumpDistance = 250.0f;

    [Export]
    public int HealAmount { get; set; } = 50;

    public override async Task Execute(Battler source, Battler[] targets = null!)
    {
        if (targets == null) targets = new Battler[0];

        if (targets.Length == 0)
        {
            GD.PrintErr("An attack action requires a target.");
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

        // Wait for a short delay and then apply healing to the targets.
        timer = source.GetTree().CreateTimer(0.1f);
        await source.ToSignal(timer, SceneTreeTimer.SignalName.Timeout);
        BattlerHit hit = new BattlerHit(-HealAmount, 100.0f);
        foreach (Battler target in targets)
        {
            target.TakeHit(hit);

            // Pause slightly between heals.
            timer = source.GetTree().CreateTimer(0.1f);
            await source.ToSignal(timer, SceneTreeTimer.SignalName.Timeout);
        }

        // Pause slightly before resuming combat.
        timer = source.GetTree().CreateTimer(0.1f);
        await source.ToSignal(timer, SceneTreeTimer.SignalName.Timeout);
    }
}
