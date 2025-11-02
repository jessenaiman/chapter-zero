// <copyright file="EscapeCinematicController.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

namespace OmegaSpiral.Source.Stages.Stage5;

using System.Collections.Generic;
using Godot;
using OmegaSpiral.Source.Backend;
using OmegaSpiral.Source.Backend.Narrative;

/// <summary>
/// Cinematic director for Escape stage.
/// This is a HYBRID stage combining narrative sequences with final escape gameplay.
///
/// <example>
/// HYBRID STAGE PATTERN:
/// 1. Narrative Phase: Run narrative sequences from stage_5.json
/// 2. Gameplay Phase: Load escape_hub_start.tscn for endgame sequences
///
/// This pattern is the standardized approach for all hybrid stages (Town, PartySelection, Escape).
/// Use RunStageWithGameplayAsync(scenePath) to combine both phases.
/// </example>
///
/// Loads stage_5.json script and orchestrates scene playback, then transitions to escape gameplay.
/// </summary>
public sealed class EscapeCinematicDirector : CinematicDirector
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EscapeCinematicDirector"/> class.
    /// </summary>
    public EscapeCinematicDirector()
        : base(new StageConfiguration
        {
            DataPath = "res://source//stages/stage_5_escape/stage_5.json",
            PlanFactory = script => new EscapeCinematicPlan(script)
        })
    {
    }

    /// <inheritdoc/>
    public override Task<IReadOnlyList<SceneResult>> RunStageAsync()
    {
        // HYBRID PATTERN: Run narrative first, then escape sequence
        return this.RunStageWithGameplayAsync("res://source//stages/stage_5_escape/escape_hub_start.tscn");
    }

    /// <inheritdoc/>
    protected override StoryPlan BuildPlan(StoryBlock script)
    {
        return new EscapeCinematicPlan(script);
    }

    // TODO: Convert to Dialogic like Ghost stage
}
