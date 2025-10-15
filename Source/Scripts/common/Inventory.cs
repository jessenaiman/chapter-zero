// <copyright file="Inventory.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
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
    private const string InventoryPath = "user://inventory.tres";

    /// <summary>
    /// Icons associated with the <see cref="ItemType"/>.
    /// </summary>
    private static readonly Dictionary<ItemType, AtlasTexture> Icons = new Dictionary<ItemType, AtlasTexture>
    {
        { ItemType.Key, GD.Load<AtlasTexture>("res://assets/items/key.atlastex") },
        { ItemType.Coin, GD.Load<AtlasTexture>("res://assets/items/coin.atlastex") },
        { ItemType.Bomb, GD.Load<AtlasTexture>("res://assets/items/bomb.atlastex") },
        { ItemType.RedWand, GD.Load<AtlasTexture>("res://assets/items/wand_red.atlastex") },
        { ItemType.BlueWand, GD.Load<AtlasTexture>("res://assets/items/wand_blue.atlastex") },
        { ItemType.GreenWand, GD.Load<AtlasTexture>("res://assets/items/wand_green.atlastex") },
    };

    /// <summary>
    /// Keep track of what is in the inventory. Dictionary keys are an ItemType, values are the amount.
    /// </summary>
    private Dictionary<ItemType, int> items = new Dictionary<ItemType, int>();

    /// <summary>
    /// Initializes a new instance of the <see cref="Inventory"/> class.
    /// </summary>
    public Inventory()
    {
        foreach (ItemType itemType in Enum.GetValues(typeof(ItemType)))
        {
            this.items[itemType] = 0;
        }
    }

    /// <summary>
    /// Emitted when the count of a given item type changes.
    /// </summary>
    /// <param name="type">The item type that changed.</param>
    [Signal]
    public delegate void ItemChangedEventHandler(ItemType type);

    /// <summary>
    /// All item types available to add or remove from the inventory.
    /// </summary>
    /// <summary>
    /// Represents the types of items that can be stored in the inventory.
    /// </summary>
    public enum ItemType
    {
        /// <summary>
        /// A key item, typically used to unlock doors or chests.
        /// </summary>
        Key = 0,

        /// <summary>
        /// A coin item, representing currency or collectible value.
        /// </summary>
        Coin = 1,

        /// <summary>
        /// A bomb item, used for explosive actions or puzzles.
        /// </summary>
        Bomb = 2,

        /// <summary>
        /// A red wand item, representing a magical tool with specific abilities.
        /// </summary>
        RedWand = 3,

        /// <summary>
        /// A blue wand item, representing a magical tool with specific abilities.
        /// </summary>
        BlueWand = 4,

        /// <summary>
        /// A green wand item, representing a magical tool with specific abilities.
        /// </summary>
        GreenWand = 5,
    }

    /// <summary>
    /// Load the <see cref="Inventory"/> from file or create a new resource, if it was missing. Godot caches calls,
    /// so this can be used every time needed.
    /// </summary>
    /// <returns>The loaded or newly created inventory instance, or null in editor hint.</returns>
    public static Inventory? Restore()
    {
        if (Engine.IsEditorHint())
        {
            return null;
        }

        if (Godot.FileAccess.FileExists(InventoryPath))
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
    /// Returns the icon associated with a given item type.
    /// </summary>
    /// <param name="itemType">The type of item to get the icon for.</param>
    /// <returns>The texture icon for the specified item type, or null if not found.</returns>
    public static Texture2D? GetItemIcon(ItemType itemType)
    {
        return Icons.TryGetValue(itemType, out var icon) ? icon : null;
    }

    /// <summary>
    /// Increment the count of a given item by one, adding it to the inventory if it does not exist.
    /// </summary>
    /// <param name="itemType">The type of item to add to the inventory.</param>
    /// <param name="amount">The amount of the item to add (can be negative to remove).</param>
    public void Add(ItemType itemType, int amount = 1)
    {
        // Note that adding negative numbers is possible. Prevent having a total of negative items.
        // NPC: "You cannot have negative potatoes."
        int oldAmount = this.items.GetValueOrDefault(itemType, 0);
        this.items[itemType] = Math.Max(oldAmount + amount, 0);

        this.EmitSignal(SignalName.ItemChanged, (int) itemType);
    }

    /// <summary>
    /// Decrement the count of a given item by one.
    /// The item will be removed entirely if there are none remaining. Removing an item that is not
    /// possessed will do nothing.
    /// </summary>
    /// <param name="itemType">The type of item to remove from the inventory.</param>
    /// <param name="amount">The amount of the item to remove.</param>
    public void Remove(ItemType itemType, int amount = 1)
    {
        this.Add(itemType, -amount);
    }

    /// <summary>
    /// Returns the number of a certain item type possessed by the player.
    /// </summary>
    /// <param name="itemType">The type of item to count.</param>
    /// <returns>The number of items of the specified type in the inventory.</returns>
    public int GetItemCount(ItemType itemType)
    {
        return this.items.GetValueOrDefault(itemType, 0);
    }

    /// <summary>
    /// Write the inventory contents to the disk.
    /// </summary>
    public void Save()
    {
        ResourceSaver.Save(this, InventoryPath);
    }
}
