// <copyright file="OmegaUiTests.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using Godot;
using GdUnit4;
using GdUnit4.Api;
using OmegaSpiral.Source.Ui.Omega;
using static GdUnit4.Assertions;

namespace OmegaSpiral.Tests.Integration.Ui;

/// <summary>
/// Integration tests for OmegaUi component.
/// Verifies initialization, layout, and component presence.
/// </summary>
[TestSuite]
[RequireGodotRuntime]
public partial class OmegaUiTests : Node
{
    private OmegaUi _OmegaUi = null!;

    [Before]
    public void Setup()
    {
        _OmegaUi = AutoFree(new OmegaUi())!;
        AddChild(_OmegaUi);
        _OmegaUi._Ready();
    }

    [After]
    public void Teardown()
    {
        // Cleanup is handled by AutoFree for _OmegaUi.
    }

    // ==================== INHERITANCE & STRUCTURE ====================

    /// <summary>
    /// OmegaUi inherits from Control.
    /// </summary>
    [TestCase]
    public void InheritsFromControl()
    {
        AssertThat(_OmegaUi).IsInstanceOf<Control>();
    }

    /// <summary>
    /// Has required ContentContainer from OmegaUi base.
    /// </summary>
    [TestCase]
    public void HasContentContainer()
    {
        var contentContainer = _OmegaUi.GetNodeOrNull<Control>("ContentContainer");
        AssertThat(contentContainer).IsNotNull();
    }

    // ==================== TEXT RENDERING ====================

    /// <summary>
    /// Initializes with valid TextRenderer.
    /// </summary>
    [TestCase]
    public void InitializesTextRenderer()
    {
        AssertThat(_OmegaUi.TextRenderer).IsNotNull();
    }

    // ==================== LAYOUT & VISIBILITY ====================

    /// <summary>
    /// OmegaUi has ContentContainer within bounds.
    /// </summary>
    [TestCase]
    public void ContentContainer_WithinBounds()
    {
        var contentContainer = _OmegaUi.GetNodeOrNull<Control>("ContentContainer");
        AssertThat(contentContainer).IsNotNull();
        AssertThat(contentContainer!.GetRect().Size.X).IsGreater(0);
        AssertThat(contentContainer.GetRect().Size.Y).IsGreater(0);
    }
}
