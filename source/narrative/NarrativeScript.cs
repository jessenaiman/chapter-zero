// <copyright file="NarrativeScript.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using System.Collections.Generic;
using YamlDotNet.Serialization;

namespace OmegaSpiral.Source.Narrative;

/// <summary>
/// Base narrative script schema - stage-agnostic.
/// Contains sequential moments of narrative content (text, questions, composites).
/// Stages can extend this with stage-specific data.
/// </summary>
public class NarrativeScript
{
    /// <summary>
    /// Gets or sets the stage title.
    /// </summary>
    [YamlMember(Alias = "title")]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the stage description.
    /// </summary>
    [YamlMember(Alias = "description")]
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the primary speaker (e.g., "Omega", "System").
    /// </summary>
    [YamlMember(Alias = "speaker")]
    public string? Speaker { get; set; }

    /// <summary>
    /// Gets or sets the sequential list of narrative moments.
    /// Rendered in order from index 0 to N.
    /// </summary>
    [YamlMember(Alias = "moments")]
    public List<ContentBlock> Moments { get; set; } = new();
}

/// <summary>
/// Metadata about the narrative script.
/// Can be extended by stages for stage-specific metadata.
/// </summary>
public class ScriptMetadata
{
    /// <summary>
    /// Gets or sets the iteration counter (can use template variables like {{LIVE_API_COUNTER}}).
    /// </summary>
    [YamlMember(Alias = "iteration")]
    public string? Iteration { get; set; }

    /// <summary>
    /// Gets or sets the fallback iteration value when API is unavailable.
    /// </summary>
    [YamlMember(Alias = "iterationFallback")]
    public int? IterationFallback { get; set; }

    /// <summary>
    /// Gets or sets the description of the previous attempt.
    /// </summary>
    [YamlMember(Alias = "previousAttempt")]
    public string? PreviousAttempt { get; set; }

    /// <summary>
    /// Gets or sets the interface type identifier.
    /// </summary>
    [YamlMember(Alias = "interface")]
    public string? Interface { get; set; }

    /// <summary>
    /// Gets or sets the status identifier.
    /// </summary>
    [YamlMember(Alias = "status")]
    public string? Status { get; set; }
}

/// <summary>
/// A single content block in the narrative script.
/// Block types: "narrative", "question", "composite".
/// </summary>
public class ContentBlock
{
    /// <summary>
    /// Gets or sets the block type.
    /// Valid values: "narrative" (text only), "question" (prompt + options), "composite" (setup + question + continuation).
    /// </summary>
    [YamlMember(Alias = "type")]
    public string Type { get; set; } = string.Empty;

    // ========== NARRATIVE fields (type: "narrative") ==========

    /// <summary>
    /// Gets or sets the narrative text lines (for "narrative" blocks).
    /// </summary>
    [YamlMember(Alias = "lines")]
    public List<string>? Lines { get; set; }

    /// <summary>
    /// Gets or sets the visual preset to apply (e.g., "boot_sequence", "secret_reveal").
    /// </summary>
    [YamlMember(Alias = "visualPreset")]
    public string? VisualPreset { get; set; }

    /// <summary>
    /// Gets or sets whether to fade to stable after this block (for glitch effects).
    /// </summary>
    [YamlMember(Alias = "fadeToStable")]
    public bool? FadeToStable { get; set; }

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

    /// <summary>
    /// Gets or sets whether this block should persist (e.g., for journal entries).
    /// </summary>
    [YamlMember(Alias = "persistent")]
    public bool? Persistent { get; set; }

    /// <summary>
    /// Gets or sets the journal entry ID (if this block unlocks a journal entry).
    /// </summary>
    [YamlMember(Alias = "journalEntry")]
    public string? JournalEntry { get; set; }

    // ========== QUESTION fields (type: "question" or "composite") ==========

    /// <summary>
    /// Gets or sets the setup narrative lines (for "composite" blocks - shown before question).
    /// </summary>
    [YamlMember(Alias = "setup")]
    public List<string>? Setup { get; set; }

    /// <summary>
    /// Gets or sets the question prompt text.
    /// </summary>
    [YamlMember(Alias = "prompt")]
    public string? Prompt { get; set; }

    /// <summary>
    /// Gets or sets the question context/clarification text.
    /// </summary>
    [YamlMember(Alias = "context")]
    public string? Context { get; set; }

    /// <summary>
    /// Gets or sets the choice options for the question.
    /// See <see cref="ChoiceOption"/> for the choice structure.
    /// </summary>
    [YamlMember(Alias = "options")]
    public List<ChoiceOption>? Options { get; set; }

    /// <summary>
    /// Gets or sets the continuation narrative lines (for "composite" blocks - shown after question).
    /// </summary>
    [YamlMember(Alias = "continuation")]
    public List<string>? Continuation { get; set; }
}
