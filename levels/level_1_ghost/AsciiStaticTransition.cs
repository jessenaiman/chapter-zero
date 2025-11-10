#pragma warning disable SA1636 // File header copyright text should match
// <copyright file="AsciiStaticTransition.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

namespace OmegaSpiral.Source.Stages.Stage1;
#pragma warning restore SA1636 // File header copyright text should match

using System;
using System.Threading.Tasks;
using Godot;

/// <summary>
/// Implements ASCII static transition effect similar to the React prototype.
/// Creates full-screen static effect with random ASCII characters.
/// </summary>
public sealed partial class AsciiStaticTransition : Control
{
    private const string _Chars = "█▓▒░.:·¯`-_¸,ø¤º°`°º¤ø,¸¸,ø¤º°`";
    private const int _Cols = 80;
    private const int _Rows = 24;

    private RichTextLabel? _StaticLabel;
    private Timer? _StaticTimer;
    private Timer? _DurationTimer;
    private float _Duration;
    private bool _IsTransitioning;

    /// <summary>
    /// Signal emitted when the static transition completes.
    /// </summary>
    [Signal]
    public delegate void TransitionCompleteEventHandler();

    public override void _Ready()
    {
        // Set up full-screen overlay
        SetAnchorsPreset(LayoutPreset.FullRect);

        // Create static display
        _StaticLabel = new RichTextLabel
        {
            Text = "",
            FitContent = true,
            AutowrapMode = TextServer.AutowrapMode.Off,
            BbcodeEnabled = false
        };

        // Style the static
        var style = new StyleBoxFlat();
        style.BgColor = Colors.Black;
        _StaticLabel.AddThemeStyleboxOverride("normal", style);

        _StaticLabel.AddThemeColorOverride("default_color", new Color(0.2f, 1f, 0.2f, 0.5f)); // Green with opacity
        _StaticLabel.AddThemeConstantOverride("outline_size", 0);
        _StaticLabel.AddThemeFontOverride("normal", GetThemeFont("font", "Console"));
        _StaticLabel.AddThemeFontSizeOverride("normal_font_size", 8);
        _StaticLabel.AddThemeConstantOverride("line_spacing", -4);

        AddChild(_StaticLabel);

        // Set up timers
        _StaticTimer = new Timer
        {
            WaitTime = 0.05f, // Update every 50ms
            OneShot = false
        };
        _StaticTimer.Timeout += OnStaticTick;
        AddChild(_StaticTimer);

        _DurationTimer = new Timer
        {
            OneShot = true
        };
        _DurationTimer.Timeout += OnDurationComplete;
        AddChild(_DurationTimer);

        // Initially hidden
        Visible = false;
        Modulate = Colors.Transparent;
    }

    /// <summary>
    /// Starts the ASCII static transition effect.
    /// </summary>
    /// <param name="duration">Duration of the static effect in milliseconds.</param>
    public async Task StartTransitionAsync(float duration = 2000f)
    {
        if (_IsTransitioning)
            return;

        _Duration = duration;
        _IsTransitioning = true;

        // Show the overlay
        Visible = true;

        // Fade in
        var tween = CreateTween();
        tween.SetParallel(false);
        tween.TweenProperty(this, "modulate", new Color(1, 1, 1, 1), 0.2f);

        await ToSignal(tween, Tween.SignalName.Finished);

        // Start static generation
        _StaticTimer.Start();
        _DurationTimer.WaitTime = duration / 1000f; // Convert to seconds
        _DurationTimer.Start();

        // Wait for completion
        await ToSignal(this, SignalName.TransitionComplete);
    }

    private void OnStaticTick()
    {
        if (_StaticLabel == null)
            return;

        var random = new Random();
        var staticText = string.Empty;

        for (var i = 0; i < _Cols * _Rows; i++)
        {
            var charIndex = random.Next(_Chars.Length);
            staticText += _Chars[charIndex];
        }

        _StaticLabel.Text = staticText;
    }

    private void OnDurationComplete()
    {
        _StaticTimer?.Stop();

        // Fade out
        var tween = CreateTween();
        tween.SetParallel(false);
        tween.TweenProperty(this, "modulate", Colors.Transparent, 0.3f);

        tween.TweenCallback(Callable.From(() =>
        {
            Visible = false;
            _IsTransitioning = false;
            EmitSignal(SignalName.TransitionComplete);
        }));
    }

    /// <summary>
    /// Stops the transition immediately.
    /// </summary>
    public void StopTransition()
    {
        if (!_IsTransitioning)
            return;

        _StaticTimer?.Stop();
        _DurationTimer?.Stop();

        var tween = CreateTween();
        tween.TweenProperty(this, "modulate", Colors.Transparent, 0.2f);

        tween.TweenCallback(Callable.From(() =>
        {
            Visible = false;
            _IsTransitioning = false;
        }));
    }

    /// <summary>
    /// Gets whether the transition is currently active.
    /// </summary>
    public bool IsTransitioning => _IsTransitioning;
}
