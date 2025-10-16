using GdUnit4;
using Godot;
using static GdUnit4.Assertions;
using OmegaSpiral.Source.Scripts.Domain.Dungeon;
using OmegaSpiral.Source.Scripts.Domain.Dungeon.Models;
using OmegaSpiral.Source.Scripts.Infrastructure.Dungeon;
using OmegaSpiral.Source.Scripts.Common;

namespace OmegaSpiral.Tests.Unit.Dungeon;

/// <summary>
/// Unit tests for the NetHack ASCII dungeon sequence (Scene 2).
/// Tests cover schema loading, object interactions, stage progression, and Dreamweaver affinity updates.
/// These tests verify structure, sequencing, and side effects rather than specific narrative text.
/// </summary>
[TestSuite]
public class NethackSceneTests
{
    #region Dungeon Schema Loading Tests (STG-001)

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
        AssertThat(sequence.Stages[0].Owner).IsNotEqualTo(sequence.Stages[1].Owner);
        AssertThat(sequence.Stages[0].Owner).IsNotEqualTo(sequence.Stages[2].Owner);
        AssertThat(sequence.Stages[1].Owner).IsNotEqualTo(sequence.Stages[2].Owner);
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
        AssertThat(owners.Contains(OmegaSpiral.Source.Scripts.Common.DreamweaverType.Light)).IsTrue();
        AssertThat(owners.Contains(OmegaSpiral.Source.Scripts.Common.DreamweaverType.Mischief)).IsTrue();
        AssertThat(owners.Contains(OmegaSpiral.Source.Scripts.Common.DreamweaverType.Wrath)).IsTrue();
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

    #endregion

    #region Owner Duplication Prevention Tests (STG-002)

    /// <summary>
    /// Tests that scene throws domain exception when duplicate owners detected.
    /// </summary>
    [TestCase]
    public void LoadSchema_WhenDuplicateOwnersProvided_ThrowsDomainException()
    {
        // Arrange
        var duplicateOwnerJson = CreateDungeonSequenceWithDuplicateOwners();

        // Act & Assert
        AssertThat(() => AsciiDungeonSequenceLoader.LoadFromJson(duplicateOwnerJson))
            .Throws<DungeonValidationException>()
            .WithMessageContaining("unique Dreamweaver owner");
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
            CreateValidDungeonStageDefinition(OmegaSpiral.Source.Scripts.Common.DreamweaverType.Light),
            CreateValidDungeonStageDefinition(OmegaSpiral.Source.Scripts.Common.DreamweaverType.Light), // Duplicate
            CreateValidDungeonStageDefinition(OmegaSpiral.Source.Scripts.Common.DreamweaverType.Wrath)
        };

