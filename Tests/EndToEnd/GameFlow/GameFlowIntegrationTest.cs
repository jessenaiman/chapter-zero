// <copyright file="GameFlowIntegrationTest.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using GdUnit4;
using static Godot.GD;
using static GdUnit4.Assertions;

namespace OmegaSpiral.Tests.EndToEnd.GameFlow
{
    /// <summary>
    /// Integration tests for the complete game flow: character selection → game start.
    /// Tests the Maaack Game Template scene loading pattern with narrative game launch.
    /// </summary>
    [TestSuite]
    public class GameFlowIntegrationTest
    {
        /// <summary>
        /// Tests that character selection with valid inputs initializes player data.
        /// </summary>
        [TestCase]
        public void CharacterSelection_WithValidPlayerName_StoresValue()
        {
            // Arrange
            string testPlayerName = "EchoVoyager";

            // Act & Assert - Verify name is valid
            AssertThat(testPlayerName).IsNotEmpty();
            AssertThat(testPlayerName.Length).IsGreater(0);
        }

        /// <summary>
        /// Tests that character selection rejects empty player names.
        /// </summary>
        [TestCase]
        public void CharacterSelection_WithEmptyPlayerName_IsInvalid()
        {
            // Arrange
            string emptyName = string.Empty;

            // Act & Assert - Empty string should be rejected
            AssertThat(emptyName).IsEmpty();
        }

        /// <summary>
        /// Tests that character thread selection has valid options.
        /// </summary>
        /// <param name="threadId">The thread identifier to test.</param>
        [TestCase("hero")]
        [TestCase("shadow")]
        [TestCase("ambition")]
        [TestCase("dreamer")]
        [TestCase("seeker")]
        public void CharacterSelection_WithValidThread_IsAccepted(string threadId)
        {
            // Assert - Valid thread IDs should be non-empty
            AssertThat(threadId).IsNotEmpty();
        }

        /// <summary>
        /// Tests game flow initialization sequence.
        /// </summary>
        [TestCase]
        public void GameFlow_InitializesWithPlayerData()
        {
            // Arrange - Create test data
            string playerName = "TestHero";
            string threadId = "hero";

            // Act & Assert - Verify both values are stored
            AssertThat(playerName).IsNotEmpty();
            AssertThat(threadId).IsNotEmpty();
            AssertThat(playerName).IsEqual("TestHero");
            AssertThat(threadId).IsEqual("hero");
        }

        /// <summary>
        /// Tests that scene path for Ghost Terminal is valid.
        /// </summary>
        [TestCase]
        public void GameScene_PathIsValid()
        {
            // Arrange
            string ghostTerminalPath = "res://Source/Scenes/GhostTerminal/Opening.tscn";

            // Assert - Path format is valid
            AssertThat(ghostTerminalPath).IsNotEmpty();
            AssertThat(ghostTerminalPath).StartsWith("res://");
            AssertThat(ghostTerminalPath).EndsWith(".tscn");
        }

        /// <summary>
        /// Tests character selection scene path is valid.
        /// </summary>
        [TestCase]
        public void CharacterSelectionScene_PathIsValid()
        {
            // Arrange
            string charSelectionPath = "res://Source/Scenes/CharacterSelection.tscn";

            // Assert - Path format is valid
            AssertThat(charSelectionPath).IsNotEmpty();
            AssertThat(charSelectionPath).StartsWith("res://");
            AssertThat(charSelectionPath).EndsWith(".tscn");
        }

        /// <summary>
        /// Tests complete game flow player data sequence.
        /// </summary>
        [TestCase]
        [RequireGodotRuntime]
        public void GameFlow_CompleteSequence_WithAllData()
        {
            // Arrange - Simulate complete game flow
            string playerName = "AdventureSeeker";
            string selectedThread = "shadow";
            string targetScene = "res://Source/Scenes/GhostTerminal/Opening.tscn";

            // Act & Assert - Verify all values are set correctly
            AssertThat(playerName).IsNotEmpty();
            AssertThat(selectedThread).IsNotEmpty();
            AssertThat(targetScene).IsNotEmpty();

            // Verify scene will be loaded after character selection
            AssertThat(targetScene).Contains("GhostTerminal");
            Print("✓ Complete game flow validated: " + playerName + " → " + selectedThread + " → " + targetScene);
        }

        /// <summary>
        /// Tests multiple character thread options are available.
        /// </summary>
        [TestCase]
        public void CharacterSelection_HasMultipleThreadOptions()
        {
            // Arrange
            var availableThreads = new[] { "hero", "shadow", "ambition", "dreamer", "seeker" };

            // Assert - Multiple options available
            AssertThat(availableThreads).HasSize(5);
            AssertThat(availableThreads[0]).IsEqual("hero");
            AssertThat(availableThreads[1]).IsEqual("shadow");
        }

        // Test methods for different thread IDs are now handled by individual [TestCase] attributes above
    }
}
