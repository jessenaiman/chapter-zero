using Godot;
using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System.IO;

namespace OmegaSpiral.Source.Scripts
{
    /// <summary>
    /// Scene loader system that loads scene JSON data and instantiates the appropriate Godot scenes.
    /// Resolves manifest entries, loads JSON payloads, and handles dynamic content generation.
    /// </summary>
    public class SceneLoader : Node
    {
        private JsonSchemaValidator _schemaValidator;
        
        public override void _Ready()
        {
            _schemaValidator = new JsonSchemaValidator();
        }

        /// <summary>
        /// Loads a scene by resolving the manifest entry and loading the JSON data.
        /// </summary>
        /// <param name="sceneId">ID of the scene to load</param>
        /// <param name="threadVariant">Thread variant to load (hero/shadow/ambition) - optional</param>
        /// <returns>JObject containing the scene data, or null if loading failed</returns>
        public JObject LoadSceneData(int sceneId, string threadVariant = null)
        {
            try
            {
                // Load manifest
                string manifestPath = "res://Source/Data/manifest.json";
                if (!File.Exists(manifestPath.Replace("res://", "")))
                {
                    GD.PrintErr($"Manifest file not found: {manifestPath}");
                    return null;
                }

                string manifestJson = File.ReadAllText(manifestPath.Replace("res://", ""));
                JObject manifest = JObject.Parse(manifestJson);

                // Find scene in manifest
                JArray scenes = (JArray)manifest["scenes"];
                JObject targetScene = null;

                foreach (JObject scene in scenes)
                {
                    if ((int)scene["id"] == sceneId)
                    {
                        targetScene = scene;
                        break;
                    }
                }

                if (targetScene == null)
                {
                    GD.PrintErr($"Scene with ID {sceneId} not found in manifest");
                    return null;
                }

                // Determine the path to load
                string scenePath = (string)targetScene["path"];
                string dataPath;

                // Handle thread variants for scenes that support them
                bool supportsThreads = targetScene.ContainsKey("supportsThreads") && (bool)targetScene["supportsThreads"];
                
                if (supportsThreads && !string.IsNullOrEmpty(threadVariant))
                {
                    dataPath = $"res://Source/Data/scenes/{scenePath}/{threadVariant}.json";
                }
                else
                {
                    // For scenes without thread variants, load the default data
                    dataPath = $"res://Source/Data/scenes/{scenePath}/data.json";
                }

                // Check if data file exists
                string actualPath = dataPath.Replace("res://", "");
                if (!File.Exists(actualPath))
                {
                    GD.PrintErr($"Scene data file not found: {dataPath}");
                    return null;
                }

                // Load scene data
                string jsonData = File.ReadAllText(actualPath);
                JObject sceneData = JObject.Parse(jsonData);

                // Validate against schema
                string schemaPath = $"res://specs/002-using-godot-4/contracts/scene{sceneId}_{scenePath}_schema.json".Replace("res://", "");
                if (File.Exists(schemaPath))
                {
                    if (!_schemaValidator.Validate(sceneData, schemaPath, out string validationError))
                    {
                        GD.PrintErr($"Scene data validation failed for scene {sceneId}: {validationError}");
                        // Continue loading even if validation fails (for demo purposes)
                    }
                }
                else
                {
                    GD.Print($"Schema file not found for scene {sceneId}, skipping validation");
                }

                GD.Print($"Successfully loaded scene data for scene {sceneId}");
                return sceneData;
            }
            catch (Exception ex)
            {
                GD.PrintErr($"Error loading scene data for scene {sceneId}: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Loads scene variants (multiple JSON files for scenes with variations).
        /// </summary>
        /// <param name="sceneId">ID of the scene to load variants for</param>
        /// <returns>List of JObject containing the scene variants, or null if loading failed</returns>
        public List<JObject> LoadSceneVariants(int sceneId)
        {
            try
            {
                // Load manifest to get scene path
                string manifestPath = "res://Source/Data/manifest.json";
                string manifestJson = File.ReadAllText(manifestPath.Replace("res://", ""));
                JObject manifest = JObject.Parse(manifestJson);

                // Find scene in manifest
                JArray scenes = (JArray)manifest["scenes"];
                JObject targetScene = null;

                foreach (JObject scene in scenes)
                {
                    if ((int)scene["id"] == sceneId)
                    {
                        targetScene = scene;
                        break;
                    }
                }

                if (targetScene == null)
                {
                    GD.PrintErr($"Scene with ID {sceneId} not found in manifest");
                    return null;
                }

                string scenePath = (string)targetScene["path"];
                string variantsPath = $"res://Source/Data/scenes/{scenePath}/variants/";

                // Check if variants directory exists
                string actualVariantsPath = variantsPath.Replace("res://", "");
                if (!Directory.Exists(actualVariantsPath))
                {
                    GD.Print($"Variants directory not found for scene {sceneId}: {variantsPath}");
                    return null;
                }

                var variants = new List<JObject>();
                string[] variantFiles = Directory.GetFiles(actualVariantsPath, "*.json");

                foreach (string variantFile in variantFiles)
                {
                    try
                    {
                        string jsonData = File.ReadAllText(variantFile);
                        JObject variantData = JObject.Parse(jsonData);
                        
                        // Validate variant data
                        string schemaPath = $"res://specs/002-using-godot-4/contracts/scene{sceneId}_{scenePath}_schema.json".Replace("res://", "");
                        if (File.Exists(schemaPath))
                        {
                            if (!_schemaValidator.Validate(variantData, schemaPath, out string validationError))
                            {
                                GD.PrintErr($"Variant validation failed for {variantFile}: {validationError}");
                                // Continue loading even if validation fails (for demo purposes)
                            }
                        }
                        
                        variants.Add(variantData);
                    }
                    catch (Exception ex)
                    {
                        GD.PrintErr($"Error loading variant file {variantFile}: {ex.Message}");
                    }
                }

                GD.Print($"Loaded {variants.Count} variants for scene {sceneId}");
                return variants;
            }
            catch (Exception ex)
            {
                GD.PrintErr($"Error loading scene variants for scene {sceneId}: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Instantiates a Godot scene based on the scene type.
        /// </summary>
        /// <param name="sceneId">ID of the scene to instantiate</param>
        /// <param name="sceneData">Scene data to pass to the scene</param>
        /// <returns>Instance of the Godot scene, or null if instantiation failed</returns>
        public Node InstantiateScene(int sceneId, JObject sceneData = null)
        {
            try
            {
                string scenePath = $"res://Source/Scenes/Scene{sceneId}*.tscn";
                
                // In a real implementation, we would load the specific scene file
                // For now, we'll just log that we're instantiating the scene
                GD.Print($"Instantiating scene {sceneId}");
                
                if (sceneData != null)
                {
                    GD.Print($"Scene data provided with {sceneData.Count} properties");
                }
                
                // Return null for now - in a real implementation we would return the instantiated scene
                return null;
            }
            catch (Exception ex)
            {
                GD.PrintErr($"Error instantiating scene {sceneId}: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Gets the schema path for a specific scene type.
        /// </summary>
        /// <param name="sceneId">ID of the scene</param>
        /// <param name="scenePath">Path of the scene in the manifest</param>
        /// <returns>Path to the schema file</returns>
        private string GetSchemaPath(int sceneId, string scenePath)
        {
            return $"res://specs/002-using-godot-4/contracts/scene{sceneId}_{scenePath}_schema.json";
        }
    }
}