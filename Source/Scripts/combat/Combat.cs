// <copyright file="Combat.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System;
using System.Threading.Tasks;
using Godot;

/// <summary>
/// Starts and ends combat, and manages the transition between the field game state and the combat game.
///
/// The battle is composed mainly from a <see cref="CombatArena"/>, which contains all necessary subelements such
/// as battlers, visual effects, music, etc.
///
/// This container handles the logic of switching between the field game state, the combat game
/// state, and the combat results screen (e.g. experience and levelling up, loot, etc.). It is
/// responsible for changing the music, playing screen transition animations, and other state-switch
/// elements.
/// </summary>
public partial class Combat : CanvasLayer
{
    private CombatArena activeArena;

    // Keep track of what music track was playing previously, and return to it once combat has finished.
    private AudioStream previousMusicTrack;

    private CenterContainer combatContainer;
    private Godot.Timer transitionDelayTimer;

    /// <inheritdoc/>
    public override void _Ready()
    {
        this.combatContainer = this.GetNode<CenterContainer>("CenterContainer");
        this.transitionDelayTimer = this.GetNode<Godot.Timer>("CenterContainer/TransitionDelay");

        var fieldEvents = this.GetNode("/root/FieldEvents");
        if (fieldEvents != null)
        {
            fieldEvents.Connect("combat_triggered", Callable.From((PackedScene arena) => this.StartAsync(arena)));
        }
    }

    /// <summary>
    /// Begin a combat. Takes a PackedScene as its only parameter, expecting it to be a CombatState
    /// object once instantiated.
    /// This is normally a response to <see cref="FieldEvents.CombatTriggered"/>.
    /// </summary>
    /// <returns><placeholder>A <see cref="Task"/> representing the asynchronous operation.</placeholder></returns>
    public async Task StartAsync(PackedScene arena)
    {
        System.Diagnostics.Debug.Assert(this.activeArena == null, "Attempting to start a combat while one is ongoing!");

        var transition = this.GetNode("/root/Transition");
        transition.Call("cover", 0.2f);
        await this.ToSignal(transition, "finished");

        var newArena = (CombatArena)arena.Instantiate();
        System.Diagnostics.Debug.Assert(newArena != null, "Failed to initiate combat. Provided 'arena' argument is not a CombatArena.");

        this.activeArena = newArena;
        this.combatContainer.AddChild(this.activeArena);

        this.activeArena.TurnQueue.CombatFinished += async (bool isPlayerVictory) =>
        {
            await DisplayCombatResultsDialog(isPlayerVictory).ConfigureAwait(false);

            // Wait a short period of time and then fade the screen to black.
            this.transitionDelayTimer.Start();
            await ToSignal(this.transitionDelayTimer, Timer.SignalName.Timeout);
            transition.Call("cover", 0.2f);
            await this.ToSignal(transition, "finished");

            System.Diagnostics.Debug.Assert(this.activeArena != null, "Combat finished but no active arena to clean up!");
            this.activeArena.QueueFree();
            this.activeArena = null;

            var music = this.GetNode("/root/Music");
            music.Call("play", this.previousMusicTrack);
            this.previousMusicTrack = null;

            // Whatever object started the combat will now be responsible for flow of the game. In
            // particular, the screen is still covered, so the combat-starting object will want to
            // decide what to do now that the outcome of the combat is known.
            var combatEvents = this.GetNode("/root/CombatEvents");
            if (combatEvents != null)
            {
                combatEvents.EmitSignal("combat_finished", isPlayerVictory);
            }
        };

        var music = this.GetNode("/root/Music");
        this.previousMusicTrack = (AudioStream)music.Call("get_playing_track");
        music.Call("play", this.activeArena.Music);

        var combatEvents = this.GetNode("/root/CombatEvents");
        if (combatEvents != null)
        {
            combatEvents.EmitSignal("combat_initiated");
        }

        // Before starting combat itself, reveal the screen again.
        // The Transition.clear() call is deferred since it follows on the heels of cover(), and needs a
        // frame to allow everything else to respond to Transition.finished.
        this.CallDeferred("_DeferredClearTransition", 0.2f);

        // Remove the await since we're not actually awaiting a signal here

        // Begin the combat. The turn queue takes over from here.
        this.activeArena.Start();
    }

    private void DeferredClearTransition(float duration)
    {
        var transition = this.GetNode("/root/Transition");
        transition.CallDeferred("clear", duration);
    }

    /// <summary>
    /// Displays a series of dialogue bubbles using Dialogic with information about the combat's outcome.
    /// </summary>
    private async Task DisplayCombatResultsDialog(bool isPlayerVictory)
    {
        string playerPartyLeaderName = this.activeArena.TurnQueue.Battlers.Players[0].Name;

        string[] timelineEvents;
        if (isPlayerVictory)
        {
            timelineEvents = GetVictoryMessageEvents(playerPartyLeaderName);
        }
        else
        {
            timelineEvents = GetLossMessageEvents(playerPartyLeaderName);
        }

        var combatRewardsTimeline = new DialogicTimeline();
        combatRewardsTimeline.Events = timelineEvents;
        var dialogic = this.GetNode("/root/Dialogic");
        dialogic.Call("start_timeline", combatRewardsTimeline);
        await this.ToSignal(dialogic, "timeline_ended");
    }

    // These two functions are placeholders for future logic for deciding combat outcomes.
    private static string[] GetVictoryMessageEvents(string leaderName)
    {
        string[] events =
        {
            $"{leaderName}'s party won the battle!",
        };
        events = events.Append("You wanted to find some coins, but animals have no pockets to carry them.").ToArray();
        return events;
    }

    private static string[] GetLossMessageEvents(string leaderName)
    {
        string[] events =
        {
            $"{leaderName}'s party lost the battle!",
        };
        return events;
    }
}
