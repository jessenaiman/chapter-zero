// <copyright file="SaveLoadTests.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System.IO;
using Godot;
using NUnit.Framework;
using OmegaSpiral.Source.Scripts;

[TestFixture]
public class SaveLoadTests : IDisposable
{
    private GameState? gameState;
    private string? testSavePath;

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
        this.gameState.DreamweaverScores[DreamweaverType.Light] = 25;
        this.gameState.DreamweaverScores[DreamweaverType.Mischief] = 15;
        this.gameState.DreamweaverScores[DreamweaverType.Wrath] = 5;
        this.gameState.SelectedDreamweaver = DreamweaverType.Light;

        // Create test party
        var partyData = new PartyData();
        var character = new Character("TestHero", CharacterClass.Fighter, CharacterRace.Human);
        partyData.Members.Add(character);
        this.gameState.PlayerParty = partyData;
    }

    [TearDown]
    public void TearDown()
    {
        // Clean up test save file
        if (Godot.FileAccess.FileExists(this.testSavePath))
        {
            DirAccess.RemoveAbsolute(ProjectSettings.GlobalizePath(this.testSavePath));
        }
    }

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
        Assert.IsTrue(Godot.FileAccess.FileExists("user://savegame.json"), "Save file should be created");
    }

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
        Assert.IsTrue(Godot.FileAccess.FileExists("user://savegame.json"), "Save file should exist");

        using var file = Godot.FileAccess.Open("user://savegame.json", Godot.FileAccess.ModeFlags.Read);
        var jsonString = file.GetAsText();
        var jsonNode = Json.ParseString(jsonString);

        Assert.IsNotNull(jsonNode, "JSON should parse successfully");
        Assert.IsInstanceOf<Godot.Collections.Dictionary>(jsonNode, "Root should be a dictionary");

        var saveData = jsonNode.AsGodotDictionary();
        Assert.IsTrue(saveData.ContainsKey("gameState"), "Save data should contain gameState");

        var gameStateData = saveData["gameState"].AsGodotDictionary();
        Assert.AreEqual(this.gameState.CurrentScene, (int)gameStateData["currentScene"], "Current scene should match");
        Assert.AreEqual(this.gameState.PlayerName, (string)gameStateData["playerName"], "Player name should match");
        Assert.AreEqual(this.gameState.DreamweaverThread.ToString(), (string)gameStateData["dreamweaverThread"], "Dreamweaver thread should match");
    }

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
        Assert.IsFalse(result, "Load should return false when no save file exists");
    }

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
        Assert.IsTrue(Godot.FileAccess.FileExists("user://savegame.json"), "Save file should exist");

        // Create a new GameState instance to simulate loading into a fresh state
        var loadedGameState = new GameState();

        // Act - Load the game
        var loadResult = loadedGameState.LoadGame();

        // Assert
        Assert.IsTrue(loadResult, "Load should succeed");
        Assert.AreEqual(this.gameState.CurrentScene, loadedGameState.CurrentScene, "Current scene should be preserved");
        Assert.AreEqual(this.gameState.PlayerName, loadedGameState.PlayerName, "Player name should be preserved");
        Assert.AreEqual(this.gameState.DreamweaverThread, loadedGameState.DreamweaverThread, "Dreamweaver thread should be preserved");
        Assert.AreEqual(this.gameState.Shards.Count, loadedGameState.Shards.Count, "Shard count should be preserved");
        Assert.AreEqual(this.gameState.SelectedDreamweaver, loadedGameState.SelectedDreamweaver, "Selected dreamweaver should be preserved");
    }

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
        Assert.IsFalse(result, "Load should return false for corrupted data");
    }

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
        Assert.Greater(this.gameState.LastSaveTime, initialTime, "Last save time should be updated");
    }

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
        Assert.AreEqual(
            this.gameState.DreamweaverScores[DreamweaverType.Light],
            loadedGameState.DreamweaverScores[DreamweaverType.Light],
            "Light dreamweaver score should be preserved");
        Assert.AreEqual(
            this.gameState.DreamweaverScores[DreamweaverType.Mischief],
            loadedGameState.DreamweaverScores[DreamweaverType.Mischief],
            "Mischief dreamweaver score should be preserved");
        Assert.AreEqual(
            this.gameState.DreamweaverScores[DreamweaverType.Wrath],
            loadedGameState.DreamweaverScores[DreamweaverType.Wrath],
            "Wrath dreamweaver score should be preserved");
    }

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
        Assert.AreEqual(this.gameState.SceneData["progress"], loadedGameState.SceneData["progress"], "Scene progress should be preserved");
        Assert.AreEqual(this.gameState.SceneData["completed_rooms"], loadedGameState.SceneData["completed_rooms"], "Completed rooms should be preserved");
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        throw new NotImplementedException();
    }
}
