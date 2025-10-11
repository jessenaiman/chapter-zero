using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OmegaSpiral.Source.Scripts.Models;
using OmegaSpiral.Source.Scripts;
using OmegaSpiral.Source.Scripts.Interfaces;

/// <summary>
/// Container for the player combat HUD.
/// The UICombatHud manages the display of combat-related information during battles,
/// including battler status, health bars, action points, and other real-time combat data.
/// It provides a heads-up display that helps players make informed decisions during combat.
/// </summary>
public partial class UICombatHud : Control
{
    /// <summary>
    /// Emitted when the player selects a battler to view details for.
    /// </summary>
    [Signal]
    public delegate void BattlerSelectedEventHandler(Battler battler);

    /// <summary>
    /// The list of combat participants.
    /// </summary>
    public BattlerList Battlers { get; private set; }

    /// <summary>
    /// The currently selected battler.
    /// </summary>
    public Battler SelectedBattler
    {
        get => selectedBattler;
        set
        {
            if (selectedBattler != value)
            {
                selectedBattler = value;
                OnSelectedBattlerChanged();
            }
        }
    }

    /// <summary>
    /// Whether the HUD is currently visible.
    /// </summary>
    public bool HudVisible
    {
        get => Visible;
        set => Visible = value;
    }

    /// <summary>
    /// The container for player battler displays.
    /// </summary>
    private Control playerBattlersContainer;

    /// <summary>
    /// The container for enemy battler displays.
    /// </summary>
    private Control enemyBattlersContainer;

    /// <summary>
    /// The detailed battler info panel.
    /// </summary>
    private Control battlerInfoPanel;

    /// <summary>
    /// Dictionary mapping battlers to their UI displays.
    /// </summary>
    private Dictionary<Battler, Control> battlerDisplays = new Dictionary<Battler, Control>();

    /// <summary>
    /// The currently selected battler.
    /// </summary>
    private Battler selectedBattler;

    public override void _Ready()
    {
        // Get references to child UI elements
        playerBattlersContainer = GetNode<Control>("PlayerBattlers");
        enemyBattlersContainer = GetNode<Control>("EnemyBattlers");
        battlerInfoPanel = GetNode<Control>("BattlerInfo");

        // Initially hide the HUD
        Visible = false;

        // Connect to any necessary signals
        ConnectSignals();
    }

    /// <summary>
    /// Connect to necessary signals.
    /// </summary>
    private void ConnectSignals()
    {
        // Connect to combat events
        // CombatEvents.BattlerSelected += OnBattlerSelected;
        // CombatEvents.BattlerStatsChanged += OnBattlerStatsChanged;
    }

    /// <summary>
    /// Setup the UI combat HUD with the given battler list.
    /// </summary>
    /// <param name="battlers">The list of combat participants</param>
    public void Setup(BattlerList battlers)
    {
        Battlers = battlers;

        // Clear any existing battler displays
        ClearBattlerDisplays();

        // Create displays for all battlers
        if (Battlers.Players != null)
        {
            foreach (var player in Battlers.Players)
            {
                CreateBattlerDisplay(player, playerBattlersContainer);
            }
        }

        if (Battlers.Enemies != null)
        {
            foreach (var enemy in Battlers.Enemies)
            {
                CreateBattlerDisplay(enemy, enemyBattlersContainer);
            }
        }

        // Show the HUD
        Visible = true;
    }

    /// <summary>
    /// Clear all battler displays.
    /// </summary>
    private void ClearBattlerDisplays()
    {
        // Remove all existing battler displays
        foreach (var display in battlerDisplays.Values)
        {
            display.QueueFree();
        }

        battlerDisplays.Clear();

        // Clear containers
        if (playerBattlersContainer != null)
        {
            foreach (var child in playerBattlersContainer.GetChildren())
            {
                child.QueueFree();
            }
        }

        if (enemyBattlersContainer != null)
        {
            foreach (var child in enemyBattlersContainer.GetChildren())
            {
                child.QueueFree();
            }
        }
    }

    /// <summary>
    /// Create a display for a battler.
    /// </summary>
    /// <param name="battler">The battler to create a display for</param>
    /// <param name="container">The container to add the display to</param>
    private void CreateBattlerDisplay(Battler battler, Control container)
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
        battlerDisplays[battler] = display;

        // Connect to the battler's signals
        if (battler.Stats != null)
        {
            battler.Stats.HealthChanged += () => OnBattlerHealthChanged(battler);
            battler.Stats.EnergyChanged += () => OnBattlerEnergyChanged(battler);
        }

        // Set up the display with initial values
        UpdateBattlerDisplay(battler, display);

