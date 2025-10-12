// <copyright file="Inventory.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

/// <summary>
/// A simple inventory implementation that includes all item types and data within the class.
/// </summary>
[Tool]
public partial class Inventory : Resource
{
    /// <summary>
    /// All item types available to add or remove from the inventory.
    /// </summary>
    public enum ItemTypes
    {
        Key,
        Coin,
        Bomb,
        RedWand,
        BlueWand,
        GreenWand,
    }

    // TODO: I expect we'll want to have a proper inventory definition somewhere. Some folks advocate for
    // spreadsheets, but whatever it is should probably integrate with the editor so that level designers
    // can easily pick from items from a dropdown list, or something similar.

    /// <summary>
    /// Icons associated with the <see cref="ItemTypes"/>.
    /// </summary>
    private static readonly Dictionary<ItemTypes, AtlasTexture> Icons = new Dictionary<ItemTypes, AtlasTexture>
    {
        { ItemTypes.Key, GD.Load<AtlasTexture>("res://assets/items/key.atlastex") },
        { ItemTypes.Coin, GD.Load<AtlasTexture>("res://assets/items/coin.atlastex") },
        { ItemTypes.Bomb, GD.Load<AtlasTexture>("res://assets/items/bomb.atlastex") },
        { ItemTypes.RedWand, GD.Load<AtlasTexture>("res://assets/items/wand_red.atlastex") },
        { ItemTypes.BlueWand, GD.Load<AtlasTexture>("res://assets/items/wand_blue.atlastex") },
        { ItemTypes.GreenWand, GD.Load<AtlasTexture>("res://assets/items/wand_green.atlastex") },
    };

    private const string InventoryPath = "user://inventory.tres";

    /// <summary>
    /// Emitted when the count of a given item type changes.
    /// </summary>
    [Signal]
    public delegate void ItemChangedEventHandler(ItemTypes type);

    // Keep track of what is in the inventory. Dictionary keys are an ItemType, values are the amount.
    private Dictionary<ItemTypes, int> items = new Dictionary<ItemTypes, int>();

    public Inventory()
    {
        foreach (ItemTypes itemType in Enum.GetValues(typeof(ItemTypes)))
        {
            this.items[itemType] = 0;
        }
    }

    /// <summary>
    /// Load the <see cref="Inventory"/> from file or create a new resource, if it was missing. Godot caches calls,
    /// so this can be used every time needed.
    /// </summary>
    /// <returns></returns>
    public static Inventory Restore()
    {
        if (Engine.IsEditorHint())
        {
            return null;
        }

        if (FileAccess.FileExists(InventoryPath))
        {
            var inventory = ResourceLoader.Load(InventoryPath) as Inventory;
            if (inventory != null)
            {
                return inventory;
            }
        }

        // Either there is no inventory associated with this profile or the file itself could not be
        // loaded. Either way, a new inventory resource must be created.
        var newInventory = new Inventory();
        newInventory.Save();
        return newInventory;
    }

    /// <summary>
    /// Increment the count of a given item by one, adding it to the inventory if it does not exist.
    /// </summary>
    public void Add(ItemTypes itemType, int amount = 1)
    {
        // Note that adding negative numbers is possible. Prevent having a total of negative items.
        // NPC: "You cannot have negative potatoes."
        int oldAmount = this.items.GetValueOrDefault(itemType, 0);
        this.items[itemType] = Math.Max(oldAmount + amount, 0);

        EmitSignal(SignalName.ItemChanged, itemType);
    }

    /// <summary>
    /// Decrement the count of a given item by one.
    /// The item will be removed entirely if there are none remaining. Removing an item that is not
    /// possessed will do nothing.
    /// </summary>
    public void Remove(ItemTypes itemType, int amount = 1)
    {
        this.Add(itemType, -amount);
    }

    /// <summary>
    /// Returns the number of a certain item type possessed by the player.
    /// </summary>
    /// <returns></returns>
    public int GetItemCount(ItemTypes itemType)
    {
        return this.items.GetValueOrDefault(itemType, 0);
    }

    /// <summary>
    /// Returns the icon associated with a given item type.
    /// </summary>
    /// <returns></returns>
    public static Texture2D GetItemIcon(ItemTypes itemType)
    {
        return Icons.GetValueOrDefault(itemType, null);
    }

    /// <summary>
    /// Write the inventory contents to the disk.
    /// </summary>
    public void Save()
    {
        ResourceSaver.Save(this, InventoryPath);
    }
}
