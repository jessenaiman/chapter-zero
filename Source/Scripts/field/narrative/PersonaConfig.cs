// <copyright file="PersonaConfig.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace OmegaSpiral.Scripts.Field.Narrative;

using System;
using System.Collections.Generic;
using YamlDotNet.Serialization;

/// <summary>
/// Represents the YAML configuration for a Dreamweaver persona.
/// </summary>
public sealed class PersonaConfig
{
    /// <summary>
    /// Gets or sets the terminal type.
    /// </summary>
    [YamlMember(Alias = "type")]
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// Gets the opening narrative lines.
    /// </summary>
    [YamlMember(Alias = "openingLines")]
    public List<string> OpeningLines { get; } = new ();

    /// <summary>
    /// Gets or sets the initial choice presented to the player.
    /// </summary>
    [YamlMember(Alias = "initialChoice")]
    public ChoiceBlock InitialChoice { get; set; } = new ();

    /// <summary>
    /// Gets the story blocks that make up the narrative.
    /// </summary>
    [YamlMember(Alias = "storyBlocks")]
    public List<StoryBlock> StoryBlocks { get; } = new ();

    /// <summary>
    /// Gets or sets the prompt asking for the player's name.
    /// </summary>
    [YamlMember(Alias = "namePrompt")]
    public string NamePrompt { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the secret question block.
    /// </summary>
    [YamlMember(Alias = "secretQuestion")]
    public SecretQuestionBlock SecretQuestion { get; set; } = new ();

    /// <summary>
    /// Gets or sets the exit line shown when leaving the narrative.
    /// </summary>
    [YamlMember(Alias = "exitLine")]
    public string ExitLine { get; set; } = string.Empty;
}

/// <summary>
/// Represents a choice block with a prompt and multiple options.
/// </summary>
public sealed class ChoiceBlock
{
    /// <summary>
    /// Gets or sets the prompt text for the choice.
    /// </summary>
    [YamlMember(Alias = "prompt")]
    public string Prompt { get; set; } = string.Empty;

    /// <summary>
    /// Gets the available choice options.
    /// </summary>
    [YamlMember(Alias = "options")]
    public List<ChoiceOption> Options { get; } = new ();
}

/// <summary>
/// Represents a single choice option with id, label, and description.
/// </summary>
public sealed class ChoiceOption
{
    /// <summary>
    /// Gets or sets the unique identifier for this choice.
    /// </summary>
    [YamlMember(Alias = "id")]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the display label for this choice.
    /// </summary>
    [YamlMember(Alias = "label")]
    public string Label { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the descriptive text for this choice.
    /// </summary>
    [YamlMember(Alias = "description")]
    public string Description { get; set; } = string.Empty;
}

/// <summary>
/// Represents a story block with paragraphs, question, and choices.
/// </summary>
public sealed class StoryBlock
{
    /// <summary>
    /// Gets the narrative paragraphs in this block.
    /// </summary>
    [YamlMember(Alias = "paragraphs")]
    public List<string> Paragraphs { get; } = new ();

    /// <summary>
    /// Gets or sets the question asked at the end of this block.
    /// </summary>
    [YamlMember(Alias = "question")]
    public string Question { get; set; } = string.Empty;

    /// <summary>
    /// Gets the available narrative choices.
    /// </summary>
    [YamlMember(Alias = "choices")]
    public List<NarrativeChoice> Choices { get; } = new ();
}

/// <summary>
/// Represents a narrative choice with text and next block reference.
/// </summary>
public sealed class NarrativeChoice
{
    /// <summary>
    /// Gets or sets the choice text displayed to the player.
    /// </summary>
    [YamlMember(Alias = "text")]
    public string Text { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the index of the next story block.
    /// </summary>
    [YamlMember(Alias = "nextBlock")]
    public int NextBlock { get; set; }
}

/// <summary>
/// Represents the secret question block with prompt and options.
/// </summary>
public sealed class SecretQuestionBlock
{
    /// <summary>
    /// Gets or sets the prompt for the secret question.
    /// </summary>
    [YamlMember(Alias = "prompt")]
    public string Prompt { get; set; } = string.Empty;

    /// <summary>
    /// Gets the available answer options.
    /// </summary>
    [YamlMember(Alias = "options")]
    public List<string> Options { get; } = new ();
}
