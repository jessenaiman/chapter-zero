// <copyright file="MainMenuRenderingTests.cs" company="Omega Spiral">
// Copyright (c) Omega Spiral. All rights reserved.
// </copyright>

namespace OmegaSpiral.Tests.UI.Menus;

using Godot;
using GdUnit4;
using GdUnit4.Api;
using static GdUnit4.Assertions;

/// <summary>
/// Comprehensive rendering and layout validation tests for MainMenu.
/// These tests verify that all UI elements render correctly, are visible,
/// properly centered, and maintain correct spacing according to design specifications.
/// </summary>
[TestSuite]
[RequireGodotRuntime]
public partial class MainMenuRenderingTests
{
    private const string MainMenuPath = "res://source/stages/stage_0_start/main_menu.tscn";
    private const float ViewportMargin = 64.0f;
    private const float Tolerance = 2.1f; // Allow 2px tolerance for floating point calculations

    // ==================== STRUCTURAL TESTS ====================

    /// <summary>
    /// Test 1: Menu root (MainMenu control) must fill entire viewport.
    /// This ensures the menu can position content relative to full screen.
    /// </summary>
    [TestCase]
    public async Task MenuRootMustFillEntireViewport()
    {
        using ISceneRunner runner = ISceneRunner.Load(MainMenuPath);
        await runner.SimulateFrames(1).ConfigureAwait(false);

        Control mainMenu = (Control)runner.Scene();
        AssertThat(mainMenu).IsNotNull();

        var viewport = mainMenu.GetViewport();
        var viewportRect = viewport.GetVisibleRect();
        var menuRect = mainMenu.GetRect();

        // Menu should have same size as viewport
        AssertThat(menuRect.Size.X).IsEqual(viewportRect.Size.X);
        AssertThat(menuRect.Size.Y).IsEqual(viewportRect.Size.Y);
    }

    /// <summary>
    /// Test 2: MenuContainer must be a MarginContainer (structural requirement).
    /// </summary>
    [TestCase]
    public async Task MenuContainerMustBeMarginContainer()
    {
        using ISceneRunner runner = ISceneRunner.Load(MainMenuPath);
        await runner.SimulateFrames(1).ConfigureAwait(false);

        Control mainMenu = (Control)runner.Scene();
        var menuContainer = mainMenu.GetNodeOrNull<MarginContainer>("ContentContainer");

        AssertThat(menuContainer).IsNotNull();
        AssertThat(menuContainer).IsInstanceOf<MarginContainer>();
    }

    /// <summary>
    /// Test 3: MenuContainer must have correct margin values (64px on all sides).
    /// </summary>
    [TestCase]
    public async Task MenuContainerMustHaveCorrectMargins()
    {
        using ISceneRunner runner = ISceneRunner.Load(MainMenuPath);
        await runner.SimulateFrames(1).ConfigureAwait(false);

        Control mainMenu = (Control)runner.Scene();
        var menuContainer = mainMenu.GetNodeOrNull<MarginContainer>("ContentContainer");

        AssertThat(menuContainer).IsNotNull();
        AssertThat(menuContainer.GetThemeConstant("margin_left")).IsEqual(64);
        AssertThat(menuContainer.GetThemeConstant("margin_top")).IsEqual(64);
        AssertThat(menuContainer.GetThemeConstant("margin_right")).IsEqual(64);
        AssertThat(menuContainer.GetThemeConstant("margin_bottom")).IsEqual(64);
    }

    /// <summary>
    /// Test 4: MenuWrapper must exist and be a Control.
    /// </summary>
    [TestCase]
    public async Task MenuWrapperMustExist()
    {
        using ISceneRunner runner = ISceneRunner.Load(MainMenuPath);
        await runner.SimulateFrames(1).ConfigureAwait(false);

        Control mainMenu = (Control)runner.Scene();
        var menuWrapper = mainMenu.GetNodeOrNull<Control>("MenuContainer/MenuWrapper");

        AssertThat(menuWrapper).IsNotNull();
        AssertThat(menuWrapper).IsInstanceOf<Control>();
    }

