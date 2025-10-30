// <copyright file="StoryBlock.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

namespace OmegaSpiral.Source.Backend.Narrative;

/// <summary>
/// Represents a story block used in narrative scenes.
/// Contains paragraphs of text, optional question with choices, and navigation info.
/// </summary>
public class StoryBlock
{
    /// <summary>
    /// Gets or sets the paragraphs of text in this story block.
    /// </summary>
    public IList<string> Paragraphs { get; set; } = new List<string>();

    /// <summary>
    /// Gets or sets the question text for interactive blocks.
    /// </summary>
    public string? Question { get; set; }

    /// <summary>
    /// Gets or sets the choices available for this story block.
    /// </summary>
    public IList<ChoiceOption> Choices { get; set; } = new List<ChoiceOption>();

    /// <summary>
    /// Gets or sets the next block index for navigation.
    /// -1 indicates no next block (end of sequence).
    /// </summary>
    public int Next { get; set; } = -1;
}
