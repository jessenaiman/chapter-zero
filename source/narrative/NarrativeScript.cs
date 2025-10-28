// <copyright file="NarrativeScript.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using YamlDotNet.Serialization;

namespace OmegaSpiral.Source.Narrative;

/// <summary>
/// Base class for all narrative elements that can have an owner (dreamweaver affiliation).
/// This provides automatic dreamweaver scoring based on the owner property.
/// </summary>
public abstract class NarrativeElement
{
    /// <summary>
    /// Gets or sets the owner/affiliation of this element.
    /// Valid values: system, omega, light, shadow, ambition, npc, none.
    /// Used for automatic dreamweaver scoring during narrative parsing.
    /// </summary>
    [YamlMember(Alias = "owner")]
    public string? Owner { get; set; }
}

/// <summary>
/// Represents a single choice option within a question.
/// Each choice has an owner (Dreamweaver affiliation) and display text.
/// </summary>
public class ChoiceOption : NarrativeElement
{
    /// <summary>
    /// Gets or sets the display text for this choice option.
    /// </summary>
    [YamlMember(Alias = "text")]
    public string? Text { get; set; }
}

/// <summary>
/// Represents a single narrative scene/beat in a script.
/// Each scene contains lines of text, optional questions with choices, and timing controls.
/// </summary>
public class NarrativeScene
{
    /// <summary>
    /// Gets or sets the narrative text lines (for narrative dialogue).
    /// </summary>
    [YamlMember(Alias = "lines")]
    public List<string>? Lines { get; set; }

    /// <summary>
    /// Gets or sets the owner/speaker for this scene.
    /// Valid values: system, omega, light, shadow, ambition, npc, none.
    /// </summary>
    [YamlMember(Alias = "owner")]
    public string? Owner { get; set; }

    /// <summary>
    /// Gets or sets the question text for scenes with questions.
    /// </summary>
    [YamlMember(Alias = "question")]
    public string? Question { get; set; }

    /// <summary>
    /// Gets or sets the speaker for the question (overrides scene owner if set).
    /// </summary>
    [YamlMember(Alias = "speaker")]
    public string? Speaker { get; set; }

    /// <summary>
    /// Gets or sets the answer options for the question.
    /// Scoring is automatic: if answer.owner matches current dreamweaver thread → +2, else → +1 to answer.owner.
    /// </summary>
    [YamlMember(Alias = "answers")]
    public List<ChoiceOption>? Answers { get; set; }

    /// <summary>
    /// Proxy property to accept YAML using 'choice' alias.
    /// Ensures backward compatibility with existing YAML files.
    /// </summary>
    [YamlMember(Alias = "choice")]
    private List<ChoiceOption>? _ChoiceProxy
    {
        get => Answers;
        set => Answers = value;
    }

    /// <summary>
    /// Gets or sets whether to fade to stable after this scene (for glitch effects).
    /// </summary>
    [YamlMember(Alias = "fadeIn")]
    public bool? FadeIn { get; set; }

    /// <summary>
    /// Gets or sets whether to fade out before this scene (for glitch effects).
    /// </summary>
    [YamlMember(Alias = "fadeOut")]
    public bool? FadeOut { get; set; }

    /// <summary>
    /// Gets or sets the cinematic timing hint (e.g., "slow_burn", "rapid").
    /// </summary>
    [YamlMember(Alias = "timing")]
    public string? Timing { get; set; }

    /// <summary>
    /// Gets or sets the pause duration in seconds (applied after displaying lines).
    /// </summary>
    [YamlMember(Alias = "pause")]
    public float? Pause { get; set; }
}

/// <summary>
/// Base narrative script schema - stage-agnostic.
/// Contains metadata about the narrative (title, description) and a sequence of scenes.
/// Stages can extend this with stage-specific data.
/// </summary>
public class NarrativeScript
{
    /// <summary>
    /// Gets or sets the title of this narrative script.
    /// </summary>
    [YamlMember(Alias = "title")]
    public string? Title { get; set; }

    /// <summary>
    /// Gets or sets the primary speaker/narrator for this script.
    /// </summary>
    [YamlMember(Alias = "speaker")]
    public string? Speaker { get; set; }

    /// <summary>
    /// Gets or sets the description of this narrative script.
    /// </summary>
    [YamlMember(Alias = "description")]
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the list of narrative scenes in this script.
    /// Each scene is a narrative beat, question, or other content moment.
    /// </summary>
    [YamlMember(Alias = "scenes")]
    public List<NarrativeScene>? Scenes { get; set; }

    /// <summary>
    /// Gets or sets the narrative text lines (for flat, non-nested scripts - deprecated).
    /// Use <see cref="Scenes"/> for modern hierarchical structures.
    /// </summary>
    [YamlMember(Alias = "lines")]
    public List<string>? Lines { get; set; }
}
