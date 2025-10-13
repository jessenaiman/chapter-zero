// <copyright file="Pickup.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using Godot;

/// <summary>
/// A pickup item that can be collected by the player.
/// Pickups are interactive objects that the player can collect by triggering them.
/// They typically grant items, currency, or other bonuses to the player.
/// </summary>
[Tool]
public partial class Pickup : Trigger
{
    private Inventory.ItemType itemType;
    private AnimationPlayer? anim;
    private Sprite2D? sprite;

    /// <summary>
    /// Gets or sets the type of item this pickup grants.
    /// </summary>
    [Export]
    public Inventory.ItemType ItemType
    {
        get => this.itemType;
        set
        {
            this.itemType = value;

            if (!this.IsInsideTree())
            {
                // Wait for the node to be ready before accessing children
                // In C# we would typically use a callback or await pattern
                // For now, we'll defer the texture update
                this.CallDeferred(nameof(this.UpdateSpriteTexture));
            }
            else
            {
                this.UpdateSpriteTexture();
            }
        }
    }

    /// <summary>
    /// Gets or sets the amount of the item to grant when picked up.
    /// </summary>
    [Export]
    public int Amount { get; set; } = 1;

    /// <inheritdoc/>
    public override void _Ready()
    {
        base._Ready();

        this.anim = this.GetNode<AnimationPlayer>("AnimationPlayer");
        this.sprite = this.GetNode<Sprite2D>("Sprite2D");

        // Update the sprite texture if ItemType was set before _Ready
        if (this.itemType != default(Inventory.ItemType))
        {
            this.UpdateSpriteTexture();
        }
    }

    /// <summary>
    /// Execute the pickup logic when triggered.
    /// Plays the pickup animation and adds the item to the player's inventory.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    protected override async Task ExecuteAsync()
    {
        if (this.anim != null)
        {
            this.anim.Play("PickupAnimations/obtain");
            await this.ToSignal(this.anim, AnimationPlayer.SignalName.AnimationFinished);
        }

        // Add the item to the player's inventory
        using (var inventory = Inventory.Restore())
        {
            if (inventory != null)
            {
                inventory.Add(this.itemType, this.Amount);
            }
        }

        // Remove the pickup from the scene
        this.QueueFree();
    }

    /// <summary>
    /// Update the sprite texture to match the item type.
    /// </summary>
    private void UpdateSpriteTexture()
    {
        if (this.sprite != null)
        {
            this.sprite.Texture = Inventory.GetItemIcon(this.itemType);
        }
    }
}
