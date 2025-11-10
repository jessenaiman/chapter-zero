
// Copyright (c) Ωmega Spiral. All rights reserved.

using Godot;
using OmegaSpiral.Source.Scripts.Field.cutscenes.Templates.Combat;
using OmegaSpiral.Source.Scripts.Common.ScreenTransitions;

namespace OmegaSpiral.Source.Scripts.Combat;
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
public sealed partial class Combat : CanvasLayer
{
    private CombatArena? _activeArena;

    /// <summary>
    /// Keep track of what music track was playing previously, and return to it once combat has finished.
    /// </summary>
    private AudioStream? _previousMusicTrack;

    private CenterContainer? _combatContainer;
    private Godot.Timer? _transitionDelayTimer;

    /// <summary>
    /// Initializes the combat system when the node is ready.
    /// Sets up Ui references, connects to field events, and prepares for combat initiation.
    /// </summary>
    /// <inheritdoc/>
    public override void _Ready()
    {
        this._combatContainer = this.GetNodeOrNull<CenterContainer>("CenterContainer");
        this._transitionDelayTimer = this.GetNodeOrNull<Godot.Timer>("CenterContainer/TransitionDelay");

        // Log warnings if essential combat nodes are missing
        if (this._combatContainer == null)
        {
            GD.PushError("Combat: CenterContainer node not found. Combat Ui will not function.");
        }
        if (this._transitionDelayTimer == null)
        {
            GD.PushError("Combat: TransitionDelay timer not found. Combat transitions may not work correctly.");
        }

        try
        {
            var fieldEvents = this.GetNodeOrNull("/root/FieldEvents");
            if (fieldEvents != null)
            {
                fieldEvents.Connect("combat_triggered", Callable.From((PackedScene arena) => StartAsync(arena)));
            }
            else
            {
                GD.PushWarning("Combat: FieldEvents autoload not found. Combat may not be triggered properly.");
            }
        }
        catch (Exception ex)
        {
            GD.PrintErr($"Failed to connect to FieldEvents in Combat: {ex.Message}");
        }
    }

    /// <summary>
    /// Begin a combat. Takes a PackedScene as its only parameter, expecting it to be a CombatState
    /// object once instantiated.
    /// This is normally a response to <see cref="CombatTrigger"/>.
    /// </summary>
    /// <param name="arena">The <see cref="PackedScene"/> representing the combat arena to be instantiated.</param>
    /// <returns>A <see cref="Task"/> that represents the asynchronous operation of starting combat.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="arena"/> is <c>null</c>.</exception>
    /// <exception cref="InvalidOperationException"></exception>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidUnmaintainableCode")]
    public async Task StartAsync(PackedScene arena)
    {
        this.ValidateArena(arena);
        await this.SetupArena(arena).ConfigureAwait(false);
        this.SetupCombatFinishedHandler();
        this.SetupMusicAndEvents();
        this.RevealScreen();
    }

    private void ValidateArena(PackedScene arena)
    {
        if (arena == null)
        {
            throw new ArgumentNullException(nameof(arena), "The arena parameter cannot be null.");
        }

        System.Diagnostics.Debug.Assert(this._activeArena == null, "Attempting to start a combat while one is ongoing!");
    }

    private async Task SetupArena(PackedScene arena)
    {
        var transition = this.GetNodeOrNull("/root/Transition");
        if (transition == null)
        {
            GD.PushError("Combat: Transition autoload not found. Combat transitions will not work.");
            return;
        }

    transition.Call("cover", 0.2f);
    await this.ToSignal(transition, ScreenTransition.SignalName.Finished);

        var newArena = (CombatArena) arena.Instantiate();
        System.Diagnostics.Debug.Assert(newArena != null, "Failed to initiate combat. Provided 'arena' argument is not a CombatArena.");

        this._activeArena = newArena;
        if (this._combatContainer != null)
        {
            this._combatContainer.AddChild(this._activeArena);
        }
        else
        {
            throw new InvalidOperationException("Combat container is not initialized.");
        }
    }

    private void SetupCombatFinishedHandler()
    {
        if (this._activeArena?.TurnQueue != null)
        {
            this._activeArena.TurnQueue.CombatFinished += this.HandleCombatFinished;
        }
    }

