// <copyright file="StoryTypes.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

namespace OmegaSpiral.Source.Backend.Narrative;

using System;
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
    /// Gets or sets the dreamweaver owner associated with this choice.
    /// </summary>
    public string? Owner { get; set; }

    /// <summary>
    /// Gets or sets the choice value or effect.
    /// </summary>
    public string? Value { get; set; }

    /// <summary>
    /// Gets or sets the optional point value awarded when this choice is selected.
    /// Defaults to <see langword="null"/> to indicate that global scoring rules apply.
    /// </summary>
    public int? Points { get; set; }
}

/// <summary>
/// Represents a single outcome within an encounter definition.
/// </summary>
public class EncounterOutcome
{
    /// <summary>
    /// Gets or sets the identifier for this outcome.
    /// </summary>
    public string? Id { get; set; }

    /// <summary>
    /// Gets or sets the owning dreamweaver for this outcome.
    /// </summary>
    public string? Owner { get; set; }

    /// <summary>
    /// Gets or sets the narrative text to present for this outcome.
    /// </summary>
    public string? Text { get; set; }

    /// <summary>
    /// Gets or sets the dreamweaver points to award when this outcome is chosen.
    /// </summary>
    public int Points { get; set; }
}

/// <summary>
/// Represents an encounter block embedded within a story scene.
/// </summary>
public class EncounterDefinition
{
    /// <summary>
    /// Gets or sets the encounter identifier.
    /// </summary>
    public string? Id { get; set; }

    /// <summary>
    /// Gets or sets the dreamweaver owner that triggered this encounter.
    /// </summary>
    public string? Owner { get; set; }

    /// <summary>
    /// Gets or sets the encounter outcomes presented to the player.
    /// </summary>
    public IList<EncounterOutcome> Outcomes { get; set; } = new List<EncounterOutcome>();
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

    /// <summary>
    /// Gets or sets the optional encounter definitions for this scene.
    /// </summary>
    public IList<EncounterDefinition>? Encounters { get; set; }
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
/// Base class for story plans that wrap script data for stage-specific access patterns.
/// </summary>
public abstract class StoryPlan
{
    /// <summary>
    /// Gets the script data for this plan.
    /// </summary>
    public StoryScriptRoot Script { get; protected set; } = new();
}

/// <summary>
/// Represents the result of executing a single scene.
/// </summary>
public sealed class SceneResult
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SceneResult"/> class.
    /// </summary>
    /// <param name="scene">The scene that was executed.</param>
    /// <param name="selectedChoice">The choice selected by the player, if any.</param>
    public SceneResult(StoryScriptElement scene, ChoiceOption? selectedChoice)
    {
        this.Scene = scene ?? throw new ArgumentNullException(nameof(scene));
        this.SelectedChoice = selectedChoice;
    }

    /// <summary>
    /// Gets the scene that was executed.
    /// </summary>
    public StoryScriptElement Scene { get; }

    /// <summary>
    /// Gets the choice selected by the player, if any.
    /// </summary>
    public ChoiceOption? SelectedChoice { get; }
}

/// <summary>
/// Event arguments describing a completed scene.
/// </summary>
public sealed class SceneResultEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SceneResultEventArgs"/> class.
    /// </summary>
    /// <param name="result">The scene execution result.</param>
    public SceneResultEventArgs(SceneResult result)
    {
        this.Result = result ?? throw new ArgumentNullException(nameof(result));
    }

    /// <summary>
    /// Gets the scene execution result.
    /// </summary>
    public SceneResult Result { get; }
}
