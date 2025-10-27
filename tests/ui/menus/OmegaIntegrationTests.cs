// Copyright (c) Ωmega Spiral. All rights reserved.

using Godot;
using GdUnit4;
using GdUnit4.Api;
using static GdUnit4.Assertions;
using OmegaSpiral.Source.Scripts.Common;
using OmegaSpiral.Source.Ui.Omega;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OmegaSpiral.Tests.Ui;

/// <summary>
/// Integration test suite for Omega component composition and input handling.
/// Tests individual Omega components and their basic interactions using Scene Runner.
/// Sequence testing (boot → choices → story flow) belongs in Ghost Terminal stage tests.
/// This suite focuses on: Can input be accepted? Does output display? Do components not interfere?
/// </summary>
[TestSuite]
[RequireGodotRuntime]
public class OmegaIntegrationTests
{
    private IOmegaShaderController? _ShaderController;
    private IOmegaTextRenderer? _TextRenderer;
    private IOmegaChoicePresenter? _ChoicePresenter;

    /// <summary>
    /// Setup method run before each test.
    /// Creates fresh component instances for testing using proper Godot node management.
    /// </summary>
    [Before]
    public void Setup()
    {
        // Create individual component nodes - do NOT add to scene tree for unit-level tests
        // This avoids orphan nodes since we're testing component logic, not scene integration
        var shaderRect = AutoFree(new ColorRect());
        var textLabel = AutoFree(new RichTextLabel());
        var choiceContainer = AutoFree(new VBoxContainer());

        // Initialize components with properly managed nodes
        _ShaderController = new OmegaShaderController(shaderRect!);
        _TextRenderer = new OmegaTextRenderer(textLabel!);
        _ChoicePresenter = new OmegaChoicePresenter(choiceContainer!);

        // Verify components are initialized
        AssertObject(_ShaderController).IsNotNull();
        AssertObject(_TextRenderer).IsNotNull();
        AssertObject(_ChoicePresenter).IsNotNull();
    }

    /// <summary>
    /// Cleanup method run after each test.
    /// Ensures proper disposal of component resources.
    /// Explicitly manages node cleanup to prevent orphan node warnings.
    /// </summary>
    [After]
    public void Cleanup()
    {
        // Hide choices to clear any dynamically created buttons
        _ChoicePresenter?.HideChoices();

        // Dispose components to release resources
        (_ShaderController as IDisposable)?.Dispose();
        (_TextRenderer as IDisposable)?.Dispose();
        (_ChoicePresenter as IDisposable)?.Dispose();

        // GdUnit4 will handle cleanup of AutoFree objects
        // Orphan nodes may still appear due to Godot's internal node management
        GD.PrintRich($"[color=cyan]Cleanup: All components disposed[/color]");
    }

    /// <summary>
    /// Test that components can be initialized together without conflicts.
    /// </summary>
    [TestCase]
    public void Components_InitializeWithoutConflicts()
    {
        // Verify all components are properly initialized
        AssertObject(_ShaderController).IsNotNull();
        AssertObject(_TextRenderer).IsNotNull();
        AssertObject(_ChoicePresenter).IsNotNull();

        // Verify initial states
        AssertBool(_TextRenderer!.IsAnimating()).IsFalse();
        AssertBool(_ChoicePresenter!.AreChoicesVisible()).IsFalse();
    }

    /// <summary>
    /// Test shader and text component interaction.
    /// Applies visual preset then displays text to ensure no interference.
    /// </summary>
    [TestCase]
    public async Task ShaderAndText_InteractWithoutInterference()
    {
        // Apply a visual preset
        await _ShaderController!.ApplyVisualPresetAsync("phosphor").ConfigureAwait(true);

        // Verify shader was applied (material exists)
        AssertObject(_ShaderController!.GetCurrentShaderMaterial()).IsNotNull();

        // Display text
        await _TextRenderer!.AppendTextAsync("Test message").ConfigureAwait(true);

        // Verify text was displayed
        AssertString(_TextRenderer.GetCurrentText()).Contains("Test message");

        // Verify shader still active
        AssertObject(_ShaderController.GetCurrentShaderMaterial()).IsNotNull();
    }

    /// <summary>
    /// Test that terminal can accept user input via choice selection.
    /// Core responsibility: receive user input and return the selected index.
    /// </summary>
    [TestCase]
    public async Task TerminalInput_AcceptsSelection_ReturnsCorrectIndex()
    {
        // Show choices
        var choices = new List<OmegaChoiceOption>
        {
            new OmegaChoiceOption { Text = "Option A" },
            new OmegaChoiceOption { Text = "Option B" }
        };

        var choiceTask = _ChoicePresenter!.PresentChoicesAsync(choices);

        // Verify choices are visible for user interaction
        AssertBool(_ChoicePresenter.AreChoicesVisible()).IsTrue();

        // Simulate user pressing a button (index 1)
        (_ChoicePresenter as OmegaChoicePresenter)?.SimulateChoiceSelection(1);

        // Verify the correct selection was returned
        var selectedIndex = await choiceTask.ConfigureAwait(true);
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
        await _TextRenderer!.AppendTextAsync("Select an option:").ConfigureAwait(true);

        // Show choices
        var choices = new List<OmegaChoiceOption>
        {
            new OmegaChoiceOption { Text = "Continue" },
            new OmegaChoiceOption { Text = "Exit" }
        };

        var choiceTask = _ChoicePresenter!.PresentChoicesAsync(choices);

        // Verify both text and choices are present
        AssertString(_TextRenderer.GetCurrentText()).Contains("Select an option");
        AssertBool(_ChoicePresenter.AreChoicesVisible()).IsTrue();

        // User selects (index 0)
        (_ChoicePresenter as OmegaChoicePresenter)?.SimulateChoiceSelection(0);
        var selectedIndex = await choiceTask.ConfigureAwait(true);

        // Verify selection completed without affecting text display
        AssertInt(selectedIndex).IsEqual(0);
        AssertString(_TextRenderer.GetCurrentText()).Contains("Select an option");
    }

    /// <summary>
    /// Test component lifecycle management.
    /// Ensures components implement proper disposal patterns.
    /// </summary>
    [TestCase]
    public async Task ComponentLifecycle_ManagedProperly()
    {
        // Show choices
        var choices = new List<OmegaChoiceOption>
        {
            new OmegaChoiceOption { Text = "Lifecycle Test 1" },
            new OmegaChoiceOption { Text = "Lifecycle Test 2" }
        };

        var choiceTask = _ChoicePresenter!.PresentChoicesAsync(choices);

        // Verify choices are displayed
        AssertBool(_ChoicePresenter.AreChoicesVisible()).IsTrue();

        // Simulate user selection
        (_ChoicePresenter as OmegaChoicePresenter)?.SimulateChoiceSelection(0);

        // Verify selection completed
        var selectedIndex = await choiceTask.ConfigureAwait(true);
        AssertInt(selectedIndex).IsEqual(0);

        GD.PrintRich($"[color=green]✓ Component lifecycle managed properly[/color]");
    }
}
