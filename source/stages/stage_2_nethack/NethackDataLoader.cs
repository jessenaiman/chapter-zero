// <copyright file="NethackDataLoader.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using OmegaSpiral.Source.Narrative;
using OmegaSpiral.Source.Scripts.Infrastructure;

namespace OmegaSpiral.Source.Stages.Stage2;

/// <summary>
/// Data loader for nethack.yaml using CinematicDirector pattern.
/// Used by NethackCinematicDirector to load the narrative script.
/// </summary>
public sealed class NethackDataLoader : CinematicDirector<NethackCinematicPlan>
{
    /// <inheritdoc/>
    protected override string GetDataPath() => "res://source/stages/stage_2_nethack/nethack.yaml";

    /// <inheritdoc/>
    protected override NethackCinematicPlan BuildPlan(NarrativeScript script)
    {
        return new NethackCinematicPlan(script);
    }
}

/// <summary>
/// Cinematic plan loaded from nethack.yaml.
/// Wraps the NarrativeScript for stage-specific access patterns.
/// </summary>
public sealed record NethackCinematicPlan(NarrativeScript Script);
