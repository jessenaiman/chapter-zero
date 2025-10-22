// <copyright file="BackgroundComponentTests.cs" company="Omega Spiral">
// Copyright (c) Omega Spiral. All rights reserved.
// </copyright>

namespace OmegaSpiral.Tests.Ui.Components;

using Godot;
using GdUnit4;
using GdUnit4.Api;
using static GdUnit4.Assertions;

/// <summary>
/// Test suite for the reusable Background component.
/// Background should be a dark bezel panel that fills the viewport.
/// SHOULD FAIL until background.tscn is built correctly.
/// </summary>
[TestSuite]
[RequireGodotRuntime]
public partial class BackgroundComponentTests : Node
{
    private const string BackgroundPath = "res://source/ui/components/background.tscn";

    /// <summary>
        /// Background scene must exist at the expected path.
        /// Screenshot: <a href="TestResults/ui_screenshots/Background_SceneExists.png">TestResults/ui_screenshots/Background_SceneExists.png</a>
    /// </summary>
    [TestCase]
    public async Task Background_SceneExists()
    {
        using ISceneRunner runner = ISceneRunner.Load(BackgroundPath);
        await runner.SimulateFrames(1).ConfigureAwait(false);
        var root = runner.Scene();

        var backgroundPanel = root as Panel;
        AssertThat(backgroundPanel).IsNotNull();

        BackgroundComponentScreenshotHelper.TakeScreenshot(root, "Background_SceneExists");
        runner.Scene().QueueFree();
    }

    /// <summary>
        /// Background must fill entire viewport (anchors 0,0 to 1,1).
        /// Screenshot: <a href="TestResults/ui_screenshots/Background_FillsViewport_XPosition.png">TestResults/ui_screenshots/Background_FillsViewport_XPosition.png</a>
    /// </summary>
    [TestCase]
    public async Task Background_FillsViewport_XPosition()
    {
        using ISceneRunner runner = ISceneRunner.Load(BackgroundPath);
        await runner.SimulateFrames(1).ConfigureAwait(false);
        var root = runner.Scene();

        var viewport = root.GetViewport();
        var viewportRect = viewport.GetVisibleRect();

        var backgroundPanel = root as Panel;
        AssertThat(backgroundPanel).IsNotNull();

        var backgroundRect = backgroundPanel!.GetGlobalRect();

        // Background X position must be 0
        AssertThat(backgroundRect.Position.X).IsEqual(0.0f);

        BackgroundComponentScreenshotHelper.TakeScreenshot(root, "Background_FillsViewport_XPosition");
        runner.Scene().QueueFree();
    }

    /// <summary>
        /// Background must fill entire viewport (anchors 0,0 to 1,1).
        /// Screenshot: <a href="TestResults/ui_screenshots/Background_FillsViewport_YPosition.png">TestResults/ui_screenshots/Background_FillsViewport_YPosition.png</a>
    /// </summary>
    [TestCase]
    public async Task Background_FillsViewport_YPosition()
    {
        using ISceneRunner runner = ISceneRunner.Load(BackgroundPath);
        await runner.SimulateFrames(1).ConfigureAwait(false);
        var root = runner.Scene();

        var viewport = root.GetViewport();
        var viewportRect = viewport.GetVisibleRect();

        var backgroundPanel = root as Panel;
        AssertThat(backgroundPanel).IsNotNull();

        var backgroundRect = backgroundPanel!.GetGlobalRect();

        // Background Y position must be 0
        AssertThat(backgroundRect.Position.Y).IsEqual(0.0f);

        BackgroundComponentScreenshotHelper.TakeScreenshot(root, "Background_FillsViewport_YPosition");
        runner.Scene().QueueFree();
    }

    /// <summary>
        /// Background must fill entire viewport (anchors 0,0 to 1,1).
        /// Screenshot: <a href="TestResults/ui_screenshots/Background_FillsViewport_Width.png">TestResults/ui_screenshots/Background_FillsViewport_Width.png</a>
    /// </summary>
    [TestCase]
    public async Task Background_FillsViewport_Width()
    {
        using ISceneRunner runner = ISceneRunner.Load(BackgroundPath);
        await runner.SimulateFrames(1).ConfigureAwait(false);
        var root = runner.Scene();

        var viewport = root.GetViewport();
        var viewportRect = viewport.GetVisibleRect();

        var backgroundPanel = root as Panel;
        AssertThat(backgroundPanel).IsNotNull();

        var backgroundRect = backgroundPanel!.GetGlobalRect();

        // Background width must equal viewport width
        AssertThat(backgroundRect.Size.X).IsEqual(viewportRect.Size.X);

        BackgroundComponentScreenshotHelper.TakeScreenshot(root, "Background_FillsViewport_Width");
        runner.Scene().QueueFree();
    }

