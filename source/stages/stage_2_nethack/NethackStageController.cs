// <copyright file="NethackStageController.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using System;
using System.Threading.Tasks;
using Godot;
using OmegaSpiral.Source.Infrastructure;
using OmegaSpiral.Source.Scripts.Infrastructure;
using OmegaSpiral.Source.Scripts.Stages.Stage2;

namespace OmegaSpiral.Source.Stages.Stage2;

/// <summary>
/// Stage controller for Stage 2: Nethack Echo Chamber.
/// Extends StageBase to manage the complete stage lifecycle.
/// Uses NethackDirector (CinematicDirector pattern) for loading nethack.yaml.
/// Instantiates nethack_start.tscn and waits for completion.
/// </summary>
[GlobalClass]
public sealed partial class NethackStageController : StageBase
{
    private readonly NethackDirector _DataLoader = new();
    private Node? _StageScene;
    private TaskCompletionSource<bool>? _StageCompletion;

    /// <inheritdoc/>
    public override int StageId => 2;

    /// <inheritdoc/>
    public override async Task ExecuteStageAsync()
    {
        GD.Print("[NethackStageController] === Stage 2: Nethack Echo Chamber Starting ===");

        try
        {
            // Load the narrative plan using CinematicDirector pattern
            var plan = _DataLoader.GetPlan();
            GD.Print($"[NethackStageController] Loaded: '{plan.Script.Title}' ({plan.Script.Moments.Count} moments)");

            // Load the scene file
            var scenePath = "res://source/stages/stage_2_nethack/nethack_start.tscn";
            var scene = GD.Load<PackedScene>(scenePath);
            if (scene == null)
            {
                GD.PrintErr($"[NethackStageController] Failed to load scene: {scenePath}");
                EmitStageComplete();
                return;
            }

            // Instantiate the scene
            var sceneInstance = scene.Instantiate();
            if (sceneInstance == null)
            {
                GD.PrintErr("[NethackStageController] Failed to instantiate scene");
                EmitStageComplete();
                return;
            }

            _StageScene = sceneInstance;

            // Setup completion tracking
            _StageCompletion = new TaskCompletionSource<bool>();

            // Connect to scene's TreeExiting signal as a fallback completion indicator
            _StageScene.TreeExiting += OnSceneExiting;

            // Add scene to tree (this triggers _Ready on the scene)
            AddChild(_StageScene);
            GD.Print("[NethackStageController] Scene instantiated and added to tree");

            // Wait for completion
            // TODO: Replace with proper completion signal from stage scene
            var timeout = Task.Delay(30000);
            var completed = await Task.WhenAny(_StageCompletion.Task, timeout).ConfigureAwait(false);

            if (completed == timeout)
            {
                GD.PrintErr("[NethackStageController] Stage timed out after 30 seconds");
            }
            else
            {
                GD.Print("[NethackStageController] Stage completed successfully");
            }

            // Cleanup
            if (_StageScene != null && IsInstanceValid(_StageScene))
            {
                _StageScene.TreeExiting -= OnSceneExiting;
            }

            EmitStageComplete();
        }
        catch (Exception ex)
        {
            GD.PrintErr($"[NethackStageController] Error: {ex.Message}");
            GD.PrintErr(ex.StackTrace);
            EmitStageComplete();
        }
    }

    private void OnSceneExiting()
    {
        GD.Print("[NethackStageController] Stage scene is exiting scene tree");
        _StageCompletion?.TrySetResult(true);
    }
}
