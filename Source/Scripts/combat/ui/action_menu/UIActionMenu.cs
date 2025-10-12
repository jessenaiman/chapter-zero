// <copyright file="UIActionMenu.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

/// <summary>
/// A menu lists a <see cref="Battler"/>'s <see cref="Battler.Actions"/>, allowing the player to select one.
/// </summary>
public partial class UIActionMenu : UIListMenu
{
    /// <summary>
    /// Emitted when a player has selected an action and the menu has faded to transparent.
    /// </summary>
    [Signal]
    public delegate void ActionSelectedEventHandler(BattlerAction action);

    /// <summary>
    /// Emitted whenever a new action is focused on the menu.
    /// </summary>
    [Signal]
    public delegate void ActionFocusedEventHandler(BattlerAction action);

    // The menu tracks the <see cref="BattlerAction"/>s available to a single <see cref="Battler"/>, depending on Battler state
    // (energy costs, for example).
    // The action menu also needs to respond to Battler state, such as a change in energy points or the
    // Battler's health.
    private Battler battler;

    public Battler Battler
    {
        get => this.battler;
        set
        {
            this.battler = value;

            if (!this.IsInsideTree())
            {
                // We'll set the value and wait for the node to be ready
                this.battler = value;
                return;
            }

            // If the battler currently choosing the action dies, close and free the menu.
            this.battler.HealthDepleted += async () =>
            {
                await this.Close();
                CombatEvents.PlayerBattlerSelected?.Invoke(null);
            };

            // If the battler's energy levels changed, re-evaluate which actions are available.
            this.battler.Stats.EnergyChanged += () =>
            {
                foreach (var entry in this.entries.OfType<UIActionButton>())
                {
                    bool canUseAction = this.battler.Stats.Energy >= entry.Action.EnergyCost;
                    entry.Disabled = !canUseAction || this.IsDisabled;
                }
            };
        }
    }

    // Refer to the BattlerList to check whether or not an action is valid when it is selected.
    // This allows us to prevent the player from selecting an invalid action.
    private BattlerList battlerList;

    /// <inheritdoc/>
    public override void _Ready()
    {
        this.Hide();
        this.SetProcessUnhandledInput(false);
    }

    // Capture any input events that will signal going "back" in the menu hierarchy.
    // This includes mouse or touch input outside of a menu or pressing the back button/key.
    /// <inheritdoc/>
    public override void _UnhandledInput(InputEvent @event)
    {
        if (this.IsDisabled)
        {
            return;
        }

        if (@event.IsActionReleased("select") || @event.IsActionReleased("back"))
        {
            // Note: In C#, we can't use async void methods directly in event handlers
            // We'll use a helper method that calls the async close method
            this.HandleClose();
        }
    }

    private async void HandleClose()
    {
        await this.Close();
        CombatEvents.PlayerBattlerSelected?.Invoke(null);
    }

    /// <summary>
    /// Create the action menu, creating an entry for each <see cref="BattlerAction"/> (valid or otherwise) available
    /// to the selected <see cref="Battler"/>.
    /// These actions are validated at run-time as they are selected in the menu.
    /// </summary>
    /// <param name="selectedBattler">The battler whose actions to display.</param>
    /// <param name="battlerList">The battler list to check action validity.</param>
    public void Setup(Battler selectedBattler, BattlerList battlerList)
    {
        this.Battler = selectedBattler;
        this.battlerList = battlerList;
        this.BuildActionMenu();

        this.Show();
        this.FadeIn();
    }

    public new async void FadeIn()
    {
        await base.FadeIn();
        this.SetProcessUnhandledInput(true);
    }

    public async void Close()
    {
        this.SetProcessUnhandledInput(false);
        await this.FadeOut();
        this.QueueFree();
    }

    // Populate the menu with a list of actions.
    private void BuildActionMenu()
    {
        foreach (var action in this.battler.Actions)
        {
            bool canUseAction = this.battler.Stats.Energy >= action.EnergyCost;

            var newEntry = this.CreateEntry() as UIActionButton;
            newEntry.Action = action;
            newEntry.Disabled = !canUseAction || this.IsDisabled;
            newEntry.FocusNeighborRight = "."; // Don't allow input to jump to the player battler list.

            newEntry.FocusEntered += () =>
            {
                if (!this.IsDisabled)
                {
                    this.EmitSignal(SignalName.ActionFocused, newEntry.Action);
                }
            };
        }

        this.LoopFirstAndLastEntries();
    }

    // Allow the player to select an action for the specified Battler.
    protected override void OnEntryPressed(BaseButton entry)
    {
        var actionEntry = entry as UIActionButton;
        var action = actionEntry.Action;

        // First of all, check to make sure that the action has valid targets. If it does
        // not, do not allow selection of the action.
        if (action.GetPossibleTargets(this.battler, this.battlerList).Count == 0)
        {
            // Normally, the button gives up focus when selected (to stop cycling menu during animation).
            // However, the action is invalid and so the menu needs to keep focus for the player to
            // select another action.
            entry.CallDeferred("grab_focus");
            return;
        }

        // There are available targets, so the UI can move on to selecting targets.
        // Note: In C#, we can't use async in overridden methods, so we'll use a helper
        this.HandleActionSelected(action);
    }

    private async void HandleActionSelected(BattlerAction action)
    {
        await this.Close();
        this.EmitSignal(SignalName.ActionSelected, action);
    }
}
