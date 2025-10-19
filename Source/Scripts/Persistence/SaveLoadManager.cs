// <copyright file="SaveLoadManager.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using System.Text.Json;
using Godot;
using Microsoft.EntityFrameworkCore;
using OmegaSpiral.Source.Scripts.Common;
using OmegaSpiral.Source.Scripts.Persistence;

namespace OmegaSpiral.Source.Scripts.Persistence
{
    /// <summary>
    /// Manages saving and loading game state using Entity Framework Core with SQLite.
    /// </summary>
    public class SaveLoadManager
    {
        private readonly GameDbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="SaveLoadManager"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        public SaveLoadManager(GameDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        /// <summary>
        /// Saves the current game state to the database.
        /// </summary>
        /// <param name="gameState">The game state to save.</param>
        /// <param name="saveSlot">The save slot name.</param>
        /// <returns>True if the save was successful, false otherwise.</returns>
        public async Task<bool> SaveGameAsync(GameState gameState, string saveSlot = "default")
        {
            try
            {
                // Check if save already exists
                var existingSave = await _context.GameSaves!
                    .FirstOrDefaultAsync(gs => gs.SaveSlot == saveSlot);

                if (existingSave != null)
                {
                    // Update existing save
                    UpdateGameSaveFromGameState(existingSave, gameState);
                    existingSave.LastModifiedAt = DateTime.UtcNow;
                }
                else
                {
                    // Create new save
                    var newSave = CreateGameSaveFromGameState(gameState, saveSlot);
                    _context.GameSaves!.Add(newSave);
                }

                await _context.SaveChangesAsync();
                GD.Print($"Game saved successfully to slot: {saveSlot}");
                return true;
            }
            catch (Exception ex)
            {
                GD.PrintErr($"Error saving game: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Loads the game state from the database.
        /// </summary>
        /// <param name="saveSlot">The save slot name.</param>
        /// <returns>The loaded game state, or null if loading failed.</returns>
        public async Task<GameState?> LoadGameAsync(string saveSlot = "default")
        {
            try
            {
                var gameSave = await _context.GameSaves!
                    .Include(gs => gs.Shards)
                    .Include(gs => gs.SceneData)
                    .Include(gs => gs.DreamweaverScores)
                    .Include(gs => gs.PartyData)
                        .ThenInclude(pd => pd!.Members)
                    .Include(gs => gs.NarratorQueue)
                    .FirstOrDefaultAsync(gs => gs.SaveSlot == saveSlot);

                if (gameSave == null)
                {
                    GD.Print($"No save found for slot: {saveSlot}");
                    return null;
                }

                var gameState = CreateGameStateFromGameSave(gameSave);
                GD.Print($"Game loaded successfully from slot: {saveSlot}");
                return gameState;
            }
            catch (Exception ex)
            {
                GD.PrintErr($"Error loading game: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Gets a list of available save slots.
        /// </summary>
        /// <returns>A list of save slot names.</returns>
        public async Task<List<string>> GetAvailableSaveSlotsAsync()
        {
            try
            {
                return await _context.GameSaves!
                    .OrderByDescending(gs => gs.LastModifiedAt)
                    .Select(gs => gs.SaveSlot)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                GD.PrintErr($"Error getting save slots: {ex.Message}");
                return new List<string>();
            }
        }

        /// <summary>
        /// Deletes a save slot.
        /// </summary>
        /// <param name="saveSlot">The save slot to delete.</param>
        /// <returns>True if the deletion was successful, false otherwise.</returns>
        public async Task<bool> DeleteSaveSlotAsync(string saveSlot)
        {
            try
            {
                var gameSave = await _context.GameSaves!
                    .FirstOrDefaultAsync(gs => gs.SaveSlot == saveSlot);

                if (gameSave == null)
                {
                    return false;
                }

                _context.GameSaves!.Remove(gameSave);
                await _context.SaveChangesAsync();
                GD.Print($"Save slot deleted: {saveSlot}");
                return true;
            }
            catch (Exception ex)
            {
                GD.PrintErr($"Error deleting save slot: {ex.Message}");
                return false;
            }
        }

        private static GameSave CreateGameSaveFromGameState(GameState gameState, string saveSlot)
        {
            var gameSave = new GameSave
            {
                SaveSlot = saveSlot,
                SaveVersion = gameState.SaveVersion,
                CreatedAt = DateTime.UtcNow,
                LastModifiedAt = DateTime.UtcNow,
                CurrentScene = gameState.CurrentScene,
                DreamweaverThread = gameState.DreamweaverThread,
                PlayerName = gameState.PlayerName,
                PlayerSecret = gameState.PlayerSecret,
                SelectedDreamweaver = gameState.SelectedDreamweaver,
            };

            // Add story shards
            foreach (var shard in gameState.Shards)
            {
                gameSave.Shards.Add(new StoryShard { Content = shard });
            }

            // Add scene data
            foreach (var kvp in gameState.SceneData)
            {
                gameSave.SceneData.Add(new SceneData
                {
                    Key = kvp.Key,
                    Value = JsonSerializer.Serialize(kvp.Value)
                });
            }

            // Add Dreamweaver scores
            foreach (var kvp in gameState.DreamweaverScores)
            {
                gameSave.DreamweaverScores.Add(new DreamweaverScoreEntity
                {
                    DreamweaverType = kvp.Key,
                    Score = kvp.Value
                });
            }

            // Add party data
            if (gameState.PlayerParty != null)
            {
                var partyData = new PartySaveData
                {
                    Gold = gameState.PlayerParty.Gold,
                    InventoryJson = JsonSerializer.Serialize(gameState.PlayerParty.Inventory),
                };

                foreach (var member in gameState.PlayerParty.Members)
                {
                    partyData.Members.Add(new CharacterSaveData
                    {
                        Name = member.Name,
                        Class = member.Class,
                        Race = member.Race,
                        Level = member.Level,
                        Experience = member.Experience,
                        StatsJson = JsonSerializer.Serialize(member.Stats),
                    });
                }

                gameSave.PartyData = partyData;
            }

            // Add narrator queue
            for (int i = 0; i < gameState.NarratorQueue.Count; i++)
            {
                gameSave.NarratorQueue.Add(new NarratorMessage
                {
                    Content = gameState.NarratorQueue[i],
                    Order = i
                });
            }

            return gameSave;
        }

        private static void UpdateGameSaveFromGameState(GameSave gameSave, GameState gameState)
        {
            gameSave.SaveVersion = gameState.SaveVersion;
            gameSave.CurrentScene = gameState.CurrentScene;
            gameSave.DreamweaverThread = gameState.DreamweaverThread;
            gameSave.PlayerName = gameState.PlayerName;
            gameSave.PlayerSecret = gameState.PlayerSecret;
            gameSave.SelectedDreamweaver = gameState.SelectedDreamweaver;

            // Clear existing collections
            gameSave.Shards.Clear();
            gameSave.SceneData.Clear();
            gameSave.DreamweaverScores.Clear();
            gameSave.NarratorQueue.Clear();

            // Re-populate collections
            foreach (var shard in gameState.Shards)
            {
                gameSave.Shards.Add(new StoryShard { Content = shard });
            }

            foreach (var kvp in gameState.SceneData)
            {
                gameSave.SceneData.Add(new SceneData
                {
                    Key = kvp.Key,
                    Value = JsonSerializer.Serialize(kvp.Value)
                });
            }

            foreach (var kvp in gameState.DreamweaverScores)
            {
                gameSave.DreamweaverScores.Add(new DreamweaverScoreEntity
                {
                    DreamweaverType = kvp.Key,
                    Score = kvp.Value
                });
            }

            for (int i = 0; i < gameState.NarratorQueue.Count; i++)
            {
                gameSave.NarratorQueue.Add(new NarratorMessage
                {
                    Content = gameState.NarratorQueue[i],
                    Order = i
                });
            }

            // Update party data
            if (gameState.PlayerParty != null)
            {
                if (gameSave.PartyData == null)
                {
                    gameSave.PartyData = new PartySaveData();
                }

                gameSave.PartyData.Gold = gameState.PlayerParty.Gold;
                gameSave.PartyData.InventoryJson = JsonSerializer.Serialize(gameState.PlayerParty.Inventory);
                gameSave.PartyData.Members.Clear();

                foreach (var member in gameState.PlayerParty.Members)
                {
                    gameSave.PartyData.Members.Add(new CharacterSaveData
                    {
                        Name = member.Name,
                        Class = member.Class,
                        Race = member.Race,
                        Level = member.Level,
                        Experience = member.Experience,
                        StatsJson = JsonSerializer.Serialize(member.Stats),
                    });
                }
            }
            else
            {
                gameSave.PartyData = null;
            }
        }

        private static GameState CreateGameStateFromGameSave(GameSave gameSave)
        {
            var gameState = new GameState();

            gameState.SaveVersion = gameSave.SaveVersion;
            gameState.LastSaveTime = gameSave.LastModifiedAt;
            gameState.CurrentScene = gameSave.CurrentScene;
            gameState.DreamweaverThread = gameSave.DreamweaverThread;
            gameState.PlayerName = gameSave.PlayerName ?? string.Empty;
            gameState.PlayerSecret = gameSave.PlayerSecret ?? string.Empty;
            gameState.SelectedDreamweaver = gameSave.SelectedDreamweaver;

            // Load story shards
            gameState.Shards.Clear();
            foreach (var shard in gameSave.Shards.OrderBy(s => s.Id))
            {
                gameState.Shards.Add(shard.Content);
            }

            // Load scene data
            gameState.SceneData.Clear();
            foreach (var sceneData in gameSave.SceneData)
            {
                try
                {
                    var value = JsonSerializer.Deserialize<object>(sceneData.Value);
                    if (value != null)
                    {
                        gameState.SceneData[sceneData.Key] = value;
                    }
                }
                catch (Exception ex)
                {
                    GD.PrintErr($"Error deserializing scene data for key {sceneData.Key}: {ex.Message}");
                }
            }

            // Load Dreamweaver scores
            foreach (var score in gameSave.DreamweaverScores)
            {
                gameState.DreamweaverScores[score.DreamweaverType] = score.Score;
            }

            // Load party data
            if (gameSave.PartyData != null)
            {
                gameState.PlayerParty = new PartyData();
                gameState.PlayerParty.Gold = gameSave.PartyData.Gold;

                try
                {
                    var inventory = JsonSerializer.Deserialize<Godot.Collections.Dictionary<string, int>>(gameSave.PartyData.InventoryJson);
                    if (inventory != null)
                    {
                        gameState.PlayerParty.Inventory.Clear();
                        foreach (var kvp in inventory)
                        {
                            gameState.PlayerParty.Inventory[kvp.Key] = kvp.Value;
                        }
                    }
                }
                catch (Exception ex)
                {
                    GD.PrintErr($"Error deserializing party inventory: {ex.Message}");
                }

                foreach (var member in gameSave.PartyData.Members)
                {
                    try
                    {
                        var stats = JsonSerializer.Deserialize<CharacterStats>(member.StatsJson);
                        if (stats != null)
                        {
                            var character = new Character(member.Name, member.Class, member.Race)
                            {
                                Level = member.Level,
                                Experience = member.Experience,
                                Stats = stats,
                            };
                            gameState.PlayerParty.Members.Add(character);
                        }
                    }
                    catch (Exception ex)
                    {
                        GD.PrintErr($"Error deserializing character {member.Name}: {ex.Message}");
                    }
                }
            }

            // Load narrator queue
            gameState.NarratorQueue.Clear();
            foreach (var message in gameSave.NarratorQueue.OrderBy(nm => nm.Order))
            {
                gameState.NarratorQueue.Add(message.Content);
            }

            return gameState;
        }
    }
}
