// <copyright file="StageController.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OmegaSpiral.Source.Scripts.Common;

namespace OmegaSpiral.Source.Scripts.Infrastructure;

/// <summary>
/// Abstract base orchestrator for managing stage-level scene transitions and flow.
/// Orchestrates scene execution, affinity tracking, and score reporting to GameState.
/// Each stage (Stage 1-5) should inherit this and implement stage-specific scene execution logic.
///
/// Stages use a manifest (stage_manifest.json) to define scene order, transitions, and properties.
/// Individual scenes are presentation-only nodes instantiated and managed by this orchestrator.
/// </summary>
[GlobalClass]
public abstract partial class StageController : Node
{
    private const string StageControllerGroupName = "__stage_controller";

    private readonly Dictionary<string, int> affinityScores = new(StringComparer.OrdinalIgnoreCase);

    private string? _currentSceneId;
    private int _currentSceneIndex = 0;

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
    /// Reports affinity scores to GameState if stage tracks affinity.
    /// </summary>
    /// <returns>A task that completes when cleanup is done.</returns>
    protected virtual async Task OnStageCompleteAsync()
    {
        // Report affinity scores to GameState if this stage tracks them
        if (affinityScores.Count > 0)
        {
            var gameState = GetNode<GameState>("/root/GameState");
            foreach (var (dreamweaverKey, points) in affinityScores)
            {
                // Convert string keys (light, shadow, ambition) to DreamweaverType enum
                if (Enum.TryParse<DreamweaverType>(dreamweaverKey, true, out var dwType))
                {
                    gameState.UpdateDreamweaverScore(dwType, points);
                    GD.Print($"[StageController] Reported {points} points to {dwType}");
                }
            }
        }

        await Task.CompletedTask;
    }

    /// <summary>
    /// Registers affinity points for a Dreamweaver/alignment within this stage.
    /// </summary>
    /// <param name="alignmentId">Identifier for the alignment/dreamweaver.</param>
    /// <param name="points">Number of points to award (can be negative).</param>
    internal void AwardAffinityScore(string alignmentId, int points)
    {
        if (string.IsNullOrWhiteSpace(alignmentId) || points == 0)
        {
            return;
        }

        if (!this.affinityScores.ContainsKey(alignmentId))
        {
            this.affinityScores[alignmentId] = 0;
        }

        this.affinityScores[alignmentId] += points;
    }

    /// <summary>
    /// Clears recorded affinity scores. Called automatically when the stage is initialized.
    /// </summary>
    protected void ResetAffinityScores()
    {
        this.affinityScores.Clear();
    }

    /// <summary>
    /// Gets a read-only view of the current affinity scores.
    /// </summary>
    /// <returns>Read-only dictionary of scores, or null if none recorded.</returns>
    protected virtual IReadOnlyDictionary<string, int>? GetAffinityScores()
    {
        return this.affinityScores.Count > 0 ? this.affinityScores : null;
    }

    /// <summary>
    /// Determines the leading alignment based on accumulated affinity points.
    /// </summary>
    /// <returns>The alignment identifier with the highest score, or null if none recorded.</returns>
    protected string? DetermineAffinityLeader()
    {
        if (this.affinityScores.Count == 0)
        {
            return null;
        }

        return this.affinityScores
            .OrderByDescending(kvp => kvp.Value)
            .ThenBy(kvp => kvp.Key, StringComparer.OrdinalIgnoreCase)
            .First()
            .Key;
    }

    /// <summary>
    /// Executes a single scene based on the manifest schema.
    /// Override this in subclasses to implement stage-specific scene execution logic.
    /// </summary>
    /// <param name="sceneEntry">The scene entry from the manifest.</param>
    /// <returns>A task that completes when the scene finishes.</returns>
    protected virtual async Task ExecuteSceneAsync(StageSceneEntry sceneEntry)
    {
        GD.Print($"[StageController] Executing scene: {sceneEntry.Id} ({sceneEntry.DisplayName})");

        // Default implementation: Load the scene file and instantiate it
        var packedScene = GD.Load<PackedScene>(sceneEntry.SceneFile);
        if (packedScene == null)
        {
            GD.PrintErr($"[StageController] Failed to load scene: {sceneEntry.SceneFile}");
            return;
        }

        var sceneInstance = packedScene.Instantiate();
        AddChild(sceneInstance);

        // Wait for scene to signal completion (subclasses should handle this)
        // For now, this is a placeholder that completes immediately
        await Task.CompletedTask;
    }

