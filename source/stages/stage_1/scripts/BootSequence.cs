// <copyright file="BootSequence.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using Godot;
using System.Threading.Tasks;
using OmegaSpiral.Source.Stages.Ghost;
using OmegaSpiral.Source.UI.Terminal;

namespace OmegaSpiral.Source.Scripts.Stages.Stage1;

/// <summary>
/// The boot sequence scene for the ghost terminal.
/// Displays initialization messages and transitions to the next scene via Stage1Controller.
/// This scene is presentation-only; flow logic is managed by Stage1Controller.
/// </summary>
[GlobalClass]
public partial class BootSequence : GhostTerminalUI
{
    private bool _waitingForInput;
#pragma warning disable CA2213 // Stage1Controller is managed by Godot's scene tree
    private Stage1Controller? _stage1Controller;
#pragma warning restore CA2213

    /// <inheritdoc/>
    public override async void _Ready()
    {
        base._Ready();

        GD.Print("BootSequence _Ready called");

        // Get reference to the Stage 1 controller
        _stage1Controller = GetNode<Stage1Controller>("/root/Stage1Controller");

        ApplyVisualPreset(GhostTerminalVisualPreset.BootSequence);

        // Enable input processing for user interaction
        SetProcessInput(true);

        // Start boot sequence
        await RunBootSequenceAsync().ConfigureAwait(false);
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

            await AppendTextAsync(line, useGhostEffect: true, charDelaySeconds: 0.045f).ConfigureAwait(false);
            await ToSignal(GetTree().CreateTimer(0.6f), SceneTreeTimer.SignalName.Timeout);
        }

        // Add continue prompt
        await AppendTextAsync("\n\n[Press any key or click to continue...]", useGhostEffect: false).ConfigureAwait(false);

        GD.Print("Boot sequence text displayed, waiting for input");

        _waitingForInput = true;
    }

    /// <summary>
    /// Proceeds to the opening monologue after user input.
    /// Delegates scene transition to Stage1Controller.
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

        // Animate dissolve effect
        await PixelDissolveAsync(1.8f).ConfigureAwait(false);
        ApplyVisualPreset(GhostTerminalVisualPreset.StableBaseline);

        // TODO: Boss Critter - Audio Architecture Refactor
        // Mixing audio responsibilities with boot sequence violates separation of concerns.
        // Should use centralized AudioManager instead of direct audio playback.
        // Professional AAA approach: AudioManager autoload handles all audio via buses and pooling.
        GD.Print("Transitioning to opening monologue");

        if (_stage1Controller != null)
        {
            await _stage1Controller.AdvanceFromBootSequenceAsync().ConfigureAwait(false);
        }
        else
        {
            GD.PrintErr("[BootSequence] Stage1Controller not found");
        }
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
