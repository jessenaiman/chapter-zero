// <copyright file="SceneTransitionTests.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

namespace OmegaSpiral.Tests.Integration
{
    using Godot;
    using NUnit.Framework;

    /// <summary>
    /// Integration tests for scene loading functionality.
    /// Tests validate that scenes can be loaded successfully.
    /// </summary>
    [TestFixture]
    public partial class SceneTransitionTests
    {
        /// <summary>
        /// Verifies that Scene1Narrative.tscn loads and instantiates successfully.
        /// </summary>
        /// <remarks>
        /// Ensures the scene and its required components are present.
        /// </remarks>
        [Test]
        public void Scene1_LoadsSuccessfully()
        {
            // Arrange
            var scenePath = "res://Source/Scenes/Scene1Narrative.tscn";

            // Act
            var packedScene = ResourceLoader.Load<PackedScene>(scenePath);
            Assert.That(packedScene, Is.Not.Null);

            var sceneInstance = packedScene.Instantiate();
            Assert.That(sceneInstance, Is.Not.Null);

            // Assert
            Assert.That(sceneInstance.Name, Is.EqualTo("Scene1Narrative"));

            // Verify the scene has required components
            var narrativeTerminal = sceneInstance.GetNodeOrNull("NarrativeTerminal");
            Assert.That(narrativeTerminal, Is.Not.Null);

            // Cleanup
            sceneInstance.QueueFree();
        }

        /// <summary>
        /// Verifies that Scene2NethackSequence.tscn loads and instantiates successfully.
        /// </summary>
        /// <remarks>
        /// Ensures the scene loads and its root node is correct.
        /// </remarks>
        [Test]
        public void Scene2_LoadsSuccessfully()
        {
            // Arrange
            var scenePath = "res://Source/Scenes/Scene2NethackSequence.tscn";

            // Act
            var packedScene = ResourceLoader.Load<PackedScene>(scenePath);
            Assert.That(packedScene, Is.Not.Null);

            var sceneInstance = packedScene.Instantiate();
            Assert.That(sceneInstance, Is.Not.Null);

            // Assert
            Assert.That(sceneInstance.Name, Is.EqualTo("Scene2NethackSequence"));

            // Cleanup
            sceneInstance.QueueFree();
        }

        /// <summary>
        /// Verifies that the combat arena scene loads and instantiates successfully.
        /// </summary>
        /// <remarks>
        /// Ensures the combat arena scene is present and can be loaded for testing.
        /// </remarks>
        [Test]
        public void CombatArena_LoadsSuccessfully()
        {
            // Arrange
            var scenePath = "res://Source/Scripts/combat/combat_arena.tscn";

            // Act
            var packedScene = ResourceLoader.Load<PackedScene>(scenePath);
            Assert.That(packedScene, Is.Not.Null);

            var sceneInstance = packedScene.Instantiate();
            Assert.That(sceneInstance, Is.Not.Null);

            // Assert
            Assert.That(sceneInstance, Is.InstanceOf<Control>());

            // Cleanup
            sceneInstance.QueueFree();
        }
    }
}
