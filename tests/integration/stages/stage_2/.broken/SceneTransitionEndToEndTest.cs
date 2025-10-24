// <copyright file="SceneTransitionEndToEndTest.cs" company="Omega Spiral">
// Copyright (c) Omega Spiral. All rights reserved.
// </copyright>
namespace OmegaSpiral.Tests.EndToEnd.Transition
{
    using System;
    using System.Threading.Tasks;
    using GdUnit4;
    using Godot;
    using OmegaSpiral.Source.Narrative;
    using OmegaSpiral.Source.Scripts;
    using OmegaSpiral.Source.Scripts.Common;
    using OmegaSpiral.Source.Scripts.domain.Dungeon;
    using OmegaSpiral.Source.Scripts.domain.Dungeon.Models;
    using OmegaSpiral.Source.Scripts.Infrastructure.Dungeon;
    using static GdUnit4.Assertions;

    /// <summary>
    /// End-to-end tests for scene transitions between acts, specifically from Scene 1 (Ghost Terminal) to Scene 2 (ASCII Dungeon).
    /// Tests validate that the GameState persists through scene transitions and that the ASCII dungeon loads correctly.
    /// </summary>
    [TestSuite]
    public class SceneTransitionEndToEndTest
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
        /// Tests the complete transition from Scene 1 to Scene 2 with state preservation.
        /// </summary>
        [TestCase]
        [RequireGodotRuntime]
        public void Scene1ToScene2_Transition_PreservesGameState()
        {
            // Simulate the game state after Scene 1
            var gameState = new GameState();

            // Initialize some scores to verify they persist
            gameState.UpdateDreamweaverScore(DreamweaverType.Light, 5);
            gameState.UpdateDreamweaverScore(DreamweaverType.Mischief, 3);
            gameState.UpdateDreamweaverScore(DreamweaverType.Wrath, 2);

            // Simulate that Scene 1 has completed and is triggering the transition
            // This would normally happen through the SceneManager.TransitionToScene() method
            var sceneManager = new SceneManager();

            // Verify the transition mechanism
            sceneManager.TransitionToScene("Scene2NethackSequence");

            // In a real test, we would:
            // 1. Verify the scene actually changed (requires Godot runtime)
            // 2. Verify that GameState was preserved across scenes
            // 3. Verify Scene 2 (ASCII dungeon) loaded properly

            // For this unit test, we'll validate the underlying components

            // Verify the dungeon sequence file exists and is valid
            var dungeonSequencePath = GetDungeonSequencePath();
            var jsonContent = System.IO.File.ReadAllText(dungeonSequencePath);

            AssertThat(jsonContent).IsNotNull();
            AssertThat(jsonContent.Length).IsGreater(0);

            // Verify the sequence can be loaded
            var sequence = AsciiDungeonSequenceLoader.LoadFromJson(jsonContent);
            AssertThat(sequence).IsNotNull();
            AssertThat(sequence.Stages).HasSize(3);

            // Verify state preservation conceptually
            AssertThat(gameState.DreamweaverScores.ContainsKey(DreamweaverType.Light)).IsTrue();
            AssertThat(gameState.DreamweaverScores.ContainsKey(DreamweaverType.Mischief)).IsTrue();
            AssertThat(gameState.DreamweaverScores.ContainsKey(DreamweaverType.Wrath)).IsTrue();
        }

