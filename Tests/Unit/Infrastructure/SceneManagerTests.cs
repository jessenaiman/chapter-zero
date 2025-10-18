// <copyright file="SceneManagerTests.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

// TODO: DUPLICATE FILE - This file appears to be duplicated with Tests/Common/SceneManagerTests.cs
// Review and consolidate duplicate test files after recent refactor

using GdUnit4;
using static Godot.GD;
using static GdUnit4.Assertions;

namespace OmegaSpiral.Tests.Unit.Infrastructure
{
    /// <summary>
    /// Unit tests for SceneManager functionality.
    /// Tests player data management, scene tracking, and initialization.
    /// </summary>
    [TestSuite]
    [RequireGodotRuntime]
    public class SceneManagerTests : IDisposable
    {
        private OmegaSpiral.Source.Scripts.SceneManager? sceneManager;

        [Before]
        public void Setup()
        {
            // Create a new SceneManager instance for each test
            this.sceneManager = new OmegaSpiral.Source.Scripts.SceneManager();
        }

        [After]
        public void AfterEachTest()
        {
            // Dispose of the scene manager after each test
            this.sceneManager?.Dispose();
            this.sceneManager = null;
        }

        /// <summary>
        /// Disposes of resources used by the test class.
        /// </summary>
        public void Dispose()
        {
            this.sceneManager?.Dispose();
            GC.SuppressFinalize(this);
        }

        [After]
        public void Teardown()
        {
            this.sceneManager = null;
        }

        /// <summary>
        /// Tests that player name is stored correctly.
        /// </summary>
        [TestCase]
        [RequireGodotRuntime]
        public void SetPlayerName_StoresPlayerName()
        {
            // Arrange
            string testPlayerName = "EchoVoyager";

            // Act
            this.sceneManager?.SetPlayerName(testPlayerName);

            // Assert
            AssertThat(this.sceneManager?.PlayerName).IsEqual(testPlayerName);
        }

        /// <summary>
        /// Tests that Dreamweaver thread is stored correctly.
        /// </summary>
        [TestCase]
        [RequireGodotRuntime]
        public void SetDreamweaverThread_StoresDreamweaverThread()
        {
            // Arrange
            string testThreadId = "hero";

            // Act
            this.sceneManager?.SetDreamweaverThread(testThreadId);

            // Assert
            AssertThat(this.sceneManager?.DreamweaverThread).IsEqual(testThreadId);
        }

        /// <summary>
        /// Tests that multiple thread identifiers are supported and stored correctly.
        /// </summary>
        /// <param name="threadId">The thread identifier to test.</param>
        [TestCase("hero")]
        [TestCase("shadow")]
        [TestCase("ambition")]
        [TestCase("dreamer")]
        [TestCase("seeker")]
        [RequireGodotRuntime]
        public void SetDreamweaverThread_SupportVariousThreadIds(string threadId)
        {
            // Act
            this.sceneManager?.SetDreamweaverThread(threadId);

            // Assert
            AssertThat(this.sceneManager?.DreamweaverThread).IsEqual(threadId);
        }

        /// <summary>
        /// Tests that current scene index is tracked and updated correctly.
        /// </summary>
        [TestCase]
        [RequireGodotRuntime]
        public void UpdateCurrentScene_UpdatesSceneIndex()
        {
            // Arrange
            int expectedSceneIndex = 2;

            // Act
            this.sceneManager?.UpdateCurrentScene(expectedSceneIndex);

            // Assert
            AssertThat(this.sceneManager?.CurrentSceneIndex).IsEqual(expectedSceneIndex);
        }

        /// <summary>
        /// Tests that scene index can be incremented through the game progression (1-5).
        /// </summary>
        /// <param name="sceneIndex">The scene index to test.</param>
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        [TestCase(4)]
        [TestCase(5)]
        [RequireGodotRuntime]
        public void UpdateCurrentScene_SupportsAllGameScenes(int sceneIndex)
        {
            // Act
            this.sceneManager?.UpdateCurrentScene(sceneIndex);

            // Assert
            AssertThat(this.sceneManager?.CurrentSceneIndex).IsEqual(sceneIndex);
        }

        /// <summary>
        /// Tests that both player name and Dreamweaver thread are stored independently.
        /// </summary>
        [TestCase]
        [RequireGodotRuntime]
        public void SetPlayerDataSequence_StoresBothValues()
        {
            // Arrange
            string playerName = "TestHero";
            string threadId = "shadow";

            // Act
            this.sceneManager?.SetPlayerName(playerName);
            this.sceneManager?.SetDreamweaverThread(threadId);

            // Assert
            AssertThat(this.sceneManager?.PlayerName).IsEqual(playerName);
            AssertThat(this.sceneManager?.DreamweaverThread).IsEqual(threadId);
        }

        /// <summary>
        /// Tests that player name can be updated/overwritten.
        /// </summary>
        [TestCase]
        [RequireGodotRuntime]
        public void SetPlayerName_CanBeUpdated()
        {
            // Arrange
            string firstName = "Hero1";
            string secondName = "Hero2";

            // Act
            this.sceneManager?.SetPlayerName(firstName);
            AssertThat(this.sceneManager?.PlayerName).IsEqual(firstName);

            this.sceneManager?.SetPlayerName(secondName);

            // Assert
            AssertThat(this.sceneManager?.PlayerName).IsEqual(secondName);
        }

        /// <summary>
        /// Tests that Dreamweaver thread can be updated/overwritten.
        /// </summary>
        [TestCase]
        [RequireGodotRuntime]
        public void SetDreamweaverThread_CanBeUpdated()
        {
            // Arrange
            string firstThread = "hero";
            string secondThread = "ambition";

            // Act
            this.sceneManager?.SetDreamweaverThread(firstThread);
            AssertThat(this.sceneManager?.DreamweaverThread).IsEqual(firstThread);

            this.sceneManager?.SetDreamweaverThread(secondThread);

            // Assert
            AssertThat(this.sceneManager?.DreamweaverThread).IsEqual(secondThread);
        }

        /// <summary>
        /// Tests initial state of SceneManager after construction.
        /// </summary>
        [TestCase]
        [RequireGodotRuntime]
        public void SceneManager_InitializesWithDefaultValues()
        {
            // Assert - Check default values
            AssertThat(this.sceneManager?.CurrentSceneIndex).IsEqual(1);
            AssertThat(this.sceneManager?.PlayerName).IsNull();
            AssertThat(this.sceneManager?.DreamweaverThread).IsNull();
        }

        // Test methods for different thread IDs are now handled by individual [TestCase] attributes
        // Test methods for different scene indices are now handled by individual [TestCase] attributes
    }
}
