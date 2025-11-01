// <copyright file="StoryCombatEncounter.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using System.Collections.Generic;
using Godot;
using Newtonsoft.Json;

namespace OmegaSpiral.Source.Backend.Narrative;

/// <summary>
/// Simple flat-file story combat encounter data.
/// Contains all encounter details in one file - the throwback approach.
/// </summary>
public class StoryCombatEncounter
{
    /// <summary>
    /// Gets or sets the encounter ID.
    /// </summary>
    [JsonProperty("id")]
    public string? Id { get; set; }

    /// <summary>
    /// Gets or sets the Dreamweaver owner associated with this encounter.
    /// Influences narrative style and AI-driven story generation.
    /// </summary>
    [JsonProperty("owner")]
    public string? Owner { get; set; }

    /// <summary>
    /// Gets or sets the encounter title.
    /// </summary>
    [JsonProperty("title")]
    public string? Title { get; set; }

    /// <summary>
    /// Gets or sets the encounter description.
    /// </summary>
    [JsonProperty("description")]
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the enemy data.
    /// </summary>
    [JsonProperty("enemy")]
    public CombatEnemyData? Enemy { get; set; }

    /// <summary>
    /// Gets or sets the available player actions.
    /// </summary>
    [JsonProperty("actions")]
    public List<string>? Actions { get; set; }

    /// <summary>
    /// Gets or sets the background music path.
    /// </summary>
    [JsonProperty("music")]
    public string? Music { get; set; }

    /// <summary>
    /// Gets or sets the victory message.
    /// </summary>
    [JsonProperty("victoryText")]
    public string? VictoryText { get; set; }

    /// <summary>
    /// Gets or sets the defeat message.
    /// </summary>
    [JsonProperty("defeatText")]
    public string? DefeatText { get; set; }
}

/// <summary>
/// Simple enemy data for flat-file encounters.
/// </summary>
public class CombatEnemyData
{
    /// <summary>
    /// Gets or sets the enemy name.
    /// </summary>
    [JsonProperty("name")]
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets the enemy sprite path.
    /// </summary>
    [JsonProperty("sprite")]
    public string? Sprite { get; set; }

    /// <summary>
    /// Gets or sets the enemy hit points.
    /// </summary>
    [JsonProperty("hp")]
    public int Hp { get; set; }

    /// <summary>
    /// Gets or sets the enemy max hit points.
    /// </summary>
    [JsonProperty("maxHp")]
    public int MaxHp { get; set; }

    /// <summary>
    /// Gets or sets the enemy attack power.
    /// </summary>
    [JsonProperty("attack")]
    public int Attack { get; set; }

    /// <summary>
    /// Gets or sets the enemy defense.
    /// </summary>
    [JsonProperty("defense")]
    public int Defense { get; set; }

    /// <summary>
    /// Gets or sets the enemy attack patterns.
    /// </summary>
    [JsonProperty("attackPatterns")]
    public List<string>? AttackPatterns { get; set; }
}
