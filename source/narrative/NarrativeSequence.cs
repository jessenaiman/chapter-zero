// <copyright file="NarrativeSequence.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using Godot;
using OmegaSpiral.Source.Scripts;
using OmegaSpiral.Source.Scripts.Common;
using OmegaSpiral.Source.Scripts.Common.screen_transitions;
using OmegaSpiral.Source.Scripts.Field.UI;

namespace OmegaSpiral.Source.Narrative;
/// <summary>
/// Base class for all narrative sequences in the Ghost Terminal stage.
/// Each sequence represents a distinct phase of the narrative presentation (opening, thread choice, story block, etc.).
/// Sequences are orchestrated by GhostTerminalDirector and inherit this class to implement their specific logic.
/// </summary>
[GlobalClass]
public abstract partial class NarrativeSequence : Control
{
    /// <summary>
    /// Emitted when the sequence has completed and is ready to transition to the next sequence.
    /// The parameter is the ID/key for the next sequence to play.
    /// </summary>
    /// <param name="nextSequenceId">The ID/key for the next sequence to play.</param>
    [Signal]
    public delegate void SequenceCompleteEventHandler(string nextSequenceId);

    /// <summary>
    /// Emitted when a user makes a choice or interaction that requires the sequence to handle input.
    /// </summary>
    /// <param name="inputId">The identifier for the input or choice made.</param>
    [Signal]
    public delegate void SequenceInputEventHandler(string inputId);

    /// <summary>
    /// Gets the unique identifier for this sequence.
    /// Used by the director to track sequence progression and routing.
    /// </summary>
    [Export]
    public string SequenceId { get; set; } = "unnamed_sequence";

    /// <summary>
    /// Gets a reference to the screen transition manager (if available).
    /// Sequences can use this to create professional fade/dissolve effects.
    /// </summary>
    protected ScreenTransition? ScreenTransition { get; private set; }

    /// <summary>
    /// Gets a reference to the UIDialogue component (if available).
    /// Sequences can use this for typewriter effects and dialogue presentation.
    /// </summary>
    protected UIDialogue? Dialogue { get; private set; }

    /// <summary>
    /// Gets a reference to the GameState singleton.
    /// Sequences can read player data, thread choice, and other game state.
    /// </summary>
    protected GameState? GameState { get; private set; }

    /// <summary>
    /// Gets a reference to the AnimationPlayer component (if available).
    /// Sequences can use this for visual effects and transitions.
    /// </summary>
    protected AnimationPlayer? AnimationPlayer { get; private set; }

    /// <summary>
    /// Gets a reference to the typewriter audio player (if available).
    /// Sequences can use this for typewriter sound effects.
    /// </summary>
    protected AudioStreamPlayer? TypewriterAudio { get; private set; }

    /// <summary>
    /// Gets a reference to the transition audio player (if available).
    /// Sequences can use this for transition sound effects.
    /// </summary>
    protected AudioStreamPlayer? TransitionAudio { get; private set; }

    /// <summary>
    /// Gets a reference to the DreamweaverSystem singleton.
    /// Sequences can use this for narrative and choice management.
    /// </summary>
    protected DreamweaverSystem? DreamweaverSystem { get; private set; }

    /// <inheritdoc/>
    public override void _Ready()
    {
        this.OnSequenceReady();
    }

    /// <summary>
    /// Called when the sequence is ready. Override this to initialize sequence-specific state.
    /// Base implementation attempts to locate common narrative components.
    /// </summary>
    protected virtual void OnSequenceReady()
    {
        this.ScreenTransition = this.GetNodeOrNull<ScreenTransition>("/root/ScreenTransition");
        this.Dialogue = this.GetNodeOrNull<UIDialogue>("/root/UIDialogue");
        this.GameState = this.GetNodeOrNull<GameState>("/root/GameState");
        this.DreamweaverSystem = this.GetNodeOrNull<DreamweaverSystem>("/root/DreamweaverSystem");
        this.AnimationPlayer = this.GetNodeOrNull<AnimationPlayer>("AnimationPlayer");
        this.TypewriterAudio = this.GetNodeOrNull<AudioStreamPlayer>("TypewriterAudio");
        this.TransitionAudio = this.GetNodeOrNull<AudioStreamPlayer>("TransitionAudio");
    }

