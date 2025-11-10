// <copyright file="PartySelectionCinematicPlan.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

namespace OmegaSpiral.Source.Stages.Stage4;

using OmegaSpiral.Source.Backend.Narrative;

/// <summary>
/// Cinematic plan for Party Selection (Echo Vault) stage.
/// Wraps the StoryBlock for stage-specific access patterns.
/// </summary>
public sealed class PartySelectionCinematicPlan : StoryPlan
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PartySelectionCinematicPlan"/> class.
    /// </summary>
    /// <param name="script">The story script root for Party Selection.</param>
    public PartySelectionCinematicPlan(StoryBlock script)
    {
        this.Script = script;
    }
}
