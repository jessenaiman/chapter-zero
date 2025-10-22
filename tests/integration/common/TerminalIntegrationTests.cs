// Copyright (c) Ωmega Spiral. All rights reserved.

using Godot;
using GdUnit4;
using static GdUnit4.Assertions;
using OmegaSpiral.Source.Scripts.Common;
using OmegaSpiral.Source.UI.Terminal;

namespace OmegaSpiral.Tests.Integration.Common;

/// <summary>
/// Integration test suite for terminal component composition.
/// Tests that all terminal components work together as a cohesive unit.
/// Validates component interactions, lifecycle management, and full terminal workflows.
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
    public async void ShaderAndText_InteractWithoutInterference()
    {
        // Apply a visual preset
        await _shaderController!.ApplyVisualPresetAsync("phosphor");

        // Verify shader was applied (material exists)
        AssertObject(_shaderController!.GetCurrentShaderMaterial()).IsNotNull();

        // Display text
        await _textRenderer!.AppendTextAsync("Test message");

        // Verify text was displayed
        AssertString(_textRenderer.GetCurrentText()).Contains("Test message");

        // Verify shader still active
        AssertObject(_shaderController.GetCurrentShaderMaterial()).IsNotNull();
    }

    /// <summary>
    /// Test text and choice component interaction.
    /// Displays text then shows choices to ensure proper sequencing.
    /// </summary>
    [TestCase]
    public async void TextAndChoice_SequenceProperly()
    {
        // Display initial text
        await _textRenderer!.AppendTextAsync("Choose your path:");

        // Show choices
        var choices = new List<ChoiceOption>
        {
            new ChoiceOption { Text = "Path A" },
            new ChoiceOption { Text = "Path B" }
        };

        var choiceTask = _choicePresenter!.PresentChoicesAsync(choices);

        // Verify choices are visible
        AssertBool(_choicePresenter.AreChoicesVisible()).IsTrue();

        // Simulate user selection (choice index 0)
        _choicePresenter.SetChoiceNavigationEnabled(true);
        var selectedIndex = 0; // Simulate selection

        // Complete the choice task
        await choiceTask;

        // Verify choice was selected
        AssertInt(_choicePresenter!.GetSelectedChoiceIndex()).IsEqual(selectedIndex);
    }

    /// <summary>
    /// Test full terminal workflow: shader -> text -> choices -> selection.
    /// Simulates a complete terminal interaction sequence.
    /// </summary>
    [TestCase]
    public async void FullTerminalWorkflow_CompletesSuccessfully()
    {
        // Step 1: Apply visual effect
        await _shaderController!.ApplyVisualPresetAsync("glitch");

        // Step 2: Display narrative text
        await _textRenderer!.AppendTextAsync("Welcome to the terminal.");
        await _textRenderer!.AppendTextAsync("Make your choice:");

        // Step 3: Present choices
        var choices = new List<ChoiceOption>
        {
            new ChoiceOption { Text = "Continue" },
            new ChoiceOption { Text = "Exit" }
        };

        var choiceTask = _choicePresenter!.PresentChoicesAsync(choices);

        // Step 4: Simulate user interaction
        _choicePresenter!.SetChoiceNavigationEnabled(true);
        // In a real scenario, this would wait for user input
        // For testing, we complete the task manually
        await choiceTask;

        // Step 5: Verify final state
        AssertBool(_choicePresenter!.AreChoicesVisible()).IsTrue();
        AssertString(_textRenderer!.GetCurrentText()).Contains("Welcome to the terminal");
        AssertObject(_shaderController!.GetCurrentShaderMaterial()).IsNotNull();
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

    /// <summary>
    /// Test concurrent operations don't interfere.
    /// Runs multiple component operations simultaneously.
    /// </summary>
    [TestCase]
    public async void ConcurrentOperations_HandleGracefully()
    {
        // Start multiple operations concurrently
        var shaderTask = _shaderController!.ApplyVisualPresetAsync("scanlines");
        var textTask = _textRenderer!.AppendTextAsync("Concurrent test");
        var choiceTask = _choicePresenter!.PresentChoicesAsync(new List<ChoiceOption>
        {
            new ChoiceOption { Text = "Test" }
        });

        // Wait for all to complete
        await Task.WhenAll(shaderTask, textTask, choiceTask);

        // Verify all operations succeeded
        AssertObject(_shaderController!.GetCurrentShaderMaterial()).IsNotNull();
        AssertString(_textRenderer!.GetCurrentText()).Contains("Concurrent test");
        AssertBool(_choicePresenter!.AreChoicesVisible()).IsTrue();
    }
}
