// <copyright file="NarrativeScriptElement.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using YamlDotNet.Serialization;

namespace OmegaSpiral.Source.Backend.Narrative;

/// <summary>
/// Represents a single choice option within a question scene.
/// Each choice has an owner (Dreamweaver thread affiliation) and display text.
///
/// YAML Structure:
/// <c>
/// choice:
///   - owner: light      # Dreamweaver thread (light, shadow, ambition)
///     text: 'Choice text'
/// </c>
/// </summary>
public class ChoiceOption
{
    /// <summary>
    /// Gets or sets the owner/affiliation of this choice.
    /// Valid values: light, shadow, ambition.
    /// Used for automatic dreamweaver scoring: if selected.owner == scene.owner → +2 points, else → +1 point.
    /// </summary>
    [YamlMember(Alias = "owner")]
    public string? Owner { get; set; }

    /// <summary>
    /// Gets or sets the display text for this choice option shown to the player.
    /// </summary>
    [YamlMember(Alias = "text")]
    public string? Text { get; set; }
}

/// <summary>
/// Represents a single scene/narrative beat within a script.
/// Each scene contains narrative lines (optional question + choices, or just lines).
///
/// YAML Structure:
/// <c>
/// scenes:
///   - id: scene_001_boot
///     owner: omega
///     lines:
///       - 'Narrative text'
///     question: 'Question text?'
///     choice:
///       - owner: light
///         text: 'Option'
/// </c>
/// </summary>
public class NarrativeScriptElement
{
    /// <summary>
    /// Gets or sets the scene identifier (optional, for reference).
    /// </summary>
    [YamlMember(Alias = "id")]
    public string? Id { get; set; }

    /// <summary>
    /// Gets or sets the owner/speaker for this scene.
    /// Valid values: system, omega, light, shadow, ambition, npc, none.
    /// Used in scoring logic: if choice.owner == scene.owner → +2 points.
    /// </summary>
    [YamlMember(Alias = "owner")]
    public string? Owner { get; set; }

    /// <summary>
    /// Gets or sets the narrative text lines for this scene.
    /// Can include inline tags like [GLITCH], [FADE_TO_STABLE], [QUESTION PROTOCOL ACTIVATED].
    /// </summary>
    [YamlMember(Alias = "lines")]
    public List<string>? Lines { get; set; }

    /// <summary>
    /// Gets or sets the question text for scenes with choices.
    /// When present, scene transitions from narrative to choice presentation.
    /// </summary>
    [YamlMember(Alias = "question")]
    public string? Question { get; set; }

    /// <summary>
    /// Gets or sets the choice options for this scene's question.
    /// Each choice has an owner that determines scoring.
    /// </summary>
    [YamlMember(Alias = "choice")]
    public List<ChoiceOption>? Choice { get; set; }

    /// <summary>
    /// Gets or sets whether a glitch effect plays at the start of this scene.
    /// </summary>
    [YamlMember(Alias = "glitchStart")]
    public bool? GlitchStart { get; set; }

    /// <summary>
    /// Gets or sets whether a glitch effect plays at the end of this scene.
    /// </summary>
    [YamlMember(Alias = "glitchEnd")]
    public bool? GlitchEnd { get; set; }

    /// <summary>
    /// Gets or sets the cinematic timing hint (e.g., "slow_burn", "rapid").
    /// </summary>
    [YamlMember(Alias = "timing")]
    public string? Timing { get; set; }

    /// <summary>
    /// Gets or sets the pause duration in seconds (applied after displaying lines/choices).
    /// </summary>
    [YamlMember(Alias = "pause")]
    public float? Pause { get; set; }
}

/// <summary>
/// Root narrative script element containing metadata and all scenes.
/// This is the top-level YAML structure loaded from stage YAML files.
///
/// YAML Structure:
/// <c>
/// title: Stage Title
/// speaker: Primary Speaker
/// description: Stage description
/// scenes:
///   - scene 1...
///   - scene 2...
/// </c>
/// </summary>
public class NarrativeScriptRoot
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
    /// Each scene is a narrative beat, question point, or other content moment.
    /// </summary>
    [YamlMember(Alias = "scenes")]
    public List<NarrativeScriptElement>? Scenes { get; set; }
}
