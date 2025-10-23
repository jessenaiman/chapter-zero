using System;
using System.Collections.Generic;
using OmegaSpiral.Source.Narrative;
using OmegaSpiral.Source.Scripts.Infrastructure;

namespace OmegaSpiral.Source.Scripts.Stages.Stage2;

/// <summary>
/// Loads and caches the data-driven plan for the Stage 2 Nethack sequence.
/// Mirrors <see cref="GhostTerminalCinematicDirector"/> but targets the dungeon alignment experience.
/// </summary>
public static class NethackDirector
{
    private const string DataPath = "res://source/stages/stage_2/stage_2.json";
    private const string SchemaPath = "res://source/data/schemas/echo_chamber_schema.json";

    private static readonly object SyncRoot = new();
    private static NethackPlan? cachedPlan;

    /// <summary>
    /// Gets the immutable plan describing Stage 2. Loads and validates data on first access.
    /// </summary>
    /// <returns>The stage plan.</returns>
    public static NethackPlan GetPlan()
    {
        if (cachedPlan != null)
        {
            return cachedPlan;
        }

        lock (SyncRoot)
        {
            if (cachedPlan == null)
            {
                cachedPlan = LoadPlan();
            }
        }

        return cachedPlan!;
    }

    /// <summary>
    /// Clears cached data. Intended for tests that mutate the underlying asset.
    /// </summary>
    public static void Reset()
    {
        lock (SyncRoot)
        {
            cachedPlan = null;
        }
    }

    private static NethackPlan LoadPlan()
    {
        var config = ConfigurationService.LoadConfiguration(DataPath);

        if (!ConfigurationService.ValidateConfiguration(config, SchemaPath))
        {
            throw new InvalidOperationException("Echo Chamber data failed schema validation.");
        }

        NarrativeSceneData sceneData = NarrativeSceneFactory.Create(config);

        if (sceneData.EchoChamber == null)
        {
            throw new InvalidOperationException("Echo Chamber data missing from parsed narrative scene.");
        }

        return BuildPlan(sceneData.EchoChamber);
    }

    private static NethackPlan BuildPlan(EchoChamberData data)
    {
        return new NethackPlan(
            data.Metadata,
            new List<EchoChamberDreamweaver>(data.Dreamweavers),
            new List<EchoChamberInterlude>(data.Interludes),
            new List<EchoChamberChamber>(data.Chambers),
            data.Finale);
    }
}

/// <summary>
/// Immutable view over the Stage 2 data used at runtime.
/// </summary>
/// <param name="Metadata">Stage metadata surfaced to UI systems.</param>
/// <param name="Dreamweavers">Dreamweaver definitions for styling and lookup.</param>
/// <param name="Interludes">Ordered interludes that precede each chamber.</param>
/// <param name="Chambers">The chamber payloads the player traverses.</param>
/// <param name="Finale">Finale configuration for claim dialogue.</param>
public sealed record NethackPlan(
    EchoChamberMetadata Metadata,
    IReadOnlyList<EchoChamberDreamweaver> Dreamweavers,
    IReadOnlyList<EchoChamberInterlude> Interludes,
    IReadOnlyList<EchoChamberChamber> Chambers,
    EchoChamberFinale Finale);
