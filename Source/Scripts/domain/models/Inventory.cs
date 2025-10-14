using Godot;
using System.Collections.Generic;

namespace OmegaSpiral.Domain.Models
{
    /// <summary>
    /// Represents an inventory system that manages items and their quantities.
    /// </summary>
    public class Inventory
    {
        /// <summary>
        /// Gets or sets the unique identifier for the inventory.
        /// </summary>
        public string Id { get; set; } = System.Guid.NewGuid().ToString();

        /// <summary>
        /// Gets or sets the maximum number of item slots in the inventory.
        /// </summary>
        public int MaxSize { get; set; } = 30;

        /// <summary>
        /// Gets or sets the maximum weight capacity of the inventory.
        /// </summary>
        public float MaxWeight { get; set; } = 100.0f;

        /// <summary>
        /// Gets or sets the current weight of items in the inventory.
        /// </summary>
        public float CurrentWeight { get; set; }

        /// <summary>
        /// Gets the list of item IDs currently in the inventory.
        /// </summary>
        public List<string> ItemIds { get; private set; } = new List<string>();

        /// <summary>
        /// Gets the quantities for each item in the inventory.
        /// </summary>
        public Dictionary<string, int> ItemQuantities { get; private set; } = new Dictionary<string, int>();

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
        /// Initializes a new instance of the <see cref="Inventory"/> class.
        /// </summary>
        public Inventory()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Inventory"/> class with specified parameters.
        /// </summary>
        /// <param name="maxSize">The maximum number of item slots in the inventory.</param>
        /// <param name="maxWeight">The maximum weight capacity of the inventory.</param>
        /// <param name="id">The unique identifier for the inventory.</param>
        public Inventory(int maxSize = 30, float maxWeight = 100.0f, string id = "")
        {
            this.MaxSize = maxSize;
            this.MaxWeight = maxWeight;
            this.Id = string.IsNullOrEmpty(id) ? System.Guid.NewGuid().ToString() : id;
        }

        /// <summary>
        /// Creates a copy of this inventory.
        /// </summary>
        /// <returns>A new instance of <see cref="Inventory"/> with the same values.</returns>
        public Inventory Clone()
        {
            return new Inventory
            {
                Id = this.Id,
                MaxSize = this.MaxSize,
                MaxWeight = this.MaxWeight,
                CurrentWeight = this.CurrentWeight,
                ItemIds = new List<string>(this.ItemIds),
                ItemQuantities = new Dictionary<string, int>(this.ItemQuantities),
                EquippedItems = new Dictionary<string, string>(this.EquippedItems),
                CustomItemData = new Dictionary<string, Dictionary<string, object>>(this.CustomItemData),
                CategoryAssignments = new Dictionary<string, List<string>>(this.CategoryAssignments),
                SortingPreferences = new Dictionary<string, object>(this.SortingPreferences)
            };
        }

        /// <summary>
        /// Adds an item to the inventory.
        /// </summary>
        /// <param name="itemId">The ID of the item to add.</param>
        /// <param name="quantity">The quantity of the item to add.</param>
        /// <returns><see langword="true"/> if the item was added successfully, <see langword="false"/> otherwise.</returns>
        public bool AddItem(string itemId, int quantity = 1)
        {
            if (string.IsNullOrEmpty(itemId) || quantity <= 0)
                return false;

            // Check if inventory has space
            if (this.ItemIds.Count >= this.MaxSize)
                return false;

            // Check if item already exists in inventory
            if (this.ItemQuantities.ContainsKey(itemId))
            {
                this.ItemQuantities[itemId] += quantity;
            }
            else
            {
                this.ItemIds.Add(itemId);
                this.ItemQuantities[itemId] = quantity;
            }

            return true;
        }

