// <copyright file="GhostTerminalDirector.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;

namespace OmegaSpiral.Source.Scripts.Field.Narrative;
/// <summary>
/// Orchestrates the playback of narrative sequences in the Ghost Terminal stage.
/// The director maintains a queue of sequences, plays them in order, and handles transitions
/// between sequences based on user input and narrative logic.
///
/// The director is responsible for:
/// - Loading and instantiating sequence scenes
/// - Managing sequence playback order
/// - Routing input from sequences to determine the next sequence
/// - Emitting completion signal when all sequences are finished
/// - Cleanup and resource management
///
/// FUTURE: LLM_INTEGRATION - The director can inject DreamweaverSystem references into
/// sequences to enable dynamic narrative generation.
/// </summary>
[GlobalClass]
public partial class GhostTerminalDirector : Node
{
    /// <summary>
    /// Emitted when all sequences have been played and the narrative is complete.
    /// </summary>
    [Signal]
    public delegate void NarrativeCompleteEventHandler();

    /// <summary>
    /// Emitted when transitioning between sequences.
    /// Useful for UI updates, logging, and debugging.
    /// </summary>
    /// <param name="fromSequence">The ID of the sequence being transitioned from.</param>
    /// <param name="toSequence">The ID of the sequence being transitioned to.</param>
    [Signal]
    public delegate void SequenceTransitionEventHandler(string fromSequence, string toSequence);

    /// <summary>
    /// Mapping of sequence IDs to scene paths.
    /// This allows the director to dynamically instantiate sequences as needed.
    /// </summary>
    private readonly Dictionary<string, string> sequenceSceneMap = new()
    {
        // Phase 2: Opening sequence
        { "opening", "res://Source/Scenes/field/narrative/sequences/Opening.tscn" },

        // Phase 3: Thread branch sequences
        { "thread_hero", "res://Source/Scenes/field/narrative/sequences/ThreadBranch_Hero.tscn" },
        { "thread_shadow", "res://Source/Scenes/field/narrative/sequences/ThreadBranch_Shadow.tscn" },
        { "thread_ambition", "res://Source/Scenes/field/narrative/sequences/ThreadBranch_Ambition.tscn" },

        // Phase 4: Input refinement sequences
        { "name_input", "res://Source/Scenes/field/narrative/sequences/NameInput.tscn" },
        { "secret_question", "res://Source/Scenes/field/narrative/sequences/SecretQuestion.tscn" },
    };

    /// <summary>
    /// Queue of sequences to play in order.
    /// Sequences are identified by their ID strings.
    /// </summary>
    private Queue<string> sequenceQueue = new();

    /// <summary>
    /// The currently playing sequence (if any).
    /// </summary>
    private NarrativeSequence? currentSequence;

    /// <summary>
    /// Container for sequence instances.
    /// All instantiated sequences are added as children to this node.
    /// </summary>
    private Node? sequenceContainer;

    /// <summary>
    /// Gets or sets a value indicating whether the director is currently playing sequences.
    /// </summary>
    public bool IsPlaying { get; private set; }

    /// <summary>
    /// Gets the ID of the currently playing sequence (or empty string if none).
    /// </summary>
    public string CurrentSequenceId => this.currentSequence?.SequenceId ?? string.Empty;

    /// <inheritdoc/>
    public override void _Ready()
    {
        // Create a container for sequence instances
        this.sequenceContainer = new Node { Name = "SequenceContainer" };
        this.AddChild(this.sequenceContainer);
    }

    /// <summary>
    /// Initializes the director with a starting sequence and optional subsequent sequence queue.
    /// This must be called before PlayAsync().
    /// </summary>
    /// <param name="startingSequenceId">The ID of the first sequence to play.</param>
    /// <param name="subsequentSequences">Optional list of subsequent sequence IDs to queue after the starting sequence.</param>
    public void Initialize(string startingSequenceId, IEnumerable<string>? subsequentSequences = null)
    {
        this.sequenceQueue.Clear();
        this.sequenceQueue.Enqueue(startingSequenceId);

        if (subsequentSequences != null)
        {
            foreach (string sequenceId in subsequentSequences)
            {
                this.sequenceQueue.Enqueue(sequenceId);
            }
        }
    }

    /// <summary>
    /// Registers a sequence scene with the director.
    /// Must be called before PlayAsync() for sequences that need to be instantiated.
    /// </summary>
    /// <param name="sequenceId">The unique identifier for the sequence.</param>
    /// <param name="scenePath">The scene file path (e.g., "res://Source/Scenes/GhostTerminal/Opening.tscn").</param>
    public void RegisterSequenceScene(string sequenceId, string scenePath)
    {
        this.sequenceSceneMap[sequenceId] = scenePath;
    }

