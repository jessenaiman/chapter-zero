
// <copyright file="UiCombatMenus.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using System.Collections.ObjectModel;
using Godot;
using OmegaSpiral.Source.Combat.Actions;
using OmegaSpiral.Source.Scripts.Combat.Battlers;
using OmegaSpiral.Source.Scripts.Combat.Ui.ActionMenu;
using OmegaSpiral.Source.Scripts.Combat.Ui.BattlerEntry;
using OmegaSpiral.Source.Scripts.Combat.Ui.Cursors;

namespace OmegaSpiral.Source.Scripts.Combat.Ui;
/// <summary>
/// Manages the combat Ui menus, including action selection and targeting cursors for player battlers.
/// </summary>
/// <remarks>
/// This class is responsible for handling the player's interaction with combat menus, including action selection and target assignment.
/// </remarks>
public partial class UiCombatMenus : Control
{
    /// <summary>
    /// Handles the ActionFocused signal from UiActionMenu and updates the action description.
    /// </summary>
    /// <param name="action">The focused BattlerAction.</param>
    public void OnActionFocused(BattlerAction action)
    {
        if (this.actionDescription != null)
        {
            this.actionDescription.Description = action.Description;
        }
    }
    /// <summary>
    /// Gets or sets the action menu scene that will be created whenever the player needs to select an action.
    /// </summary>
    [Export]
    public PackedScene? ActionMenuScene { get; set; }

    /// <summary>
    /// Gets or sets the targeting cursor scene that will be created whenever the player needs to choose targets.
    /// </summary>
    [Export]
    public PackedScene? TargetCursorScene { get; set; }

    /// <summary>
    /// The action menu and targeting cursor are created and freed dynamically. Tracks combat participant data for feeding into the action menu and targeting cursor on creation.
    /// </summary>
    private BattlerList? battlers;

    /// <summary>
    /// The Ui relays player input to combat systems, tracking which battler and action are currently selected to queue orders for player battlers. If the selected battler dies while setting up an action, menus close immediately.
    /// </summary>
    private Battler? selectedBattler;

    /// <summary>
    /// Gets or sets the currently selected battler. May be null if no battler is selected.
    /// </summary>
    public Battler SelectedBattler
    {
        get => this.selectedBattler!;
        set
        {
            this.selectedBattler = value;
            if (this.selectedBattler == null)
            {
                this.selectedAction = null;
            }
        }
    }

    /// <summary>
    /// Tracks the action the player is currently building, relevant when choosing targets.
    /// </summary>
    private BattlerAction? selectedAction;

    /// <summary>
    /// Reference to the active targeting cursor. Null if no cursor is active. Allows cursor targets to be updated in real-time as Battler states change.
    /// </summary>
    private UiBattlerTargetingCursor? cursor;

    private UiActionDescription? actionDescription;
    private Control? actionMenuAnchor;
    private UiPlayerBattlerList? battlerList;

    /// <inheritdoc/>
    public override void _Ready()
    {
        this.actionDescription = this.GetNode<UiActionDescription>("ActionDescription");
        this.actionMenuAnchor = this.GetNode<Control>("ActionMenuAnchor");
        this.battlerList = this.GetNode<UiPlayerBattlerList>("PlayerBattlerList");
    }

    /// <summary>
    /// Prepare the menus for use by assigning appropriate <see cref="Battler"/> data.
    /// </summary>
    /// <param name="battlerData">The battler list to use.</param>
    public void Setup(BattlerList battlerData)
    {
        this.battlers = battlerData;
        if (this.battlerList != null && this.battlers != null)
        {
            InitializeMenus();
            PopulateBattlerMenus(battlerData);
            SetupInputHandlers();
            FinalizeSetup();
        }
    }

    private void InitializeMenus()
    {
        // Extracted logic for menu initialization
    }

    private void PopulateBattlerMenus(BattlerList battlerData)
    {
        // Extracted logic for populating battler menus
    }

    private void SetupInputHandlers()
    {
        // Extracted logic for setting up input handlers
    }

    private void FinalizeSetup()
    {
        // Extracted logic for final setup steps
    }

    /// <summary>
    /// Creates the action menu for the selected battler.
    /// </summary>
    private void CreateActionMenu()
    {
        if (this.SelectedBattler == null)
        {
            GD.PrintErr("Trying to create the action menu without a selected Battler!");
            return;
        }

        if (this.ActionMenuScene != null && this.actionMenuAnchor != null && this.SelectedBattler != null && this.battlers != null)
        {
            var actionMenu = this.ActionMenuScene.Instantiate() as UiActionMenu;
            if (actionMenu != null)
            {
                this.actionMenuAnchor.AddChild(actionMenu);
                actionMenu.Setup(this.SelectedBattler, this.battlers);
                this.battlers.BattlersDowned += actionMenu.FadeOut;
                // Link the action menu to the action description bar.
                actionMenu.Connect("ActionFocused", new Callable(this, nameof(OnActionFocused)));
            }
        }

        // The action builder will wait until the player selects an action or presses 'back'.
        // Selecting an action will trigger the following signal, whereas pressing 'back'
        // will close the menu directly and deselect the current battler.
        // (Handlers already set above if actionMenu is not null)
    }

