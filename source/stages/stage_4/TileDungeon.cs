using Godot;
using OmegaSpiral.Source.Scripts.Common;

namespace OmegaSpiral.Source.Scripts.Stages.Stage4;

/// <summary>
/// Handles tile-based dungeon exploration gameplay.
/// Manages player movement, interactions, and dungeon progression.
/// </summary>
[GlobalClass]
public partial class TileDungeon : Control
{
    /// <summary>
    /// Emitted when player moves to a new tile position.
    /// </summary>
    /// <param name="x">The X coordinate of the new position.</param>
    /// <param name="y">The Y coordinate of the new position.</param>
    /// <param name="prevX">The X coordinate of the previous position.</param>
    /// <param name="prevY">The Y coordinate of the previous position.</param>
    [Signal]
    public delegate void PlayerMovedEventHandler(int x, int y, int prevX, int prevY);

    /// <summary>
    /// Emitted when player encounters an enemy.
    /// </summary>
    /// <param name="enemyType">The type of enemy encountered.</param>
    /// <param name="x">The X coordinate where the encounter occurred.</param>
    /// <param name="y">The Y coordinate where the encounter occurred.</param>
    [Signal]
    public delegate void EnemyEncounteredEventHandler(string enemyType, int x, int y);

    /// <summary>
    /// Emitted when combat is initiated.
    /// </summary>
    /// <param name="enemyType">The type of enemy initiating combat.</param>
    /// <param name="encounterId">The unique identifier for this encounter.</param>
    [Signal]
    public delegate void CombatInitiatedEventHandler(string enemyType, string encounterId);

    /// <summary>
    /// Emitted when player finds an item in the dungeon.
    /// </summary>
    /// <param name="itemType">The type of item found.</param>
    /// <param name="x">The X coordinate where the item was found.</param>
    /// <param name="y">The Y coordinate where the item was found.</param>
    [Signal]
    public delegate void ItemFoundEventHandler(string itemType, int x, int y);

    /// <summary>
    /// Emitted when a door is opened or path is unlocked.
    /// </summary>
    /// <param name="pathId">The identifier of the path or door.</param>
    [Signal]
    public delegate void PathUnlockedEventHandler(string pathId);

    /// <summary>
    /// Emitted when player health changes.
    /// </summary>
    /// <param name="newHealth">The new health value.</param>
    /// <param name="maxHealth">The maximum health value.</param>
    [Signal]
    public delegate void PlayerHealthChangedEventHandler(int newHealth, int maxHealth);

    /// <summary>
    /// Emitted when a level or dungeon section is completed.
    /// </summary>
    /// <param name="completionData">Data about the completed level.</param>
    [Signal]
    public delegate void LevelCompletedEventHandler(Godot.Collections.Dictionary<string, Variant> completionData);

    /// <inheritdoc/>
    public override void _Ready()
    {
        // Initialize tile dungeon logic
        GD.Print("Tile Dungeon initialized");
    }

    /// <summary>
    /// Emits the PlayerMoved signal.
    /// </summary>
    /// <param name="newPosition">The new grid position of the player.</param>
    /// <param name="previousPosition">The previous grid position of the player.</param>
    public void EmitPlayerMoved(Vector2I newPosition, Vector2I previousPosition)
    {
        this.EmitSignal(SignalName.PlayerMoved, newPosition.X, newPosition.Y, previousPosition.X, previousPosition.Y);
    }

    /// <summary>
    /// Emits the EnemyEncountered signal.
    /// </summary>
    /// <param name="enemyType">The type of enemy encountered.</param>
    /// <param name="position">The position where the encounter occurred.</param>
    public void EmitEnemyEncountered(string enemyType, Vector2I position)
    {
        this.EmitSignal(SignalName.EnemyEncountered, enemyType, position.X, position.Y);
    }

    /// <summary>
    /// Emits the CombatInitiated signal.
    /// </summary>
    /// <param name="enemyType">The type of enemy initiating combat.</param>
    /// <param name="encounterId">The unique identifier for this encounter.</param>
    public void EmitCombatInitiated(string enemyType, string encounterId)
    {
        this.EmitSignal(SignalName.CombatInitiated, enemyType, encounterId);
    }

    /// <summary>
    /// Emits the ItemFound signal.
    /// </summary>
    /// <param name="itemType">The type of item found.</param>
    /// <param name="position">The position where the item was found.</param>
    public void EmitItemFound(string itemType, Vector2I position)
    {
        this.EmitSignal(SignalName.ItemFound, itemType, position.X, position.Y);
    }

    /// <summary>
    /// Emits the PathUnlocked signal.
    /// </summary>
    /// <param name="pathId">The identifier of the path or door.</param>
    public void EmitPathUnlocked(string pathId)
    {
        this.EmitSignal(SignalName.PathUnlocked, pathId);
    }

    /// <summary>
    /// Emits the PlayerHealthChanged signal.
    /// </summary>
    /// <param name="newHealth">The new health value.</param>
    /// <param name="maxHealth">The maximum health value.</param>
    public void EmitPlayerHealthChanged(int newHealth, int maxHealth)
    {
        this.EmitSignal(SignalName.PlayerHealthChanged, newHealth, maxHealth);
    }

    /// <summary>
    /// Emits the LevelCompleted signal.
    /// </summary>
    /// <param name="completionData">Data about the completed level.</param>
    public void EmitLevelCompleted(Godot.Collections.Dictionary<string, Variant> completionData)
    {
        this.EmitSignal(SignalName.LevelCompleted, completionData);
    }
}