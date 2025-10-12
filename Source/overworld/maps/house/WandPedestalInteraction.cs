using Godot;
using Godot.Collections;
using System.Linq;

/// <summary>
/// A puzzle interaction where the player must place the correct colored wand on each pedestal.
/// When all pedestals have the correct wands, the puzzle is solved.
/// </summary>
[Tool]
[GlobalClass]
public partial class WandPedestalInteraction : Node // Should extend ConversationTemplate when available
{
    /// <summary>
    /// The pedestal should only accept a subset of inventory items: a type of wand.
    /// We also accept an option to pull a wand off the pedestal (-1).
    /// </summary>
    private static readonly int[] ValidItems = new int[]
    {
        // Inventory.ItemTypes.RED_WAND,
        // Inventory.ItemTypes.BLUE_WAND,
        // Inventory.ItemTypes.GREEN_WAND,
        -1
    };

    /// <summary>
    /// A list of ALL pedestals in the current scene that have the correct wand placed on them.
    /// The keys will be the pedestal itself, and the values whether or not a given pedestal is correct.
    /// </summary>
    private static System.Collections.Generic.Dictionary<WandPedestalInteraction, bool> _correctPedestals = new System.Collections.Generic.Dictionary<WandPedestalInteraction, bool>();

    /// <summary>
    /// Link the obstacle's animation player to this object for when the puzzle is solved.
    /// </summary>
    [Export]
    public AnimationPlayer SolvedAnimation { get; set; } = null!;

    /// <summary>
    /// Specify a timeline that should be run if an item is already placed on the pedestal.
    /// </summary>
    [Export]
    public Resource WandPlacedTimeline { get; set; } = null!; // DialogicTimeline

    /// <summary>
    /// Specify which wand color this pedestal expects.
    /// </summary>
    [Export(PropertyHint.Enum, "Red,Blue,Green")]
    public string PedestalRequirement { get; set; } = "Red";

    /// <summary>
    /// Keep track of the id of the item currently placed on the pedestal, from Inventory.ItemTypes enum.
    /// </summary>
    private int _currentItemId = -1;

    /// <summary>
    /// The timeline to run when no item is on the pedestal.
    /// </summary>
    private Resource _unoccupiedTimeline = null!; // Renamed from timeline in InteractionTemplateConversation

    private Sprite2D _sprite = null!;

    public override void _Ready()
    {
        base._Ready();

        if (!Engine.IsEditorHint())
        {
            GD.PushError("WandPedestalInteraction requires implementation: SolvedAnimation assertion");
            // assert(SolvedAnimation, "This interaction requires the obstacle's animation player!");

            // Setup the static variable to account for this pedestal
            _correctPedestals[this] = false;

            _sprite = GetNode<Sprite2D>("Sprite2D");

            // Connect to inventory signals
            var inventory = GetNode("/root/Inventory");
            if (inventory != null)
            {
                inventory.Connect("item_changed", Callable.From((int itemType) =>
                {
                    OnInventoryItemChanged(itemType, inventory);
                }));
            }
        }
    }

    /// <summary>
    /// Execute the interaction logic.
    /// </summary>
    protected async void Execute()
    {
        // Run the default timeline unless there is already something on this pedestal
        Resource timelineToRun = _unoccupiedTimeline;
        if (_sprite?.Texture != null)
        {
            timelineToRun = WandPlacedTimeline;
        }

        // Start the timeline
        var dialogic = GetNode("/root/Dialogic");
        if (dialogic != null && timelineToRun != null)
        {
            dialogic.Call("start_timeline", timelineToRun);
            await ToSignal(dialogic, "timeline_ended");
        }

        // Check to see if the puzzle has been solved
        if (IsPuzzleSolved())
        {
            // Deactivate the pedestal interactions so they cannot be interacted with
            foreach (var pedestal in _correctPedestals.Keys)
            {
                pedestal.Set("is_active", false);
            }

            // The referenced animation player will resolve the puzzle
            if (SolvedAnimation != null)
            {
                SolvedAnimation.Play("solve");
                await ToSignal(SolvedAnimation, AnimationPlayer.SignalName.AnimationFinished);
            }
        }
    }

    /// <summary>
    /// Check to see if ALL pedestals have the correct wand placed on them.
    /// </summary>
    private bool IsPuzzleSolved()
    {
        return _correctPedestals.Values.All(value => value);
    }

    /// <summary>
    /// This responds to a signal event within a Dialogic timeline.
    /// The following method ensures that the designer has passed a correct item id string from Dialogic,
    /// and either adds a wand to the pedestal or pulls one off.
    /// Argument is expected to be the key (item type) of the Inventory.ItemTypes enum.
    /// </summary>
    private void OnDialogicSignalEvent(string argument)
    {
        // Get the Inventory singleton
        var inventory = GetNode("/root/Inventory");
        if (inventory == null)
        {
            return;
        }

        // Convert the argument into an item id
        var itemTypes = (Godot.Collections.Dictionary)inventory.Get("ItemTypes");
        int itemId = -1;
        if (itemTypes != null && itemTypes.ContainsKey(argument.ToUpper()))
        {
            itemId = (int)itemTypes[argument.ToUpper()];
        }

        if (!ValidItems.Contains(itemId))
        {
            return;
        }

        // Convert the pedestal requirement to an item id
        string expectedWand = PedestalRequirement.ToUpper() + "_WAND";
        int expectedWandId = -1;
        if (itemTypes != null && itemTypes.ContainsKey(expectedWand))
        {
            expectedWandId = (int)itemTypes[expectedWand];
        }

        // Handle item placement/removal
        if (itemId < 0)
        {
            if (_currentItemId >= 0)
            {
                inventory.Call("add", _currentItemId);
            }
        }
        else
        {
            inventory.Call("remove", itemId);
        }

        // Update the sprite texture
        _currentItemId = itemId;
        _sprite.Texture = (Texture2D)inventory.Call("get_item_icon", itemId);

        // Flag whether or not this pedestal has the correct wand placed on it
        _correctPedestals[this] = _currentItemId == expectedWandId;
    }

    /// <summary>
    /// Match puzzle-specific variables to the player's inventory.
    /// </summary>
    private void OnInventoryItemChanged(int itemType, Node inventory)
    {
        var dialogic = GetNode("/root/Dialogic");
        if (dialogic == null)
        {
            return;
        }

        // TODO: Map itemType to appropriate Dialogic variable
        // This would need the Inventory.ItemTypes enum values
        int itemCount = (int)inventory.Call("get_item_count", itemType);

        // Set Dialogic variables based on item type
        // dialogic.Call("VAR.set_variable", "RedWandCount", itemCount);
        // dialogic.Call("VAR.set_variable", "BlueWandCount", itemCount);
        // dialogic.Call("VAR.set_variable", "GreenWandCount", itemCount);
    }
}
