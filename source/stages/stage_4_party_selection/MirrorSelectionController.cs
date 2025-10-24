// <copyright file="MirrorSelectionController.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using Godot;
using OmegaSpiral.Source.Scripts.Common;
using OmegaSpiral.Source.Narrative;

namespace OmegaSpiral.Source.Stages.Stage4
{
    /// <summary>
    /// Controls the mirror selection Ui for character choice in Echo Vault stage (Stage 4).
    /// Displays available characters and handles selection input.
    /// </summary>
    [GlobalClass]
    public partial class MirrorSelectionController : Control
    {

                [Export]
                private Label? promptLabel;

                [Export]
                private VBoxContainer? characterContainer;

                [Export]
                private Label? dwCommentLabel;

                [Export]
                private Button? confirmButton;

                /// <summary>
                /// Disposes of all IDisposable fields.
                /// </summary>
                public new void Dispose()
                {
                    promptLabel?.Dispose();
                    characterContainer?.Dispose();
                    dwCommentLabel?.Dispose();
                    confirmButton?.Dispose();
                }

                ~MirrorSelectionController()
                {
                    Dispose();
                }

        private Godot.Collections.Array<Godot.Collections.Dictionary<string, Variant>> availableCharacters = new();
        private List<CharacterData> characterDataList = new();
        private Stage4Main? stageController;
        private string currentBeatId = string.Empty;
        private string currentPrompt = string.Empty;
        private string dwReflection = string.Empty;

        /// <summary>
        /// Emitted when a character is selected.
        /// </summary>
        /// <param name="characterId">The selected character ID.</param>
        [Signal]
        public delegate void CharacterSelectedEventHandler(string characterId);

        /// <inheritdoc/>
        public override void _Ready()
        {
            this.stageController = this.GetNode<Stage4Main>("/root/Stage4Main");
            this.Connect(Stage4Main.SignalName.CharacterSelected, new Callable(this, nameof(OnCharacterSelected)));
            if (this.confirmButton != null)
            {
                this.confirmButton.Pressed += this.OnConfirmPressed;
            }
        }

        /// <summary>
        /// Initializes the selection Ui with beat data.
        /// </summary>
        /// <param name="beatId">The current beat identifier.</param>
        /// <param name="availableChars">The array of available character data.</param>
        /// <param name="prompt">The selection prompt text.</param>
        /// <param name="dwIntro">The Dreamweaver intro comments.</param>
        public void InitializeSelection(string beatId, Godot.Collections.Array<Godot.Collections.Dictionary<string, Variant>> availableChars, string prompt, Godot.Collections.Dictionary<string, Variant> dwIntro)
        {
            this.currentBeatId = beatId;
            this.availableCharacters = availableChars;
            this.currentPrompt = prompt;
            this.dwReflection = this.GetDominantDwReflection();

            if (this.promptLabel != null)
            {
                this.promptLabel.Text = prompt;
            }

            this.DisplayDwComment(dwIntro);
            this.PopulateCharacterButtons();
            this.Show();
        }

        private void PopulateCharacterButtons()
        {
            this.ClearContainerChildren();
            this.characterDataList.Clear();

            foreach (var charDict in this.availableCharacters)
            {
                var characterData = CharacterData.FromDictionary(charDict);
                this.characterDataList.Add(characterData);

                var button = new Button();
                button.Text = characterData.Name;
                button.SizeFlagsHorizontal = SizeFlags.ExpandFill;
                button.Pressed += () => this.OnCharacterButtonPressed(characterData.Id);

                var descriptionLabel = new Label();
                descriptionLabel.Text = characterData.Description;
                descriptionLabel.HorizontalAlignment = HorizontalAlignment.Left;
                descriptionLabel.AddThemeFontSizeOverride("font_size", 12);

                var hbox = new HBoxContainer();
                hbox.AddChild(button);
                hbox.AddChild(descriptionLabel);

                if (this.characterContainer != null)
                {
                    this.characterContainer.AddChild(hbox);
                }
            }
        }

        private void OnCharacterButtonPressed(string characterId)
        {
            GD.Print($"Character button pressed: {characterId}");
            this.stageController?.EmitSignal(Stage4Main.SignalName.CharacterSelected, characterId);
        }

        private void OnConfirmPressed()
        {
            // For multi-selection beats, confirm would finalize choices
            GD.Print("Confirm selection pressed");
            this.Hide();
        }

        private void DisplayDwComment(Godot.Collections.Dictionary<string, Variant> dwComments)
        {
            if (dwComments.TryGetValue(this.dwReflection, out var commentVar))
            {
                var comment = commentVar.AsString();
                if (this.dwCommentLabel != null)
                {
                    this.dwCommentLabel.Text = comment;
                }
            }
            else
            {
                if (this.dwCommentLabel != null)
                {
                    this.dwCommentLabel.Text = "The echoes whisper guidance...";
                }
            }
        }

        private string GetDominantDwReflection()
        {
            // Determine dominant Dreamweaver based on GameState
            var gameState = this.GetNode<GameState>("/root/GameState");
            var scores = gameState.DreamweaverScores;
            var maxScore = 0;
            var dominant = "light";

            if (scores.TryGetValue(DreamweaverType.Light, out var lightScore) && lightScore > maxScore)
            {
                dominant = "light";
                maxScore = lightScore;
            }

            if (scores.TryGetValue(DreamweaverType.Mischief, out var mischiefScore) && mischiefScore > maxScore)
            {
                dominant = "mischief";
                maxScore = mischiefScore;
            }

            if (scores.TryGetValue(DreamweaverType.Wrath, out var wrathScore) && wrathScore > maxScore)
            {
                dominant = "wrath";
            }

            return dominant;
        }

        private void OnCharacterSelected(string characterId)
        {
            // Update Ui to show selection
            GD.Print($"Character selected in Ui: {characterId}");
            if (this.confirmButton != null)
            {
                this.confirmButton.Disabled = false;
            }
        }

        private void ClearContainerChildren()
        {
            if (this.characterContainer != null)
            {
                while (this.characterContainer.GetChildCount() > 0)
                {
                    this.characterContainer.GetChild(0).QueueFree();
                }
            }
        }

        /// <summary>
        /// Clears the current selection Ui.
        /// </summary>
        public void ClearSelection()
        {
            this.ClearContainerChildren();
            this.characterDataList.Clear();
            this.Hide();
        }
    }
}
