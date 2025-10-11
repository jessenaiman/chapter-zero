namespace OmegaSpiral.Source.Scripts.Models
{
    using Godot;

    /// <summary>
    /// Represents an equipment slot that can hold an item.
    /// </summary>
    public class EquipmentSlot
    {
        /// <summary>
        /// Gets or sets the name of the equipment slot (e.g., "Head", "Weapon", "Shield").
        /// </summary>
        [Export]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the item currently equipped in this slot.
        /// </summary>
        public Item? Item { get; set; }

        /// <summary>
        /// Gets or sets the icon texture to display when the slot is empty.
        /// </summary>
        public Texture2D? EmptyIcon { get; set; }

        /// <summary>
        /// Gets or sets the type of items that can be equipped in this slot.
        /// </summary>
        [Export]
        public string SlotType { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether this slot is required (cannot be empty).
        /// </summary>
        [Export]
        public bool IsRequired { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="EquipmentSlot"/> class.
        /// </summary>
        public EquipmentSlot()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EquipmentSlot"/> class with a name and slot type.
        /// </summary>
        /// <param name="name">The name of the slot.</param>
        /// <param name="slotType">The type of items this slot accepts.</param>
        public EquipmentSlot(string name, string slotType)
        {
            Name = name;
            SlotType = slotType;
        }
    }
}
