using Godot;

namespace OmegaSpiral.Source.Scripts.Stages.Stage4;

/// <summary>
/// Handles real-time field combat gameplay.
/// Manages combat actions, turn-based or real-time mechanics.
/// </summary>
[GlobalClass]
public partial class FieldCombat : Control
{
    /// <summary>
    /// Emitted when a combat round starts.
    /// </summary>
    /// <param name="roundNumber">The number of the current combat round.</param>
    [Signal]
    public delegate void CombatRoundStartedEventHandler(int roundNumber);

    /// <summary>
    /// Emitted when player selects an action during combat.
    /// </summary>
    /// <param name="actionType">The type of action selected.</param>
    /// <param name="target">The target of the action.</param>
    [Signal]
    public delegate void PlayerActionSelectedEventHandler(string actionType, string target);

    /// <summary>
    /// Emitted when an attack is executed.
    /// </summary>
    /// <param name="attacker">The entity performing the attack.</param>
    /// <param name="defender">The entity receiving the attack.</param>
    /// <param name="damage">The damage dealt.</param>
    [Signal]
    public delegate void AttackExecutedEventHandler(string attacker, string defender, int damage);

    /// <summary>
    /// Emitted when a special ability is used.
    /// </summary>
    /// <param name="abilityName">The name of the ability used.</param>
    /// <param name="user">The entity using the ability.</param>
    /// <param name="targetCount">The number of targets.</param>
    [Signal]
    public delegate void SpecialAbilityUsedEventHandler(string abilityName, string user, int targetCount);

    /// <summary>
    /// Emitted when a combatant's status changes.
    /// </summary>
    /// <param name="combatant">The combatant whose status changed.</param>
    /// <param name="status">The new status.</param>
    /// <param name="duration">The duration of the status effect.</param>
    [Signal]
    public delegate void CombatantStatusChangedEventHandler(string combatant, string status, int duration);

    /// <summary>
    /// Emitted when combat results in victory or defeat.
    /// </summary>
    /// <param name="outcome">The combat outcome.</param>
    /// <param name="victor">The entity that won the combat.</param>
    [Signal]
    public delegate void CombatCompletedEventHandler(string outcome, string victor);

    /// <inheritdoc/>
    public override void _Ready()
    {
        // Initialize field combat logic
        GD.Print("Field Combat initialized");
    }

    /// <summary>
    /// Emits the CombatRoundStarted signal.
    /// </summary>
    /// <param name="roundNumber">The number of the current combat round.</param>
    public void EmitCombatRoundStarted(int roundNumber)
    {
        this.EmitSignal(SignalName.CombatRoundStarted, roundNumber);
    }

    /// <summary>
    /// Emits the PlayerActionSelected signal.
    /// </summary>
    /// <param name="actionType">The type of action selected.</param>
    /// <param name="target">The target of the action.</param>
    public void EmitPlayerActionSelected(string actionType, string target)
    {
        this.EmitSignal(SignalName.PlayerActionSelected, actionType, target);
    }

    /// <summary>
    /// Emits the AttackExecuted signal.
    /// </summary>
    /// <param name="attacker">The entity performing the attack.</param>
    /// <param name="defender">The entity receiving the attack.</param>
    /// <param name="damage">The damage dealt.</param>
    public void EmitAttackExecuted(string attacker, string defender, int damage)
    {
        this.EmitSignal(SignalName.AttackExecuted, attacker, defender, damage);
    }

    /// <summary>
    /// Emits the SpecialAbilityUsed signal.
    /// </summary>
    /// <param name="abilityName">The name of the ability used.</param>
    /// <param name="user">The entity using the ability.</param>
    /// <param name="targets">The targets of the ability.</param>
    public void EmitSpecialAbilityUsed(string abilityName, string user, string[] targets)
    {
        this.EmitSignal(SignalName.SpecialAbilityUsed, abilityName, user, targets.Length);
    }

    /// <summary>
    /// Emits the CombatantStatusChanged signal.
    /// </summary>
    /// <param name="combatant">The combatant whose status changed.</param>
    /// <param name="status">The new status.</param>
    /// <param name="duration">The duration of the status effect.</param>
    public void EmitCombatantStatusChanged(string combatant, string status, int duration)
    {
        this.EmitSignal(SignalName.CombatantStatusChanged, combatant, status, duration);
    }

    /// <summary>
    /// Emits the CombatCompleted signal.
    /// </summary>
    /// <param name="outcome">The combat outcome.</param>
    /// <param name="victor">The entity that won the combat.</param>
    public void EmitCombatCompleted(string outcome, string victor)
    {
        this.EmitSignal(SignalName.CombatCompleted, outcome, victor);
    }
}
