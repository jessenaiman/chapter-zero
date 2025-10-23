
// <copyright file="GameState.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using Godot;
using OmegaSpiral.Source.Scripts.persistence;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;

namespace OmegaSpiral.Source.Scripts.Common;

/// <summary>
/// Global game state singleton managing player progress, Dreamweaver alignment, and persistence.
/// FUTURE: Will track LLM consultation history and dynamic narrative state (see ADR-0003).
/// </summary>
[GlobalClass]
public partial class GameState : Node
{
    /// <summary>
    /// Gets or sets the index of the current scene in the game.
    /// </summary>
    public int CurrentScene { get; set; }

    /// <summary>
    /// Gets or sets the player's chosen Dreamweaver narrative thread.
    /// </summary>
    public DreamweaverThread DreamweaverThread { get; set; } = DreamweaverThread.Hero;

    /// <summary>
    /// Gets or sets the mood selected at the press start menu.
    /// </summary>
    public PressStartMood PressStartMood { get; set; } = PressStartMood.Neutral;

    /// <summary>
    /// Gets or sets the player's chosen name.
    /// </summary>
    public string PlayerName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the player's secret response from Scene1.
    /// </summary>
    public string PlayerSecret { get; set; } = string.Empty;

    /// <summary>
    /// Gets the list of story shards collected by the player.
    /// </summary>
    public Collection<string> Shards { get; } = new();

    /// <summary>
    /// Gets scene-specific data stored as key-value pairs.
    /// </summary>
    public Dictionary<string, object> SceneData { get; } = new();

    /// <summary>
    /// Gets the queue of narrator messages to be displayed.
    /// </summary>
    public Collection<string> NarratorQueue { get; } = new();

    /// <summary>
    /// Gets the player's Dreamweaver alignment scores.
    /// </summary>
    public Dictionary<DreamweaverType, int> DreamweaverScores { get; } = new()
    {
        [DreamweaverType.Light] = 0,
        [DreamweaverType.Mischief] = 0,
        [DreamweaverType.Wrath] = 0,
    };

    /// <summary>
    /// Gets the complete history of choices made during gameplay.
    /// </summary>
    /// <remarks>
    /// Tracks all player choices across all stages for narrative continuity and save/load support.
    /// </remarks>
    public Collection<ChoiceRecord> ChoiceHistory { get; } = new();

    /// <summary>
    /// Gets or sets the player's selected Dreamweaver type.
    /// </summary>
    public DreamweaverType? SelectedDreamweaver { get; set; }

    /// <summary>
    /// Gets or sets the timestamp of the last save operation.
    /// </summary>
    public DateTime LastSaveTime { get; set; } = DateTime.Now;

    /// <summary>
    /// Gets or sets the version of the save file format.
    /// </summary>
    public string SaveVersion { get; set; } = "1.0.0";

    /// <summary>
    /// Gets the player's party data including characters and inventory.
    /// </summary>
    public PartyData PlayerParty { get; set; } = new();

    /// <summary>
    /// Gets the save/load manager for database operations.
    /// </summary>
    private SaveLoadManager? saveLoadManager;

    /// <inheritdoc/>
    public override void _Ready()
    {
        try
        {
            // Initialize database context
            var dbPath = Path.Combine(OS.GetUserDataDir(), "omega_spiral_saves.db");
            var options = new DbContextOptionsBuilder<GameDbContext>()
                .UseSqlite($"Data Source={dbPath}")
                .Options;

            var context = new GameDbContext(options);

            // Ensure database is created and migrated
            context.Database.Migrate();

            saveLoadManager = new SaveLoadManager(context);
        }
        catch (Exception ex)
        {
            GD.PrintErr($"Failed to initialize database in GameState: {ex.Message}");
            // Continue without database functionality
        }

        // Set initial values regardless of database status
        this.CurrentScene = 1;
        this.DreamweaverThread = DreamweaverThread.Hero;
        this.PlayerName = string.Empty;
        this.Shards.Clear();
        this.SceneData.Clear();
        this.NarratorQueue.Clear();
        this.PressStartMood = PressStartMood.Neutral;

        // Reset dreamweaver scores
        this.DreamweaverScores[DreamweaverType.Light] = 0;
        this.DreamweaverScores[DreamweaverType.Mischief] = 0;
        this.DreamweaverScores[DreamweaverType.Wrath] = 0;

        this.PlayerParty = new PartyData();
    }

