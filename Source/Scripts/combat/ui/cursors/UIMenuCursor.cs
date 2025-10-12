using Godot;
using System;

/// <summary>
/// The cursor of a <see cref="UIListMenu"/>, indicating which option is currently in focus.
/// </summary>
public partial class UIMenuCursor : Marker2D
{
    /// <summary>
    /// The time taken to move the cursor from one menu entry to the next.
    /// </summary>
    private const float SlideTime = 0.1f;

    // The tween used to move the cursor between menu entries.
    private Tween _slideTween = null;

    private AnimationPlayer _anim;

    public override void _Ready()
    {
        _anim = GetNode<AnimationPlayer>("AnimationPlayer");

        // The arrow needs to move independently from its parent.
        SetAsTopLevel(true);
    }

    /// <summary>
    /// Smoothly move the cursor to an arbitrary position.
    /// Called by the menu to move the cursor from entry to entry.
    /// </summary>
    /// <param name="target">The target position to move to</param>
    public void MoveTo(Vector2 target)
    {
        if (_slideTween != null)
        {
            _slideTween.Kill();
        }
        _slideTween = CreateTween().SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Cubic);
        _slideTween.TweenProperty(this, "position", target, SlideTime);
    }

    /// <summary>
    /// Advance the arrow's animation to a given point.
    /// </summary>
    /// <param name="seconds">The time to advance the animation to</param>
    public void AdvanceAnimation(float seconds)
    {
        _anim.Seek(seconds, true);
    }

    /// <summary>
    /// Get the current position of the bounce animation.
    /// </summary>
    /// <returns>The current animation position in seconds</returns>
    public float GetAnimationPosition()
    {
        return _anim.GetCurrentAnimationPosition();
    }
}
