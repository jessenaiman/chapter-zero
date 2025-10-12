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
    /// The item type ID from the Inventory.ItemTypes enum.
    /// </summary>
    public int ID { get; set; } = 0; // Inventory.ItemTypes.KEY

    private int _count = 0;
    private Label _countLabel;

    /// <summary>
    /// The number of this item in the inventory.
    /// </summary>
    public int Count
    {
        get => _count;
        set
        {
            _count = Mathf.Max(value, 0);

            if (_count == 0)
            {
                QueueFree();
            }
            else if (_count > 1 && _countLabel != null)
            {
                _countLabel.Show();
                _countLabel.Text = _count.ToString();
            }
            else if (_countLabel != null)
            {
                _countLabel.Hide();
            }
        }
    }

    public override void _Ready()
    {
        base._Ready();
        _countLabel = GetNode<Label>("Count");
    }
}
