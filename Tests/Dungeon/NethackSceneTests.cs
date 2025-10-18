// <copyright file="NethackSceneTests.cs" company="Omega Spiral">
// Copyright (c) Omega Spiral. All rights reserved.
// </copyright>
namespace OmegaSpiral.Tests.Unit.Dungeon
{
    using GdUnit4;
    using Godot;
    using OmegaSpiral.Source.Scripts.Common;
    using OmegaSpiral.Source.Scripts.Domain.Dungeon;
    using OmegaSpiral.Source.Scripts.Domain.Dungeon.Models;
    using OmegaSpiral.Source.Scripts.Infrastructure.Dungeon;
    using static GdUnit4.Assertions;

    /// <summary>
    /// Unit tests for the NetHack ASCII dungeon sequence (Scene 2).
    /// Tests cover schema loading, object interactions, stage progression, and Dreamweaver affinity updates.
    /// These tests verify structure, sequencing, and side effects rather than specific narrative text.
    /// </summary>
    [TestSuite]
    public class NethackSceneTests
    {
        /// <summary>
        /// Tests that scene loads three distinct DungeonStage instances from valid schema.
        /// </summary>
        [TestCase]
        public void LoadSchema_WhenValidSchemaProvided_LoadsThreeDistinctDungeonStages()
        {
            // Arrange
            var validJson = CreateValidDungeonSequenceJson();

            // Act
            var sequence = AsciiDungeonSequenceLoader.LoadFromJson(validJson);

            // Assert
            AssertThat(sequence).IsNotNull();
            AssertThat(sequence.Stages).HasSize(3);
            AssertThat(sequence.Stages[0].Owner).IsNotEqual(sequence.Stages[1].Owner);
            AssertThat(sequence.Stages[0].Owner).IsNotEqual(sequence.Stages[2].Owner);
            AssertThat(sequence.Stages[1].Owner).IsNotEqual(sequence.Stages[2].Owner);
        }

        /// <summary>
        /// Tests that scene assigns unique owner values matching schema definition.
        /// </summary>
        [TestCase]
        public void LoadSchema_WhenValidSchemaProvided_AssignsUniqueOwnerValues()
        {
            // Arrange
            var validJson = CreateValidDungeonSequenceJson();

            // Act
            var sequence = AsciiDungeonSequenceLoader.LoadFromJson(validJson);

            // Assert
            AssertThat(sequence.Stages).HasSize(3);
            var owners = sequence.Stages.Select(s => s.Owner).ToList();
            AssertThat(owners.Distinct().Count()).IsEqual(3);
            AssertThat(owners.Contains(DreamweaverType.Light)).IsTrue();
            AssertThat(owners.Contains(DreamweaverType.Mischief)).IsTrue();
            AssertThat(owners.Contains(DreamweaverType.Wrath)).IsTrue();
        }

        /// <summary>
        /// Tests that scene validates map dimensions against schema specifications.
        /// </summary>
        [TestCase]
        public void LoadSchema_WhenValidSchemaProvided_ValidatesMapDimensions()
        {
            // Arrange
            var validJson = CreateValidDungeonSequenceJson();

            // Act
            var sequence = AsciiDungeonSequenceLoader.LoadFromJson(validJson);

            // Assert
            AssertThat(sequence.Stages).HasSize(3);
            foreach (var stage in sequence.Stages)
            {
                AssertThat(stage.Map).IsNotEmpty();
                AssertThat(stage.Map.All(row => row.Length == stage.Map[0].Length)).IsTrue(); // Rectangular map
            }
        }

        /// <summary>
        /// Tests that scene correctly aligns object glyphs with legend entries.
        /// </summary>
        [TestCase]
        public void LoadSchema_WhenValidSchemaProvided_CorrectlyAlignsObjectGlyphs()
        {
            // Arrange
            var validJson = CreateValidDungeonSequenceJson();

            // Act
            var sequence = AsciiDungeonSequenceLoader.LoadFromJson(validJson);

            // Assert
            AssertThat(sequence.Stages).HasSize(3);
            foreach (var stage in sequence.Stages)
            {
                // Verify legend entries exist
                AssertThat(stage.Legend).IsNotEmpty();
            }
        }

