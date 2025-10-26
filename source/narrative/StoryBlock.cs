// <copyright file="StoryBlock.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

namespace OmegaSpiral.Source.Narrative;

/// <summary>
/// Represents a segment of narrative story with text and branching choices.
/// Contains paragraphs of story text and the choices that lead to other blocks.
/// Forms the building blocks of the interactive narrative experience.
/// </summary>
public partial class StoryBlock
{
    /// <summary>
    /// Gets or sets the collection of text paragraphs that make up this story block.
    /// Each paragraph is displayed sequentially with typewriter effects.
    /// </summary>
    public IList<string> Paragraphs { get; set; } = new List<string>();

    /// <summary>
    /// Gets or sets the question or prompt presented after the story text.
    /// Asks the player to make a decision that affects narrative progression.
    /// </summary>
    public string? Question { get; set; }

    /// <summary>
    /// Gets or sets the collection of choice options available after this story block.
    /// Each option leads to a different story path or outcome.
    /// </summary>
    public IList<ChoiceOption> Choices { get; set; } = new List<ChoiceOption>();
}
