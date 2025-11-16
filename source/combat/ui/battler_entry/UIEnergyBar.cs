
// <copyright file="UiEnergyBar.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using Godot;
using OmegaSpiral.Source.Scripts.Combat.Battlers;

namespace OmegaSpiral.Source.Scripts.Combat.Ui.BattlerEntry;
/// <summary>
/// Bar representing a <see cref="Battler"/>'s energy points. Each point is a <see cref="UiEnergyPoint"/>.
/// </summary>
[GlobalClass]
public partial class UiBattlerEnergyBar : Node
{
    private readonly PackedScene energyPointScene = GD.Load<PackedScene>("res://source/combat/ui/battler_entry/UiEnergyPoint.tscn");

    private int maxValue;

    /// <summary>
    /// Gets or sets the maximum number of energy points that a <see cref="Battler"/> may accumulate.
    /// </summary>
    public int MaxValue
    {
        get => this.maxValue;
        set
        {
            this.maxValue = value;

            for (int i = 0; i < this.maxValue; i++)
            {
                var newPoint = this.energyPointScene.Instantiate() as UiEnergyPoint;
                this.AddChild(newPoint);
            }
        }
    }

    private int value;

    /// <summary>
    /// Gets or sets the number of energy points currently available to a given <see cref="Battler"/>.
    /// </summary>
    public int Value
    {
        get => this.value;
        set
        {
            int oldValue = this.value;
            this.value = Mathf.Clamp(value, 0, this.maxValue);

            // If we have more points, we need to play the "appear" animation on the added points only.
            // That's why we generate a range of indices from `oldValue` to `value`.
            if (this.value > oldValue)
            {
                for (int i = oldValue; i < this.value; i++)
                {
                    this.GetChild<UiEnergyPoint>(i).Appear();
                }
            }

            // Otherwise, flag which points need to "disappear".
            else
            {
                for (int i = oldValue; i > this.value; i--)
                {
                    this.GetChild<UiEnergyPoint>(i - 1).Disappear();
                }
            }
        }
    }

    private int selectedPointCount;

    /// <summary>
    /// Gets or sets the number of points currently selected, often shown when previewing an action.
    /// </summary>
    public int SelectedPointCount
    {
        get => this.selectedPointCount;
        set
        {
            int oldValue = this.selectedPointCount;
            this.selectedPointCount = Mathf.Clamp(value, 0, this.maxValue);
            if (this.selectedPointCount > oldValue)
            {
                for (int i = oldValue; i < this.selectedPointCount; i++)
                {
                    this.GetChild<UiEnergyPoint>(i).Select();
                }
            }
            else
            {
                for (int i = oldValue; i > this.selectedPointCount; i--)
                {
                    this.GetChild<UiEnergyPoint>(i - 1).Deselect();
                }
            }
        }
    }

    /// <summary>
    /// Setup the energy bar with max energy and starting energy.
    /// </summary>
    /// <param name="maxEnergy">The maximum energy.</param>
    /// <param name="startEnergy">The starting energy.</param>
    public void Setup(int maxEnergy, int startEnergy)
    {
        this.MaxValue = maxEnergy;
        this.Value = startEnergy;
    }
}
