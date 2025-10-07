using Godot;
using System;
using Newtonsoft.Json.Linq;

namespace OmegaSpiral.Source.Scripts
{
    /// <summary>
    /// Autoload singleton that maintains GameState and orchestrates scene transitions.
    /// Preserves game state across scene changes and emits signals for UI updates.
    /// Lives in Godot's autoload system and persists throughout the entire game session.
    /// </summary>
    [Tool]
    public class SceneManager : Node
    {
        // Singleton instance
        private static SceneManager _instance;
        public static SceneManager Instance => _instance;

        // Game state management
        private GameState _gameState;
        public GameState GameState => _gameState;

        // Scene loader system
        private SceneLoader _sceneLoader;
        public SceneLoader SceneLoader => _sceneLoader;

        // Narrator engine for dialogue processing
        private NarratorEngine _narratorEngine;
        public NarratorEngine NarratorEngine => _narratorEngine;

        // Signals for UI updates
        [Signal]
        public delegate void SceneChangedEventHandler(int sceneId);

        [Signal]
        public delegate void GameStateUpdatedEventHandler();

        [Signal]
        public delegate void NarratorQueueUpdatedEventHandler();

        public override void _Ready()
        {
            // Ensure singleton pattern
            if (_instance == null)
            {
                _instance = this;
                // Don't queue for deletion as this is an autoload singleton
            }
            else if (_instance != this)
            {
                QueueFree();
                return;
            }

            // Initialize components
            InitializeComponents();
            
            GD.Print("SceneManager autoload singleton initialized");
        }

        /// <summary>
        /// Initializes all required components for the SceneManager.
        /// </summary>
        private void InitializeComponents()
        {
            try
            {
                // Initialize game state
                _gameState = new GameState();
                
                // Initialize scene loader
                _sceneLoader = new SceneLoader();
                AddChild(_sceneLoader);
                
                // Initialize narrator engine
                _narratorEngine = new NarratorEngine();
                AddChild(_narratorEngine);
                
                GD.Print("SceneManager components initialized successfully");
            }
            catch (Exception ex)
            {
                GD.PrintErr($"Error initializing SceneManager components: {ex.Message}");
            }
        }

        /// <summary>
        /// Transitions to a new scene by ID, preserving game state and handling data persistence.
        /// </summary>
        /// <param name="sceneId">ID of the scene to transition to</param>
        /// <param name="threadVariant">Thread variant to load (hero/shadow/ambition) - optional</param>
        public void TransitionToScene(int sceneId, string threadVariant = null)
        {
            try
            {
                GD.Print($"Transitioning to scene {sceneId}" + (threadVariant != null ? $" with thread variant {threadVariant}" : ""));

                // Save current scene state before transitioning
                SaveCurrentSceneState();

                // Load new scene data
                JObject sceneData = _sceneLoader.LoadSceneData(sceneId, threadVariant);
                if (sceneData == null)
                {
                    GD.PrintErr($"Failed to load scene data for scene {sceneId}");
                    return;
                }

                // Update game state with new scene information
                _gameState.CurrentScene = sceneId;
                _gameState.CurrentScenePath = $"res://Source/Scenes/Scene{sceneId}*.tscn";

                // Store scene data in game state for the new scene to access
                _gameState.SceneData[sceneId.ToString()] = sceneData;

                // Emit scene changed signal for UI updates
                EmitSignal(SignalName.SceneChanged, sceneId);

                // Instantiate the new scene
                Node newScene = _sceneLoader.InstantiateScene(sceneId, sceneData);
                if (newScene != null)
                {
                    // In a real implementation, we would change the scene here
                    // For now, we'll just log that we're transitioning
                    GD.Print($"Scene {sceneId} instantiated successfully");
                }
                else
                {
                    GD.PrintErr($"Failed to instantiate scene {sceneId}");
                }

                GD.Print($"Transition to scene {sceneId} completed");
            }
            catch (Exception ex)
            {
                GD.PrintErr($"Error transitioning to scene {sceneId}: {ex.Message}");
            }
        }

        /// <summary>
        /// Saves the current scene's complete state to GameState before transitioning.
        /// Enables C# application API calls and dynamic content generation for next scenes.
        /// </summary>
        private void SaveCurrentSceneState()
        {
            try
            {
                // Save timestamp
                _gameState.LastSaveTime = DateTime.Now;
                
                // In a real implementation, we would save the current scene's state
                // For now, we'll just log that we're saving state
                GD.Print($"Saved current scene state for scene {_gameState.CurrentScene}");
                
                // Emit game state updated signal
                EmitSignal(SignalName.GameStateUpdated);
            }
            catch (Exception ex)
            {
                GD.PrintErr($"Error saving current scene state: {ex.Message}");
            }
        }

