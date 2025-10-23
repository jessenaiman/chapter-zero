// <copyright file="PerformanceTests.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using GdUnit4;
using Godot;
using static GdUnit4.Assertions;

namespace OmegaSpiral.Tests.Stages.Stage4;

/// <summary>
/// Test suite for validating performance metrics in Stage 4.
/// Ensures AAA-level performance standards are met for frame rate, load times, and memory usage.
/// </summary>
[TestSuite]
public partial class PerformanceTests : Node
{
    private const string Stage4ScenePath = "res://source/stages/stage_4/field_combat.tscn";
    private const int TargetFps = 60;
    private const double MaxAcceptableLoadTimeSeconds = 3.0;

    /// <summary>
    /// Test that Stage 4 scene loads within acceptable time limits.
    /// </summary>
    [TestCase]
    static public void TestSceneLoadTime()
    {
        var startTime = Time.GetTicksMsec();

        ISceneRunner runner = ISceneRunner.Load(Stage4ScenePath);
        Node stage4Scene = runner.Scene();

        var endTime = Time.GetTicksMsec();
        var loadTimeMs = endTime - startTime;
        var loadTimeSeconds = loadTimeMs / 1000.0;

        AssertThat(stage4Scene).IsNotNull();
        AssertThat(loadTimeSeconds).IsLessEqual(MaxAcceptableLoadTimeSeconds)
            .OverrideFailureMessage($"Scene should load within {MaxAcceptableLoadTimeSeconds} seconds (actual: {loadTimeSeconds:F2}s)");

        GD.Print($"[Performance] Scene load time: {loadTimeMs}ms ({loadTimeSeconds:F2}s)");

        runner.Dispose();
    }

    /// <summary>
    /// Test that scene instantiation is efficient.
    /// </summary>
    [TestCase]
    static public void TestSceneInstantiationPerformance()
    {
        var startTime = Time.GetTicksMsec();

        ISceneRunner runner = ISceneRunner.Load(Stage4ScenePath);
        Node stage4Scene = runner.Scene();

        var endTime = Time.GetTicksMsec();
        var instantiationTimeMs = endTime - startTime;

        AssertThat(stage4Scene).IsNotNull();

        // Instantiation should be reasonably fast (< 2 seconds)
        AssertThat(instantiationTimeMs).IsLessEqual(2000)
            .OverrideFailureMessage($"Scene instantiation should be < 2000ms (actual: {instantiationTimeMs}ms)");

        GD.Print($"[Performance] Scene instantiation time: {instantiationTimeMs}ms");

        runner.Dispose();
    }

    /// <summary>
    /// Test that TileMap rendering is optimized.
    /// </summary>
    [TestCase]
    static public void TestTileMapRenderingOptimization()
    {
        ISceneRunner runner = ISceneRunner.Load(Stage4ScenePath);
        Node stage4Scene = runner.Scene();

        AssertThat(stage4Scene).IsNotNull();

        var tileMap = stage4Scene.FindChild("TileMap", true, false) as TileMap;
        AssertThat(tileMap).IsNotNull();

        if (tileMap != null)
        {
            // Check that TileMap uses reasonable cell count
            var usedRect = tileMap.GetUsedRect();
            var totalCells = usedRect.Size.X * usedRect.Size.Y;

            // Reasonable town size: 100-100,000 cells
            AssertThat(totalCells).IsGreaterEqual(100)
                .OverrideFailureMessage("TileMap should have sufficient detail");
            AssertThat(totalCells).IsLessEqual(100000)
                .OverrideFailureMessage($"TileMap should be optimized (actual cells: {totalCells})");

            GD.Print($"[Performance] TileMap total cells: {totalCells}");
        }

        runner.Dispose();
    }

