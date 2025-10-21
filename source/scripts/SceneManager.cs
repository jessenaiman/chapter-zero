using Godot;

/// <summary>
/// Simple scene manager for basic scene transitions.
/// </summary>


[GlobalClass]
public partial class SceneManager : Node
{
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
        this.CurrentDreamweaverThread = threadId;
    }

    /// <summary>
    /// Gets the current Dreamweaver thread identifier.
    /// </summary>
    public string? CurrentDreamweaverThread { get; private set; }

    /// <summary>
    /// Transitions to the specified scene.
    /// </summary>
    /// <param name="sceneName">Name of the scene to transition to.</param>
    /// <param name="showLoadingScreen">Whether to show a loading screen.</param>
    public void TransitionToScene(string sceneName, bool showLoadingScreen = true)
    {
        GD.Print($"Transitioning to scene: {sceneName}");
        // TODO: Implement actual scene transition logic
    }

    /// <summary>
    /// Updates the current scene index.
    /// </summary>
    /// <param name="sceneIndex">The scene index to set.</param>
    public void UpdateCurrentScene(int sceneIndex)
    {
        GD.Print($"Updated current scene to: {sceneIndex}");
        // TODO: Implement scene tracking logic
    }
}
