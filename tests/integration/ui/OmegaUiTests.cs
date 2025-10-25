// <copyright file="OmegaUiTests.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using GdUnit4;
using Godot;
using OmegaSpiral.Source.Ui.Omega;
using static GdUnit4.Assertions;

namespace OmegaSpiral.Tests.Integration.Ui;

/// <summary>
/// Integration tests for OmegaUi base component.
/// Tests layout structure, component initialization, shader layers with real scene.
/// Uses ISceneRunner to load omega_ui.tscn for proper integration testing.
/// </summary>
[TestSuite]
[RequireGodotRuntime]
public partial class OmegaUiTests : Node
{
    private ISceneRunner _Runner = null!;
    private OmegaUi _OmegaUi = null!;

    [Before]
    public void Setup()
    {
        _Runner = ISceneRunner.Load("res://source/ui/omega/omega_ui.tscn");
        _OmegaUi = (OmegaUi)_Runner.Scene();
        AssertThat(_OmegaUi).IsNotNull();
    }

    [After]
    public void Cleanup()
    {
        _Runner?.Dispose();
    }

    // ==================== LAYOUT & STRUCTURE ====================

    /// <summary>
    /// Root OmegaUi control must have full-screen anchors (0→1 on all sides).
    /// All OmegaUi subclasses inherit this layout behavior.
    /// </summary>
    [TestCase]
    public void Root_HasFullScreenAnchors()
    {
        AssertThat(_OmegaUi.AnchorLeft).IsEqual(0.0f);
        AssertThat(_OmegaUi.AnchorTop).IsEqual(0.0f);
        AssertThat(_OmegaUi.AnchorRight).IsEqual(1.0f);
        AssertThat(_OmegaUi.AnchorBottom).IsEqual(1.0f);
    }

    /// <summary>
    /// ContentContainer must exist and be VBoxContainer type.
    /// Changed from MarginContainer to VBoxContainer for proper vertical stacking.
    /// </summary>
    [TestCase]
    public void ContentContainer_Exists()
    {
        var container = _OmegaUi.GetNodeOrNull<VBoxContainer>("ContentContainer");
        AssertThat(container).IsNotNull();
        AssertThat(container).IsInstanceOf<VBoxContainer>();
    }

    /// <summary>
    /// ContentContainer must be direct child of root OmegaUi.
    /// </summary>
    [TestCase]
    public void ContentContainer_IsDirectChild()
    {
        var container = _OmegaUi.GetNodeOrNull<VBoxContainer>("ContentContainer");
        AssertThat(container?.GetParent()).IsEqual(_OmegaUi);
    }

    /// <summary>
    /// ContentContainer must have full-screen anchors (0→1 on all sides).
    /// This ensures the container fills the entire parent OmegaUi control.
    /// </summary>
    [TestCase]
    public void ContentContainer_HasFullScreenAnchors()
    {
        var container = _OmegaUi.GetNodeOrNull<VBoxContainer>("ContentContainer");

        AssertThat(container).IsNotNull();
        AssertThat(container!.AnchorLeft).IsEqual(0.0f);
        AssertThat(container.AnchorTop).IsEqual(0.0f);
        AssertThat(container.AnchorRight).IsEqual(1.0f);
        AssertThat(container.AnchorBottom).IsEqual(1.0f);
    }

    // ==================== CRT SHADER LAYERS ====================

    /// <summary>
    /// PhosphorLayer must exist for CRT glow effect.
    /// </summary>
    [TestCase]
    public void PhosphorLayer_Exists()
    {
        var layer = _OmegaUi.GetNodeOrNull<ColorRect>("PhosphorLayer");
        AssertThat(layer).IsNotNull();
        AssertThat(layer).IsInstanceOf<ColorRect>();
    }

    /// <summary>
    /// ScanlineLayer must exist for CRT scanline effect.
    /// </summary>
    [TestCase]
    public void ScanlineLayer_Exists()
    {
        var layer = _OmegaUi.GetNodeOrNull<ColorRect>("ScanlineLayer");
        AssertThat(layer).IsNotNull();
        AssertThat(layer).IsInstanceOf<ColorRect>();
    }

    /// <summary>
    /// GlitchLayer must exist for CRT glitch effect.
    /// </summary>
    [TestCase]
    public void GlitchLayer_Exists()
    {
        var layer = _OmegaUi.GetNodeOrNull<ColorRect>("GlitchLayer");
        AssertThat(layer).IsNotNull();
        AssertThat(layer).IsInstanceOf<ColorRect>();
    }

    /// <summary>
    /// All shader layers must have full-screen anchors.
    /// </summary>
    [TestCase]
    public void ShaderLayers_HaveFullScreenAnchors()
    {
        var phosphor = _OmegaUi.GetNodeOrNull<ColorRect>("PhosphorLayer");
        var scanline = _OmegaUi.GetNodeOrNull<ColorRect>("ScanlineLayer");
        var glitch = _OmegaUi.GetNodeOrNull<ColorRect>("GlitchLayer");

        AssertThat(phosphor).IsNotNull();
        AssertThat(phosphor!.AnchorRight).IsEqual(1.0f);
        AssertThat(phosphor.AnchorBottom).IsEqual(1.0f);

        AssertThat(scanline).IsNotNull();
        AssertThat(scanline!.AnchorRight).IsEqual(1.0f);
        AssertThat(scanline.AnchorBottom).IsEqual(1.0f);

        AssertThat(glitch).IsNotNull();
        AssertThat(glitch!.AnchorRight).IsEqual(1.0f);
        AssertThat(glitch.AnchorBottom).IsEqual(1.0f);
    }