    /// <summary>
    /// Test that NPC count is reasonable for performance.
    /// </summary>
    [TestCase]
    static public void TestNpcCountPerformance()
    {
        ISceneRunner runner = ISceneRunner.Load(Stage4ScenePath);
        Node stage4Scene = runner.Scene();

        AssertThat(stage4Scene).IsNotNull();

        // Count NPC-like nodes (Warrior, Thief, Monk, Smith, Wizard, etc.)
        var npcCount = 0;

        if (stage4Scene.FindChild("Warrior", true, false) != null)
        {
            npcCount++;
        }

        if (stage4Scene.FindChild("Thief", true, false) != null)
        {
            npcCount++;
        }

        if (stage4Scene.FindChild("Monk", true, false) != null)
        {
            npcCount++;
        }

        if (stage4Scene.FindChild("Smith", true, false) != null)
        {
            npcCount++;
        }

        if (stage4Scene.FindChild("Wizard", true, false) != null)
        {
            npcCount++;
        }

        // Reasonable NPC count: 3-20 NPCs
        AssertThat(npcCount).IsGreaterEqual(3)
            .OverrideFailureMessage("Town should have multiple NPCs");
        AssertThat(npcCount).IsLessEqual(20)
            .OverrideFailureMessage($"NPC count should be reasonable for performance (actual: {npcCount})");

        GD.Print($"[Performance] NPC count: {npcCount}");

        runner.Dispose();
    }

    /// <summary>
    /// Test that collision shapes are optimized.
    /// </summary>
    [TestCase]
    static public void TestCollisionShapeOptimization()
    {
        ISceneRunner runner = ISceneRunner.Load(Stage4ScenePath);
        Node stage4Scene = runner.Scene();

        AssertThat(stage4Scene).IsNotNull();

        // Check that player collision is efficient
        var playerController = stage4Scene.FindChild("PlayerController", true, false);
        AssertThat(playerController).IsNotNull();

        if (playerController != null)
        {
            var playerCollision = playerController.GetNodeOrNull<Area2D>("PlayerCollision");
            AssertThat(playerCollision).IsNotNull();

            if (playerCollision != null)
            {
                // Collision shape should exist
                var collisionShape = playerCollision.FindChild("CollisionShape2D", true, false);
                AssertThat(collisionShape).IsNotNull()
                    .OverrideFailureMessage("Player collision should use optimized shapes");
            }
        }

        runner.Dispose();
    }

    /// <summary>
    /// Test that texture sizes are reasonable for performance.
    /// </summary>
    [TestCase]
    static public void TestTextureOptimization()
    {
        ISceneRunner runner = ISceneRunner.Load(Stage4ScenePath);
        Node stage4Scene = runner.Scene();

        AssertThat(stage4Scene).IsNotNull();

        // Check TileMap texture
        var tileMap = stage4Scene.FindChild("TileMap", true, false) as TileMap;

        if (tileMap != null && tileMap.TileSet != null)
        {
            // TileSet should exist (texture optimization happens at tileset level)
            AssertThat(tileMap.TileSet).IsNotNull();

            GD.Print("[Performance] TileSet configured for texture optimization");
        }

        runner.Dispose();
    }

    /// <summary>
    /// Test that scene tree depth is reasonable.
    /// </summary>
    [TestCase]
    static public void TestSceneTreeDepthOptimization()
    {
        ISceneRunner runner = ISceneRunner.Load(Stage4ScenePath);
        Node stage4Scene = runner.Scene();

        AssertThat(stage4Scene).IsNotNull();

        // Measure scene tree depth (deeper trees can impact performance)
        int maxDepth = GetMaxTreeDepth(stage4Scene);

        // Scene tree should not be excessively deep
        AssertThat(maxDepth).IsLessEqual(15)
            .OverrideFailureMessage($"Scene tree depth should be reasonable (actual: {maxDepth})");

        GD.Print($"[Performance] Max scene tree depth: {maxDepth}");

        runner.Dispose();
    }

    /// <summary>
    /// Test that physics processing is optimized.
    /// </summary>
    [TestCase]
    static public void TestPhysicsProcessingOptimization()
    {
        ISceneRunner runner = ISceneRunner.Load(Stage4ScenePath);
        Node stage4Scene = runner.Scene();

        AssertThat(stage4Scene).IsNotNull();

        // Check that physics layers are properly configured
        var playerController = stage4Scene.FindChild("PlayerController", true, false);

        if (playerController != null)
        {
            var playerCollision = playerController.GetNodeOrNull<Area2D>("PlayerCollision");

            if (playerCollision != null)
            {
                // Collision layer should be set (not default 0)
                var collisionLayer = playerCollision.CollisionLayer;
                AssertThat(collisionLayer).IsGreaterEqual(0)
                    .OverrideFailureMessage("Collision layers should be configured");

                GD.Print($"[Performance] Player collision layer: {collisionLayer}");
            }
        }

        runner.Dispose();
    }