    /// <summary>
    /// Advances to the next scene in the stage sequence.
    /// </summary>
    protected async Task AdvanceToNextSceneAsync()
    {
        if (StageManifest == null)
        {
            GD.PrintErr("[StageController] Cannot advance: StageManifest not loaded");
            return;
        }

        if (_currentSceneId == null)
        {
            // Start with first scene
            _currentSceneIndex = 0;
            var firstScene = StageManifest.GetFirstScene();
            if (firstScene != null)
            {
                _currentSceneId = firstScene.Id;
                await ExecuteSceneAsync(firstScene);
            }
            return;
        }

        // Get next scene
        var nextSceneId = StageManifest.GetNextSceneId(_currentSceneId);
        if (string.IsNullOrEmpty(nextSceneId))
        {
            // Stage complete
            GD.Print($"[StageController] Stage {StageId} complete");
            await OnStageCompleteAsync();

            // Transition to next stage
            if (!string.IsNullOrEmpty(StageManifest.NextStagePath))
            {
                await TransitionToNextStageAsync(StageManifest.NextStagePath);
            }
            return;
        }

        var nextScene = StageManifest.GetScene(nextSceneId);
        if (nextScene != null)
        {
            _currentSceneId = nextSceneId;
            _currentSceneIndex++;
            await ExecuteSceneAsync(nextScene);
        }
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

    /// <summary>
    /// Transition to a specific scene id within this stage manifest.
    /// </summary>
    /// <param name="sceneId">Scene identifier from the manifest.</param>
    protected async Task TransitionToSceneAsync(string sceneId)
    {
        if (StageManifest == null)
        {
            GD.PrintErr("[StageController] Cannot transition: StageManifest not loaded");
            return;
        }

        var scene = StageManifest.GetScene(sceneId);
        if (scene == null)
        {
            GD.PrintErr($"[StageController] Scene id not found in manifest: {sceneId}");
            return;
        }

        await ExecuteSceneAsync(scene);
    }

    /// <summary>
    /// Transition to the scene that follows the provided scene id.
    /// Provided for compatibility with existing controllers that call TransitionToNextSceneAsync.
    /// </summary>
    protected async Task TransitionToNextSceneAsync(string currentSceneId)
    {
        if (StageManifest == null)
        {
            GD.PrintErr("[StageController] Cannot transition: StageManifest not loaded");
            return;
        }

        var nextId = StageManifest.GetNextSceneId(currentSceneId);
        if (string.IsNullOrEmpty(nextId))
        {
            GD.Print($"[StageController] No next scene after {currentSceneId}");
            await OnStageCompleteAsync();
            return;
        }

        await TransitionToSceneAsync(nextId);
    }

    /// <inheritdoc/>
    public override async void _Ready()
    {
        base._Ready();

        AddToGroup(StageControllerGroupName);

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

            ResetAffinityScores();

            // Call stage-specific initialization
            await OnStageInitializeAsync();
        }
        catch (Exception ex)
        {
            GD.PrintErr($"[StageController] Error during stage initialization: {ex.Message}");
        }
    }

    /// <inheritdoc/>
    public override void _ExitTree()
    {
        RemoveFromGroup(StageControllerGroupName);
        base._ExitTree();
    }

    /// <summary>
    /// Retrieves the active stage controller from the scene tree, if any.
    /// </summary>
    /// <param name="tree">The current scene tree.</param>
    /// <returns>The active stage controller, or null if none registered.</returns>
    internal static StageController? GetActiveController(SceneTree tree)
    {
        foreach (var node in tree.GetNodesInGroup(StageControllerGroupName))
        {
            if (node is StageController controller)
            {
                return controller;
            }
        }

        return null;
    }
}