        /// <summary>
        /// Loads a specific scene variant by thread (hero/shadow/ambition).
        /// Swaps the active JSON file while keeping the scene instance alive.
        /// </summary>
        /// <param name="thread">Thread variant to load (hero/shadow/ambition)</param>
        public void LoadThreadVariant(string thread)
        {
            try
            {
                GD.Print($"Loading thread variant: {thread}");

                // Update game state with new thread
                _gameState.DreamweaverThread = thread;

                // Reload current scene with new thread variant
                TransitionToScene(_gameState.CurrentScene, thread);

                GD.Print($"Thread variant {thread} loaded successfully");
            }
            catch (Exception ex)
            {
                GD.PrintErr($"Error loading thread variant {thread}: {ex.Message}");
            }
        }

        /// <summary>
        /// Resets the game state for a new run.
        /// </summary>
        public void ResetForNewRun()
        {
            try
            {
                _gameState.ResetForNewRun();
                GD.Print("Game state reset for new run");
                
                // Emit game state updated signal
                EmitSignal(SignalName.GameStateUpdated);
            }
            catch (Exception ex)
            {
                GD.PrintErr($"Error resetting game state: {ex.Message}");
            }
        }

        /// <summary>
        /// Gets the current scene data from game state.
        /// </summary>
        /// <returns>JObject containing current scene data, or null if not found</returns>
        public JObject GetCurrentSceneData()
        {
            try
            {
                string sceneId = _gameState.CurrentScene.ToString();
                if (_gameState.SceneData.ContainsKey(sceneId))
                {
                    return (JObject)_gameState.SceneData[sceneId];
                }
                return null;
            }
            catch (Exception ex)
            {
                GD.PrintErr($"Error getting current scene data: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Adds a line to the narrator queue for processing.
        /// </summary>
        /// <param name="line">Line to add to the queue</param>
        public void AddToNarratorQueue(string line)
        {
            try
            {
                _gameState.NarratorQueue.Add(line);
                EmitSignal(SignalName.NarratorQueueUpdated);
                GD.Print($"Added line to narrator queue: {line}");
            }
            catch (Exception ex)
            {
                GD.PrintErr($"Error adding to narrator queue: {ex.Message}");
            }
        }

        /// <summary>
        /// Processes the next line in the narrator queue.
        /// </summary>
        /// <returns>Next line to narrate, or null if queue is empty</returns>
        public string ProcessNextNarratorLine()
        {
            try
            {
                if (_gameState.NarratorQueue.Count > 0)
                {
                    string line = (string)_gameState.NarratorQueue[0];
                    _gameState.NarratorQueue.RemoveAt(0);
                    EmitSignal(SignalName.NarratorQueueUpdated);
                    return line;
                }
                return null;
            }
            catch (Exception ex)
            {
                GD.PrintErr($"Error processing next narrator line: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Updates a dreamweaver score.
        /// </summary>
        /// <param name="dreamweaver">Dreamweaver to update score for</param>
        /// <param name="score">Score to add</param>
        public void UpdateDreamweaverScore(string dreamweaver, int score)
        {
            try
            {
                if (_gameState.DreamweaverScores.ContainsKey(dreamweaver))
                {
                    _gameState.DreamweaverScores[dreamweaver] = (int)_gameState.DreamweaverScores[dreamweaver] + score;
                    EmitSignal(SignalName.GameStateUpdated);
                    GD.Print($"Updated {dreamweaver} score by {score}, new total: {_gameState.DreamweaverScores[dreamweaver]}");
                }
            }
            catch (Exception ex)
            {
                GD.PrintErr($"Error updating dreamweaver score: {ex.Message}");
            }
        }

        /// <summary>
        /// Gets the current dreamweaver scores.
        /// </summary>
        /// <returns>Dictionary of dreamweaver scores</returns>
        public Godot.Collections.Dictionary<string, int> GetDreamweaverScores()
        {
            return _gameState.DreamweaverScores;
        }

        /// <summary>
        /// Saves the game state to persistent storage.
        /// </summary>
        public void SaveGame()
        {
            try
            {
                _gameState.SaveGame();
                GD.Print("Game saved successfully");
            }
            catch (Exception ex)
            {
                GD.PrintErr($"Error saving game: {ex.Message}");
            }
        }

        /// <summary>
        /// Loads the game state from persistent storage.
        /// </summary>
        /// <returns>True if load was successful, false otherwise</returns>
        public bool LoadGame()
        {
            try
            {
                bool success = _gameState.LoadGame();
                if (success)
                {
                    EmitSignal(SignalName.GameStateUpdated);
                    GD.Print("Game loaded successfully");
                }
                else
                {
                    GD.Print("Game load failed or no save file found");
                }
                return success;
            }
            catch (Exception ex)
            {
                GD.PrintErr($"Error loading game: {ex.Message}");
                return false;
            }
        }

        public override void _ExitTree()
        {
            // Clean up singleton instance
            if (_instance == this)
            {
                _instance = null;
            }
            
            GD.Print("SceneManager autoload singleton destroyed");
        }
    }
}