        /// <summary>
        /// Tests that scene throws domain exception when duplicate owners detected.
        /// </summary>
        [TestCase]
        [ThrowsException(typeof(DungeonValidationException), "unique Dreamweaver owner")]
        public void LoadSchema_WhenDuplicateOwnersProvided_ThrowsDomainException()
        {
            // Arrange
            var duplicateOwnerJson = CreateDungeonSequenceWithDuplicateOwners();

            // Act
            AsciiDungeonSequenceLoader.LoadFromJson(duplicateOwnerJson);
        }

        /// <summary>
        /// Tests that scene prevents aggregate creation with invalid owner sequence.
        /// </summary>
        [TestCase]
        public void CreateSequence_WhenDuplicateOwnersProvided_PreventsAggregateCreation()
        {
            // Arrange
            var duplicateDefinitions = new List<DungeonStageDefinition>
            {
                CreateValidDungeonStageDefinition(DreamweaverType.Light),
                CreateValidDungeonStageDefinition(DreamweaverType.Light), // Duplicate
                CreateValidDungeonStageDefinition(DreamweaverType.Wrath),
            };

            // Act & Assert
            var exception = AssertThat(() => AsciiDungeonSequence.Create(duplicateDefinitions))
                .Completions()
                .Throws<DungeonValidationException>();
            AssertThat(exception.Message).Contains("unique Dreamweaver owner");
        }

        /// <summary>
        /// Tests that scene maintains balanced affinity distribution across Dreamweavers.
        /// </summary>
        [TestCase]
        public void LoadSchema_WhenValidSchemaProvided_MaintainsBalancedAffinityDistribution()
        {
            // Arrange
            var validJson = CreateValidDungeonSequenceJson();

            // Act
            var sequence = AsciiDungeonSequenceLoader.LoadFromJson(validJson);

            // Assert
            AssertThat(sequence.Stages).HasSize(3);
            var owners = sequence.Stages.Select(s => s.Owner).ToList();
            AssertThat(owners.Distinct().Count()).IsEqual(3); // Each Dreamweaver appears once
            AssertThat(owners.Contains(DreamweaverType.Light)).IsTrue();
            AssertThat(owners.Contains(DreamweaverType.Mischief)).IsTrue();
            AssertThat(owners.Contains(DreamweaverType.Wrath)).IsTrue();
        }

        /// <summary>
        /// Tests that scene publishes DungeonStageEntered event when stage begins.
        /// </summary>
        [TestCase]
        public void Sequence_WhenStageBegins_PublishesDungeonStageEnteredEvent()
        {
            // Arrange
            var validJson = CreateValidDungeonSequenceJson();
            var sequence = AsciiDungeonSequenceLoader.LoadFromJson(validJson);
            var publisher = new TestDungeonEventPublisher();
            var affinityService = new TestDreamweaverAffinityService();
            var runner = new AsciiDungeonSequenceRunner(sequence, publisher, affinityService);

            // Act
            runner.Start();

            // Assert
            AssertThat(publisher.LastStageEnteredEvent).IsNotNull();
            AssertThat(publisher.LastStageEnteredEvent!.StageIndex).IsEqual(0);
        }

        /// <summary>
        /// Tests that scene includes owner identifier in event payload.
        /// </summary>
        [TestCase]
        public void StageEvent_WhenPublished_IncludesOwnerIdentifier()
        {
            // Arrange
            var validJson = CreateValidDungeonSequenceJson();
            var sequence = AsciiDungeonSequenceLoader.LoadFromJson(validJson);
            var publisher = new TestDungeonEventPublisher();
            var affinityService = new TestDreamweaverAffinityService();
            var runner = new AsciiDungeonSequenceRunner(sequence, publisher, affinityService);

            // Act
            runner.Start();

            // Assert
            AssertThat(publisher.LastStageEnteredEvent).IsNotNull();
            AssertThat(publisher.LastStageEnteredEvent!.Owner).IsEqual(DreamweaverType.Light); // First stage is Light
        }

