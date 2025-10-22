// <copyright file="StageManifest.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Linq;

namespace OmegaSpiral.Source.Scripts.Infrastructure;

/// <summary>
/// Represents a single scene entry in a stage manifest.
/// </summary>
public class StageSceneEntry
{
    /// <summary>The unique identifier for this scene (e.g., "boot_sequence").</summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>User-friendly display name (e.g., "Boot Sequence").</summary>
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>Path to the scene file (e.g., "res://source/stages/ghost/scenes/boot_sequence.tscn").</summary>
    public string SceneFile { get; set; } = string.Empty;

    /// <summary>The ID of the scene that comes after this one (e.g., "opening_monologue").</summary>
    public string? NextSceneId { get; set; }

    /// <summary>Optional: Script class name for this scene's controller.</summary>
    public string? ScriptClass { get; set; }

    /// <summary>Optional: Description of what happens in this scene.</summary>
    public string? Description { get; set; }
}

/// <summary>
/// Represents the complete manifest for a stage.
/// Defines the sequence of scenes, their order, and transition rules.
/// Allows designers to modify stage structure without touching code.
/// </summary>
public class StageManifest
{
    /// <summary>The stage ID (1-5).</summary>
    public int StageId { get; set; }

    /// <summary>User-friendly stage name (e.g., "Ghost Terminal").</summary>
    public string StageName { get; set; } = string.Empty;

    /// <summary>Description of the stage's narrative and purpose.</summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>Path to the next stage's entry point (e.g., "res://source/stages/echo_hub/echo_hub_main.tscn").</summary>
    public string NextStagePath { get; set; } = string.Empty;

    /// <summary>All scenes in this stage, in no particular order (use GetFirstScene/GetNextScene to traverse).</summary>
    public List<StageSceneEntry> Scenes { get; set; } = new();

    /// <summary>
    /// Gets the first scene in the stage flow.
    /// </summary>
    /// <returns>The first scene entry, or null if no scenes exist.</returns>
    public StageSceneEntry? GetFirstScene()
    {
        // The first scene is one that no other scene points to
        var allNextIds = new HashSet<string?>(Scenes.Select(s => s.NextSceneId));
        return Scenes.FirstOrDefault(s => !allNextIds.Contains(s.Id));
    }

    /// <summary>
    /// Gets a scene by its ID.
    /// </summary>
    /// <param name="sceneId">The scene ID to retrieve.</param>
    /// <returns>The scene entry, or null if not found.</returns>
    public StageSceneEntry? GetScene(string sceneId)
    {
        return Scenes.FirstOrDefault(s => s.Id == sceneId);
    }

    /// <summary>
    /// Gets the next scene after the given scene ID.
    /// </summary>
    /// <param name="currentSceneId">The current scene's ID.</param>
    /// <returns>The next scene ID, or null if at the end.</returns>
    public string? GetNextSceneId(string currentSceneId)
    {
        var currentScene = GetScene(currentSceneId);
        return currentScene?.NextSceneId;
    }
}
