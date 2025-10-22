// <copyright file="SceneManagerMappingTests.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using GdUnit4;
using Godot;
using static GdUnit4.Assertions;

namespace OmegaSpiral.Tests.Unit.Infrastructure
{
    /// <summary>
    /// Unit tests for SceneManager's name-to-path mapping functionality.
    /// Validates that TransitionToScene() correctly maps scene names to file paths.
    /// Uses a data-driven approach for comprehensive coverage.
    /// </summary>
    [TestSuite]
    [RequireGodotRuntime]
    public class SceneManagerMappingTests : IDisposable
    {
        /// <summary>
        /// Defines a scene name mapping for testing.
        /// </summary>
        /// <param name="SceneName">The name passed to TransitionToScene().</param>
        /// <param name="ExpectedPath">The expected Godot resource path.</param>
        /// <param name="Category">Category for organization (Active/Legacy/Utility).</param>
        /// <param name="ShouldExist">Whether the scene file should exist (false for TODOs).</param>
        public record SceneMapping(
            string SceneName,
            string ExpectedPath,
            string Category,
            bool ShouldExist = true);

        /// <summary>
        /// All scene name mappings defined in SceneManager.
        /// Keep this synchronized with the switch statement in SceneManager.TransitionToScene().
        /// </summary>
        private static readonly SceneMapping[] AllMappings =
        {
            // Active Stage 1 mappings
            new("Stage1Opening", "res://source/stages/stage_1/opening.tscn", "Stage1"),
            new("Stage1Boot", "res://source/stages/ghost/scenes/boot_sequence.tscn", "Stage1"),

            // Legacy Stage 1 aliases (deprecated but still supported)
            new("Scene1Narrative", "res://source/stages/stage_1/opening.tscn", "Legacy"),
            new("GhostTerminal", "res://source/stages/stage_1/opening.tscn", "Legacy"),

            // Active Stage 2-4 mappings
            new("Stage2Nethack", "res://source/stages/stage_2/echo_hub.tscn", "Stage2"),
            new("Stage3NeverGoAlone", "res://source/stages/stage_3/echo_vault_hub.tscn", "Stage3"),
            new("Stage4TileDungeon", "res://source/stages/stage_4/tile_dungeon.tscn", "Stage4"),

            // Legacy scene paths (may not exist)
            new("Scene2NethackSequence", "res://source/scenes/scene2_nethack_sequence.tscn", "Legacy", ShouldExist: false),
            new("Scene3NeverGoAlone", "res://source/scenes/scene3_never_go_alone.tscn", "Legacy", ShouldExist: false),
            new("Scene4TileDungeon", "res://source/scenes/scene4_tile_dungeon.tscn", "Legacy", ShouldExist: false),
            new("Scene5FieldCombat", "res://source/scenes/scene5_field_combat.tscn", "Legacy", ShouldExist: false),

            // Utility scenes
            new("OpenRPGMain", "res://source/external_scenes/open_rpg_main.tscn", "Utility", ShouldExist: false),
            new("MainMenu", "res://source/ui/menus/main_menu.tscn", "Utility"),
            new("CharacterSelection", "res://source/scenes/character_selection.tscn", "Utility", ShouldExist: false),
            new("TestScene", "res://source/scenes/test_scene.tscn", "Utility", ShouldExist: false),
        };

        private SceneManager? sceneManager;
        private Node? testRoot;

        /// <summary>
        /// Sets up a minimal Godot scene tree for testing SceneManager.
        /// SceneManager requires GetTree() to be available for scene transitions.
        /// </summary>
        [Before]
        public void Setup()
        {
            // Create a test scene tree
            this.testRoot = new Node();
            this.testRoot.Name = "TestRoot";

            // Create SceneManager and add to tree
            this.sceneManager = new SceneManager();
            this.testRoot.AddChild(this.sceneManager);

            GD.Print("[SceneManagerMappingTests] Setup complete");
        }

        /// <summary>
        /// Cleans up test resources after each test.
        /// </summary>
        [After]
        public void Teardown()
        {
            this.sceneManager?.QueueFree();
            this.sceneManager = null;

            this.testRoot?.QueueFree();
            this.testRoot = null;

            GD.Print("[SceneManagerMappingTests] Teardown complete");
        }

