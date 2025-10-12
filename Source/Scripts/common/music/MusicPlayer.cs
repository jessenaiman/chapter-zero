using Godot;
using System;
using System.Threading.Tasks;

public partial class MusicPlayer : Node
{
    private AnimationPlayer _anim;
    private AudioStreamPlayer _track;

    public override void _Ready()
    {
        _anim = GetNode<AnimationPlayer>("AnimationPlayer");
        _track = GetNode<AudioStreamPlayer>("AudioStreamPlayer");
    }

    public async void Play(AudioStream newStream, float timeIn = 0.0f, float timeOut = 0.0f)
    {
        if (newStream == _track.Stream)
        {
            return;
        }

        if (IsPlaying())
        {
            if (Mathf.IsEqualApprox(timeOut, 0.0f))
            {
                timeOut = 0.005f;
            }

            _anim.SpeedScale = 1.0f / timeOut;
            _anim.Play("fade_out");
            await ToSignal(_anim, AnimationPlayer.SignalName.AnimationFinished);

            _track.Stop();
        }

        _track.Stream = newStream;
        if (Mathf.IsEqualApprox(timeIn, 0.0f))
        {
            timeIn = 0.005f;
        }

        _track.VolumeDb = -50.0f;
        _track.Play();
        _anim.SpeedScale = 1.0f / timeIn;
        _anim.Play("fade_in");
        await ToSignal(_anim, AnimationPlayer.SignalName.AnimationFinished);

        _anim.SpeedScale = 1.0f;
    }

    public async void Stop(float timeOut = 0.0f)
    {
        if (Mathf.IsEqualApprox(timeOut, 0.0f))
        {
            timeOut = 0.005f;
        }

        _anim.SpeedScale = 1.0f / timeOut;
        _anim.Play("fade_out");
        await ToSignal(_anim, AnimationPlayer.SignalName.AnimationFinished);

        _track.Stop();
        _track.Stream = null;
    }

    public bool IsPlaying()
    {
        return _track.Playing;
    }

    public AudioStream GetPlayingTrack()
    {
        return _track.Stream;
    }
}
