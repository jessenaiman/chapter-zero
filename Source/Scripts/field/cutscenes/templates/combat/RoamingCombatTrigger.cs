// <copyright file="RoamingCombatTrigger.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System;
using System.Threading.Tasks;
using Godot;

[Tool]
public partial class RoamingCombatTrigger : CombatTrigger
{
    // If the player has defeated this 'roaming encounter', remove the encounter.
    /// <inheritdoc/>
    protected override async void RunVictoryCutscene()
    {
        this.QueueFree();
    }

    // If the player has lost to this 'roaming encounter', play the game-over screen.
    /// <inheritdoc/>
    protected override async void RunLossCutscene()
    {
        await this.ToSignal(this.GetTree(), SceneTree.SignalName.ProcessFrame);
    }
}
