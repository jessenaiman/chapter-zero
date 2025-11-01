// <copyright file="EscapeCinematicPlan.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

namespace OmegaSpiral.Source.Stages.Stage5;

using OmegaSpiral.Source.Backend.Narrative;

/// <summary>
/// Cinematic plan for Escape stage.
/// Wraps the StoryBlock for stage-specific access patterns.
/// </summary>
public sealed class EscapeCinematicPlan : StoryPlan
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EscapeCinematicPlan"/> class.
    /// </summary>
    /// <param name="script">The story script root for Escape.</param>
    public EscapeCinematicPlan(StoryBlock script)
    {
        this.Script = script;
    }
}
