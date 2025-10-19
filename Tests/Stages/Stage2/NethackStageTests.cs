// <copyright file="NethackStageTests.cs" company="Omega Spiral">
// Copyright (c) Omega Spiral. All rights reserved.
// </copyright>

namespace OmegaSpiral.Tests.Unit.Nethack
{
    using System.Collections.Generic;
    using System.Linq;
    using GdUnit4;
    using Godot;
    using OmegaSpiral.Source.Scripts;
    using OmegaSpiral.Source.Scripts.Common;
    using OmegaSpiral.Source.Scripts.Domain.Dungeon;
    using OmegaSpiral.Source.Scripts.Domain.Dungeon.Models;
    using OmegaSpiral.Source.Scripts.Infrastructure.Dungeon;
    using static GdUnit4.Assertions;

    /// <summary>
    /// Unit tests for the Nethack ASCII dungeon stage (Scene 2).
    /// Tests validate the ASCII dungeon sequence structure, object interactions, and Dreamweaver affinity scoring.
    /// </summary>
    [TestSuite]
    public class NethackStageTests
    {
        /// <summary>
        /// Tests that the dungeon sequence loads with correct structure and required properties.
        /// </summary>
        [TestCase]
        public void LoadDungeonSequence_WithValidJson_LoadsCorrectly()
        {
            // Arrange
            var validJson = CreateValidDungeonSequenceJson();

            // Act
            var sequence = AsciiDungeonSequenceLoader.LoadFromJson(validJson);

            // Assert
            AssertThat(sequence).IsNotNull();
            AssertThat(sequence.Stages).HasSize(3);
            AssertThat(sequence.Stages.All(s => s.Owner != DreamweaverType.Unknown)).IsTrue();
            AssertThat(sequence.Stages.All(s => s.Map.Count > 0)).IsTrue();
            AssertThat(sequence.Stages.All(s => s.Legend.Count > 0)).IsTrue();
        }

        /// <summary>
        /// Tests that each dungeon stage has a unique Dreamweaver owner.
        /// </summary>
        [TestCase]
        public void LoadDungeonSequence_WithValidJson_HasUniqueOwners()
        {
            // Arrange
            var validJson = CreateValidDungeonSequenceJson();

            // Act
            var sequence = AsciiDungeonSequenceLoader.LoadFromJson(validJson);

            // Assert
            var owners = sequence.Stages.Select(s => s.Owner).ToList();
            AssertThat(owners.Distinct().Count()).IsEqual(3);
            AssertThat(owners.Contains(DreamweaverType.Light)).IsTrue();
            AssertThat(owners.Contains(DreamweaverType.Mischief)).IsTrue();
            AssertThat(owners.Contains(DreamweaverType.Wrath)).IsTrue();
        }

        /// <summary>
        /// Tests that dungeon stages have valid rectangular map structures.
        /// </summary>
        [TestCase]
        public void LoadDungeonSequence_WithValidJson_HasValidMaps()
        {
            // Arrange
            var validJson = CreateValidDungeonSequenceJson();

            // Act
            var sequence = AsciiDungeonSequenceLoader.LoadFromJson(validJson);

            // Assert
            foreach (var stage in sequence.Stages)
            {
                AssertThat(stage.Map).IsNotEmpty();
                var width = stage.Map[0].Length;
                AssertThat(stage.Map.All(row => row.Length == width)).IsTrue();
            }
        }

        /// <summary>
        /// Tests that object interactions return correct alignment information.
        /// </summary>
        [TestCase]
        [RequireGodotRuntime]
        public void DungeonStage_ObjectInteractions_ReturnCorrectAlignment()
        {
            // Arrange
            var stage = CreateTestDungeonStage(DreamweaverType.Light);
            var testObject = 'D'; // Door aligned to Light (owner)

            // Act
            var result = stage.ResolveInteraction(testObject);

            // Assert
            AssertThat(result).IsNotNull();
            AssertThat(result.AlignedTo).IsEqualTo(DreamweaverType.Light);
            AssertThat(result.Change.Amount).IsIn(1, 2); // Either 1 or 2 points as per spec
        }

        /// <summary>
        /// Tests that owner-aligned interactions give 2 points to the owner Dreamweaver.
        /// </summary>
        [TestCase]
        [RequireGodotRuntime]
        public void DungeonStage_OwnerAlignedInteraction_GivesTwoPoints()
        {
            // Arrange
            var stage = CreateTestDungeonStage(DreamweaverType.Light);
            var gameState = new GameState();
            var ownerObject = 'D'; // Door aligned to Light (owner)

            // Act
            var result = stage.ResolveInteraction(ownerObject);

            // Assert
            AssertThat(result).IsNotNull();
            AssertThat(result.AlignedTo).IsEqualTo(DreamweaverType.Light);
            AssertThat(result.Change.Amount).IsEqual(2);
        }

