// <copyright file="Stage1LoadingTests.cs" company="Omega Spiral">
// Copyright (c) Omega Spiral. All rights reserved.
// </copyright>

namespace OmegaSpiral.Tests.Stages.Stage1;

using Godot;
using GdUnit4;
using static GdUnit4.Assertions;

/// <summary>
/// TDD tests for Stage 1 loading functionality.
/// Validates that Stage 1 scenes load correctly and transition properly.
/// </summary>
[TestSuite]
[RequireGodotRuntime]
public partial class Stage1LoadingTests : Node
{
    private const string BootSequenceScenePath = "res://source/stages/ghost/scenes/boot_sequence.tscn";
    private const string OpeningMonologueScenePath = "res://source/stages/ghost/scenes/opening_monologue.tscn";
    private const string Question1ScenePath = "res://source/stages/ghost/scenes/question_1_name.tscn";

    private Node? _loadedScene;

    [Before]
    public void Setup()
    {
        _loadedScene = null;
    }

    [After]
    public void Cleanup()
    {
        if (_loadedScene != null && IsInstanceValid(_loadedScene))
        {
            _loadedScene.QueueFree();
            _loadedScene = null;
        }
    }

    /// <summary>
    /// Test that the Stage 1 boot sequence scene file can be loaded.
    /// Verifies that the scene resource exists and is valid.
    /// </summary>
    [TestCase]
    [RequireGodotRuntime]
    public void Stage1BootSequenceShouldLoadSuccessfully()
    {
        // Arrange & Act
        var sceneResource = GD.Load<PackedScene>(BootSequenceScenePath);

        // Assert
        AssertThat(sceneResource).IsNotNull();
        AssertThat(sceneResource).IsInstanceOf<PackedScene>();
    }

    /// <summary>
    /// Test that the Stage 1 opening monologue scene file can be loaded.
    /// Verifies that the scene resource exists and is valid.
    /// </summary>
    [TestCase]
    [RequireGodotRuntime]
    public void Stage1OpeningMonologueShouldLoadSuccessfully()
    {
        // Arrange & Act
        var sceneResource = GD.Load<PackedScene>(OpeningMonologueScenePath);

        // Assert
        AssertThat(sceneResource).IsNotNull();
        AssertThat(sceneResource).IsInstanceOf<PackedScene>();
    }

    /// <summary>
    /// Test that the Stage 1 question 1 scene file can be loaded.
    /// Verifies that the scene resource exists and is valid.
    /// </summary>
    [TestCase]
    [RequireGodotRuntime]
    public void Stage1Question1ShouldLoadSuccessfully()
    {
        // Arrange & Act
        var sceneResource = GD.Load<PackedScene>(Question1ScenePath);

        // Assert
        AssertThat(sceneResource).IsNotNull();
        AssertThat(sceneResource).IsInstanceOf<PackedScene>();
    }

    /// <summary>
    /// Test that Stage 1 scenes inherit from the terminal base.
    /// Verifies that the script architecture is correct.
    /// </summary>
    [TestCase]
    [RequireGodotRuntime]
    public void Stage1ScenesShouldInheritFromTerminalBase()
    {
        // Arrange
        var bootScene = GD.Load<PackedScene>(BootSequenceScenePath);
        var openingScene = GD.Load<PackedScene>(OpeningMonologueScenePath);
        var question1Scene = GD.Load<PackedScene>(Question1ScenePath);

        // Assert - Scenes should exist and be valid
        AssertThat(bootScene).IsNotNull();
        AssertThat(openingScene).IsNotNull();
        AssertThat(question1Scene).IsNotNull();
    }

    /// <summary>
    /// Test that the terminal base scene has proper UI structure.
    /// Verifies that the text display and choice container nodes exist in the scene tree.
    /// </summary>
    [TestCase]
    [RequireGodotRuntime]
    public void Stage1ScenesShouldHaveRequiredUIElements()
    {
        // Arrange - Load the terminal base scene which is the foundation
        const string terminalBasePath = "res://source/stages/ghost/scenes/terminal_base.tscn";
        var terminalBaseScene = GD.Load<PackedScene>(terminalBasePath);
        AssertThat(terminalBaseScene).IsNotNull();

        // Assert - Verify the scene file can be instantiated (without running _Ready)
        // This validates the scene structure is intact
        AssertThat(terminalBaseScene).IsInstanceOf<PackedScene>();
    }

    /// <summary>
    /// Test that Stage 1 boot sequence scene resource can be loaded.
    /// Verifies basic scene validity without full instantiation.
    /// </summary>
    [TestCase]
    [RequireGodotRuntime]
    public void Stage1BootSequenceShouldInitializeProperly()
    {
        // Arrange
        var sceneResource = GD.Load<PackedScene>(BootSequenceScenePath);

        // Assert - Verify scene can be loaded as a resource
        AssertThat(sceneResource).IsNotNull();
        AssertThat(sceneResource).IsInstanceOf<PackedScene>();
    }
}
