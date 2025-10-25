// <copyright file="NarrativeUi.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using System;
using System.Threading.Tasks;
using Godot;
using OmegaSpiral.Source.Ui.Terminal;

namespace OmegaSpiral.Source.Narrative;

/// <summary>
/// Narrative Ui that extends OmegaUi and adds sequential story progression.
/// Handles data-driven narrative beats, Dreamweaver persona switching, and linear story flow.
/// Base class for all stages that play as sequential narratives (e.g., Stage 1 Ghost Terminal).
/// </summary>
[GlobalClass]
public partial class NarrativeUi : OmegaUi
{
    /// <summary>
    /// Represents a narrative beat with associated visual and audio effects.
    /// </summary>
    public class NarrativeBeat
    {
        /// <summary>Gets the display text for this beat.</summary>
        public string Text { get; set; } = string.Empty;

        /// <summary>Gets the optional visual preset to apply during this beat.</summary>
        public string? VisualPreset { get; set; }

        /// <summary>Gets the delay before displaying this beat in seconds.</summary>
        public float DelaySeconds { get; set; }

        /// <summary>Gets the typing speed for this beat (characters per second).</summary>
        public float TypingSpeed { get; set; } = 30f;
    }

    /// <summary>
    /// Plays a sequence of narrative beats in order.
    /// Each beat displays text with optional visual effects and delays.
    /// </summary>
    /// <param name="beats">The beats to play sequentially.</param>
    /// <returns>A task representing the async operation.</returns>
    protected async Task PlayNarrativeSequenceAsync(NarrativeBeat[] beats)
    {
        if (beats == null || beats.Length == 0)
        {
            GD.PushWarning("[NarrativeUi] Cannot play narrative sequence - no beats provided.");
            return;
        }

        foreach (var beat in beats)
        {
            // Apply visual preset if specified
            if (!string.IsNullOrEmpty(beat.VisualPreset) && ShaderController != null)
            {
                await ShaderController.ApplyVisualPresetAsync(beat.VisualPreset).ConfigureAwait(false);
            }

            // Wait for delay before displaying text
            if (beat.DelaySeconds > 0)
            {
                await Task.Delay((int)(beat.DelaySeconds * 1000)).ConfigureAwait(false);
            }

            // Display the beat text with typing animation
            if (!string.IsNullOrEmpty(beat.Text) && TextRenderer != null)
            {
                await AppendTextAsync(beat.Text, beat.TypingSpeed).ConfigureAwait(false);
            }
        }
    }

    /// <summary>
    /// Performs a Dreamweaver persona transition with visual effect.
    /// </summary>
    /// <param name="personaPreset">The visual preset representing the persona.</param>
    /// <param name="transitionDurationSeconds">Duration of the transition effect.</param>
    /// <returns>A task representing the async operation.</returns>
    protected async Task TransitionPersonaAsync(string personaPreset, float transitionDurationSeconds = 0.5f)
    {
        if (string.IsNullOrEmpty(personaPreset))
        {
            GD.PushWarning("[NarrativeUi] Cannot transition persona - preset name is empty.");
            return;
        }

        if (ShaderController == null)
        {
            GD.PushWarning("[NarrativeUi] Cannot transition persona - ShaderController not initialized.");
            return;
        }

        await ShaderController.ApplyVisualPresetAsync(personaPreset).ConfigureAwait(false);
        await Task.Delay((int)(transitionDurationSeconds * 1000)).ConfigureAwait(false);
    }

    /// <summary>
    /// Clears the narrative display and resets Ui state for next sequence.
    /// </summary>
    protected void ClearNarrative()
    {
        ClearText();
        if (ChoicePresenter != null && _choiceContainer != null)
        {
            _choiceContainer.Visible = false;
        }
    }

    /// <summary>
    /// Presents narrative choices and returns the selected option.
    /// Wrapper around base PresentChoicesAsync for narrative-specific flow.
    /// </summary>
    /// <param name="prompt">The prompt text before choices.</param>
    /// <param name="choices">Array of choice texts.</param>
    /// <returns>The text of the selected choice.</returns>
    protected new async Task<string> PresentChoicesAsync(string prompt, string[] choices)
    {
        return await base.PresentChoicesAsync(prompt, choices).ConfigureAwait(false);
    }

    // Cache for choice container to avoid repeated lookups
    private VBoxContainer? _choiceContainer;

    /// <inheritdoc/>
    protected override void CacheRequiredNodes()
    {
        base.CacheRequiredNodes();
        _choiceContainer = GetNodeOrNull<VBoxContainer>("ChoiceContainer");
    }
}
