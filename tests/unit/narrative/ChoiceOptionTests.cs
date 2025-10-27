// <copyright file="ChoiceOptionTests.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

namespace OmegaSpiral.Tests.Unit.Narrative;

using GdUnit4;
using System.Collections.Generic;
using OmegaSpiral.Source.Narrative;
using static GdUnit4.Assertions;

/// <summary>
/// Unit tests for ChoiceOption class.
/// Validates choice data structure and scoring functionality.
/// </summary>
[TestSuite]
public class ChoiceOptionTests
{
    /// <summary>
    /// ChoiceOption should initialize with default values.
    /// </summary>
    [TestCase]
    public void ChoiceOption_InitializesWithDefaults()
    {
        // Arrange & Act
        var choice = new ChoiceOption();

        // Assert
        AssertThat(choice.Id).IsEqual(string.Empty);
        AssertThat(choice.Text).IsNull();
        AssertThat(choice.Label).IsNull(); // Should fall back to Text
        AssertThat(choice.Response).IsNull();
        AssertThat(choice.IsAvailable).IsTrue();
        AssertThat(choice.NextNodeId).IsNull();
        AssertThat(choice.NextBlock).IsEqual(0);
        AssertThat(choice.Description).IsEqual(string.Empty);
        AssertThat(choice.Dreamweaver).IsNull();
        AssertThat(choice.Scores).IsNull();
    }

    /// <summary>
    /// Label should fall back to Text when not set.
    /// </summary>
    [TestCase]
    public void ChoiceOption_LabelFallsBackToText()
    {
        // Arrange
        var choice = new ChoiceOption
        {
            Text = "Display Text",
            Label = null
        };

        // Act & Assert
        AssertThat(choice.Label).IsEqual("Display Text");
    }

    /// <summary>
    /// Label should return explicit value when set.
    /// </summary>
    [TestCase]
    public void ChoiceOption_LabelReturnsExplicitValue()
    {
        // Arrange
        var choice = new ChoiceOption
        {
            Text = "Display Text",
            Label = "Custom Label"
        };

        // Act & Assert
        AssertThat(choice.Label).IsEqual("Custom Label");
    }

    /// <summary>
    /// ChoiceOption should support dreamweaver scoring.
    /// </summary>
    [TestCase]
    public void ChoiceOption_SupportsDreamweaverScoring()
    {
        // Arrange
        var scores = new Dictionary<string, int>
        {
            ["light"] = 2,
            ["shadow"] = 1,
            ["ambition"] = 0
        };

        var choice = new ChoiceOption
        {
            Id = "choice_1",
            Text = "I choose the light",
            Dreamweaver = "light",
            Scores = scores
        };

        // Assert
        AssertThat(choice.Id).IsEqual("choice_1");
        AssertThat(choice.Text).IsEqual("I choose the light");
        AssertThat(choice.Dreamweaver).IsEqual("light");
        AssertThat(choice.Scores).IsNotNull();
        AssertThat(choice.Scores!["light"]).IsEqual(2);
        AssertThat(choice.Scores!["shadow"]).IsEqual(1);
        AssertThat(choice.Scores!["ambition"]).IsEqual(0);
    }

    /// <summary>
    /// ChoiceOption should support all dreamweaver types.
    /// </summary>
    [TestCase]
    public void ChoiceOption_SupportsAllDreamweaverTypes()
    {
        // Arrange & Act
        var lightChoice = new ChoiceOption { Dreamweaver = "light" };
        var shadowChoice = new ChoiceOption { Dreamweaver = "shadow" };
        var ambitionChoice = new ChoiceOption { Dreamweaver = "ambition" };
        var systemChoice = new ChoiceOption { Dreamweaver = "system" };
        var omegaChoice = new ChoiceOption { Dreamweaver = "omega" };
        var noneChoice = new ChoiceOption { Dreamweaver = "none" };

        // Assert
        AssertThat(lightChoice.Dreamweaver).IsEqual("light");
        AssertThat(shadowChoice.Dreamweaver).IsEqual("shadow");
        AssertThat(ambitionChoice.Dreamweaver).IsEqual("ambition");
        AssertThat(systemChoice.Dreamweaver).IsEqual("system");
        AssertThat(omegaChoice.Dreamweaver).IsEqual("omega");
        AssertThat(noneChoice.Dreamweaver).IsEqual("none");
    }

    /// <summary>
    /// ChoiceOption should support branching navigation.
    /// </summary>
    [TestCase]
    public void ChoiceOption_SupportsBranchingNavigation()
    {
        // Arrange
        var choice = new ChoiceOption
        {
            Id = "branch_1",
            NextNodeId = "node_42",
            NextBlock = 5
        };

        // Assert
        AssertThat(choice.NextNodeId).IsEqual("node_42");
        AssertThat(choice.NextBlock).IsEqual(5);
    }

    /// <summary>
    /// ChoiceOption should support availability flag.
    /// </summary>
    [TestCase]
    public void ChoiceOption_SupportsAvailabilityFlag()
    {
        // Arrange
        var availableChoice = new ChoiceOption { IsAvailable = true };
        var unavailableChoice = new ChoiceOption { IsAvailable = false };

        // Assert
        AssertThat(availableChoice.IsAvailable).IsTrue();
        AssertThat(unavailableChoice.IsAvailable).IsFalse();
    }

    /// <summary>
    /// ChoiceOption should support response text.
    /// </summary>
    [TestCase]
    public static void ChoiceOption_SupportsResponseText()
    {
        // Arrange
        var choice = new ChoiceOption
        {
            Text = "Make the choice",
            Response = "You chose wisely."
        };

        // Assert
        AssertThat(choice.Text).IsEqual("Make the choice");
        AssertThat(choice.Response).IsEqual("You chose wisely.");
    }

    /// <summary>
    /// ChoiceOption should support description field.
    /// </summary>
    [TestCase]
    public static void ChoiceOption_SupportsDescriptionField()
    {
        // Arrange
        var choice = new ChoiceOption
        {
            Text = "Complex choice",
            Description = "This choice has long-term consequences."
        };

        // Assert
        AssertThat(choice.Text).IsEqual("Complex choice");
        AssertThat(choice.Description).IsEqual("This choice has long-term consequences.");
    }
}
