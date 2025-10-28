// <copyright file="NarrativeUi.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using System;
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

        /// <summary>Gets the typing speed for this beat (characters per second). Default pulled from design config.</summary>
        public float TypingSpeed { get; set; } = -1f; // -1 means use design config default
    }

    /// <summary>
    /// Plays the boot sequence at the start of any narrative UI.
    /// Applies the boot_sequence shader effect and displays boot text.
    /// Can be overridden by subclasses to customize boot behavior.
    /// Called automatically before the main narrative begins.
    /// </summary>
    /// <returns>A task representing the async operation.</returns>
    protected virtual async Task PlayBootSequenceAsync()
    {
        if (ShaderController == null)
        {
            GD.PushWarning("[NarrativeUi] ShaderController not available for boot sequence.");
            return;
        }

        // Try to apply boot sequence shader effect (fail gracefully if preset not found)
        try
        {
            await ShaderController.ApplyVisualPresetAsync("boot_sequence").ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            GD.PrintErr($"[NarrativeUi] Boot sequence shader preset failed (continuing without effect): {ex.Message}");
        }

        // Display default boot text (subclasses can override this method for custom boot sequences)
        await DisplayBootTextAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// Displays the boot sequence text. Override this method in subclasses to provide custom boot sequences.
    /// </summary>
    /// <returns>A task representing the async operation.</returns>
    protected virtual async Task DisplayBootTextAsync()
    {
        await AppendTextAsync("[INITIALIZING NARRATIVE INTERFACE...]", 50f).ConfigureAwait(false);
        await Task.Delay(500).ConfigureAwait(false);
        await AppendTextAsync("[SYSTEM READY]", 50f).ConfigureAwait(false);
        await Task.Delay(300).ConfigureAwait(false);
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
            // Apply visual preset if specified (fail gracefully if not found)
            if (!string.IsNullOrEmpty(beat.VisualPreset) && ShaderController != null)
            {
                try
                {
                    await ShaderController.ApplyVisualPresetAsync(beat.VisualPreset).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    GD.PrintErr($"[NarrativeUi] Visual preset '{beat.VisualPreset}' failed (continuing without effect): {ex.Message}");
                }
            }

            // Wait for delay before displaying text
            if (beat.DelaySeconds > 0)
            {
                await Task.Delay((int)(beat.DelaySeconds * 1000)).ConfigureAwait(false);
            }

            // Display the beat text with typing animation
            if (!string.IsNullOrEmpty(beat.Text))
            {
                // Use configured typing speed if beat doesn't specify one
                float typingSpeed = beat.TypingSpeed > 0 ? beat.TypingSpeed : GetNarrativeTypingSpeed();

                // Add line break if text doesn't end with one
                string textToDisplay = beat.Text;
                if (!textToDisplay.EndsWith("\n", StringComparison.Ordinal))
                {
                    textToDisplay += "\n";
                }

                await AppendTextAsync(textToDisplay, typingSpeed).ConfigureAwait(false);
            }
        }
    }

    /// <summary>
    /// Gets the default typing speed for narrative text from design configuration.
    /// </summary>
    /// <returns>Characters per second for typewriter effect.</returns>
    private static float GetNarrativeTypingSpeed()
    {
        try
        {
            var designConfig = OmegaSpiral.Source.Scripts.Infrastructure.DesignConfigService.DesignConfig;
            if (designConfig.TryGetValue("ui_tokens", out var tokensVariant) &&
                tokensVariant.Obj is Godot.Collections.Dictionary<string, Variant> tokens &&
                tokens.TryGetValue("narrative", out var narrativeVariant) &&
                narrativeVariant.Obj is Godot.Collections.Dictionary<string, Variant> narrative &&
                narrative.TryGetValue("typing_speed", out var speedVariant) &&
                speedVariant.Obj is Godot.Collections.Dictionary<string, Variant> speedDict &&
                speedDict.TryGetValue("value", out var valueVariant))
            {
                return valueVariant.AsSingle();
            }
        }
        catch (Exception ex)
        {
            GD.PrintErr($"[NarrativeUi] Failed to load typing speed from design config: {ex.Message}");
        }

        // Fallback default
        return 15f;
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
        ClearChoiceButtons();
        if (_ChoiceContainer != null)
        {
            _ChoiceContainer.Visible = false;
        }
    }

    /// <summary>
    /// Presents narrative choices and returns the selected option.
    /// Creates buttons directly for consistent Omega styling.
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
        await AppendTextAsync(prompt + "\n").ConfigureAwait(false);

        // Ensure we have a choice container
        if (_ChoiceContainer == null)
        {
            GD.PushError("[NarrativeUi] Choice container not found - cannot present choices.");
            return choices[0];
        }

        // Clear any existing choice buttons
        ClearChoiceButtons();

        // Create task completion source for async selection
        var selectionTaskSource = new TaskCompletionSource<string>();

        // Create choice buttons
        for (int i = 0; i < choices.Length; i++)
        {
            var button = new OmegaUiButton { Text = choices[i], Name = $"ChoiceButton{i}" };
            if (button != null)
            {
                string selectedChoice = choices[i]; // Capture for closure
                button.Pressed += () => OnChoiceSelected(selectedChoice, selectionTaskSource);
                _ChoiceContainer.AddChild(button);
                _ChoiceButtons.Add(button);
            }
        }

        // Show choice container
        _ChoiceContainer.Visible = true;

        // Wait for user selection
        return await selectionTaskSource.Task.ConfigureAwait(false);
    }

    /// <summary>
    /// Handles choice selection by completing the task with the selected choice.
    /// </summary>
    /// <param name="selectedChoice">The text of the selected choice.</param>
    /// <param name="taskSource">The task completion source to complete.</param>
    private void OnChoiceSelected(string selectedChoice, TaskCompletionSource<string> taskSource)
    {
        // Hide choice container after selection
        if (_ChoiceContainer != null)
        {
            _ChoiceContainer.Visible = false;
        }

        // Clear buttons after selection
        ClearChoiceButtons();

        // Complete the task with the selected choice
        taskSource.TrySetResult(selectedChoice);
    }

    /// <summary>
    /// Clears all choice buttons from the container.
    /// </summary>
    private void ClearChoiceButtons()
    {
        foreach (var button in _ChoiceButtons)
        {
            if (_ChoiceContainer != null)
            {
                _ChoiceContainer.RemoveChild(button);
            }
            button.QueueFree();
        }
        _ChoiceButtons.Clear();
    }

    // Cache for choice container to avoid repeated lookups
    private VBoxContainer? _ChoiceContainer;
    private readonly List<Button> _ChoiceButtons = new();
    private bool _BootSequenceCompleted;

    /// <inheritdoc/>
    public override void _Ready()
    {
        base._Ready();

        // Cache ChoiceContainer
        _ChoiceContainer = FindChoiceContainerRecursive(this);

        // Play boot sequence on first ready (deferred to ensure all nodes initialized)
        if (!_BootSequenceCompleted)
        {
            CallDeferred(nameof(PlayBootSequenceDeferred));
        }
    }

    /// <summary>
    /// Deferred boot sequence playback to ensure all components are ready.
    /// </summary>
    private async void PlayBootSequenceDeferred()
    {
        await PlayBootSequenceAsync().ConfigureAwait(false);
        _BootSequenceCompleted = true;
    }

    /// <summary>
    /// Recursively searches for a VBoxContainer named "ChoiceContainer" in the node tree.
    /// </summary>
    private VBoxContainer? FindChoiceContainerRecursive(Node node)
    {
        if (node is VBoxContainer container && node.Name == "ChoiceContainer")
        {
            return container;
        }

        for (int i = 0; i < node.GetChildCount(); i++)
        {
            var found = FindChoiceContainerRecursive(node.GetChild(i));
            if (found != null) return found;
        }

        return null;
    }
}
