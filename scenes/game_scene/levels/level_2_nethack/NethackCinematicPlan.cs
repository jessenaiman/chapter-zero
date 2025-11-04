// <copyright file="NethackCinematicPlan.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

namespace OmegaSpiral.Source.Stages.Stage2;

using OmegaSpiral.Source.Backend.Narrative;

/// <summary>
/// Cinematic plan for Nethack stage.
/// Wraps the StoryBlock for stage-specific access patterns.
/// </summary>
public sealed class NethackCinematicPlan : StoryPlan
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NethackCinematicPlan"/> class.
    /// </summary>
    /// <param name="script">The story script root for Nethack.</param>
    public NethackCinematicPlan(StoryBlock script)
    {
        this.Script = script;
    }
}