    /// <summary>
    /// Resets the game state for a new run.
    /// </summary>
    public void ResetForNewRun()
    {
        this.CurrentScene = 1;
        this.DreamweaverThread = DreamweaverThread.Hero;
        this.PlayerName = string.Empty;
        this.Shards.Clear();
        this.SceneData.Clear();
        this.NarratorQueue.Clear();

        this.DreamweaverScores[DreamweaverType.Light] = 0;
        this.DreamweaverScores[DreamweaverType.Mischief] = 0;
        this.DreamweaverScores[DreamweaverType.Wrath] = 0;

        this.ChoiceHistory.Clear();
        this.PlayerParty = new PartyData();
        this.PressStartMood = PressStartMood.Neutral;
    }

    // FUTURE: LLM_INTEGRATION - Dreamweaver consultation history
    // Will store LLM responses for replay/analysis and state persistence
    // public List<DreamweaverConsultation> ConsultationHistory { get; set; } = new();
    //
    // public class DreamweaverConsultation
    // {
    //     public DateTime Timestamp { get; set; }
    //     public string Situation { get; set; } = string.Empty;
    //     public string HeroResponse { get; set; } = string.Empty;
    //     public string ShadowResponse { get; set; } = string.Empty;
    //     public string AmbitionResponse { get; set; } = string.Empty;
    //     public string OmegaResponse { get; set; } = string.Empty;
    //     public string PlayerChoice { get; set; } = string.Empty;
    // }
    // Initialize the GameState

    /// <summary>
    /// Updates the score for the specified dreamweaver type.
    /// </summary>
    /// <param name="dreamweaverType">The dreamweaver type to update.</param>
    /// <param name="points">The points to add to the score.</param>
    public void UpdateDreamweaverScore(DreamweaverType dreamweaverType, int points)
    {
        if (this.DreamweaverScores.TryGetValue(dreamweaverType, out int currentScore))
        {
            this.DreamweaverScores[dreamweaverType] = currentScore + points;
        }
    }

    /// <summary>
    /// Gets the dreamweaver type with the highest score.
    /// </summary>
    /// <returns>The dreamweaver type with the highest score.</returns>
    public DreamweaverType GetHighestScoringDreamweaver()
    {
        DreamweaverType topDreamweaver = DreamweaverType.Light;
        int topScore = this.DreamweaverScores[topDreamweaver];

        foreach (var kvp in this.DreamweaverScores)
        {
            if (kvp.Value > topScore)
            {
                topScore = kvp.Value;
                topDreamweaver = kvp.Key;
            }
        }

        return topDreamweaver;
    }

    /// <summary>
    /// Gets the total score across all Dreamweaver types.
    /// </summary>
    /// <returns>The sum of all Dreamweaver scores.</returns>
    public int GetTotalScore()
    {
        return this.DreamweaverScores.Values.Sum();
    }

    /// <summary>
    /// Saves the current game state to the database.
    /// </summary>
    /// <param name="saveSlot">The save slot name (optional, defaults to "default").</param>
    /// <returns>True if the save was successful, false otherwise.</returns>
    public async Task<bool> SaveGameAsync(string saveSlot = "default")
    {
        if (saveLoadManager == null)
        {
            GD.PrintErr("SaveLoadManager not initialized");
            return false;
        }

        this.LastSaveTime = DateTime.Now;
        return await saveLoadManager.SaveGameAsync(this, saveSlot).ConfigureAwait(false);
    }

    /// <summary>
    /// Saves the current game state to the database (synchronous wrapper).
    /// </summary>
    public void SaveGame()
    {
        // Run async save on a background thread
        Task.Run(() => SaveGameAsync()).Wait();
    }

