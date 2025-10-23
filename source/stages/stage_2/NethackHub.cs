// <copyright file="EchoHub.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using Godot;
using System.Threading.Tasks;
using OmegaSpiral.Source.Scripts.Infrastructure;
using OmegaSpiral.Source.Scripts.Stages.Stage2;

namespace OmegaSpiral.Source.Stages.Stage2;

/// <summary>
/// Stage 2 controller managing the Echo Chamber stage.
/// Inherits from StageController for unified stage orchestration.
/// Tracks Dreamweaver affinity and reports scores on completion.
/// </summary>
[GlobalClass]
public partial class EchoHub : StageController
{
    private const string Stage2ManifestPath = "res://source/stages/stage_2/stage_2_manifest.json";

    /// <inheritdoc/>
    protected override int StageId => 2;

    /// <inheritdoc/>
    protected override string StageManifestPath => Stage2ManifestPath;

    /// <summary>
    /// <summary>
    /// Emitted when the entire Echo Chamber stage completes.
    /// </summary>
    /// <param name="claimedDreamweaver">The Dreamweaver that claimed the player (based on affinity).</param>
    [Signal]
    public delegate void StageCompleteEventHandler(string claimedDreamweaver);

    /// <inheritdoc/>
    protected override async Task OnStageInitializeAsync()
    {
        GD.Print("[EchoHub] Initializing Stage 2 - Echo Chamber");

        // Start scene progression
        await AdvanceToNextSceneAsync();
    }

    /// <inheritdoc/>
    protected override async Task OnStageCompleteAsync()
    {
        string claimedDreamweaver = DetermineAffinityLeader() ?? "light";
        GD.Print($"[EchoHub] Stage 2 complete. Claimed by: {claimedDreamweaver}");

        // Report scores to GameState (handled by base class)
        await base.OnStageCompleteAsync();

        // Emit stage-specific completion signal
        EmitSignal(SignalName.StageComplete, claimedDreamweaver);
    }
}
