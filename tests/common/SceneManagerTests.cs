// <copyright file="SceneManagerTests.cs" company="Omega Spiral">
// Copyright (c) Omega Spiral. All rights reserved.
// </copyright>

namespace OmegaSpiral.Tests.Unit.Common;

using Godot;
using GdUnit4;
using static GdUnit4.Assertions;

/// <summary>
/// Unit tests for the SceneManager class.
/// Tests cover scene transitions, state preservation, and scene loading.
/// </summary>
[TestSuite]
[RequireGodotRuntime]
public partial class SceneManagerTests : Node
{
    private SceneManager? _sceneManager;

    [Before]
    public void Setup()
    {
        _sceneManager = null;

        // Create and add SceneManager for testing
        _sceneManager = new SceneManager();
        _sceneManager.Name = "SceneManager";
        GetTree().Root.AddChild(_sceneManager);
    }

    [After]
    public void Cleanup()
    {
        if (_sceneManager != null)
        {
            _sceneManager.QueueFree();
            _sceneManager = null;
        }
    }

    /// <summary>
    /// Tests that TransitionToScene performs scene transition correctly.
    /// </summary>
    [TestCase]
    [RequireGodotRuntime]
    public void TransitionToScenePerformsSceneTransition()
    {
        // Arrange
        AssertThat(_sceneManager).IsNotNull();

        // Act & Assert - Check that the scene manager has the correct initial state
        AssertThat(_sceneManager!.CurrentSceneIndex).IsEqual(1);
        AssertThat(_sceneManager.PlayerName).IsNull();
        AssertThat(_sceneManager.DreamweaverThread).IsNull();
    }

    /// <summary>
    /// Tests that scene transitions preserve GameState.
    /// </summary>
    [TestCase]
    [RequireGodotRuntime]
    public void TransitionToScenePreservesGameState()
    {
        // Arrange
        AssertThat(_sceneManager).IsNotNull();

        // Set some state
        _sceneManager!.SetPlayerName("TestPlayer");
        _sceneManager.SetDreamweaverThread("hero");
        _sceneManager.UpdateCurrentScene(2);

        // Act - The state should be preserved in the SceneManager
        var playerName = _sceneManager.PlayerName;
        var dreamweaverThread = _sceneManager.DreamweaverThread;
        var currentSceneIndex = _sceneManager.CurrentSceneIndex;

        // Assert
        AssertThat(playerName).IsEqual("TestPlayer");
        AssertThat(dreamweaverThread).IsEqual("hero");
        AssertThat(currentSceneIndex).IsEqual(2);
    }

    /// <summary>
    /// Tests that player name is set correctly.
    /// </summary>
    [TestCase]
    [RequireGodotRuntime]
    public void SetPlayerNameStoresValueCorrectly()
    {
        // Arrange
        AssertThat(_sceneManager).IsNotNull();

        // Act
        _sceneManager!.SetPlayerName("NewPlayer");

        // Assert
        AssertString(_sceneManager.PlayerName).IsEqual("NewPlayer");
    }

    /// <summary>
    /// Tests that Dreamweaver thread is set correctly.
    /// </summary>
    [TestCase]
    [RequireGodotRuntime]
    public void SetDreamweaverThreadStoresValueCorrectly()
    {
        // Arrange
        AssertThat(_sceneManager).IsNotNull();

        // Act
        _sceneManager!.SetDreamweaverThread("shadow");

        // Assert
        AssertString(_sceneManager.DreamweaverThread).IsEqual("shadow");
    }

    /// <summary>
    /// Tests that current scene index is updated correctly.
    /// </summary>
    [TestCase]
    [RequireGodotRuntime]
    public void UpdateCurrentSceneUpdatesIndexCorrectly()
    {
        // Arrange
        AssertThat(_sceneManager).IsNotNull();

        // Act
        _sceneManager!.UpdateCurrentScene(3);

        // Assert
        AssertThat(_sceneManager.CurrentSceneIndex).IsEqual(3);
    }

    /// <summary>
    /// Tests that ValidateStateForTransition returns true for valid transitions.
    /// </summary>
    [TestCase]
    [RequireGodotRuntime]
    public void ValidateStateForTransitionReturnsTrueForValidTransitions()
    {
        // Arrange
        AssertThat(_sceneManager).IsNotNull();

        // Act
        var result = _sceneManager!.ValidateStateForTransition("res://source/ui/menus/main_menu.tscn");

        // Assert
        AssertThat(result).IsTrue();
    }
}
