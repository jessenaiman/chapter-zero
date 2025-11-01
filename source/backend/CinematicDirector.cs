// <copyright file="CinematicDirector.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

namespace OmegaSpiral.Source.Backend;

using System.Collections.Generic;
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
    /// <returns>A task that completes when the stage is finished and yields the scene results.</returns>
    Task<IReadOnlyList<SceneResult>> RunStageAsync();
}

/// <summary>
/// Result of running a scene.
/// </summary>
public class SceneResult
{
    /// <summary>
    /// Gets or sets the scene ID.
    /// </summary>
    public string? SceneId { get; set; }

    /// <summary>
    /// Gets or sets the choice made.
    /// </summary>
    public string? Choice { get; set; }
}

/// <summary>
/// Base class for stage story directors that load JSON story scripts and cache their plans.
/// Handles thread-safe caching, loading, and plan building for all stages.
/// Each stage director inherits this and implements abstract methods for data path and plan building.
/// <para>
/// Inheritance Example:
/// <code>
/// public class GhostStageDirector : CinematicDirector
/// {
///     public GhostStageDirector() : base(new StageConfiguration
///     {
///         DataPath = "res://source/stages/stage_1_ghost/ghost.json",
///         ScenePath = "res://source/scenes/narrative_ui.tscn", // Optional UI scene
///         PlanFactory = script => new GhostPlan(script),
///         ManagerFactory = (scene, data) => new GhostSceneManager(scene, data)
///     }) { }
/// }
/// </code>
/// </para>
/// <para>
/// Usage Example:
/// <code>
/// var director = new GhostStageDirector();
/// var results = await director.RunStageAsync();
/// // Results contain scene outcomes for game state updates
/// </code>
/// </para>
/// </summary>
public abstract class CinematicDirector : ICinematicDirector
{
    protected StageConfiguration Config { get; }

    protected StoryPlan? Plan { get; set; }

    protected CinematicDirector(StageConfiguration config)
    {
        this.Config = config;
    }

    /// <summary>
    /// Runs the stage by iterating through scenes, creating SceneManager for each, awaiting completion.
    ///
    /// PURE NARRATIVE PATTERN (Default):
    /// Uses base implementation for stages with narrative sequences only (Ghost, Nethack).
    ///
    /// NARRATIVE WITH UI PATTERN (Override GetScenePath):
    /// Subclasses can override GetScenePath() to return a scene path, which will be loaded
    /// before running narrative sequences. This is for narrative stages that need a UI scene (Ghost).
    ///
    /// HYBRID STAGE PATTERN (Override RunStageAsync):
    /// Subclasses can override this method and call RunStageWithGameplayAsync(scenePath)
    /// to combine narrative sequences with interactive gameplay scenes.
    /// This is the standardized approach for hybrid stages (Town, PartySelection, Escape).
    /// <para>
    /// Override Example for Hybrid Stage:
    /// <code>
    /// public override async Task&lt;IReadOnlyList&lt;SceneResult&gt;&gt; RunStageAsync()
    /// {
    ///     // Phase 1: Run narrative
    ///     var narrativeResults = await base.RunStageAsync();
    ///
    ///     // Phase 2: Load gameplay scene
    ///     await RunStageWithGameplayAsync("res://source/scenes/town_exploration.tscn");
    ///
    ///     return narrativeResults;
    /// }
    /// </code>
    /// </para>
    /// <para>
    /// Override Example for Pure Narrative with UI:
    /// <code>
    /// protected override string? GetScenePath() => "res://source/scenes/narrative_display.tscn";
    /// </code>
    /// </para>
    /// </summary>
    /// <returns>A task that completes when the stage is finished.</returns>
    public virtual async Task<IReadOnlyList<SceneResult>> RunStageAsync()
    {
        // Optional: Load UI scene if specified by subclass
        var scenePath = this.GetScenePath();
        if (!string.IsNullOrEmpty(scenePath))
        {
            GD.Print($"[CinematicDirector] Loading UI scene: {scenePath}");
            await this.LoadUiSceneAsync(scenePath);
        }

        // Run narrative sequences
        var script = StoryLoader.LoadJsonScript(this.GetDataPath());
        this.Plan = this.BuildPlan(script);

        var results = new List<SceneResult>();

        if (this.Plan?.Script?.Scenes != null)
        {
            foreach (var scene in this.Plan.Script.Scenes)
            {
                // Query NarrativeEngine for what's needed - stub for now
                var data = this.GatherSceneData(scene);

                var sceneManager = this.CreateSceneManager(scene, data);
                var result = await sceneManager.RunSceneAsync();
                results.Add(result);
            }
        }

        // Signal GameManager - stub
        return results;
    }

