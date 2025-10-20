// Copyright (c) Ωmega Spiral. All rights reserved.

using GdUnit4;
using Godot;

namespace OmegaSpiral.Tests.Stages.Stage4
{
    /// <summary>
    /// Tests for Stage 4 initialization and scene loading.
    /// Verifies that the godot-open-rpg town exploration scene loads correctly
    /// and all required systems are properly initialized.
    /// </summary>
    [TestSuite]
    public partial class Stage4InitializationTests : GdUnitTestSuite
    {
        private const string Stage4ScenePath = "res://source/stages/stage_4/field_combat.tscn";

        /// <summary>
        /// Verifies that the Stage 4 scene file exists.
        /// </summary>
        [TestCase]
        public void TestStage4SceneExists()
        {
            AssertThat(ResourceLoader.Exists(Stage4ScenePath))
                .IsTrue()
                .WithMessage($"Stage 4 scene should exist at {Stage4ScenePath}");
        }

        /// <summary>
        /// Verifies that Stage 4 scene loads without errors.
        /// </summary>
        [TestCase]
        public void TestStage4SceneLoads()
        {
            var scene = ResourceLoader.Load<PackedScene>(Stage4ScenePath);

            AssertThat(scene)
                .IsNotNull()
                .WithMessage("Stage 4 scene should load successfully");
        }

        /// <summary>
        /// Verifies that Stage 4 scene can be instantiated.
        /// </summary>
        [TestCase]
        public void TestStage4SceneInstantiates()
        {
            var scene = ResourceLoader.Load<PackedScene>(Stage4ScenePath);
            var instance = scene.Instantiate();

            AssertThat(instance)
                .IsNotNull()
                .WithMessage("Stage 4 scene should instantiate successfully");

            instance.QueueFree();
        }

        /// <summary>
        /// Verifies that the Field node exists in Stage 4.
        /// </summary>
        [TestCase]
        public void TestFieldNodeExists()
        {
            var scene = ResourceLoader.Load<PackedScene>(Stage4ScenePath);
            var instance = scene.Instantiate();
            var sceneTree = SceneRunner.Scene(instance);

            var fieldNode = sceneTree.FindChild("Field", true, false);

            AssertThat(fieldNode)
                .IsNotNull()
                .WithMessage("Field node should exist in Stage 4 scene");

            sceneTree.Free();
        }

        /// <summary>
        /// Verifies that the Map node exists in Stage 4.
        /// </summary>
        [TestCase]
        public void TestMapNodeExists()
        {
            var scene = ResourceLoader.Load<PackedScene>(Stage4ScenePath);
            var instance = scene.Instantiate();
            var sceneTree = SceneRunner.Scene(instance);

            var mapNode = sceneTree.FindChild("Map", true, false);

            AssertThat(mapNode)
                .IsNotNull()
                .WithMessage("Map node should exist in Stage 4 scene");

            sceneTree.Free();
        }

        /// <summary>
        /// Verifies that the Player node exists in Stage 4.
        /// </summary>
        [TestCase]
        public void TestPlayerNodeExists()
        {
            var scene = ResourceLoader.Load<PackedScene>(Stage4ScenePath);
            var instance = scene.Instantiate();
            var sceneTree = SceneRunner.Scene(instance);

            var playerNode = sceneTree.FindChild("PlayerController", true, false);

            AssertThat(playerNode)
                .IsNotNull()
                .WithMessage("PlayerController node should exist in Stage 4 scene");

            sceneTree.Free();
        }

        /// <summary>
        /// Verifies that TileMap nodes exist in Stage 4.
        /// </summary>
        [TestCase]
        public void TestTileMapExists()
        {
            var scene = ResourceLoader.Load<PackedScene>(Stage4ScenePath);
            var instance = scene.Instantiate();
            var sceneTree = SceneRunner.Scene(instance);

            var tileMapNode = sceneTree.FindChild("TileMap", true, false);

            AssertThat(tileMapNode)
                .IsNotNull()
                .WithMessage("TileMap node should exist in Stage 4 scene for town layout");

            sceneTree.Free();
        }

        /// <summary>
        /// Verifies that NPC nodes exist in Stage 4.
        /// </summary>
        [TestCase]
        public void TestNpcNodesExist()
        {
            var scene = ResourceLoader.Load<PackedScene>(Stage4ScenePath);
            var instance = scene.Instantiate();
            var sceneTree = SceneRunner.Scene(instance);

            // Check for some expected NPCs from godot-open-rpg
            var warriorNode = sceneTree.FindChild("Warrior", true, false);
            var thiefNode = sceneTree.FindChild("Thief", true, false);
            var monkNode = sceneTree.FindChild("Monk", true, false);

            AssertThat(warriorNode != null || thiefNode != null || monkNode != null)
                .IsTrue()
                .WithMessage("At least one NPC node should exist in Stage 4 scene");

            sceneTree.Free();
        }

        /// <summary>
        /// Verifies that the Field script is attached and valid.
        /// </summary>
        [TestCase]
        public void TestFieldScriptAttached()
        {
            var scene = ResourceLoader.Load<PackedScene>(Stage4ScenePath);
            var instance = scene.Instantiate();
            var sceneTree = SceneRunner.Scene(instance);

            var fieldNode = sceneTree.FindChild("Field", true, false);

            AssertThat(fieldNode)
                .IsNotNull()
                .WithMessage("Field node should exist");

            var script = fieldNode?.GetScript();

            AssertThat(script)
                .IsNotNull()
                .WithMessage("Field node should have a script attached");

            sceneTree.Free();
        }

        /// <summary>
        /// Verifies that GameState singleton is available.
        /// </summary>
        [TestCase]
        public void TestGameStateSingletonAvailable()
        {
            var scene = ResourceLoader.Load<PackedScene>(Stage4ScenePath);
            var instance = scene.Instantiate();
            var sceneTree = SceneRunner.Scene(instance);

            // GameState is an autoload singleton
            var gameState = sceneTree.GetNode("/root/GameState");

            AssertThat(gameState)
                .IsNotNull()
                .WithMessage("GameState singleton should be available");

            sceneTree.Free();
        }

        /// <summary>
        /// Verifies that FieldEvents singleton is available.
        /// </summary>
        [TestCase]
        public void TestFieldEventsSingletonAvailable()
        {
            var scene = ResourceLoader.Load<PackedScene>(Stage4ScenePath);
            var instance = scene.Instantiate();
            var sceneTree = SceneRunner.Scene(instance);

            // FieldEvents is an autoload singleton
            var fieldEvents = sceneTree.GetNode("/root/FieldEvents");

            AssertThat(fieldEvents)
                .IsNotNull()
                .WithMessage("FieldEvents singleton should be available");

            sceneTree.Free();
        }
    }
}
