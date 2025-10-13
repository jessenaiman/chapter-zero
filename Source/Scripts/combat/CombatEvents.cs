// <copyright file="CombatEvents.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System;
using Godot;

/// <summary>
/// A signal bus to connect distant scenes to various combat-exclusive events.
/// </summary>
public partial class CombatEvents : Node
{
    /// <summary>
    /// Emitted whenever a combat has been setup and is ready to become the active 'game state'. At this
    /// point, the screen is fully covered by the <see cref="ScreenTransition"/> autoload.
    /// </summary>
    /// <param name="arena">The packed scene representing the combat arena.</param>
    [Signal]
    public delegate void CombatInitiatedEventHandler(PackedScene arena);

    /// <summary>
    /// Emitted whenever the player has finished with the combat state regardless of whether or not the
    /// combat was won by the player. At this point the screen has faded to black and any events that
    /// immediately follow the combat may occur.
    /// </summary>
    /// <param name="isPlayerVictory">Indicates whether the player won the combat.</param>
    [Signal]
    public delegate void CombatFinishedEventHandler(bool isPlayerVictory);

    /// <summary>
    /// Emitted whenever a player battler is selected, prompting the player to choose an action.
    /// </summary>
    /// <param name="battler">The battler that was selected.</param>
    [Signal]
    public delegate void PlayerBattlerSelectedEventHandler(Battler battler);

    /// <summary>
    /// Emitted whenever a battler has selected an action to perform once it is
    /// <see cref="Battler.ReadyToAct"/>.
    /// </summary>
    /// <param name="action">The action that was selected.</param>
    /// <param name="source">The battler performing the action.</param>
    /// <param name="targets">The battlers that are the targets of the action.</param>
    [Signal]
    public delegate void ActionSelectedEventHandler(BattlerAction action, Battler source, Battler[] targets);
}
