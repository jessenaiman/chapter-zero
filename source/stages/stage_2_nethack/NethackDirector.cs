using System;
using OmegaSpiral.Source.Narrative;
using OmegaSpiral.Source.Scripts.Infrastructure;

namespace OmegaSpiral.Source.Scripts.Stages.Stage2;

/// <summary>
/// Transforms YAML narrative script for the Nethack Echo Chamber into deterministic cinematic beats.
/// No new narrative text is generated here; all output is sourced from nethack.yaml.
/// Inherits from <see cref="CinematicDirector{TPlan}"/> for thread-safe caching and plan management.
/// </summary>
public sealed class NethackDirector : CinematicDirector<NethackCinematicPlan>
{
    /// <inheritdoc/>
    protected override string GetDataPath() => "res://source/stages/stage_2_nethack/nethack.yaml";

    /// <inheritdoc/>
    protected override NethackCinematicPlan BuildPlan(NarrativeScript script)
    {
        // Simple wrapper - just hold the script for now
        return new NethackCinematicPlan(script);
    }
}

/// <summary>
/// Cinematic plan loaded from nethack.yaml.
/// Wraps the NarrativeScript for stage-specific access patterns.
/// </summary>
public sealed record NethackCinematicPlan(NarrativeScript Script);
