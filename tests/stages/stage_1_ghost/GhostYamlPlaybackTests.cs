// <copyright file="GhostYamlPlaybackTests.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

namespace OmegaSpiral.Tests.Stages.Stage1Ghost;

using GdUnit4;
using Godot;
using OmegaSpiral.Source.Stages.Stage1;
using OmegaSpiral.Source.Narrative;
using static GdUnit4.Assertions;

/// <summary>
/// Tests for Ghost Terminal YAML playback.
/// Validates that ghost.yaml loads and scenes can be presented.
/// </summary>
[TestSuite]
public class GhostYamlPlaybackTests
{
    /// <summary>
    /// Test that ghost.yaml loads successfully with scenes.
    /// </summary>
    [TestCase]
    [RequireGodotRuntime]
    public void GhostYaml_LoadsSuccessfully()
    {
        // Arrange
        var loader = new GhostDataLoader();

        // Act
        var plan = loader.GetPlan();

        // Assert
        AssertThat(plan).IsNotNull();
        AssertThat(plan.Script).IsNotNull();
        AssertThat(plan.Script.Title).IsEqual("Ghost Terminal");
        AssertThat(plan.Script.Speaker).IsEqual("Omega");
        AssertThat(plan.Script.Scenes).IsNotNull();
        AssertThat(plan.Script.Scenes.Count).IsGreater(0);

        GD.Print($"[GhostYamlTest] Loaded {plan.Script.Scenes.Count} scenes");
    }

    /// <summary>
    /// Test that the opening scene (Scene 0) has expected structure.
    /// Should have lines, question, owner, and answers.
    /// </summary>
    [TestCase]
    public void GhostYaml_OpeningScene_HasExpectedStructure()
    {
        // Arrange
        var loader = new GhostDataLoader();
        var plan = loader.GetPlan();

        // Act
        var openingScene = plan.Script.Scenes[0];

        // Assert - Scene should have lines
        AssertThat(openingScene.Lines).IsNotNull();
        AssertThat(openingScene.Lines!.Count).IsGreater(0);        // Assert - Scene should have question
        AssertThat(openingScene.Question).IsNotNull();
        AssertThat(openingScene.Question).IsNotEmpty();

        // Assert - Scene should have owner
        AssertThat(openingScene.Owner).IsEqual("omega");

        // Assert - Scene should have answers
        AssertThat(openingScene.Choice).IsNotNull();
        AssertThat(openingScene.Choice!.Count).IsEqual(3);

        // Assert - Each answer should have owner and text
        foreach (var answer in openingScene.Choice)
        {
            AssertThat(answer.Owner).IsNotNull();
            AssertThat(answer.Text).IsNotNull();
            AssertThat(answer.Text).IsNotEmpty();
        }

        // Assert - Answers should have different owners (light, shadow, ambition)
        var owners = openingScene.Choice.Select(a => a.Owner).ToList();
        AssertThat(owners).Contains("light");
        AssertThat(owners).Contains("shadow");
        AssertThat(owners).Contains("ambition");
    }

    /// <summary>
    /// Test that opening scene lines contain expected content.
    /// This test will help us see what's actually in the YAML.
    /// </summary>
    [TestCase]
    [RequireGodotRuntime]
    public void GhostYaml_OpeningScene_ContainsExpectedLines()
    {
        // Arrange
        var loader = new GhostDataLoader();
        var plan = loader.GetPlan();
        var openingScene = plan.Script.Scenes[0];

        // Act - Print all lines for inspection
        GD.Print("[GhostYamlTest] Opening scene lines:");
        foreach (var line in openingScene.Lines!)
        {
            GD.Print($"  {line}");
        }

        GD.Print($"[GhostYamlTest] Question: {openingScene.Question}");
        GD.Print("[GhostYamlTest] Answers:");
        foreach (var answer in openingScene.Choice!)
        {
            GD.Print($"  [{answer.Owner}] {answer.Text}");
        }

        // Assert - Should contain key opening text
        var allText = string.Join(" ", openingScene.Lines);
        AssertThat(allText).Contains("Shard");
        AssertThat(allText).Contains("story");
    }

    /// <summary>
    /// Test that all scenes are valid (have either lines or question).
    /// </summary>
    [TestCase]
    [RequireGodotRuntime]
    public void GhostYaml_AllScenes_AreValid()
    {
        var loader = new GhostDataLoader();
        var plan = loader.GetPlan();

        // Act & Assert
        for (int i = 0; i < plan.Script.Scenes.Count; i++)
        {
            var scene = plan.Script.Scenes[i];

            // Scene must have lines OR question+answers
            bool hasLines = scene.Lines != null && scene.Lines.Count > 0;
            bool hasQuestion = !string.IsNullOrEmpty(scene.Question) &&
                               scene.Choice != null &&
                               scene.Choice.Count > 0;

            AssertThat(hasLines || hasQuestion)
                .OverrideFailureMessage($"Scene {i} must have either lines or question+answers")
                .IsTrue();

            GD.Print($"[GhostYamlTest] Scene {i}: " +
                     $"lines={scene.Lines?.Count ?? 0}, " +
                     $"question={!string.IsNullOrEmpty(scene.Question)}, " +
                     $"answers={scene.Choice?.Count ?? 0}");
        }
    }
}
