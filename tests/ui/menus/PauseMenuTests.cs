// <copyright file="PauseMenuTests.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using GdUnit4;
using Godot;
using OmegaSpiral.Source.Ui.Menus;
using OmegaSpiral.Source.Ui.Omega;
using OmegaSpiral.Tests.Shared;
using static GdUnit4.Assertions;

namespace OmegaSpiral.Tests.Ui;

/// <summary>
/// Integration tests for PauseMenu component.
    /// Tests pause menu structure, functionality, and visual layout.
    /// </summary>
    [TestSuite]
    [RequireGodotRuntime]
    public partial class PauseMenuTests : Node
    {
        private ISceneRunner? _Runner;
        private PauseMenu? _PauseMenu;

        [Before]
        public async Task Setup()
        {
            // Load PauseMenu scene to test real UI behavior
            _Runner = ISceneRunner.Load("res://source/ui/menus/pause_menu.tscn");
            var scene = _Runner.Scene();

            if (scene == null)
            {
                throw new InvalidOperationException("Failed to load PauseMenu scene.");
            }

            if (scene is PauseMenu pauseMenu)
            {
                _PauseMenu = AutoFree(pauseMenu)!;
            }
            else
            {
                throw new InvalidOperationException("Loaded scene is not of type PauseMenu.");
            }

            // Wait for scene initialization
            await _Runner.SimulateFrames(10);
        }

    [After]
    public void Cleanup()
    {
        if (_Runner != null)
        {
            GD.Print("Disposing Runner...");
            _Runner.Dispose();
        }

        if (_PauseMenu != null)
        {
            GD.Print("Freeing PauseMenu...");
            _PauseMenu.QueueFree();
        }
    }        // ==================== INHERITANCE & STRUCTURE ====================

        /// <summary>
        /// PauseMenu extends MenuUi.
        /// </summary>
        [TestCase]
        public void PauseMenu_ExtendsMenuUi()
        {
            AssertThat(typeof(PauseMenu).BaseType).IsEqual(typeof(MenuUi));
            AssertThat(typeof(PauseMenu).IsAssignableTo(typeof(Control))).IsTrue();
        }

        /// <summary>
        /// PauseMenu has proper title.
        /// </summary>
        [TestCase]
        public void PauseMenu_HasTitle()
        {
            AssertThat(_PauseMenu!.MenuTitle).IsNotNull();
            AssertThat(_PauseMenu!.MenuTitle!.Text).IsEqual("PAUSED");
        }

        /// <summary>
        /// PauseMenu has all expected buttons with correct text.
        /// </summary>
        [TestCase]
        public void PauseMenu_HasButtons()
        {
            AssertThat(_PauseMenu!.ResumeButton).IsNotNull();
            AssertThat(_PauseMenu!.RestartButton).IsNotNull();
            AssertThat(_PauseMenu!.MainMenuButton).IsNotNull();
            AssertThat(_PauseMenu!.QuitButton).IsNotNull();

            AssertThat(_PauseMenu!.ResumeButton!.Text).IsEqual("Resume Game");
            AssertThat(_PauseMenu!.RestartButton!.Text).IsEqual("Restart");
            AssertThat(_PauseMenu!.MainMenuButton!.Text).IsEqual("Main Menu");
            AssertThat(_PauseMenu!.QuitButton!.Text).IsEqual("Quit Game");
        }

        /// <summary>
        /// PauseMenu has semi-transparent background.
        /// </summary>
        [TestCase]
        public void PauseMenu_HasBackground()
        {
            AssertThat(_PauseMenu!.Background).IsNotNull();
            AssertThat(_PauseMenu!.Background!.Color).IsEqual(new Color(0, 0, 0, 0.8f));
        }

        // ==================== FUNCTIONALITY ====================

        /// <summary>
        /// PauseMenu can show and hide properly.
        /// </summary>
        [TestCase]
        public void PauseMenu_CanShowAndHide()
        {
            // Initially visible
            AssertThat(_PauseMenu!.Visible).IsTrue();

            _PauseMenu.HidePauseMenu();
            AssertThat(_PauseMenu.Visible).IsFalse();

            _PauseMenu.ShowPauseMenu();
            AssertThat(_PauseMenu.Visible).IsTrue();
        }

        /// <summary>
        /// PauseMenu focuses first button when shown.
        /// </summary>
        [TestCase]
        public void PauseMenu_FocusesFirstButton()
        {
            AssertThat(_PauseMenu!.ResumeButton).IsNotNull();

            _PauseMenu!.ShowPauseMenu();

            // Focus is set immediately on ShowPauseMenu
            AssertThat(_PauseMenu!.ResumeButton!.HasFocus()).IsTrue();
        }
        // ==================== VISUAL COMPONENT TESTS ====================

        // NOTE: Visual component tests (ShaderController, BorderFrame, etc.) moved to OmegaContainer_Tests
        // Those are Omega framework concerns, not PauseMenu-specific concerns
        // To visually verify this menu, run the pause_menu.tscn scene in the editor
    }
