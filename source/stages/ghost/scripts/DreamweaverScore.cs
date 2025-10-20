using Godot;
using System.Collections.Generic;
using System.Linq;

namespace OmegaSpiral.Source.Scripts.Stages.Stage1;

/// <summary>
/// Tracks player choices and determines Dreamweaver thread selection for Stage 1 opening sequence.
/// </summary>
/// <remarks>
/// <para>
/// Implements a multi-dimensional point scoring system where player choices contribute to three
/// philosophical frameworks (threads): Light, Shadow, and Ambition. The final thread selection
/// is determined by whichever thread has the highest score, with a special "Balance" ending
/// if no thread exceeds 60% of total points.
/// </para>
/// <para>
/// Thread Philosophies:
/// <list type="bullet">
/// <item><description><strong>Light</strong>: Moral certainty, protecting others, definitive action based on conviction</description></item>
/// <item><description><strong>Shadow</strong>: Observation, patience, understanding before judgment, long-term thinking</description></item>
/// <item><description><strong>Ambition</strong>: Self-empowerment, accepting risk, choosing autonomy over safety</description></item>
/// </list>
/// </para>
/// <para>
/// Point Distribution:
/// <list type="bullet">
/// <item><description>Question 1 (Name): 2 points available</description></item>
/// <item><description>Question 2 (Bridge): 3 points available</description></item>
/// <item><description>Question 3 (Darkness): 3 points available</description></item>
/// <item><description>Secret Question (Optional): 4 points available</description></item>
/// <item><description><strong>Total Maximum</strong>: 12 points</description></item>
/// </list>
/// </para>
/// <para>
/// Balance Ending Trigger: If no single thread has ≥60% of total earned points (e.g., 5 out of 8 if player
/// skips secret question), player receives the Balance ending which acknowledges philosophical complexity.
/// </para>
/// </remarks>
public partial class DreamweaverScore : Node
{
    private int _lightPoints;
    private int _shadowPoints;
    private int _ambitionPoints;
    private readonly List<ChoiceRecord> _choiceHistory = new();

    /// <summary>
    /// Gets the current Light thread score.
    /// </summary>
    public int LightPoints => _lightPoints;

    /// <summary>
    /// Gets the current Shadow thread score.
    /// </summary>
    public int ShadowPoints => _shadowPoints;

    /// <summary>
    /// Gets the current Ambition thread score.
    /// </summary>
    public int AmbitionPoints => _ambitionPoints;

    /// <summary>
    /// Gets the total points earned across all threads.
    /// </summary>
    public int TotalPoints => _lightPoints + _shadowPoints + _ambitionPoints;

    /// <summary>
    /// Gets whether the secret question was answered (contributes 4 additional points to pool).
    /// </summary>
    public bool SecretAnswered => _choiceHistory.Any(c => c.QuestionId == "secret_question");

    /// <summary>
    /// Gets the maximum possible points based on questions answered.
    /// </summary>
    /// <remarks>
    /// Returns 8 if secret question skipped, 12 if answered.
    /// </remarks>
    public int MaximumPossiblePoints => SecretAnswered ? 12 : 8;

    /// <summary>
    /// Gets the complete history of choices made during the sequence.
    /// </summary>
    public IReadOnlyList<ChoiceRecord> ChoiceHistory => _choiceHistory.AsReadOnly();

    /// <summary>
    /// Records a choice made by the player and updates thread scores.
    /// </summary>
    /// <param name="questionId">Unique identifier for the question (e.g., "question1_name")</param>
    /// <param name="choiceText">The text of the choice selected</param>
    /// <param name="lightPoints">Points awarded to Light thread</param>
    /// <param name="shadowPoints">Points awarded to Shadow thread</param>
    /// <param name="ambitionPoints">Points awarded to Ambition thread</param>
    /// <exception cref="System.ArgumentException">
    /// Thrown if <paramref name="questionId"/> or <paramref name="choiceText"/> is <see langword="null"/> or empty.
    /// </exception>
    public void RecordChoice(
        string questionId,
        string choiceText,
        int lightPoints,
        int shadowPoints,
        int ambitionPoints)
    {
        if (string.IsNullOrWhiteSpace(questionId))
        {
            throw new System.ArgumentException("Question ID cannot be null or empty", nameof(questionId));
        }

        if (string.IsNullOrWhiteSpace(choiceText))
        {
            throw new System.ArgumentException("Choice text cannot be null or empty", nameof(choiceText));
        }

        _lightPoints += lightPoints;
        _shadowPoints += shadowPoints;
        _ambitionPoints += ambitionPoints;

        var record = new ChoiceRecord(
            questionId,
            choiceText,
            lightPoints,
            shadowPoints,
            ambitionPoints);

        _choiceHistory.Add(record);

        GD.Print($"[DreamweaverScore] Choice recorded: {questionId} | Light:{LightPoints} Shadow:{ShadowPoints} Ambition:{AmbitionPoints}");
    }

