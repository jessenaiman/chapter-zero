// <copyright file="CinematicDirector.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using OmegaSpiral.Source.Backend.Narrative;

namespace OmegaSpiral.Source.Backend;

/// <summary>
/// Base class for stage cinematic directors that load JSON narrative scripts and cache their plans.
/// Handles thread-safe caching, loading, and plan building for all stages.
/// Each stage director inherits this and implements abstract methods for data path and plan building.
/// </summary>
/// <typeparam name="TPlan">The plan record type for this stage (e.g., GhostTerminalCinematicPlan, NethackCinematicPlan).</typeparam>
/// <summary>
/// Abstract base class for loading and caching stage-specific JSON cinematic plans.
/// Handles JSON loading with thread-safe lazy caching. Plans are data containers,
/// not controllers. Controllers (IStageBase implementations) use plans to orchestrate stage execution.
/// </summary>
/// <typeparam name="TPlan">The stage-specific plan type (e.g., GhostTerminalCinematicPlan).</typeparam>
public abstract class CinematicDirector<TPlan>
{
    private readonly object _SyncRoot = new();
    private TPlan? _CachedPlan;

    /// <summary>
    /// Gets the cached cinematic plan, loading and translating data if necessary.
    /// Thread-safe lazy loading with double-check locking pattern.
    /// </summary>
    /// <returns>A fully populated cinematic plan.</returns>
    public TPlan GetPlan()
    {
        if (_CachedPlan != null)
        {
            return _CachedPlan;
        }

        lock (_SyncRoot)
        {
            if (_CachedPlan == null)
            {
                NarrativeScriptRoot script = LoadJsonScript();
                _CachedPlan = BuildPlan(script);
            }
        }

        return _CachedPlan;
    }

    /// <summary>
    /// Forces the director to reload content from disk. Useful for tests when stage data is modified.
    /// </summary>
    public void Reset()
    {
        lock (_SyncRoot)
        {
            _CachedPlan = default;
        }
    }

    /// <summary>
    /// Gets the path to the JSON script file for this stage.
    /// Must be implemented by subclasses.
    /// </summary>
    /// <returns>Path to JSON file (e.g., "res://source/frontend/stages/stage_1_ghost/ghost.json").</returns>
    protected abstract string GetDataPath();

    /// <summary>
    /// Loads the JSON script from the path.
    /// </summary>
    /// <returns>Parsed NarrativeScriptRoot.</returns>
    private NarrativeScriptRoot LoadJsonScript()
    {
        string path = GetDataPath();
        return NarrativeScriptJsonLoader.LoadJsonScript(path);
    }

    /// <summary>
    /// Transforms a loaded NarrativeScriptRoot into the stage-specific plan.
    /// Must be implemented by subclasses.
    /// </summary>
    /// <param name="script">The loaded JSON narrative script.</param>
    /// <returns>Stage-specific plan object.</returns>
    protected abstract TPlan BuildPlan(NarrativeScriptRoot script);
}
