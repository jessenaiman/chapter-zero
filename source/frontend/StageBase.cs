// <copyright file="StageBase.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

namespace OmegaSpiral.Source.Frontend;

using Godot;
using System.Threading.Tasks;

/// <summary>
/// Base class for Node-based stage controllers. Each concrete stage controller should inherit from
/// <see cref="StageBase"/> and implement <see cref="ExecuteStageAsync"/> to run the stage.
/// Implements IStageBase to work with GameManager.
/// </summary>
[GlobalClass]
public abstract partial class StageBase : Node, IStageBase
{
    /// <summary>
    /// Event for stage completion (implements IStageBase contract).
    /// </summary>
    public event StageCompleteDelegate? StageComplete;

    /// <summary>
    /// Gets the stage identifier. Each stage should override this to provide its ID.
    /// </summary>
    public abstract int StageId { get; }

    /// <summary>
    /// Starts and executes the stage logic. Implementations should perform their sequence
    /// (e.g., display narrative, run combat, etc.) and emit StageComplete when finished.
    /// </summary>
    /// <returns>A <see cref="Task"/> that completes when the stage finishes.</returns>
    public abstract Task ExecuteStageAsync();

    /// <summary>
    /// Helper method to emit the StageComplete event from derived implementations.
    /// Call this when the stage's work is complete.
    /// </summary>
    protected void EmitStageComplete()
    {
        this.StageComplete?.Invoke();
    }
}
