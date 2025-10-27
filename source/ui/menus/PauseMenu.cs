// <copyright file="PauseMenu.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using Godot;
using OmegaSpiral.Source.Ui.Menus;

namespace OmegaSpiral.Source.Ui.Menus
{
    /// <summary>
    /// Pause menu that appears during gameplay.
    /// Provides options to resume, restart, or return to main menu.
    /// Extends MenuUi for consistent menu behavior and styling.
    /// </summary>
    [GlobalClass]
    public partial class PauseMenu : BaseMenuUi
    {
        // --- EXPORTED PROPERTIES ---

        /// <summary>
        /// Whether the pause menu blocks input to the game world.
        /// </summary>
        [Export] public bool Modal { get; set; } = true;

        // --- PRIVATE FIELDS ---

        private Button? _ResumeButton;
        private Button? _RestartButton;
        private Button? _MainMenuButton;
        private Button? _QuitButton;
        private ColorRect? _Background;

        // --- PUBLIC PROPERTIES ---

        /// <summary>
        /// Gets the Resume Game button.
        /// </summary>
        public Button? ResumeButton => _ResumeButton;

        /// <summary>
        /// Gets the Restart Level button.
        /// </summary>
        public Button? RestartButton => _RestartButton;

        /// <summary>
        /// Gets the Main Menu button.
        /// </summary>
        public Button? MainMenuButton => _MainMenuButton;

        /// <summary>
        /// Gets the Quit Game button.
        /// </summary>
        public Button? QuitButton => _QuitButton;

        /// <summary>
        /// Gets the modal background overlay.
        /// </summary>
        public ColorRect? Background => _Background;

        // --- GODOT LIFECYCLE ---

        /// <summary>
        /// Called when the node enters the scene tree.
        /// Sets up the pause menu modal behavior.
        /// Button population is handled by PopulateMenuButtons().
        /// </summary>
        public override void _Ready()
        {
            // Create modal background for pause menu
            _Background = new ColorRect
            {
                Name = "Background",
                Color = new Color(0, 0, 0, 0.8f),
                AnchorLeft = 0, AnchorTop = 0, AnchorRight = 1, AnchorBottom = 1,
                ZIndex = -1 // Render behind content
            };
            AddChild(_Background);

            // Set modal behavior BEFORE calling base._Ready()
            Mode = Modal ? MenuMode.Modal : MenuMode.Standard;

            // Base class will call PopulateMenuButtons() after initialization
            base._Ready();

            SetMenuTitle("PAUSED");
        }

        /// <summary>
        /// Populates the pause menu with standard pause options.
        /// Called by MenuBase after initialization completes.
        /// </summary>
        protected override void PopulateMenuButtons()
        {
            // Create menu buttons dynamically
            _ResumeButton = CreateMenuButton("ResumeButton", "Resume Game");
            _RestartButton = CreateMenuButton("RestartButton", "Restart Level");
            _MainMenuButton = CreateMenuButton("MainMenuButton", "Main Menu");
            _QuitButton = CreateMenuButton("QuitButton", "Quit Game");

            // Connect button signals
            if (_ResumeButton != null)
                _ResumeButton.Pressed += OnResumePressed;

            if (_RestartButton != null)
                _RestartButton.Pressed += OnRestartPressed;

            if (_MainMenuButton != null)
                _MainMenuButton.Pressed += OnMainMenuPressed;

            if (_QuitButton != null)
                _QuitButton.Pressed += OnQuitPressed;
        }

        // --- PUBLIC API ---

        /// <summary>
        /// Shows the pause menu and pauses the game.
        /// </summary>
    public void ShowPauseMenu()
    {
        Visible = true;
        GetViewport()?.SetInputAsHandled();
        if (GetTree() != null)
        {
            GetTree().Paused = true;
        }
        FocusFirstButton();
        OnMenuOpened();
        }

        /// <summary>
        /// Hides the pause menu and resumes the game.
        /// </summary>
    public void HidePauseMenu()
    {
        Visible = false;
        if (GetTree() != null)
        {
            GetTree().Paused = false;
        }
        OnMenuClosed();
    }

    /// <inheritdoc/>
    protected override void OpenMenu()
    {
        ShowPauseMenu();
    }

    /// <inheritdoc/>
    protected override void CloseMenu()
    {
        HidePauseMenu();
    }

    /// <inheritdoc/>
    protected override void OnBackRequested()
    {
        HidePauseMenu();
    }

    // --- EVENT HANDLERS ---

        private void OnResumePressed()
        {
            HidePauseMenu();
        }

        private void OnRestartPressed()
        {
            if (GetTree() != null)
            {
                GetTree().Paused = false;
                GetTree().ReloadCurrentScene();
            }
        }

        private void OnMainMenuPressed()
        {
            GetTree().Paused = false;
            // TODO: Transition to main menu scene
            GD.Print("Transitioning to main menu...");
        }

        private void OnQuitPressed()
        {
            GetTree().Quit();
        }
    }
}
