// <copyright file="ChoiceOption.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

namespace OmegaSpiral.Source.Narrative;

using System.Collections.Generic;

/// <summary>
/// Represents a single choice option in narrative/dialogue systems.
/// Used across YAML narratives, JSON dialogues, and UI presentation layers.
/// This is a common data structure shared by all stages and systems.
/// </summary>
public partial class ChoiceOption : NarrativeElement
{
    /// <summary>
    /// Gets or sets the unique identifier for this choice option.
    /// Used to track which choice was selected and determine progression.
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the display text for this choice option.
    /// The text shown to the player as an available choice.
    /// </summary>
    public string? Text { get; set; }

    private string? _Label;

    /// <summary>
    /// Gets or sets the display label for this choice option.
    /// Falls back to <see cref="Text"/> when not explicitly provided.
    /// </summary>
    public string? Label
    {
        get => _Label ?? Text;
        set => _Label = value;
    }

    /// <summary>
    /// Gets or sets the response or consequence of selecting this choice.
    /// Shown after the player makes their selection.
    /// </summary>
    public string? Response { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this choice is available.
    /// Can be used to conditionally show/hide choices based on game state.
    /// </summary>
    public bool IsAvailable { get; set; } = true;

    /// <summary>
    /// Gets or sets the next dialogue node to go to when this choice is selected.
    /// Used in dialogue tree systems.
    /// </summary>
    public string? NextNodeId { get; set; }

    /// <summary>
    /// Gets or sets the story block number this choice leads to.
    /// Used in narrative block progression systems.
    /// </summary>
    public int NextBlock { get; set; }

    /// <summary>
    /// Gets or sets the detailed description for this choice option.
    /// Provides additional context about what this choice means or does.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the dreamweaver persona this choice aligns with (LIGHT, SHADOW, AMBITION).
    /// Used in Spiral Storytelling to track player alignment with narrative personas.
    /// </summary>
    public string? Dreamweaver { get; set; }

    /// <summary>
    /// Gets or sets the scores this choice grants to each dreamweaver persona.
    /// Dictionary mapping persona names (light, shadow, ambition) to score values.
    /// </summary>
    public Dictionary<string, int>? Scores { get; set; }
}
