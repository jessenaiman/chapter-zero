using Godot;
using System;
using System.Collections.Generic;

namespace OmegaSpiral.Source.Scripts
{
    /// <summary>
    /// Central game state container that persists across all scenes and manages global game progression.
    /// This is an autoload singleton that maintains continuity across scene transitions.
    /// </summary>
    public partial class GameState : Node
    {
        // Scene progression
        public int CurrentScene { get; set; } = 1;
        public string CurrentScenePath { get; set; } = "res://Source/Scenes/Scene1Narrative.tscn";

        // Player identity and narrative thread
        public string PlayerName { get; set; } = "";
        public DreamweaverThread DreamweaverThread { get; set; } = DreamweaverThread.Hero;
        public string PlayerSecret { get; set; } = "";

        // Dreamweaver alignment system
        public Godot.Collections.Dictionary<string, int> DreamweaverScores { get; set; } = new()
        {
            ["light"] = 0,
            ["mischief"] = 0,
            ["wrath"] = 0
        };
        public DreamweaverType? SelectedDreamweaver { get; set; } = null;

        // Party and character data
        public PartyData PlayerParty { get; set; } = new();

        // Inventory and progression
        public Godot.Collections.Array<string> CollectedShards { get; set; } = new();
        public Godot.Collections.Dictionary<string, object> SceneProgress { get; set; } = new();

        // Save/load metadata
        public DateTime LastSaveTime { get; set; } = DateTime.Now;
        public string SaveVersion { get; set; } = "1.0.0";

        /// <summary>
        /// Process a dungeon choice and update Dreamweaver scores according to the alignment rules.
        /// </summary>
        /// <param name="dungeonOwner">The Dreamweaver who owns the current dungeon</param>
        /// <param name="chosenObject">The object the player chose (door/monster/chest)</param>
        /// <param name="objectAlignment">Which Dreamweaver the chosen object is aligned with</param>
        public void ProcessDungeonChoice(DreamweaverType dungeonOwner, string chosenObject, DreamweaverType objectAlignment)
        {
            if (objectAlignment == dungeonOwner)
            {
                // Bonus for harmony - choosing an object aligned with the dungeon owner
                DreamweaverScores[objectAlignment.ToString().ToLower()] += 2;
            }
            else
            {
                // Cross-alignment - choosing an object aligned with another Dreamweaver
                DreamweaverScores[objectAlignment.ToString().ToLower()] += 1;
            }

            GD.Print($"Choice processed: Dungeon owner={dungeonOwner}, Chosen object={chosenObject}, Object alignment={objectAlignment}");
            GD.Print($"Updated scores: Light={DreamweaverScores["light"]}, Mischief={DreamweaverScores["mischief"]}, Wrath={DreamweaverScores["wrath"]}");
        }

        /// <summary>
        /// Determine the selected Dreamweaver based on final scores.
        /// </summary>
        /// <returns>The Dreamweaver type with the highest score</returns>
        public DreamweaverType GetSelectedDreamweaver()
        {
            string maxKey = "light";
            int maxScore = DreamweaverScores["light"];

            if (DreamweaverScores["mischief"] > maxScore)
            {
                maxKey = "mischief";
                maxScore = DreamweaverScores["mischief"];
            }

            if (DreamweaverScores["wrath"] > maxScore)
            {
                maxKey = "wrath";
                maxScore = DreamweaverScores["wrath"];
            }

            return (DreamweaverType)Enum.Parse(typeof(DreamweaverType), maxKey, true);
        }

        /// <summary>
        /// Reset all game state for a new game run.
        /// </summary>
        public void ResetForNewRun()
        {
            CurrentScene = 1;
            DreamweaverThread = DreamweaverThread.Hero;
            PlayerName = "";
            PlayerSecret = "";
            SelectedDreamweaver = null;
            PlayerParty = new PartyData();

            CollectedShards.Clear();
            SceneProgress.Clear();

            // Reset all Dreamweaver scores
            foreach (var key in DreamweaverScores.Keys)
            {
                DreamweaverScores[key] = 0;
            }

            LastSaveTime = DateTime.Now;
            SaveVersion = "1.0.0";

            GD.Print("GameState reset for new run");
        }

        /// <summary>
        /// Save game state to persistent storage.
        /// </summary>
        public void SaveGame()
        {
            try
            {
                var saveData = new Godot.Collections.Dictionary<string, Variant>
                {
                    ["version"] = SaveVersion,
                    ["timestamp"] = DateTime.Now.ToString("O"),
                    ["currentScene"] = CurrentScene,
                    ["playerName"] = PlayerName,
                    ["dreamweaverThread"] = DreamweaverThread.ToString().ToLower(),
                    ["playerSecret"] = PlayerSecret,
                    ["dreamweaverScores"] = new Godot.Collections.Dictionary<string, int>(DreamweaverScores),
                    ["collectedShards"] = new Godot.Collections.Array<string>(CollectedShards),
                    ["sceneProgress"] = new Godot.Collections.Dictionary<string, Variant>(SceneProgress)
                };

                var jsonString = Json.Stringify(saveData);
                var savePath = "user://savegame.json";

                using var file = FileAccess.Open(savePath, FileAccess.ModeFlags.Write);
                file.StoreString(jsonString);

                GD.Print($"Game saved successfully to {savePath}");
            }
            catch (Exception e)
            {
                GD.PrintErr($"Failed to save game: {e.Message}");
            }
        }

        /// <summary>
        /// Load game state from persistent storage.
        /// </summary>
        /// <returns>True if load was successful, false otherwise</returns>
        public bool LoadGame()
        {
            try
            {
                var savePath = "user://savegame.json";

                if (!FileAccess.FileExists(savePath))
                {
                    GD.Print("No save file found");
                    return false;
                }

                using var file = FileAccess.Open(savePath, FileAccess.ModeFlags.Read);
                var jsonString = file.GetAsText();
                var saveData = Json.ParseString(jsonString).AsGodotDictionary();

                if (saveData == null)
                {
                    GD.PrintErr("Invalid save file format");
                    return false;
                }

                // Load basic state
                CurrentScene = (int)saveData["currentScene"];
                PlayerName = (string)saveData["playerName"];
                DreamweaverThread = (DreamweaverThread)Enum.Parse(typeof(DreamweaverThread), (string)saveData["dreamweaverThread"], true);
                PlayerSecret = (string)saveData["playerSecret"];

                // Load Dreamweaver scores
                var scoresDict = saveData["dreamweaverScores"].AsGodotDictionary();
                DreamweaverScores["light"] = (int)scoresDict["light"];
                DreamweaverScores["mischief"] = (int)scoresDict["mischief"];
                DreamweaverScores["wrath"] = (int)scoresDict["wrath"];

                // Load collections
                var shardsArray = saveData["collectedShards"].AsGodotArray();
                CollectedShards.Clear();
                foreach (var shard in shardsArray)
                {
                    CollectedShards.Add((string)shard);
                }

                GD.Print($"Game loaded successfully from {savePath}");
                return true;
            }
            catch (Exception e)
            {
                GD.PrintErr($"Failed to load game: {e.Message}");
                return false;
            }
        }

        /// <summary>
        /// Initialize the GameState singleton when the node is ready.
        /// </summary>
        public override void _Ready()
        {
            GD.Print("GameState singleton initialized");
        }

        /// <summary>
        /// Clean up when the GameState node is destroyed.
        /// </summary>
        public override void _ExitTree()
        {
            // Auto-save when the game exits
            SaveGame();
            GD.Print("GameState singleton destroyed, auto-saved");
        }
    }
}