namespace OmegaSpiral.Source.Scripts.Interfaces
{
    using System.Collections.Generic;
    using OmegaSpiral.Source.Scripts.Models;

    /// <summary>
    /// Service interface for managing character equipment.
    /// Provides operations for equipping, unequipping, and querying equipment state.
    /// </summary>
    public interface IEquipmentService
    {
        /// <summary>
        /// Equips an item to the specified equipment slot for a character.
        /// </summary>
        /// <param name="character">The character to equip the item on.</param>
        /// <param name="item">The item to equip.</param>
        /// <param name="slot">The equipment slot to equip the item to.</param>
        /// <returns><see langword="true"/> if the item was successfully equipped; otherwise, <see langword="false"/>.</returns>
        bool EquipItem(Character character, Item item, EquipmentSlot slot);

        /// <summary>
        /// Unequips an item from the specified equipment slot for a character.
        /// </summary>
        /// <param name="character">The character to unequip the item from.</param>
        /// <param name="slot">The equipment slot to unequip.</param>
        /// <returns><see langword="true"/> if the item was successfully unequipped; otherwise, <see langword="false"/>.</returns>
        bool UnequipItem(Character character, EquipmentSlot slot);

        /// <summary>
        /// Gets the item currently equipped in the specified slot.
        /// </summary>
        /// <param name="character">The character to query.</param>
        /// <param name="slot">The equipment slot to query.</param>
        /// <returns>The equipped item, or <see langword="null"/> if the slot is empty.</returns>
        Item? GetEquippedItem(Character character, EquipmentSlot slot);

        /// <summary>
        /// Gets all items currently equipped by the character.
        /// </summary>
        /// <param name="character">The character to query.</param>
        /// <returns>A dictionary mapping equipment slots to equipped items.</returns>
        IReadOnlyDictionary<EquipmentSlot, Item> GetAllEquippedItems(Character character);

        /// <summary>
        /// Checks if an item can be equipped in the specified slot.
        /// </summary>
        /// <param name="character">The character to check for.</param>
        /// <param name="item">The item to check.</param>
        /// <param name="slot">The equipment slot to check.</param>
        /// <returns><see langword="true"/> if the item can be equipped; otherwise, <see langword="false"/>.</returns>
        bool CanEquipItem(Character character, Item item, EquipmentSlot slot);

        /// <summary>
        /// Checks if a slot is currently occupied.
        /// </summary>
        /// <param name="character">The character to check for.</param>
        /// <param name="slot">The equipment slot to check.</param>
        /// <returns><see langword="true"/> if the slot is occupied; otherwise, <see langword="false"/>.</returns>
        bool IsSlotOccupied(Character character, EquipmentSlot slot);
    }
}
