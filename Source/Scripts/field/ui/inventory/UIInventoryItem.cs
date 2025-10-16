namespace OmegaSpiral.Source.Scripts.Field.UI.Inventory;

// <copyright file="UIInventoryItem.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using Godot;

/// <summary>
/// An inventory item, tracking both its UI representation and underlying data.
/// Will be replaced in future iterations of the OpenRPG project.
/// Please see <see cref="UIInventory"/> for additional information.
/// </summary>
[GlobalClass]
public partial class UIInventoryItem : TextureRect
{
    /// <summary>
    /// Gets or sets the item type ID from the Inventory.ItemTypes enum.
    /// </summary>
    public int ID { get; set; } // Inventory.ItemTypes.KEY

    private int count;
    private Label? countLabel;

    /// <summary>
    /// Gets or sets the number of this item in the inventory.
    /// </summary>
    public int Count
    {
        get => this.count;
        set
        {
            this.count = Mathf.Max(value, 0);

            if (this.count == 0)
            {
                this.QueueFree();
            }
            else if (this.count > 1 && this.countLabel != null)
            {
                this.countLabel.Show();
                this.countLabel.Text = this.count.ToString();
            }
            else if (this.countLabel != null)
            {
                this.countLabel.Hide();
            }
        }
    }

    /// <inheritdoc/>
    public override void _Ready()
    {
        base._Ready();
        this.countLabel = this.GetNode<Label>("Count");
    }
}
