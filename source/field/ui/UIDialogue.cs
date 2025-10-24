// <copyright file="UiDialogue.cs" company="Omega Spiral">
// Copyright (c) Omega Spiral. All rights reserved.
// </copyright>

using Godot;
using OmegaSpiral.Source.Narrative;
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
public partial class UiDialogue : Control
#pragma warning restore IDE1006
{
    /// <summary>
    /// Private fields
    /// </summary>
    private RichTextLabel? dialogueText;
    private Label? characterNameLabel;
    private TextureRect? characterPortrait;
    private VBoxContainer? choicesContainer;
    private Control? continueIndicator;
    private string currentText = string.Empty;
    private Character? currentSpeaker;
    private List<ChoiceOption> currentChoices = new List<ChoiceOption>();
    private bool isTyping;
    private int textPosition;
    private float characterDelay;
    private Godot.Timer? typewriterTimer;

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
        this.dialogueText = this.GetNode<RichTextLabel>("DialogueText");
        this.characterNameLabel = this.GetNode<Label>("CharacterName");
        this.characterPortrait = this.GetNode<TextureRect>("CharacterPortrait");
        this.choicesContainer = this.GetNode<VBoxContainer>("ChoicesContainer");
        this.continueIndicator = this.GetNode<Control>("ContinueIndicator");

        // Initially hide the dialogue system
        this.Visible = false;

        // Set up the typewriter timer
        this.typewriterTimer = new Godot.Timer();
        this.typewriterTimer.OneShot = true;
        this.typewriterTimer.Timeout += this.OnTypewriterTimeout;
        this.AddChild(this.typewriterTimer);

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
        this.currentText = text;
        this.currentSpeaker = speaker;
        this.textPosition = 0;
        this.isTyping = true;

        // Show the dialogue system
        this.Visible = true;

        // Update the speaker information
        this.UpdateSpeakerDisplay(speaker);

        // Clear any existing choices
        this.ClearChoices();

        // Hide the continue indicator while typing
        if (this.continueIndicator != null)
        {
            this.continueIndicator.Hide();
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
        this.currentChoices = new List<ChoiceOption>(choices);

        // Clear any existing choices
        this.ClearChoices();

        // Create choice buttons for each choice
        foreach (var choice in choices)
        {
            this.CreateChoiceButton(choice);
        }

        // Show the choices container
        if (this.choicesContainer != null)
        {
            this.choicesContainer.Show();
        }

        // Hide the dialogue text and continue indicator
        if (this.dialogueText != null)
        {
            this.dialogueText.Hide();
        }

        if (this.continueIndicator != null)
        {
            this.continueIndicator.Hide();
        }
    }

    /// <summary>
    /// Hide the dialogue system.
    /// </summary>
    public void HideDialogue()
    {
        this.Visible = false;
        this.IsActive = false;
        this.isTyping = false;
        this.currentText = string.Empty;
        this.currentSpeaker = null;
        this.textPosition = 0;

        // Clear any existing choices
        this.ClearChoices();

        // Stop the typewriter timer
        if (this.typewriterTimer != null)
        {
            this.typewriterTimer.Stop();
        }

        // Emit the dialogue finished signal
        this.EmitSignal(SignalName.DialogueFinished);
    }

    /// <summary>
    /// Skip the current typewriter effect and display the full text immediately.
    /// </summary>
    public void SkipTypewriter()
    {
        if (!this.isTyping || string.IsNullOrEmpty(this.currentText))
        {
            return;
        }

        // Stop the typewriter timer
        if (this.typewriterTimer != null)
        {
            this.typewriterTimer.Stop();
        }

        // Display the full text immediately
        if (this.dialogueText != null)
        {
            this.dialogueText.Text = this.currentText;
        }

        // Set the typing state to false
        this.isTyping = false;
        this.textPosition = this.currentText.Length;

        // Show the continue indicator
        if (this.continueIndicator != null)
        {
            this.continueIndicator.Show();
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
        if (this.dialogueText != null)
        {
            this.dialogueText.Text = message;
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
        if (string.IsNullOrEmpty(text) || this.dialogueText == null)
        {
            return;
        }

        // Calculate the character delay based on the typewriter speed
        this.characterDelay = 1.0f / this.TypewriterSpeed;

        // Type out each character
        while (this.textPosition < text.Length && this.isTyping)
        {
            // Add the next character to the displayed text
            if (this.dialogueText != null)
            {
                this.dialogueText.Text = text.Substring(0, this.textPosition + 1);
            }

            this.textPosition++;

            // Wait for the character delay
            if (this.typewriterTimer != null)
            {
                this.typewriterTimer.Start(this.characterDelay);
                await this.ToSignal(this.typewriterTimer, Godot.Timer.SignalName.Timeout);
            }
        }

        // If we've finished typing, show the continue indicator
        if (this.textPosition >= text.Length && this.isTyping)
        {
            this.isTyping = false;
            if (this.continueIndicator != null)
            {
                this.continueIndicator.Show();
            }
        }
    }

    /// <summary>
    /// Update the character name display.
    /// </summary>
    /// <param name="name">The new character name.</param>
    public void UpdateCharacterName(string name)
    {
        if (string.IsNullOrEmpty(name) || this.characterNameLabel == null)
        {
            return;
        }

        // Update the character name
        this.characterNameLabel.Text = name;
    }

    /// <summary>
    /// Update the character portrait display.
    /// </summary>
    /// <param name="portrait">The new character portrait.</param>
    public void UpdateCharacterPortrait(Texture2D portrait)
    {
        if (portrait == null || this.characterPortrait == null)
        {
            return;
        }

        // Update the character portrait
        this.characterPortrait.Texture = portrait;
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
        this.currentChoices.Add(choice);

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
        this.currentChoices.Remove(choice);

        // Remove the corresponding button
        if (this.choicesContainer != null)
        {
            foreach (var child in this.choicesContainer.GetChildren())
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
        this.currentChoices.Clear();
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
        this.characterDelay = 1.0f / this.TypewriterSpeed;
    }

    /// <summary>
    /// Get the current dialogue text.
    /// </summary>
    /// <returns>The current dialogue text.</returns>
    public string GetDialogueText()
    {
        return this.currentText;
    }

    /// <summary>
    /// Get the current character speaker.
    /// </summary>
    /// <returns>The current character speaker, or null if none.</returns>
    public Character? GetSpeaker()
    {
        return this.currentSpeaker;
    }

    /// <summary>
    /// Get the current dialogue choices.
    /// </summary>
    /// <returns>The current dialogue choices.</returns>
    public Collection<ChoiceOption> GetChoices()
    {
        return new Collection<ChoiceOption>(this.currentChoices);
    }

    /// <summary>
    /// Check if the dialogue system is currently typing text.
    /// </summary>
    /// <returns>True if the dialogue system is currently typing text, false otherwise.</returns>
    public bool IsTypingText()
    {
        return this.isTyping;
    }

    /// <summary>
    /// Check if the dialogue system is currently showing choices.
    /// </summary>
    /// <returns>True if the dialogue system is currently showing choices, false otherwise.</returns>
    public bool IsShowingChoices()
    {
        return this.choicesContainer != null && this.choicesContainer.Visible;
    }

    /// <summary>
    /// Refresh the dialogue system with current data.
    /// </summary>
    public void Refresh()
    {
        // Update the speaker display
        this.UpdateSpeakerDisplay(this.currentSpeaker);

        // Update the dialogue text
        if (this.dialogueText != null)
        {
            this.dialogueText.Text = this.currentText;
        }

        // Update the choices display
        this.ClearChoices();
        foreach (var choice in this.currentChoices)
        {
            this.CreateChoiceButton(choice);
        }

        // Show/hide the continue indicator based on typing state
        if (this.continueIndicator != null)
        {
            if (this.isTyping)
            {
                this.continueIndicator.Hide();
            }
            else
            {
                this.continueIndicator.Show();
            }
        }
    }

    /// <summary>
    /// Reset the dialogue system to its initial state.
    /// </summary>
    public void Reset()
    {
        this.HideDialogue();
        this.currentText = string.Empty;
        this.currentSpeaker = null;
        this.currentChoices.Clear();
        this.textPosition = 0;
        this.isTyping = false;

        // Reset the typewriter speed to default
        this.TypewriterSpeed = 50.0f;
        this.characterDelay = 1.0f / this.TypewriterSpeed;

        // Clear all displays
        if (this.dialogueText != null)
        {
            this.dialogueText.Text = string.Empty;
        }

        if (this.characterNameLabel != null)
        {
            this.characterNameLabel.Text = string.Empty;
            this.characterNameLabel.Hide();
        }

        if (this.characterPortrait != null)
        {
            this.characterPortrait.Texture = null;
            this.characterPortrait.Hide();
        }

        this.ClearChoices();

        if (this.continueIndicator != null)
        {
            this.continueIndicator.Hide();
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
            if (this.characterNameLabel != null)
            {
                this.characterNameLabel.Hide();
            }

            if (this.characterPortrait != null)
            {
                this.characterPortrait.Hide();
            }
        }
        else
        {
            // Show speaker information
            if (this.characterNameLabel != null)
            {
                this.characterNameLabel.Text = speaker.Name ?? string.Empty;
                this.characterNameLabel.Show();
            }

            // TODO: Add Portrait property to Character class
            // if (this.characterPortrait != null && speaker.Portrait != null)
            // {
            //     this.characterPortrait.Texture = speaker.Portrait;
            //     this.characterPortrait.Show();
            // }
            // else if (this.characterPortrait != null)
            // {
            //     this.characterPortrait.Hide();
            // }
        }
    }

    /// <summary>
    /// Create a choice button for a dialogue choice.
    /// </summary>
    /// <param name="choice">The dialogue choice to create a button for.</param>
    private void CreateChoiceButton(ChoiceOption choice)
    {
        if (choice == null || this.choicesContainer == null)
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
        this.choicesContainer.AddChild(button);
    }

    /// <summary>
    /// Clear all dialogue choices.
    /// </summary>
    private void ClearChoices()
    {
        if (this.choicesContainer == null)
        {
            return;
        }

        // Remove all existing choice buttons
        foreach (var child in this.choicesContainer.GetChildren())
        {
            if (child is Button button)
            {
                // Note: We can't remove the lambda event handler easily in Godot,
                // but since we're freeing the button immediately, it's not necessary
                button.QueueFree();
            }
        }

        // Hide the choices container
        this.choicesContainer.Hide();

        // Show the dialogue text again
        if (this.dialogueText != null)
        {
            this.dialogueText.Show();
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
        var choiceIndex = this.currentChoices.IndexOf(choice);
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
        if (this.isTyping)
        {
            // Skip the typewriter effect
            this.SkipTypewriter();
        }
        else if (this.choicesContainer != null && !this.choicesContainer.Visible)
        {
            // Continue to the next dialogue line or end dialogue
            // This would typically involve checking if there's more dialogue to display
            // or emitting a signal to indicate the player wants to continue

            // For now, we'll just hide the dialogue
            this.HideDialogue();
        }
    }
}
