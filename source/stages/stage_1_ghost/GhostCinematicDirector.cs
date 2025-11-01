#pragma warning disable SA1636 // File header copyright text should match
// <copyright file="GhostCinematicDirector.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

namespace OmegaSpiral.Source.Stages.Stage1;
#pragma warning restore SA1636 // File header copyright text should match

using Godot;
using OmegaSpiral.Source.Backend;
using OmegaSpiral.Source.Backend.Narrative;

/// <summary>
/// Cinematic director for Ghost Terminal stage.
/// This is a NARRATIVE WITH UI stage that loads a scene before running narrative sequences.
///
/// <example>
/// NARRATIVE WITH UI PATTERN:
/// Overrides GetScenePath() to load ghost_terminal.tscn before narrative.
/// Uses base RunStageAsync() implementation which:
/// 1. Loads the UI scene
/// 2. Loads narrative sequences from ghost.json
/// 3. Iterates through scenes and displays them sequentially
///
/// This pattern is for narrative stages that need a visual UI scene (Ghost).
/// Pure narrative stages don't override GetScenePath().
/// Hybrid stages override RunStageAsync() for complex scene management.
/// </example>
///
/// Loads ghost.json script and orchestrates scene playback.
/// </summary>
public sealed class GhostCinematicDirector : CinematicDirector
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GhostCinematicDirector"/> class.
    /// </summary>
    public GhostCinematicDirector()
        : base(new StageConfiguration
        {
            DataPath = "res://source//stages/stage_1_ghost/ghost.json",
            ScenePath = "res://source//stages/stage_1_ghost/ghost_terminal.tscn",
            PlanFactory = script => new GhostCinematicPlan(script)
        })
    {
    }
    /// <inheritdoc/>
    protected override StoryPlan BuildPlan(StoryBlock script)
    {
        return new GhostCinematicPlan(script);
    }

    /// <inheritdoc/>
    protected override OmegaSceneManager CreateSceneManager(Scene scene, object data)
    {
        GD.Print($"[GhostCinematicDirector] Creating GhostSceneManager for: {scene.Id}");
        return new GhostSceneManager(scene, data);
    }
}
