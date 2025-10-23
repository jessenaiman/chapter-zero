using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using Godot;
using Dictionary = Godot.Collections.Dictionary;
using Array = Godot.Collections.Array;

namespace OmegaSpiral.Source.Scripts.Infrastructure;

/// <summary>
/// Represents a stage entry from the manifest.
/// </summary>
public class ManifestStage
{
    /// <summary>Numeric stage ID (1-5).</summary>
    public int Id { get; set; }

    /// <summary>Type of stage (e.g., "narrative_terminal", "ascii_dungeon_sequence").</summary>
    public string Type { get; set; } = string.Empty;

    /// <summary>Path identifier for the stage (maps to scene files).</summary>
    public string Path { get; set; } = string.Empty;

    /// <summary>Whether this stage supports Dreamweaver threads.</summary>
    public bool SupportsThreads { get; set; }

    /// <summary>User-friendly display name (derived from type or ID).</summary>
    public string DisplayName { get; set; } = string.Empty;
}

/// <summary>
/// Loads and parses the game manifest defining all stages.
/// Enables dynamic stage discovery without hardcoding scene paths.
/// </summary>
public class ManifestLoader : BaseManifestLoader<IReadOnlyList<ManifestStage>>
{
    private List<ManifestStage> _stages = new();

    /// <summary>
    /// Loads the manifest from the JSON file.
    /// </summary>
    /// <param name="manifestPath">Path to manifest.json (e.g., "res://source/data/manifest.json").</param>
    /// <returns>List of loaded stages, or empty list if loading failed.</returns>
    public new IReadOnlyList<ManifestStage> LoadManifest(string manifestPath)
    {
        var stages = base.LoadManifest(manifestPath);
        _stages = stages?.ToList() ?? new List<ManifestStage>();
        return _stages;
    }

    /// <summary>
    /// Gets all loaded stages.
    /// </summary>
    public IReadOnlyList<ManifestStage> GetAllStages()
    {
        return _stages;
    }

    /// <summary>
    /// Gets a specific stage by ID.
    /// </summary>
    /// <param name="stageId">The numeric stage ID (1-5).</param>
    /// <returns>The stage, or null if not found.</returns>
    public ManifestStage? GetStage(int stageId)
    {
        return _stages.FirstOrDefault(s => s.Id == stageId);
    }

    /// <summary>
    /// Gets the scene flow file path for a specific stage.
    /// Maps stage IDs to their corresponding scene_flow.json files.
    /// </summary>
    /// <param name="stageId">The numeric stage ID (1-5).</param>
    /// <returns>Full path to the stage's scene_flow.json, or null if stage not found.</returns>
    public string? GetSceneFlowPath(int stageId)
    {
        return stageId switch
        {
            1 => "res://source/data/stages/ghost_terminal_archives/scene_flow.json",
            2 => "res://source/data/stages/stage_2/scene_flow.json",
            3 => "res://source/data/stages/stage_3/scene_flow.json",
            4 => "res://source/data/stages/stage_4/scene_flow.json",
            5 => "res://source/data/stages/stage_5/scene_flow.json",
            _ => null
        };
    }

    /// <summary>
    /// Parses the manifest JSON into ManifestStage objects.
    /// </summary>
    protected override IReadOnlyList<ManifestStage>? ParseManifest(Variant jsonData)
    {
        var stages = new List<ManifestStage>();

        if (jsonData.VariantType != Variant.Type.Dictionary)
        {
            GD.PrintErr("[ManifestLoader] Invalid JSON structure: root is not a dictionary");
            return stages;
        }

        var dict = (Dictionary)jsonData;

        if (dict.ContainsKey("scenes"))
        {
            var scenesArray = (Array)dict["scenes"];
            foreach (var stageVariant in scenesArray)
            {
                var stageDict = (Dictionary)stageVariant;

                var stage = new ManifestStage
                {
                    Id = (int)stageDict["id"].AsInt32(),
                    Type = stageDict["type"].AsString(),
                    Path = stageDict["path"].AsString(),
                    SupportsThreads = stageDict.ContainsKey("supportsThreads") && stageDict["supportsThreads"].AsBool(),
                };

                // Generate display name from stage ID
                stage.DisplayName = $"Stage {stage.Id}";

                stages.Add(stage);
            }
        }

        return stages;
    }
}
