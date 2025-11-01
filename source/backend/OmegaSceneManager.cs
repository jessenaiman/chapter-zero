using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;
using Newtonsoft.Json;
using OmegaSpiral.Source.Backend.Narrative;
using OmegaSpiral.Source.Backend;

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
        using var file = Godot.FileAccess.Open(jsonPath, Godot.FileAccess.ModeFlags.Read);
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

        // Handle different scene types
        if (scene.Type == "combat" && scene.CombatData != null)
        {
            // For combat scenes, emit a combat started signal
            // In a full implementation, this would transition to combat UI
            GD.Print($"Combat encounter: {scene.CombatData.Title}");

            // Award points for combat victory based on encounter owner
            if (!string.IsNullOrEmpty(scene.CombatData.Owner))
            {
                AwardCombatVictoryPoints(scene.CombatData.Owner);
            }

            // For now, just show victory text and continue
            if (!string.IsNullOrEmpty(scene.CombatData.VictoryText))
            {
                // Could emit a different signal for combat results
                GD.Print($"Combat result: {scene.CombatData.VictoryText}");
            }
        }
        else if (!string.IsNullOrEmpty(scene.Question) && scene.Choice != null && scene.Choice.Count > 0)
        {
            // Handle narrative choices
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

    /// <summary>
    /// Awards Dreamweaver points for combat victory.
    /// </summary>
    /// <param name="owner">The Dreamweaver owner of the combat encounter.</param>
    private void AwardCombatVictoryPoints(string owner)
    {
        // Get the OmegaGameManager autoload
        var gameManager = GetNode("/root/OmegaGameManager");
        if (gameManager == null)
        {
            GD.PrintErr("Could not find OmegaGameManager node");
            return;
        }

        // Get current scores from GDScript GameManager
        var currentScores = (Godot.Collections.Array)gameManager.Call("get_dreamweaver_scores");
        if (currentScores == null || currentScores.Count < 3)
        {
            GD.PrintErr("Could not get current dreamweaver scores");
            return;
        }

        // Award 2 points to the owner Dreamweaver (matching choice scoring)
        int lightPoints = (int)currentScores[0];
        int shadowPoints = (int)currentScores[1];
        int ambitionPoints = (int)currentScores[2];

        switch (owner.ToLower())
        {
            case "light":
                gameManager.Call("update_dreamweaver_score", 0, 2); // Index 0 = Light
                lightPoints += 2;
                break;
            case "shadow":
            case "mischief":
                gameManager.Call("update_dreamweaver_score", 1, 2); // Index 1 = Shadow/Mischief
                shadowPoints += 2;
                break;
            case "ambition":
            case "wrath":
                gameManager.Call("update_dreamweaver_score", 2, 2); // Index 2 = Ambition/Wrath
                ambitionPoints += 2;
                break;
            default:
                GD.Print($"Unknown combat owner: {owner}");
                return;
        }

        GD.Print($"Combat victory: Awarded 2 points to {owner} | Scores: Light:{lightPoints} Shadow:{shadowPoints} Ambition:{ambitionPoints}");
    }
}
