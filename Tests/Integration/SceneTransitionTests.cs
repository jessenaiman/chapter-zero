// <copyright file="SceneTransitionTests.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
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
