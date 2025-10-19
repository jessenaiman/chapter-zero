// <copyright file="NarrativeAssetMapper.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using System.Collections.Generic;
using Godot;

namespace OmegaSpiral.Source.Narrative.asset_management;
/// <summary>
/// Maps and manages unused assets for the narrative system, providing easy access to available resources.
/// This system catalogs unused audio, visual, and other assets that can be utilized in narrative sequences.
/// </summary>
[GlobalClass]
public partial class NarrativeAssetMapper : Node
{
    /// <summary>
    /// Dictionary mapping asset names to their resource paths.
    /// </summary>
    private readonly Dictionary<string, string> assetMap = new();

    /// <summary>
    /// Dictionary mapping asset types to their available resources.
    /// </summary>
    private readonly Dictionary<AssetType, List<string>> assetTypeMap = new();

    /// <summary>
    /// Gets a value indicating whether the asset mapper has been initialized.
    /// </summary>
    public bool IsInitialized { get; private set; }

    /// <inheritdoc/>
    public override void _Ready()
    {
        this.InitializeAssetMaps();
        this.IsInitialized = true;
    }

    /// <summary>
    /// Initializes the asset maps with available resources from the Source/assets directory.
    /// </summary>
    private void InitializeAssetMaps()
    {
        // Music assets
        this.AddAsset("apple_cider", "res://source/assets/music/Apple Cider.mp3", AssetType.Music);
        this.AddAsset("insect_factory", "res://source/assets/music/Insect Factory.mp3", AssetType.Music);
        this.AddAsset("squashin_bugs", "res://source/assets/music/squashin_bugs_fixed.mp3", AssetType.Music);
        this.AddAsset("fun_run", "res://source/assets/music/the_fun_run.mp3", AssetType.Music);

        // Sound effect assets
        this.AddAsset("chop_sfx", "res://source/assets/sfx/chop.ogg", AssetType.SoundEffect);
        this.AddAsset("confirmation_sfx", "res://source/assets/sfx/confirmation_002.ogg", AssetType.SoundEffect);
        this.AddAsset("door_close_1", "res://source/assets/sfx/doorClose_1.ogg", AssetType.SoundEffect);
        this.AddAsset("door_close_4", "res://source/assets/sfx/doorClose_4.ogg", AssetType.SoundEffect);
        this.AddAsset("door_open_2", "res://source/assets/sfx/doorOpen_2.ogg", AssetType.SoundEffect);
        this.AddAsset("drop_sfx", "res://source/assets/sfx/drop_002.ogg", AssetType.SoundEffect);
        this.AddAsset("error_sfx", "res://source/assets/sfx/error_006.ogg", AssetType.SoundEffect);
        this.AddAsset("impact_wood_1", "res://Source/assets/sfx/impactWood_light_002.ogg", AssetType.SoundEffect);
        this.AddAsset("impact_wood_2", "res://Source/assets/sfx/impactWood_light_003.ogg", AssetType.SoundEffect);

        // GUI assets
        this.AddAsset("logo_panels", "res://Source/assets/gui/logo-panels.png", AssetType.Texture);
        this.AddAsset("turn_bar_bg", "res://Source/assets/gui/combat/turn_bar_bg.png", AssetType.Texture);

        // Item assets
        this.AddAsset("bomb_texture", "res://Source/assets/items/bomb.atlastex", AssetType.Texture);
        this.AddAsset("coin_texture", "res://Source/assets/items/coin.atlastex", AssetType.Texture);
        this.AddAsset("key_texture", "res://Source/assets/items/key.atlastex", AssetType.Texture);
        this.AddAsset("wand_blue", "res://Source/assets/items/wand_blue.atlastex", AssetType.Texture);
        this.AddAsset("wand_green", "res://Source/assets/items/wand_green.atlastex", AssetType.Texture);
        this.AddAsset("wand_red", "res://Source/assets/items/wand_red.atlastex", AssetType.Texture);

        // Tileset assets
        this.AddAsset("dungeon_tilemap", "res://Source/assets/tilesets/dungeon_tilemap.png", AssetType.Texture);
        this.AddAsset("town_tilemap", "res://Source/assets/tilesets/town_tilemap.png", AssetType.Texture);

        // Font assets
        this.AddAsset("kenney_pixel_font", "res://Source/assets/gui/font/Kenney Pixel.ttf", AssetType.Font);
        this.AddAsset("source_code_font", "res://Source/assets/gui/font/SourceCodePro-Bold.ttf", AssetType.Font);
        this.AddAsset("terminal_font", "res://Source/assets/gui/font/terminal_font.tres", AssetType.Font);

        // Icon assets
        this.AddAsset("app_icon", "res://Source/assets/gui/icons/app_icon.png", AssetType.Texture);
        this.AddAsset("emote_default", "res://Source/assets/gui/emotes/emote__.png", AssetType.Texture);
        this.AddAsset("emote_combat", "res://Source/assets/gui/emotes/emote_combat.png", AssetType.Texture);
        this.AddAsset("emote_exclamations", "res://Source/assets/gui/emotes/emote_exclamations.png", AssetType.Texture);
        this.AddAsset("emote_question", "res://Source/assets/gui/emotes/emote_question.png", AssetType.Texture);
    }

