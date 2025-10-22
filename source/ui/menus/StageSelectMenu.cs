using Godot;
using System;
using System.Collections.Generic;

namespace OmegaSpiral.UI.Menus;

/// <summary>
/// Controller for the Stage Select menu. Replaces plain Button nodes in the
/// scene with the StageButton template, assigns StageId/display name and
/// evaluates content status (static/LLM/missing).
/// </summary>
[GlobalClass]
public partial class StageSelectMenu : Control
{
    private const string StageButtonScenePath = "res://source/ui/menus/stage_button.tscn";
    private const string DefaultStageButtonContainerPath = "TerminalWindow/MenuVBox/StagesPanel";
    private const string DefaultQuitButtonPath = "TerminalWindow/MenuVBox/StagesPanel/QuitButton";

    private VBoxContainer _stagesPanel = new VBoxContainer();
    private PackedScene _stageButtonScene = new PackedScene();

    [Export]
    public NodePath StageButtonContainerPath { get; set; } = new(DefaultStageButtonContainerPath);

    [Export]
    public NodePath QuitButtonPath { get; set; } = new(DefaultQuitButtonPath);

    /// <summary>
    /// Stage descriptor used to populate UI.
    /// </summary>
    private record StageDescriptor(string StageId, string DisplayName, int Index);

    private readonly List<StageDescriptor> StageDescriptors = new()
    {
        new StageDescriptor("Stage1", "Stage 1: Ghost Terminal", 1),
        new StageDescriptor("Stage2", "Stage 2: Echo Hub", 2),
        new StageDescriptor("Stage3", "Stage 3: Echo Vault", 3),
        new StageDescriptor("Stage4", "Stage 4: Town", 4),
        new StageDescriptor("Stage5", "Stage 5: Fractured", 5),
    };

    public override void _Ready()
    {
        base._Ready();

        var containerPath = string.IsNullOrEmpty(StageButtonContainerPath.ToString())
            ? new NodePath(DefaultStageButtonContainerPath)
            : StageButtonContainerPath;

        _stagesPanel = GetNodeOrNull<VBoxContainer>(containerPath);
        if (_stagesPanel == null)
        {
            GD.PrintErr($"StageSelectMenu: Stage button container not found at path '{containerPath}'");
            return;
        }

        _stageButtonScene = GD.Load<PackedScene>(StageButtonScenePath);

        if (_stageButtonScene == null)
        {
            GD.PrintErr("StageSelectMenu: Failed to load StageButton scene: ", StageButtonScenePath);
            return;
        }

        // Replace placeholder buttons with StageButton instances
        ReplaceButtonsWithStageButtons();

        // Wire quit button
        if (!string.IsNullOrEmpty(QuitButtonPath.ToString()))
        {
            var quit = GetNodeOrNull<Button>(QuitButtonPath);
            if (quit != null)
            {
                quit.Pressed += OnQuitPressed;
            }
            else
            {
                GD.PrintErr($"StageSelectMenu: Quit button not found at path '{QuitButtonPath}'");
            }
        }
        else
        {
            var quit = GetNodeOrNull<Button>(DefaultQuitButtonPath);
            quit?.Pressed += OnQuitPressed;
        }
    }

