// <copyright file="GameStateTests.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

namespace OmegaSpiral.Tests.Unit.Common
{
    using NUnit.Framework;
    using OmegaSpiral.Source.Scripts.Common;

    /// <summary>
    /// Tests for the GameState class.
    /// </summary>
    [TestFixture]
    public class GameStateTests
    {
        /// <summary>
        /// Tests that GameState can be instantiated.
        /// </summary>
        [Test]
        public void GameState_CanBeInstantiated()
        {
            // Act
            var gameState = new GameState();

            // Assert
            Assert.That(gameState, Is.Not.Null, "GameState should be instantiable");
        }

        /// <summary>
        /// Tests that GameState initializes with default values.
        /// </summary>
        [Test]
        public void GameState_InitializesWithDefaultValues()
        {
            // Act
            var gameState = new GameState();

            // Assert
            Assert.That(gameState.CurrentScene, Is.EqualTo(1), "Current scene should default to 1");
            Assert.That(gameState.PlayerName, Is.EqualTo(string.Empty), "Player name should default to empty string");
            Assert.That(gameState.DreamweaverThread, Is.EqualTo(DreamweaverThread.Hero), "Dreamweaver thread should default to Hero");
        }

        /// <summary>
        /// Tests that GameState properties can be set and retrieved.
        /// </summary>
        [Test]
        public void GameState_Properties_CanBeSetAndRetrieved()
        {
            // Arrange
            var gameState = new GameState();
            const string testName = "TestPlayer";
            const int testScene = 5;
            var testThread = DreamweaverThread.Shadow;

            // Act
            gameState.PlayerName = testName;
            gameState.CurrentScene = testScene;
            gameState.DreamweaverThread = testThread;

            // Assert
            Assert.That(gameState.PlayerName, Is.EqualTo(testName), "Player name should be settable");
            Assert.That(gameState.CurrentScene, Is.EqualTo(testScene), "Current scene should be settable");
            Assert.That(gameState.DreamweaverThread, Is.EqualTo(testThread), "Dreamweaver thread should be settable");
        }
    }
}
