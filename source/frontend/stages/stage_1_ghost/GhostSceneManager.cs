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

    /// <summary>
    /// Gets or creates the GhostUi handler for this stage.
    /// Searches the scene tree for GhostUi which implements IStoryHandler.
    /// Falls back to NarrativeUi if GhostUi is not available.
    /// </summary>
    /// <param name="tree">The active scene tree.</param>
    /// <returns>The GhostUi handler, or a fallback NarrativeUi if not found.</returns>
    protected override IStoryHandler? GetOrCreateUiHandler(SceneTree tree)
    {
        var root = tree.Root;
        if (root == null)
        {
            GD.PrintErr("[GhostSceneManager] Could not find scene root");
            return null;
        }

        // First, try to find GhostUi (Stage 1 specific)
        var ghostUi = root.FindChild("GhostUi", owned: true) as IStoryHandler;
        if (ghostUi != null)
        {
            GD.Print("[GhostSceneManager] Found GhostUi handler");
            return ghostUi;
        }

        // Fallback to generic NarrativeUi
        var narrativeUi = root.FindChild("NarrativeUi", owned: true) as IStoryHandler;
        if (narrativeUi != null)
        {
            GD.Print("[GhostSceneManager] Found fallback NarrativeUi handler");
            return narrativeUi;
        }

        GD.PrintErr("[GhostSceneManager] No UI handler found in scene tree");
        return null;
    }
}
