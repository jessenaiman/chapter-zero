namespace OmegaSpiral.Source.Scripts.Models
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Manages a character's inventory of items.
    /// </summary>
    public class Inventory
    {
        /// <summary>
        /// Inventory sort types for sorting item lists.
        /// </summary>
        public enum SortType
        {
            /// <summary>Sort by item name.</summary>
            Name,

            /// <summary>Sort by item type.</summary>
            Type,

            /// <summary>Sort by item value.</summary>
            Value,

            /// <summary>Sort by item weight.</summary>
            Weight,

            /// <summary>Sort by date acquired.</summary>
            DateAcquired,
        }

        /// <summary>
        /// Inventory filter types for filtering item lists.
        /// </summary>
        public enum FilterType
        {
            /// <summary>Show all items.</summary>
            All,

            /// <summary>Show only weapons.</summary>
            Weapons,

            /// <summary>Show only armor.</summary>
            Armor,

            /// <summary>Show only consumables.</summary>
            Consumables,

            /// <summary>Show only quest items.</summary>
            QuestItems,

            /// <summary>Show only miscellaneous items.</summary>
            Miscellaneous,
        }

        /// <summary>
        /// Gets or sets the list of items in the inventory.
        /// </summary>
        public List<Item> Items { get; set; } = new List<Item>();

        /// <summary>
        /// Gets or sets the list of equipped items.
        /// </summary>
        public List<Item> EquippedItems { get; set; } = new List<Item>();

        /// <summary>
        /// Gets or sets the maximum number of items the inventory can hold.
        /// </summary>
        public int MaxCapacity { get; set; } = 50;

        /// <summary>
        /// Gets or sets the current gold amount.
        /// </summary>
        public int Gold { get; set; }

        /// <summary>
        /// Gets the number of items currently in the inventory.
        /// </summary>
        public int Count => Items.Count;

        /// <summary>
        /// Gets a value indicating whether the inventory is full.
        /// </summary>
        public bool IsFull => Items.Count >= MaxCapacity;

        /// <summary>
        /// Initializes a new instance of the <see cref="Inventory"/> class.
        /// </summary>
        public Inventory()
        {
        }

        /// <summary>
        /// Adds an item to the inventory.
        /// </summary>
        /// <param name="item">The item to add.</param>
        /// <returns><see langword="true"/> if the item was added successfully; otherwise, <see langword="false"/>.</returns>
        public bool AddItem(Item item)
        {
            if (item == null || IsFull)
            {
                return false;
            }

            // Check if item is stackable and already exists
            if (item.IsStackable)
            {
                var existingItem = Items.FirstOrDefault(i => i.Id == item.Id);
                if (existingItem != null)
                {
                    // Stack the item (not implemented in detail here)
                    return true;
                }
            }

            Items.Add(item);
            return true;
        }

        /// <summary>
        /// Removes an item from the inventory.
        /// </summary>
        /// <param name="item">The item to remove.</param>
        /// <returns><see langword="true"/> if the item was removed successfully; otherwise, <see langword="false"/>.</returns>
        public bool RemoveItem(Item item)
        {
            return Items.Remove(item);
        }

        /// <summary>
        /// Gets an item by ID.
        /// </summary>
        /// <param name="itemId">The ID of the item to get.</param>
        /// <returns>The item if found; otherwise, <see langword="null"/>.</returns>
        public Item? GetItemById(string itemId)
        {
            return Items.FirstOrDefault(i => i.Id == itemId);
        }

        /// <summary>
        /// Checks if the inventory contains an item.
        /// </summary>
        /// <param name="item">The item to check for.</param>
        /// <returns><see langword="true"/> if the inventory contains the item; otherwise, <see langword="false"/>.</returns>
        public bool ContainsItem(Item item)
        {
            return Items.Contains(item);
        }

        /// <summary>
        /// Adds gold to the inventory.
        /// </summary>
        /// <param name="amount">The amount of gold to add.</param>
        public void AddGold(int amount)
        {
            Gold += amount;
        }

        /// <summary>
        /// Removes gold from the inventory.
        /// </summary>
        /// <param name="amount">The amount of gold to remove.</param>
        /// <returns><see langword="true"/> if the gold was removed successfully; otherwise, <see langword="false"/>.</returns>
        public bool RemoveGold(int amount)
        {
            if (Gold >= amount)
            {
                Gold -= amount;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Clears all items from the inventory.
        /// </summary>
        public void Clear()
        {
            Items.Clear();
        }
    }
}
