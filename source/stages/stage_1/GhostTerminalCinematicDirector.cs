using System;
using System.Collections.Generic;
using Godot;
using OmegaSpiral.Source.Narrative;
using OmegaSpiral.Source.Scripts.Infrastructure;

namespace OmegaSpiral.Source.Scripts.Stages.Stage1;

/// <summary>
/// Transforms serialized <see cref="NarrativeSceneData"/> for the Ghost Terminal opening into deterministic cinematic beats.
/// No new narrative text is generated here; all output is sourced from JSON via <see cref="NarrativeSceneFactory"/>.
/// </summary>
public static class GhostTerminalCinematicDirector
{
    private const string DataPath = "res://source/stages/stage_1/stage1.json";
    private const string SchemaPath = "res://source/data/schemas/ghost_terminal_cinematic_schema.json";

    private static GhostTerminalCinematicPlan? cachedPlan;
    private static readonly object SyncRoot = new();

    /// <summary>
    /// Gets the cached cinematic plan, loading and translating data if necessary.
    /// </summary>
    /// <returns>A fully populated cinematic plan.</returns>
    public static GhostTerminalCinematicPlan GetPlan()
    {
        if (cachedPlan != null)
        {
            return cachedPlan;
        }

        lock (SyncRoot)
        {
            if (cachedPlan == null)
            {
                NarrativeSceneData data = LoadNarrativeData();
                cachedPlan = BuildPlan(data);
            }
        }

        return cachedPlan!;
    }

    /// <summary>
    /// Forces the director to reload content from disk. Useful for tests when stage data is modified.
    /// </summary>
    public static void Reset()
    {
        lock (SyncRoot)
        {
            cachedPlan = null;
        }
    }

    private static NarrativeSceneData LoadNarrativeData()
    {
        var configuration = ConfigurationService.LoadConfiguration(DataPath);

        if (!ConfigurationService.ValidateConfiguration(configuration, SchemaPath))
        {
            throw new InvalidOperationException("Ghost Terminal cinematic data failed schema validation.");
        }

        NarrativeSceneData sceneData = NarrativeSceneFactory.Create(configuration);

        if (sceneData.Cinematic == null)
        {
            throw new InvalidOperationException("Ghost Terminal cinematic data is missing required cinematic section.");
        }

        return sceneData;
    }

    private static GhostTerminalCinematicPlan BuildPlan(NarrativeSceneData sceneData)
    {
        GhostTerminalCinematicData cinematic = sceneData.Cinematic ?? throw new ArgumentException("Cinematic data cannot be null.", nameof(sceneData));

        var bootBeat = new GhostTerminalBootBeat(
            cinematic.BootSequence.GlitchLines.ToArray(),
            cinematic.BootSequence.FadeToStable);

        var monologueBeat = new GhostTerminalNarrationBeat(
            GhostTerminalBeatKind.Monologue,
            cinematic.OpeningMonologue.Lines.ToArray(),
            cinematic.OpeningMonologue.CinematicTiming);

        var firstChoice = ConvertChoiceBlock(cinematic.FirstChoice, GhostTerminalBeatKind.InitialChoice);
        var storyIntro = new GhostTerminalNarrationBeat(
            GhostTerminalBeatKind.StoryIntro,
            cinematic.StoryFragment.Intro.ToArray());

        var storyChoice = ConvertQuestion(
            cinematic.StoryFragment.Question,
            GhostTerminalBeatKind.StoryChoice);

        var storyContinuation = new GhostTerminalNarrationBeat(
            GhostTerminalBeatKind.StoryContinuation,
            cinematic.StoryFragment.Continuation.ToArray());

        var secretSetup = new GhostTerminalNarrationBeat(
            GhostTerminalBeatKind.SecretSetup,
            cinematic.SecretQuestion.Setup.ToArray());

        var secretChoice = ConvertSecretQuestion(
            cinematic.SecretQuestion,
            GhostTerminalBeatKind.SecretChoice);

        var nameSetup = new GhostTerminalNarrationBeat(
            GhostTerminalBeatKind.NameSetup,
            cinematic.NameQuestion.Setup.ToArray());

        var nameChoice = ConvertQuestion(
            cinematic.NameQuestion.ToQuestion(),
            GhostTerminalBeatKind.NameChoice);

        var exitBeat = new GhostTerminalNarrationBeat(
            GhostTerminalBeatKind.Exit,
            cinematic.Exit.FinalLines.ToArray());

        return new GhostTerminalCinematicPlan(
            cinematic.Metadata,
            bootBeat,
            monologueBeat,
            firstChoice,
            storyIntro,
            storyChoice,
            storyContinuation,
            secretSetup,
            secretChoice,
            nameSetup,
            nameChoice,
            exitBeat);
    }

    private static GhostTerminalChoiceBeat ConvertChoiceBlock(GhostTerminalChoiceBlock block, GhostTerminalBeatKind kind)
    {
        var setup = block.Setup.ToArray();
        GhostTerminalChoicePrompt prompt = ConvertQuestion(block.Question);
        return new GhostTerminalChoiceBeat(kind, setup, prompt);
    }

