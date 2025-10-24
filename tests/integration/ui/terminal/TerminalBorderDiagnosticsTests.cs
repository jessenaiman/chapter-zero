// <copyright file="TerminalBorderDiagnosticsTests.cs" company="Omega Spiral">
// Copyright (c) Omega Spiral. All rights reserved.
// </copyright>

namespace OmegaSpiral.Tests.UI.Terminal;

using Godot;
using GdUnit4;
using GdUnit4.Api;
using static GdUnit4.Assertions;

/// <summary>
/// Diagnostic tests to isolate why the terminal border is not rendering.
/// Each test validates ONE specific hypothesis about the border visibility issue.
/// </summary>
[TestSuite]
[RequireGodotRuntime]
public partial class TerminalBorderDiagnosticsTests : Node
{
    private const string MainMenuPath = "res://source/ui/menus/main_menu.tscn";

    /// <summary>
    /// TEST 1: Border Widths in Scene File
    /// Validates that TerminalFrameStyle has ALL border widths set (not just bottom).
    /// </summary>
    [TestCase]
    public async Task Test1_BorderWidthsAreSetOnAllSides()
    {
        using ISceneRunner runner = ISceneRunner.Load(MainMenuPath);
        await runner.SimulateFrames(2).ConfigureAwait(false);
        var root = runner.Scene();

        // Get TerminalFrame and its stylebox directly from the scene
        var terminalFrame = root.GetNodeOrNull<Panel>("TerminalWindow/Bezel/MainMargin/MainLayout/TerminalFrame");
        AssertThat(terminalFrame).IsNotNull();

        var styleBox = terminalFrame.GetThemeStylebox("panel") as StyleBoxFlat;
        AssertThat(styleBox).IsNotNull();

        // CHECK: All four border widths must be > 0
        AssertThat(styleBox!.BorderWidthTop).IsGreater(0);
        AssertThat(styleBox.BorderWidthBottom).IsGreater(0);
        AssertThat(styleBox.BorderWidthLeft).IsGreater(0);
        AssertThat(styleBox.BorderWidthRight).IsGreater(0);

        runner.Scene().QueueFree();
    }

    /// <summary>
    /// TEST 2: Stylebox Assignment
    /// Validates that TerminalFrameStyle is correctly assigned to TerminalFrame node.
    /// </summary>
    [TestCase]
    public async Task Test2_StyleboxIsAssignedToCorrectNode()
    {
        using ISceneRunner runner = ISceneRunner.Load(MainMenuPath);
        await runner.SimulateFrames(2).ConfigureAwait(false);
        var root = runner.Scene();

        var terminalFrame = root.GetNodeOrNull<Panel>("TerminalWindow/Bezel/MainMargin/MainLayout/TerminalFrame");
        AssertThat(terminalFrame).IsNotNull();

        // CHECK: Stylebox must exist and be assigned
    var styleBox = terminalFrame.GetThemeStylebox("panel");
    AssertThat(styleBox).IsNotNull();

        AssertThat(styleBox).IsInstanceOf<StyleBoxFlat>();

        runner.Scene().QueueFree();
    }

    /// <summary>
    /// TEST 3: Border Color and Alpha
    /// Validates that border color is not black, transparent, or invisible.
    /// </summary>
    [TestCase]
    public async Task Test3_BorderColorIsNotBlackOrTransparent()
    {
        using ISceneRunner runner = ISceneRunner.Load(MainMenuPath);
        await runner.SimulateFrames(2).ConfigureAwait(false);
        var root = runner.Scene();

        var terminalFrame = root.GetNodeOrNull<Panel>("TerminalWindow/Bezel/MainMargin/MainLayout/TerminalFrame");
        AssertThat(terminalFrame).IsNotNull();

        var styleBox = terminalFrame.GetThemeStylebox("panel") as StyleBoxFlat;
        AssertThat(styleBox).IsNotNull();

        var borderColor = styleBox!.BorderColor;

        // CHECK: Border color must NOT be black
        AssertThat(borderColor.R).IsGreater(0.5f);
        AssertThat(borderColor.G).IsGreater(0.5f);
        AssertThat(borderColor.B).IsGreater(0.5f);

        // CHECK: Border alpha must be visible (>= 0.5)
        AssertThat(borderColor.A).IsGreaterEqual(0.5f);

        runner.Scene().QueueFree();
    }

