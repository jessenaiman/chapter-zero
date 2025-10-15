// <copyright file="UIActionMenu.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using OmegaSpiral.Source.Scripts.Combat.Actions;
using OmegaSpiral.Combat.Battlers;
using OmegaSpiral.Combat;

/// <summary>
/// A menu lists a <see cref="Battler"/>'s <see cref="Battler.Actions"/>, allowing the player to select one.
/// </summary>
public partial class UIActionMenu : UIListMenu
{
    private Battler? battler;

    /// <summary>
    /// Reference to the <see cref="BattlerList"/> used to check whether an action is valid when selected,
    /// preventing the player from selecting an invalid action.
    /// </summary>
    private BattlerList? battlerList;

    /// <summary>
    /// Emitted when a player has selected an action and the menu has faded to transparent.
    /// </summary>
    /// <param name="action">The selected battler action.</param>
    [Signal]
    public delegate void ActionSelectedEventHandler(BattlerAction action);

    /// <summary>
    /// Emitted whenever a new action is focused on the menu.
    /// </summary>
    /// <param name="action">The focused battler action.</param>
    [Signal]
    public delegate void ActionFocusedEventHandler(BattlerAction action);

    /// <summary>
    /// Gets or sets the battler associated with this action menu.
    /// </summary>
    public Battler? Battler
    {
        get => this.battler;
        set
        {
            this.battler = value;

            if (!this.IsInsideTree() || this.battler == null)
            {
                // We'll set the value and wait for the node to be ready
                this.battler = value;
                return;
            }

            // If the battler currently choosing the action dies, close and free the menu.
            this.battler.HealthDepleted += async () => await this.CloseAsync().ConfigureAwait(false);

            // If the battler's energy levels changed, re-evaluate which actions are available.
            this.battler.Stats?.EnergyChanged += () =>
            {
                foreach (var entry in this.entries.OfType<UIActionButton>())
                {
                    bool canUseAction = this.battler?.Stats?.Energy >= entry.Action?.EnergyCost;
                    entry.Disabled = !canUseAction || this.IsDisabled;
                }
            };
        }
    }

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
        if (this.IsDisabled || @event == null)
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

    /// <summary>
    /// Fades in the action menu.
    /// </summary>
    public new void FadeIn()
    {
        _ = this.FadeInAsync();
    }

    /// <summary>
    /// Closes the action menu.
    /// </summary>
    public void Close()
    {
        _ = this.CloseAsync();
    }

    /// <summary>
    /// Handles when an entry is pressed.
    /// </summary>
    /// <param name="entry">The pressed button entry.</param>
    protected override void OnEntryPressed(BaseButton entry)
    {
        var actionEntry = entry as UIActionButton;
        if (actionEntry?.Action == null || this.battler == null || this.battlerList == null)
        {
            return;
        }

        var action = actionEntry.Action;

        // First of all, check to make sure that the action has valid targets. If it does
        // not, do not allow selection of the action.
        var possibleTargets = action.GetPossibleTargets(this.battler, this.battlerList);
        if (possibleTargets == null || possibleTargets.Length == 0)
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

    /// <summary>
    /// Disposes the UIActionMenu and its disposable fields.
    /// </summary>
    /// <param name="disposing">True if disposing managed resources.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            this.battler?.Dispose();
            this.battlerList?.Dispose();
        }

        base.Dispose(disposing);
    }

    private async void HandleClose()
    {
        await this.CloseAsync().ConfigureAwait(false);
    }

    private async Task CloseAsync()
    {
        this.SetProcessUnhandledInput(false);
        this.FadeOut();

        // Wait for the fade out animation to complete
        var anim = this.GetNode<AnimationPlayer>("AnimationPlayer");
        if (anim != null)
        {
            await this.ToSignal(anim, AnimationPlayer.SignalName.AnimationFinished);
        }
        else
        {
            // If no animation player, just wait a frame
            await this.ToSignal(this.GetTree(), SceneTree.SignalName.ProcessFrame);
        }

        this.QueueFree();
    }

    private async Task FadeInAsync()
    {
        base.FadeIn();

        // Wait for the fade in animation to complete
        var anim = this.GetNode<AnimationPlayer>("AnimationPlayer");
        if (anim != null)
        {
            await this.ToSignal(anim, AnimationPlayer.SignalName.AnimationFinished);
        }
        else
        {
            // If no animation player, just wait a frame
            await this.ToSignal(this.GetTree(), SceneTree.SignalName.ProcessFrame);
        }

        this.SetProcessUnhandledInput(true);
    }

    /// <summary>
    /// Populates the menu with a list of actions.
    /// </summary>
    private void BuildActionMenu()
    {
        if (this.battler == null)
        {
            return;
        }

        foreach (var action in this.battler.Actions)
        {
            bool canUseAction = this.battler.Stats?.Energy >= action.EnergyCost;

            var newEntry = this.CreateEntry() as UIActionButton;
            if (newEntry != null)
            {
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
        }

        this.LoopFirstAndLastEntries();
    }

    private async void HandleActionSelected(BattlerAction action)
    {
        await this.CloseAsync().ConfigureAwait(false);
        this.EmitSignal(SignalName.ActionSelected, action);
    }
}
