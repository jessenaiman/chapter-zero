using Godot;

/// <summary>
/// An interaction that opens and closes an animated chest and adds an item to the player's inventory.
/// The item is only added the first time the player opens the chest.
/// </summary>
[Tool]
[GlobalClass]
public partial class TreasureChestInteraction : Interaction
{
    /// <summary>
    /// The animation player for the chest animation.
    /// </summary>
    [Export]
    public AnimationPlayer Anim { get; set; }

    /// <summary>
    /// The popup that displays when an item is received.
    /// </summary>
    [Export]
    public Node Popup { get; set; } // InteractionPopup

    /// <summary>
    /// The type of item in the chest.
    /// </summary>
    public int ItemType { get; set; } // Inventory.ItemTypes enum value

    /// <summary>
    /// The amount of the item in the chest.
    /// </summary>
    public int Amount { get; set; } = 1;

    private bool _isOpen = false;
    private bool _itemReceived = false;

    /// <summary>
    /// Whether the item has been received from this chest.
    /// </summary>
    public bool ItemReceived
    {
        get => _itemReceived;
        set
        {
            _itemReceived = value;
            if (_itemReceived && Popup != null)
            {
                Popup.Call("hide_and_free");
            }
        }
    }

    /// <summary>
    /// Open or close the chest, depending on whether it is closed or open.
    /// If this is the first time opening it, apply the items inside to the player's inventory.
    /// </summary>
    protected async void Execute()
    {
        if (_isOpen)
        {
            Anim?.Play("close");
            if (Anim != null)
            {
                await ToSignal(Anim, AnimationPlayer.SignalName.AnimationFinished);
            }
            _isOpen = false;
        }
        else
        {
            Anim?.Play("open");
            if (Anim != null)
            {
                await ToSignal(Anim, AnimationPlayer.SignalName.AnimationFinished);
            }

            if (!_itemReceived)
            {
                // Get the Inventory singleton and add the item
                var inventory = GetNode("/root/Inventory");
                if (inventory != null)
                {
                    inventory.Call("add", ItemType, Amount);
                }
                ItemReceived = true;
            }

            _isOpen = true;
        }
    }

    public override async void Run()
    {
        Execute();
        await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
        base.Run();
    }
}
