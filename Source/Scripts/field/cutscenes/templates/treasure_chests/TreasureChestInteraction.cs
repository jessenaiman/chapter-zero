// <copyright file="TreasureChestInteraction.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using Godot;

/// <summary>
/// An interaction that opens and closes an animated chest and adds an item to the player's inventory.
/// The item is only added the first time the player opens the chest.
/// </summary>
[Tool]
[GlobalClass]
public partial class TreasureChestInteraction : Interaction
{
    private bool isOpen;
    private bool itemReceived;

    /// <summary>
    /// Gets or sets the animation player for the chest animation.
    /// </summary>
    [Export]
    public AnimationPlayer? Anim { get; set; }

    /// <summary>
    /// Gets or sets the popup that displays when an item is received.
    /// </summary>
    [Export]
    public Node? Popup { get; set; } // InteractionPopup

    /// <summary>
    /// Gets or sets the type of item in the chest.
    /// </summary>
    public int ItemType { get; set; } // Inventory.ItemTypes enum value

    /// <summary>
    /// Gets or sets the amount of the item in the chest.
    /// </summary>
    public int Amount { get; set; } = 1;

    /// <summary>
    /// Gets or sets a value indicating whether whether the item has been received from this chest.
    /// </summary>
    public bool ItemReceived
    {
        get => this.itemReceived;
        set
        {
            this.itemReceived = value;
            if (this.itemReceived && this.Popup != null)
            {
                this.Popup.Call("hide_and_free");
            }
        }
    }

    /// <inheritdoc/>
    public override async void Run()
    {
        this.Execute();
        await this.ToSignal(this.GetTree(), SceneTree.SignalName.ProcessFrame);
        base.Run();
    }

    /// <summary>
    /// Open or close the chest, depending on whether it is closed or open.
    /// If this is the first time opening it, apply the items inside to the player's inventory.
    /// </summary>
    protected async void Execute()
    {
        if (this.isOpen)
        {
            this.Anim?.Play("close");
            if (this.Anim != null)
            {
                await this.ToSignal(this.Anim, AnimationPlayer.SignalName.AnimationFinished);
            }

            this.isOpen = false;
        }
        else
        {
            this.Anim?.Play("open");
            if (this.Anim != null)
            {
                await this.ToSignal(this.Anim, AnimationPlayer.SignalName.AnimationFinished);
            }

            if (!this.itemReceived)
            {
                // Get the Inventory singleton and add the item
                var inventory = this.GetNode("/root/Inventory");
                if (inventory != null)
                {
                    inventory.Call("add", this.ItemType, this.Amount);
                }

                this.ItemReceived = true;
            }

            this.isOpen = true;
        }
    }
}
