// <copyright file="GhostCinematicDirector.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

namespace OmegaSpiral.Source.Stages.Stage1;

using System;
using System.Threading.Tasks;
using Godot;
using OmegaSpiral.Source.Backend.Narrative;
using OmegaSpiral.Source.Frontend;

/// <summary>
/// Lightweight stage orchestrator for Stage 1: Ghost Terminal.
/// Responsibilities:
/// - Load YAML script
/// - Instantiate scene
/// - Initialize GhostStageManager
/// - Listen for completion and emit StageComplete signal
///
/// All narrative logic delegated to GhostStageManager.
/// </summary>
[GlobalClass]
public sealed partial class GhostCinematicDirector : StageBase
{
    /// <inheritdoc/>
    public override int StageId => 1;

    /// <inheritdoc/>
    public override async Task ExecuteStageAsync()
    {
        GD.Print("[GhostCinematicDirector] === Stage 1: Ghost Terminal Starting ===");

        try
        {
            // 1. Load JSON script
            var script = NarrativeScriptJsonLoader.LoadJsonScript("res://source/frontend/stages/stage_1_ghost/ghost.json");
            if (script == null)
            {
                GD.PrintErr("[GhostCinematicDirector] Failed to load ghost.json");
                EmitStageComplete();
                return;
            }

            GD.Print($"[GhostCinematicDirector] Loaded: '{script.Title}' ({script.Scenes?.Count ?? 0} scenes)");

            // 2. Load scene (.tscn)
            var scenePath = "res://source/frontend/stages/stage_1_ghost/ghost_terminal.tscn";
            var packedScene = ResourceLoader.Load<PackedScene>(scenePath);
            if (packedScene == null)
            {
                GD.PrintErr("[GhostCinematicDirector] Failed to load ghost_terminal.tscn");
                EmitStageComplete();
                return;
            }

            var ghostTerminal = packedScene.Instantiate<Control>();
            AddChild(ghostTerminal);
            GD.Print("[GhostCinematicDirector] Scene instantiated and added to tree");

            // 3. Get GhostStageManager from scene
            var stageManager = ghostTerminal.GetNodeOrNull<GhostStageManager>("GhostStageManager");
            if (stageManager == null)
            {
                GD.PrintErr("[GhostCinematicDirector] Failed to find GhostStageManager in scene");
                EmitStageComplete();
                return;
            }

            // 4. Listen for completion
            stageManager.StageComplete += (scores) => OnStageComplete(ghostTerminal, scores);

            // 5. Play narrative script
            await stageManager.PlayScriptAsync(script).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            GD.PrintErr($"[GhostCinematicDirector] Error: {ex.Message}");
            GD.PrintErr(ex.StackTrace);
            EmitStageComplete();
        }
    }

    private void OnStageComplete(Control ghostTerminal, int[] dreamweaverScores)
    {
        GD.Print($"[GhostCinematicDirector] Stage 1 complete. Scores: Light={dreamweaverScores[0]}, Shadow={dreamweaverScores[1]}, Ambition={dreamweaverScores[2]}");

        // TODO: Pass scores to StageManager/GameManager
        // For now: TODO implementation

        if (ghostTerminal != null && IsInstanceValid(ghostTerminal))
        {
            ghostTerminal.QueueFree();
        }

        EmitStageComplete();
    }
}