    /// <summary>
    /// Replace child Button nodes in the StagesPanel with instances of StageButton.
    /// This keeps scene editing simple while enabling richer behavior from StageButton.
    /// </summary>
    private void ReplaceButtonsWithStageButtons()
    {
        var existingChildren = new List<Node>(_stagesPanel.GetChildren());

        // Free placeholder buttons
        foreach (var child in existingChildren)
        {
            if (child is Button)
            {
                child.QueueFree();
            }
        }

        // Create StageButtons from descriptors in order
        foreach (var desc in StageDescriptors)
        {
            var inst = _stageButtonScene.Instantiate() as Node;
            if (inst == null)
            {
                GD.PrintErr("StageSelectMenu: Failed to instantiate StageButton");
                continue;
            }

            inst.Name = desc.StageId + "Button";
            _stagesPanel.AddChild(inst);

            var nameLabel = inst.GetNodeOrNull<Label>("HBox/NameLabel");
            var statusLabel = inst.GetNodeOrNull<Label>("HBox/StatusLabel");

            if (nameLabel != null)
            {
                nameLabel.Text = desc.DisplayName;
            }

            // Configure StageId and status on the StageButton script
            if (inst is StageButton sb)
            {
                sb.StageId = desc.StageId;

                // Stage 1 is always static-ready
                if (desc.Index == 1)
                {
                    sb.Status = StageButton.ContentStatus.Ready;
                }
                else
                {
                    // Check for LLM-generated content file
                    bool hasLLM = CheckLLMContentExists(desc);
                    sb.Status = hasLLM ? StageButton.ContentStatus.LLMGenerated : StageButton.ContentStatus.Missing;
                }

                // Connect to the custom signal using the generated SignalName helper (Godot 4 C#)
                sb.Connect(StageButton.SignalName.ClickedStage, new Callable(this, nameof(OnStageClicked)));
            }
            else
            {
                // Fallback: update labels for non-scripted instances
                if (statusLabel != null)
                {
                    statusLabel.Text = "[Pending]";
                }
            }
        }
    }

    /// <summary>
    /// Check whether LLM-generated content exists for the given stage descriptor.
    /// This method uses a conservative, configurable path pattern; adjust as your
    /// content generation pipeline defines output locations.
    /// </summary>
    /// <param name="desc">Descriptor for the stage.</param>
    /// <returns>True when generated content file is present.</returns>
    private bool CheckLLMContentExists(StageDescriptor desc)
    {
        // Default mapping: look for res://source/data/stages/stage_{index}/generated.json
        string folder = $"res://source/data/stages/stage_{desc.Index}";
        string candidate = $"{folder}/generated.json";

        // Use Godot's FileAccess to check existence
        try
        {
            bool exists = Godot.FileAccess.FileExists(candidate);
            if (exists)
            {
                GD.Print($"StageSelectMenu: Found generated content for {desc.StageId} at {candidate}");
                return true;
            }

            // Legacy/alternate filename check
            string alt = $"{folder}/{desc.StageId.ToLower()}_content.json";
            if (Godot.FileAccess.FileExists(alt))
            {
                GD.Print($"StageSelectMenu: Found generated content for {desc.StageId} at {alt}");
                return true;
            }
        }
        catch (Exception ex)
        {
            GD.PrintErr("StageSelectMenu: Error while checking content files: ", ex.Message);
        }

        GD.Print($"StageSelectMenu: No generated content for {desc.StageId}");
        return false;
    }

    /// <summary>
    /// Handler for stage button clicked signal.
    /// </summary>
    /// <param name="stageId">Selected stage identifier.</param>
    private void OnStageClicked(string stageId)
    {
        GD.Print($"StageSelectMenu: Stage clicked {stageId}");

        // Map stage IDs to scene paths
        var stageSceneMap = new Dictionary<string, string>
        {
            { "Stage1", "res://source/stages/ghost/scenes/boot_sequence.tscn" },
            { "Stage2", "res://source/stages/stage_2/echo_hub.tscn" },
            { "Stage3", "res://source/stages/stage_3/echo_vault_hub.tscn" },
            { "Stage4", "res://source/stages/stage_4/stage_4_main.tscn" },
            { "Stage5", "res://source/stages/stage_5/stage5.tscn" }
        };

        if (stageSceneMap.TryGetValue(stageId, out string? scenePath) && scenePath != null)
        {
            var sceneManager = GetNodeOrNull<SceneManager>("/root/SceneManager");
            if (sceneManager != null)
            {
                sceneManager.TransitionToScene(scenePath);
            }
            else
            {
                GD.PrintErr("SceneManager not found!");
            }
        }
        else
        {
            GD.PrintErr($"Unknown stage ID: {stageId}");
        }
    }

    /// <summary>
    /// Quit action handler.
    /// </summary>
    private void OnQuitPressed()
    {
        GD.Print("StageSelectMenu: Quit pressed. Exiting game.");
        GetTree().Quit();
    }
}
