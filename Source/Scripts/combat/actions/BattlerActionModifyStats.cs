// <copyright file="BattlerActionModifyStats.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

namespace OmegaSpiral.Combat.Actions;

using System;
using System.Threading.Tasks;
using Godot;
using OmegaSpiral.Source.Scripts.Combat.Battlers;

/// <summary>
/// A sample <see cref="BattlerAction"/> implementation that simulates a ranged attack, such as a fireball.
/// </summary>
public partial class StatsBattlerAction : BattlerAction
{
    private const float JumpDistance = 250.0f;

    /// <summary>
    /// Gets or sets the value to add to the target battler's attack and hit chance stats.
    /// </summary>
    [Export]
    public int AddedValue { get; set; } = 10;

    /// <summary>
    /// Executes the stat modification action, jumping and applying stat bonuses to targets.
    /// </summary>
    /// <param name="source">The battler performing the stat modification.</param>
    /// <param name="targets">The targets to receive the stat bonuses.</param>
    /// <returns>A task representing the asynchronous stat modification execution.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="source"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="targets"/> is empty.</exception>
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
            throw new ArgumentException("A ranged attack action requires a target.", nameof(targets));
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
            target.Stats?.AddModifier("attack", this.AddedValue);
            target.Stats?.AddModifier("hit_chance", this.AddedValue);
        }

        timer = source.GetTree().CreateTimer(0.1f);
        await source.ToSignal(timer, SceneTreeTimer.SignalName.Timeout);
    }
}
