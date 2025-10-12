// <copyright file="DoorTrigger.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System;
using Godot;

/// <summary>
/// A trigger that plays an animation when an area enters its trigger zone.
/// This is typically used for door objects that open when the player approaches.
/// </summary>
[GlobalClass]
public partial class DoorTrigger : Trigger
{
    /// <summary>
    /// Gets or sets the AnimationPlayer node that handles the door opening/closing animations.
    /// </summary>
    [Export]
    public AnimationPlayer Anim { get; set; }

    /// <inheritdoc/>
    public override void _Ready()
    {
        base._Ready();

        // Validate that the AnimationPlayer reference is set
        if (this.Anim == null)
        {
            GD.PrintErr($"{this.Name} error: AnimationPlayer reference is not set!");
        }
    }

    /// <summary>
    /// Activate the door trigger's logic.
    /// Plays the "open" animation when triggered.
    /// </summary>
    /// <param name="triggeringObject">The object that triggered this door.</param>
    public async void Activate(Node2D triggeringObject)
    {
        // Play the open animation if the animation player is set
        if (this.Anim != null)
        {
            this.Anim.Play("open");
        }
    }
}
