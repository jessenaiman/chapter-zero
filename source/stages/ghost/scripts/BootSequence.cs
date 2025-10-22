// <copyright file="BootSequence.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using Godot;
using System.Threading.Tasks;
using OmegaSpiral.Source.UI.Terminal;

namespace OmegaSpiral.Source.Scripts.Stages.Stage1;

/// <summary>
/// Initial boot sequence scene for Stage 1 opening.
/// Displays system initialization messages and transitions to the opening monologue.
/// </summary>
[GlobalClass]
public partial class BootSequence : TerminalBase
{
    private bool _waitingForInput;

    /// <inheritdoc/>
    public override async void _Ready()
    {
        base._Ready();

        GD.Print("BootSequence _Ready called");

        ApplyVisualPreset(TerminalVisualPreset.BootSequence);

        // Enable input processing for user interaction
        SetProcessInput(true);

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

        // Add continue prompt
        await AppendTextAsync("\n\n[Press any key or click to continue...]", useGhostEffect: false);

        GD.Print("Boot sequence text displayed, waiting for input");

        _waitingForInput = true;
    }

    /// <summary>
    /// Proceeds to the opening monologue after user input.
    /// </summary>
    private async Task ProceedToMonologueAsync()
    {
        GD.Print("ProceedToMonologueAsync called");
        GhostTerminalCinematicPlan plan = GhostTerminalCinematicDirector.GetPlan();
        GhostTerminalBootBeat bootBeat = plan.Boot;

        if (bootBeat.FadeToStable)
        {
            await ToSignal(GetTree().CreateTimer(1.5f), SceneTreeTimer.SignalName.Timeout);
        }

        await PixelDissolveAsync(1.8);
        ApplyVisualPreset(TerminalVisualPreset.StableBaseline);

        // TODO: Boss Critter - Audio Architecture Refactor
        // Mixing audio responsibilities with boot sequence violates separation of concerns.
        // Should use centralized AudioManager instead of direct audio playback.
        // Professional AAA approach: AudioManager autoload handles all audio via buses and pooling.
        GD.Print("Transitioning to opening monologue");
        TransitionToScene("res://source/stages/ghost/scenes/opening_monologue.tscn");
    }

    /// <inheritdoc/>
    public override void _Input(InputEvent @event)
    {
        base._Input(@event);

        if (_waitingForInput && (@event is InputEventMouseButton mouse && mouse.Pressed ||
                                @event is InputEventKey key && key.Pressed))
        {
            GD.Print("Input received, proceeding to monologue");
            _waitingForInput = false;
            GetViewport().SetInputAsHandled();
            _ = ProceedToMonologueAsync();
        }
    }
}
