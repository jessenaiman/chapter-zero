// <copyright file="BattlerActionHeal.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using Godot;
using OmegaSpiral.Source.Scripts.Combat.Battlers;

namespace OmegaSpiral.Source.Combat.Actions;
/// <summary>
/// A sample <see cref="HealBattlerAction"/> implementation that simulates a healing action for battlers.
/// </summary>
public partial class HealBattlerAction : BattlerAction
{
    private const float JumpDistance = 250.0f;

    /// <summary>
    /// Gets or sets the amount of healing to apply.
    /// </summary>
    [Export]
    public int HealAmount { get; set; } = 50;

    /// <inheritdoc/>
    public override async Task Execute(Battler source, Battler[] targets = null!)
    {
        ArgumentNullException.ThrowIfNull(source);

        if (targets.Length == 0)
        {
            GD.PrintErr("An attack action requires a target.");
            return;
        }

        var timer = source.GetTree().CreateTimer(0.1f);
        await source.ToSignal(timer, SceneTreeTimer.SignalName.Timeout).ConfigureAwait(false);

        // Animate a little jump from the source Battler to add some movement to the action.
        Vector2 origin = source.Position;

        Tween tween = source.CreateTween().SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Quad);
        tween.TweenProperty(source, "position", origin + new Vector2(0, -JumpDistance), 0.15f);
        await source.ToSignal(tween, Tween.SignalName.Finished).ConfigureAwait(false);
        timer = source.GetTree().CreateTimer(0.1f);
        await source.ToSignal(timer, SceneTreeTimer.SignalName.Timeout).ConfigureAwait(false);
        tween = source.CreateTween().SetEase(Tween.EaseType.In).SetTrans(Tween.TransitionType.Quad);
        tween.TweenProperty(source, "position", origin, 0.15f);
        await source.ToSignal(tween, Tween.SignalName.Finished).ConfigureAwait(false);

        // Wait for a short delay and then apply healing to the targets.
        timer = source.GetTree().CreateTimer(0.1f);
        await source.ToSignal(timer, SceneTreeTimer.SignalName.Timeout).ConfigureAwait(false);
        using (BattlerHit hit = new BattlerHit(-this.HealAmount))
        {
            foreach (Battler target in targets)
            {
                target.TakeHit(hit);

                // Pause slightly between heals.
                timer = source.GetTree().CreateTimer(0.1f);
                await source.ToSignal(timer, SceneTreeTimer.SignalName.Timeout).ConfigureAwait(false);
            }
        }

        // Pause slightly before resuming combat.
        timer = source.GetTree().CreateTimer(0.1f);
        await source.ToSignal(timer, SceneTreeTimer.SignalName.Timeout).ConfigureAwait(false);
    }
}
