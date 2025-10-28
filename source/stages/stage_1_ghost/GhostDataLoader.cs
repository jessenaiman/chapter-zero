// <copyright file="GhostDataLoader.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using OmegaSpiral.Source.Narrative;
using OmegaSpiral.Source.Scripts.Infrastructure;

namespace OmegaSpiral.Source.Stages.Stage1;

/// <summary>
/// Data loader for ghost.yaml using CinematicDirector pattern.
/// Used by GhostUi to load the narrative script.
/// </summary>
public sealed class GhostDataLoader : CinematicDirector<GhostTerminalCinematicPlan>
{
    /// <inheritdoc/>
    protected override string GetDataPath() => "res://source/stages/stage_1_ghost/ghost.yaml";

    /// <inheritdoc/>
    protected override GhostTerminalCinematicPlan BuildPlan(NarrativeScript script)
    {
        return new GhostTerminalCinematicPlan(script);
    }
}

/// <summary>
/// Cinematic plan loaded from ghost.yaml.
/// Wraps the NarrativeScript for stage-specific access patterns.
/// </summary>
public sealed record GhostTerminalCinematicPlan(NarrativeScript Script);
