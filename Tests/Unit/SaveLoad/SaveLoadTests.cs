// <copyright file="SaveLoadTests.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using System.IO;
using Godot;
using NUnit.Framework;
using OmegaSpiral.Source.Scripts;
using OmegaSpiral.Source.Scripts.Common;
using OmegaSpiral.Tests;

namespace OmegaSpiral.Tests.Unit.SaveLoad
{
    /// <summary>
    /// Test class for save and load functionality.
    /// </summary>
    [TestFixture]
    public class SaveLoadTests : IDisposable
    {
        private GameState? gameState;
        private string? testSavePath;

        /// <summary>
        /// Sets up the test environment before each test.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            this.gameState = new GameState();
            this.testSavePath = "user://test_savegame.json";

            if (this.gameState == null || this.testSavePath == null)
            {
                Assert.Fail("Failed to initialize test objects");
                return;
            }

            // Clean up any existing test save
            if (Godot.FileAccess.FileExists(this.testSavePath))
            {
                DirAccess.RemoveAbsolute(ProjectSettings.GlobalizePath(this.testSavePath));
            }

            // Initialize test data
            this.gameState.PlayerName = "TestPlayer";
            this.gameState.DreamweaverThread = DreamweaverThread.Hero;
            this.gameState.CurrentScene = 2;
            this.gameState.Shards.Add("light_shard");
            this.gameState.Shards.Add("mischief_shard");
            this.gameState.SceneData["progress"] = 50;
            this.gameState.SceneData["completed_rooms"] = 3;
            this.gameState.DreamweaverScores[OmegaSpiral.Source.Scripts.Common.DreamweaverType.Light] = 25;
            this.gameState.DreamweaverScores[OmegaSpiral.Source.Scripts.Common.DreamweaverType.Mischief] = 15;
            this.gameState.DreamweaverScores[OmegaSpiral.Source.Scripts.Common.DreamweaverType.Wrath] = 5;
            this.gameState.SelectedDreamweaver = OmegaSpiral.Source.Scripts.Common.DreamweaverType.Light;

            // Create test party
            var partyData = new PartyData();
            var character = new Character("TestHero", CharacterClass.Fighter, CharacterRace.Human);
            partyData.Members.Add(character);
            this.gameState.PlayerParty = partyData;
        }

        /// <summary>
        /// Cleans up the test environment after each test.
        /// </summary>
        [TearDown]
        public void TearDown()
        {
            // Clean up test save file
            if (Godot.FileAccess.FileExists(this.testSavePath))
            {
                DirAccess.RemoveAbsolute(ProjectSettings.GlobalizePath(this.testSavePath));
            }
        }

        /// <summary>
        /// Tests that saving a game creates a save file.
        /// </summary>
        [Test]
        public void TestSaveGameCreatesFile()
        {
            if (this.gameState == null)
            {
                Assert.Fail("Test objects not initialized");
                return;
            }

            // Act
            this.gameState.SaveGame();

            // Assert
            Assert.That(Godot.FileAccess.FileExists("user://savegame.json"), Is.True, "Save file should be created");
        }

        /// <summary>
        /// Tests that the saved game contains the correct data.
        /// </summary>
        [Test]
        public void TestSaveGameContainsCorrectData()
        {
            if (this.gameState == null)
            {
                Assert.Fail("Test objects not initialized");
                return;
            }

            // Act
            this.gameState.SaveGame();

            // Assert
            Assert.That(Godot.FileAccess.FileExists("user://savegame.json"), Is.True, "Save file should exist");

            using var file = Godot.FileAccess.Open("user://savegame.json", Godot.FileAccess.ModeFlags.Read);
            var jsonString = file.GetAsText();
            var jsonNode = Json.ParseString(jsonString);

            Assert.That(jsonNode.VariantType, Is.Not.EqualTo(Variant.Type.Nil), "JSON should parse successfully");
            Assert.That(jsonNode.VariantType, Is.EqualTo(Variant.Type.Dictionary), "Root should be a dictionary");

            var saveData = jsonNode.AsGodotDictionary();
            Assert.That(saveData.ContainsKey("gameState"), Is.True, "Save data should contain gameState");

            var gameStateData = saveData["gameState"].AsGodotDictionary();
            Assert.That((int) gameStateData["currentScene"], Is.EqualTo(this.gameState.CurrentScene), "Current scene should match");
            Assert.That((string) gameStateData["playerName"], Is.EqualTo(this.gameState.PlayerName), "Player name should match");
            Assert.That((string) gameStateData["dreamweaverThread"], Is.EqualTo(this.gameState.DreamweaverThread.ToString()), "Dreamweaver thread should match");
        }

        /// <summary>
        /// Tests that loading a game returns false when no save file exists.
        /// </summary>
        [Test]
        public void TestLoadGameReturnsFalseWhenNoSaveExists()
        {
            if (this.gameState == null)
            {
                Assert.Fail("Test objects not initialized");
                return;
            }

            // Act
            var result = this.gameState.LoadGame();

            // Assert
            Assert.That(result, Is.False, "Load should return false when no save file exists");
        }

