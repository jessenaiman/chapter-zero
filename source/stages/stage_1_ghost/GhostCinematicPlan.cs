// <copyright file="GhostCinematicPlan.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

namespace OmegaSpiral.Source.Stages.Stage1;

using OmegaSpiral.Source.Backend.Narrative;

/// <summary>
/// Plan for Ghost Terminal stage, wrapping the narrative script.
/// </summary>
public sealed class GhostCinematicPlan : StoryPlan
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GhostCinematicPlan"/> class.
    /// </summary>
    /// <param name="script">The story script root.</param>
    public GhostCinematicPlan(StoryBlock script)
    {
        this.Script = script;
    }
}
