#pragma warning disable SA1636 // File header copyright text should match
// <copyright file="NethackCinematicDirector.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

namespace OmegaSpiral.Source.Stages.Stage2;
#pragma warning restore SA1636 // File header copyright text should match

using System.Threading.Tasks;
using Godot;
using GodotDictionary = Godot.Collections.Dictionary;
using OmegaSpiral.Source.Backend;
using OmegaSpiral.Source.Backend.Narrative;

/// <summary>
/// Cinematic director for Nethack Echo Chamber stage.
/// Uses Dialogic timeline system with 3 Dreamweaver narrators.
///
/// <para>
/// Architecture:
/// - Three Dreamweaver characters (Light, Shadow, Ambition) act as DM narrators
/// - Player explores 3 chambers, each with Door/Monster/Chest objects
/// - Each object is secretly aligned to one Dreamweaver philosophy
/// - Scoring tracks which Dreamweaver's philosophy player resonates with
/// - Winning Dreamweaver becomes player's guide for future stages
/// </para>
///
/// <para>
/// Unlike Stage 1 (single Omega voice), Stage 2 has three distinct narrator voices
/// that compete to "claim" the player based on philosophical alignment.
/// </para>
/// </summary>
public sealed partial class NethackCinematicDirector : Node
{
    private Node? _DialogicBridge;
    private TaskCompletionSource<Godot.Collections.Dictionary>? _TimelineCompletionSource;

    /// <summary>
    /// Runs the Nethack Echo Chamber stage using Dialogic.
    /// </summary>
    /// <returns>Player choices and chosen Dreamweaver for future stages.</returns>
    public async Task<Godot.Collections.Dictionary> RunStageAsync()
    {
        GD.Print("[NethackCinematicDirector] Starting Nethack Echo Chamber stage");

        // Load the Dialogic bridge script
        var bridgeScript = GD.Load<GDScript>("res://source/scenes/game_scene/levels/level_2_nethack/nethack_dialogic_bridge.gd");
        _DialogicBridge = (Node) bridgeScript.New();

        if (_DialogicBridge == null)
        {
            GD.PrintErr("[NethackCinematicDirector] Failed to create Dialogic bridge");
            return new Godot.Collections.Dictionary();
        }

        // Add bridge to scene tree so it can receive Dialogic signals
        var tree = Engine.GetMainLoop() as SceneTree;
        tree?.Root.AddChild(_DialogicBridge);

        // Setup completion tracking
        _TimelineCompletionSource = new TaskCompletionSource<Godot.Collections.Dictionary>();
        _DialogicBridge.Connect("timeline_completed", Callable.From<Godot.Collections.Dictionary>(OnTimelineCompleted));

        // Start the Dialogic timeline
        _DialogicBridge.Call("start_nethack_timeline");

        // Wait for timeline to complete
        var results = await _TimelineCompletionSource.Task;

        GD.Print($"[NethackCinematicDirector] Stage complete. Chosen Dreamweaver: {results.GetValueOrDefault("chosen_dreamweaver", "")}");

        // Cleanup
        _DialogicBridge?.QueueFree();

        return results;
    }

    private void OnTimelineCompleted(Godot.Collections.Dictionary results)
    {
        GD.Print("[NethackCinematicDirector] Timeline completed callback");
        _TimelineCompletionSource?.SetResult(results);
    }
}
