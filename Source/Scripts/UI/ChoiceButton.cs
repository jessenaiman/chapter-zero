using Godot;

namespace OmegaSpiral.Source.Scripts.UI;

/// <summary>
/// Interactive button for presenting philosophical choices in Stage 1 opening sequence.
/// </summary>
/// <remarks>
/// <para>
/// Supports both mouse and gamepad navigation with visual feedback. Designed for accessibility
/// with high-contrast focus states and screen reader compatibility.
/// </para>
/// <para>
/// Visual States:
/// <list type="bullet">
/// <item><description><strong>Normal</strong>: Dim green phosphor glow</description></item>
/// <item><description><strong>Hover/Focus</strong>: Bright phosphor glow with subtle pulse</description></item>
/// <item><description><strong>Pressed</strong>: Burst of light with scanline interference</description></item>
/// </list>
/// </para>
/// </remarks>
[GlobalClass]
public partial class ChoiceButton : Button
{
    private string _choiceText = string.Empty;
    private int _lightPoints;
    private int _shadowPoints;
    private int _ambitionPoints;

    /// <summary>
    /// Emitted when this choice button is activated by player.
    /// </summary>
    /// <param name="choiceText">The text of the selected choice</param>
    /// <param name="lightPoints">Points awarded to Light thread</param>
    /// <param name="shadowPoints">Points awarded to Shadow thread</param>
    /// <param name="ambitionPoints">Points awarded to Ambition thread</param>
    [Signal]
    public delegate void ChoiceSelectedEventHandler(
        string choiceText,
        int lightPoints,
        int shadowPoints,
        int ambitionPoints);

    /// <summary>
    /// Gets or sets the choice text displayed on the button.
    /// </summary>
    public string ChoiceText
    {
        get => _choiceText;
        set
        {
            _choiceText = value;
            Text = value;
        }
    }

    /// <summary>
    /// Gets or sets the Light thread points for this choice.
    /// </summary>
    public int LightPoints
    {
        get => _lightPoints;
        set => _lightPoints = value;
    }

    /// <summary>
    /// Gets or sets the Shadow thread points for this choice.
    /// </summary>
    public int ShadowPoints
    {
        get => _shadowPoints;
        set => _shadowPoints = value;
    }

    /// <summary>
    /// Gets or sets the Ambition thread points for this choice.
    /// </summary>
    public int AmbitionPoints
    {
        get => _ambitionPoints;
        set => _ambitionPoints = value;
    }

    public override void _Ready()
    {
        // Connect button signals
        Pressed += OnPressed;
        MouseEntered += OnMouseEntered;
        MouseExited += OnMouseExited;
        FocusEntered += OnFocusEntered;
        FocusExited += OnFocusExited;

        // Configure button properties
        FocusMode = FocusModeEnum.All; // Support both mouse and keyboard/gamepad
        ActionMode = ActionModeEnum.Press; // Trigger on press, not release
    }

    /// <summary>
    /// Configures this button with choice data from JSON.
    /// </summary>
    /// <param name="choiceText">The text to display on the button</param>
    /// <param name="lightPoints">Points awarded to Light thread</param>
    /// <param name="shadowPoints">Points awarded to Shadow thread</param>
    /// <param name="ambitionPoints">Points awarded to Ambition thread</param>
    /// <exception cref="System.ArgumentException">
    /// Thrown if <paramref name="choiceText"/> is <see langword="null"/> or empty.
    /// </exception>
    public void Configure(
        string choiceText,
        int lightPoints,
        int shadowPoints,
        int ambitionPoints)
    {
        if (string.IsNullOrWhiteSpace(choiceText))
        {
            throw new System.ArgumentException("Choice text cannot be null or empty", nameof(choiceText));
        }

        ChoiceText = choiceText;
        LightPoints = lightPoints;
        ShadowPoints = shadowPoints;
        AmbitionPoints = ambitionPoints;

        GD.Print($"[ChoiceButton] Configured: '{choiceText}' (L:{lightPoints} S:{shadowPoints} A:{ambitionPoints})");
    }

    private void OnPressed()
    {
        GD.Print($"[ChoiceButton] Choice selected: '{ChoiceText}'");
        EmitSignal(
            SignalName.ChoiceSelected,
            ChoiceText,
            LightPoints,
            ShadowPoints,
            AmbitionPoints);
    }

    private void OnMouseEntered()
    {
        // Visual feedback handled by theme
        // Could add audio cue here
        GD.Print($"[ChoiceButton] Mouse hover: '{ChoiceText}'");
    }

    private void OnMouseExited()
    {
        // Visual feedback handled by theme
    }

    private void OnFocusEntered()
    {
        // Visual feedback handled by theme
        // Important for gamepad navigation
        GD.Print($"[ChoiceButton] Focus gained: '{ChoiceText}'");
    }

    private void OnFocusExited()
    {
        // Visual feedback handled by theme
    }

    public override void _ExitTree()
    {
        // Clean up signal connections
        Pressed -= OnPressed;
        MouseEntered -= OnMouseEntered;
        MouseExited -= OnMouseExited;
        FocusEntered -= OnFocusEntered;
        FocusExited -= OnFocusExited;
    }
}