    private async void HandleCombatFinished(bool isPlayerVictory)
    {
        this.DisplayCombatResultsDialogAsync(isPlayerVictory);

        // Wait a short period of time and then fade the screen to black.
        if (this._transitionDelayTimer != null)
        {
            this._transitionDelayTimer.Start();
        }
        else
        {
            throw new InvalidOperationException("Transition delay timer is not initialized.");
        }

        await this.ToSignal(this._transitionDelayTimer, "timeout");
        var transition = this.GetNodeOrNull("/root/Transition");
        if (transition != null)
        {
            transition.Call("cover", 0.2f);
            await this.ToSignal(transition, ScreenTransition.SignalName.Finished);
        }

        this._activeArena?.QueueFree();

        var music = this.GetNodeOrNull("/root/Music");
        if (music != null && this._previousMusicTrack != null)
        {
            music.Call("play", this._previousMusicTrack);
        }

        // Whatever object started the combat will now be responsible for flow of the game. In
        // particular, the screen is still covered, so the combat-starting object will want to
        // decide what to do now that the outcome of the combat is known.
        var combatEvents = this.GetNodeOrNull("/root/CombatEvents");
        if (combatEvents != null)
        {
            combatEvents.EmitSignal("combat_finished", isPlayerVictory);
        }
    }    private void SetupMusicAndEvents()
    {
        var music = this.GetNodeOrNull("/root/Music");
        if (music != null)
        {
            this._previousMusicTrack = (AudioStream)music.Call("get_playing_track");
            if (this._activeArena?.Music != null)
            {
                music.Call("play", this._activeArena.Music);
            }
        }

        var combatEvents = this.GetNodeOrNull("/root/CombatEvents");
        if (combatEvents != null)
        {
            combatEvents.EmitSignal("combat_initiated");
        }
    }

    private void RevealScreen()
    {
        // Before starting combat itself, reveal the screen again.
        // The Transition.clear() call is deferred since it follows on the heels of cover(), and needs a
        // frame to allow everything else to respond to Transition.finished.
        this.CallDeferred("DeferredClearTransition", 0.2f);
    }

    private void DeferredClearTransition(float duration)
    {
        var transition = this.GetNodeOrNull("/root/Transition");
        if (transition != null)
        {
            transition.CallDeferred("clear", duration);
        }
    }

    /// <summary>
    /// Displays a series of dialogue bubbles using Dialogic with information about the combat's outcome.
    /// </summary>
    /// <param name="leaderName">The name of the party leader.</param>
    /// <returns>An array of strings representing the victory message events.</returns>
    /// <remarks>These two functions are placeholders for future logic for deciding combat outcomes.</remarks>
    private static string[] GetVictoryMessageEvents(string leaderName)
    {
        return new[] { $"{leaderName}'s party won the battle!", "You wanted to find some coins, but animals have no pockets to carry them." };
    }

    private static string[] GetLossMessageEvents(string leaderName)
    {
        string[] events =
        {
            $"{leaderName}'s party lost the battle!",
        };
        return events;
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
    private void DisplayCombatResultsDialogAsync(bool isPlayerVictory)
    {
        if (!this.TryGetPlayerPartyLeaderName(out string playerPartyLeaderName))
        {
            GD.PrintErr("Cannot display combat results - no player battlers found");
            return;
        }

        string[] timelineEvents = Combat.GetCombatResultMessageEvents(isPlayerVictory, playerPartyLeaderName);

        // For now, skip the dialogic integration and just log the result
        GD.Print($"Combat finished - Player victory: {isPlayerVictory}");
    }

    private bool TryGetPlayerPartyLeaderName(out string leaderName)
    {
        leaderName = string.Empty;

        var players = this._activeArena?.TurnQueue?.Battlers?.Players;
        if (players == null || players.Length == 0)
        {
            return false;
        }

        leaderName = players[0]?.Name ?? "Unknown";
        return true;
    }

    private static string[] GetCombatResultMessageEvents(bool isPlayerVictory, string leaderName)
    {
        return isPlayerVictory
            ? GetVictoryMessageEvents(leaderName)
            : GetLossMessageEvents(leaderName);
    }
}