    /// <summary>
        /// Background must fill entire viewport (anchors 0,0 to 1,1).
        /// Screenshot: <a href="TestResults/ui_screenshots/Background_FillsViewport_Height.png">TestResults/ui_screenshots/Background_FillsViewport_Height.png</a>
    /// </summary>
    [TestCase]
    public async Task Background_FillsViewport_Height()
    {
        using ISceneRunner runner = ISceneRunner.Load(BackgroundPath);
        await runner.SimulateFrames(1).ConfigureAwait(false);
        var root = runner.Scene();

        var viewport = root.GetViewport();
        var viewportRect = viewport.GetVisibleRect();

        var backgroundPanel = root as Panel;
        AssertThat(backgroundPanel).IsNotNull();

        var backgroundRect = backgroundPanel!.GetGlobalRect();

        // Background height must equal viewport height
        AssertThat(backgroundRect.Size.Y).IsEqual(viewportRect.Size.Y);

        BackgroundComponentScreenshotHelper.TakeScreenshot(root, "Background_FillsViewport_Height");
        runner.Scene().QueueFree();
    }

    /// <summary>
        /// Background must have dark gray bezel color (0.2, 0.2, 0.2, 1).
        /// Screenshot: <a href="TestResults/ui_screenshots/Background_HasDarkGrayBezelColor_Red.png">TestResults/ui_screenshots/Background_HasDarkGrayBezelColor_Red.png</a>
    /// </summary>
    [TestCase]
    public async Task Background_HasDarkGrayBezelColor_Red()
    {
        using ISceneRunner runner = ISceneRunner.Load(BackgroundPath);
        await runner.SimulateFrames(1).ConfigureAwait(false);
        var root = runner.Scene();

        var backgroundPanel = root as Panel;
        var styleBox = backgroundPanel!.GetThemeStylebox("panel") as StyleBoxFlat;
        AssertThat(styleBox).IsNotNull();

        var bgColor = styleBox!.BgColor;
        AssertThat(bgColor.R).IsEqual(0.2f);

        BackgroundComponentScreenshotHelper.TakeScreenshot(root, "Background_HasDarkGrayBezelColor_Red");
        runner.Scene().QueueFree();
    }

    /// <summary>
        /// Background must have dark gray bezel color (0.2, 0.2, 0.2, 1).
        /// Screenshot: <a href="TestResults/ui_screenshots/Background_HasDarkGrayBezelColor_Green.png">TestResults/ui_screenshots/Background_HasDarkGrayBezelColor_Green.png</a>
    /// </summary>
    [TestCase]
    public async Task Background_HasDarkGrayBezelColor_Green()
    {
        using ISceneRunner runner = ISceneRunner.Load(BackgroundPath);
        await runner.SimulateFrames(1).ConfigureAwait(false);
        var root = runner.Scene();

        var backgroundPanel = root as Panel;
        var styleBox = backgroundPanel!.GetThemeStylebox("panel") as StyleBoxFlat;
        AssertThat(styleBox).IsNotNull();

        var bgColor = styleBox!.BgColor;
        AssertThat(bgColor.G).IsEqual(0.2f);

        BackgroundComponentScreenshotHelper.TakeScreenshot(root, "Background_HasDarkGrayBezelColor_Green");
        runner.Scene().QueueFree();
    }

    /// <summary>
        /// Background must have dark gray bezel color (0.1, 0.1, 0.1, 1).
        /// Screenshot: <a href="TestResults/ui_screenshots/Background_HasDarkGrayBezelColor_Blue.png">TestResults/ui_screenshots/Background_HasDarkGrayBezelColor_Blue.png</a>
    /// </summary>
    [TestCase]
    public async Task Background_HasDarkGrayBezelColor_Blue()
    {
        using ISceneRunner runner = ISceneRunner.Load(BackgroundPath);
        await runner.SimulateFrames(1).ConfigureAwait(false);
        var root = runner.Scene();

        var backgroundPanel = root as Panel;
        var styleBox = backgroundPanel!.GetThemeStylebox("panel") as StyleBoxFlat;
        AssertThat(styleBox).IsNotNull();

        var bgColor = styleBox!.BgColor;
        AssertThat(bgColor.B).IsEqual(0.1f);

        BackgroundComponentScreenshotHelper.TakeScreenshot(root, "Background_HasDarkGrayBezelColor_Blue");
        runner.Scene().QueueFree();
    }

