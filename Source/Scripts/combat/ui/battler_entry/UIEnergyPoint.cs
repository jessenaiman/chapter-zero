// <copyright file="UIEnergyPoint.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System;
using Godot;

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

    private Tween colorTween;
    private Tween offsetTween;

    private TextureRect fill;

    // We store the start modulate value of the `Fill` node because it's semi-transparent.
    // This way, we can animate the color from and to this value.
    private Color colorTransparent;

    /// <inheritdoc/>
    public override void _Ready()
    {
        this.fill = this.GetNode<TextureRect>("EnergyPoint/Fill");
        this.colorTransparent = this.fill.Modulate;
    }

    /// <summary>
    /// Animate the point fill texture to fully opaque.
    /// </summary>
    public void Appear()
    {
        if (this.colorTween != null)
        {
            this.colorTween.Kill();
        }

        this.colorTween = this.CreateTween();
        this.colorTween.TweenProperty(this.fill, "modulate", Colors.White, FadeTime);
    }

    /// <summary>
    /// Animate the point fill texture to mostly-transparent.
    /// </summary>
    public void Disappear()
    {
        if (this.colorTween != null)
        {
            this.colorTween.Kill();
        }

        this.colorTween = this.CreateTween();
        this.colorTween.TweenProperty(this.fill, "modulate", this.colorTransparent, FadeTime);
    }

    public void Select()
    {
        if (this.offsetTween != null)
        {
            this.offsetTween.Kill();
        }

        this.offsetTween = this.CreateTween();
        this.offsetTween.TweenProperty(this.fill, "position", SelectedOffset, SelectTime);
    }

    public void Deselect()
    {
        if (this.offsetTween != null)
        {
            this.offsetTween.Kill();
        }

        this.offsetTween = this.CreateTween();
        this.offsetTween.TweenProperty(this.fill, "position", Vector2.Zero, SelectTime);
    }
}
