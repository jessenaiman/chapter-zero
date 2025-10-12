using Godot;
using System;

/// <summary>
/// A trigger that plays an animation when an area enters its trigger zone.
/// This is typically used for door objects that open when the player approaches.
/// </summary>
[GlobalClass]
public partial class DoorTrigger : Trigger
{
    /// <summary>
    /// The AnimationPlayer node that handles the door opening/closing animations.
    /// </summary>
    [Export]
    public AnimationPlayer Anim { get; set; }

    public override void _Ready()
    {
        base._Ready();

        // Validate that the AnimationPlayer reference is set
        if (Anim == null)
        {
            GD.PrintErr($"{Name} error: AnimationPlayer reference is not set!");
        }
    }

    /// <summary>
    /// Activate the door trigger's logic.
    /// Plays the "open" animation when triggered.
    /// </summary>
    /// <param name="triggeringObject">The object that triggered this door</param>
    public async void Activate(Node2D triggeringObject)
    {

        // Play the open animation if the animation player is set
        if (Anim != null)
        {
            Anim.Play("open");
        }
    }
}
