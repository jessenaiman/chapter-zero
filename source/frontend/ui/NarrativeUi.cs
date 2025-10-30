// <copyright file="NarrativeUi.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using Godot;
using OmegaSpiral.Source.Ui.Omega;

namespace OmegaSpiral.Source.Narrative;

/// <summary>
/// Simplified Narrative UI extending OmegaContainer for sequential story progression.
/// Handles narrative beats, persona transitions, and choices using Godot signals and coroutines.
/// Base class for narrative stages; implements INarrativeHandler.
/// </summary>
[GlobalClass]
public partial class NarrativeUi : OmegaContainer, INarrativeHandler
{
    [Export] public bool EnableBootSequence { get; set; } = true;
    [Export] public float DefaultTypingSpeed { get; set; } = 15f;
    [Export] public float PersonaTransitionDuration { get; set; } = 3.0f;
    [Export] public string BootSequenceText { get; set; } = "[INITIALIZING NARRATIVE INTERFACE...]\n[SYSTEM READY]";

    [Signal] public delegate void ChoiceSelectedEventHandler(string choice);

    private VBoxContainer? _choiceContainer;
    private bool _bootSequencePlayed;

    /// <inheritdoc/>
    public override void _Ready()
    {
        base._Ready();
        _choiceContainer = GetNodeOrNull<VBoxContainer>("ChoiceContainer");
        TextRenderer = GetNodeOrNull<OmegaTextRenderer>("TextRenderer");
        if (EnableBootSequence && !_bootSequencePlayed)
        {
            CallDeferred(nameof(StartBootSequence));
        }
    }

    /// <summary>
    /// Starts the boot sequence using a coroutine.
    /// </summary>
    private async void StartBootSequence()
    {
        await PlayBootSequenceCoroutine();
        _bootSequencePlayed = true;
    }

    /// <summary>
    /// Plays boot sequence with shader and text display.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    protected virtual async Task PlayBootSequenceCoroutine()
    {
        var lines = BootSequenceText.Split('\n');
        await PlayBootSequenceAsync(lines, 50f, 0.5f);
    }

    /// <summary>
    /// Plays narrative beats sequentially.
    /// </summary>
    /// <param name="beats">The beats to play.</param>
    protected async void PlayNarrativeSequence(NarrativeBeat[] beats)
    {
        await PlayNarrativeBeatsAsync(beats);
    }

    /// <summary>
    /// Transitions persona with shader animation.
    /// </summary>
    /// <param name="threadName">Thread name (light/shadow/ambition).</param>
    protected async void TransitionPersona(string threadName)
    {
        if (string.IsNullOrEmpty(threadName) || ShaderController == null) return;

        Color targetColor = GetThreadColor(threadName);
        var material = ShaderController.GetCurrentShaderMaterial();
        if (material != null)
        {
            await CrossfadePhosphorTint(material, targetColor, PersonaTransitionDuration);
        }
    }

    /// <summary>
    /// Gets thread color.
    /// </summary>
    /// <param name="threadName">Thread name.</param>
    /// <returns>Color.</returns>
    private static Color GetThreadColor(string threadName)
    {
        return threadName.ToLowerInvariant() switch
        {
            "light" => OmegaSpiralColors.LightThread,
            "shadow" => OmegaSpiralColors.ShadowThread,
            "ambition" => OmegaSpiralColors.AmbitionThread,
            _ => OmegaSpiralColors.WarmAmber
        };
    }

    /// <summary>
    /// Crossfades phosphor tint.
    /// </summary>
    /// <param name="material">Shader material.</param>
    /// <param name="targetColor">Target color.</param>
    /// <param name="duration">Duration in seconds.</param>
    private async Task CrossfadePhosphorTint(ShaderMaterial material, Color targetColor, float duration)
    {
        Variant currentVariant = material.GetShaderParameter("phosphor_tint");
        Vector3 currentTint = currentVariant.VariantType == Variant.Type.Vector3 ? currentVariant.AsVector3() : new Vector3(1.0f, 0.9f, 0.5f);
        Vector3 targetTint = new(targetColor.R, targetColor.G, targetColor.B);

        int totalFrames = (int) (duration * 60);
        for (int frame = 0; frame <= totalFrames; frame++)
        {
            float t = (float) frame / totalFrames;
            Vector3 interpolated = currentTint.Lerp(targetTint, t);
            material.SetShaderParameter("phosphor_tint", interpolated);
            await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
        }
        material.SetShaderParameter("phosphor_tint", targetTint);
    }

    /// <summary>
    /// Clears narrative display.
    /// </summary>
    protected void ClearNarrative()
    {
        ClearText();
        if (_choiceContainer != null)
        {
            foreach (Node child in _choiceContainer.GetChildren())
            {
                child.QueueFree();
            }
            _choiceContainer.Visible = false;
        }
    }

    /// <summary>
    /// Presents choices using signals.
    /// </summary>
    /// <param name="prompt">Prompt text.</param>
    /// <param name="choices">Choice texts.</param>
    /// <returns>Selected choice.</returns>
    protected async Task<string> PresentChoices(string prompt, string[] choices)
    {
        if (choices == null || choices.Length == 0 || _choiceContainer == null) return string.Empty;

        await AppendTextAsync(prompt + "\n", DefaultTypingSpeed);

        foreach (var choice in choices)
        {
            var button = new OmegaUiButton { Text = choice };
            button.Pressed += () => EmitSignal(SignalName.ChoiceSelected, choice);
            _choiceContainer.AddChild(button);
        }
        _choiceContainer.Visible = true;

        var result = await ToSignal(this, SignalName.ChoiceSelected);
        ClearNarrative();
        return (string) result[0];
    }

    // INarrativeHandler implementation (simplified)
    async Task INarrativeHandler.PlayBootSequenceAsync() => await PlayBootSequenceCoroutine();

    public async Task DisplayLinesAsync(IList<string> lines)
    {
        foreach (var line in lines)
        {
            await AppendTextAsync(line, DefaultTypingSpeed);
            await ToSignal(GetTree().CreateTimer(0.12f), SceneTreeTimer.SignalName.Timeout);
        }
    }

    public Task<bool> HandleCommandLineAsync(string line) => Task.FromResult(false);

    public async Task ApplySceneEffectsAsync(NarrativeScriptElement scene)
    {
        if (scene.Pause.HasValue && scene.Pause.Value > 0)
        {
            await ToSignal(GetTree().CreateTimer(scene.Pause.Value), SceneTreeTimer.SignalName.Timeout);
        }
    }

    public async Task<ChoiceOption> PresentChoiceAsync(string question, string speaker, IList<ChoiceOption> choices)
    {
        await AppendTextAsync($"{question}\n", DefaultTypingSpeed);
        var container = _choiceContainer ?? new VBoxContainer { Name = "ChoiceContainer" };
        if (container.GetParent() != this) AddChild(container);

        var tcs = new TaskCompletionSource<ChoiceOption>();
        foreach (var choice in choices)
        {
            var button = new Button { Text = choice.Text };
            button.Pressed += () => tcs.TrySetResult(choice);
            container.AddChild(button);
        }
        return await tcs.Task;
    }

    public Task ProcessChoiceAsync(ChoiceOption selected) => Task.CompletedTask;
    public Task NotifySequenceCompleteAsync() => Task.CompletedTask;
}
