using System.Collections.ObjectModel;
using Godot;
using FileAccess = Godot.FileAccess;
using Dictionary = Godot.Collections.Dictionary;
using Array = Godot.Collections.Array;

namespace OmegaSpiral.Source.Backend;

/// <summary>
/// Represents a single scene in a stage's scene flow.
/// </summary>
public class SceneFlowEntry
{
    /// <summary>Unique identifier for this scene.</summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>Human-readable display name.</summary>
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>Full path to the .tscn file.</summary>
    public string SceneFile { get; set; } = string.Empty;

    /// <summary>C# script class name that extends this scene.</summary>
    public string ScriptClass { get; set; } = string.Empty;

    /// <summary>Description of this scene's purpose.</summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>The ID of the next scene in the flow (null if terminal).</summary>
    public string? NextScene { get; set; }

    /// <summary>Whether this is the last scene in the stage.</summary>
    public bool IsTerminal { get; set; }
}

/// <summary>
/// Represents a complete stage's scene flow configuration.
/// </summary>
public class StageSceneFlow
{
    /// <summary>Human-readable stage name.</summary>
    public string StageName { get; set; } = string.Empty;

    /// <summary>Unique stage identifier (e.g., "stage_1").</summary>
    public string StageId { get; set; } = string.Empty;

    /// <summary>Description of the stage.</summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>All scenes in this stage's flow.</summary>
    private List<SceneFlowEntry> _scenes = new();

    public ReadOnlyCollection<SceneFlowEntry> Scenes => new ReadOnlyCollection<SceneFlowEntry>(_scenes);

    /// <summary>
    /// Adds a scene to this flow.
    /// </summary>
    /// <param name="entry">The scene entry to add.</param>
    internal void AddScene(SceneFlowEntry entry)
    {
        _scenes.Add(entry);
    }
}

/// <summary>
/// Loads and manages scene flow configuration from JSON files.
/// Provides type-safe access to scene sequences, eliminating hardcoded strings in code.
/// </summary>
public class SceneFlowLoader
{
    private StageSceneFlow? _currentFlow;
    private Dictionary<string, SceneFlowEntry> _sceneIndex = new();

    /// <summary>
    /// Loads a stage's scene flow from a JSON file.
    /// </summary>
    /// <param name="flowJsonPath">Path to the scene_flow.json file (e.g., "res://source/data/stages/ghost_terminal/scene_flow.json").</param>
    /// <returns>The loaded scene flow, or null if loading failed.</returns>
    public StageSceneFlow? LoadSceneFlow(string flowJsonPath)
    {
        if (!ResourceLoader.Exists(flowJsonPath))
        {
            GD.PrintErr($"[SceneFlowLoader] Scene flow file not found: {flowJsonPath}");
            return null;
        }

        try
        {
            var fileContent = ResourceLoader.Load<Resource>(flowJsonPath);
            if (fileContent == null)
            {
                GD.PrintErr($"[SceneFlowLoader] Failed to load JSON: {flowJsonPath}");
                return null;
            }

            // Parse JSON manually since Godot's JSON class can be finicky
            var jsonText = FileAccess.GetFileAsString(flowJsonPath);
            var json = new Json();
            if (json.Parse(jsonText) != Error.Ok)
            {
                GD.PrintErr($"[SceneFlowLoader] Invalid JSON in {flowJsonPath}: {json.GetErrorMessage()}");
                return null;
            }

            var flow = ParseSceneFlow(json.Data);
            if (flow != null)
            {
                _currentFlow = flow;
                _sceneIndex = flow.Scenes.ToDictionary(s => s.Id);
                GD.Print($"[SceneFlowLoader] Loaded scene flow for {flow.StageName} ({flow.Scenes.Count} scenes)");
            }

            return flow;
        }
        catch (Exception ex)
        {
            GD.PrintErr($"[SceneFlowLoader] Error loading scene flow: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// Gets the next scene in the flow for a given scene ID.
    /// </summary>
    /// <param name="currentSceneId">The ID of the current scene.</param>
    /// <returns>The next scene entry, or null if this is the last scene.</returns>
    public SceneFlowEntry? GetNextScene(string currentSceneId)
    {
        if (!_sceneIndex.TryGetValue(currentSceneId, out var currentScene))
        {
            GD.PrintErr($"[SceneFlowLoader] Scene not found: {currentSceneId}");
            return null;
        }

        if (currentScene.NextScene == null || currentScene.IsTerminal)
        {
            return null; // No next scene
        }

        if (!_sceneIndex.TryGetValue(currentScene.NextScene, out var nextScene))
        {
            GD.PrintErr($"[SceneFlowLoader] Next scene not found: {currentScene.NextScene}");
            return null;
        }

        return nextScene;
    }

    /// <summary>
    /// Gets a scene by its ID.
    /// </summary>
    /// <param name="sceneId">The unique scene identifier.</param>
    /// <returns>The scene entry, or null if not found.</returns>
    public SceneFlowEntry? GetScene(string sceneId)
    {
        return _sceneIndex.TryGetValue(sceneId, out var scene) ? scene : null;
    }

    /// <summary>
    /// Gets the starting scene of the loaded flow.
    /// </summary>
    /// <returns>The first scene, or null if no flow is loaded.</returns>
    public SceneFlowEntry? GetFirstScene()
    {
        return _currentFlow?.Scenes.FirstOrDefault();
    }

    /// <summary>
    /// Gets all scenes in the loaded flow.
    /// </summary>
    /// <returns>The list of scenes, or empty list if no flow is loaded.</returns>
    public ReadOnlyCollection<SceneFlowEntry> GetAllScenes()
    {
        return _currentFlow?.Scenes ?? new ReadOnlyCollection<SceneFlowEntry>(new List<SceneFlowEntry>());
    }

    /// <summary>
    /// Gets the current loaded stage flow.
    /// </summary>
    public StageSceneFlow? CurrentFlow => _currentFlow;

    /// <summary>
    /// Parses a JSON object into a StageSceneFlow.
    /// </summary>
    private static StageSceneFlow? ParseSceneFlow(Variant jsonData)
    {
        if (jsonData.VariantType != Variant.Type.Dictionary)
        {
            GD.PrintErr("[SceneFlowLoader] Invalid JSON structure: root is not a dictionary");
            return null;
        }

        var dict = (Dictionary)jsonData;

        var flow = new StageSceneFlow
        {
            StageName = dict["stageName"].AsString(),
            StageId = dict["stageId"].AsString(),
            Description = dict["description"].AsString(),
        };

        if (dict.ContainsKey("scenes"))
        {
            var scenesArray = (Array)dict["scenes"];
            foreach (var sceneVariant in scenesArray)
            {
                var sceneDict = (Dictionary)sceneVariant;
                var nextSceneValue = sceneDict.ContainsKey("nextScene") ? sceneDict["nextScene"].AsString() : "";
                string? nextScene = !string.IsNullOrEmpty(nextSceneValue) ? nextSceneValue : null;

                var entry = new SceneFlowEntry
                {
                    Id = sceneDict["id"].AsString(),
                    DisplayName = sceneDict["displayName"].AsString(),
                    SceneFile = sceneDict["sceneFile"].AsString(),
                    ScriptClass = sceneDict["scriptClass"].AsString(),
                    Description = sceneDict["description"].AsString(),
                    NextScene = nextScene,
                    IsTerminal = sceneDict.ContainsKey("isTerminal") && sceneDict["isTerminal"].AsBool(),
                };

                flow.AddScene(entry);
            }
        }

        return flow;
    }
}
