// <copyright file="IStageManager.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

namespace OmegaSpiral.Source.Scripts.Infrastructure.StageManagement;

/// <summary>
/// Provides the contract for determining how to enter a stage from the main menu or scene manager.
/// </summary>
public interface IStageManager
{
    /// <summary>
    /// Gets the numeric stage identifier (1-based).
    /// </summary>
    int StageId { get; }

    /// <summary>
    /// Resolves the scene path that should be loaded to begin this stage.
    /// </summary>
    /// <returns>The Godot resource path for the stage's entry scene.</returns>
    string ResolveEntryScenePath();

    /// <summary>
    /// Performs the transition into this stage using the provided scene manager.
    /// </summary>
    /// <param name="sceneManager">The scene manager responsible for scene transitions.</param>
    void TransitionToStage(SceneManager sceneManager);
}
