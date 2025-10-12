// <copyright file="TreasureChest.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using Godot;

/// <summary>
/// A treasure chest gamepiece that contains an item for the player to collect.
/// </summary>
[Tool]
[GlobalClass]
public partial class TreasureChest : Gamepiece
{
    private Interaction? interaction;

    /// <summary>
    /// Gets or sets the type of item contained in the chest.
    /// </summary>
    [Export]
    public int ItemType { get; set; } // Inventory.ItemTypes enum value

    /// <summary>
    /// Gets or sets the amount of the item in the chest.
    /// </summary>
    [Export]
    public int Amount { get; set; } = 1;

    /// <inheritdoc/>
    public override void _Ready()
    {
        base._Ready();

        if (!Engine.IsEditorHint())
        {
            this.interaction = this.GetNode<Interaction>("Interaction");
            if (this.interaction != null)
            {
                this.interaction.Set("item_type", this.ItemType);
                this.interaction.Set("amount", this.Amount);
            }
        }
    }
}
