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
/// This is a HYBRID NARRATIVE+COMBAT stage using narrative sequences with combat encounters.
///
/// <example>
/// HYBRID NARRATIVE+COMBAT PATTERN:
/// Uses base RunStageAsync() implementation which:
/// 1. Loads narrative sequences from nethack.json
/// 2. Iterates through scenes and displays them sequentially
/// 3. Detects combat scenes (type: "combat") and handles them with flat-file encounters
/// 4. No additional gameplay scenes loaded beyond combat encounters
///
/// This pattern supports the throwback approach where combat encounters
/// are defined in simple flat JSON files with all details in one place.
/// </example>
///
/// Loads nethack.json script and orchestrates scene/combat playback.
/// </summary>
public sealed class NethackCinematicDirector : CinematicDirector
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NethackCinematicDirector"/> class.
    /// </summary>
    public NethackCinematicDirector()
        : base(new StageConfiguration
        {
            DataPath = "res://source//stages/stage_2_nethack/nethack.json",
            PlanFactory = script => new NethackCinematicPlan(script)
        })
    {
    }

    /// <inheritdoc/>
    protected override StoryPlan BuildPlan(StoryBlock script)
    {
        return new NethackCinematicPlan(script);
    }

    /// <inheritdoc/>
    protected override OmegaSceneManager CreateSceneManager(Scene scene, object data)
    {
        GD.Print($"[Nethack] Creating scene manager for: {scene.Id} (type: {scene.Type})");

        // For combat scenes, we use the basic OmegaSceneManager but could extend this
        // to use a specialized CombatSceneManager in the future if needed
        // The combat logic is handled through the CombatData in the scene
        return new OmegaSceneManager();
    }
}
