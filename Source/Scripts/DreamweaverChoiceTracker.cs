using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace OmegaSpiral;

/// <summary>
/// Tracks player choices and observer interest scores to determine which
/// Dreamweaver thread the player is aligning with most (Hero, Shadow, or Ambition).
/// Emits a signal when a winning thread is determined.
/// </summary>
public partial class DreamweaverChoiceTracker : Node
{
    [Signal]
    public delegate void ThreadWinnerDeterminedEventHandler(string winningThread, float score);

    [Signal]
    public delegate void ChoiceRecordedEventHandler(string stepId, string choiceText);

    private const float WinningThreshold = 0.6f; // 60% sentiment to determine winner
    private const int MinimumChoicesForWinner = 3; // Need at least 3 choices before declaring winner

    private List<ChoiceRecord> choiceHistory = new();
    private Dictionary<string, float> cumulativeScores = new()
    {
        ["hero"] = 0f,
        ["shadow"] = 0f,
        ["ambition"] = 0f,
    };

    private string? determinedWinner = null;
    private GameState? gameState;

    /// <summary>
    /// Represents a recorded player choice with observer commentary.
    /// </summary>
    private record ChoiceRecord
    {
        public required string StepId { get; init; }
        public required string ChoiceText { get; init; }
        public required DateTime RecordedAt { get; init; }
        public required Dictionary<string, CommentaryData> Commentary { get; init; }
    }

    /// <summary>
    /// Represents observer commentary data with sentiment analysis.
    /// </summary>
    private record CommentaryData
    {
        public required string Text { get; init; }
        public required float SentimentScore { get; init; }
    }

    /// <inheritdoc/>
    public override void _Ready()
    {
        this.gameState = this.GetNode<GameState>("/root/GameState");
        GD.Print("DreamweaverChoiceTracker initialized");
    }

    /// <summary>
    /// Records a player choice along with observer commentary and sentiment scores.
    /// Automatically analyzes sentiment and updates cumulative scores.
    /// </summary>
    /// <param name="stepId">The step ID where the choice was made.</param>
    /// <param name="choiceText">The text of the choice the player selected.</param>
    /// <param name="heroCommentary">Commentary from the Hero observer.</param>
    /// <param name="shadowCommentary">Commentary from the Shadow observer.</param>
    /// <param name="ambitionCommentary">Commentary from the Ambition observer.</param>
    public void RecordChoice(
        string stepId,
        string choiceText,
        string heroCommentary,
        string shadowCommentary,
        string ambitionCommentary)
    {
        // Analyze sentiment for each observer's commentary
        var heroSentiment = this.AnalyzeSentiment(heroCommentary, "hero");
        var shadowSentiment = this.AnalyzeSentiment(shadowCommentary, "shadow");
        var ambitionSentiment = this.AnalyzeSentiment(ambitionCommentary, "ambition");

        var commentary = new Dictionary<string, CommentaryData>
        {
            ["hero"] = new CommentaryData { Text = heroCommentary, SentimentScore = heroSentiment },
            ["shadow"] = new CommentaryData { Text = shadowCommentary, SentimentScore = shadowSentiment },
            ["ambition"] = new CommentaryData { Text = ambitionCommentary, SentimentScore = ambitionSentiment },
        };

        var record = new ChoiceRecord
        {
            StepId = stepId,
            ChoiceText = choiceText,
            RecordedAt = DateTime.UtcNow,
            Commentary = commentary,
        };

        this.choiceHistory.Add(record);

        // Update cumulative scores
        this.cumulativeScores["hero"] += heroSentiment;
        this.cumulativeScores["shadow"] += shadowSentiment;
        this.cumulativeScores["ambition"] += ambitionSentiment;

        this.EmitSignal(SignalName.ChoiceRecorded, stepId, choiceText);

        GD.Print($"Choice recorded: {stepId} | Hero:{heroSentiment:F2} Shadow:{shadowSentiment:F2} Ambition:{ambitionSentiment:F2}");

        // Check if we can determine a winner
        this.CheckForWinner();
    }

    /// <summary>
    /// Gets the current score for a specific thread.
    /// </summary>
    /// <param name="thread">The thread name (hero, shadow, ambition).</param>
    /// <returns>The cumulative score for that thread.</returns>
    public float GetThreadScore(string thread)
    {
        return this.cumulativeScores.GetValueOrDefault(thread.ToLower(), 0f);
    }

    /// <summary>
    /// Gets all thread scores.
    /// </summary>
    /// <returns>Dictionary mapping thread name to score.</returns>
    public Dictionary<string, float> GetAllScores()
    {
        return new Dictionary<string, float>(this.cumulativeScores);
    }

    /// <summary>
    /// Gets the currently leading thread (highest score).
    /// </summary>
    /// <returns>Name of the leading thread, or null if no choices recorded yet.</returns>
    public string? GetLeadingThread()
    {
        if (this.choiceHistory.Count == 0)
        {
            return null;
        }

        return this.cumulativeScores.OrderByDescending(kvp => kvp.Value).First().Key;
    }

