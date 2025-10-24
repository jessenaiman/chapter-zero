// <copyright file="StageSelectMenuLayoutTests.cs" company="Omega Spiral">
// Copyright (c) Omega Spiral. All rights reserved.
// </copyright>

namespace OmegaSpiral.Tests.UI.Menus;

using Godot;
using GdUnit4;
using GdUnit4.Api;
using static GdUnit4.Assertions;

/// <summary>
/// Phase 1: Layout & Centering Tests
/// Verifies that the Stage Select Menu is properly centered and all elements are within viewport bounds.
/// </summary>
[TestSuite]
[RequireGodotRuntime]
public partial class MainMenuLayoutTests
{
    private const string StageSelectMenuPath = "res://tests/fixtures/menu_ui_test_fixture.tscn";

    /// <summary>
    /// Test 1: Menu center should match viewport center horizontally
    /// Verifies that the menu's horizontal position is centered at the viewport's horizontal midpoint.
    /// </summary>
    [TestCase]
    [RequireGodotRuntime]
    public async Task MenuCenterShouldMatchViewportCenterHorizontally()
    {
        // Arrange
        using ISceneRunner runner = ISceneRunner.Load(StageSelectMenuPath);
        await runner.SimulateFrames(1).ConfigureAwait(false);

        Control stageSelectMenu = (Control)runner.Scene();
        AssertThat(stageSelectMenu).IsNotNull();

        Viewport viewport = runner.Scene().GetViewport();
        var viewportSize = viewport.GetVisibleRect().Size;
        var viewportCenterX = viewportSize.X / 2;

        // Act
        var menuGlobalRect = stageSelectMenu.GetGlobalRect();
        var menuCenterX = menuGlobalRect.GetCenter().X;

        // Assert
        AssertThat(Mathf.Abs(menuCenterX - viewportCenterX))
            .IsLess(2.1f);
    }

    /// <summary>
    /// Test 2: Menu center should match viewport center vertically
    /// Verifies that the menu's vertical position is centered at the viewport's vertical midpoint.
    /// </summary>
    [TestCase]
    [RequireGodotRuntime]
    public async Task MenuCenterShouldMatchViewportCenterVertically()
    {
        // Arrange
        using ISceneRunner runner = ISceneRunner.Load(StageSelectMenuPath);
        await runner.SimulateFrames(1).ConfigureAwait(false);

        Control stageSelectMenu = (Control)runner.Scene();
        AssertThat(stageSelectMenu).IsNotNull();

        Viewport viewport = runner.Scene().GetViewport();
        var viewportSize = viewport.GetVisibleRect().Size;
        var viewportCenterY = viewportSize.Y / 2;

        // Act
        var menuGlobalRect = stageSelectMenu.GetGlobalRect();
        var menuCenterY = menuGlobalRect.GetCenter().Y;

        // Assert
        AssertThat(Mathf.Abs(menuCenterY - viewportCenterY))
            .IsLess(2.1f);
    }

    /// <summary>
    /// Test 3: Title should be centered horizontally
    /// Verifies that the title label is centered within the menu container.
    /// </summary>
    [TestCase]
    [RequireGodotRuntime]
    public async Task TitleShouldBeCenteredHorizontally()
    {
        // Arrange
        using ISceneRunner runner = ISceneRunner.Load(StageSelectMenuPath);
        await runner.SimulateFrames(1).ConfigureAwait(false);

        Control stageSelectMenu = (Control)runner.Scene();
        AssertThat(stageSelectMenu).IsNotNull();

        var titleLabel = stageSelectMenu.GetNodeOrNull<Label>("ContentContainer/MenuTitle");
        AssertThat(titleLabel).IsNotNull();

        var menuContainerRect = stageSelectMenu.GetGlobalRect();
        var menuCenterX = menuContainerRect.GetCenter().X;

        // Act
        var titleRect = titleLabel.GetGlobalRect();
        var titleCenterX = titleRect.GetCenter().X;

        // Assert
        AssertThat(Mathf.Abs(titleCenterX - menuCenterX))
            .IsLess(2.1f);
    }

    /// <summary>
    /// Test 4: Stage 1 button should be within viewport bounds
    /// Verifies that the Stage 1 button is completely visible within the viewport.
    /// </summary>
    [TestCase]
    [RequireGodotRuntime]
    public async Task Stage1ButtonShouldBeWithinViewportBounds()
    {
        // Arrange
        using ISceneRunner runner = ISceneRunner.Load(StageSelectMenuPath);
        await runner.SimulateFrames(1).ConfigureAwait(false);

        Control stageSelectMenu = (Control)runner.Scene();
        AssertThat(stageSelectMenu).IsNotNull();

        var stage1Button = stageSelectMenu.GetNodeOrNull<Button>("ContentContainer/MenuButtonContainer/Button1");
        AssertThat(stage1Button).IsNotNull();

        Viewport viewport = runner.Scene().GetViewport();
        var viewportRect = viewport.GetVisibleRect();

        // Act
        var buttonGlobalRect = stage1Button.GetGlobalRect();

        // Assert
        AssertThat(viewportRect.Intersects(buttonGlobalRect))
            .IsTrue();
    }

    /// <summary>
    /// Test 5: Stage 2 button should be within viewport bounds
    /// Verifies that the Stage 2 button is completely visible within the viewport.
    /// </summary>
    [TestCase]
    [RequireGodotRuntime]
    public async Task Stage2ButtonShouldBeWithinViewportBounds()
    {
        // Arrange
        using ISceneRunner runner = ISceneRunner.Load(StageSelectMenuPath);
        await runner.SimulateFrames(1).ConfigureAwait(false);

        Control stageSelectMenu = (Control)runner.Scene();
        AssertThat(stageSelectMenu).IsNotNull();

        var stage2Button = stageSelectMenu.GetNodeOrNull<Button>("ContentContainer/MenuButtonContainer/Button2");
        AssertThat(stage2Button).IsNotNull();

        Viewport viewport = runner.Scene().GetViewport();
        var viewportRect = viewport.GetVisibleRect();

        // Act
        var buttonGlobalRect = stage2Button.GetGlobalRect();

        // Assert
        AssertThat(viewportRect.Intersects(buttonGlobalRect))
            .IsTrue();
    }

    /// <summary>
    /// Test 6: Quit button should be within viewport bounds
    /// Verifies that the Quit button is completely visible within the viewport.
    /// </summary>
    [TestCase]
    [RequireGodotRuntime]
    public async Task QuitButtonShouldBeWithinViewportBounds()
    {
        // Arrange
        using ISceneRunner runner = ISceneRunner.Load(StageSelectMenuPath);
        await runner.SimulateFrames(1).ConfigureAwait(false);

        Control stageSelectMenu = (Control)runner.Scene();
        AssertThat(stageSelectMenu).IsNotNull();

        var quitButton = stageSelectMenu.GetNodeOrNull<Button>("ContentContainer/MenuButtonContainer/QuitButton");
        AssertThat(quitButton).IsNotNull();

        Viewport viewport = runner.Scene().GetViewport();
        var viewportRect = viewport.GetVisibleRect();

        // Act
        var buttonGlobalRect = quitButton.GetGlobalRect();

        // Assert
        AssertThat(viewportRect.Intersects(buttonGlobalRect))
            .IsTrue();
    }
}
