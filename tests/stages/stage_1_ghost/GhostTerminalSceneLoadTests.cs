// <copyright file="GhostTerminalSceneLoadTests.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

namespace OmegaSpiral.Tests.Stages.Stage1Ghost;

using GdUnit4;
using Godot;
using OmegaSpiral.Source.Stages.Stage1;
using static GdUnit4.Assertions;

/// <summary>
/// TDD Test Suite: Verify Ghost Terminal scene loads and initializes correctly.
///
/// Test 1 of 3 in Ghost Terminal TDD sequence:
/// - This test focuses on scene loading and node attachment
/// - Validates ghost_terminal.tscn can be instantiated
/// - Confirms GhostStageManager is properly attached
/// </summary>
[TestSuite]
[RequireGodotRuntime]
public class GhostTerminalSceneLoadTests
{
    private ISceneRunner? _Runner;

    /// <summary>
    /// Setup: Load the ghost_terminal scene for all tests.
    /// Always dispose the runner after tests to prevent orphan nodes.
    /// </summary>
    [Before]
    public void Setup()
    {
        _Runner = ISceneRunner.Load("res://source/stages/stage_1_ghost/ghost_terminal.tscn");
    }

    /// <summary>
    /// Teardown: Dispose the scene runner to free all scene nodes.
    /// This is CRITICAL to prevent orphan nodes from accumulating.
    /// </summary>
    [After]
    public void Teardown()
    {
        _Runner?.Dispose();
    }

    /// <summary>
    /// Test 1: Terminal scene loads without errors.
    ///
    /// Validates:
    /// - Scene file exists and can be instantiated
    /// - No Godot errors during load
    /// - Root node is a Control
    /// </summary>
    [TestCase]
    [RequireGodotRuntime]
    public void GhostTerminal_Loads_Successfully()
    {
        // Arrange
        AssertThat(_Runner).IsNotNull();

        // Act
        var root = _Runner!.Scene();

        // Assert - Scene should load and have a root Control node
        AssertThat(root).IsNotNull();
        AssertThat(root).IsInstanceOf<Control>();

        GD.Print("[GhostTerminalSceneLoadTests] ✓ ghost_terminal.tscn loaded successfully");
    }

    /// <summary>
    /// Test 1b: GhostStageManager is attached to root node.
    ///
    /// Validates:
    /// - GhostStageManager script is attached
    /// - Manager has required properties (DefaultTypingSpeed, Terminal ref, ChoiceContainer ref)
    /// </summary>
    [TestCase]
    [RequireGodotRuntime]
    public void GhostTerminal_HasGhostStageManagerScript()
    {
        // Arrange
        var root = _Runner!.Scene();

        // Act
        var stageManager = root as GhostStageManager;

        // Assert - Root should have GhostStageManager script
        AssertThat(stageManager).IsNotNull();
        AssertThat(stageManager!.DefaultTypingSpeed).IsEqual(15f);

        GD.Print("[GhostTerminalSceneLoadTests] ✓ GhostStageManager script attached and initialized");
    }

    /// <summary>
    /// Test 1c: Terminal node exists in scene.
    ///
    /// Validates:
    /// - Terminal (godot_xterm) node exists at expected path
    /// - Terminal node is ready for text output
    /// </summary>
    [TestCase]
    [RequireGodotRuntime]
    public void GhostTerminal_HasTerminalNodeAtExpectedPath()
    {
        // Arrange
        var root = _Runner!.Scene();

        // Act - Terminal should be at this path in the scene tree
        var terminal = root.GetNodeOrNull("OmegaFrame/NarrativeViewport/NarrativeStack/Terminal");

        // Assert - Terminal node should exist
        AssertThat(terminal).IsNotNull();
        AssertThat(terminal!.GetClass()).IsEqual("Terminal");

        GD.Print("[GhostTerminalSceneLoadTests] ✓ Terminal node found at expected path");
    }

    /// <summary>
    /// Test 1d: ChoiceContainer node exists for displaying choices.
    ///
    /// Validates:
    /// - ChoiceContainer (VBoxContainer) exists at expected path
    /// - Container is initially hidden
    /// </summary>
    [TestCase]
    [RequireGodotRuntime]
    public void GhostTerminal_HasChoiceContainer()
    {
        // Arrange
        var root = _Runner!.Scene();

        // Act - ChoiceContainer should be at this path
        var choiceContainer = root.GetNodeOrNull<VBoxContainer>("OmegaFrame/NarrativeViewport/NarrativeStack/ChoiceContainer");

        // Assert - Container should exist and be hidden initially
        AssertThat(choiceContainer).IsNotNull();
        AssertThat(choiceContainer!.Visible).IsFalse();

        GD.Print("[GhostTerminalSceneLoadTests] ✓ ChoiceContainer found and initially hidden");
    }
}