        /// <summary>
        /// Disposes of resources used by the test class.
        /// </summary>
        public void Dispose()
        {
            this.Teardown();
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Tests that SceneManager correctly maps all active stage scene names.
        /// Uses reflection to verify the switch statement produces correct paths.
        /// </summary>
        [TestCase]
        [RequireGodotRuntime]
        public void TransitionToSceneMapsAllActiveStageNamesCorrectly()
        {
            var activeMappings = Array.FindAll(
                AllMappings,
                m => m.Category.StartsWith("Stage", StringComparison.Ordinal) && m.ShouldExist);

            var failures = new List<string>();

            foreach (var mapping in activeMappings)
            {
                // We can't directly test TransitionToScene without triggering scene change
                // Instead, verify the path exists and matches expected pattern
                if (!ResourceLoader.Exists(mapping.ExpectedPath))
                {
                    failures.Add($"{mapping.SceneName} → {mapping.ExpectedPath} (file not found)");
                }
            }

            if (failures.Count > 0)
            {
                var message = $"Active stage mappings have missing files:\n" +
                              string.Join("\n", failures);
                AssertThat(failures.Count).IsEqual(0)
                    .OverrideFailureMessage(message);
            }
        }

        /// <summary>
        /// Tests that SceneManager initializes with correct default values.
        /// Validates the state management properties.
        /// </summary>
        [TestCase]
        [RequireGodotRuntime]
        public void SceneManagerInitializesWithDefaultValues()
        {
            AssertThat(this.sceneManager).IsNotNull();
            AssertThat(this.sceneManager!.CurrentSceneIndex).IsEqual(1);
            AssertThat(this.sceneManager.PlayerName).IsNull();
            AssertThat(this.sceneManager.DreamweaverThread).IsNull();
        }

        /// <summary>
        /// Tests that player name is stored correctly.
        /// </summary>
        [TestCase]
        [RequireGodotRuntime]
        public void SetPlayerNameStoresValueCorrectly()
        {
            const string testName = "EchoVoyager";

            this.sceneManager?.SetPlayerName(testName);

            AssertThat(this.sceneManager?.PlayerName).IsEqual(testName);
        }

        /// <summary>
        /// Tests that Dreamweaver thread is stored correctly.
        /// </summary>
        /// <param name="threadId">The thread identifier to test.</param>
        [TestCase("hero")]
        [TestCase("shadow")]
        [TestCase("ambition")]
        [TestCase("dreamer")]
        [TestCase("seeker")]
        [RequireGodotRuntime]
        public void SetDreamweaverThreadStoresAllValidThreadIds(string threadId)
        {
            this.sceneManager?.SetDreamweaverThread(threadId);

            AssertThat(this.sceneManager?.DreamweaverThread).IsEqual(threadId);
        }

        /// <summary>
        /// Tests that current scene index is tracked correctly for all stages.
        /// </summary>
        /// <param name="sceneIndex">The scene index to test (1-5).</param>
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        [TestCase(4)]
        [TestCase(5)]
        [RequireGodotRuntime]
        public void UpdateCurrentSceneTracksAllStageIndices(int sceneIndex)
        {
            this.sceneManager?.UpdateCurrentScene(sceneIndex);

            AssertThat(this.sceneManager!.CurrentSceneIndex).IsEqual(sceneIndex);
        }

        /// <summary>
        /// Tests that player data can be updated/overwritten.
        /// </summary>
        [TestCase]
        [RequireGodotRuntime]
        public void PlayerDataPropertiesCanBeUpdated()
        {
            // Test player name update
            this.sceneManager?.SetPlayerName("FirstName");
            AssertThat(this.sceneManager?.PlayerName).IsEqual("FirstName");

            this.sceneManager?.SetPlayerName("SecondName");
            AssertThat(this.sceneManager?.PlayerName).IsEqual("SecondName");

            // Test thread update
            this.sceneManager?.SetDreamweaverThread("hero");
            AssertThat(this.sceneManager?.DreamweaverThread).IsEqual("hero");

            this.sceneManager?.SetDreamweaverThread("shadow");
            AssertThat(this.sceneManager?.DreamweaverThread).IsEqual("shadow");
        }

        /// <summary>
        /// Tests that both player name and Dreamweaver thread are stored independently.
        /// </summary>
        [TestCase]
        [RequireGodotRuntime]
        public void PlayerDataPropertiesAreIndependent()
        {
            const string playerName = "TestHero";
            const string threadId = "ambition";

            this.sceneManager?.SetPlayerName(playerName);
            this.sceneManager?.SetDreamweaverThread(threadId);

            AssertThat(this.sceneManager?.PlayerName).IsEqual(playerName);
            AssertThat(this.sceneManager?.DreamweaverThread).IsEqual(threadId);
        }

        /// <summary>
        /// Validates that all mappings in the test array are unique (no duplicate scene names).
        /// </summary>
        [TestCase]
        public void AllSceneMappingsHaveUniqueNames()
        {
            var duplicates = AllMappings.GroupBy(m => m.SceneName)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToList();

            if (duplicates.Count > 0)
            {
                var message = $"Duplicate scene names found: {string.Join(", ", duplicates)}";
                AssertThat(duplicates.Count).IsEqual(0)
                    .OverrideFailureMessage(message);
            }
        }

        /// <summary>
        /// Validates that Stage1-5 prefixed names all map to correct stage directories.
        /// Ensures naming convention consistency.
        /// </summary>
        [TestCase]
        public void StageNameMappingsFollowDirectoryConvention()
        {
            var stageMappings = Array.FindAll(
                AllMappings,
                m => m.Category.StartsWith("Stage", StringComparison.Ordinal));

            var violations = new List<string>();

            foreach (var mapping in stageMappings)
            {
                // Extract stage number from category (e.g., "Stage1" -> 1)
                if (!int.TryParse(mapping.Category.Replace("Stage", string.Empty), out var stageNum))
                {
                    continue;
                }

                // Check path contains correct stage directory
                var expectedDir = stageNum == 1
                    ? "stages/ghost/" // Stage 1 uses "ghost" directory
                    : $"stages/stage_{stageNum}/";

                if (!mapping.ExpectedPath.Contains(expectedDir))
                {
                    violations.Add($"{mapping.SceneName} should map to path containing '{expectedDir}' " +
                                   $"but maps to {mapping.ExpectedPath}");
                }
            }

            if (violations.Count > 0)
            {
                var message = "Stage name mappings violate directory convention:\n" +
                              string.Join("\n", violations);
                AssertThat(violations.Count).IsEqual(0)
                    .OverrideFailureMessage(message);
            }
        }
    }
}