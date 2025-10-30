// <copyright file="NarrativeSceneData.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

namespace OmegaSpiral.Source.Backend.Narrative;

/// <summary>
/// Contains data for narrative terminal scenes with branching story paths.
/// Supports dynamic story progression based on player choices and Dreamweaver alignment.
/// Used to define interactive narrative experiences with multiple endings.
/// </summary>
public partial class NarrativeSceneData
{
    /// <summary>
    /// Gets or sets the type identifier for this narrative scene.
    /// Used to distinguish between different narrative scene formats.
    /// </summary>
    public string Type { get; set; } = "narrative_terminal";

    /// <summary>
    /// Gets or sets the introductory text lines displayed at the start of the scene.
    /// Sets the narrative context before player choices begin.
    /// </summary>
    public IList<string> OpeningLines { get; set; } = new List<string>();

    /// <summary>
    /// Gets or sets the collection of story blocks that make up the narrative.
    /// Each block represents a segment of story with choices and progression.
    /// </summary>
    public IList<StoryBlock> StoryBlocks { get; set; } = new List<StoryBlock>();

    /// <summary>
    /// Gets or sets the final line displayed when exiting the narrative scene.
    /// Provides closure to the narrative experience.
    /// </summary>
    public string? ExitLine { get; set; }
}
