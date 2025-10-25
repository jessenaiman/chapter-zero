// <copyright file="CreditsMenuTests.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using GdUnit4;
using Godot;
using OmegaSpiral.Source.Ui.Menus;
using static GdUnit4.Assertions;

namespace OmegaSpiral.Tests.Integration.Ui
{
    /// <summary>
    /// Integration tests for CreditsMenu component.
    /// Tests credits menu structure, scrolling functionality, and content.
    /// </summary>
    [TestSuite]
    [RequireGodotRuntime]
    public partial class CreditsMenuTests : Node
    {
        private CreditsMenu _CreditsMenu = null!;

        [Before]
        public void Setup()
        {
            _CreditsMenu = AutoFree(new CreditsMenu())!;
            AddChild(_CreditsMenu);
            _CreditsMenu._Ready();
            AssertThat(_CreditsMenu).IsNotNull();
        }

        // ==================== INHERITANCE & STRUCTURE ====================

        /// <summary>
        /// CreditsMenu extends MenuUi.
        /// </summary>
        [TestCase]
        public void CreditsMenu_ExtendsMenuUi()
        {
            AssertThat(typeof(CreditsMenu).BaseType).IsEqual(typeof(MenuUi));
            AssertThat(typeof(CreditsMenu).IsAssignableTo(typeof(Control))).IsTrue();
        }

        /// <summary>
        /// CreditsMenu has proper title.
        /// </summary>
        [TestCase]
        public void CreditsMenu_HasTitle()
        {
            var titleLabel = _CreditsMenu?.GetNodeOrNull<Label>("ContentContainer/MenuTitle");
            AssertThat(titleLabel).IsNotNull();
            AssertThat(titleLabel!.Text).IsEqual("CREDITS");
        }

        /// <summary>
        /// CreditsMenu has scrolling container and content.
        /// </summary>
        [TestCase]
        public void CreditsMenu_HasScrollContainer()
        {
            var scrollContainer = _CreditsMenu?.GetNodeOrNull<ScrollContainer>("ContentContainer/CreditsScrollContainer");
            var creditsContent = _CreditsMenu?.GetNodeOrNull<VBoxContainer>("ContentContainer/CreditsScrollContainer/CreditsContent");
            var backButton = _CreditsMenu?.GetNodeOrNull<Button>("ContentContainer/MenuButtonContainer/BackButton");

            AssertThat(scrollContainer).IsNotNull();
            AssertThat(creditsContent).IsNotNull();
            AssertThat(backButton).IsNotNull();
        }

        /// <summary>
        /// CreditsMenu populates credits content on ready.
        /// </summary>
        [TestCase]
        public void CreditsMenu_PopulatesCredits()
        {
            var creditsContent = _CreditsMenu?.GetNodeOrNull<VBoxContainer>("ContentContainer/CreditsScrollContainer/CreditsContent");
            AssertThat(creditsContent).IsNotNull();

            // Should have multiple credit entries
            AssertThat(creditsContent!.GetChildCount()).IsGreater(5);

            // Check for main title
            var firstLabel = creditsContent.GetChild(0) as Label;
            AssertThat(firstLabel).IsNotNull();
            AssertThat(firstLabel!.Text).Contains("ΩMEGA SPIRAL");
        }

        // ==================== FUNCTIONALITY ====================

        /// <summary>
        /// CreditsMenu can show and hide properly.
        /// </summary>
        [TestCase]
        public void CreditsMenu_CanShowAndHide()
        {
            // Initially visible
            AssertThat(_CreditsMenu!.Visible).IsTrue();

            _CreditsMenu.HideCredits();
            AssertThat(_CreditsMenu.Visible).IsFalse();

            _CreditsMenu.ShowCredits();
            AssertThat(_CreditsMenu.Visible).IsTrue();
        }

        /// <summary>
        /// CreditsMenu focuses back button when shown.
        /// </summary>
        [TestCase]
        public void CreditsMenu_FocusesBackButton()
        {
            var backButton = _CreditsMenu?.GetNodeOrNull<Button>("ContentContainer/MenuButtonContainer/BackButton");
            AssertThat(backButton).IsNotNull();

            _CreditsMenu!.ShowCredits();

            // Focus is set immediately on ShowCredits
            AssertThat(backButton!.HasFocus()).IsTrue();
        }

        /// <summary>
        /// CreditsMenu starts scrolling when shown.
        /// </summary>
        [TestCase]
        public void CreditsMenu_StartsScrollingWhenShown()
        {
            var scrollContainer = _CreditsMenu?.GetNodeOrNull<ScrollContainer>("ContentContainer/CreditsScrollContainer");
            AssertThat(scrollContainer).IsNotNull();

            // Initially not scrolling
            AssertThat(scrollContainer!.ScrollVertical).IsEqual(0);

            _CreditsMenu!.ShowCredits();

            // Simulate scroll with _Process calls
            for (int i = 0; i < 10; i++)
            {
                _CreditsMenu._Process(0.016); // ~60 FPS frame time
            }

            // Should have scrolled down
            AssertThat(scrollContainer.ScrollVertical).IsGreater(0);
        }

        // NOTE: Visual verification should be done using visual test scenes.
        // See: docs/testing/VISUAL_UI_TESTING_GUIDE.md
        // To visually verify this menu, run: tests/fixtures/visual/credits_menu_visual.tscn
    }
}
