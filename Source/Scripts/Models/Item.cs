namespace OmegaSpiral.Source.Scripts.Models
{
    using System.Collections.Generic;
    using Godot;

    /// <summary>
    /// Represents an item in the game inventory system.
    /// </summary>
    public partial class Item : Resource
    {
        /// <summary>
        /// Item type categories.
        /// </summary>
        public enum ItemType
        {
            /// <summary>Weapon item.</summary>
            Weapon,

            /// <summary>Armor item.</summary>
            Armor,

            /// <summary>Consumable item.</summary>
            Consumable,

            /// <summary>Quest item.</summary>
            QuestItem,

            /// <summary>Miscellaneous item.</summary>
            Miscellaneous,
        }

        /// <summary>
        /// Gets or sets the unique identifier for this item.
        /// </summary>
        [Export]
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the display name of the item.
        /// </summary>
        [Export]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the item description.
        /// </summary>
        [Export]
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the item icon texture path.
        /// </summary>
        [Export]
        public string IconPath { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the item type category.
        /// </summary>
        [Export]
        public ItemType Type { get; set; } = ItemType.Miscellaneous;

        /// <summary>
        /// Gets or sets the item value in gold.
        /// </summary>
        [Export]
        public int Value { get; set; }

        /// <summary>
        /// Gets or sets the item weight.
        /// </summary>
        [Export]
        public float Weight { get; set; }

        /// <summary>
        /// Gets or sets whether this item is stackable.
        /// </summary>
        [Export]
        public bool IsStackable { get; set; }

        /// <summary>
        /// Gets or sets the maximum stack size.
        /// </summary>
        [Export]
        public int MaxStackSize { get; set; } = 1;

        /// <summary>
        /// Gets or sets the item rarity level.
        /// </summary>
        [Export]
        public int Rarity { get; set; }

        /// <summary>
        /// Gets or sets whether this item can be used.
        /// </summary>
        [Export]
        public bool IsUsable { get; set; }

        /// <summary>
        /// Gets or sets the target scope for usable items.
        /// </summary>
        [Export]
        public string TargetScope { get; set; } = string.Empty;
    }
}
