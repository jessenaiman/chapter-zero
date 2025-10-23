// <copyright file="Stage2NarrativeData.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Text.Json.Serialization;

#pragma warning disable CA1002 // List<T> is required for JSON serialization

namespace OmegaSpiral.Source.Stages.Stage2;

/// <summary>
/// Root data structure for Stage 2's Echo Chamber experience.
/// Mirrors the stage_2.json schema for roguelike chamber exploration.
/// </summary>
public class Stage2NarrativeData
{
    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    [JsonPropertyName("metadata")]
    public EchoChamberMetadata Metadata { get; set; } = new();

    [JsonPropertyName("dreamweavers")]
    public List<DreamweaverData> Dreamweavers { get; set; } = new();

    [JsonPropertyName("interludes")]
    public List<InterludeData> Interludes { get; set; } = new();

    [JsonPropertyName("chambers")]
    public List<ChamberData> Chambers { get; set; } = new();

    [JsonPropertyName("finale")]
    public FinaleData Finale { get; set; } = new();
}

/// <summary>
/// Metadata about the Echo Chamber stage iteration and status.
/// </summary>
public class EchoChamberMetadata
{
    [JsonPropertyName("iteration")]
    public string Iteration { get; set; } = string.Empty;

    [JsonPropertyName("iterationFallback")]
    public int IterationFallback { get; set; }

    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;

    [JsonPropertyName("systemIntro")]
    public List<string> SystemIntro { get; set; } = new();
}

/// <summary>
/// Configuration for one of the three Dreamweavers.
/// </summary>
public class DreamweaverData
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("accentColor")]
    public string AccentColor { get; set; } = string.Empty;

    [JsonPropertyName("textTheme")]
    public string TextTheme { get; set; } = string.Empty;
}

/// <summary>
/// An interlude sequence between chambers (dialogue and choice).
/// </summary>
public class InterludeData
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("owner")]
    public string Owner { get; set; } = string.Empty;

    [JsonPropertyName("prompt")]
    public string Prompt { get; set; } = string.Empty;

    [JsonPropertyName("options")]
    public List<InterludeOption> Options { get; set; } = new();
}

/// <summary>
/// A choice option within an interlude, with alignment and banter.
/// </summary>
public class InterludeOption
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("text")]
    public string Text { get; set; } = string.Empty;

    [JsonPropertyName("alignment")]
    public string Alignment { get; set; } = string.Empty;

    [JsonPropertyName("banter")]
    public BanterBlock Banter { get; set; } = new();
}

/// <summary>
/// A chamber dungeon layout owned by one Dreamweaver.
/// Contains set pieces (door, monster, chest) and decoys.
/// </summary>
public class ChamberData
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("owner")]
    public string Owner { get; set; } = string.Empty;

    [JsonPropertyName("style")]
    public ChamberStyle Style { get; set; } = new();

    [JsonPropertyName("objects")]
    public List<ChamberObject> Objects { get; set; } = new();

    [JsonPropertyName("decoys")]
    public List<DecoyData> Decoys { get; set; } = new();
}

/// <summary>
/// Visual and audio styling configuration for a chamber.
/// </summary>
public class ChamberStyle
{
    [JsonPropertyName("template")]
    public string Template { get; set; } = string.Empty;

    [JsonPropertyName("ambient")]
    public string Ambient { get; set; } = string.Empty;

    [JsonPropertyName("decoyCount")]
    public int DecoyCount { get; set; }

    [JsonPropertyName("glitchProfile")]
    public string GlitchProfile { get; set; } = string.Empty;
}

/// <summary>
/// A set piece object in a chamber slot (door, monster, chest).
/// </summary>
public class ChamberObject
{
    [JsonPropertyName("slot")]
    public string Slot { get; set; } = string.Empty;

    [JsonPropertyName("alignment")]
    public string Alignment { get; set; } = string.Empty;

    [JsonPropertyName("prompt")]
    public string Prompt { get; set; } = string.Empty;

    [JsonPropertyName("interactionLog")]
    public List<string> InteractionLog { get; set; } = new();

    [JsonPropertyName("banter")]
    public BanterBlock Banter { get; set; } = new();
}

/// <summary>
/// A fake/decoy object that reveals when investigated.
/// </summary>
public class DecoyData
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("revealText")]
    public string RevealText { get; set; } = string.Empty;
}

/// <summary>
/// Banter reaction from Dreamweavers: approval or dissent.
/// </summary>
public class BanterBlock
{
    [JsonPropertyName("approve")]
    public BanterLine Approve { get; set; } = new();

    [JsonPropertyName("dissent")]
    public List<BanterLine> Dissent { get; set; } = new();
}

/// <summary>
/// A single line of banter spoken by a Dreamweaver.
/// </summary>
public class BanterLine
{
    [JsonPropertyName("speaker")]
    public string Speaker { get; set; } = string.Empty;

    [JsonPropertyName("line")]
    public string Line { get; set; } = string.Empty;
}

/// <summary>
/// Finale data: which Dreamweaver claims the shard and accompanying banter.
/// </summary>
public class FinaleData
{
    [JsonPropertyName("claimants")]
    public FinaleClaimants Claimants { get; set; } = new();

    [JsonPropertyName("systemOutro")]
    public string SystemOutro { get; set; } = string.Empty;
}

/// <summary>
/// Claims and responses for each Dreamweaver in the finale.
/// </summary>
public class FinaleClaimants
{
    [JsonPropertyName("light")]
    public FinaleClaimant Light { get; set; } = new();

    [JsonPropertyName("shadow")]
    public FinaleClaimant Shadow { get; set; } = new();

    [JsonPropertyName("ambition")]
    public FinaleClaimant Ambition { get; set; } = new();
}

/// <summary>
/// A single Dreamweaver's claim in the finale, with responses from the other two.
/// </summary>
public class FinaleClaimant
{
    [JsonPropertyName("claim")]
    public string Claim { get; set; } = string.Empty;

    [JsonPropertyName("responses")]
    public List<BanterLine> Responses { get; set; } = new();
}