    /// <summary>
    /// Adds an asset to the mapping system.
    /// </summary>
    /// <param name="name">The name identifier for the asset.</param>
    /// <param name="path">The resource path for the asset.</param>
    /// <param name="type">The type of asset.</param>
    private void AddAsset(string name, string path, AssetType type)
    {
        this.assetMap[name] = path;

        if (!this.assetTypeMap.TryGetValue(type, out var list))
        {
            list = new List<string>();
            this.assetTypeMap[type] = list;
        }

        list.Add(name);
    }

    /// <summary>
    /// Gets an asset path by its name.
    /// </summary>
    /// <param name="name">The name identifier for the asset.</param>
    /// <returns>The asset path, or null if not found.</returns>
    public string? GetAssetPath(string name)
    {
        return this.assetMap.TryGetValue(name, out string? path) ? path : null;
    }

    /// <summary>
    /// Gets an asset resource by its name.
    /// </summary>
    /// <param name="name">The name identifier for the asset.</param>
    /// <typeparam name="T">The type of resource to load.</typeparam>
    /// <returns>The loaded asset resource, or null if not found or load fails.</returns>
    public T? GetAsset<T>(string name) where T : class
    {
        string? path = this.GetAssetPath(name);
        if (path == null)
        {
            return null;
        }

        try
        {
            var resource = ResourceLoader.Load(path);
            return resource as T;
        }
        catch (Exception ex)
        {
            GD.PrintErr($"Failed to load asset '{name}' from path '{path}': {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// Gets all assets of a specific type.
    /// </summary>
    /// <param name="type">The type of assets to retrieve.</param>
    /// <returns>A list of asset names of the specified type.</returns>
    public List<string> GetAssetsByType(AssetType type)
    {
        if (this.assetTypeMap.TryGetValue(type, out List<string>? assets))
        {
            return new List<string>(assets);
        }

        return new List<string>();
    }

    /// <summary>
    /// Gets all available asset names.
    /// </summary>
    /// <returns>A list of all asset names.</returns>
    public List<string> GetAllAssetNames()
    {
        return new List<string>(this.assetMap.Keys);
    }

    /// <summary>
    /// Checks if an asset exists by name.
    /// </summary>
    /// <param name="name">The name identifier for the asset.</param>
    /// <returns>True if the asset exists, false otherwise.</returns>
    public bool AssetExists(string name)
    {
        return this.assetMap.ContainsKey(name);
    }

    /// <summary>
    /// Gets the type of an asset by its name.
    /// </summary>
    /// <param name="name">The name identifier for the asset.</param>
    /// <returns>The asset type, or AssetType.Unknown if not found.</returns>
    public AssetType GetAssetType(string name)
    {
        foreach (var kvp in this.assetTypeMap)
        {
            if (kvp.Value.Contains(name))
            {
                return kvp.Key;
            }
        }

        return AssetType.Unknown;
    }
}

/// <summary>
/// Enumerates the types of assets available in the narrative system.
/// </summary>
public enum AssetType
{
    /// <summary>
    /// Unknown or undefined asset type.
    /// </summary>
    Unknown,

    /// <summary>
    /// Audio stream for music tracks.
    /// </summary>
    Music,

    /// <summary>
    /// Audio stream for sound effects.
    /// </summary>
    SoundEffect,

    /// <summary>
    /// Texture or image resources.
    /// </summary>
    Texture,

    /// <summary>
    /// Font resources.
    /// </summary>
    Font,

    /// <summary>
    /// Animation resources.
    /// </summary>
    Animation,

    /// <summary>
    /// Scene resources.
    /// </summary>
    Scene,
}
