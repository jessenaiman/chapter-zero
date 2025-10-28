// <copyright file="NarrativeScriptTests.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

namespace OmegaSpiral.TestsUi.Narrative;

using GdUnit4;
using System.Collections.Generic;
using OmegaSpiral.Source.Narrative;
using static GdUnit4.Assertions;

/// <summary>
/// Unit tests for NarrativeScript and related classes.
/// Updated to match the new schema: NarrativeScript with Title, Scenes, and NarrativeScene model.
/// </summary>
[TestSuite]
public class NarrativeScriptTests
{
    /// <summary>
    /// NarrativeScript should initialize with default values.
    /// </summary>
    [TestCase]
    public void NarrativeScript_InitializesWithDefaults()
    {
        // Arrange & Act
        var script = new NarrativeScript();

        // Assert
        AssertThat(script.Title).IsEqual(string.Empty);
        AssertThat(script.Speaker).IsNull();
        AssertThat(script.Description).IsNull();
        AssertThat(script.Scenes).IsNotNull();
        AssertThat(script.Scenes!.Count).IsEqual(0);
    }

    /// <summary>
    /// NarrativeScene should initialize with defaults.
    /// </summary>
    [TestCase]
    public void NarrativeScene_InitializesWithDefaults()
    {
        // Arrange & Act
        var scene = new NarrativeScene();

        // Assert
        AssertThat(scene.Owner).IsNull();
        AssertThat(scene.Question).IsNull();
        AssertThat(scene.Speaker).IsNull();
        AssertThat(scene.Answers).IsNull();
        AssertThat(scene.Lines).IsNull();
        AssertThat(scene.FadeIn).IsNull();
        AssertThat(scene.FadeOut).IsNull();
        AssertThat(scene.Timing).IsNull();
        AssertThat(scene.Pause).IsNull();
    }

    /// <summary>
    /// NarrativeScene should support narrative type with multiple lines.
    /// </summary>
    [TestCase]
    public void NarrativeScene_SupportsNarrativeType()
    {
        // Arrange
        var scene = new NarrativeScene
        {
            Owner = "omega",
            Lines = new List<string> { "Line 1", "Line 2" },
            FadeIn = true,
            Pause = 2.0f
        };

        // Assert
        AssertThat(scene.Owner).IsEqual("omega");
        AssertThat(scene.Lines).HasSize(2);
        AssertThat(scene.Lines![0]).IsEqual("Line 1");
        AssertThat(scene.Lines![1]).IsEqual("Line 2");
        AssertThat(scene.FadeIn).IsTrue();
        AssertThat(scene.Pause).IsEqual(2.0f);
        AssertThat(scene.Question).IsNull();
        AssertThat(scene.Answers).IsNull();
    }

    /// <summary>
    /// NarrativeScene should support question type with answers.
    /// </summary>
    [TestCase]
    public void NarrativeScene_SupportsQuestionType()
    {
        // Arrange
        var scene = new NarrativeScene
        {
            Owner = "omega",
            Question = "What is your choice?",
            Speaker = "system",
            Answers = new List<ChoiceOption>
            {
                new ChoiceOption { Owner = "light", Text = "Choice A" },
                new ChoiceOption { Owner = "shadow", Text = "Choice B" }
            }
        };

        // Assert
        AssertThat(scene.Owner).IsEqual("omega");
        AssertThat(scene.Question).IsEqual("What is your choice?");
        AssertThat(scene.Speaker).IsEqual("system");
        AssertThat(scene.Answers).HasSize(2);
        AssertThat(scene.Answers![0].Owner).IsEqual("light");
        AssertThat(scene.Answers![0].Text).IsEqual("Choice A");
        AssertThat(scene.Answers![1].Owner).IsEqual("shadow");
        AssertThat(scene.Answers![1].Text).IsEqual("Choice B");
    }

    /// <summary>
    /// NarrativeScript should support full script with multiple scenes.
    /// </summary>
    [TestCase]
    public void NarrativeScript_SupportsMultipleScenes()
    {
        // Arrange
        var script = new NarrativeScript
        {
            Title = "Test Script",
            Speaker = "omega",
            Description = "A test narrative script",
            Scenes = new List<NarrativeScene>
            {
                new NarrativeScene
                {
                    Owner = "omega",
                    Lines = new List<string> { "Welcome to the story" },
                    Pause = 1.0f
                },
                new NarrativeScene
                {
                    Owner = "omega",
                    Question = "What do you choose?",
                    Answers = new List<ChoiceOption>
                    {
                        new ChoiceOption { Owner = "light", Text = "The light path" },
                        new ChoiceOption { Owner = "shadow", Text = "The shadow path" }
                    }
                }
            }
        };

        // Assert
        AssertThat(script.Title).IsEqual("Test Script");
        AssertThat(script.Speaker).IsEqual("omega");
        AssertThat(script.Description).IsEqual("A test narrative script");
        AssertThat(script.Scenes).HasSize(2);
        AssertThat(script.Scenes[0].Owner).IsEqual("omega");
        AssertThat(script.Scenes[0].Lines![0]).IsEqual("Welcome to the story");
        AssertThat(script.Scenes[1].Question).IsEqual("What do you choose?");
        AssertThat(script.Scenes[1].Answers).HasSize(2);
    }
}
