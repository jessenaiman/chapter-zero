// <copyright file="DoorUnlockInteraction.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using Godot;
using OmegaSpiral.Field.Cutscenes.Templates.Doors;

namespace OmegaSpiral.Overworld.Maps.Town
{
    /// <summary>
    /// Interaction that handles unlocking and opening doors.
    /// When triggered, this interaction checks if the player has a key and uses it to unlock the door.
    /// The door will then open automatically.
    /// </summary>
    [Tool]
    public partial class DoorUnlockInteraction : Interaction
    {
        /// <summary>
        /// Gets or sets the door that this interaction controls.
        /// </summary>
        [Export]
        public Node DoorNode { get; set; } = null!;

        private InteractionPopup popup = null!;

        /// <inheritdoc/>
        public override void _Ready()
        {
            base._Ready();

            // Get references to child nodes
            this.popup = this.GetNode<InteractionPopup>("InteractionPopup");

            // If DoorNode is not set, try to get it from parent
            if (this.DoorNode == null)
            {
                this.DoorNode = this.GetParent<Node>();
            }
        }

        /// <summary>
        /// Execute the door unlock interaction.
        /// Checks if the door is locked and if the player has a key to unlock it.
        /// If successful, unlocks and opens the door.
        /// </summary>
        public override void Run()
        {
            base.Run();

            if (this.DoorNode is Door door && door.IsLocked)
            {
                var inventory = Inventory.Restore();
                if (inventory != null && inventory.GetItemCount(Inventory.ItemType.Key) > 0)
                {
                    inventory.Remove(Inventory.ItemType.Key, 1);
                    door.IsLocked = false;
                    this.IsActive = false;
                    this.popup.IsActive = false;
                }
            }

            if (this.DoorNode is Door doorToOpen)
            {
                doorToOpen.Open();
            }
        }
    }
}
