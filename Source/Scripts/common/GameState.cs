// <copyright file="GameState.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using Godot;
using OmegaSpiral.Source.Scripts;
using OmegaSpiral.Source.Scripts.Common;

/// <summary>
/// Global game state singleton managing player progress, Dreamweaver alignment, and persistence.
/// FUTURE: Will track LLM consultation history and dynamic narrative state (see ADR-0003).
/// </summary>
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
    public List<string> Shards { get; } = new ();

    /// <summary>
    /// Gets scene-specific data stored as key-value pairs.
    /// </summary>
    public Dictionary<string, object> SceneData { get; } = new ();

    /// <summary>
    /// Gets the queue of narrator messages to be displayed.
    /// </summary>
    public List<string> NarratorQueue { get; } = new ();

    /// <summary>
    /// Gets the player's Dreamweaver alignment scores.
    /// </summary>
    public Dictionary<DreamweaverType, int> DreamweaverScores { get; } = new ()
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
    /// Gets or sets the player's party data including characters and inventory.
    /// </summary>
    public PartyData PlayerParty { get; set; } = new ();

    /// <inheritdoc/>
    public override void _Ready()
    {
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
        if (this.DreamweaverScores.ContainsKey(dreamweaverType))
        {
            this.DreamweaverScores[dreamweaverType] += points;
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
    /// Saves the current game state to a file.
    /// </summary>
    public void SaveGame()
    {
        var saveData = new Godot.Collections.Dictionary<string, Variant>
        {
            ["version"] = this.SaveVersion,
            ["timestamp"] = this.LastSaveTime.ToString("O"),
            ["gameState"] = new Godot.Collections.Dictionary<string, Variant>
            {
                ["currentScene"] = this.CurrentScene,
                ["playerName"] = this.PlayerName,
                ["dreamweaverThread"] = this.DreamweaverThread.ToString(),
                ["dreamweaverScores"] = new Godot.Collections.Dictionary<string, int>
                {
                    ["Light"] = this.DreamweaverScores[DreamweaverType.Light],
                    ["Mischief"] = this.DreamweaverScores[DreamweaverType.Mischief],
                    ["Wrath"] = this.DreamweaverScores[DreamweaverType.Wrath],
                },
                ["selectedDreamweaver"] = this.SelectedDreamweaver?.ToString() ?? string.Empty,
                ["partyData"] = this.PlayerParty.ToDictionary(),
                ["collectedShards"] = new Godot.Collections.Array<string>(this.Shards),
                ["sceneProgress"] = new Godot.Collections.Dictionary<string, Variant>(this.SceneData.ToDictionary(kvp => kvp.Key, kvp => Variant.From(kvp.Value))),
            },
        };

        var jsonString = Json.Stringify(saveData);
        using var file = Godot.FileAccess.Open("user://savegame.json", Godot.FileAccess.ModeFlags.Write);
        file.StoreString(jsonString);
        this.LastSaveTime = DateTime.Now;
        GD.Print("Game saved successfully");
    }

    /// <summary>
    /// Loads the game state from a file.
    /// </summary>
    /// <returns>True if the game was loaded successfully, false otherwise.</returns>
    public bool LoadGame()
    {
        if (!Godot.FileAccess.FileExists("user://savegame.json"))
        {
            GD.Print("No save file found");
            return false;
        }

        try
        {
            using var file = Godot.FileAccess.Open("user://savegame.json", Godot.FileAccess.ModeFlags.Read);
            var jsonString = file.GetAsText();
            var jsonNode = Json.ParseString(jsonString);
            if (jsonNode.VariantType != Variant.Type.Dictionary)
            {
                GD.Print("Invalid save file format");
                return false;
            }

            var saveData = jsonNode.AsGodotDictionary<string, Variant>();
            if (!saveData.ContainsKey("gameState"))
            {
                GD.Print("Invalid save file structure");
                return false;
            }

            var gameStateVar = saveData["gameState"];
            if (gameStateVar.VariantType != Variant.Type.Dictionary)
            {
                GD.Print("Invalid save file structure");
                return false;
            }

            var gameStateData = gameStateVar.AsGodotDictionary<string, Variant>();
            this.LoadCoreState(gameStateData);
            this.LoadPartyState(gameStateData);
            LoadProgressState(gameStateData);
            GD.Print("Game loaded successfully");
            return true;
        }
        catch (Exception ex)
        {
            GD.PrintErr($"Error loading game: {ex.Message}");
            throw;
        }
    }

    private static void LoadProgressState(Godot.Collections.Dictionary<string, Variant> gameStateData)
    {
        if (gameStateData.ContainsKey("sceneProgress"))
        {
            var progressVar = gameStateData["sceneProgress"];
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

    private void LoadCoreState(Godot.Collections.Dictionary<string, Variant> gameStateData)
    {
        if (gameStateData.ContainsKey("currentScene") && gameStateData["currentScene"].VariantType == Variant.Type.Int)
        {
            int sceneId = gameStateData["currentScene"].AsInt32();
            this.CurrentScene = Mathf.Clamp(sceneId, 1, 5);
        }

        if (gameStateData.ContainsKey("playerName") && gameStateData["playerName"].VariantType == Variant.Type.String)
        {
            string playerName = gameStateData["playerName"].AsString();
            this.PlayerName = SanitizePlayerName(playerName);
        }

        if (gameStateData.ContainsKey("dreamweaverThread") && gameStateData["dreamweaverThread"].VariantType == Variant.Type.String)
        {
            string threadStr = gameStateData["dreamweaverThread"].AsString();
            if (Enum.TryParse<DreamweaverThread>(threadStr, out var thread))
            {
                this.DreamweaverThread = thread;
            }
        }

        if (gameStateData.ContainsKey("dreamweaverScores"))
        {
            var scoresVar = gameStateData["dreamweaverScores"];
            if (scoresVar.VariantType == Variant.Type.Dictionary)
            {
                var scoresDict = scoresVar.AsGodotDictionary<string, Variant>();
                this.LoadDreamweaverScores(scoresDict);
            }
        }

        if (gameStateData.ContainsKey("selectedDreamweaver") && gameStateData["selectedDreamweaver"].VariantType == Variant.Type.String)
        {
            string selectedStr = gameStateData["selectedDreamweaver"].AsString();
            if (!string.IsNullOrEmpty(selectedStr) && Enum.TryParse<DreamweaverType>(selectedStr, out var selected))
            {
                this.SelectedDreamweaver = selected;
            }
        }
    }

    private void LoadPartyState(Godot.Collections.Dictionary<string, Variant> gameStateData)
    {
        if (gameStateData.ContainsKey("partyData"))
        {
            var partyVar = gameStateData["partyData"];
            if (partyVar.VariantType == Variant.Type.Dictionary)
            {
                var partyDict = partyVar.AsGodotDictionary<string, Variant>();
                this.PlayerParty = PartyData.FromDictionary(partyDict);
            }
        }

        if (gameStateData.ContainsKey("collectedShards"))
        {
            var shardsVar = gameStateData["collectedShards"];
            if (shardsVar.VariantType == Variant.Type.Array)
            {
                var shardsArray = shardsVar.AsGodotArray();
                this.LoadShards(shardsArray);
            }
        }
    }

    private void LoadDreamweaverScores(Godot.Collections.Dictionary<string, Variant> scoresDict)
    {
        foreach (var kvp in scoresDict)
        {
            if (Enum.TryParse<DreamweaverType>(kvp.Key, out var dreamweaverType))
            {
                if (kvp.Value.VariantType == Variant.Type.Int)
                {
                    this.DreamweaverScores[dreamweaverType] = kvp.Value.AsInt32();
                }
            }
        }
    }

    private void LoadShards(Godot.Collections.Array shardsArray)
    {
        this.Shards.Clear();
        foreach (var shard in shardsArray)
        {
            if (shard.VariantType == Variant.Type.String)
            {
                string shardName = shard.AsString();
                this.Shards.Add(shardName);
            }
        }
    }
}
