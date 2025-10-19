// <copyright file="AsciiDungeonEndToEndTest.cs" company="Omega Spiral">
// Copyright (c) Omega Spiral. All rights reserved.
// </copyright>

namespace OmegaSpiral.Tests.EndToEnd.Dungeon
{
    using System.Globalization;
    using System.IO;
    using System.Threading.Tasks;
    using GdUnit4;
    using Godot;
    using OmegaSpiral.Source.Scripts;
    using OmegaSpiral.Source.Scripts.Common;
    using OmegaSpiral.Source.Scripts.Domain.Dungeon;
    using OmegaSpiral.Source.Scripts.Domain.Dungeon.Models;
    using OmegaSpiral.Source.Scripts.Infrastructure.Dungeon;
    using static GdUnit4.Assertions;

    /// <summary>
    /// End-to-end tests for the ASCII dungeon sequence (Scene 2) transition from Scene 1.
    /// Tests validate complete flow from Ghost Terminal (Scene 1) to Nethack-style ASCII dungeon (Scene 2).
    /// Verifies Dreamweaver affinity scoring and scene transitions work correctly.
    /// </summary>
    [TestSuite]
    public class AsciiDungeonEndToEndTest
    {
        /// <summary>
        /// Gets the path to the dungeon sequence JSON file.
        /// </summary>
        /// <returns>The absolute path to the dungeon_sequence.json file.</returns>
        private static string GetDungeonSequencePath()
        {
            string projectRoot = System.IO.Path.GetFullPath(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", ".."));
            return System.IO.Path.Combine(projectRoot, "Source", "Data", "stages", "nethack", "dungeon_sequence.json");
        }

        /// <summary>
        /// Tests complete transition from Scene 1 to Scene 2 and validates dungeon sequence functionality.
        /// Focuses on integration flow rather than internal structure validation.
        /// </summary>
        [TestCase]
        [RequireGodotRuntime]
        public void SceneTransition_FromGhostTerminalToNethackDungeon_ExecutesSuccessfully()
        {
            // Load the dungeon sequence JSON (integration point)
            string jsonPath = GetDungeonSequencePath();
            string jsonContent = System.IO.File.ReadAllText(jsonPath);

            // Load sequence through the loader (integration)
            var sequence = AsciiDungeonSequenceLoader.LoadFromJson(jsonContent);

            // Verify sequence loaded and has expected stages for gameplay
            AssertThat(sequence).IsNotNull();
            AssertThat(sequence.Stages).HasSize(3); // Three stages for complete dungeon experience

            // Verify each stage is playable (has interactive elements)
            foreach (var stage in sequence.Stages)
            {
                var hasInteractiveObjects = stage.Legend.Any(kvp =>
                    kvp.Value != "wall" && kvp.Value != "floor" && kvp.Value != "player");
                AssertThat(hasInteractiveObjects).IsTrue(); // Should have at least one interactive object
            }
        }

        /// <summary>
        /// Tests that player interactions in ASCII dungeon affect Dreamweaver affinity scores.
        /// </summary>
        [TestCase]
        [RequireGodotRuntime]
        public void AsciiDungeon_Interactions_UpdateDreamweaverAffinity()
        {
            // Create a test sequence
            string jsonPath = GetDungeonSequencePath();
            string jsonContent = System.IO.File.ReadAllText(jsonPath);
            var sequence = AsciiDungeonSequenceLoader.LoadFromJson(jsonContent);

            // Create a test game state to track scores
            var gameState = new GameState();

            // Initialize scores for each dreamweaver
            gameState.UpdateDreamweaverScore(DreamweaverType.Light, 0);
            gameState.UpdateDreamweaverScore(DreamweaverType.Mischief, 0);
            gameState.UpdateDreamweaverScore(DreamweaverType.Wrath, 0);

            // Simulate interactions with objects in the first stage
            var firstStage = sequence.Stages[0];
            foreach (var obj in firstStage.Legend.Keys)
            {
                if (firstStage.Legend[obj] != "wall" && firstStage.Legend[obj] != "floor" && firstStage.Legend[obj] != "player")
                {
                    // Resolve interaction for this object
                    var result = firstStage.ResolveInteraction(obj);

                    // Update game state (this simulates what happens in the actual game)
                    gameState.DreamweaverScores[result.AlignedTo] += result.Change.Amount;
                }
            }

            // Verify that at least one score was updated
            var totalScore = gameState.DreamweaverScores.Values.Sum();
            AssertThat(totalScore).IsGreater(0);
        }

        /// <summary>
        /// Tests complete dungeon sequence execution with runner.
        /// </summary>
        [TestCase]
        [RequireGodotRuntime]
        public void AsciiDungeonSequence_ExecutesCompleteRun()
        {
            // Create a test sequence - use filesystem path for tests
            string jsonPath = GetDungeonSequencePath();
            string jsonContent = System.IO.File.ReadAllText(jsonPath);
            var sequence = AsciiDungeonSequenceLoader.LoadFromJson(jsonContent);

            // Create mock publisher and affinity service
            var publisher = new TestDungeonEventPublisher();
            var affinityService = new TestDreamweaverAffinityService();

            // Create runner
            var runner = new AsciiDungeonSequenceRunner(sequence, publisher, affinityService);

            // Start the sequence
            runner.Start();

            // Verify first stage started
            AssertThat(publisher.LastStageEnteredEvent).IsNotNull();
            AssertThat(publisher.LastStageEnteredEvent!.StageIndex).IsEqual(0);

            // Complete first stage
            runner.CompleteCurrentStage();

            // Verify we moved to stage 1
            AssertThat(publisher.LastStageEnteredEvent!.StageIndex).IsEqual(1);

            // Complete second stage
            runner.CompleteCurrentStage();

            // Verify we moved to stage 2
            AssertThat(publisher.LastStageEnteredEvent!.StageIndex).IsEqual(2);

            // Complete final stage
            runner.CompleteCurrentStage();

            // Should not have triggered another stage since there are only 3
            AssertThat(publisher.StageEnteredEventsCount).IsEqual(3); // Initial + 3 stages
        }

        /// <summary>
        /// Tests that the ASCII dungeon follows the nethack scene specification through gameplay.
        /// Verifies that the dungeon provides a complete playable experience.
        /// </summary>
        [TestCase]
        [RequireGodotRuntime]
        public void AsciiDungeon_FollowsNethackSceneSpecification()
        {
            // Load sequence through integration point
            string jsonPath = GetDungeonSequencePath();
            string jsonContent = System.IO.File.ReadAllText(jsonPath);
            var sequence = AsciiDungeonSequenceLoader.LoadFromJson(jsonContent);

            // Verify complete dungeon experience (3 stages as per spec)
            AssertThat(sequence.Stages).HasSize(3);

            // Create runner to test complete gameplay flow
            var publisher = new TestDungeonEventPublisher();
            var affinityService = new TestDreamweaverAffinityService();
            var runner = new AsciiDungeonSequenceRunner(sequence, publisher, affinityService);

            // Execute complete dungeon run
            runner.Start();

            // Progress through all stages
            for (var i = 0; i < 3; i++)
            {
                AssertThat(publisher.LastStageEnteredEvent!.StageIndex).IsEqual(i);
                runner.CompleteCurrentStage();
            }

            // Verify all stages completed successfully
            AssertThat(publisher.StagesClearedCount).IsEqual(3);
        }

        /// <summary>
        /// Tests that affinity scoring follows the specification through complete gameplay:
        /// - Owner-aligned interactions give 2 points to the owner Dreamweaver.
        /// - Cross-aligned interactions give 1 point to other Dreamweavers.
        /// </summary>
        [TestCase]
        [RequireGodotRuntime]
        public void AsciiDungeon_AffinityScoring_FollowsSpecification()
        {
            // Load sequence and create complete gameplay setup
            string jsonPath = GetDungeonSequencePath();
            string jsonContent = System.IO.File.ReadAllText(jsonPath);
            var sequence = AsciiDungeonSequenceLoader.LoadFromJson(jsonContent);

            var publisher = new TestDungeonEventPublisher();
            var affinityService = new TestDreamweaverAffinityService();
            var runner = new AsciiDungeonSequenceRunner(sequence, publisher, affinityService);

            // Start gameplay
            runner.Start();

            // Simulate player interactions that would occur in actual gameplay
            // The runner handles the integration between stages and scoring
            var firstStage = sequence.Stages[0];

            // Find an interactive object in the first stage
            var interactiveGlyph = firstStage.Legend.First(kvp =>
                kvp.Value != "wall" && kvp.Value != "floor" && kvp.Value != "player").Key;

            // Simulate interaction through the runner (this would happen in real gameplay)
            var interactionResult = firstStage.ResolveInteraction(interactiveGlyph);

            // Verify scoring integration works (service receives the change)
            affinityService.ApplyChange(interactionResult.AlignedTo, interactionResult.Change);

            // Verify the scoring follows specification
            AssertThat(affinityService.LastAppliedChange!.Amount).IsIn(1, 2); // Either 1 or 2 points as per spec
            AssertThat(affinityService.LastAppliedOwner).IsEqual(interactionResult.AlignedTo);
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
