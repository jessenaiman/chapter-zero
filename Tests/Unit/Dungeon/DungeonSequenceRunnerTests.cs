// <copyright file="DungeonSequenceRunnerTests.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

namespace OmegaSpiral.Tests.Unit.Dungeon
{
    using System.Collections.Generic;
    using NUnit.Framework;
    using OmegaSpiral.Source.Scripts.Common;
    using OmegaSpiral.Source.Scripts.Domain.Dungeon;
    using OmegaSpiral.Source.Scripts.Domain.Dungeon.Models;
    using OmegaSpiral.Source.Scripts.Field.Narrative;

    /// <summary>
    /// Unit tests covering the application orchestration for the ASCII dungeon sequence.
    /// </summary>
    [TestFixture]
    public class DungeonSequenceRunnerTests
    {
        /// <summary>
        /// Ensures the runner publishes a stage-entered notification when the sequence begins.
        /// </summary>
        [Test]
        public void AsciiDungeonSequenceRunner_Start_WhenStageBegins_PublishesDungeonStageEntered()
        {
            var sequence = AsciiDungeonSequence.Create(this.CreateValidStageDefinitions());
            var publisher = new CapturingDungeonEventPublisher();
            var affinity = new CapturingAffinityService();
            var runner = new AsciiDungeonSequenceRunner(sequence, publisher, affinity);

            runner.Start();

            Assert.That(publisher.Entries.Count, Is.EqualTo(1));
            var entry = publisher.Entries[0];
            Assert.That(entry.StageIndex, Is.EqualTo(0));
            Assert.That(entry.Owner, Is.EqualTo(DreamweaverType.Light));
            Assert.That(entry.MapRows.Count, Is.EqualTo(5));
        }

        /// <summary>
        /// Ensures interacting with an aligned glyph forwards the affinity change to the service.
        /// </summary>
        [Test]
        public void AsciiDungeonSequenceRunner_InteractObject_WhenAlignedWithOwner_PushesAffinityChange()
        {
            var sequence = AsciiDungeonSequence.Create(this.CreateValidStageDefinitions());
            var publisher = new CapturingDungeonEventPublisher();
            var affinity = new CapturingAffinityService();
            var runner = new AsciiDungeonSequenceRunner(sequence, publisher, affinity);
            runner.Start();

            runner.ResolveInteraction('C');

            Assert.That(affinity.Changes.Count, Is.EqualTo(1));
            var change = affinity.Changes[0];
            Assert.That(change.Owner, Is.EqualTo(DreamweaverType.Light));
            Assert.That(change.Change.Type, Is.EqualTo(DreamweaverAffinityChangeType.Increase));
            Assert.That(change.Change.Amount, Is.EqualTo(1));
        }

        private List<DungeonStageDefinition> CreateValidStageDefinitions()
        {
            return new List<DungeonStageDefinition>
            {
                DungeonSequenceTestData.CreateStage(DreamweaverType.Light),
                DungeonSequenceTestData.CreateStage(DreamweaverType.Mischief),
                DungeonSequenceTestData.CreateStage(DreamweaverType.Wrath),
            };
        }

        private sealed class CapturingDungeonEventPublisher : IDungeonEventPublisher
        {
            public List<DungeonStageEnteredEvent> Entries { get; } = new();

            public void PublishStageEntered(DungeonStageEnteredEvent domainEvent) => this.Entries.Add(domainEvent);

            public void PublishStageCleared(DungeonStageClearedEvent domainEvent) => this.Entries.Add(new DungeonStageEnteredEvent(domainEvent.StageIndex, domainEvent.Owner, domainEvent.MapRows));
        }

        private sealed class CapturingAffinityService : IDreamweaverAffinityService
        {
            public List<(DreamweaverType Owner, DreamweaverAffinityChange Change)> Changes { get; } = new();

            public void ApplyChange(DreamweaverType owner, DreamweaverAffinityChange change) => this.Changes.Add((owner, change));
        }
    }
}
