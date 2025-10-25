using System;
using OmegaSpiral.Source.Narrative;
using OmegaSpiral.Source.Scripts.Infrastructure;

namespace OmegaSpiral.Source.Scripts.Stages.Stage1;

/// <summary>
/// Transforms YAML narrative script for the Ghost Terminal opening into deterministic cinematic beats.
/// No new narrative text is generated here; all output is sourced from ghost.yaml.
/// Implements <see cref="ICinematicDirector{TPlan}"/> to provide caching via <see cref="CinematicDirector{TDirector, TPlan}"/>.
/// </summary>
public sealed class GhostTerminalCinematicDirector : CinematicDirector<GhostTerminalCinematicPlan>
{
    protected override string GetDataPath() => "res://source/stages/stage_1_ghost/ghost.yaml";

    protected override GhostTerminalCinematicPlan BuildPlan(NarrativeScript script)
    {
        return new GhostTerminalCinematicPlan(script);
    }
}

/// <summary>
/// Cinematic plan loaded from ghost.yaml.
/// Wraps the NarrativeScript for stage-specific access patterns.
/// </summary>
public sealed record GhostTerminalCinematicPlan(NarrativeScript Script);
