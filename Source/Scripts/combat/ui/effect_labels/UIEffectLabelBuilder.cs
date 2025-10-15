// <copyright file="UIEffectLabelBuilder.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using System;
using Godot;
using OmegaSpiral.Combat.Battlers;

/// <summary>
/// A builder class responsible for adding visual feedback to <see cref="OmegaSpiral.Source.Scripts.Combat.Actions.BattlerAction"/>s.
///
/// This feedback takes the form of different UI elements (such as an animated label) that may
/// demonstrate how much damage was done or if an action missed the target completely.
/// </summary>
public partial class UIEffectLabelBuilder : Node2D
{
    /// <summary>
    /// Gets or sets the packed scene for the damage label.
    /// </summary>
    [Export]
    public PackedScene DamageLabelScene { get; set; } = null!;

    /// <summary>
    /// Gets or sets the packed scene for the missed label.
    /// </summary>
    [Export]
    public PackedScene MissedLabelScene { get; set; } = null!;

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
                if (label is Node2D label2D && battler?.Anim?.Top != null)
                {
                    label2D.GlobalPosition = battler.Anim.Top.GlobalPosition;
                }
            };

            battler.HitReceived += (amount) =>
            {
                var label = this.DamageLabelScene.Instantiate() as UIDamageLabel;
                this.AddChild(label);
                if (label != null && battler?.Anim?.Top != null)
                {
                    label.Setup(battler.Anim.Top.GlobalPosition, amount);
                }
            };
        }
    }
}
