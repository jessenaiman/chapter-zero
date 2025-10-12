using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// The player combat menus coordinate all player input during the combat game-state.
///
/// The menus are largely signal driven, which are emitted according to player input. The player is
/// responsible for issuing <see cref="BattlerAction"/>s to their respective <see cref="Battler"/>s, which are acted out in
/// order by the <see cref="ActiveTurnQueue"/>.
///
/// Actions are issued according to the following steps:
///     - The player selects one of their Battlers from the <see cref="UIPlayerBattlerList"/>.
///     - A <see cref="UIActionMenu"/> appears, which allows the player to select a valid action.
///     - Finally, potential targets are navigated using a <see cref="UIBattlerTargetingCursor"/>.
/// The player may traverse the menus, going backwards and forwards through the menus as needed.
/// Once the player has picked an action and targets, it is assigned to the queue by means of the
/// <see cref="CombatEvents.ActionSelected"/> global signal.
/// </summary>
public partial class UICombatMenus : Control
{
    /// <summary>
    /// The action menu scene that will be created whenever the player needs to select an action.
    /// </summary>
    [Export]
    public PackedScene ActionMenuScene { get; set; }

    /// <summary>
    /// The targeting cursor scene that will be created whenever the player needs to choose targets.
    /// </summary>
    [Export]
    public PackedScene TargetCursorScene { get; set; }

    // The action menu/targeting cursor are created/freed dynamically. We'll track the combat participant
    // data so that it can be fed into the action menu and targeting cursor on creation.
    private BattlerList _battlers;

    // The UI is responsible for relaying player input to the combat systems. In this case, we want to
    // track which battler and action are currently selected, so that we may queue orders for player
    // battlers once the player has selected an action and targets.
    // One caveat is that the selected battler may die while the player is setting up an action, in which
    // case we want the menus to close immediately.
    private Battler _selectedBattler = null;
    public Battler SelectedBattler
    {
        get => _selectedBattler;
        set
        {
            _selectedBattler = value;
            if (_selectedBattler == null)
            {
                _selectedAction = null;
            }
        }
    }

    // Keep track of which action the player is currently building. This is relevant whenever the player
    // is choosing targets.
    private BattlerAction _selectedAction = null;

    // Keep reference to the active targeting cursor. If no cursor is active, the value is null.
    // This allows the cursor targets to be updated in real-time as Battler states change.
    private UIBattlerTargetingCursor _cursor = null;

    private UIActionDescription _actionDescription;
    private Control _actionMenuAnchor;
    private UIPlayerBattlerList _battlerList;

    public override void _Ready()
    {
        _actionDescription = GetNode<UIActionDescription>("ActionDescription");
        _actionMenuAnchor = GetNode<Control>("ActionMenuAnchor");
        _battlerList = GetNode<UIPlayerBattlerList>("PlayerBattlerList");
    }

    /// <summary>
    /// Prepare the menus for use by assigning appropriate <see cref="Battler"/> data.
    /// </summary>
    /// <param name="battlerData">The battler list to use</param>
    public void Setup(BattlerList battlerData)
    {
        _battlers = battlerData;
        _battlerList.Setup(_battlers);

        _battlers.BattlersDowned += _battlerList.FadeOut;

        // If a player battler has been selected, the action menu should open so that the player may
        // choose an action.
        // If the selected Battler had already queued an action, the player must rechoose that
        // action.
        CombatEvents.PlayerBattlerSelected += (battler) =>
        {
            // Reset the action description bar.
            _actionDescription.Description = "";

            SelectedBattler = battler;
            if (SelectedBattler != null)
            {
                CreateActionMenu();

                // There is a chance that the player had already selected an action for this Battler
                // and now wants to change it. In that case, unqueue the action through the proper
                // CombatEvents signal.
                // Note that the targets parameter must be cast to the correct array type.
                List<Battler> emptyTargetArray = new List<Battler>();
                CombatEvents.ActionSelected?.Invoke(null, SelectedBattler, emptyTargetArray);
            }
        };

        // If there is a change in Battler states (for now, only consider a change in health points),
        // re-evaluate the targeting cursor's target list, if the cursor is currently active.
        foreach (var battler in battlerData.GetAllBattlers())
        {
            battler.Stats.HealthChanged += OnBattlerHealthChanged;
        }

        // If a player Battler dies while the player is selecting an action or choosing targets, signal
        // that the targeting cursor/menu should close.
        foreach (var battler in battlerData.Players)
        {
            battler.HealthDepleted += (downedBattler) =>
            {
                if (downedBattler == SelectedBattler)
                {
                    CombatEvents.PlayerBattlerSelected?.Invoke(null);
                }
            };
        }
    }

    private void CreateActionMenu()
    {
        if (SelectedBattler == null)
        {
            GD.PrintErr("Trying to create the action menu without a selected Battler!");
            return;
        }

        var actionMenu = ActionMenuScene.Instantiate() as UIActionMenu;
        _actionMenuAnchor.AddChild(actionMenu);
        actionMenu.Setup(SelectedBattler, _battlers);

        // On combat end, remove the action menu immediately.
        _battlers.BattlersDowned += actionMenu.FadeOut;

        // Link the action menu to the action description bar.
        actionMenu.ActionFocused += (action) =>
        {
            _actionDescription.Description = action.Description;
        };

        // The action builder will wait until the player selects an action or presses 'back'.
        // Selecting an action will trigger the following signal, whereas pressing 'back'
        // will close the menu directly and deselect the current battler.
        actionMenu.ActionSelected += (action) =>
        {
            _selectedAction = action;
            CreateTargetingCursor();
        };
    }

    private void CreateTargetingCursor()
    {
        if (_selectedAction == null)
        {
            GD.PrintErr("Trying to create the targeting cursor without a selected action!");
            return;
        }

        // Create the cursor which will respond to player input and allow choosing a target.
        _cursor = TargetCursorScene.Instantiate() as UIBattlerTargetingCursor;
        _cursor.TargetsAll = _selectedAction.TargetsAll();
        _cursor.Targets = _selectedAction.GetPossibleTargets(SelectedBattler, _battlers);
        AddChild(_cursor);

        // On combat end, remove the cursor from the scene tree.
        _battlers.BattlersDowned += _cursor.QueueFree;

        // Finally, connect to the cursor's signals that will indicate that targets have been chosen.
        _cursor.TargetsSelected += (targets) =>
        {
            // The cursor will be freed after emitting this signal. Remove reference to the cursor.
            _cursor = null;

            if (targets.Count > 0)
            {
                // At this point, the player should have selected a valid action and assigned it
                // targets, so the action may be cached for whenever the battler is ready.
                CombatEvents.ActionSelected?.Invoke(_selectedAction, SelectedBattler, targets.ToArray());

                // The player has properly queued an action. Return the UI to the state where the
                // player will pick a player Battler.
                CombatEvents.PlayerBattlerSelected?.Invoke(null);
            }
            else
            {
                _selectedAction = null;
                CreateActionMenu();
            }
        };
    }

    private void OnBattlerHealthChanged()
    {
        if (_cursor != null)
        {
            _cursor.Targets = _selectedAction.GetPossibleTargets(SelectedBattler, _battlers);
        }
    }
}
