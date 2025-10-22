// <copyright file="NarrativeAudioManager.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using Godot;

namespace OmegaSpiral.Source.Narrative.Audio;
/// <summary>
/// Manages audio resources for narrative sequences including typewriter effects and sound effects.
/// Provides centralized audio loading, playback, and management for the Ghost Terminal narrative system.
/// </summary>
[GlobalClass]
public partial class NarrativeAudioManager : Node
{
    /// <summary>
    /// Reference to the centralized AudioManager.
    /// </summary>
    private AudioManager? audioManager;

    /// <summary>
    /// Audio stream for typewriter sound effect.
    /// </summary>
    private AudioStream? typewriterSfx;

    /// <summary>
    /// Audio stream for selection confirmation sound.
    /// </summary>
    private AudioStream? selectionSfx;

    /// <summary>
    /// Audio stream for transition sounds.
    /// </summary>
    private AudioStream? transitionSfx;

    /// <summary>
    /// Gets a value indicating whether the audio manager has been initialized.
    /// </summary>
    public bool IsInitialized { get; private set; }

    /// <inheritdoc/>
    public override void _Ready()
    {
        // Get reference to centralized AudioManager
        this.audioManager = GetNode<AudioManager>("/root/AudioManager");
        this.LoadAudioAssets();
        this.IsInitialized = true;
        GD.Print("NarrativeAudioManager: Initialized with centralized AudioManager");
    }



    /// <summary>
    /// Loads audio assets for the narrative system.
    /// </summary>
    private void LoadAudioAssets()
    {
        try
        {
            // Load typewriter sound effects
            this.typewriterSfx = ResourceLoader.Load<AudioStream>("res://source/assets/sfx/confirmation_002.ogg");

            // Load selection confirmation sound
            this.selectionSfx = ResourceLoader.Load<AudioStream>("res://source/assets/sfx/drop_002.ogg");

            // Load transition sound.
            this.transitionSfx = ResourceLoader.Load<AudioStream>("res://source/assets/sfx/impactWood_light_002.ogg");
        }
        catch (Exception ex)
        {
            GD.PrintErr($"NarrativeAudioManager: Error loading audio assets: {ex.Message}");
        }
    }

    /// <summary>
    /// Plays typewriter sound effect.
    /// </summary>
    public void PlayTypewriterSound()
    {
        if (!this.IsInitialized || this.audioManager == null || this.typewriterSfx == null)
        {
            return;
        }

        this.audioManager.PlayOneShot(this.typewriterSfx, AudioCategory.Sfx, -5.0f);
    }

    /// <summary>
    /// Plays selection confirmation sound.
    /// </summary>
    public void PlaySelectionSound()
    {
        if (!this.IsInitialized || this.audioManager == null || this.selectionSfx == null)
        {
            return;
        }

        this.audioManager.PlayOneShot(this.selectionSfx, AudioCategory.Sfx, 0.0f);
    }

    /// <summary>
    /// Plays transition sound effect.
    /// </summary>
    public void PlayTransitionSound()
    {
        if (!this.IsInitialized || this.audioManager == null || this.transitionSfx == null)
        {
            return;
        }

        this.audioManager.PlayOneShot(this.transitionSfx, AudioCategory.Sfx, -3.0f);
    }

    /// <summary>
    /// Plays a custom sound effect.
    /// </summary>
    /// <param name="audioStream">The audio stream to play.</param>
    /// <param name="playerType">The type of player to use (affects volume adjustment).</param>
    public void PlayCustomSound(AudioStream audioStream, AudioPlayerType playerType = AudioPlayerType.Typewriter)
    {
        if (!this.IsInitialized || this.audioManager == null || audioStream == null)
        {
            return;
        }

        // Map player type to volume adjustment
        var volumeDb = playerType switch
        {
            AudioPlayerType.Selection => 0.0f,
            AudioPlayerType.Transition => -3.0f,
            _ => -5.0f, // Typewriter
        };

        this.audioManager.PlayOneShot(audioStream, AudioCategory.Sfx, volumeDb);
    }

    /// <summary>
    /// Gets the typewriter audio stream.
    /// </summary>
    /// <returns>The typewriter audio stream, or null if not loaded.</returns>
    public AudioStream? GetTypewriterSfx()
    {
        return this.typewriterSfx;
    }

    /// <summary>
    /// Gets the selection audio stream.
    /// </summary>
    /// <returns>The selection audio stream, or null if not loaded.</returns>
    public AudioStream? GetSelectionSfx()
    {
        return this.selectionSfx;
    }

    /// <summary>
    /// Gets the transition audio stream.
    /// </summary>
    /// <returns>The transition audio stream, or null if not loaded.</returns>
    public AudioStream? GetTransitionSfx()
    {
        return this.transitionSfx;
    }
}

/// <summary>
/// Enumerates the types of audio players available in the narrative system.
/// </summary>
public enum AudioPlayerType
{
    /// <summary>
    /// Typewriter sound effect player.
    /// </summary>
    Typewriter,

    /// <summary>
    /// Selection confirmation sound player.
    /// </summary>
    Selection,

    /// <summary>
    /// Transition sound effect player.
    /// </summary>
    Transition,
}
