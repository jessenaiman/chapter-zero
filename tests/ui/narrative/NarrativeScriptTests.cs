// <copyright file="NarrativeScriptTests.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

namespace OmegaSpiral.TestsUi.Narrative;

using GdUnit4;
using System.Collections.Generic;
using OmegaSpiral.Source.Narrative;
using static GdUnit4.Assertions;

/// <summary>
/// Unit tests for NarrativeScript and ContentBlock classes.
/// Validates YAML schema structure and data integrity.
/// </summary>
[TestSuite]
public class NarrativeScriptTests
{
    /// <summary>
    /// NarrativeScript should initialize with empty collections.
    /// </summary>
    [TestCase]
    public void NarrativeScript_InitializesWithEmptyCollections()
    {
        // Arrange & Act
        var script = new NarrativeScript();

        // Assert
        AssertThat(script.Title).IsEqual(string.Empty);
        AssertThat(script.Scenes).IsNotNull();
        AssertThat(script.Scenes.Count).IsEqual(0);
    }

    /// <summary>
    /// ContentBlock should initialize with defaults.
    /// </summary>
    [TestCase]
    public void ContentBlock_InitializesWithDefaults()
    {
        // Arrange & Act
        var block = new ContentBlock();

        // Assert
        AssertThat(block.Type).IsEqual(string.Empty);
        AssertThat(block.Lines).IsNull();
        AssertThat(block.Answers).IsNull();
        AssertThat(block.Owner).IsNull();
    }

    /// <summary>
    /// ContentBlock should support narrative type with multiple lines.
    /// </summary>
    [TestCase]
    public void ContentBlock_SupportsNarrativeType()
    {
        // Arrange
        var block = new ContentBlock
        {
            Type = "narrative",
            Lines = new List<string> { "Line 1", "Line 2" },
            Owner = "omega"
        };

        // Assert
        AssertThat(block.Type).IsEqual("narrative");
        AssertThat(block.Lines).HasSize(2);
        AssertThat(block.Lines![0]).IsEqual("Line 1");
        AssertThat(block.Owner).IsEqual("omega");
    }

    /// <summary>
    /// ContentBlock should support question type with choice options.
    /// </summary>
    [TestCase]
    public void ContentBlock_SupportsQuestionType()
    {
        // Arrange
        var block = new ContentBlock
        {
            Type = "question",
            Prompt = "What is your choice?",
            Options = new List<ChoiceOption>
            {
                new ChoiceOption { Id = "1", Text = "Choice A", Dreamweaver = "light" },
                new ChoiceOption { Id = "2", Text = "Choice B", Dreamweaver = "shadow" }
            }
        };

        // Assert
        AssertThat(block.Type).IsEqual("question");
        AssertThat(block.Prompt).IsEqual("What is your choice?");
        AssertThat(block.Options).HasSize(2);
        AssertThat(block.Options![0].Dreamweaver).IsEqual("light");
        AssertThat(block.Options![1].Dreamweaver).IsEqual("shadow");
    }

    /// <summary>
    /// ContentBlock should support composite type with setup, prompt, and continuation.
    /// </summary>
    [TestCase]
    public void ContentBlock_SupportsCompositeType()
    {
        // Arrange
        var block = new ContentBlock
        {
            Type = "composite",
            Setup = new List<string> { "Setup text" },
            Prompt = "Make your choice:",
            Options = new List<ChoiceOption>
            {
                new ChoiceOption { Id = "1", Text = "Continue" }
            },
            Continuation = new List<string> { "Continuation text" }
        };

        // Assert
        AssertThat(block.Type).IsEqual("composite");
        AssertThat(block.Setup).HasSize(1);
        AssertThat(block.Setup![0]).IsEqual("Setup text");
        AssertThat(block.Continuation).HasSize(1);
        AssertThat(block.Continuation![0]).IsEqual("Continuation text");
    }

    /// <summary>
    /// NarrativeElement base class should support owner property.
    /// </summary>
    [TestCase]
    public void NarrativeElement_SupportsOwnerProperty()
    {
        // Arrange
        var choice = new ChoiceOption { Owner = "light" };

        // Assert
        AssertThat(choice.Owner).IsEqual("light");
    }

    /// <summary>
    /// ScriptMetadata should initialize with null values.
    /// </summary>
    [TestCase]
    public void ScriptMetadata_InitializesWithNulls()
    {
        // Arrange & Act
        var metadata = new ScriptMetadata();

        // Assert
        AssertThat(metadata.Iteration).IsNull();
        AssertThat(metadata.PreviousAttempt).IsNull();
        AssertThat(metadata.Interface).IsNull();
        AssertThat(metadata.Status).IsNull();
    }
}
