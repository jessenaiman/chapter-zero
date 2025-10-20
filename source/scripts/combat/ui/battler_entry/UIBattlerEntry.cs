
// <copyright file="UIBattlerEntry.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using Godot;
using OmegaSpiral.Source.Combat.Actions;
using OmegaSpiral.Source.Scripts.Combat.Battlers;

namespace OmegaSpiral.Source.Scripts.Combat.UI.BattlerEntry;
/// <summary>
/// An entry in the <see cref="UIPlayerBattlerList"/> for one of the player's <see cref="Battler"/>s.
/// </summary>
public partial class UIBattlerEntry : TextureButton
{
    private UIBattlerEnergyBar? energy;
    private UIBattlerLifeBar? life;

    private Battler? battler;

    /// <summary>
    /// Gets or sets setup the entry UI values and connect to different changes in <see cref="BattlerStats"/> that the UI will
    /// measure.
    /// </summary>
    public Battler? Battler
    {
        get => this.battler;
        set
        {
            this.battler = value;

            if (!this.IsInsideTree() || this.battler == null || this.energy == null || this.life == null)
            {
                // We'll set the value and wait for the node to be ready
                this.battler = value;
                return;
            }

            if (this.energy != null && this.battler.Stats != null)
            {
                this.energy.Setup(this.battler.Stats.MaxEnergy, this.battler.Stats.Energy);
            }

            if (this.life != null && this.battler.Stats != null)
            {
                this.life.Setup(this.battler.Name, this.battler.Stats.MaxHealth, this.battler.Stats.Health);
            }

            if (this.battler.Stats != null)
            {
                this.battler.Stats.EnergyChanged += () =>
                {
                    if (this.energy != null)
                    {
                        this.energy.Value = this.battler.Stats.Energy;
                    }
                };
            }

            if (this.battler.Stats != null)
            {
                this.battler.Stats.HealthChanged += () =>
                {
                    if (this.life != null)
                    {
                        this.life.TargetValue = this.battler.Stats.Health;
                    }

                    this.Disabled = this.battler.Stats.Health <= 0;

                    // If the Battler has been downed, it no longer has a cached action so the preview
                    // can be removed.
                    if (this.Disabled && this.life != null)
                    {
                        this.life.SetActionIcon(null);
                    }
                };
            }

            // Once the player has started to act, remove the action preview icon. The icon only exists
            // to help the player with their battlefield strategy.
            this.battler.ReadyToAct += () =>
            {
                if (this.life != null)
                {
                    this.life.SetActionIcon(null);
                }
            };
        }
    }

    /// <inheritdoc/>
    public override void _Ready()
    {
        this.energy = this.GetNode<UIBattlerEnergyBar>("VBoxContainer/CenterContainer/EnergyBar");
        this.life = this.GetNode<UIBattlerLifeBar>("VBoxContainer/LifeBar");

        // If Battler was set before the node was ready, apply it now
        if (this.battler != null && this.energy != null && this.life != null)
        {
            if (this.energy != null && this.battler.Stats != null)
            {
                this.energy.Setup(this.battler.Stats.MaxEnergy, this.battler.Stats.Energy);
            }

            if (this.life != null && this.battler.Stats != null)
            {
                this.life.Setup(this.battler.Name, this.battler.Stats.MaxHealth, this.battler.Stats.Health);
            }

            if (this.battler.Stats != null)
            {
                this.battler.Stats.EnergyChanged += () =>
                {
                    if (this.energy != null)
                    {
                        this.energy.Value = this.battler.Stats.Energy;
                    }
                };
            }

            if (this.battler.Stats != null)
            {
                this.battler.Stats.HealthChanged += () =>
                {
                    if (this.life != null)
                    {
                        this.life.TargetValue = this.battler.Stats.Health;
                    }

                    this.Disabled = this.battler.Stats.Health <= 0;

                    // If the Battler has been downed, it no longer has a cached action so the preview
                    // can be removed.
                    if (this.Disabled && this.life != null)
                    {
                        this.life.SetActionIcon(null);
                    }
                };
            }

            // Once the player has started to act, remove the action preview icon. The icon only exists
            // to help the player with their battlefield strategy.
            this.battler.ReadyToAct += () =>
            {
                if (this.life != null)
                {
                    this.life.SetActionIcon(null);
                }
            };
        }

        // If the player queues an action for this Battler, display the queued action's icon next to the
        // Battler name and health points information.
        var combatEvents = this.GetNode("/root/CombatEvents");
        if (combatEvents != null)
        {
            combatEvents.Connect("action_selected", Callable.From((BattlerAction action, Battler source, Battler[] _) =>
            {
                if (source == this.battler && this.life != null)
                {
                    if (action != null)
                    {
                        this.life.SetActionIcon(action.Icon);
                    }
                    else
                    {
                        this.life.SetActionIcon(null);
                    }
                }
            }));
        }
    }
}
