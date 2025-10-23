// <copyright file="BackgroundViewportFillTests.cs" company="Omega Spiral">
// Copyright (c) Omega Spiral. All rights reserved.
// </copyright>

namespace OmegaSpiral.Tests.Integration.System;

using Godot;
using GdUnit4;
using GdUnit4.Api;
using static GdUnit4.Assertions;

/// <summary>
/// System test: Verifies that the root background fills the entire viewport for any loaded scene.
/// </summary>
[TestSuite]
[RequireGodotRuntime]
public partial class BackgroundViewportFillTests : Node
{
    private const string TerminalWindowPath = "res://source/ui/terminal/terminal_window.tscn";

    [TestCase]
    public async Task Background_FillsEntireViewport()
    {
        // Arrange
        using ISceneRunner runner = ISceneRunner.Load(TerminalWindowPath);
        await runner.SimulateFrames(1).ConfigureAwait(false);

        var root = runner.Scene();
        var viewport = root.GetViewport();
        var viewportRect = viewport.GetVisibleRect();

        var background = root.GetNodeOrNull<ColorRect>("Background");
        AssertThat(background).IsNotNull();

        // Act
        var bgRect = background!.GetGlobalRect();

        // Assert - Background covers full viewport
        AssertThat(bgRect.Position.X).IsEqual(0f);
        AssertThat(bgRect.Position.Y).IsEqual(0f);
        AssertThat(bgRect.Size.X).IsEqual(viewportRect.Size.X);
        AssertThat(bgRect.Size.Y).IsEqual(viewportRect.Size.Y);
    }
}
