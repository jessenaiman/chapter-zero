// <copyright file="Music.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

namespace OmegaSpiral.Source.Scripts
{
    using System.Collections.Generic;
    using Godot;

    /// <summary>
    /// Centralized music and audio management system.
    /// The Music class provides a centralized way to manage and play background music
    /// and sound effects throughout the game. It handles audio streaming, volume control,
    /// crossfading between tracks, and maintaining a history of played tracks.
    /// </summary>
    public partial class Music : Node
    {
        /// <summary>
        /// Gets singleton instance of Music.
        /// </summary>
        public static Music Instance { get; private set; } = null!;

        /// <summary>
        /// Gets or sets the default bus name for music playback.
        /// </summary>
        [Export]
        public string MusicBusName { get; set; } = "Music";

        /// <summary>
        /// Gets or sets the default bus name for sound effects playback.
        /// </summary>
        [Export]
        public string SfxBusName { get; set; } = "SFX";

        /// <summary>
        /// Gets or sets the default volume for music playback.
        /// </summary>
        [Export]
        public float DefaultMusicVolume { get; set; } = 0.0f; // In dB

        /// <summary>
        /// Gets or sets the default volume for sound effects playback.
        /// </summary>
        [Export]
        public float DefaultSfxVolume { get; set; } = 0.0f; // In dB

        /// <summary>
        /// Gets or sets the duration of crossfade between music tracks in seconds.
        /// </summary>
        [Export]
        public float CrossfadeDuration { get; set; } = 2.0f;

        /// <summary>
        /// The currently playing audio stream player.
        /// </summary>
        private AudioStreamPlayer? currentMusicPlayer;

        /// <summary>
        /// The next audio stream player for crossfading.
        /// </summary>
        private AudioStreamPlayer? nextMusicPlayer;

        /// <summary>
        /// The audio stream player for sound effects.
        /// </summary>
        private AudioStreamPlayer? sfxPlayer;

        /// <summary>
        /// The history of played music tracks.
        /// </summary>
        private List<AudioStream> musicHistory = new List<AudioStream>();

        /// <summary>
        /// Timer for crossfade operations.
        /// </summary>
        private Godot.Timer? crossfadeTimer;

        /// <summary>
        /// Called when the node enters the scene tree for the first time.
        /// </summary>
        public override void _Ready()
        {
            Instance = this;

            // Create audio stream players
            this.currentMusicPlayer = new AudioStreamPlayer();
            this.currentMusicPlayer.Bus = this.MusicBusName;
            this.currentMusicPlayer.VolumeDb = this.DefaultMusicVolume;
            this.AddChild(this.currentMusicPlayer);

            this.nextMusicPlayer = new AudioStreamPlayer();
            this.nextMusicPlayer.Bus = this.MusicBusName;
            this.nextMusicPlayer.VolumeDb = -80.0f; // Start muted
            this.AddChild(this.nextMusicPlayer);

            this.sfxPlayer = new AudioStreamPlayer();
            this.sfxPlayer.Bus = this.SfxBusName;
            this.sfxPlayer.VolumeDb = this.DefaultSfxVolume;
            this.AddChild(this.sfxPlayer);

            // Create crossfade timer
            this.crossfadeTimer = new Godot.Timer();
            this.crossfadeTimer.OneShot = true;
            this.crossfadeTimer.Timeout += this.OnCrossfadeTimeout;
            this.AddChild(this.crossfadeTimer);
        }

        /// <summary>
        /// Play a music track.
        /// </summary>
        /// <param name="track">The audio stream to play.</param>
        public void Play(AudioStream track)
        {
            if (track == null || this.currentMusicPlayer == null)
            {
                return;
            }

            // Add to history
            this.musicHistory.Add(track);

            // If no music is currently playing, play immediately
            if (this.currentMusicPlayer.Stream == null || !this.currentMusicPlayer.Playing)
            {
                this.currentMusicPlayer.Stream = track;
                this.currentMusicPlayer.Play();
                return;
            }

            // If the same track is already playing, do nothing
            if (this.currentMusicPlayer.Stream == track)
            {
                return;
            }

            // Crossfade to the new track
            this.CrossfadeTo(track);
        }

        /// <summary>
        /// Play a sound effect.
        /// </summary>
        /// <param name="sfx">The sound effect to play.</param>
        public void PlaySfx(AudioStream sfx)
        {
            if (sfx == null || this.sfxPlayer == null)
            {
                return;
            }

            this.sfxPlayer.Stream = sfx;
            this.sfxPlayer.Play();
        }

