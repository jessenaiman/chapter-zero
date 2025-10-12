// <copyright file="CombatActor.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System;
using Godot;

/// <summary>
/// A battle actor is any character or object that performs actions during combat.
/// This could be a player, an enemy, or even special events like traps or timed effects.
///
/// Actors provide logic to objects that want to act in combat. An actor is typically applied to a
/// given <see cref="Battler"/>, though other objects may wish to also make use of actors. For example, a combat
/// event that deals damage to all battlers on every third turn would make use of a custom CombatActor,
/// even though it is not present as a Battler.<br/><br/>
///
/// Actors take turns based on their initiative value. Higher initiative means
/// they act earlier in the turn order. Each actor completes their full turn before
/// the next one begins, following whatever logic you define in your custom actor classes.<br/><br/>
///
/// <b>Important:</b> This CombatActor class is like a template that specific actor types extend.
/// For example, a player-controlled actor would show the action menu UI, while an AI enemy
/// might have different attack patterns or decision-making logic.
/// </summary>
public abstract partial class CombatActor : Node2D
{
    /// <summary>
    /// Emitted whenever the actor's turn is finished. You should emit this only
    /// after all actions and animations are complete.
    /// </summary>
    [Signal]
    public delegate void TurnFinishedEventHandler();

    /// <summary>
    /// The name of the node group that will contain all combat Actors.
    /// </summary>
    public const string Group = "combat_actors";

    /// <summary>
    /// Gets or sets influences when this actor takes their turn in combat. This is a speed rating:
    /// actors with higher initiative values (closer to 1.0) will act earlier in the turn order,
    /// while lower values (closer to 0.0) make them act later.
    /// </summary>
    [Export]
    public float Initiative { get; set; } = 1.0f;

    /// <summary>
    /// Gets or sets a value indicating whether if this is <b>true</b>, this actor takes part in the battle. Inactive actors won't take turns.
    /// </summary>
    [Export]
    public bool IsActive { get; set; } = false;

    /// <summary>
    /// Gets or sets a value indicating whether if this is <b>true</b>, this actor is controlled by the player. Use this to
    /// differentiate between player-controlled actors and AI-controlled ones.
    /// </summary>
    [Export]
    public bool IsPlayer { get; set; } = false;

    /// <summary>
    /// Gets or sets a value indicating whether describes whether or not the CombatActor has taken a turn during this combat round.
    /// </summary>
    public bool HasActedThisRound { get; set; }

    public static bool Sort(CombatActor a, CombatActor b)
    {
        return a.Initiative > b.Initiative;
    }

    /// <inheritdoc/>
    public override void _Ready()
    {
        this.AddToGroup(Group);
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        string msg = $"{this.Name} (CombatActor)";
        if (!this.IsActive)
        {
            msg += " - INACTIVE";
        }
        else if (this.HasActedThisRound)
        {
            msg += " - HAS ACTED";
        }

        return msg;
    }

    public virtual void MeleeAttack()
    {
        GD.Print("Attack!");
    }

    public virtual async Task StartTurnAsync()
    {
        GD.Print(this.GetParent().Name, " starts their turn!");

        var timer = this.GetTree().CreateTimer(1.5f);
        await this.ToSignal(timer, SceneTreeTimer.SignalName.Timeout);
        this.EmitSignal(SignalName.TurnFinished);
    }
}
