using Godot;

/// <summary>
/// A pickup item that can be collected by the player.
/// Pickups are interactive objects that the player can collect by triggering them.
/// They typically grant items, currency, or other bonuses to the player.
/// </summary>
[Tool]
public partial class Pickup : Trigger
{
    /// <summary>
    /// The type of item this pickup grants.
    /// </summary>
    [Export]
    public Inventory.ItemTypes ItemType
    {
        get => itemType;
        set
        {
            itemType = value;

            if (!IsInsideTree())
            {
                // Wait for the node to be ready before accessing children
                // In C# we would typically use a callback or await pattern
                // For now, we'll defer the texture update
                CallDeferred(nameof(UpdateSpriteTexture));
            }
            else
            {
                UpdateSpriteTexture();
            }
        }
    }

    /// <summary>
    /// The amount of the item to grant when picked up.
    /// </summary>
    [Export]
    public int Amount { get; set; } = 1;

    private Inventory.ItemTypes itemType;
    private AnimationPlayer anim;
    private Sprite2D sprite;

    public override void _Ready()
    {
        base._Ready();

        anim = GetNode<AnimationPlayer>("AnimationPlayer");
        sprite = GetNode<Sprite2D>("Sprite2D");

        // Update the sprite texture if ItemType was set before _Ready
        if (itemType != default(Inventory.ItemTypes))
        {
            UpdateSpriteTexture();
        }
    }

    /// <summary>
    /// Update the sprite texture to match the item type.
    /// </summary>
    private void UpdateSpriteTexture()
    {
        if (sprite != null)
        {
            sprite.Texture = Inventory.GetItemIcon(itemType);
        }
    }

    /// <summary>
    /// Execute the pickup logic when triggered.
    /// Plays the pickup animation and adds the item to the player's inventory.
    /// </summary>
    protected override async void _Execute()
    {
        base._Execute();

        if (anim != null)
        {
            anim.Play("PickupAnimations/obtain");
            await ToSignal(anim, AnimationPlayer.SignalName.AnimationFinished);
        }

        // Add the item to the player's inventory
        var inventory = Inventory.Restore();
        if (inventory != null)
        {
            inventory.Add(itemType, Amount);
        }

        // Remove the pickup from the scene
        QueueFree();
    }
}