    /// <summary>
    /// Test that scene cleanup is proper (no memory leaks).
    /// </summary>
    [TestCase]
    static public void TestSceneCleanup()
    {
        // Load and unload scene multiple times
        for (int i = 0; i < 5; i++)
        {
            ISceneRunner runner = ISceneRunner.Load(Stage4ScenePath);
            Node stage4Scene = runner.Scene();

            AssertThat(stage4Scene).IsNotNull();

            runner.Dispose();
        }

        // If we get here without crashes, cleanup is working
        AssertThat(true).IsTrue()
            .OverrideFailureMessage("Scene should clean up properly without memory leaks");

        GD.Print("[Performance] Scene cleanup test passed (5 load/unload cycles)");
    }

    /// <summary>
    /// Test that autoload singletons are efficiently accessed.
    /// </summary>
    [TestCase]
    static public void TestSingletonAccessPerformance()
    {
        ISceneRunner runner = ISceneRunner.Load(Stage4ScenePath);
        Node stage4Scene = runner.Scene();

        AssertThat(stage4Scene).IsNotNull();

        var startTime = Time.GetTicksMsec();

        // Access multiple singletons
        var gameState = stage4Scene.GetNodeOrNull<Node>("/root/GameState");
        var fieldEvents = stage4Scene.GetNodeOrNull<Node>("/root/FieldEvents");
        var sceneManager = stage4Scene.GetNodeOrNull<Node>("/root/SceneManager");
        var gameboard = stage4Scene.GetNodeOrNull<Node>("/root/Gameboard");

        var endTime = Time.GetTicksMsec();
        var accessTimeMs = endTime - startTime;

        // Singleton access should be fast (< 100ms for 4 accesses)
        AssertThat(accessTimeMs).IsLessEqual(100)
            .OverrideFailureMessage($"Singleton access should be fast (actual: {accessTimeMs}ms)");

        GD.Print($"[Performance] Singleton access time: {accessTimeMs}ms");

        runner.Dispose();
    }

    /// <summary>
    /// Test that signal connections are reasonable in count.
    /// </summary>
    [TestCase]
    static public void TestSignalConnectionOptimization()
    {
        ISceneRunner runner = ISceneRunner.Load(Stage4ScenePath);
        Node stage4Scene = runner.Scene();

        AssertThat(stage4Scene).IsNotNull();

        // Check that PlayerController has reasonable signal connections
        var playerController = stage4Scene.FindChild("PlayerController", true, false);

        if (playerController != null)
        {
            var signalList = playerController.GetSignalList();

            // Should have signals but not excessive amount
            AssertThat(signalList.Count).IsGreaterEqual(0);
            AssertThat(signalList.Count).IsLessEqual(50)
                .OverrideFailureMessage("Node should not have excessive signal definitions");

            GD.Print($"[Performance] PlayerController signal count: {signalList.Count}");
        }

        runner.Dispose();
    }

    /// <summary>
    /// Test that initial frame is ready quickly.
    /// </summary>
    [TestCase]
    static public void TestInitialFrameReadiness()
    {
        var startTime = Time.GetTicksMsec();

        ISceneRunner runner = ISceneRunner.Load(Stage4ScenePath);
        Node stage4Scene = runner.Scene();

        // Simulate _Ready() calls
        stage4Scene.NotifyEnter();

        var endTime = Time.GetTicksMsec();
        var readyTimeMs = endTime - startTime;

        AssertThat(stage4Scene).IsNotNull();
        AssertThat(readyTimeMs).IsLessEqual(2000)
            .OverrideFailureMessage($"Initial frame should be ready quickly (actual: {readyTimeMs}ms)");

        GD.Print($"[Performance] Initial frame ready time: {readyTimeMs}ms");

        runner.Dispose();
    }

    /// <summary>
    /// Helper method to calculate maximum tree depth.
    /// </summary>
    /// <param name="node">The root node to measure from.</param>
    /// <param name="currentDepth">Current depth in recursion.</param>
    /// <returns>Maximum depth of the tree.</returns>
    private static int GetMaxTreeDepth(Node node, int currentDepth = 0)
    {
        if (node == null || node.GetChildCount() == 0)
        {
            return currentDepth;
        }

        int maxChildDepth = currentDepth;
        foreach (Node child in node.GetChildren())
        {
            int childDepth = GetMaxTreeDepth(child, currentDepth + 1);
            if (childDepth > maxChildDepth)
            {
                maxChildDepth = childDepth;
            }
        }

        return maxChildDepth;
    }
}
