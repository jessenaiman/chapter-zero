// <copyright file="UITurnBar.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System;
using Godot;

/// <summary>
/// Displays the timeline representing the turn order of all battlers in the arena.
/// Battler icons move along the timeline in real-time as their readiness updates.
/// </summary>
public partial class UITurnBar : Control
{
    private readonly PackedScene iconScene = GD.Load<PackedScene>("res://src/combat/ui/turn_bar/ui_battler_icon.tscn");

    private AnimationPlayer anim;
    private TextureRect background;
    private Control icons;

    /// <inheritdoc/>
    public override void _Ready()
    {
        this.anim = this.GetNode<AnimationPlayer>("AnimationPlayer");
        this.background = this.GetNode<TextureRect>("Background");
        this.icons = this.GetNode<Control>("Background/Icons");
    }

    /// <summary>
    /// Fade in (from transparent) the turn bar and all of its UI elements.
    /// </summary>
    public void FadeIn()
    {
        this.anim.Play("fade_in");
    }

    /// <summary>
    /// Fade out (to transparent) the turn bar and all of its UI elements.
    /// </summary>
    public void FadeOut()
    {
        this.anim.Play("fade_out");
    }

    /// <summary>
    /// Initialize the turn bar, passing in all the battlers that we want to display.
    /// </summary>
    /// <param name="battlerData">The battler list to display.</param>
    public void Setup(BattlerList battlerData)
    {
        foreach (var battler in battlerData.GetAllBattlers())
        {
            // Connect a handful of signals to the icon so that it may respond to changes in the
            // Battler's readiness and fade out if the Battler falls in combat.
            var icon = this.iconScene.Instantiate() as UIBattlerIcon;
            icon.IconTexture = battler.Anim.BattlerIcon;
            icon.BattlerType = battler.IsPlayer ? UIBattlerIcon.Types.Player : UIBattlerIcon.Types.Enemy;
            icon.PositionRange = new Vector2(
                -icon.Size.X / 2.0f,
                this.background.Size.X - (icon.Size.X / 2.0f));

            battler.HealthDepleted += () => icon.FadeOut();
            battler.ReadinessChanged += (readiness) =>
            {
                // There is an edge case where a player Battler has managed to deplete their own hp.
                // In this case, the UIBattlerIcon is probably already freed when the Battler's readiness
                // changes after the action has finished.
                // Thus, we need to make sure that the icon is valid before updating it.
                if (IsInstanceValid(icon))
                {
                    icon.Progress = readiness / 100.0f;
                }
            };

            this.icons.AddChild(icon);
        }
    }
}
