// <copyright file="DesignConfigService.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using Godot;
using Godot.Collections;
using System;

namespace OmegaSpiral.Source.Scripts.Infrastructure
{
    /// <summary>
    /// Service for loading design system configuration (colors, shader parameters) from JSON
    /// and applying them globally to all shader materials in the game.
    /// Ensures single source of truth: JSON → C# → All Shaders (no manual .tres edits needed).
    /// </summary>
    public static class DesignConfigService
    {
        private const string _DesignConfigPath = "res://source/resources/omega_spiral_colors.json";

        private static Godot.Collections.Dictionary<string, Variant>? _CachedDesignConfig;

        /// <summary>
        /// Gets the cached design configuration dictionary.
        /// </summary>
        public static Godot.Collections.Dictionary<string, Variant> DesignConfig => _CachedDesignConfig ??= LoadDesignConfig();

        /// <summary>
        /// Loads the design configuration from JSON file.
        /// </summary>
        /// <returns>Dictionary containing design system colors and shader parameters.</returns>
        private static Godot.Collections.Dictionary<string, Variant> LoadDesignConfig()
        {
            try
            {
                string jsonContent = Godot.FileAccess.GetFileAsString(_DesignConfigPath);
                var json = new Json();
                json.Parse(jsonContent);
                var config = (Godot.Collections.Dictionary<string, Variant>)json.Data;
                GD.Print("[DesignConfigService] Design config loaded successfully.");
                return config;
            }
            catch (Exception ex)
            {
                GD.PrintErr($"[DesignConfigService] Failed to load design config: {ex.Message}");
                return new Godot.Collections.Dictionary<string, Variant>();
            }
        }

        /// <summary>
        /// Applies design system colors and shader parameters to all shader materials in a node tree.
        /// Traverses the scene tree starting from the given node and updates all ShaderMaterial instances.
        /// </summary>
        /// <param name="rootNode">The root node to start traversing from. If null, uses the scene tree root.</param>
        public static void ApplyDesignColorsToAllShaders(Node? rootNode = null)
        {
            rootNode ??= Engine.GetMainLoop() is SceneTree tree ? tree.Root : null;
            if (rootNode == null)
            {
                GD.PrintErr("[DesignConfigService] Cannot apply colors: root node is null.");
                return;
            }

            var config = DesignConfig;
            if (config.Count == 0)
            {
                GD.PrintErr("[DesignConfigService] Design config is empty. Cannot apply colors.");
                return;
            }

            var updated = 0;
            TraverseAndApplyColors(rootNode, config, ref updated);
            GD.Print($"[DesignConfigService] Applied design colors to {updated} shader materials.");
        }

        /// <summary>
        /// Recursively traverses the node tree and applies design colors to all ShaderMaterial instances.
        /// </summary>
        private static void TraverseAndApplyColors(Node node, Godot.Collections.Dictionary<string, Variant> config, ref int updated)
        {
            // Check if this node has a CanvasItem with a ShaderMaterial
            if (node is CanvasItem canvasItem && canvasItem.Material is ShaderMaterial material)
            {
                ApplyShaderParameters(material, config);
                updated++;
            }

            // Recursively process children
            foreach (Node child in node.GetChildren())
            {
                TraverseAndApplyColors(child, config, ref updated);
            }
        }

        /// <summary>
        /// Applies shader parameters from the design config to a specific ShaderMaterial.
        /// </summary>
        private static void ApplyShaderParameters(ShaderMaterial material, Godot.Collections.Dictionary<string, Variant> config)
        {
            try
            {
                // Get shader parameters section
                if (config.TryGetValue("shader_parameters", out var shaderParamsVar))
                {
                    var shaderParams = (Dictionary)shaderParamsVar;

                    // Try to identify which shader this material uses and apply relevant parameters
                    ApplyPulsingBackgroundParams(material, shaderParams);
                    ApplySpiralBorderParams(material, shaderParams);
                    ApplyCRTParams(material, shaderParams);
                }
            }
            catch (Exception ex)
            {
                GD.PrintErr($"[DesignConfigService] Error applying shader parameters: {ex.Message}");
            }
        }

        /// <summary>
        /// Applies pulsing background shader parameters.
        /// </summary>
        private static void ApplyPulsingBackgroundParams(ShaderMaterial material, Dictionary shaderParams)
        {
            if (shaderParams.TryGetValue("pulsing_background", out var paramsVar))
            {
                var bgParams = (Dictionary)paramsVar;

                if (bgParams.TryGetValue("base_color", out var baseColorVar))
                {
                    var colorDict = (Dictionary)baseColorVar;
                    var color = ExtractColor(colorDict);
                    material.SetShaderParameter("base_color", color);
                }

                if (bgParams.TryGetValue("glow_color", out var glowColorVar))
                {
                    var colorDict = (Dictionary)glowColorVar;
                    var color = ExtractColor(colorDict);
                    material.SetShaderParameter("glow_color", color);
                }

                if (bgParams.TryGetValue("pulse_speed", out var speedVar))
                {
                    material.SetShaderParameter("pulse_speed", (float)speedVar);
                }

                if (bgParams.TryGetValue("pulse_strength", out var strengthVar))
                {
                    material.SetShaderParameter("pulse_strength", (float)strengthVar);
                }

                if (bgParams.TryGetValue("vignette_radius", out var radiusVar))
                {
                    material.SetShaderParameter("vignette_radius", (float)radiusVar);
                }

                if (bgParams.TryGetValue("vignette_softness", out var softnessVar))
                {
                    material.SetShaderParameter("vignette_softness", (float)softnessVar);
                }
            }
        }

