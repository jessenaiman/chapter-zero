// <copyright file="OmegaSpiralIntroMechanics.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using Godot;
using OmegaSpiral.Source.Design;

namespace OmegaSpiral.Source.Narrative.IntroMechanics;
/// <summary>
/// Implements engaging intro mechanics for the Game Ωmega Spiral opening scene.
/// Provides interactive elements, visual effects, and narrative hooks to draw players into the experience.
/// </summary>
[GlobalClass]
public partial class OmegaSpiralIntroMechanics : Node
{
    /// <summary>
    /// Timer for controlling the intro sequence timing.
    /// </summary>
    private Godot.Timer? sequenceTimer;

    /// <summary>
    /// Animation player for visual effects.
    /// </summary>
    private AnimationPlayer? animationPlayer;

    /// <summary>
    /// Rich text label for displaying narrative text.
    /// </summary>
    private RichTextLabel? narrativeLabel;

    /// <summary>
    /// Control node for the terminal interface.
    /// </summary>
    private Control? terminalInterface;

    /// <summary>
    /// List of interactive narrative elements.
    /// </summary>
    private readonly List<IntroNarrativeElement> narrativeElements = new();

    /// <summary>
    /// Current step in the intro sequence.
    /// </summary>
    private int currentStep;

    /// <summary>
    /// Gets a value indicating whether the intro mechanics are currently active.
    /// </summary>
    public bool IsActive { get; private set; }

    /// <inheritdoc/>
    public override void _Ready()
    {
        this.InitializeIntroMechanics();
    }

    /// <summary>
    /// Initializes the intro mechanics system.
    /// </summary>
    private void InitializeIntroMechanics()
    {
        // Create sequence timer
        this.sequenceTimer = new Godot.Timer();
        this.sequenceTimer.OneShot = false;
        this.AddChild(this.sequenceTimer);

        // Find required nodes
        this.narrativeLabel = this.GetNodeOrNull<RichTextLabel>("NarrativeLabel");
        this.terminalInterface = this.GetNodeOrNull<Control>("TerminalInterface");
        this.animationPlayer = this.GetNodeOrNull<AnimationPlayer>("AnimationPlayer");

        // Initialize narrative elements
        this.InitializeNarrativeElements();

        GD.Print("OmegaSpiralIntroMechanics: Initialized");
    }

    /// <summary>
    /// Initializes the narrative elements for the intro sequence.
    /// </summary>
    private void InitializeNarrativeElements()
    {
        // Core narrative elements for Ωmega Spiral
        this.narrativeElements.Add(new IntroNarrativeElement
        {
            Text = $"[center][color={DesignService.GetColor("intro_white").ToHtml()}]ΩMEGA SPIRAL[/color][/center]",
            Duration = 2.0f,
            EffectType = NarrativeEffectType.FadeIn,
            AudioCue = "res://source/assets/sfx/confirmation_002.ogg"
        });

        this.narrativeElements.Add(new IntroNarrativeElement
        {
            Text = $"[center][color={DesignService.GetColor("intro_gray_light").ToHtml()}]A Revolutionary Narrative Experience[/color][/center]",
            Duration = 1.5f,
            EffectType = NarrativeEffectType.Typewriter,
            AudioCue = "res://source/assets/sfx/drop_002.ogg"
        });

        this.narrativeElements.Add(new IntroNarrativeElement
        {
            Text = $"[center][color={DesignService.GetColor("intro_gray_medium").ToHtml()}]Five Eras of Gaming Aesthetics Await[/color][/center]",
            Duration = 1.5f,
            EffectType = NarrativeEffectType.Glow,
            AudioCue = "res://source/assets/sfx/impactWood_light_002.ogg"
        });

        this.narrativeElements.Add(new IntroNarrativeElement
        {
            Text = $"[center][color={DesignService.GetColor("intro_gray_dark").ToHtml()}]Dynamic AI-Driven Dreamweavers Adapt to Your Choices[/color][/center]",
            Duration = 2.0f,
            EffectType = NarrativeEffectType.Pulse,
            AudioCue = "res://source/assets/sfx/chop.ogg"
        });

        this.narrativeElements.Add(new IntroNarrativeElement
        {
            Text = $"[center][color={DesignService.GetColor("intro_gray_darker").ToHtml()}]The Journey Begins...[/color][/center]",
            Duration = 1.0f,
            EffectType = NarrativeEffectType.FadeOut,
            AudioCue = "res://source/assets/sfx/error_006.ogg"
        });
    }

    /// <summary>
    /// Starts the intro sequence.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public async Task StartIntroSequenceAsync()
    {
        if (this.IsActive)
        {
            return; // Already running
        }

        this.IsActive = true;
        this.currentStep = 0;

        GD.Print("OmegaSpiralIntroMechanics: Starting intro sequence");

        while (this.currentStep < this.narrativeElements.Count && this.IsActive)
        {
            await this.DisplayNarrativeElementAsync(this.narrativeElements[this.currentStep]).ConfigureAwait(false);
            this.currentStep++;
        }

        GD.Print("OmegaSpiralIntroMechanics: Intro sequence completed");
        this.IsActive = false;
    }

