// <copyright file="BattlerAction.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using System;
using System.Threading.Tasks;
using Godot;
using OmegaSpiral.Source.Scripts.Combat;
using OmegaSpiral.Source.Scripts.Combat.Battlers;

namespace OmegaSpiral.Combat.Actions;
/// <summary>
/// Specifies the scope of targets that a <see cref="BattlerAction"/> can affect during combat.
/// </summary>
public enum ActionTargetScope
{
    /// <summary>
    /// The action targets only the battler performing it.
    /// </summary>
    Self = 0,

    /// <summary>
    /// The action targets exactly one battler.
    /// </summary>
    One = 1,

    /// <summary>
    /// The action targets all valid battlers.
    /// </summary>
    All = 2,
}

/// <summary>
/// Discrete actions that a <see cref="Battler"/> may take on its turn.
///
/// The following class is an interface that specific actions should implement. <see cref="Execute"/> is
/// called once an action has been chosen and is a coroutine, containing the logic of the action
/// including any animations or effects.
/// </summary>
[GlobalClass]
public partial class BattlerAction : Resource
{
    /// <summary>
    /// An action-specific icon. Shown primarily in menus.
    /// </summary>
    [ExportGroup("UI")]
    [Export]
    public Texture2D? Icon { get; set; }

    /// <summary>
    /// Gets or sets the 'name' of the action. Shown primarily in menus.
    /// </summary>
    [Export]
    public string Label { get; set; } = "Base combat action";

    /// <summary>
    /// Gets or sets tells the player exactly what an action does. Shown primarily in menus.
    /// </summary>
    [Export]
    public string Description { get; set; } = "A combat action.";

    /// <summary>
    /// Action targeting properties.
    /// </summary>
    [ExportGroup("Targets")]
    [Export]
    public ActionTargetScope TargetScope { get; set; } = ActionTargetScope.One;

    /// <summary>
    /// Gets or sets a value indicating whether can this action target friendly <see cref="Battler"/>s? Has no effect if <see cref="TargetScope"/> is
    /// <see cref="ActionTargetScope.Self"/>.
    /// </summary>
    [Export]
    public bool TargetsFriendlies { get; set; } = false;

    /// <summary>
    /// Gets or sets a value indicating whether can this action target enemy <see cref="Battler"/>s? Has no effect if <see cref="TargetScope"/> is
    /// <see cref="ActionTargetScope.Self"/>.
    /// </summary>
    [Export]
    public bool TargetsEnemies { get; set; } = false;

    /// <summary>
    /// The action's <see cref="Element"/>.
    /// </summary>
    [ExportGroup("")]
    [Export]
    public Element Element { get; set; } = Element.None;

    /// <summary>
    /// Gets or sets amount of energy required to perform the action.
    /// </summary>
    [Export]
    public int EnergyCost { get; set; } = 0;

    /// <summary>
    /// Gets or sets the amount of <see cref="Battler.Readiness"/> left to the Battler after acting. This can be used to
    /// design weak attacks that allow the Battler to take fast turns.
    /// </summary>
    [Export]
    public float ReadinessSaved { get; set; } = 0.0f;

    /// <summary>
    /// Verifies that an action can be run. This can be dependent on any number of details regarding the
    /// source and target <see cref="Battler"/>s.
    /// </summary>
    /// <param name="source">The <see cref="Battler"/> performing the action.</param>
    /// <param name="targets">The target <see cref="Battler"/>s for the action.</param>
    /// <returns><see langword="true"/> if the action can be executed; otherwise, <see langword="false"/>.</returns>
    public virtual bool CanExecute(Battler source, Battler[] targets = null!)
    {
        if (targets == null)
        {
            targets = Array.Empty<Battler>();
        }

        if (source == null
            || source.Stats?.Health <= 0
            || source.Stats?.Energy < this.EnergyCost)
        {
            return false;
        }

        return targets.Length > 0;
    }

