using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Container for the turn order bar.
/// The UITurnBar displays the order of battlers in combat, showing which battlers
/// will act next. It provides a visual representation of the combat flow and helps
/// players anticipate upcoming actions.
/// </summary>
public partial class UITurnBar : Control
{
    /// <summary>
    /// The list of combat participants.
    /// </summary>
    public BattlerList Battlers { get; private set; }

    /// <summary>
    /// The container for turn order displays.
    /// </summary>
    private HBoxContainer turnOrderContainer;

    /// <summary>
    /// The container for battler portraits.
    /// </summary>
    private HBoxContainer portraitsContainer;

    /// <summary>
    /// The container for readiness bars.
    /// </summary>
    private VBoxContainer readinessContainer;

    /// <summary>
    /// Dictionary mapping battlers to their UI displays.
    /// </summary>
    private Dictionary<Battler, Control> battlerDisplays = new Dictionary<Battler, Control>();

    /// <summary>
    /// Dictionary mapping battlers to their readiness bars.
    /// </summary>
    private Dictionary<Battler, TextureProgressBar> readinessBars = new Dictionary<Battler, TextureProgressBar>();

    public override void _Ready()
    {
        // Get references to child UI elements
        turnOrderContainer = GetNode<HBoxContainer>("TurnOrder");
        portraitsContainer = GetNode<HBoxContainer>("Portraits");
        readinessContainer = GetNode<VBoxContainer>("Readiness");

        // Initially hide the turn bar
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
        // CombatEvents.BattlerReadinessChanged += OnBattlerReadinessChanged;
        // CombatEvents.BattlerReadyToAct += OnBattlerReadyToAct;
        // CombatEvents.BattlerActionFinished += OnBattlerActionFinished;
    }

    /// <summary>
    /// Setup the UI turn bar with the given battler list.
    /// </summary>
    /// <param name="battlers">The list of combat participants</param>
    public void Setup(BattlerList battlers)
    {
        Battlers = battlers;

        // Clear any existing displays
        ClearDisplays();

        // Create displays for all battlers
        if (Battlers.Players != null)
        {
            foreach (var player in Battlers.Players)
            {
                CreateBattlerDisplay(player);
            }
        }

        if (Battlers.Enemies != null)
        {
            foreach (var enemy in Battlers.Enemies)
            {
                CreateBattlerDisplay(enemy);
            }
        }

        // Show the turn bar
        Visible = true;

        // Update all displays
        UpdateAllDisplays();
    }

    /// <summary>
    /// Clear all battler displays.
    /// </summary>
    private void ClearDisplays()
    {
        // Remove all existing battler displays
        foreach (var display in battlerDisplays.Values)
        {
            display.QueueFree();
        }

        battlerDisplays.Clear();
        readinessBars.Clear();

        // Clear containers
        if (turnOrderContainer != null)
        {
            foreach (var child in turnOrderContainer.GetChildren())
            {
                child.QueueFree();
            }
        }

        if (portraitsContainer != null)
        {
            foreach (var child in portraitsContainer.GetChildren())
            {
                child.QueueFree();
            }
        }

        if (readinessContainer != null)
        {
            foreach (var child in readinessContainer.GetChildren())
            {
                child.QueueFree();
            }
        }
    }

    /// <summary>
    /// Create a display for a battler.
    /// </summary>
    /// <param name="battler">The battler to create a display for</param>
    private void CreateBattlerDisplay(Battler battler)
    {
        if (battler == null)
        {
            return;
        }

        // Create a new battler display control
        var display = new Control();
        display.Name = battler.Name;

        // Add the display to the turn order container
        if (turnOrderContainer != null)
        {
            turnOrderContainer.AddChild(display);
        }

        // Store the display in the dictionary
        battlerDisplays[battler] = display;

        // Connect to the battler's signals
        if (battler.Stats != null)
        {
            battler.Stats.ReadinessChanged += () => OnBattlerReadinessChanged(battler);
            battler.ReadyToAct += () => OnBattlerReadyToAct(battler);
            battler.ActionFinished += () => OnBattlerActionFinished(battler);
        }

        // Set up the display with initial values
        UpdateBattlerDisplay(battler, display);

        // Create a readiness bar for the battler
        CreateReadinessBar(battler);
    }

