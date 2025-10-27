// <copyright file="PauseMenuTests.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using GdUnit4;
using Godot;
using OmegaSpiral.Source.Ui.Menus;
using OmegaSpiral.Source.Ui.Omega;
using OmegaSpiral.Tests.Shared;
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
            // Load the actual scene file used in the game to test real UI behavior
            var scene = AutoFree(ResourceLoader.Load<PackedScene>("res://source/ui/menus/pause_menu.tscn"))!;
            _PauseMenu = AutoFree(scene.Instantiate<PauseMenu>())!;
            AddChild(_PauseMenu); // Add to scene tree for proper initialization

            // Validate background/theme using shared helper
            // If this fails, all subsequent tests will cascade fail
            OmegaUiTestHelper.ValidateBackgroundTheme(_PauseMenu, "PauseMenu");
        }        [After]
        public void Cleanup()
        {
            // AutoFree() handles cleanup automatically, no manual action needed
            // GdUnit4 will free the object when the test completes
        }

        // ==================== INHERITANCE & STRUCTURE ====================

        /// <summary>
        /// PauseMenu extends BaseMenuUi.
        /// </summary>
        [TestCase]
        public void PauseMenu_ExtendsMenuUi()
        {
            AssertThat(typeof(PauseMenu).BaseType).IsEqual(typeof(BaseMenuUi));
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

        // NOTE: Visual verification should be done using visual test scenes.
        // See: docs/testing/VISUAL_UI_TESTING_GUIDE.md
        // To visually verify this menu, run: tests/fixtures/visual/pause_menu_visual.tscn
    }
}
