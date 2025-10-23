// <copyright file="Stage2MenuIntegrationTests.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

namespace OmegaSpiral.Tests.Stages.Stage2;

using GdUnit4;
using GdUnit4.Api;
using static GdUnit4.Assertions;
using OmegaSpiral.Source.Scripts.Infrastructure;

/// <summary>
/// Integration tests for Stage 2 menu selection and beat progression.
/// Verifies that:
/// 1. Stage 2 manifest loads correctly
/// 2. Beat scenes can transition to the next beat via the manifest
/// 3. The full beat sequence is wired correctly
/// </summary>
[TestSuite]
public partial class Stage2MenuIntegrationTests
{
    private const string Stage2ManifestPath = "res://source/stages/stage_2/stage_2_manifest.json";

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
    /// Test that Stage 2 manifest loads successfully.
    /// </summary>
    [TestCase]
    public void Stage2ManifestShouldLoadSuccessfully()
    {
        // Arrange & Act
        var loader = new StageManifestLoader();

        // Act
        var manifest = loader.LoadManifest(Stage2ManifestPath);

        // Assert
        AssertThat(manifest).IsNotNull();
    }

    /// <summary>
    /// Test that Stage 2 manifest loads with all beats.
    /// </summary>
    [TestCase]
    public void Stage2ManifestShouldLoadWithAllBeats()
    {
        // Arrange
        var loader = new StageManifestLoader();

        // Act
        var manifest = loader.LoadManifest(Stage2ManifestPath);

        // Assert
        AssertThat(manifest).IsNotNull();
        AssertThat(manifest!.Scenes).IsNotNull();
        AssertThat(manifest.Scenes.Count).IsEqual(7); // 7 beats: 3 interludes + 3 chambers + 1 finale
        AssertThat(manifest.StageId).IsEqual(2);
    }

    /// <summary>
    /// Test that the manifest's first scene is interlude 1.
    /// </summary>
    [TestCase]
    public void Stage2ManifestFirstSceneShouldBeInterlude1()
    {
        // Arrange
        var loader = new StageManifestLoader();
        var manifest = loader.LoadManifest(Stage2ManifestPath);

        // Act
        var firstScene = manifest!.GetFirstScene();

        // Assert
        AssertThat(firstScene).IsNotNull();
        AssertThat(firstScene!.Id).IsEqual("beat_1_interlude_light");
    }

    /// <summary>
    /// Test that beat progression follows the correct sequence.
    /// </summary>
    [TestCase]
    public void Stage2BeatProgressionShouldFollowCorrectSequence()
    {
        // Arrange
        var loader = new StageManifestLoader();
        var manifest = loader.LoadManifest(Stage2ManifestPath);

        // Act & Assert - Verify beat sequence
        var beats = manifest!.Scenes;

        AssertThat(beats[0].Id).IsEqual("beat_1_interlude_light");
        AssertThat(beats[0].NextSceneId).IsEqual("beat_2_chamber_light");

        AssertThat(beats[1].Id).IsEqual("beat_2_chamber_light");
        AssertThat(beats[1].NextSceneId).IsEqual("beat_3_interlude_shadow");

        AssertThat(beats[2].Id).IsEqual("beat_3_interlude_shadow");
        AssertThat(beats[2].NextSceneId).IsEqual("beat_4_chamber_shadow");

        AssertThat(beats[3].Id).IsEqual("beat_4_chamber_shadow");
        AssertThat(beats[3].NextSceneId).IsEqual("beat_5_interlude_ambition");

        AssertThat(beats[4].Id).IsEqual("beat_5_interlude_ambition");
        AssertThat(beats[4].NextSceneId).IsEqual("beat_6_chamber_ambition");

        AssertThat(beats[5].Id).IsEqual("beat_6_chamber_ambition");
        AssertThat(beats[5].NextSceneId).IsEqual("beat_7_finale");

        AssertThat(beats[6].Id).IsEqual("beat_7_finale");
        AssertThat(beats[6].NextSceneId).IsNull();
    }

    /// <summary>
    /// Test that we can get the next scene ID from any beat.
    /// </summary>
    [TestCase]
    public void ShouldGetNextSceneIdCorrectly()
    {
        // Arrange
        var loader = new StageManifestLoader();
        var manifest = loader.LoadManifest(Stage2ManifestPath);

        // Act & Assert
        var nextAfterBeat0 = manifest!.GetNextSceneId("beat_1_interlude_light");
        AssertThat(nextAfterBeat0).IsEqual("beat_2_chamber_light");

        var nextAfterBeat1 = manifest.GetNextSceneId("beat_2_chamber_light");
        AssertThat(nextAfterBeat1).IsEqual("beat_3_interlude_shadow");

        var nextAfterBeat6 = manifest.GetNextSceneId("beat_7_finale");
        AssertThat(nextAfterBeat6).IsEmpty(); // No next scene after finale
    }

    /// <summary>
    /// Test that we can get scene details by ID.
    /// </summary>
    [TestCase]
    public void ShouldGetSceneDetailsById()
    {
        // Arrange
        var loader = new StageManifestLoader();
        var manifest = loader.LoadManifest(Stage2ManifestPath);

        // Act
        var interludeScene = manifest!.GetScene("beat_1_interlude_light");
        var chamberScene = manifest.GetScene("beat_2_chamber_light");
        var finaleScene = manifest.GetScene("beat_7_finale");

        // Assert
        AssertThat(interludeScene).IsNotNull();
        AssertThat(interludeScene!.SceneFile).Contains("interlude_1.tscn");

        AssertThat(chamberScene).IsNotNull();
        AssertThat(chamberScene!.SceneFile).Contains("chamber_light.tscn");

        AssertThat(finaleScene).IsNotNull();
        AssertThat(finaleScene!.SceneFile).Contains("finale.tscn");
    }

    /// <summary>
    /// Test that manifest defines the correct next stage path.
    /// </summary>
    [TestCase]
    public void Stage2ManifestShouldDefineNextStagePath()
    {
        // Arrange
        var loader = new StageManifestLoader();

        // Act
        var manifest = loader.LoadManifest(Stage2ManifestPath);

        // Assert
        AssertThat(manifest).IsNotNull();
        AssertThat(manifest!.NextStagePath).IsNotEmpty();
        AssertThat(manifest.NextStagePath).Contains("stage_3");
    }
}
