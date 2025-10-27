// <copyright file="NarrativeUi.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using System;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using OmegaSpiral.Source.Ui.Omega;

namespace OmegaSpiral.Source.Narrative;

/// <summary>
/// Narrative Ui that extends OmegaThemedContainer and adds sequential story progression.
/// Handles data-driven narrative beats, Dreamweaver persona switching, and linear story flow.
/// Base class for all stages that play as sequential narratives (e.g., Stage 1 Ghost Terminal).
/// </summary>
[GlobalClass]
public partial class NarrativeUi : OmegaThemedContainer
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
    /// Crossfades phosphor tint color to thread-specific colors over 3 seconds.
    /// </summary>
    /// <param name="threadName">The Dreamweaver thread name (light/shadow/ambition).</param>
    /// <param name="transitionDurationSeconds">Duration of the transition effect (default 3.0 seconds per design doc).</param>
    /// <returns>A task representing the async operation.</returns>
    protected async Task TransitionPersonaAsync(string threadName, float transitionDurationSeconds = 3.0f)
    {
        if (string.IsNullOrEmpty(threadName))
        {
            GD.PushWarning("[NarrativeUi] Cannot transition persona - thread name is empty.");
            return;
        }

        if (ShaderController == null)
        {
            GD.PushWarning("[NarrativeUi] Cannot transition persona - ShaderController not initialized.");
            return;
        }

        // Get the thread-specific color from OmegaSpiralColors
        Color targetColor = GetThreadColor(threadName);

        // Get current shader material and animate phosphor_tint parameter
        var shaderMaterial = ShaderController.GetCurrentShaderMaterial();
        if (shaderMaterial != null)
        {
            await CrossfadePhosphorTintAsync(shaderMaterial, targetColor, transitionDurationSeconds).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Gets the color associated with a Dreamweaver thread.
    /// </summary>
    /// <param name="threadName">The thread name (light/shadow/ambition).</param>
    /// <returns>The thread-specific color.</returns>
    private static Color GetThreadColor(string threadName)
    {
        return threadName.ToLowerInvariant() switch
        {
            "light" => OmegaSpiralColors.LightThread,
            "shadow" => OmegaSpiralColors.ShadowThread,
            "ambition" => OmegaSpiralColors.AmbitionThread,
            _ => OmegaSpiralColors.WarmAmber // Default fallback
        };
    }

    /// <summary>
    /// Crossfades the phosphor_tint shader parameter to a target color.
    /// </summary>
    /// <param name="material">The shader material to animate.</param>
    /// <param name="targetColor">The target color to transition to.</param>
    /// <param name="duration">The duration of the crossfade in seconds.</param>
    /// <returns>A task representing the async operation.</returns>
    private static async Task CrossfadePhosphorTintAsync(ShaderMaterial material, Color targetColor, float duration)
    {
        // Get current phosphor_tint or use default warm amber
        Variant currentTintVariant = material.GetShaderParameter("phosphor_tint");
        Vector3 currentTint = currentTintVariant.VariantType == Variant.Type.Vector3
            ? currentTintVariant.AsVector3()
            : new Vector3(1.0f, 0.9f, 0.5f); // Default to warm amber

        Vector3 targetTint = new(targetColor.R, targetColor.G, targetColor.B);

        // Animate the crossfade at 60fps
        const int frameRate = 60;
        int totalFrames = (int)(duration * frameRate);

        for (int frame = 0; frame <= totalFrames; frame++)
        {
            float t = (float)frame / totalFrames;
            Vector3 interpolatedTint = currentTint.Lerp(targetTint, t);
            material.SetShaderParameter("phosphor_tint", interpolatedTint);
            await Task.Delay(1000 / frameRate).ConfigureAwait(false);
        }

        // Ensure we end exactly on target
        material.SetShaderParameter("phosphor_tint", targetTint);
    }

    /// <summary>
    /// Clears the narrative display and resets Ui state for next sequence.
    /// </summary>
    protected void ClearNarrative()
    {
        ClearText();
        if (_ChoiceContainer != null)
        {
            _ChoiceContainer.Visible = false;
        }
    }

    /// <summary>
    /// Presents narrative choices and returns the selected option.
    /// </summary>
    /// <param name="prompt">The prompt text before choices.</param>
    /// <param name="choices">Array of choice texts.</param>
    /// <returns>The text of the selected choice.</returns>
    protected async Task<string> PresentChoicesAsync(string prompt, string[] choices)
    {
        if (choices == null || choices.Length == 0)
        {
            GD.PushWarning("[NarrativeUi] No choices provided to present.");
            return string.Empty;
        }

        // Display the prompt
        await AppendTextAsync(prompt).ConfigureAwait(false);

        // Check if we have a choice presenter
        if (_ChoicePresenter == null && _ChoiceContainer != null)
        {
            _ChoicePresenter = new OmegaChoicePresenter(_ChoiceContainer);
        }

        if (_ChoicePresenter != null)
        {
            // Use the choice presenter to show buttons and wait for selection
            var selectedIndex = await _ChoicePresenter.PresentChoicesAsync(choices.ToList()).ConfigureAwait(false);

            if (selectedIndex.Count > 0 && selectedIndex[0] >= 0 && selectedIndex[0] < choices.Length)
            {
                var selectedChoice = choices[selectedIndex[0]];
                _ChoicePresenter.HideChoices();
                return selectedChoice;
            }
        }

        // Fallback: return first choice if presenter not available
        GD.PushWarning("[NarrativeUi] Choice presenter not available, using first choice as fallback.");
        return choices[0];
    }

    // Cache for choice container to avoid repeated lookups
    private VBoxContainer? _ChoiceContainer;
    private IOmegaChoicePresenter? _ChoicePresenter;

    /// <inheritdoc/>
    protected override void CacheRequiredNodes()
    {
        base.CacheRequiredNodes();
        _ChoiceContainer = GetNodeOrNull<VBoxContainer>("ChoiceContainer");
    }
}