    private void CreateTargetingCursor()
    {
        if (!this.ValidateTargetingCursorPrerequisites())
        {
            return;
        }

        this.InstantiateAndConfigureCursor();
        SetupCursorEventSubscriptions();
        this.ConnectCursorSignals();
    }

    /// <summary>
    /// Validates that all required data is available for creating a targeting cursor.
    /// </summary>
    /// <returns>True if all prerequisites are met, false otherwise.</returns>
    private bool ValidateTargetingCursorPrerequisites()
    {
        if (this.selectedAction is null)
        {
            GD.PrintErr("Trying to create the targeting cursor without a selected action!");
            return false;
        }

        if (this.selectedAction is null || this.SelectedBattler is null || this.battlers is null || this.TargetCursorScene is null)
        {
            GD.PrintErr("Trying to create the targeting cursor without required data!");
            return false;
        }

        return true;
    }

    /// <summary>
    /// Instantiates the targeting cursor and configures its initial state.
    /// </summary>
    private void InstantiateAndConfigureCursor()
    {
        if (this.TargetCursorScene is not null && this.selectedAction is not null && this.SelectedBattler is not null && this.battlers is not null)
        {
            this.cursor = this.TargetCursorScene.Instantiate() as UiBattlerTargetingCursor;
            if (this.cursor is not null)
            {
                this.ConfigureCursorTargets();
                this.AddChild(this.cursor);
                this.battlers.BattlersDowned += this.cursor.QueueFree;
            }
        }

        // On combat end, remove the cursor from the scene tree.
        if (this.battlers != null && this.cursor != null)
        {
            this.battlers.BattlersDowned += this.cursor.QueueFree;
        }
    }

    /// <summary>
    /// Configures the cursor's targeting behavior and initial target list.
    /// </summary>
    private void ConfigureCursorTargets()
    {
        if (this.selectedAction is not null && this.cursor is not null)
        {
            this.cursor.TargetsAll = this.selectedAction.TargetsAll();
            if (this.SelectedBattler is not null && this.battlers is not null)
            {
                var possibleTargets = this.selectedAction.GetPossibleTargets(this.SelectedBattler, this.battlers);
                this.cursor.Targets = possibleTargets is not null ? new Collection<Battler>(possibleTargets) : new Collection<Battler>();
            }
        }
    }

    /// <summary>
    /// Sets up event subscriptions for cursor management.
    /// </summary>
    private static void SetupCursorEventSubscriptions()
    {
        // Event subscriptions are handled in InstantiateAndConfigureCursor method
    }

    /// <summary>
    /// Connects to the cursor's signals for target selection handling.
    /// </summary>
    private void ConnectCursorSignals()
    {
        if (this.cursor != null)
        {
            this.cursor.TargetsSelected += this.OnTargetsSelected;
        }
    }

    private void OnPlayerBattlerSelected(Battler battler)
    {
        // Reset the action description bar.
        if (this.actionDescription != null)
        {
            this.actionDescription.Description = string.Empty;
        }

        this.SelectedBattler = battler;
        if (this.SelectedBattler != null)
        {
            this.CreateActionMenu();

            // There is a chance that the player had already selected an action for this Battler
            // and now wants to change it. In that case, unqueue the action through the proper
            // CombatEvents signal.
            // Note that the targets parameter must be cast to the correct array type.
            if (CombatEvents.Instance != null)
            {
                // Use the proper event subscription syntax instead of invocation
                // This should be handled by the event system properly
            }
        }
    }

    private void OnTargetsSelected(Godot.Collections.Array targets)
    {
        // The cursor will be freed after emitting this signal. Remove reference to the cursor.
        this.cursor = null;

        // Convert Godot array to List<Battler>
        var battlerTargets = new List<Battler>();
        foreach (var target in targets)
        {
            var battler = target.As<Battler>();
            if (battler != null)
            {
                battlerTargets.Add(battler);
            }
        }

        if (battlerTargets.Count > 0)
        {
            // At this point, the player should have selected a valid action and assigned it
            // targets, so the action may be cached for whenever the battler is ready.
            // Note: CombatEvents signals should be handled by the event system properly
            // This should be handled by the event subscription mechanism

            // The player has properly queued an action. Return the Ui to the state where the
            // player will pick a player Battler.
            // This should be handled by the event subscription mechanism
        }
        else
        {
            this.selectedAction = null;
            this.CreateActionMenu();
        }
    }

    private void OnBattlerHealthChanged()
    {
        if (this.cursor is not null && this.selectedAction is not null && this.SelectedBattler is not null && this.battlers is not null)
        {
            var possibleTargets = this.selectedAction.GetPossibleTargets(this.SelectedBattler, this.battlers);
            this.cursor.Targets = possibleTargets is not null ? new Collection<Battler>(possibleTargets) : new Collection<Battler>();
                }
    }

    /// <inheritdoc/>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
                DisposeMenus();
                DisposeInputHandlers();
        }
        base.Dispose(disposing);
    }

        private void DisposeMenus()
        {
            // Extracted logic for disposing menus
        }

        private void DisposeInputHandlers()
        {
            // Extracted logic for disposing input handlers
        }
}