        /// <summary>
        /// Tests that a save-load cycle preserves the game data.
        /// </summary>
        [Test]
        public void TestSaveLoadCyclePreservesData()
        {
            if (this.gameState == null)
            {
                Assert.Fail("Test objects not initialized");
                return;
            }

            // Arrange - Save the game
            this.gameState.SaveGame();
            Assert.That(Godot.FileAccess.FileExists("user://savegame.json"), Is.True, "Save file should exist");

            // Create a new GameState instance to simulate loading into a fresh state
            var loadedGameState = new GameState();

            // Act - Load the game
            var loadResult = loadedGameState.LoadGame();

            // Assert
            Assert.That(loadResult, Is.True, "Load should succeed");
            Assert.That(this.gameState.CurrentScene, Is.EqualTo(loadedGameState.CurrentScene), "Current scene should be preserved");
            Assert.That(this.gameState.PlayerName, Is.EqualTo(loadedGameState.PlayerName), "Player name should be preserved");
            Assert.That(this.gameState.DreamweaverThread, Is.EqualTo(loadedGameState.DreamweaverThread), "Dreamweaver thread should be preserved");
            Assert.That(this.gameState.Shards.Count, Is.EqualTo(loadedGameState.Shards.Count), "Shard count should be preserved");
            Assert.That(this.gameState.SelectedDreamweaver, Is.EqualTo(loadedGameState.SelectedDreamweaver), "Selected dreamweaver should be preserved");
        }

        /// <summary>
        /// Tests that loading a game handles corrupted data gracefully.
        /// </summary>
        [Test]
        public void TestLoadGameHandlesCorruptedData()
        {
            if (this.gameState == null)
            {
                Assert.Fail("Test objects not initialized");
                return;
            }

            // Arrange - Create a corrupted save file
            using var file = Godot.FileAccess.Open("user://savegame.json", Godot.FileAccess.ModeFlags.Write);
            file.StoreString("invalid json content");

            // Act
            var result = this.gameState.LoadGame();

            // Assert
            Assert.That(result, Is.False, "Load should return false for corrupted data");
        }

        /// <summary>
        /// Tests that saving a game updates the timestamp.
        /// </summary>
        [Test]
        public void TestSaveGameUpdatesTimestamp()
        {
            if (this.gameState == null)
            {
                Assert.Fail("Test objects not initialized");
                return;
            }

            // Arrange
            var initialTime = this.gameState.LastSaveTime;

            // Act
            System.Threading.Thread.Sleep(10); // Small delay to ensure timestamp difference
            this.gameState.SaveGame();

            // Assert
            Assert.That(this.gameState.LastSaveTime, Is.GreaterThan(initialTime), "Last save time should be updated");
        }

        /// <summary>
        /// Tests that loading a game restores Dreamweaver scores.
        /// </summary>
        [Test]
        public void TestLoadGameRestoresDreamweaverScores()
        {
            if (this.gameState == null)
            {
                Assert.Fail("Test objects not initialized");
                return;
            }

            // Arrange - Save the game
            this.gameState.SaveGame();

            // Create new instance and load
            var loadedGameState = new GameState();
            loadedGameState.LoadGame();

            // Assert
            Assert.That(
                this.gameState.DreamweaverScores[OmegaSpiral.Source.Scripts.Common.DreamweaverType.Light],
                Is.EqualTo(loadedGameState.DreamweaverScores[OmegaSpiral.Source.Scripts.Common.DreamweaverType.Light]),
                "Light dreamweaver score should be preserved");
            Assert.That(
                this.gameState.DreamweaverScores[OmegaSpiral.Source.Scripts.Common.DreamweaverType.Mischief],
                Is.EqualTo(loadedGameState.DreamweaverScores[OmegaSpiral.Source.Scripts.Common.DreamweaverType.Mischief]),
                "Mischief dreamweaver score should be preserved");
            Assert.That(
                this.gameState.DreamweaverScores[OmegaSpiral.Source.Scripts.Common.DreamweaverType.Wrath],
                Is.EqualTo(loadedGameState.DreamweaverScores[OmegaSpiral.Source.Scripts.Common.DreamweaverType.Wrath]),
                "Wrath dreamweaver score should be preserved");
        }

        /// <summary>
        /// Tests that loading a game restores scene data.
        /// </summary>
        [Test]
        public void TestLoadGameRestoresSceneData()
        {
            if (this.gameState == null)
            {
                Assert.Fail("Test objects not initialized");
                return;
            }

            // Arrange - Save the game
            this.gameState.SaveGame();

            // Create new instance and load
            var loadedGameState = new GameState();
            loadedGameState.LoadGame();

            // Assert
            Assert.That(this.gameState.SceneData["progress"], Is.EqualTo(loadedGameState.SceneData["progress"]), "Scene progress should be preserved");
            Assert.That(this.gameState.SceneData["completed_rooms"], Is.EqualTo(loadedGameState.SceneData["completed_rooms"]), "Completed rooms should be preserved");
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            // Cleanup code if needed
            GC.SuppressFinalize(this);
        }
    }
}
