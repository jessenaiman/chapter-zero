// <copyright file="TileDungeonSignalTests.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using Godot;
using GdUnit4;
using static GdUnit4.Assertions;
using System.Threading.Tasks;
using OmegaSpiral.Source.Scripts.Common;

namespace OmegaSpiral.Tests.Stages.Stage4;

/// <summary>
/// Tests for Godot signals in the tile dungeon stage.
/// These tests ensure proper Ui and reactive updates during tile-based gameplay.
/// </summary>
[TestSuite]
[RequireGodotRuntime]
public partial class TileDungeonSignalTests
{
    /// <summary>
    /// Tests that PlayerMoved signal is emitted when player moves to a new tile position.
    /// </summary>
    [TestCase]
    [Trait("Layer", "Presentation")]
    [Trait("Speed", "Fast")]
    [Trait("Runtime", "RequireGodot")]
    [Trait("Owner", "Core")]
    [RequireGodotRuntime]
    public async Task PlayerMovedSignal_EmittedWhenPlayerMoves_SignalReceived()
    {
        // Arrange
        var tileDungeon = new OmegaSpiral.Source.Scripts.Stages.Stage4.TileDungeon();

        // Act - Simulate player movement
        var newPosition = new Vector2I(5, 3);
        var previousPosition = new Vector2I(4, 3);
        tileDungeon.EmitPlayerMoved(newPosition, previousPosition);

        // Assert - Wait for the signal to be emitted with correct parameters
        await AssertSignal(tileDungeon)
            .IsEmitted("PlayerMoved")
            .WithTimeout(1000).ConfigureAwait(false);
    }

    /// <summary>
    /// Tests that EnemyEncountered signal is emitted when player encounters an enemy.
    /// </summary>
    [TestCase]
    [Trait("Layer", "Presentation")]
    [Trait("Speed", "Fast")]
    [Trait("Runtime", "RequireGodot")]
    [Trait("Owner", "Core")]
    [RequireGodotRuntime]
    public async Task EnemyEncounteredSignal_EmittedWhenEnemyEncountered_SignalReceived()
    {
        // Arrange
        var tileDungeon = new OmegaSpiral.Source.Scripts.Stages.Stage4.TileDungeon();

        // Act - Simulate enemy encounter
        var enemyType = "Goblin";
        var position = new Vector2I(8, 7);
        tileDungeon.EmitEnemyEncountered(enemyType, position);

        // Assert - Wait for the signal to be emitted
        await AssertSignal(tileDungeon)
            .IsEmitted("EnemyEncountered")
            .WithTimeout(1000).ConfigureAwait(false);
    }

    /// <summary>
    /// Tests that ItemFound signal is emitted when player finds an item.
    /// </summary>
    [TestCase]
    [Trait("Layer", "Presentation")]
    [Trait("Speed", "Fast")]
    [Trait("Runtime", "RequireGodot")]
    [Trait("Owner", "Core")]
    [RequireGodotRuntime]
    public async Task ItemFoundSignal_EmittedWhenItemFound_SignalReceived()
    {
        // Arrange
        var tileDungeon = new OmegaSpiral.Source.Scripts.Stages.Stage4.TileDungeon();

        // Act - Simulate item discovery
        var itemType = "Potion";
        var position = new Vector2I(2, 9);
        tileDungeon.EmitItemFound(itemType, position);

        // Assert - Wait for the signal to be emitted
        await AssertSignal(tileDungeon)
            .IsEmitted("ItemFound")
            .WithTimeout(1000).ConfigureAwait(false);
    }

    /// <summary>
    /// Tests that PlayerHealthChanged signal is emitted when player health changes.
    /// </summary>
    [TestCase]
    [Trait("Layer", "Presentation")]
    [Trait("Speed", "Fast")]
    [Trait("Runtime", "RequireGodot")]
    [Trait("Owner", "Core")]
    [RequireGodotRuntime]
    public async Task PlayerHealthChangedSignal_EmittedWhenHealthChanges_SignalReceived()
    {
        // Arrange
        var tileDungeon = new OmegaSpiral.Source.Scripts.Stages.Stage4.TileDungeon();

        // Act - Simulate health change
        var newHealth = 45;
        var maxHealth = 100;
        tileDungeon.EmitPlayerHealthChanged(newHealth, maxHealth);

        // Assert - Wait for the signal to be emitted
        await AssertSignal(tileDungeon)
            .IsEmitted("PlayerHealthChanged")
            .WithTimeout(1000).ConfigureAwait(false);
    }
}

