// <copyright file="IDialogueChoice.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

namespace OmegaSpiral.Source.Scripts.Common.Dialogue;

/// <summary>
/// Interface representing a dialogue choice.
/// Defines the contract for choices that appear in dialogue and narrative contexts.
/// </summary>
public interface IDialogueChoice
{
    /// <summary>
    /// Gets the unique identifier for this choice.
    /// </summary>
    string Id { get; }

    /// <summary>
    /// Gets the display text for this choice.
    /// </summary>
    string Text { get; }

    /// <summary>
    /// Gets the description or additional context for this choice.
    /// </summary>
    string Description { get; }

    /// <summary>
    /// Gets the index of the next dialogue block to show.
    /// </summary>
    int NextDialogueIndex { get; }
}
