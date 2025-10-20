// <copyright file="NarrativeAssetPreloader.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;

namespace OmegaSpiral.Source.Narrative.AssetManagement;
/// <summary>
/// Preloads and manages assets for narrative sequences to improve performance and loading times.
/// This system loads commonly used assets in advance to prevent hitches during gameplay.
/// </summary>
[GlobalClass]
public partial class NarrativeAssetPreloader : Node
{
    /// <summary>
    /// Dictionary of preloaded assets.
    /// </summary>
    private readonly Dictionary<string, Resource> preloadedAssets = new();

    /// <summary>
    /// List of asset paths to preload.
    /// </summary>
    private readonly List<string> preloadPaths = new();

    /// <summary>
    /// Gets a value indicating whether the preloader has completed initialization.
    /// </summary>
    public bool IsInitialized { get; private set; }

    /// <summary>
    /// Gets a value indicating whether assets are currently being preloaded.
    /// </summary>
    public bool IsPreloading { get; private set; }

    /// <inheritdoc/>
    public override void _Ready()
    {
        this.InitializePreloadList();
    }

    /// <summary>
    /// Initializes the list of assets to preload.
    /// </summary>
    private void InitializePreloadList()
    {
        // Add commonly used audio assets
        this.preloadPaths.Add("res://source/assets/sfx/confirmation_002.ogg");
        this.preloadPaths.Add("res://source/assets/sfx/drop_002.ogg");
        this.preloadPaths.Add("res://source/assets/sfx/impactWood_light_002.ogg");
        this.preloadPaths.Add("res://source/assets/sfx/chop.ogg");
        this.preloadPaths.Add("res://source/assets/sfx/error_006.ogg");

        // Add commonly used music assets
        this.preloadPaths.Add("res://source/assets/music/Apple Cider.mp3");
        this.preloadPaths.Add("res://source/assets/music/Insect Factory.mp3");
        this.preloadPaths.Add("res://source/assets/music/squashin_bugs_fixed.mp3");
        this.preloadPaths.Add("res://source/assets/music/the_fun_run.mp3");

        // Add commonly used texture assets
        this.preloadPaths.Add("res://source/assets/gui/logo-panels.png");
        this.preloadPaths.Add("res://source/assets/gui/combat/turn_bar_bg.png");
    }

    /// <summary>
    /// Preloads all registered assets asynchronously.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public async Task PreloadAssetsAsync()
    {
        if (this.IsPreloading)
        {
            GD.Print("NarrativeAssetPreloader: Already preloading assets");
            return;
        }

        this.IsPreloading = true;
        GD.Print($"NarrativeAssetPreloader: Starting to preload {this.preloadPaths.Count} assets");

        var startTime = Time.GetTicksMsec();

        foreach (string path in this.preloadPaths)
        {
            try
            {
                if (this.preloadedAssets.ContainsKey(path))
                {
                    continue; // Already preloaded
                }

                var resource = ResourceLoader.Load(path);
                if (resource != null)
                {
                    this.preloadedAssets[path] = resource;
                    GD.Print($"NarrativeAssetPreloader: Preloaded asset: {path}");
                }
                else
                {
                    GD.PrintErr($"NarrativeAssetPreloader: Failed to load asset: {path}");
                }
            }
            catch (Exception ex)
            {
                GD.PrintErr($"NarrativeAssetPreloader: Error preloading asset '{path}': {ex.Message}");
            }

            // Yield control to prevent frame drops during preloading
            await this.ToSignal(this.GetTree().CreateTimer(0.001f), Godot.Timer.SignalName.Timeout);
        }

        var endTime = Time.GetTicksMsec();
        GD.Print($"NarrativeAssetPreloader: Completed preloading in {(endTime - startTime)}ms. Loaded {this.preloadedAssets.Count} assets");

        this.IsInitialized = true;
        this.IsPreloading = false;
    }

    /// <summary>
    /// Gets a preloaded asset by its path.
    /// </summary>
    /// <param name="path">The path of the asset to retrieve.</param>
    /// <returns>The preloaded asset, or null if not found or not preloaded.</returns>
    public Resource? GetPreloadedAsset(string path)
    {
        return this.preloadedAssets.TryGetValue(path, out Resource? asset) ? asset : null;
    }

    /// <summary>
    /// Gets a preloaded asset by its path with type casting.
    /// </summary>
    /// <param name="path">The path of the asset to retrieve.</param>
    /// <typeparam name="T">The expected type of the asset.</typeparam>
    /// <returns>The preloaded asset cast to the specified type, or null if not found or wrong type.</returns>
    public T? GetPreloadedAsset<T>(string path) where T : Resource
    {
        var asset = this.GetPreloadedAsset(path);
        return asset as T;
    }

    /// <summary>
    /// Checks if an asset has been preloaded.
    /// </summary>
    /// <param name="path">The path of the asset to check.</param>
    /// <returns>True if the asset has been preloaded, false otherwise.</returns>
    public bool IsAssetPreloaded(string path)
    {
        return this.preloadedAssets.ContainsKey(path);
    }

    /// <summary>
    /// Adds an asset path to the preload list.
    /// </summary>
    /// <param name="path">The path of the asset to add for preloading.</param>
    public void AddToPreloadList(string path)
    {
        if (!this.preloadPaths.Contains(path))
        {
            this.preloadPaths.Add(path);
        }
    }

    /// <summary>
    /// Removes an asset path from the preload list.
    /// </summary>
    /// <param name="path">The path of the asset to remove from preloading.</param>
    public void RemoveFromPreloadList(string path)
    {
        this.preloadPaths.Remove(path);
    }

    /// <summary>
    /// Clears all preloaded assets from memory.
    /// </summary>
    public void ClearPreloadedAssets()
    {
        this.preloadedAssets.Clear();
        this.IsInitialized = false;
    }

    /// <summary>
    /// Gets the total number of preloaded assets.
    /// </summary>
    /// <returns>The count of preloaded assets.</returns>
    public int GetPreloadedAssetCount()
    {
        return this.preloadedAssets.Count;
    }

    /// <summary>
    /// Gets all preloaded asset paths.
    /// </summary>
    /// <returns>A list of all preloaded asset paths.</returns>
    public List<string> GetPreloadedAssetPaths()
    {
        return new List<string>(this.preloadedAssets.Keys);
    }
}
