using Godot;
using System;

public partial class SceneManager : Node
{
    private GameState _gameState;
    
    public override void _Ready()
    {
        // Find or create GameState singleton
        _gameState = GetNodeOrNull<GameState>("/root/GameState");
        if (_gameState == null)
        {
            _gameState = new GameState();
            _gameState.Name = "GameState";
            GetTree().Root.AddChild(_gameState);
        }
    }
    
    public async void TransitionToScene(string scenePath)
    {
        try
        {
            // Validate state before transition
            if (!ValidateStateForTransition(scenePath))
            {
                GD.PrintErr("State validation failed for scene transition");
                return;
            }
            
            // Save current state before transition
            SaveCurrentState();
            
            PackedScene newScene = ResourceLoader.Load($"res://Source/Scenes/{scenePath}.tscn") as PackedScene;
            if (newScene == null)
            {
                GD.PrintErr($"Could not load scene: {scenePath}");
                return;
            }
            
            Node2D instance = (Node2D)newScene.Instantiate();
            GetTree().Root.AddChild(instance);
            
            // Remove the current scene (this node)
            GetParent().RemoveChild(this);
            
            // Change to the new scene
            GetTree().CurrentScene = instance;
        }
        catch (Exception ex)
        {
            GD.PrintErr($"Error transitioning to scene: {ex.Message}");
        }
    }
    
    public void SaveCurrentState()
    {
        // This method would save the current state to GameState
        // Implementation depends on the specific scene type
    }
    
    public bool ValidateStateForTransition(string scenePath)
    {
        // Validate required state based on target scene
        switch (scenePath)
        {
            case "Scene1Narrative":
                // Scene 1 requires player name and dreamweaver thread
                if (string.IsNullOrEmpty(_gameState.PlayerName))
                {
                    GD.PrintErr("Player name is required for narrative scene");
                    return false;
                }
                if (_gameState.DreamweaverThread == DreamweaverThread.None)
                {
                    GD.PrintErr("Dreamweaver thread must be selected for narrative scene");
                    return false;
                }
                break;
                
            case "Scene2NethackSequence":
                // Scene 2 requires completed narrative
                if (_gameState.CurrentScene < 1)
                {
                    GD.PrintErr("Narrative scene must be completed first");
                    return false;
                }
                break;
                
            case "Scene3WizardryParty":
                // Scene 3 requires completed dungeon exploration
                if (_gameState.CurrentScene < 2)
                {
                    GD.PrintErr("Dungeon exploration must be completed first");
                    return false;
                }
                break;
                
            case "Scene4TileDungeon":
                // Scene 4 requires party creation
                if (_gameState.PlayerParty == null || _gameState.PlayerParty.Members.Count == 0)
                {
                    GD.PrintErr("Party must be created before entering dungeon");
                    return false;
                }
                break;
                
            case "Scene5PixelCombat":
                // Scene 5 requires dungeon completion
                if (_gameState.CurrentScene < 4)
                {
                    GD.PrintErr("Tile dungeon must be completed first");
                    return false;
                }
                break;
        }
        
        return true;
    }
    
    public void UpdateCurrentScene(int sceneId)
    {
        _gameState.CurrentScene = sceneId;
    }
    
    public void SetDreamweaverThread(string thread)
    {
        _gameState.DreamweaverThread = thread;
    }
    
    public void SetPlayerName(string name)
    {
        _gameState.PlayerName = name;
    }
    
    public void AddShard(string shard)
    {
        if (!_gameState.Shards.Contains(shard))
        {
            _gameState.Shards.Add(shard);
        }
    }
}