// <copyright file="StageManager.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using Godot;
using System.Threading.Tasks;

namespace OmegaSpiral.Source.Infrastructure;

/// <summary>
/// Base class for stage controllers. Each concrete stage controller should inherit from
/// <see cref="StageManager"/> and implement <see cref="ExecuteStageAsync"/> to run the stage.
/// Emits <see cref="StageComplete"/> when the stage finishes its work.
/// </summary>
[GlobalClass]
public abstract partial class StageManager : Node
{
    /// <summary>
    /// Emitted when the stage completes its work and the GameManager can advance to the next stage.
    /// </summary>
    [Signal]
    public delegate void StageCompleteEventHandler();

    /// <summary>
    /// Gets the stage identifier. Each stage should override this to provide its ID.
    /// </summary>
    public abstract int StageId { get; }

    /// <summary>
    /// Starts and executes the stage logic. Implementations should perform their sequence
    /// (e.g., display narrative, run combat, etc.) and emit <see cref="StageComplete"/> when finished.
    /// </summary>
    /// <returns>A <see cref="Task"/> that completes when the stage finishes.</returns>
    public abstract Task ExecuteStageAsync();

    /// <summary>
    /// Helper method to emit the <see cref="StageComplete"/> signal from derived implementations.
    /// Call this when the stage's work is complete.
    /// </summary>
    protected void EmitStageComplete()
    {
        EmitSignal(SignalName.StageComplete);
    }
}
