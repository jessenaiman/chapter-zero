// <copyright file="PauseMenuTests.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using GdUnit4;
using Godot;
using OmegaSpiral.Source.Ui.Menus;
using static GdUnit4.Assertions;

namespace OmegaSpiral.Tests.Integration.Ui
{
    /// <summary>
    /// Integration tests for PauseMenu component.
    /// Tests pause menu structure, functionality, and visual layout.
    /// </summary>
    [TestSuite]
    [RequireGodotRuntime]
    public partial class PauseMenuTests : Node
    {
        private PauseMenu _PauseMenu = null!;

        [Before]
        public void Setup()
        {
            _PauseMenu = AutoFree(new PauseMenu())!;
            AddChild(_PauseMenu);
            _PauseMenu._Ready();
            AssertThat(_PauseMenu).IsNotNull();
        }

        // ==================== INHERITANCE & STRUCTURE ====================

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
            var titleLabel = _PauseMenu?.GetNodeOrNull<Label>("ContentContainer/MenuTitle");
            AssertThat(titleLabel).IsNotNull();
            AssertThat(titleLabel!.Text).IsEqual("PAUSED");
        }

        /// <summary>
        /// PauseMenu has all required buttons.
        /// </summary>
        [TestCase]
        public void PauseMenu_HasAllButtons()
        {
            var resumeButton = _PauseMenu?.GetNodeOrNull<Button>("ContentContainer/MenuButtonContainer/ResumeButton");
            var restartButton = _PauseMenu?.GetNodeOrNull<Button>("ContentContainer/MenuButtonContainer/RestartButton");
            var mainMenuButton = _PauseMenu?.GetNodeOrNull<Button>("ContentContainer/MenuButtonContainer/MainMenuButton");
            var quitButton = _PauseMenu?.GetNodeOrNull<Button>("ContentContainer/MenuButtonContainer/QuitButton");

            AssertThat(resumeButton).IsNotNull();
            AssertThat(restartButton).IsNotNull();
            AssertThat(mainMenuButton).IsNotNull();
            AssertThat(quitButton).IsNotNull();

            AssertThat(resumeButton!.Text).IsEqual("Resume Game");
            AssertThat(restartButton!.Text).IsEqual("Restart Level");
            AssertThat(mainMenuButton!.Text).IsEqual("Main Menu");
            AssertThat(quitButton!.Text).IsEqual("Quit Game");
        }

        /// <summary>
        /// PauseMenu has semi-transparent background.
        /// </summary>
        [TestCase]
        public void PauseMenu_HasBackground()
        {
            var background = _PauseMenu?.GetNodeOrNull<ColorRect>("Background");
            AssertThat(background).IsNotNull();
            AssertThat(background!.Color).IsEqual(new Color(0, 0, 0, 0.8f));
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
            var resumeButton = _PauseMenu?.GetNodeOrNull<Button>("ContentContainer/MenuButtonContainer/ResumeButton");
            AssertThat(resumeButton).IsNotNull();

            _PauseMenu!.ShowPauseMenu();

            // Focus is set immediately on ShowPauseMenu
            AssertThat(resumeButton!.HasFocus()).IsTrue();
        }

        // NOTE: Visual verification should be done using visual test scenes.
        // See: docs/testing/VISUAL_UI_TESTING_GUIDE.md
        // To visually verify this menu, run: tests/fixtures/visual/pause_menu_visual.tscn
    }
}
