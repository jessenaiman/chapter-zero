using Godot;

namespace OmegaSpiral.Narrative;

/// <summary>
/// Scene1Narrative handles the opening narrative sequence with typewriter effect.
/// </summary>
[GlobalClass]
public partial class Scene1Narrative : Node2D
{
    /// <summary>
    /// Introduction text displayed at the start.
    /// </summary>
    [Export]
    public string IntroductionText { get; set; } = "If you could hear only one story... what would it be?";

    /// <summary>
    /// Speed of typewriter effect in seconds per character.
    /// </summary>
    [Export]
    public float TypewriterSpeed { get; set; } = 0.05f;

    private RichTextLabel? _outputLabel;
    private LineEdit? _inputField;
    private Button? _submitButton;
    private Label? _promptLabel;
    private string _fullText = "";
    private int _currentCharIndex;
    private Godot.Timer? _typewriterTimer;

    private ShaderMaterial? _crtMaterial;

    /// <summary>
    /// Called when the node enters the scene tree for the first time.
    /// Initializes UI components and starts the typewriter effect.
    /// </summary>
    public override void _Ready()
    {
        _outputLabel = GetNode<RichTextLabel>("OutputLabel");
        _inputField = GetNode<LineEdit>("InputField");
        _submitButton = GetNode<Button>("SubmitButton");
        _promptLabel = GetNode<Label>("PromptLabel");

        _crtMaterial = (ShaderMaterial?) _outputLabel.Material;

        _submitButton.Pressed += OnSubmitPressed;

        // Start typewriter effect
        StartTypewriterEffect();
    }

    /// <summary>
    /// Called every frame. Updates the CRT shader material with the current time.
    /// </summary>
    /// <param name="delta">The elapsed time since the previous frame.</param>
    public override void _Process(double delta)
    {
        if (_crtMaterial != null)
        {
            _crtMaterial.SetShaderParameter("time", (float) (Time.GetTicksMsec() / 1000.0));
        }
    }

    private void StartTypewriterEffect()
    {
        _fullText = IntroductionText;
        _currentCharIndex = 0;
        _outputLabel!.Text = "";

        _typewriterTimer = new Godot.Timer();
        _typewriterTimer.WaitTime = TypewriterSpeed;
        _typewriterTimer.OneShot = false;
        _typewriterTimer.Timeout += OnTypewriterTimeout;
        AddChild(_typewriterTimer);
        _typewriterTimer.Start();
    }

    private void OnTypewriterTimeout()
    {
        if (_currentCharIndex < _fullText.Length)
        {
            _outputLabel!.Text += _fullText[_currentCharIndex];
            _currentCharIndex++;
        }
        else
        {
            _typewriterTimer!.Stop();
            _typewriterTimer.QueueFree();
            // After typewriter, show the prompt
            ShowNamePrompt();
        }
    }

    private void ShowNamePrompt()
    {
        _promptLabel!.Text = "What is your name, traveler?";
        _inputField!.Visible = true;
        _submitButton!.Visible = true;
    }

    private void OnSubmitPressed()
    {
        string playerName = _inputField!.Text.Trim();
        if (!string.IsNullOrEmpty(playerName))
        {
            GD.Print($"Player name: {playerName}");
            // TODO: Save to GameState and transition to next scene
            // For now, just print
            _outputLabel!.Text += $"\n\nWelcome, {playerName}!";
            _inputField.Visible = false;
            _submitButton!.Visible = false;
            _promptLabel!.Text = "Press any key to continue...";
            // TODO: Wait for input to proceed
        }
    }

    /// <summary>
    /// Called when an input event occurs. Handles key presses to proceed to the next scene.
    /// </summary>
    /// <param name="event">The input event that occurred.</param>
    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventKey keyEvent && keyEvent.Pressed && _inputField!.Visible == false)
        {
            // Proceed to next scene
            GD.Print("Proceeding to next scene...");
            // TODO: Load next scene
        }
    }
}
