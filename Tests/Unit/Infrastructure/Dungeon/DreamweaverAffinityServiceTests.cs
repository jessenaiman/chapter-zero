using GdUnit4;
using OmegaSpiral.Source.Scripts.Common;
using OmegaSpiral.Source.Scripts.Domain.Dungeon;
using OmegaSpiral.Source.Scripts.Domain.Dungeon.Models;
using OmegaSpiral.Source.Scripts.Infrastructure.Dungeon;
using static GdUnit4.Assertions;

namespace OmegaSpiral.Tests.Unit.Infrastructure.Dungeon;

/// <summary>
/// Unit tests for DreamweaverAffinityService.
/// Tests the application of affinity changes to GameState.
/// </summary>
[TestSuite]
public partial class DreamweaverAffinityServiceTests : IDisposable
{
    private GameState? _gameState;
    private DreamweaverAffinityService? _service;

    /// <summary>
    /// Sets up the test environment before each test.
    /// </summary>
    [Before]
    public void Setup()
    {
        _gameState = new GameState();
        _service = new DreamweaverAffinityService(_gameState);
    }

    /// <summary>
    /// Cleans up the test environment after each test.
    /// </summary>
    [After]
    public void TearDown()
    {
        _service = null;
        _gameState = null;
    }

    /// <summary>
    /// Disposes of the test resources.
    /// </summary>
    public void Dispose()
    {
        _service = null;
        _gameState = null;
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Tests that ApplyChange adds positive points to the specified Dreamweaver.
    /// </summary>
    [TestCase]
    public void ApplyChange_AddsPositivePoints()
    {
        // Arrange
        var change = new DreamweaverAffinityChange(DreamweaverAffinityChangeType.Increase, 5);

        // Act
        _service!.ApplyChange(DreamweaverType.Light, change);

        // Assert
        AssertThat(_gameState!.DreamweaverScores[DreamweaverType.Light]).IsEqual(5);
    }

    /// <summary>
    /// Tests that ApplyChange subtracts points for decrease changes.
    /// </summary>
    [TestCase]
    public void ApplyChange_SubtractsPointsForDecrease()
    {
        // Arrange
        _gameState!.UpdateDreamweaverScore(DreamweaverType.Mischief, 10);
        var change = new DreamweaverAffinityChange(DreamweaverAffinityChangeType.Decrease, 3);

        // Act
        _service!.ApplyChange(DreamweaverType.Mischief, change);

        // Assert
        AssertThat(_gameState.DreamweaverScores[DreamweaverType.Mischief]).IsEqual(7);
    }

    /// <summary>
    /// Tests that ApplyChange works for all Dreamweaver types.
    /// </summary>
    [TestCase]
    public void ApplyChange_WorksForAllDreamweaverTypes()
    {
        // Arrange
        var lightChange = new DreamweaverAffinityChange(DreamweaverAffinityChangeType.Increase, 2);
        var mischiefChange = new DreamweaverAffinityChange(DreamweaverAffinityChangeType.Increase, 3);
        var wrathChange = new DreamweaverAffinityChange(DreamweaverAffinityChangeType.Increase, 4);

        // Act
        _service!.ApplyChange(DreamweaverType.Light, lightChange);
        _service.ApplyChange(DreamweaverType.Mischief, mischiefChange);
        _service.ApplyChange(DreamweaverType.Wrath, wrathChange);

        // Assert
        AssertThat(_gameState!.DreamweaverScores[DreamweaverType.Light]).IsEqual(2);
        AssertThat(_gameState.DreamweaverScores[DreamweaverType.Mischief]).IsEqual(3);
        AssertThat(_gameState.DreamweaverScores[DreamweaverType.Wrath]).IsEqual(4);
    }

    /// <summary>
    /// Tests that ApplyChange throws ArgumentNullException when change is null.
    /// </summary>
    [TestCase]
    public void ApplyChange_ThrowsWhenChangeIsNull()
    {
        // Act & Assert
        AssertThat(() => _service!.ApplyChange(DreamweaverType.Light, null!))
            .Throws<ArgumentNullException>();
    }

    /// <summary>
    /// Tests that constructor throws ArgumentNullException when gameState is null.
    /// </summary>
    [TestCase]
    public void Constructor_ThrowsWhenGameStateIsNull()
    {
        // Act & Assert
        AssertThat(() => new DreamweaverAffinityService(null!))
            .Throws<ArgumentNullException>();
    }

    /// <summary>
    /// Tests that ApplyChange handles zero amount changes.
    /// </summary>
    [TestCase]
    public void ApplyChange_HandlesZeroAmount()
    {
        // Arrange
        _gameState!.UpdateDreamweaverScore(DreamweaverType.Wrath, 5);
        var change = new DreamweaverAffinityChange(DreamweaverAffinityChangeType.Neutral, 0);

        // Act
        _service!.ApplyChange(DreamweaverType.Wrath, change);

        // Assert
        AssertThat(_gameState.DreamweaverScores[DreamweaverType.Wrath]).IsEqual(5);
    }
}
