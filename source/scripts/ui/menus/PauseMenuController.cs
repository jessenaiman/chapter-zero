// <copyright file="PauseMenuController.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using Godot;

namespace OmegaSpiral.Source.Scripts.ui.Menus
{
    /// <summary>
    /// Controls the in-game pause menu with options to resume, access settings, or return to main menu.
    /// </summary>
    [GlobalClass]
    public partial class PauseMenuController : CanvasLayer
    {
        [Export]
        public string MainMenuPath { get; set; } = "";

        private Control? pausePanel;
        private Button? resumeButton;
        private Button? optionsButton;
        private Button? mainMenuButton;
        private bool isPaused;

        public override void _Ready()
        {
            this.CacheNodeReferences();
            this.ConnectSignals();
            this.HidePauseMenu();

            if (string.IsNullOrEmpty(MainMenuPath))
            {
                MainMenuPath = (string) GetNode("/root/AppConfig").Get("main_menu_scene_path");
            }
        }

        public override void _Process(double delta)
        {
            if (Input.IsActionJustPressed("ui_cancel"))
            {
                this.TogglePause();
            }
        }

        /// <summary>
        /// Caches references to UI nodes using unique names.
        /// </summary>
        private void CacheNodeReferences()
        {
            this.pausePanel = this.GetNode<Control>("%PausePanel");
            this.resumeButton = this.GetNode<Button>("%ResumeButton");
            this.optionsButton = this.GetNode<Button>("%OptionsButton");
            this.mainMenuButton = this.GetNode<Button>("%MainMenuButton");
        }

        /// <summary>
        /// Connects button signals to their respective handlers.
        /// </summary>
        private void ConnectSignals()
        {
            if (this.resumeButton is not null)
            {
                this.resumeButton.Pressed += this.OnResumePressed;
            }

            if (this.optionsButton is not null)
            {
                this.optionsButton.Pressed += this.OnOptionsPressed;
            }

            if (this.mainMenuButton is not null)
            {
                this.mainMenuButton.Pressed += this.OnMainMenuPressed;
            }
        }

        /// <summary>
        /// Toggles the pause state of the game.
        /// </summary>
        private void TogglePause()
        {
            this.isPaused = !this.isPaused;

            if (this.isPaused)
            {
                this.ShowPauseMenu();
            }
            else
            {
                this.HidePauseMenu();
            }
        }

        /// <summary>
        /// Shows the pause menu and pauses the game.
        /// </summary>
        private void ShowPauseMenu()
        {
            if (this.pausePanel is not null)
            {
                this.pausePanel.Show();
            }

            this.GetTree().Paused = true;
            GD.Print("Game paused");
        }

        /// <summary>
        /// Hides the pause menu and resumes the game.
        /// </summary>
        private void HidePauseMenu()
        {
            if (this.pausePanel is not null)
            {
                this.pausePanel.Hide();
            }

            this.GetTree().Paused = false;
            GD.Print("Game resumed");
        }

        /// <summary>
        /// Handles resume button press.
        /// </summary>
        private void OnResumePressed()
        {
            this.TogglePause();
        }

        /// <summary>
        /// Handles options button press.
        /// </summary>
        private void OnOptionsPressed()
        {
            GD.Print("Opening pause menu options...");
            // TODO: Implement options overlay for pause menu
        }

        /// <summary>
        /// Handles main menu button press - returns to main menu.
        /// </summary>
        private void OnMainMenuPressed()
        {
            GD.Print("Returning to main menu...");
            this.GetTree().Paused = false;
            this.GetTree().ChangeSceneToFile(this.MainMenuPath);
        }
    }
}
