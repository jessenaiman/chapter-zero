// <copyright file="SceneManager.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using Godot;

namespace OmegaSpiral.Source.Scripts;

/// <summary>
/// Manages scene transitions and tracks current scene state across the game.
/// Serves as a singleton autoload for centralized scene management.
/// </summary>
[GlobalClass]
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
        /// Transitions to a new scene by name with loading screen support.
        /// Implements the Maaack Game Template loading pattern.
        /// </summary>
        /// <param name="sceneName">The name of the scene to transition to (e.g., "Scene2NethackSequence").</param>
        /// <param name="showLoadingScreen">Whether to show a loading screen during transition.</param>
        public void TransitionToScene(string sceneName, bool showLoadingScreen = true)
        {
            GD.Print($"Transitioning to scene: {sceneName}");

            // TODO: Implement loading screen logic when showLoadingScreen is true
            if (showLoadingScreen)
            {
                GD.Print("Loading screen requested but not yet implemented");
            }
            string scenePath = sceneName switch
            {
                // Stage 1: Ghost Terminal Opening (CRT Terminal aesthetic)
                "Stage1Opening" => "res://Source/Stages/Stage1/Opening.tscn",
                "Stage1Boot" => "res://Source/Stages/Stage1/boot_sequence.tscn",

                // Legacy aliases (deprecated, use Stage1Opening instead)
                "Scene1Narrative" => "res://Source/Stages/Stage1/Opening.tscn",
                "GhostTerminal" => "res://Source/Stages/Stage1/Opening.tscn",

                // Stage 2-5: To be implemented
                "Stage2Nethack" => "res://Source/Stages/Stage2/Entry.tscn", // TODO: Create this
                "Stage3NeverGoAlone" => "res://Source/Stages/Stage3/Entry.tscn", // TODO: Create this
                "Stage4TileDungeon" => "res://Source/Stages/Stage4/Entry.tscn", // TODO: Create this
                "Stage5FieldCombat" => "res://Source/Stages/Stage5/Entry.tscn", // TODO: Create this

                // Legacy scene paths (may not exist)
                "Scene2NethackSequence" => "res://Source/Scenes/Scene2NethackSequence.tscn",
                "Scene3NeverGoAlone" => "res://Source/Scenes/Scene3NeverGoAlone.tscn",
                "Scene4TileDungeon" => "res://Source/Scenes/Scene4TileDungeon.tscn",
                "Scene5FieldCombat" => "res://Source/Scenes/Scene5FieldCombat.tscn",

                // External/Utility scenes
                "OpenRPGMain" => "res://Source/ExternalScenes/OpenRPGMain.tscn",
                "MainMenu" => "res://addons/maaacks_game_template/base/scenes/menus/main_menu/MainMenu.tscn",
                "CharacterSelection" => "res://Source/Scenes/CharacterSelection.tscn",
                "TestScene" => "res://Source/Scenes/TestScene.tscn",

                _ => string.Empty,
            };

            if (string.IsNullOrEmpty(scenePath))
            {
                GD.PrintErr($"Unknown scene name: {sceneName}");
                return;
            }

            // Use standard Godot scene change functionality
            // Loading screens handled by LoadingScreenController in scene hierarchy
            var error = this.GetTree().ChangeSceneToFile(scenePath);
            if (error != Error.Ok)
            {
                GD.PrintErr($"Failed to change scene to {scenePath}: {error}");
            }
        }
    }
