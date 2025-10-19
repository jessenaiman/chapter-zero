
// Copyright (c) Ωmega Spiral. All rights reserved.

using System.Collections.Generic;
using Godot;
using OmegaSpiral.Source.Scripts.Combat.Battlers;

namespace OmegaSpiral.Source.Scripts.Combat.UI;
/// <summary>
/// Container for the player combat HUD.
/// The UICombatHud manages the display of combat-related information during battles,
/// including battler status, health bars, action points, and other real-time combat data.
/// It provides a heads-up display that helps players make informed decisions during combat.
/// </summary>
[GlobalClass]
public partial class UICombatHud : Control
{
    /// <summary>
    /// Emitted when the player selects a battler to view details for.
    /// </summary>
    /// <param name="battler">The battler that was selected.</param>
    [Signal]
    public delegate void BattlerSelectedEventHandler(Battler battler);

    /// <summary>
    /// Gets the list of combat participants.
    /// </summary>
    public BattlerList Battlers { get; private set; } = null!;

    /// <summary>
    /// Gets or sets the currently selected battler.
    /// </summary>
    public Battler? SelectedBattler
    {
        get => this.selectedBattler;
        set
        {
            if (this.selectedBattler != value)
            {
                this.selectedBattler = value;
                this.OnSelectedBattlerChanged();
            }
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether whether the HUD is currently visible.
    /// </summary>
    public bool HudVisible
    {
        get => this.Visible;
        set => this.Visible = value;
    }

    /// <summary>
    /// The container for player battler displays.
    /// </summary>
    private Control? playerBattlersContainer;

    /// <summary>
    /// The container for enemy battler displays.
    /// </summary>
    private Control? enemyBattlersContainer;

    /// <summary>
    /// The detailed battler info panel.
    /// </summary>
    private Control? battlerInfoPanel;

    /// <summary>
    /// Dictionary mapping battlers to their UI displays.
    /// </summary>
    private readonly Dictionary<Battler, Control> battlerDisplays = new();

    /// <summary>
    /// The currently selected battler.
    /// </summary>
    private Battler? selectedBattler;

    /// <inheritdoc/>
    public override void _Ready()
    {
        // Get references to child UI elements
        this.playerBattlersContainer = this.GetNode<Control>("PlayerBattlers");
        this.enemyBattlersContainer = this.GetNode<Control>("EnemyBattlers");
        this.battlerInfoPanel = this.GetNode<Control>("BattlerInfo");

        // Initially hide the HUD
        this.Visible = false;

        // Connect to any necessary signals
        ConnectSignals();
    }

    /// <summary>
    /// Connect to necessary signals.
    /// </summary>
    private static void ConnectSignals()
    {
        // Connect to combat events
        // CombatEvents.BattlerSelected += OnBattlerSelected;
        // CombatEvents.BattlerStatsChanged += OnBattlerStatsChanged;
    }

    /// <summary>
    /// Setup the UI combat HUD with the given battler list.
    /// </summary>
    /// <param name="battlers">The list of combat participants.</param>
    public void Setup(BattlerList battlers)
    {
        this.Battlers = battlers;

        // Clear any existing battler displays
        this.ClearBattlerDisplays();

        // Create displays for all battlers
        if (this.Battlers.Players != null)
        {
            foreach (var player in this.Battlers.Players)
            {
                this.CreateBattlerDisplay(player, this.playerBattlersContainer);
            }
        }

        if (this.Battlers.Enemies != null)
        {
            foreach (var enemy in this.Battlers.Enemies)
            {
                this.CreateBattlerDisplay(enemy, this.enemyBattlersContainer);
            }
        }

        // Show the HUD
        this.Visible = true;
    }

    /// <summary>
    /// Clear all battler displays.
    /// </summary>
    private void ClearBattlerDisplays()
    {
        // Remove all existing battler displays
        foreach (var display in this.battlerDisplays.Values)
        {
            display.QueueFree();
        }

        this.battlerDisplays.Clear();

        // Clear containers
        if (this.playerBattlersContainer != null)
        {
            foreach (var child in this.playerBattlersContainer.GetChildren())
            {
                child.QueueFree();
            }
        }

        if (this.enemyBattlersContainer != null)
        {
            foreach (var child in this.enemyBattlersContainer.GetChildren())
            {
                child.QueueFree();
            }
        }
    }

    /// <summary>
    /// Create a display for a battler.
    /// </summary>
    /// <param name="battler">The battler to create a display for.</param>
    /// <param name="container">The container to add the display to.</param>
    private void CreateBattlerDisplay(Battler battler, Control? container)
    {
        if (battler == null || container == null)
        {
            return;
        }

        // Create a new battler display control
        var display = new Control();
        display.Name = battler.Name;

        // Add the display to the container
        container.AddChild(display);

        // Store the display in the dictionary
        this.battlerDisplays[battler] = display;

        // Connect to the battler's signals
        if (battler.Stats != null)
        {
            battler.Stats.HealthChanged += () => this.OnBattlerHealthChanged(battler);
            battler.Stats.EnergyChanged += () => this.OnBattlerEnergyChanged(battler);
        }

        // Set up the display with initial values
        UpdateBattlerDisplay(battler, display);

        // Connect input events to allow selecting the battler
        display.GuiInput += (inputEvent) => this.OnBattlerDisplayInput(battler, inputEvent);
    }

    /// <summary>
    /// Update a battler display with current values.
    /// </summary>
    /// <param name="battler">The battler to update the display for.</param>
    /// <param name="display">The display to update.</param>
    private static void UpdateBattlerDisplay(Battler? battler, Control? display)
    {
        if (battler == null || display == null || battler.Stats == null)
        {
            return;
        }

        // Update the display with the battler's current stats
        // This would typically involve updating labels, progress bars, etc.
        // The exact implementation depends on the UI structure being used.

        // For example:
        // var nameLabel = display.GetNode<Label>("Name");
        // nameLabel.Text = battler.Name;

        // var healthBar = display.GetNode<TextureProgressBar>("HealthBar");
        // healthBar.Value = battler.Stats.Health;
        // healthBar.MaxValue = battler.Stats.MaxHealth;

        // var energyBar = display.GetNode<TextureProgressBar>("EnergyBar");
        // energyBar.Value = battler.Stats.Energy;
        // energyBar.MaxValue = battler.Stats.MaxEnergy;
    }

    /// <summary>
    /// Update the detailed battler info panel.
    /// </summary>
    /// <param name="battler">The battler to display info for.</param>
    private void UpdateBattlerInfoPanel(Battler? battler)
    {
        if (battler == null || this.battlerInfoPanel == null || battler.Stats == null)
        {
            return;
        }

        // Update the detailed info panel with the battler's stats
        // This would typically involve updating labels, stats display, etc.
        // The exact implementation depends on the UI structure being used.

        // For example:
        // var nameLabel = battlerInfoPanel.GetNode<Label>("Name");
        // nameLabel.Text = battler.Name;

        // var healthLabel = battlerInfoPanel.GetNode<Label>("Health");
        // healthLabel.Text = $"Health: {battler.Stats.Health}/{battler.Stats.MaxHealth}";

        // var attackLabel = battlerInfoPanel.GetNode<Label>("Attack");
        // attackLabel.Text = $"Attack: {battler.Stats.Attack}";

        // var defenseLabel = battlerInfoPanel.GetNode<Label>("Defense");
        // defenseLabel.Text = $"Defense: {battler.Stats.Defense}";
    }

    /// <summary>
    /// Callback when a battler's health changes.
    /// </summary>
    /// <param name="battler">The battler whose health changed.</param>
    private void OnBattlerHealthChanged(Battler battler)
    {
        if (this.battlerDisplays.TryGetValue(battler, out Control? value))
        {
            UpdateBattlerDisplay(battler, value);
        }

        // If this is the selected battler, also update the info panel
        if (battler == this.SelectedBattler)
        {
            this.UpdateBattlerInfoPanel(battler);
        }
    }

    /// <summary>
    /// Callback when a battler's energy changes.
    /// </summary>
    /// <param name="battler">The battler whose energy changed.</param>
    private void OnBattlerEnergyChanged(Battler battler)
    {
        if (this.battlerDisplays.TryGetValue(battler, out Control? value))
        {
            UpdateBattlerDisplay(battler, value);
        }

        // If this is the selected battler, also update the info panel
        if (battler == this.SelectedBattler)
        {
            this.UpdateBattlerInfoPanel(battler);
        }
    }

    /// <summary>
    /// Callback when input is received on a battler display.
    /// </summary>
    /// <param name="battler">The battler associated with the display.</param>
    /// <param name="inputEvent">The input event.</param>
    private void OnBattlerDisplayInput(Battler battler, InputEvent inputEvent)
    {
        if (inputEvent is InputEventMouseButton mouseEvent && mouseEvent.Pressed)
        {
            // Select the battler when clicked
            this.SelectedBattler = battler;
            this.EmitSignal(SignalName.BattlerSelected, battler);
        }
    }

    /// <summary>
    /// Callback when the selected battler changes.
    /// </summary>
    private void OnSelectedBattlerChanged()
    {
        // Update the battler info panel with the selected battler's details
        this.UpdateBattlerInfoPanel(this.SelectedBattler);

        // Highlight the selected battler's display
        this.HighlightSelectedBattler();
    }

    /// <summary>
    /// Highlight the selected battler's display.
    /// </summary>
    private void HighlightSelectedBattler()
    {
        // Remove highlight from all battler displays
        foreach (var displayItem in this.battlerDisplays.Values)
        {
            // Remove highlight styling
            // displayItem.Modulate = Colors.White;
        }

        // Add highlight to the selected battler's display
        if (this.SelectedBattler != null && this.battlerDisplays.TryGetValue(this.SelectedBattler, out Control? selectedDisplay))
        {

            // Add highlight styling
            // selectedDisplay.Modulate = Colors.Yellow;
        }
    }

    /// <summary>
    /// Hide the combat HUD.
    /// </summary>
    public void HideHUD()
    {
        this.Visible = false;
    }

    /// <summary>
    /// Show the combat HUD.
    /// </summary>
    public void ShowHUD()
    {
        this.Visible = true;
    }

    /// <summary>
    /// Update all battler displays.
    /// </summary>
    public void UpdateAllDisplays()
    {
        foreach (var kvp in this.battlerDisplays)
        {
            UpdateBattlerDisplay(kvp.Key, kvp.Value);
        }

        // If there's a selected battler, update the info panel
        if (this.SelectedBattler != null)
        {
            this.UpdateBattlerInfoPanel(this.SelectedBattler);
        }
    }

    /// <summary>
    /// Get the display for a specific battler.
    /// </summary>
    /// <param name="battler">The battler to get the display for.</param>
    /// <returns>The display control for the battler, or null if not found.</returns>
    public Control? GetBattlerDisplay(Battler? battler)
    {
        if (battler == null)
        {
            return null;
        }

        return this.battlerDisplays.TryGetValue(battler, out var display) ? display : null;
    }

    /// <summary>
    /// Refresh the HUD with current battler data.
    /// </summary>
    public void Refresh()
    {
        // Clear and recreate all battler displays
        this.ClearBattlerDisplays();

        if (this.Battlers != null)
        {
            if (this.Battlers.Players != null)
            {
                foreach (var player in this.Battlers.Players)
                {
                    this.CreateBattlerDisplay(player, this.playerBattlersContainer);
                }
            }

            if (this.Battlers.Enemies != null)
            {
                foreach (var enemy in this.Battlers.Enemies)
                {
                    this.CreateBattlerDisplay(enemy, this.enemyBattlersContainer);
                }
            }
        }

        // Update all displays
        this.UpdateAllDisplays();
    }

    /// <summary>
    /// Show a message in the HUD.
    /// </summary>
    /// <param name="message">The message to show.</param>
    /// <param name="duration">The duration to show the message for.</param>
    public async void ShowMessage(string message, float duration = 2.0f)
    {
        _ = message;
        // Show a temporary message in the HUD
        // This would typically involve showing a label or panel with the message
        // For example:
        // var messageLabel = GetNode<Label>("MessageLabel");
        // messageLabel.Text = message;
        // messageLabel.Show();
        // Wait for the specified duration
        await this.ToSignal(this.GetTree().CreateTimer(duration), Godot.Timer.SignalName.Timeout);
        // Hide the message
        // messageLabel.Hide();
    }

    /// <summary>
    /// Show an effect label (like damage numbers or healing amounts).
    /// </summary>
    /// <param name="text">The text to show.</param>
    /// <param name="position">The position to show the text at.</param>
    /// <param name="color">The color of the text.</param>
    public static void ShowEffectLabel(string text, Vector2 position, Color color)
    {
        _ = text;
        _ = position;
        _ = color;
        // Show a floating label at the specified position
        // This would typically involve creating a temporary label that floats upward and fades out
        // For example:
        // var label = new Label();
        // label.Text = text;
        // label.AddThemeColorOverride("font_color", color);
        // label.Position = position;
        // AddChild(label);
        // Create a tween to animate the label
        // var tween = CreateTween();
        // tween.TweenProperty(label, "position:y", position.Y - 50, 1.0f);
        // tween.Parallel().TweenProperty(label, "modulate:a", 0.0f, 1.0f);
        // tween.TweenCallback(new Callable(label, "queue_free"));
    }

    /// <summary>
    /// Update the turn order display.
    /// </summary>
    /// <param name="turnOrder">The current turn order.</param>
    public static void UpdateTurnOrder(List<Battler> turnOrder)
    {
        _ = turnOrder;
        // Update the display showing the turn order
        // This would typically involve updating icons or names showing who is next to act
        // For example:
        // var turnOrderContainer = GetNode<HBoxContainer>("TurnOrder");
        // foreach (var child in turnOrderContainer.GetChildren())
        // {
        //     child.QueueFree();
        // }
        // foreach (var battler in turnOrder)
        // {
        //     var icon = new TextureRect();
        //     icon.Texture = battler.Icon;
        //     turnOrderContainer.AddChild(icon);
        // }
    }
}