    /// <summary>
    /// Displays a single narrative element with its associated effects.
    /// </summary>
    /// <param name="element">The narrative element to display.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    private async Task DisplayNarrativeElementAsync(IntroNarrativeElement element)
    {
        if (this.narrativeLabel == null)
        {
            return;
        }

        // Apply the narrative effect
        await this.ApplyNarrativeEffectAsync(element).ConfigureAwait(false);

        // Wait for the specified duration
        if (element.Duration > 0)
        {
            await Task.Delay((int) (element.Duration * 1000)).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Applies the specified narrative effect to the current element.
    /// </summary>
    /// <param name="element">The narrative element with effect information.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    private async Task ApplyNarrativeEffectAsync(IntroNarrativeElement element)
    {
        if (this.narrativeLabel == null)
        {
            return;
        }

        // Play associated audio cue
        this.PlayAudioCue(element.AudioCue);

        switch (element.EffectType)
        {
            case NarrativeEffectType.FadeIn:
                await this.ApplyFadeInEffectAsync(element.Text).ConfigureAwait(false);
                break;
            case NarrativeEffectType.Typewriter:
                await this.ApplyTypewriterEffectAsync(element.Text).ConfigureAwait(false);
                break;
            case NarrativeEffectType.Glow:
                await this.ApplyGlowEffectAsync(element.Text).ConfigureAwait(false);
                break;
            case NarrativeEffectType.Pulse:
                await this.ApplyPulseEffectAsync(element.Text).ConfigureAwait(false);
                break;
            case NarrativeEffectType.FadeOut:
                await this.ApplyFadeOutEffectAsync(element.Text).ConfigureAwait(false);
                break;
            default:
                this.narrativeLabel.Text = element.Text;
                break;
        }
    }

    /// <summary>
    /// Applies a fade-in effect to the text.
    /// </summary>
    /// <param name="text">The text to display with fade-in effect.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    private async Task ApplyFadeInEffectAsync(string text)
    {
        if (this.narrativeLabel == null)
        {
            return;
        }

        // Start with transparent text
        this.narrativeLabel.Text = text;
        this.narrativeLabel.Modulate = new Color(1, 1, 1, 0);

        // Create fade-in animation
        var tween = this.CreateTween();
        tween.TweenProperty(this.narrativeLabel, "modulate:a", 1.0f, 1.5f)
              .SetTrans(Tween.TransitionType.Sine)
              .SetEase(Tween.EaseType.Out);

        await this.ToSignal(tween, Tween.SignalName.Finished);
    }

    /// <summary>
    /// Applies a typewriter effect to the text.
    /// </summary>
    /// <param name="text">The text to display with typewriter effect.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    private async Task ApplyTypewriterEffectAsync(string text)
    {
        if (this.narrativeLabel == null)
        {
            return;
        }

        // Clear existing text
        this.narrativeLabel.Text = string.Empty;

        // Display text character by character with audio feedback
        var charDelay = 0.05f;
        var timer = this.GetTree().CreateTimer(charDelay);

        foreach (char character in text)
        {
            this.narrativeLabel.Text += character.ToString();

            // Play typewriter sound
            this.PlayAudioCue("res://source/assets/sfx/confirmation_002.ogg");

            await this.ToSignal(timer, Godot.Timer.SignalName.Timeout);
            timer = this.GetTree().CreateTimer(charDelay);
        }
    }

    /// <summary>
    /// Applies a glow effect to the text.
    /// </summary>
    /// <param name="text">The text to display with glow effect.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    private async Task ApplyGlowEffectAsync(string text)
    {
        if (this.narrativeLabel == null)
        {
            return;
        }

        this.narrativeLabel.Text = text;

        // Create glow animation using color cycling
        var tween = this.CreateTween();
        tween.SetParallel(true);

        // Cycle through different glow colors
        tween.TweenProperty(this.narrativeLabel, "modulate:r", 0.8f, 0.5f)
              .SetTrans(Tween.TransitionType.Sine)
              .SetEase(Tween.EaseType.InOut);
        tween.TweenProperty(this.narrativeLabel, "modulate:g", 0.6f, 0.5f)
              .SetTrans(Tween.TransitionType.Sine)
              .SetEase(Tween.EaseType.InOut);
        tween.TweenProperty(this.narrativeLabel, "modulate:b", 1.0f, 0.5f)
              .SetTrans(Tween.TransitionType.Sine)
              .SetEase(Tween.EaseType.InOut);

        await this.ToSignal(tween, Tween.SignalName.Finished);
    }

    /// <summary>
    /// Applies a pulse effect to the text.
    /// </summary>
    /// <param name="text">The text to display with pulse effect.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    private async Task ApplyPulseEffectAsync(string text)
    {
        if (this.narrativeLabel == null)
        {
            return;
        }

        this.narrativeLabel.Text = text;

        // Create pulse animation
        var tween = this.CreateTween();
        tween.SetParallel(true);

        // Scale pulsing effect
        tween.TweenProperty(this.narrativeLabel, "scale", new Vector2(1.1f, 1.1f), 0.3f)
              .SetTrans(Tween.TransitionType.Elastic)
              .SetEase(Tween.EaseType.Out);
        tween.TweenProperty(this.narrativeLabel, "scale", new Vector2(1.0f, 1.0f), 0.3f)
              .SetTrans(Tween.TransitionType.Elastic)
              .SetEase(Tween.EaseType.Out);

        await this.ToSignal(tween, Tween.SignalName.Finished);
    }

    /// <summary>
    /// Applies a fade-out effect to the text.
    /// </summary>
    /// <param name="text">The text to display with fade-out effect.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    private async Task ApplyFadeOutEffectAsync(string text)
    {
        if (this.narrativeLabel == null)
        {
            return;
        }

        this.narrativeLabel.Text = text;

        // Create fade-out animation
        var tween = this.CreateTween();
        tween.TweenProperty(this.narrativeLabel, "modulate:a", 0.0f, 1.0f)
              .SetTrans(Tween.TransitionType.Sine)
              .SetEase(Tween.EaseType.In);

        await this.ToSignal(tween, Tween.SignalName.Finished);
    }

    /// <summary>
    /// Plays an audio cue.
    /// </summary>
    /// <param name="audioPath">The path to the audio file to play.</param>
    private void PlayAudioCue(string? audioPath)
    {
        if (string.IsNullOrEmpty(audioPath))
        {
            return;
        }

        try
        {
            var audioStream = ResourceLoader.Load<AudioStream>(audioPath);
            if (audioStream != null)
            {
                // Create temporary audio player for the cue
                var audioPlayer = new AudioStreamPlayer();
                audioPlayer.Stream = audioStream;
                audioPlayer.Bus = "SFX";
                this.AddChild(audioPlayer);

                audioPlayer.Play();

                // Use async helper method to handle audio player cleanup
                this.HandleAudioPlayerCleanup(audioPlayer);
            }
        }
        catch (Exception ex)
        {
            GD.PrintErr($"Failed to play audio cue '{audioPath}': {ex.Message}");
        }
    }

    /// <summary>
    /// Cancels the current intro sequence.
    /// </summary>
    public void CancelIntro()
    {
        this.IsActive = false;
        this.currentStep = this.narrativeElements.Count; // Stop the sequence
    }
    /// <summary>
    /// Handles audio player cleanup after playback finishes.
    /// </summary>
    /// <param name="audioPlayer">The audio player to clean up.</param>
    private async void HandleAudioPlayerCleanup(AudioStreamPlayer audioPlayer)
    {
        await this.ToSignal(audioPlayer, AudioStreamPlayer.SignalName.Finished);

        if (audioPlayer != null && audioPlayer.IsNodeReady())
        {
            audioPlayer.QueueFree();
        }
    }

    /// <summary>
    /// Gets the total duration of the intro sequence.
    /// </summary>
    /// <returns>The total duration in seconds.</returns>
    public float GetTotalDuration()
    {
        float total = 0;
        foreach (var element in this.narrativeElements)
        {
            total += element.Duration;
        }
        return total;
    }
}

/// <summary>
/// Represents a single narrative element in the intro sequence.
/// </summary>
public class IntroNarrativeElement
{
    /// <summary>
    /// Gets or sets the text content of the narrative element.
    /// </summary>
    public string Text { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the duration to display this element (in seconds).
    /// </summary>
    public float Duration { get; set; }

    /// <summary>
    /// Gets or sets the type of visual effect to apply.
    /// </summary>
    public NarrativeEffectType EffectType { get; set; }

    /// <summary>
    /// Gets or sets the audio cue to play with this element.
    /// </summary>
    public string? AudioCue { get; set; }
}

/// <summary>
/// Enumerates the types of narrative effects available.
/// </summary>
public enum NarrativeEffectType
{
    /// <summary>
    /// No special effect, just display the text.
    /// </summary>
    None,

    /// <summary>
    /// Fade in the text from transparent to opaque.
    /// </summary>
    FadeIn,

    /// <summary>
    /// Type out the text character by character like a typewriter.
    /// </summary>
    Typewriter,

    /// <summary>
    /// Apply a glowing color effect to the text.
    /// </summary>
    Glow,

    /// <summary>
    /// Apply a pulsing scale effect to the text.
    /// </summary>
    Pulse,

    /// <summary>
    /// Fade out the text from opaque to transparent.
    /// </summary>
    FadeOut,
}