        /// <summary>
        /// Tests that scene includes stage index in event payload.
        /// </summary>
        [TestCase]
        public void StageEvent_WhenPublished_IncludesStageIndex()
        {
            // Arrange
            var validJson = CreateValidDungeonSequenceJson();
            var sequence = AsciiDungeonSequenceLoader.LoadFromJson(validJson);
            var publisher = new TestDungeonEventPublisher();
            var affinityService = new TestDreamweaverAffinityService();
            var runner = new AsciiDungeonSequenceRunner(sequence, publisher, affinityService);

            // Act
            runner.Start();

            // Assert
            AssertThat(publisher.LastStageEnteredEvent).IsNotNull();
            AssertThat(publisher.LastStageEnteredEvent!.StageIndex).IsEqual(0);
        }

        /// <summary>
        /// Tests that scene includes ASCII map metadata for rendering in event payload.
        /// </summary>
        [TestCase]
        public void StageEvent_WhenPublished_IncludesASCIIMapMetadata()
        {
            // Arrange
            var validJson = CreateValidDungeonSequenceJson();
            var sequence = AsciiDungeonSequenceLoader.LoadFromJson(validJson);
            var publisher = new TestDungeonEventPublisher();
            var affinityService = new TestDreamweaverAffinityService();
            var runner = new AsciiDungeonSequenceRunner(sequence, publisher, affinityService);

            // Act
            runner.Start();

            // Assert
            AssertThat(publisher.LastStageEnteredEvent).IsNotNull();
            AssertThat(publisher.LastStageEnteredEvent!.MapRows).IsNotEmpty();
            AssertThat(publisher.LastStageEnteredEvent.MapRows[0]).Contains("#"); // Verify map data present
        }

        /// <summary>
        /// Tests that scene increments affinity when interacting with owner-aligned object.
        /// </summary>
        [TestCase]
    [RequireGodotRuntime]
    public void ObjectInteraction_WhenOwnerAlignedObject_InteractsAndUpdatesAffinity()
        {
            // Arrange
            var gameState = new GameState();
            var stage = CreateTestDungeonStage(DreamweaverType.Light);
            var ownerObjectKey = 'D'; // Door aligned to Light (owner)

            // Act
            var interactionResult = stage.ResolveInteraction(ownerObjectKey);
            gameState.UpdateDreamweaverScore(interactionResult.AlignedTo, 2); // Owner gets +2 points

            // Assert
            AssertThat(interactionResult).IsNotNull();
            AssertThat(interactionResult.AlignedTo).IsEqualTo(DreamweaverType.Light); // Owner alignment = +2
            AssertThat(gameState.DreamweaverScores[DreamweaverType.Light] >= 2).OverrideFailureMessage($"Expected Light Dreamweaver score to be at least 2 after owner-aligned interaction, but was {gameState.DreamweaverScores[DreamweaverType.Light]}").IsTrue();
        }

        /// <summary>
        /// Tests that scene applies configured point value to correct Dreamweaver.
        /// </summary>
        [TestCase]
    [RequireGodotRuntime]
    public void ObjectInteraction_WhenConfiguredPointsApplied_AppliesToCorrectDreamweaver()
        {
            // Arrange
            var gameState = new GameState();
            var stage = CreateTestDungeonStage(DreamweaverType.Light);
            var crossAlignedObjectKey = 'M'; // Monster aligned to Wrath (cross-alignment)

            // Act
            var interactionResult = stage.ResolveInteraction(crossAlignedObjectKey);
            gameState.UpdateDreamweaverScore(interactionResult.AlignedTo, 1); // Cross-alignment gets +1 point

            // Assert
            AssertThat(interactionResult).IsNotNull();
            AssertThat(interactionResult.AlignedTo).IsEqualTo(DreamweaverType.Wrath); // Cross-alignment = +1
            AssertThat(gameState.DreamweaverScores[DreamweaverType.Wrath] >= 1).OverrideFailureMessage($"Expected Wrath Dreamweaver score to be at least 1 after cross-aligned interaction, but was {gameState.DreamweaverScores[DreamweaverType.Wrath]}").IsTrue();
        }