    /// <summary>
    /// Loads the game state from the database.
    /// </summary>
    /// <param name="saveSlot">The save slot name (optional, defaults to "default").</param>
    /// <returns>True if the game was loaded successfully, false otherwise.</returns>
    public async Task<bool> LoadGameAsync(string saveSlot = "default")
    {
        if (saveLoadManager == null)
        {
            GD.PrintErr("SaveLoadManager not initialized");
            return false;
        }

        var loadedState = await saveLoadManager.LoadGameAsync(saveSlot).ConfigureAwait(false);
        if (loadedState == null)
        {
            return false;
        }

        // Copy loaded state to this instance
        CopyFrom(loadedState);
        return true;
    }

    /// <summary>
    /// Loads the game state from the database (synchronous wrapper).
    /// </summary>
    /// <returns>True if the game was loaded successfully, false otherwise.</returns>
    public bool LoadGame()
    {
        return Task.Run(() => LoadGameAsync()).Result;
    }

    /// <summary>
    /// Copies all state from another GameState instance.
    /// </summary>
    /// <param name="other">The GameState to copy from.</param>
    private void CopyFrom(GameState other)
    {
        this.SaveVersion = other.SaveVersion;
        this.LastSaveTime = other.LastSaveTime;
        this.CurrentScene = other.CurrentScene;
        this.DreamweaverThread = other.DreamweaverThread;
        this.PlayerName = other.PlayerName;
        this.PlayerSecret = other.PlayerSecret;
        this.SelectedDreamweaver = other.SelectedDreamweaver;

        this.Shards.Clear();
        foreach (var shard in other.Shards)
        {
            this.Shards.Add(shard);
        }

        this.SceneData.Clear();
        foreach (var kvp in other.SceneData)
        {
            this.SceneData[kvp.Key] = kvp.Value;
        }

        foreach (var kvp in other.DreamweaverScores)
        {
            this.DreamweaverScores[kvp.Key] = kvp.Value;
        }

        this.ChoiceHistory.Clear();
        foreach (var choice in other.ChoiceHistory)
        {
            this.ChoiceHistory.Add(choice);
        }

        this.PlayerParty = other.PlayerParty;

        this.NarratorQueue.Clear();
        foreach (var message in other.NarratorQueue)
        {
            this.NarratorQueue.Add(message);
        }
    }

    private static void LoadProgressState(Godot.Collections.Dictionary<string, Variant> gameStateData)
    {
        if (gameStateData.TryGetValue("sceneProgress", out var progressVar))
        {
            if (progressVar.VariantType == Variant.Type.Dictionary)
            {
                var progressDict = progressVar.AsGodotDictionary<string, Variant>();
                LoadSceneProgress(progressDict);
            }
        }
    }

    private static void LoadSceneProgress(Godot.Collections.Dictionary<string, Variant> progressDict)
    {
        // Scene progress loading logic would go here
        // For now, just log that it's being loaded
        _ = progressDict;
        GD.Print("Loading scene progress data");
    }

    private static string SanitizePlayerName(string name)
    {
        // Basic sanitization - remove potentially harmful characters
        return name.Replace("<", "&lt;").Replace(">", "&gt;").Replace("&", "&amp;");
    }

    // ============================================================================
    // STAGE 1 OPENING SEQUENCE - Choice Tracking & Thread Determination
    // ============================================================================

