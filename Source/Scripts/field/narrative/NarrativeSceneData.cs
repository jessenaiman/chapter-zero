// <copyright file="NarrativeSceneData.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;

/// <summary>
/// Contains data for narrative terminal scenes with branching story paths.
/// Supports dynamic story progression based on player choices and Dreamweaver alignment.
/// Used to define interactive narrative experiences with multiple endings.
/// </summary>
public partial class NarrativeSceneData
{
    /// <summary>
    /// Gets or sets the type identifier for this narrative scene.
    /// Used to distinguish between different narrative scene formats.
    /// </summary>
    public string Type { get; set; } = "narrative_terminal";

    /// <summary>
    /// Gets or sets the introductory text lines displayed at the start of the scene.
    /// Sets the narrative context before player choices begin.
    /// </summary>
    public List<string> OpeningLines { get; set; } = new ();

    /// <summary>
    /// Gets or sets the initial choice presented to the player.
    /// The first decision point that determines narrative branching.
    /// </summary>
    public NarrativeChoice? InitialChoice { get; set; }

    /// <summary>
    /// Gets or sets the collection of story blocks that make up the narrative.
    /// Each block represents a segment of story with choices and progression.
    /// </summary>
    public List<StoryBlock> StoryBlocks { get; set; } = new ();

    /// <summary>
    /// Gets or sets the prompt asking for the player's name.
    /// Used to personalize the narrative experience.
    /// </summary>
    public string? NamePrompt { get; set; }

    /// <summary>
    /// Gets or sets the secret question used for Dreamweaver alignment detection.
    /// Helps determine which Dreamweaver persona the player resonates with.
    /// </summary>
    public SecretQuestion? SecretQuestion { get; set; }

    /// <summary>
    /// Gets or sets the final line displayed when exiting the narrative scene.
    /// Provides closure to the narrative experience.
    /// </summary>
    public string? ExitLine { get; set; }
}

/// <summary>
/// Represents a choice point in the narrative with multiple Dreamweaver-aligned options.
/// Players select from different Dreamweaver perspectives to influence story progression.
/// Each option affects Dreamweaver scoring and determines narrative branching.
/// </summary>
public partial class NarrativeChoice
{
    /// <summary>
    /// Gets or sets the prompt text asking the player to make a choice.
    /// Provides context for the decision the player needs to make.
    /// </summary>
    public string? Prompt { get; set; }

    /// <summary>
    /// Gets or sets the collection of Dreamweaver-aligned choice options.
    /// Each option represents a different Dreamweaver perspective on the situation.
    /// </summary>
    public List<DreamweaverChoice> Options { get; set; } = new ();
}

/// <summary>
/// Represents a segment of narrative story with text and branching choices.
/// Contains paragraphs of story text and the choices that lead to other blocks.
/// Forms the building blocks of the interactive narrative experience.
/// </summary>
public partial class StoryBlock
{
    /// <summary>
    /// Gets or sets the collection of text paragraphs that make up this story block.
    /// Each paragraph is displayed sequentially with typewriter effects.
    /// </summary>
    public List<string> Paragraphs { get; set; } = new ();

    /// <summary>
    /// Gets or sets the question or prompt presented after the story text.
    /// Asks the player to make a decision that affects narrative progression.
    /// </summary>
    public string? Question { get; set; }

    /// <summary>
    /// Gets or sets the collection of choice options available after this story block.
    /// Each option leads to a different story path or outcome.
    /// </summary>
    public List<ChoiceOption> Choices { get; set; } = new ();
}

/// <summary>
/// Represents a single choice option within a story block.
/// Players select from multiple options to determine narrative progression.
/// Each option leads to a different story block or outcome.
/// </summary>
public partial class ChoiceOption
{
    /// <summary>
    /// Gets or sets the unique identifier for this choice option.
    /// Used to track which choice was selected and determine story progression.
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the display text for this choice option.
    /// The text shown to the player as an available choice.
    /// </summary>
    public string? Text { get; set; }

    /// <summary>
    /// Gets or sets the detailed description for this choice option.
    /// Provides additional context or explanation for the choice.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the story block number this choice leads to.
    /// Determines which story block is displayed after this choice is selected.
    /// </summary>
    public int NextBlock { get; set; }
}

/// <summary>
/// Represents a secret question used to determine Dreamweaver alignment.
/// Players answer questions that reveal their personality and Dreamweaver affinity.
/// Used during character creation to determine starting Dreamweaver preferences.
/// </summary>
public partial class SecretQuestion
{
    /// <summary>
    /// Gets or sets the question text presented to the player.
    /// The question that helps determine Dreamweaver alignment.
    /// </summary>
    public string? Prompt { get; set; }

    /// <summary>
    /// Gets the collection of possible answers to the secret question.
    /// Each answer corresponds to a different Dreamweaver alignment.
    /// </summary>
    public List<string> Options { get; init; } = new ();
}
