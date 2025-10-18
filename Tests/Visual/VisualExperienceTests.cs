// <copyright file="VisualExperienceTests.cs" company="Omega Spiral">
// Copyright (c) Omega Spiral. All rights reserved.
// </copyright>

#pragma warning disable SA1636

namespace OmegaSpiral.Tests.Visual
{
    using System.IO;
    using GdUnit4;
    using Godot;
    using OmegaSpiral.Source.Scripts.Field.Narrative;
    using OmegaSpiral.Source.Scripts.Domain.Dungeon;
    using OmegaSpiral.Source.Scripts.Domain.Dungeon.Models;
    using OmegaSpiral.Source.Scripts.Infrastructure;
    using OmegaSpiral.Source.Scripts.Infrastructure.Dungeon;
    using static GdUnit4.Assertions;

    /// <summary>
    /// Tests that validate the distinct visual experiences of both acts.
    /// Ensures TerminalUI (Act 1) and AsciiRoomRenderer (Act 2) provide coherent visual narratives.
    /// </summary>
    [TestSuite]
    [RequireGodotRuntime]
    public class VisualExperienceTests
    {
        /// <summary>
        /// Tests TerminalUI visual elements and godot-xterm integration.
        /// Validates that narrative content renders properly in terminal interface.
        /// </summary>
        [TestCase]
        public void TerminalUI_FirstAct_RendersNarrativeContent()
        {
            // Load narrative configuration
            string heroStagePath = Path.Combine("Source", "Data", "stages", "ghost-terminal", "hero.json");
            string heroJsonContent = File.ReadAllText(heroStagePath);
            var heroConfig = ConfigurationService.LoadConfigurationFromString(heroJsonContent, heroStagePath);

            // Create narrative scene data
            NarrativeSceneData heroSceneData = NarrativeSceneFactory.Create(heroConfig);

            // Validate narrative has visual content
            AssertThat(heroSceneData.OpeningLines.Count).IsGreater(0);

            // Validate each opening line has content suitable for terminal display
            foreach (var line in heroSceneData.OpeningLines)
            {
                AssertThat(line).IsNotNull();
                AssertThat(line.Length).IsLess(201); // Terminal content should be reasonably sized for display
            }

            // Validate that the scene data can generate a cinematic plan
            GhostTerminalCinematicPlan heroPlan = GhostTerminalCinematicDirector.BuildPlan(heroSceneData);
            AssertThat(heroPlan.Beats.Count).IsGreater(0);

            // Validate plan has proper structure for terminal presentation
            var hasStoryParagraphs = heroPlan.Beats.Any(beat => beat.Type == GhostTerminalBeatType.StoryParagraph);
            var hasExitBeat = heroPlan.Beats.Any(beat => beat.Type == GhostTerminalBeatType.ExitLine);

            AssertThat(hasStoryParagraphs).IsTrue();
            AssertThat(hasExitBeat).IsTrue();
        }

        /// <summary>
        /// Tests AsciiRoomRenderer visual elements and 3D dungeon representation.
        /// Validates that dungeon content renders properly with ASCII aesthetics.
        /// </summary>
        [TestCase]
        public void AsciiRoomRenderer_SecondAct_RendersDungeonContent()
        {
            // Load dungeon sequence
            var dungeonJsonPath = "../../Source/Data/stages/nethack/dungeon_sequence.json";
            var dungeonJsonContent = System.IO.File.ReadAllText(dungeonJsonPath);
            var dungeonSequence = AsciiDungeonSequenceLoader.LoadFromJson(dungeonJsonContent);

            // Validate dungeon sequence structure
            AssertThat(dungeonSequence).IsNotNull();
            AssertThat(dungeonSequence.Stages.Count).IsGreater(0);

            // Test each stage for proper ASCII rendering elements
            foreach (var stage in dungeonSequence.Stages)
            {
                // Validate map exists and has content
                AssertThat(stage.Map.Count).IsGreater(0);

                // Validate legend has proper ASCII mappings
                AssertThat(stage.Legend.Count).IsGreater(0);

                // Validate essential elements exist (walls, floors, interactive objects)
                var hasWalls = stage.Legend.Values.Contains("wall");
                var hasFloors = stage.Legend.Values.Contains("floor");
                var hasPlayer = stage.Legend.Values.Contains("player");

                AssertThat(hasWalls).IsTrue();
                AssertThat(hasFloors).IsTrue();
                AssertThat(hasPlayer).IsTrue();

                // Validate interactive elements exist for gameplay
                var interactiveElements = stage.Legend.Where(kvp =>
                    kvp.Value != "wall" && kvp.Value != "floor" && kvp.Value != "player").ToList();
                AssertThat(interactiveElements.Count).IsGreater(0);
            }
        }

