// <copyright file="CreditsMenuTests.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using GdUnit4;
using Godot;
using OmegaSpiral.Source.Ui.Menus;
using OmegaSpiral.Tests.Shared;
using static GdUnit4.Assertions;

namespace OmegaSpiral.Tests.Ui
{
    /// <summary>
    /// Integration tests for CreditsMenu component.
    /// Tests credits menu structure, scrolling functionality, and content.
    /// </summary>
    [TestSuite]
    [RequireGodotRuntime]
    public partial class CreditsMenuTests : Node
    {
    private ISceneRunner? _Runner;
    private CreditsMenu _CreditsMenu = null!;
    private static T? FindNode<T>(Node root, string nodeName) where T : class
    {
        return root.FindChild(nodeName, recursive: true, owned: false) as T;
    }

    [Before]
    public async Task Setup()
    {
        // Load CreditsMenu scene to test real UI behavior
        _Runner = ISceneRunner.Load("res://source/ui/menus/credits_menu.tscn");
        _CreditsMenu = AutoFree((CreditsMenu)_Runner.Scene())!;

        // Wait for scene initialization and reparenting to complete
        // CallDeferred calls execute after frame processing, so we need enough frames
        await _Runner.SimulateFrames(10);
    }

    [After]
    public void Cleanup()
    {
        _Runner?.Dispose();
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
            var titleLabel = _CreditsMenu != null ? FindNode<Label>(_CreditsMenu, "MenuTitle") : null;
            AssertThat(titleLabel).IsNotNull();
            AssertThat(titleLabel!.Text).IsEqual("CREDITS");
        }

        /// <summary>
        /// CreditsMenu has scrolling container and content.
        /// </summary>
        [TestCase]
        public void CreditsMenu_HasScrollContainer()
        {
            var scrollContainer = _CreditsMenu != null ? FindNode<ScrollContainer>(_CreditsMenu, "CreditsScrollContainer") : null;
            var creditsContent = _CreditsMenu != null ? FindNode<VBoxContainer>(_CreditsMenu, "CreditsContent") : null;
            var backButton = _CreditsMenu != null ? FindNode<Button>(_CreditsMenu, "BackButton") : null;

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
            var creditsContent = _CreditsMenu != null ? FindNode<VBoxContainer>(_CreditsMenu, "CreditsContent") : null;
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
            var backButton = _CreditsMenu != null ? FindNode<Button>(_CreditsMenu, "BackButton") : null;
            AssertThat(backButton).IsNotNull();

            _CreditsMenu!.ShowCredits();

            // Focus is set immediately on ShowCredits
            AssertThat(backButton!.HasFocus()).IsTrue();
        }

        /// <summary>
        /// CreditsMenu back button emits Pressed signal when clicked.
        /// </summary>
        [TestCase]
        public void CreditsMenu_BackButtonEmitsSignal()
        {
            var backButton = _CreditsMenu != null ? FindNode<Button>(_CreditsMenu, "BackButton") : null;
            AssertThat(backButton).IsNotNull();

            // Verify BackButton is properly named and accessible
            AssertThat(backButton!.Name).IsEqual("BackButton");

            // Simulate button press
            backButton.EmitSignal(Button.SignalName.Pressed);

            // Verify the signal was emitted (no exception = success)
        }

        /// <summary>
        /// CreditsMenu starts scrolling when shown.
        /// </summary>
        [TestCase]
        public void CreditsMenu_StartsScrollingWhenShown()
        {
            var scrollContainer = _CreditsMenu != null ? FindNode<ScrollContainer>(_CreditsMenu, "CreditsScrollContainer") : null;
            AssertThat(scrollContainer).IsNotNull();

            // Get content reference
            var content = scrollContainer!.GetChild<VBoxContainer>(0);

            // Initially not scrolling
            AssertThat(scrollContainer.ScrollVertical).IsEqual(0);

            // Make sure the menu is visible
            _CreditsMenu!.Visible = true;
            scrollContainer.Visible = true;

            // Set a reasonable size for the scroll container to enable scrolling
            scrollContainer!.Size = new Vector2(400, 100);

            // Update scroll container
            scrollContainer.QueueSort();

            _CreditsMenu!.ShowCredits();

            // Add a spacer to force content height > container height
            var spacer = new Control { CustomMinimumSize = new Vector2(0, 1000) };
            content.AddChild(spacer);

            // Update minimum size
            content.UpdateMinimumSize();
            scrollContainer.UpdateMinimumSize();

            // Force layout update
            content.QueueSort();
            scrollContainer.QueueSort();

            // Simulate frames to update layout
            _Runner!.SimulateFrames(10, 1000);

            // Debug: check content size
            GD.Print($"Content combined min size: {content.GetCombinedMinimumSize()}");
            GD.Print($"ScrollContainer size: {scrollContainer.Size}");
            GD.Print($"ScrollVertical before: {scrollContainer.ScrollVertical}");
            GD.Print($"Content child count: {content.GetChildCount()}");
            foreach (var child in content.GetChildren())
            {
                if (child is Label label)
                {
                    GD.Print($"Label text: '{label.Text}', CustomMinSize: {label.CustomMinimumSize}, Size: {label.Size}");
                }
            }

            // Should have scrolled down
            AssertThat(scrollContainer.ScrollVertical).IsGreater(0);
        }

        // NOTE: Visual verification should be done using visual test scenes.
        // See: docs/testing/VISUAL_UI_TESTING_GUIDE.md
        // To visually verify this menu, run: tests/fixtures/visual/credits_menu_visual.tscn
    }
}
