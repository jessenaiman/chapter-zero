// Copyright (c) Ωmega Spiral. All rights reserved.

using System.Collections.ObjectModel;
using System.Text.Json;
using Godot;

namespace OmegaSpiral.Domain.Models
{
    /// <summary>
    /// Represents the data structure for exporting inventory data.
    /// </summary>
    public class InventoryExportData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InventoryExportData"/> class.
        /// </summary>
        public InventoryExportData()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InventoryExportData"/> class from an inventory instance.
        /// </summary>
        /// <param name="inventory">The inventory to export data from.</param>
        public InventoryExportData(Inventory inventory)
        {
            if (inventory != null)
            {
                this.Id = inventory.Id;
                this.MaxSize = inventory.MaxSize;
                this.MaxWeight = inventory.MaxWeight;

                // Export inventory items
                // Note: This assumes the Inventory class has a way to access its items
                // The actual implementation would depend on how the Inventory class stores items
                this._itemIds = new List<string>();
                this.ItemQuantities = new Dictionary<string, int>();

                // This would need to be adapted based on the actual Inventory class implementation
                // For now, assuming there's a way to iterate through items
                // foreach (var item in inventory.Items)
                // {
                //     _itemIds.Add(item.Id);
                //     ItemQuantities[item.Id] = item.Quantity;
                // }
            }
        }

        /// <summary>
        /// Gets or sets the unique identifier for the inventory.
        /// </summary>
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the maximum size of the inventory.
        /// </summary>
        public int MaxSize { get; set; } = 30;

        /// <summary>
        /// Gets or sets the maximum weight capacity of the inventory.
        /// </summary>
        public float MaxWeight { get; set; } = 100.0f;

        /// <summary>
        /// Gets the list of item IDs in the inventory.
        /// </summary>
        [System.Text.Json.Serialization.JsonIgnore]
        private List<string> _itemIds = new List<string>();

        /// <summary>
        /// Gets the read-only collection of item IDs in the inventory.
        /// </summary>
        public ReadOnlyCollection<string> ItemIds => new ReadOnlyCollection<string>(this._itemIds);

        /// <summary>
        /// Gets the quantities for each item in the inventory.
        /// </summary>
        public Dictionary<string, int> ItemQuantities { get; } = new Dictionary<string, int>();

        /// <summary>
        /// Gets the equipped item IDs for this inventory.
        /// </summary>
        public Dictionary<string, string> EquippedItems { get; } = new Dictionary<string, string>();

        /// <summary>
        /// Gets the custom item data for items with special properties.
        /// </summary>
        public Dictionary<string, Dictionary<string, object>> CustomItemData { get; } = new Dictionary<string, Dictionary<string, object>>();

        /// <summary>
        /// Gets the inventory categories and their item assignments.
        /// </summary>
        public Dictionary<string, List<string>> CategoryAssignments { get; } = new Dictionary<string, List<string>>();

        /// <summary>
        /// Gets the sorting preferences for the inventory.
        /// </summary>
        public Dictionary<string, object> SortingPreferences { get; } = new Dictionary<string, object>();

        /// <summary>
        /// Gets or sets the timestamp of when this data was exported.
        /// </summary>
        public string ExportTimestamp { get; set; } = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ", System.Globalization.CultureInfo.InvariantCulture);

        /// <summary>
        /// Gets or sets the version of the export format.
        /// </summary>
        public string Version { get; set; } = "1.0";

        /// <summary>
        /// Gets or sets the source game or application.
        /// </summary>
        public string Source { get; set; } = "OmegaSpiral";

        /// <summary>
        /// Gets additional metadata for the export.
        /// </summary>
        public Dictionary<string, object> Metadata { get; } = new Dictionary<string, object>();

        /// <summary>
        /// Creates export data from a JSON string representation.
        /// </summary>
        /// <summary>
        /// Creates export data from a JSON string representation.
        /// </summary>
        /// <param name="json">The JSON string to parse.</param>
        /// <returns>A new instance of <see cref="InventoryExportData"/> parsed from the JSON.</returns>
        public static InventoryExportData FromJson(string json)
        {
            if (string.IsNullOrEmpty(json))
            {
                return new InventoryExportData();
            }

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                WriteIndented = false
            };

            var exportData = JsonSerializer.Deserialize<InventoryExportData>(json, options) ?? new InventoryExportData();

            // Initialize the private _itemIds list from the deserialized ItemIds if it exists
            if (exportData.ItemIds != null)
            {
                exportData._itemIds = new List<string>(exportData.ItemIds);
            }
            else
            {
                exportData._itemIds = new List<string>();
            }

