using Godot;

/// <summary>
/// Interaction that handles unlocking and opening doors.
/// When triggered, this interaction checks if the player has a key and uses it to unlock the door.
/// The door will then open automatically.
/// </summary>
[Tool]
public partial class DoorUnlockInteraction : Interaction
{
    /// <summary>
    /// The door that this interaction controls.
    /// </summary>
    [Export]
    public Door Door { get; set; } = null!;

    private InteractionPopup _popup = null!;

    public override void _Ready()
    {
        base._Ready();

        // Get references to child nodes
        Door = GetParent<Door>();
        _popup = GetNode<InteractionPopup>("InteractionPopup");
    }

    /// <summary>
    /// Execute the door unlock interaction.
    /// Checks if the door is locked and if the player has a key to unlock it.
    /// If successful, unlocks and opens the door.
    /// </summary>
    public override void Run()
    {
        base.Run();

        if (Door.IsLocked)
        {
            var inventory = Inventory.Restore();
            if (inventory != null && inventory.GetItemCount(Inventory.ItemTypes.Key) > 0)
            {
                inventory.Remove(Inventory.ItemTypes.Key, 1);
                Door.IsLocked = false;
                IsActive = false;
                _popup.IsActive = false;
            }
        }

        Door.Open();
    }
}