        /// <summary>
        /// Removes an item from the inventory.
        /// </summary>
        /// <param name="itemId">The ID of the item to remove.</param>
        /// <param name="quantity">The quantity of the item to remove.</param>
        /// <returns>The actual quantity removed.</returns>
        public int RemoveItem(string itemId, int quantity = 1)
        {
            if (string.IsNullOrEmpty(itemId) || quantity <= 0 || !this.ItemQuantities.ContainsKey(itemId))
                return 0;

            int actualQuantity = System.Math.Min(quantity, this.ItemQuantities[itemId]);
            this.ItemQuantities[itemId] -= actualQuantity;

            if (this.ItemQuantities[itemId] <= 0)
            {
                this.ItemQuantities.Remove(itemId);
                this.ItemIds.Remove(itemId);

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

            return actualQuantity;
        }

        /// <summary>
        /// Gets the quantity of a specific item in the inventory.
        /// </summary>
        /// <param name="itemId">The ID of the item to check.</param>
        /// <returns>The quantity of the item, or 0 if not found.</returns>
        public int GetItemQuantity(string itemId)
        {
            return this.ItemQuantities.ContainsKey(itemId) ? this.ItemQuantities[itemId] : 0;
        }

        /// <summary>
        /// Checks if the inventory contains a specific item.
        /// </summary>
        /// <param name="itemId">The ID of the item to check for.</param>
        /// <param name="minQuantity">The minimum quantity required.</param>
        /// <returns><see langword="true"/> if the item exists in sufficient quantity, <see langword="false"/> otherwise.</returns>
        public bool ContainsItem(string itemId, int minQuantity = 1)
        {
            return this.GetItemQuantity(itemId) >= minQuantity;
        }

        /// <summary>
        /// Checks if the inventory has space for more items.
        /// </summary>
        /// <returns><see langword="true"/> if the inventory has space, <see langword="false"/> otherwise.</returns>
        public bool HasSpace()
        {
            return this.ItemIds.Count < this.MaxSize;
        }

        /// <summary>
        /// Checks if the inventory can hold additional weight.
        /// </summary>
        /// <param name="additionalWeight">The additional weight to check.</param>
        /// <returns><see langword="true"/> if the inventory can hold the additional weight, <see langword="false"/> otherwise.</returns>
        public bool CanHoldWeight(float additionalWeight)
        {
            return this.CurrentWeight + additionalWeight <= this.MaxWeight;
        }

        /// <summary>
        /// Clears all items from the inventory.
        /// </summary>
        public void Clear()
        {
            this.ItemIds.Clear();
            this.ItemQuantities.Clear();
            this.EquippedItems.Clear();
            this.CustomItemData.Clear();
            this.CategoryAssignments.Clear();
            this.CurrentWeight = 0f;
        }

        /// <summary>
        /// Gets the total number of unique items in the inventory.
        /// </summary>
        /// <returns>The count of unique items in the inventory.</returns>
        public int GetUniqueItemCount()
        {
            return this.ItemIds.Count;
        }

        /// <summary>
        /// Gets the total quantity of all items in the inventory.
        /// </summary>
        /// <returns>The total quantity of all items in the inventory.</returns>
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
        /// Gets the current usage percentage of the inventory.
        /// </summary>
        /// <returns>The usage percentage as a value between 0 and 1.</returns>
        public float GetUsagePercentage()
        {
            return this.MaxSize > 0 ? (float) this.ItemIds.Count / this.MaxSize : 0f;
        }

        /// <summary>
        /// Gets the current weight percentage of the inventory.
        /// </summary>
        /// <returns>The weight percentage as a value between 0 and 1.</returns>
        public float GetWeightPercentage()
        {
            return this.MaxWeight > 0 ? this.CurrentWeight / this.MaxWeight : 0f;
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
            return this.CustomItemData.ContainsKey(itemId) ? this.CustomItemData[itemId] : null;
        }

        /// <summary>
        /// Equips an item in the specified slot.
        /// </summary>
        /// <param name="slot">The equipment slot (e.g., "Weapon", "Armor", "Accessory").</param>
        /// <param name="itemId">The ID of the item to equip.</param>
        /// <returns><see langword="true"/> if the item was equipped successfully, <see langword="false"/> otherwise.</returns>
        public bool EquipItem(string slot, string itemId)
        {
            if (string.IsNullOrEmpty(slot) || string.IsNullOrEmpty(itemId) || !this.ContainsItem(itemId))
                return false;

            this.EquippedItems[slot] = itemId;
            return true;
        }

        /// <summary>
        /// Unequips an item from the specified slot.
        /// </summary>
        /// <param name="slot">The equipment slot to unequip.</param>
        /// <returns>The ID of the unequipped item, or empty string if no item was equipped.</returns>
        public string UnequipItem(string slot)
        {
            if (string.IsNullOrEmpty(slot) || !this.EquippedItems.ContainsKey(slot))
                return string.Empty;

            string itemId = this.EquippedItems[slot];
            this.EquippedItems.Remove(slot);
            return itemId;
        }

        /// <summary>
        /// Gets the item ID equipped in the specified slot.
        /// </summary>
        /// <param name="slot">The equipment slot to check.</param>
        /// <returns>The ID of the equipped item, or empty string if no item is equipped.</returns>
        public string GetEquippedItem(string slot)
        {
            return this.EquippedItems.ContainsKey(slot) ? this.EquippedItems[slot] : string.Empty;
        }

        /// <summary>
        /// Gets all equipped items.
        /// </summary>
        /// <returns>A dictionary of slot names to equipped item IDs.</returns>
        public Dictionary<string, string> GetAllEquippedItems()
        {
            return new Dictionary<string, string>(this.EquippedItems);
        }

        /// <summary>
        /// Adds an item to a specific category.
        /// </summary>
        /// <param name="category">The category to add the item to.</param>
        /// <param name="itemId">The ID of the item to categorize.</param>
        public void AddToCategory(string category, string itemId)
        {
            if (string.IsNullOrEmpty(category) || string.IsNullOrEmpty(itemId))
                return;

            if (!this.CategoryAssignments.ContainsKey(category))
            {
                this.CategoryAssignments[category] = new List<string>();
            }

            if (!this.CategoryAssignments[category].Contains(itemId))
            {
                this.CategoryAssignments[category].Add(itemId);
            }
        }

        /// <summary>
        /// Removes an item from a specific category.
        /// </summary>
        /// <param name="category">The category to remove the item from.</param>
        /// <param name="itemId">The ID of the item to remove from category.</param>
        public void RemoveFromCategory(string category, string itemId)
        {
            if (string.IsNullOrEmpty(category) || string.IsNullOrEmpty(itemId) ||
                !this.CategoryAssignments.ContainsKey(category))
            {
                return;
            }

            this.CategoryAssignments[category].Remove(itemId);
        }

        /// <summary>
        /// Gets all items in a specific category.
        /// </summary>
        /// <param name="category">The category to get items from.</param>
        /// <returns>A list of item IDs in the specified category.</returns>
        public List<string> GetCategoryItems(string category)
        {
            return this.CategoryAssignments.ContainsKey(category) ?
                   new List<string>(this.CategoryAssignments[category]) :
                   new List<string>();
        }

        /// <summary>
        /// Gets all available categories.
        /// </summary>
        /// <returns>A list of all category names.</returns>
        public List<string> GetCategories()
        {
            return new List<string>(this.CategoryAssignments.Keys);
        }

        /// <summary>
        /// Validates the inventory state and removes any invalid references.
        /// </summary>
        public void Validate()
        {
            // Remove any item IDs that don't have corresponding quantities
            var itemsToRemove = new List<string>();
            foreach (var itemId in this.ItemIds)
            {
                if (!this.ItemQuantities.ContainsKey(itemId))
                {
                    itemsToRemove.Add(itemId);
                }
            }

            foreach (var itemId in itemsToRemove)
            {
                this.ItemIds.Remove(itemId);
            }

            // Remove any quantities that don't have corresponding item IDs
            itemsToRemove.Clear();
            foreach (var kvp in this.ItemQuantities)
            {
                if (!this.ItemIds.Contains(kvp.Key))
                {
                    itemsToRemove.Add(kvp.Key);
                }
            }

            foreach (var itemId in itemsToRemove)
            {
                this.ItemQuantities.Remove(itemId);
            }

            // Remove equipped items that don't exist in inventory
            var equippedSlots = new List<string>();
            foreach (var kvp in this.EquippedItems)
            {
                if (!this.ItemIds.Contains(kvp.Value))
                {
                    equippedSlots.Add(kvp.Key);
                }
            }

            foreach (var slot in equippedSlots)
            {
                this.EquippedItems.Remove(slot);
            }

            // Remove custom data for items that don't exist
            itemsToRemove.Clear();
            foreach (var kvp in this.CustomItemData)
            {
                if (!this.ItemIds.Contains(kvp.Key))
                {
                    itemsToRemove.Add(kvp.Key);
                }
            }

            foreach (var itemId in itemsToRemove)
            {
                this.CustomItemData.Remove(itemId);
            }

            // Remove items from categories that don't exist
            foreach (var kvp in this.CategoryAssignments)
            {
                var itemsInCategory = kvp.Value;
                var itemsToRemoveFromCategory = new List<string>();
                foreach (var itemId in itemsInCategory)
                {
                    if (!this.ItemIds.Contains(itemId))
                    {
                        itemsToRemoveFromCategory.Add(itemId);
                    }
                }

                foreach (var itemId in itemsToRemoveFromCategory)
                {
                    itemsInCategory.Remove(itemId);
                }
            }
        }
    }
}
