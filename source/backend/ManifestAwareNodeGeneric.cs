// <copyright file="ManifestAwareNodeGeneric.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using Godot;

namespace OmegaSpiral.Source.Backend;

/// <summary>
/// Generic abstract base class for strongly-typed manifest loading.
/// Provides typed access to loaded manifest via <see cref="LoadedData"/>.
/// </summary>
/// <typeparam name="T">The type of manifest data to load (e.g., StageManifest, IReadOnlyList&lt;ManifestStage&gt;).</typeparam>
public abstract partial class ManifestAwareNode<T> : ManifestAwareNode where T : class
{
    /// <summary>
    /// Gets the loaded manifest data as the strongly-typed generic parameter.
    /// Returns null if <see cref="LoadManifest"/> has not been called or if loading failed.
    /// </summary>
    protected T? LoadedData => _loadedManifest as T;

    /// <inheritdoc/>
    public override void LoadManifest()
    {
        if (string.IsNullOrWhiteSpace(ManifestPath))
        {
            GD.PrintErr($"[ManifestAwareNode<T>] ManifestPath is not defined for {GetType().Name}");
            return;
        }

        try
        {
            // Dynamically select the appropriate concrete loader based on the generic type
            _loadedManifest = LoadManifestInternal();

            if (_loadedManifest == null)
            {
                GD.PrintErr($"[ManifestAwareNode<T>] Failed to load manifest from: {ManifestPath}");
                return;
            }

            GD.Print($"[ManifestAwareNode<T>] Successfully loaded manifest for {GetType().Name}");
        }
        catch (Exception ex)
        {
            GD.PrintErr($"[ManifestAwareNode<T>] Error loading manifest: {ex.Message}");
        }
    }

    /// <summary>
    /// Internal method to load manifest using appropriate concrete loader.
    /// Handles StageManifest, IReadOnlyList&lt;ManifestStage&gt;, and other types.
    /// </summary>
    private T? LoadManifestInternal()
    {
        var manifestType = typeof(T);

        // Handle StageManifest type
        if (manifestType == typeof(StageManifest))
        {
            var loader = new StageManifestLoader();
            return (T?)(object?)loader.LoadManifest(ManifestPath);
        }

        // Handle IReadOnlyList<ManifestStage> type
        if (manifestType == typeof(IReadOnlyList<ManifestStage>))
        {
            var loader = new ManifestLoader();
            return (T?)(object?)loader.LoadManifest(ManifestPath);
        }

        GD.PrintErr($"[ManifestAwareNode<T>] Unsupported manifest type: {manifestType.Name}");
        return null;
    }
}
