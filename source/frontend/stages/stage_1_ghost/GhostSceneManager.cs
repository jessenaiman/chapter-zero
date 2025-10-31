// <copyright file="GhostSceneManager.cs" company="Omega Spiral">
// Copyright (c) Omega Spiral. All rights reserved.
// </copyright>

namespace OmegaSpiral.Source.Stages.Stage1;

using Godot;
using OmegaSpiral.Source.Backend;
using OmegaSpiral.Source.Backend.Narrative;

/// <summary>
/// Executes a single Ghost Terminal scene.
/// Stage 1-specific scene manager that delegates to GhostUi for narrative display.
/// Inherits core RunSceneAsync() from base SceneManager which handles IStoryHandler delegation.
/// </summary>
public sealed partial class GhostSceneManager : SceneManager
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GhostSceneManager"/> class.
    /// </summary>
    /// <param name="scene">The scene element to play.</param>
    /// <param name="data">Additional data for the scene.</param>
    public GhostSceneManager(StoryScriptElement scene, object data)
        : base(scene, data)
    {
    }


}
