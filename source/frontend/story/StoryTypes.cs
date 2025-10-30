// <copyright file="StoryTypes.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

namespace OmegaSpiral.Source.Backend.Narrative;

using System.Collections.Generic;

/// <summary>
/// Represents a choice option in a story scene.
/// </summary>
public class ChoiceOption
{
    /// <summary>
    /// Gets or sets the choice identifier.
    /// </summary>
    public string? Id { get; set; }

    /// <summary>
    /// Gets or sets the choice text.
    /// </summary>
    public string? Text { get; set; }

    /// <summary>
    /// Gets or sets the choice value or effect.
    /// </summary>
    public string? Value { get; set; }
}

/// <summary>
/// Represents a single scene or element in a story script.
/// Contains narrative content, choices, and scene metadata.
/// </summary>
public class StoryScriptElement
{
    /// <summary>
    /// Gets or sets the scene identifier.
    /// </summary>
    public string? Id { get; set; }

    /// <summary>
    /// Gets or sets the owner or speaker of this scene.
    /// </summary>
    public string? Owner { get; set; }

    /// <summary>
    /// Gets or sets the narrative lines in this scene.
    /// </summary>
    public IList<string> Lines { get; set; } = new List<string>();

    /// <summary>
    /// Gets or sets the question text for interactive scenes.
    /// </summary>
    public string? Question { get; set; }

    /// <summary>
    /// Gets or sets the choice options for this scene.
    /// </summary>
    public IList<ChoiceOption>? Choice { get; set; }

    /// <summary>
    /// Gets or sets the pause duration in milliseconds.
    /// </summary>
    public int? Pause { get; set; }

    /// <summary>
    /// Gets or sets additional scene data.
    /// </summary>
    public object? Data { get; set; }

    /// <summary>
    /// Gets or sets the scene type or category.
    /// </summary>
    public string? Type { get; set; }

    /// <summary>
    /// Gets or sets the presentation tier for this scene.
    /// </summary>
    public int? Tier { get; set; }
}

/// <summary>
/// Represents the root of a story script loaded from JSON.
/// Contains script metadata and all scenes in the story.
/// </summary>
public class StoryScriptRoot
{
    /// <summary>
    /// Gets or sets the script title.
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// Gets or sets the speaker or narrator.
    /// </summary>
    public string? Speaker { get; set; }

    /// <summary>
    /// Gets or sets the script description.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the list of scenes in this script.
    /// </summary>
    public IList<StoryScriptElement>? Scenes { get; set; } = new List<StoryScriptElement>();

    /// <summary>
    /// Gets or sets the stage identifier for stage-specific scripts.
    /// </summary>
    public string? StageId { get; set; }

    /// <summary>
    /// Gets or sets the stage name for stage-specific scripts.
    /// </summary>
    public string? StageName { get; set; }

    /// <summary>
    /// Gets or sets the stage description for stage-specific scripts.
    /// </summary>
    public string? StageDescription { get; set; }

    /// <summary>
    /// Gets or sets metadata for the script.
    /// </summary>
    public object? Metadata { get; set; }

    /// <summary>
    /// Gets or sets the points ledger for stage scripts.
    /// </summary>
    public IList<object>? PointsLedger { get; set; }

    /// <summary>
    /// Gets or sets the beats for stage scripts.
    /// </summary>
    public IList<object>? Beats { get; set; }

    /// <summary>
    /// Gets or sets echo definitions for stage scripts.
    /// </summary>
    public IList<object>? EchoDefinitions { get; set; }

    /// <summary>
    /// Gets or sets special options for stage scripts.
    /// </summary>
    public IList<object>? SpecialOptions { get; set; }

    /// <summary>
    /// Gets or sets combat definitions for stage scripts.
    /// </summary>
    public IList<object>? Combats { get; set; }

    /// <summary>
    /// Gets or sets omega logs for stage scripts.
    /// </summary>
    public IList<string>? OmegaLogs { get; set; }

    /// <summary>
    /// Gets or sets party persistence settings for stage scripts.
    /// </summary>
    public object? PartyPersistence { get; set; }
}

/// <summary>
/// Interface for handling story presentation and interaction.
/// UI implementations must provide these methods to display story content.
/// </summary>
public interface IStoryHandler
{
    /// <summary>
    /// Plays the boot sequence for the story handler.
    /// </summary>
    Task PlayBootSequenceAsync();

    /// <summary>
    /// Displays a collection of narrative lines.
    /// </summary>
    /// <param name="lines">The lines to display.</param>
    Task DisplayLinesAsync(IList<string> lines);

    /// <summary>
    /// Applies scene-specific visual and timing effects.
    /// </summary>
    /// <param name="scene">The scene to apply effects for.</param>
    Task ApplySceneEffectsAsync(StoryScriptElement scene);

    /// <summary>
    /// Handles a command line from the script.
    /// </summary>
    /// <param name="command">The command line to handle.</param>
    /// <returns>True if the command was handled, false otherwise.</returns>
    Task<bool> HandleCommandLineAsync(string command);

    /// <summary>
    /// Presents a choice to the user.
    /// </summary>
    /// <param name="question">The question to present.</param>
    /// <param name="context">The context information.</param>
    /// <param name="choices">The available choices.</param>
    /// <returns>The selected choice.</returns>
    Task<ChoiceOption> PresentChoiceAsync(string question, string context, IList<ChoiceOption> choices);

    /// <summary>
    /// Processes a selected choice.
    /// </summary>
    /// <param name="choice">The choice to process.</param>
    Task ProcessChoiceAsync(ChoiceOption choice);

    /// <summary>
    /// Notifies that the story sequence is complete.
    /// </summary>
    Task NotifySequenceCompleteAsync();
}

/// <summary>
/// Base class for story plans that wrap script data for stage-specific access patterns.
/// </summary>
public abstract class StoryPlan
{
    /// <summary>
    /// Gets the script data for this plan.
    /// </summary>
    public StoryScriptRoot Script { get; protected set; } = new();
}
