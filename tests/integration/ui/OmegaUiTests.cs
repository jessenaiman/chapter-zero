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
/// Tests layout structure, margins, component initialization with real scenes.
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
        // Load scene and let GdUnit4 manage its lifecycle - no manual Dispose needed
        _Runner = ISceneRunner.Load("res://tests/fixtures/omega_ui_test_fixture.tscn");
        _Runner.SimulateFrames(5);
        _OmegaUi = (OmegaUi)_Runner.Scene();
        AssertThat(_OmegaUi).IsNotNull();
    }

    // ==================== LAYOUT & STRUCTURE ====================

    /// <summary>
    /// Root OmegaUi control must have full-screen anchors (0→1 on all sides).
    /// All OmegaUi subclasses inherit this layout behavior.
    /// </summary>
    [TestCase]
    public void Root_HasFullScreenAnchors()
    {
        AssertThat(_OmegaUi).IsNotNull();
        AssertThat(_OmegaUi.AnchorLeft).IsEqual(0.0f);
        AssertThat(_OmegaUi.AnchorTop).IsEqual(0.0f);
        AssertThat(_OmegaUi.AnchorRight).IsEqual(1.0f);
        AssertThat(_OmegaUi.AnchorBottom).IsEqual(1.0f);
    }

    /// <summary>
    /// ContentContainer must exist and be MarginContainer type.
    /// </summary>
    [TestCase]
    public void ContentContainer_Exists()
    {
        var container = _OmegaUi.GetNodeOrNull<MarginContainer>("ContentContainer");
        AssertThat(container).IsNotNull();
        AssertThat(container).IsInstanceOf<MarginContainer>();
    }

    /// <summary>
    /// ContentContainer must be direct child of root OmegaUi.
    /// </summary>
    [TestCase]
    public void ContentContainer_IsDirectChild()
    {
        var container = _OmegaUi.GetNodeOrNull<MarginContainer>("ContentContainer");
        AssertThat(container?.GetParent()).IsEqual(_OmegaUi);
    }

    /// <summary>
    /// ContentContainer must have full-screen anchors (0→1 on all sides).
    /// This ensures the container fills the entire parent OmegaUi control.
    /// </summary>
    [TestCase]
    public void ContentContainer_HasFullScreenAnchors()
    {
        var container = _OmegaUi.GetNodeOrNull<MarginContainer>("ContentContainer");

        AssertThat(container).IsNotNull();
        AssertThat(container!.AnchorLeft).IsEqual(0.0f);
        AssertThat(container.AnchorTop).IsEqual(0.0f);
        AssertThat(container.AnchorRight).IsEqual(1.0f);
        AssertThat(container.AnchorBottom).IsEqual(1.0f);
    }

    // ==================== COMPONENT INITIALIZATION ====================

    /// <summary>
    /// OmegaUi initializes successfully and components are created.
    /// This validates the UI loads properly - component presence is implementation detail.
    /// </summary>
    [TestCase]
    public void Initialization_Succeeds()
    {
        // The fact that Setup() passed means initialization worked
        // These components are created based on scene structure (shader layer, text display)
        AssertThat(_OmegaUi.ShaderController).IsNotNull();
        AssertThat(_OmegaUi.TextRenderer).IsNotNull();
    }
}
