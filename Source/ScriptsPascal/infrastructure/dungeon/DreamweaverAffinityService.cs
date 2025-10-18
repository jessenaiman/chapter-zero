using OmegaSpiral.Source.Scripts.Common;
using OmegaSpiral.Source.Scripts.Domain.Dungeon;
using OmegaSpiral.Source.Scripts.Domain.Dungeon.Models;

namespace OmegaSpiral.Source.Scripts.Infrastructure.Dungeon;

/// <summary>
/// Service for managing Dreamweaver affinity changes.
/// Applies affinity score changes to the global GameState.
/// </summary>
public sealed class DreamweaverAffinityService : IDreamweaverAffinityService
{
    private readonly GameState _gameState;

    /// <summary>
    /// Initializes a new instance of the <see cref="DreamweaverAffinityService"/> class.
    /// </summary>
    /// <param name="gameState">The game state to modify.</param>
    /// <exception cref="ArgumentNullException">Thrown when gameState is null.</exception>
    public DreamweaverAffinityService(GameState gameState)
    {
        ArgumentNullException.ThrowIfNull(gameState);
        _gameState = gameState;
    }

    /// <summary>
    /// Applies an affinity change to the specified Dreamweaver type.
    /// </summary>
    /// <param name="owner">The Dreamweaver type to apply the change to.</param>
    /// <param name="change">The affinity change to apply.</param>
    /// <exception cref="ArgumentNullException">Thrown when change is null.</exception>
    public void ApplyChange(DreamweaverType owner, DreamweaverAffinityChange change)
    {
        ArgumentNullException.ThrowIfNull(change);
        _gameState.UpdateDreamweaverScore(owner, change.Amount);
    }
}