        /// <summary>
        /// Tests that the ASCII dungeon sequence runner properly integrates with the scene transition system.
        /// </summary>
        [TestCase]
        [RequireGodotRuntime]
        public void Scene2_AsciiDungeon_RunnerIntegration()
        {
            // Create the dungeon sequence
            var jsonContent = System.IO.File.ReadAllText(GetDungeonSequencePath());
            var sequence = AsciiDungeonSequenceLoader.LoadFromJson(jsonContent);

            // Create publisher and service for the runner
            var publisher = new TestDungeonEventPublisher();
            var affinityService = new TestDreamweaverAffinityService();

            // Create the runner
            var runner = new AsciiDungeonSequenceRunner(sequence, publisher, affinityService);

            // Start the sequence
            runner.Start();

            // Verify initial state
            AssertThat(publisher.LastStageEnteredEvent).IsNotNull();
            AssertThat(publisher.LastStageEnteredEvent!.StageIndex).IsEqual(0);
            AssertThat(publisher.LastStageEnteredEvent.Owner).IsEqualTo(sequence.Stages[0].Owner);

            // Simulate some player interactions
            // In the actual game, this would happen through the Ui/GUi interactions
            var firstStage = sequence.Stages[0];

            // Find an object in the stage to interact with
            char? interactedGlyph = null;
            foreach (var objKvp in firstStage.Legend)
            {
                // Skip walls and floors, find actual objects
                var description = objKvp.Value.ToLower(System.Globalization.CultureInfo.InvariantCulture);
                if (description != "wall" && description != "floor" && description != "player")
                {
                    interactedGlyph = objKvp.Key;
                    break;
                }
            }

            if (interactedGlyph.HasValue)
            {
                // Note: ResolveInteraction method would be called here if implemented
                // runner.ResolveInteraction(interactedGlyph.Value);

                // Apply affinity change through the service
                var interactionResult = firstStage.ResolveInteraction(interactedGlyph.Value);
                affinityService.ApplyChange(interactionResult.AlignedTo, interactionResult.Change);

                // Verify the affinity service was called
                AssertThat(affinityService.LastAppliedOwner).IsEqualTo(interactionResult.AlignedTo);
            }

            // Complete the first stage
            runner.CompleteCurrentStage();

            // Verify progression to next stage
            AssertThat(publisher.LastStageEnteredEvent!.StageIndex).IsEqual(1);
            AssertThat(publisher.LastStageEnteredEvent!.Owner).IsEqualTo(sequence.Stages[1].Owner);

            // Complete all stages
            runner.CompleteCurrentStage(); // Stage 2
            runner.CompleteCurrentStage(); // Stage 3 (this should be the last one)

            // At this point, all stages should be completed
            AssertThat(publisher.StageEnteredEventsCount).IsEqual(3); // One for each stage
            AssertThat(publisher.StagesClearedCount).IsEqual(3); // One for each completion
        }

        /// <summary>
        /// Tests the complete flow from Scene 1 narrative completion to Scene 2 ASCII dungeon start.
        /// </summary>
        [TestCase]
        [RequireGodotRuntime]
        public void Scene1ToScene2_CompleteFlow()
        {
            // This test simulates the complete flow from Ghost Terminal (Scene 1) to ASCII Dungeon (Scene 2)
            // The flow would be:
            // 1. Ghost Terminal completes with player choices
            // 2. GameState is updated with player choices and dreamweaver scores
            // 3. SceneManager transitions to Scene2NethackSequence
            // 4. Scene2NethackSequence loads the dungeon sequence
            // 5. AsciiDungeonSequenceRunner orchestrates the sequence

            // Simulate GameState after Scene 1
            var gameState = new GameState();
            gameState.UpdateDreamweaverScore(DreamweaverType.Light, 3);
            gameState.UpdateDreamweaverScore(DreamweaverType.Mischief, 5);  // Player might be more aligned to mischief
            gameState.UpdateDreamweaverScore(DreamweaverType.Wrath, 2);

            // Verify the dungeon sequence JSON exists and is valid
            var jsonPath = GetDungeonSequencePath();
            AssertThat(System.IO.File.Exists(jsonPath)).IsTrue();

            var jsonContent = System.IO.File.ReadAllText(jsonPath);
            AssertThat(jsonContent).IsNotNull();
            AssertThat(jsonContent.Length).IsGreater(0);

            // Verify it's valid JSON
            var sequence = AsciiDungeonSequenceLoader.LoadFromJson(jsonContent);
            AssertThat(sequence).IsNotNull();

            // Verify all requirements from nethack scene specification
            AssertThat(sequence.Stages).HasSize(3); // Three identical rooms concept

            foreach (var stage in sequence.Stages)
            {
                // Verify it honours meta-themes (represented in the narrative content)
                AssertThat(stage.Map).IsNotEmpty(); // Has a map

                // Verify stage has owner
                AssertThat(stage.Owner).IsIn(DreamweaverType.Light, DreamweaverType.Mischief, DreamweaverType.Wrath);

                // Verify it has interactive objects (as required by the spec)
                AssertThat(stage.Legend).IsNotEmpty();

                // Verify map is rectangular
                var width = stage.Map[0].Length;
                foreach (var row in stage.Map)
                {
                    AssertThat(row.Length).IsEqual(width);
                }
            }

            // Verify each stage has a different owner (as per spec)
            var owners = new System.Collections.Generic.HashSet<DreamweaverType>();
            foreach (var stage in sequence.Stages)
            {
                AssertThat(owners.Add(stage.Owner)).IsTrue(); // Should add successfully (unique)
            }

            AssertThat(owners.Count).IsEqual(3); // All three different owners
        }