    /// <summary>
    /// Gets the determined winner thread, if one has been declared.
    /// </summary>
    /// <returns>Name of winning thread, or null if not yet determined.</returns>
    public string? GetWinningThread()
    {
        return this.determinedWinner;
    }

    /// <summary>
    /// Gets the full choice history.
    /// </summary>
    /// <returns>List of all recorded choices with commentary.</returns>
    public List<Dictionary<string, object>> GetChoiceHistory()
    {
        return this.choiceHistory.Select(record => new Dictionary<string, object>
        {
            ["StepId"] = record.StepId,
            ["ChoiceText"] = record.ChoiceText,
            ["RecordedAt"] = record.RecordedAt.ToString("o"),
            ["HeroScore"] = record.Commentary["hero"].SentimentScore,
            ["ShadowScore"] = record.Commentary["shadow"].SentimentScore,
            ["AmbitionScore"] = record.Commentary["ambition"].SentimentScore,
        }).ToList();
    }

    /// <summary>
    /// Gets statistics about tracked choices.
    /// </summary>
    /// <returns>Dictionary with choice tracking statistics.</returns>
    public Dictionary<string, object> GetStats()
    {
        var stats = new Dictionary<string, object>
        {
            ["TotalChoices"] = this.choiceHistory.Count,
            ["WinnerDetermined"] = this.determinedWinner != null,
            ["LeadingThread"] = this.GetLeadingThread() ?? "None",
            ["HeroScore"] = this.cumulativeScores["hero"],
            ["ShadowScore"] = this.cumulativeScores["shadow"],
            ["AmbitionScore"] = this.cumulativeScores["ambition"],
        };

        if (this.determinedWinner != null)
        {
            stats["WinningThread"] = this.determinedWinner;
            stats["WinningScore"] = this.cumulativeScores[this.determinedWinner];
        }

        return stats;
    }

    /// <summary>
    /// Resets all tracked choices and scores.
    /// Useful for starting a new playthrough.
    /// </summary>
    public void Reset()
    {
        this.choiceHistory.Clear();
        this.cumulativeScores["hero"] = 0f;
        this.cumulativeScores["shadow"] = 0f;
        this.cumulativeScores["ambition"] = 0f;
        this.determinedWinner = null;
        GD.Print("DreamweaverChoiceTracker reset");
    }

    private float AnalyzeSentiment(string commentary, string observerType)
    {
        // Simple sentiment analysis based on keyword presence and length
        // In production, this could use the LLM's sentiment scoring or a dedicated model

        if (string.IsNullOrWhiteSpace(commentary))
        {
            return 0f;
        }

        var lowered = commentary.ToLower();
        float score = 0f;

        // Base score: longer commentary indicates more interest
        score += Math.Min(commentary.Length / 100f, 0.5f);

        // Positive keywords per observer type
        var positiveKeywords = observerType switch
        {
            "hero" => new[] { "honor", "courage", "noble", "brave", "sacrifice", "duty", "light", "hope" },
            "shadow" => new[] { "balance", "nature", "wisdom", "pragmatic", "survival", "adapt", "cycle", "truth" },
            "ambition" => new[] { "power", "ambition", "control", "dominate", "achieve", "superior", "strong", "master" },
            _ => Array.Empty<string>(),
        };

        // Negative/neutral keywords
        var negativeKeywords = new[] { "weak", "foolish", "naive", "mistake", "wrong", "doubt", "hesitate" };

        // Score based on keyword presence
        foreach (var keyword in positiveKeywords)
        {
            if (lowered.Contains(keyword))
            {
                score += 0.3f;
            }
        }

        foreach (var keyword in negativeKeywords)
        {
            if (lowered.Contains(keyword))
            {
                score -= 0.2f;
            }
        }

        // Normalize to 0-1 range
        return Math.Clamp(score, 0f, 1f);
    }

    private void CheckForWinner()
    {
        // Don't check if winner already determined
        if (this.determinedWinner != null)
        {
            return;
        }

        // Need minimum number of choices
        if (this.choiceHistory.Count < MinimumChoicesForWinner)
        {
            return;
        }

        // Calculate total score
        var totalScore = this.cumulativeScores.Values.Sum();
        if (totalScore == 0f)
        {
            return;
        }

        // Calculate percentages
        var percentages = this.cumulativeScores.ToDictionary(
            kvp => kvp.Key,
            kvp => kvp.Value / totalScore);

        // Find highest percentage
        var winner = percentages.OrderByDescending(kvp => kvp.Value).First();

        // Check if winner meets threshold
        if (winner.Value >= WinningThreshold)
        {
            this.determinedWinner = winner.Key;

            // Update game state
            if (this.gameState != null)
            {
                this.gameState.SelectedThread = this.determinedWinner;
            }

            this.EmitSignal(SignalName.ThreadWinnerDetermined, this.determinedWinner, winner.Value);
            GD.Print($"Winner determined: {this.determinedWinner} ({winner.Value:P0})");
        }
    }
}