    /// <summary>
    /// Create a readiness bar for a battler.
    /// </summary>
    /// <param name="battler">The battler to create a readiness bar for</param>
    private void CreateReadinessBar(Battler battler)
    {
        if (battler == null || readinessContainer == null)
        {
            return;
        }

        // Create a new readiness bar
        var readinessBar = new TextureProgressBar();
        readinessBar.Name = $"{battler.Name}_Readiness";
        readinessBar.MaxValue = 100.0f;
        readinessBar.MinValue = 0.0f;
        readinessBar.Value = battler.Readiness;

        // Add the readiness bar to the readiness container
        readinessContainer.AddChild(readinessBar);

        // Store the readiness bar in the dictionary
        readinessBars[battler] = readinessBar;
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

        // var readinessBar = display.GetNode<TextureProgressBar>("Readiness");
        // readinessBar.Value = battler.Readiness;
        // readinessBar.MaxValue = 100.0f;

        // Update the readiness bar if it exists
        if (readinessBars.TryGetValue(battler, out TextureProgressBar readinessBar))
        {
            readinessBar.Value = battler.Readiness;
        }
    }

    /// <summary>
    /// Update the turn order display.
    /// </summary>
    private void UpdateTurnOrderDisplay()
    {
        if (turnOrderContainer == null || Battlers == null)
        {
            return;
        }

        // Clear the turn order container
        foreach (var child in turnOrderContainer.GetChildren())
        {
            child.QueueFree();
        }

        // Get the current turn order
        var turnOrder = GetCurrentTurnOrder();

        // Create displays for the next few battlers in the turn order
        foreach (var battler in turnOrder.Take(5)) // Show next 5 battlers
        {
            if (battlerDisplays.TryGetValue(battler, out Control display))
            {
                // Clone the display or create a new one for the turn order
                var turnOrderDisplay = new TextureRect();
                turnOrderDisplay.Texture = GetBattlerPortrait(battler);

                // Highlight the current battler
                if (battler == turnOrder.FirstOrDefault())
                {
                    turnOrderDisplay.Modulate = Colors.Yellow;
                }

                turnOrderContainer.AddChild(turnOrderDisplay);
            }
        }
    }

    /// <summary>
    /// Get the current turn order.
    /// </summary>
    /// <returns>The current turn order as a list of battlers</returns>
    private List<Battler> GetCurrentTurnOrder()
    {
        if (Battlers == null)
        {
            return new List<Battler>();
        }

        // Combine all battlers and sort by readiness (highest first)
        var allBattlers = Battlers.GetAllBattlers();
        allBattlers.Sort((a, b) => b.Readiness.CompareTo(a.Readiness));

        return allBattlers;
    }

    /// <summary>
    /// Get a battler's portrait texture.
    /// </summary>
    /// <param name="battler">The battler to get the portrait for</param>
    /// <returns>The portrait texture</returns>
    private Texture2D GetBattlerPortrait(Battler battler)
    {
        if (battler == null)
        {
            return null;
        }

        // This would typically return a texture based on the battler's appearance
        // For example:
        // return battler.PortraitTexture;

        // For now, return null
        return null;
    }

    /// <summary>
    /// Callback when a battler's readiness changes.
    /// </summary>
    /// <param name="battler">The battler whose readiness changed</param>
    private void OnBattlerReadinessChanged(Battler battler)
    {
        if (battlerDisplays.ContainsKey(battler))
        {
            UpdateBattlerDisplay(battler, battlerDisplays[battler]);
        }

        // Update the turn order display
        UpdateTurnOrderDisplay();
    }

    /// <summary>
    /// Callback when a battler is ready to act.
    /// </summary>
    /// <param name="battler">The battler that is ready to act</param>
    private void OnBattlerReadyToAct(Battler battler)
    {
        // Highlight the battler in the turn order display
        UpdateTurnOrderDisplay();
    }

