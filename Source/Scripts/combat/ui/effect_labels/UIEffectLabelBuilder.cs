// <copyright file="UIEffectLabelBuilder.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System;
using Godot;

/// <summary>
/// A builder class responsible for adding visual feedback to <see cref="BattlerAction"/>s.
///
/// This feedback takes the form of different UI elements (such as an animated label) that may
/// demonstrate how much damage was done or if an action missed the target completely.
/// </summary>
public partial class UIEffectLabelBuilder : Node2D
{
    [Export]
    public PackedScene DamageLabelScene { get; set; }

    [Export]
    public PackedScene MissedLabelScene { get; set; }

    /// <summary>
    /// Setup the effect label builder with battler data.
    /// </summary>
    /// <param name="battlerData">The battler list to connect to.</param>
    public void Setup(BattlerList battlerData)
    {
        foreach (var battler in battlerData.GetAllBattlers())
        {
            battler.HitMissed += () =>
            {
                var label = this.MissedLabelScene.Instantiate();
                this.AddChild(label);
                label.GlobalPosition = battler.Anim.Top.GlobalPosition;
            };

            battler.HitReceived += (amount) =>
            {
                var label = this.DamageLabelScene.Instantiate() as UIDamageLabel;
                this.AddChild(label);
                label.Setup(battler.Anim.Top.GlobalPosition, amount);
            };
        }
    }
}
