using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using OmegaSpiral.Source.Scripts.Common;
using OmegaSpiral.Source.Narrative;

namespace OmegaSpiral.Source.Scripts.Stages.Stage3;

/// <summary>
/// Holds run state for the Echo Vault stage across scene transitions.
/// </summary>
public static class EchoVaultSession
{
    private static EchoVaultPlan? plan;
    private static int beatIndex;
    private static bool initialised;
    private static readonly Dictionary<string, int> points = new();
    private static readonly List<string> selectedEchoIds = new();
    private static readonly List<string> memoryLossEntries = new();

    /// <summary>
    /// Gets the current plan. Call <see cref="Initialize"/> first.
    /// </summary>
    public static EchoVaultPlan Plan
        => plan ?? throw new InvalidOperationException("EchoVaultSession accessed before initialization.");

    /// <summary>
    /// Gets the current beat index.
    /// </summary>
    public static int BeatIndex => beatIndex;

    /// <summary>
    /// Gets read-only view of selected echo ids.
    /// </summary>
    public static IReadOnlyList<string> SelectedEchoIds => selectedEchoIds;

    /// <summary>
    /// Gets read-only view of accumulated memory losses.
    /// </summary>
    public static IReadOnlyList<string> MemoryLossEntries => memoryLossEntries;

    /// <summary>
    /// Gets the current selection beat.
    /// </summary>
    public static EchoVaultBeat CurrentBeat => Plan.Beats[beatIndex];

    /// <summary>
    /// Initializes the session with a plan if not already initialised.
    /// </summary>
    /// <param name="planData">Plan to use.</param>
    public static void Initialize(EchoVaultPlan planData)
    {
        if (initialised)
        {
            return;
        }

        plan = planData ?? throw new ArgumentNullException(nameof(planData));
        beatIndex = 0;
        for (int i = 0; i < Plan.Beats.Count; i++)
        {
            if (Plan.Beats[i].Type == "selection")
            {
                beatIndex = i;
                break;
            }
        }

        points.Clear();
        points["light"] = 0;
        points["mischief"] = 0;
        points["wrath"] = 0;

        selectedEchoIds.Clear();
        memoryLossEntries.Clear();
        initialised = true;
    }

    /// <summary>
    /// Resets the session state.
    /// </summary>
    public static void Reset()
    {
        initialised = false;
        plan = null;
        beatIndex = 0;
        points.Clear();
        selectedEchoIds.Clear();
        memoryLossEntries.Clear();
    }

    /// <summary>
    /// Gets the echo options for the current selection beat.
    /// </summary>
    public static IReadOnlyList<EchoVaultEchoDefinition> GetCurrentEchoOptions()
    {
        if (Plan.Beats[beatIndex].Type != "selection")
        {
            return Array.Empty<EchoVaultEchoDefinition>();
        }

        var options = new List<EchoVaultEchoDefinition>();
        foreach (string optionId in Plan.Beats[beatIndex].Options)
        {
            EchoVaultEchoDefinition? echo = Plan.Echoes.FirstOrDefault(e => e.Id == optionId);
            if (echo != null)
            {
                options.Add(echo);
            }
        }

        return options;
    }

    /// <summary>
    /// Attempts to select the specified echo, updating points and memory loss.
    /// </summary>
    /// <param name="echoId">Echo identifier to select.</param>
    /// <param name="feedback">Descriptive feedback (memory cost, owner, etc.).</param>
    /// <returns><see langword="true"/> if selection recorded, otherwise false.</returns>
    public static bool TrySelectEcho(string echoId, out EchoSelectionFeedback feedback)
    {
        feedback = EchoSelectionFeedback.Empty;
        EchoVaultEchoDefinition? echo = Plan.Echoes.FirstOrDefault(e => e.Id == echoId);
        if (echo == null)
        {
            return false;
        }

        selectedEchoIds.Add(echo.Id);
        if (!string.IsNullOrEmpty(echo.MemoryCost))
        {
            memoryLossEntries.Add(echo.MemoryCost);
        }

        string ledgerOwner = GetOwnerForBeat(CurrentBeat.Id);
        string awarded = ledgerOwner;

        if (!string.IsNullOrEmpty(echo.Owner) &&
            !string.Equals(echo.Owner, ledgerOwner, StringComparison.OrdinalIgnoreCase))
        {
            awarded = echo.Owner;
        }

        if (!points.ContainsKey(awarded))
        {
            points[awarded] = 0;
        }

        points[awarded] += 1;

        feedback = new EchoSelectionFeedback(
            echo,
            ledgerOwner,
            awarded,
            echo.DreamweaverResponses);

        AdvanceToNextBeat();
        return true;
    }

