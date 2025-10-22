// <copyright file="EchoAffinityTracker.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Linq;

namespace OmegaSpiral.Source.Scripts.Stages.Stage2;

/// <summary>
/// Tracks Dreamweaver affinity scores throughout the Echo Chamber stage.
/// Maintains scoring rules: +2 for harmony (owner alignment), +1 for cross-alignment.
/// Immutable once final determination is made.
/// </summary>
public sealed class EchoAffinityTracker
{
    private readonly Dictionary<string, int> scores = new()
    {
        { "light", 0 },
        { "shadow", 0 },
        { "ambition", 0 }
    };

    private readonly List<EchoChoiceRecord> choiceHistory = new();
    private string? claimedDreamweaver;

    /// <summary>
    /// Records a choice made during an interlude.
    /// </summary>
    /// <param name="interludeOwner">The Dreamweaver that owns the upcoming chamber.</param>
    /// <param name="choiceId">The selected option ID.</param>
    /// <param name="choiceAlignment">The Dreamweaver alignment of the selected option.</param>
    public void RecordInterludeChoice(string interludeOwner, string choiceId, string choiceAlignment)
    {
        int points = choiceAlignment == interludeOwner ? 2 : 1;

        if (this.scores.ContainsKey(choiceAlignment))
        {
            this.scores[choiceAlignment] += points;
        }

        this.choiceHistory.Add(new EchoChoiceRecord(
            SequenceType: "interlude",
            OwnerId: interludeOwner,
            ChoiceId: choiceId,
            Alignment: choiceAlignment,
            PointsAwarded: points));
    }

    /// <summary>
    /// Records a chamber object interaction.
    /// </summary>
    /// <param name="chamberOwner">The Dreamweaver that owns the chamber.</param>
    /// <param name="objectSlot">The object slot (door, monster, chest).</param>
    /// <param name="objectAlignment">The Dreamweaver alignment of the object.</param>
    public void RecordChamberChoice(string chamberOwner, string objectSlot, string objectAlignment)
    {
        int points = objectAlignment == chamberOwner ? 2 : 1;

        if (this.scores.ContainsKey(objectAlignment))
        {
            this.scores[objectAlignment] += points;
        }

        this.choiceHistory.Add(new EchoChoiceRecord(
            SequenceType: "chamber",
            OwnerId: chamberOwner,
            ChoiceId: objectSlot,
            Alignment: objectAlignment,
            PointsAwarded: points));
    }

    /// <summary>
    /// Determines the final Dreamweaver claim based on accumulated scores.
    /// Once called, the tracker becomes immutable.
    /// </summary>
    /// <returns>The Dreamweaver ID with the highest score (light, shadow, or ambition).</returns>
    public string DetermineClaim()
    {
        if (this.claimedDreamweaver != null)
        {
            return this.claimedDreamweaver;
        }

        this.claimedDreamweaver = this.scores
            .OrderByDescending(kvp => kvp.Value)
            .ThenBy(kvp => kvp.Key) // Deterministic tiebreaker
            .First()
            .Key;

        return this.claimedDreamweaver;
    }

    /// <summary>
    /// Gets the current score for a specific Dreamweaver.
    /// </summary>
    /// <param name="dreamweaverId">The Dreamweaver ID (light, shadow, ambition).</param>
    /// <returns>The current score, or 0 if invalid ID.</returns>
    public int GetScore(string dreamweaverId)
    {
        return this.scores.TryGetValue(dreamweaverId, out int score) ? score : 0;
    }

    /// <summary>
    /// Gets all scores as a read-only dictionary.
    /// </summary>
    /// <returns>Immutable view of current scores.</returns>
    public IReadOnlyDictionary<string, int> GetAllScores()
    {
        return new Dictionary<string, int>(this.scores);
    }

    /// <summary>
    /// Gets the complete choice history.
    /// </summary>
    /// <returns>Immutable list of all recorded choices.</returns>
    public IReadOnlyList<EchoChoiceRecord> GetChoiceHistory()
    {
        return this.choiceHistory.AsReadOnly();
    }

    /// <summary>
    /// Resets all scores and history. Used for testing or stage restart.
    /// </summary>
    public void Reset()
    {
        foreach (string key in this.scores.Keys.ToList())
        {
            this.scores[key] = 0;
        }

        this.choiceHistory.Clear();
        this.claimedDreamweaver = null;
    }
}

/// <summary>
/// Immutable record of a single choice made during the Echo Chamber stage.
/// </summary>
/// <param name="SequenceType">The sequence type (interlude or chamber).</param>
/// <param name="OwnerId">The Dreamweaver that owns the sequence.</param>
/// <param name="ChoiceId">The choice or object ID selected.</param>
/// <param name="Alignment">The Dreamweaver alignment of the choice.</param>
/// <param name="PointsAwarded">The points awarded for this choice (1 or 2).</param>
public sealed record EchoChoiceRecord(
    string SequenceType,
    string OwnerId,
    string ChoiceId,
    string Alignment,
    int PointsAwarded);
