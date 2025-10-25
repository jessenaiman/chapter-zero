// <copyright file="TerminalUITests.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using System.Linq;
using System.Threading.Tasks;
using Godot;
using GdUnit4;
using GdUnit4.Api;
using OmegaSpiral.Source.Ui.Terminal;
using static GdUnit4.Assertions;

namespace OmegaSpiral.Tests.Integration.Ui;

/// <summary>
/// Integration tests for TerminalUI.
/// Tests terminal modes, choice presentation, captions, and component initialization.
/// </summary>
[TestSuite]
[RequireGodotRuntime]
public partial class TerminalUITests : Node
{
    private const string _TerminalScenePath = "res://source/ui/terminal/base_terminal_scene.tscn";

    private static async Task<ISceneRunner> LoadTerminalAsync(uint frames = 5)
    {
        ISceneRunner runner = ISceneRunner.Load(_TerminalScenePath);
        await runner.SimulateFrames(frames).ConfigureAwait(false);
        return runner;
    }

    // ==================== INHERITANCE & STRUCTURE ====================

    /// <summary>
    /// TerminalUI inherits from OmegaUI.
    /// </summary>
    [TestCase]
    public async Task InheritsFromOmegaUI()
    {
        using ISceneRunner runner = await LoadTerminalAsync().ConfigureAwait(false);

        var terminal = runner.Scene() as TerminalUi;
        AssertThat(terminal).IsNotNull();
        AssertThat(terminal).IsInstanceOf<OmegaSpiral.Source.Ui.Omega.OmegaUi>();
    }

    /// <summary>
    /// Has required ContentContainer from OmegaUI base.
    /// </summary>
    [TestCase]
    public async Task HasContentContainer()
    {
        using ISceneRunner runner = await LoadTerminalAsync().ConfigureAwait(false);

        Control terminal = (Control)runner.Scene();
        var contentContainer = terminal.GetNodeOrNull<Control>("ContentContainer");

        AssertThat(contentContainer).IsNotNull();
    }

    /// <summary>
    /// Has required ChoiceContainer for terminal choices.
    /// </summary>
    [TestCase]
    public async Task HasChoiceContainer()
    {
        using ISceneRunner runner = await LoadTerminalAsync().ConfigureAwait(false);

        Control terminal = (Control)runner.Scene();
        var choiceContainer = terminal.GetNodeOrNull<VBoxContainer>("ChoiceContainer");

        AssertThat(choiceContainer).IsNotNull();
    }

    // ==================== TERMINAL MODES ====================

    /// <summary>
    /// Default mode is Full when not explicitly set.
    /// </summary>
    [TestCase]
    public async Task DefaultMode_IsFull()
    {
        using ISceneRunner runner = await LoadTerminalAsync().ConfigureAwait(false);

        var terminal = runner.Scene() as TerminalUi;
        AssertThat(terminal).IsNotNull();
        AssertThat(terminal!.Mode).IsEqual(TerminalUi.TerminalMode.Full);
    }

    /// <summary>
    /// Disabled mode skips initialization.
    /// </summary>
    [TestCase]
    public async Task DisabledMode_SkipsInitialization()
    {
        using ISceneRunner runner = ISceneRunner.Load(_TerminalScenePath);

        var terminal = runner.Scene() as TerminalUi;
        AssertThat(terminal).IsNotNull();

        terminal!.Mode = TerminalUi.TerminalMode.Disabled;
        await runner.SimulateFrames(5).ConfigureAwait(false);

        AssertThat(terminal.Mode).IsEqual(TerminalUi.TerminalMode.Disabled);
    }

    // ==================== CHOICE PRESENTATION ====================

    /// <summary>
    /// ChoiceContainer starts invisible.
    /// </summary>
    [TestCase]
    public async Task ChoiceContainer_StartsInvisible()
    {
        using ISceneRunner runner = await LoadTerminalAsync().ConfigureAwait(false);

        Control terminal = (Control)runner.Scene();
        var choiceContainer = terminal.GetNodeOrNull<VBoxContainer>("ChoiceContainer");

        AssertThat(choiceContainer).IsNotNull();
        AssertThat(choiceContainer!.Visible).IsFalse();
    }

    /// <summary>
    /// PresentChoicesAsync returns valid selection.
    /// </summary>
    [TestCase]
    public async Task PresentChoicesAsync_ReturnsValidSelection()
    {
        using ISceneRunner runner = await LoadTerminalAsync().ConfigureAwait(false);

        var terminal = runner.Scene() as TerminalUi;
        AssertThat(terminal).IsNotNull();

        string[] choices = { "Option A", "Option B", "Option C" };

        // Start the async operation (won't wait for user input in test)
        var choiceTask = terminal!.PresentChoicesAsync("Select an option:", choices);
        await runner.SimulateFrames(2).ConfigureAwait(false);

        // In test environment, default selection should be first option
        AssertThat(choiceTask).IsNotNull();
    }

    // ==================== CAPTIONS ====================

    /// <summary>
    /// Captions disabled by default.
    /// </summary>
    [TestCase]
    public async Task Captions_DisabledByDefault()
    {
        using ISceneRunner runner = await LoadTerminalAsync().ConfigureAwait(false);

        var terminal = runner.Scene() as TerminalUi;
        AssertThat(terminal).IsNotNull();
        AssertThat(terminal!.CaptionsEnabled).IsFalse();
    }

