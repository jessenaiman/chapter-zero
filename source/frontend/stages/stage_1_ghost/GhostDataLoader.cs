// <copyright file="GhostDataLoader.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using OmegaSpiral.Source.Backend;
using OmegaSpiral.Source.Backend.Narrative;

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
    protected override GhostTerminalCinematicPlan BuildPlan(NarrativeScriptRoot script)
    {
        return new GhostTerminalCinematicPlan(script);
    }
}

/// <summary>
/// Cinematic plan loaded from ghost.yaml.
/// Wraps the NarrativeScriptRoot for stage-specific access patterns.
/// Implements IStageBase to allow use as a stage directly.
/// </summary>
public sealed record GhostTerminalCinematicPlan(NarrativeScriptRoot Script) : IStageBase
{
    /// <inheritdoc/>
    public int StageId => 1;

    /// <inheritdoc/>
    public event StageCompleteDelegate? StageComplete;

    /// <inheritdoc/>
    public Task ExecuteStageAsync()
    {
        // This is a data-only plan. The actual stage controller will handle execution.
        // For now, complete immediately.
        StageComplete?.Invoke();
        return Task.CompletedTask;
    }
}
