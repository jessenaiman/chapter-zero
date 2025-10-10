using Godot;
using System;
using System.Collections.Generic;

/// <summary>
/// Centralized music and audio management system.
/// The Music class provides a centralized way to manage and play background music
/// and sound effects throughout the game. It handles audio streaming, volume control,
/// crossfading between tracks, and maintaining a history of played tracks.
/// </summary>
public partial class Music : Node
{
    /// <summary>
    /// Singleton instance of Music
    /// </summary>
    public static Music Instance { get; private set; }

    /// <summary>
    /// The default bus name for music playback.
    /// </summary>
    [Export]
    public string MusicBusName { get; set; } = "Music";

    /// <summary>
    /// The default bus name for sound effects playback.
    /// </summary>
    [Export]
    public string SfxBusName { get; set; } = "SFX";

    /// <summary>
    /// The default volume for music playback.
    /// </summary>
    [Export]
    public float DefaultMusicVolume { get; set; } = 0.0f; // In dB

    /// <summary>
    /// The default volume for sound effects playback.
    /// </summary>
    [Export]
    public float DefaultSfxVolume { get; set; } = 0.0f; // In dB

    /// <summary>
    /// The duration of crossfade between music tracks in seconds.
    /// </summary>
    [Export]
    public float CrossfadeDuration { get; set; } = 2.0f;

    /// <summary>
    /// The currently playing audio stream player.
    /// </summary>
    private AudioStreamPlayer currentMusicPlayer;

    /// <summary>
    /// The next audio stream player for crossfading.
    /// </summary>
    private AudioStreamPlayer nextMusicPlayer;

    /// <summary>
    /// The audio stream player for sound effects.
    /// </summary>
    private AudioStreamPlayer sfxPlayer;

    /// <summary>
    /// The history of played music tracks.
    /// </summary>
    private List<AudioStream> musicHistory = new List<AudioStream>();

    /// <summary>
    /// Timer for crossfade operations.
    /// </summary>
    private Timer crossfadeTimer;

    public override void _Ready()
    {
        Instance = this;

        // Create audio stream players
        currentMusicPlayer = new AudioStreamPlayer();
        currentMusicPlayer.Bus = MusicBusName;
        currentMusicPlayer.VolumeDb = DefaultMusicVolume;
        AddChild(currentMusicPlayer);

        nextMusicPlayer = new AudioStreamPlayer();
        nextMusicPlayer.Bus = MusicBusName;
        nextMusicPlayer.VolumeDb = -80.0f; // Start muted
        AddChild(nextMusicPlayer);

        sfxPlayer = new AudioStreamPlayer();
        sfxPlayer.Bus = SfxBusName;
        sfxPlayer.VolumeDb = DefaultSfxVolume;
        AddChild(sfxPlayer);

        // Create crossfade timer
        crossfadeTimer = new Timer();
        crossfadeTimer.OneShot = true;
        crossfadeTimer.Timeout += OnCrossfadeTimeout;
        AddChild(crossfadeTimer);
    }

    /// <summary>
    /// Play a music track.
    /// </summary>
    public void Play(AudioStream track)
    {
        if (track == null)
        {
            return;
        }

        // Add to history
        musicHistory.Add(track);

        // If no music is currently playing, play immediately
        if (currentMusicPlayer.Stream == null || currentMusicPlayer.Playing == false)
        {
            currentMusicPlayer.Stream = track;
            currentMusicPlayer.Play();
            return;
        }

        // If the same track is already playing, do nothing
        if (currentMusicPlayer.Stream == track)
        {
            return;
        }

        // Crossfade to the new track
        CrossfadeTo(track);
    }

    /// <summary>
    /// Play a sound effect.
    /// </summary>
    public void PlaySfx(AudioStream sfx)
    {
        if (sfx == null || sfxPlayer == null)
        {
            return;
        }

        sfxPlayer.Stream = sfx;
        sfxPlayer.Play();
    }

    /// <summary>
    /// Crossfade from the current track to a new track.
    /// </summary>
    public void CrossfadeTo(AudioStream newTrack)
    {
        if (newTrack == null)
        {
            return;
        }

        // Set up the next player with the new track
        nextMusicPlayer.Stream = newTrack;
        nextMusicPlayer.Play();

        // Start the crossfade
        crossfadeTimer.WaitTime = CrossfadeDuration;
        crossfadeTimer.Start();
    }

    /// <summary>
    /// Stop the currently playing music.
    /// </summary>
    public void Stop()
    {
        currentMusicPlayer.Stop();
        nextMusicPlayer.Stop();
        crossfadeTimer.Stop();
    }

    /// <summary>
    /// Pause the currently playing music.
    /// </summary>
    public void Pause()
    {
        currentMusicPlayer.StreamPaused = true;
        nextMusicPlayer.StreamPaused = true;
    }

    /// <summary>
    /// Resume the currently paused music.
    /// </summary>
    public void Resume()
    {
        currentMusicPlayer.StreamPaused = false;
        nextMusicPlayer.StreamPaused = false;
    }

    /// <summary>
    /// Get the currently playing music track.
    /// </summary>
    public AudioStream GetPlayingTrack()
    {
        return currentMusicPlayer?.Stream;
    }

    /// <summary>
    /// Get the history of played music tracks.
    /// </summary>
    public List<AudioStream> GetMusicHistory()
    {
        return new List<AudioStream>(musicHistory);
    }

    /// <summary>
    /// Clear the music history.
    /// </summary>
    public void ClearMusicHistory()
    {
        musicHistory.Clear();
    }

    /// <summary>
    /// Set the volume for music playback.
    /// </summary>
    public void SetMusicVolume(float volumeDb)
    {
        currentMusicPlayer.VolumeDb = volumeDb;
        nextMusicPlayer.VolumeDb = volumeDb;
    }

    /// <summary>
    /// Set the volume for sound effects playback.
    /// </summary>
    public void SetSfxVolume(float volumeDb)
    {
        sfxPlayer.VolumeDb = volumeDb;
    }

    /// <summary>
    /// Callback when the crossfade timer times out.
    /// </summary>
    private void OnCrossfadeTimeout()
    {
        // Swap players
        var tempPlayer = currentMusicPlayer;
        currentMusicPlayer = nextMusicPlayer;
        nextMusicPlayer = tempPlayer;

        // Mute the old player
        nextMusicPlayer.VolumeDb = -80.0f;
        nextMusicPlayer.Stop();
    }

    /// <summary>
    /// Gradually adjust the volume of the current music player.
    /// </summary>
    public async void FadeVolume(float targetVolume, float duration)
    {
        if (currentMusicPlayer == null)
        {
            return;
        }

        var startVolume = currentMusicPlayer.VolumeDb;
        var elapsed = 0.0f;

        while (elapsed < duration)
        {
            elapsed += GetProcessDeltaTime();
            var t = elapsed / duration;
            currentMusicPlayer.VolumeDb = Mathf.Lerp(startVolume, targetVolume, t);

            // Wait for the next frame
            await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
        }

        currentMusicPlayer.VolumeDb = targetVolume;
    }
}
