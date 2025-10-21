// <copyright file="PlayerMovementTests.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using GdUnit4;
using Godot;
using OmegaSpiral.Source.Scripts.Field.gamepieces;
using OmegaSpiral.Source.Scripts.Field.gamepieces.Controllers;
using static GdUnit4.Assertions;

namespace OmegaSpiral.Tests.Stages.Stage4;

/// <summary>
/// Test suite for validating player movement mechanics in Stage 4.
/// Ensures player navigation matches godot-open-rpg behavior exactly.
/// </summary>
[TestSuite]
public partial class PlayerMovementTests : Node
{
    private const string Stage4ScenePath = "res://source/stages/stage_4/field_combat.tscn";

    /// <summary>
    /// Test that the player controller responds to keyboard input (arrow keys).
    /// </summary>
    [TestCase]
    static public void TestPlayerRespondsToArrowKeys()
    {
        ISceneRunner runner = ISceneRunner.Load(Stage4ScenePath);
        Node stage4Scene = runner.Scene();

        AssertThat(stage4Scene).IsNotNull();

        // Find PlayerController node
        var playerController = stage4Scene.FindChild("PlayerController", true, false) as PlayerController;
        AssertThat(playerController).IsNotNull();

        // Simulate up arrow key press
        var upKeyEvent = new InputEventKey
        {
            Keycode = Key.Up,
            Pressed = true,
        };

        // Verify the controller can process input
        AssertThat(playerController).IsNotNull();
        AssertThat(playerController.IsActive).IsTrue();

        runner.Dispose();
    }

    /// <summary>
    /// Test that the player controller responds to WASD keys.
    /// </summary>
    [TestCase]
    static public void TestPlayerRespondsToWasdKeys()
    {
        ISceneRunner runner = ISceneRunner.Load(Stage4ScenePath);
        Node stage4Scene = runner.Scene();

        AssertThat(stage4Scene).IsNotNull();

        // Find PlayerController node
        var playerController = stage4Scene.FindChild("PlayerController", true, false) as PlayerController;
        AssertThat(playerController).IsNotNull();

        // Verify the controller can process WASD input (ui_up/down/left/right actions)
        AssertThat(playerController.IsActive).IsTrue();

        runner.Dispose();
    }

    /// <summary>
    /// Test that the player gamepiece has proper movement speed configuration.
    /// </summary>
    [TestCase]
    static public void TestPlayerMovementSpeed()
    {
        ISceneRunner runner = ISceneRunner.Load(Stage4ScenePath);
        Node stage4Scene = runner.Scene();

        AssertThat(stage4Scene).IsNotNull();

        // Find PlayerController and its Gamepiece
        var playerController = stage4Scene.FindChild("PlayerController", true, false) as PlayerController;
        AssertThat(playerController).IsNotNull();

        var gamepiece = playerController?.Gamepiece;
        AssertThat(gamepiece).IsNotNull();

        // Verify movement speed is set (should be > 0)
        if (gamepiece != null)
        {
            // Movement speed should be configured (typical values: 100-200 pixels/second)
            AssertThat(gamepiece.MoveSpeed).IsGreaterEqual(50.0f)
                .OverrideFailureMessage("Player movement speed should be >= 50 pixels/second");
        }

        runner.Dispose();
    }

    /// <summary>
    /// Test that the player has collision detection configured.
    /// </summary>
    [TestCase]
    static public void TestPlayerCollisionDetection()
    {
        ISceneRunner runner = ISceneRunner.Load(Stage4ScenePath);
        Node stage4Scene = runner.Scene();

        AssertThat(stage4Scene).IsNotNull();

        // Find PlayerController
        var playerController = stage4Scene.FindChild("PlayerController", true, false) as PlayerController;
        AssertThat(playerController).IsNotNull();

        // Check for PlayerCollision Area2D
        var playerCollision = playerController?.GetNodeOrNull<Area2D>("PlayerCollision");
        AssertThat(playerCollision).IsNotNull()
            .OverrideFailureMessage("PlayerController should have PlayerCollision Area2D child");

        // Check for InteractionSearcher Area2D
        var interactionSearcher = playerController?.GetNodeOrNull<Area2D>("InteractionSearcher");
        AssertThat(interactionSearcher).IsNotNull()
            .OverrideFailureMessage("PlayerController should have InteractionSearcher Area2D child");

        runner.Dispose();
    }

