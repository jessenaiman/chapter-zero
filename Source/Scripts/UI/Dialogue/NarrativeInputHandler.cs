namespace OmegaSpiral.Source.Scripts.UI.Dialogue
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Threading.Tasks;
    using Godot;

    /// <summary>
    /// Handles user input processing for the narrative terminal including choice selection and text input.
    /// </summary>
    public partial class NarrativeInputHandler : Node
    {
        private enum PromptKind
        {
            None,
            StoryChoice,
            PlayerName,
            Freeform,
        }

        private LineEdit inputField = default!;
        private Button submitButton = default!;
        private Label promptLabel = default!;
        private HBoxContainer inputRow = default!;

        private PromptKind currentPrompt = PromptKind.None;
        private bool awaitingInput;
        private TaskCompletionSource<ChoiceOption?>? choiceCompletion;
        private TaskCompletionSource<string>? inputCompletion;
        private List<ChoiceOption> activeChoices = new();

        /// <summary>
        /// Initializes the input handler with required UI components.
        /// </summary>
        /// <param name="inputField">The text input field for user responses.</param>
        /// <param name="submitButton">The button to submit user input.</param>
        /// <param name="promptLabel">The label displaying the current prompt text.</param>
        /// <param name="inputRow">The container holding input components.</param>
        public void Initialize(LineEdit inputField, Button submitButton, Label promptLabel, HBoxContainer inputRow)
        {
            this.inputField = inputField;
            this.submitButton = submitButton;
            this.promptLabel = promptLabel;
            this.inputRow = inputRow;

            this.submitButton.Pressed += this.OnSubmitPressed;
            this.inputField.TextSubmitted += _ => this.OnSubmitPressed();

            this.HidePrompt();
        }

        /// <summary>
        /// Displays a choice prompt and waits for user selection.
        /// </summary>
        /// <param name="choices">The list of available choice options.</param>
        /// <param name="promptText">The prompt text to display.</param>
        /// <returns>A task that completes with the selected choice option.</returns>
        public async Task<ChoiceOption?> AwaitChoiceAsync(List<ChoiceOption> choices, string promptText)
        {
            this.activeChoices = choices;
            this.ConfigurePrompt(PromptKind.StoryChoice, promptText);

            this.choiceCompletion = new TaskCompletionSource<ChoiceOption?>();
            ChoiceOption? selection = await this.choiceCompletion.Task;
            this.choiceCompletion = null;

            return selection;
        }

        /// <summary>
        /// Displays an input prompt and waits for user text input.
        /// </summary>
        /// <param name="promptText">The prompt text to display.</param>
        /// <returns>A task that completes with the user's input string.</returns>
        public async Task<string> AwaitInputAsync(string promptText)
        {
            this.ConfigurePrompt(PromptKind.PlayerName, promptText);

            this.inputCompletion = new TaskCompletionSource<string>();
            string input = await this.inputCompletion.Task;
            this.inputCompletion = null;

            return input;
        }

        /// <summary>
        /// Hides all input prompts and resets state.
        /// </summary>
        public void HidePrompt()
        {
            this.awaitingInput = false;
            this.currentPrompt = PromptKind.None;
            this.promptLabel.Visible = false;
            this.inputRow.Visible = false;
            this.submitButton.Visible = false;
            this.inputField.Text = string.Empty;
        }

        /// <summary>
        /// Attempts to complete the current choice prompt with the selected option.
        /// </summary>
        /// <param name="option">The choice option selected by the user.</param>
        public void CompleteChoice(ChoiceOption? option)
        {
            if (option == null || this.choiceCompletion == null)
            {
                return;
            }

            this.choiceCompletion.TrySetResult(option);
        }

        private void ConfigurePrompt(PromptKind promptKind, string promptText)
        {
            this.currentPrompt = promptKind;
            this.awaitingInput = promptKind != PromptKind.None;

            this.promptLabel.Visible = true;
            this.promptLabel.Text = promptText;

            bool needsInput = promptKind is PromptKind.PlayerName or PromptKind.Freeform or PromptKind.StoryChoice;
            this.inputRow.Visible = needsInput;
            this.submitButton.Visible = needsInput;
            this.inputField.Visible = needsInput;

            if (needsInput)
            {
                this.inputField.Text = string.Empty;
                this.inputField.PlaceholderText = promptKind switch
                {
                    PromptKind.PlayerName => "Type your response…",
                    PromptKind.StoryChoice => "Type number or choice label…",
                    _ => ">",
                };

                this.inputField.GrabFocus();
            }
        }

        private void OnSubmitPressed()
        {
            if (!this.awaitingInput)
            {
                return;
            }

            string rawInput = this.inputField.Text.Trim();
            if (string.IsNullOrEmpty(rawInput))
            {
                return;
            }

            this.inputField.Text = string.Empty;

            switch (this.currentPrompt)
            {
                case PromptKind.StoryChoice:
                    ChoiceOption? choice = this.ResolveChoiceOption(rawInput, this.activeChoices);
                    if (choice != null)
                    {
                        this.choiceCompletion?.TrySetResult(choice);
                    }
                    else
                    {
                        GD.Print("[color=#ffae42]Please choose a valid option (number or label).[/color]");
                    }

                    break;

                case PromptKind.PlayerName:
                case PromptKind.Freeform:
                    this.inputCompletion?.TrySetResult(rawInput);
                    break;
            }
        }

        private ChoiceOption? ResolveChoiceOption(string input, List<ChoiceOption> options)
        {
            if (options.Count == 0)
            {
                return null;
            }

            if (int.TryParse(input, NumberStyles.Integer, CultureInfo.InvariantCulture, out int index))
            {
                index -= 1;
                if (index >= 0 && index < options.Count)
                {
                    return options[index];
                }
            }

            foreach (ChoiceOption option in options)
            {
                if ((option.Id != null && option.Id.Equals(input, StringComparison.OrdinalIgnoreCase)) ||
                    (option.Text != null && option.Text.Equals(input, StringComparison.OrdinalIgnoreCase)))
                {
                    return option;
                }
            }

            return null;
        }
    }
}
