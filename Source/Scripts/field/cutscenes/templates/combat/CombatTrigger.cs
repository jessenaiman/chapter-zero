// <copyright file="CombatTrigger.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System;
using System.Threading.Tasks;
using Godot;

[Tool]
public partial class CombatTrigger : Trigger
{
    [Export]
    public PackedScene CombatArena { get; set; }

    /// <inheritdoc/>
    protected override async void Execute()
    {
        // Let other systems know that a combat has been triggered and then wait for its outcome.
        // FieldEvents.combat_triggered.emit(combat_arena);

        // var did_player_win = await CombatEvents.combat_finished;
        // For now, we'll simulate the await of CombatEvents.combat_finished
        // This would typically be replaced with actual event handling when the combat system is implemented
        var tcs = new TaskCompletionSource<bool>();

        // In a real implementation, this would be connected to CombatEvents.CombatFinished
        // For now, we'll just simulate a result after a short delay
        var timer = new Timer();
        timer.Timeout += () => tcs.SetResult(true); // Assuming player wins for this example
        AddChild(timer);
        timer.Start(0.1f); // Short delay to simulate combat

        bool didPlayerWin = await tcs.Task.ConfigureAwait(false);

        // The combat ends with a covered screen, and so we fix that here.
        // Transition.clear.call_deferred(0.2)
        // await Transition.finished
        // For now, we'll just simulate the transition

        // We want to run post-combat events. In some cases, this may not involve much, such as playing a
        // game-over screen or removing an AI combat-starting gamepiece from the field.
        // In some cases, however, we'll want a dialogue to play or some creative event to occur if, for
        // example, the player lost a difficult but non-essential battle.
        if (didPlayerWin)
        {
            await this.RunVictoryCutscene();
        }
        else
        {
            await this.RunLossCutscene();
        }
    }

    /// <summary>
    /// The following method may be overwridden to allow for custom behaviour following a combat victory.
    /// Examples include adding an item to the player's inventory, running a dialogue, removing an enemy
    /// <see cref="Gamepiece"/>, etc.
    /// </summary>
    protected virtual async void RunVictoryCutscene()
    {
        await this.ToSignal(this.GetTree(), SceneTree.SignalName.ProcessFrame);
    }

    /// <summary>
    /// The following method may be overwridden to allow for custom behaviour following a combat loss.
    /// In most cases this may result in a gameover, but in others it may run a cutscene, change some
    /// sort of event flag, etc.
    /// </summary>
    protected virtual async void RunLossCutscene()
    {
        await this.ToSignal(this.GetTree(), SceneTree.SignalName.ProcessFrame);
    }
}
