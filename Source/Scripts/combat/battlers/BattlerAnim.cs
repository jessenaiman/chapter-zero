using Godot;
using System;

/// <summary>
/// Determines which direction a battler faces on the screen.
/// </summary>
public enum BattlerDirection
{
    Left,
    Right
}

/// <summary>
/// The visual representation of a <see cref="Battler"/>.
///
/// Battler animations respond visually to a closed set of stimuli, such as receiving a hit or
/// moving to a position. These animations often represent a single character or a class of enemies
/// and are added as children to a given Battler.
///
/// <br/><br/>Note: BattlerAnims must be children of a Battler object to function correctly!
/// </summary>
[Tool]
public partial class BattlerAnim : Marker2D
{
    /// <summary>
    /// Dictates how far the battler moves forwards and backwards at the beginning/end of its turn.
    /// </summary>
    private const float MoveOffset = 40.0f;

    /// <summary>
    /// Emitted whenever an action-based animation wants to apply an effect. May be triggered multiple
    /// times per animation.
    /// </summary>
    [Signal]
    public delegate void ActionTriggeredEventHandler();

    /// <summary>
    /// Forward AnimationPlayer's same signal.
    /// </summary>
    [Signal]
    public delegate void AnimationFinishedEventHandler(string name);

    /// <summary>
    /// An icon that shows up on the turn bar.
    /// </summary>
    [Export]
    public Texture2D BattlerIcon { get; set; }

    private BattlerDirection _direction = BattlerDirection.Right;

    /// <summary>
    /// Determines which direction the <see cref="BattlerAnim"/> faces. This is generally set by whichever "side"
    /// the battler is on, player or enemy.
    /// </summary>
    [Export]
    public BattlerDirection Direction
    {
        get => _direction;
        set
        {
            _direction = value;

            Scale = new Vector2(1, Scale.Y);
            if (_direction == BattlerDirection.Left)
            {
                Scale = new Vector2(-1, Scale.Y);
            }
        }
    }

    /// <summary>
    /// Determines the time it takes for the <see cref="BattlerAnim"/> to slide forward or backward when its turn
    /// comes up.
    /// </summary>
    [Export]
    public float SelectMoveTime { get; set; } = 0.3f;

    private Tween? _moveTween = null;
    private Vector2 _restPosition = Vector2.Zero;

    private Marker2D? _front;
    private Marker2D? _top;
    private AnimationPlayer? _anim;

    public override void _Ready()
    {
        _anim = GetNode<AnimationPlayer>("Pivot/AnimationPlayer");
        _front = GetNode<Marker2D>("FrontAnchor");
        _top = GetNode<Marker2D>("TopAnchor");

        _anim.AnimationFinished += (StringName animName) =>
        {
            EmitSignal(SignalName.AnimationFinished, animName.ToString());
        };

        _restPosition = Position;
    }

    /// <summary>
    /// Setup the BattlerAnim object to respond to gameplay signals from a <see cref="Battler"/> class.
    /// </summary>
    public void Setup(Battler battler, BattlerDirection facing)
    {
        // BattlerAnim objects are assigned in-editor and created dynamically both in-game and in-editor.
        // We do not want the BattlerAnim objects to be saved with the CombatArena scenes, since they are
        // instantiated at runtime, so they should not be assigned an owner when in the editor.
        // However, in gameplay the BattlerAnim class must have an owner, as these objects need to be
        // discoverable by Node::find_children(). This allows us to wait for animations to finish playing
        // before ending combat, for example.
        if (!Engine.IsEditorHint())
        {
            Owner = battler;
        }

        Direction = facing;

        battler.HealthDepleted += () =>
        {
            _anim.Play("die");
        };

        battler.HitReceived += (int value) =>
        {
            if (value > 0) _anim.Play("hurt");
        };

        battler.SelectionToggled += (bool value) =>
        {
            if (value) MoveForward(SelectMoveTime);
            else MoveToRest(SelectMoveTime);
        };
    }

    /// <summary>
    /// A function that wraps around the animation players' `play()` function, delegating the work to the
    /// `AnimationPlayerDamage` node when necessary.
    /// </summary>
    public void Play(string animName)
    {
        System.Diagnostics.Debug.Assert(_anim != null && _anim.HasAnimation(animName), $"Battler animation '{Name}' does not have animation '{animName}'!");

        _anim.Play(animName);
    }

    /// <summary>
    /// Returns true if an animation is currently playing, otherwise returns false.
    /// </summary>
    public bool IsPlaying()
    {
        return _anim != null && _anim.IsPlaying();
    }

    /// <summary>
    /// Queues the specified animation sequence and plays it if the animation player is stopped.
    /// </summary>
    public void QueueAnimation(string animName)
    {
        System.Diagnostics.Debug.Assert(_anim != null && _anim.HasAnimation(animName), $"Battler animation '{Name}' does not have animation '{animName}'!");

        _anim.Queue(animName);
        if (_anim != null && !_anim.IsPlaying())
        {
            _anim.Play();
        }
    }

    /// <summary>
    /// Tween the object <see cref="MoveOffset"/> pixels from its rest position towards enemy <see cref="Battler"/>s.
    /// </summary>
    public void MoveForward(float duration)
    {
        if (_moveTween != null)
        {
            _moveTween.Kill();
        }

        _moveTween = CreateTween().SetEase(Tween.EaseType.InOut).SetTrans(Tween.TransitionType.Quart);
        _moveTween.TweenProperty(
            this,
            "position",
            _restPosition + Vector2.Left * Scale.X * MoveOffset,
            duration
        );
    }

    /// <summary>
    /// Tween the object back to its rest position.
    /// </summary>
    public void MoveToRest(float duration)
    {
        if (_moveTween != null)
        {
            _moveTween.Kill();
        }

        _moveTween = CreateTween().SetEase(Tween.EaseType.InOut).SetTrans(Tween.TransitionType.Quart);
        _moveTween.TweenProperty(
            this,
            "position",
            _restPosition,
            duration
        );
    }
}
