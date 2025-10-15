// <copyright file="StatePersistenceTests.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using System;
using System.IO;
using Godot;
using NUnit.Framework;
using OmegaSpiral.Source.Scripts;
using OmegaSpiral.Source.Scripts.Common;

namespace OmegaSpiral.Tests.Unit.SaveLoad
{
    /// <summary>
    /// Contains tests for verifying state persistence across scenes, shards, and party data.
    /// </summary>
    /// <remarks>
    /// Ensures that game state is correctly saved and restored in various scenarios.
    /// </remarks>
    [TestFixture]
    public class StatePersistenceTests : IDisposable
    {
        private GameState? gameState;
        private SceneManager? sceneManager;

        /// <summary>
        /// Sets up the test environment for state persistence tests.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            // Initialize test objects here
            this.gameState = new GameState();
            this.sceneManager = new SceneManager();
        }

        /// <summary>
        /// Tests that state persists correctly across different scenes.
        /// </summary>
        [Test]
        public void TestStatePersistenceAcrossScenes()
        {
            if (this.gameState == null || this.sceneManager == null)
            {
                Assert.Fail("Test objects not initialized");
                return;
            }

            // Act - Simulate scene transition
            this.sceneManager.UpdateCurrentScene(2);
            this.sceneManager.SetPlayerName("UpdatedPlayer");
            this.sceneManager.SetDreamweaverThread(DreamweaverThread.Ambition.ToString());

            // Assert - State should persist
            Assert.That(this.gameState.CurrentScene, Is.EqualTo(2), "Current scene should be updated");
            Assert.That(this.gameState.PlayerName, Is.EqualTo("UpdatedPlayer"), "Player name should be updated");
            Assert.That(this.gameState.DreamweaverThread, Is.EqualTo(DreamweaverThread.Ambition), "Dreamweaver thread should be updated");
        }

        /// <summary>
        /// Tests that shard collections are persisted correctly.
        /// </summary>
        [Test]
        public void TestShardCollectionPersistence()
        {
            if (this.gameState == null || this.sceneManager == null)
            {
                Assert.Fail("Test objects not initialized");
                return;
            }

            // Arrange
            var initialShardCount = this.gameState.Shards.Count;

            // Act
            this.gameState.Shards.Add("new_shard");

            // Assert
            Assert.That(this.gameState.Shards.Count, Is.EqualTo(initialShardCount + 1), "Shard count should increase");
            Assert.That(this.gameState.Shards, Contains.Item("new_shard"), "New shard should be added");
        }

        /// <summary>
        /// Tests that scene data is persisted and restored correctly.
        /// </summary>
        [Test]
        public void TestSceneDataPersistence()
        {
            if (this.gameState == null)
            {
                Assert.Fail("Test objects not initialized");
                return;
            }

            // Arrange
            var testKey = "scene_progress";
            var testValue = 75;

            // Act
            this.gameState.SceneData[testKey] = testValue;

            // Assert
            Assert.That(this.gameState.SceneData[testKey], Is.EqualTo(testValue), "Scene data should persist");
        }

        /// <summary>
        /// Tests that party data is persisted and restored correctly.
        /// </summary>
        [Test]
        public void TestPartyDataPersistence()
        {
            if (this.gameState == null)
            {
                Assert.Fail("Test objects not initialized");
                return;
            }

            // Arrange
            var partyData = new PartyData();
            var testCharacter = new Character("TestChar", CharacterClass.Fighter, CharacterRace.Human);
            partyData.Members.Add(testCharacter);

            // Act
            this.gameState.PlayerParty = partyData;

            // Assert
            Assert.That(this.gameState.PlayerParty.Members.Count, Is.EqualTo(1), "Party should have one member");
            Assert.That(this.gameState.PlayerParty.Members[0].Name, Is.EqualTo("TestChar"), "Character name should match");
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            // Clean up resources if needed
        }
    }
}
