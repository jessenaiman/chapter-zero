using System.Collections.ObjectModel;
namespace OmegaSpiral.Domain.Models
{
    /// <summary>
    /// Represents a preset inventory configuration that can be applied to characters.
    /// </summary>
    public class InventoryPreset
    {
        /// <summary>
        /// Gets or sets the unique identifier for the inventory preset.
        /// </summary>
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the display name of the inventory preset.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the description of the inventory preset.
        /// </summary>
        public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Gets the list of item IDs included in this preset (read-only).
    /// </summary>
    public ReadOnlyCollection<string> ItemIds => new ReadOnlyCollection<string>(_itemIds);
    private readonly List<string> _itemIds = new();

        /// <summary>
        /// Gets or sets the quantities for each item in the preset.
        /// </summary>
        public Dictionary<string, int> ItemQuantities { get; set; } = [];

        /// <summary>
        /// Gets or sets the equipped item IDs for this preset.
        /// </summary>
        public Dictionary<string, string> EquippedItems { get; set; } = [];

        /// <summary>
        /// Gets or sets a value indicating whether gets or sets whether this preset is available for selection.
        /// </summary>
        public bool IsAvailable { get; set; } = true;

        /// <summary>
        /// Gets or sets the unlock condition for this preset.
        /// </summary>
        public string UnlockCondition { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the icon resource path for this preset.
        /// </summary>
        public string IconPath { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the category of this preset (e.g., "Default", "Premium", "Class-Specific").
        /// </summary>
        public string Category { get; set; } = "Default";

        /// <summary>
        /// Gets or sets the maximum inventory size for this preset.
        /// </summary>
        public int MaxInventorySize { get; set; } = 30;

        /// <summary>
        /// Gets or sets the maximum weight capacity for this preset.
        /// </summary>
        public float MaxWeightCapacity { get; set; } = 100.0f;

        /// <summary>
        /// Initializes a new instance of the <see cref="InventoryPreset"/> class.
        /// </summary>
        public InventoryPreset()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InventoryPreset"/> class with specified parameters.
        /// </summary>
        /// <param name="id">The unique identifier for the inventory preset.</param>
        /// <param name="name">The display name of the inventory preset.</param>
        /// <param name="description">The description of the inventory preset.</param>
        /// <param name="itemIds">The list of item IDs included in this preset.</param>
        /// <param name="itemQuantities">The quantities for each item in the preset.</param>
        /// <param name="equippedItems">The equipped item IDs for this preset.</param>
        /// <param name="isAvailable">Whether this preset is available for selection.</param>
        /// <param name="unlockCondition">The unlock condition for this preset.</param>
        /// <param name="iconPath">The icon resource path for this preset.</param>
        /// <param name="category">The category of this preset.</param>
        /// <param name="maxInventorySize">The maximum inventory size for this preset.</param>
        /// <param name="maxWeightCapacity">The maximum weight capacity for this preset.</param>
        public class InventoryPresetOptions
        {
            public string Id { get; set; } = string.Empty;
            public string Name { get; set; } = string.Empty;
            public string Description { get; set; } = string.Empty;
            public IEnumerable<string> ItemIds { get; set; } = new List<string>();
            public Dictionary<string, int> ItemQuantities { get; set; } = new();
            public Dictionary<string, string> EquippedItems { get; set; } = new();
            public bool IsAvailable { get; set; } = true;
            public string UnlockCondition { get; set; } = string.Empty;
            public string IconPath { get; set; } = string.Empty;
            public string Category { get; set; } = "Default";
            public int MaxInventorySize { get; set; } = 30;
            public float MaxWeightCapacity { get; set; } = 100.0f;
        }

        public InventoryPreset(InventoryPresetOptions options)
        {
            this.Id = options.Id;
            this.Name = options.Name;
            this.Description = options.Description;
            if (options.ItemIds != null)
            {
                _itemIds.AddRange(options.ItemIds);
            }
            this.ItemQuantities = options.ItemQuantities ?? new();
            this.EquippedItems = options.EquippedItems ?? new();
            this.IsAvailable = options.IsAvailable;
            this.UnlockCondition = options.UnlockCondition;
            this.IconPath = options.IconPath;
            this.Category = options.Category;
            this.MaxInventorySize = options.MaxInventorySize;
            this.MaxWeightCapacity = options.MaxWeightCapacity;
        }

        /// <summary>
        /// Creates a copy of this inventory preset.
        /// </summary>
        /// <returns>A new instance of <see cref="InventoryPreset"/> with the same values.</returns>
        public InventoryPreset Clone()
        {
            return new InventoryPreset(new InventoryPresetOptions
            {
                Id = this.Id,
                Name = this.Name,
                Description = this.Description,
                ItemIds = new List<string>(this._itemIds),
                ItemQuantities = new Dictionary<string, int>(this.ItemQuantities),
                EquippedItems = new Dictionary<string, string>(this.EquippedItems),
                IsAvailable = this.IsAvailable,
                UnlockCondition = this.UnlockCondition,
                IconPath = this.IconPath,
                Category = this.Category,
                MaxInventorySize = this.MaxInventorySize,
                MaxWeightCapacity = this.MaxWeightCapacity
            });
        }
        /// <summary>
        /// Validates whether this preset is properly configured.
        /// </summary>
        /// <returns><see langword="true"/> if the preset is valid, <see langword="false"/> otherwise.</returns>
        public bool IsValid()
        {
            return !string.IsNullOrEmpty(this.Id)
                && !string.IsNullOrEmpty(this.Name)
                && this._itemIds != null
                && this.ItemQuantities != null
                && this.EquippedItems != null;
        }

        /// <summary>
        /// Adds an item to this preset configuration.
        /// </summary>
        /// <param name="itemId">The ID of the item to add.</param>
        /// <param name="quantity">The quantity of the item to add.</param>
        public void AddItem(string itemId, int quantity = 1)
        {
            if (!string.IsNullOrEmpty(itemId) && quantity > 0)
            {
                if (!_itemIds.Contains(itemId))
                {
                    _itemIds.Add(itemId);
                }
                this.ItemQuantities[itemId] = quantity;
            }
        }

        /// <summary>
        /// Removes an item from this preset configuration.
        /// </summary>
        /// <param name="itemId">The ID of the item to remove.</param>
        public void RemoveItem(string itemId)
        {
            if (!string.IsNullOrEmpty(itemId))
            {
                _itemIds.Remove(itemId);
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
            }
        }

        /// <summary>
        /// Equips an item in the specified slot.
        /// </summary>
        /// <param name="slot">The equipment slot (e.g., "Weapon", "Armor", "Accessory").</param>
        /// <param name="itemId">The ID of the item to equip.</param>
        public void EquipItem(string slot, string itemId)
        {
            if (!string.IsNullOrEmpty(slot) && !string.IsNullOrEmpty(itemId))
            {
                this.EquippedItems[slot] = itemId;
            }
        }

        /// <summary>
        /// Unequips an item from the specified slot.
        /// </summary>
        /// <param name="slot">The equipment slot to unequip.</param>
        public void UnequipItem(string slot)
        {
            if (!string.IsNullOrEmpty(slot))
            {
                this.EquippedItems.Remove(slot);
            }
        }

        /// <summary>
        /// Gets the total weight of all items in this preset.
        /// </summary>
        /// <param name="itemWeightProvider">A function that provides the weight of an item by its ID.</param>
        /// <returns>The total weight of all items in the preset.</returns>
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
        /// Gets the total number of unique items in this preset.
        /// </summary>
        /// <returns>The count of unique items in the preset.</returns>
        public int GetUniqueItemCount()
        {
            return this.ItemIds.Count;
        }

        /// <summary>
        /// Gets the total quantity of all items in this preset.
        /// </summary>
        /// <returns>The total quantity of all items in the preset.</returns>
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
        /// Clears all items from this preset.
        /// </summary>
        public void ClearAllItems()
        {
            _itemIds.Clear();
            this.ItemQuantities.Clear();
            this.EquippedItems.Clear();
        }

        /// <summary>
        /// Gets the display name with category prefix if applicable.
        /// </summary>
        /// <returns>The formatted display name.</returns>
        public string GetDisplayName()
        {
            if (string.IsNullOrEmpty(this.Category) || this.Category == "Default")
            {
                return this.Name;
            }

            return $"{this.Category}: {this.Name}";
        }
    }
}
