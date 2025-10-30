// <copyright file="TownCinematicPlan.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

namespace OmegaSpiral.Source.Stages.Stage3;

using OmegaSpiral.Source.Backend.Narrative;

/// <summary>
/// Cinematic plan for Town stage.
/// Wraps the StoryScriptRoot for stage-specific access patterns.
/// </summary>
public sealed class TownCinematicPlan : StoryPlan
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TownCinematicPlan"/> class.
    /// </summary>
    /// <param name="script">The story script root for Town.</param>
    public TownCinematicPlan(StoryScriptRoot script)
    {
        this.Script = script;
    }
}
