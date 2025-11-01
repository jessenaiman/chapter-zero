using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;
using Newtonsoft.Json;
using OmegaSpiral.Source.Backend.Narrative;

/// <summary>
/// Simple scene manager for Omega Spiral stages.
/// Loads JSON story files and manages scene progression.
/// Emits signals for UI to display content and handle choices.
/// </summary>
public partial class OmegaSceneManager : Node
{
    /// <summary>
    /// Emitted when a new scene should be displayed.
    /// </summary>
    [Signal]
    public delegate void SceneStartedEventHandler(string sceneId, string[] lines);

    /// <summary>
    /// Emitted when choices should be presented to the player.
    /// </summary>
    [Signal]
    public delegate void ChoicesPresentedEventHandler(string question, string[] choiceTexts);

    /// <summary>
    /// Emitted when the story is complete.
    /// </summary>
    [Signal]
    public delegate void StoryCompleteEventHandler();

    private StoryBlock? _story;
    private int _currentSceneIndex = -1;

    /// <summary>
    /// Loads a story from JSON file.
    /// </summary>
    /// <param name="jsonPath">Path to the JSON file.</param>
    public void LoadStory(string jsonPath)
    {
        using var file = FileAccess.Open(jsonPath, FileAccess.ModeFlags.Read);
        if (file == null)
        {
            GD.PrintErr($"Failed to load story file: {jsonPath}");
            return;
        }

        var jsonText = file.GetAsText();
        _story = JsonConvert.DeserializeObject<StoryBlock>(jsonText);

        if (_story?.Scenes == null || _story.Scenes.Count == 0)
        {
            GD.PrintErr("Story file contains no scenes");
            return;
        }

        GD.Print($"Loaded story: {_story.Title} with {_story.Scenes.Count} scenes");
        _currentSceneIndex = -1;
    }

    /// <summary>
    /// Advances to the next scene.
    /// </summary>
    public void NextScene()
    {
        if (_story?.Scenes == null)
        {
            GD.PrintErr("No story loaded");
            return;
        }

        _currentSceneIndex++;

        if (_currentSceneIndex >= _story.Scenes.Count)
        {
            EmitSignal(SignalName.StoryComplete);
            return;
        }

        var scene = _story.Scenes[_currentSceneIndex];
        var lines = scene.Lines?.ToArray() ?? System.Array.Empty<string>();
        EmitSignal(SignalName.SceneStarted, scene.Id ?? "", lines);

        // If scene has choices, present them
        if (!string.IsNullOrEmpty(scene.Question) && scene.Choice != null && scene.Choice.Count > 0)
        {
            var choiceTexts = scene.Choice.Select(c => c.Text ?? "").ToArray();
            EmitSignal(SignalName.ChoicesPresented, scene.Question ?? "", choiceTexts);
        }
    }

    /// <summary>
    /// Makes a choice and advances to next scene.
    /// </summary>
    /// <param name="choiceText">The selected choice text.</param>
    public void MakeChoice(string choiceText)
    {
        GD.Print($"Choice made: {choiceText}");
        // For now, just advance to next scene
        // Later we can add choice-based branching logic
        NextScene();
    }

    /// <summary>
    /// Starts the story from the beginning.
    /// </summary>
    public void StartStory()
    {
        _currentSceneIndex = -1;
        NextScene();
    }

    /// <summary>
    /// Gets the current scene, or null if no story is loaded.
    /// </summary>
    public Scene? GetCurrentScene()
    {
        if (_story?.Scenes == null || _currentSceneIndex < 0 || _currentSceneIndex >= _story.Scenes.Count)
        {
            return null;
        }

        return _story.Scenes[_currentSceneIndex];
    }
}
