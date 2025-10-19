// <copyright file="FieldCombatStageTests.cs" company="Omega Spiral">
// Copyright (c) Omega Spiral. All rights reserved.
// </copyright>

namespace OmegaSpiral.Tests.Field;

using System.IO;
using System.Linq;
using System.Text.Json;
using GdUnit4;
using static GdUnit4.Assertions;

/// <summary>
/// Regression tests that ensure the restored town+combat stage wiring stays intact.
/// These tests validate manifests, scene resources, and autoload configuration without requiring the Godot runtime.
/// </summary>
[TestSuite]
[RequireGodotRuntime]
public class FieldCombatStageTests
{
    private static readonly string ProjectRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", ".."));

    private static string ManifestPath => ResolveProjectPath("Source/Data/manifest.json");

    private static string StageMetadataPath => ResolveProjectPath("Source/Data/stages/field-combat/stage.json");

    private static string StageScenePath => ResolveProjectPath("Source/Scenes/Scene5FieldCombat.tscn");

    private static string ProjectConfigPath => ResolveProjectPath("project.godot");

    /// <summary>
    /// Verifies that the manifest entry for stage five uses the field combat scene.
    /// </summary>
    [TestCase]
    public void Manifest_EntryForStageFive_UsesFieldCombatScene()
    {
        var manifestJson = File.ReadAllText(ManifestPath);
        using var manifest = JsonDocument.Parse(manifestJson);
        var scenesElement = manifest.RootElement.GetProperty("scenes");

        var stageFive = scenesElement.EnumerateArray()
            .Single(scene => scene.GetProperty("id").GetInt32() == 5);

        AssertThat(stageFive.GetProperty("type").GetString()).IsEqual("field_combat");
        AssertThat(stageFive.GetProperty("path").GetString()).IsEqual("scene5_field_combat");
        AssertThat(stageFive.GetProperty("supportsThreads").GetBoolean()).IsFalse();
    }

    /// <summary>
    /// Verifies that stage metadata defines title, scene, and resource hints.
    /// </summary>
    [TestCase]
    public void StageMetadata_DefinesTitleSceneAndResourceHints()
    {
        AssertThat(File.Exists(StageMetadataPath)).IsTrue();

        var metadataJson = File.ReadAllText(StageMetadataPath);
        using var metadata = JsonDocument.Parse(metadataJson);

        var type = metadata.RootElement.GetProperty("type").GetString();
        var title = metadata.RootElement.GetProperty("title").GetString();
        var scene = metadata.RootElement.GetProperty("scene").GetString();

        AssertThat(type).IsEqual("field_combat");
        AssertThat(title).IsNotNull();
        AssertThat(title!).IsNotEmpty();
        AssertThat(scene).IsEqual("Scene5FieldCombat");

        var resources = metadata.RootElement.GetProperty("resources");
        AssertThat(resources.GetProperty("fieldScene").GetString()).IsEqual("res://Source/Scenes/Scene5FieldCombat.tscn");
        AssertThat(resources.GetProperty("combatAssets").GetString()).IsEqual("res://Source/combat");
    }

    /// <summary>
    /// Verifies that the scene file does not reference legacy src paths.
    /// </summary>
    [TestCase]
    public void SceneFile_DoesNotReferenceLegacySrcPaths()
    {
        var sceneText = File.ReadAllText(StageScenePath);

        AssertThat(sceneText.Contains("res://src/")).IsFalse();
        AssertThat(sceneText.Contains("res://Source/Scripts/field/Field.cs")).IsTrue();
        AssertThat(sceneText.Contains("res://Source/overworld/maps/town/battles/test_combat_arena.tscn")).IsTrue();
        AssertThat(sceneText.Contains("res://Source/overworld/maps/town/warrior.dtl")).IsTrue();
    }

    /// <summary>
    /// Verifies that project autoloads include field and combat singletons.
    /// </summary>
    [TestCase]
    public void ProjectAutoloads_IncludeFieldAndCombatSingletons()
    {
        var projectLines = File.ReadAllLines(ProjectConfigPath);

        AssertThat(projectLines).Contains("FieldEvents=\"*res://Source/Scripts/field/FieldEvents.cs\"");
        AssertThat(projectLines).Contains("FieldCamera=\"*res://Source/Scripts/field/FieldCamera.cs\"");
        AssertThat(projectLines).Contains("Gameboard=\"*res://Source/Scripts/field/gameboard/Gameboard.cs\"");
        AssertThat(projectLines).Contains("GamepieceRegistry=\"*res://Source/Scripts/field/gamepieces/GamepieceRegistry.cs\"");
        AssertThat(projectLines).Contains("Player=\"*res://Source/Scripts/common/Player.cs\"");
        AssertThat(projectLines).Contains("CombatEvents=\"*res://Source/Scripts/combat/CombatEvents.cs\"");
        AssertThat(projectLines).Contains("Music=\"*res://Source/Scripts/common/music/MusicPlayer.tscn\"");
        AssertThat(projectLines).Contains("Transition=\"*res://Source/Scripts/common/screen_transitions/ScreenTransition.tscn\"");
    }

    private static string ResolveProjectPath(string relativePath) => Path.Combine(ProjectRoot, relativePath.Replace('/', Path.DirectorySeparatorChar));
}