    /// <summary>
    /// Plays the queued sequences in order until the queue is empty.
    /// This is the main entry point for the director.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public async Task PlayAsync()
    {
        this.IsPlaying = true;

        try
        {
            while (this.sequenceQueue.Count > 0)
            {
                string nextSequenceId = this.sequenceQueue.Dequeue();
                await this.PlaySequenceAsync(nextSequenceId).ConfigureAwait(false);
            }

            this.EmitSignal(SignalName.NarrativeComplete);
        }
        finally
        {
            this.IsPlaying = false;
            this.Cleanup();
        }
    }

    /// <summary>
    /// Plays a single sequence by its ID.
    /// If the sequence has a registered scene path, it will be instantiated.
    /// Otherwise, the director looks for an existing child node with that ID.
    /// </summary>
    /// <param name="sequenceId">The ID of the sequence to play.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    private async Task PlaySequenceAsync(string sequenceId)
    {
        // Emit transition signal (from previous sequence to new sequence)
        string previousSequenceId = this.currentSequence?.SequenceId ?? "none";
        this.EmitSignal(SignalName.SequenceTransition, previousSequenceId, sequenceId);

        // Unload the previous sequence
        this.UnloadSequence();

        // Load and instantiate the new sequence
        this.currentSequence = this.LoadSequence(sequenceId);
        if (this.currentSequence == null)
        {
            GD.PrintErr($"GhostTerminalDirector: Failed to load sequence '{sequenceId}'");
            return;
        }

        // Connect the sequence's completion signal
        this.currentSequence.SequenceComplete += this.OnSequenceComplete;
        this.currentSequence.SequenceInput += this.OnSequenceInput;

        // Play the sequence
        await this.currentSequence.PlayAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// Loads a sequence by its ID.
    /// If a scene is registered for the sequence, it will be instantiated.
    /// Otherwise, looks for an existing child node with that ID.
    /// </summary>
    /// <param name="sequenceId">The ID of the sequence to load.</param>
    /// <returns>The loaded NarrativeSequence, or null if not found.</returns>
    private NarrativeSequence? LoadSequence(string sequenceId)
    {
        // Check if a scene is registered for this sequence
        if (this.sequenceSceneMap.TryGetValue(sequenceId, out string? scenePath))
        {
            try
            {
                var scene = GD.Load<PackedScene>(scenePath);
                if (scene == null)
                {
                    GD.PrintErr($"GhostTerminalDirector: Failed to load scene '{scenePath}'");
                    return null;
                }

                var instance = scene.Instantiate<NarrativeSequence>();
                instance.SequenceId = sequenceId;
                this.sequenceContainer?.AddChild(instance);
                return instance;
            }
            catch (Exception ex)
            {
                GD.PrintErr($"GhostTerminalDirector: Exception loading sequence '{sequenceId}': {ex.Message}");
                return null;
            }
        }

        // Look for an existing child node with the sequence ID
        var existing = this.GetNode<NarrativeSequence>($"SequenceContainer/{sequenceId}");
        if (existing != null)
        {
            return existing;
        }

        GD.PrintErr($"GhostTerminalDirector: Sequence '{sequenceId}' not found in scene map or children");
        return null;
    }

    /// <summary>
    /// Unloads (removes) the currently playing sequence.
    /// </summary>
    private void UnloadSequence()
    {
        if (this.currentSequence == null)
        {
            return;
        }

        this.currentSequence.SequenceComplete -= this.OnSequenceComplete;
        this.currentSequence.SequenceInput -= this.OnSequenceInput;
        this.currentSequence.QueueFree();
        this.currentSequence = null;
    }

    /// <summary>
    /// Handles the SequenceComplete signal from a sequence.
    /// Enqueues the next sequence specified by the sequence.
    /// </summary>
    /// <param name="nextSequenceId">The ID of the next sequence to play.</param>
    private void OnSequenceComplete(string nextSequenceId)
    {
        if (!string.IsNullOrEmpty(nextSequenceId))
        {
            this.sequenceQueue.Enqueue(nextSequenceId);
        }
    }

    /// <summary>
    /// Handles the SequenceInput signal from a sequence.
    /// This is used for routing user choices to determine branching paths.
    /// Default behavior enqueues the input as a sequence ID.
    /// Override or extend this method to implement custom routing logic.
    /// </summary>
    /// <param name="inputId">The input identifier from the sequence.</param>
    private void OnSequenceInput(string inputId)
    {
        // Default: Treat input as a sequence ID to enqueue
        if (!string.IsNullOrEmpty(inputId))
        {
            this.sequenceQueue.Enqueue(inputId);
        }
    }

    /// <summary>
    /// Cleans up all sequence instances and resets state.
    /// </summary>
    private void Cleanup()
    {
        this.UnloadSequence();
        this.sequenceQueue.Clear();

        // Clear all child sequences
        if (this.sequenceContainer != null)
        {
            foreach (Node child in this.sequenceContainer.GetChildren())
            {
                child.QueueFree();
            }
        }
    }
}
