// <copyright file="IDialogueManager.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

namespace OmegaSpiral.Source.Scripts.Common.Dialogue;

/// <summary>
/// Interface for dialogue managers that handle loading, caching, and serving dialogue.
/// Manages the lifecycle of dialogue data from loading to serving.
/// </summary>
public interface IDialogueManager
{
    /// <summary>
    /// Loads dialogue data from a JSON resource path.
    /// </summary>
    /// <param name="resourcePath">Path to the dialogue JSON file.</param>
    /// <returns>A task that resolves to the dialogue data.</returns>
    Task<IDialogueData> LoadDialogueAsync(string resourcePath);

    /// <summary>
    /// Gets cached dialogue data by ID, or loads it if not cached.
    /// </summary>
    /// <param name="dialogueId">The unique identifier for the dialogue.</param>
    /// <returns>The dialogue data, or <see langword="null"/> if not found.</returns>
    Task<IDialogueData> GetDialogueAsync(string dialogueId);

    /// <summary>
    /// Preloads and caches dialogue data for future use.
    /// </summary>
    /// <param name="dialogueId">The identifier for the dialogue to preload.</param>
    /// <param name="resourcePath">The path to the dialogue resource.</param>
    void PreloadDialogue(string dialogueId, string resourcePath);

    /// <summary>
    /// Clears cached dialogue data.
    /// </summary>
    /// <param name="dialogueId">The ID of the dialogue to clear, or <see langword="null"/> for all.</param>
    void ClearCache(string? dialogueId = null);
}
