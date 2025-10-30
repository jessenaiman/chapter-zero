using System.Collections.ObjectModel;
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
        public string Id { get; set; } = Guid.NewGuid().ToString();

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
    /// Gets the list of item IDs currently in the inventory as a read-only collection.
    /// </summary>
    private readonly List<string> _itemIds = new();
    public ReadOnlyCollection<string> ItemIds => _itemIds.AsReadOnly();

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
        /// Gets or sets the inventory categories and their item assignments as a read-only dictionary.
        /// </summary>
        private readonly Dictionary<string, List<string>> _categoryAssignments = new();
        public IReadOnlyDictionary<string, ReadOnlyCollection<string>> CategoryAssignments => _categoryAssignments.ToDictionary(
            kvp => kvp.Key,
            kvp => kvp.Value.AsReadOnly()
        );

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
            this.Id = string.IsNullOrEmpty(id) ? Guid.NewGuid().ToString() : id;
        }

        /// <summary>
        /// Creates a copy of this inventory.
        /// </summary>
        /// <returns>A new instance of <see cref="Inventory"/> with the same values.</returns>
        public Inventory Clone()
        {
            var clone = new Inventory
            {
                Id = this.Id,
                MaxSize = this.MaxSize,
                MaxWeight = this.MaxWeight,
                CurrentWeight = this.CurrentWeight,
                ItemQuantities = new Dictionary<string, int>(this.ItemQuantities),
                EquippedItems = new Dictionary<string, string>(this.EquippedItems),
                CustomItemData = new Dictionary<string, Dictionary<string, object>>(this.CustomItemData),
                SortingPreferences = new Dictionary<string, object>(this.SortingPreferences)
            };
            clone._itemIds.AddRange(this._itemIds);
            foreach (var kvp in this._categoryAssignments)
            {
                clone._categoryAssignments[kvp.Key] = new List<string>(kvp.Value);
            }
            return clone;
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
            if (this._itemIds.Count >= this.MaxSize)
                return false;

            // Check if item already exists in inventory
            if (this.ItemQuantities.ContainsKey(itemId))
            {
                this.ItemQuantities[itemId] += quantity;
            }
            else
            {
                this._itemIds.Add(itemId);
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
            if (string.IsNullOrEmpty(itemId) || quantity <= 0 || !this.ItemQuantities.TryGetValue(itemId, out int value))
                return 0;

            int actualQuantity = Math.Min(quantity, value);
            this.ItemQuantities[itemId] -= actualQuantity;

            if (this.ItemQuantities[itemId] <= 0)
            {
                this.ItemQuantities.Remove(itemId);
                this._itemIds.Remove(itemId);

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
                foreach (var kvp in this._categoryAssignments)
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
            return this.ItemQuantities.TryGetValue(itemId, out int value) ? value : 0;
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
            this._itemIds.Clear();
            this.ItemQuantities.Clear();
            this.EquippedItems.Clear();
            this.CustomItemData.Clear();
            this._categoryAssignments.Clear();
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
            return this.CustomItemData.TryGetValue(itemId, out Dictionary<string, object>? value) ? value : null;
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
            if (string.IsNullOrEmpty(slot) || !this.EquippedItems.TryGetValue(slot, out string? itemId))
                return string.Empty;
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
            return this.EquippedItems.TryGetValue(slot, out string? value) ? value : string.Empty;
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

            if (!_categoryAssignments.TryGetValue(category, out var value))
            {
                value = new List<string>();
                _categoryAssignments[category] = value;
            }
            if (!value.Contains(itemId))
            {
                value.Add(itemId);
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
                !_categoryAssignments.TryGetValue(category, out var value))
            {
                return;
            }
            value.Remove(itemId);
        }

        /// <summary>
        /// Gets all items in a specific category.
        /// </summary>
        /// <param name="category">The category to get items from.</param>
        /// <returns>A read-only list of item IDs in the specified category.</returns>
        public ReadOnlyCollection<string> GetCategoryItems(string category)
        {
            return _categoryAssignments.TryGetValue(category, out var value) ?
                new ReadOnlyCollection<string>(value) :
                new ReadOnlyCollection<string>(new List<string>());
        }

        /// <summary>
        /// Gets all available categories.
        /// </summary>
        /// <returns>A read-only list of all category names.</returns>
        public ReadOnlyCollection<string> GetCategories()
        {
            return new ReadOnlyCollection<string>(_categoryAssignments.Keys.ToList());
        }

        /// <summary>
        /// Validates the inventory state and removes any inconsistencies.
        /// </summary>
        public void Validate()
        {
            // Remove orphaned item IDs and quantities
            _itemIds.RemoveAll(id => !ItemQuantities.ContainsKey(id));
            ItemQuantities.Keys.Where(id => !_itemIds.Contains(id)).ToList().ForEach(id => ItemQuantities.Remove(id));

            // Remove invalid equipped items
            EquippedItems.Keys.Where(slot => !_itemIds.Contains(EquippedItems[slot])).ToList().ForEach(slot => EquippedItems.Remove(slot));

            // Remove invalid custom data
            CustomItemData.Keys.Where(id => !_itemIds.Contains(id)).ToList().ForEach(id => CustomItemData.Remove(id));

            // Remove invalid category items
            foreach (var kvp in _categoryAssignments)
            {
                kvp.Value.RemoveAll(id => !_itemIds.Contains(id));
            }
        }
    }
}
