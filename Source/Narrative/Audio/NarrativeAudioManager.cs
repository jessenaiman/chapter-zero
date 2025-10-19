// <copyright file="NarrativeAudioManager.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using Godot;
using System;

namespace OmegaSpiral.Source.Narrative.Audio;
/// <summary>
/// Manages audio resources for narrative sequences including typewriter effects and sound effects.
/// Provides centralized audio loading, playback, and management for the Ghost Terminal narrative system.
/// </summary>
[GlobalClass]
public partial class NarrativeAudioManager : Node
{
    /// <summary>
    /// Audio stream player for typewriter sound effects.
    /// </summary>
    private AudioStreamPlayer? typewriterPlayer;

    /// <summary>
    /// Audio stream player for selection confirmation sounds.
    /// </summary>
    private AudioStreamPlayer? selectionPlayer;

    /// <summary>
    /// Audio stream player for transition sounds.
    /// </summary>
    private AudioStreamPlayer? transitionPlayer;

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
        this.InitializeAudioPlayers();
        this.LoadAudioAssets();
        this.IsInitialized = true;
    }

    /// <summary>
    /// Initializes the audio stream players for the narrative system.
    /// </summary>
    private void InitializeAudioPlayers()
    {
        // Create typewriter audio player
        this.typewriterPlayer = new AudioStreamPlayer();
        this.typewriterPlayer.Bus = "SFX";
        this.typewriterPlayer.VolumeDb = -5.0f; // Slightly quieter for typewriter
        this.AddChild(this.typewriterPlayer);

        // Create selection audio player
        this.selectionPlayer = new AudioStreamPlayer();
        this.selectionPlayer.Bus = "SFX";
        this.selectionPlayer.VolumeDb = 0.0f;
        this.AddChild(this.selectionPlayer);

        // Create transition audio player
        this.transitionPlayer = new AudioStreamPlayer();
        this.transitionPlayer.Bus = "SFX";
        this.transitionPlayer.VolumeDb = -3.0f;
        this.AddChild(this.transitionPlayer);
    }

    /// <summary>
    /// Loads audio assets for the narrative system.
    /// </summary>
    private void LoadAudioAssets()
    {
        try
        {
            // Load typewriter sound effects
            this.typewriterSfx = ResourceLoader.Load<AudioStream>("res://Source/assets/sfx/confirmation_002.ogg");

            // Load selection confirmation sound
            this.selectionSfx = ResourceLoader.Load<AudioStream>("res://Source/assets/sfx/drop_002.ogg");

            // Load transition sound.
            this.transitionSfx = ResourceLoader.Load<AudioStream>("res://Source/assets/sfx/impactWood_light_002.ogg");
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
        if (!this.IsInitialized || this.typewriterPlayer == null || this.typewriterSfx == null)
        {
            return;
        }

        this.typewriterPlayer.Stream = this.typewriterSfx;
        this.typewriterPlayer.Play();
    }

    /// <summary>
    /// Plays selection confirmation sound.
    /// </summary>
    public void PlaySelectionSound()
    {
        if (!this.IsInitialized || this.selectionPlayer == null || this.selectionSfx == null)
        {
            return;
        }

        this.selectionPlayer.Stream = this.selectionSfx;
        this.selectionPlayer.Play();
    }

    /// <summary>
    /// Plays transition sound effect.
    /// </summary>
    public void PlayTransitionSound()
    {
        if (!this.IsInitialized || this.transitionPlayer == null || this.transitionSfx == null)
        {
            return;
        }

        this.transitionPlayer.Stream = this.transitionSfx;
        this.transitionPlayer.Play();
    }

    /// <summary>
    /// Plays a custom sound effect.
    /// </summary>
    /// <param name="audioStream">The audio stream to play.</param>
    /// <param name="playerType">The type of player to use (typewriter, selection, or transition).</param>
    public void PlayCustomSound(AudioStream audioStream, AudioPlayerType playerType = AudioPlayerType.Typewriter)
    {
        if (!this.IsInitialized || audioStream == null)
        {
            return;
        }

        var player = playerType switch
        {
            AudioPlayerType.Selection => this.selectionPlayer,
            AudioPlayerType.Transition => this.transitionPlayer,
            _ => this.typewriterPlayer,
        };

        if (player != null)
        {
            player.Stream = audioStream;
            player.Play();
        }
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
