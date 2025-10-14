// <copyright file="MusicPlayer.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System;
using System.Threading.Tasks;
using Godot;

public partial class MusicPlayer : Node
{
    /// <summary>
    /// The animation player used for crossfading between tracks.
    /// </summary>
    private AnimationPlayer? anim;
    /// <summary>
    /// The audio stream player that plays the music tracks.
    /// </summary>
    private AudioStreamPlayer? track;

    /// <inheritdoc/>
    public override void _Ready()
    {
        this.anim = this.GetNode<AnimationPlayer>("AnimationPlayer");
        this.track = this.GetNode<AudioStreamPlayer>("AudioStreamPlayer");
    }

    /// <summary>
    /// Plays a new music track with optional fade in/out transitions.
    /// </summary>
    /// <param name="newStream">The audio stream to play.</param>
    /// <param name="timeIn">The fade-in duration in seconds. Defaults to 0.0.</param>
    /// <param name="timeOut">The fade-out duration in seconds for the current track. Defaults to 0.0.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public async void Play(AudioStream newStream, float timeIn = 0.0f, float timeOut = 0.0f)
    {
        if (this.track == null || this.anim == null)
        {
            return;
        }

        if (newStream == this.track.Stream)
        {
            return;
        }

        if (this.IsPlaying())
        {
            if (Mathf.IsEqualApprox(timeOut, 0.0f))
            {
                timeOut = 0.005f;
            }

            this.anim.SpeedScale = 1.0f / timeOut;
            this.anim.Play("fade_out");
            await this.ToSignal(this.anim, AnimationPlayer.SignalName.AnimationFinished);

            this.track.Stop();
        }

        this.track.Stream = newStream;
        if (Mathf.IsEqualApprox(timeIn, 0.0f))
        {
            timeIn = 0.005f;
        }

        this.track.VolumeDb = -50.0f;
        this.track.Play();
        this.anim.SpeedScale = 1.0f / timeIn;
        this.anim.Play("fade_in");
        await this.ToSignal(this.anim, AnimationPlayer.SignalName.AnimationFinished);

        this.anim.SpeedScale = 1.0f;
    }

    /// <summary>
    /// Stops the currently playing music with an optional fade-out transition.
    /// </summary>
    /// <param name="timeOut">The fade-out duration in seconds. Defaults to 0.0.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public async void Stop(float timeOut = 0.0f)
    {
        if (this.track == null || this.anim == null)
        {
            return;
        }

        if (Mathf.IsEqualApprox(timeOut, 0.0f))
        {
            timeOut = 0.005f;
        }

        this.anim.SpeedScale = 1.0f / timeOut;
        this.anim.Play("fade_out");
        await this.ToSignal(this.anim, AnimationPlayer.SignalName.AnimationFinished);

        this.track.Stop();
        this.track.Stream = null;
    }

    /// <summary>
    /// Checks if a music track is currently playing.
    /// </summary>
    /// <returns><see langword="true"/> if a track is playing; otherwise, <see langword="false"/>.</returns>
    public bool IsPlaying()
    {
        return this.track?.Playing ?? false;
    }

    /// <summary>
    /// Gets the currently playing audio stream.
    /// </summary>
    /// <returns>The current <see cref="AudioStream"/>, or <see langword="null"/> if no track is playing.</returns>
    public AudioStream? GetPlayingTrack()
    {
        return this.track?.Stream;
    }
}
