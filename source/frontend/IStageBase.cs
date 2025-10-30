// <copyright file="IStageBase.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

namespace OmegaSpiral.Source.Frontend;

using System.Threading.Tasks;

/// <summary>
/// Delegate for stage completion events.
/// </summary>
public delegate void StageCompleteDelegate();

/// <summary>
/// Contract for stage implementations. Stage controllers must implement this interface.
/// </summary>
public interface IStageBase
{
    /// <summary>
    /// Gets the stage identifier. Each stage should provide its ID.
    /// </summary>
    int StageId { get; }

    /// <summary>
    /// Starts and executes the stage logic. Implementations should perform their sequence
    /// (e.g., display narrative, run combat, etc.) and emit the StageComplete event when finished.
    /// </summary>
    /// <returns>A <see cref="Task"/> that completes when the stage finishes.</returns>
    Task ExecuteStageAsync();

    /// <summary>
    /// Event emitted when the stage completes its work and the GameManager can advance to the next stage.
    /// </summary>
    event StageCompleteDelegate? StageComplete;
}
