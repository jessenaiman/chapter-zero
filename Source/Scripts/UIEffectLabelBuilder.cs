using Godot;
using System;
using System.Collections.Generic;

/// <summary>
/// Container for creating and managing effect labels in combat.
/// The UIEffectLabelBuilder is responsible for creating visual feedback labels
/// that appear during combat, such as damage numbers, healing amounts, status
/// effects, and other combat-related notifications. It manages the positioning,
/// animation, and lifetime of these labels.
/// </summary>
public partial class UIEffectLabelBuilder : Control
{
    /// <summary>
    /// Emitted when an effect label is created.
    /// </summary>
    [Signal]
    public delegate void EffectLabelCreatedEventHandler(Label label);

    /// <summary>
    /// The list of combat participants.
    /// </summary>
    public BattlerList Battlers { get; private set; }

    /// <summary>
    /// The font to use for effect labels.
    /// </summary>
    [Export]
    public Font EffectFont { get; set; }

    /// <summary>
    /// The default color for damage labels.
    /// </summary>
    [Export]
    public Color DamageColor { get; set; } = Colors.Red;

    /// <summary>
    /// The default color for healing labels.
    /// </summary>
    [Export]
    public Color HealColor { get; set; } = Colors.Green;

    /// <summary>
    /// The default color for status effect labels.
    /// </summary>
    [Export]
    public Color StatusColor { get; set; } = Colors.Blue;

    /// <summary>
    /// The default color for miss labels.
    /// </summary>
    [Export]
    public Color MissColor { get; set; } = Colors.Gray;

    /// <summary>
    /// The duration in seconds that effect labels stay visible.
    /// </summary>
    [Export]
    public float LabelDuration { get; set; } = 1.0f;

    /// <summary>
    /// The distance effect labels move upward when appearing.
    /// </summary>
    [Export]
    public float FloatDistance { get; set; } = 50.0f;

    /// <summary>
    /// Pool of reusable label nodes to reduce allocations.
    /// </summary>
    private Queue<Label> labelPool = new Queue<Label>();

    /// <summary>
    /// Maximum number of labels to keep in the pool.
    /// </summary>
    private const int MaxPoolSize = 20;

    public override void _Ready()
    {
        // Hide the container since it's just a manager
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
        // CombatEvents.BattlerDamaged += OnBattlerDamaged;
        // CombatEvents.BattlerHealed += OnBattlerHealed;
        // CombatEvents.BattlerMissed += OnBattlerMissed;
        // CombatEvents.StatusEffectApplied += OnStatusEffectApplied;
    }

    /// <summary>
    /// Setup the UI effect label builder with the given battler list.
    /// </summary>
    /// <param name="battlers">The list of combat participants</param>
    public void Setup(BattlerList battlers)
    {
        Battlers = battlers;
    }

    /// <summary>
    /// Create a damage label for a battler.
    /// </summary>
    /// <param name="battler">The battler that took damage</param>
    /// <param name="damageAmount">The amount of damage taken</param>
    public void CreateDamageLabel(Battler battler, int damageAmount)
    {
        if (battler == null)
        {
            return;
        }

        var labelText = damageAmount.ToString();
        var label = CreateLabel(labelText, DamageColor);

        // Position the label above the battler
        PositionLabelAboveBattler(label, battler);

        // Animate the label
        AnimateLabel(label);

        EmitSignal(SignalName.EffectLabelCreated, label);
    }

    /// <summary>
    /// Create a healing label for a battler.
    /// </summary>
    /// <param name="battler">The battler that was healed</param>
    /// <param name="healAmount">The amount of healing received</param>
    public void CreateHealLabel(Battler battler, int healAmount)
    {
        if (battler == null)
        {
            return;
        }

        var labelText = $"+{healAmount}";
        var label = CreateLabel(labelText, HealColor);

        // Position the label above the battler
        PositionLabelAboveBattler(label, battler);

        // Animate the label
        AnimateLabel(label);

        EmitSignal(SignalName.EffectLabelCreated, label);
    }

    /// <summary>
    /// Create a miss label for a battler.
    /// </summary>
    /// <param name="battler">The battler that was missed</param>
    public void CreateMissLabel(Battler battler)
    {
        if (battler == null)
        {
            return;
        }

        var label = CreateLabel("MISS", MissColor);

        // Position the label above the battler
        PositionLabelAboveBattler(label, battler);

        // Animate the label
        AnimateLabel(label);

        EmitSignal(SignalName.EffectLabelCreated, label);
    }

