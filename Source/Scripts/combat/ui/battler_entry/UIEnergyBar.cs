using Godot;
using System;

/// <summary>
/// Bar representing a <see cref="Battler"/>'s energy points. Each point is a <see cref="UIEnergyPoint"/>.
/// </summary>
public partial class UIBattlerEnergyBar : Node
{
    private readonly PackedScene EnergyPointScene = GD.Load<PackedScene>("res://src/combat/ui/battler_entry/ui_energy_point.tscn");

    private int _maxValue = 0;
    /// <summary>
    /// The maximum number of energy points that a <see cref="Battler"/> may accumulate.
    /// </summary>
    public int MaxValue
    {
        get => _maxValue;
        set
        {
            _maxValue = value;

            for (int i = 0; i < _maxValue; i++)
            {
                var newPoint = EnergyPointScene.Instantiate() as UIEnergyPoint;
                AddChild(newPoint);
            }
        }
    }

    private int _value = 0;
    /// <summary>
    /// The number of energy points currently available to a given <see cref="Battler"/>.
    /// </summary>
    public int Value
    {
        get => _value;
        set
        {
            int oldValue = _value;
            _value = Mathf.Clamp(value, 0, _maxValue);

            // If we have more points, we need to play the "appear" animation on the added points only.
            // That's why we generate a range of indices from `oldValue` to `value`.
            if (_value > oldValue)
            {
                for (int i = oldValue; i < _value; i++)
                {
                    GetChild<UIEnergyPoint>(i).Appear();
                }
            }
            // Otherwise, flag which points need to "disappear".
            else
            {
                for (int i = oldValue; i > _value; i--)
                {
                    GetChild<UIEnergyPoint>(i - 1).Disappear();
                }
            }
        }
    }

    private int _selectedPointCount = 0;
    /// <summary>
    /// The number of points currently selected, often shown when previewing an action.
    /// </summary>
    public int SelectedPointCount
    {
        get => _selectedPointCount;
        set
        {
            int oldValue = _selectedPointCount;
            _selectedPointCount = Mathf.Clamp(value, 0, _maxValue);
            if (_selectedPointCount > oldValue)
            {
                for (int i = oldValue; i < _selectedPointCount; i++)
                {
                    GetChild<UIEnergyPoint>(i).Select();
                }
            }
            else
            {
                for (int i = oldValue; i > _selectedPointCount; i--)
                {
                    GetChild<UIEnergyPoint>(i - 1).Deselect();
                }
            }
        }
    }

    /// <summary>
    /// Setup the energy bar with max energy and starting energy
    /// </summary>
    /// <param name="maxEnergy">The maximum energy</param>
    /// <param name="startEnergy">The starting energy</param>
    public void Setup(int maxEnergy, int startEnergy)
    {
        MaxValue = maxEnergy;
        Value = startEnergy;
    }
}
