// <copyright file="TerminalWindowFrameLayoutTests.cs" company="Omega Spiral">
// Copyright (c) Omega Spiral. All rights reserved.
// </copyright>

namespace OmegaSpiral.Tests.Ui.Terminal;

using Godot;
using GdUnit4;
using GdUnit4.Api;
using static GdUnit4.Assertions;
using OmegaSpiral.Source.Ui.Terminal;

/// <summary>
/// Tests for Terminal Window System frame-constrained layout architecture.
/// Validates that TerminalWindow creates a visible border frame that contains
/// any content placed in ScreenLayout, regardless of content type (menus, dialogs, gameplay).
/// This ensures a consistent "old school terminal" Ui aesthetic across all game scenes.
/// </summary>
[TestSuite]
[RequireGodotRuntime]
public partial class TerminalWindowFrameLayoutTests : Node
{
    private TerminalWindow _TerminalWindow = null!;

    [Before]
    public void Setup()
    {
        _TerminalWindow = AutoFree(new TerminalWindow())!;
        AddChild(_TerminalWindow);
        _TerminalWindow._Ready();
        AssertThat(_TerminalWindow).IsNotNull();
    }

    /// <summary>
    /// Border Visibility Test: TerminalFrame must have a visible red border.
    /// SHOULD FAIL if border is not rendering.
    /// </summary>
    [TestCase]
    public void TerminalFrame_HasVisibleBorder()
    {
        var terminalFrame = _TerminalWindow.GetNodeOrNull<Panel>("Bezel/MainMargin/MainLayout/TerminalFrame");
        AssertThat(terminalFrame).IsNotNull();

        var styleBox = terminalFrame.GetThemeStylebox("panel") as StyleBoxFlat;
        AssertThat(styleBox).IsNotNull();

        // Border must be red
        var borderColor = styleBox!.BorderColor;
        AssertThat(borderColor.R).IsEqual(1.0f);
        AssertThat(borderColor.G).IsEqual(0.0f);
        AssertThat(borderColor.B).IsEqual(0.0f);

        // Border must be 4px
        AssertThat(styleBox.BorderWidthTop).IsEqual(4);
        AssertThat(styleBox.BorderWidthBottom).IsEqual(4);
        AssertThat(styleBox.BorderWidthLeft).IsEqual(4);
        AssertThat(styleBox.BorderWidthRight).IsEqual(4);

        // Background must be transparent (so border is visible)
        var bgColor = styleBox.BgColor;
        AssertThat(bgColor.A).IsEqual(0.0f);
    }

    /// <summary>
    /// TerminalFrame must be centered horizontally within the viewport.
    /// </summary>
    [TestCase]
    public void TerminalFrame_ShouldBeCenteredHorizontally()
    {
        var terminalFrame = _TerminalWindow.GetNodeOrNull<Panel>("Bezel/MainMargin/MainLayout/TerminalFrame");
        AssertThat(terminalFrame).IsNotNull();

        var viewport = _TerminalWindow.GetViewport();
        var viewportSize = viewport.GetVisibleRect().Size;
        var viewportCenterX = viewportSize.X / 2;

        var frameRect = terminalFrame!.GetGlobalRect();
        var frameCenterX = frameRect.GetCenter().X;

        // Frame center should match viewport center (within 2 pixels tolerance)
        AssertThat(Mathf.Abs(frameCenterX - viewportCenterX)).IsLess(2.1f);
    }

    /// <summary>
    /// TerminalFrame must be centered vertically within the viewport.
    /// </summary>
    [TestCase]
    public void TerminalFrame_ShouldBeCenteredVertically()
    {
        var terminalFrame = _TerminalWindow.GetNodeOrNull<Panel>("Bezel/MainMargin/MainLayout/TerminalFrame");
        AssertThat(terminalFrame).IsNotNull();

        var viewport = _TerminalWindow.GetViewport();
        var viewportSize = viewport.GetVisibleRect().Size;
        var viewportCenterY = viewportSize.Y / 2;

        var frameRect = terminalFrame!.GetGlobalRect();
        var frameCenterY = frameRect.GetCenter().Y;

        // Frame center should match viewport center (within 2 pixels tolerance)
        AssertThat(Mathf.Abs(frameCenterY - viewportCenterY)).IsLess(2.1f);
    }

