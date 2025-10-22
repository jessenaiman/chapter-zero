// <copyright file="StageManifestLoader.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using Godot;
using System.Collections.Generic;

namespace OmegaSpiral.Source.Scripts.Infrastructure;

/// <summary>
/// Loads stage manifests from JSON files.
/// Enables designers to define stage structure (scenes, order, transitions) without coding.
/// </summary>
public class StageManifestLoader
{
    /// <summary>
    /// Loads a stage manifest from a JSON file.
    /// </summary>
    /// <param name="manifestPath">Path to the stage_manifest.json file.</param>
    /// <returns>A populated StageManifest, or null if loading failed.</returns>
    public StageManifest? LoadManifest(string manifestPath)
    {
        if (!ResourceLoader.Exists(manifestPath))
        {
            GD.PrintErr($"[StageManifestLoader] Manifest not found: {manifestPath}");
            return null;
        }

        try
        {
            var jsonText = Godot.FileAccess.GetFileAsString(manifestPath);
            var json = new Json();
            if (json.Parse(jsonText) != Error.Ok)
            {
                GD.PrintErr($"[StageManifestLoader] Invalid JSON in {manifestPath}: {json.GetErrorMessage()}");
                return null;
            }

            var root = json.Data.AsGodotDictionary();
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
        catch (System.Exception ex)
        {
            GD.PrintErr($"[StageManifestLoader] Error loading manifest: {ex.Message}");
            return null;
        }
    }
}
