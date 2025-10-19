// <copyright file="CreditsController.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using Godot;
using OmegaSpiral.Source.Scripts.Infrastructure;

namespace OmegaSpiral.Source.Scripts.UI.Menus
{
    /// <summary>
    /// Controls the credits scene with scrolling credits display and navigation.
    /// </summary>
    [GlobalClass]
    public partial class CreditsController : Control
    {
        [Export]
        public float ScrollSpeed { get; set; } = 50.0f;

        [Export]
        public string CreditsTextPath { get; set; } = "res://Source/Data/credits.txt";

        private RichTextLabel? creditsDisplay;
        private Button? backButton;
        private float scrollProgress;

        public override void _Ready()
        {
            this.CacheNodeReferences();
            this.ConnectSignals();
            this.LoadCreditsText();
        }

        public override void _Process(double delta)
        {
            // For now, just handle input for skipping credits
            // Scrolling functionality needs to be implemented differently in Godot 4.x
            if (Input.IsActionJustPressed("ui_accept") || Input.IsActionJustPressed("ui_cancel"))
            {
                this.ReturnToMainMenu();
            }
        }

        /// <summary>
        /// Caches references to UI nodes using unique names.
        /// </summary>
        private void CacheNodeReferences()
        {
            this.creditsDisplay = this.GetNode<RichTextLabel>("%CreditsDisplay");
            this.backButton = this.GetNode<Button>("%BackButton");
        }

        /// <summary>
        /// Connects button signals to their respective handlers.
        /// </summary>
        private void ConnectSignals()
        {
            if (this.backButton is not null)
            {
                this.backButton.Pressed += this.ReturnToMainMenu;
            }
        }

        /// <summary>
        /// Loads the credits text from a file.
        /// </summary>
        private void LoadCreditsText()
        {
            if (this.creditsDisplay is null)
            {
                return;
            }

            var file = Godot.FileAccess.Open(this.CreditsTextPath, Godot.FileAccess.ModeFlags.Read);
            if (file is null)
            {
                GD.PrintErr($"Failed to load credits from {this.CreditsTextPath}");
                this.creditsDisplay.Text = "Credits data not found.";
                return;
            }

            string creditsContent = file.GetAsText();
            this.creditsDisplay.Text = creditsContent;
            GD.Print("Credits loaded successfully");
        }

        /// <summary>
        /// Returns to the main menu.
        /// </summary>
        private void ReturnToMainMenu()
        {
            GD.Print("Returning to main menu from credits...");
            this.GetTree().ChangeSceneToFile(AppConfig.MainMenuScenePath);
        }
    }
}
