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
    /// <summary>
    /// Gets or sets the type of item this pickup grants.
    /// </summary>
    [Export]
    public Inventory.ItemTypes ItemType
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

    private Inventory.ItemTypes itemType;
    private AnimationPlayer anim;
    private Sprite2D sprite;

    /// <inheritdoc/>
    public override void _Ready()
    {
        base._Ready();

        this.anim = this.GetNode<AnimationPlayer>("AnimationPlayer");
        this.sprite = this.GetNode<Sprite2D>("Sprite2D");

        // Update the sprite texture if ItemType was set before _Ready
        if (this.itemType != default(Inventory.ItemTypes))
        {
            this.UpdateSpriteTexture();
        }
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

    /// <summary>
    /// Execute the pickup logic when triggered.
    /// Plays the pickup animation and adds the item to the player's inventory.
    /// </summary>
    protected override async void Execute()
    {
        base.Execute();

        if (this.anim != null)
        {
            this.anim.Play("PickupAnimations/obtain");
            await this.ToSignal(this.anim, AnimationPlayer.SignalName.AnimationFinished);
        }

        // Add the item to the player's inventory
        var inventory = Inventory.Restore();
        if (inventory != null)
        {
            inventory.Add(this.itemType, this.Amount);
        }

        // Remove the pickup from the scene
        this.QueueFree();
    }
}