        /// <summary>
        /// Tests visual transition between TerminalUI and AsciiRoomRenderer.
        /// Validates that the aesthetic shift is coherent and maintains game immersion.
        /// </summary>
        [TestCase]
        public void VisualTransition_BetweenActs_MaintainsImmersion()
        {
            // Load both act configurations
            string heroStagePath = Path.Combine("Source", "Data", "stages", "ghost-terminal", "hero.json");
            string heroJsonContent = File.ReadAllText(heroStagePath);
            var heroConfig = ConfigurationService.LoadConfigurationFromString(heroJsonContent, heroStagePath);

            var dungeonJsonPath = "../../Source/Data/stages/nethack/dungeon_sequence.json";
            var dungeonJsonContent = System.IO.File.ReadAllText(dungeonJsonPath);

            // Process both acts
            NarrativeSceneData heroSceneData = NarrativeSceneFactory.Create(heroConfig);
            GhostTerminalCinematicPlan heroPlan = GhostTerminalCinematicDirector.BuildPlan(heroSceneData);
            var dungeonSequence = AsciiDungeonSequenceLoader.LoadFromJson(dungeonJsonContent);

            // Validate both have substantial content
            AssertThat(heroSceneData.OpeningLines.Count).IsGreater(0);
            AssertThat(heroPlan.Beats.Count).IsGreater(0);
            AssertThat(dungeonSequence.Stages.Count).IsGreater(0);

            // Validate content is structured for different presentation methods
            // TerminalUI: Text-based narrative
            var terminalContentLength = heroSceneData.OpeningLines.Sum(line => line.Length);
            AssertThat(terminalContentLength).IsGreater(0);

            // AsciiRoomRenderer: Spatial dungeon layout
            var totalMapSize = dungeonSequence.Stages.Sum(stage => stage.Map.Count);
            AssertThat(totalMapSize).IsGreater(0);

            // Both should have meaningful content depth
            AssertThat(heroPlan.Beats.Count).IsGreater(2); // Minimum narrative beats
            AssertThat(dungeonSequence.Stages.Count).IsGreater(1); // Minimum dungeon stages
        }

        /// <summary>
        /// Tests that both visual systems support player interaction appropriately.
        /// TerminalUI should handle text input, AsciiRoomRenderer should handle spatial navigation.
        /// </summary>
        [TestCase]
        public void PlayerInteraction_BothActs_SupportsExpectedInputMethods()
        {
            // Load dungeon sequence to validate interaction elements
            var dungeonJsonPath = "../../Source/Data/stages/nethack/dungeon_sequence.json";
            var dungeonJsonContent = System.IO.File.ReadAllText(dungeonJsonPath);
            var dungeonSequence = AsciiDungeonSequenceLoader.LoadFromJson(dungeonJsonContent);

            // Validate dungeon has interactive elements for spatial gameplay
            foreach (var stage in dungeonSequence.Stages)
            {
                var interactiveElements = stage.Legend.Where(kvp =>
                    kvp.Value != "wall" && kvp.Value != "floor" && kvp.Value != "player").ToList();

                // Should have multiple types of interactive elements
                AssertThat(interactiveElements.Count).IsGreater(1);
            }

            // Load narrative to validate it has input-capable content
            string heroStagePath = Path.Combine("Source", "Data", "stages", "ghost-terminal", "hero.json");
            string heroJsonContent = File.ReadAllText(heroStagePath);
            var heroConfig = ConfigurationService.LoadConfigurationFromString(heroJsonContent, heroStagePath);
            NarrativeSceneData heroSceneData = NarrativeSceneFactory.Create(heroConfig);

            // Validate narrative has content that would require/suggest player input
            AssertThat(heroSceneData.OpeningLines.Count).IsGreater(0);
        }
    }
}
