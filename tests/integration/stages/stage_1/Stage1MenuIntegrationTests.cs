// <copyright file="Stage1MenuIntegrationTests.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

namespace OmegaSpiral.Tests.Stages.Stage1;

using Godot;
using GdUnit4;
using GdUnit4.Api;
using static GdUnit4.Assertions;
using OmegaSpiral.Source.Scripts.Infrastructure;
using OmegaSpiral.Ui.Menus;

/// <summary>
/// Integration tests for Stage 1 menu selection and beat progression.
/// Verifies that:
/// 1. MainMenu loads correctly
/// 2. Stage 1 button loads the manifest and gets the first beat
/// 3. Beat scenes can transition to the next beat via the manifest
/// 4. The full beat sequence is wired correctly
/// </summary>
[TestSuite]
[RequireGodotRuntime]
public partial class Stage1MenuIntegrationTests : Node
{
    private const string MainMenuScenePath = "res://source/ui/menus/main_menu.tscn";
    private const string Stage1ManifestPath = "res://source/stages/stage_1/stage_manifest.json";
    private const string Beat1BootSequencePath = "res://source/stages/ghost/scenes/boot_sequence.tscn";
    private const string Beat2OpeningMonologuePath = "res://source/stages/ghost/scenes/opening_monologue.tscn";

    [Before]
    public void Setup()
    {
        // Setup for each test
    }

    [After]
    public void Cleanup()
    {
        // Cleanup after each test
    }

    /// <summary>
    /// Test that the MainMenu scene loads successfully.
    /// </summary>
    [TestCase]
    [RequireGodotRuntime]
    public void MainMenuShouldLoadSuccessfully()
    {
        // Arrange & Act
        var sceneResource = GD.Load<PackedScene>(MainMenuScenePath);

        // Assert
        AssertThat(sceneResource).IsNotNull();
        AssertThat(sceneResource).IsInstanceOf<PackedScene>();
    }

    /// <summary>
    /// Test that Stage 1 manifest loads correctly with all beats.
    /// </summary>
    [TestCase]
    [RequireGodotRuntime]
    public void Stage1ManifestShouldLoadWithAllBeats()
    {
        // Arrange
        var loader = new StageManifestLoader();

        // Act
        var manifest = loader.LoadManifest(Stage1ManifestPath);

        // Assert
        AssertThat(manifest).IsNotNull();
        AssertThat(manifest!.Scenes).IsNotNull();
        AssertThat(manifest.Scenes.Count).IsEqual(8); // 8 beats in Stage 1
        AssertThat(manifest.StageId).IsEqual(1);
    }

    /// <summary>
    /// Test that the manifest's first scene is the boot sequence.
    /// </summary>
    [TestCase]
    [RequireGodotRuntime]
    public void Stage1ManifestFirstSceneShouldBeBootSequence()
    {
        // Arrange
        var loader = new StageManifestLoader();
        var manifest = loader.LoadManifest(Stage1ManifestPath);

        // Act
        var firstScene = manifest!.GetFirstScene();

        // Assert
        AssertThat(firstScene).IsNotNull();
        AssertThat(firstScene!.Id).IsEqual("boot_sequence");
        AssertThat(firstScene.SceneFile).IsEqual(Beat1BootSequencePath);
    }

    /// <summary>
    /// Test that beat progression follows the correct sequence.
    /// </summary>
    [TestCase]
    [RequireGodotRuntime]
    public void Stage1BeatSequenceShouldProgressCorrectly()
    {
        // Arrange
        var loader = new StageManifestLoader();
        var manifest = loader.LoadManifest(Stage1ManifestPath);

        // Act & Assert: Verify sequence
        var beat1 = manifest!.GetFirstScene();
        AssertThat(beat1!.Id).IsEqual("boot_sequence");

        var beat2Id = manifest.GetNextSceneId("boot_sequence");
        AssertThat(beat2Id).IsEqual("opening_monologue");

        var beat2 = manifest.GetScene(beat2Id!);
        AssertThat(beat2).IsNotNull();
        AssertThat(beat2!.SceneFile).IsEqual(Beat2OpeningMonologuePath);

        var beat3Id = manifest.GetNextSceneId("opening_monologue");
        AssertThat(beat3Id).IsEqual("question_1_name");

        // Verify last beat has no next scene (completion)
        var beat8Id = manifest.GetNextSceneId("name_question");
        AssertThat(beat8Id).IsEqual("exit");

        var beat9Id = manifest.GetNextSceneId("exit");
        AssertThat(beat9Id).IsNull();
    }

