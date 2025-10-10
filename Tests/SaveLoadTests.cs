using NUnit.Framework;
using Godot;
using System.IO;
using OmegaSpiral.Source.Scripts;

[TestFixture]
public class SaveLoadTests
{
    private GameState _gameState;
    private string _testSavePath;

    [SetUp]
    public void Setup()
    {
        _gameState = new GameState();
        _testSavePath = "user://test_savegame.json";

        // Clean up any existing test save
        if (Godot.FileAccess.FileExists(_testSavePath))
        {
            DirAccess.RemoveAbsolute(ProjectSettings.GlobalizePath(_testSavePath));
        }

        // Initialize test data
        _gameState.PlayerName = "TestPlayer";
        _gameState.DreamweaverThread = DreamweaverThread.Hero;
        _gameState.CurrentScene = 2;
        _gameState.Shards.Add("light_shard");
        _gameState.Shards.Add("mischief_shard");
        _gameState.SceneData["progress"] = 50;
        _gameState.SceneData["completed_rooms"] = 3;
        _gameState.DreamweaverScores[DreamweaverType.Light] = 25;
        _gameState.DreamweaverScores[DreamweaverType.Mischief] = 15;
        _gameState.DreamweaverScores[DreamweaverType.Wrath] = 5;
        _gameState.SelectedDreamweaver = DreamweaverType.Light;

        // Create test party
        var partyData = new PartyData();
        var character = new Character("TestHero", CharacterClass.Fighter, CharacterRace.Human);
        partyData.Members.Add(character);
        _gameState.PlayerParty = partyData;
    }

    [TearDown]
    public void TearDown()
    {
        // Clean up test save file
        if (Godot.FileAccess.FileExists(_testSavePath))
        {
            DirAccess.RemoveAbsolute(ProjectSettings.GlobalizePath(_testSavePath));
        }
    }

    [Test]
    public void TestSaveGameCreatesFile()
    {
        // Act
        _gameState.SaveGame();

        // Assert
        Assert.IsTrue(Godot.FileAccess.FileExists("user://savegame.json"), "Save file should be created");
    }

    [Test]
    public void TestSaveGameContainsCorrectData()
    {
        // Act
        _gameState.SaveGame();

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
        Assert.AreEqual(_gameState.CurrentScene, (int)gameStateData["currentScene"], "Current scene should match");
        Assert.AreEqual(_gameState.PlayerName, (string)gameStateData["playerName"], "Player name should match");
        Assert.AreEqual(_gameState.DreamweaverThread.ToString(), (string)gameStateData["dreamweaverThread"], "Dreamweaver thread should match");
    }

    [Test]
    public void TestLoadGameReturnsFalseWhenNoSaveExists()
    {
        // Act
        var result = _gameState.LoadGame();

        // Assert
        Assert.IsFalse(result, "Load should return false when no save file exists");
    }

    [Test]
    public void TestSaveLoadCyclePreservesData()
    {
        // Arrange - Save the game
        _gameState.SaveGame();
        Assert.IsTrue(Godot.FileAccess.FileExists("user://savegame.json"), "Save file should exist");

        // Create a new GameState instance to simulate loading into a fresh state
        var loadedGameState = new GameState();

        // Act - Load the game
        var loadResult = loadedGameState.LoadGame();

        // Assert
        Assert.IsTrue(loadResult, "Load should succeed");
        Assert.AreEqual(_gameState.CurrentScene, loadedGameState.CurrentScene, "Current scene should be preserved");
        Assert.AreEqual(_gameState.PlayerName, loadedGameState.PlayerName, "Player name should be preserved");
        Assert.AreEqual(_gameState.DreamweaverThread, loadedGameState.DreamweaverThread, "Dreamweaver thread should be preserved");
        Assert.AreEqual(_gameState.Shards.Count, loadedGameState.Shards.Count, "Shard count should be preserved");
        Assert.AreEqual(_gameState.SelectedDreamweaver, loadedGameState.SelectedDreamweaver, "Selected dreamweaver should be preserved");
    }

    [Test]
    public void TestLoadGameHandlesCorruptedData()
    {
        // Arrange - Create a corrupted save file
        using var file = Godot.FileAccess.Open("user://savegame.json", Godot.FileAccess.ModeFlags.Write);
        file.StoreString("invalid json content");

        // Act
        var result = _gameState.LoadGame();

        // Assert
        Assert.IsFalse(result, "Load should return false for corrupted data");
    }

    [Test]
    public void TestSaveGameUpdatesTimestamp()
    {
        // Arrange
        var initialTime = _gameState.LastSaveTime;

        // Act
        System.Threading.Thread.Sleep(10); // Small delay to ensure timestamp difference
        _gameState.SaveGame();

        // Assert
        Assert.Greater(_gameState.LastSaveTime, initialTime, "Last save time should be updated");
    }

    [Test]
    public void TestLoadGameRestoresDreamweaverScores()
    {
        // Arrange - Save the game
        _gameState.SaveGame();

        // Create new instance and load
        var loadedGameState = new GameState();
        loadedGameState.LoadGame();

        // Assert
        Assert.AreEqual(_gameState.DreamweaverScores[DreamweaverType.Light],
                       loadedGameState.DreamweaverScores[DreamweaverType.Light],
                       "Light dreamweaver score should be preserved");
        Assert.AreEqual(_gameState.DreamweaverScores[DreamweaverType.Mischief],
                       loadedGameState.DreamweaverScores[DreamweaverType.Mischief],
                       "Mischief dreamweaver score should be preserved");
        Assert.AreEqual(_gameState.DreamweaverScores[DreamweaverType.Wrath],
                       loadedGameState.DreamweaverScores[DreamweaverType.Wrath],
                       "Wrath dreamweaver score should be preserved");
    }

    [Test]
    public void TestLoadGameRestoresSceneData()
    {
        // Arrange - Save the game
        _gameState.SaveGame();

        // Create new instance and load
        var loadedGameState = new GameState();
        loadedGameState.LoadGame();

        // Assert
        Assert.AreEqual(_gameState.SceneData["progress"], loadedGameState.SceneData["progress"], "Scene progress should be preserved");
        Assert.AreEqual(_gameState.SceneData["completed_rooms"], loadedGameState.SceneData["completed_rooms"], "Completed rooms should be preserved");
    }
}