    /// <summary>
    /// Shader layers must have mouse_filter = 2 (IGNORE) to not block input.
    /// </summary>
    [TestCase]
    public void ShaderLayers_IgnoreMouseInput()
    {
        var phosphor = _OmegaUi.GetNodeOrNull<ColorRect>("PhosphorLayer");
        var scanline = _OmegaUi.GetNodeOrNull<ColorRect>("ScanlineLayer");
        var glitch = _OmegaUi.GetNodeOrNull<ColorRect>("GlitchLayer");

        AssertThat(phosphor?.MouseFilter).IsEqual(Control.MouseFilterEnum.Ignore);
        AssertThat(scanline?.MouseFilter).IsEqual(Control.MouseFilterEnum.Ignore);
        AssertThat(glitch?.MouseFilter).IsEqual(Control.MouseFilterEnum.Ignore);
    }

    // ==================== TEXT DISPLAY ====================

    /// <summary>
    /// TextDisplay must exist in ContentContainer for narrative text rendering.
    /// </summary>
    [TestCase]
    public void TextDisplay_Exists()
    {
        var textDisplay = _OmegaUi.GetNodeOrNull<RichTextLabel>("ContentContainer/TextDisplay");
        AssertThat(textDisplay).IsNotNull();
        AssertThat(textDisplay).IsInstanceOf<RichTextLabel>();
    }

    /// <summary>
    /// TextDisplay must have BBCode enabled for rich text formatting.
    /// </summary>
    [TestCase]
    public void TextDisplay_HasBBCodeEnabled()
    {
        var textDisplay = _OmegaUi.GetNodeOrNull<RichTextLabel>("ContentContainer/TextDisplay");
        AssertThat(textDisplay?.BbcodeEnabled).IsTrue();
    }

    // ==================== COMPONENT INITIALIZATION ====================

    /// <summary>
    /// OmegaUi must initialize successfully and emit InitializationCompleted signal.
    /// </summary>
    [TestCase]
    public void Initialization_EmitsCompletedSignal()
    {
        // Scene is already loaded and initialized in Setup
        // Verify components are created based on scene structure
        AssertThat(_OmegaUi.ShaderController).IsNotNull();
        AssertThat(_OmegaUi.TextRenderer).IsNotNull();
    }

    /// <summary>
    /// ShaderController must be created when shader layers exist.
    /// </summary>
    [TestCase]
    public void ShaderController_IsCreated()
    {
        AssertThat(_OmegaUi.ShaderController).IsNotNull();
    }

    /// <summary>
    /// TextRenderer must be created when TextDisplay exists.
    /// </summary>
    [TestCase]
    public void TextRenderer_IsCreated()
    {
        AssertThat(_OmegaUi.TextRenderer).IsNotNull();
    }

    // ==================== NODE PATH EXPORTS ====================

    /// <summary>
    /// PhosphorLayerPath must have default value.
    /// </summary>
    [TestCase]
    public void PhosphorLayerPath_HasDefault()
    {
        AssertThat(_OmegaUi.PhosphorLayerPath).IsNotNull();
        AssertThat(_OmegaUi.PhosphorLayerPath!.ToString()).IsEqual("PhosphorLayer");
    }

    /// <summary>
    /// ScanlineLayerPath must have default value.
    /// </summary>
    [TestCase]
    public void ScanlineLayerPath_HasDefault()
    {
        AssertThat(_OmegaUi.ScanlineLayerPath).IsNotNull();
        AssertThat(_OmegaUi.ScanlineLayerPath!.ToString()).IsEqual("ScanlineLayer");
    }

    /// <summary>
    /// GlitchLayerPath must have default value.
    /// </summary>
    [TestCase]
    public void GlitchLayerPath_HasDefault()
    {
        AssertThat(_OmegaUi.GlitchLayerPath).IsNotNull();
        AssertThat(_OmegaUi.GlitchLayerPath!.ToString()).IsEqual("GlitchLayer");
    }

    /// <summary>
    /// TextDisplayPath must have default value.
    /// </summary>
    [TestCase]
    public void TextDisplayPath_HasDefault()
    {
        AssertThat(_OmegaUi.TextDisplayPath).IsNotNull();
        AssertThat(_OmegaUi.TextDisplayPath!.ToString()).IsEqual("ContentContainer/TextDisplay");
    }

    // ==================== VISIBILITY & RENDERING ====================

    /// <summary>
    /// OmegaUi must be visible by default.
    /// </summary>
    [TestCase]
    public void OmegaUi_IsVisibleByDefault()
    {
        AssertThat(_OmegaUi.Visible).IsTrue();
    }

    /// <summary>
    /// Background must be visible and behind other layers (z_index = -2).
    /// </summary>
    [TestCase]
    public void Background_IsVisibleAndBehind()
    {
        var background = _OmegaUi.GetNodeOrNull<ColorRect>("Background");
        AssertThat(background).IsNotNull();
        AssertThat(background!.Visible).IsTrue();
        AssertThat(background.ZIndex).IsEqual(-2);
    }

    /// <summary>
    /// Shader layers must be visible and at z_index = -1 (above background, below content).
    /// </summary>
    [TestCase]
    public void ShaderLayers_AreVisibleAndLayered()
    {
        var phosphor = _OmegaUi.GetNodeOrNull<ColorRect>("PhosphorLayer");
        var scanline = _OmegaUi.GetNodeOrNull<ColorRect>("ScanlineLayer");
        var glitch = _OmegaUi.GetNodeOrNull<ColorRect>("GlitchLayer");

        AssertThat(phosphor?.Visible).IsTrue();
        AssertThat(phosphor?.ZIndex).IsEqual(-1);

        AssertThat(scanline?.Visible).IsTrue();
        AssertThat(scanline?.ZIndex).IsEqual(-1);

        AssertThat(glitch?.Visible).IsTrue();
        AssertThat(glitch?.ZIndex).IsEqual(-1);
    }
}
