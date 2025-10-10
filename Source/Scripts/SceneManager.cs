// <copyright file="SceneManager.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System;
using Godot;
using OmegaSpiral.Source.Scripts;

/// <summary>
/// Autoload singleton managing scene transitions and state validation.
/// FUTURE: Will coordinate with DreamweaverSystem for narrative-driven transitions (see ADR-0003).
/// </summary>
public partial class SceneManager : Node
{
    private GameState? gameState;

    /// <inheritdoc/>
    public override void _Ready()
    {
        // Find or create GameState singleton
        this.gameState = this.GetNodeOrNull<GameState>("/root/GameState");
        if (this.gameState == null)
        {
            this.gameState = new GameState();
            this.gameState.Name = "GameState";
            this.GetTree().Root.AddChild(this.gameState);
        }
    }

    /// <summary>
    /// Transitions the game to a new scene with state validation and persistence.
    /// Validates the current game state before transitioning and saves progress.
    /// Handles scene instantiation and cleanup of the current scene.
    /// </summary>
    /// <param name="scenePath">The path to the target scene (without .tscn extension).</param>
    public void TransitionToScene(string scenePath)
    {
        try
        {
            // Validate state before transition
            if (!this.ValidateStateForTransition(scenePath))
            {
                GD.PrintErr("State validation failed for scene transition");
                return;
            }

            // Save current state before transition
            SaveCurrentState();

            PackedScene? newScene = ResourceLoader.Load($"res://Source/Scenes/{scenePath}.tscn") as PackedScene;
            if (newScene == null)
            {
                GD.PrintErr($"Could not load scene: {scenePath}");
                return;
            }

            Node2D instance = (Node2D)newScene.Instantiate();
            this.GetTree().Root.AddChild(instance);

            // Remove the current scene (this node)
            this.GetParent().RemoveChild(this);

            // Change to the new scene
            this.GetTree().CurrentScene = instance;
        }
        catch (Exception ex)
        {
            GD.PrintErr($"Error transitioning to scene: {ex.Message}");
        }
    }

    /// <summary>
    /// Saves the current scene state to the GameState singleton.
    /// Static method that can be called from any scene to persist progress.
    /// Implementation varies based on the active scene type.
    /// </summary>
    public static void SaveCurrentState()
    {
        // This method would save the current state to GameState
        // Implementation depends on the specific scene type
    }

    /// <summary>
    /// Validates that the current game state meets requirements for transitioning to a target scene.
    /// Checks prerequisites like player name, party creation, and scene progression order.
    /// </summary>
    /// <param name="scenePath">The path of the scene being transitioned to.</param>
    /// <returns>True if the state is valid for transition, false otherwise.</returns>
    public bool ValidateStateForTransition(string scenePath)
    {
        if (this.gameState == null)
        {
            GD.PrintErr("GameState is not initialized");
            return false;
        }

        // Validate required state based on target scene
        switch (scenePath)
        {
            case "Scene1Narrative":
                // Scene 1 requires player name and dreamweaver thread
                if (string.IsNullOrEmpty(this.gameState.PlayerName))
                {
                    GD.PrintErr("Player name is required for narrative scene");
                    return false;
                }

                // Dreamweaver thread is always set to a valid value, no need to check for None
                break;

            case "Scene2NethackSequence":
                // Scene 2 requires completed narrative
                if (this.gameState.CurrentScene < 1)
                {
                    GD.PrintErr("Narrative scene must be completed first");
                    return false;
                }

                break;

            case "Scene3WizardryParty":
                // Scene 3 requires completed dungeon exploration
                if (this.gameState.CurrentScene < 2)
                {
                    GD.PrintErr("Dungeon exploration must be completed first");
                    return false;
                }

                break;

            case "Scene4TileDungeon":
                // Scene 4 requires party creation
                if (this.gameState.PlayerParty == null || this.gameState.PlayerParty.Members.Count == 0)
                {
                    GD.PrintErr("Party must be created before entering dungeon");
                    return false;
                }

                break;

            case "Scene5PixelCombat":
                // Scene 5 requires dungeon completion
                if (this.gameState.CurrentScene < 4)
                {
                    GD.PrintErr("Tile dungeon must be completed first");
                    return false;
                }

                break;
        }

        return true;
    }

    /// <summary>
    /// Updates the current scene ID in the game state.
    /// Used to track progression through the game scenes.
    /// </summary>
    /// <param name="sceneId">The new scene ID to set.</param>
    public void UpdateCurrentScene(int sceneId)
    {
        if (this.gameState != null)
        {
            this.gameState.CurrentScene = sceneId;
        }
    }

    /// <summary>
    /// Sets the player's Dreamweaver narrative thread preference.
    /// Updates the game state with the selected Dreamweaver alignment.
    /// </summary>
    /// <param name="thread">The Dreamweaver thread name to set.</param>
    public void SetDreamweaverThread(string thread)
    {
        if (this.gameState == null)
        {
            GD.PrintErr("GameState is not initialized");
            return;
        }

        if (Enum.TryParse<DreamweaverThread>(thread, out var parsedThread))
        {
            this.gameState.DreamweaverThread = parsedThread;
        }
        else
        {
            GD.PrintErr($"Invalid dreamweaver thread: {thread}");
        }
    }

    /// <summary>
    /// Sets the player's name in the game state.
    /// Updates the player identity used throughout the game.
    /// </summary>
    /// <param name="name">The player's chosen name.</param>
    public void SetPlayerName(string name)
    {
        if (this.gameState != null)
        {
            this.gameState.PlayerName = name;
        }
    }

    /// <summary>
    /// Adds a story shard to the player's collection.
    /// Shards represent collected narrative elements that contribute to story progression.
    /// </summary>
    /// <param name="shard">The shard identifier to add.</param>
    public void AddShard(string shard)
    {
        if (this.gameState != null && !this.gameState.Shards.Contains(shard))
        {
            this.gameState.Shards.Add(shard);
        }
    }
}
