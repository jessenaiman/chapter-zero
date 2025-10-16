namespace OmegaSpiral.Source.Scripts.Combat.UI.BattlerEntry;

// <copyright file="UIBattlerLifeBar.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using System;
using Godot;
using OmegaSpiral.Source.Scripts.Combat.Battlers;

/// <summary>
/// An element of the <see cref="UIBattlerEntry"/> that visually shows player <see cref="Battler"/> life points.
/// The LifeBar also can show an action icon to demonstrate when the player has queued an action for
/// one of their Battlers.
/// </summary>
[GlobalClass]
public partial class UIBattlerLifeBar : TextureProgressBar
{
    private float targetValue;
    private Tween? tween;
    private AnimationPlayer? anim;
    private Label? nameLabel;
    private TextureRect? queuedActionIcon;
    private Label? valueLabel;

    /// <summary>
    /// Gets or sets rate of the animation relative to the maximum value.
    /// A value of 1.0 means the animation fills the entire bar in one second.
    /// </summary>
    [Export]
    public float FillRate { get; set; } = 0.5f;

    /// <summary>
    /// Gets or sets the health percentage below which the danger animation plays.
    /// </summary>
    [Export(PropertyHint.Range, "0,1.0")]
    public float DangerCutoff { get; set; } = 0.2f;

    /// <summary>
    /// Gets or sets when this value changes, the bar smoothly animates towards it using a tween.
    /// </summary>
    public float TargetValue
    {
        get => this.targetValue;
        set
        {
            // If the amount is lower than the current TargetValue, it means the battler lost health.
            if (this.targetValue > value)
            {
                this.anim?.Play("damage");
            }

            this.targetValue = value;

            this.tween?.Kill();

            var duration = Math.Abs(this.targetValue - this.Value) / this.MaxValue * this.FillRate;
            this.tween = this.CreateTween().SetTrans(Tween.TransitionType.Quad);
            this.tween.TweenProperty(this, "value", this.targetValue, duration);
            this.tween.TweenCallback(Callable.From(() =>
            {
                if (this.Value < this.DangerCutoff * this.MaxValue)
                {
                    this.anim?.Play("danger");
                }
            }));
        }
    }

    /// <inheritdoc/>
    public override void _Ready()
    {
        base._Ready();

        this.anim = this.GetNode<AnimationPlayer>("AnimationPlayer");
        this.nameLabel = this.GetNode<Label>("MarginContainer/HBoxContainer/Name");
        this.queuedActionIcon = this.GetNode<TextureRect>("MarginContainer/HBoxContainer/QueuedActionIcon");
        this.valueLabel = this.GetNode<Label>("MarginContainer/HBoxContainer/Value");

        this.ValueChanged += this.OnValueChanged;
    }

    /// <summary>
    /// Initialize the life bar with battler information.
    /// </summary>
    /// <param name="battlerName">The name of the battler.</param>
    /// <param name="maxHp">The maximum hit points.</param>
    /// <param name="startHp">The starting hit points.</param>
    public void Setup(string battlerName, int maxHp, int startHp)
    {
        if (this.nameLabel != null)
        {
            this.nameLabel.Text = battlerName;
        }

        this.MaxValue = maxHp;
        this.Value = startHp;
    }

    /// <summary>
    /// Set the action icon texture to display the queued action.
    /// </summary>
    /// <param name="texture">The texture to display.</param>
    public void SetActionIcon(Texture2D? texture)
    {
        if (this.queuedActionIcon != null)
        {
            this.queuedActionIcon.Texture = texture;
        }
    }

    /// <summary>
    /// Callback when the value changes to update the label.
    /// </summary>
    /// <param name="newValue">The new value of the life bar.</param>
    private void OnValueChanged(double newValue)
    {
        if (this.valueLabel != null)
        {
            this.valueLabel.Text = ((int)newValue).ToString(System.Globalization.CultureInfo.InvariantCulture);
        }
    }
}