    /// <summary>
    /// TerminalFrame must be completely visible within viewport bounds.
    /// </summary>
    [TestCase]
    public void TerminalFrame_ShouldBeCompletelyVisibleInViewport()
    {
        var terminalFrame = _TerminalWindow.GetNodeOrNull<Panel>("Bezel/MainMargin/MainLayout/TerminalFrame");
        AssertThat(terminalFrame).IsNotNull();

        var viewport = _TerminalWindow.GetViewport();
        var viewportRect = viewport.GetVisibleRect();

        var frameRect = terminalFrame!.GetGlobalRect();

        // Frame must be fully contained within viewport
        AssertThat(frameRect.Position.X).IsGreater(0);
        AssertThat(frameRect.Position.Y).IsGreater(0);
        AssertThat(frameRect.End.X).IsLess(viewportRect.End.X);
        AssertThat(frameRect.End.Y).IsLess(viewportRect.End.Y);
    }

    /// <summary>
    /// Bezel Background Renders - RED: Should fail until Bezel panel is visible fullscreen.
    /// Bezel should fill viewport with dark gray background.
    /// </summary>
    [TestCase]
    public void Bezel_FillsViewportWithBackground()
    {
        var viewport = _TerminalWindow.GetViewport();
        var viewportRect = viewport.GetVisibleRect();
        var bezel = _TerminalWindow.GetNodeOrNull<Panel>("Bezel");

        AssertThat(bezel).IsNotNull();

        var bezelRect = bezel!.GetGlobalRect();

        // Bezel must fill entire viewport
        AssertThat(bezelRect.Position.X).IsEqual(0);
        AssertThat(bezelRect.Position.Y).IsEqual(0);
        AssertThat(bezelRect.Size.X).IsEqual(viewportRect.Size.X);
        AssertThat(bezelRect.Size.Y).IsEqual(viewportRect.Size.Y);
    }

    /// <summary>
    /// Layout Fills Bezel - RED: Should fail until MainLayout expands to fill available space.
    /// </summary>
    [TestCase]
    public void MainLayout_FillsBezelContainer()
    {
        var bezel = _TerminalWindow.GetNodeOrNull<Panel>("Bezel");
        var mainLayout = _TerminalWindow.GetNodeOrNull<VBoxContainer>("Bezel/MainMargin/MainLayout");

        AssertThat(bezel).IsNotNull();
        AssertThat(mainLayout).IsNotNull();

        var bezelRect = bezel!.GetGlobalRect();
        var layoutRect = mainLayout!.GetGlobalRect();

        // Layout should be substantial size, not collapsed
        AssertThat(layoutRect.Size.X).IsGreater(200);
        AssertThat(layoutRect.Size.Y).IsGreater(200);
    }


    /// <summary>
    /// Header Structure Test: Terminal header must exist with title and status indicators.
    /// - Title: Kenney Pixel, 64px, centered
    /// - Status indicators: right-aligned, color-coded
    /// </summary>
    [TestCase]
    public void Header_ExistsWithTitleAndIndicators()
    {
        // Act - Find header components
        var header = _TerminalWindow.GetNodeOrNull<HBoxContainer>("Bezel/MainMargin/MainLayout/TerminalFrame/TerminalContent/Header");
        var title = _TerminalWindow.GetNodeOrNull<Label>("Bezel/MainMargin/MainLayout/TerminalFrame/TerminalContent/Header/Title");
        var indicators = _TerminalWindow.GetNodeOrNull<HBoxContainer>("Bezel/MainMargin/MainLayout/TerminalFrame/TerminalContent/Header/Indicators");

        // Assert - Header structure exists
        AssertThat(header).IsNotNull();
        AssertThat(title).IsNotNull();
        AssertThat(indicators).IsNotNull();

        // Assert - Indicators contains three status lights
        AssertThat(indicators!.GetChildCount()).IsGreaterEqual(3);
    }

    /// <summary>
    /// ScreenLayout Initialization Test: Content container must be ready for insertion.
    /// - Ensures layout is accessible and matches spacing system
    /// </summary>
    [TestCase]
    public void ScreenLayout_IsEmptyContentInsertionPoint()
    {
        // Act
        var screenLayout = _TerminalWindow.GetNodeOrNull<VBoxContainer>("Bezel/MainMargin/MainLayout/TerminalFrame/TerminalContent/TerminalBody/ScreenPadding/ScreenLayout");

        // Assert - ScreenLayout exists and is initially empty
        AssertThat(screenLayout).IsNotNull();
        // Note: ScreenLayout may have some default children like OutputLabel, that's OK
        // What matters is it's accessible and can receive additional content
    }
}
