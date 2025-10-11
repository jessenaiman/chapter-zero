namespace OmegaSpiral.Source.Scripts.Interfaces
{
    using Godot;
    using OmegaSpiral.Source.Scripts.Models;

    /// <summary>
    /// Factory interface for creating UI display elements for equipment slots and items.
    /// </summary>
    public interface IEquipmentDisplayFactory
    {
        /// <summary>
        /// Creates a UI control for displaying an equipment slot.
        /// </summary>
        /// <param name="slot">The equipment slot to create a display for.</param>
        /// <returns>The created control.</returns>
        Control CreateEquipmentSlotDisplay(EquipmentSlot slot);

        /// <summary>
        /// Creates a UI control for displaying an equipped item.
        /// </summary>
        /// <param name="item">The item to create a display for.</param>
        /// <returns>The created control.</returns>
        Control CreateEquippedItemDisplay(Item item);

        /// <summary>
        /// Updates an existing equipment slot display with current data.
        /// </summary>
        /// <param name="slot">The equipment slot to update.</param>
        /// <param name="display">The control to update.</param>
        void UpdateEquipmentSlotDisplay(EquipmentSlot slot, Control display);

        /// <summary>
        /// Updates an existing equipped item display with current data.
        /// </summary>
        /// <param name="item">The item to update.</param>
        /// <param name="display">The control to update.</param>
        void UpdateEquippedItemDisplay(Item item, Control display);
    }
}
