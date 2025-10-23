// <copyright file="EchoOrchestratorBeat.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using System.Collections.Generic;
using OmegaSpiral.Source.Narrative;

namespace OmegaSpiral.Source.Scripts.Stages.Stage2;

/// <summary>
/// Represents the flow of the Echo Chamber stage as an ordered sequence of beats.
/// Mirrors the GhostTerminalCinematicPlan pattern but for dungeon exploration.
/// Each beat represents a playable segment: intro, interlude, chamber, or finale.
/// </summary>
public sealed class EchoOrchestratorBeat
{
    private readonly List<IEchoBeat> beats = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="EchoOrchestratorBeat"/> class.
    /// Constructs the beat sequence from the Echo Chamber plan.
    /// </summary>
    /// <param name="plan">The loaded stage plan.</param>
    public EchoOrchestratorBeat(EchoChamberPlan plan)
    {
        // Beat 1: System intro
        this.beats.Add(new EchoIntroBeat(plan.Metadata));

        // Beats 2-7: Interlude + Chamber pairs (3 iterations)
        for (int i = 0; i < plan.Interludes.Count && i < plan.Chambers.Count; i++)
        {
            this.beats.Add(new EchoInterludeBeat(plan.Interludes[i], i));
            this.beats.Add(new EchoChamberBeat(plan.Chambers[i], i));
        }

        // Beat 8: Finale claim
        this.beats.Add(new EchoFinaleBeat(plan.Finale, plan.Dreamweavers));
    }

    /// <summary>
    /// Gets all beats in playable order.
    /// </summary>
    public IReadOnlyList<IEchoBeat> AllBeats => this.beats.AsReadOnly();

    /// <summary>
    /// Gets the beat at the specified index.
    /// </summary>
    /// <param name="index">The zero-based beat index.</param>
    /// <returns>The beat at that position.</returns>
    public IEchoBeat GetBeat(int index)
    {
        return this.beats[index];
    }

    /// <summary>
    /// Gets the total number of beats.
    /// </summary>
    public int BeatCount => this.beats.Count;
}

/// <summary>
/// Common interface for all beat types in the Echo Chamber orchestration.
/// </summary>
public interface IEchoBeat
{
    /// <summary>
    /// Gets the beat type classification.
    /// </summary>
    EchoBeatKind Kind { get; }

    /// <summary>
    /// Gets the unique identifier for this beat instance.
    /// </summary>
    string BeatId { get; }
}

/// <summary>
/// Beat type enumeration for Echo Chamber stage flow.
/// </summary>
public enum EchoBeatKind
{
    /// <summary>System intro with metadata display.</summary>
    SystemIntro,

    /// <summary>Interlude with Dreamweaver question and three choices.</summary>
    Interlude,

    /// <summary>Chamber with dungeon exploration and object interaction.</summary>
    Chamber,

    /// <summary>Finale with Dreamweaver claim and responses.</summary>
    Finale
}

/// <summary>
/// Represents the system intro beat at stage start.
/// </summary>
/// <param name="Metadata">The stage metadata to display.</param>
public sealed record EchoIntroBeat(EchoChamberMetadata Metadata) : IEchoBeat
{
    /// <inheritdoc/>
    public EchoBeatKind Kind => EchoBeatKind.SystemIntro;

    /// <inheritdoc/>
    public string BeatId => "intro";
}

/// <summary>
/// Represents an interlude beat where a Dreamweaver poses a philosophical question.
/// </summary>
/// <param name="Interlude">The interlude data containing prompt and options.</param>
/// <param name="Index">The zero-based interlude index (0-2).</param>
public sealed record EchoInterludeBeat(EchoChamberInterlude Interlude, int Index) : IEchoBeat
{
    /// <inheritdoc/>
    public EchoBeatKind Kind => EchoBeatKind.Interlude;

    /// <inheritdoc/>
    public string BeatId => $"interlude_{this.Index}_{this.Interlude.Owner}";
}

/// <summary>
/// Represents a chamber beat where the player explores a dungeon and interacts with objects.
/// </summary>
/// <param name="Chamber">The chamber data containing layout and objects.</param>
/// <param name="Index">The zero-based chamber index (0-2).</param>
public sealed record EchoChamberBeat(EchoChamberChamber Chamber, int Index) : IEchoBeat
{
    /// <inheritdoc/>
    public EchoBeatKind Kind => EchoBeatKind.Chamber;

    /// <inheritdoc/>
    public string BeatId => $"chamber_{this.Index}_{this.Chamber.Owner}";
}

/// <summary>
/// Represents the finale beat where the chosen Dreamweaver makes their claim.
/// </summary>
/// <param name="Finale">The finale configuration with claim dialogue.</param>
/// <param name="Dreamweavers">All Dreamweaver definitions for styling.</param>
public sealed record EchoFinaleBeat(
    EchoChamberFinale Finale,
    IReadOnlyList<EchoChamberDreamweaver> Dreamweavers) : IEchoBeat
{
    /// <inheritdoc/>
    public EchoBeatKind Kind => EchoBeatKind.Finale;

    /// <inheritdoc/>
    public string BeatId => "finale";
}