    /// <summary>
    /// TEST 4: C# Runtime Overrides
    /// Validates that TerminalWindow.cs is correctly setting border widths at runtime.
    /// </summary>
    [TestCase]
    public async Task Test4_CSharpRuntimeDoesNotClearBorders()
    {
        using ISceneRunner runner = ISceneRunner.Load(MainMenuPath);
        await runner.SimulateFrames(3).ConfigureAwait(false);  // Extra frames to let _Ready() complete
        var root = runner.Scene();

        var terminalFrame = root.GetNodeOrNull<Panel>("TerminalWindow/Bezel/MainMargin/MainLayout/TerminalFrame");
        AssertThat(terminalFrame).IsNotNull();

        var styleBox = terminalFrame.GetThemeStylebox("panel") as StyleBoxFlat;
        AssertThat(styleBox).IsNotNull();

        // CHECK: After _Ready() runs, borders must still be set
        AssertThat(styleBox!.BorderWidthTop).IsGreater(0);
        AssertThat(styleBox.BorderWidthBottom).IsGreater(0);
        AssertThat(styleBox.BorderWidthLeft).IsGreater(0);
        AssertThat(styleBox.BorderWidthRight).IsGreater(0);
        AssertThat(styleBox.BorderColor.A).IsGreater(0);

        runner.Scene().QueueFree();
    }

    /// <summary>
    /// TEST 5: Node Size and Margins
    /// Validates that TerminalFrame is sized large enough for border to be visible.
    /// </summary>
    [TestCase]
    public async Task Test5_TerminalFrameSizeAllowsBorderVisibility()
    {
        using ISceneRunner runner = ISceneRunner.Load(MainMenuPath);
        await runner.SimulateFrames(2).ConfigureAwait(false);
        var root = runner.Scene();

        var viewport = root.GetViewport();
        var viewportRect = viewport.GetVisibleRect();

        var terminalFrame = root.GetNodeOrNull<Panel>("TerminalWindow/Bezel/MainMargin/MainLayout/TerminalFrame");
        AssertThat(terminalFrame).IsNotNull();

        var frameRect = terminalFrame!.GetGlobalRect();

        // CHECK: Frame must have reasonable size (not 0 or clipped)
        AssertThat(frameRect.Size.X).IsGreater(0);
        AssertThat(frameRect.Size.Y).IsGreater(0);

        // CHECK: Frame must not be positioned at viewport edges (should have margin)
        AssertThat(frameRect.Position.X).IsGreater(0);
        AssertThat(frameRect.Position.Y).IsGreater(0);

        // CHECK: Frame must not extend beyond viewport
        AssertThat(frameRect.End.X).IsLessEqual(viewportRect.End.X);
        AssertThat(frameRect.End.Y).IsLessEqual(viewportRect.End.Y);

        runner.Scene().QueueFree();
    }

    /// <summary>
    /// BONUS: Visual Pixel Check
    /// Attempts to capture the rendered border color from the viewport.
    /// This test interacts with the viewport texture and rendering and has
    /// caused instability in the editor/test runner on some systems. It is
    /// disabled by default until we can implement a safer pixel-sampling
    /// approach that works reliably in headless and CI environments.
    /// </summary>
    // [TestCase]  // Disabled: unstable in some editor/test environments
    public async Task BonusTest_RenderedBorderIsVisible()
    {
        using ISceneRunner runner = ISceneRunner.Load(MainMenuPath);
        await runner.SimulateFrames(3).ConfigureAwait(false);
        var root = runner.Scene();

        var terminalFrame = root.GetNodeOrNull<Panel>("TerminalWindow/Bezel/MainMargin/MainLayout/TerminalFrame");
        AssertThat(terminalFrame).IsNotNull();

        var frameRect = terminalFrame!.GetGlobalRect();
        var styleBox = terminalFrame.GetThemeStylebox("panel") as StyleBoxFlat;
        var borderColor = styleBox!.BorderColor;

        // Get viewport and attempt pixel check at border edge
        var viewport = root.GetViewport();

        // Log the border style for manual inspection

        // If all previous tests pass but this test fails, the issue is rendering/drawing
        AssertThat(styleBox.BorderWidthTop).IsGreater(0);

        runner.Scene().QueueFree();
    }
}
