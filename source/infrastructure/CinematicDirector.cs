// <copyright file="CinematicDirector.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using System;
using OmegaSpiral.Source.Narrative;

namespace OmegaSpiral.Source.Scripts.Infrastructure;

/// <summary>
/// Base class for stage cinematic directors that load YAML narrative scripts and cache their plans.
/// Handles thread-safe caching, loading, and plan building for all stages.
/// Each stage director inherits this and implements abstract methods for data path and plan building.
/// </summary>
/// <typeparam name="TPlan">The plan record type for this stage (e.g., GhostTerminalCinematicPlan, NethackCinematicPlan).</typeparam>
public abstract class CinematicDirector<TPlan>
    where TPlan : class
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
                NarrativeScript script = LoadYamlScript();
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
            _CachedPlan = null;
        }
    }

    /// <summary>
    /// Gets the path to the YAML script file for this stage.
    /// Must be implemented by subclasses.
    /// </summary>
    /// <returns>Path to YAML file (e.g., "res://source/stages/stage_1_ghost/ghost.yaml").</returns>
    protected abstract string GetDataPath();

    /// <summary>
    /// Loads the YAML script from the path.
    /// </summary>
    /// <returns>Parsed NarrativeScript.</returns>
    private NarrativeScript LoadYamlScript()
    {
        string path = GetDataPath();
        return NarrativeScriptLoader.LoadYamlScript(path);
    }

    /// <summary>
    /// Transforms a loaded NarrativeScript into the stage-specific plan.
    /// Must be implemented by subclasses.
    /// </summary>
    /// <param name="script">The loaded YAML narrative script.</param>
    /// <returns>Stage-specific plan object.</returns>
    protected abstract TPlan BuildPlan(NarrativeScript script);
}