    /// <summary>
    /// Create a status effect label for a battler.
    /// </summary>
    /// <param name="battler">The battler that received the status effect</param>
    /// <param name="statusEffect">The status effect applied</param>
    public void CreateStatusEffectLabel(Battler battler, string statusEffect)
    {
        if (battler == null || string.IsNullOrEmpty(statusEffect))
        {
            return;
        }

        var label = CreateLabel(statusEffect, StatusColor);

        // Position the label above the battler
        PositionLabelAboveBattler(label, battler);

        // Animate the label
        AnimateLabel(label);

        EmitSignal(SignalName.EffectLabelCreated, label);
    }

    /// <summary>
    /// Create a generic effect label.
    /// </summary>
    /// <param name="text">The text to display</param>
    /// <param name="color">The color of the text</param>
    /// <param name="position">The position to display the label at</param>
    public void CreateEffectLabel(string text, Color color, Vector2 position)
    {
        if (string.IsNullOrEmpty(text))
        {
            return;
        }

        var label = CreateLabel(text, color);

        // Position the label
        label.Position = position;

        // Animate the label
        AnimateLabel(label);

        EmitSignal(SignalName.EffectLabelCreated, label);
    }

    /// <summary>
    /// Create a label with the specified text and color.
    /// </summary>
    /// <param name="text">The text to display</param>
    /// <param name="color">The color of the text</param>
    /// <returns>The created label</returns>
    private Label CreateLabel(string text, Color color)
    {
        Label label;

        // Try to get a label from the pool first
        if (labelPool.Count > 0)
        {
            label = labelPool.Dequeue();
            label.Show();
        }
        else
        {
            // Create a new label if the pool is empty
            label = new Label();
            AddChild(label);
        }

        // Configure the label
        label.Text = text;
        label.AddThemeColorOverride("font_color", color);

        if (EffectFont != null)
        {
            label.AddThemeFontOverride("font", EffectFont);
        }

        // Reset any previous animations
        label.Scale = Vector2.One;
        label.Modulate = new Color(color, 1.0f);

        return label;
    }

    /// <summary>
    /// Position a label above a battler.
    /// </summary>
    /// <param name="label">The label to position</param>
    /// <param name="battler">The battler to position the label above</param>
    private void PositionLabelAboveBattler(Label label, Battler battler)
    {
        if (label == null || battler == null)
        {
            return;
        }

        // Position the label above the battler
        // This assumes the battler has a visual representation in the scene
        // The exact positioning would depend on the battler's visual structure
        label.Position = battler.Position + new Vector2(0, -50);
    }

    /// <summary>
    /// Animate a label with a floating and fading effect.
    /// </summary>
    /// <param name="label">The label to animate</param>
    private async void AnimateLabel(Label label)
    {
        if (label == null)
        {
            return;
        }

        // Create a tween for the animation
        var tween = CreateTween();
        tween.SetParallel(true);

        // Move the label upward
        tween.TweenProperty(label, "position:y", label.Position.Y - FloatDistance, LabelDuration);

        // Fade out the label
        tween.TweenProperty(label, "modulate:a", 0.0f, LabelDuration);

        // Scale the label slightly for emphasis
        tween.TweenProperty(label, "scale", new Vector2(1.2f, 1.2f), LabelDuration * 0.5f)
             .SetTrans(Tween.TransitionType.Quad)
             .SetEase(Tween.EaseType.Out);
        tween.TweenProperty(label, "scale", Vector2.One, LabelDuration * 0.5f)
             .SetTrans(Tween.TransitionType.Quad)
             .SetEase(Tween.EaseType.In)
             .SetDelay(LabelDuration * 0.5f);

        // Wait for the animation to complete
        await ToSignal(tween, Tween.SignalName.Finished);

        // Return the label to the pool or free it
        ReturnLabelToPool(label);
    }

    /// <summary>
    /// Return a label to the pool for reuse.
    /// </summary>
    /// <param name="label">The label to return to the pool</param>
    private void ReturnLabelToPool(Label label)
    {
        if (label == null)
        {
            return;
        }

        // Hide the label
        label.Hide();

        // Reset its properties
        label.Text = "";
        label.Position = Vector2.Zero;
        label.Scale = Vector2.One;
        label.Modulate = Colors.White;

        // Add it to the pool if there's room
        if (labelPool.Count < MaxPoolSize)
        {
            labelPool.Enqueue(label);
        }
        else
        {
            // If the pool is full, free the label
            label.QueueFree();
        }
    }

    /// <summary>
    /// Clear all effect labels.
    /// </summary>
    public void ClearLabels()
    {
        // Clear all labels in the scene
        foreach (var child in GetChildren())
        {
            if (child is Label label)
            {
                label.QueueFree();
            }
        }

        // Clear the pool
        labelPool.Clear();
    }

    /// <summary>
    /// Callback when a battler takes damage.
    /// </summary>
    private void OnBattlerDamaged(Battler battler, int damage)
    {
        CreateDamageLabel(battler, damage);
    }

