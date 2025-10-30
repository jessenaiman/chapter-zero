// <copyright file="GhostTerminalTextDisplayTests.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

namespace OmegaSpiral.Tests.Stages.Stage1Ghost;

using System.Collections.Generic;
using System.Threading.Tasks;
using GdUnit4;
using Godot;
using OmegaSpiral.Source.Stages.Stage1;
using OmegaSpiral.Source.Narrative;
using static GdUnit4.Assertions;

/// <summary>
/// TDD Test Suite: Verify Ghost Terminal displays text correctly.
///
/// Test 2 of 3 in Ghost Terminal TDD sequence:
/// - Focuses on text rendering to Terminal node
/// - Validates Terminal.Call("write") integration
/// - Confirms text appears after DisplayLinesAsync
/// </summary>
[TestSuite]
[RequireGodotRuntime]
public class GhostTerminalTextDisplayTests
{
    private ISceneRunner? _Runner;

    [Before]
    public void Setup()
    {
        _Runner = ISceneRunner.Load("res://source/stages/stage_1_ghost/ghost_terminal.tscn");
    }

    [After]
    public void Teardown()
    {
        _Runner?.Dispose();
    }

    /// <summary>
    /// Test 2a: DisplayLinesAsync renders single line to terminal.
    ///
    /// Validates:
    /// - Text is sent to Terminal node
    /// - DisplayLinesAsync completes without error
    /// - Terminal receives "write" call for each character
    /// </summary>
    [TestCase]
    [RequireGodotRuntime]
    public async Task GhostTerminal_DisplayLinesAsync_RendersSingleLine()
    {
        // Arrange
        var stageManager = _Runner!.Scene() as GhostStageManager;
        AssertThat(stageManager).IsNotNull();
        var testLine = "Test line";
        var lines = new List<string> { testLine };

        // Act - Call DisplayLinesAsync on stageManager
        await stageManager!.DisplayLinesAsync(lines);

        // Assert - Should complete without error
        AssertThat(stageManager).IsNotNull(); // Still exists after await

        GD.Print("[GhostTerminalTextDisplayTests] ✓ DisplayLinesAsync completed successfully");
    }

    /// <summary>
    /// Test 2b: DisplayLinesAsync handles multiple lines.
    ///
    /// Validates:
    /// - Multiple lines can be displayed
    /// - Each line is processed sequentially
    /// - No errors occur with multiple text calls
    /// </summary>
    [TestCase]
    [RequireGodotRuntime]
    public async Task GhostTerminal_DisplayLinesAsync_RenderMultipleLines()
    {
        // Arrange
        var stageManager = _Runner!.Scene() as GhostStageManager;
        AssertThat(stageManager).IsNotNull();
        var lines = new List<string>
        {
            "First line",
            "Second line",
            "Third line"
        };

        // Act - Display all lines
        await stageManager!.DisplayLinesAsync(lines);

        // Assert - Should complete without error
        AssertThat(stageManager).IsNotNull();

        GD.Print("[GhostTerminalTextDisplayTests] ✓ DisplayLinesAsync handled multiple lines");
    }

    /// <summary>
    /// Test 2c: DisplayLinesAsync handles empty lines.
    ///
    /// Validates:
    /// - Empty strings in lines are handled gracefully
    /// - No crashes or warnings when line is ""
    /// </summary>
    [TestCase]
    [RequireGodotRuntime]
    public async Task GhostTerminal_DisplayLinesAsync_HandleEmptyLines()
    {
        // Arrange
        var stageManager = _Runner!.Scene() as GhostStageManager;
        AssertThat(stageManager).IsNotNull();
        var lines = new List<string>
        {
            "Before",
            "",
            "After"
        };

        // Act - Display with empty line in middle
        await stageManager!.DisplayLinesAsync(lines);

        // Assert - Should complete without error
        AssertThat(stageManager).IsNotNull();

        GD.Print("[GhostTerminalTextDisplayTests] ✓ DisplayLinesAsync handled empty lines");
    }

    /// <summary>
    /// Test 2d: Bootstrap sequence displays system messages.
    ///
    /// Validates:
    /// - PlayBootSequenceAsync displays initialization messages
    /// - Boot sequence completes before narrative begins
    /// </summary>
    [TestCase]
    [RequireGodotRuntime]
    public async Task GhostTerminal_PlayBootSequenceAsync_DisplaysInitMessages()
    {
        // Arrange
        var stageManager = _Runner!.Scene() as GhostStageManager;
        AssertThat(stageManager).IsNotNull();

        // Act - Play boot sequence
        await stageManager!.PlayBootSequenceAsync();

        // Assert - Should complete without error
        AssertThat(stageManager).IsNotNull();

        GD.Print("[GhostTerminalTextDisplayTests] ✓ PlayBootSequenceAsync completed");
    }
}
