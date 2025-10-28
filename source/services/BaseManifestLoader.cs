using Godot;
using System;

namespace OmegaSpiral.Source.Scripts.Infrastructure
{
    /// <summary>
    /// Generic base class for manifest loaders. Handles file IO and JSON parsing.
    /// </summary>
    /// <typeparam name="T">Type of manifest object to load.</typeparam>
    public abstract class BaseManifestLoader<T>
    {
        /// <summary>
        /// Loads and parses a manifest file.
        /// </summary>
        /// <param name="manifestPath">Path to the manifest JSON file.</param>
        /// <returns>Parsed manifest object, or null if loading failed.</returns>
        public T? LoadManifest(string manifestPath)
        {
            if (!ResourceLoader.Exists(manifestPath))
            {
                GD.PrintErr($"[BaseManifestLoader] Manifest not found: {manifestPath}");
                return default;
            }

            try
            {
                var jsonText = Godot.FileAccess.GetFileAsString(manifestPath);
                var json = new Json();
                if (json.Parse(jsonText) != Error.Ok)
                {
                    GD.PrintErr($"[BaseManifestLoader] Invalid JSON in {manifestPath}: {json.GetErrorMessage()}");
                    return default;
                }
                return ParseManifest(json.Data);
            }
            catch (Exception ex)
            {
                GD.PrintErr($"[BaseManifestLoader] Error loading manifest: {ex.Message}");
                return default;
            }
        }

        /// <summary>
        /// Parses the manifest JSON data into the manifest object.
        /// </summary>
        /// <param name="jsonData">Root JSON Variant.</param>
        /// <returns>Parsed manifest object, or null if parsing failed.</returns>
        protected abstract T? ParseManifest(Variant jsonData);
    }
}
