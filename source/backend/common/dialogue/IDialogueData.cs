// <copyright file="IDialogueData.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

namespace OmegaSpiral.Source.Scripts.Common.Dialogue;

/// <summary>
/// Interface for dialogue data structures that can be loaded from JSON.
/// Provides the core structure for dialogue content including opening lines,
/// dialogue lines, choices, and narrative blocks.
/// </summary>
public interface IDialogueData
{
    /// <summary>
    /// Gets the dialogue lines for the initial greeting/opening.
    /// </summary>
    IReadOnlyList<string> OpeningLines { get; }

    /// <summary>
    /// Gets the dialogue lines for the main content.
    /// </summary>
    IReadOnlyList<string> DialogueLines { get; }

    /// <summary>
    /// Gets the choices available to the player.
    /// </summary>
    IReadOnlyList<IDialogueChoice> Choices { get; }

    /// <summary>
    /// Gets any special narrative blocks or responses.
    /// </summary>
    IReadOnlyList<INarrativeBlock> NarrativeBlocks { get; }
}
