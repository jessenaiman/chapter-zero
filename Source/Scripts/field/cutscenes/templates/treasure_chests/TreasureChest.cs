using Godot;

/// <summary>
/// A treasure chest gamepiece that contains an item for the player to collect.
/// </summary>
[Tool]
[GlobalClass]
public partial class TreasureChest : Gamepiece
{
    /// <summary>
    /// The type of item contained in the chest.
    /// </summary>
    [Export]
    public int ItemType { get; set; } // Inventory.ItemTypes enum value

    /// <summary>
    /// The amount of the item in the chest.
    /// </summary>
    [Export]
    public int Amount { get; set; } = 1;

    private Interaction _interaction;

    public override void _Ready()
    {
        base._Ready();

        if (!Engine.IsEditorHint())
        {
            _interaction = GetNode<Interaction>("Interaction");
            if (_interaction != null)
            {
                _interaction.Set("item_type", ItemType);
                _interaction.Set("amount", Amount);
            }
        }
    }
}
