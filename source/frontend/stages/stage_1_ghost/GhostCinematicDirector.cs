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
/// This is a PURE NARRATIVE stage using only narrative sequences.
///
/// <example>
/// PURE NARRATIVE PATTERN:
/// Uses base RunStageAsync() implementation which:
/// 1. Loads narrative sequences from ghost.json
/// 2. Iterates through scenes and displays them sequentially
/// 3. No additional gameplay scenes loaded
///
/// This pattern is the standardized approach for pure narrative stages (Ghost, Nethack).
/// Override RunStageAsync() only if adding gameplay/exploration phases (hybrid stages).
/// </example>
///
/// Loads ghost.json script and orchestrates scene playback.
/// </summary>
public sealed class GhostCinematicDirector : CinematicDirector<GhostCinematicPlan>
{
    /// <inheritdoc/>
    protected override string GetDataPath()
    {
        return "res://source/frontend/stages/stage_1_ghost/ghost.json";
    }

    /// <inheritdoc/>
    protected override GhostCinematicPlan BuildPlan(StoryScriptRoot script)
    {
        return new GhostCinematicPlan(script);
    }

    /// <inheritdoc/>
    protected override SceneManager CreateSceneManager(StoryScriptElement scene, object data)
    {
        // For now, return a simple scene manager that logs the scene
        GD.Print($"[Ghost] Creating scene manager for: {scene.Id}");
        var manager = new SceneManager(scene, data);
        return manager;
    }
}
