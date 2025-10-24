// Copyright (c) Ωmega Spiral. All rights reserved.

using GdUnit4;
using Godot;
using OmegaSpiral.Source.Narrative;
using OmegaSpiral.Source.Scripts.Infrastructure;
using static GdUnit4.Assertions;

namespace OmegaSpiral.Tests.Unit.Stages;

/// <summary>
/// Validates the Stage 6 system log JSON maps into <see cref="NarrativeSceneData"/>
/// and preserves the cue structure required by the runtime controller.
/// </summary>
[TestSuite]
public class Stage6SystemLogDataTests
{
    private const string DataPath = "res://source/stages/stage_6_epilogue/epilog.json";
    private const string SchemaPath = "res://source/data/schemas/narrative_terminal_schema.json";

    /// <summary>
    /// Ensures the Stage 6 narrative payload loads, validates, and exposes the expected cues.
    /// </summary>
    [TestCase]
    [RequireGodotRuntime]
    public void Stage6NarrativeConfigurationLoadsAndContainsCues()
    {
        var payload = ConfigurationService.LoadConfiguration(DataPath);
        AssertThat(ConfigurationService.ValidateConfiguration(payload, SchemaPath))
            .IsTrue()
            .OverrideFailureMessage("Stage 6 JSON should satisfy the narrative_terminal schema.");

        var data = NarrativeSceneFactory.Create(payload);

        AssertThat(data.OpeningLines.Count > 1).IsTrue();
        AssertThat(data.StoryBlocks.Count >= 5).IsTrue();

        var thirdBlock = data.StoryBlocks[2];
        AssertThat(thirdBlock.Paragraphs[0]).StartsWith("[VISUAL");
        AssertThat(thirdBlock.Paragraphs).Contains("[AUDIO bitcrush:on]");

        var questionBlock = data.StoryBlocks[3];
        AssertThat(questionBlock.Question).IsEqual("When the loop offered comfort, why step through?");
        AssertThat(questionBlock.Choices.Count).IsEqual(2);

        var finalBlock = data.StoryBlocks[^1];
        AssertThat(finalBlock.Paragraphs[^1]).IsEqual("Await continuation signal...");
    }
}
