// <copyright file="SceneManager.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

namespace OmegaSpiral.Source.Backend;

using System.Threading.Tasks;
using Godot;
using OmegaSpiral.Source.Backend.Narrative;

/// <summary>
/// Per-scene manager that takes data from CinematicDirector, loads .tscn, runs the scene, signals completion.
/// </summary>
public partial class SceneManager : Node
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SceneManager"/> class (parameterless for Godot autoload).
    /// </summary>
    public SceneManager()
    {
        this.SceneData = new StoryScriptElement();
        this.AdditionalData = new object();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SceneManager"/> class.
    /// </summary>
    /// <param name="scene">The scene data.</param>
    /// <param name="data">Additional data for the scene.</param>
    public SceneManager(StoryScriptElement scene, object data)
    {
        this.SceneData = scene;
        this.AdditionalData = data;
    }

    /// <summary>
    /// Gets or sets the scene data for this manager.
    /// </summary>
    protected StoryScriptElement SceneData { get; set; }

    /// <summary>
    /// Gets or sets the additional data for this scene.
    /// </summary>
    protected object AdditionalData { get; set; }

    /// <summary>
    /// Runs the scene by loading .tscn, displaying narrative, handling UI, signals when done.
    /// </summary>
    /// <returns>A task that completes when the scene is finished.</returns>
    public async Task RunSceneAsync()
    {
        // Load .tscn - stub, assume sceneData has scenePath
        // var scenePath = sceneData.ScenePath;
        // var packedScene = ResourceLoader.Load<PackedScene>(scenePath);
        // var instance = packedScene.Instantiate();
        // AddChild(instance);

        // Run the scene (display narrative, handle UI) - stub
        // Use NarrativeEngine to play the scene

        // For now, just delay
        await Task.Delay(1000);

        // Signal completion - stub
    }
}
