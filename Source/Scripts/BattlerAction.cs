using Godot;
using System;
using System.Collections.Generic;

/// <summary>
/// Discrete actions that a Battler may take on its turn.
/// The following class is an interface that specific actions should implement. Execute is
/// called once an action has been chosen and is a coroutine, containing the logic of the action
/// including any animations or effects.
/// </summary>
public partial class BattlerAction : Resource
{
    /// <summary>
    /// Enum for target scope options
    /// </summary>
    public enum TargetScope
    {
        Self,
        Single,
        All
    }

    [ExportGroup("UI")]
    /// <summary>
    /// An action-specific icon. Shown primarily in menus.
    /// </summary>
    [Export]
    public Texture2D Icon { get; set; }

    /// <summary>
    /// The 'name' of the action. Shown primarily in menus.
    /// </summary>
    [Export]
    public string Label { get; set; } = "Base combat action";

    /// <summary>
    /// Tells the player exactly what an action does. Shown primarily in menus.
    /// </summary>
    [Export]
    public string Description { get; set; } = "A combat action.";

    [ExportGroup("Targets")]
    /// <summary>
    /// Determines how many Battlers this action targets. Note: TargetScope.Self will not
    /// make use of the TargetsFriendlies or TargetsEnemies flags.
    /// </summary>
    [Export]
    public TargetScope TargetScope { get; set; } = TargetScope.Single;

    /// <summary>
    /// Can this action target friendly Battlers? Has no effect if TargetScope is TargetScope.Self.
    /// </summary>
    [Export]
    public bool TargetsFriendlies { get; set; } = false;

    /// <summary>
    /// Can this action target enemy Battlers? Has no effect if TargetScope is TargetScope.Self.
    /// </summary>
    [Export]
    public bool TargetsEnemies { get; set; } = false;

    [ExportGroup("")]
    /// <summary>
    /// The action's ElementsType.
    /// </summary>
    [Export]
    public ElementsType Element { get; set; } = ElementsType.None;

    /// <summary>
    /// Amount of energy required to perform the action.
    /// </summary>
    [Export]
    [Range(0, 10)]
    public int EnergyCost { get; set; } = 0;

    /// <summary>
    /// The amount of Battler readiness left to the Battler after acting. This can be used to
    /// design weak attacks that allow the Battler to take fast turns.
    /// </summary>
    [Export]
    [Range(0.0f, 100.0f)]
    public float ReadinessSaved { get; set; } = 0.0f;

    /// <summary>
    /// Verifies that an action can be run. This can be dependent on any number of details regarding the
    /// source and target Battlers.
    /// </summary>
    public virtual bool CanExecute(Battler source, List<Battler> targets = null)
    {
        if (targets == null) targets = new List<Battler>();

        if (source == null ||
            source.Stats.Health <= 0 ||
            source.Stats.Energy < EnergyCost)
        {
            return false;
        }

        return targets.Count > 0;
    }

    /// <summary>
    /// Evaluate whether or not a given target is valid for this action, irrespective of the battler's
    /// team (player or enemy).
    /// For example: a resurrection spell will target only dead battlers, looking for battlers
    /// with Health that is not greater than zero. Most actions, on the other hand, will want targets
    /// that are selectable and have health points greater than zero.
    /// </summary>
    public virtual bool IsTargetValid(Battler target)
    {
        if (target.IsSelectable && target.Stats.Health > 0)
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// The body of the action, where different animations/modifiers/damage/etc. will be played out.
    /// Battler actions are (almost?) always coroutines, so it is expected that the caller will wait for
    /// execution to finish.
    /// Note: The base action class does nothing, but must be overridden to do anything.
    /// </summary>
    public virtual async void Execute(Battler source, List<Battler> targets = null)
    {
        if (targets == null) targets = new List<Battler>();

        // Wait for one frame (equivalent to GDScript's await get_tree().process_frame)
        await source.ToSignal(source.GetTree(), SceneTree.SignalName.ProcessFrame);
    }

    /// <summary>
    /// Returns an array of Battlers that could be affected by the action.
    /// This includes most cases, accounting for parameters such as TargetsSelf. Specific
    /// actions may wish to override GetPossibleTargets (to target only mushrooms, for example).
    /// </summary>
    public virtual List<Battler> GetPossibleTargets(Battler source, BattlerList battlers)
    {
        List<Battler> possibleTargets = new List<Battler>();

        // Normally, actions can pick from battlers of the opposing team. However, actions may be
        // specified to target the source battler only or to target ALL battlers instead.
        if (TargetScope == TargetScope.Self)
        {
            possibleTargets.Add(source);
        }
        else if (source.IsPlayer)
        {
            if (TargetsFriendlies)
            {
                possibleTargets.AddRange(battlers.Players);
            }

            if (TargetsEnemies)
            {
                possibleTargets.AddRange(battlers.Enemies);
            }
        }
        else
        {
            if (TargetsFriendlies)
            {
                possibleTargets.AddRange(battlers.Enemies);
            }
            else if (TargetsEnemies)
            {
                possibleTargets.AddRange(battlers.Players);
            }
        }

        // Filter the targets to only include live Battlers.
        possibleTargets = battlers.GetLiveBattlers(possibleTargets);
        return possibleTargets;
    }

    /// <summary>
    /// Determines if this action can target the specified battler
    /// </summary>
    public virtual bool CanTargetBattler(Battler target)
    {
        if (target.IsSelectable && target.Stats.Health > 0)
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// Returns true if this action targets all battlers
    /// </summary>
    public virtual bool TargetsAll()
    {
        return TargetScope == TargetScope.All;
    }
}
