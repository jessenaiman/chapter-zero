using Godot;
using System;

[Tool]
/// <summary>
/// An animated pop-up graphic. These are often found, for example, in dialogue bubbles to
/// demonstrate the need for player input.
/// </summary>
public partial class UIPopup : Node2D
{
    /// <summary>
    /// Emitted when the popup has completely disappeared.
    /// </summary>
    [Signal]
    public delegate void DisappearedEventHandler();

    /// <summary>
    /// The states in which a popup may exist.
    /// </summary>
    public enum States
    {
        Hidden,
        Shown,
        Hiding,
        Showing
    }

    // The target state of the popup. Setting it to true or false will cause a change in behaviour.
    // True if the popup should be shown or false if the popup should be hidden.
    // Note that this shows the TARGET state of the popup, so _is_shown may be false even while the
    // popup is appearing.
    private bool _isShown = false;
    protected bool _is_shown
    {
        get => _isShown;
        set
        {
            _isShown = value;

            if (!IsInsideTree())
            {
                // We'll set the value and wait for the node to be ready
                _isShown = value;
                return;
            }

            if (_isShown && _state == States.Hidden)
            {
                _anim.Play("appear");
                _state = States.Showing;
            }

            // A fully shown, idling popup bounces slightly to draw the player's eye. Note that there is
            // a small wait time between bounces so that the popup doesn't look overly energetic.
            // Unfortunately, this creates an edge case for smooth animation (see _on_bounce_finished).
            //
            // Basically, if the bounce animation isn't playing, but the popup is waiting for the next
            // 'bounce', we want to be able to hide the popup immediately, rather than wait for 'wait'
            // a fraction of a second for the animation to finish playing, which looks 'off'.
            //
            // So, we check here to see if the popup is sitting in this 'wait' window, where it can be
            // immediately hidden and still look smooth as butter.
            else if (!_isShown && _anim.CurrentAnimation == "bounce_wait")
            {
                _anim.Play("disappear");
                _state = States.Hiding;
            }
        }
    }

    // Track what is currently happening to the popup.
    private States _state = States.Hidden;

    protected AnimationPlayer _anim;
    protected Sprite2D _sprite;

    public override void _Ready()
    {
        if (!Engine.IsEditorHint())
        {
            _sprite.Scale = Vector2.Zero;
            _anim.AnimationFinished += _onAnimationFinished;
        }
    }

    /// <summary>
    /// Wait for the popup to disappear cleanly before freeing. If the popup is already hidden, it may be
    /// freed immediately.
    /// This is useful for smoothly removing a poup from an external object.
    /// </summary>
    public async void HideAndFree()
    {
        if (_state != States.Hidden)
        {
            _is_shown = false;
            await ToSignal(this, SignalName.Disappeared);
        }
        QueueFree();
    }

    // Please see the note attached embedded in _is_shown's setter.
    // A peculiarity of the bounce animation is that there is a wait time afterwards before the next
    // bounce. However, it doesn't look 'right' to wait to hide the popup until the animation has
    // finished when the bounce and wait are baked together into a single animation. Ideally, we should
    // be able to hide the popup whenever it's not growing or shrinking (or bouncing).
    //
    // Therefore, the bounce animation will check, via the following method, for whether or not the wait
    // portion of the animation should be played or if the popup should disappear beforehand.
    protected void _onBounceFinished()
    {
        if (_is_shown)
        {
            _anim.Play("bounce_wait");
        }
        else
        {
            _anim.Play("disappear");
            _state = States.Hiding;
        }
    }

    // An animation has finished, so we may want to change the popup's behaviour depending on whether or
    // not it has been flagged for a state change through _is_shown.
    private void _onAnimationFinished(StringName animName)
    {
        if (_state == States.Hiding)
        {
            EmitSignal(SignalName.Disappeared);
        }

        // The popup has should be shown. If the popup is hiding or is hidden, go ahead and have it
        // appear. Otherwise, the popup can play a default bouncy animation to draw the player's eye.
        if (_is_shown)
        {
            switch (_state)
            {
                case States.Hiding:
                case States.Hidden:
                    _anim.Play("appear");
                    _state = States.Showing;
                    break;
                default:
                    _anim.Play("bounce");
                    _state = States.Shown;
                    break;
            }
        }

        // The popup should be hidden. If it has just appeared, cause it to disappear. Otherwise just
        // flag it as hidden.
        else
        {
            switch (_state)
            {
                case States.Showing:
                case States.Shown:
                    _anim.Play("disappear");
                    _state = States.Hiding;
                    break;
                default:
                    _state = States.Hidden;
                    break;
            }
        }
    }

    public override void _EnterTree()
    {
        _anim = GetNode<AnimationPlayer>("AnimationPlayer");
        _sprite = GetNode<Sprite2D>("Sprite2D");
    }
}
