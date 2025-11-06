#pragma warning disable SA1636 // File header copyright text should match
// <copyright file="GhostCinematicDirector.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

namespace OmegaSpiral.Source.Stages.Stage1;
#pragma warning restore SA1636 // File header copyright text should match

using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;
using Dictionary = Godot.Collections.Dictionary;

/// <summary>
/// Cinematic director for Ghost Terminal stage.
/// Uses Dialogic timeline system for narrative display with 3-thread architecture.
///
/// <para>
/// Architecture:
/// - Dialogic timeline (ghost_terminal.dtl) contains all 3 Dreamweaver threads
/// - Uses Dialogic's conditional system to show only the chosen thread's content
/// - GhostDialogicBridge (GDScript) handles Dialogic signals and choice capture
/// - C# director orchestrates the flow and returns results for scoring
/// </para>
///
/// <para>
/// The 3-thread design allows AI to generate all parallel storylines in one pass,
/// while Dialogic filters at runtime based on player's thread selection.
/// </para>
/// </summary>
public sealed partial class GhostCinematicDirector : Node
{
    private Node? _DialogicBridge;
    private TaskCompletionSource<Dictionary>? _TimelineCompletionSource;

    /// <summary>
    /// Runs the Ghost Terminal stage using Dialogic.
    /// </summary>
    /// <returns>Player choices and thread selection for Dreamweaver scoring.</returns>
    public async Task<Dictionary> RunStageAsync()
    {
        GD.Print("[GhostCinematicDirector] Starting Ghost Terminal stage");

                // Load the Dialogic bridge script
        var bridgeScript = GD.Load<GDScript>("res://levels/level_1_ghost/ghost_dialogic_bridge.gd");
        _DialogicBridge = (Node)bridgeScript.New();

        if (_DialogicBridge == null)
        {
            GD.PrintErr("[GhostCinematicDirector] Failed to create Dialogic bridge");
            return new Dictionary();
        }

        // Add bridge to scene tree so it can receive Dialogic signals
        var tree = Engine.GetMainLoop() as SceneTree;
        tree?.Root.AddChild(_DialogicBridge);

        // Setup completion tracking
        _TimelineCompletionSource = new TaskCompletionSource<Dictionary>();
        _DialogicBridge.Connect("timeline_completed", Callable.From<Dictionary>(OnTimelineCompleted));

        // Start the Dialogic timeline
        _DialogicBridge.Call("start_ghost_timeline");

        // Wait for timeline to complete
        var results = await _TimelineCompletionSource.Task;

        GD.Print($"[GhostCinematicDirector] Stage complete. Selected thread: {results.GetValueOrDefault("thread", "")}");

        // Cleanup
        _DialogicBridge?.QueueFree();

        return results;
    }

    private void OnTimelineCompleted(Dictionary results)
    {
        GD.Print("[GhostCinematicDirector] Timeline completed callback");
        _TimelineCompletionSource?.SetResult(results);
    }
}