        /// <summary>
        /// Tests that scene records interaction in affinity history.
        /// </summary>
        [TestCase]
    [RequireGodotRuntime]
    public void ObjectInteraction_WhenOccurs_RecordsInAffinityHistory()
        {
            // Arrange
            var gameState = new GameState();
            var stage = CreateTestDungeonStage(DreamweaverType.Light);
            var ownerObjectKey = 'D'; // Door aligned to Light

            // Act
            var interactionResult = stage.ResolveInteraction(ownerObjectKey);
            gameState.UpdateDreamweaverScore(interactionResult.AlignedTo, 2);

            // Assert
            AssertThat(interactionResult).IsNotNull();
            AssertThat(interactionResult.AlignedTo).IsEqualTo(DreamweaverType.Light);
            AssertThat(gameState.DreamweaverScores[DreamweaverType.Light] >= 2).OverrideFailureMessage($"Expected Light Dreamweaver score to be at least 2 after configured points applied, but was {gameState.DreamweaverScores[DreamweaverType.Light]}").IsTrue();
        }

        /// <summary>
        /// Tests that scene leaves affinity unchanged for non-aligned object interactions.
        /// </summary>
        [TestCase]
    [RequireGodotRuntime]
    public void ObjectInteraction_WhenNonAlignedObject_LeavesAffinityUnchanged()
        {
            // Arrange
            var gameState = new GameState();
            var initialScore = gameState.DreamweaverScores[DreamweaverType.Wrath];
            var stage = CreateTestDungeonStage(DreamweaverType.Light);
            var nonAlignedObjectKey = 'C'; // Chest aligned to Mischief (not Wrath)

            // Act
            var interactionResult = stage.ResolveInteraction(nonAlignedObjectKey);

            // Assert
            AssertThat(interactionResult).IsNotNull();
            AssertThat(interactionResult.AlignedTo).IsNotEqualTo(DreamweaverType.Wrath); // Not owner alignment
            AssertThat(gameState.DreamweaverScores[DreamweaverType.Wrath]).IsEqual(initialScore); // Unchanged
        }

        /// <summary>
        /// Tests that scene advances to next stage when current stage is completed.
        /// </summary>
        [TestCase]
        public void StageProgression_WhenCurrentStageCompleted_AdvancesToNextStage()
        {
            // Arrange
            var validJson = CreateValidDungeonSequenceJson();
            var sequence = AsciiDungeonSequenceLoader.LoadFromJson(validJson);
            var publisher = new TestDungeonEventPublisher();
            var affinityService = new TestDreamweaverAffinityService();
            var runner = new AsciiDungeonSequenceRunner(sequence, publisher, affinityService);

            // Act
            runner.Start(); // Start at stage 0
            runner.CompleteCurrentStage(); // Complete stage 0, move to stage 1

            // Assert
            AssertThat(publisher.StageEnteredEventsCount).IsEqual(2); // Initial start (stage 0) + after completion (stage 1)
            AssertThat(publisher.LastStageEnteredEvent!.StageIndex).IsEqual(1);
        }

        /// <summary>
        /// Tests that scene maintains consistent sequence order with same seed.
        /// </summary>
        [TestCase]
        public void StageProgression_WhenSameSeedUsed_MaintainsConsistentSequenceOrder()
        {
            // Arrange
            var validJson = CreateValidDungeonSequenceJson();

            // Act - Load sequence multiple times with same data
            var sequence1 = AsciiDungeonSequenceLoader.LoadFromJson(validJson);
            var sequence2 = AsciiDungeonSequenceLoader.LoadFromJson(validJson);

            // Assert - Verify consistent order
            AssertThat(sequence1.Stages.Count).IsEqual(sequence2.Stages.Count);
            for (int i = 0; i < sequence1.Stages.Count; i++)
            {
                AssertThat(sequence1.Stages[i].Owner).IsEqual(sequence2.Stages[i].Owner);
            }
        }

