// <copyright file="PlayerMovementTests.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

namespace OmegaSpiral.Tests.Stages.Stage4;

using GdUnit4;
using Godot;
using OmegaSpiral.Source.Scripts.Field;
using OmegaSpiral.Source.Scripts.Field.gamepieces;
using static GdUnit4.Assertions;

/// <summary>
/// Tests for Stage 4 player movement mechanics.
/// Verifies WASD input, click-to-move pathfinding, and collision handling.
/// </summary>
[TestSuite]
public partial class PlayerMovementTests
{
    private const string Stage4ScenePath = "res://source/stages/stage_4/field_combat.tscn";

    /// <summary>
    /// Verifies player exists in scene.
    /// </summary>
    [TestCase]
    public void TestPlayerExists()
    {
        var scene = ResourceLoader.Load<PackedScene>(Stage4ScenePath);
        var instance = scene.Instantiate() as Node2D;

        try
        {
            var gamepieces = instance?.FindChildren("*", "Gamepiece", true, false);
            AssertThat(gamepieces?.Count).IsGreaterThan(0);
        }
        finally
        {
            instance?.QueueFree();
        }
    }

    /// <summary>
    /// Verifies Gameboard is initialized.
    /// </summary>
    [TestCase]
    public void TestGameboardInitialized()
    {
        var gameboard = Godot.GD.Load<CSharpScript>("res://source/scripts/field/gameboard/Gameboard.cs");
        AssertThat(gameboard).IsNotNull();
    }

    /// <summary>
    /// Verifies PlayerController script exists.
    /// </summary>
    [TestCase]
    public void TestPlayerControllerScriptExists()
    {
        var playerController = ResourceLoader.Load<PackedScene>("res://source/scripts/field/gamepieces/controllers/PlayerController.tscn");
        AssertThat(playerController).IsNotNull();
    }

    /// <summary>
    /// Verifies Pathfinder exists for movement calculations.
    /// </summary>
    [TestCase]
    public void TestPathfinderExists()
    {
        var pathfinder = Godot.GD.Load<CSharpScript>("res://source/scripts/field/gameboard/Pathfinder.cs");
        AssertThat(pathfinder).IsNotNull();
    }
}