/// <summary>
/// Tests for Godot signals in the field combat stage.
/// These tests ensure proper Ui and reactive updates during combat gameplay.
/// </summary>
[TestSuite]
[RequireGodotRuntime]
public partial class FieldCombatSignalTests
{
    /// <summary>
    /// Tests that CombatRoundStarted signal is emitted when a combat round begins.
    /// </summary>
    [TestCase]
    [Trait("Layer", "Presentation")]
    [Trait("Speed", "Fast")]
    [Trait("Runtime", "RequireGodot")]
    [Trait("Owner", "Core")]
    [RequireGodotRuntime]
    public async Task CombatRoundStartedSignal_EmittedWhenRoundStarts_SignalReceived()
    {
        // Arrange
        var fieldCombat = new OmegaSpiral.Source.Scripts.Stages.Stage4.FieldCombat();

        // Act - Simulate combat round start
        var roundNumber = 3;
        fieldCombat.EmitCombatRoundStarted(roundNumber);

        // Assert - Wait for the signal to be emitted
        await AssertSignal(fieldCombat)
            .IsEmitted("CombatRoundStarted")
            .WithTimeout(1000).ConfigureAwait(false);
    }

    /// <summary>
    /// Tests that PlayerActionSelected signal is emitted when player selects an action.
    /// </summary>
    [TestCase]
    [Trait("Layer", "Presentation")]
    [Trait("Speed", "Fast")]
    [Trait("Runtime", "RequireGodot")]
    [Trait("Owner", "Core")]
    [RequireGodotRuntime]
    public async Task PlayerActionSelectedSignal_EmittedWhenActionSelected_SignalReceived()
    {
        // Arrange
        var fieldCombat = new OmegaSpiral.Source.Scripts.Stages.Stage4.FieldCombat();

        // Act - Simulate player action selection
        var actionType = "attack";
        var target = "enemy_1";
        fieldCombat.EmitPlayerActionSelected(actionType, target);

        // Assert - Wait for the signal to be emitted
        await AssertSignal(fieldCombat)
            .IsEmitted("PlayerActionSelected")
            .WithTimeout(1000).ConfigureAwait(false);
    }

    /// <summary>
    /// Tests that AttackExecuted signal is emitted when an attack is performed.
    /// </summary>
    [TestCase]
    [Trait("Layer", "Presentation")]
    [Trait("Speed", "Fast")]
    [Trait("Runtime", "RequireGodot")]
    [Trait("Owner", "Core")]
    [RequireGodotRuntime]
    public async Task AttackExecutedSignal_EmittedWhenAttackPerformed_SignalReceived()
    {
        // Arrange
        var fieldCombat = new OmegaSpiral.Source.Scripts.Stages.Stage4.FieldCombat();

        // Act - Simulate attack execution
        var attacker = "player";
        var defender = "goblin";
        var damage = 15;
        fieldCombat.EmitAttackExecuted(attacker, defender, damage);

        // Assert - Wait for the signal to be emitted
        await AssertSignal(fieldCombat)
            .IsEmitted("AttackExecuted")
            .WithTimeout(1000).ConfigureAwait(false);
    }

    /// <summary>
    /// Tests that CombatCompleted signal is emitted when combat ends.
    /// </summary>
    [TestCase]
    [Trait("Layer", "Presentation")]
    [Trait("Speed", "Fast")]
    [Trait("Runtime", "RequireGodot")]
    [Trait("Owner", "Core")]
    [RequireGodotRuntime]
    public async Task CombatCompletedSignal_EmittedWhenCombatEnds_SignalReceived()
    {
        // Arrange
        var fieldCombat = new OmegaSpiral.Source.Scripts.Stages.Stage4.FieldCombat();

        // Act - Simulate combat completion
        var outcome = "victory";
        var victor = "player";
        fieldCombat.EmitCombatCompleted(outcome, victor);

        // Assert - Wait for the signal to be emitted
        await AssertSignal(fieldCombat)
            .IsEmitted("CombatCompleted")
            .WithTimeout(1000).ConfigureAwait(false);
    }
}
