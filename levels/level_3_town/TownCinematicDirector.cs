// <copyright file="TownCinematicDirector.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

namespace OmegaSpiral.Source.Stages.Stage3;

using System.Collections.Generic;
using Godot;
using OmegaSpiral.Source.Backend;
using OmegaSpiral.Source.Backend.Narrative;

/// <summary>
/// Cinematic director for Town stage.
/// This is a HYBRID stage combining narrative sequences with interactive gameplay.
///
/// <example>
/// HYBRID STAGE PATTERN:
/// 1. Narrative Phase: Run narrative sequences from town_stage.json
/// 2. Gameplay Phase: Load town_main_start.tscn for player exploration/interaction
///
/// This pattern is the standardized approach for all hybrid stages (Town, PartySelection, Escape).
/// Use RunStageWithGameplayAsync(scenePath) to combine both phases.
/// </example>
///
/// Loads town_stage.json script and orchestrates scene playback, then transitions to gameplay scene.
/// </summary>
// public sealed class TownCinematicDirector : CinematicDirector
// {
//     /// <summary>
//     /// Initializes a new instance of the <see cref="TownCinematicDirector"/> class.
//     /// </summary>
//     // public TownCinematicDirector()
//     //     : base(new StageConfiguration
//     //     {
//     //         DataPath = "res://source//stages/stage_3_town/town_stage.json",
//     //         PlanFactory = script => new TownCinematicPlan(script)
//     //     })
//     // {
//     // }

// /// <inheritdoc/>
// public override Task<IReadOnlyList<SceneResult>> RunStageAsync()
// {
//     // HYBRID PATTERN: Run narrative first, then gameplay
//     return this.RunStageWithGameplayAsync("res://source//stages/stage_3_town/town_main_start.tscn");
// }

// /// <inheritdoc/>
// protected override StoryPlan BuildPlan(StoryBlock script)
// {
//     return new TownCinematicPlan(script);
// }

//     // TODO: Convert to Dialogic like Ghost stage
// }
