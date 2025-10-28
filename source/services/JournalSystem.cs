// <copyright file="JournalSystem.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Godot;

namespace OmegaSpiral.Source.Infrastructure;

/// <summary>
/// Represents a single stage's narrative record within a playthrough journal.
/// Captures what actually happened during a stage, including narrative text, player choices, and score changes.
/// </summary>
public class StageRecord
{
    /// <summary>Gets or sets the stage identifier (e.g., "ghost", "nethack").</summary>
    public string StageId { get; set; } = string.Empty;

    /// <summary>Gets or sets the narrative transcript for this stage (concatenated text of all moments as they played).</summary>
    public string Transcript { get; set; } = string.Empty;

    /// <summary>Gets or sets the timestamp when this stage was completed.</summary>
    public DateTime CompletedAt { get; set; }

    /// <summary>Gets or sets the Dreamweaver scores at the end of this stage.</summary>
    public Dictionary<string, int> FinalScores { get; set; } = new();

    /// <summary>Gets or sets the player's choices during this stage.</summary>
    public List<string> PlayerChoices { get; set; } = new();
}

/// <summary>
/// Manages the journal/transcript system for tracking what happens during a playthrough.
/// Each stage's CinematicDirector records what occurred, and JournalSystem aggregates it into a full transcript.
/// The journal is written to disk at game completion, creating a readable story of the playthrough.
/// </summary>
[GlobalClass]
public partial class JournalSystem : RefCounted
{
    private List<StageRecord> _StageRecords = new();

    /// <summary>
    /// Records the completion of a stage, including its narrative transcript and final scores.
    /// </summary>
    /// <param name="stageId">The identifier of the completed stage (e.g., "ghost", "nethack").</param>
    /// <param name="transcript">The full narrative transcript of what happened during the stage.</param>
    /// <param name="finalScores">The Dreamweaver scores at the end of the stage (light, shadow, ambition).</param>
    /// <param name="playerChoices">The list of choices the player made during the stage.</param>
    public void RecordStage(string stageId, string transcript, Dictionary<string, int> finalScores, List<string>? playerChoices = null)
    {
        var record = new StageRecord
        {
            StageId = stageId,
            Transcript = transcript,
            FinalScores = new Dictionary<string, int>(finalScores ?? new Dictionary<string, int>()),
            PlayerChoices = playerChoices ?? new List<string>(),
            CompletedAt = DateTime.Now
        };

        _StageRecords.Add(record);
        GD.Print($"[JournalSystem] Recorded stage '{stageId}' - {record.Transcript.Length} characters");
    }

    /// <summary>
    /// Gets the complete playthrough transcript, including all stages in order.
    /// </summary>
    /// <returns>A formatted string representing the entire playthrough as a narrative.</returns>
    public string GetPlaythroughTranscript()
    {
        if (_StageRecords.Count == 0)
        {
            return "[No recorded moments]";
        }

        var sb = new StringBuilder();
        sb.AppendLine("═══════════════════════════════════════════════════════");
        sb.AppendLine("OMEGA SPIRAL - PLAYTHROUGH TRANSCRIPT");
        sb.AppendLine("═══════════════════════════════════════════════════════");
        sb.AppendLine();

        for (int i = 0; i < _StageRecords.Count; i++)
        {
            var record = _StageRecords[i];
            sb.AppendLine($"───────────────────────────────────────────────────────");
            sb.AppendLine($"STAGE {i + 1}: {record.StageId.ToUpperInvariant()}");
            sb.AppendLine($"Completed: {record.CompletedAt:yyyy-MM-dd HH:mm:ss}");
            sb.AppendLine($"───────────────────────────────────────────────────────");
            sb.AppendLine();

            sb.AppendLine(record.Transcript);
            sb.AppendLine();

            if (record.PlayerChoices.Count > 0)
            {
                sb.AppendLine("Player Choices:");
                foreach (var choice in record.PlayerChoices)
                {
                    sb.AppendLine($"  → {choice}");
                }
                sb.AppendLine();
            }

            if (record.FinalScores.Count > 0)
            {
                sb.AppendLine("Dreamweaver Influence:");
                foreach (var score in record.FinalScores)
                {
                    sb.AppendLine($"  {score.Key}: {score.Value}");
                }
                sb.AppendLine();
            }
        }

        sb.AppendLine("═══════════════════════════════════════════════════════");
        sb.AppendLine($"End of Playthrough - {_StageRecords.Count} stages completed");
        sb.AppendLine("═══════════════════════════════════════════════════════");

        return sb.ToString();
    }

    /// <summary>
    /// Writes the complete playthrough transcript to a file in the user data directory.
    /// </summary>
    /// <returns>The path where the transcript was written, or empty string if write failed.</returns>
    public string WritePlaythroughToFile()
    {
        try
        {
            var userDir = OS.GetUserDataDir();
            var timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
            var filename = $"playthrough_{timestamp}.txt";
            var filepath = System.IO.Path.Combine(userDir, filename);

            var transcript = GetPlaythroughTranscript();
            System.IO.File.WriteAllText(filepath, transcript, Encoding.UTF8);

            GD.Print($"[JournalSystem] Playthrough transcript written to: {filepath}");
            return filepath;
        }
        catch (Exception ex)
        {
            GD.PrintErr($"[JournalSystem] Failed to write playthrough transcript: {ex.Message}");
            return string.Empty;
        }
    }

    /// <summary>
    /// Gets the number of stages recorded so far.
    /// </summary>
    public int StageCount => _StageRecords.Count;

    /// <summary>
    /// Gets all recorded stage IDs in order.
    /// </summary>
    public IEnumerable<string> RecordedStageIds => _StageRecords.Select(r => r.StageId);

    /// <summary>
    /// Clears all recorded journal entries (for new playthrough).
    /// </summary>
    public void Clear()
    {
        _StageRecords.Clear();
        GD.Print("[JournalSystem] Journal cleared");
    }
}