    /// <summary>
    /// Callback when a battler finishes their action.
    /// </summary>
    /// <param name="battler">The battler that finished their action</param>
    private void OnBattlerActionFinished(Battler battler)
    {
        // Update the turn order display
        UpdateTurnOrderDisplay();
    }

    /// <summary>
    /// Hide the turn bar.
    /// </summary>
    public void HideBar()
    {
        Visible = false;
    }

    /// <summary>
    /// Show the turn bar.
    /// </summary>
    public void ShowBar()
    {
        Visible = true;
    }

    /// <summary>
    /// Fade out the turn bar.
    /// </summary>
    public void FadeOut()
    {
        // Create a tween to fade out the turn bar
        var tween = CreateTween();
        tween.TweenProperty(this, "modulate:a", 0.0f, 0.5f);
        tween.TweenCallback(new Callable(this, MethodName.QueueFree)).SetDelay(0.5f);
    }

    /// <summary>
    /// Fade in the turn bar.
    /// </summary>
    public void FadeIn()
    {
        // Make sure the turn bar is visible
        Visible = true;
        Modulate = new Color(1, 1, 1, 0); // Start transparent

        // Create a tween to fade in the turn bar
        var tween = CreateTween();
        tween.TweenProperty(this, "modulate:a", 1.0f, 0.5f);
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

        // Update the turn order display
        UpdateTurnOrderDisplay();
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
    /// Refresh the turn bar with current battler data.
    /// </summary>
    public void Refresh()
    {
        // Clear and recreate all battler displays
        ClearDisplays();

        if (Battlers != null)
        {
            if (Battlers.Players != null)
            {
                foreach (var player in Battlers.Players)
                {
                    CreateBattlerDisplay(player);
                }
            }

            if (Battlers.Enemies != null)
            {
                foreach (var enemy in Battlers.Enemies)
                {
                    CreateBattlerDisplay(enemy);
                }
            }
        }

        // Update all displays
        UpdateAllDisplays();
    }

    /// <summary>
    /// Highlight the current acting battler.
    /// </summary>
    /// <param name="battler">The battler to highlight</param>
    public void HighlightCurrentBattler(Battler battler)
    {
        // Remove highlight from all battler displays
        foreach (var display in battlerDisplays.Values)
        {
            // Remove highlight styling
            // display.Modulate = Colors.White;
        }

        // Add highlight to the current battler's display
        if (battler != null && battlerDisplays.ContainsKey(battler))
        {
            var display = battlerDisplays[battler];
            // Add highlight styling
            // display.Modulate = Colors.Yellow;
        }
    }

    /// <summary>
    /// Update the readiness bars for all battlers.
    /// </summary>
    public void UpdateReadinessBars()
    {
        foreach (var kvp in readinessBars)
        {
            var battler = kvp.Key;
            var readinessBar = kvp.Value;

            if (battler != null && battler.Stats != null)
            {
                readinessBar.Value = battler.Readiness;
            }
        }
    }

    /// <summary>
    /// Show a warning for battlers with low health.
    /// </summary>
    public void ShowHealthWarnings()
    {
        if (Battlers == null)
        {
            return;
        }

        // Get all battlers
        var allBattlers = Battlers.GetAllBattlers();

        foreach (var battler in allBattlers)
        {
            if (battler == null || battler.Stats == null)
            {
                continue;
            }

            // Calculate health percentage
            var healthPercentage = (float)battler.Stats.Health / battler.Stats.MaxHealth;

            // Show a warning if health is below 25%
            if (healthPercentage < 0.25f && battlerDisplays.TryGetValue(battler, out Control display))
            {
                // Add warning styling to the display
                // For example:
                // display.Modulate = Colors.Red;
            }
        }
    }

    /// <summary>
    /// Hide all health warnings.
    /// </summary>
    public void HideHealthWarnings()
    {
        // Remove warning styling from all battler displays
        foreach (var display in battlerDisplays.Values)
        {
            // Remove warning styling
            // display.Modulate = Colors.White;
        }
    }

    /// <summary>
    /// Animate the turn bar when a battler is ready to act.
    /// </summary>
    /// <param name="battler">The battler that is ready to act</param>
    public async void AnimateBattlerReady(Battler battler)
    {
        if (battler == null || !battlerDisplays.TryGetValue(battler, out Control display))
        {
            return;
        }

        // Create a tween to animate the battler display
        var tween = CreateTween();
        tween.SetParallel(true);

        // Scale the display for emphasis
        tween.TweenProperty(display, "scale", new Vector2(1.2f, 1.2f), 0.25f)
             .SetTrans(Tween.TransitionType.Back)
             .SetEase(Tween.EaseType.Out);
        tween.TweenProperty(display, "scale", Vector2.One, 0.25f)
             .SetTrans(Tween.TransitionType.Back)
             .SetEase(Tween.EaseType.In)
             .SetDelay(0.25f);

        // Change the color for emphasis
        tween.TweenProperty(display, "modulate", Colors.Yellow, 0.25f)
             .SetTrans(Tween.TransitionType.Sine)
             .SetEase(Tween.EaseType.Out);
        tween.TweenProperty(display, "modulate", Colors.White, 0.25f)
             .SetTrans(Tween.TransitionType.Sine)
             .SetEase(Tween.EaseType.In)
             .SetDelay(0.25f);

        // Wait for the animation to complete
        await ToSignal(tween, Tween.SignalName.Finished);
    }

    /// <summary>
    /// Animate the turn bar when a battler's action is finished.
    /// </summary>
    /// <param name="battler">The battler that finished their action</param>
    public async void AnimateBattlerActionFinished(Battler battler)
    {
        if (battler == null || !battlerDisplays.TryGetValue(battler, out Control display))
        {
            return;
        }

        // Create a tween to animate the battler display
        var tween = CreateTween();
        tween.SetParallel(true);

        // Fade out the display briefly
        tween.TweenProperty(display, "modulate:a", 0.5f, 0.1f)
             .SetTrans(Tween.TransitionType.Sine)
             .SetEase(Tween.EaseType.Out);
        tween.TweenProperty(display, "modulate:a", 1.0f, 0.1f)
             .SetTrans(Tween.TransitionType.Sine)
             .SetEase(Tween.EaseType.In)
             .SetDelay(0.1f);

        // Wait for the animation to complete
        await ToSignal(tween, Tween.SignalName.Finished);
    }

    /// <summary>
    /// Update the turn bar with a new turn order.
    /// </summary>
    /// <param name="turnOrder">The new turn order</param>
    public void UpdateTurnOrder(List<Battler> turnOrder)
    {
        if (turnOrder == null || turnOrderContainer == null)
        {
            return;
        }

        // Clear the turn order container
        foreach (var child in turnOrderContainer.GetChildren())
        {
            child.QueueFree();
        }

        // Create displays for the battlers in the turn order
        foreach (var battler in turnOrder.Take(5)) // Show next 5 battlers
        {
            if (battlerDisplays.TryGetValue(battler, out Control display))
            {
                // Clone the display or create a new one for the turn order
                var turnOrderDisplay = new TextureRect();
                turnOrderDisplay.Texture = GetBattlerPortrait(battler);

                // Highlight the current battler
                if (battler == turnOrder.FirstOrDefault())
                {
                    turnOrderDisplay.Modulate = Colors.Yellow;
                }

                turnOrderContainer.AddChild(turnOrderDisplay);
            }
        }
    }

    /// <summary>
    /// Get the battler that will act next.
    /// </summary>
    /// <returns>The battler that will act next, or null if not found</returns>
    public Battler GetNextBattler()
    {
        var turnOrder = GetCurrentTurnOrder();
        return turnOrder.FirstOrDefault();
    }

    /// <summary>
    /// Get the battlers that will act in the near future.
    /// </summary>
    /// <param name="count">The number of battlers to get</param>
    /// <returns>A list of battlers that will act in the near future</returns>
    public List<Battler> GetUpcomingBattlers(int count = 3)
    {
        var turnOrder = GetCurrentTurnOrder();
        return turnOrder.Take(count).ToList();
    }
}
