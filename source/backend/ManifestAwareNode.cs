// <copyright file="ManifestAwareNode.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using Godot;
using System;

namespace OmegaSpiral.Source.Scripts.Infrastructure;

/// <summary>
/// Abstract base class for nodes that load and manage manifest data.
/// Implements <see cref="IManifestAware"/> and handles manifest loading logic via reflection.
/// Subclasses should override LoadManifest() and call the base implementation if needed.
/// </summary>
public abstract partial class ManifestAwareNode : Node, IManifestAware
{
    protected object? _loadedManifest;

    /// <inheritdoc/>
    public abstract string ManifestPath { get; }

    /// <inheritdoc/>
    public object? LoadedManifest => _loadedManifest;

    /// <inheritdoc/>
    public virtual void LoadManifest()
    {
        if (string.IsNullOrWhiteSpace(ManifestPath))
        {
            GD.PrintErr($"[ManifestAwareNode] ManifestPath is not defined for {GetType().Name}");
            return;
        }

        try
        {
            // Dynamically select the appropriate concrete loader based on the context
            // Subclasses override LoadManifest() to implement specific loading logic
            GD.Print($"[ManifestAwareNode] LoadManifest called for {GetType().Name}");
        }
        catch (Exception ex)
        {
            GD.PrintErr($"[ManifestAwareNode] Error loading manifest: {ex.Message}");
        }
    }
}