    /// <summary>
    /// Gets the path to the JSON script file for this stage.
    /// Must be implemented by subclasses.
    /// </summary>
    /// <returns>Path to narrative data file (e.g., "res://source//stages/stage_1_ghost/ghost.json").</returns>
    protected virtual string GetDataPath() => this.Config.DataPath;

    /// <summary>
    /// Gets the path to a UI scene to load before running narrative sequences.
    /// Override in subclasses that need a visual scene for narrative display.
    /// </summary>
    /// <returns>Path to .tscn file, or null if no scene should be loaded.</returns>
    protected virtual string? GetScenePath() => this.Config.ScenePath;

    /// <summary>
    /// Builds the plan from the parsed script.
    /// Must be implemented by subclasses.
    /// </summary>
    /// <param name="script">The parsed narrative script.</param>
    /// <returns>The stage-specific plan.</returns>
    protected virtual StoryPlan BuildPlan(StoryBlock script) => this.Config.PlanFactory(script);

    /// <summary>
    /// Creates a SceneManager for the given scene.
    /// Must be implemented by subclasses.
    /// </summary>
    /// <param name="scene">The scene data.</param>
    /// <param name="data">Additional data for the scene.</param>
    /// <returns>The SceneManager instance.</returns>
    protected virtual SceneManager CreateSceneManager(StoryBlock scene, object data) => this.Config.ManagerFactory(scene, data);

    /// <summary>
    /// Gathers data required for the scene.
    /// Stub implementation - subclasses can override.
    /// </summary>
    /// <param name="scene">The scene data.</param>
    /// <returns>Data object for the scene.</returns>
    protected virtual object GatherSceneData(StoryBlock scene)
    {
        // Query NarrativeEngine - stub
        return new object();
    }

    /// <summary>
    /// Loads a UI scene into the scene tree.
    /// Uses ResourceLoader to instantiate the scene but does NOT run narrative on it.
    /// The scene is simply added to the tree for UI display during narrative playback.
    /// </summary>
    /// <param name="scenePath">Path to the .tscn file.</param>
    /// <returns>A task that completes when the scene is loaded.</returns>
    protected virtual async Task LoadUiSceneAsync(string scenePath)
    {
        var packedScene = ResourceLoader.Load<PackedScene>(scenePath);
        if (packedScene == null)
        {
            GD.PrintErr($"[CinematicDirector] Failed to load UI scene: {scenePath}");
            return;
        }

        var tree = Engine.GetMainLoop() as SceneTree;
        if (tree == null)
        {
            GD.PrintErr("[CinematicDirector] Could not get scene tree");
            return;
        }

        var instance = packedScene.Instantiate();
        if (instance != null)
        {
            tree.Root.AddChild(instance);
            GD.Print($"[CinematicDirector] UI scene loaded: {scenePath}");
        }

        await Task.CompletedTask;
    }

    /// <summary>
    /// Hybrid stage helper: Runs the narrative sequence followed by a gameplay scene.
    /// For hybrid stages that combine story narration with interactive gameplay.
    /// Call this from a subclass override of RunStageAsync() for hybrid behavior.
    /// </summary>
    /// <param name="gameplayScenePath">Path to the gameplay scene .tscn file.</param>
    /// <returns>A task that completes when both narrative and gameplay are finished.</returns>
    protected async Task<IReadOnlyList<SceneResult>> RunStageWithGameplayAsync(string gameplayScenePath)
    {
        // Phase 1: Run narrative sequences
        GD.Print("[CinematicDirector] Starting narrative phase...");
        var script = StoryLoader.LoadJsonScript(this.GetDataPath());
        this.Plan = this.BuildPlan(script);

        var results = new List<SceneResult>();

        if (this.Plan?.Script?.Scenes != null)
        {
            foreach (var scene in this.Plan.Script.Scenes)
            {
                var data = this.GatherSceneData(scene);
                var sceneManager = this.CreateSceneManager(scene, data);
                var result = await sceneManager.RunSceneAsync();
                results.Add(result);
            }
        }

        // Phase 2: Load and run gameplay scene
        GD.Print($"[CinematicDirector] Starting gameplay phase: {gameplayScenePath}");
        await this.LoadUiSceneAsync(gameplayScenePath);

        GD.Print("[CinematicDirector] Hybrid stage complete");
        return results;
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
