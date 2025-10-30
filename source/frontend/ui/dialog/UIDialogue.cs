// <copyright file="UiDialogue.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using Godot;
using OmegaSpiral.Source.Backend.Models;
using OmegaSpiral.Source.Backend.Narrative;
using OmegaSpiral.Source.Ui.Omega;
using System.Collections.ObjectModel;

namespace OmegaSpiral.Source.Scripts.Field.Ui;

/// <summary>
/// Container for the dialogue system display.
/// The UiDialogue manages the presentation of character dialogue, narrative text,
/// and conversation options during gameplay. It handles typewriter effects,
/// character portraits, dialogue choices, and branching conversation trees.
/// It provides a rich interface for storytelling and player interaction with NPCs.
/// </summary>
#pragma warning disable IDE1006  // Naming: 2-letter acronym Ui stays uppercase per C# style guide
public partial class UiDialogue : OmegaContainer
#pragma warning restore IDE1006
{
    /// <summary>
    /// Private fields
    /// Following project style guide: Private fields use camelCase with underscore prefix
    /// </summary>
#pragma warning disable CA1707  // Remove underscores from member names
#pragma warning disable IDE1006  // Naming rule: private fields are _camelCase per project convention
    private RichTextLabel? _dialogueText;
    private Label? _characterNameLabel;
    private TextureRect? _characterPortrait;
    private VBoxContainer? _choicesContainer;
    private Control? _continueIndicator;
    private string _currentText = string.Empty;
    private Character? _currentSpeaker;
    private List<ChoiceOption> _currentChoices = new List<ChoiceOption>();
    private bool _isTyping;
    private int _textPosition;
    private float _characterDelay;
    private Godot.Timer? _typewriterTimer;
#pragma warning restore IDE1006
#pragma warning restore CA1707

    /// <summary>
    /// Emitted when dialogue is finished displaying.
    /// </summary>
    [Signal]
    public delegate void DialogueFinishedEventHandler();

    /// <summary>
    /// Emitted when a dialogue choice is selected.
    /// </summary>
    /// <param name="choiceIndex"></param>
    [Signal]
    public delegate void ChoiceSelectedEventHandler(int choiceIndex);

    /// <summary>
    /// Emitted when dialogue starts.
    /// </summary>
    [Signal]
    public delegate void DialogueStartedEventHandler();

    /// <summary>
    /// Gets a value indicating whether whether the dialogue system is currently active.
    /// </summary>
    public bool IsActive { get; private set; }

    /// <summary>
    /// Gets or sets a value indicating whether whether the dialogue is currently visible.
    /// </summary>
    public bool DialogueVisible
    {
        get => this.Visible;
        set => this.Visible = value;
    }

    /// <summary>
    /// Gets or sets the typewriter effect speed (characters per second).
    /// </summary>
    [Export]
    public float TypewriterSpeed { get; set; } = 50.0f;

    /// <summary>
    /// Show an effect label (like emotion indicators or emphasis).
    /// </summary>
    /// <param name="text">The text to show.</param>
    /// <exception cref="ArgumentNullException"><paramref name="text"/> is <c>null</c>.</exception>
    public static void ShowEffectLabel(string text)
    {
        ArgumentNullException.ThrowIfNull(text);

        // Show a floating label at the specified position
        // This would typically involve creating a temporary label that floats upward and fades out

        // For example:
        // var label = new Label();
        // label.Text = text;
        // AddChild(label);

        // Create a tween to animate the label
        // var tween = CreateTween();
        // tween.TweenProperty(label, "position:y", position.Y - 50, 1.0f);
        // tween.Parallel().TweenProperty(label, "modulate:a", 0.0f, 1.0f);
        // tween.TweenCallback(new Callable(label, "queue_free"));
    }

    /// <inheritdoc/>
    public override void _Ready()
    {
        // Get references to child Ui elements
        this._dialogueText = this.GetNode<RichTextLabel>("DialogueText");
        this._characterNameLabel = this.GetNode<Label>("CharacterName");
        this._characterPortrait = this.GetNode<TextureRect>("CharacterPortrait");
        this._choicesContainer = this.GetNode<VBoxContainer>("ChoicesContainer");
        this._continueIndicator = this.GetNode<Control>("ContinueIndicator");

        // Initially hide the dialogue system
        this.Visible = false;

        // Set up the typewriter timer
        this._typewriterTimer = new Godot.Timer();
        this._typewriterTimer.OneShot = true;
        this._typewriterTimer.Timeout += this.OnTypewriterTimeout;
        this.AddChild(this._typewriterTimer);

        // Connect to any necessary signals
        ConnectSignals();
    }

    /// <summary>
    /// Start displaying dialogue.
    /// </summary>
    /// <param name="text">The dialogue text to display.</param>
    /// <param name="speaker">The character speaking (optional).</param>
    public async Task StartDialogueAsync(string text, Character? speaker = null)
    {
        if (string.IsNullOrEmpty(text))
        {
            return;
        }

        // Set the current dialogue state
        this.IsActive = true;
        this._currentText = text;
        this._currentSpeaker = speaker;
        this._textPosition = 0;
        this._isTyping = true;

        // Show the dialogue system
        this.Visible = true;

        // Update the speaker information
        this.UpdateSpeakerDisplay(speaker);

        // Clear any existing choices
        this.ClearChoices();

        // Hide the continue indicator while typing
        if (this._continueIndicator != null)
        {
            this._continueIndicator.Hide();
        }

        // Emit the dialogue started signal
        this.EmitSignal(SignalName.DialogueStarted);

        // Start the typewriter effect
        await this.TypeText(text).ConfigureAwait(false);
    }

    /// <summary>
    /// Display dialogue choices.
    /// </summary>
    /// <param name="choices">The dialogue choices to display.</param>
    public void ShowChoices(Collection<ChoiceOption> choices)
    {
        if (choices == null || choices.Count == 0)
        {
            return;
        }

        // Store the current choices
        this._currentChoices = new List<ChoiceOption>(choices);

        // Clear any existing choices
        this.ClearChoices();

        // Create choice buttons for each choice
        foreach (var choice in choices)
        {
            this.CreateChoiceButton(choice);
        }

        // Show the choices container
        if (this._choicesContainer != null)
        {
            this._choicesContainer.Show();
        }

        // Hide the dialogue text and continue indicator
        if (this._dialogueText != null)
        {
            this._dialogueText.Hide();
        }

        if (this._continueIndicator != null)
        {
            this._continueIndicator.Hide();
        }
    }

    /// <summary>
    /// Hide the dialogue system.
    /// </summary>
    public void HideDialogue()
    {
        this.Visible = false;
        this.IsActive = false;
        this._isTyping = false;
        this._currentText = string.Empty;
        this._currentSpeaker = null;
        this._textPosition = 0;

        // Clear any existing choices
        this.ClearChoices();

        // Stop the typewriter timer
        if (this._typewriterTimer != null)
        {
            this._typewriterTimer.Stop();
        }

        // Emit the dialogue finished signal
        this.EmitSignal(SignalName.DialogueFinished);
    }

    /// <summary>
    /// Skip the current typewriter effect and display the full text immediately.
    /// </summary>
    public void SkipTypewriter()
    {
        if (!this._isTyping || string.IsNullOrEmpty(this._currentText))
        {
            return;
        }

        // Stop the typewriter timer
        if (this._typewriterTimer != null)
        {
            this._typewriterTimer.Stop();
        }

        // Display the full text immediately
        if (this._dialogueText != null)
        {
            this._dialogueText.Text = this._currentText;
        }

        // Set the typing state to false
        this._isTyping = false;
        this._textPosition = this._currentText.Length;

        // Show the continue indicator
        if (this._continueIndicator != null)
        {
            this._continueIndicator.Show();
        }
    }

    /// <summary>
    /// Callback when input is received.
    /// </summary>
    /// <param name="event">The input event.</param>
    public override void _Input(InputEvent @event)
    {
        if (!this.IsActive || !this.Visible)
        {
            return;
        }

        // Check for continue input (e.g., spacebar or enter)
        if (@event is InputEventKey keyEvent && keyEvent.Pressed)
        {
            if (keyEvent.Keycode == Key.Space || keyEvent.Keycode == Key.Enter)
            {
                this.OnContinueInput();
            }
        }
        else if (@event is InputEventMouseButton mouseEvent && mouseEvent.Pressed)
        {
            if (mouseEvent.ButtonIndex == MouseButton.Left)
            {
                this.OnContinueInput();
            }
        }
    }

    /// <summary>
    /// Connect to necessary signals.
    /// </summary>
    private static void ConnectSignals()
    {
        // Connect to dialogue events
        // DialogueEvents.DialogueStarted += OnDialogueStarted;
        // DialogueEvents.DialogueEnded += OnDialogueEnded;
        // DialogueEvents.ChoiceSelected += OnChoiceSelected;
    }

    /// <summary>
    /// Show a message in the dialogue system.
    /// </summary>
    /// <param name="message">The message to show.</param>
    /// <param name="duration">The duration to show the message for.</param>
    public async Task ShowMessageAsync(string message, float duration = 2.0f)
    {
        if (string.IsNullOrEmpty(message))
        {
            return;
        }

        // Show the message in the dialogue text
        if (this._dialogueText != null)
        {
            this._dialogueText.Text = message;
        }

        // Show the dialogue system
        this.Visible = true;

        // Wait for the specified duration
        await Task.Delay(TimeSpan.FromSeconds(duration)).ConfigureAwait(false);

        // Hide the dialogue system
        this.HideDialogue();
    }

    /// <summary>
    /// Type out text with a typewriter effect.
    /// </summary>
    /// <param name="text">The text to type out.</param>
    public async Task TypeText(string text)
    {
        if (string.IsNullOrEmpty(text) || this._dialogueText == null)
        {
            return;
        }

        // Calculate the character delay based on the typewriter speed
        this._characterDelay = 1.0f / this.TypewriterSpeed;

        // Type out each character
        while (this._textPosition < text.Length && this._isTyping)
        {
            // Add the next character to the displayed text
            if (this._dialogueText != null)
            {
                this._dialogueText.Text = text.Substring(0, this._textPosition + 1);
            }

            this._textPosition++;

            // Wait for the character delay
            if (this._typewriterTimer != null)
            {
                this._typewriterTimer.Start(this._characterDelay);
                await this.ToSignal(this._typewriterTimer, Godot.Timer.SignalName.Timeout);
            }
        }

        // If we've finished typing, show the continue indicator
        if (this._textPosition >= text.Length && this._isTyping)
        {
            this._isTyping = false;
            if (this._continueIndicator != null)
            {
                this._continueIndicator.Show();
            }
        }
    }

    /// <summary>
    /// Update the character name display.
    /// </summary>
    /// <param name="name">The new character name.</param>
    public void UpdateCharacterName(string name)
    {
        if (string.IsNullOrEmpty(name) || this._characterNameLabel == null)
        {
            return;
        }

        // Update the character name
        this._characterNameLabel.Text = name;
    }

    /// <summary>
    /// Update the character portrait display.
    /// </summary>
    /// <param name="portrait">The new character portrait.</param>
    public void UpdateCharacterPortrait(Texture2D portrait)
    {
        if (portrait == null || this._characterPortrait == null)
        {
            return;
        }

        // Update the character portrait
        this._characterPortrait.Texture = portrait;
    }

    /// <summary>
    /// Add a dialogue choice.
    /// </summary>
    /// <param name="choice">The dialogue choice to add.</param>
    public void AddChoice(ChoiceOption choice)
    {
        if (choice == null)
        {
            return;
        }

        // Add the choice to the current choices list
        this._currentChoices.Add(choice);

        // Create a button for the choice
        this.CreateChoiceButton(choice);
    }

    /// <summary>
    /// Remove a dialogue choice.
    /// </summary>
    /// <param name="choice">The dialogue choice to remove.</param>
    public void RemoveChoice(ChoiceOption choice)
    {
        if (choice == null)
        {
            return;
        }

        // Remove the choice from the current choices list
        this._currentChoices.Remove(choice);

        // Remove the corresponding button
        if (this._choicesContainer != null)
        {
            foreach (var child in this._choicesContainer.GetChildren())
            {
                if (child is Button button && button.Text == choice.Text)
                {
                    // Note: We can't remove the lambda event handler easily in Godot,
                    // but since we're freeing the button immediately, it's not necessary
                    button.QueueFree();
                    break;
                }
            }
        }
    }

    /// <summary>
    /// Clear all dialogue choices.
    /// </summary>
    public void ClearAllChoices()
    {
        this._currentChoices.Clear();
        this.ClearChoices();
    }

    /// <summary>
    /// Set the typewriter speed.
    /// </summary>
    /// <param name="speed">The new typewriter speed (characters per second).</param>
    public void SetTypewriterSpeed(float speed)
    {
        if (speed <= 0)
        {
            return;
        }

        this.TypewriterSpeed = speed;
        this._characterDelay = 1.0f / this.TypewriterSpeed;
    }

    /// <summary>
    /// Get the current dialogue text.
    /// </summary>
    /// <returns>The current dialogue text.</returns>
    public string GetDialogueText()
    {
        return this._currentText;
    }

    /// <summary>
    /// Get the current character speaker.
    /// </summary>
    /// <returns>The current character speaker, or null if none.</returns>
    public Character? GetSpeaker()
    {
        return this._currentSpeaker;
    }

    /// <summary>
    /// Get the current dialogue choices.
    /// </summary>
    /// <returns>The current dialogue choices.</returns>
    public Collection<ChoiceOption> GetChoices()
    {
        return new Collection<ChoiceOption>(this._currentChoices);
    }

    /// <summary>
    /// Check if the dialogue system is currently typing text.
    /// </summary>
    /// <returns>True if the dialogue system is currently typing text, false otherwise.</returns>
    public bool IsTypingText()
    {
        return this._isTyping;
    }

    /// <summary>
    /// Check if the dialogue system is currently showing choices.
    /// </summary>
    /// <returns>True if the dialogue system is currently showing choices, false otherwise.</returns>
    public bool IsShowingChoices()
    {
        return this._choicesContainer != null && this._choicesContainer.Visible;
    }

    /// <summary>
    /// Refresh the dialogue system with current data.
    /// </summary>
    public void Refresh()
    {
        // Update the speaker display
        this.UpdateSpeakerDisplay(this._currentSpeaker);

        // Update the dialogue text
        if (this._dialogueText != null)
        {
            this._dialogueText.Text = this._currentText;
        }

        // Update the choices display
        this.ClearChoices();
        foreach (var choice in this._currentChoices)
        {
            this.CreateChoiceButton(choice);
        }

        // Show/hide the continue indicator based on typing state
        if (this._continueIndicator != null)
        {
            if (this._isTyping)
            {
                this._continueIndicator.Hide();
            }
            else
            {
                this._continueIndicator.Show();
            }
        }
    }

    /// <summary>
    /// Reset the dialogue system to its initial state.
    /// </summary>
    public void Reset()
    {
        this.HideDialogue();
        this._currentText = string.Empty;
        this._currentSpeaker = null;
        this._currentChoices.Clear();
        this._textPosition = 0;
        this._isTyping = false;

        // Reset the typewriter speed to default
        this.TypewriterSpeed = 50.0f;
        this._characterDelay = 1.0f / this.TypewriterSpeed;

        // Clear all displays
        if (this._dialogueText != null)
        {
            this._dialogueText.Text = string.Empty;
        }

        if (this._characterNameLabel != null)
        {
            this._characterNameLabel.Text = string.Empty;
            this._characterNameLabel.Hide();
        }

        if (this._characterPortrait != null)
        {
            this._characterPortrait.Texture = null;
            this._characterPortrait.Hide();
        }

        this.ClearChoices();

        if (this._continueIndicator != null)
        {
            this._continueIndicator.Hide();
        }
    }

    /// <summary>
    /// Update the speaker display with the current speaker's information.
    /// </summary>
    /// <param name="speaker">The character speaking, or null to clear.</param>
    private void UpdateSpeakerDisplay(Character? speaker)
    {
        if (speaker == null)
        {
            // Hide speaker information if no speaker
            if (this._characterNameLabel != null)
            {
                this._characterNameLabel.Hide();
            }

            if (this._characterPortrait != null)
            {
                this._characterPortrait.Hide();
            }
        }
        else
        {
            // Show speaker information
            if (this._characterNameLabel != null)
            {
                this._characterNameLabel.Text = speaker.Name ?? string.Empty;
                this._characterNameLabel.Show();
            }

            // TODO: Add Portrait property to Character class
            // if (this._characterPortrait != null && speaker.Portrait != null)
            // {
            //     this._characterPortrait.Texture = speaker.Portrait;
            //     this._characterPortrait.Show();
            // }
            // else if (this._characterPortrait != null)
            // {
            //     this._characterPortrait.Hide();
            // }
        }
    }

    /// <summary>
    /// Create a choice button for a dialogue choice.
    /// </summary>
    /// <param name="choice">The dialogue choice to create a button for.</param>
    private void CreateChoiceButton(ChoiceOption choice)
    {
        if (choice == null || this._choicesContainer == null)
        {
            return;
        }

        // Create a new button for the choice
        var button = new Button();
        button.Text = choice.Text ?? string.Empty;
        button.FocusMode = FocusModeEnum.None;

        // Connect the button's pressed signal to handle choice selection
        void OnPressed() => this.OnChoiceButtonPressed(choice);
        button.Pressed += OnPressed;

        // Add the button to the choices container
        this._choicesContainer.AddChild(button);
    }

    /// <summary>
    /// Clear all dialogue choices.
    /// </summary>
    private void ClearChoices()
    {
        if (this._choicesContainer == null)
        {
            return;
        }

        // Remove all existing choice buttons
        foreach (var child in this._choicesContainer.GetChildren())
        {
            if (child is Button button)
            {
                // Note: We can't remove the lambda event handler easily in Godot,
                // but since we're freeing the button immediately, it's not necessary
                button.QueueFree();
            }
        }

        // Hide the choices container
        this._choicesContainer.Hide();

        // Show the dialogue text again
        if (this._dialogueText != null)
        {
            this._dialogueText.Show();
        }
    }

    /// <summary>
    /// Callback when the typewriter timer times out.
    /// </summary>
    private void OnTypewriterTimeout()
    {
        // This is handled in the TypeText method
    }

    /// <summary>
    /// Callback when a choice button is pressed.
    /// </summary>
    /// <param name="choice">The selected dialogue choice.</param>
    private void OnChoiceButtonPressed(ChoiceOption choice)
    {
        if (choice == null || !this.IsActive)
        {
            return;
        }

        // Find the index of the selected choice
        var choiceIndex = this._currentChoices.IndexOf(choice);
        if (choiceIndex >= 0)
        {
            // Emit the choice selected signal
            this.EmitSignal(SignalName.ChoiceSelected, choiceIndex);

            // Hide the choices
            this.ClearChoices();
        }
    }

    /// <summary>
    /// Callback when continue input is received.
    /// </summary>
    private void OnContinueInput()
    {
        if (this._isTyping)
        {
            // Skip the typewriter effect
            this.SkipTypewriter();
        }
        else if (this._choicesContainer != null && !this._choicesContainer.Visible)
        {
            // Continue to the next dialogue line or end dialogue
            // This would typically involve checking if there's more dialogue to display
            // or emitting a signal to indicate the player wants to continue

            // For now, we'll just hide the dialogue
            this.HideDialogue();
        }
    }
}
