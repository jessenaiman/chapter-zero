// <copyright file="BootSequence.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using Godot;
using System.Threading.Tasks;

namespace OmegaSpiral.Source.Scripts.Stages.Stage1;

/// <summary>
/// Initial boot sequence scene for Stage 1 opening.
/// Displays system initialization messages and transitions to the opening monologue.
/// </summary>
[GlobalClass]
public partial class BootSequence : TerminalBase
{
    /// <inheritdoc/>
    public override async void _Ready()
    {
        base._Ready();

        ApplyVisualPreset(TerminalVisualPreset.BootSequence);

        // Start boot sequence
        await RunBootSequenceAsync();
    }

    /// <summary>
    /// Runs the complete boot sequence with system messages.
    /// </summary>
    /// <returns>A task that completes when boot sequence finishes.</returns>
    private async Task RunBootSequenceAsync()
    {
        GhostTerminalCinematicPlan plan = GhostTerminalCinematicDirector.GetPlan();
        GhostTerminalBootBeat bootBeat = plan.Boot;

        foreach (string line in bootBeat.GlitchLines)
        {
            if (GhostTerminalNarrationHelper.TryParsePause(line, out double pauseSeconds))
            {
                await ToSignal(GetTree().CreateTimer(pauseSeconds), SceneTreeTimer.SignalName.Timeout);
                continue;
            }

            await AppendTextAsync(line, useGhostEffect: true, charDelaySeconds: 0.045);
            await ToSignal(GetTree().CreateTimer(0.6f), SceneTreeTimer.SignalName.Timeout);
        }

        if (bootBeat.FadeToStable)
        {
            await ToSignal(GetTree().CreateTimer(1.5f), SceneTreeTimer.SignalName.Timeout);
        }

        await PixelDissolveAsync(1.8);
        ApplyVisualPreset(TerminalVisualPreset.StableBaseline);

        // Transition to opening monologue
        TransitionToScene("res://Source/Stages/Stage1/opening_monologue.tscn");
    }
}
