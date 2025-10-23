// <copyright file="IEchoSequence.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using Godot;

namespace OmegaSpiral.Source.Scripts.Stages.Stage2;

/// <summary>
/// Common interface for all Echo Chamber sequence controllers (interludes and chambers).
/// Provides a consistent API for the EchoHub orchestrator to manage narrative flow.
/// Mirrors the NarrativeSequence pattern from Stage 1 but adapted for dungeon exploration.
/// </summary>
public interface IEchoSequence
{
    /// <summary>
    /// Emitted when the sequence completes and is ready to transition.
    /// </summary>
    /// <param name="nextSequenceId">The identifier of the next sequence to play, or empty string for default flow.</param>
    [Signal]
    public delegate void SequenceCompleteEventHandler(string nextSequenceId);

    /// <summary>
    /// Emitted when user makes a choice within the sequence.
    /// </summary>
    /// <param name="choiceId">The identifier of the selected choice.</param>
    /// <param name="alignment">The Dreamweaver alignment associated with this choice.</param>
    [Signal]
    public delegate void ChoiceMadeEventHandler(string choiceId, string alignment);

    /// <summary>
    /// Plays the sequence asynchronously. Implementations should handle their own content display,
    /// user input, and emit SequenceComplete when ready to proceed.
    /// </summary>
    /// <returns>A task that completes when the sequence finishes.</returns>
    System.Threading.Tasks.Task PlayAsync();

    /// <summary>
    /// Gets the unique identifier for this sequence.
    /// </summary>
    string SequenceId { get; }
}
