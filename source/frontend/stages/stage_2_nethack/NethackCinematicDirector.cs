#pragma warning disable SA1636 // File header copyright text should match
// <copyright file="NethackCinematicDirector.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

namespace OmegaSpiral.Source.Stages.Stage2;
#pragma warning restore SA1636 // File header copyright text should match

using Godot;
using OmegaSpiral.Source.Backend;
using OmegaSpiral.Source.Backend.Narrative;

/// <summary>
/// Cinematic director for Nethack stage.
/// This is a PURE NARRATIVE stage using only narrative sequences.
///
/// <example>
/// PURE NARRATIVE PATTERN:
/// Uses base RunStageAsync() implementation which:
/// 1. Loads narrative sequences from nethack.json
/// 2. Iterates through scenes and displays them sequentially
/// 3. No additional gameplay scenes loaded
///
/// This pattern is the standardized approach for pure narrative stages (Ghost, Nethack).
/// Override RunStageAsync() only if adding gameplay/exploration phases (hybrid stages).
/// </example>
///
/// Loads nethack.json script and orchestrates scene playback.
/// </summary>
public sealed class NethackCinematicDirector : CinematicDirector<NethackCinematicPlan>
{
    /// <inheritdoc/>
    protected override string GetDataPath()
    {
        return "res://source/frontend/stages/stage_2_nethack/nethack.json";
    }

    /// <inheritdoc/>
    protected override NethackCinematicPlan BuildPlan(StoryScriptRoot script)
    {
        return new NethackCinematicPlan(script);
    }

    /// <inheritdoc/>
    protected override SceneManager CreateSceneManager(StoryScriptElement scene, object data)
    {
        GD.Print($"[Nethack] Creating scene manager for: {scene.Id}");
        return new SceneManager(scene, data);
    }
}
