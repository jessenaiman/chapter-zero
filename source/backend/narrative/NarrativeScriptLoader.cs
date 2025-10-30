// <copyright file="NarrativeScriptLoader.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using Godot;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace OmegaSpiral.Source.Backend.Narrative;

/// <summary>
/// Generic YAML narrative script loader.
/// Provides stage-agnostic deserialization of YAML narrative files into <see cref="NarrativeScriptRoot"/> instances.
/// </summary>
public static class NarrativeScriptLoader
{
    /// <summary>
    /// Loads a YAML narrative script from the specified file path.
    /// </summary>
    /// <param name="yamlFilePath">The Godot resource path to the YAML file (e.g., "res://source/stages/stage_1_ghost/ghost.yaml").</param>
    /// <returns>A deserialized <see cref="NarrativeScriptRoot"/> instance.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the file cannot be opened or read.</exception>
    public static NarrativeScriptRoot LoadYamlScript(string yamlFilePath)
    {
        using var file = Godot.FileAccess.Open(yamlFilePath, Godot.FileAccess.ModeFlags.Read);
        if (file == null)
        {
            throw new InvalidOperationException($"Failed to open {yamlFilePath}");
        }

        var yamlContent = file.GetAsText();

        var deserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .IgnoreUnmatchedProperties()
            .Build();

        try
        {
            return deserializer.Deserialize<NarrativeScriptRoot>(yamlContent);
        }
        catch (YamlDotNet.Core.YamlException ex)
        {
            GD.PrintErr($"YAML deserialization failed for {yamlFilePath}: {ex.Message}");
            if (ex.InnerException != null)
            {
                GD.PrintErr($"Inner error: {ex.InnerException.Message}");
            }
            throw;
        }
    }

    /// <summary>
    /// Loads a YAML narrative script with stage-specific extensions.
    /// </summary>
    /// <typeparam name="T">The stage-specific script type that extends <see cref="NarrativeScriptRoot"/>.</typeparam>
    /// <param name="yamlFilePath">The Godot resource path to the YAML file.</param>
    /// <returns>A deserialized instance of the stage-specific script type.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the file cannot be opened or read.</exception>
    public static T LoadYamlScript<T>(string yamlFilePath) where T : NarrativeScriptRoot
    {
        using var file = Godot.FileAccess.Open(yamlFilePath, Godot.FileAccess.ModeFlags.Read);
        if (file == null)
        {
            throw new InvalidOperationException($"Failed to open {yamlFilePath}");
        }

        var yamlContent = file.GetAsText();

        var deserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .IgnoreUnmatchedProperties()
            .Build();

        try
        {
            return deserializer.Deserialize<T>(yamlContent);
        }
        catch (YamlDotNet.Core.YamlException ex)
        {
            GD.PrintErr($"YAML deserialization failed for {yamlFilePath}: {ex.Message}");
            if (ex.InnerException != null)
            {
                GD.PrintErr($"Inner error: {ex.InnerException.Message}");
            }
            throw;
        }
    }
}
