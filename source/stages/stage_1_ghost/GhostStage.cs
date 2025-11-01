using Godot;
using OmegaSpiral.Source.Backend;
using OmegaSpiral.Source.Backend.Narrative;
using OmegaSpiral.Source.Stages.Stage1;

/// <summary>
/// Simple stage controller for Ghost Terminal.
/// Loads the story and manages scene progression.
/// </summary>
public partial class GhostStage : Node
{
    private OmegaSceneManager? _sceneManager;
    private GhostUi? _ui;

    public override void _Ready()
    {
        // Get references to scene manager and UI
        _sceneManager = GetNode<OmegaSceneManager>("OmegaSceneManager");
        _ui = GetNode<GhostUi>("GhostUi");

        if (_sceneManager == null || _ui == null)
        {
            GD.PrintErr("Missing required nodes: OmegaSceneManager or GhostUi");
            return;
        }

        // Connect signals
        _sceneManager.SceneStarted += OnSceneStarted;
        _sceneManager.ChoicesPresented += OnChoicesPresented;
        _sceneManager.StoryComplete += OnStoryComplete;

        // Load and start the story
        _sceneManager.LoadStory("res://source/stages/stage_1_ghost/ghost.json");
        _sceneManager.StartStory();
    }

    private void OnSceneStarted(string sceneId, string[] lines)
    {
        GD.Print($"Scene started: {sceneId}");

        // Display scene lines
        foreach (var line in lines)
        {
            GD.Print(line);
            // TODO: Send to UI for display
        }
    }

    private void OnChoicesPresented(string question, string[] choiceTexts)
    {
        GD.Print($"Question: {question}");
        foreach (var choiceText in choiceTexts)
        {
            GD.Print($"  - {choiceText}");
        }

        // TODO: Present choices to UI
        // For now, just auto-select first choice after a delay
        GetTree().CreateTimer(2.0f).Timeout += () =>
        {
            if (choiceTexts.Length > 0)
            {
                _sceneManager?.MakeChoice(choiceTexts[0]);
            }
        };
    }

    private void OnStoryComplete()
    {
        GD.Print("Story complete!");
        // TODO: Handle story completion
    }
}
