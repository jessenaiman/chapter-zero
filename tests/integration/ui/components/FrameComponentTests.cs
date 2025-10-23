// <copyright file="FrameComponentTests.cs" company="Omega Spiral">
// Copyright (c) Omega Spiral. All rights reserved.
// </copyright>

namespace OmegaSpiral.Tests.Ui.Components;

using Godot;
using GdUnit4;
using GdUnit4.Api;
using static GdUnit4.Assertions;

/// <summary>
/// Test suite for the reusable Frame component.
/// Frame should be a pure border component with no content logic.
///
/// <para>
/// <b>Visual Verification Required:</b> These tests save screenshots for each method to <c>TestResults/ui_screenshots</c>.
/// To verify correctness, inspect the PNGs visually or send them to an agent for vision analysis.
/// </para>
/// </summary>
[TestSuite]
[RequireGodotRuntime]
public partial class FrameComponentTests : UiScreenshotTestBase
{
    private const string FramePath = "res://source/ui/components/frame.tscn";

    /// <summary>
    /// Frame must exist at the expected path.
    /// </summary>
    [TestCase]
    [TakeScreenshot]
    public void Frame_SceneExists()
    {
        var frameScene = ResourceLoader.Load(FramePath);
        AssertThat(frameScene).IsNotNull();
        if (frameScene is PackedScene packedScene)
        {
            var node = packedScene.Instantiate();
            MaybeTakeScreenshot(nameof(Frame_SceneExists), node);
            node.QueueFree();
        }
    }

    /// <summary>
    /// Frame must have red border (1, 0, 0, 1).
    /// </summary>
    [TestCase]
    [TakeScreenshot]
    public async Task Frame_HasRedBorder()
    {
        using ISceneRunner runner = ISceneRunner.Load(FramePath);
        await runner.SimulateFrames(1).ConfigureAwait(false);
        var root = runner.Scene();

        var framePanel = root as Panel;
        AssertThat(framePanel).IsNotNull();

        var styleBox = framePanel!.GetThemeStylebox("panel") as StyleBoxFlat;
        AssertThat(styleBox).IsNotNull();

        var borderColor = styleBox!.BorderColor;
        AssertThat(borderColor.R).IsEqual(1.0f);
        AssertThat(borderColor.G).IsEqual(0.0f);
        AssertThat(borderColor.B).IsEqual(0.0f);
        AssertThat(borderColor.A).IsEqual(1.0f);

        MaybeTakeScreenshot(nameof(Frame_HasRedBorder), root);
        runner.Scene().QueueFree();
    }

    /// <summary>
    /// Frame must have 4px border on all sides.
    /// </summary>
    [TestCase]
    [TakeScreenshot]
    public async Task Frame_HasCorrectBorderWidth()
    {
        using ISceneRunner runner = ISceneRunner.Load(FramePath);
        await runner.SimulateFrames(1).ConfigureAwait(false);
        var root = runner.Scene();

        var framePanel = root as Panel;
        var styleBox = framePanel!.GetThemeStylebox("panel") as StyleBoxFlat;

        AssertThat(styleBox!.BorderWidthTop).IsEqual(4);
        AssertThat(styleBox.BorderWidthBottom).IsEqual(4);
        AssertThat(styleBox.BorderWidthLeft).IsEqual(4);
        AssertThat(styleBox.BorderWidthRight).IsEqual(4);

        MaybeTakeScreenshot(nameof(Frame_HasCorrectBorderWidth), root);
        runner.Scene().QueueFree();
    }

    /// <summary>
    /// Frame background must be transparent (not opaque black).
    /// </summary>
    [TestCase]
    [TakeScreenshot]
    public async Task Frame_HasTransparentBackground()
    {
        using ISceneRunner runner = ISceneRunner.Load(FramePath);
        await runner.SimulateFrames(1).ConfigureAwait(false);
        var root = runner.Scene();

        var framePanel = root as Panel;
        var styleBox = framePanel!.GetThemeStylebox("panel") as StyleBoxFlat;

        var bgColor = styleBox!.BgColor;
        AssertThat(bgColor.A).IsEqual(0.0f);

        MaybeTakeScreenshot(nameof(Frame_HasTransparentBackground), root);
        runner.Scene().QueueFree();
    }

    /// <summary>
    /// Frame must have ContentArea placeholder node for content insertion.
    /// </summary>
    [TestCase]
    [TakeScreenshot]
    public async Task Frame_HasContentAreaPlaceholder()
    {
        using ISceneRunner runner = ISceneRunner.Load(FramePath);
        await runner.SimulateFrames(1).ConfigureAwait(false);
        var root = runner.Scene();

        var contentArea = root.GetNodeOrNull<Control>("ContentArea");
        AssertThat(contentArea).IsNotNull();

        MaybeTakeScreenshot(nameof(Frame_HasContentAreaPlaceholder), root);
        runner.Scene().QueueFree();
    }
}
