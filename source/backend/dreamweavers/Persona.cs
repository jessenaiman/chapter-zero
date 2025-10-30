// <copyright file="Persona.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using Godot;
using Newtonsoft.Json;

namespace OmegaSpiral.Source.Backend.Dreamweavers;

/// <summary>
/// Represents a lightweight Dreamweaver persona with AI personality traits.
/// Personas are loaded from JSON files and hold only essential data.
/// </summary>
public sealed class Persona
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Persona"/> class.
    /// </summary>
    /// <param name="personaId">The unique identifier ("hero", "shadow", "ambition").</param>
    /// <param name="name">The display name of the persona.</param>
    /// <param name="archetype">The personality archetype/type.</param>
    /// <param name="systemPrompt">The AI system prompt that defines this persona's behavior.</param>
    private Persona(string personaId, string name, string archetype, string systemPrompt)
    {
        this.PersonaId = personaId;
        this.Name = name;
        this.Archetype = archetype;
        this.SystemPrompt = systemPrompt;
    }

    /// <summary>
    /// Gets the unique identifier for this persona.
    /// </summary>
    public string PersonaId { get; }

    /// <summary>
    /// Gets the display name of this persona.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the personality archetype or type.
    /// </summary>
    public string Archetype { get; }

    /// <summary>
    /// Gets the AI system prompt that defines this persona's behavior and personality.
    /// </summary>
    public string SystemPrompt { get; }

    /// <summary>
    /// Loads a persona from its JSON configuration file.
    /// </summary>
    /// <param name="personaId">The persona identifier ("hero", "shadow", "ambition").</param>
    /// <returns>The loaded persona, or null if the file doesn't exist or fails to load.</returns>
    public static Persona? LoadFromJson(string personaId)
    {
        var path = $"res://source/data/personas/{personaId}.json";

        if (!Godot.FileAccess.FileExists(path))
        {
            GD.PrintErr($"Persona config not found: {path}");
            return null;
        }

        try
        {
            using var file = Godot.FileAccess.Open(path, Godot.FileAccess.ModeFlags.Read);
            if (file == null)
            {
                GD.PrintErr($"Failed to open persona config: {path}");
                return null;
            }

            var jsonContent = file.GetAsText();
            var jsonData = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonContent);
            if (jsonData == null)
            {
                GD.PrintErr($"Failed to deserialize persona config for {personaId}");
                return null;
            }

            var id = jsonData.GetValueOrDefault("personaId", personaId);
            var name = jsonData.GetValueOrDefault("name", "Unknown");
            var archetype = jsonData.GetValueOrDefault("archetype", "neutral");
            var systemPrompt = jsonData.GetValueOrDefault("systemPrompt", "You are a narrative guide.");

            return new Persona(id, name, archetype, systemPrompt);
        }
        catch (Exception ex)
        {
            GD.PrintErr($"Failed to load persona {personaId}: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// Gets a persona by its ID. Loads it from JSON on first access (lazy initialization).
    /// </summary>
    /// <param name="personaId">The persona identifier ("hero", "shadow", "ambition").</param>
    /// <returns>The persona, or null if the ID is not recognized or fails to load.</returns>
    public static Persona? GetPersona(string personaId) => personaId switch
    {
        "hero" or "shadow" or "ambition" => LoadFromJson(personaId),
        _ => null,
    };
}
