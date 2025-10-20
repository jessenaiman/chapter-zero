// <copyright file="MainMenu.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using Godot;

namespace OmegaSpiral.Source.Scripts.ui.Menus
{
    /// <summary>
    /// Controls the main menu scene with character selection, options, and navigation.
    /// </summary>
    public partial class MainMenuController : Control
    {
        [Export]
        public string? GameScenePath { get; set; }

        [Export]
        public string? CharacterSelectionPath { get; set; }

        private Button? newGameButton;
        private Button? continueButton;
        private Button? optionsButton;
        private Button? creditsButton;
        private Button? exitButton;

        public override void _Ready()
        {
            this.CacheButtonReferences();
            this.ConnectButtonSignals();
            this.UpdateContinueButtonVisibility();

            if (string.IsNullOrEmpty(GameScenePath))
            {
                GameScenePath = (string) GetNode("/root/AppConfig").Get("game_scene_path");
            }

            if (string.IsNullOrEmpty(CharacterSelectionPath))
            {
                CharacterSelectionPath = (string) GetNode("/root/AppConfig").Get("game_scene_path");
            }
        }

        /// <summary>
        /// Caches references to UI buttons using unique names.
        /// </summary>
        private void CacheButtonReferences()
        {
            this.newGameButton = this.GetNode<Button>("%NewGameButton");
            this.continueButton = this.GetNode<Button>("%ContinueButton");
            this.optionsButton = this.GetNode<Button>("%OptionsButton");
            this.creditsButton = this.GetNode<Button>("%CreditsButton");
            this.exitButton = this.GetNode<Button>("%ExitButton");
        }

        /// <summary>
        /// Connects button signals to their respective handlers.
        /// </summary>
        private void ConnectButtonSignals()
        {
            if (this.newGameButton is not null)
            {
                this.newGameButton.Pressed += this.OnNewGamePressed;
            }

            if (this.continueButton is not null)
            {
                this.continueButton.Pressed += this.OnContinuePressed;
            }

            if (this.optionsButton is not null)
            {
                this.optionsButton.Pressed += this.OnOptionsPressed;
            }

            if (this.creditsButton is not null)
            {
                this.creditsButton.Pressed += this.OnCreditsPressed;
            }

            if (this.exitButton is not null)
            {
                this.exitButton.Pressed += this.OnExitPressed;
            }
        }

        /// <summary>
        /// Updates the continue button visibility based on game state.
        /// </summary>
        private void UpdateContinueButtonVisibility()
        {
            var gameState = this.GetNode<Node>("/root/GameState");
            if (gameState is not null)
            {
                bool hasSaveData = false; // TODO: Implement save data check
                if (this.continueButton is not null)
                {
                    this.continueButton.Visible = hasSaveData;
                }
            }
        }

        /// <summary>
        /// Handles new game button press - transitions to character selection.
        /// </summary>
        private void OnNewGamePressed()
        {
            GD.Print("Starting new game...");
            if (this.TryGetSceneManager(out var sceneManager))
            {
                sceneManager.TransitionToScene("Stage1Boot");
                return;
            }

            Error result = this.GetTree().ChangeSceneToFile(this.GameScenePath);
            if (result != Error.Ok)
            {
                GD.PrintErr($"Failed to load game scene at '{this.GameScenePath}': {result}");
            }
        }

        /// <summary>
        /// Handles continue button press - loads last save.
        /// </summary>
        private void OnContinuePressed()
        {
            GD.Print("Continuing game...");
            // TODO: Load save data and navigate to appropriate scene
            Error result = this.GetTree().ChangeSceneToFile(this.GameScenePath);
            if (result != Error.Ok)
            {
                GD.PrintErr($"Failed to load game scene at '{this.GameScenePath}': {result}");
            }
        }

        /// <summary>
        /// Handles options button press - opens options menu.
        /// </summary>
        private void OnOptionsPressed()
        {
            GD.Print("Opening options menu...");
            // TODO: Emit signal or navigate to options scene
        }

        /// <summary>
        /// Handles credits button press - shows credits scene.
        /// </summary>
        private void OnCreditsPressed()
        {
            GD.Print("Opening credits...");
            this.GetTree().ChangeSceneToFile("res://addons/maaacks_game_template/base/scenes/credits/Credits.tscn");
        }

        /// <summary>
        /// Handles exit button press - quits the application.
        /// </summary>
        private void OnExitPressed()
        {
            GD.Print("Exiting game...");
            this.GetTree().Quit();
        }

        private bool TryGetSceneManager(out SceneManager sceneManager)
        {
            sceneManager = this.GetNodeOrNull<SceneManager>("/root/SceneManager");
            return sceneManager is not null;
        }
    }
}
