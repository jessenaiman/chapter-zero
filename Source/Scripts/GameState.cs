using Godot;
using System;
using System.Collections.Generic;
using OmegaSpiral.Source.Scripts;

public partial class GameState : Node
{
    public int CurrentScene { get; set; }
    public DreamweaverThread DreamweaverThread { get; set; } = DreamweaverThread.Hero;
    public string PlayerName { get; set; } = string.Empty;
    public List<string> Shards { get; set; } = new();
    public Dictionary<string, object> SceneData { get; set; } = new();
    public List<string> NarratorQueue { get; set; } = new();
    public Dictionary<DreamweaverType, int> DreamweaverScores { get; set; } = new()
    {
        [DreamweaverType.Light] = 0,
        [DreamweaverType.Mischief] = 0,
        [DreamweaverType.Wrath] = 0
    };
    public DreamweaverType? SelectedDreamweaver { get; set; } = null;
    public DateTime LastSaveTime { get; set; } = DateTime.Now;
    public string SaveVersion { get; set; } = "1.0.0";

    // Initialize the GameState
    public override void _Ready()
    {
        // Set initial values
        CurrentScene = 1;
        DreamweaverThread = DreamweaverThread.Hero;
        PlayerName = string.Empty;
        Shards.Clear();
        SceneData.Clear();
        NarratorQueue.Clear();
        
        // Reset dreamweaver scores
        DreamweaverScores[DreamweaverType.Light] = 0;
        DreamweaverScores[DreamweaverType.Mischief] = 0;
        DreamweaverScores[DreamweaverType.Wrath] = 0;
        
        PlayerParty = new PartyData();
    }

    public void ResetForNewRun()
    {
        CurrentScene = 1;
        DreamweaverThread = DreamweaverThread.Hero;
        PlayerName = string.Empty;
        Shards.Clear();
        SceneData.Clear();
        NarratorQueue.Clear();
        
        DreamweaverScores[DreamweaverType.Light] = 0;
        DreamweaverScores[DreamweaverType.Mischief] = 0;
        DreamweaverScores[DreamweaverType.Wrath] = 0;
        
        PlayerParty = new PartyData();
    }
    
    public void UpdateDreamweaverScore(string dreamweaverType, int points)
    {
        if (DreamweaverScores.ContainsKey(dreamweaverType))
        {
            DreamweaverScores[dreamweaverType] += points;
        }
    }
    
    public string GetHighestScoringDreamweaver()
    {
        string topDreamweaver = "light";
        int topScore = DreamweaverScores[topDreamweaver];
        
        foreach (var kvp in DreamweaverScores)
        {
            if (kvp.Value > topScore)
            {
                topScore = kvp.Value;
                topDreamweaver = kvp.Key;
            }
        }
        
        return topDreamweaver;
    }

    public void SaveGame()
    {
        var saveData = new Godot.Collections.Dictionary<string, Variant>
        {
            ["version"] = SaveVersion,
            ["timestamp"] = LastSaveTime.ToString("O"),
            ["gameState"] = new Godot.Collections.Dictionary<string, Variant>
            {
                ["currentScene"] = CurrentScene,
                ["playerName"] = PlayerName,
                ["dreamweaverThread"] = DreamweaverThread.ToString(),
                ["dreamweaverScores"] = new Godot.Collections.Dictionary<string, int>
                {
                    ["Light"] = DreamweaverScores[DreamweaverType.Light],
                    ["Mischief"] = DreamweaverScores[DreamweaverType.Mischief],
                    ["Wrath"] = DreamweaverScores[DreamweaverType.Wrath]
                },
                ["selectedDreamweaver"] = SelectedDreamweaver?.ToString() ?? "",
                ["partyData"] = PlayerParty.ToDictionary(),
                ["collectedShards"] = new Godot.Collections.Array<string>(Shards),
                ["sceneProgress"] = new Godot.Collections.Dictionary<string, Variant>(SceneData)
            }
        };

        var jsonString = Json.Stringify(saveData);
        using var file = FileAccess.Open("user://savegame.json", FileAccess.ModeFlags.Write);
        file.StoreString(jsonString);
        LastSaveTime = DateTime.Now;
        GD.Print("Game saved successfully");
    }

    public bool LoadGame()
    {
        if (!FileAccess.FileExists("user://savegame.json"))
        {
            GD.Print("No save file found");
            return false;
        }

        try
        {
            using var file = FileAccess.Open("user://savegame.json", FileAccess.ModeFlags.Read);
            var jsonString = file.GetAsText();
            var jsonNode = Json.ParseString(jsonString);

            if (jsonNode is not Godot.Collections.Dictionary saveData)
            {
                GD.Print("Invalid save file format");
                return false;
            }

            if (!saveData.ContainsKey("gameState") || saveData["gameState"].Obj is not Godot.Collections.Dictionary gameStateData)
            {
                GD.Print("Invalid save file structure");
                return false;
            }

            // Load game state with validation
            if (gameStateData.ContainsKey("currentScene") && gameStateData["currentScene"].Obj is int sceneId)
            {
                CurrentScene = Mathf.Clamp(sceneId, 1, 5); // Valid scene range
            }

            if (gameStateData.ContainsKey("playerName") && gameStateData["playerName"].Obj is string playerName)
            {
                // Sanitize player name - limit length and remove potentially harmful characters
                PlayerName = SanitizePlayerName(playerName);
            }

            if (gameStateData.ContainsKey("dreamweaverThread") && gameStateData["dreamweaverThread"].Obj is string threadStr)
            {
                if (Enum.TryParse<DreamweaverThread>(threadStr, out var thread))
                {
                    DreamweaverThread = thread;
                }
            }

            if (gameStateData.ContainsKey("dreamweaverScores") && gameStateData["dreamweaverScores"].Obj is Godot.Collections.Dictionary scoresDict)
            {
                LoadDreamweaverScores(scoresDict);
            }

            if (gameStateData.ContainsKey("selectedDreamweaver") && gameStateData["selectedDreamweaver"].Obj is string selectedStr)
            {
                if (!string.IsNullOrEmpty(selectedStr) && Enum.TryParse<DreamweaverType>(selectedStr, out var selected))
                {
                    SelectedDreamweaver = selected;
                }
            }

            if (gameStateData.ContainsKey("partyData") && gameStateData["partyData"].Obj is Godot.Collections.Dictionary partyDict)
            {
                PlayerParty = PartyData.FromDictionary(partyDict);
            }

            if (gameStateData.ContainsKey("collectedShards") && gameStateData["collectedShards"].Obj is Godot.Collections.Array shardsArray)
            {
                LoadShards(shardsArray);
            }

            if (gameStateData.ContainsKey("sceneProgress") && gameStateData["sceneProgress"].Obj is Godot.Collections.Dictionary progressDict)
            {
                LoadSceneProgress(progressDict);
            }

            GD.Print("Game loaded successfully");
            return true;
        }
        catch (Exception ex)
        {
            GD.PrintErr($"Error loading game: {ex.Message}");
            return false;
        }
    }
}