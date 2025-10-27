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
    /// Verifies that the constructor properly initializes the DreamweaverScores dictionary
    /// with all three Dreamweaver types set to zero.
    /// </summary>
    /// <exception cref="ArgumentException">Thrown if DreamweaverScores is not properly initialized.</exception>
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
    /// Verifies that score updates are applied only to the targeted Dreamweaver while
    /// leaving other Dreamweaver scores unchanged.
    /// </summary>
    /// <param name="dreamweaverType">The type of Dreamweaver to update (Light, Mischief, or Wrath).</param>
    /// <exception cref="ArgumentException">Thrown if an invalid DreamweaverType is provided.</exception>
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
    /// Verifies that the method correctly processes negative values to decrease
    /// a Dreamweaver's score, ensuring proper arithmetic operations.
    /// </summary>
    /// <param name="dreamweaverType">The type of Dreamweaver to update.</param>
    /// <param name="negativePoints">The negative point value to subtract.</param>
    /// <exception cref="ArgumentException">Thrown if score would go below zero.</exception>
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
    /// Verifies that the method correctly identifies and returns the Dreamweaver type
    /// that has accumulated the most points across all score updates.
    /// </summary>
    /// <returns>The DreamweaverType with the highest score.</returns>
    /// <exception cref="InvalidOperationException">Thrown if no Dreamweaver scores exist.</exception>
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
    /// Verifies that when multiple Dreamweavers have the same highest score, the method
    /// returns one of the tied Dreamweaver types consistently.
    /// </summary>
    /// <returns>A DreamweaverType that has the highest score (may be any in case of ties).</returns>
    /// <exception cref="InvalidOperationException">Thrown if no Dreamweaver scores exist.</exception>
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
    /// Verifies that the method correctly aggregates all individual Dreamweaver scores
    /// into a single total score representing the overall game progress.
    /// </summary>
    /// <returns>The sum of all Dreamweaver scores.</returns>
    /// <exception cref="InvalidOperationException">Thrown if no Dreamweaver scores exist.</exception>
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