    /// <summary>
    /// Test that beat scene files can be loaded.
    /// </summary>
    [TestCase]
    [RequireGodotRuntime]
    public void Beat1BootSequenceShouldLoadSuccessfully()
    {
        // Arrange & Act
        var sceneResource = GD.Load<PackedScene>(Beat1BootSequencePath);

        // Assert
        AssertThat(sceneResource).IsNotNull();
        AssertThat(sceneResource).IsInstanceOf<PackedScene>();
    }

    /// <summary>
    /// Test that beat scene files can be loaded.
    /// </summary>
    [TestCase]
    [RequireGodotRuntime]
    public void Beat2OpeningMonologueShouldLoadSuccessfully()
    {
        // Arrange & Act
        var sceneResource = GD.Load<PackedScene>(Beat2OpeningMonologuePath);

        // Assert
        AssertThat(sceneResource).IsNotNull();
        AssertThat(sceneResource).IsInstanceOf<PackedScene>();
    }

    /// <summary>
    /// Test that all 8 beat scenes can be loaded.
    /// </summary>
    [TestCase]
    [RequireGodotRuntime]
    public void AllEightBeatScenesShouldLoadSuccessfully()
    {
        // Arrange
        var beatPaths = new[]
        {
            "res://source/stages/ghost/scenes/boot_sequence.tscn",
            "res://source/stages/ghost/scenes/opening_monologue.tscn",
            "res://source/stages/ghost/scenes/question_1_name.tscn",
            "res://source/stages/ghost/scenes/story_fragment.tscn",
            "res://source/stages/ghost/scenes/secret_question.tscn",
            "res://source/stages/ghost/scenes/secret_reveal.tscn",
            "res://source/stages/ghost/scenes/name_question.tscn",
            "res://source/stages/ghost/scenes/exit.tscn",
        };

        // Act & Assert
        foreach (var beatPath in beatPaths)
        {
            var sceneResource = GD.Load<PackedScene>(beatPath);
            AssertThat(sceneResource).IsNotNull($"Beat scene should load: {beatPath}");
        }
    }

    /// <summary>
    /// Test that the manifest next stage path is configured.
    /// </summary>
    [TestCase]
    [RequireGodotRuntime]
    public void Stage1ManifestShouldSpecifyNextStageTransition()
    {
        // Arrange
        var loader = new StageManifestLoader();
        var manifest = loader.LoadManifest(Stage1ManifestPath);

        // Act & Assert
        AssertThat(manifest!.NextStagePath).IsNotNull();
        AssertThat(manifest.NextStagePath).IsNotEmpty();
        AssertThat(manifest.NextStagePath).Contains("stage_2");
    }

    /// <summary>
    /// Test that Stage 1 button press resolves to the first beat from manifest.
    /// This verifies the integration between MainMenu and StageManifestLoader.
    /// </summary>
    [TestCase]
    [RequireGodotRuntime]
    public void Stage1ButtonShouldLoadFirstBeatFromManifest()
    {
        // Arrange
        var stageManifestLoader = new StageManifestLoader();
        var stageManifest = stageManifestLoader.LoadManifest(Stage1ManifestPath);

        // Act: Simulate what MainMenu.TransitionToStage(1) does
        string? resolvedScenePath = null;

        if (stageManifest != null && stageManifest.Scenes.Count > 0)
        {
            var firstBeat = stageManifest.GetFirstScene();
            if (firstBeat != null)
            {
                resolvedScenePath = firstBeat.SceneFile;
            }
        }

        // Assert
        AssertThat(resolvedScenePath).IsNotNull();
        AssertThat(resolvedScenePath).IsEqual(Beat1BootSequencePath);
    }
}
