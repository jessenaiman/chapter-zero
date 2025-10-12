using Godot;
using System;
using System.Collections.Generic;

/// <summary>
/// An entry in the <see cref="UIPlayerBattlerList"/> for one of the player's <see cref="Battler"/>s.
/// </summary>
public partial class UIBattlerEntry : TextureButton
{
    private UIBattlerEnergyBar _energy;
    private UIBattlerLifeBar _life;

    private Battler _battler;
    /// <summary>
    /// Setup the entry UI values and connect to different changes in <see cref="BattlerStats"/> that the UI will
    /// measure.
    /// </summary>
    public Battler Battler
    {
        get => _battler;
        set
        {
            _battler = value;

            if (!IsInsideTree())
            {
                // We'll set the value and wait for the node to be ready
                _battler = value;
                return;
            }

            _energy.Setup(_battler.Stats.MaxEnergy, _battler.Stats.Energy);
            _life.Setup(_battler.Name, _battler.Stats.MaxHealth, _battler.Stats.Health);

            _battler.Stats.EnergyChanged += () => _energy.Value = _battler.Stats.Energy;
            _battler.Stats.HealthChanged += () =>
            {
                _life.TargetValue = _battler.Stats.Health;
                Disabled = _battler.Stats.Health <= 0;

                // If the Battler has been downed, it no longer has a cached action so the preview
                // can be removed.
                if (Disabled)
                {
                    _life.SetActionIcon(null);
                }
            };

            // Once the player has started to act, remove the action preview icon. The icon only exists
            // to help the player with their battlefield strategy.
            _battler.ReadyToAct += () =>
            {
                _life.SetActionIcon(null);
            };
        }
    }

    public override void _Ready()
    {
        _energy = GetNode<UIBattlerEnergyBar>("VBoxContainer/CenterContainer/EnergyBar");
        _life = GetNode<UIBattlerLifeBar>("VBoxContainer/LifeBar");

        // If Battler was set before the node was ready, apply it now
        if (_battler != null)
        {
            _energy.Setup(_battler.Stats.MaxEnergy, _battler.Stats.Energy);
            _life.Setup(_battler.Name, _battler.Stats.MaxHealth, _battler.Stats.Health);

            _battler.Stats.EnergyChanged += () => _energy.Value = _battler.Stats.Energy;
            _battler.Stats.HealthChanged += () =>
            {
                _life.TargetValue = _battler.Stats.Health;
                Disabled = _battler.Stats.Health <= 0;

                // If the Battler has been downed, it no longer has a cached action so the preview
                // can be removed.
                if (Disabled)
                {
                    _life.SetActionIcon(null);
                }
            };

            // Once the player has started to act, remove the action preview icon. The icon only exists
            // to help the player with their battlefield strategy.
            _battler.ReadyToAct += () =>
            {
                _life.SetActionIcon(null);
            };
        }

        // If the player queues an action for this Battler, display the queued action's icon next to the
        // Battler name and health points information.
        CombatEvents.ActionSelected += (action, source, targets) =>
        {
            if (source == _battler)
            {
                if (action != null)
                {
                    _life.SetActionIcon(action.Icon);
                }
                else
                {
                    _life.SetActionIcon(null);
                }
            }
        };
    }
}
