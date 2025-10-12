// <copyright file="MusicPlayer.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System;
using System.Threading.Tasks;
using Godot;

public partial class MusicPlayer : Node
{
    private AnimationPlayer anim;
    private AudioStreamPlayer track;

    /// <inheritdoc/>
    public override void _Ready()
    {
        this.anim = this.GetNode<AnimationPlayer>("AnimationPlayer");
        this.track = this.GetNode<AudioStreamPlayer>("AudioStreamPlayer");
    }

    public async void Play(AudioStream newStream, float timeIn = 0.0f, float timeOut = 0.0f)
    {
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

    public async void Stop(float timeOut = 0.0f)
    {
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

    public bool IsPlaying()
    {
        return this.track.Playing;
    }

    public AudioStream GetPlayingTrack()
    {
        return this.track.Stream;
    }
}
