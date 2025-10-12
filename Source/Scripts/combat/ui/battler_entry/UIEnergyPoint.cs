using Godot;
using System;

/// <summary>
/// A single energy point UI element, animating smoothly as a player Battler gains and spends energy.
/// </summary>
public partial class UIEnergyPoint : MarginContainer
{
    /// <summary>
    /// When highlighted, the point indicator will be offset by the following amount.
    /// </summary>
    private static readonly Vector2 SelectedOffset = new Vector2(0.0f, -6.0f);

    /// <summary>
    /// The time required to move the point to or from <see cref="SelectedOffset"/>.
    /// </summary>
    private const float SelectTime = 0.2f;

    /// <summary>
    /// The time required to fade in or out the filled point.
    /// </summary>
    private const float FadeTime = 0.3f;

    private Tween _colorTween = null;
    private Tween _offsetTween = null;

    private TextureRect _fill;

    // We store the start modulate value of the `Fill` node because it's semi-transparent.
    // This way, we can animate the color from and to this value.
    private Color _colorTransparent;

    public override void _Ready()
    {
        _fill = GetNode<TextureRect>("EnergyPoint/Fill");
        _colorTransparent = _fill.Modulate;
    }

    /// <summary>
    /// Animate the point fill texture to fully opaque.
    /// </summary>
    public void Appear()
    {
        if (_colorTween != null)
        {
            _colorTween.Kill();
        }
        _colorTween = CreateTween();
        _colorTween.TweenProperty(_fill, "modulate", Colors.White, FadeTime);
    }

    /// <summary>
    /// Animate the point fill texture to mostly-transparent.
    /// </summary>
    public void Disappear()
    {
        if (_colorTween != null)
        {
            _colorTween.Kill();
        }
        _colorTween = CreateTween();
        _colorTween.TweenProperty(_fill, "modulate", _colorTransparent, FadeTime);
    }

    public void Select()
    {
        if (_offsetTween != null)
        {
            _offsetTween.Kill();
        }
        _offsetTween = CreateTween();
        _offsetTween.TweenProperty(_fill, "position", SelectedOffset, SelectTime);
    }

    public void Deselect()
    {
        if (_offsetTween != null)
        {
            _offsetTween.Kill();
        }
        _offsetTween = CreateTween();
        _offsetTween.TweenProperty(_fill, "position", Vector2.Zero, SelectTime);
    }
}
