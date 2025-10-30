using Godot;
using System;

/// <summary>
/// Scene manager for Omega Spiral game.
/// Handles scene transitions WITH validation and state management.
/// Tracks player state (PlayerName, DreamweaverThread) across scene transitions.
/// </summary>
[GlobalClass]
public partial class SceneManager : Node
{
    private int _CurrentSceneIndex = 1;

    /// <summary>
    /// Sets the current player name for the session.
    /// </summary>
    /// <param name="playerName">The name of the player.</param>
    public void SetPlayerName(string playerName)
    {
        GD.Print($"Player name set to: {playerName}");
        this.PlayerName = playerName;
    }

    /// <summary>
    /// Gets the current player name.
    /// </summary>
    public string? PlayerName { get; private set; }

    /// <summary>
    /// Sets the current Dreamweaver thread for narrative progression.
    /// </summary>
    /// <param name="threadId">The identifier of the Dreamweaver thread.</param>
    public void SetDreamweaverThread(string threadId)
    {
        GD.Print($"Dreamweaver thread set to: {threadId}");
        this.DreamweaverThread = threadId;
    }

    /// <summary>
    /// Gets the current Dreamweaver thread identifier.
    /// </summary>
    public string? DreamweaverThread { get; private set; }

    /// <summary>
    /// Gets or sets the current scene index.
    /// </summary>
    public int CurrentSceneIndex
    {
        get => _CurrentSceneIndex;
        private set => _CurrentSceneIndex = value;
    }

    /// <summary>
    /// Transitions to the specified scene.
    /// </summary>
    /// <param name="scenePath">Path to the scene to transition to.</param>
    /// <param name="showLoadingScreen">Whether to show a loading screen.</param>
    public void TransitionToScene(string scenePath, bool showLoadingScreen = true)
    {
        GD.Print($"Transitioning to scene: {scenePath}");

        // Validate the scene path exists
        if (!ResourceLoader.Exists(scenePath))
        {
            GD.PrintErr($"Scene does not exist: {scenePath}");
            return;
        }

        // Load the scene
        var packedScene = ResourceLoader.Load<PackedScene>(scenePath);
        if (packedScene == null)
        {
            GD.PrintErr($"Failed to load scene: {scenePath}");
            return;
        }

        // Perform the scene transition
        GetTree().ChangeSceneToPacked(packedScene);
    }

    /// <summary>
    /// Loads a stage scene as a child of the specified parent node.
    /// Used by GameManager for stage progression.
    /// </summary>
    /// <typeparam name="T">The expected type of the instantiated scene root.</typeparam>
    /// <param name="stageScene">The packed scene to instantiate.</param>
    /// <param name="parent">The parent node to add the scene to.</param>
    /// <returns>The instantiated scene root, or null if instantiation failed.</returns>
    public T? LoadStageAsChild<T>(PackedScene stageScene, Node parent) where T : Node
    {
        if (stageScene == null)
        {
            GD.PrintErr("[SceneManager] Stage scene is null!");
            return null;
        }

        GD.Print($"[SceneManager] Loading stage as child of {parent.Name}...");
        var stageInstance = stageScene.Instantiate<T>();

        if (stageInstance == null)
        {
            GD.PrintErr("[SceneManager] Failed to instantiate stage scene.");
            return null;
        }

        parent.AddChild(stageInstance);
        GD.Print($"[SceneManager] Stage loaded and added to {parent.Name}");

        return stageInstance;
    }

    /// <summary>
    /// Unloads a stage by removing it from its parent and freeing it.
    /// Used by GameManager when transitioning between stages.
    /// </summary>
    /// <param name="stage">The stage node to unload.</param>
    public void UnloadStage(Node stage)
    {
        if (stage == null || !IsInstanceValid(stage))
        {
            GD.Print("[SceneManager] Stage is null or invalid, skipping unload.");
            return;
        }

        var parent = stage.GetParent();
        if (parent != null)
        {
            GD.Print($"[SceneManager] Removing stage {stage.Name} from {parent.Name}");
            parent.RemoveChild(stage);
        }

        stage.QueueFree();
        GD.Print("[SceneManager] Stage unloaded and freed");
    }

    /// <summary>
    /// Updates the current scene index.
    /// </summary>
    /// <param name="sceneIndex">The scene index to set.</param>
    public void UpdateCurrentScene(int sceneIndex)
    {
        GD.Print($"Updated current scene to: {sceneIndex}");
        this.CurrentSceneIndex = sceneIndex;
    }

    /// <summary>
    /// Validates if a transition to the specified scene is allowed based on game state.
    /// </summary>
    /// <param name="scenePath">The path to the target scene.</param>
    /// <returns>True if the transition is valid, false otherwise.</returns>
    public bool ValidateStateForTransition(string scenePath)
    {
        // For now, allow all transitions. This can be expanded with more complex validation
        GD.Print($"Validating transition to: {scenePath}");
        return true;
    }
}
