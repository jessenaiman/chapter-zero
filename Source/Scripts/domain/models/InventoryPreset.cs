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
        /// Gets or sets the list of item IDs included in this preset.
        /// </summary>
        public List<string> ItemIds { get; set; } = [];

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
        public InventoryPreset(
            string id,
            string name,
            string description,
            List<string> itemIds,
            Dictionary<string, int> itemQuantities,
            Dictionary<string, string> equippedItems,
            bool isAvailable = true,
            string unlockCondition = "",
            string iconPath = "",
            string category = "Default",
            int maxInventorySize = 30,
            float maxWeightCapacity = 100.0f)
        {
            this.Id = id;
            this.Name = name;
            this.Description = description;
            this.ItemIds = itemIds ?? [];
            this.ItemQuantities = itemQuantities ?? [];
            this.EquippedItems = equippedItems ?? [];
            this.IsAvailable = isAvailable;
            this.UnlockCondition = unlockCondition;
            this.IconPath = iconPath;
            this.Category = category;
            this.MaxInventorySize = maxInventorySize;
            this.MaxWeightCapacity = maxWeightCapacity;
        }

        /// <summary>
        /// Creates a copy of this inventory preset.
        /// </summary>
        /// <returns>A new instance of <see cref="InventoryPreset"/> with the same values.</returns>
        public InventoryPreset Clone()
        {
            return new InventoryPreset
            {
                Id = this.Id,
                Name = this.Name,
                Description = this.Description,
                ItemIds = new List<string>(this.ItemIds),
                ItemQuantities = new Dictionary<string, int>(this.ItemQuantities),
                EquippedItems = new Dictionary<string, string>(this.EquippedItems),
                IsAvailable = this.IsAvailable,
                UnlockCondition = this.UnlockCondition,
                IconPath = this.IconPath,
                Category = this.Category,
                MaxInventorySize = this.MaxInventorySize,
                MaxWeightCapacity = this.MaxWeightCapacity,
            };
        }

        /// <summary>
        /// Creates an inventory instance based on this preset.
        /// </summary>
        /// <returns>A new inventory instance with the preset's configuration.</returns>
        public Inventory CreateInventory()
        {
            var inventory = new Inventory
            {
                MaxSize = this.MaxInventorySize,
                MaxWeight = this.MaxWeightCapacity,
            };

            // Add items from the preset
            foreach (var itemId in this.ItemIds)
            {
                var quantity = this.ItemQuantities.TryGetValue(itemId, out int value) ? value : 1;
                inventory.AddItem(itemId, quantity);
            }

            return inventory;
        }

        /// <summary>
        /// Validates whether this preset is properly configured.
        /// </summary>
        /// <returns><see langword="true"/> if the preset is valid, <see langword="false"/> otherwise.</returns>
        public bool IsValid()
        {
            return !string.IsNullOrEmpty(this.Id) &&
                   !string.IsNullOrEmpty(this.Name) &&
                   this.ItemIds != null &&
                   this.ItemQuantities != null &&
                   this.EquippedItems != null;
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
                if (!this.ItemIds.Contains(itemId))
                {
                    this.ItemIds.Add(itemId);
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
            this.ItemIds.Clear();
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
