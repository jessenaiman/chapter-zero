namespace OmegaSpiral.Source.Scripts.Models
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Manages a character's equipment slots and equipped items.
    /// </summary>
    public class Equipment
    {
        /// <summary>
        /// Gets or sets the list of equipment slots available to the character.
        /// </summary>
        public List<EquipmentSlot> Slots { get; set; } = new List<EquipmentSlot>();

        /// <summary>
        /// Gets or sets the dictionary of equipped items keyed by slot.
        /// </summary>
        public Dictionary<EquipmentSlot, Item> EquippedItems { get; set; } = new Dictionary<EquipmentSlot, Item>();

        /// <summary>
        /// Initializes a new instance of the <see cref="Equipment"/> class.
        /// </summary>
        public Equipment()
        {
            InitializeDefaultSlots();
        }

        /// <summary>
        /// Equips an item to the specified slot.
        /// </summary>
        /// <param name="item">The item to equip.</param>
        /// <param name="slot">The slot to equip the item to.</param>
        /// <returns><see langword="true"/> if the item was equipped successfully; otherwise, <see langword="false"/>.</returns>
        public bool EquipItem(Item item, EquipmentSlot slot)
        {
            if (item == null || slot == null || !Slots.Contains(slot))
            {
                return false;
            }

            // Unequip any existing item in the slot
            if (slot.Item != null)
            {
                UnequipItem(slot);
            }

            // Equip the new item
            slot.Item = item;
            EquippedItems[slot] = item;
            return true;
        }

        /// <summary>
        /// Unequips the item from the specified slot.
        /// </summary>
        /// <param name="slot">The slot to unequip.</param>
        /// <returns><see langword="true"/> if the item was unequipped successfully; otherwise, <see langword="false"/>.</returns>
        public bool UnequipItem(EquipmentSlot slot)
        {
            if (slot == null || slot.Item == null)
            {
                return false;
            }

            slot.Item = null;
            EquippedItems.Remove(slot);
            return true;
        }

        /// <summary>
        /// Removes an item from equipment by finding its slot.
        /// </summary>
        /// <param name="item">The item to remove.</param>
        /// <returns><see langword="true"/> if the item was removed successfully; otherwise, <see langword="false"/>.</returns>
        public bool RemoveItem(Item item)
        {
            if (item == null)
            {
                return false;
            }

            var slot = Slots.FirstOrDefault(s => s.Item == item);
            if (slot != null)
            {
                return UnequipItem(slot);
            }

            return false;
        }

        /// <summary>
        /// Gets the item equipped in the specified slot.
        /// </summary>
        /// <param name="slot">The slot to query.</param>
        /// <returns>The equipped item, or <see langword="null"/> if the slot is empty.</returns>
        public Item? GetEquippedItem(EquipmentSlot slot)
        {
            if (slot == null)
            {
                return null;
            }

            return slot.Item;
        }

        /// <summary>
        /// Initializes the default equipment slots.
        /// </summary>
        private void InitializeDefaultSlots()
        {
            Slots.Add(new EquipmentSlot("Head", "Helmet"));
            Slots.Add(new EquipmentSlot("Body", "Armor"));
            Slots.Add(new EquipmentSlot("Hands", "Gloves"));
            Slots.Add(new EquipmentSlot("Legs", "Greaves"));
            Slots.Add(new EquipmentSlot("Feet", "Boots"));
            Slots.Add(new EquipmentSlot("Main Hand", "Weapon"));
            Slots.Add(new EquipmentSlot("Off Hand", "Shield"));
            Slots.Add(new EquipmentSlot("Ring 1", "Ring"));
            Slots.Add(new EquipmentSlot("Ring 2", "Ring"));
            Slots.Add(new EquipmentSlot("Amulet", "Amulet"));
        }
    }
}
