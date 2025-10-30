// <copyright file="IManifestAware.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

namespace OmegaSpiral.Source.Backend;

/// <summary>
/// Marks a node or object as capable of loading and managing manifest data.
/// Enforces a standard contract for all manifest-aware components across the codebase.
/// Implementations should use this interface to provide type-safe manifest access.
/// </summary>
public interface IManifestAware
{
    /// <summary>
    /// Gets the path to the manifest file that this component loads.
    /// Must be a valid Godot resource path (e.g., "res://source/data/manifest.json").
    /// </summary>
    string ManifestPath { get; }

    /// <summary>
    /// Gets the loaded manifest data. Returns null if LoadManifest() has not been called or if loading failed.
    /// </summary>
    object? LoadedManifest { get; }

    /// <summary>
    /// Loads the manifest file from the path specified by <see cref="ManifestPath"/>.
    /// Implementations should handle errors gracefully and log failures.
    /// </summary>
    void LoadManifest();
}
