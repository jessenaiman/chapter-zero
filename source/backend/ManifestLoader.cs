using System.Collections.Generic;
using System.Linq;
using Godot;
using Newtonsoft.Json;

namespace OmegaSpiral.Source.Backend;

/// <summary>
/// Minimal loader for the main menu stage manifest.
/// </summary>
public sealed class ManifestLoader
{
    private readonly Dictionary<int, ManifestStage> _stageIndex = new();

    /// <summary>
    /// Loads the manifest located at the specified path.
    /// </summary>
    /// <param name="manifestPath">Resource path to the JSON manifest.</param>
    /// <returns>Ordered list of stages; empty list on failure.</returns>
    public IReadOnlyList<ManifestStage> LoadManifest(string manifestPath)
    {
        _stageIndex.Clear();

        if (!FileAccess.FileExists(manifestPath))
        {
            GD.PrintErr($"[ManifestLoader] Manifest not found: {manifestPath}");
            return System.Array.Empty<ManifestStage>();
        }

        try
        {
            var json = FileAccess.GetFileAsString(manifestPath);
            var manifest = JsonConvert.DeserializeObject<MenuManifest>(json);

            if (manifest?.Stages == null || manifest.Stages.Count == 0)
            {
                GD.PrintErr($"[ManifestLoader] Manifest has no stages: {manifestPath}");
                return System.Array.Empty<ManifestStage>();
            }

            foreach (var stage in manifest.Stages)
            {
                _stageIndex[stage.Id] = stage;
            }

            return _stageIndex.Values
                .OrderBy(stage => stage.Id)
                .ToList();
        }
        catch (JsonException ex)
        {
            GD.PrintErr($"[ManifestLoader] Failed to parse manifest: {ex.Message}");
            return System.Array.Empty<ManifestStage>();
        }
    }

    /// <summary>
    /// Retrieves a stage by its identifier after a manifest has been loaded.
    /// </summary>
    public ManifestStage? GetStage(int stageId)
    {
        return _stageIndex.TryGetValue(stageId, out var stage) ? stage : null;
    }

    private sealed class MenuManifest
    {
        [JsonProperty("stages")]
        public List<ManifestStage> Stages { get; set; } = new();
    }
}

/// <summary>
/// Represents a stage entry in the main menu manifest.
/// </summary>
public sealed class ManifestStage
{
    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("path")]
    public string Path { get; set; } = string.Empty;

    [JsonProperty("displayName")]
    public string DisplayName { get; set; } = string.Empty;

    [JsonProperty("type")]
    public string? Type { get; set; }

    [JsonProperty("supportsThreads")]
    public bool SupportsThreads { get; set; }
}