        // Connect input events to allow selecting the battler
        display.GuiInput += (inputEvent) => OnBattlerDisplayInput(battler, inputEvent);
    }

    /// <summary>
    /// Update a battler display with current values.
    /// </summary>
    /// <param name="battler">The battler to update the display for</param>
    /// <param name="display">The display to update</param>
    private void UpdateBattlerDisplay(Battler battler, Control display)
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
    /// <param name="battler">The battler to display info for</param>
    private void UpdateBattlerInfoPanel(Battler battler)
    {
        if (battler == null || battlerInfoPanel == null || battler.Stats == null)
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
    /// <param name="battler">The battler whose health changed</param>
    private void OnBattlerHealthChanged(Battler battler)
    {
        if (battlerDisplays.ContainsKey(battler))
        {
            UpdateBattlerDisplay(battler, battlerDisplays[battler]);
        }

        // If this is the selected battler, also update the info panel
        if (battler == SelectedBattler)
        {
            UpdateBattlerInfoPanel(battler);
        }
    }

    /// <summary>
    /// Callback when a battler's energy changes.
    /// </summary>
    /// <param name="battler">The battler whose energy changed</param>
    private void OnBattlerEnergyChanged(Battler battler)
    {
        if (battlerDisplays.ContainsKey(battler))
        {
            UpdateBattlerDisplay(battler, battlerDisplays[battler]);
        }

        // If this is the selected battler, also update the info panel
        if (battler == SelectedBattler)
        {
            UpdateBattlerInfoPanel(battler);
        }
    }

    /// <summary>
    /// Callback when input is received on a battler display.
    /// </summary>
    /// <param name="battler">The battler associated with the display</param>
    /// <param name="inputEvent">The input event</param>
    private void OnBattlerDisplayInput(Battler battler, InputEvent inputEvent)
    {
        if (inputEvent is InputEventMouseButton mouseEvent && mouseEvent.Pressed)
        {
            // Select the battler when clicked
            SelectedBattler = battler;
            EmitSignal(SignalName.BattlerSelected, battler);
        }
    }

    /// <summary>
    /// Callback when the selected battler changes.
    /// </summary>
    private void OnSelectedBattlerChanged()
    {
        // Update the battler info panel with the selected battler's details
        UpdateBattlerInfoPanel(SelectedBattler);

        // Highlight the selected battler's display
        HighlightSelectedBattler();
    }

    /// <summary>
    /// Highlight the selected battler's display.
    /// </summary>
    private void HighlightSelectedBattler()
    {
        // Remove highlight from all battler displays
        foreach (var display in battlerDisplays.Values)
        {
            // Remove highlight styling
            // display.Modulate = Colors.White;
        }

        // Add highlight to the selected battler's display
        if (SelectedBattler != null && battlerDisplays.ContainsKey(SelectedBattler))
        {
            var display = battlerDisplays[SelectedBattler];
            // Add highlight styling
            // display.Modulate = Colors.Yellow;
        }
    }

    /// <summary>
    /// Hide the combat HUD.
    /// </summary>
    public void HideHUD()
    {
        Visible = false;
    }

    /// <summary>
    /// Show the combat HUD.
    /// </summary>
    public void ShowHUD()
    {
        Visible = true;
    }

    /// <summary>
    /// Update all battler displays.
    /// </summary>
    public void UpdateAllDisplays()
    {
        foreach (var kvp in battlerDisplays)
        {
            UpdateBattlerDisplay(kvp.Key, kvp.Value);
        }

        // If there's a selected battler, update the info panel
        if (SelectedBattler != null)
        {
            UpdateBattlerInfoPanel(SelectedBattler);
        }
    }

    /// <summary>
    /// Get the display for a specific battler.
    /// </summary>
    /// <param name="battler">The battler to get the display for</param>
    /// <returns>The display control for the battler, or null if not found</returns>
    public Control GetBattlerDisplay(Battler battler)
    {
        if (battler == null)
        {
            return null;
        }

        return battlerDisplays.GetValueOrDefault(battler, null);
    }

    /// <summary>
    /// Refresh the HUD with current battler data.
    /// </summary>
    public void Refresh()
    {
        // Clear and recreate all battler displays
        ClearBattlerDisplays();

        if (Battlers != null)
        {
            if (Battlers.Players != null)
            {
                foreach (var player in Battlers.Players)
                {
                    CreateBattlerDisplay(player, playerBattlersContainer);
                }
            }

            if (Battlers.Enemies != null)
            {
                foreach (var enemy in Battlers.Enemies)
                {
                    CreateBattlerDisplay(enemy, enemyBattlersContainer);
                }
            }
        }

        // Update all displays
        UpdateAllDisplays();
    }

    /// <summary>
    /// Show a message in the HUD.
    /// </summary>
    /// <param name="message">The message to show</param>
    /// <param name="duration">The duration to show the message for</param>
    public async void ShowMessage(string message, float duration = 2.0f)
    {
        // Show a temporary message in the HUD
        // This would typically involve showing a label or panel with the message

        // For example:
        // var messageLabel = GetNode<Label>("MessageLabel");
        // messageLabel.Text = message;
        // messageLabel.Show();

        // Wait for the specified duration
        await ToSignal(GetTree().CreateTimer(duration), Timer.SignalName.Timeout);

        // Hide the message
        // messageLabel.Hide();
    }

    /// <summary>
    /// Show an effect label (like damage numbers or healing amounts).
    /// </summary>
    /// <param name="text">The text to show</param>
    /// <param name="position">The position to show the text at</param>
    /// <param name="color">The color of the text</param>
    public void ShowEffectLabel(string text, Vector2 position, Color color)
    {
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
    /// <param name="turnOrder">The current turn order</param>
    public void UpdateTurnOrder(List<Battler> turnOrder)
    {
        // Update the display showing the turn order
        // This would typically involve updating icons or names showing who is next to act

        // For example:
        // var turnOrderContainer = GetNode<HBoxContainer>("TurnOrder");
        // foreach (var child in turnOrderContainer.GetChildren())
        // {
        //     child.QueueFree();
        // }

        // foreach (var battler in turnOrder.Take(5)) // Show next 5 battlers
        // {
        //     var icon = new TextureRect();
        //     // Set icon texture based on battler
        //     turnOrderContainer.AddChild(icon);
        // }
    }
}
