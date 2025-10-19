
// <copyright file="WandPedestalInteraction.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using System.Globalization;
using System.Linq;
using Godot;

namespace OmegaSpiral.Source.Overworld.Maps.house;
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
        -1,
    };

    /// <summary>
    /// A list of ALL pedestals in the current scene that have the correct wand placed on them.
    /// The keys will be the pedestal itself, and the values whether or not a given pedestal is correct.
    /// </summary>
    private static System.Collections.Generic.Dictionary<WandPedestalInteraction, bool> correctPedestals = new System.Collections.Generic.Dictionary<WandPedestalInteraction, bool>();

    /// <summary>
    /// Keep track of the id of the item currently placed on the pedestal, from Inventory.ItemTypes enum.
    /// </summary>
    private int currentItemId = -1;

    /// <summary>
    /// The timeline to run when no item is on the pedestal.
    /// </summary>
    private Resource unoccupiedTimeline = null!; // Renamed from timeline in InteractionTemplateConversation

    private Sprite2D sprite = null!;

    /// <summary>
    /// Gets or sets link the obstacle's animation player to this object for when the puzzle is solved.
    /// </summary>
    [Export]
    public AnimationPlayer SolvedAnimation { get; set; } = null!;

    /// <summary>
    /// Gets or sets specify a timeline that should be run if an item is already placed on the pedestal.
    /// </summary>
    [Export]
    public Resource WandPlacedTimeline { get; set; } = null!; // DialogicTimeline

    /// <summary>
    /// Gets or sets specify which wand color this pedestal expects.
    /// </summary>
    [Export(PropertyHint.Enum, "Red,Blue,Green")]
    public string PedestalRequirement { get; set; } = "Red";

    /// <inheritdoc/>
    public override void _Ready()
    {
        base._Ready();

        if (!Engine.IsEditorHint())
        {
            GD.PushError("WandPedestalInteraction requires implementation: SolvedAnimation assertion");

            // assert(SolvedAnimation, "This interaction requires the obstacle's animation player!");

            // Setup the static variable to account for this pedestal
            correctPedestals[this] = false;

            this.sprite = this.GetNode<Sprite2D>("Sprite2D");

            // Connect to inventory signals
            var inventory = this.GetNode("/root/Inventory");
            if (inventory != null)
            {
                inventory.Connect("item_changed", Callable.From((int itemType) => this.OnInventoryItemChanged(itemType, inventory)));
            }
        }
    }

    /// <summary>
    /// Execute the interaction logic.
    /// </summary>
    protected async void Execute()
    {
        // Run the default timeline unless there is already something on this pedestal
        Resource timelineToRun = this.unoccupiedTimeline;
        if (this.sprite?.Texture != null)
        {
            timelineToRun = this.WandPlacedTimeline;
        }

        // Start the timeline
        var dialogic = this.GetNode("/root/Dialogic");
        if (dialogic != null && timelineToRun != null)
        {
            dialogic.Call("start_timeline", timelineToRun);
            await this.ToSignal(dialogic, "timeline_ended");
        }

        // Check to see if the puzzle has been solved
        if (IsPuzzleSolved())
        {
            // Deactivate the pedestal interactions so they cannot be interacted with
            foreach (var pedestal in correctPedestals.Keys)
            {
                pedestal.Set("is_active", false);
            }

            // The referenced animation player will resolve the puzzle
            if (this.SolvedAnimation != null)
            {
                this.SolvedAnimation.Play("solve");
                await this.ToSignal(this.SolvedAnimation, AnimationPlayer.SignalName.AnimationFinished);
            }
        }
    }

    /// <summary>
    /// Check to see if ALL pedestals have the correct wand placed on them.
    /// </summary>
    private static bool IsPuzzleSolved()
    {
        return correctPedestals.Values.All(value => value);
    }

    /// <summary>
    /// This responds to a signal event within a Dialogic timeline.
    /// The following method ensures that the designer has passed a correct item id string from Dialogic,
    /// and either adds a wand to the pedestal or pulls one off.
    /// Argument is expected to be the key (item type) of the Inventory.ItemTypes enum.
    /// </summary>
    /// <param name="argument"></param>
    private void OnDialogicSignalEvent(string argument)
    {
        var inventory = this.GetNode("/root/Inventory");
        if (inventory == null)
        {
            return;
        }

        int itemId = GetItemIdFromArgument(argument, inventory);
        if (!ValidItems.Contains(itemId))
        {
            return;
        }

        int expectedWandId = GetExpectedWandId(inventory, this.PedestalRequirement);
        UpdatePedestalState(itemId, expectedWandId, inventory);
    }

    /// <summary>
    /// Converts a string argument to an item ID using the inventory's ItemTypes dictionary.
    /// </summary>
    /// <param name="argument">The item type string to convert.</param>
    /// <param name="inventory">The inventory node.</param>
    /// <returns>The item ID, or -1 if not found.</returns>
    private static int GetItemIdFromArgument(string argument, Node inventory)
    {
        var itemTypes = (Godot.Collections.Dictionary) inventory.Get("ItemTypes");
        if (itemTypes != null && itemTypes.ContainsKey(argument.ToUpper(CultureInfo.InvariantCulture)))
        {
            return (int) itemTypes[argument.ToUpper(CultureInfo.InvariantCulture)];
        }
        return -1;
    }

    /// <summary>
    /// Gets the expected wand ID for this pedestal based on its requirement.
    /// </summary>
    /// <param name="inventory">The inventory node.</param>
    /// <param name="pedestalRequirement">The pedestal's requirement string.</param>
    /// <returns>The expected wand item ID, or -1 if not found.</returns>
    private static int GetExpectedWandId(Node inventory, string pedestalRequirement)
    {
        var itemTypes = (Godot.Collections.Dictionary) inventory.Get("ItemTypes");
        string expectedWand = pedestalRequirement.ToUpper(CultureInfo.InvariantCulture) + "_WAND";
        if (itemTypes != null && itemTypes.ContainsKey(expectedWand))
        {
            return (int) itemTypes[expectedWand];
        }
        return -1;
    }

    /// <summary>
    /// Updates the pedestal's state with the new item and checks if it's correct.
    /// </summary>
    /// <param name="itemId">The new item ID to place on the pedestal.</param>
    /// <param name="expectedWandId">The expected wand ID for correctness check.</param>
    /// <param name="inventory">The inventory node.</param>
    private void UpdatePedestalState(int itemId, int expectedWandId, Node inventory)
    {
        // Handle item placement/removal
        if (itemId < 0)
        {
            if (this.currentItemId >= 0)
            {
                inventory.Call("add", this.currentItemId);
            }
        }
        else
        {
            inventory.Call("remove", itemId);
        }

        // Update the sprite texture
        this.currentItemId = itemId;
        this.sprite.Texture = (Texture2D) inventory.Call("get_item_icon", itemId);

        // Flag whether or not this pedestal has the correct wand placed on it
        correctPedestals[this] = this.currentItemId == expectedWandId;
    }

    /// <summary>
    /// Match puzzle-specific variables to the player's inventory.
    /// </summary>
    /// <param name="itemType"></param>
    /// <param name="inventory"></param>
    private void OnInventoryItemChanged(int itemType, Node inventory)
    {
        var dialogic = this.GetNode("/root/Dialogic");
        if (dialogic == null)
        {
            return;
        }

        // TODO: Map itemType to appropriate Dialogic variable
        // This would need the Inventory.ItemTypes enum values
        int itemCount = (int) inventory.Call("get_item_count", itemType);

        // Set Dialogic variables based on item type
        // dialogic.Call("VAR.set_variable", "RedWandCount", itemCount);
        // dialogic.Call("VAR.set_variable", "BlueWandCount", itemCount);
        // dialogic.Call("VAR.set_variable", "GreenWandCount", itemCount);
    }
}
