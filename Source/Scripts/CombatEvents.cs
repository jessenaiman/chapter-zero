using Godot;
using System;

/// <summary>
/// Centralized event management for combat-related events.
/// CombatEvents provides a centralized way to manage and dispatch combat-related events
/// throughout the game. It uses Godot's signal system to allow different parts of the
/// combat system to communicate with each other without tight coupling.
/// </summary>
public partial class CombatEvents : Node
{
    /// <summary>
    /// Singleton instance of CombatEvents
    /// </summary>
    public static CombatEvents Instance { get; private set; }

    /// <summary>
    /// Emitted when combat is initiated.
    /// </summary>
    [Signal]
    public delegate void CombatInitiatedEventHandler();

    /// <summary>
    /// Emitted when combat is finished.
    /// Parameter: bool - True if the player won, false if the player lost.
    /// </summary>
    [Signal]
    public delegate void CombatFinishedEventHandler(bool isPlayerVictory);

    /// <summary>
    /// Emitted when a player battler is selected.
    /// Parameter: Battler - The selected battler, or null if no battler is selected.
    /// </summary>
    [Signal]
    public delegate void PlayerBattlerSelectedEventHandler(Battler battler);

    /// <summary>
    /// Emitted when an action is selected.
    /// Parameters:
    /// - BattlerAction - The selected action, or null to unqueue the source battler.
    /// - Battler - The source battler.
    /// - Array of Battler - The target battlers.
    /// </summary>
    [Signal]
    public delegate void ActionSelectedEventHandler(BattlerAction action, Battler source, Godot.Collections.Array<Battler> targets);

    /// <summary>
    /// Emitted when a battler's action is finished.
    /// </summary>
    [Signal]
    public delegate void ActionFinishedEventHandler();

    public override void _Ready()
    {
        Instance = this;
    }

    /// <summary>
    /// Emit the CombatInitiated signal.
    /// </summary>
    public static void EmitCombatInitiated()
    {
        if (Instance != null)
        {
            Instance.EmitSignal(SignalName.CombatInitiated);
        }
    }

    /// <summary>
    /// Emit the CombatFinished signal.
    /// </summary>
    public static void EmitCombatFinished(bool isPlayerVictory)
    {
        if (Instance != null)
        {
            Instance.EmitSignal(SignalName.CombatFinished, isPlayerVictory);
        }
    }

    /// <summary>
    /// Emit the PlayerBattlerSelected signal.
    /// </summary>
    public static void EmitPlayerBattlerSelected(Battler battler)
    {
        if (Instance != null)
        {
            Instance.EmitSignal(SignalName.PlayerBattlerSelected, battler);
        }
    }

    /// <summary>
    /// Emit the ActionSelected signal.
    /// </summary>
    public static void EmitActionSelected(BattlerAction action, Battler source, Godot.Collections.Array<Battler> targets)
    {
        if (Instance != null)
        {
            Instance.EmitSignal(SignalName.ActionSelected, action, source, targets);
        }
    }

    /// <summary>
    /// Emit the ActionFinished signal.
    /// </summary>
    public static void EmitActionFinished()
    {
        if (Instance != null)
        {
            Instance.EmitSignal(SignalName.ActionFinished);
        }
    }
}