    /// <summary>
    /// Gets the current combat definition.
    /// </summary>
    public static EchoVaultCombat GetCurrentCombat()
    {
        EchoVaultBeat beat = Plan.Beats[beatIndex];
        if (beat.Type != "combat" || string.IsNullOrEmpty(beat.EncounterId))
        {
            throw new InvalidOperationException("Current beat is not combat.");
        }

        var combat = Plan.Combats.FirstOrDefault(c => c.EncounterId == beat.EncounterId);
        return combat ?? new EchoVaultCombat { EncounterId = beat.EncounterId ?? string.Empty };
    }

    /// <summary>
    /// Resolves combat and advances to the next beat.
    /// </summary>
    /// <param name="victory">Whether combat was won.</param>
    public static void ResolveCombat(bool victory)
    {
        AdvanceToNextBeat();
        if (beatIndex >= Plan.Beats.Count)
        {
            beatIndex = Plan.Beats.Count - 1;
        }
    }

    /// <summary>
    /// Determines whether the current beat is the finale.
    /// </summary>
    public static bool IsFinaleBeat()
    {
        return Plan.Beats[beatIndex].Type == "finale";
    }

    /// <summary>
    /// Gets finale summary data.
    /// </summary>
    public static EchoVaultFinaleData GetFinaleData()
    {
        EchoVaultBeat beat = Plan.Beats[beatIndex];
        return new EchoVaultFinaleData(
            beat.Summary,
            new Dictionary<string, int>(points),
            new List<string>(selectedEchoIds),
            new List<string>(memoryLossEntries));
    }

    private static void AdvanceToNextBeat()
    {
        beatIndex = Math.Clamp(beatIndex + 1, 0, Plan.Beats.Count - 1);
    }

    private static string GetOwnerForBeat(string beatId)
    {
        foreach (var entry in Plan.PointsLedger)
        {
            if (string.Equals(entry.BeatId, beatId, StringComparison.OrdinalIgnoreCase))
            {
                return entry.Owner;
            }
        }

        return "light";
    }
}

/// <summary>
/// Feedback returned after selecting an echo.
/// </summary>
/// <param name="Echo">The echo definition chosen.</param>
/// <param name="LedgerOwner">Owner assigned to the decision point.</param>
/// <param name="AwardedOwner">Owner who received the point.</param>
/// <param name="Reactions">Reaction dictionary for supportive/caution lines.</param>
public readonly record struct EchoSelectionFeedback(
    EchoVaultEchoDefinition Echo,
    string LedgerOwner,
    string AwardedOwner,
    Dictionary<string, EchoVaultResponsePair> Reactions)
{
    /// <summary>Empty fallback.</summary>
    public static EchoSelectionFeedback Empty { get; } =
        new EchoSelectionFeedback(new EchoVaultEchoDefinition(), string.Empty, string.Empty, new Dictionary<string, EchoVaultResponsePair>());
}

/// <summary>
/// Finale summary data for UI presentation.
/// </summary>
/// <param name="Summary">Dreamweaver/system lines keyed by speaker id.</param>
/// <param name="Points">Point ledger results.</param>
/// <param name="SelectedEchoIds">Echo ids selected during the run.</param>
/// <param name="MemoryLossEntries">Memory entries removed.</param>
public readonly record struct EchoVaultFinaleData(
    Dictionary<string, List<string>> Summary,
    Dictionary<string, int> Points,
    List<string> SelectedEchoIds,
    List<string> MemoryLossEntries);
