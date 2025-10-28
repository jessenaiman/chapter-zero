using Godot;
using System;

/// <summary>
/// Scene manager for Omega Spiral game.
/// Handles scene transitions WITH validation and state management.
/// Tracks player state (PlayerName, DreamweaverThread) across scene transitions.
/// </summary>
[GlobalClass]
public partial class StageManager : Node
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
