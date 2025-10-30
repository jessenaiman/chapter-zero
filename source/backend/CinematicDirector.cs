// <copyright file="CinematicDirector.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

namespace OmegaSpiral.Source.Backend;

using Godot;
using OmegaSpiral.Source.Backend.Narrative;

/// <summary>
/// Non-generic interface for running stages.
/// Allows GameManager to work with any stage director without caring about the specific plan type.
/// </summary>
public interface ICinematicDirector
{
    /// <summary>
    /// Runs the stage.
    /// </summary>
    /// <returns>A task that completes when the stage is finished.</returns>
    Task RunStageAsync();
}

/// <summary>
/// Base class for stage story directors that load JSON story scripts and cache their plans.
/// Handles thread-safe caching, loading, and plan building for all stages.
/// Each stage director inherits this and implements abstract methods for data path and plan building.
/// </summary>
/// <typeparam name="TPlan">The plan record type for this stage (e.g., GhostTerminalStoryPlan, NethackStoryPlan).</typeparam>
public abstract class CinematicDirector<TPlan> : ICinematicDirector
    where TPlan : StoryPlan
{
    /// <summary>
    /// Gets the cached plan for this stage.
    /// </summary>
    protected TPlan? Plan { get; private set; }

    /// <summary>
    /// Runs the stage by iterating through scenes, creating SceneManager for each, awaiting completion.
    ///
    /// PURE NARRATIVE PATTERN (Default):
    /// Uses base implementation for stages with narrative sequences only (Ghost, Nethack).
    ///
    /// HYBRID STAGE PATTERN (Override):
    /// Subclasses can override this method and call RunStageWithGameplayAsync(scenePath)
    /// to combine narrative sequences with interactive gameplay scenes.
    /// This is the standardized approach for hybrid stages (Town, PartySelection, Escape).
    /// </summary>
    /// <returns>A task that completes when the stage is finished.</returns>
    public virtual async Task RunStageAsync()
    {
        var script = StoryLoader.LoadJsonScript(this.GetDataPath());
        this.Plan = this.BuildPlan(script);

        if (this.Plan?.Script?.Scenes != null)
        {
            foreach (var scene in this.Plan.Script.Scenes)
            {
                // Query NarrativeEngine for what's needed - stub for now
                var data = this.GatherSceneData(scene);

                var sceneManager = this.CreateSceneManager(scene, data);
                await sceneManager.RunSceneAsync();
            }
        }

        // Signal GameManager - stub
    }

    /// <summary>
    /// Gets the path to the JSON script file for this stage.
    /// Must be implemented by subclasses.
    /// </summary>
    /// <returns>Path to JSON file (e.g., "res://source/frontend/stages/stage_1_ghost/ghost.json").</returns>
    protected abstract string GetDataPath();

    /// <summary>
    /// Builds the plan from the parsed script.
    /// Must be implemented by subclasses.
    /// </summary>
    /// <param name="script">The parsed narrative script.</param>
    /// <returns>The stage-specific plan.</returns>
    protected abstract TPlan BuildPlan(StoryScriptRoot script);

    /// <summary>
    /// Creates a SceneManager for the given scene.
    /// Must be implemented by subclasses.
    /// </summary>
    /// <param name="scene">The scene data.</param>
    /// <param name="data">Additional data for the scene.</param>
    /// <returns>The SceneManager instance.</returns>
    protected abstract SceneManager CreateSceneManager(StoryScriptElement scene, object data);

    /// <summary>
    /// Gathers data required for the scene.
    /// Stub implementation - subclasses can override.
    /// </summary>
    /// <param name="scene">The scene data.</param>
    /// <returns>Data object for the scene.</returns>
    protected virtual object GatherSceneData(StoryScriptElement scene)
    {
        // Query NarrativeEngine - stub
        return new object();
    }

    /// <summary>
    /// Overload: Runs the narrative sequence followed by a gameplay scene.
    /// For hybrid stages that combine story narration with interactive gameplay.
    /// Call this from a subclass override of RunStageAsync() for hybrid behavior.
    /// </summary>
    /// <param name="gameplayScenePath">Path to the gameplay scene .tscn file.</param>
    /// <returns>A task that completes when both narrative and gameplay are finished.</returns>
    protected async Task RunStageWithGameplayAsync(string gameplayScenePath)
    {
        // Phase 1: Run narrative sequences
        GD.Print("[CinematicDirector] Starting narrative phase...");
        var script = StoryLoader.LoadJsonScript(this.GetDataPath());
        this.Plan = this.BuildPlan(script);

        if (this.Plan?.Script?.Scenes != null)
        {
            foreach (var scene in this.Plan.Script.Scenes)
            {
                var data = this.GatherSceneData(scene);
                var sceneManager = this.CreateSceneManager(scene, data);
                await sceneManager.RunSceneAsync();
            }
        }

        // Phase 2: Load and run gameplay scene
        GD.Print($"[CinematicDirector] Starting gameplay phase: {gameplayScenePath}");
        await this.RunGameplaySceneAsync(gameplayScenePath);

        GD.Print("[CinematicDirector] Hybrid stage complete");
    }

    /// <summary>
    /// Helper: Loads and runs a gameplay/exploration scene.
    /// </summary>
    /// <param name="scenePath">Path to the .tscn file.</param>
    /// <returns>A task that completes when the scene is finished.</returns>
    protected virtual async Task RunGameplaySceneAsync(string scenePath)
    {
        var packedScene = ResourceLoader.Load<PackedScene>(scenePath);
        if (packedScene == null)
        {
            GD.PrintErr($"[CinematicDirector] Failed to load gameplay scene: {scenePath}");
            return;
        }

        var instance = packedScene.Instantiate();
        if (instance != null)
        {
            // Get root node using GetTree() - this requires a Node context
            // For now, we'll defer actual scene loading to stage-specific directors
            GD.Print($"[CinematicDirector] Prepared gameplay scene: {scenePath}");
        }

        // TODO: Implement scene completion detection and awaiting
        // Stage-specific directors should override this to handle actual scene loading
        await Task.CompletedTask;
    }
}
