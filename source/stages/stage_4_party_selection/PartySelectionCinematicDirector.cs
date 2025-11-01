// <copyright file="PartySelectionCinematicDirector.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

namespace OmegaSpiral.Source.Stages.Stage4;

using System.Collections.Generic;
using Godot;
using OmegaSpiral.Source.Backend;
using OmegaSpiral.Source.Backend.Narrative;

/// <summary>
/// Cinematic director for Party Selection (Echo Vault) stage.
/// This is a HYBRID stage combining narrative sequences with character selection UI.
///
/// <example>
/// HYBRID STAGE PATTERN:
/// 1. Narrative Phase: Run narrative sequences from stage4.json
/// 2. Gameplay Phase: Load character selection UI scene for player to create party
///
/// This pattern is the standardized approach for all hybrid stages (Town, PartySelection, Escape).
/// Use RunStageWithGameplayAsync(scenePath) to combine both phases.
/// </example>
///
/// Loads stage4.json script and orchestrates scene playback, then transitions to party selection UI.
/// </summary>
public sealed class PartySelectionCinematicDirector : CinematicDirector<PartySelectionCinematicPlan>
{
    /// <inheritdoc/>
    public override Task<IReadOnlyList<SceneResult>> RunStageAsync()
    {
        // HYBRID PATTERN: Run narrative first, then party selection UI
        // TODO: Implement party selection scene path
        return this.RunStageWithGameplayAsync("res://source//stages/stage_4_party_selection/party_selection_ui.tscn");
    }

    /// <inheritdoc/>
    protected override string GetDataPath()
    {
        return "res://source//stages/stage_4_party_selection/stage4.json";
    }

    /// <inheritdoc/>
    protected override PartySelectionCinematicPlan BuildPlan(StoryScriptRoot script)
    {
        return new PartySelectionCinematicPlan(script);
    }

    /// <inheritdoc/>
    protected override SceneManager CreateSceneManager(StoryScriptElement scene, object data)
    {
        GD.Print($"[PartySelection] Creating scene manager for: {scene.Id}");
        return new SceneManager(scene, data);
    }
}
