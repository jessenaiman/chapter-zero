using Godot;
using System;

/// <summary>
/// An animated combat UI element emphasizing damage done (or healed) to a battler.
/// </summary>
public partial class UIDamageLabel : Marker2D
{
    /// <summary>
    /// Determines how far the label will move upwards.
    /// </summary>
    [Export]
    public float MoveDistance { get; set; } = 96.0f;

    /// <summary>
    /// Determines how long the label will be moving upwards.
    /// </summary>
    [Export]
    public float MoveTime { get; set; } = 0.6f;

    /// <summary>
    /// Determines how long it will take for the label to fade to transparent. This occurs at the end of
    /// the upwards movement.
    /// <br/><br/><b>Note:</b> fade_time must be less than <see cref="MoveTime"/>.
    /// </summary>
    [Export]
    public float FadeTime { get; set; } = 0.2f;

    /// <summary>
    /// Label color when <see cref="Amount"/> is >= 0.
    /// </summary>
    [Export]
    public Color ColorDamage { get; set; } = new Color("#b0305c");

    /// <summary>
    /// Label outline color when <see cref="Amount"/> is >= 0.
    /// </summary>
    [Export]
    public Color ColorDamageOutline { get; set; } = new Color("#b0305c");

    /// <summary>
    /// Label color when <see cref="Amount"/> is &lt; 0.
    /// </summary>
    [Export]
    public Color ColorHeal { get; set; } = new Color("#3ca370");

    /// <summary>
    /// Label outline color when <see cref="Amount"/> is &lt; 0.
    /// </summary>
    [Export]
    public Color ColorHealOutline { get; set; } = new Color("#3ca370");

    private int _amount = 0;
    /// <summary>
    /// Consistent with <see cref="BattlerHit"/>, damage values greater than 0 incur damage whereas those less than 0
    /// are for healing.
    /// </summary>
    public int Amount
    {
        get => _amount;
        set
        {
            _amount = value;

            if (!IsInsideTree())
            {
                // We'll set the value and wait for the node to be ready
                _amount = value;
                return;
            }

            _label.Text = _amount.ToString();

            if (_amount >= 0)
            {
                _label.Modulate = ColorDamage;
                _label.AddThemeColorOverride("font_outline_color", ColorDamageOutline);
            }
            else
            {
                _label.Modulate = ColorHeal;
                _label.AddThemeColorOverride("font_outline_color", ColorHealOutline);
            }
        }
    }

    private Tween _tween = null;

    private Label _label;

    public override void _Ready()
    {
        _label = GetNode<Label>("Label");

        // If Amount was set before the node was ready, apply it now
        if (_label != null)
        {
            _label.Text = _amount.ToString();

            if (_amount >= 0)
            {
                _label.Modulate = ColorDamage;
                _label.AddThemeColorOverride("font_outline_color", ColorDamageOutline);
            }
            else
            {
                _label.Modulate = ColorHeal;
                _label.AddThemeColorOverride("font_outline_color", ColorHealOutline);
            }
        }

        System.Diagnostics.Debug.Assert(FadeTime < MoveTime, $"{nameof(UIDamageLabel)}'s FadeTime must be less than its MoveTime!");
    }

    /// <summary>
    /// Setup the damage label with origin position and damage amount
    /// </summary>
    /// <param name="origin">The origin position</param>
    /// <param name="damageAmount">The damage amount (positive for damage, negative for healing)</param>
    public void Setup(Vector2 origin, int damageAmount)
    {
        GlobalPosition = origin;
        Amount = damageAmount;

        // Animate the label, moving it in an upwards direction.
        // We define a range of 60 degrees for the labels movement.
        var angle = GD.RandfRange(-Mathf.Pi / 6.0f, Mathf.Pi / 6.0f);
        var target = Vector2.Up.Rotated(angle) * MoveDistance + _label.Position;

        _tween = CreateTween().SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Quad);
        _tween.TweenProperty(
            _label,
            "position",
            target,
            MoveTime
        );

        // Fade out the label at the end of it's movement upwards.
        _tween.Parallel().TweenProperty(
            this,
            "modulate",
            Colors.Transparent,
            FadeTime
        ).SetEase(Tween.EaseType.In).SetTrans(Tween.TransitionType.Linear).SetDelay(MoveTime - FadeTime);

        // Finally, after everything prior has finished, free the label.
        _tween.TweenCallback(QueueFree);
    }
}
