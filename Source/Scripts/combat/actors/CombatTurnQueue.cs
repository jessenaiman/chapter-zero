namespace OmegaSpiral.Source.Scripts.Combat.Actors;

// <copyright file="CombatTurnQueue.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using System.Linq;
using Godot;
using OmegaSpiral.Source.Scripts.Combat.Battlers;

/// <summary>
/// Manages the turn-based combat system, coordinating the sequence of actions between all combat participants.
/// Handles round progression, actor initiative ordering, and battle end conditions.
/// </summary>
[Icon("res://Source/Scripts/combat/actors/icon_turn_queue.png")]
public partial class CombatTurnQueue : Node
{
    private int roundCount = 1;

    /// <summary>
    /// Emitted whenever the combat logic has finished, including all animation details.
    /// </summary>
    /// <param name="hasPlayerWon">Indicates whether the player won the combat encounter.</param>
    [Signal]
    public delegate void FinishedEventHandler(bool hasPlayerWon);

    /// <summary>
    /// Gets a list of the combat participants, in <see cref="BattlerList"/> form. This object is created by the turn
    /// queue from children <see cref="Battler"/>s and then made available to other combat systems.
    /// </summary>
    public BattlerRoster BattlerRoster { get; private set; } = null!;

    /// <summary>
    /// Gets or sets tracks which combat round is currently being played. Every round, all active Actors will get a
    /// turn to act.
    /// </summary>
    public int RoundCount
    {
        get => this.roundCount;
        set
        {
            this.roundCount = value;
            GD.Print($"\nBegin turn {this.roundCount}");
        }
    }

    /// <inheritdoc/>
    public override void _Ready()
    {
        this.BattlerRoster = new BattlerRoster(this.GetTree());
    }

    /// <summary>
    /// Initializes and starts the combat turn queue, resetting the round count and beginning the first turn.
    /// </summary>
    public void Start()
    {
        this.RoundCount = 1;
        this.CallDeferred("_NextTurn");
    }

    /// <summary>
    /// Retrieves all combat actors currently in the scene that belong to the combat actor group.
    /// </summary>
    /// <returns>An array of all combat actors in the scene.</returns>
    public CombatActor[] GetActors()
    {
        var actorList = this.GetTree().GetNodesInGroup(CombatActor.Group);
        var combatActors = new CombatActor[actorList.Count];
        for (int i = 0; i < actorList.Count; i++)
        {
            combatActors[i] = (CombatActor) actorList[i];
        }

        return combatActors;
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        CombatActor[] actors = this.GetActors();
        var sortedActors = actors.OrderByDescending(actor => actor.Initiative).ToArray();

        string msg = $"\n{this.Name} (CombatTurnQueue) - round {this.RoundCount}";
        foreach (CombatActor actor in sortedActors)
        {
            msg += $"\n\t{actor}";
        }

        return msg;
    }

    private async void NextTurn()
    {
        // Check for battle end conditions, that one side has been downed.
        if (BattlerRoster.AreBattlersDefeated(this.BattlerRoster.GetPlayerBattlers()))
        {
            this.CallDeferred("EmitFinished", false);
            return;
        }
        else if (BattlerRoster.AreBattlersDefeated(this.BattlerRoster.GetEnemyBattlers()))
        {
            this.CallDeferred("EmitFinished", true);
            return;
        }

        // Check for an active actor. If there are none, it may be that the turn has finished and all
        // actors can have their has_acted_this_turn flag reset.
        CombatActor? nextActor = this.GetNextActor();
        if (nextActor == null)
        {
            this.ResetHasActedFlag();

            // If there is no actor now, there is a situation where the only remaining Battler's don't
            // have assigned actors. In other words, all controlled actors (player included) are downed.
            // In this case, register as a player loss.
            nextActor = this.GetNextActor();
            if (nextActor == null)
            {
                this.EmitSignal(SignalName.Finished, false);
                return;
            }

            this.RoundCount += 1;
        }

        // Connect to the actor's turn_finished signal. The actor is guaranteed to emit the signal,
        // even if it will be freed at the end of this frame.
        // However, we'll call_defer the next turn, since the current actor may have been downed on its
        // turn and we need a frame to process the change.
        nextActor.TurnFinished += () =>
        {
            nextActor.HasActedThisRound = true;
            this.CallDeferred("_NextTurn");
        };

        await nextActor.StartTurnAsync().ConfigureAwait(false);
    }

    private void EmitFinished(bool hasPlayerWon)
    {
        this.EmitSignal(SignalName.Finished, hasPlayerWon);
    }

    private CombatActor? GetNextActor()
    {
        CombatActor[] actors = this.GetActors();

        // Sort actors by initiative (descending order)
        var sortedActors = actors.OrderByDescending(actor => actor.Initiative).ToArray();

        var readyToActActors = sortedActors.Where(actor => actor.IsActive && !actor.HasActedThisRound).ToArray();
        if (readyToActActors.Length == 0)
        {
            return null;
        }

        return readyToActActors[0];
    }

    private void ResetHasActedFlag()
    {
        foreach (CombatActor actor in this.GetActors())
        {
            actor.HasActedThisRound = false;
        }
    }
}