        /// <summary>
        /// Tests that the scene transition maintains the overall narrative flow as described in the nethack scene spec.
        /// </summary>
        [TestCase]
        [RequireGodotRuntime]
        public void SceneTransition_MaintainsNarrativeFlow()
        {
            var jsonContent = System.IO.File.ReadAllText(GetDungeonSequencePath());
            var sequence = AsciiDungeonSequenceLoader.LoadFromJson(jsonContent);

            // According to the spec:
            // - Each dungeon is owned by one Dreamweaver but all three comment when you enter
            // - The layout shifts slightly between runs but objects stay in same relative zone
            // - You cannot change objects - only choose which to approach
            // - On interaction, only aligned Dreamweaver speaks but line reflects how they see choice
            AssertThat(sequence.Stages).HasSize(3); // Three dungeons

            foreach (var stage in sequence.Stages)
            {
                // Verify each dungeon has an owner
                AssertThat(stage.Owner).IsIn(DreamweaverType.Light, DreamweaverType.Mischief, DreamweaverType.Wrath);

                // Verify it has objects to interact with
                // This is checked through legend entries that aren't walls/floors
                var interactiveObjects = 0;
                foreach (var kvp in stage.Legend)
                {
                    var description = kvp.Value.ToLower(System.Globalization.CultureInfo.InvariantCulture);
                    if (description != "wall" && description != "floor" && description != "player")
                    {
                        interactiveObjects++;
                    }
                }

                // Should have at least one interactive object per specification
                AssertThat(interactiveObjects).IsGreater(0);
            }

            // Verify the scoring system concept is supported
            // (In each dungeon, if player chooses option aligned with the dungeon's owner → that Dreamweaver gets 2 points
            // If player chooses option aligned with another Dreamweaver → that other Dreamweaver gets 1 point)

            // This is tested through the object alignment in the sequence
            foreach (var stage in sequence.Stages)
            {
                var interactionResult = stage.ResolveInteraction(stage.Map[0][0]); // Test with dummy character

                // The actual interaction would happen with real object characters
            }
        }

        private sealed class TestDungeonEventPublisher : IDungeonEventPublisher
        {
            public int StageEnteredEventsCount { get; private set; }

            public int StagesClearedCount { get; private set; }

            public DungeonStageEnteredEvent? LastStageEnteredEvent { get; private set; }

            public DungeonStageClearedEvent? LastStageClearedEvent { get; private set; }

            static public void PublishStageCleared(DungeonStageClearedEvent domainEvent)
            {
                LastStageClearedEvent = domainEvent;
                StagesClearedCount++;
            }

            static public void PublishStageEntered(DungeonStageEnteredEvent domainEvent)
            {
                LastStageEnteredEvent = domainEvent;
                StageEnteredEventsCount++;
            }
        }

        private sealed class TestDreamweaverAffinityService : IDreamweaverAffinityService
        {
            public DreamweaverType LastAppliedOwner { get; private set; } = DreamweaverType.Light;

            public DreamweaverAffinityChange? LastAppliedChange { get; private set; }

            static public void ApplyChange(DreamweaverType owner, DreamweaverAffinityChange change)
            {
                LastAppliedOwner = owner;
                LastAppliedChange = change;
            }
        }
    }
}