            return exportData;
        }

        /// <summary>
        /// Validates whether this export data contains all required information.
        /// </summary>
        /// <returns><see langword="true"/> if the data is valid for export, <see langword="false"/> otherwise.</returns>
        public bool IsValid()
        {
            return !string.IsNullOrEmpty(this.Id) &&
                   this.ItemIds != null &&
                   this.ItemQuantities != null &&
                   this.EquippedItems != null;
        }

        /// <summary>
        /// Creates an inventory instance from this export data.
        /// </summary>
        /// <returns>A new inventory instance populated from the export data.</returns>
        public Inventory CreateInventory()
        {
            var inventory = new Inventory
            {
                Id = this.Id,
                MaxSize = this.MaxSize,
                MaxWeight = this.MaxWeight,
            };

            // Add items from the export data
            foreach (var itemId in this.ItemIds)
            {
                var quantity = this.ItemQuantities.TryGetValue(itemId, out int value) ? value : 1;
                inventory.AddItem(itemId, quantity);
            }

            return inventory;
        }

        /// <summary>
        /// Converts this export data to a JSON string representation.
        /// </summary>
        /// <returns>A JSON string representation of the export data.</returns>
        public string ToJson()
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = false,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            return JsonSerializer.Serialize(this, options);
        }

        /// <summary>
        /// Adds an item to this export data configuration.
        /// </summary>
        /// <param name="itemId">The ID of the item to add.</param>
        /// <param name="quantity">The quantity of the item to add.</param>
        public void AddItem(string itemId, int quantity = 1)
        {
            if (!string.IsNullOrEmpty(itemId) && quantity > 0)
            {
                if (!this._itemIds.Contains(itemId))
                {
                    this._itemIds.Add(itemId);
                }

                this.ItemQuantities[itemId] = quantity;
            }
        }

        /// <summary>
        /// Removes an item from this export data configuration.
        /// </summary>
        /// <param name="itemId">The ID of the item to remove.</param>
        public void RemoveItem(string itemId)
        {
            if (!string.IsNullOrEmpty(itemId))
            {
                this._itemIds.Remove(itemId);
                this.ItemQuantities.Remove(itemId);

                // Remove from equipped items if present
                var equippedSlots = new List<string>();
                foreach (var kvp in this.EquippedItems)
                {
                    if (kvp.Value == itemId)
                    {
                        equippedSlots.Add(kvp.Key);
                    }
                }

                foreach (var slot in equippedSlots)
                {
                    this.EquippedItems.Remove(slot);
                }

                // Remove from custom data if present
                this.CustomItemData.Remove(itemId);

                // Remove from categories if present
                foreach (var kvp in this.CategoryAssignments)
                {
                    kvp.Value.Remove(itemId);
                }
            }
        }

        /// <summary>
        /// Sets custom data for a specific item.
        /// </summary>
        /// <param name="itemId">The ID of the item.</param>
        /// <param name="customData">The custom data dictionary for the item.</param>
        public void SetItemCustomData(string itemId, Dictionary<string, object> customData)
        {
            if (!string.IsNullOrEmpty(itemId) && customData != null)
            {
                this.CustomItemData[itemId] = customData;
            }
        }

        /// <summary>
        /// Gets custom data for a specific item.
        /// </summary>
        /// <param name="itemId">The ID of the item.</param>
        /// <returns>The custom data dictionary for the item, or null if not found.</returns>
        public Dictionary<string, object>? GetItemCustomData(string itemId)
        {
            return this.CustomItemData.TryGetValue(itemId, out Dictionary<string, object>? value) ? value : null;
        }

        /// <summary>
        /// Gets the total weight of all items in this export data.
        /// </summary>
        /// <param name="itemWeightProvider">A function that provides the weight of an item by its ID.</param>
        /// <returns>The total weight of all items in the export data.</returns>
        public float GetTotalWeight(System.Func<string, float> itemWeightProvider)
        {
            float totalWeight = 0f;

            foreach (var itemId in this.ItemIds)
            {
                var quantity = this.ItemQuantities.TryGetValue(itemId, out int value) ? value : 1;
                var itemWeight = itemWeightProvider?.Invoke(itemId) ?? 0f;
                totalWeight += itemWeight * quantity;
            }

            return totalWeight;
        }

        /// <summary>
        /// Gets the total number of unique items in this export data.
        /// </summary>
        /// <returns>The count of unique items in the export data.</returns>
        public int GetUniqueItemCount()
        {
            return this.ItemIds.Count;
        }

        /// <summary>
        /// Gets the total quantity of all items in this export data.
        /// </summary>
        /// <returns>The total quantity of all items in the export data.</returns>
        public int GetTotalItemCount()
        {
            int totalCount = 0;
            foreach (var kvp in this.ItemQuantities)
            {
                totalCount += kvp.Value;
            }

            return totalCount;
        }

        /// <summary>
        /// Clears all items from this export data.
        /// </summary>
        public void ClearAllItems()
        {
            this._itemIds.Clear();
            this.ItemQuantities.Clear();
            this.EquippedItems.Clear();
            this.CustomItemData.Clear();
            this.CategoryAssignments.Clear();
        }

        /// <summary>
        /// Creates a deep copy of this export data.
        /// </summary>
        /// <returns>A new instance of <see cref="InventoryExportData"/> with the same values.</returns>
        public InventoryExportData Clone()
        {
            var clone = new InventoryExportData
            {
                Id = this.Id,
                MaxSize = this.MaxSize,
                MaxWeight = this.MaxWeight,
                ExportTimestamp = this.ExportTimestamp,
                Version = this.Version,
                Source = this.Source,
            };

            foreach (var itemId in this.ItemIds)
            {
                clone._itemIds.Add(itemId);
            }

            foreach (var kvp in this.ItemQuantities)
            {
                clone.ItemQuantities[kvp.Key] = kvp.Value;
            }

            foreach (var kvp in this.EquippedItems)
            {
                clone.EquippedItems[kvp.Key] = kvp.Value;
            }

            foreach (var kvp in this.CustomItemData)
            {
                clone.CustomItemData[kvp.Key] = new Dictionary<string, object>(kvp.Value);
            }

            foreach (var kvp in this.CategoryAssignments)
            {
                clone.CategoryAssignments[kvp.Key] = new List<string>(kvp.Value);
            }

            foreach (var kvp in this.SortingPreferences)
            {
                clone.SortingPreferences[kvp.Key] = kvp.Value;
            }

            foreach (var kvp in this.Metadata)
            {
                clone.Metadata[kvp.Key] = kvp.Value;
            }

            return clone;
        }
    }
}
