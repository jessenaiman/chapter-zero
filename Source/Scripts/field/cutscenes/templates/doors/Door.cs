// <copyright file="Door.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;
using Timer = Godot.Timer; // Resolve ambiguity between Godot.Timer and System.Threading.Timer

/// <summary>
/// A door that can be locked or unlocked, blocking or allowing passage through its cell.
/// When a door is locked, it places a dummy gamepiece on the cell to block movement.
/// When unlocked, the door allows passage and can open when triggered.
/// </summary>
[GlobalClass]
public partial class Door : AreaTransition
{
    /// <summary>
    /// Gets or sets a value indicating whether whether the door is currently locked and blocking movement.
    /// </summary>
    [Export]
    public bool IsLocked
    {
        get => this.isLocked;
        set
        {
            if (value != this.isLocked)
            {
                this.isLocked = value;

                // Wait for the node to be ready if not already inside the tree
                if (!this.IsInsideTree())
                {
                    this.CallDeferred(nameof(this.SetDoorLockState), value);
                    return;
                }

                this.SetDoorLockState(value);
            }
        }
    }

    // NewMusic is already defined in the parent AreaTransition class, so we don't need to redefine it
    // The Door class will use the NewMusic property from its parent AreaTransition class

    /// <summary>
    /// Keep a reference to the object used to block movement through a locked door.
    /// Note that this gamepiece has no animation, movement, etc. It exists to occupy a board cell.
    /// </summary>
    private Gamepiece dummyGp;

    /// <summary>
    /// Animation player for door animations.
    /// </summary>
    private AnimationPlayer anim;

    /// <summary>
    /// Sprite for the closed door.
    /// </summary>
    private Sprite2D closedDoor;

    /// <summary>
    /// Whether the door is currently locked.
    /// </summary>
    private bool isLocked;

    /// <inheritdoc/>
    public override void _Ready()
    {
        base._Ready();

        // Get references to child nodes
        this.anim = this.GetNode<AnimationPlayer>("AnimationPlayer");
        this.closedDoor = this.GetNode<Sprite2D>("Area2D/ClosedDoor");
    }

    /// <summary>
    /// Set the locked state of the door.
    /// </summary>
    /// <param name="locked">Whether the door should be locked.</param>
    private void SetDoorLockState(bool locked)
    {
        if (locked)
        {
            // If locked and no dummy gamepiece exists, create one
            if (this.dummyGp == null)
            {
                // In Godot C#, we can't directly preload scenes like in GDScript
                // Instead, we'll need to create a basic gamepiece or load from resource
                this.dummyGp = new Gamepiece();
                this.dummyGp.Name = "CellBlocker";
                this.closedDoor.AddChild(this.dummyGp);
            }
        }
        else
        {
            this.Open();

            // Remove the dummy gamepiece if it exists
            if (this.dummyGp != null && this.dummyGp.IsInsideTree())
            {
                this.dummyGp.QueueFree();
                this.dummyGp = null;
            }
        }
    }

    /// <summary>
    /// Open the door if it's closed.
    /// </summary>
    public async void Open()
    {
        // Do not open the door if it is already open.
        if (!this.closedDoor.Visible)
        {
            return;
        }
        else if (this.IsLocked)
        {
            if (this.anim != null)
            {
                this.anim.Play("locked");
                await this.ToSignal(this.anim, AnimationPlayer.SignalName.AnimationFinished);
            }
        }
        else
        {
            if (this.anim != null)
            {
                this.anim.Play("open");
                await this.ToSignal(this.anim, AnimationPlayer.SignalName.AnimationFinished);
            }
        }
    }

    /// <summary>
    /// Activate the door's logic.
    /// Opens the door if it's closed before proceeding with the area transition.
    /// </summary>
    /// <param name="triggeringObject">The object that triggered this door.</param>
    public override async void Activate(Node2D triggeringObject)
    {
        // Only open the door if it is closed.
        if (this.closedDoor.Visible && this.anim != null)
        {
            this.anim.Play("open");
            await this.ToSignal(this.anim, AnimationPlayer.SignalName.AnimationFinished);
        }

        // Call the parent implementation to handle the area transition
        base.Activate(triggeringObject);
    }

    /// <summary>
    /// Callback when the blackout occurs during area transition.
    /// Plays new music if specified.
    /// </summary>
    /// <returns><placeholder>A <see cref="Task"/> representing the asynchronous operation.</placeholder></returns>
    protected override async Task OnBlackout()
    {
        // Play new music if specified
        if (this.NewMusic != null)
        {
            Music.Instance?.Play(this.NewMusic);
        }

        // Call the parent implementation
        await base.OnBlackout().ConfigureAwait(false);
    }
}
