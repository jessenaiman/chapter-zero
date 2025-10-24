// <copyright file="INarrativeBlock.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

namespace OmegaSpiral.Source.Scripts.Common.Dialogue;

/// <summary>
/// Interface representing a narrative block containing paragraphs and choices.
/// Used for story segments that require player interaction and branching.
/// </summary>
public interface INarrativeBlock
{
    /// <summary>
    /// Gets the paragraphs of text in this narrative block.
    /// </summary>
    IReadOnlyList<string> Paragraphs { get; }

    /// <summary>
    /// Gets the question or prompt following the paragraphs.
    /// </summary>
    string Question { get; }

    /// <summary>
    /// Gets the choices available after this narrative block.
    /// </summary>
    IReadOnlyList<IDialogueChoice> Choices { get; }

    /// <summary>
    /// Gets the index of the next narrative block.
    /// </summary>
    int NextBlock { get; }
}
