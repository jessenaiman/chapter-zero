using System;
using System.Threading.Tasks;
using Godot;
using OmegaSpiral.Source.Infrastructure;
using OmegaSpiral.Source.Narrative;
using OmegaSpiral.Source.Scripts.Infrastructure;
using OmegaSpiral.Source.Stages.Stage1;

namespace OmegaSpiral.Source.Scripts.Stages.Stage1;

/// <summary>
/// Stage controller for Stage 1: Ghost Terminal.
/// Extends StageBase to manage the complete stage lifecycle.
/// Uses GhostDataLoader (CinematicDirector pattern) for loading ghost.yaml.
/// Instantiates ghost_terminal.tscn and waits for completion.
/// </summary>
[GlobalClass]
public sealed partial class GhostCinematicDirector : StageBase
{
    private readonly GhostDataLoader _DataLoader = new();
    private GhostUi? _GhostUi;
    private TaskCompletionSource<bool>? _StageCompletion;

    /// <inheritdoc/>
    public override int StageId => 1;

    /// <inheritdoc/>
    public override async Task ExecuteStageAsync()
    {
        GD.Print("[GhostCinematicDirector] === Stage 1: Ghost Terminal Starting ===");

        try
        {
            // Load the narrative plan using CinematicDirector pattern
            var plan = _DataLoader.GetPlan();
            GD.Print($"[GhostCinematicDirector] Loaded: '{plan.Script.Title}' ({plan.Script.Scenes.Count} scenes)");

            // Load the scene file
            var scenePath = "res://source/stages/stage_1_ghost/ghost_terminal.tscn";
            var scene = GD.Load<PackedScene>(scenePath);
            if (scene == null)
            {
                GD.PrintErr($"[GhostCinematicDirector] Failed to load scene: {scenePath}");
                EmitStageComplete();
                return;
            }

            // Instantiate the scene
            var sceneInstance = scene.Instantiate();
            if (sceneInstance == null)
            {
                GD.PrintErr("[GhostCinematicDirector] Failed to instantiate scene");
                EmitStageComplete();
                return;
            }

            // Verify it's a GhostUi node
            _GhostUi = sceneInstance as GhostUi;
            if (_GhostUi == null)
            {
                GD.PrintErr($"[GhostCinematicDirector] Scene root is not GhostUi (got {sceneInstance.GetType().Name})");
                sceneInstance.QueueFree();
                EmitStageComplete();
                return;
            }

            // Setup completion tracking
            _StageCompletion = new TaskCompletionSource<bool>();

            // Connect to GhostUi's SequenceComplete signal
            _GhostUi.SequenceComplete += OnSequenceComplete;

            // Add scene to tree (this triggers _Ready on GhostUi)
            AddChild(_GhostUi);
            GD.Print("[GhostCinematicDirector] Scene instantiated and added to tree");

            // Wait for sequence to complete
            var timeout = Task.Delay(60000); // 60 second timeout
            var completed = await Task.WhenAny(_StageCompletion.Task, timeout).ConfigureAwait(false);

            if (completed == timeout)
            {
                GD.PrintErr("[GhostCinematicDirector] Stage timed out after 60 seconds");
            }
            else
            {
                GD.Print("[GhostCinematicDirector] Stage completed successfully");
            }

            // Cleanup
            if (_GhostUi != null && IsInstanceValid(_GhostUi))
            {
                _GhostUi.SequenceComplete -= OnSequenceComplete;
            }

            EmitStageComplete();
        }
        catch (Exception ex)
        {
            GD.PrintErr($"[GhostCinematicDirector] Error: {ex.Message}");
            GD.PrintErr(ex.StackTrace);
            EmitStageComplete();
        }
    }

    private void OnSequenceComplete()
    {
        GD.Print("[GhostCinematicDirector] GhostUi sequence complete");
        _StageCompletion?.TrySetResult(true);
    }

    /// <summary>
    /// Data loader for ghost.yaml using CinematicDirector pattern.
    /// </summary>
    private sealed class GhostDataLoader : CinematicDirector<GhostTerminalCinematicPlan>
    {
        protected override string GetDataPath() => "res://source/stages/stage_1_ghost/ghost.yaml";

        protected override GhostTerminalCinematicPlan BuildPlan(NarrativeScript script)
        {
            return new GhostTerminalCinematicPlan(script);
        }
    }
}

/// <summary>
/// Cinematic plan loaded from ghost.yaml.
/// Wraps the NarrativeScript for stage-specific access patterns.
/// </summary>
public sealed record GhostTerminalCinematicPlan(NarrativeScript Script);