    /// <summary>
    /// Determines the dominant Dreamweaver thread based on current scores.
    /// </summary>
    /// <returns>
    /// The thread type with the highest score, or <c>"Balance"</c> if no thread
    /// has achieved 60% or more of total earned points.
    /// </returns>
    /// <remarks>
    /// <para>
    /// Balance Ending Criteria: No single thread reaches 60% of <see cref="TotalPoints"/>.
    /// This represents philosophical complexity rather than indecision.
    /// </para>
    /// <para>
    /// Tiebreaker Order (if equal scores): Light → Shadow → Ambition
    /// </para>
    /// </remarks>
    public string GetDominantThread()
    {
        if (TotalPoints == 0)
        {
            GD.PrintErr("[DreamweaverScore] Cannot determine thread with zero total points");
            return "Balance"; // Default to Balance if no choices made
        }

        // Calculate percentage thresholds
        float lightPercent = (float) _lightPoints / TotalPoints;
        float shadowPercent = (float) _shadowPoints / TotalPoints;
        float ambitionPercent = (float) _ambitionPoints / TotalPoints;

        // Check for Balance ending (no thread reaches 60%)
        const float balanceThreshold = 0.60f;
        if (lightPercent < balanceThreshold &&
            shadowPercent < balanceThreshold &&
            ambitionPercent < balanceThreshold)
        {
            GD.Print($"[DreamweaverScore] Balance ending triggered: L:{lightPercent:P0} S:{shadowPercent:P0} A:{ambitionPercent:P0}");
            return "Balance";
        }

        // Find highest score (with tiebreaker order: Light → Shadow → Ambition)
        int maxPoints = Mathf.Max(_lightPoints, Mathf.Max(_shadowPoints, _ambitionPoints));

        if (_lightPoints == maxPoints)
        {
            GD.Print($"[DreamweaverScore] Light thread selected: {lightPercent:P0} ({_lightPoints}/{TotalPoints} points)");
            return "Light";
        }

        if (_shadowPoints == maxPoints)
        {
            GD.Print($"[DreamweaverScore] Shadow thread selected: {shadowPercent:P0} ({_shadowPoints}/{TotalPoints} points)");
            return "Shadow";
        }

        GD.Print($"[DreamweaverScore] Ambition thread selected: {ambitionPercent:P0} ({_ambitionPoints}/{TotalPoints} points)");
        return "Ambition";
    }

    /// <summary>
    /// Checks whether any single thread has achieved 60% or more of total points.
    /// </summary>
    /// <returns>
    /// <see langword="true"/> if a thread has reached the dominance threshold;
    /// <see langword="false"/> if balanced between threads.
    /// </returns>
    public bool IsDominantThreadClear()
    {
        if (TotalPoints == 0) return false;

        const float dominanceThreshold = 0.60f;

        return (_lightPoints / (float) TotalPoints >= dominanceThreshold) ||
               (_shadowPoints / (float) TotalPoints >= dominanceThreshold) ||
               (_ambitionPoints / (float) TotalPoints >= dominanceThreshold);
    }

    /// <summary>
    /// Resets all scores and clears choice history.
    /// </summary>
    /// <remarks>
    /// Used for testing or allowing player to restart the sequence.
    /// </remarks>
    public void Reset()
    {
        _lightPoints = 0;
        _shadowPoints = 0;
        _ambitionPoints = 0;
        _choiceHistory.Clear();
        GD.Print("[DreamweaverScore] Scores reset");
    }

    /// <summary>
    /// Gets a formatted summary of current scores for display purposes.
    /// </summary>
    /// <returns>A human-readable string showing point distribution and percentages.</returns>
    public string GetScoreSummary()
    {
        if (TotalPoints == 0)
        {
            return "No choices recorded yet.";
        }

        float lightPercent = (float) _lightPoints / TotalPoints * 100f;
        float shadowPercent = (float) _shadowPoints / TotalPoints * 100f;
        float ambitionPercent = (float) _ambitionPoints / TotalPoints * 100f;

        return $"Light: {_lightPoints} ({lightPercent:F1}%) | " +
               $"Shadow: {_shadowPoints} ({shadowPercent:F1}%) | " +
               $"Ambition: {_ambitionPoints} ({ambitionPercent:F1}%) | " +
               $"Total: {TotalPoints}/{MaximumPossiblePoints}";
    }
}

/// <summary>
/// Represents a single choice made during the opening sequence.
/// </summary>
/// <param name="QuestionId">Unique identifier for the question</param>
/// <param name="ChoiceText">The text of the selected choice</param>
/// <param name="LightPoints">Points awarded to Light thread</param>
/// <param name="ShadowPoints">Points awarded to Shadow thread</param>
/// <param name="AmbitionPoints">Points awarded to Ambition thread</param>
public readonly record struct ChoiceRecord(
    string QuestionId,
    string ChoiceText,
    int LightPoints,
    int ShadowPoints,
    int AmbitionPoints);
