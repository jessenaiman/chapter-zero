// <copyright file="CharacterSelectionController.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using Godot;
using OmegaSpiral.Source.Scripts;
using OmegaSpiral.Source.Scripts.Infrastructure;

namespace OmegaSpiral.Source.Scripts.ui.Menus
{
    /// <summary>
    /// Controls the character selection scene where players choose their protagonist.
    /// Integrates with SceneManager to set player data before loading the game.
    /// </summary>
    [GlobalClass]
    public partial class CharacterSelectionController : Control
    {
        [Export]
        public string GameScenePath { get; set; } = "res://source/stages/stage_1/opening.tscn";

        private LineEdit? playerNameInput;
        private OptionButton? characterSelectDropdown;
        private Button? startButton;
        private Button? backButton;

        public override void _Ready()
        {
            this.CacheNodeReferences();
            this.ConnectSignals();
        }

        /// <summary>
        /// Caches references to UI nodes using unique names.
        /// </summary>
        private void CacheNodeReferences()
        {
            this.playerNameInput = this.GetNode<LineEdit>("%PlayerNameInput");
            this.characterSelectDropdown = this.GetNode<OptionButton>("%CharacterSelectDropdown");
            this.startButton = this.GetNode<Button>("%StartButton");
            this.backButton = this.GetNode<Button>("%BackButton");
        }

        /// <summary>
        /// Connects button signals to their respective handlers.
        /// </summary>
        private void ConnectSignals()
        {
            if (this.startButton is not null)
            {
                this.startButton.Pressed += this.OnStartPressed;
            }

            if (this.backButton is not null)
            {
                this.backButton.Pressed += this.OnBackPressed;
            }
        }

        /// <summary>
        /// Handles start button press - validates input and transitions to game.
        /// </summary>
        private void OnStartPressed()
        {
            if (this.playerNameInput is null || this.characterSelectDropdown is null)
            {
                GD.PrintErr("UI references not properly initialized");
                return;
            }

            string playerName = this.playerNameInput.Text.Trim();
            if (string.IsNullOrEmpty(playerName))
            {
                GD.PrintErr("Player name cannot be empty");
                return;
            }

            string selectedCharacter = this.characterSelectDropdown.GetItemText(
                this.characterSelectDropdown.Selected
            );

            // Store player data in SceneManager
            var sceneManager = this.GetNode<SceneManager>("/root/SceneManager");
            if (sceneManager is not null)
            {
                sceneManager.SetPlayerName(playerName);
                sceneManager.SetDreamweaverThread(selectedCharacter.ToLower(System.Globalization.CultureInfo.InvariantCulture));
            }

            GD.Print($"Starting game with player: {playerName}, character: {selectedCharacter}");
            this.GetTree().ChangeSceneToFile(this.GameScenePath);
        }

        /// <summary>
        /// Handles back button press - returns to main menu.
        /// </summary>
        private void OnBackPressed()
        {
            GD.Print("Returning to main menu from character selection...");
            this.GetTree().ChangeSceneToFile((string) GetNode("/root/AppConfig").Get("main_menu_scene_path"));
        }
    }
}
