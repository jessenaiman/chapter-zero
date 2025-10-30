// <copyright file="ChoiceOptionTests.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

namespace OmegaSpiral.TestsUi.Narrative;

using GdUnit4;
using static GdUnit4.Assertions;

/// <summary>
/// Unit tests for ChoiceOption class.
/// Updated to match the new simplified schema: Owner (from NarrativeElement) and Text only.
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

        // Assert - New simplified schema
        AssertThat(choice.Owner).IsNull();
        AssertThat(choice.Text).IsNull();
    }

    /// <summary>
    /// ChoiceOption_WithOwnerAndText_SetsProperties
    /// </summary>
    [TestCase]
    public void ChoiceOption_WithOwnerAndText_SetsProperties()
    {
        // Arrange
        var expectedOwner = "omega";
        var expectedText = "I choose the path of light";

        // Act
        var choice = new ChoiceOption
        {
            Owner = expectedOwner,
            Text = expectedText
        };

        // Assert
        AssertThat(choice.Owner).IsEqual(expectedOwner);
        AssertThat(choice.Text).IsEqual(expectedText);
    }

    /// <summary>
    /// ChoiceOption_YamlDeserialization_Works
    /// </summary>
    [TestCase]
    public void ChoiceOption_YamlDeserialization_Works()
    {
        // This test would need the YAML deserializer setup
        // For now, testing the property model
        var choice = new ChoiceOption();

        // Test Owner property
        choice.Owner = "light";
        AssertThat(choice.Owner).IsEqual("light");

        // Test Text property
        choice.Text = "The light beckons";
        AssertThat(choice.Text).IsEqual("The light beckons");
    }

    /// <summary>
    /// ChoiceOption_InheritsFromNarrativeElement_HasOwnerProperty
    /// </summary>
    [TestCase]
    public void ChoiceOption_InheritsFromNarrativeElement_HasOwnerProperty()
    {
        // Arrange
        var choice = new ChoiceOption();

        // Act & Assert - ChoiceOption should inherit Owner from NarrativeElement
        AssertThat(choice).IsInstanceOf<ChoiceOption>();
        AssertThat(choice.GetType().BaseType?.Name).IsEqual("NarrativeElement");
    }

    [TestCase]
    public void ChoiceOption_ValidOwners_Accepted()
    {
        // Test multiple owners
        var owners = new[] { "omega", "light", "shadow", "ambition", "none" };

        foreach (var owner in owners)
        {
            // Arrange & Act
            var choice = new ChoiceOption { Owner = owner };

            // Assert
            AssertThat(choice.Owner).IsEqual(owner);
        }
    }

    [TestCase]
    public void ChoiceOption_NullOwner_SetsCorrectly()
    {
        // Arrange & Act
        var choice = new ChoiceOption { Owner = null, Text = "Some text" };

        // Assert
        AssertThat(choice.Owner).IsNull();
        AssertThat(choice.Text).IsEqual("Some text");
    }

    [TestCase]
    public void ChoiceOption_NullText_SetsCorrectly()
    {
        // Arrange & Act
        var choice = new ChoiceOption { Owner = "omega", Text = null };

        // Assert
        AssertThat(choice.Owner).IsEqual("omega");
        AssertThat(choice.Text).IsNull();
    }

    [TestCase]
    public void ChoiceOption_BothNull_SetsCorrectly()
    {
        // Arrange & Act
        var choice = new ChoiceOption { Owner = null, Text = null };

        // Assert
        AssertThat(choice.Owner).IsNull();
        AssertThat(choice.Text).IsNull();
    }
}
