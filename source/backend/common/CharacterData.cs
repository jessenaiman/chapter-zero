// <copyright file="CharacterData.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

namespace OmegaSpiral.Source.Backend.Common;

using Godot;
using Godot.Collections;

/// <summary>
/// Data transfer object for character metadata used in party selection and mirror encounters.
/// Bridges between character configuration and in-game Character instances.
/// </summary>
/// <param name="Id">Unique character identifier (e.g., "fighter", "wizard").</param>
/// <param name="Name">Display name of the character.</param>
/// <param name="Description">Flavor text describing the character's role.</param>
/// <param name="Affinity">Dreamweaver affinity tag (e.g., "light", "mischief").</param>
public sealed record CharacterData(
    string Id,
    string Name,
    string Description,
    string Affinity)
{
    /// <summary>
    /// Constructs a CharacterData from a Godot Dictionary (typically from scene exports or configs).
    /// </summary>
    /// <param name="dict">Dictionary with keys: "id", "name", "description", "affinity".</param>
    /// <returns>A new CharacterData instance populated from the dictionary.</returns>
    public static CharacterData FromDictionary(Dictionary<string, Variant> dict)
    {
        return new CharacterData(
            Id: dict.TryGetValue("id", out var idVar) ? (string) idVar : "unknown",
            Name: dict.TryGetValue("name", out var nameVar) ? (string) nameVar : "Unknown",
            Description: dict.TryGetValue("description", out var descVar) ? (string) descVar : "",
            Affinity: dict.TryGetValue("affinity", out var affVar) ? (string) affVar : "neutral");
    }

    /// <summary>
    /// Converts CharacterData to a full Character instance for gameplay.
    /// </summary>
    /// <returns>A Character with basic stats initialized.</returns>
    public Character ToCharacter()
    {
        return new Character(
            id: this.Id,
            name: this.Name,
            characterClass: this.Name, // Use name as class for now
            health: 100,
            maxHealth: 100,
            experience: 0,
            level: 1);
    }
}
