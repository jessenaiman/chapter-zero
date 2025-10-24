
// <copyright file="UiInventoryItem.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using Godot;

namespace OmegaSpiral.Source.Scripts.Field.Ui.Inventory;
/// <summary>
/// An inventory item, tracking both its Ui representation and underlying data.
/// Will be replaced in future iterations of the OpenRPG project.
/// Please see <see cref="UiInventory"/> for additional information.
/// </summary>
[GlobalClass]
public partial class UiInventoryItem : TextureRect
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
                this.countLabel.Text = this.count.ToString(System.Globalization.CultureInfo.InvariantCulture);
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