    /// <summary>
    /// Callback when a battler is healed.
    /// </summary>
    private void OnBattlerHealed(Battler battler, int healAmount)
    {
        CreateHealLabel(battler, healAmount);
    }

    /// <summary>
    /// Callback when an attack misses a battler.
    /// </summary>
    private void OnBattlerMissed(Battler battler)
    {
        CreateMissLabel(battler);
    }

    /// <summary>
    /// Callback when a status effect is applied to a battler.
    /// </summary>
    private void OnStatusEffectApplied(Battler battler, string statusEffect)
    {
        CreateStatusEffectLabel(battler, statusEffect);
    }

    /// <summary>
    /// Create a critical hit label.
    /// </summary>
    /// <param name="battler">The battler that took critical damage</param>
    /// <param name="damageAmount">The critical damage amount</param>
    public void CreateCriticalHitLabel(Battler battler, int damageAmount)
    {
        if (battler == null)
        {
            return;
        }

        var labelText = $"CRITICAL!\n{damageAmount}";
        var label = CreateLabel(labelText, Colors.Orange);

        // Position the label above the battler
        PositionLabelAboveBattler(label, battler);

        // Animate the label with a more dramatic effect
        AnimateCriticalLabel(label);

        EmitSignal(SignalName.EffectLabelCreated, label);
    }

    /// <summary>
    /// Animate a critical hit label with a more dramatic effect.
    /// </summary>
    /// <param name="label">The critical hit label to animate</param>
    private async void AnimateCriticalLabel(Label label)
    {
        if (label == null)
        {
            return;
        }

        // Create a tween for the animation
        var tween = CreateTween();
        tween.SetParallel(true);

        // Move the label upward faster
        tween.TweenProperty(label, "position:y", label.Position.Y - FloatDistance * 1.5f, LabelDuration * 1.5f);

        // Fade out the label
        tween.TweenProperty(label, "modulate:a", 0.0f, LabelDuration * 1.5f);

        // Scale the label for emphasis
        tween.TweenProperty(label, "scale", new Vector2(1.5f, 1.5f), LabelDuration * 0.75f)
             .SetTrans(Tween.TransitionType.Elastic)
             .SetEase(Tween.EaseType.Out);
        tween.TweenProperty(label, "scale", Vector2.One, LabelDuration * 0.75f)
             .SetTrans(Tween.TransitionType.Back)
             .SetEase(Tween.EaseType.In)
             .SetDelay(LabelDuration * 0.75f);

        // Add a shake effect
        var startPosition = label.Position;
        tween.TweenProperty(label, "position:x", startPosition.X + 5, LabelDuration * 0.1f)
             .SetTrans(Tween.TransitionType.Sine)
             .SetEase(Tween.EaseType.InOut);
        tween.TweenProperty(label, "position:x", startPosition.X - 5, LabelDuration * 0.1f)
             .SetTrans(Tween.TransitionType.Sine)
             .SetEase(Tween.EaseType.InOut)
             .SetDelay(LabelDuration * 0.1f);
        tween.TweenProperty(label, "position:x", startPosition.X, LabelDuration * 0.1f)
             .SetTrans(Tween.TransitionType.Sine)
             .SetEase(Tween.EaseType.InOut)
             .SetDelay(LabelDuration * 0.2f);

        // Wait for the animation to complete
        await ToSignal(tween, Tween.SignalName.Finished);

        // Return the label to the pool or free it
        ReturnLabelToPool(label);
    }

    /// <summary>
    /// Create a buff label for a battler.
    /// </summary>
    /// <param name="battler">The battler that received the buff</param>
    /// <param name="buffName">The name of the buff</param>
    public void CreateBuffLabel(Battler battler, string buffName)
    {
        if (battler == null || string.IsNullOrEmpty(buffName))
        {
            return;
        }

        var label = CreateLabel($"BUFF!\n{buffName}", Colors.LightBlue);

        // Position the label above the battler
        PositionLabelAboveBattler(label, battler);

        // Animate the label
        AnimateLabel(label);

        EmitSignal(SignalName.EffectLabelCreated, label);
    }

    /// <summary>
    /// Create a debuff label for a battler.
    /// </summary>
    /// <param name="battler">The battler that received the debuff</param>
    /// <param name="debuffName">The name of the debuff</param>
    public void CreateDebuffLabel(Battler battler, string debuffName)
    {
        if (battler == null || string.IsNullOrEmpty(debuffName))
        {
            return;
        }

        var label = CreateLabel($"DEBUFF!\n{debuffName}", Colors.Purple);

        // Position the label above the battler
        PositionLabelAboveBattler(label, battler);

        // Animate the label
        AnimateLabel(label);

        EmitSignal(SignalName.EffectLabelCreated, label);
    }
}