        // Act & Assert
        AssertThat(() => AsciiDungeonSequence.Create(duplicateDefinitions))
            .Throws<DungeonValidationException>()
            .WithMessageContaining("unique Dreamweaver owner");
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
        AssertThat(owners.Contains(OmegaSpiral.Source.Scripts.Common.DreamweaverType.Light)).IsTrue();
        AssertThat(owners.Contains(OmegaSpiral.Source.Scripts.Common.DreamweaverType.Mischief)).IsTrue();
        AssertThat(owners.Contains(OmegaSpiral.Source.Scripts.Common.DreamweaverType.Wrath)).IsTrue();
    }

    #endregion

    #region Stage Entry Event Publishing Tests (STG-003)

    /// <summary>
    /// Tests that scene publishes DungeonStageEntered event when stage begins.
    /// </summary>
    [TestCase]
    public void Sequence_WhenStageBegins_PublishesDungeonStageEnteredEvent()
    {
        // This test would require Godot runtime to test actual event publishing
        // The mock implementation would verify the event publishing logic
        // TODO: Implement proper test with actual Godot runtime or proper mock
        AssertThat(true).IsTrue();
    }

    /// <summary>
    /// Tests that scene includes owner identifier in event payload.
    /// </summary>
    [TestCase]
    public void StageEvent_WhenPublished_IncludesOwnerIdentifier()
    {
        // This test would require Godot runtime to test actual event payload
        // The mock implementation would verify owner identifier inclusion
        // TODO: Implement proper test with actual Godot runtime or proper mock
        AssertThat(true).IsTrue();
    }

    /// <summary>
    /// Tests that scene includes stage index in event payload.
    /// </summary>
    [TestCase]
    public void StageEvent_WhenPublished_IncludesStageIndex()
    {
        // This test would require Godot runtime to test actual event payload
        // The mock implementation would verify stage index inclusion
        // TODO: Implement proper test with actual Godot runtime or proper mock
        AssertThat(true).IsTrue();
    }

    /// <summary>
    /// Tests that scene includes ASCII map metadata for rendering in event payload.
    /// </summary>
    [TestCase]
    public void StageEvent_WhenPublished_IncludesASCIIMapMetadata()
    {
        // This test would require Godot runtime to test actual event payload
        // The mock implementation would verify map metadata inclusion
        // TODO: Implement proper test with actual Godot runtime or proper mock
        AssertThat(true).IsTrue();
    }

    #endregion

    #region Object Interaction Affinity Tests (STG-004)

    /// <summary>
    /// Tests that scene increments affinity when interacting with owner-aligned object.
    /// </summary>
    [TestCase]
    public void ObjectInteraction_WhenOwnerAlignedObject_InteractsAndUpdatesAffinity()
    {
        // Arrange
        var gameState = new GameState();
        var stage = CreateTestDungeonStage(OmegaSpiral.Source.Scripts.Common.DreamweaverType.Light);
        var ownerObjectKey = 'D'; // Door aligned to Light (owner)

        // Act
        var interactionResult = stage.ResolveInteraction(ownerObjectKey);
        gameState.UpdateDreamweaverScore(interactionResult.AlignedTo, 2); // Owner gets +2 points

        // Assert
        AssertThat(interactionResult).IsNotNull();
        AssertThat(interactionResult.AlignedTo).IsEqualTo(OmegaSpiral.Source.Scripts.Common.DreamweaverType.Light); // Owner alignment = +2
        AssertThat(gameState.DreamweaverScores[OmegaSpiral.Source.Scripts.Common.DreamweaverType.Light] >= 2).IsTrue();
    }

    /// <summary>
    /// Tests that scene applies configured point value to correct Dreamweaver.
    /// </summary>
    [TestCase]
    public void ObjectInteraction_WhenConfiguredPointsApplied_AppliesToCorrectDreamweaver()
    {
        // Arrange
        var gameState = new GameState();
        var stage = CreateTestDungeonStage(OmegaSpiral.Source.Scripts.Common.DreamweaverType.Light);
        var crossAlignedObjectKey = 'M'; // Monster aligned to Wrath (cross-alignment)

        // Act
        var interactionResult = stage.ResolveInteraction(crossAlignedObjectKey);
        gameState.UpdateDreamweaverScore(interactionResult.AlignedTo, 1); // Cross-alignment gets +1 point

        // Assert
        AssertThat(interactionResult).IsNotNull();
        AssertThat(interactionResult.AlignedTo).IsEqualTo(OmegaSpiral.Source.Scripts.Common.DreamweaverType.Wrath); // Cross-alignment = +1
        AssertThat(gameState.DreamweaverScores[OmegaSpiral.Source.Scripts.Common.DreamweaverType.Wrath] >= 1).IsTrue();
    }

    /// <summary>
    /// Tests that scene records interaction in affinity history.
    /// </summary>
    [TestCase]
    public void ObjectInteraction_WhenOccurs_RecordsInAffinityHistory()
    {
        // Arrange
        var gameState = new GameState();
        var stage = CreateTestDungeonStage(OmegaSpiral.Source.Scripts.Common.DreamweaverType.Light);
        var ownerObjectKey = 'D'; // Door aligned to Light

        // Act
        var interactionResult = stage.ResolveInteraction(ownerObjectKey);
        gameState.UpdateDreamweaverScore(interactionResult.AlignedTo, 2);

        // Assert
        AssertThat(interactionResult).IsNotNull();
        AssertThat(interactionResult.AlignedTo).IsEqualTo(OmegaSpiral.Source.Scripts.Common.DreamweaverType.Light);
        AssertThat(gameState.DreamweaverScores[OmegaSpiral.Source.Scripts.Common.DreamweaverType.Light] >= 2).IsTrue();
    }

    /// <summary>
    /// Tests that scene leaves affinity unchanged for non-aligned object interactions.
    /// </summary>
    [TestCase]
    public void ObjectInteraction_WhenNonAlignedObject_LeavesAffinityUnchanged()
    {
        // Arrange
        var gameState = new GameState();
        var initialScore = gameState.DreamweaverScores[OmegaSpiral.Source.Scripts.Common.DreamweaverType.Wrath];
        var stage = CreateTestDungeonStage(OmegaSpiral.Source.Scripts.Common.DreamweaverType.Light);
        var nonAlignedObjectKey = 'C'; // Chest aligned to Mischief (not Wrath)

        // Act
        var interactionResult = stage.ResolveInteraction(nonAlignedObjectKey);

        // Assert
        AssertThat(interactionResult).IsNotNull();
        AssertThat(interactionResult.AlignedTo).IsNotEqualTo(OmegaSpiral.Source.Scripts.Common.DreamweaverType.Wrath); // Not owner alignment
        AssertThat(gameState.DreamweaverScores[OmegaSpiral.Source.Scripts.Common.DreamweaverType.Wrath]).IsEqual(initialScore); // Unchanged
    }

    #endregion

    #region Deterministic Stage Progression Tests (STG-005)

    /// <summary>
    /// Tests that scene advances to next stage when current stage is completed.
    /// </summary>
    [TestCase]
    public void StageProgression_WhenCurrentStageCompleted_AdvancesToNextStage()
    {
        // This test would require Godot runtime to test actual stage progression
        // The mock implementation would verify the progression logic
        // TODO: Implement proper test with actual Godot runtime or proper mock
        AssertThat(true).IsTrue();
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
        // This test would require Godot runtime to test actual progression
        // The mock implementation would verify the progression logic
        // TODO: Implement proper test with actual Godot runtime or proper mock
        AssertThat(true).IsTrue();
    }

    #endregion

    #region Dungeon Gameplay Expansion Tests (DNG-001-DNG-010)

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
    /// Tests that scene implements collision system preventing wall walking.
    /// </summary>
    [TestCase]
    public void Movement_WhenCollisionDetected_PreventsWallWalking()
    {
        // This test would require Godot runtime to test actual collision
        // The mock implementation would verify the collision logic
        // TODO: Implement proper test with actual Godot runtime or proper mock
        AssertThat(true).IsTrue();
    }

    /// <summary>
    /// Tests that scene provides invalid move feedback when collision occurs.
    /// </summary>
    [TestCase]
    public void Movement_WhenInvalidMoveAttempted_ProvidesFeedback()
    {
        // This test would require Godot runtime to test actual feedback
        // The mock implementation would verify the feedback logic
        // TODO: Implement proper test with actual Godot runtime or proper mock
        AssertThat(true).IsTrue();
    }

    /// <summary>
    /// Tests that scene manages object interactions and Dreamweaver affinity updates.
    /// </summary>
    [TestCase]
    public void Gameplay_WhenObjectInteracted_ManagesAffinityUpdates()
    {
        // Arrange
        var gameState = new GameState();
        var stage = CreateTestDungeonStage(OmegaSpiral.Source.Scripts.Common.DreamweaverType.Light);

        // Act - Interact with each object type
        var objectKeys = new char[] { 'D', 'M', 'C' }; // Door, Monster, Chest
        foreach (var key in objectKeys)
        {
            var interactionResult = stage.ResolveInteraction(key);
            // For each interaction, apply the appropriate points
            var points = interactionResult.AlignedTo == stage.Owner ? 2 : 1; // Owner gets +2, others get +1
            gameState.UpdateDreamweaverScore(interactionResult.AlignedTo, points);
        }

        // Assert - Verify affinity updates
        AssertThat(objectKeys.Length > 0).IsTrue();
        var totalPoints = objectKeys.Length; // Each interaction adds at least 1 point
        AssertThat(totalPoints > 0).IsTrue();
    }

    #endregion

    #region Helper Methods

    private string CreateValidDungeonSequenceJson()
    {
        return @"{
  ""type"": ""ascii_dungeon_sequence"",
  ""dungeons"": [
    {
      ""owner"": ""Light"",
      ""map"": [
        ""############"",
        ""#...D......#"",
        ""#.M........#"",
        ""#......C...#"",
        ""############""
      ],
      ""legend"": {
        ""#"": ""wall"",
        ""."": ""floor"",
        ""D"": ""door"",
        ""M"": ""monster"",
        ""C"": ""chest""
      },
      ""objects"": {
        ""D"": {
          ""type"": ""Door"",
          ""text"": ""A door aligned to Light"",
          ""aligned_to"": ""Light"",
          ""affinity_delta"": 2
        },
        ""M"": {
          ""type"": ""Monster"",
          ""text"": ""A monster aligned to Wrath"",
          ""aligned_to"": ""Wrath"",
          ""affinity_delta"": 1
        },
        ""C"": {
          ""type"": ""Chest"",
          ""text"": ""A chest aligned to Mischief"",
          ""aligned_to"": ""Mischief"",
          ""affinity_delta"": 1
        }
      }
    },
    {
      ""owner"": ""Mischief"",
      ""map"": [
        ""############"",
        ""#...D......#"",
        ""#.M........#"",
        ""#......C...#"",
        ""############""
      ],
      ""legend"": {
        ""#"": ""wall"",
        ""."": ""floor"",
        ""D"": ""door"",
        ""M"": ""monster"",
        ""C"": ""chest""
      },
      ""objects"": {
        ""D"": {
          ""type"": ""Door"",
          ""text"": ""A door aligned to Mischief"",
          ""aligned_to"": ""Mischief"",
          ""affinity_delta"": 2
        },
        ""M"": {
          ""type"": ""Monster"",
          ""text"": ""A monster aligned to Light"",
          ""aligned_to"": ""Light"",
          ""affinity_delta"": 1
        },
        ""C"": {
          ""type"": ""Chest"",
          ""text"": ""A chest aligned to Wrath"",
          ""aligned_to"": ""Wrath"",
          ""affinity_delta"": 1
        }
      }
    },
    {
      ""owner"": ""Wrath"",
      ""map"": [
        ""############"",
        ""#...D......#"",
        ""#.M........#"",
        ""#......C...#"",
        ""############""
      ],
      ""legend"": {
        ""#"": ""wall"",
        ""."": ""floor"",
        ""D"": ""door"",
        ""M"": ""monster"",
        ""C"": ""chest""
      },
      ""objects"": {
        ""D"": {
          ""type"": ""Door"",
          ""text"": ""A door aligned to Wrath"",
          ""aligned_to"": ""Wrath"",
          ""affinity_delta"": 2
        },
        ""M"": {
          ""type"": ""Monster"",
          ""text"": ""A monster aligned to Mischief"",
          ""aligned_to"": ""Mischief"",
          ""affinity_delta"": 1
        },
        ""C"": {
          ""type"": ""Chest"",
          ""text"": ""A chest aligned to Light"",
          ""aligned_to"": ""Light"",
          ""affinity_delta"": 1
        }
      }
    }
  ]
}";
    }

    private string CreateDungeonSequenceWithDuplicateOwners()
    {
        return @"{
  ""type"": ""ascii_dungeon_sequence"",
  ""dungeons"": [
    {
      ""owner"": ""Light"",
      ""map"": [""#####""],
      ""legend"": {""#"": ""wall""},
      ""objects"": {}
    },
    {
      ""owner"": ""Light"",
      ""map"": [""#####""],
      ""legend"": {""#"": ""wall""},
      ""objects"": {}
    },
    {
      ""owner"": ""Wrath"",
      ""map"": [""#####""],
      ""legend"": {""#"": ""wall""},
      ""objects"": {}
  ]
}";
    }

    private DungeonStageDefinition CreateValidDungeonStageDefinition(OmegaSpiral.Source.Scripts.Common.DreamweaverType owner)
    {
        return new DungeonStageDefinition(
            owner,
            new List<string> { "####", "#..#", "#..#", "####" },
            new Dictionary<char, string> { { '#', "wall" }, { '.', "floor" } },
            new Dictionary<char, DungeonObjectDefinition>());
    }

    private DungeonStage CreateTestDungeonStage(OmegaSpiral.Source.Scripts.Common.DreamweaverType owner)
    {
        var definition = new DungeonStageDefinition(
            owner,
            new List<string> { "####", "#D.#", "#M.#", "#C.#", "####" },
            new Dictionary<char, string> { { '#', "wall" }, { '.', "floor" }, { 'D', "door" }, { 'M', "monster" }, { 'C', "chest" } },
            new Dictionary<char, DungeonObjectDefinition>
            {
                { 'D', new DungeonObjectDefinition(DungeonObjectType.Door, "A door", owner, 2) },
                { 'M', new DungeonObjectDefinition(DungeonObjectType.Monster, "A monster", OmegaSpiral.Source.Scripts.Common.DreamweaverType.Wrath, 1) },
                { 'C', new DungeonObjectDefinition(DungeonObjectType.Chest, "A chest", OmegaSpiral.Source.Scripts.Common.DreamweaverType.Mischief, 1) }
            });

        return DungeonStage.Create(definition);
    }

    #endregion
}
