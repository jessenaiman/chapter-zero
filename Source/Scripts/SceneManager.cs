// <copyright file="SceneManager.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

namespace OmegaSpiral.Source.Scripts
{
    using Godot;

    /// <summary>
    /// Manages scene transitions and tracks current scene state across the game.
    /// Serves as a singleton autoload for centralized scene management.
    /// </summary>
    public partial class SceneManager : Node
    {
        private int currentSceneIndex = 1;
        private string? playerName;
        private string? dreamweaverThread;

        /// <summary>
        /// Gets or sets the current scene index (1-5 for the five main scenes).
        /// </summary>
        public int CurrentSceneIndex
        {
            get => this.currentSceneIndex;
            set => this.currentSceneIndex = value;
        }

        /// <summary>
        /// Gets the player's chosen name.
        /// </summary>
        public string? PlayerName => this.playerName;

        /// <summary>
        /// Gets the selected Dreamweaver thread identifier.
        /// </summary>
        public string? DreamweaverThread => this.dreamweaverThread;

        /// <summary>
        /// Sets the player's name for use throughout the game.
        /// </summary>
        /// <param name="name">The player's chosen name.</param>
        public void SetPlayerName(string name)
        {
            this.playerName = name;
            GD.Print($"Player name set to: {name}");
        }

        /// <summary>
        /// Sets the Dreamweaver thread identifier based on player choice.
        /// </summary>
        /// <param name="threadId">The Dreamweaver thread identifier (e.g., "hero", "shadow", "ambition").</param>
        public void SetDreamweaverThread(string threadId)
        {
            this.dreamweaverThread = threadId;
            GD.Print($"Dreamweaver thread set to: {threadId}");
        }

        /// <summary>
        /// Updates the current scene index for tracking progression.
        /// </summary>
        /// <param name="sceneIndex">The scene index (1-5).</param>
        public void UpdateCurrentScene(int sceneIndex)
        {
            this.currentSceneIndex = sceneIndex;
            GD.Print($"Current scene updated to: {sceneIndex}");
        }

        /// <summary>
        /// Transitions to a new scene by name.
        /// </summary>
        /// <param name="sceneName">The name of the scene to transition to (e.g., "Scene2NethackSequence").</param>
        public void TransitionToScene(string sceneName)
        {
            GD.Print($"Transitioning to scene: {sceneName}");

            // Map scene names to their file paths
            string scenePath = sceneName switch
            {
                "Scene1Narrative" => "res://Source/Scenes/Scene1Narrative.tscn",
                "Scene2NethackSequence" => "res://Source/Scenes/Scene2NethackSequence.tscn",
                "Scene3NeverGoAlone" => "res://Source/Scenes/Scene3NeverGoAlone.tscn",
                "Scene4TileDungeon" => "res://Source/Scenes/Scene4TileDungeon.tscn",
                "Scene5FieldCombat" => "res://Source/Scenes/Scene5FieldCombat.tscn",
                _ => string.Empty,
            };

            if (string.IsNullOrEmpty(scenePath))
            {
                GD.PrintErr($"Unknown scene name: {sceneName}");
                return;
            }

            // Use Godot's scene change functionality
            var error = this.GetTree().ChangeSceneToFile(scenePath);
            if (error != Error.Ok)
            {
                GD.PrintErr($"Failed to change scene to {scenePath}: {error}");
            }
        }
    }
}
