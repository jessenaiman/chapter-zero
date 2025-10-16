namespace OmegaSpiral.Source.Scripts.Field.UI.Popups;

// <copyright file="UIPopup.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using Godot;

/// <summary>
/// An animated pop-up graphic. These are often found, for example, in dialogue bubbles to
/// demonstrate the need for player input.
/// </summary>
[GlobalClass]
[Tool]
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
        /// <summary>
        /// The popup is not visible.
        /// </summary>
        Hidden = 0,
        /// <summary>
        /// The popup is fully visible and idle.
        /// </summary>
        Shown = 1,
        /// <summary>
        /// The popup is in the process of disappearing.
        /// </summary>
        Hiding = 2,
        /// <summary>
        /// The popup is in the process of appearing.
        /// </summary>
        Showing = 3,
    }

    /// <summary>
    /// Gets or sets a value indicating whether the popup is currently shown.
    /// Setting this property triggers the appropriate animation and state transitions.
    /// </summary>
    /// <remarks>
    /// When set to <see langword="true"/>, the popup will appear if it is hidden.
    /// When set to <see langword="false"/>, the popup will disappear if it is shown or in the bounce wait state.
    /// </remarks>
    protected bool IsShown
    {
        get => field;
        set
        {
            field = value;

            if (!this.IsInsideTree())
            {
                // We'll set the value and wait for the node to be ready
                field = value;
                return;
            }

            if (field && this.state == States.Hidden)
            {
                this.Anim?.Play("appear");
                this.state = States.Showing;
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
            else if (!field && Anim != null && Anim.CurrentAnimation == "bounce_wait")
            {
                this.Anim.Play("disappear");
                this.state = States.Hiding;
            }
        }
    }

    /// <summary>
    /// Track what is currently happening to the popup.
    /// </summary>
    private States state = States.Hidden;

    /// <summary>
    /// The animation player used for controlling popup animations.
    /// </summary>
    protected AnimationPlayer? Anim { get; set; }
    /// <summary>
    /// The sprite used to visually represent the popup.
    /// </summary>
    protected Sprite2D? Sprite { get; set; }

    /// <inheritdoc/>
    public override void _Ready()
    {
        if (!Engine.IsEditorHint())
        {
            if (this.Sprite != null)
            {
                this.Sprite.Scale = Vector2.Zero;
            }
            if (this.Anim != null)
            {
                this.Anim.AnimationFinished += this.OnAnimationFinished;
            }
        }
    }

    /// <summary>
    /// Wait for the popup to disappear cleanly before freeing. If the popup is already hidden, it may be
    /// freed immediately.
    /// This is useful for smoothly removing a poup from an external object.
    /// </summary>
    public async void HideAndFree()
    {
        if (this.state != States.Hidden)
        {
            this.IsShown = false;
            await this.ToSignal(this, SignalName.Disappeared);
        }

        this.QueueFree();
    }

    // Please see the note attached embedded in _is_shown's setter.
    // A peculiarity of the bounce animation is that there is a wait time afterwards before the next
    // bounce. However, it doesn't look 'right' to wait to hide the popup until the animation has
    // finished when the bounce and wait are baked together into a single animation. Ideally, we should
    // be able to hide the popup whenever it's not growing or shrinking (or bouncing).
    //
    // Therefore, the bounce animation will check, via the following method, for whether or not the wait
    // portion of the animation should be played or if the popup should disappear beforehand.
    //

    /// <summary>
    /// Called when the bounce animation finishes to determine the next animation state.
    /// </summary>
    protected void OnBounceFinished()
    {
        if (this.Anim == null)
        {
            return;
        }

        if (this.IsShown)
        {
            this.Anim.Play("bounce_wait");
        }
        else
        {
            this.Anim.Play("disappear");
            this.state = States.Hiding;
        }
    }

    /// <inheritdoc/>
    public override void _EnterTree()
    {
        this.Anim = this.GetNode<AnimationPlayer>("AnimationPlayer");
        this.Sprite = this.GetNode<Sprite2D>("Sprite2D");
    }

    /// <summary>
    /// An animation has finished, so we may want to change the popup's behaviour depending on whether or
    /// not it has been flagged for a state change through _is_shown.
    /// </summary>
    /// <param name="animName"></param>
    private void OnAnimationFinished(StringName animName)
    {
        if (this.Anim == null)
        {
            return;
        }

        if (this.state == States.Hiding)
        {
            this.EmitSignal(SignalName.Disappeared);
        }

        // The popup has should be shown. If the popup is hiding or is hidden, go ahead and have it
        // appear. Otherwise, the popup can play a default bouncy animation to draw the player's eye.
        if (this.IsShown)
        {
            switch (this.state)
            {
                case States.Hiding:
                case States.Hidden:
                    this.Anim.Play("appear");
                    this.state = States.Showing;
                    break;
                default:
                    this.Anim.Play("bounce");
                    this.state = States.Shown;
                    break;
            }
        }

        // The popup should be hidden. If it has just appeared, cause it to disappear. Otherwise just
        // flag it as hidden.
        else
        {
            switch (this.state)
            {
                case States.Showing:
                case States.Shown:
                    this.Anim.Play("disappear");
                    this.state = States.Hiding;
                    break;
                default:
                    this.state = States.Hidden;
                    break;
            }
        }
    }
}
