// <copyright file="StageController.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using Godot;
using System;
using System.Threading.Tasks;

namespace OmegaSpiral.Source.Scripts.Infrastructure;

/// <summary>
/// Abstract base controller for managing stage-level scene transitions and flow.
/// Provides a template pattern for stages to orchestrate their scenes without coupling individual scenes to scene paths.
/// Each stage (Stage 1-5) should inherit this and implement stage-specific initialization and flow.
///
/// Stages use a manifest (stage_manifest.json) to define scene order, transitions, and properties.
/// This allows designers to modify stage structure without touching code.
/// </summary>
[GlobalClass]
public abstract partial class StageController : Node
{
    /// <summary>
    /// The stage ID (1-5).
    /// </summary>
    protected abstract int StageId { get; }

    /// <summary>
    /// Path to the stage_manifest.json that defines this stage's structure.
    /// Example: "res://source/stages/ghost/data/stage_manifest.json"
    /// </summary>
    protected abstract string StageManifestPath { get; }

    /// <summary>
    /// Stage manifest loaded from JSON that defines scene order and transitions.
    /// </summary>
    protected StageManifest? StageManifest { get; private set; }

    /// <summary>
    /// Called when the stage initializes. Override to perform stage-specific setup.
    /// </summary>
    /// <returns>A task that completes when initialization is done.</returns>
    protected virtual async Task OnStageInitializeAsync()
    {
        await Task.CompletedTask;
    }

    /// <summary>
    /// Called when the stage is complete and ready to transition to the next stage.
    /// Override to perform cleanup or final logic.
    /// </summary>
    /// <returns>A task that completes when cleanup is done.</returns>
    protected virtual async Task OnStageCompleteAsync()
    {
        await Task.CompletedTask;
    }

    /// <summary>
    /// Transitions to the next scene in the stage manifest flow.
    /// </summary>
    /// <param name="currentSceneId">The ID of the current scene (e.g., "boot_sequence").</param>
    /// <returns>A task that completes when transition is done.</returns>
    protected async Task TransitionToNextSceneAsync(string currentSceneId)
    {
        if (StageManifest == null)
            throw new InvalidOperationException("StageManifest not initialized");

        string? nextSceneId = StageManifest.GetNextSceneId(currentSceneId);
        if (nextSceneId == null)
        {
            GD.PrintErr($"[StageController] No next scene found for '{currentSceneId}'");
            return;
        }

        await TransitionToSceneAsync(nextSceneId);
    }

    /// <summary>
    /// Transitions to a specific scene in the stage manifest by ID.
    /// </summary>
    /// <param name="sceneId">The scene ID to transition to (e.g., "opening_monologue").</param>
    /// <returns>A task that completes when transition is done.</returns>
    protected async Task TransitionToSceneAsync(string sceneId)
    {
        if (StageManifest == null)
            throw new InvalidOperationException("StageManifest not initialized");

        var sceneEntry = StageManifest.GetScene(sceneId);
        if (sceneEntry == null)
        {
            GD.PrintErr($"[StageController] Scene not found: {sceneId}");
            return;
        }

        // Load the scene file
        var nextScene = GD.Load<PackedScene>(sceneEntry.SceneFile);
        if (nextScene == null)
        {
            GD.PrintErr($"[StageController] Failed to load scene: {sceneEntry.SceneFile}");
            return;
        }

        GD.Print($"[StageController] Transitioning to {sceneId} ({sceneEntry.DisplayName})");

        // Use SceneManager to handle the transition
        var sceneManager = GetNode<SceneManager>("/root/SceneManager");
        sceneManager.TransitionToScene(sceneEntry.SceneFile);
    }

    /// <summary>
    /// Transitions out of the stage to the next stage.
    /// </summary>
    /// <param name="nextStagePath">Path to the next stage's scene (e.g., "res://source/stages/echo_hub/echo_hub_main.tscn").</param>
    /// <returns>A task that completes when transition is done.</returns>
    protected async Task TransitionToNextStageAsync(string nextStagePath)
    {
        await OnStageCompleteAsync();

        var nextScene = GD.Load<PackedScene>(nextStagePath);
        if (nextScene == null)
        {
            GD.PrintErr($"[StageController] Failed to load next stage: {nextStagePath}");
            return;
        }

        GD.Print($"[StageController] Transitioning to next stage: {nextStagePath}");

        var sceneManager = GetNode<SceneManager>("/root/SceneManager");
        sceneManager.TransitionToScene(nextStagePath);
    }

    /// <inheritdoc/>
    public override async void _Ready()
    {
        try
        {
            // Load stage manifest that defines scene order and properties
            var loader = new StageManifestLoader();
            StageManifest = loader.LoadManifest(StageManifestPath);
            if (StageManifest == null)
            {
                GD.PrintErr($"[StageController] Failed to load stage manifest from {StageManifestPath}");
                return;
            }

            GD.Print($"[StageController] Stage {StageId} initialized with {StageManifest.Scenes.Count} scenes");

            // Call stage-specific initialization
            await OnStageInitializeAsync();
        }
        catch (Exception ex)
        {
            GD.PrintErr($"[StageController] Error during stage initialization: {ex.Message}");
        }
    }
}
