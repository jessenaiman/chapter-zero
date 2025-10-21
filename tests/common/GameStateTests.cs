// <copyright file="GameStateTests.cs" company="Omega Spiral">
// Copyright (c) Omega Spiral. All rights reserved.
// </copyright>

namespace OmegaSpiral.Tests.Unit.Common;

using GdUnit4;
using OmegaSpiral.Source.Scripts.Common;
using static GdUnit4.Assertions;

/// <summary>
/// Unit tests for the GameState class.
/// Tests cover Dreamweaver score management, party state, and game progression.
/// </summary>
[TestSuite]
public class GameStateTests
{
    /// <summary>
    /// Tests that a new GameState initializes with zero scores for all Dreamweavers.
    /// </summary>
    [TestCase]
    [RequireGodotRuntime]
    public void ConstructorInitializeswithzeroscores()
    {
        // Arrange & Act
        var gameState = new GameState();

        // Assert
        AssertThat(gameState.DreamweaverScores).HasSize(3);
        AssertThat(gameState.DreamweaverScores[DreamweaverType.Light]).IsEqual(0);
        AssertThat(gameState.DreamweaverScores[DreamweaverType.Mischief]).IsEqual(0);
        AssertThat(gameState.DreamweaverScores[DreamweaverType.Wrath]).IsEqual(0);
    }

    /// <summary>
    /// Tests that UpdateDreamweaverScore correctly adds points to the specified Dreamweaver.
    /// </summary>
    [TestCase]
    [RequireGodotRuntime]
    public void UpdatedreamweaverscoreAddspointscorrectly()
    {
        // Arrange
        var gameState = new GameState();

        // Act
        gameState.UpdateDreamweaverScore(DreamweaverType.Light, 5);

        // Assert
        AssertThat(gameState.DreamweaverScores[DreamweaverType.Light]).IsEqual(5);
        AssertThat(gameState.DreamweaverScores[DreamweaverType.Mischief]).IsEqual(0);
        AssertThat(gameState.DreamweaverScores[DreamweaverType.Wrath]).IsEqual(0);
    }

    /// <summary>
    /// Tests that UpdateDreamweaverScore can handle negative score changes.
    /// </summary>
    [TestCase]
    [RequireGodotRuntime]
    public void UpdatedreamweaverscoreHandlesnegativechanges()
    {
        // Arrange
        var gameState = new GameState();
        gameState.UpdateDreamweaverScore(DreamweaverType.Light, 10);

        // Act
        gameState.UpdateDreamweaverScore(DreamweaverType.Light, -3);

        // Assert
        AssertThat(gameState.DreamweaverScores[DreamweaverType.Light]).IsEqual(7);
    }

    /// <summary>
    /// Tests that GetHighestScoringDreamweaver returns the Dreamweaver with the highest score.
    /// </summary>
    [TestCase]
    [RequireGodotRuntime]
    public void GethighestscoringdreamweaverReturnscorrectdreamweaver()
    {
        // Arrange
        var gameState = new GameState();
        gameState.UpdateDreamweaverScore(DreamweaverType.Light, 3);
        gameState.UpdateDreamweaverScore(DreamweaverType.Mischief, 7);
        gameState.UpdateDreamweaverScore(DreamweaverType.Wrath, 5);

        // Act
        var highest = gameState.GetHighestScoringDreamweaver();

        // Assert
        AssertThat(highest).IsEqual(DreamweaverType.Mischief);
    }

    /// <summary>
    /// Tests that GetHighestScoringDreamweaver handles ties by returning one of the highest.
    /// </summary>
    [TestCase]
    [RequireGodotRuntime]
    public void GethighestscoringdreamweaverHandlesties()
    {
        // Arrange
        var gameState = new GameState();
        gameState.UpdateDreamweaverScore(DreamweaverType.Light, 5);
        gameState.UpdateDreamweaverScore(DreamweaverType.Mischief, 5);
        gameState.UpdateDreamweaverScore(DreamweaverType.Wrath, 3);

        // Act
        var highest = gameState.GetHighestScoringDreamweaver();

        // Assert
        AssertThat(highest == DreamweaverType.Light || highest == DreamweaverType.Mischief).IsTrue();
    }

    /// <summary>
    /// Tests that GetTotalScore returns the sum of all Dreamweaver scores.
    /// </summary>
    [TestCase]
    [RequireGodotRuntime]
    public void GettotalscoreReturnssumofallscores()
    {
        // Arrange
        var gameState = new GameState();
        gameState.UpdateDreamweaverScore(DreamweaverType.Light, 2);
        gameState.UpdateDreamweaverScore(DreamweaverType.Mischief, 4);
        gameState.UpdateDreamweaverScore(DreamweaverType.Wrath, 6);

        // Act
        var total = gameState.GetTotalScore();

        // Assert
        AssertThat(total).IsEqual(12);
    }
}
