using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

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
    private Battler _battler;
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

            // If the battler currently choosing the action dies, close and free the menu.
            _battler.HealthDepleted += async () =>
            {
                await Close();
                CombatEvents.PlayerBattlerSelected?.Invoke(null);
            };

            // If the battler's energy levels changed, re-evaluate which actions are available.
            _battler.Stats.EnergyChanged += () =>
            {
                foreach (var entry in _entries.OfType<UIActionButton>())
                {
                    bool canUseAction = _battler.Stats.Energy >= entry.Action.EnergyCost;
                    entry.Disabled = !canUseAction || IsDisabled;
                }
            };
        }
    }

    // Refer to the BattlerList to check whether or not an action is valid when it is selected.
    // This allows us to prevent the player from selecting an invalid action.
    private BattlerList _battlerList;

    public override void _Ready()
    {
        Hide();
        SetProcessUnhandledInput(false);
    }

    // Capture any input events that will signal going "back" in the menu hierarchy.
    // This includes mouse or touch input outside of a menu or pressing the back button/key.
    public override void _UnhandledInput(InputEvent @event)
    {
        if (IsDisabled)
        {
            return;
        }

        if (@event.IsActionReleased("select") || @event.IsActionReleased("back"))
        {
            // Note: In C#, we can't use async void methods directly in event handlers
            // We'll use a helper method that calls the async close method
            HandleClose();
        }
    }

    private async void HandleClose()
    {
        await Close();
        CombatEvents.PlayerBattlerSelected?.Invoke(null);
    }

    /// <summary>
    /// Create the action menu, creating an entry for each <see cref="BattlerAction"/> (valid or otherwise) available
    /// to the selected <see cref="Battler"/>.
    /// These actions are validated at run-time as they are selected in the menu.
    /// </summary>
    /// <param name="selectedBattler">The battler whose actions to display</param>
    /// <param name="battlerList">The battler list to check action validity</param>
    public void Setup(Battler selectedBattler, BattlerList battlerList)
    {
        Battler = selectedBattler;
        _battlerList = battlerList;
        BuildActionMenu();

        Show();
        FadeIn();
    }

    public new async void FadeIn()
    {
        await base.FadeIn();
        SetProcessUnhandledInput(true);
    }

    public async void Close()
    {
        SetProcessUnhandledInput(false);
        await FadeOut();
        QueueFree();
    }

    // Populate the menu with a list of actions.
    private void BuildActionMenu()
    {
        foreach (var action in _battler.Actions)
        {
            bool canUseAction = _battler.Stats.Energy >= action.EnergyCost;

            var newEntry = CreateEntry() as UIActionButton;
            newEntry.Action = action;
            newEntry.Disabled = !canUseAction || IsDisabled;
            newEntry.FocusNeighborRight = "."; // Don't allow input to jump to the player battler list.

            newEntry.FocusEntered += () =>
            {
                if (!IsDisabled)
                {
                    EmitSignal(SignalName.ActionFocused, newEntry.Action);
                }
            };
        }

        LoopFirstAndLastEntries();
    }

    // Allow the player to select an action for the specified Battler.
    protected void OnEntryPressed(BaseButton entry)
    {
        var actionEntry = entry as UIActionButton;
        var action = actionEntry.Action;

        // First of all, check to make sure that the action has valid targets. If it does
        // not, do not allow selection of the action.
        if (action.GetPossibleTargets(_battler, _battlerList).Count == 0)
        {
            // Normally, the button gives up focus when selected (to stop cycling menu during animation).
            // However, the action is invalid and so the menu needs to keep focus for the player to
            // select another action.
            entry.CallDeferred("grab_focus");
            return;
        }

        // There are available targets, so the UI can move on to selecting targets.
        // Note: In C#, we can't use async in overridden methods, so we'll use a helper
        HandleActionSelected(action);
    }

    private async void HandleActionSelected(BattlerAction action)
    {
        await Close();
        EmitSignal(SignalName.ActionSelected, action);
    }
}