    /// <summary>
    /// Evaluate whether or not a given target is valid for this action, irrespective of the battler's
    /// team (player or enemy).<br/><br/>
    /// <b>For example:</b> a resurrection spell will target only dead battlers, looking for battlers
    /// with <see cref="BattlerStats.Health"/> that is not greater than zero. Most actions, on the other
    /// hand, will want targets that are selectable and have health points greater than zero.
    /// </summary>
    /// <param name="target">The <see cref="Battler"/> to evaluate as a potential target.</param>
    /// <returns><see langword="true"/> if the target is valid for this action; otherwise, <see langword="false"/>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="target"/> is <c>null</c>.</exception>
    public virtual bool IsTargetValid(Battler target)
    {
        ArgumentNullException.ThrowIfNull(target);

        return target.IsSelectable && target.Stats?.Health > 0;
    }

    /// <summary>
    /// The body of the action, where different animations/modifiers/damage/etc. will be played out.
    /// Battler actions are (almost?) always coroutines, so it is expected that the caller will wait for
    /// execution to finish.
    /// <br/><br/>Note: The base action class does nothing, but must be overridden to do anything.
    /// </summary>
    /// <param name="source">The <see cref="Battler"/> performing the action.</param>
    /// <param name="targets">The target <see cref="Battler"/>s for the action.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="source"/> is <c>null</c>.</exception>
    public virtual async Task Execute(Battler source, Battler[] targets = null!)
    {
        ArgumentNullException.ThrowIfNull(source);

        if (targets == null)
        {
            targets = Array.Empty<Battler>();
        }

        await source.ToSignal(source.GetTree(), SceneTree.SignalName.ProcessFrame);
    }

    /// <summary>
    /// Returns and array of <see cref="Battler"/>s that could be affected by the action.
    /// This includes most cases, accounting for parameters such as <see cref="TargetsFriendlies"/>. Specific
    /// actions may wish to override GetPossibleTargets (to target only mushrooms, for example).
    /// </summary>
    /// <param name="source">The <see cref="Battler"/> performing the action.</param>
    /// <param name="battlers">The list of all available <see cref="Battler"/>s in the current battle.</param>
    /// <returns>An array of <see cref="Battler"/>s that could be affected by this action.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="source"/> is <c>null</c>.</exception>
    public virtual Battler[] GetPossibleTargets(Battler source, BattlerList battlers)
    {
        ArgumentNullException.ThrowIfNull(source);

        ArgumentNullException.ThrowIfNull(battlers);

        var possibleTargets = new System.Collections.Generic.List<Battler>();

        // Normally, actions can pick from battlers of the opposing team. However, actions may be
        // specified to target the source battler only or to target ALL battlers instead.
        if (this.TargetScope == ActionTargetScope.Self)
        {
            possibleTargets.Add(source);
        }
        else if (source.IsPlayer)
        {
            if (this.TargetsFriendlies)
            {
                possibleTargets.AddRange(battlers.Players);
            }

            if (this.TargetsEnemies)
            {
                possibleTargets.AddRange(battlers.Enemies);
            }
        }
        else
        {
            if (this.TargetsFriendlies)
            {
                possibleTargets.AddRange(battlers.Enemies);
            }
            else if (this.TargetsEnemies)
            {
                possibleTargets.AddRange(battlers.Players);
            }
        }

        // Filter the targets to only include live Battlers.
        return BattlerList.GetLiveBattlers(possibleTargets.ToArray());
    }

    /// <summary>
    /// Determines whether the specified <see cref="Battler"/> can be targeted by this action.
    /// </summary>
    /// <param name="target">The <see cref="Battler"/> to check.</param>
    /// <returns><see langword="true"/> if the battler can be targeted; otherwise, <see langword="false"/>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="target"/> is <c>null</c>.</exception>
    public virtual bool CanTargetBattler(Battler target)
    {
        ArgumentNullException.ThrowIfNull(target);

        return target.IsSelectable && target.Stats?.Health > 0;
    }

    /// <summary>
    /// Determines whether this action targets all available battlers.
    /// </summary>
    /// <returns><see langword="true"/> if the action targets all battlers; otherwise, <see langword="false"/>.</returns>
    public virtual bool TargetsAll()
    {
        return this.TargetScope == ActionTargetScope.All;
    }
}