        /// <summary>
        /// Applies spiral border shader parameters.
        /// </summary>
        private static void ApplySpiralBorderParams(ShaderMaterial material, Dictionary shaderParams)
        {
            if (shaderParams.TryGetValue("spiral_border", out var paramsVar))
            {
                var borderParams = (Dictionary)paramsVar;

                if (borderParams.TryGetValue("border_color", out var borderColorVar))
                {
                    var colorDict = (Dictionary)borderColorVar;
                    var color = ExtractColor(colorDict);
                    material.SetShaderParameter("border_color", color);
                }

                if (borderParams.TryGetValue("border_width", out var widthVar))
                {
                    material.SetShaderParameter("border_width", (float)widthVar);
                }

                if (borderParams.TryGetValue("wave_speed", out var waveSpeedVar))
                {
                    material.SetShaderParameter("wave_speed", (float)waveSpeedVar);
                }

                if (borderParams.TryGetValue("wave_amplitude", out var waveAmpVar))
                {
                    material.SetShaderParameter("wave_amplitude", (float)waveAmpVar);
                }
            }
        }

        /// <summary>
        /// Applies CRT shader parameters (phosphor, scanlines, glitch).
        /// </summary>
        private static void ApplyCRTParams(ShaderMaterial material, Dictionary shaderParams)
        {
            // Phosphor overlay
            if (shaderParams.TryGetValue("crt_phosphor", out var phosphorVar))
            {
                var phosphorParams = (Dictionary)phosphorVar;
                if (phosphorParams.TryGetValue("phosphor_color", out var phosphorColorVar))
                {
                    var colorDict = (Dictionary)phosphorColorVar;
                    var color = ExtractColor(colorDict);
                    material.SetShaderParameter("phosphor_color", color);
                }
            }

            // Scanlines
            if (shaderParams.TryGetValue("crt_scanlines", out var scanlinesVar))
            {
                var scanlineParams = (Dictionary)scanlinesVar;
                if (scanlineParams.TryGetValue("scanline_color", out var scanlineColorVar))
                {
                    var colorDict = (Dictionary)scanlineColorVar;
                    var color = ExtractColor(colorDict);
                    material.SetShaderParameter("scanline_color", color);
                }

                if (scanlineParams.TryGetValue("scanline_intensity", out var intensityVar))
                {
                    material.SetShaderParameter("scanline_intensity", (float)intensityVar);
                }
            }

            // Glitch
            if (shaderParams.TryGetValue("crt_glitch", out var glitchVar))
            {
                var glitchParams = (Dictionary)glitchVar;
                if (glitchParams.TryGetValue("glitch_color", out var glitchColorVar))
                {
                    var colorDict = (Dictionary)glitchColorVar;
                    var color = ExtractColor(colorDict);
                    material.SetShaderParameter("glitch_color", color);
                }

                if (glitchParams.TryGetValue("glitch_intensity", out var glitchIntensityVar))
                {
                    material.SetShaderParameter("glitch_intensity", (float)glitchIntensityVar);
                }
            }
        }

        /// <summary>
        /// Extracts a Color from a dictionary with r, g, b, a keys.
        /// </summary>
        private static Color ExtractColor(Dictionary colorDict)
        {
            var r = colorDict.TryGetValue("r", out var rVar) ? (float)rVar : 1f;
            var g = colorDict.TryGetValue("g", out var gVar) ? (float)gVar : 1f;
            var b = colorDict.TryGetValue("b", out var bVar) ? (float)bVar : 1f;
            var a = colorDict.TryGetValue("a", out var aVar) ? (float)aVar : 1f;

            return new Color(r, g, b, a);
        }

        /// <summary>
        /// Gets a specific design color by path (e.g., "design_system.deep_space").
        /// </summary>
        /// <param name="colorPath">Dot-separated path to the color (e.g., "design_system.deep_space").</param>
        /// <returns>The color value, or white if not found.</returns>
        public static Color GetDesignColor(string colorPath)
        {
            try
            {
                var parts = colorPath.Split('.');
                var current = (Variant)DesignConfig;

                foreach (var part in parts)
                {
                    if (current.Obj is Dictionary dict && dict.TryGetValue(part, out var value))
                    {
                        current = value;
                    }
                    else
                    {
                        return Colors.White;
                    }
                }

                if (current.Obj is Dictionary colorDict)
                {
                    return ExtractColor(colorDict);
                }

                return Colors.White;
            }
            catch (Exception ex)
            {
                GD.PrintErr($"[DesignConfigService] Error getting design color '{colorPath}': {ex.Message}");
                return Colors.White;
            }
        }
    }
}
