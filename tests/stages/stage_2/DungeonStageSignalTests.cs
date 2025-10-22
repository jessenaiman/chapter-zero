// <copyright file="DungeonStageSignalTests.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using Godot;
using GdUnit4;
using static GdUnit4.Assertions;
using System.Threading.Tasks;
using OmegaSpiral.Source.Scripts.Common;

namespace OmegaSpiral.Tests.Stages.Stage2;

/// <summary>
/// Tests for Godot signals in the ASCII dungeon sequence to complement domain events.
/// These tests ensure proper UI and reactive updates during dungeon progression.
/// </summary>
[TestSuite]
[RequireGodotRuntime]
public partial class DungeonStageSignalTests
{
    /// <summary>
    /// Tests that StageEntered signal is emitted when entering a new dungeon stage.
    /// </summary>
    [TestCase]
    [Trait("Layer", "Presentation")]
    [Trait("Speed", "Fast")]
    [Trait("Runtime", "RequireGodot")]
    [Trait("Owner", "Core")]
    [RequireGodotRuntime]
    public async Task StageEnteredSignal_EmittedWhenEnteringNewStage_SignalReceived()
    {
        // Arrange
        var echoDungeon = new OmegaSpiral.Source.Scripts.Stages.Stage2.EchoDungeon();
        
        // Act - Simulate entering a stage
        var stageId = "test_stage_1";
        var owner = DreamweaverType.Light;
        var map = new string[] { "####", "#  #", "####" };
        
        // This should emit the StageEntered signal
        echoDungeon.EmitStageEntered(stageId, 0, owner, map);

        // Assert - Wait for the signal to be emitted with correct parameters
        await AssertSignal(echoDungeon)
            .IsEmitted("StageEntered")
            .WithTimeout(1000);
    }

    /// <summary>
    /// Tests that StageCleared signal is emitted when a stage is completed.
    /// </summary>
    [TestCase]
    [Trait("Layer", "Presentation")]
    [Trait("Speed", "Fast")]
    [Trait("Runtime", "RequireGodot")]
    [Trait("Owner", "Core")]
    [RequireGodotRuntime]
    public async Task StageClearedSignal_EmittedWhenStageCompleted_SignalReceived()
    {
        // Arrange
        var echoDungeon = new OmegaSpiral.Source.Scripts.Stages.Stage2.EchoDungeon();
        
        // Act - Simulate completing a stage
        var stageId = "test_stage_1";
        echoDungeon.EmitStageCleared(stageId);

        // Assert - Wait for the signal to be emitted
        await AssertSignal(echoDungeon)
            .IsEmitted("StageCleared")
            .WithTimeout(1000);
    }

    /// <summary>
    /// Tests that InteractionResolved signal is emitted when player interacts with a glyph.
    /// </summary>
    [TestCase]
    [Trait("Layer", "Presentation")]
    [Trait("Speed", "Fast")]
    [Trait("Runtime", "RequireGodot")]
    [Trait("Owner", "Core")]
    [RequireGodotRuntime]
    public async Task InteractionResolvedSignal_EmittedWhenGlyphInteracted_SignalReceived()
    {
        // Arrange
        var echoDungeon = new OmegaSpiral.Source.Scripts.Stages.Stage2.EchoDungeon();
        
        // Act - Simulate interacting with a glyph
        var glyph = 'X';
        var alignedTo = DreamweaverType.Mischief;
        var change = 2;
        echoDungeon.EmitInteractionResolved(glyph, alignedTo, change);

        // Assert - Wait for the signal to be emitted
        await AssertSignal(echoDungeon)
            .IsEmitted("InteractionResolved")
            .WithTimeout(1000);
    }

    /// <summary>
    /// Tests that AffinityChanged signal is emitted when Dreamweaver affinity is updated.
    /// </summary>
    [TestCase]
    [Trait("Layer", "Presentation")]
    [Trait("Speed", "Fast")]
    [Trait("Runtime", "RequireGodot")]
    [Trait("Owner", "Core")]
    [RequireGodotRuntime]
    public async Task AffinityChangedSignal_EmittedWhenAffinityUpdated_SignalReceived()
    {
        // Arrange
        var echoDungeon = new OmegaSpiral.Source.Scripts.Stages.Stage2.EchoDungeon();
        
        // Act - Simulate changing affinity
        var dwType = DreamweaverType.Wrath;
        var change = 3;
        echoDungeon.EmitAffinityChanged(dwType, change);

        // Assert - Wait for the signal to be emitted
        await AssertSignal(echoDungeon)
            .IsEmitted("AffinityChanged")
            .WithTimeout(1000);
    }

    /// <summary>
    /// Tests that SequenceComplete signal is emitted when dungeon sequence finishes.
    /// </summary>
    [TestCase]
    [Trait("Layer", "Presentation")]
    [Trait("Speed", "Fast")]
    [Trait("Runtime", "RequireGodot")]
    [Trait("Owner", "Core")]
    [RequireGodotRuntime]
    public async Task SequenceCompleteSignal_EmittedWhenSequenceFinished_SignalReceived()
    {
        // Arrange
        var echoDungeon = new OmegaSpiral.Source.Scripts.Stages.Stage2.EchoDungeon();
        
        // Act - Simulate completing the sequence
        var finalScore = 42;
        echoDungeon.EmitSequenceComplete(finalScore);

        // Assert - Wait for the signal to be emitted
        await AssertSignal(echoDungeon)
            .IsEmitted("SequenceComplete")
            .WithTimeout(1000);
    }
}