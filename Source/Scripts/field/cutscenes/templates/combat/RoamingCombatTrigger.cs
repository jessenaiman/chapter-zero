// <copyright file="RoamingCombatTrigger.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using System;
using System.Threading.Tasks;
using Godot;

/// <summary>
/// Represents a roaming combat trigger that handles victory and loss cutscenes for roaming encounters.
/// This trigger automatically removes itself after a victory and handles game-over scenarios on loss.
/// </summary>
[Tool]
public partial class RoamingCombatTrigger : CombatTrigger
{
    /// <summary>
    /// If the player has defeated this 'roaming encounter', remove the encounter.
    /// </summary>
    /// <inheritdoc/>
    /// <returns>A task representing the asynchronous operation.</returns>
    protected override Task RunVictoryCutsceneAsync()
    {
        this.QueueFree();
        return Task.CompletedTask;
    }

    /// <summary>
    /// If the player has lost to this 'roaming encounter', play the game-over screen.
    /// </summary>
    /// <inheritdoc/>
    /// <returns>A task representing the asynchronous operation.</returns>
    protected override async Task RunLossCutsceneAsync()
    {
        await this.ToSignal(this.GetTree(), SceneTree.SignalName.ProcessFrame);
    }
}