    /// <summary>
    /// Records a choice made by the player and updates thread scores.
    /// </summary>
    /// <param name="questionId">Unique identifier for the question (e.g., "question1_name")</param>
    /// <param name="choiceText">The text of the choice selected</param>
    /// <param name="lightPoints">Points awarded to Light thread</param>
    /// <param name="shadowPoints">Points awarded to Shadow thread (mapped to Mischief)</param>
    /// <param name="ambitionPoints">Points awarded to Ambition thread (mapped to Wrath)</param>
    /// <exception cref="ArgumentException">
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
            throw new ArgumentException("Question ID cannot be null or empty", nameof(questionId));
        }

        if (string.IsNullOrWhiteSpace(choiceText))
        {
            throw new ArgumentException("Choice text cannot be null or empty", nameof(choiceText));
        }

        // Update scores (map Stage 1 threads to DreamweaverType)
        UpdateDreamweaverScore(DreamweaverType.Light, lightPoints);
        UpdateDreamweaverScore(DreamweaverType.Mischief, shadowPoints); // Shadow -> Mischief
        UpdateDreamweaverScore(DreamweaverType.Wrath, ambitionPoints);  // Ambition -> Wrath

        var record = new ChoiceRecord(
            questionId,
            choiceText,
            lightPoints,
            shadowPoints,
            ambitionPoints);

        ChoiceHistory.Add(record);

        GD.Print($"[GameState] Choice recorded: {questionId} | Light:{DreamweaverScores[DreamweaverType.Light]} Shadow:{DreamweaverScores[DreamweaverType.Mischief]} Ambition:{DreamweaverScores[DreamweaverType.Wrath]}");
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
    /// Balance Ending Criteria: No single thread reaches 60% of total points.
    /// This represents philosophical complexity rather than indecision.
    /// </para>
    /// <para>
    /// Tiebreaker Order (if equal scores): Light → Mischief (Shadow) → Wrath (Ambition)
    /// </para>
    /// </remarks>
    public string GetDominantThread()
    {
        int totalPoints = GetTotalScore();

        if (totalPoints == 0)
        {
            GD.PrintErr("[GameState] Cannot determine thread with zero total points");
            return "Balance"; // Default to Balance if no choices made
        }

        const float dominanceThreshold = 0.6f; // 60%

        int lightScore = DreamweaverScores[DreamweaverType.Light];
        int shadowScore = DreamweaverScores[DreamweaverType.Mischief];
        int ambitionScore = DreamweaverScores[DreamweaverType.Wrath];

        // Check if any thread has achieved dominance
        bool hasDominantThread = (lightScore / (float) totalPoints >= dominanceThreshold) ||
                                  (shadowScore / (float) totalPoints >= dominanceThreshold) ||
                                  (ambitionScore / (float) totalPoints >= dominanceThreshold);

        if (!hasDominantThread)
        {
            return "Balance";
        }

        // Determine highest scoring thread (with tiebreaker: Light > Shadow > Ambition)
        if (lightScore >= shadowScore && lightScore >= ambitionScore)
        {
            return "Light";
        }

        if (shadowScore >= ambitionScore)
        {
            return "Shadow";
        }

        return "Ambition";
    }

    /// <summary>
    /// Gets a formatted summary of current scores for display purposes.
    /// </summary>
    /// <returns>A human-readable string showing point distribution and percentages.</returns>
    public string GetScoreSummary()
    {
        int totalPoints = GetTotalScore();

        if (totalPoints == 0)
        {
            return "No choices recorded yet.";
        }

        int lightScore = DreamweaverScores[DreamweaverType.Light];
        int shadowScore = DreamweaverScores[DreamweaverType.Mischief];
        int ambitionScore = DreamweaverScores[DreamweaverType.Wrath];

        float lightPercent = (float) lightScore / totalPoints * 100f;
        float shadowPercent = (float) shadowScore / totalPoints * 100f;
        float ambitionPercent = (float) ambitionScore / totalPoints * 100f;

        return $"Light: {lightScore} ({lightPercent:F1}%) | " +
               $"Shadow: {shadowScore} ({shadowPercent:F1}%) | " +
               $"Ambition: {ambitionScore} ({ambitionPercent:F1}%) | " +
               $"Total: {totalPoints}";
    }
}

/// <summary>
/// Mood selected on the press start screen.
/// </summary>
public enum PressStartMood
{
    /// <summary>Neutral mood for a balanced game experience.</summary>
    Neutral,

    /// <summary>
    /// Inviting mood for a welcoming game experience.
    /// </summary>
    Inviting,

    /// <summary>
    /// Ominous mood for a darker, more intense experience.
    /// </summary>
    Ominous,
}

/// <summary>
/// Represents a single choice made during gameplay.
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
