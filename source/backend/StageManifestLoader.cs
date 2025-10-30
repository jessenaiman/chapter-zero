// <copyright file="StageManifestLoader.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using Godot;

namespace OmegaSpiral.Source.Backend;

/// <summary>
/// Loads stage manifests from JSON files.
/// Enables designers to define stage structure (scenes, order, transitions) without coding.
/// </summary>
public class StageManifestLoader : BaseManifestLoader<StageManifest>
{
    /// <summary>
    /// Loads a stage manifest from a JSON file.
    /// </summary>
    /// <param name="manifestPath">Path to the stage_manifest.json file.</param>
    /// <returns>A populated StageManifest, or null if loading failed.</returns>
    public new StageManifest? LoadManifest(string manifestPath)
    {
        return base.LoadManifest(manifestPath);
    }

    /// <summary>
    /// Parses the manifest JSON data into a StageManifest object.
    /// </summary>
    /// <param name="jsonData">Root JSON Variant.</param>
    /// <returns>Parsed StageManifest object, or null if parsing failed.</returns>
    protected override StageManifest? ParseManifest(Variant jsonData)
    {
        if (jsonData.VariantType != Variant.Type.Dictionary)
        {
            GD.PrintErr("[StageManifestLoader] Invalid JSON structure: root is not a dictionary");
            return null;
        }

        var root = jsonData.AsGodotDictionary();
        var manifest = new StageManifest
        {
            StageId = (int)root["stageId"],
            StageName = root["stageName"].AsString(),
            Description = root["description"].AsString(),
            NextStagePath = root["nextStagePath"].AsString(),
        };

        // Load scenes
        var scenesArray = root["scenes"].AsGodotArray();
        foreach (var sceneData in scenesArray)
        {
            var sceneDict = sceneData.AsGodotDictionary();
            var scene = new StageSceneEntry
            {
                Id = sceneDict["id"].AsString(),
                DisplayName = sceneDict["displayName"].AsString(),
                SceneFile = sceneDict["sceneFile"].AsString(),
                NextSceneId = sceneDict.ContainsKey("nextSceneId") ? sceneDict["nextSceneId"].AsString() : null,
                ScriptClass = sceneDict.ContainsKey("scriptClass") ? sceneDict["scriptClass"].AsString() : null,
                Description = sceneDict.ContainsKey("description") ? sceneDict["description"].AsString() : null,
            };

            manifest.Scenes.Add(scene);
        }

        GD.Print($"[StageManifestLoader] Loaded stage manifest: Stage {manifest.StageId} ({manifest.StageName}) with {manifest.Scenes.Count} scenes");
        return manifest;
    }
}
