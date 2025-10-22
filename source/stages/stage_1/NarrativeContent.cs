// <copyright file="NarrativeContent.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using System.Collections.Generic;

namespace OmegaSpiral.Source.Stages.Stage1;

/// <summary>
/// Data structures for Stage 1 narrative content loaded from opening.json.
/// Allows all beat text and choices to be data-driven without code changes.
/// </summary>

/// <summary>
/// Container for all Stage 1 narrative content.
/// </summary>
public class NarrativeDocument
{
    public string? Type { get; set; }
    public NarrativeMetadata? Metadata { get; set; }
    public BootSequenceContent? BootSequence { get; set; }
    public OpeningMonologueContent? OpeningMonologue { get; set; }
    public FirstChoiceContent? FirstChoice { get; set; }
    public StoryFragmentContent? StoryFragment { get; set; }
    public SecretQuestionContent? SecretQuestion { get; set; }
    public NameQuestionContent? NameQuestion { get; set; }
    public ExitContent? Exit { get; set; }
}

/// <summary>
/// Metadata about the narrative session.
/// </summary>
public class NarrativeMetadata
{
    public int? Iteration { get; set; }
    public int? IterationFallback { get; set; }
    public string? PreviousAttempt { get; set; }
    public string? Interface { get; set; }
    public string? Status { get; set; }
    public string? Note { get; set; }
}

/// <summary>
/// Boot sequence glitch lines and fade settings.
/// </summary>
public class BootSequenceContent
{
    public List<string>? GlitchLines { get; set; }
    public bool FadeToStable { get; set; }
}

/// <summary>
/// Opening monologue lines and timing.
/// </summary>
public class OpeningMonologueContent
{
    public List<string>? Lines { get; set; }
    public string? CinematicTiming { get; set; }
}

/// <summary>
/// First choice (identity question) content.
/// </summary>
public class FirstChoiceContent
{
    public List<string>? Setup { get; set; }
    public Question1Content? Question1 { get; set; }
}

/// <summary>
/// Identity question details.
/// </summary>
public class Question1Content
{
    public string? Prompt { get; set; }
    public string? Context { get; set; }
    public List<ChoiceOption>? Options { get; set; }
}

/// <summary>
/// A choice option with scoring.
/// </summary>
public class ChoiceOption
{
    public string? Id { get; set; }
    public string? Text { get; set; }
    public string? Dreamweaver { get; set; }
    public ScoreData? Scores { get; set; }
    public string? Philosophical { get; set; }
    public string? Response { get; set; }
}

/// <summary>
/// Scoring for a choice option.
/// </summary>
public class ScoreData
{
    public int Light { get; set; }
    public int Shadow { get; set; }
    public int Ambition { get; set; }
}

/// <summary>
/// Story fragment with parable and second choice.
/// </summary>
public class StoryFragmentContent
{
    public List<string>? Intro { get; set; }
    public Question2Content? Question2 { get; set; }
    public List<string>? Continuation { get; set; }
}

/// <summary>
/// Second choice (story interpretation).
/// </summary>
public class Question2Content
{
    public string? Prompt { get; set; }
    public List<ChoiceOption>? Options { get; set; }
}

/// <summary>
/// Secret question and reveal content.
/// </summary>
public class SecretQuestionContent
{
    public List<string>? Setup { get; set; }
    public string? Prompt { get; set; }
    public List<ChoiceOption>? Options { get; set; }
    public SecretRevealContent? SecretReveal { get; set; }
}

/// <summary>
/// Secret code fragment reveal.
/// </summary>
public class SecretRevealContent
{
    public string? Visual { get; set; }
    public List<string>? Text { get; set; }
    public bool Persistent { get; set; }
    public string? JournalEntry { get; set; }
}

/// <summary>
/// Final naming question.
/// </summary>
public class NameQuestionContent
{
    public List<string>? Setup { get; set; }
    public string? Prompt { get; set; }
    public List<ChoiceOption>? Options { get; set; }
}

/// <summary>
/// Exit/completion content.
/// </summary>
public class ExitContent
{
    public string? SelectedThread { get; set; }
    public List<string>? FinalLines { get; set; }
}