    /// <summary>
    /// Test 5: MenuWrapper must fill its parent (MarginContainer).
    /// </summary>
    [TestCase]
    public async Task MenuWrapperMustFillParent()
    {
        using ISceneRunner runner = ISceneRunner.Load(MainMenuPath);
        await runner.SimulateFrames(1).ConfigureAwait(false);

        Control mainMenu = (Control)runner.Scene();
        var menuContainer = mainMenu.GetNodeOrNull<MarginContainer>("MenuContainer");
        var menuWrapper = mainMenu.GetNodeOrNull<Control>("MenuContainer/MenuWrapper");

        AssertThat(menuWrapper).IsNotNull();
        AssertThat(menuContainer).IsNotNull();

        var containerRect = menuContainer.GetRect();
        var wrapperRect = menuWrapper.GetRect();

        // MenuWrapper should have same size as available space in MarginContainer
        AssertThat(wrapperRect.Size.X).IsGreater(0);
        AssertThat(wrapperRect.Size.Y).IsGreater(0);
    }

    /// <summary>
    /// Test 6: MenuContent must NOT have conflicting offsets (should be 0).
    /// Offsets + anchors = double-spacing bug.
    /// </summary>
    [TestCase]
    public async Task MenuContentMustNotHaveConflictingOffsets()
    {
        using ISceneRunner runner = ISceneRunner.Load(MainMenuPath);
        await runner.SimulateFrames(1).ConfigureAwait(false);

        Control mainMenu = (Control)runner.Scene();
        var menuContent = mainMenu.GetNodeOrNull<VBoxContainer>("MenuContainer/MenuWrapper/MenuContent");

        AssertThat(menuContent).IsNotNull();

        // All offsets should be 0 or very close to 0
        AssertThat(menuContent.OffsetLeft).IsEqual(0.0f);
        AssertThat(menuContent.OffsetTop).IsEqual(0.0f);
        AssertThat(menuContent.OffsetRight).IsEqual(0.0f);
        AssertThat(menuContent.OffsetBottom).IsEqual(0.0f);
    }

    /// <summary>
    /// Test 7: MenuContent must use full-screen anchors (0→1) to fill parent.
    /// </summary>
    [TestCase]
    public async Task MenuContentMustUseFullScreenAnchors()
    {
        using ISceneRunner runner = ISceneRunner.Load(MainMenuPath);
        await runner.SimulateFrames(1).ConfigureAwait(false);

        Control mainMenu = (Control)runner.Scene();
        var menuContent = mainMenu.GetNodeOrNull<VBoxContainer>("MenuContainer/MenuWrapper/MenuContent");

        AssertThat(menuContent).IsNotNull();
        AssertThat(menuContent.AnchorLeft).IsEqual(0.0f);
        AssertThat(menuContent.AnchorTop).IsEqual(0.0f);
        AssertThat(menuContent.AnchorRight).IsEqual(1.0f);
        AssertThat(menuContent.AnchorBottom).IsEqual(1.0f);
    }

    // ==================== SPACING TESTS ====================

    /// <summary>
    /// Test 8: Top margin must be visible (64px minimum from viewport top).
    /// </summary>
    [TestCase]
    public async Task TopMarginMustBeVisible()
    {
        using ISceneRunner runner = ISceneRunner.Load(MainMenuPath);
        await runner.SimulateFrames(1).ConfigureAwait(false);

        Control mainMenu = (Control)runner.Scene();
        var menuContainer = mainMenu.GetNodeOrNull<MarginContainer>("MenuContainer");
        var menuFrame = mainMenu.GetNodeOrNull<Panel>("MenuContainer/MenuWrapper/MenuFrame");

        AssertThat(menuContainer).IsNotNull();
        AssertThat(menuFrame).IsNotNull();

        var containerGlobalRect = menuContainer.GetGlobalRect();
        var frameGlobalRect = menuFrame.GetGlobalRect();

        // MenuFrame top should be at least 64px below viewport top
        var topSpacing = frameGlobalRect.Position.Y - containerGlobalRect.Position.Y;
        AssertThat(topSpacing).IsGreater(ViewportMargin - Tolerance);
    }

