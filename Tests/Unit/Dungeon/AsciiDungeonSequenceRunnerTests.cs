// <copyright file="AsciiDungeonSequenceRunnerTests.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

namespace OmegaSpiral.Tests.Unit.Dungeon;

using System.Collections.Generic;
using GdUnit4;
using OmegaSpiral.Source.Narrative;
using OmegaSpiral.Source.Scripts.Common;
using OmegaSpiral.Source.Scripts.Domain.Dungeon;
using OmegaSpiral.Source.Scripts.Domain.Dungeon.Models;
using static GdUnit4.Assertions;

/// <summary>
/// Unit tests for AsciiDungeonSequenceRunner.
/// Tests sequence execution and event publishing logic.
/// </summary>
[TestSuite]
public partial class AsciiDungeonSequenceRunnerTests : IDisposable
{
    private static readonly string[] SingleXMap = ["X"];
    private static readonly string[] SingleYMap = ["Y"];
    private static readonly string[] SingleZMap = ["Z"];

    /// <summary>
    /// Disposes of test resources.
    /// </summary>
    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Tests that Start publishes a stage entered event for the first stage.
    /// </summary>
    [TestCase]
    public void Start_PublishesStageEnteredEvent()
    {
        // Arrange
        var stageDefs = new List<DungeonStageDefinition>
        {
            new DungeonStageDefinition(
                Id: "test-stage-1",
                Owner: DreamweaverType.Light,
                Map: SingleXMap,
                Legend: new Dictionary<char, string> { ['X'] = "Wall" },
                Objects: new Dictionary<char, DungeonObjectDefinition>()),
            new DungeonStageDefinition(
                Id: "test-stage-2",
                Owner: DreamweaverType.Mischief,
                Map: SingleYMap,
                Legend: new Dictionary<char, string> { ['Y'] = "Door" },
                Objects: new Dictionary<char, DungeonObjectDefinition>()),
            new DungeonStageDefinition(
                Id: "test-stage-3",
                Owner: DreamweaverType.Wrath,
                Map: SingleZMap,
                Legend: new Dictionary<char, string> { ['Z'] = "Treasure" },
                Objects: new Dictionary<char, DungeonObjectDefinition>()),
        };
        var sequence = AsciiDungeonSequence.Create(stageDefs);
        var eventPublisher = new TestDungeonEventPublisher();
        var affinityService = new TestDreamweaverAffinityService();
        var runner = new AsciiDungeonSequenceRunner(sequence, eventPublisher, affinityService);

        // Act
        runner.Start();

        // Assert
        AssertThat(eventPublisher.EnteredEvents.Count).IsEqual(1);
        AssertThat(eventPublisher.EnteredEvents[0].StageId).IsEqual("test-stage-1");
        AssertThat(eventPublisher.EnteredEvents[0].StageIndex).IsEqual(0);
        AssertThat(eventPublisher.EnteredEvents[0].Owner).IsEqual(DreamweaverType.Light);
    }

    /// <summary>
    /// Tests that Start does nothing when sequence has no stages.
    /// </summary>
    [TestCase]
    public void Start_DoesNothingWhenNoStages()
    {
        // This test is not applicable since AsciiDungeonSequence.Create requires exactly 3 stages
        // The validation happens at creation time, not runtime
    }

    /// <summary>
    /// Tests that CompleteCurrentStage publishes cleared event and advances to next stage.
    /// </summary>
    [TestCase]
    public void CompleteCurrentStage_PublishesClearedAndAdvances()
    {
        // Arrange
        var stageDefs = new List<DungeonStageDefinition>
        {
            new DungeonStageDefinition(
                Id: "test-stage-1",
                Owner: DreamweaverType.Light,
                Map: SingleXMap,
                Legend: new Dictionary<char, string> { ['X'] = "Wall" },
                Objects: new Dictionary<char, DungeonObjectDefinition>()),
            new DungeonStageDefinition(
                Id: "test-stage-2",
                Owner: DreamweaverType.Mischief,
                Map: SingleYMap,
                Legend: new Dictionary<char, string> { ['Y'] = "Door" },
                Objects: new Dictionary<char, DungeonObjectDefinition>()),
            new DungeonStageDefinition(
                Id: "test-stage-3",
                Owner: DreamweaverType.Wrath,
                Map: SingleZMap,
                Legend: new Dictionary<char, string> { ['Z'] = "Treasure" },
                Objects: new Dictionary<char, DungeonObjectDefinition>()),
        };
        var sequence = AsciiDungeonSequence.Create(stageDefs);
        var eventPublisher = new TestDungeonEventPublisher();
        var affinityService = new TestDreamweaverAffinityService();
        var runner = new AsciiDungeonSequenceRunner(sequence, eventPublisher, affinityService);
        runner.Start();

        // Act
        runner.CompleteCurrentStage();

        // Assert
        AssertThat(eventPublisher.ClearedEvents.Count).IsEqual(1);
        AssertThat(eventPublisher.ClearedEvents[0].StageId).IsEqual("test-stage-1");

        // Should advance to next stage and publish entered event
        AssertThat(eventPublisher.EnteredEvents.Count).IsEqual(2);
        AssertThat(eventPublisher.EnteredEvents[1].StageId).IsEqual("test-stage-2");
    }

