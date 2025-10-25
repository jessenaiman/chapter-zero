// <copyright file="TerminalUITests.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using Godot;
using GdUnit4;
using GdUnit4.Api;
using OmegaSpiral.Source.Ui.Terminal;
using static GdUnit4.Assertions;

namespace OmegaSpiral.Tests.Integration.Ui;

/// <summary>
/// Integration tests for TerminalUI component.
/// Verifies initialization, modes, layout, and component presence.
/// </summary>
[TestSuite]
[RequireGodotRuntime]
public partial class TerminalUITests : Node
{
    private TerminalUi _Terminal = null!;

    [Before]
    public void Setup()
    {
        _Terminal = AutoFree(new TerminalUi())!;
        AddChild(_Terminal);
        _Terminal._Ready();
    }

    [After]
    public void Teardown()
    {
        // Cleanup is handled by AutoFree for _terminal.
    }

    // ==================== INHERITANCE & STRUCTURE ====================

    /// <summary>
    /// TerminalUI inherits from OmegaUI.
    /// </summary>
    [TestCase]
    public void InheritsFromOmegaUI()
    {
        AssertThat(_Terminal).IsInstanceOf<OmegaSpiral.Source.Ui.Omega.OmegaUi>();
    }

    /// <summary>
    /// Has required ContentContainer from OmegaUI base.
    /// </summary>
    [TestCase]
    public void HasContentContainer()
    {
        var contentContainer = _Terminal.GetNodeOrNull<Control>("ContentContainer");
        AssertThat(contentContainer).IsNotNull();
    }

    /// <summary>
    /// Has required ChoiceContainer for terminal choices.
    /// </summary>
    [TestCase]
    public void HasChoiceContainer()
    {
        var choiceContainer = _Terminal.GetNodeOrNull<VBoxContainer>("ChoiceContainer");
        AssertThat(choiceContainer).IsNotNull();
    }

    // ==================== TERMINAL MODES ====================

    /// <summary>
    /// Default mode is Full when not explicitly set.
    /// </summary>
    [TestCase]
    public void DefaultMode_IsFull()
    {
        AssertThat(_Terminal.Mode).IsEqual(TerminalUi.TerminalMode.Full);
    }

    /// <summary>
    /// Disabled mode can be set.
    /// </summary>
    [TestCase]
    public void DisabledMode_CanBeSet()
    {
        _Terminal.Mode = TerminalUi.TerminalMode.Disabled;
        AssertThat(_Terminal.Mode).IsEqual(TerminalUi.TerminalMode.Disabled);
    }

    // ==================== CHOICE PRESENTATION ====================

    /// <summary>
    /// ChoiceContainer starts invisible.
    /// </summary>
    [TestCase]
    public void ChoiceContainer_StartsInvisible()
    {
        var choiceContainer = _Terminal.GetNodeOrNull<VBoxContainer>("ChoiceContainer");
        AssertThat(choiceContainer).IsNotNull();
        AssertThat(choiceContainer!.Visible).IsFalse();
    }

    // ==================== CAPTIONS ====================

    /// <summary>
    /// Captions disabled by default.
    /// </summary>
    [TestCase]
    public void Captions_DisabledByDefault()
    {
        AssertThat(_Terminal.CaptionsEnabled).IsFalse();
    }

    // ==================== TEXT RENDERING ====================

    /// <summary>
    /// Initializes with valid TextRenderer from OmegaUI.
    /// </summary>
    [TestCase]
    public void InitializesTextRenderer()
    {
        AssertThat(_Terminal.TextRenderer).IsNotNull();
    }

    // ==================== LAYOUT & VISIBILITY ====================

    /// <summary>
    /// Terminal has ContentContainer within bounds.
    /// </summary>
    [TestCase]
    public void ContentContainer_WithinBounds()
    {
        var contentContainer = _Terminal.GetNodeOrNull<Control>("ContentContainer");
        AssertThat(contentContainer).IsNotNull();
        AssertThat(contentContainer!.GetRect().Size.X).IsGreater(0);
        AssertThat(contentContainer.GetRect().Size.Y).IsGreater(0);
    }
}
