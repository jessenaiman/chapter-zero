using Godot;
using System.Collections.Generic;

/// <summary>
/// An exceptionally simple item inventory that tracks which items the player has picked up.
/// Normally, inventory design would be more complex. In particular, you would want to separate the
/// inventory data structures from the UI implementation, as should be done in a future update to
/// the OpenRPG project.
/// In this case, we just want to show the player which items have been picked up so that we can demo
/// a variety of RPG events.
/// </summary>
[GlobalClass]
public partial class UIInventory : HBoxContainer
{
    /// <summary>
    /// Keep track of the inventory item packed scene to easily instantiate new items.
    /// </summary>
    private PackedScene _itemScene;

    public override void _Ready()
    {
        base._Ready();

        _itemScene = GD.Load<PackedScene>("res://src/field/ui/inventory/ui_inventory_item.tscn");

        // Get the Inventory singleton
        var inventory = GetNode("/root/Inventory");
        if (inventory != null)
        {
            // Initialize UI with current inventory state
            var itemTypes = (Godot.Collections.Dictionary)inventory.Get("ItemTypes");
            if (itemTypes != null)
            {
                foreach (var itemName in itemTypes.Keys)
                {
                    var itemId = (int)itemTypes[itemName];
                    UpdateItem(itemId, inventory);
                }
            }

            // Connect to item changed signal
            inventory.Connect("item_changed", Callable.From((int itemType) =>
            {
                OnInventoryItemChanged(itemType, inventory);
            }));
        }
    }

    /// <summary>
    /// Get the UI item node for a specific item ID.
    /// </summary>
    /// <param name="itemId">The item type ID.</param>
    /// <returns>The UI item node, or <see langword="null"/> if not found.</returns>
    private UIInventoryItem GetUIItem(int itemId)
    {
        foreach (Node child in GetChildren())
        {
            if (child is UIInventoryItem item && item.ID == itemId)
            {
                return item;
            }
        }
        return null;
    }

    /// <summary>
    /// Update the UI for a specific item.
    /// </summary>
    /// <param name="itemId">The item type ID.</param>
    /// <param name="inventory">The inventory singleton node.</param>
    private void UpdateItem(int itemId, Node inventory)
    {
        var amount = (int)inventory.Call("get_item_count", itemId);
        var item = GetUIItem(itemId);

        if (amount > 0)
        {
            if (item == null)
            {
                item = _itemScene.Instantiate<UIInventoryItem>();
                item.ID = itemId;
                item.Texture = (Texture2D)inventory.Call("get_item_icon", itemId);
                AddChild(item);
            }

            item.Count = amount;
        }
        else if (item != null)
        {
            item.QueueFree();
        }
    }

    /// <summary>
    /// Called when an item changes in the inventory.
    /// </summary>
    /// <param name="itemType">The item type that changed.</param>
    /// <param name="inventory">The inventory singleton node.</param>
    private void OnInventoryItemChanged(int itemType, Node inventory)
    {
        UpdateItem(itemType, inventory);
    }
}