    /// <summary>
        /// Background must have dark gray bezel color (0.1, 0.1, 0.1, 1).
        /// Screenshot: <a href="TestResults/ui_screenshots/Background_HasDarkGrayBezelColor_Alpha.png">TestResults/ui_screenshots/Background_HasDarkGrayBezelColor_Alpha.png</a>
    /// </summary>
    [TestCase]
    public async Task Background_HasDarkGrayBezelColor_Alpha()
    {
        using ISceneRunner runner = ISceneRunner.Load(BackgroundPath);
        await runner.SimulateFrames(1).ConfigureAwait(false);
        var root = runner.Scene();

        var backgroundPanel = root as Panel;
        var styleBox = backgroundPanel!.GetThemeStylebox("panel") as StyleBoxFlat;
        AssertThat(styleBox).IsNotNull();

        var bgColor = styleBox!.BgColor;
        AssertThat(bgColor.A).IsEqual(1.0f);

        BackgroundComponentScreenshotHelper.TakeScreenshot(root, "Background_HasDarkGrayBezelColor_Alpha");
        runner.Scene().QueueFree();
    }

    /// <summary>
        /// Background must NOT have border (border widths should be 0).
        /// Screenshot: <a href="TestResults/ui_screenshots/Background_HasNoBorder_Top.png">TestResults/ui_screenshots/Background_HasNoBorder_Top.png</a>
    /// </summary>
    [TestCase]
    public async Task Background_HasNoBorder_Top()
    {
        using ISceneRunner runner = ISceneRunner.Load(BackgroundPath);
        await runner.SimulateFrames(1).ConfigureAwait(false);
        var root = runner.Scene();

        var backgroundPanel = root as Panel;
        var styleBox = backgroundPanel!.GetThemeStylebox("panel") as StyleBoxFlat;

        AssertThat(styleBox!.BorderWidthTop).IsEqual(0);

        BackgroundComponentScreenshotHelper.TakeScreenshot(root, "Background_HasNoBorder_Top");
        runner.Scene().QueueFree();
    }

    /// <summary>
        /// Background must NOT have border (border widths should be 0).
        /// Screenshot: <a href="TestResults/ui_screenshots/Background_HasNoBorder_Bottom.png">TestResults/ui_screenshots/Background_HasNoBorder_Bottom.png</a>
    /// </summary>
    [TestCase]
    public async Task Background_HasNoBorder_Bottom()
    {
        using ISceneRunner runner = ISceneRunner.Load(BackgroundPath);
        await runner.SimulateFrames(1).ConfigureAwait(false);
        var root = runner.Scene();

        var backgroundPanel = root as Panel;
        var styleBox = backgroundPanel!.GetThemeStylebox("panel") as StyleBoxFlat;

        AssertThat(styleBox!.BorderWidthBottom).IsEqual(0);

        BackgroundComponentScreenshotHelper.TakeScreenshot(root, "Background_HasNoBorder_Bottom");
        runner.Scene().QueueFree();
    }

    /// <summary>
        /// Background must NOT have border (border widths should be 0).
        /// Screenshot: <a href="TestResults/ui_screenshots/Background_HasNoBorder_Left.png">TestResults/ui_screenshots/Background_HasNoBorder_Left.png</a>
    /// </summary>
    [TestCase]
    public async Task Background_HasNoBorder_Left()
    {
        using ISceneRunner runner = ISceneRunner.Load(BackgroundPath);
        await runner.SimulateFrames(1).ConfigureAwait(false);
        var root = runner.Scene();

        var backgroundPanel = root as Panel;
        var styleBox = backgroundPanel!.GetThemeStylebox("panel") as StyleBoxFlat;

        AssertThat(styleBox!.BorderWidthLeft).IsEqual(0);

        BackgroundComponentScreenshotHelper.TakeScreenshot(root, "Background_HasNoBorder_Left");
        runner.Scene().QueueFree();
    }

    /// <summary>
        /// Background must NOT have border (border widths should be 0).
        /// Screenshot: <a href="TestResults/ui_screenshots/Background_HasNoBorder_Right.png">TestResults/ui_screenshots/Background_HasNoBorder_Right.png</a>
    /// </summary>
    [TestCase]
    public async Task Background_HasNoBorder_Right()
    {
        using ISceneRunner runner = ISceneRunner.Load(BackgroundPath);
        await runner.SimulateFrames(1).ConfigureAwait(false);
        var root = runner.Scene();

        var backgroundPanel = root as Panel;
        var styleBox = backgroundPanel!.GetThemeStylebox("panel") as StyleBoxFlat;

        AssertThat(styleBox!.BorderWidthRight).IsEqual(0);

        BackgroundComponentScreenshotHelper.TakeScreenshot(root, "Background_HasNoBorder_Right");
        runner.Scene().QueueFree();
    }
}
