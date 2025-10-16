// <copyright file="PersonaConfig.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

namespace OmegaSpiral.Scripts.Field.Narrative;

using System.Collections.Generic;

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
    public ChoiceBlock InitialChoice { get; set; } = new();

    /// <summary>
    /// Gets or sets the story blocks that make up the narrative.
    /// </summary>
    public List<StoryBlock> StoryBlocks { get; set; } = new();

    /// <summary>
    /// Gets or sets the prompt asking for the player's name.
    /// </summary>
    public string NamePrompt { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the secret question block.
    /// </summary>
    public SecretQuestionBlock SecretQuestion { get; set; } = new();

    /// <summary>
    /// Gets or sets the exit line shown when leaving the narrative.
    /// </summary>
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
    public string Prompt { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the available choice options.
    /// </summary>
    public List<ChoiceOption> Options { get; set; } = new();
}

/// <summary>
/// Represents a single choice option with id, label, and description.
/// </summary>
public sealed class ChoiceOption
{
    /// <summary>
    /// Gets or sets the unique identifier for this choice.
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the display label for this choice.
    /// </summary>
    public string Label { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the descriptive text for this choice.
    /// </summary>
    public string Description { get; set; } = string.Empty;
}

/// <summary>
/// Represents a story block with paragraphs, question, and choices.
/// </summary>
public sealed class StoryBlock
{
    /// <summary>
    /// Gets or sets the narrative paragraphs in this block.
    /// </summary>
    public List<string> Paragraphs { get; set; } = new();

    /// <summary>
    /// Gets or sets the question asked at the end of this block.
    /// </summary>
    public string Question { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the available narrative choices.
    /// </summary>
    public List<NarrativeChoice> Choices { get; set; } = new();
}

/// <summary>
/// Represents a narrative choice with text and next block reference.
/// </summary>
public sealed class NarrativeChoice
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
public sealed class SecretQuestionBlock
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
