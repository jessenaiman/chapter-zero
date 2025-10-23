using System.Threading.Tasks;
using Godot;

namespace OmegaSpiral.Source.Scripts.Infrastructure;

/// <summary>
/// Manages scene navigation based on scene flow configuration.
/// Decouples scene transitions from hardcoded strings by using SceneFlowLoader.
/// </summary>
[GlobalClass]
public partial class SceneNavigator : Node
{
    private SceneFlowLoader _flowLoader = new();
    private string? _currentSceneId;
    private SceneManager? _sceneManager;

    /// <summary>
    /// Initializes the navigator with a specific stage's scene flow.
    /// </summary>
    /// <param name="stageFlowPath">Path to the stage's scene_flow.json file.</param>
    public void Initialize(string stageFlowPath)
    {
        _sceneManager = GetNode<SceneManager>("/root/SceneManager");

        var flow = _flowLoader.LoadSceneFlow(stageFlowPath);
        if (flow == null)
        {
            GD.PrintErr($"[SceneNavigator] Failed to load scene flow: {stageFlowPath}");
            return;
        }

        GD.Print($"[SceneNavigator] Initialized with {flow.Scenes.Count} scenes from {flow.StageName}");
    }

    /// <summary>
    /// Sets the current scene ID (call this when entering a scene).
    /// </summary>
    /// <param name="sceneId">The ID of the current scene.</param>
    public void SetCurrentScene(string sceneId)
    {
        _currentSceneId = sceneId;
        GD.Print($"[SceneNavigator] Current scene: {sceneId}");
    }

    /// <summary>
    /// Transitions to the next scene in the flow based on the current scene.
    /// </summary>
    /// <returns>True if transition was successful, false otherwise.</returns>
    public bool TransitionToNext()
    {
        if (_currentSceneId == null)
        {
            GD.PrintErr("[SceneNavigator] No current scene set");
            return false;
        }

        var nextScene = _flowLoader.GetNextScene(_currentSceneId);
        if (nextScene == null)
        {
            GD.PrintErr($"[SceneNavigator] No next scene for: {_currentSceneId}");
            return false;
        }

        return TransitionToScene(nextScene.Id);
    }

    /// <summary>
    /// Transitions to a specific scene by ID.
    /// </summary>
    /// <param name="sceneId">The ID of the target scene.</param>
    /// <returns>True if transition was successful, false otherwise.</returns>
    public bool TransitionToScene(string sceneId)
    {
        var targetScene = _flowLoader.GetScene(sceneId);
        if (targetScene == null)
        {
            GD.PrintErr($"[SceneNavigator] Scene not found: {sceneId}");
            return false;
        }

        if (_sceneManager == null)
        {
            GD.PrintErr("[SceneNavigator] SceneManager not found");
            return false;
        }

        GD.Print($"[SceneNavigator] Transitioning to {targetScene.DisplayName} ({sceneId})");
        _sceneManager.TransitionToScene(targetScene.SceneFile, showLoadingScreen: false);
        return true;
    }

    /// <summary>
    /// Gets the current scene's entry from the flow.
    /// </summary>
    public SceneFlowEntry? GetCurrentSceneEntry()
    {
        return _currentSceneId != null ? _flowLoader.GetScene(_currentSceneId) : null;
    }

    /// <summary>
    /// Gets the next scene's entry without transitioning.
    /// </summary>
    public SceneFlowEntry? GetNextSceneEntry()
    {
        return _currentSceneId != null ? _flowLoader.GetNextScene(_currentSceneId) : null;
    }

    /// <summary>
    /// Gets the currently loaded scene flow.
    /// </summary>
    public StageSceneFlow? GetCurrentFlow()
    {
        return _flowLoader.CurrentFlow;
    }
}