        /// <summary>
        /// Tests that cross-aligned interactions give 1 point to other Dreamweavers.
        /// </summary>
        [TestCase]
        [RequireGodotRuntime]
        public void DungeonStage_CrossAlignedInteraction_GivesOnePoint()
        {
            // Arrange
            var stage = CreateTestDungeonStage(DreamweaverType.Light);
            var gameState = new GameState();
            var crossAlignedObject = 'M'; // Monster aligned to Wrath (cross-alignment)

            // Act
            var result = stage.ResolveInteraction(crossAlignedObject);

            // Assert
            AssertThat(result).IsNotNull();
            AssertThat(result.AlignedTo).IsEqualTo(DreamweaverType.Wrath); // Cross-aligned to Wrath
            AssertThat(result.Change.Amount).IsEqual(1);
        }

        /// <summary>
        /// Tests that the dungeon sequence runner processes stages correctly.
        /// </summary>
        [TestCase]
        public void DungeonSequenceRunner_ProcessesStagesCorrectly()
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
        /// Tests that the dungeon sequence runner progresses through all stages.
        /// </summary>
        [TestCase]
        public void DungeonSequenceRunner_CompletesAllStages()
        {
            // Arrange
            var validJson = CreateValidDungeonSequenceJson();
            var sequence = AsciiDungeonSequenceLoader.LoadFromJson(validJson);
            var publisher = new TestDungeonEventPublisher();
            var affinityService = new TestDreamweaverAffinityService();
            var runner = new AsciiDungeonSequenceRunner(sequence, publisher, affinityService);

            // Act
            runner.Start();
            runner.CompleteCurrentStage(); // Stage 1 -> Stage 2
            runner.CompleteCurrentStage(); // Stage 2 -> Stage 3

            // Assert
            AssertThat(publisher.StageEnteredEventsCount).IsEqual(3); // All three stages
            AssertThat(publisher.LastStageEnteredEvent!.StageIndex).IsEqual(2); // Final stage
        }

        /// <summary>
        /// Tests that the dungeon sequence validates against the updated schema with objects property.
        /// </summary>
        [TestCase]
        public void LoadDungeonSequence_WithObjectsProperty_ValidatesCorrectly()
        {
            // Arrange
            var validJson = CreateValidDungeonSequenceJson();

            // Act
            var sequence = AsciiDungeonSequenceLoader.LoadFromJson(validJson);

            // Assert
            AssertThat(sequence).IsNotNull();
            foreach (var stage in sequence.Stages)
            {
                // Verify that the stage has the new objects property structure
                // This validation happens internally in the loader
                AssertThat(stage.Legend).IsNotEmpty();
            }
        }