    /// <summary>
    /// Test 9: Bottom margin must be visible (64px minimum from viewport bottom).
    /// </summary>
    [TestCase]
    public async Task BottomMarginMustBeVisible()
    {
        using ISceneRunner runner = ISceneRunner.Load(MainMenuPath);
        await runner.SimulateFrames(1).ConfigureAwait(false);

        Control mainMenu = (Control)runner.Scene();
        var menuContainer = mainMenu.GetNodeOrNull<MarginContainer>("MenuContainer");
        var menuFrame = mainMenu.GetNodeOrNull<Panel>("MenuContainer/MenuWrapper/MenuFrame");

        AssertThat(menuContainer).IsNotNull();
        AssertThat(menuFrame).IsNotNull();

        var containerGlobalRect = menuContainer.GetGlobalRect();
        var frameGlobalRect = menuFrame.GetGlobalRect();

        var containerBottom = containerGlobalRect.Position.Y + containerGlobalRect.Size.Y;
        var frameBottom = frameGlobalRect.Position.Y + frameGlobalRect.Size.Y;
        var bottomSpacing = containerBottom - frameBottom;

        AssertThat(bottomSpacing).IsGreater(ViewportMargin - Tolerance);
    }

    /// <summary>
    /// Test 10: Left margin must be visible (64px minimum from viewport left).
    /// </summary>
    [TestCase]
    public async Task LeftMarginMustBeVisible()
    {
        using ISceneRunner runner = ISceneRunner.Load(MainMenuPath);
        await runner.SimulateFrames(1).ConfigureAwait(false);

        Control mainMenu = (Control)runner.Scene();
        var menuContainer = mainMenu.GetNodeOrNull<MarginContainer>("MenuContainer");
        var menuFrame = mainMenu.GetNodeOrNull<Panel>("MenuContainer/MenuWrapper/MenuFrame");

        AssertThat(menuContainer).IsNotNull();
        AssertThat(menuFrame).IsNotNull();

        var containerGlobalRect = menuContainer.GetGlobalRect();
        var frameGlobalRect = menuFrame.GetGlobalRect();

        var leftSpacing = frameGlobalRect.Position.X - containerGlobalRect.Position.X;
        AssertThat(leftSpacing).IsGreater(ViewportMargin - Tolerance);
    }

    /// <summary>
    /// Test 11: Right margin must be visible (64px minimum from viewport right).
    /// </summary>
    [TestCase]
    public async Task RightMarginMustBeVisible()
    {
        using ISceneRunner runner = ISceneRunner.Load(MainMenuPath);
        await runner.SimulateFrames(1).ConfigureAwait(false);

        Control mainMenu = (Control)runner.Scene();
        var menuContainer = mainMenu.GetNodeOrNull<MarginContainer>("MenuContainer");
        var menuFrame = mainMenu.GetNodeOrNull<Panel>("MenuContainer/MenuWrapper/MenuFrame");

        AssertThat(menuContainer).IsNotNull();
        AssertThat(menuFrame).IsNotNull();

        var containerGlobalRect = menuContainer.GetGlobalRect();
        var frameGlobalRect = menuFrame.GetGlobalRect();

        var containerRight = containerGlobalRect.Position.X + containerGlobalRect.Size.X;
        var frameRight = frameGlobalRect.Position.X + frameGlobalRect.Size.X;
        var rightSpacing = containerRight - frameRight;

        AssertThat(rightSpacing).IsGreater(ViewportMargin - Tolerance);
    }

    // ==================== CONTENT VISIBILITY TESTS ====================

    /// <summary>
    /// Test 12: Title label must be visible (non-zero size, within viewport).
    /// </summary>
    [TestCase]
    public async Task TitleMustBeVisible()
    {
        using ISceneRunner runner = ISceneRunner.Load(MainMenuPath);
        await runner.SimulateFrames(1).ConfigureAwait(false);

        Control mainMenu = (Control)runner.Scene();
        var titleLabel = mainMenu.GetNodeOrNull<Label>("MenuContainer/MenuWrapper/MenuContent/TitleMargin/TitleLabel");

        AssertThat(titleLabel).IsNotNull();
        AssertThat(titleLabel.Visible).IsTrue();
        AssertThat(titleLabel.GetRect().Size.X).IsGreater(0);
        AssertThat(titleLabel.GetRect().Size.Y).IsGreater(0);
        AssertThat(titleLabel.Text).IsEqual("Ωmega Spiral");
    }

