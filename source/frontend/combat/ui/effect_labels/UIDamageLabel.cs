
// <copyright file="UiDamageLabel.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using Godot;
using System.Globalization;
using OmegaSpiral.Source.Combat.Actions;
using OmegaSpiral.Source.Design;

namespace OmegaSpiral.Source.Scripts.Combat.Ui.EffectLabels;
/// <summary>
/// An animated combat Ui element emphasizing damage done (or healed) to a battler.
/// </summary>
[GlobalClass]
public partial class UiDamageLabel : Marker2D
{
    /// <summary>
    /// Gets or sets determines how far the label will move upwards.
    /// </summary>
    [Export]
    public float MoveDistance { get; set; } = 96.0f;

    /// <summary>
    /// Gets or sets determines how long the label will be moving upwards.
    /// </summary>
    [Export]
    public float MoveTime { get; set; } = 0.6f;

    /// <summary>
    /// Gets or sets determines how long it will take for the label to fade to transparent. This occurs at the end of
    /// the upwards movement.
    /// <br/><br/><b>Note:</b> fade_time must be less than <see cref="MoveTime"/>.
    /// </summary>
    [Export]
    public float FadeTime { get; set; } = 0.2f;

    /// <summary>
    /// Gets or sets label color when <see cref="Amount"/> is >= 0.
    /// </summary>
    [Export]
    public Color ColorDamage { get; set; } = DesignService.GetColor("damage_color");

    /// <summary>
    /// Gets or sets label outline color when <see cref="Amount"/> is >= 0.
    /// </summary>
    [Export]
    public Color ColorDamageOutline { get; set; } = DesignService.GetColor("damage_color");

    /// <summary>
    /// Gets or sets label color when <see cref="Amount"/> is &lt; 0.
    /// </summary>
    [Export]
    public Color ColorHeal { get; set; } = DesignService.GetColor("heal_color");

    /// <summary>
    /// Gets or sets label outline color when <see cref="Amount"/> is &lt; 0.
    /// </summary>
    [Export]
    public Color ColorHealOutline { get; set; } = DesignService.GetColor("heal_color");

    private int amount;

    /// <summary>
    /// Gets or sets consistent with <see cref="BattlerHit"/>, damage values greater than 0 incur damage whereas those less than 0
    /// are for healing.
    /// </summary>
    public int Amount
    {
        get => this.amount;
        set
        {
            this.amount = value;

            if (!this.IsInsideTree())
            {
                // We'll set the value and wait for the node to be ready
                this.amount = value;
                return;
            }

            if (this.label != null)
            {
                this.label.Text = this.amount.ToString(CultureInfo.InvariantCulture);

                if (this.amount >= 0)
                {
                    this.label.Modulate = this.ColorDamage;
                    this.label.AddThemeColorOverride("font_outline_color", this.ColorDamageOutline);
                }
                else
                {
                    this.label.Modulate = this.ColorHeal;
                    this.label.AddThemeColorOverride("font_outline_color", this.ColorHealOutline);
                }
            }
        }
    }

    private Tween? tween;

    private Label? label;

    /// <inheritdoc/>
    public override void _Ready()
    {
        this.label = this.GetNode<Label>("Label");

        // If Amount was set before the node was ready, apply it now
        if (this.label != null)
        {
            this.label.Text = this.amount.ToString(CultureInfo.InvariantCulture);

            if (this.amount >= 0)
            {
                this.label.Modulate = this.ColorDamage;
                this.label.AddThemeColorOverride("font_outline_color", this.ColorDamageOutline);
            }
            else
            {
                this.label.Modulate = this.ColorHeal;
                this.label.AddThemeColorOverride("font_outline_color", this.ColorHealOutline);
            }
        }

        System.Diagnostics.Debug.Assert(this.FadeTime < this.MoveTime, $"{nameof(UiDamageLabel)}'s FadeTime must be less than its MoveTime!");
    }

    /// <summary>
    /// Setup the damage label with origin position and damage amount.
    /// </summary>
    /// <param name="origin">The origin position.</param>
    /// <param name="damageAmount">The damage amount (positive for damage, negative for healing).</param>
    public void Setup(Vector2 origin, int damageAmount)
    {
        this.GlobalPosition = origin;
        this.Amount = damageAmount;

        // Animate the label, moving it in an upwards direction.
        // We define a range of 60 degrees for the labels movement.
        var angle = (GD.Randf() * (Mathf.Pi / 3.0f)) - (Mathf.Pi / 6.0f); // Random angle between -π/6 and π/6
        var target = this.Position;
        if (this.label != null)
        {
            target = (Vector2.Up.Rotated(angle) * this.MoveDistance) + this.label.Position;
        }

        this.tween = this.CreateTween().SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Quad);
        if (this.label != null)
        {
            this.tween.TweenProperty(
                this.label,
                "position",
                target,
                this.MoveTime);
        }

        // Fade out the label at the end of it's movement upwards.
        if (this.tween != null)
        {
            this.tween.Parallel().TweenProperty(
                this,
                "modulate",
                Colors.Transparent,
                this.FadeTime).SetEase(Tween.EaseType.In).SetTrans(Tween.TransitionType.Linear).SetDelay(this.MoveTime - this.FadeTime);

            // Finally, after everything prior has finished, free the label.
            this.tween.TweenCallback(new Callable(this, "QueueFree"));
        }
    }
}