        /// <summary>
        /// Crossfade from the current track to a new track.
        /// </summary>
        /// <param name="newTrack">The new audio track to crossfade to.</param>
        public void CrossfadeTo(AudioStream newTrack)
        {
            if (newTrack == null || this.nextMusicPlayer == null || this.crossfadeTimer == null)
            {
                return;
            }

            // Set up the next player with the new track
            this.nextMusicPlayer.Stream = newTrack;
            this.nextMusicPlayer.Play();

            // Start the crossfade
            this.crossfadeTimer.WaitTime = this.CrossfadeDuration;
            this.crossfadeTimer.Start();
        }

        /// <summary>
        /// Stop the currently playing music.
        /// </summary>
        public void Stop()
        {
            if (this.currentMusicPlayer != null)
            {
                this.currentMusicPlayer.Stop();
            }
            if (this.nextMusicPlayer != null)
            {
                this.nextMusicPlayer.Stop();
            }
            if (this.crossfadeTimer != null)
            {
                this.crossfadeTimer.Stop();
            }
        }

        /// <summary>
        /// Pause the currently playing music.
        /// </summary>
        public void Pause()
        {
            if (this.currentMusicPlayer != null)
            {
                this.currentMusicPlayer.StreamPaused = true;
            }
            if (this.nextMusicPlayer != null)
            {
                this.nextMusicPlayer.StreamPaused = true;
            }
        }

        /// <summary>
        /// Resume the currently paused music.
        /// </summary>
        public void Resume()
        {
            if (this.currentMusicPlayer != null)
            {
                this.currentMusicPlayer.StreamPaused = false;
            }
            if (this.nextMusicPlayer != null)
            {
                this.nextMusicPlayer.StreamPaused = false;
            }
        }

        /// <summary>
        /// Get the currently playing music track.
        /// </summary>
        /// <returns>The currently playing audio stream, or <see langword="null"/> if no track is playing.</returns>
        public AudioStream? GetPlayingTrack()
        {
            return this.currentMusicPlayer?.Stream;
        }

        /// <summary>
        /// Get the history of played music tracks.
        /// </summary>
        /// <returns>A list containing the history of played music tracks.</returns>
        public List<AudioStream> GetMusicHistory()
        {
            return new List<AudioStream>(this.musicHistory);
        }

        /// <summary>
        /// Clear the music history.
        /// </summary>
        public void ClearMusicHistory()
        {
            this.musicHistory.Clear();
        }

        /// <summary>
        /// Set the volume for music playback.
        /// </summary>
        /// <param name="volumeDb">The volume level in decibels.</param>
        public void SetMusicVolume(float volumeDb)
        {
            if (this.currentMusicPlayer != null)
            {
                this.currentMusicPlayer.VolumeDb = volumeDb;
            }
            if (this.nextMusicPlayer != null)
            {
                this.nextMusicPlayer.VolumeDb = volumeDb;
            }
        }

        /// <summary>
        /// Set the volume for sound effects playback.
        /// </summary>
        /// <param name="volumeDb">The volume level in decibels.</param>
        public void SetSfxVolume(float volumeDb)
        {
            if (this.sfxPlayer != null)
            {
                this.sfxPlayer.VolumeDb = volumeDb;
            }
        }

        /// <summary>
        /// Gradually adjust the volume of the current music player.
        /// </summary>
        /// <param name="targetVolume">The target volume level in decibels.</param>
        /// <param name="duration">The duration of the fade in seconds.</param>
        public async void FadeVolume(float targetVolume, float duration)
        {
            if (this.currentMusicPlayer == null)
            {
                return;
            }

            var startVolume = this.currentMusicPlayer.VolumeDb;
            var elapsed = 0.0f;

            while (elapsed < duration)
            {
                elapsed += (float) this.GetProcessDeltaTime();
                var t = elapsed / duration;
                this.currentMusicPlayer.VolumeDb = Mathf.Lerp(startVolume, targetVolume, t);

                // Wait for the next frame
                await this.ToSignal(this.GetTree(), SceneTree.SignalName.ProcessFrame);
            }

            this.currentMusicPlayer.VolumeDb = targetVolume;
        }

        /// <summary>
        /// Callback when the crossfade timer times out.
        /// </summary>
        private void OnCrossfadeTimeout()
        {
            // Swap players
            var tempPlayer = this.currentMusicPlayer;
            this.currentMusicPlayer = this.nextMusicPlayer;
            this.nextMusicPlayer = tempPlayer;

            // Mute the old player
            if (this.nextMusicPlayer != null)
            {
                this.nextMusicPlayer.VolumeDb = -80.0f;
                this.nextMusicPlayer.Stop();
            }
        }
    }
}