    /// <summary>
    /// Test 13: Description label must be visible and contain expected text.
    /// </summary>
    [TestCase]
    public async Task DescriptionMustBeVisible()
    {
        using ISceneRunner runner = ISceneRunner.Load(MainMenuPath);
        await runner.SimulateFrames(1).ConfigureAwait(false);

        Control mainMenu = (Control)runner.Scene();
        var descriptionLabel = mainMenu.GetNodeOrNull<Label>("MenuContainer/MenuWrapper/MenuContent/DescriptionMargin/DescriptionLabel");

        AssertThat(descriptionLabel).IsNotNull();
        AssertThat(descriptionLabel.Visible).IsTrue();
        AssertThat(descriptionLabel.GetRect().Size.X).IsGreater(0);
        AssertThat(descriptionLabel.GetRect().Size.Y).IsGreater(0);
        AssertThat(descriptionLabel.Text).Contains("Omega Terminal");
    }

    /// <summary>
    /// Test 14: Start button must be visible and clickable.
    /// </summary>
    [TestCase]
    public async Task StartButtonMustBeVisible()
    {
        using ISceneRunner runner = ISceneRunner.Load(MainMenuPath);
        await runner.SimulateFrames(1).ConfigureAwait(false);

        Control mainMenu = (Control)runner.Scene();
        var startButton = mainMenu.GetNodeOrNull<Button>("MenuContainer/MenuWrapper/MenuContent/MenuButtonsMargin/MenuButtonsContainer/StartButton");

        AssertThat(startButton).IsNotNull();
        AssertThat(startButton.Visible).IsTrue();
        AssertThat(startButton.GetRect().Size.X).IsGreater(0);
        AssertThat(startButton.GetRect().Size.Y).IsGreater(56); // custom_minimum_size = 56
        AssertThat(startButton.Disabled).IsFalse();
    }

    /// <summary>
    /// Test 15: Stage header label must be visible.
    /// </summary>
    [TestCase]
    public async Task StageHeaderMustBeVisible()
    {
        using ISceneRunner runner = ISceneRunner.Load(MainMenuPath);
        await runner.SimulateFrames(1).ConfigureAwait(false);

        Control mainMenu = (Control)runner.Scene();
        var stageHeader = mainMenu.GetNodeOrNull<Label>("MenuContainer/MenuWrapper/MenuContent/MenuButtonsMargin/MenuButtonsContainer/StageHeader");

        AssertThat(stageHeader).IsNotNull();
        AssertThat(stageHeader.Visible).IsTrue();
        AssertThat(stageHeader.GetRect().Size.X).IsGreater(0);
        AssertThat(stageHeader.Text).IsEqual("Stage Access");
    }

    /// <summary>
    /// Test 16: Footer label must be visible.
    /// </summary>
    [TestCase]
    public async Task FooterMustBeVisible()
    {
        using ISceneRunner runner = ISceneRunner.Load(MainMenuPath);
        await runner.SimulateFrames(1).ConfigureAwait(false);

        Control mainMenu = (Control)runner.Scene();
        var footerLabel = mainMenu.GetNodeOrNull<Label>("MenuContainer/MenuWrapper/MenuContent/FooterMargin/FooterLabel");

        AssertThat(footerLabel).IsNotNull();
        AssertThat(footerLabel.Visible).IsTrue();
        AssertThat(footerLabel.GetRect().Size.X).IsGreater(0);
        AssertThat(footerLabel.Text).Contains("Dreamweaver");
    }

    /// <summary>
    /// Test 17: Divider must be visible and have correct dimensions.
    /// </summary>
    [TestCase]
    public async Task DividerMustBeVisible()
    {
        using ISceneRunner runner = ISceneRunner.Load(MainMenuPath);
        await runner.SimulateFrames(1).ConfigureAwait(false);

        Control mainMenu = (Control)runner.Scene();
        var divider = mainMenu.GetNodeOrNull<ColorRect>("MenuContainer/MenuWrapper/MenuContent/Divider");

        AssertThat(divider).IsNotNull();
        AssertThat(divider.Visible).IsTrue();
        AssertThat(divider.GetRect().Size.X).IsGreater(0);
        AssertThat(divider.GetRect().Size.Y).IsGreater(2); // custom_minimum_size = 3
    }

