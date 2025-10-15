// <copyright file="DungeonSequenceTests.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

namespace OmegaSpiral.Tests.Unit.Dungeon
{
    using System.Collections.Generic;
    using NUnit.Framework;
    using OmegaSpiral.Source.Scripts.Common;
    using OmegaSpiral.Source.Scripts.Domain.Dungeon;
    using OmegaSpiral.Source.Scripts.Domain.Dungeon.Models;
    using OmegaSpiral.Source.Scripts.Infrastructure.Dungeon;

    /// <summary>
    /// Unit tests for the ASCII dungeon sequence aggregate and supporting value objects.
    /// </summary>
    [TestFixture]
    public class DungeonSequenceTests
    {
        /// <summary>
        /// Verifies that valid definitions construct a sequence with three unique Dreamweaver-owned stages.
        /// </summary>
        [Test]
        public void DungeonStageLoader_Load_WhenSchemaValid_ConstructsStages()
        {
            var definitions = this.CreateValidStageDefinitions();
            var loader = new AsciiDungeonSequenceLoader();
            var json = DungeonSequenceTestData.CreateSequenceJson(definitions);

            var sequence = loader.LoadFromJson(json);

            Assert.That(sequence.Stages.Count, Is.EqualTo(3));
            Assert.That(sequence.Stages[0].Owner, Is.Not.EqualTo(sequence.Stages[1].Owner));
            Assert.That(sequence.Stages[0].Owner, Is.Not.EqualTo(sequence.Stages[2].Owner));
            Assert.That(sequence.Stages[1].Owner, Is.Not.EqualTo(sequence.Stages[2].Owner));
        }

        /// <summary>
        /// Ensures that duplicate Dreamweaver ownership is rejected as a domain violation.
        /// </summary>
        [Test]
        public void DreamweaverDungeon_Create_WhenOwnersDuplicate_ThrowsDomainException()
        {
            var definitions = this.CreateValidStageDefinitions();
            definitions[1] = definitions[0] with { Owner = DreamweaverType.Light };

            var loader = new AsciiDungeonSequenceLoader();
            var json = DungeonSequenceTestData.CreateSequenceJson(definitions);

            Assert.That(() => loader.LoadFromJson(json), Throws.TypeOf<DungeonValidationException>());
        }

        /// <summary>
        /// Ensures that resolving an aligned object yields an affinity-aware interaction result.
        /// </summary>
        [Test]
        public void DreamweaverDungeon_InteractObject_WhenAlignedWithOwner_ReturnsInteractionResult()
        {
            var definitions = this.CreateValidStageDefinitions();
            var sequence = AsciiDungeonSequence.Create(definitions);
            var stage = sequence.Stages[0];

            var interaction = stage.ResolveInteraction('C');

            Assert.That(interaction, Is.Not.Null);
            Assert.That(interaction.AlignedTo, Is.EqualTo(stage.Owner));
            Assert.That(interaction.Change.Amount, Is.EqualTo(1));
            Assert.That(interaction.Change.Type, Is.EqualTo(DreamweaverAffinityChangeType.Increase));
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
    }
}