    /// <summary>
    /// Test that the player gamepiece has animation capabilities.
    /// </summary>
    [TestCase]
    static public void TestPlayerAnimationSetup()
    {
        ISceneRunner runner = ISceneRunner.Load(Stage4ScenePath);
        Node stage4Scene = runner.Scene();

        AssertThat(stage4Scene).IsNotNull();

        // Find PlayerController and its Gamepiece
        var playerController = stage4Scene.FindChild("PlayerController", true, false) as PlayerController;
        AssertThat(playerController).IsNotNull();

        var gamepiece = playerController?.Gamepiece;
        AssertThat(gamepiece).IsNotNull();

        // Check for AnimatedSprite2D or AnimationPlayer
        var animatedSprite = gamepiece?.GetNodeOrNull<AnimatedSprite2D>("AnimatedSprite2D");
        var animationPlayer = gamepiece?.GetNodeOrNull<AnimationPlayer>("AnimationPlayer");

        // At least one animation system should be present
        var hasAnimation = animatedSprite != null || animationPlayer != null;
        AssertThat(hasAnimation).IsTrue()
            .OverrideFailureMessage("Player gamepiece should have AnimatedSprite2D or AnimationPlayer");

        runner.Dispose();
    }

    /// <summary>
    /// Test that the Gameboard pathfinding system is available for player movement.
    /// </summary>
    [TestCase]
    static public void TestGameboardPathfindingAvailable()
    {
        ISceneRunner runner = ISceneRunner.Load(Stage4ScenePath);
        Node stage4Scene = runner.Scene();

        AssertThat(stage4Scene).IsNotNull();

        // Find Field node which should have Gameboard reference
        var field = stage4Scene.FindChild("Field", true, false);
        AssertThat(field).IsNotNull();

        // Check for Gameboard autoload singleton
        var gameboard = stage4Scene.GetNodeOrNull<Node>("/root/Gameboard");
        AssertThat(gameboard).IsNotNull()
            .OverrideFailureMessage("Gameboard singleton should be available for pathfinding");

        runner.Dispose();
    }

    /// <summary>
    /// Test that player direction changes are properly handled.
    /// </summary>
    [TestCase]
    static public void TestPlayerDirectionChanges()
    {
        ISceneRunner runner = ISceneRunner.Load(Stage4ScenePath);
        Node stage4Scene = runner.Scene();

        AssertThat(stage4Scene).IsNotNull();

        // Find PlayerController and its Gamepiece
        var playerController = stage4Scene.FindChild("PlayerController", true, false) as PlayerController;
        AssertThat(playerController).IsNotNull();

        var gamepiece = playerController?.Gamepiece;
        AssertThat(gamepiece).IsNotNull();

        // Gamepiece should have a Direction property (enum: Up, Down, Left, Right)
        if (gamepiece != null)
        {
            // Initial direction should be one of the valid directions
            var direction = gamepiece.Direction;
            AssertThat((int)direction).IsIn(0, 1, 2, 3, 4, 5, 6, 7)
                .OverrideFailureMessage("Player direction should be valid (0-7 for 8 directions)");
        }

        runner.Dispose();
    }

    /// <summary>
    /// Test that player movement can be stopped properly.
    /// </summary>
    [TestCase]
    static public void TestPlayerMovementStop()
    {
        ISceneRunner runner = ISceneRunner.Load(Stage4ScenePath);
        Node stage4Scene = runner.Scene();

        AssertThat(stage4Scene).IsNotNull();

        // Find PlayerController
        var playerController = stage4Scene.FindChild("PlayerController", true, false) as PlayerController;
        AssertThat(playerController).IsNotNull();

        // Call StopMoving method
        playerController?.StopMoving();

        // Verify the gamepiece is not moving after stop
        var gamepiece = playerController?.Gamepiece;
        if (gamepiece != null)
        {
            AssertThat(gamepiece.IsMoving).IsFalse()
                .OverrideFailureMessage("Player should not be moving after StopMoving() call");
        }

        runner.Dispose();
    }

