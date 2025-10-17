
// <copyright file="GameState.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using Godot;
using OmegaSpiral.Source.Scripts;
using OmegaSpiral.Source.Scripts.Persistence;
using Microsoft.EntityFrameworkCore;

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
    public List<string> Shards { get; } = new();

    /// <summary>
    /// Gets scene-specific data stored as key-value pairs.
    /// </summary>
    public Dictionary<string, object> SceneData { get; } = new();

    /// <summary>
    /// Gets the queue of narrator messages to be displayed.
    /// </summary>
    public List<string> NarratorQueue { get; } = new();

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
    private SaveLoadManager? _saveLoadManager;

    /// <inheritdoc/>
    public override void _Ready()
    {
        // Initialize database context
        var dbPath = Path.Combine(OS.GetUserDataDir(), "omega_spiral_saves.db");
        var options = new DbContextOptionsBuilder<GameDbContext>()
            .UseSqlite($"Data Source={dbPath}")
            .Options;

        var context = new GameDbContext(options);

        // Ensure database is created and migrated
        context.Database.Migrate();

        _saveLoadManager = new SaveLoadManager(context);

        // Set initial values
        this.CurrentScene = 1;
        this.DreamweaverThread = DreamweaverThread.Hero;
        this.PlayerName = string.Empty;
        this.Shards.Clear();
        this.SceneData.Clear();
        this.NarratorQueue.Clear();

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

        this.PlayerParty = new PartyData();
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
    /// Saves the current game state to the database.
    /// </summary>
    /// <param name="saveSlot">The save slot name (optional, defaults to "default").</param>
    /// <returns>True if the save was successful, false otherwise.</returns>
    public async Task<bool> SaveGameAsync(string saveSlot = "default")
    {
        if (_saveLoadManager == null)
        {
            GD.PrintErr("SaveLoadManager not initialized");
            return false;
        }

        this.LastSaveTime = DateTime.Now;
        return await _saveLoadManager.SaveGameAsync(this, saveSlot);
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
        if (_saveLoadManager == null)
        {
            GD.PrintErr("SaveLoadManager not initialized");
            return false;
        }

        var loadedState = await _saveLoadManager.LoadGameAsync(saveSlot);
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
        this.Shards.AddRange(other.Shards);

        this.SceneData.Clear();
        foreach (var kvp in other.SceneData)
        {
            this.SceneData[kvp.Key] = kvp.Value;
        }

        foreach (var kvp in other.DreamweaverScores)
        {
            this.DreamweaverScores[kvp.Key] = kvp.Value;
        }

        this.PlayerParty = other.PlayerParty;

        this.NarratorQueue.Clear();
        this.NarratorQueue.AddRange(other.NarratorQueue);
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
}
