using Godot;
using System;
using System.Threading.Tasks;

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
    private CombatArena _activeArena = null;

    // Keep track of what music track was playing previously, and return to it once combat has finished.
    private AudioStream _previousMusicTrack = null;

    private CenterContainer _combatContainer;
    private Godot.Timer _transitionDelayTimer;

    public override void _Ready()
    {
        _combatContainer = GetNode<CenterContainer>("CenterContainer");
        _transitionDelayTimer = GetNode<Godot.Timer>("CenterContainer/TransitionDelay");

        var fieldEvents = GetNode("/root/FieldEvents");
        if (fieldEvents != null)
        {
            fieldEvents.Connect("combat_triggered", Callable.From((PackedScene arena) => StartAsync(arena)));
        }
    }

    /// <summary>
    /// Begin a combat. Takes a PackedScene as its only parameter, expecting it to be a CombatState
    /// object once instantiated.
    /// This is normally a response to <see cref="FieldEvents.CombatTriggered"/>.
    /// </summary>
    public async Task StartAsync(PackedScene arena)
    {
        System.Diagnostics.Debug.Assert(_activeArena == null, "Attempting to start a combat while one is ongoing!");

        var transition = GetNode("/root/Transition");
        transition.Call("cover", 0.2f);
        await ToSignal(transition, "finished");

        var newArena = (CombatArena)arena.Instantiate();
        System.Diagnostics.Debug.Assert(newArena != null, "Failed to initiate combat. Provided 'arena' argument is not a CombatArena.");

        _activeArena = newArena;
        _combatContainer.AddChild(_activeArena);

        _activeArena.TurnQueue.CombatFinished += async (bool isPlayerVictory) =>
        {
            await _DisplayCombatResultsDialog(isPlayerVictory);

            // Wait a short period of time and then fade the screen to black.
            _transitionDelayTimer.Start();
            await ToSignal(_transitionDelayTimer, Timer.SignalName.Timeout);
            transition.Call("cover", 0.2f);
            await ToSignal(transition, "finished");

            System.Diagnostics.Debug.Assert(_activeArena != null, "Combat finished but no active arena to clean up!");
            _activeArena.QueueFree();
            _activeArena = null;

            var music = GetNode("/root/Music");
            music.Call("play", _previousMusicTrack);
            _previousMusicTrack = null;

            // Whatever object started the combat will now be responsible for flow of the game. In
            // particular, the screen is still covered, so the combat-starting object will want to
            // decide what to do now that the outcome of the combat is known.
            var combatEvents = GetNode("/root/CombatEvents");
            if (combatEvents != null)
            {
                combatEvents.EmitSignal("combat_finished", isPlayerVictory);
            }
        };

        var music = GetNode("/root/Music");
        _previousMusicTrack = (AudioStream)music.Call("get_playing_track");
        music.Call("play", _activeArena.Music);

        var combatEvents = GetNode("/root/CombatEvents");
        if (combatEvents != null)
        {
            combatEvents.EmitSignal("combat_initiated");
        }

        // Before starting combat itself, reveal the screen again.
        // The Transition.clear() call is deferred since it follows on the heels of cover(), and needs a
        // frame to allow everything else to respond to Transition.finished.
        CallDeferred("_DeferredClearTransition", 0.2f);
        // Remove the await since we're not actually awaiting a signal here

        // Begin the combat. The turn queue takes over from here.
        _activeArena.Start();
    }

    private void _DeferredClearTransition(float duration)
    {
        var transition = GetNode("/root/Transition");
        transition.CallDeferred("clear", duration);
    }

    /// <summary>
    /// Displays a series of dialogue bubbles using Dialogic with information about the combat's outcome.
    /// </summary>
    private async Task _DisplayCombatResultsDialog(bool isPlayerVictory)
    {
        string playerPartyLeaderName = _activeArena.TurnQueue.Battlers.Players[0].Name;

        string[] timelineEvents;
        if (isPlayerVictory)
        {
            timelineEvents = _GetVictoryMessageEvents(playerPartyLeaderName);
        }
        else
        {
            timelineEvents = _GetLossMessageEvents(playerPartyLeaderName);
        }

        var combatRewardsTimeline = new DialogicTimeline();
        combatRewardsTimeline.Events = timelineEvents;
        var dialogic = GetNode("/root/Dialogic");
        dialogic.Call("start_timeline", combatRewardsTimeline);
        await ToSignal(dialogic, "timeline_ended");
    }

    // These two functions are placeholders for future logic for deciding combat outcomes.
    private string[] _GetVictoryMessageEvents(string leaderName)
    {
        string[] events = {
            $"{leaderName}'s party won the battle!"
        };
        events = events.Append("You wanted to find some coins, but animals have no pockets to carry them.").ToArray();
        return events;
    }

    private string[] _GetLossMessageEvents(string leaderName)
    {
        string[] events = {
            $"{leaderName}'s party lost the battle!"
        };
        return events;
    }
}