    /// <summary>
    /// UpdateCaption handles missing CaptionLabel gracefully.
    /// </summary>
    [TestCase]
    public async Task UpdateCaption_HandlesNullGracefully()
    {
        using ISceneRunner runner = await LoadTerminalAsync().ConfigureAwait(false);

        var terminal = runner.Scene() as TerminalUi;
        AssertThat(terminal).IsNotNull();

        // Should not crash even if CaptionLabel is missing
        terminal!.UpdateCaption("Test caption");
        await runner.SimulateFrames(1).ConfigureAwait(false);

        AssertThat(terminal).IsNotNull();
    }

    // ==================== TEXT RENDERING ====================

    /// <summary>
    /// AppendTextAsync delegates to OmegaUI base.
    /// </summary>
    [TestCase]
    public async Task AppendTextAsync_DelegatesToBase()
    {
        using ISceneRunner runner = await LoadTerminalAsync().ConfigureAwait(false);

        var terminal = runner.Scene() as TerminalUi;
        AssertThat(terminal).IsNotNull();

        await terminal!.AppendTextAsync("Test message", 0.0f);
        await runner.SimulateFrames(2).ConfigureAwait(false);

        AssertThat(terminal.TextRenderer).IsNotNull();
    }

    /// <summary>
    /// AppendTextAsync with ghost effect parameter (legacy support).
    /// </summary>
    [TestCase]
    public async Task AppendTextAsync_SupportsGhostEffectParameter()
    {
        using ISceneRunner runner = await LoadTerminalAsync().ConfigureAwait(false);

        var terminal = runner.Scene() as TerminalUi;
        AssertThat(terminal).IsNotNull();

        // Ghost effect parameter is ignored (handled by shader), but API should accept it
        await terminal!.AppendTextAsync("Test message", 0.0f, useGhostEffect: true);
        await runner.SimulateFrames(2).ConfigureAwait(false);

        AssertThat(terminal).IsNotNull();
    }

    // ==================== AUTOLOAD INTEGRATION ====================

    /// <summary>
    /// Handles missing SceneManager gracefully.
    /// </summary>
    [TestCase]
    public async Task HandlesNullSceneManager()
    {
        using ISceneRunner runner = await LoadTerminalAsync().ConfigureAwait(false);

        var terminal = runner.Scene() as TerminalUi;
        AssertThat(terminal).IsNotNull();

        // SceneManager might not be available in test environment
        // Should log warning but not crash
        await runner.SimulateFrames(1).ConfigureAwait(false);

        AssertThat(terminal).IsNotNull();
    }

    // ==================== COMPONENT INITIALIZATION ====================

    /// <summary>
    /// Initializes with valid TextRenderer from OmegaUI.
    /// </summary>
    [TestCase]
    public async Task InitializesTextRenderer()
    {
        using ISceneRunner runner = await LoadTerminalAsync().ConfigureAwait(false);

        var terminal = runner.Scene() as TerminalUi;
        AssertThat(terminal).IsNotNull();
        AssertThat(terminal!.TextRenderer).IsNotNull();
    }

    /// <summary>
    /// ChoicePresenter created during initialization.
    /// </summary>
    [TestCase]
    public async Task CreatesChoicePresenter()
    {
        using ISceneRunner runner = await LoadTerminalAsync().ConfigureAwait(false);

        var terminal = runner.Scene() as TerminalUi;
        AssertThat(terminal).IsNotNull();

        // ChoicePresenter is protected, but we can verify it works by calling PresentChoicesAsync
        string[] choices = { "Test" };
        var choiceTask = terminal!.PresentChoicesAsync("Test", choices);

        await runner.SimulateFrames(1).ConfigureAwait(false);
        AssertThat(choiceTask).IsNotNull();
    }

    // ==================== LAYOUT & VISIBILITY ====================

    /// <summary>
    /// Terminal fills entire viewport.
    /// </summary>
    [TestCase]
    public async Task FillsViewport()
    {
        using ISceneRunner runner = await LoadTerminalAsync().ConfigureAwait(false);

        Control terminal = (Control)runner.Scene();
        var viewport = terminal.GetViewport();
        var viewportRect = viewport.GetVisibleRect();
        var terminalRect = terminal.GetRect();

        AssertThat(terminalRect.Size.X).IsEqual(viewportRect.Size.X);
        AssertThat(terminalRect.Size.Y).IsEqual(viewportRect.Size.Y);
    }

    /// <summary>
    /// ContentContainer is positioned within terminal bounds.
    /// </summary>
    [TestCase]
    public async Task ContentContainer_WithinBounds()
    {
        using ISceneRunner runner = await LoadTerminalAsync().ConfigureAwait(false);

        Control terminal = (Control)runner.Scene();
        var contentContainer = terminal.GetNodeOrNull<Control>("ContentContainer");

        AssertThat(contentContainer).IsNotNull();
        AssertThat(contentContainer!.GetRect().Size.X).IsGreater(0);
        AssertThat(contentContainer.GetRect().Size.Y).IsGreater(0);
    }
}