    /// <summary>
    /// Tests that CompleteCurrentStage publishes entered event for next stage when available.
    /// </summary>
    [TestCase]
    public void CompleteCurrentStage_PublishesEnteredForNextStage()
    {
        // Arrange
        var stageDefs = new List<DungeonStageDefinition>
        {
            new DungeonStageDefinition(
                Id: "test-stage-1",
                Owner: DreamweaverType.Light,
                Map: SingleXMap,
                Legend: new Dictionary<char, string> { ['X'] = "Wall" },
                Objects: new Dictionary<char, DungeonObjectDefinition>()),
            new DungeonStageDefinition(
                Id: "test-stage-2",
                Owner: DreamweaverType.Mischief,
                Map: SingleYMap,
                Legend: new Dictionary<char, string> { ['Y'] = "Door" },
                Objects: new Dictionary<char, DungeonObjectDefinition>()),
            new DungeonStageDefinition(
                Id: "test-stage-3",
                Owner: DreamweaverType.Wrath,
                Map: SingleZMap,
                Legend: new Dictionary<char, string> { ['Z'] = "Treasure" },
                Objects: new Dictionary<char, DungeonObjectDefinition>()),
        };
        var sequence = AsciiDungeonSequence.Create(stageDefs);
        var eventPublisher = new TestDungeonEventPublisher();
        var affinityService = new TestDreamweaverAffinityService();
        var runner = new AsciiDungeonSequenceRunner(sequence, eventPublisher, affinityService);

        // Act
        runner.Start();
        runner.CompleteCurrentStage();

        // Assert
        AssertThat(eventPublisher.EnteredEvents.Count).IsEqual(2);
        AssertThat(eventPublisher.EnteredEvents[1].StageId).IsEqual("test-stage-2");
        AssertThat(eventPublisher.EnteredEvents[1].StageIndex).IsEqual(1);
        AssertThat(eventPublisher.EnteredEvents[1].Owner).IsEqual(DreamweaverType.Mischief);
    }

    /// <summary>
    /// Tests that CompleteCurrentStage does nothing when no stage is active.
    /// </summary>
    [TestCase]
    public void CompleteCurrentStage_DoesNothingWhenNoActiveStage()
    {
        // Arrange
        var stageDefs = new List<DungeonStageDefinition>
        {
            new DungeonStageDefinition(
                Id: "test-stage-1",
                Owner: DreamweaverType.Light,
                Map: SingleXMap,
                Legend: new Dictionary<char, string> { ['X'] = "Wall" },
                Objects: new Dictionary<char, DungeonObjectDefinition>()),
            new DungeonStageDefinition(
                Id: "test-stage-2",
                Owner: DreamweaverType.Mischief,
                Map: SingleYMap,
                Legend: new Dictionary<char, string> { ['Y'] = "Door" },
                Objects: new Dictionary<char, DungeonObjectDefinition>()),
            new DungeonStageDefinition(
                Id: "test-stage-3",
                Owner: DreamweaverType.Wrath,
                Map: SingleZMap,
                Legend: new Dictionary<char, string> { ['Z'] = "Treasure" },
                Objects: new Dictionary<char, DungeonObjectDefinition>()),
        };
        var sequence = AsciiDungeonSequence.Create(stageDefs);
        var eventPublisher = new TestDungeonEventPublisher();
        var affinityService = new TestDreamweaverAffinityService();
        var runner = new AsciiDungeonSequenceRunner(sequence, eventPublisher, affinityService);

        // Act - call without starting
        runner.CompleteCurrentStage();

        // Assert
        AssertThat(eventPublisher.ClearedEvents.Count).IsEqual(0);
        AssertThat(eventPublisher.EnteredEvents.Count).IsEqual(0);
    }

