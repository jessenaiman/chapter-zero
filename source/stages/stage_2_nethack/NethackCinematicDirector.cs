// <copyright file="NethackCinematicDirector.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using System;
using System.Threading.Tasks;
using Godot;
using OmegaSpiral.Source.Infrastructure;
using OmegaSpiral.Source.Narrative;

namespace OmegaSpiral.Source.Stages.Stage2;

/// <summary>
/// Stage controller for Stage 2: Nethack.
/// Thin orchestrator that loads the narrative script and delegates playback to the generic engine.
/// The NarrativeUi component handles all rendering; the director only manages the stage lifecycle.
/// </summary>
[GlobalClass]
public sealed partial class NethackCinematicDirector : StageBase
{
    private readonly NethackDataLoader _DataLoader = new();
    private NarrativeUi? _NarrativeUi;

    /// <inheritdoc/>
    public override int StageId => 2;

    /// <inheritdoc/>
    public override async Task ExecuteStageAsync()
    {
        GD.Print("[NethackCinematicDirector] === Stage 2: Nethack Starting ===");

        try
        {
            // Load the narrative plan using CinematicDirector pattern
            var plan = _DataLoader.GetPlan();
            GD.Print($"[NethackCinematicDirector] Loaded: '{plan.Script.Title}' ({plan.Script.Scenes?.Count ?? 0} scenes)");

            // Create a NarrativeUi instance (no scene file needed – UI is instantiated programmatically)
            _NarrativeUi = new NarrativeUi();
            if (_NarrativeUi == null)
            {
                GD.PrintErr("[NethackCinematicDirector] Failed to create NarrativeUi");
                EmitStageComplete();
                return;
            }

            // Add UI to the scene tree (triggers _Ready)
            AddChild(_NarrativeUi);
            GD.Print("[NethackCinematicDirector] NarrativeUi instantiated and added to tree");

            // Run the generic narrative engine to drive the script
            var engine = new NarrativeEngine();
            await engine.PlayAsync(plan.Script, _NarrativeUi).ConfigureAwait(false);

            GD.Print("[NethackCinematicDirector] Narrative engine completed");

            // Cleanup
            if (_NarrativeUi != null && IsInstanceValid(_NarrativeUi))
            {
                _NarrativeUi.QueueFree();
            }

            EmitStageComplete();
        }
        catch (Exception ex)
        {
            GD.PrintErr($"[NethackCinematicDirector] Error: {ex.Message}");
            GD.PrintErr(ex.StackTrace);
            EmitStageComplete();
        }
    }
}
