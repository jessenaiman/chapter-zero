// <copyright file="DialogueChoice.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

namespace OmegaSpiral.Source.Narrative;

/// <summary>
/// Represents a choice option that can be used in both dialogue and narrative contexts.
/// Consolidates the functionality of both DialogueChoice and ChoiceOption classes.
/// </summary>
public partial class ChoiceOption
{
    /// <summary>
    /// Gets or sets the response or consequence of selecting this choice.
    /// </summary>
    public string? Response { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this choice is available.
    /// </summary>
    public bool IsAvailable { get; set; } = true;

    /// <summary>
    /// Gets or sets the next dialogue node to go to when this choice is selected.
    /// </summary>
    public string? NextNodeId { get; set; }

    /// <summary>
    /// Gets or sets the story block number this choice leads to (for narrative contexts).
    /// </summary>
    public int NextBlock { get; set; }

    /// <summary>
    /// Gets or sets the detailed description for this choice option.
    /// Provides additional context or explanation for the choice.
    /// </summary>
    public string Description { get; set; } = string.Empty;
}
