// <copyright file="DreamweaverChoice.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using OmegaSpiral.Source.Scripts;
using OmegaSpiral.Source.Scripts.Common;

/// <summary>
/// Represents a choice option aligned with a specific Dreamweaver thread.
/// Extends the base choice system with Dreamweaver-specific alignment bonuses.
/// Used in narrative sequences to influence player progression and scoring.
/// </summary>
public partial class DreamweaverChoice : ChoiceOption
{
    /// <summary>
    /// Gets or sets the unique identifier for this choice option.
    /// Overrides base class property with Dreamweaver-specific behavior.
    /// </summary>
    public new string? Id { get; set; }

    /// <summary>
    /// Gets or sets the display text for this choice option.
    /// Overrides base class property with Dreamweaver-specific behavior.
    /// </summary>
    public new string? Text { get; set; }

    /// <summary>
    /// Gets or sets the detailed description for this choice option.
    /// Overrides base class property with Dreamweaver-specific behavior.
    /// </summary>
    public new string? Description { get; set; }

    /// <summary>
    /// Gets or sets the Dreamweaver narrative thread this choice belongs to.
    /// Determines which Dreamweaver persona will respond to this choice.
    /// </summary>
    public DreamweaverThread Thread { get; set; }

    /// <summary>
    /// Gets or sets the alignment bonuses awarded when this choice is selected.
    /// Maps each Dreamweaver type to the score increase it receives.
    /// </summary>
    public Dictionary<DreamweaverType, int> AlignmentBonus { get; set; } = new ();
}
