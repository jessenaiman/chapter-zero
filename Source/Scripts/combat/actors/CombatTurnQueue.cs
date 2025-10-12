using Godot;
using System;
using System.Linq;

[Icon("res://src/combat/actors/icon_turn_queue.png")]
public partial class CombatTurnQueue : Node
{
    /// <summary>
    /// Emitted whenever the combat logic has finished, including all animation details.
    /// </summary>
    [Signal]
    public delegate void FinishedEventHandler(bool hasPlayerWon);

    /// <summary>
    /// A list of the combat participants, in <see cref="BattlerList"/> form. This object is created by the turn
    /// queue from children <see cref="Battler"/>s and then made available to other combat systems.
    /// </summary>
    public BattlerRoster BattlerRoster { get; private set; }

    private int _roundCount = 1;

    /// <summary>
    /// Tracks which combat round is currently being played. Every round, all active Actors will get a
    /// turn to act.
    /// </summary>
    public int RoundCount
    {
        get => _roundCount;
        set
        {
            _roundCount = value;
            GD.Print($"\nBegin turn {_roundCount}");
        }
    }

    public override void _Ready()
    {
        BattlerRoster = new BattlerRoster(GetTree());
    }

    public void Start()
    {
        RoundCount = 1;
        CallDeferred("_NextTurn");
    }

    public CombatActor[] GetActors()
    {
        var actorList = GetTree().GetNodesInGroup(CombatActor.Group);
        var combatActors = new CombatActor[actorList.Count];
        for (int i = 0; i < actorList.Count; i++)
        {
            combatActors[i] = (CombatActor)actorList[i];
        }
        return combatActors;
    }

    private async void _NextTurn()
    {
        // Check for battle end conditions, that one side has been downed.
        if (BattlerRoster.AreBattlersDefeated(BattlerRoster.GetPlayerBattlers()))
        {
            CallDeferred("EmitFinished", false);
            return;
        }
        else if (BattlerRoster.AreBattlersDefeated(BattlerRoster.GetEnemyBattlers()))
        {
            CallDeferred("EmitFinished", true);
            return;
        }

        // Check for an active actor. If there are none, it may be that the turn has finished and all
        // actors can have their has_acted_this_turn flag reset.
        CombatActor nextActor = _GetNextActor();
        if (nextActor == null)
        {
            _ResetHasActedFlag();

            // If there is no actor now, there is a situation where the only remaining Battler's don't
            // have assigned actors. In other words, all controlled actors (player included) are downed.
            // In this case, register as a player loss.
            nextActor = _GetNextActor();
            if (nextActor == null)
            {
                EmitSignal(SignalName.Finished, false);
                return;
            }

            RoundCount += 1;
        }

        // Connect to the actor's turn_finished signal. The actor is guaranteed to emit the signal,
        // even if it will be freed at the end of this frame.
        // However, we'll call_defer the next turn, since the current actor may have been downed on its
        // turn and we need a frame to process the change.
        nextActor.TurnFinished += () =>
        {
            nextActor.HasActedThisRound = true;
            CallDeferred("_NextTurn");
        };

        await nextActor.StartTurnAsync();
    }

    private void EmitFinished(bool hasPlayerWon)
    {
        EmitSignal(SignalName.Finished, hasPlayerWon);
    }

    private CombatActor _GetNextActor()
    {
        CombatActor[] actors = GetActors();
        // Sort actors by initiative (descending order)
        var sortedActors = actors.OrderByDescending(actor => actor.Initiative).ToArray();

        var readyToActActors = sortedActors.Where(actor => actor.IsActive && !actor.HasActedThisRound).ToArray();
        if (readyToActActors.Length == 0)
        {
            return null;
        }

        return readyToActActors[0];
    }

    private void _ResetHasActedFlag()
    {
        foreach (CombatActor actor in GetActors())
        {
            actor.HasActedThisRound = false;
        }
    }

    public override string ToString()
    {
        CombatActor[] actors = GetActors();
        var sortedActors = actors.OrderByDescending(actor => actor.Initiative).ToArray();

        string msg = $"\n{Name} (CombatTurnQueue) - round {RoundCount}";
        foreach (CombatActor actor in sortedActors)
        {
            msg += $"\n\t{actor}";
        }
        return msg;
    }
}
