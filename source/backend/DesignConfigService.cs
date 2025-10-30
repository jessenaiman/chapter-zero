// <copyright file="DesignConfigService.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OmegaSpiral.Source.Scripts.Infrastructure
{
    /// <summary>
    /// Service for loading design-system configuration (colors, shader presets, stage stacks) from JSON
    /// and applying them to shader materials or UI systems at runtime.
    /// Ensures single source of truth: JSON → C# → Shaders/UI (no manual .tres edits).
    /// </summary>
    public static class DesignConfigService
    {
        private const string _DesignConfigPath = "res://source/resources/omega_spiral_colors.json";

        private static Godot.Collections.Dictionary<string, Variant>? _CachedDesignConfig;

        /// <summary>
        /// Maps shader resource paths to base preset keys located inside the configuration file.
        /// </summary>
        private static readonly System.Collections.Generic.Dictionary<string, string> _ShaderResourceToPresetMap = new(StringComparer.OrdinalIgnoreCase)
        {
            { "res://source/shaders/spiral_border.gdshader", "spiral_border_base" },
            { "res://source/shaders/crt_phosphor.gdshader", "crt_phosphor_base" },
            { "res://source/shaders/crt_scanlines.gdshader", "crt_scanlines_base" },
            { "res://source/shaders/crt_glitch.gdshader", "crt_glitch_base" },
            { "res://source/shaders/pulsing_background.gdshader", "pulsing_background_base" }
        };

        /// <summary>
        /// Gets the cached design configuration dictionary.
        /// </summary>
        public static Godot.Collections.Dictionary<string, Variant> DesignConfig => _CachedDesignConfig ??= LoadDesignConfig();

        /// <summary>
        /// Attempts to retrieve a shader preset defined in the configuration file.
        /// </summary>
        /// <param name="presetName">Preset key (e.g., <c>boot_sequence</c>).</param>
        /// <param name="preset">Resolved preset containing shader path and parameter map.</param>
        /// <returns><see langword="true"/> when the preset exists; otherwise <see langword="false"/>.</returns>
        public static bool TryGetShaderPreset(string presetName, out DesignShaderPreset preset)
        {
            preset = default;
            if (string.IsNullOrWhiteSpace(presetName))
            {
                return false;
            }

            if (!TryGetSection("shader_presets", out var shaderPresets) ||
                !shaderPresets.TryGetValue(presetName, out var presetVariant) ||
                presetVariant.Obj is not Godot.Collections.Dictionary<string, Variant> presetDict)
            {
                return false;
            }

            var shaderPath = presetDict.TryGetValue("shader", out var shaderVariant) && shaderVariant.VariantType == Variant.Type.String
                ? (string)shaderVariant
                : null;

            var parameters = new System.Collections.Generic.Dictionary<string, Variant>(StringComparer.OrdinalIgnoreCase);
            if (presetDict.TryGetValue("parameters", out var parameterVariant) &&
                parameterVariant.Obj is Godot.Collections.Dictionary<string, Variant> parameterDict)
            {
                foreach (var key in parameterDict.Keys.OfType<string>())
                {
                    if (key.StartsWith("_", StringComparison.Ordinal))
                    {
                        continue;
                    }

                    if (!parameterDict.TryGetValue(key, out var valueVariant))
                    {
                        continue;
                    }

                    if (TryResolveParameterVariant(valueVariant, out var resolved))
                    {
                        parameters[key] = resolved;
                    }
                }
            }

            preset = new DesignShaderPreset(shaderPath, parameters);
            return true;
        }

        /// <summary>
        /// Attempts to retrieve the ordered stack of shader presets that define a stage/state visual.
        /// </summary>
        /// <param name="stageKey">Stage identifier (for example, <c>ghost_terminal</c>).</param>
        /// <param name="stateKey">State identifier (for example, <c>boot_sequence</c>).</param>
        /// <param name="presetStack">Ordered preset sequence (outermost → innermost layers).</param>
        /// <returns><see langword="true"/> when documentation for the stack exists; otherwise <see langword="false"/>.</returns>
        public static bool TryGetStagePresetStack(string stageKey, string stateKey, out IReadOnlyList<string> presetStack)
        {
            presetStack = System.Array.Empty<string>();
            if (!TryGetSection("stage_presets", out var stagePresets) ||
                !stagePresets.TryGetValue(stageKey, out var stageVariant) ||
                stageVariant.Obj is not Godot.Collections.Dictionary<string, Variant> stageDict ||
                !stageDict.TryGetValue("states", out var statesVariant) ||
                statesVariant.Obj is not Godot.Collections.Dictionary<string, Variant> statesDict ||
                !statesDict.TryGetValue(stateKey, out var stateVariant) ||
                stateVariant.Obj is not Godot.Collections.Dictionary<string, Variant> stateDict ||
                !stateDict.TryGetValue("visual_stack", out var stackVariant) ||
                stackVariant.Obj is not Godot.Collections.Array stackArray)
            {
                return false;
            }

            var presets = new List<string>();
            foreach (var entry in stackArray)
            {
                if (entry.VariantType == Variant.Type.String)
                {
                    presets.Add((string)entry);
                }
            }

            presetStack = presets;
            return presets.Count > 0;
        }

        /// <summary>
        /// Loads the design configuration from JSON.
        /// </summary>
        /// <returns>Fully parsed configuration dictionary.</returns>
        private static Godot.Collections.Dictionary<string, Variant> LoadDesignConfig()
        {
            try
            {
                var jsonContent = Godot.FileAccess.GetFileAsString(_DesignConfigPath);
                var json = new Json();
                var parseError = json.Parse(jsonContent);
                if (parseError != Error.Ok)
                {
                    GD.PrintErr($"[DesignConfigService] Failed to parse design config: {json.GetErrorMessage()}");
                    return new Godot.Collections.Dictionary<string, Variant>();
                }

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
        /// Applies design-system shader parameters to every <see cref="ShaderMaterial"/> found inside the node tree.
        /// </summary>
        /// <param name="rootNode">Optional root; defaults to the scene-tree root.</param>
        public static void ApplyDesignColorsToAllShaders(Node? rootNode = null)
        {
            rootNode ??= Engine.GetMainLoop() is SceneTree tree ? tree.Root : null;
            if (rootNode == null)
            {
                GD.PrintErr("[DesignConfigService] Cannot apply colors: root node is null.");
                return;
            }

            if (DesignConfig.Count == 0)
            {
                GD.PrintErr("[DesignConfigService] Design config is empty. Cannot apply colors.");
                return;
            }

            var updated = 0;
            TraverseAndApplyColors(rootNode, ref updated);
            GD.Print($"[DesignConfigService] Applied design colors to {updated} shader materials.");
        }

        private static void TraverseAndApplyColors(Node node, ref int updated)
        {
            if (node is CanvasItem canvasItem && canvasItem.Material is ShaderMaterial material)
            {
                if (ApplyShaderParameters(material))
                {
                    updated++;
                }
            }

            foreach (Node child in node.GetChildren())
            {
                TraverseAndApplyColors(child, ref updated);
            }
        }

        private static bool ApplyShaderParameters(ShaderMaterial material)
        {
            try
            {
                var shader = material.Shader;
                if (shader == null || string.IsNullOrEmpty(shader.ResourcePath))
                {
                    return false;
                }

                if (!_ShaderResourceToPresetMap.TryGetValue(shader.ResourcePath, out var presetKey))
                {
                    return false;
                }

                if (!TryGetShaderPreset(presetKey, out var preset))
                {
                    return false;
                }

                foreach (var parameter in preset.Parameters)
                {
                    material.SetShaderParameter(parameter.Key, parameter.Value);
                }

                return true;
            }
            catch (Exception ex)
            {
                GD.PrintErr($"[DesignConfigService] Error applying shader parameters: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Gets a design color using a dotted path (e.g., <c>design_system.deep_space</c>).
        /// </summary>
        /// <param name="colorPath">Dotted path to color dictionary.</param>
        /// <returns>The resolved color or <see cref="Colors.White"/> when not found.</returns>
        public static Color GetDesignColor(string colorPath)
        {
            try
            {
                if (TryGetDesignColor(colorPath, out var color))
                {
                    return color;
                }
            }
            catch (Exception ex)
            {
                GD.PrintErr($"[DesignConfigService] Error getting design color '{colorPath}': {ex.Message}");
            }

            return Colors.White;
        }

        private static bool TryGetDesignColor(string colorPath, out Color color)
        {
            var parts = colorPath.Split('.', StringSplitOptions.RemoveEmptyEntries);
            Variant current = DesignConfig;

            foreach (var part in parts)
            {
            if (current.Obj is not Godot.Collections.Dictionary<string, Variant> dict || !dict.TryGetValue(part, out var next))
            {
                color = Colors.White;
                return false;
            }

                current = next;
            }

            if (current.Obj is Godot.Collections.Dictionary<string, Variant> colorDict && TryParseColor(colorDict, out color))
            {
                return true;
            }

            color = Colors.White;
            return false;
        }

        private static bool TryParseColor(Godot.Collections.Dictionary<string, Variant> colorDict, out Color color)
        {
            var r = colorDict.TryGetValue("r", out var rVar) ? (float)rVar : 1f;
            var g = colorDict.TryGetValue("g", out var gVar) ? (float)gVar : 1f;
            var b = colorDict.TryGetValue("b", out var bVar) ? (float)bVar : 1f;
            var a = colorDict.TryGetValue("a", out var aVar) ? (float)aVar : 1f;

            color = new Color(r, g, b, a);
            return true;
        }

        private static bool TryGetSection(string sectionKey, out Godot.Collections.Dictionary<string, Variant> section)
        {
            if (DesignConfig.TryGetValue(sectionKey, out var sectionVariant) &&
                sectionVariant.Obj is Godot.Collections.Dictionary<string, Variant> sectionDict)
            {
                section = sectionDict;
                return true;
            }

            section = new Godot.Collections.Dictionary<string, Variant>();
            return false;
        }

        private static bool TryResolveParameterVariant(Variant valueVariant, out Variant resolved)
        {
            resolved = default;

            if (valueVariant.Obj is Godot.Collections.Dictionary<string, Variant> nestedDict)
            {
                if (nestedDict.TryGetValue("value", out var innerValue))
                {
                    return TryResolveParameterVariant(innerValue, out resolved);
                }

                if (nestedDict.TryGetValue("color_ref", out var colorRef) && colorRef.VariantType == Variant.Type.String)
                {
                    var color = GetDesignColor((string)colorRef);
                    resolved = color;
                    return true;
                }

                if (nestedDict.ContainsKey("r") && nestedDict.ContainsKey("g") && nestedDict.ContainsKey("b"))
                {
                    var color = TryParseColor(nestedDict, out var parsedColor) ? parsedColor : Colors.White;
                    resolved = color;
                    return true;
                }

                if (nestedDict.ContainsKey("x") && nestedDict.ContainsKey("y") && nestedDict.ContainsKey("z"))
                {
                    var x = (float)nestedDict["x"];
                    var y = (float)nestedDict["y"];
                    var z = (float)nestedDict["z"];
                    resolved = new Vector3(x, y, z);
                    return true;
                }

                if (nestedDict.TryGetValue("scalar", out var scalarValue))
                {
                    return TryResolveParameterVariant(scalarValue, out resolved);
                }
            }

            switch (valueVariant.VariantType)
            {
                case Variant.Type.Float:
                case Variant.Type.Int:
                case Variant.Type.String:
                case Variant.Type.Bool:
                    resolved = valueVariant;
                    return true;
                default:
                    if (valueVariant.Obj is Color colorValue)
                    {
                        resolved = colorValue;
                        return true;
                    }

                    if (valueVariant.Obj is Vector3 vector)
                    {
                        resolved = vector;
                        return true;
                    }

                    return false;
            }
        }

        /// <summary>
        /// Represents a shader preset resolved from the design configuration.
        /// </summary>
        public readonly struct DesignShaderPreset
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="DesignShaderPreset"/> struct.
            /// </summary>
            /// <param name="shaderPath">Shader resource path or <see langword="null"/> for pass-through.</param>
            /// <param name="parameters">Shader parameter dictionary.</param>
            public DesignShaderPreset(string? shaderPath, System.Collections.Generic.Dictionary<string, Variant> parameters)
            {
                ShaderPath = shaderPath;
                Parameters = parameters;
            }

            /// <summary>
            /// Gets the shader resource path (may be null when preset removes shader).
            /// </summary>
            public string? ShaderPath { get; }

            /// <summary>
            /// Gets the resolved shader parameters to apply.
            /// </summary>
            public System.Collections.Generic.Dictionary<string, Variant> Parameters { get; }
        }
    }
}