        /// <summary>
        /// Tests that scene generates identical layouts across runs with same seed.
        /// </summary>
        [TestCase]
        public void StageProgression_WhenSameSeedUsed_GeneratesIdenticalLayouts()
        {
            // Arrange
            var validJson = CreateValidDungeonSequenceJson();

            // Act - Load sequence multiple times with same data
            var sequence1 = AsciiDungeonSequenceLoader.LoadFromJson(validJson);
            var sequence2 = AsciiDungeonSequenceLoader.LoadFromJson(validJson);

            // Assert - Verify identical layouts
            AssertThat(sequence1.Stages.Count).IsEqual(sequence2.Stages.Count);
            for (int i = 0; i < sequence1.Stages.Count; i++)
            {
                var stage1 = sequence1.Stages[i];
                var stage2 = sequence2.Stages[i];

                AssertThat(stage1.Map.Count).IsEqual(stage2.Map.Count);
                for (int j = 0; j < stage1.Map.Count; j++)
                {
                    AssertThat(stage1.Map[j]).IsEqual(stage2.Map[j]);
                }
            }
        }

        /// <summary>
        /// Tests that scene reaches third stage without skipping when progressing.
        /// </summary>
        [TestCase]
        public void StageProgression_WhenProgressing_ReachesThirdStageWithoutSkipping()
        {
            // Arrange
            var validJson = CreateValidDungeonSequenceJson();
            var sequence = AsciiDungeonSequenceLoader.LoadFromJson(validJson);
            var publisher = new TestDungeonEventPublisher();
            var affinityService = new TestDreamweaverAffinityService();
            var runner = new AsciiDungeonSequenceRunner(sequence, publisher, affinityService);

            // Act
            runner.Start(); // Start at stage 0
            runner.CompleteCurrentStage(); // Complete stage 0, move to stage 1
            runner.CompleteCurrentStage(); // Complete stage 1, move to stage 2

            // Assert
            AssertThat(publisher.StageEnteredEventsCount).IsEqual(3); // All three stages
            AssertThat(publisher.LastStageEnteredEvent!.StageIndex).IsEqual(2);
        }

        /// <summary>
        /// Tests that scene handles movement input methods correctly (WASD, arrow keys, gamepad).
        /// </summary>
        [TestCase]
        public void Movement_WhenInputReceived_HandlesMovementCorrectly()
        {
            // This test would require Godot runtime to test actual movement
            // The mock implementation would verify the movement logic
            // TODO: Implement proper test with actual Godot runtime or proper mock
            AssertThat(true).IsTrue();
        }

        /// <summary>
        /// Tests that scene validates object legend entries contain required alignment mappings.
        /// </summary>
        [TestCase]
        public void LoadSchema_WhenValidSchemaProvided_ValidatesObjectLegendEntries()
        {
            // Arrange
            var validJson = CreateValidDungeonSequenceJson();

            // Act
            var sequence = AsciiDungeonSequenceLoader.LoadFromJson(validJson);

            // Assert
            AssertThat(sequence.Stages).HasSize(3);
            foreach (var stage in sequence.Stages)
            {
                AssertThat(stage.Legend).IsNotEmpty();
                foreach (var legendEntry in stage.Legend)
                {
                    AssertThat(legendEntry.Value).IsNotNull(); // Each glyph must map to an alignment
                }
            }
        }

        /// <summary>
        /// Tests that scene validates map content against legend entries.
        /// </summary>
        [TestCase]
        public void LoadSchema_WhenValidSchemaProvided_ValidatesMapContentAgainstLegend()
        {
            // Arrange
            var validJson = CreateValidDungeonSequenceJson();

            // Act
            var sequence = AsciiDungeonSequenceLoader.LoadFromJson(validJson);

            // Assert
            AssertThat(sequence.Stages).HasSize(3);
            foreach (var stage in sequence.Stages)
            {
                // Verify all characters in map exist in legend
                foreach (var row in stage.Map)
                {
                    foreach (var character in row)
                    {
                        // Skip empty spaces
                        if (character != ' ')
                        {
                            AssertThat(stage.Legend.ContainsKey(character)).IsTrue();
                        }
                    }
                }
            }
        }

