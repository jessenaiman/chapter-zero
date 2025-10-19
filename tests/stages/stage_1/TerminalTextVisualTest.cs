// <copyright file="TerminalTextVisualTest.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using Godot;
using GdUnit4;
using OmegaSpiral.Source.Scripts.Stages.Stage1;
using System.Threading.Tasks;

namespace OmegaSpiral.Tests.Stages.Stage1;

/// <summary>
/// Visual test for TerminalBase text rendering.
/// Run this test to verify text is visible on the terminal display.
/// </summary>
[TestSuite]
public partial class TerminalTextVisualTest : Node
{
    private TerminalBase? _terminal;

    /// <summary>
    /// Creates a TerminalBase instance for testing text display.
    /// </summary>
    /// <returns>A task that completes after setup.</returns>
    [Before]
    public async Task Setup()
    {
        // Load the TerminalBase scene
        var scene = GD.Load<PackedScene>("res://Source/Stages/Stage1/terminal_base.tscn");
        _terminal = scene.Instantiate<TerminalBase>();

        // Add to scene tree and wait for _Ready
        AddChild(_terminal);
        await _terminal.ToSignal(_terminal.GetTree(), "process_frame");
    }

    /// <summary>
    /// Cleans up after each test.
    /// </summary>
    [After]
    public void Cleanup()
    {
        if (_terminal != null)
        {
            _terminal.QueueFree();
            _terminal = null;
        }
    }

    /// <summary>
    /// Visual test: Displays test text to verify rendering.
    /// MANUAL: Run this test and visually confirm green text appears on black background with scanlines.
    /// </summary>
    /// <returns>A task that completes after text display.</returns>
    [TestCase]
    public async Task TestTextVisualDisplay()
    {
        Assertions.AssertThat(_terminal).IsNotNull();

        // Display test text
        await _terminal!.AppendTextAsync("> SYSTEM INITIALIZED");
        await _terminal.AppendTextAsync("> OMEGA SPIRAL v2.7.13");
        await _terminal.AppendTextAsync("> Terminal rendering test");
        await _terminal.AppendTextAsync("");
        await _terminal.AppendTextAsync("If you can read this text in CRT green,");
        await _terminal.AppendTextAsync("the terminal display is working correctly.");

        // Keep test alive for visual inspection (5 seconds)
        await _terminal.ToSignal(_terminal.GetTree().CreateTimer(5.0), "timeout");

        Assertions.AssertThat(_terminal.GetNode<RichTextLabel>("%TextDisplay").Text)
            .IsNotEmpty()
            .Contains("SYSTEM INITIALIZED");
    }

    /// <summary>
    /// Tests that text actually appears in the RichTextLabel node.
    /// </summary>
    /// <returns>A task that completes after text verification.</returns>
    [TestCase]
    public async Task TestTextContentIsSet()
    {
        Assertions.AssertThat(_terminal).IsNotNull();

        // Append some text instantly
        await _terminal!.AppendTextAsync("Test message", instant: true);

        var textDisplay = _terminal.GetNode<RichTextLabel>("%TextDisplay");
        Assertions.AssertThat(textDisplay.Text)
            .IsNotEmpty()
            .IsEqual("Test message");
    }

    /// <summary>
    /// Tests that multiple lines append correctly with newlines.
    /// </summary>
    /// <returns>A task that completes after text verification.</returns>
    [TestCase]
    public async Task TestMultipleLineAppend()
    {
        Assertions.AssertThat(_terminal).IsNotNull();

        await _terminal!.AppendTextAsync("Line 1", instant: true);
        await _terminal.AppendTextAsync("Line 2", instant: true);
        await _terminal.AppendTextAsync("Line 3", instant: true);

        var textDisplay = _terminal.GetNode<RichTextLabel>("%TextDisplay");
        string expectedText = "Line 1\nLine 2\nLine 3";

        Assertions.AssertThat(textDisplay.Text)
            .IsEqual(expectedText);
    }
}