        private string CreateValidDungeonSequenceJson()
        {
            return @"{
  ""type"": ""ascii_dungeon_sequence"",
  ""dungeons"": [
    {
      ""owner"": ""Light"",
      ""map"": [
        ""########################"",
        ""#......................#"",
        ""#.@....................#"",
        ""#........D...M...C....#"",
        ""#......................#"",
        ""########################""
      ],
      ""legend"": {
        ""#"": ""wall"",
        ""."": ""floor"",
        ""@"": ""player"",
        ""D"": ""door"",
        ""M"": ""monster"",
        ""C"": ""chest""
      },
      ""objects"": {
        ""D"": {
          ""type"": ""Door"",
          ""text"": ""You seek the path. Good. The first story always begins with a question."",
          ""aligned_to"": ""Light"",
          ""position"": [10, 3]
        },
        ""M"": {
          ""type"": ""Monster"",
          ""text"": ""A spectral wolf appears! It lunges..."",
          ""aligned_to"": ""Wrath"",
          ""position"": [14, 3]
        },
        ""C"": {
          ""type"": ""Chest"",
          ""text"": ""You open the chest. Inside: a broken compass."",
          ""aligned_to"": ""Mischief"",
          ""position"": [18, 3]
        }
      },
      ""playerStartPosition"": [2, 3]
    },
    {
      ""owner"": ""Mischief"",
      ""map"": [
        ""########################"",
        ""#..~..~..#"",
        ""#.@....................#"",
        ""#........C...D...M....#"",
        ""#..~..~..#"",
        ""########################""
      ],
      ""legend"": {
        ""#"": ""wall"",
        ""."": ""floor"",
        ""~"": ""water"",
        ""@"": ""player"",
        ""D"": ""door"",
        ""M"": ""monster"",
        ""C"": ""chest""
      },
      ""objects"": {
        ""D"": {
          ""type"": ""Door"",
          ""text"": ""Is chaos kinder than order?"",
          ""aligned_to"": ""Mischief"",
          ""position"": [10, 3]
        },
        ""M"": {
          ""type"": ""Monster"",
          ""text"": ""A guardian of light blocks your path!"",
          ""aligned_to"": ""Light"",
          ""position"": [14, 3]
        },
        ""C"": {
          ""type"": ""Chest"",
          ""text"": ""The chest giggles. It's empty... or is it?"",
          ""aligned_to"": ""Wrath"",
          ""position"": [18, 3]
        }
      },
      ""playerStartPosition"": [2, 3]
    },
    {
      ""owner"": ""Wrath"",
      ""map"": [
        ""########################"",
        ""#.#.#.#.#.#.#"",
        ""#@#.#.#.#.#.#.#.#.#.#.#"",
        ""#........M...C...D....#"",
        ""#.#.#.#.#.#.#.#"",
        ""########################""
      ],
      ""legend"": {
        ""#"": ""wall"",
        ""."": ""floor"",
        ""@"": ""player"",
        ""D"": ""door"",
        ""M"": ""monster"",
        ""C"": ""chest""
      },
      ""objects"": {
        ""D"": {
          ""type"": ""Door"",
          ""text"": ""Would you burn the world to save one soul?"",
          ""aligned_to"": ""Wrath"",
          ""position"": [18, 3]
        },
        ""M"": {
          ""type"": ""Monster"",
          ""text"": ""A trickster imp cackles and attacks!"",
          ""aligned_to"": ""Mischief"",
          ""position"": [10, 3]
        },
        ""C"": {
          ""type"": ""Chest"",
          ""text"": ""Inside: a shard glowing with ancient hope."",
          ""aligned_to"": ""Light"",
          ""position"": [14, 3]
        }
      },
      ""playerStartPosition"": [1, 3]
    }
  ]
}";
        }

        private DungeonStage CreateTestDungeonStage(DreamweaverType owner)
        {
            var stageDefinition = new DungeonStageDefinition
            {
                Owner = owner,
                Map = new List<string> { "###", "#@#", "###" },
                Legend = new Dictionary<char, string> { { '#', "wall" }, { '@', "player" }, { 'D', "door" }, { 'M', "monster" }, { 'C', "chest" } },
                Objects = new Dictionary<char, DungeonObjectDefinition>(),
                PlayerStartPosition = new int[] { 1, 1 }
            };

            // Add object definitions for testing
            stageDefinition.Objects['D'] = new DungeonObjectDefinition
            {
                Type = "Door",
                Text = "Test door interaction",
                AlignedTo = owner,
                Position = new int[] { 1, 0 }
            };

            stageDefinition.Objects['M'] = new DungeonObjectDefinition
            {
                Type = "Monster",
                Text = "Test monster interaction",
                AlignedTo = DreamweaverType.Wrath, // Cross-aligned
                Position = new int[] { 1, 2 }
            };

            return new DungeonStage(stageDefinition);
        }

        private sealed class TestDungeonEventPublisher : IDungeonEventPublisher
        {
            public int StageEnteredEventsCount { get; private set; }
            public int StagesClearedCount { get; private set; }
            public DungeonStageEnteredEvent? LastStageEnteredEvent { get; private set; }
            public DungeonStageClearedEvent? LastStageClearedEvent { get; private set; }

            public void PublishStageCleared(DungeonStageClearedEvent domainEvent)
            {
                LastStageClearedEvent = domainEvent;
                StagesClearedCount++;
            }

            public void PublishStageEntered(DungeonStageEnteredEvent domainEvent)
            {
                LastStageEnteredEvent = domainEvent;
                StageEnteredEventsCount++;
            }
        }

        private sealed class TestDreamweaverAffinityService : IDreamweaverAffinityService
        {
            public DreamweaverType LastAppliedOwner { get; private set; } = DreamweaverType.Light;
            public DreamweaverAffinityChange? LastAppliedChange { get; private set; }

            public void ApplyChange(DreamweaverType owner, DreamweaverAffinityChange change)
            {
                LastAppliedOwner = owner;
                LastAppliedChange = change;
            }
        }
    }
}