// <copyright file="NethackHub.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using Godot;
using System.Threading.Tasks;
using OmegaSpiral.Source.Scripts.Infrastructure;

namespace OmegaSpiral.Source.Stages.Stage2;

/// <summary>
/// <summary>
/// Stage 2 controller managing the Nethack-inspired dungeon sequence.
/// Inherits from StageController for unified stage orchestration.
/// Tracks Dreamweaver affinity and reports scores on completion.
/// </summary>
[GlobalClass]
public partial class NethackHub : StageController
{
    private const string Stage2ManifestPath = "res://source/stages/stage_2/stage_2_manifest.json";

    /// <inheritdoc/>
    protected override int StageId => 2;

    /// <inheritdoc/>
    protected override string StageManifestPath => Stage2ManifestPath;

    /// <summary>Emitted when the entire Nethack stage completes.</summary>
    /// <param name="claimedDreamweaver">The Dreamweaver that claimed the player (based on affinity).</param>
    [Signal]
    public delegate void StageCompleteEventHandler(string claimedDreamweaver);

    /// <inheritdoc/>
    protected override async Task OnStageInitializeAsync()
    {
        GD.Print("[NethackHub] Initializing Stage 2 - Nethack Chambers");

        // Start scene progression
        await AdvanceToNextSceneAsync().ConfigureAwait(false);
    }

    /// <inheritdoc/>
    protected override async Task OnStageCompleteAsync()
    {
        string claimedDreamweaver = DetermineAffinityLeader() ?? "light";
        GD.Print($"[NethackHub] Stage 2 complete. Claimed by: {claimedDreamweaver}");

        // Report scores to GameState (handled by base class)
        await base.OnStageCompleteAsync().ConfigureAwait(false);

        // Emit stage-specific completion signal
        EmitSignal(SignalName.StageComplete, claimedDreamweaver);
    }
}
