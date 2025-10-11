namespace OmegaSpiral.Source.Scripts.Models
{
    using System.Collections.Generic;

    /// <summary>
    /// Represents a preset configuration for an inventory.
    /// </summary>
    public class InventoryPreset
    {
        /// <summary>
        /// Gets or sets the name of the preset.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the list of items in the preset.
        /// </summary>
        public List<Item> Items { get; set; } = new List<Item>();

        /// <summary>
        /// Gets or sets the gold amount in the preset.
        /// </summary>
        public int Gold { get; set; }

        /// <summary>
        /// Gets or sets the description of the preset.
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="InventoryPreset"/> class.
        /// </summary>
        public InventoryPreset()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InventoryPreset"/> class with a name.
        /// </summary>
        /// <param name="name">The name of the preset.</param>
        public InventoryPreset(string name)
        {
            Name = name;
        }
    }
}