    /// <summary>
    /// Plays the sequence asynchronously.
    /// Each sequence type implements this method to define its specific behavior:
    /// - Displaying content (text, choices, etc.)
    /// - Waiting for user input
    /// - Playing animations and transitions
    /// - Emitting SequenceComplete when ready to transition to the next sequence
    ///
    /// Override this method in derived classes to implement sequence-specific logic.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public abstract Task PlayAsync();

    /// <summary>
    /// Emits the SequenceComplete signal to notify the director that this sequence is done.
    /// </summary>
    /// <param name="nextSequenceId">The ID of the next sequence to play. Can be an empty string if no next sequence is defined.</param>
    protected void CompleteSequence(string nextSequenceId = "")
    {
        this.EmitSignal(SignalName.SequenceComplete, nextSequenceId);
    }

    /// <summary>
    /// Emits the SequenceInput signal to notify the director of a user interaction.
    /// </summary>
    /// <param name="inputId">An identifier for the input (e.g., "hero", "shadow", "ambition" for thread choice).</param>
    protected void OnInput(string inputId)
    {
        this.EmitSignal(SignalName.SequenceInput, inputId);
    }

    /// <summary>
    /// Utility: displays text with typewriter effect using the UIDialogue component.
    /// Returns immediately if UIDialogue is not available.
    /// </summary>
    /// <param name="text">The text to display.</param>
    /// <param name="characterDelay">The delay between characters in seconds. Defaults to 0.02 (50 chars/sec).</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    protected async Task DisplayTextWithTypewriterAsync(string text, float characterDelay = 0.02f)
    {
        if (this.Dialogue == null)
        {
            return;
        }

        // Note: This would call UIDialogue's typewriter method if it's exposed.
        // For now, we defer to direct display.
        await Task.Delay((int) (text.Length * characterDelay * 1000)).ConfigureAwait(false);
    }

    /// <summary>
    /// Utility: plays an animation if AnimationPlayer is available.
    /// Returns immediately if AnimationPlayer is not available or animation doesn't exist.
    /// </summary>
    /// <param name="animationName">The name of the animation to play.</param>
    protected void PlayAnimation(string animationName)
    {
        if (this.AnimationPlayer == null || !this.AnimationPlayer.HasAnimation(animationName))
        {
            return;
        }

        this.AnimationPlayer.Play(animationName);
    }

    /// <summary>
    /// Utility: plays typewriter sound effect.
    /// </summary>
    protected void PlayTypewriterSound()
    {
        if (this.TypewriterAudio != null && this.TypewriterAudio.Stream != null)
        {
            this.TypewriterAudio.Play();
        }
    }

    /// <summary>
    /// Utility: plays transition sound effect.
    /// </summary>
    protected void PlayTransitionSound()
    {
        if (this.TransitionAudio != null && this.TransitionAudio.Stream != null)
        {
            this.TransitionAudio.Play();
        }
    }

    /// <summary>
    /// Utility: fades the screen to black.
    /// Returns immediately if ScreenTransition is not available.
    /// </summary>
    /// <param name="duration">Duration of the fade in seconds.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    protected async Task FadeToBlackAsync(float duration = 0.5f)
    {
        if (this.ScreenTransition == null)
        {
            return;
        }

        await this.ScreenTransition.Cover(duration).ConfigureAwait(false);
    }

    /// <summary>
    /// Utility: fades the screen from black back to visible.
    /// Returns immediately if ScreenTransition is not available.
    /// </summary>
    /// <param name="duration">Duration of the fade in seconds.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    protected async Task FadeFromBlackAsync(float duration = 0.5f)
    {
        if (this.ScreenTransition == null)
        {
            return;
        }

        await this.ScreenTransition.Reveal(duration).ConfigureAwait(false);
    }
}
