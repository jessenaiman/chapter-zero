// <copyright file="StageManagerBase.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using System;
using Godot;
using OmegaSpiral.Source.Scripts.Infrastructure;

namespace OmegaSpiral.Source.Scripts.Infrastructure.StageManagement;

/// <summary>
/// Base implementation for stage managers that centralizes entry-scene resolution and validation.
/// </summary>
public abstract class StageManagerBase : IStageManager
{
    private readonly string _entryScenePath;

    protected StageManagerBase(string entryScenePath)
    {
        if (string.IsNullOrWhiteSpace(entryScenePath))
        {
            throw new ArgumentException("Entry scene path must be provided.", nameof(entryScenePath));
        }

        _entryScenePath = entryScenePath;
    }

    /// <inheritdoc/>
    public abstract int StageId { get; }

    /// <summary>
    /// Gets the primary entry scene path configured for the stage.
    /// </summary>
    protected virtual string EntryScenePath => _entryScenePath;

    /// <summary>
    /// Gets the optional path to a stage manifest that can provide a more granular first scene.
    /// </summary>
    protected virtual string? StageManifestPath => null;

    /// <inheritdoc/>
    public virtual string ResolveEntryScenePath()
    {
        string? manifestScene = ResolveManifestFirstScene();
        return string.IsNullOrEmpty(manifestScene) ? EntryScenePath : manifestScene;
    }

    /// <inheritdoc/>
    public virtual void TransitionToStage(SceneManager sceneManager)
    {
        if (sceneManager == null)
        {
            throw new ArgumentNullException(nameof(sceneManager));
        }

        string scenePath = ResolveEntryScenePath();

        if (!ResourceLoader.Exists(scenePath))
        {
            GD.PrintErr($"[StageManagerBase] Entry scene not found for Stage {StageId}: {scenePath}");
            return;
        }

        if (!sceneManager.ValidateStateForTransition(scenePath))
        {
            GD.PrintErr($"[StageManagerBase] Transition validation failed for Stage {StageId} -> {scenePath}");
            return;
        }

        sceneManager.TransitionToScene(scenePath);
    }

    /// <summary>
    /// Attempts to resolve the first scene from the stage manifest, if available.
    /// </summary>
    /// <returns>The first scene path or <c>null</c> if resolution fails.</returns>
    protected string? ResolveManifestFirstScene()
    {
        if (string.IsNullOrEmpty(StageManifestPath))
        {
            return null;
        }

        var loader = new StageManifestLoader();
        StageManifest? manifest = loader.LoadManifest(StageManifestPath);

        return manifest?.GetFirstScene()?.SceneFile;
    }
}
