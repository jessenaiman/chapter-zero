using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OmegaSpiral.Source.Scripts
{
    /// <summary>
    /// Container for the dialogue system display.
    /// The UIDialogue manages the presentation of character dialogue, narrative text,
    /// and conversation options during gameplay. It handles typewriter effects,
    /// character portraits, dialogue choices, and branching conversation trees.
    /// It provides a rich interface for storytelling and player interaction with NPCs.
    /// </summary>
    public partial class UIDialogue : Control
    {
        /// <summary>
        /// Emitted when dialogue is finished displaying.
        /// </summary>
        [Signal]
        public delegate void DialogueFinishedEventHandler();

        /// <summary>
        /// Emitted when a dialogue choice is selected.
        /// </summary>
        [Signal]
        public delegate void ChoiceSelectedEventHandler(int choiceIndex);

        /// <summary>
        /// Emitted when dialogue starts.
        /// </summary>
        [Signal]
        public delegate void DialogueStartedEventHandler();

        /// <summary>
        /// Whether the dialogue system is currently active.
        /// </summary>
        public bool IsActive { get; private set; } = false;

        /// <summary>
        /// Whether the dialogue is currently visible.
        /// </summary>
        public bool DialogueVisible
        {
            get => Visible;
            set => Visible = value;
        }

        /// <summary>
        /// The container for dialogue text.
        /// </summary>
        private RichTextLabel dialogueText;

        /// <summary>
        /// The container for character name display.
        /// </summary>
        private Label characterNameLabel;

        /// <summary>
        /// The container for character portrait.
        /// </summary>
        private TextureRect characterPortrait;

        /// <summary>
        /// The container for dialogue choices.
        /// </summary>
        private VBoxContainer choicesContainer;

        /// <summary>
        /// The continue indicator (e.g., blinking cursor).
        /// </summary>
        private Control continueIndicator;

        /// <summary>
        /// The current dialogue text being displayed.
        /// </summary>
        private string currentText = "";

        /// <summary>
        /// The current character speaking.
        /// </summary>
        private Character currentSpeaker;

        /// <summary>
        /// The current dialogue choices.
        /// </summary>
        private List<DialogueChoice> currentChoices = new List<DialogueChoice>();

        /// <summary>
        /// Whether the dialogue text is currently being typed out.
        /// </summary>
        private bool isTyping = false;

        /// <summary>
        /// The current position in the dialogue text.
        /// </summary>
        private int textPosition = 0;

        /// <summary>
        /// The typewriter effect speed (characters per second).
        /// </summary>
        [Export]
        public float TypewriterSpeed { get; set; } = 50.0f;

        /// <summary>
        /// The time between each character in the typewriter effect.
        /// </summary>
        private float characterDelay = 0.0f;

        /// <summary>
        /// Timer for typewriter effect.
        /// </summary>
        private Godot.Timer typewriterTimer;

        public override void _Ready()
        {
            // Get references to child UI elements
            dialogueText = GetNode<RichTextLabel>("DialogueText");
            characterNameLabel = GetNode<Label>("CharacterName");
            characterPortrait = GetNode<TextureRect>("CharacterPortrait");
            choicesContainer = GetNode<VBoxContainer>("ChoicesContainer");
            continueIndicator = GetNode<Control>("ContinueIndicator");

            // Initially hide the dialogue system
            Visible = false;

            // Set up the typewriter timer
            typewriterTimer = new Timer();
            typewriterTimer.OneShot = true;
            typewriterTimer.Timeout += OnTypewriterTimeout;
            AddChild(typewriterTimer);

            // Connect to any necessary signals
            ConnectSignals();
        }

        /// <summary>
        /// Connect to necessary signals.
        /// </summary>
        private void ConnectSignals()
        {
            // Connect to dialogue events
            // DialogueEvents.DialogueStarted += OnDialogueStarted;
            // DialogueEvents.DialogueEnded += OnDialogueEnded;
            // DialogueEvents.ChoiceSelected += OnChoiceSelected;
        }

        /// <summary>
        /// Start displaying dialogue.
        /// </summary>
        /// <param name="text">The dialogue text to display</param>
        /// <param name="speaker">The character speaking (optional)</param>
        public async void StartDialogue(string text, Character? speaker = null)
        {
            if (string.IsNullOrEmpty(text))
            {
                return;
            }

            // Set the current dialogue state
            IsActive = true;
            currentText = text;
            currentSpeaker = speaker;
            textPosition = 0;
            isTyping = true;

            // Show the dialogue system
            Visible = true;

            // Update the speaker information
            UpdateSpeakerDisplay(speaker);

            // Clear any existing choices
            ClearChoices();

            // Hide the continue indicator while typing
            if (continueIndicator != null)
            {
                continueIndicator.Hide();
            }

            // Emit the dialogue started signal
            EmitSignal(SignalName.DialogueStarted);

            // Start the typewriter effect
            await TypeText(text);
        }

        /// <summary>
        /// Display dialogue choices.
        /// </summary>
        /// <param name="choices">The dialogue choices to display</param>
        public void ShowChoices(List<DialogueChoice> choices)
        {
            if (choices == null || choices.Count == 0)
            {
                return;
            }

            // Store the current choices
            currentChoices = new List<DialogueChoice>(choices);

            // Clear any existing choices
            ClearChoices();

            // Create choice buttons for each choice
            foreach (var choice in choices)
            {
                CreateChoiceButton(choice);
            }

            // Show the choices container
            if (choicesContainer != null)
            {
                choicesContainer.Show();
            }

            // Hide the dialogue text and continue indicator
            if (dialogueText != null)
            {
                dialogueText.Hide();
            }

            if (continueIndicator != null)
            {
                continueIndicator.Hide();
            }
        }

        /// <summary>
        /// Hide the dialogue system.
        /// </summary>
        public void HideDialogue()
        {
            Visible = false;
            IsActive = false;
            isTyping = false;
            currentText = "";
            currentSpeaker = null;
            textPosition = 0;

            // Clear any existing choices
            ClearChoices();

            // Stop the typewriter timer
            if (typewriterTimer != null)
            {
                typewriterTimer.Stop();
            }

            // Emit the dialogue finished signal
            EmitSignal(SignalName.DialogueFinished);
        }

        /// <summary>
        /// Skip the current typewriter effect and display the full text immediately.
        /// </summary>
        public void SkipTypewriter()
        {
            if (!isTyping || string.IsNullOrEmpty(currentText))
            {
                return;
            }

            // Stop the typewriter timer
            if (typewriterTimer != null)
            {
                typewriterTimer.Stop();
            }

            // Display the full text immediately
            if (dialogueText != null)
            {
                dialogueText.Text = currentText;
            }

            // Set the typing state to false
            isTyping = false;
            textPosition = currentText.Length;

            // Show the continue indicator
            if (continueIndicator != null)
            {
                continueIndicator.Show();
            }
        }

        /// <summary>
        /// Type out text with a typewriter effect.
        /// </summary>
        /// <param name="text">The text to type out</param>
        private async Task TypeText(string text)
        {
            if (string.IsNullOrEmpty(text) || dialogueText == null)
            {
                return;
            }

            // Calculate the character delay based on the typewriter speed
            characterDelay = 1.0f / TypewriterSpeed;

            // Type out each character
            while (textPosition < text.Length && isTyping)
            {
                // Add the next character to the displayed text
                if (dialogueText != null)
                {
                    dialogueText.Text = text.Substring(0, textPosition + 1);
                }

                textPosition++;

                // Wait for the character delay
                typewriterTimer.Start(characterDelay);
                await ToSignal(typewriterTimer, Timer.SignalName.Timeout);
            }

            // If we've finished typing, show the continue indicator
            if (textPosition >= text.Length && isTyping)
            {
                isTyping = false;
                if (continueIndicator != null)
                {
                    continueIndicator.Show();
                }
            }
        }

        /// <summary>
        /// Update the speaker display with the current speaker's information.
        /// </summary>
        /// <param name="speaker">The character speaking</param>
        private void UpdateSpeakerDisplay(Character speaker)
        {
            if (speaker == null)
            {
                // Hide speaker information if no speaker
                if (characterNameLabel != null)
                {
                    characterNameLabel.Hide();
                }

                if (characterPortrait != null)
                {
                    characterPortrait.Hide();
                }
            }
            else
            {
                // Show speaker information
                if (characterNameLabel != null)
                {
                    characterNameLabel.Text = speaker.Name;
                    characterNameLabel.Show();
                }

                if (characterPortrait != null && speaker.Portrait != null)
                {
                    characterPortrait.Texture = speaker.Portrait;
                    characterPortrait.Show();
                }
                else if (characterPortrait != null)
                {
                    characterPortrait.Hide();
                }
            }
        }

        /// <summary>
        /// Create a choice button for a dialogue choice.
        /// </summary>
        /// <param name="choice">The dialogue choice to create a button for</param>
        private void CreateChoiceButton(DialogueChoice choice)
        {
            if (choice == null || choicesContainer == null)
            {
                return;
            }

            // Create a new button for the choice
            var button = new Button();
            button.Text = choice.Text;
            button.FocusMode = FocusModeEnum.None;

            // Connect the button's pressed signal to handle choice selection
            button.Pressed += () => OnChoiceButtonPressed(choice);

            // Add the button to the choices container
            choicesContainer.AddChild(button);
        }

        /// <summary>
        /// Clear all dialogue choices.
        /// </summary>
        private void ClearChoices()
        {
            if (choicesContainer == null)
            {
                return;
            }

            // Remove all existing choice buttons
            foreach (var child in choicesContainer.GetChildren())
            {
                if (child is Button button)
                {
                    button.Pressed -= OnChoiceButtonPressed;
                    button.QueueFree();
                }
            }

            // Hide the choices container
            choicesContainer.Hide();

            // Show the dialogue text again
            if (dialogueText != null)
            {
                dialogueText.Show();
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
        /// <param name="choice">The selected dialogue choice</param>
        private void OnChoiceButtonPressed(DialogueChoice choice)
        {
            if (choice == null || !IsActive)
            {
                return;
            }

            // Find the index of the selected choice
            var choiceIndex = currentChoices.IndexOf(choice);
            if (choiceIndex >= 0)
            {
                // Emit the choice selected signal
                EmitSignal(SignalName.ChoiceSelected, choiceIndex);

                // Hide the choices
                ClearChoices();
            }
        }

        /// <summary>
        /// Callback when input is received.
        /// </summary>
        /// <param name="inputEvent">The input event</param>
        public override void _Input(InputEvent inputEvent)
        {
            if (!IsActive || !Visible)
            {
                return;
            }

            // Check for continue input (e.g., spacebar or enter)
            if (inputEvent is InputEventKey keyEvent && keyEvent.Pressed)
            {
                if (keyEvent.Keycode == Key.Space || keyEvent.Keycode == Key.Enter)
                {
                    OnContinueInput();
                }
            }
            else if (inputEvent is InputEventMouseButton mouseEvent && mouseEvent.Pressed)
            {
                if (mouseEvent.ButtonIndex == MouseButton.Left)
                {
                    OnContinueInput();
                }
            }
        }

        /// <summary>
        /// Callback when continue input is received.
        /// </summary>
        private void OnContinueInput()
        {
            if (isTyping)
            {
                // Skip the typewriter effect
                SkipTypewriter();
            }
            else if (choicesContainer != null && !choicesContainer.Visible)
            {
                // Continue to the next dialogue line or end dialogue
                // This would typically involve checking if there's more dialogue to display
                // or emitting a signal to indicate the player wants to continue

                // For now, we'll just hide the dialogue
                HideDialogue();
            }
        }

        /// <summary>
        /// Show a message in the dialogue system.
        /// </summary>
        /// <param name="message">The message to show</param>
        /// <param name="duration">The duration to show the message for</param>
        public async void ShowMessage(string message, float duration = 2.0f)
        {
            if (string.IsNullOrEmpty(message))
            {
                return;
            }

            // Show the message in the dialogue text
            if (dialogueText != null)
            {
                dialogueText.Text = message;
            }

            // Show the dialogue system
            Visible = true;

            // Wait for the specified duration
            await Task.Delay(TimeSpan.FromSeconds(duration));

            // Hide the dialogue system
            HideDialogue();
        }

        /// <summary>
        /// Show an effect label (like emotion indicators or emphasis).
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
        /// Update the dialogue text display.
        /// </summary>
        /// <param name="text">The new dialogue text</param>
        public void UpdateDialogueText(string text)
        {
            if (string.IsNullOrEmpty(text) || dialogueText == null)
            {
                return;
            }

            // Update the dialogue text
            dialogueText.Text = text;
        }

        /// <summary>
        /// Update the character name display.
        /// </summary>
        /// <param name="name">The new character name</param>
        public void UpdateCharacterName(string name)
        {
            if (string.IsNullOrEmpty(name) || characterNameLabel == null)
            {
                return;
            }

            // Update the character name
            characterNameLabel.Text = name;
        }

        /// <summary>
        /// Update the character portrait display.
        /// </summary>
        /// <param name="portrait">The new character portrait</param>
        public void UpdateCharacterPortrait(Texture2D portrait)
        {
            if (portrait == null || characterPortrait == null)
            {
                return;
            }

            // Update the character portrait
            characterPortrait.Texture = portrait;
        }

        /// <summary>
        /// Add a dialogue choice.
        /// </summary>
        /// <param name="choice">The dialogue choice to add</param>
        public void AddChoice(DialogueChoice choice)
        {
            if (choice == null)
            {
                return;
            }

            // Add the choice to the current choices list
            currentChoices.Add(choice);

            // Create a button for the choice
            CreateChoiceButton(choice);
        }

        /// <summary>
        /// Remove a dialogue choice.
        /// </summary>
        /// <param name="choice">The dialogue choice to remove</param>
        public void RemoveChoice(DialogueChoice choice)
        {
            if (choice == null)
            {
                return;
            }

            // Remove the choice from the current choices list
            currentChoices.Remove(choice);

            // Remove the corresponding button
            if (choicesContainer != null)
            {
                foreach (var child in choicesContainer.GetChildren())
                {
                    if (child is Button button && button.Text == choice.Text)
                    {
                        button.Pressed -= OnChoiceButtonPressed;
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
            currentChoices.Clear();
            ClearChoices();
        }

        /// <summary>
        /// Set the typewriter speed.
        /// </summary>
        /// <param name="speed">The new typewriter speed (characters per second)</param>
        public void SetTypewriterSpeed(float speed)
        {
            if (speed <= 0)
            {
                return;
            }

            TypewriterSpeed = speed;
            characterDelay = 1.0f / TypewriterSpeed;
        }

        /// <summary>
        /// Get the current dialogue text.
        /// </summary>
        /// <returns>The current dialogue text</returns>
        public string GetDialogueText()
        {
            return currentText;
        }

        /// <summary>
        /// Get the current character speaker.
        /// </summary>
        /// <returns>The current character speaker</returns>
        public Character GetSpeaker()
        {
            return currentSpeaker;
        }

        /// <summary>
        /// Get the current dialogue choices.
        /// </summary>
        /// <returns>The current dialogue choices</returns>
        public List<DialogueChoice> GetChoices()
        {
            return new List<DialogueChoice>(currentChoices);
        }

        /// <summary>
        /// Check if the dialogue system is currently typing text.
        /// </summary>
        /// <returns>True if the dialogue system is currently typing text, false otherwise</returns>
        public bool IsTypingText()
        {
            return isTyping;
        }

        /// <summary>
        /// Check if the dialogue system is currently showing choices.
        /// </summary>
        /// <returns>True if the dialogue system is currently showing choices, false otherwise</returns>
        public bool IsShowingChoices()
        {
            return choicesContainer != null && choicesContainer.Visible;
        }

        /// <summary>
        /// Refresh the dialogue system with current data.
        /// </summary>
        public void Refresh()
        {
            // Update the speaker display
            UpdateSpeakerDisplay(currentSpeaker);

            // Update the dialogue text
            if (dialogueText != null)
            {
                dialogueText.Text = currentText;
            }

            // Update the choices display
            ClearChoices();
            foreach (var choice in currentChoices)
            {
                CreateChoiceButton(choice);
            }

            // Show/hide the continue indicator based on typing state
            if (continueIndicator != null)
            {
                if (isTyping)
                {
                    continueIndicator.Hide();
                }
                else
                {
                    continueIndicator.Show();
                }
            }
        }

        /// <summary>
        /// Reset the dialogue system to its initial state.
        /// </summary>
        public void Reset()
        {
            HideDialogue();
            currentText = "";
            currentSpeaker = null;
            currentChoices.Clear();
            textPosition = 0;
            isTyping = false;

            // Reset the typewriter speed to default
            TypewriterSpeed = 50.0f;
            characterDelay = 1.0f / TypewriterSpeed;

            // Clear all displays
            if (dialogueText != null)
            {
                dialogueText.Text = "";
            }

            if (characterNameLabel != null)
            {
                characterNameLabel.Text = "";
                characterNameLabel.Hide();
            }

            if (characterPortrait != null)
            {
                characterPortrait.Texture = null;
                characterPortrait.Hide();
            }

            ClearChoices();

            if (continueIndicator != null)
            {
                continueIndicator.Hide();
            }
        }
    }
}
