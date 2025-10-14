// Copyright (c) Ωmega Spiral. All rights reserved.

using System;
using Godot;

/// <summary>
/// The visual representation of a <see cref="Battler"/>.
///
/// Battler animations respond visually to a closed set of stimuli, such as receiving a hit or
/// moving to a position. These animations often represent a single character or a class of enemies
/// and are added as children to a given Battler.
///
/// <br/><br/>Note: BattlerAnims must be children of a Battler object to function correctly!.
/// </summary>
[Tool]
public partial class BattlerAnim : Marker2D
{
    /// <summary>
    /// Dictates how far the battler moves forwards and backwards at the beginning/end of its turn.
    /// </summary>
    private const float MoveOffset = 40.0f;

    /// <summary>
    /// Private fields used for internal state management of the <see cref="BattlerAnim"/> instance.
    /// </summary>
    private BattlerDirection direction = BattlerDirection.Right;
    private Tween? moveTween;
    private Vector2 restPosition = Vector2.Zero;
    private Marker2D? front;
    private Marker2D? top;
    private AnimationPlayer? anim;

    /// <summary>
    /// Gets a value indicating whether the animation player is currently playing an animation.
    /// </summary>
    public bool IsPlaying => this.anim?.IsPlaying() ?? false;

    /// <summary>
    /// Gets the top anchor marker for positioning UI elements.
    /// </summary>
    public Marker2D Top => this.top ??= this.GetNode<Marker2D>("TopAnchor");

    /// <summary>
    /// Emitted whenever an action-based animation wants to apply an effect. May be triggered multiple
    /// times per animation.
    /// </summary>
    [Signal]
    public delegate void ActionTriggeredEventHandler();

    /// <summary>
    /// Forward AnimationPlayer's same signal.
    /// </summary>
    /// <param name="name">The name of the finished animation.</param>
    [Signal]
    public delegate void AnimationFinishedEventHandler(string name);

    /// <summary>
    /// Gets or sets an icon that shows up on the turn bar.
    /// </summary>
    [Export]
    public Texture2D? BattlerIcon { get; set; }

    /// <summary>
    /// Gets or sets determines which direction the <see cref="BattlerAnim"/> faces. This is generally set by whichever "side"
    /// the battler is on, player or enemy.
    /// </summary>
    [Export]
    public BattlerDirection Direction
    {
        get => this.direction;
        set
        {
            this.direction = value;

            this.Scale = new Vector2(1, this.Scale.Y);
            if (this.direction == BattlerDirection.Left)
            {
                this.Scale = new Vector2(-1, this.Scale.Y);
            }
        }
    }

    /// <summary>
    /// Gets or sets determines the time it takes for the <see cref="BattlerAnim"/> to slide forward or backward when its turn
    /// comes up.
    /// </summary>
    [Export]
    public float SelectMoveTime { get; set; } = 0.3f;

    /// <inheritdoc/>
    public override void _Ready()
    {
        this.anim = this.GetNode<AnimationPlayer>("Pivot/AnimationPlayer");
        this.front = this.GetNode<Marker2D>("FrontAnchor");
        this.top = this.GetNode<Marker2D>("TopAnchor");

        this.anim.AnimationFinished += (StringName animName) => this.EmitSignal(SignalName.AnimationFinished, animName.ToString());

        this.restPosition = this.Position;
    }

    /// <summary>
    /// Setup the BattlerAnim object to respond to gameplay signals from a <see cref="Battler"/> instance.
    /// </summary>
    /// <param name="battler">The battler to associate with this animation.</param>
    /// <param name="facing">The initial direction the battler is facing.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="battler"/> is <see langword="null"/>.</exception>
    public void Setup(Battler battler, BattlerDirection facing)
    {
        if (battler == null)
        {
            throw new ArgumentNullException(nameof(battler));
        }

        // BattlerAnim objects are assigned in-editor and created dynamically both in-game and in-editor.
        // We do not want the BattlerAnim objects to be saved with the CombatArena scenes, since they are
        // instantiated at runtime, so they should not be assigned an owner when in the editor.
        // However, in gameplay the BattlerAnim class must have an owner, as these objects need to be
        // discoverable by Node::find_children(). This allows us to wait for animations to finish playing
        // before ending combat, for example.
        if (!Engine.IsEditorHint())
        {
            this.Owner = battler;
        }

        this.Direction = facing;

        battler.HealthDepleted += () =>
        {
            if (this.anim != null)
            {
                this.anim.Play("die");
            }
        };

        battler.HitReceived += (int value) =>
        {
            if (value > 0 && this.anim != null)
            {
                this.anim.Play("hurt");
            }
        };

        battler.SelectionToggled += (bool value) =>
        {
            if (value)
            {
                this.MoveForward(this.SelectMoveTime);
            }
            else
            {
                this.MoveToRest(this.SelectMoveTime);
            }
        };
    }

    /// <summary>
    /// Queues the specified animation sequence and plays it if the animation player is stopped.
    /// </summary>
    /// <param name="animName">The name of the animation to queue and play.</param>
    public void QueueAnimation(string animName)
    {
        System.Diagnostics.Debug.Assert(this.anim != null && this.anim.HasAnimation(animName), $"Battler animation '{this.Name}' does not have animation '{animName}'!");

        this.anim.Queue(animName);
        if (this.anim != null && !this.anim.IsPlaying())
        {
            this.anim.Play();
        }
    }

    /// <summary>
    /// Tween the object <see cref="MoveOffset"/> pixels from its rest position towards enemy <see cref="Battler"/>s.
    /// </summary>
    /// <param name="duration">The time in seconds for the tween animation to complete.</param>
    public void MoveForward(float duration)
    {
        if (this.moveTween != null)
        {
            this.moveTween.Kill();
        }

        this.moveTween = this.CreateTween().SetEase(Tween.EaseType.InOut).SetTrans(Tween.TransitionType.Quart);
        this.moveTween.TweenProperty(
            this,
            "position",
            this.restPosition + (Vector2.Left * this.Scale.X * MoveOffset),
            duration);
    }

    /// <summary>
    /// Tween the object back to its rest position.
    /// </summary>
    /// <param name="duration">The time in seconds for the tween animation to complete.</param>
    public void MoveToRest(float duration)
    {
        if (this.moveTween != null)
        {
            this.moveTween.Kill();
        }

        this.moveTween = this.CreateTween().SetEase(Tween.EaseType.InOut).SetTrans(Tween.TransitionType.Quart);
        this.moveTween.TweenProperty(
            this,
            "position",
            this.restPosition,
            duration);
    }
}
