// <copyright file="InventoryImportData.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace OmegaSpiral.Domain.Models
{
    using System.Collections.Generic;
    using Godot;

    /// <summary>
    /// Represents the data structure for importing inventory data.
    /// </summary>
    public class InventoryImportData
    {
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
        /// Gets or sets the list of item IDs in the inventory.
        /// </summary>
        public List<string> ItemIds { get; set; } = new List<string>();

        /// <summary>
        /// Gets the quantities for each item in the inventory.
        /// </summary>
        public Dictionary<string, int> ItemQuantities { get; } = new Dictionary<string, int>();

        /// <summary>
        /// Gets or sets the equipped item IDs for this inventory.
        /// </summary>
        public Dictionary<string, string> EquippedItems { get; set; } = new Dictionary<string, string>();

        /// <summary>
        /// Gets or sets the custom item data for items with special properties.
        /// </summary>
        public Dictionary<string, Dictionary<string, object>> CustomItemData { get; set; } = new Dictionary<string, Dictionary<string, object>>();

        /// <summary>
        /// Gets or sets the inventory categories and their item assignments.
        /// </summary>
        public Dictionary<string, List<string>> CategoryAssignments { get; set; } = new Dictionary<string, List<string>>();

        /// <summary>
        /// Gets or sets the sorting preferences for the inventory.
        /// </summary>
        public Dictionary<string, object> SortingPreferences { get; set; } = new Dictionary<string, object>();

        /// <summary>
        /// Gets or sets the timestamp of when this data was exported.
        /// </summary>
        public string ExportTimestamp { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the version of the export format.
        /// </summary>
        public string Version { get; set; } = "1.0";

        /// <summary>
        /// Gets or sets the source game or application.
        /// </summary>
        public string Source { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets additional metadata for the import.
        /// </summary>
        public Dictionary<string, object> Metadata { get; set; } = new Dictionary<string, object>();

        /// <summary>
        /// Initializes a new instance of the <see cref="InventoryImportData"/> class.
        /// </summary>
        public InventoryImportData()
        {
        }

        /// <summary>
        /// Validates whether this import data contains all required information.
        /// </summary>
        /// <returns><see langword="true"/> if the data is valid for import, <see langword="false"/> otherwise.</returns>
        public bool IsValid()
        {
            return !string.IsNullOrEmpty(this.Id) &&
                   this.ItemIds != null &&
                   this.ItemQuantities != null &&
                   this.EquippedItems != null;
        }

        /// <summary>
        /// Creates an inventory instance from this import data.
        /// </summary>
        /// <returns>A new inventory instance populated from the import data.</returns>
        public Inventory CreateInventory()
        {
            var inventory = new Inventory
            {
                Id = this.Id,
                MaxSize = this.MaxSize,
                MaxWeight = this.MaxWeight,
            };

            // Add items from the import data
            foreach (var itemId in this.ItemIds)
            {
                var quantity = this.ItemQuantities.ContainsKey(itemId) ? this.ItemQuantities[itemId] : 1;
                inventory.AddItem(itemId, quantity);
            }

            return inventory;
        }

        /// <summary>
        /// Merges this import data with an existing inventory, updating only specified fields.
        /// </summary>
        /// <param name="existingInventory">The existing inventory to update.</param>
        /// <param name="updateItems">Whether to update the inventory items.</param>
        /// <param name="updateEquippedItems">Whether to update the equipped items.</param>
        /// <param name="updateCustomData">Whether to update the custom item data.</param>
        /// <param name="updateCategories">Whether to update the category assignments.</param>
        public void MergeInto(Inventory existingInventory, bool updateItems = true, bool updateEquippedItems = false, bool updateCustomData = false, bool updateCategories = false)
        {
            if (existingInventory == null)
            {
                return;
            }

            if (updateItems)
            {
                // Clear existing items and add new ones
                existingInventory.Clear();

                foreach (var itemId in this.ItemIds)
                {
                    var quantity = this.ItemQuantities.ContainsKey(itemId) ? this.ItemQuantities[itemId] : 1;
                    existingInventory.AddItem(itemId, quantity);
                }
            }

            if (updateEquippedItems)
            {
                // Update equipped items (this would require an EquipmentSystem or similar)
                foreach (var kvp in this.EquippedItems)
                {
                    // This would depend on the equipment system implementation
                    // existingInventory.EquipItem(kvp.Key, kvp.Value);
                }
            }

            if (updateCustomData)
            {
                // Update custom item data
                foreach (var kvp in this.CustomItemData)
                {
                    var itemId = kvp.Key;
                    var customData = kvp.Value;

                    // This would depend on the item system implementation
                    // existingInventory.UpdateItemCustomData(itemId, customData);
                }
            }

            if (updateCategories)
            {
                // Update category assignments
                foreach (var kvp in this.CategoryAssignments)
                {
                    var category = kvp.Key;
                    var items = kvp.Value;

                    // This would depend on the category system implementation
                    // existingInventory.SetCategoryItems(category, items);
                }
            }
        }

        /// <summary>
        /// Adds an item to this import data configuration.
        /// </summary>
        /// <param name="itemId">The ID of the item to add.</param>
        /// <param name="quantity">The quantity of the item to add.</param>
        public void AddItem(string itemId, int quantity = 1)
        {
            if (!string.IsNullOrEmpty(itemId) && quantity > 0)
            {
                if (!this.ItemIds.Contains(itemId))
                {
                    this.ItemIds.Add(itemId);
                }

                this.ItemQuantities[itemId] = quantity;
            }
        }

        /// <summary>
        /// Removes an item from this import data configuration.
        /// </summary>
        /// <param name="itemId">The ID of the item to remove.</param>
        public void RemoveItem(string itemId)
        {
            if (!string.IsNullOrEmpty(itemId))
            {
                this.ItemIds.Remove(itemId);
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
        public Dictionary<string, object> GetItemCustomData(string itemId)
        {
            return this.CustomItemData.ContainsKey(itemId) ? this.CustomItemData[itemId] : null;
        }

        /// <summary>
        /// Gets the total weight of all items in this import data.
        /// </summary>
        /// <param name="itemWeightProvider">A function that provides the weight of an item by its ID.</param>
        /// <returns>The total weight of all items in the import data.</returns>
        public float GetTotalWeight(System.Func<string, float> itemWeightProvider)
        {
            float totalWeight = 0f;

            foreach (var itemId in this.ItemIds)
            {
                var quantity = this.ItemQuantities.ContainsKey(itemId) ? this.ItemQuantities[itemId] : 1;
                var itemWeight = itemWeightProvider?.Invoke(itemId) ?? 0f;
                totalWeight += itemWeight * quantity;
            }

            return totalWeight;
        }

        /// <summary>
        /// Gets the total number of unique items in this import data.
        /// </summary>
        /// <returns>The count of unique items in the import data.</returns>
        public int GetUniqueItemCount()
        {
            return this.ItemIds.Count;
        }

        /// <summary>
        /// Gets the total quantity of all items in this import data.
        /// </summary>
        /// <returns>The total quantity of all items in the import data.</returns>
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
        /// Clears all items from this import data.
        /// </summary>
        public void ClearAllItems()
        {
            this.ItemIds.Clear();
            this.ItemQuantities.Clear();
            this.EquippedItems.Clear();
            this.CustomItemData.Clear();
            this.CategoryAssignments.Clear();
        }

        /// <summary>
        /// Creates a deep copy of this import data.
        /// </summary>
        /// <returns>A new instance of <see cref="InventoryImportData"/> with the same values.</returns>
        public InventoryImportData Clone()
        {
            return new InventoryImportData
            {
                Id = this.Id,
                MaxSize = this.MaxSize,
                MaxWeight = this.MaxWeight,
                ItemIds = new List<string>(this.ItemIds),
                ItemQuantities = new Dictionary<string, int>(this.ItemQuantities),
                EquippedItems = new Dictionary<string, string>(this.EquippedItems),
                CustomItemData = new Dictionary<string, Dictionary<string, object>>(this.CustomItemData),
                CategoryAssignments = new Dictionary<string, List<string>>(this.CategoryAssignments),
                SortingPreferences = new Dictionary<string, object>(this.SortingPreferences),
                ExportTimestamp = this.ExportTimestamp,
                Version = this.Version,
                Source = this.Source,
                Metadata = new Dictionary<string, object>(this.Metadata),
            };
        }
    }
}