    private static GhostTerminalChoiceBeat ConvertQuestion(GhostTerminalQuestion question, GhostTerminalBeatKind kind)
    {
        GhostTerminalChoicePrompt prompt = ConvertQuestion(question);
        return new GhostTerminalChoiceBeat(kind, Array.Empty<string>(), prompt);
    }

    private static GhostTerminalChoicePrompt ConvertQuestion(GhostTerminalQuestion question)
    {
        var options = new List<GhostTerminalChoiceOption>(question.Options.Count);

        foreach (GhostTerminalOption option in question.Options)
        {
            var scores = new GhostTerminalScore(
                option.Scores.Light,
                option.Scores.Shadow,
                option.Scores.Ambition);

            options.Add(new GhostTerminalChoiceOption(
                option.Id,
                option.Text,
                option.Dreamweaver,
                option.Philosophical,
                option.Response,
                scores));
        }

        return new GhostTerminalChoicePrompt(
            question.Prompt,
            question.Context,
            options);
    }

    private static GhostTerminalSecretChoiceBeat ConvertSecretQuestion(
        GhostTerminalSecretQuestion secretQuestion,
        GhostTerminalBeatKind kind)
    {
        GhostTerminalChoicePrompt prompt = ConvertQuestion(secretQuestion.ToQuestion());

        var reveal = new GhostTerminalSecretRevealPlan(
            secretQuestion.SecretReveal.Visual,
            secretQuestion.SecretReveal.Text.ToArray(),
            secretQuestion.SecretReveal.Persistent,
            secretQuestion.SecretReveal.JournalEntry);

        return new GhostTerminalSecretChoiceBeat(
            kind,
            prompt,
            reveal);
    }
}

/// <summary>
/// Immutable cinematic plan assembled from JSON.
/// </summary>
/// <param name="Metadata">The narrative metadata for this cinematic.</param>
/// <param name="Boot">The boot sequence beat.</param>
/// <param name="OpeningMonologue">The opening monologue beat.</param>
/// <param name="FirstChoice">The first choice beat presented to the player.</param>
/// <param name="StoryIntro">The story introduction beat.</param>
/// <param name="StoryChoice">The story-related choice beat.</param>
/// <param name="StoryContinuation">The story continuation beat.</param>
/// <param name="SecretSetup">The setup for the secret reveal.</param>
/// <param name="SecretChoice">The secret choice beat.</param>
/// <param name="NameSetup">The setup for the name-related beat.</param>
/// <param name="NameChoice">The name-related choice beat.</param>
/// <param name="Exit">The exit/conclusion beat.</param>
public sealed record GhostTerminalCinematicPlan(
    GhostTerminalMetadata Metadata,
    GhostTerminalBootBeat Boot,
    GhostTerminalNarrationBeat OpeningMonologue,
    GhostTerminalChoiceBeat FirstChoice,
    GhostTerminalNarrationBeat StoryIntro,
    GhostTerminalChoiceBeat StoryChoice,
    GhostTerminalNarrationBeat StoryContinuation,
    GhostTerminalNarrationBeat SecretSetup,
    GhostTerminalSecretChoiceBeat SecretChoice,
    GhostTerminalNarrationBeat NameSetup,
    GhostTerminalChoiceBeat NameChoice,
    GhostTerminalNarrationBeat Exit)
{
    /// <summary>
    /// Gets all beats in scripted order for iteration or testing.
    /// </summary>
    public IReadOnlyList<IGhostTerminalBeat> AllBeats { get; } =
        new IGhostTerminalBeat[]
        {
            Boot,
            OpeningMonologue,
            FirstChoice,
            StoryIntro,
            StoryChoice,
            StoryContinuation,
            SecretSetup,
            SecretChoice,
            NameSetup,
            NameChoice,
            Exit
        };
}

/// <summary>
/// Beat types used to represent the cinematic flow.
/// </summary>
public enum GhostTerminalBeatKind
{
    BootSequence,
    Monologue,
    InitialChoice,
    StoryIntro,
    StoryChoice,
    StoryContinuation,
    SecretSetup,
    SecretChoice,
    NameSetup,
    NameChoice,
    Exit
}

/// <summary>
/// Common interface for beats in the cinematic plan.
/// </summary>
public interface IGhostTerminalBeat
{
    /// <summary>Gets the beat classification.</summary>
    GhostTerminalBeatKind Kind { get; }
}

/// <summary>
/// Represents the boot sequence beat containing glitch lines and fade information.
/// </summary>
/// <param name="GlitchLines">The glitch text lines to display.</param>
/// <param name="FadeToStable">Whether to fade to a stable display state.</param>
public sealed record GhostTerminalBootBeat(IReadOnlyList<string> GlitchLines, bool FadeToStable) : IGhostTerminalBeat
{
    /// <inheritdoc/>
    public GhostTerminalBeatKind Kind => GhostTerminalBeatKind.BootSequence;
}

