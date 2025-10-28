// <copyright file="BaseNarrativeUi.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;

namespace OmegaSpiral.Source.Narrative;

/// <summary>
/// Reusable base class for narrative UI implementations across all stages.
/// Contains all generic narrative rendering logic (displaying lines, handling commands,
/// applying effects, presenting choices, pauses, etc.).
///
/// Stage-specific UI classes inherit from this and override only the truly unique parts:
/// node hierarchy, shader effects, signals, and stage-specific plan types.
/// </summary>
public abstract partial class BaseNarrativeUi : Control, INarrativeHandler
{
    /// <summary>
    /// Holds the selected choice when presenting a question.
    /// </summary>
    protected TaskCompletionSource<ChoiceOption> ChoiceTcs = null!;

    /// <summary>
    /// Holds completion state for a single line display.
    /// </summary>
    protected TaskCompletionSource<bool>? LineTcs;

    /// <summary>
    /// Gets or creates the container where narrative content is added.
    /// Derived classes must implement this to return their stage-specific stack container.
    /// </summary>
    protected abstract VBoxContainer GetNarrativeStack();

    /// <summary>
    /// Presents a single line as a Label and returns when complete.
    /// This is the only truly UI-rendering method that stages might customize.
    /// </summary>
    protected virtual Task EnqueueLineAsync(string line)
    {
        LineTcs = new TaskCompletionSource<bool>();

        var stack = GetNarrativeStack();
        if (stack != null)
        {
            var label = new Label { Text = line ?? string.Empty };
            stack.AddChild(label);
            LineTcs.TrySetResult(true);
        }
        else
        {
            LineTcs.TrySetResult(true);
        }

        return LineTcs.Task;
    }

    /// <summary>
    /// Presents a question and list of choices as a Label + Buttons.
    /// Returns the selected ChoiceOption.
    /// </summary>
    protected virtual async Task<ChoiceOption> PresentChoicesAsync(
        string question,
        List<ChoiceOption> choices)
    {
        ChoiceTcs = new TaskCompletionSource<ChoiceOption>();

        var stack = GetNarrativeStack();
        if (stack != null)
        {
            var questionLabel = new Label { Text = question ?? string.Empty };
            stack.AddChild(questionLabel);

            var choicesContainer = new VBoxContainer();
            stack.AddChild(choicesContainer);

            for (int i = 0; i < choices.Count; i++)
            {
                var choice = choices[i];
                var btn = new Button { Text = choice.Text ?? $"Choice {i + 1}" };
                var index = i;

                btn.Pressed += () =>
                {
                    OnChoiceSelected(choice, index);
                    ChoiceTcs.TrySetResult(choice);
                };

                choicesContainer.AddChild(btn);
            }
        }
        else
        {
            // Fallback: if UI is not available, automatically select the first choice.
            ChoiceTcs.TrySetResult(choices.Count > 0 ? choices[0] : new ChoiceOption { Text = string.Empty });
        }

        return await ChoiceTcs.Task;
    }

    /// <summary>
    /// Called when a choice button is pressed. Override in derived classes to emit signals or handle stage-specific logic.
    /// </summary>
    /// <param name="choice">The selected ChoiceOption.</param>
    /// <param name="index">The index of the selected choice.</param>
    protected virtual void OnChoiceSelected(ChoiceOption choice, int index)
    {
        // Default: do nothing. Stages can override to emit signals or custom handlers.
    }

    /// <summary>
    /// Applies a CRT preset or other stage-specific visual effect.
    /// Derived classes should override to apply their own shader presets or effects.
    /// </summary>
    /// <param name="presetName">Name of the preset to apply.</param>
    protected virtual void ApplyCrtPreset(string presetName)
    {
        // Default: try to call a generic ShaderController node if it exists.
        var shaderCtrl = GetNodeOrNull("ShaderController");
        shaderCtrl?.Call("ApplyPreset", presetName);
    }

    // =====================================================================
    // INarrativeHandler implementation – all generic, no stage-specific logic
    // =====================================================================

    /// <inheritdoc/>
    public virtual Task PlayBootSequenceAsync()
    {
        // Default: no boot sequence. Stages can override if needed.
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public virtual async Task DisplayLinesAsync(IList<string> lines)
    {
        foreach (var line in lines)
        {
            await EnqueueLineAsync(line).ConfigureAwait(false);
            // Small pause between lines for readability
            await Task.Delay(120).ConfigureAwait(false);
        }
    }

    /// <inheritdoc/>
    public virtual Task<bool> HandleCommandLineAsync(string line)
    {
        // Default: recognize common CRT commands. Stages can override for custom commands.
        if (line.StartsWith("[FADE_TO_STABLE]"))
        {
            ApplyCrtPreset("stable_baseline");
            return Task.FromResult(true);
        }
        if (line.StartsWith("[GLITCH]"))
        {
            ApplyCrtPreset("glitch_spike");
            return Task.FromResult(true);
        }
        return Task.FromResult(false);
    }

    /// <inheritdoc/>
    public virtual Task ApplySceneEffectsAsync(NarrativeScene scene)
    {
        // Respect optional pause field.
        if (scene.Pause.HasValue && scene.Pause.Value > 0)
        {
            return Task.Delay(TimeSpan.FromSeconds(scene.Pause.Value));
        }
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public virtual async Task<ChoiceOption> PresentChoiceAsync(
        string question,
        string speaker,
        IList<ChoiceOption> choices)
    {
        var choiceList = new List<ChoiceOption>(choices);
        return await PresentChoicesAsync(question, choiceList).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public virtual Task ProcessChoiceAsync(ChoiceOption selected)
    {
        // Default: do nothing. Stages can override to update game state, Dreamweaver scoring, etc.
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public virtual Task NotifySequenceCompleteAsync()
    {
        // Default: do nothing. Stages can override to emit signals or clean up.
        return Task.CompletedTask;
    }
}
