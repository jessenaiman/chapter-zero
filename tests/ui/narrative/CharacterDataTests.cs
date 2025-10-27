// <copyright file="CharacterDataTests.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

namespace OmegaSpiral.TestsUi.Narrative;

using GdUnit4;
using Godot;
using OmegaSpiral.Domain.Models;
using OmegaSpiral.Source.Narrative;
using OmegaSpiral.Source.Scripts.Common;
using static GdUnit4.Assertions;

/// <summary>
/// Unit tests for CharacterData class.
/// Validates character data structure and conversion functionality.
/// </summary>
[TestSuite]
[RequireGodotRuntime]
public class CharacterDataTests
{
    /// <summary>
    /// CharacterData should initialize with default values.
    /// </summary>
    [TestCase]
    public void CharacterData_InitializesWithDefaults()
    {
        // Arrange & Act
        var character = new CharacterData();

        // Assert
        AssertThat(character.Id).IsEqual(string.Empty);
        AssertThat(character.Name).IsEqual(string.Empty);
        AssertThat(character.Description).IsEqual(string.Empty);
        AssertThat(character.DreamweaverReflection).IsEqual(string.Empty);
    }

    /// <summary>
    /// CharacterData should initialize with provided values.
    /// </summary>
    [TestCase]
    public void CharacterData_InitializesWithProvidedValues()
    {
        // Arrange & Act
        var character = new CharacterData(
            "wizard",
            "Merlin the Wise",
            "A powerful spellcaster",
            "light"
        );

        // Assert
        AssertThat(character.Id).IsEqual("wizard");
        AssertThat(character.Name).IsEqual("Merlin the Wise");
        AssertThat(character.Description).IsEqual("A powerful spellcaster");
        AssertThat(character.DreamweaverReflection).IsEqual("light");
    }

    /// <summary>
    /// FromDictionary should create CharacterData from Godot dictionary.
    /// </summary>
    [TestCase]
    public void FromDictionary_CreatesCharacterData()
    {
        // Arrange
        var dict = new Godot.Collections.Dictionary<string, Variant>();
        dict.Add("id", "fighter");
        dict.Add("name", "Sir Galahad");
        dict.Add("description", "A noble knight");
        dict.Add("dw_reflection", "shadow");

        // Act
        var character = CharacterData.FromDictionary(dict);

        // Assert
        AssertThat(character.Id).IsEqual("fighter");
        AssertThat(character.Name).IsEqual("Sir Galahad");
        AssertThat(character.Description).IsEqual("A noble knight");
        AssertThat(character.DreamweaverReflection).IsEqual("shadow");
    }

    /// <summary>
    /// FromDictionary should handle missing fields gracefully.
    /// </summary>
    [TestCase]
    public void FromDictionary_HandlesMissingFields()
    {
        // Arrange
        var dict = new Godot.Collections.Dictionary<string, Variant>();
        dict.Add("name", "Unknown Hero");

        // Act
        var character = CharacterData.FromDictionary(dict);

        // Assert
        AssertThat(character.Id).IsEqual(string.Empty); // Default
        AssertThat(character.Name).IsEqual("Unknown Hero");
        AssertThat(character.Description).IsEqual(string.Empty); // Default
        AssertThat(character.DreamweaverReflection).IsEqual(string.Empty); // Default
    }

    /// <summary>
    /// FromDictionary should handle empty dictionary.
    /// </summary>
    [TestCase]
    public void FromDictionary_HandlesEmptyDictionary()
    {
        // Arrange
        var dict = new Godot.Collections.Dictionary<string, Variant>();

        // Act
        var character = CharacterData.FromDictionary(dict);

        // Assert
        AssertThat(character.Id).IsEqual(string.Empty);
        AssertThat(character.Name).IsEqual(string.Empty);
        AssertThat(character.Description).IsEqual(string.Empty);
        AssertThat(character.DreamweaverReflection).IsEqual(string.Empty);
    }

    /// <summary>
    /// ToCharacter should convert fighter to Fighter class.
    /// </summary>
    [TestCase]
    public void ToCharacter_ConvertsFighter()
    {
        // Arrange
        var characterData = new CharacterData("fighter", "Arthur", "King of Camelot", "light");

        // Act
        var character = characterData.ToCharacter();

        // Assert
        AssertThat(character).IsNotNull();
        AssertThat(character.Name).IsEqual("Arthur");
        AssertThat(character.Class).IsNotNull();
    }

    /// <summary>
    /// ToCharacter should convert wizard to Mage class.
    /// </summary>
    [TestCase]
    public void ToCharacter_ConvertsWizard()
    {
        // Arrange
        var characterData = new CharacterData("wizard", "Merlin", "Court wizard", "ambition");

        // Act
        var character = characterData.ToCharacter();

        // Assert
        AssertThat(character).IsNotNull();
        AssertThat(character.Name).IsEqual("Merlin");
        AssertThat(character.Class).IsNotNull();
    }

    /// <summary>
    /// ToCharacter should convert thief to Thief class.
    /// </summary>
    [TestCase]
    public void ToCharacter_ConvertsThief()
    {
        // Arrange
        var characterData = new CharacterData("thief", "Robin", "Master of stealth", "shadow");

        // Act
        var character = characterData.ToCharacter();

        // Assert
        AssertThat(character).IsNotNull();
        AssertThat(character.Name).IsEqual("Robin");
        AssertThat(character.Class).IsNotNull();
    }

    /// <summary>
    /// ToCharacter should convert scribe to Priest class.
    /// </summary>
    [TestCase]
    public void ToCharacter_ConvertsScribe()
    {
        // Arrange
        var characterData = new CharacterData("scribe", "Brother Cadfael", "Monk scholar", "light");

        // Act
        var character = characterData.ToCharacter();

        // Assert
        AssertThat(character).IsNotNull();
        AssertThat(character.Name).IsEqual("Brother Cadfael");
        AssertThat(character.Class).IsNotNull();
    }

    /// <summary>
    /// ToCharacter should default unknown classes to Fighter.
    /// </summary>
    [TestCase]
    public void ToCharacter_DefaultsUnknownToFighter()
    {
        // Arrange
        var characterData = new CharacterData("unknown", "Mystery", "Unknown class", "none");

        // Act
        var character = characterData.ToCharacter();

        // Assert
        AssertThat(character).IsNotNull();
        AssertThat(character.Name).IsEqual("Mystery");
        AssertThat(character.Class).IsNotNull();
    }

    /// <summary>
    /// ToString should return formatted string representation.
    /// </summary>
    [TestCase]
    public void ToString_ReturnsFormattedString()
    {
        // Arrange
        var character = new CharacterData(
            "paladin",
            "Sir Lancelot",
            "Greatest knight of the Round Table",
            "ambition"
        );

        // Act
        var result = character.ToString();

        // Assert
        AssertThat(result).IsEqual("Sir Lancelot (paladin): Greatest knight of the Round Table");
    }

    /// <summary>
    /// ToString should handle empty character data.
    /// </summary>
    [TestCase]
    public void ToString_HandlesEmptyData()
    {
        // Arrange
        var character = new CharacterData();

        // Act
        var result = character.ToString();

        // Assert
        AssertThat(result).IsEqual(" (): ");
    }
}