/// <summary>
/// Represents a narration-only beat.
/// </summary>
/// <param name="Kind">The beat classification.</param>
/// <param name="Lines">The lines of narration text.</param>
/// <param name="Timing">Optional timing classification (e.g., 'slow_burn').</param>
public sealed record GhostTerminalNarrationBeat(GhostTerminalBeatKind Kind, IReadOnlyList<string> Lines, string? Timing = null) : IGhostTerminalBeat;

/// <summary>
/// Represents a beat that includes setup lines and a prompt with selectable options.
/// </summary>
/// <param name="Kind">The beat classification.</param>
/// <param name="SetupLines">The setup text lines before the choice.</param>
/// <param name="Prompt">The choice prompt and available options.</param>
public sealed record GhostTerminalChoiceBeat(
    GhostTerminalBeatKind Kind,
    IReadOnlyList<string> SetupLines,
    GhostTerminalChoicePrompt Prompt) : IGhostTerminalBeat;

/// <summary>
/// Represents the secret question beat which includes the reveal configuration.
/// </summary>
/// <param name="Kind">The beat classification.</param>
/// <param name="Prompt">The secret choice prompt and available options.</param>
/// <param name="Reveal">The secret reveal plan executed after the choice.</param>
public sealed record GhostTerminalSecretChoiceBeat(
    GhostTerminalBeatKind Kind,
    GhostTerminalChoicePrompt Prompt,
    GhostTerminalSecretRevealPlan Reveal) : IGhostTerminalBeat;

/// <summary>
/// Encapsulates the prompt and options for a choice beat.
/// </summary>
/// <param name="Prompt">The main question text presented to the player.</param>
/// <param name="Context">Optional contextual information about the choice.</param>
/// <param name="Options">The list of selectable response options.</param>
public sealed record GhostTerminalChoicePrompt(
    string Prompt,
    string? Context,
    IReadOnlyList<GhostTerminalChoiceOption> Options);

/// <summary>
/// Represents a single selectable option.
/// </summary>
/// <param name="Id">The unique identifier for this option.</param>
/// <param name="Text">The display text for this option.</param>
/// <param name="Dreamweaver">The associated Dreamweaver persona, if any.</param>
/// <param name="PhilosophicalTag">Optional philosophical classification of the choice.</param>
/// <param name="Response">Optional response text from the narrative.</param>
/// <param name="Scores">The score distribution affected by selecting this option.</param>
public sealed record GhostTerminalChoiceOption(
    string Id,
    string Text,
    string? Dreamweaver,
    string? PhilosophicalTag,
    string? Response,
    GhostTerminalScore Scores);

/// <summary>
/// Score distribution for an option. Mirrors JSON without adding derived values.
/// </summary>
/// <param name="Light">The score contribution to the Light persona.</param>
/// <param name="Shadow">The score contribution to the Shadow persona.</param>
/// <param name="Ambition">The score contribution to the Ambition persona.</param>
public sealed record GhostTerminalScore(int Light, int Shadow, int Ambition);

/// <summary>
/// Represents the plan for the secret reveal sequence.
/// </summary>
/// <param name="Visual">Optional visual effect identifier for the reveal.</param>
/// <param name="Lines">The revelation text lines presented to the player.</param>
/// <param name="Persistent">Whether this revelation persists in the player's state.</param>
/// <param name="JournalEntry">Optional journal entry to unlock with the revelation.</param>
public sealed record GhostTerminalSecretRevealPlan(
    string? Visual,
    IReadOnlyList<string> Lines,
    bool Persistent,
    string? JournalEntry);

internal static class GhostTerminalCinematicDataExtensions
{
    /// <summary>
    /// Converts a name question block into a standard question structure.
    /// </summary>
    /// <param name="nameQuestion">The name question block.</param>
    /// <returns>A <see cref="GhostTerminalQuestion"/> representing the same data.</returns>
    public static GhostTerminalQuestion ToQuestion(this GhostTerminalNameQuestion nameQuestion)
    {
        ArgumentNullException.ThrowIfNull(nameQuestion);

        var question = new GhostTerminalQuestion
        {
            Prompt = nameQuestion.Prompt
        };

        question.Options.AddRange(nameQuestion.Options);

        return question;
    }

    /// <summary>
    /// Converts a secret question into the common question format.
    /// </summary>
    /// <param name="secretQuestion">The secret question data.</param>
    /// <returns>A <see cref="GhostTerminalQuestion"/> instance.</returns>
    public static GhostTerminalQuestion ToQuestion(this GhostTerminalSecretQuestion secretQuestion)
    {
        ArgumentNullException.ThrowIfNull(secretQuestion);

        var question = new GhostTerminalQuestion
        {
            Prompt = secretQuestion.Prompt
        };

        question.Options.AddRange(secretQuestion.Options);

        return question;
    }
}