    /// <summary>
    /// Test 18: Options button must be visible and clickable.
    /// </summary>
    [TestCase]
    public async Task OptionsButtonMustBeVisible()
    {
        using ISceneRunner runner = ISceneRunner.Load(MainMenuPath);
        await runner.SimulateFrames(1).ConfigureAwait(false);

        Control mainMenu = (Control)runner.Scene();
        var optionsButton = mainMenu.GetNodeOrNull<Button>("MenuContainer/MenuWrapper/MenuContent/MenuButtonsMargin/MenuButtonsContainer/OptionsButton");

        AssertThat(optionsButton).IsNotNull();
        AssertThat(optionsButton.Visible).IsTrue();
        AssertThat(optionsButton.GetRect().Size.X).IsGreater(0);
        AssertThat(optionsButton.GetRect().Size.Y).IsGreater(47);
        AssertThat(optionsButton.Disabled).IsFalse();
    }

    /// <summary>
    /// Test 19: Quit button must be visible and clickable.
    /// </summary>
    [TestCase]
    public async Task QuitButtonMustBeVisible()
    {
        using ISceneRunner runner = ISceneRunner.Load(MainMenuPath);
        await runner.SimulateFrames(1).ConfigureAwait(false);

        Control mainMenu = (Control)runner.Scene();
        var quitButton = mainMenu.GetNodeOrNull<Button>("MenuContainer/MenuWrapper/MenuContent/MenuButtonsMargin/MenuButtonsContainer/QuitButton");

        AssertThat(quitButton).IsNotNull();
        AssertThat(quitButton.Visible).IsTrue();
        AssertThat(quitButton.GetRect().Size.X).IsGreater(0);
        AssertThat(quitButton.GetRect().Size.Y).IsGreater(47);
        AssertThat(quitButton.Disabled).IsFalse();
    }

    // ==================== VIEWPORT BOUNDS TESTS ====================

    /// <summary>
    /// Test 20: Menu frame must be completely within viewport bounds (no clipping).
    /// </summary>
    [TestCase]
    public async Task MenuFrameMustBeCompletelyWithinViewport()
    {
        using ISceneRunner runner = ISceneRunner.Load(MainMenuPath);
        await runner.SimulateFrames(1).ConfigureAwait(false);

        Control mainMenu = (Control)runner.Scene();
        var menuFrame = mainMenu.GetNodeOrNull<Panel>("MenuContainer/MenuWrapper/MenuFrame");

        AssertThat(menuFrame).IsNotNull();

        var viewport = mainMenu.GetViewport();
        var viewportRect = viewport.GetVisibleRect();
        var frameGlobalRect = menuFrame.GetGlobalRect();

        // Check frame is fully inside viewport (no clipping)
        AssertThat(frameGlobalRect.Position.X).IsGreater(-1);
        AssertThat(frameGlobalRect.Position.Y).IsGreater(-1);
        AssertThat(frameGlobalRect.Position.X + frameGlobalRect.Size.X).IsLess(viewportRect.Size.X + 1);
        AssertThat(frameGlobalRect.Position.Y + frameGlobalRect.Size.Y).IsLess(viewportRect.Size.Y + 1);
    }

    /// <summary>
    /// Test 21: All visible buttons must be within viewport bounds.
    /// </summary>
    [TestCase]
    public async Task AllButtonsMustBeWithinViewport()
    {
        using ISceneRunner runner = ISceneRunner.Load(MainMenuPath);
        await runner.SimulateFrames(1).ConfigureAwait(false);

        Control mainMenu = (Control)runner.Scene();
        var viewport = mainMenu.GetViewport();
        var viewportRect = viewport.GetVisibleRect();

        var buttons = new[] { "StartButton", "OptionsButton", "QuitButton" };
        foreach (var buttonName in buttons)
        {
            var button = mainMenu.GetNodeOrNull<Button>($"MenuContainer/MenuWrapper/MenuContent/MenuButtonsMargin/MenuButtonsContainer/{buttonName}");
            AssertThat(button).IsNotNull();

            var buttonGlobalRect = button.GetGlobalRect();
            AssertThat(viewportRect.Intersects(buttonGlobalRect)).IsTrue();
        }
    }
}
