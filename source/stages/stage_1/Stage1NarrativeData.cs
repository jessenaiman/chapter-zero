// <copyright file="Stage1NarrativeData.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace OmegaSpiral.Source.Stages.Stage1;

/// <summary>
/// Data structure for Stage 1's opening.json narrative content.
/// Matches the JSON schema for the terminal cinematic experience.
/// </summary>
public class Stage1NarrativeData
{
    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    [JsonPropertyName("metadata")]
    public NarrativeMetadata Metadata { get; set; } = new();

    [JsonPropertyName("bootSequence")]
    public BootSequenceData BootSequence { get; set; } = new();

    [JsonPropertyName("openingMonologue")]
    public OpeningMonologueData OpeningMonologue { get; set; } = new();

    [JsonPropertyName("firstChoice")]
    public FirstChoiceData FirstChoice { get; set; } = new();

    [JsonPropertyName("storyFragment")]
    public StoryFragmentData StoryFragment { get; set; } = new();

    [JsonPropertyName("secretQuestion")]
    public SecretQuestionData SecretQuestion { get; set; } = new();

    [JsonPropertyName("nameQuestion")]
    public NameQuestionData NameQuestion { get; set; } = new();

    [JsonPropertyName("exit")]
    public ExitData Exit { get; set; } = new();
}

/// <summary>
/// Metadata about the narrative iteration and context.
/// </summary>
public class NarrativeMetadata
{
    [JsonPropertyName("iteration")]
    public string Iteration { get; set; } = string.Empty;

    [JsonPropertyName("iterationFallback")]
    public int IterationFallback { get; set; }

    [JsonPropertyName("previousAttempt")]
    public string PreviousAttempt { get; set; } = string.Empty;

    [JsonPropertyName("interface")]
    public string Interface { get; set; } = string.Empty;

    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;

    [JsonPropertyName("note")]
    public string Note { get; set; } = string.Empty;
}

/// <summary>
/// Boot sequence glitch effect data.
/// </summary>
public class BootSequenceData
{
    [JsonPropertyName("glitchLines")]
    public List<string> GlitchLines { get; set; } = new();

    [JsonPropertyName("fadeToStable")]
    public bool FadeToStable { get; set; }
}

/// <summary>
/// Opening monologue narrative data.
/// </summary>
public class OpeningMonologueData
{
    [JsonPropertyName("lines")]
    public List<string> Lines { get; set; } = new();

    [JsonPropertyName("cinematicTiming")]
    public string CinematicTiming { get; set; } = string.Empty;
}

/// <summary>
/// First choice question data.
/// </summary>
public class FirstChoiceData
{
    [JsonPropertyName("prompt")]
    public string Prompt { get; set; } = string.Empty;

    [JsonPropertyName("options")]
    public List<ChoiceOption> Options { get; set; } = new();
}

/// <summary>
/// Story fragment (bridge parable) data.
/// </summary>
public class StoryFragmentData
{
    [JsonPropertyName("intro")]
    public List<string> Intro { get; set; } = new();

    [JsonPropertyName("parable")]
    public List<string> Parable { get; set; } = new();

    [JsonPropertyName("question")]
    public string Question { get; set; } = string.Empty;

    [JsonPropertyName("options")]
    public List<ChoiceOption> Options { get; set; } = new();
}

/// <summary>
/// Secret question data.
/// </summary>
public class SecretQuestionData
{
    [JsonPropertyName("setup")]
    public List<string> Setup { get; set; } = new();

    [JsonPropertyName("question")]
    public string Question { get; set; } = string.Empty;

    [JsonPropertyName("options")]
    public List<ChoiceOption> Options { get; set; } = new();
}

/// <summary>
/// Naming question data.
/// </summary>
public class NameQuestionData
{
    [JsonPropertyName("setup")]
    public List<string> Setup { get; set; } = new();

    [JsonPropertyName("question")]
    public string Question { get; set; } = string.Empty;

    [JsonPropertyName("options")]
    public List<ChoiceOption> Options { get; set; } = new();
}

/// <summary>
/// Exit sequence data.
/// </summary>
public class ExitData
{
    [JsonPropertyName("finalLines")]
    public List<string> FinalLines { get; set; } = new();

    [JsonPropertyName("transitionEffect")]
    public string TransitionEffect { get; set; } = string.Empty;
}

/// <summary>
/// Choice option with dreamweaver thread alignment.
/// </summary>
public class ChoiceOption
{
    [JsonPropertyName("text")]
    public string Text { get; set; } = string.Empty;

    [JsonPropertyName("thread")]
    public string Thread { get; set; } = string.Empty;

    [JsonPropertyName("scores")]
    public DreamweaverScores Scores { get; set; } = new();
}

/// <summary>
/// Dreamweaver alignment scores for a choice.
/// </summary>
public class DreamweaverScores
{
    [JsonPropertyName("light")]
    public int Light { get; set; }

    [JsonPropertyName("shadow")]
    public int Shadow { get; set; }

    [JsonPropertyName("ambition")]
    public int Ambition { get; set; }
}
