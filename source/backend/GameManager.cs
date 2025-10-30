// <copyright file="GameManager.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using Godot;
using System.Threading.Tasks;
using OmegaSpiral.Source.Audio;
using OmegaSpiral.Source.Frontend;
using OmegaSpiral.Source.Infrastructure;
using OmegaSpiral.Source.InputSystem;

namespace OmegaSpiral.Source.Services;

/// <summary>
/// Global game orchestrator. Manages STAGE progression (Stage 1 → Stage 2 → Stage 3 → etc.).
/// NOT responsible for scene transitions within a stage - use SceneManager for that.
/// This should be configured as an autoload singleton in project.godot.
/// </summary>
[GlobalClass]
public partial class GameManager : Node
{
    /// <summary>
    /// Array of stage scenes to load in sequence. Assign these in the Godot editor.
    /// Each stage should have a root node that inherits from <see cref="StageBase"/>.
    /// </summary>
    [Export]
    public PackedScene[] StageScenes { get; set; } = System.Array.Empty<PackedScene>();

    private int _CurrentStageIndex = -1;
    private IStageBase? _CurrentStage;
    private StageBase? _CurrentStageNode;
    private AudioManager? _AudioManager;
    private OmegaInputRouter? _InputRouter;
    private JournalSystem _Journal = new();

    /// <summary>
    /// Gets the journal system that tracks the playthrough transcript.
    /// </summary>
    public JournalSystem Journal => _Journal;

    /// <summary>
    /// Gets the current stage index (0-based). Returns -1 if no stage is active.
    /// </summary>
    public int CurrentStageIndex => _CurrentStageIndex;

    /// <summary>
    /// Called when the node enters the scene tree.
    /// </summary>
    public override void _Ready()
    {
        base._Ready();
        GD.Print("[GameManager] Ready. Configured with ", StageScenes.Length, " stages.");
        EnsureInputRouter();
        ResolveAudioManager();
    }

    /// <summary>
    /// Starts the game by loading the first stage.
    /// </summary>
    /// <returns>A <see cref="Task"/> that completes when the game starts.</returns>
    public async Task StartGameAsync()
    {
        GD.Print("[GameManager] Starting game...");
        _CurrentStageIndex = -1;
        await AdvanceToNextStageAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// Starts the game at a specific stage index.
    /// </summary>
    /// <param name="stageIndex">The 0-based index of the stage to start at.</param>
    /// <returns>A <see cref="Task"/> that completes when the stage is loaded and started.</returns>
    public async Task StartGameAtStageAsync(int stageIndex)
    {
        if (stageIndex < 0 || stageIndex >= StageScenes.Length)
        {
            GD.PrintErr($"[GameManager] Invalid stage index: {stageIndex}. Must be between 0 and {StageScenes.Length - 1}.");
            return;
        }

        GD.Print($"[GameManager] Starting game at stage {stageIndex + 1}...");
        _CurrentStageIndex = stageIndex - 1; // Will be incremented in AdvanceToNextStageAsync
        await AdvanceToNextStageAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// Advances to the next stage in the sequence. If no more stages exist, the game ends.
    /// </summary>
    /// <returns>A <see cref="Task"/> that completes when the next stage is loaded and started.</returns>
    public async Task AdvanceToNextStageAsync()
    {
        // Clean up current stage if it exists
        if (_CurrentStageNode != null)
        {
            GD.Print("[GameManager] Removing current stage: ", _CurrentStageNode.Name);
            RemoveChild(_CurrentStageNode);
            _CurrentStageNode.QueueFree();
            _CurrentStageNode = null;
        }

        _CurrentStage = null;

        // Move to next stage
        _CurrentStageIndex++;

        // Check if we've reached the end
        if (_CurrentStageIndex >= StageScenes.Length)
        {
            GD.Print("[GameManager] All stages complete. Game finished!");

            // Write the playthrough transcript before signaling completion
            _Journal.WritePlaythroughToFile();

            EmitSignal(SignalName.GameComplete);
            return;
        }

        // Load and start the next stage
        var stageScene = StageScenes[_CurrentStageIndex];
        if (stageScene == null)
        {
            GD.PrintErr($"[GameManager] Stage scene at index {_CurrentStageIndex} is null!");
            return;
        }

        GD.Print($"[GameManager] Loading stage {_CurrentStageIndex + 1}/{StageScenes.Length}...");
        var stageInstance = stageScene.Instantiate<Node>();

        if (stageInstance == null || stageInstance is not IStageBase stageBase)
        {
            GD.PrintErr($"[GameManager] Failed to instantiate stage at index {_CurrentStageIndex}. Scene must have a StageBase script implementing IStageBase.");
            return;
        }

        _CurrentStage = stageBase;
        _CurrentStageNode = stageInstance as StageBase;

        if (_CurrentStageNode != null)
        {
            AddChild(_CurrentStageNode);
        }

        // Connect to the stage's completion event
        _CurrentStage.StageComplete += OnStageComplete;

        // Execute the stage
        GD.Print($"[GameManager] Executing stage {_CurrentStage.StageId}...");
        await _CurrentStage.ExecuteStageAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// Called when the current stage emits its completion event.
    /// </summary>
    private async void OnStageComplete()
    {
        GD.Print("[GameManager] Stage complete event received.");

        if (_CurrentStage != null)
        {
            _CurrentStage.StageComplete -= OnStageComplete;
        }

        await AdvanceToNextStageAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// Emitted when all stages have been completed.
    /// </summary>
    [Signal]
    public delegate void GameCompleteEventHandler();

    private void EnsureInputRouter()
    {
        var tree = GetTree();
        if (tree?.Root == null)
        {
            return;
        }

        _InputRouter = tree.Root.GetNodeOrNull<OmegaInputRouter>(OmegaInputRouter.DefaultNodeName);
        if (_InputRouter == null)
        {
            _InputRouter = new OmegaInputRouter();
            // Use CallDeferred to add child after scene tree finishes initializing
            // Direct AddChild() fails when parent is busy setting up children
            tree.Root.CallDeferred(Node.MethodName.AddChild, _InputRouter);
        }
    }

    private AudioManager? ResolveAudioManager()
    {
        if (_AudioManager != null && IsInstanceValid(_AudioManager))
        {
            return _AudioManager;
        }

        var tree = GetTree();
        if (tree?.Root == null)
        {
            return null;
        }

        _AudioManager = tree.Root.GetNodeOrNull<AudioManager>("AudioManager");
        return _AudioManager;
    }
}
