namespace OmegaSpiral.Combat;

// <copyright file="CombatEvents.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using Godot;
using OmegaSpiral.Source.Scripts.Combat.Battlers;
using OmegaSpiral.Combat.Actions;

/// <summary>
/// A signal bus to connect distant scenes to various combat-exclusive events.
/// </summary>
[GlobalClass]
public partial class CombatEvents : Node
{
    /// <summary>
    /// Emitted when a player battler is selected for action.
    /// </summary>
    /// <param name="battler">The selected battler.</param>
    [Signal]
    public delegate void PlayerBattlerSelectedEventHandler(Battler battler);

    /// <summary>
    /// Emitted when an action is selected with targets.
    /// </summary>
    /// <param name="action">The selected action.</param>
    /// <param name="source">The source battler of the action.</param>
    /// <param name="targets">The target battlers of the action.</param>
    [Signal]
    public delegate void ActionSelectedEventEventHandler(BattlerAction action, Battler source, Battler[] targets);

    /// <summary>
    /// Emitted when combat is initiated.
    /// </summary>
    [Signal]
    public delegate void CombatInitiatedEventHandler();

    /// <summary>
    /// Emitted when combat is finished.
    /// </summary>
    /// <param name="isPlayerVictory">True if the player won, false otherwise.</param>
    [Signal]
    public delegate void CombatFinishedEventHandler(bool isPlayerVictory);

    /// <summary>
    /// Singleton instance for global access to CombatEvents.
    /// </summary>
    public static CombatEvents? Instance { get; private set; }

    /// <summary>
    /// Called when the node enters the scene tree. Sets the singleton instance for global access.
    /// </summary>
    public override void _Ready()
    {
        Instance = this;
    }
}
