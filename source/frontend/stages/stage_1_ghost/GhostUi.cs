// <copyright file="GhostUi.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using OmegaSpiral.Source.Narrative;
using OmegaSpiral.Source.Ui.Omega;

namespace OmegaSpiral.Source.Stages.Stage1;

/// <summary>
/// UI for Ghost Terminal stage using godot_xterm for authentic terminal experience.
/// Handles narrative display, choices, and shader effects.
/// </summary>
[GlobalClass]
public partial class GhostUi : OmegaContainer, INarrativeHandler
{
    [Export] public bool EnableBootSequence { get; set; } = true;
    [Export] public float DefaultTypingSpeed { get; set; } = 15f;

    private Node? Terminal { get; set; }
    private VBoxContainer? ChoiceContainer { get; set; }
    private bool BootSequencePlayed { get; set; }

    /// <inheritdoc/>
    public override void _Ready()
    {
        base._Ready();
        Terminal = GetNodeOrNull<Node>("../../NarrativeViewport/NarrativeStack/Terminal");
        ChoiceContainer = GetNodeOrNull<VBoxContainer>("../../NarrativeViewport/NarrativeStack/ChoiceContainer");

        if (EnableBootSequence && !BootSequencePlayed)
        {
            CallDeferred(nameof(StartBootSequence));
        }
    }

    /// <summary>
    /// Starts the boot sequence.
    /// </summary>
    private async void StartBootSequence()
    {
        await PlayBootSequenceAsync();
        BootSequencePlayed = true;
    }

    /// <summary>
    /// Plays boot sequence with terminal effects.
    /// </summary>
    private async Task PlayBootSequenceAsync()
    {
        if (Terminal == null) return;

        var lines = new[]
        {
            "[INITIALIZING GHOST TERMINAL...]",
            "[LOADING ARCHIVES...]",
            "[SYSTEM READY]"
        };

        foreach (var line in lines)
        {
            await TypeTextAsync(line + "\n");
            await Task.Delay(500);
        }
    }

    /// <summary>
    /// Types text to terminal with delay.
    /// </summary>
    private async Task TypeTextAsync(string text)
    {
        if (Terminal == null) return;

        foreach (char c in text)
        {
            Terminal.Call("write", c.ToString());
            await Task.Delay((int) (1000 / DefaultTypingSpeed));
        }
    }

    // INarrativeHandler implementation

    async Task INarrativeHandler.PlayBootSequenceAsync() => await PlayBootSequenceAsync();

    public async Task DisplayLinesAsync(IList<string> lines)
    {
        foreach (var line in lines)
        {
            await TypeTextAsync(line + "\n");
            await Task.Delay(120);
        }
    }

    public Task<bool> HandleCommandLineAsync(string line) => Task.FromResult(false);

    public async Task ApplySceneEffectsAsync(NarrativeScriptElement scene)
    {
        if (scene.Pause.HasValue && scene.Pause.Value > 0)
        {
            await Task.Delay((int) (scene.Pause.Value * 1000));
        }

        // Apply shader effects based on tags
        if (scene.Lines?.Any(l => l.Contains("[GLITCH]")) == true)
        {
            ApplyGlitchEffect();
        }
        else if (scene.Lines?.Any(l => l.Contains("[FADE_TO_STABLE]")) == true)
        {
            FadeToStable();
        }
    }

    private void ApplyGlitchEffect()
    {
        // TODO: Implement shader effect
        GD.Print("Applying glitch effect");
    }

    private void FadeToStable()
    {
        // TODO: Implement fade effect
        GD.Print("Fading to stable");
    }

    public async Task<ChoiceOption> PresentChoiceAsync(string question, string speaker, IList<ChoiceOption> choices)
    {
        await TypeTextAsync(question + "\n");

        if (ChoiceContainer == null) return choices[0]; // Default

        var tcs = new TaskCompletionSource<ChoiceOption>();

        foreach (var choice in choices)
        {
            var button = new Button { Text = choice.Text };
            button.Pressed += () => tcs.TrySetResult(choice);
            ChoiceContainer.AddChild(button);
        }

        ChoiceContainer.Visible = true;
        var selected = await tcs.Task;
        ChoiceContainer.Visible = false;

        foreach (Node child in ChoiceContainer.GetChildren())
        {
            child.QueueFree();
        }

        return selected;
    }

    public Task ProcessChoiceAsync(ChoiceOption selected)
    {
        // Handle choice processing, e.g., update scores
        GD.Print($"Choice selected: {selected.Text}");
        return Task.CompletedTask;
    }

    public Task NotifySequenceCompleteAsync()
    {
        GD.Print("Sequence complete");
        return Task.CompletedTask;
    }
}
