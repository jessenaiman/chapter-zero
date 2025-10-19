// <copyright file="PersonaConfig.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using System.Collections.Generic;

namespace OmegaSpiral.Source.Narrative;
/// <summary>
/// Represents the JSON configuration for a Dreamweaver persona.
/// </summary>
public sealed class PersonaConfig
{
    /// <summary>
    /// Gets or sets the terminal type.
    /// </summary>
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the opening narrative lines.
    /// </summary>
    public List<string> OpeningLines { get; set; } = new();

    /// <summary>
    /// Gets or sets the initial choice presented to the player.
    /// </summary>
    public PersonaChoiceBlock InitialChoice { get; set; } = new();

    /// <summary>
    /// Gets or sets the story blocks that make up the narrative.
    /// </summary>
    public List<PersonaStoryBlock> StoryBlocks { get; set; } = new();

    /// <summary>
    /// Gets or sets the prompt asking for the player's name.
    /// </summary>
    public string NamePrompt { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the secret question block.
    /// </summary>
    public PersonaSecretQuestionBlock SecretQuestion { get; set; } = new();

    /// <summary>
    /// Gets or sets the exit line shown when leaving the narrative.
    /// </summary>
    public string ExitLine { get; set; } = string.Empty;
}

/// <summary>
/// Represents a choice block with a prompt and multiple options.
/// </summary>
public sealed class PersonaChoiceBlock
{
    /// <summary>
    /// Gets or sets the prompt text asking the player to make a choice.
    /// </summary>
    public string Prompt { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the collection of choice options.
    /// </summary>
    public List<PersonaChoiceOption> Options { get; set; } = new();
}

/// <summary>
/// Represents a choice option with text and description.
/// </summary>
public sealed class PersonaChoiceOption
{
    // TODO: duplicate Source/Scripts/field/narrative/NarrativeSceneData.cs - Consider consolidating choice option classes
    /// <summary>
    /// Gets or sets the unique identifier for this choice.
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the display label for this choice.
    /// </summary>
    public string Label { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the description providing more details about this choice.
    /// </summary>
    public string Description { get; set; } = string.Empty;
}

/// <summary>
/// Represents a segment of narrative story with text and branching choices.
/// </summary>
public sealed class PersonaStoryBlock
{
    /// <summary>
    /// Gets or sets the collection of text paragraphs that make up this story block.
    /// </summary>
    public List<string> Paragraphs { get; set; } = new();

    /// <summary>
    /// Gets or sets the question or prompt presented after the story text.
    /// </summary>
    public string Question { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the collection of choice options available after this story block.
    /// </summary>
    public List<PersonaNarrativeChoice> Choices { get; set; } = new();
}

/// <summary>
/// Represents a choice block with a prompt and multiple Dreamweaver-aligned options.
/// </summary>
public sealed class PersonaNarrativeChoice
{
    /// <summary>
    /// Gets or sets the choice text displayed to the player.
    /// </summary>
    public string Text { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the index of the next story block.
    /// </summary>
    public int NextBlock { get; set; }
}

/// <summary>
/// Represents the secret question block with prompt and options.
/// </summary>
public sealed class PersonaSecretQuestionBlock
{
    /// <summary>
    /// Gets or sets the prompt for the secret question.
    /// </summary>
    public string Prompt { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the available answer options.
    /// </summary>
    public List<string> Options { get; set; } = new();
}
