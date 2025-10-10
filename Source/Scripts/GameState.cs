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
    public PartyData PlayerParty { get; set; } = new();

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
    
    public void UpdateDreamweaverScore(DreamweaverType dreamweaverType, int points)
    {
        if (DreamweaverScores.ContainsKey(dreamweaverType))
        {
            DreamweaverScores[dreamweaverType] += points;
        }
    }
    
    public DreamweaverType GetHighestScoringDreamweaver()
    {
        DreamweaverType topDreamweaver = DreamweaverType.Light;
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
                ["sceneProgress"] = new Godot.Collections.Dictionary<string, Variant>(SceneData.ToDictionary(kvp => kvp.Key, kvp => Variant.From(kvp.Value)))
            }
        };

        var jsonString = Json.Stringify(saveData);
        using var file = Godot.FileAccess.Open("user://savegame.json", Godot.FileAccess.ModeFlags.Write);
        file.StoreString(jsonString);
        LastSaveTime = DateTime.Now;
        GD.Print("Game saved successfully");
    }

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

            // Load game state with validation
            if (gameStateData.ContainsKey("currentScene") && gameStateData["currentScene"].VariantType == Variant.Type.Int)
            {
                int sceneId = gameStateData["currentScene"].AsInt32();
                CurrentScene = Mathf.Clamp(sceneId, 1, 5); // Valid scene range
            }

            if (gameStateData.ContainsKey("playerName") && gameStateData["playerName"].VariantType == Variant.Type.String)
            {
                string playerName = gameStateData["playerName"].AsString();
                // Sanitize player name - limit length and remove potentially harmful characters
                PlayerName = SanitizePlayerName(playerName);
            }

            if (gameStateData.ContainsKey("dreamweaverThread") && gameStateData["dreamweaverThread"].VariantType == Variant.Type.String)
            {
                string threadStr = gameStateData["dreamweaverThread"].AsString();
                if (Enum.TryParse<DreamweaverThread>(threadStr, out var thread))
                {
                    DreamweaverThread = thread;
                }
            }

            if (gameStateData.ContainsKey("dreamweaverScores"))
            {
                var scoresVar = gameStateData["dreamweaverScores"];
                if (scoresVar.VariantType == Variant.Type.Dictionary)
                {
                    var scoresDict = scoresVar.AsGodotDictionary<string, Variant>();
                    LoadDreamweaverScores(scoresDict);
                }
            }

            if (gameStateData.ContainsKey("selectedDreamweaver") && gameStateData["selectedDreamweaver"].VariantType == Variant.Type.String)
            {
                string selectedStr = gameStateData["selectedDreamweaver"].AsString();
                if (!string.IsNullOrEmpty(selectedStr) && Enum.TryParse<DreamweaverType>(selectedStr, out var selected))
                {
                    SelectedDreamweaver = selected;
                }
            }

            if (gameStateData.ContainsKey("partyData"))
            {
                var partyVar = gameStateData["partyData"];
                if (partyVar.VariantType == Variant.Type.Dictionary)
                {
                    var partyDict = partyVar.AsGodotDictionary<string, Variant>();
                    PlayerParty = PartyData.FromDictionary(partyDict);
                }
            }

            if (gameStateData.ContainsKey("collectedShards"))
            {
                var shardsVar = gameStateData["collectedShards"];
                if (shardsVar.VariantType == Variant.Type.Array)
                {
                    var shardsArray = shardsVar.AsGodotArray();
                    LoadShards(shardsArray);
                }
            }

            if (gameStateData.ContainsKey("sceneProgress"))
            {
                var progressVar = gameStateData["sceneProgress"];
                if (progressVar.VariantType == Variant.Type.Dictionary)
                {
                    var progressDict = progressVar.AsGodotDictionary<string, Variant>();
                    LoadSceneProgress(progressDict);
                }
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

    private void LoadDreamweaverScores(Godot.Collections.Dictionary<string, Variant> scoresDict)
    {
        foreach (var kvp in scoresDict)
        {
            if (Enum.TryParse<DreamweaverType>(kvp.Key, out var dreamweaverType))
            {
                if (kvp.Value.VariantType == Variant.Type.Int)
                {
                    DreamweaverScores[dreamweaverType] = kvp.Value.AsInt32();
                }
            }
        }
    }

    private void LoadShards(Godot.Collections.Array shardsArray)
    {
        Shards.Clear();
        foreach (var shard in shardsArray)
        {
            if (shard.VariantType == Variant.Type.String)
            {
                string shardName = shard.AsString();
                Shards.Add(shardName);
            }
        }
    }

    private void LoadSceneProgress(Godot.Collections.Dictionary<string, Variant> progressDict)
    {
        // Scene progress loading logic would go here
        // For now, just log that it's being loaded
        GD.Print("Loading scene progress data");
    }

    private string SanitizePlayerName(string name)
    {
        // Basic sanitization - remove potentially harmful characters
        return name.Replace("<", "&lt;").Replace(">", "&gt;").Replace("&", "&amp;");
    }
}