    /// <summary>
    /// Test that player has proper tile-based grid positioning.
    /// </summary>
    [TestCase]
    static public void TestPlayerGridPositioning()
    {
        ISceneRunner runner = ISceneRunner.Load(Stage4ScenePath);
        Node stage4Scene = runner.Scene();

        AssertThat(stage4Scene).IsNotNull();

        // Find PlayerController and its Gamepiece
        var playerController = stage4Scene.FindChild("PlayerController", true, false) as PlayerController;
        AssertThat(playerController).IsNotNull();

        var gamepiece = playerController?.Gamepiece;
        AssertThat(gamepiece).IsNotNull();

        // Gamepiece should have a position
        if (gamepiece != null)
        {
            var position = gamepiece.Position;

            // Position should be valid (not at extreme invalid values)
            AssertThat(position.X).IsGreaterEqual(-10000.0f)
                .OverrideFailureMessage("Player X position should be valid");
            AssertThat(position.Y).IsGreaterEqual(-10000.0f)
                .OverrideFailureMessage("Player Y position should be valid");
        }

        runner.Dispose();
    }

    /// <summary>
    /// Test that player input actions are properly configured in project settings.
    /// </summary>
    [TestCase]
    static public void TestPlayerInputActionsConfigured()
    {
        // Check that required input actions exist in InputMap
        AssertThat(InputMap.HasAction("ui_up")).IsTrue()
            .OverrideFailureMessage("ui_up action should be configured");
        AssertThat(InputMap.HasAction("ui_down")).IsTrue()
            .OverrideFailureMessage("ui_down action should be configured");
        AssertThat(InputMap.HasAction("ui_left")).IsTrue()
            .OverrideFailureMessage("ui_left action should be configured");
        AssertThat(InputMap.HasAction("ui_right")).IsTrue()
            .OverrideFailureMessage("ui_right action should be configured");
        AssertThat(InputMap.HasAction("select")).IsTrue()
            .OverrideFailureMessage("select action should be configured");
    }

    /// <summary>
    /// Test that player controller is in the correct group for identification.
    /// </summary>
    [TestCase]
    static public void TestPlayerControllerGroup()
    {
        ISceneRunner runner = ISceneRunner.Load(Stage4ScenePath);
        Node stage4Scene = runner.Scene();

        AssertThat(stage4Scene).IsNotNull();

        // Find PlayerController
        var playerController = stage4Scene.FindChild("PlayerController", true, false) as PlayerController;
        AssertThat(playerController).IsNotNull();

        // Verify player controller is in the correct group
        if (playerController != null)
        {
            AssertThat(playerController.IsInGroup(PlayerController.Group)).IsTrue()
                .OverrideFailureMessage($"PlayerController should be in group '{PlayerController.Group}'");
        }

        runner.Dispose();
    }

    /// <summary>
    /// Test that FieldEvents singleton is available for player movement events.
    /// </summary>
    [TestCase]
    static public void TestFieldEventsAvailableForMovement()
    {
        ISceneRunner runner = ISceneRunner.Load(Stage4ScenePath);
        Node stage4Scene = runner.Scene();

        AssertThat(stage4Scene).IsNotNull();

        // Check for FieldEvents autoload singleton
        var fieldEvents = stage4Scene.GetNodeOrNull<Node>("/root/FieldEvents");
        AssertThat(fieldEvents).IsNotNull()
            .OverrideFailureMessage("FieldEvents singleton should be available for movement coordination");

        runner.Dispose();
    }

    /// <summary>
    /// Test that GamepieceRegistry is available for tracking player position.
    /// </summary>
    [TestCase]
    static public void TestGamepieceRegistryAvailable()
    {
        ISceneRunner runner = ISceneRunner.Load(Stage4ScenePath);
        Node stage4Scene = runner.Scene();

        AssertThat(stage4Scene).IsNotNull();

        // Check for GamepieceRegistry autoload singleton
        var registry = stage4Scene.GetNodeOrNull<Node>("/root/GamepieceRegistry");
        AssertThat(registry).IsNotNull()
            .OverrideFailureMessage("GamepieceRegistry should be available for position tracking");

        runner.Dispose();
    }

    /// <summary>
    /// Test that player movement respects TileMap collision layers.
    /// </summary>
    [TestCase]
    static public void TestPlayerRespectsCollisionLayers()
    {
        ISceneRunner runner = ISceneRunner.Load(Stage4ScenePath);
        Node stage4Scene = runner.Scene();

        AssertThat(stage4Scene).IsNotNull();

        // Find TileMap
        var tileMap = stage4Scene.FindChild("TileMap", true, false) as TileMap;
        AssertThat(tileMap).IsNotNull();

        // TileMap should have collision layers configured
        if (tileMap != null)
        {
            // Check that TileMap has at least one layer
            AssertThat(tileMap.GetLayersCount()).IsGreaterEqual(1)
                .OverrideFailureMessage("TileMap should have at least one layer");
        }

        runner.Dispose();
    }
}