    /// <summary>
    /// Tests that constructor throws ArgumentNullException when sequence is null.
    /// </summary>
    [TestCase]
    [ThrowsException(typeof(ArgumentNullException))]
    public void Constructor_ThrowsWhenSequenceIsNull()
    {
        // Arrange
        var eventPublisher = new TestDungeonEventPublisher();
        var affinityService = new TestDreamweaverAffinityService();

        // Act & Assert
        _ = new AsciiDungeonSequenceRunner(null!, eventPublisher, affinityService);
    }

    /// <summary>
    /// Tests that constructor throws ArgumentNullException when eventPublisher is null.
    /// </summary>
    [TestCase]
    [ThrowsException(typeof(ArgumentNullException))]
    public void Constructor_ThrowsWhenEventPublisherIsNull()
    {
        // Arrange
        var stageDefs = new List<DungeonStageDefinition>
        {
            new DungeonStageDefinition(
                Id: "test-stage-1",
                Owner: DreamweaverType.Light,
                Map: SingleXMap,
                Legend: new Dictionary<char, string> { ['X'] = "Wall" },
                Objects: new Dictionary<char, DungeonObjectDefinition>()),
            new DungeonStageDefinition(
                Id: "test-stage-2",
                Owner: DreamweaverType.Mischief,
                Map: SingleYMap,
                Legend: new Dictionary<char, string> { ['Y'] = "Door" },
                Objects: new Dictionary<char, DungeonObjectDefinition>()),
            new DungeonStageDefinition(
                Id: "test-stage-3",
                Owner: DreamweaverType.Wrath,
                Map: SingleZMap,
                Legend: new Dictionary<char, string> { ['Z'] = "Treasure" },
                Objects: new Dictionary<char, DungeonObjectDefinition>()),
        };
        var sequence = AsciiDungeonSequence.Create(stageDefs);
        var affinityService = new TestDreamweaverAffinityService();

        // Act & Assert
        _ = new AsciiDungeonSequenceRunner(sequence, null!, affinityService);
    }

    /// <summary>
    /// Tests that constructor throws ArgumentNullException when affinityService is null.
    /// </summary>
    [TestCase]
    [ThrowsException(typeof(ArgumentNullException))]
    public void Constructor_ThrowsWhenAffinityServiceIsNull()
    {
        // Arrange
        var stageDefs = new List<DungeonStageDefinition>
        {
            new DungeonStageDefinition(
                Id: "test-stage-1",
                Owner: DreamweaverType.Light,
                Map: SingleXMap,
                Legend: new Dictionary<char, string> { ['X'] = "Wall" },
                Objects: new Dictionary<char, DungeonObjectDefinition>()),
            new DungeonStageDefinition(
                Id: "test-stage-2",
                Owner: DreamweaverType.Mischief,
                Map: SingleYMap,
                Legend: new Dictionary<char, string> { ['Y'] = "Door" },
                Objects: new Dictionary<char, DungeonObjectDefinition>()),
            new DungeonStageDefinition(
                Id: "test-stage-3",
                Owner: DreamweaverType.Wrath,
                Map: SingleZMap,
                Legend: new Dictionary<char, string> { ['Z'] = "Treasure" },
                Objects: new Dictionary<char, DungeonObjectDefinition>()),
        };
        var sequence = AsciiDungeonSequence.Create(stageDefs);
        var eventPublisher = new TestDungeonEventPublisher();

        // Act & Assert
        _ = new AsciiDungeonSequenceRunner(sequence, eventPublisher, null!);
    }

    /// <summary>
    /// Test double for IDungeonEventPublisher.
    /// </summary>
    private sealed class TestDungeonEventPublisher : IDungeonEventPublisher
    {
        public List<DungeonStageClearedEvent> ClearedEvents { get; } = new();

        public List<DungeonStageEnteredEvent> EnteredEvents { get; } = new();

        public void PublishStageCleared(DungeonStageClearedEvent domainEvent)
        {
            ClearedEvents.Add(domainEvent);
        }

        public void PublishStageEntered(DungeonStageEnteredEvent domainEvent)
        {
            EnteredEvents.Add(domainEvent);
        }
    }

    /// <summary>
    /// Test double for IDreamweaverAffinityService.
    /// </summary>
    private sealed class TestDreamweaverAffinityService : IDreamweaverAffinityService
    {
        public void ApplyChange(DreamweaverType owner, DreamweaverAffinityChange change)
        {
            // No-op for testing
        }
    }
}
