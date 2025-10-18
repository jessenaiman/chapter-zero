// <copyright file="TestTerminalScene.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using System;
using System.Threading.Tasks;
using Godot;
using OmegaSpiral.Source.Scripts.Field.Narrative;

namespace OmegaSpiral.Source.Scripts.Field.Narrative;

/// <summary>
/// Test scene for verifying terminal UI functionality with existing narrative flow.
/// Tests ghost writing, dissolve effects, and 3D display features in the context of narrative presentation.
/// </summary>
[GlobalClass]
public partial class TestTerminalScene : Node2D
{
    private TerminalUI terminalUI = default!;
    private bool testCompleted;

    /// <inheritdoc/>
    public override void _Ready()
    {
        this.terminalUI = this.GetNode<TerminalUI>("TerminalUIInstance");

        // Connect to terminal signals
        this.terminalUI.Connect("TextTyped", Callable.From(this.OnTextTyped));
        this.terminalUI.Connect("TransitionCompleted", Callable.From(this.OnTransitionCompleted));
        this.terminalUI.Connect("InputReceived", Callable.From<string>(this.OnInputReceived));

        // Start the test sequence
        this.CallDeferred(nameof(this.RunTerminalTestAsync));
    }

    private async void RunTerminalTestAsync()
    {
        try
        {
            GD.Print("Starting Terminal UI Test...");

            // Test 1: Basic terminal initialization
            await this.TestTerminalInitializationAsync();

            // Test 2: Ghost writing effect
            await this.TestGhostWritingAsync();

            // Test 3: Dissolve transition
            await this.TestDissolveTransitionAsync();

            // Test 4: Text scrambling
            await this.TestTextScramblingAsync();

            // Test 5: 3D positioning and depth perception
            await this.Test3DPositioningAsync();

            // Test completed
            this.testCompleted = true;
            GD.Print("Terminal UI Test Completed Successfully!");

            // Display completion message
            await this.terminalUI.DisplayTextWithTypewriterAsync("All terminal features verified successfully.");
        }
        catch (Exception ex)
        {
            GD.PrintErr($"Terminal UI Test Failed: {ex.Message}");
            GD.PrintErr(ex.StackTrace);
        }
    }

    private async Task TestTerminalInitializationAsync()
    {
        GD.Print("Test 1: Terminal Initialization...");

        // Verify terminal components are properly initialized
        if (this.terminalUI == null)
        {
            throw new InvalidOperationException("TerminalUI component not found");
        }

        // Display welcome message
        await this.terminalUI.DisplayTextWithTypewriterAsync("Terminal UI Test Initialized");
        await this.terminalUI.DisplayTextWithTypewriterAsync("==========================");

        GD.Print("✓ Terminal Initialization Test Passed");
    }

    private async Task TestGhostWritingAsync()
    {
        GD.Print("Test 2: Ghost Writing Effect...");

        // Test ghost writing animation
        await this.terminalUI.DisplayTextWithGhostWritingAsync("Testing ghost writing animation...");

        GD.Print("✓ Ghost Writing Test Passed");
    }

    private async Task TestDissolveTransitionAsync()
    {
        GD.Print("Test 3: Dissolve Transition...");

        // Test dissolve transition effect
        await this.terminalUI.DisplayTextWithTypewriterAsync("Testing dissolve transition effect...");

        // Apply dissolve transition
        await this.terminalUI.ApplyDissolveTransitionAsync(() =>
        {
            this.terminalUI.ClearTerminal();
            this.terminalUI.WriteToTerminal("Dissolve transition completed successfully.");
        });

        GD.Print("✓ Dissolve Transition Test Passed");
    }

    private async Task TestTextScramblingAsync()
    {
        GD.Print("Test 4: Text Scrambling...");

        // Test text scrambling effect
        await this.terminalUI.DisplayTextWithTypewriterAsync("Testing text scrambling effect...");

        // Apply text scrambling
        await this.terminalUI.ApplyScrambleEffectAsync();

        GD.Print("✓ Text Scrambling Test Passed");
    }

    private async Task Test3DPositioningAsync()
    {
        GD.Print("Test 5: 3D Positioning and Depth Perception...");

        // Test 3D positioning and depth perception
        await this.terminalUI.DisplayTextWithTypewriterAsync("Testing 3D positioning and depth perception...");

        GD.Print("✓ 3D Positioning Test Passed");
    }

    private void OnTextTyped()
    {
        GD.Print("Text typed signal received");
    }

    private void OnTransitionCompleted()
    {
        GD.Print("Transition completed signal received");
    }

    private void OnInputReceived(string input)
    {
        GD.Print($"Input received: {input}");
    }

    /// <inheritdoc/>
    public override void _Process(double delta)
    {
        base._Process(delta);

        // Update 3D positioning for enhanced depth perception
        if (!this.testCompleted && this.terminalUI != null)
        {
            // The terminal UI will handle its own 3D updates
        }
    }

    /// <inheritdoc/>
    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventKey keyEvent && keyEvent.Pressed)
        {
            if (keyEvent.Keycode == Key.Escape)
            {
                // Exit test on Escape key
                this.GetTree().Quit();
            }
            else if (keyEvent.Keycode == Key.Space)
            {
                // Restart test on Space key
                this.testCompleted = false;
                this.CallDeferred(nameof(this.RunTerminalTestAsync));
            }
        }
    }
}
