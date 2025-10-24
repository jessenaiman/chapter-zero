// Copyright (c) Ωmega Spiral. All rights reserved.

using Godot;
using GdUnit4;
using static GdUnit4.Assertions;
using OmegaSpiral.Source.Scripts.Common;
using OmegaSpiral.Source.Ui.Terminal;
using System.Collections.ObjectModel;

namespace OmegaSpiral.Tests.Integration.Common;

/// <summary>
/// Integration test suite for terminal component composition and input handling.
/// Tests individual terminal components and their basic interactions.
/// Sequence testing (boot → choices → story flow) belongs in Ghost Terminal stage tests.
/// This suite focuses on: Can input be accepted? Does output display? Do components not interfere?
/// </summary>
[TestSuite]
[RequireGodotRuntime]
public partial class TerminalIntegrationTests
{
    private ITerminalShaderController? _shaderController;
    private ITerminalTextRenderer? _textRenderer;
    private ITerminalChoicePresenter? _choicePresenter;

    /// <summary>
    /// Setup method run before each test.
    /// Creates fresh component instances for testing.
    /// </summary>
    [Before]
    public void Setup()
    {
        // Create mock Godot nodes for components
        var shaderRect = new ColorRect();
        var textLabel = new RichTextLabel();
        var choiceContainer = new VBoxContainer();

        // Initialize components with mock nodes
        _shaderController = new TerminalShaderController(shaderRect);
        _textRenderer = new TerminalTextRenderer(textLabel);
        _choicePresenter = new TerminalChoicePresenter(choiceContainer);

        // Add to scene tree for Godot operations
        var testScene = new Node();
        testScene.AddChild(shaderRect);
        testScene.AddChild(textLabel);
        testScene.AddChild(choiceContainer);

        // Auto-free will clean up after test
        AutoFree(testScene);

        // Verify components are initialized
        AssertObject(_shaderController).IsNotNull();
        AssertObject(_textRenderer).IsNotNull();
        AssertObject(_choicePresenter).IsNotNull();
    }

    /// <summary>
    /// Cleanup method run after each test.
    /// Ensures proper disposal of components.
    /// </summary>
    [After]
    public void Cleanup()
    {
        (_shaderController as IDisposable)?.Dispose();
        (_textRenderer as IDisposable)?.Dispose();
        (_choicePresenter as IDisposable)?.Dispose();
    }

    /// <summary>
    /// Test that components can be initialized together without conflicts.
    /// </summary>
    [TestCase]
    public void Components_InitializeWithoutConflicts()
    {
        // Verify all components are properly initialized
        AssertObject(_shaderController).IsNotNull();
        AssertObject(_textRenderer).IsNotNull();
        AssertObject(_choicePresenter).IsNotNull();

        // Verify initial states
        AssertBool(_textRenderer!.IsAnimating()).IsFalse();
        AssertBool(_choicePresenter!.AreChoicesVisible()).IsFalse();
    }

    /// <summary>
    /// Test shader and text component interaction.
    /// Applies visual preset then displays text to ensure no interference.
    /// </summary>
    [TestCase]
    public async Task ShaderAndText_InteractWithoutInterference()
    {
        // Apply a visual preset
        await _shaderController!.ApplyVisualPresetAsync("phosphor").ConfigureAwait(false);

        // Verify shader was applied (material exists)
        AssertObject(_shaderController!.GetCurrentShaderMaterial()).IsNotNull();

        // Display text
        await _textRenderer!.AppendTextAsync("Test message").ConfigureAwait(false);

        // Verify text was displayed
        AssertString(_textRenderer.GetCurrentText()).Contains("Test message");

        // Verify shader still active
        AssertObject(_shaderController.GetCurrentShaderMaterial()).IsNotNull();
    }

    /// <summary>
    /// Test that terminal can accept user input via choice selection.
    /// Core responsibility: receive user input and return the selected index.
    /// </summary>
    [TestCase]
    public async Task TerminalInput_AcceptsSelection_ReturnsCorrectIndex()
    {
        // Show choices
        var choices = new Collection<ChoiceOption>
        {
            new ChoiceOption { Text = "Option A" },
            new ChoiceOption { Text = "Option B" }
        };

        var choiceTask = _choicePresenter!.PresentChoicesAsync(choices);

        // Verify choices are visible for user interaction
        AssertBool(_choicePresenter.AreChoicesVisible()).IsTrue();

        // Simulate user pressing a button (index 1)
        (_choicePresenter as TerminalChoicePresenter)?.SimulateChoiceSelection(1);

        // Verify the correct selection was returned
        var selectedIndex = await choiceTask.ConfigureAwait(false);
        AssertInt(selectedIndex).IsEqual(1);
    }

    /// <summary>
    /// Test that text display and choice selection don't interfere with each other.
    /// Validates component isolation at the terminal level.
    /// (Full sequence testing belongs in Ghost Terminal tests)
    /// </summary>
    [TestCase]
    public async Task TextDisplay_AndChoiceInput_DontInterfere()
    {
        // Display some text
        await _textRenderer!.AppendTextAsync("Select an option:").ConfigureAwait(false);

        // Show choices
        var choices = new Collection<ChoiceOption>
        {
            new ChoiceOption { Text = "Continue" },
            new ChoiceOption { Text = "Exit" }
        };

        var choiceTask = _choicePresenter!.PresentChoicesAsync(choices);

        // Verify both text and choices are present
        AssertString(_textRenderer.GetCurrentText()).Contains("Select an option");
        AssertBool(_choicePresenter.AreChoicesVisible()).IsTrue();

        // User selects (index 0)
        (_choicePresenter as TerminalChoicePresenter)?.SimulateChoiceSelection(0);
        var selectedIndex = await choiceTask.ConfigureAwait(false);

        // Verify selection completed without affecting text display
        AssertInt(selectedIndex).IsEqual(0);
        AssertString(_textRenderer.GetCurrentText()).Contains("Select an option");
    }

    /// <summary>
    /// Test component lifecycle management.
    /// Ensures components are properly disposed and cleaned up.
    /// </summary>
    [TestCase]
    public void ComponentLifecycle_ManagedProperly()
    {
        // Components should implement IDisposable
        AssertBool(_shaderController is IDisposable).IsTrue();
        AssertBool(_textRenderer is IDisposable).IsTrue();
        AssertBool(_choicePresenter is IDisposable).IsTrue();

        // Dispose should not throw exceptions (test will fail if they do)
        (_shaderController as IDisposable)?.Dispose();
        (_textRenderer as IDisposable)?.Dispose();
        (_choicePresenter as IDisposable)?.Dispose();
    }
}
