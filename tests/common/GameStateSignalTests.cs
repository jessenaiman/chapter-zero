// <copyright file="GameStateSignalTests.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using Godot;
using GdUnit4;
using static GdUnit4.Assertions;
using System.Threading.Tasks;
using OmegaSpiral.Source.Scripts.Common;

namespace OmegaSpiral.Tests.Common;

/// <summary>
/// Tests for cross-stage Godot signals that manage global game state.
/// These tests ensure proper communication across different game components.
/// </summary>
[TestSuite]
[RequireGodotRuntime]
public partial class GameStateSignalTests : Node
{
    /// <summary>
    /// Tests that AffinityScoreUpdated signal is emitted when Dreamweaver affinity changes.
    /// </summary>
    [TestCase]
    [Trait("Layer", "Domain")]
    [Trait("Speed", "Fast")]
    [Trait("Runtime", "RequireGodot")]
    [Trait("Owner", "Core")]
    [RequireGodotRuntime]
    public async Task AffinityScoreUpdatedSignal_EmittedWhenAffinityChanges_SignalReceived()
    {
        // Arrange
    var gameStateSignals = AutoFree(new OmegaSpiral.Source.Scripts.GameStateSignals())!;
    AddChild(gameStateSignals);
    var signalMonitor = AssertSignal(gameStateSignals);

        // Act - Simulate affinity score update
        var dwType = DreamweaverType.Light;
        var change = 5;
        var newScore = 15;
        gameStateSignals.EmitAffinityScoreUpdated(dwType, change, newScore);

        // Assert - Wait for the signal to be emitted
        await signalMonitor
            .IsEmitted("AffinityScoreUpdated")
            .WithTimeout(1000);
    }

    /// <summary>
    /// Tests that PlayerProgressionChanged signal is emitted when milestone reached.
    /// </summary>
    [TestCase]
    [Trait("Layer", "Domain")]
    [Trait("Speed", "Fast")]
    [Trait("Runtime", "RequireGodot")]
    [Trait("Owner", "Core")]
    [RequireGodotRuntime]
    public async Task PlayerProgressionChangedSignal_EmittedWhenMilestoneReached_SignalReceived()
    {
        // Arrange
    var gameStateSignals = AutoFree(new OmegaSpiral.Source.Scripts.GameStateSignals())!;
    AddChild(gameStateSignals);
    var signalMonitor = AssertSignal(gameStateSignals);

        // Act - Simulate reaching a milestone
        var milestoneId = "stage_2_complete";
        var progressData = new Godot.Collections.Dictionary<string, Variant>
        {
            { "stage_id", "stage_2" },
            { "completion_time", 120.5f },
            { "score", 500 }
        };
        gameStateSignals.EmitPlayerProgressionChanged(milestoneId, progressData);

        // Assert - Wait for the signal to be emitted
        await signalMonitor
            .IsEmitted("PlayerProgressionChanged")
            .WithTimeout(1000);
    }

    /// <summary>
    /// Tests that InventoryUpdated signal is emitted when inventory changes.
    /// </summary>
    [TestCase]
    [Trait("Layer", "Domain")]
    [Trait("Speed", "Fast")]
    [Trait("Runtime", "RequireGodot")]
    [Trait("Owner", "Core")]
    [RequireGodotRuntime]
    public async Task InventoryUpdatedSignal_EmittedWhenInventoryChanges_SignalReceived()
    {
        // Arrange
    var gameStateSignals = AutoFree(new OmegaSpiral.Source.Scripts.GameStateSignals())!;
    AddChild(gameStateSignals);
    var signalMonitor = AssertSignal(gameStateSignals);

        // Act - Simulate inventory update
        var itemId = "health_potion";
        var action = "added";
        var quantity = 3;
        gameStateSignals.EmitInventoryUpdated(itemId, action, quantity);

        // Assert - Wait for the signal to be emitted
        await signalMonitor
            .IsEmitted("InventoryUpdated")
            .WithTimeout(1000);
    }

    /// <summary>
    /// Tests that CharacterPartyChanged signal is emitted when party composition changes.
    /// </summary>
    [TestCase]
    [Trait("Layer", "Domain")]
    [Trait("Speed", "Fast")]
    [Trait("Runtime", "RequireGodot")]
    [Trait("Owner", "Core")]
    [RequireGodotRuntime]
    public async Task CharacterPartyChangedSignal_EmittedWhenPartyChanges_SignalReceived()
    {
        // Arrange
    var gameStateSignals = AutoFree(new OmegaSpiral.Source.Scripts.GameStateSignals())!;
    AddChild(gameStateSignals);
    var signalMonitor = AssertSignal(gameStateSignals);

        // Act - Simulate party change
        var action = "added";
        var characterId = "warrior_001";
        var partyData = new Godot.Collections.Dictionary<string, Variant>
        {
            { "member_count", 3 },
            { "active_members", new Godot.Collections.Array<Variant> { "warrior_001", "mage_002", "rogue_003" } }
        };
        gameStateSignals.EmitCharacterPartyChanged(action, characterId, partyData);

        // Assert - Wait for the signal to be emitted
        await signalMonitor
            .IsEmitted("CharacterPartyChanged")
            .WithTimeout(1000);
    }

    /// <summary>
    /// Tests that DreamweaverInfluenceChanged signal is emitted when dominant Dreamweaver changes.
    /// </summary>
    [TestCase]
    [Trait("Layer", "Domain")]
    [Trait("Speed", "Fast")]
    [Trait("Runtime", "RequireGodot")]
    [Trait("Owner", "Core")]
    [RequireGodotRuntime]
    public async Task DreamweaverInfluenceChangedSignal_EmittedWhenInfluenceChanges_SignalReceived()
    {
        // Arrange
    var gameStateSignals = AutoFree(new OmegaSpiral.Source.Scripts.GameStateSignals())!;
    AddChild(gameStateSignals);
    var signalMonitor = AssertSignal(gameStateSignals);

        // Act - Simulate influence change
        var newDominant = DreamweaverType.Mischief;
        var influenceLevel = 7;
        gameStateSignals.EmitDreamweaverInfluenceChanged(newDominant, influenceLevel);

        // Assert - Wait for the signal to be emitted
        await signalMonitor
            .IsEmitted("DreamweaverInfluenceChanged")
            .WithTimeout(1000);
    }
}
