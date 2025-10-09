using NUnit.Framework;
using Godot;
using System.IO;
using OmegaSpiral.Source.Scripts;

[TestFixture]
public class StatePersistenceTests
{
    private GameState _gameState;
    private SceneManager _sceneManager;

    [SetUp]
    public void Setup()
    {
        // Create test instances
        _gameState = new GameState();
        _sceneManager = new SceneManager();

        // Initialize game state
        _gameState.PlayerName = "TestPlayer";
        _gameState.DreamweaverThread = DreamweaverThread.Light;
        _gameState.CurrentScene = 1;
        _gameState.Shards.Add("test_shard");
        _gameState.SceneData["test_key"] = "test_value";
    }

    [Test]
    public void TestStatePersistenceAcrossScenes()
    {
        // Arrange
        var initialScene = _gameState.CurrentScene;
        var initialPlayerName = _gameState.PlayerName;
        var initialThread = _gameState.DreamweaverThread;

        // Act - Simulate scene transition
        _sceneManager.UpdateCurrentScene(2);
        _sceneManager.SetPlayerName("UpdatedPlayer");
        _sceneManager.SetDreamweaverThread("Mischief");

        // Assert - State should persist
        Assert.AreEqual(2, _gameState.CurrentScene, "Current scene should be updated");
        Assert.AreEqual("UpdatedPlayer", _gameState.PlayerName, "Player name should be updated");
        Assert.AreEqual(DreamweaverThread.Mischief, _gameState.DreamweaverThread, "Dreamweaver thread should be updated");
    }

    [Test]
    public void TestShardCollectionPersistence()
    {
        // Arrange
        var initialShardCount = _gameState.Shards.Count;

        // Act
        _sceneManager.AddShard("new_shard");

        // Assert
        Assert.AreEqual(initialShardCount + 1, _gameState.Shards.Count, "Shard count should increase");
        Assert.Contains("new_shard", _gameState.Shards, "New shard should be added");
    }

    [Test]
    public void TestSceneDataPersistence()
    {
        // Arrange
        var testKey = "scene_progress";
        var testValue = 75;

        // Act
        _gameState.SceneData[testKey] = testValue;

        // Assert
        Assert.AreEqual(testValue, _gameState.SceneData[testKey], "Scene data should persist");
    }

    [Test]
    public void TestStateValidationForSceneTransitions()
    {
        // Arrange - Set invalid state for scene 2
        _gameState.CurrentScene = 1; // Not completed scene 1

        // Act & Assert
        Assert.IsFalse(_sceneManager.ValidateStateForTransition("Scene2NethackSequence"),
            "Should not allow transition to scene 2 without completing scene 1");

        // Arrange - Set valid state
        _gameState.CurrentScene = 1;

        // Act & Assert
        Assert.IsTrue(_sceneManager.ValidateStateForTransition("Scene2NethackSequence"),
            "Should allow transition to scene 2 after completing scene 1");
    }

    [Test]
    public void TestPartyDataPersistence()
    {
        // Arrange
        var partyData = new PartyData();
        var testCharacter = new Character("TestChar", CharacterRace.Human, CharacterClass.Fighter);
        partyData.Members.Add(testCharacter);

        // Act
        _gameState.PlayerParty = partyData;

        // Assert
        Assert.AreEqual(1, _gameState.PlayerParty.Members.Count, "Party should have one member");
        Assert.AreEqual("TestChar", _gameState.PlayerParty.Members[0].Name, "Character name should match");
    }
}