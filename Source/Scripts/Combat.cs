using Godot;
using System;
using System.Threading.Tasks;

/// <summary>
/// Starts and ends combat, and manages the transition between the field game state and the combat game.
/// The battle is composed mainly from a CombatArena, which contains all necessary subelements such
/// as battlers, visual effects, music, etc.
/// This container handles the logic of switching between the field game state, the combat game
/// state, and the combat results screen (e.g. experience and levelling up, loot, etc.). It is
/// responsible for changing the music, playing screen transition animations, and other state-switch
/// elements.
/// </summary>
public partial class Combat : CanvasLayer
{
    private CombatArena activeArena = null;

    // Keep track of what music track was playing previously, and return to it once combat has finished.
    private AudioStream previousMusicTrack = null;

    private Godot.Timer transitionDelayTimer;
    private CenterContainer combatContainer;

    public override void _Ready()
    {
        // FieldEvents.CombatTriggered += Start;

        combatContainer = GetNode<CenterContainer>("CenterContainer");
        transitionDelayTimer = GetNode<Godot.Timer>("CenterContainer/TransitionDelay");
    }

    /// <summary>
    /// Begin a combat. Takes a PackedScene as its only parameter, expecting it to be a CombatState
    /// object once instantiated.
    /// This is normally a response to FieldEvents.CombatTriggered.
    /// </summary>
    public async void Start(PackedScene arena)
    {
        if (activeArena != null)
        {
            GD.PrintErr("Attempting to start a combat while one is ongoing!");
            return;
        }

        // await Transition.Cover(0.2f);

        var newArena = arena.Instantiate() as CombatArena;
        if (newArena == null)
        {
            GD.PrintErr("Failed to initiate combat. Provided 'arena' argument is not a CombatArena.");
            return;
        }

        activeArena = newArena;
        combatContainer.AddChild(activeArena);

        activeArena.TurnQueue.CombatFinished += OnCombatFinished;

        previousMusicTrack = Music.GetPlayingTrack();
        Music.Play(activeArena.Music);

        // CombatEvents.CombatInitiated.Emit();

        // Before starting combat itself, reveal the screen again.
        // The Transition.Clear() call is deferred since it follows on the heels of cover(), and needs a
        // frame to allow everything else to respond to Transition.Finished.
        // Transition.Clear.CallDeferred(0.2f);
        // await ToSignal(Transition, "finished");

        // Begin the combat. The turn queue takes over from here.
        activeArena.Start();
    }

    /// <summary>
    /// Callback when combat is finished
    /// </summary>
    private async void OnCombatFinished(bool isPlayerVictory)
    {
        await DisplayCombatResultsDialog(isPlayerVictory);

        // Wait a short period of time and then fade the screen to black.
        transitionDelayTimer.Start();
        await ToSignal(transitionDelayTimer, Godot.Timer.SignalName.Timeout);
        // await Transition.Cover(0.2f);

        if (activeArena != null)
        {
            activeArena.QueueFree();
            activeArena = null;
        }

        Music.Play(previousMusicTrack);
        previousMusicTrack = null;

        // Whatever object started the combat will now be responsible for flow of the game. In
        // particular, the screen is still covered, so the combat-starting object will want to
        // decide what to do now that the outcome of the combat is known.
        // CombatEvents.CombatFinished.Emit(isPlayerVictory);
    }

    /// <summary>
    /// Displays a series of dialogue bubbles using Dialogic with information about the combat's outcome.
    /// </summary>
    private async Task DisplayCombatResultsDialog(bool isPlayerVictory)
    {
        if (activeArena == null || activeArena.TurnQueue == null || activeArena.TurnQueue.Battlers == null)
        {
            return;
        }

        var playerPartyLeaderName = activeArena.TurnQueue.Battlers.Players[0]?.Name ?? "Player";

        string[] timelineEvents;
        if (isPlayerVictory)
        {
            timelineEvents = GetVictoryMessageEvents(playerPartyLeaderName);
        }
        else
        {
            timelineEvents = GetLossMessageEvents(playerPartyLeaderName);
        }

        // var combatRewardsTimeline = new DialogicTimeline();
        // combatRewardsTimeline.Events = timelineEvents.ToList();
        // Dialogic.StartTimeline(combatRewardsTimeline);
        // await ToSignal(Dialogic, "timeline_ended");

        // Placeholder for dialog display
        GD.Print($"Combat result: {(isPlayerVictory ? "Victory" : "Defeat")} for {playerPartyLeaderName}");
        await Task.Delay(1000); // Simulate dialog display time
    }

    /// <summary>
    /// Gets victory message events
    /// </summary>
    private string[] GetVictoryMessageEvents(string leaderName)
    {
        return new string[]
        {
            $"{leaderName}'s party won the battle!",
            "You wanted to find some coins, but animals have no pockets to carry them."
        };
    }

    /// <summary>
    /// Gets loss message events
    /// </summary>
    private string[] GetLossMessageEvents(string leaderName)
    {
        return new string[]
        {
            $"{leaderName}'s party lost the battle!"
        };
    }
}
