namespace OmegaSpiral.Source.Scripts.Services
{
    using System.Collections.Generic;
    using OmegaSpiral.Source.Scripts.Models;
    using OmegaSpiral.Source.Scripts.Interfaces;

    /// <summary>
    /// Default implementation of the equipment management service.
    /// Handles equipping, unequipping, and querying equipment state for characters.
    /// </summary>
    public class EquipmentService : IEquipmentService
    {
        /// <inheritdoc/>
        public bool EquipItem(Character character, Item item, EquipmentSlot slot)
        {
            if (character == null || item == null)
            {
                return false;
            }

            if (!CanEquipItem(character, item, slot))
            {
                return false;
            }

            // Unequip current item in slot if present
            if (IsSlotOccupied(character, slot))
            {
                UnequipItem(character, slot);
            }

            // Add item to character's equipped items
            character.Equipment[slot] = item;

            // Apply equipment bonuses
            ApplyEquipmentBonuses(character, item);

            return true;
        }

        /// <inheritdoc/>
        public bool UnequipItem(Character character, EquipmentSlot slot)
        {
            if (character == null || !IsSlotOccupied(character, slot))
            {
                return false;
            }

            Item? item = character.Equipment[slot];
            if (item != null)
            {
                // Remove equipment bonuses
                RemoveEquipmentBonuses(character, item);

                // Remove from equipped items
                character.Equipment.Remove(slot);

                // Add to inventory if there's space
                if (character.Inventory != null && character.Inventory.CanAddItem(item))
                {
                    character.Inventory.AddItem(item);
                }
            }

            return true;
        }

        /// <inheritdoc/>
        public Item? GetEquippedItem(Character character, EquipmentSlot slot)
        {
            if (character == null || character.Equipment == null)
            {
                return null;
            }

            return character.Equipment.TryGetValue(slot, out Item? item) ? item : null;
        }

        /// <inheritdoc/>
        public IReadOnlyDictionary<EquipmentSlot, Item> GetAllEquippedItems(Character character)
        {
            if (character == null || character.Equipment == null)
            {
                return new Dictionary<EquipmentSlot, Item>();
            }

            return character.Equipment;
        }

        /// <inheritdoc/>
        public bool CanEquipItem(Character character, Item item, EquipmentSlot slot)
        {
            if (character == null || item == null)
            {
                return false;
            }

            // Check if item can be equipped in this slot
            if (!item.CanEquipInSlot(slot))
            {
                return false;
            }

            // Check character level requirements
            if (item.RequiredLevel > character.Level)
            {
                return false;
            }

            // Check class requirements
            if (item.RequiredClass != null && !character.Classes.Contains(item.RequiredClass))
            {
                return false;
            }

            return true;
        }

        /// <inheritdoc/>
        public bool IsSlotOccupied(Character character, EquipmentSlot slot)
        {
            return character != null
                && character.Equipment != null
                && character.Equipment.ContainsKey(slot);
        }

        /// <summary>
        /// Applies stat bonuses and effects from equipped item to character.
        /// </summary>
        /// <param name="character">The character to apply bonuses to.</param>
        /// <param name="item">The item providing the bonuses.</param>
        private void ApplyEquipmentBonuses(Character character, Item item)
        {
            if (item.StatBonuses == null)
            {
                return;
            }

            foreach (var bonus in item.StatBonuses)
            {
                character.Stats.ApplyBonus(bonus.Key, bonus.Value);
            }

            // Apply special effects
            if (item.SpecialEffects != null)
            {
                foreach (var effect in item.SpecialEffects)
                {
                    character.ApplyEffect(effect);
                }
            }
        }

        /// <summary>
        /// Removes stat bonuses and effects from unequipped item.
        /// </summary>
        /// <param name="character">The character to remove bonuses from.</param>
        /// <param name="item">The item whose bonuses should be removed.</param>
        private void RemoveEquipmentBonuses(Character character, Item item)
        {
            if (item.StatBonuses == null)
            {
                return;
            }

            foreach (var bonus in item.StatBonuses)
            {
                character.Stats.RemoveBonus(bonus.Key, bonus.Value);
            }

            // Remove special effects
            if (item.SpecialEffects != null)
            {
                foreach (var effect in item.SpecialEffects)
                {
                    character.RemoveEffect(effect);
                }
            }
        }
    }
}
