using System;
using System.Collections.Generic;
using Godot;
using OmegaSpiral.Source.Scripts.Field.Narrative;
using OmegaSpiral.Source.Scripts.Infrastructure;

namespace OmegaSpiral.Source.Scripts.Stages.Stage1;

/// <summary>
/// Transforms serialized <see cref="NarrativeSceneData"/> for the Ghost Terminal opening into deterministic cinematic beats.
/// No new narrative text is generated here; all output is sourced from JSON via <see cref="NarrativeSceneFactory"/>.
/// </summary>
public static class GhostTerminalCinematicDirector
{
    private const string DataPath = "res://Source/Stages/Stage1/opening.json";
    private const string SchemaPath = "res://Source/Data/Schemas/ghost_terminal_cinematic_schema.json";

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
public sealed record GhostTerminalBootBeat(IReadOnlyList<string> GlitchLines, bool FadeToStable) : IGhostTerminalBeat
{
    /// <inheritdoc/>
    public GhostTerminalBeatKind Kind => GhostTerminalBeatKind.BootSequence;
}

/// <summary>
/// Represents a narration-only beat.
/// </summary>
public sealed record GhostTerminalNarrationBeat(GhostTerminalBeatKind Kind, IReadOnlyList<string> Lines, string? Timing = null) : IGhostTerminalBeat;

/// <summary>
/// Represents a beat that includes setup lines and a prompt with selectable options.
/// </summary>
public sealed record GhostTerminalChoiceBeat(
    GhostTerminalBeatKind Kind,
    IReadOnlyList<string> SetupLines,
    GhostTerminalChoicePrompt Prompt) : IGhostTerminalBeat;

/// <summary>
/// Represents the secret question beat which includes the reveal configuration.
/// </summary>
public sealed record GhostTerminalSecretChoiceBeat(
    GhostTerminalBeatKind Kind,
    GhostTerminalChoicePrompt Prompt,
    GhostTerminalSecretRevealPlan Reveal) : IGhostTerminalBeat;

/// <summary>
/// Encapsulates the prompt and options for a choice beat.
/// </summary>
public sealed record GhostTerminalChoicePrompt(
    string Prompt,
    string? Context,
    IReadOnlyList<GhostTerminalChoiceOption> Options);

/// <summary>
/// Represents a single selectable option.
/// </summary>
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
public sealed record GhostTerminalScore(int Light, int Shadow, int Ambition);

/// <summary>
/// Represents the plan for the secret reveal sequence.
/// </summary>
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