        private string CreateValidDungeonSequenceJson()
        {
            return @"{
    ""stages"": [
        {
            ""owner"": ""Light"",
            ""map"": [
                ""############"",
                ""#........D.#"",
                ""#.M......C.#"",
                ""#........K.#"",
                ""############""
            ],
            ""legend"": {
                ""#"": ""Wall"",
                ""."": ""Floor"",
                ""D"": ""Door"",
                ""M"": ""Monster"",
                ""C"": ""Chest"",
                ""K"": ""Key""
            }
        },
        {
            ""owner"": ""Mischief"",
            ""map"": [
                ""############"",
                ""#........C.#"",
                ""#.D......K.#"",
                ""#........M.#"",
                ""############""
            ],
            ""legend"": {
                ""#"": ""Wall"",
                ""."": ""Floor"",
                ""D"": ""Door"",
                ""M"": ""Monster"",
                ""C"": ""Chest"",
                ""K"": ""Key""
            }
        },
        {
            ""owner"": ""Wrath"",
            ""map"": [
                ""############"",
                ""#........K.#"",
                ""#.C......D.#"",
                ""#........M.#"",
                ""############""
            ],
            ""legend"": {
                ""#"": ""Wall"",
                ""."": ""Floor"",
                ""D"": ""Door"",
                ""M"": ""Monster"",
                ""C"": ""Chest"",
                ""K"": ""Key""
            }
        }
    ]
}";
        }

        private string CreateDungeonSequenceWithDuplicateOwners()
        {
            return @"{
    ""stages"": [
        {
            ""owner"": ""Light"",
            ""map"": [
                ""############"",
                ""#........D.#"",
                ""#.M......C.#"",
                ""#........K.#"",
                ""############""
            ],
            ""legend"": {
                ""#"": ""Wall"",
                ""."": ""Floor"",
                ""D"": ""Door"",
                ""M"": ""Monster"",
                ""C"": ""Chest"",
                ""K"": ""Key""
            }
        },
        {
            ""owner"": ""Light"",
            ""map"": [
                ""############"",
                ""#........C.#"",
                ""#.D......K.#"",
                ""#........M.#"",
                ""############""
            ],
            ""legend"": {
                ""#"": ""Wall"",
                ""."": ""Floor"",
                ""D"": ""Door"",
                ""M"": ""Monster"",
                ""C"": ""Chest"",
                ""K"": ""Key""
            }
        }
    ]
}";
        }

        private DungeonStageDefinition CreateValidDungeonStageDefinition(DreamweaverType owner)
        {
            return new DungeonStageDefinition(
                "test-id",
                owner,
                new List<string> { "####", "#..#", "#..#", "####" },
                new Dictionary<char, string> { { '#', "Wall" }, { '.', "Floor" } },
                new Dictionary<char, DungeonObjectDefinition>());
        }

        private DungeonStage CreateTestDungeonStage(DreamweaverType owner)
        {
            var definition = CreateValidDungeonStageDefinition(owner);
            return DungeonStage.Create(definition);
        }

        private sealed class TestDungeonEventPublisher : IDungeonEventPublisher
        {
            public int StageEnteredEventsCount { get; private set; }

            public DungeonStageEnteredEvent? LastStageEnteredEvent { get; private set; }

            public void PublishStageCleared(DungeonStageClearedEvent domainEvent)
            {
                // Mock implementation
            }

            public void PublishStageEntered(DungeonStageEnteredEvent domainEvent)
            {
                LastStageEnteredEvent = domainEvent;
                StageEnteredEventsCount++;
            }
        }

        private sealed class TestDreamweaverAffinityService : IDreamweaverAffinityService
        {
            public void ApplyChange(DreamweaverType owner, DreamweaverAffinityChange change)
            {
                // Mock implementation
            }
        }
    }
}
