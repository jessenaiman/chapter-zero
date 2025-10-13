// <copyright file="Interaction.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System;
using System.Threading.Tasks;
using Godot;

/// <summary>
/// Base class for interactive objects in the game world.
/// Interactions are objects that respond to player input, typically when the player
/// presses a key or button while near the object. They can trigger dialogues, events,
/// or other game mechanics.
/// </summary>
[GlobalClass]
public partial class Interaction : Area2D
{
    /// <summary>
    /// Emitted when the interaction is triggered by the player.
    /// </summary>
    [Signal]
    public delegate void TriggeredEventHandler();

    /// <summary>
    /// Gets or sets the interaction's name. Used in UI and dialogue.
    /// </summary>
    [Export]
    public string InteractionName { get; set; } = "Interaction";

    /// <summary>
    /// Gets or sets a description of what happens when this interaction is triggered.
    /// </summary>
    [Export]
    public string Description { get; set; } = "Interact with this object.";

    /// <summary>
    /// Gets or sets a value indicating whether whether this interaction is currently active and can be triggered.
    /// </summary>
    [Export]
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether whether this interaction should be destroyed after being triggered once.
    /// </summary>
    [Export]
    public bool IsOneShot { get; set; } = false;

    /// <summary>
    /// Gets or sets the key or button that triggers this interaction.
    /// </summary>
    [Export]
    public string TriggerAction { get; set; } = "interact";

    /// <summary>
    /// Gets or sets the distance within which the player can trigger this interaction.
    /// </summary>
    [Export]
    public float TriggerDistance { get; set; } = 32.0f;

    /// <summary>
    /// Gets or sets a value indicating whether whether this interaction requires the player to face the object to trigger it.
    /// </summary>
    [Export]
    public bool RequiresFacing { get; set; } = true;

    /// <summary>
    /// Gets a value indicating whether whether this interaction is currently being hovered over by the player.
    /// </summary>
    public bool IsHovered { get; private set; }

    /// <inheritdoc/>
    public override void _Ready()
    {
        base._Ready();

        // Connect to input events
        this.InputEvent += OnInputEvent;

        // Connect to area events for hover detection
        this.AreaEntered += this.OnAreaEntered;
        this.AreaExited += this.OnAreaExited;
    }

    /// <inheritdoc/>
    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event == null)
        {
            throw new ArgumentNullException(nameof(@event));
        }

        base._UnhandledInput(@event);

        // Check if this interaction is active and can be triggered
        if (!this.IsActive || !this.IsHovered)
        {
            return;
        }

        // Check if the trigger action was pressed
        if (@event.IsActionPressed(this.TriggerAction))
        {
            this.Run();
        }
    }

    /// <summary>
    /// Run the interaction's logic.
    /// This method is called when the interaction is triggered by the player.
    /// Override this method to implement custom interaction behavior.
    /// </summary>
    public virtual async void Run()
    {
        if (!this.IsActive)
        {
            return;
        }

        // Emit the triggered signal
        this.EmitSignal(SignalName.Triggered);

        // Handle one-shot interactions
        if (this.IsOneShot)
        {
            this.IsActive = false;
        }

        // Wait a frame to allow signal handlers to run
        await this.ToSignal(this.GetTree(), SceneTree.SignalName.ProcessFrame);
    }

    /// <summary>
    /// Check if the player is within trigger distance of this interaction.
    /// </summary>
    /// <returns></returns>
    public bool IsPlayerWithinTriggerDistance(Node2D player)
    {
        if (player == null)
        {
            return false;
        }

        var distance = this.Position.DistanceTo(player.Position);
        return distance <= this.TriggerDistance;
    }

    /// <summary>
    /// Check if the player is facing this interaction.
    /// </summary>
    /// <returns></returns>
    public bool IsPlayerFacing(Node2D player)
    {
        if (player == null)
        {
            return false;
        }

        // Calculate the direction from player to this interaction
        var directionToInteraction = (this.Position - player.Position).Normalized();

        // Get the player's facing direction (this would depend on your player implementation)
        // For now, we'll assume the player has a FacingDirection property
        var playerFacingDirection = Vector2.Zero;

        // Check if the player is facing roughly toward this interaction
        var dotProduct = directionToInteraction.Dot(playerFacingDirection);
        return dotProduct > 0.5f; // Roughly within 60 degrees
    }

    /// <summary>
    /// Callback when input events occur within this interaction's area.
    /// </summary>
    private void OnInputEvent(Viewport viewport, InputEvent @event, int shapeIdx)
    {
        // Handle input events
        if (@event.IsActionPressed(this.TriggerAction))
        {
            this.Run();
        }
    }

    /// <summary>
    /// Callback when another area enters this interaction's area.
    /// </summary>
    private void OnAreaEntered(Area2D area)
    {
        // Check if the entering area is the player
        if (area.Name == "PlayerInteractionArea")
        {
            this.IsHovered = true;
            this.OnPlayerEntered();
        }
    }

    /// <summary>
    /// Callback when another area exits this interaction's area.
    /// </summary>
    private void OnAreaExited(Area2D area)
    {
        // Check if the exiting area is the player
        if (area.Name == "PlayerInteractionArea")
        {
            this.IsHovered = false;
            this.OnPlayerExited();
        }
    }

    /// <summary>
    /// Callback when the player enters this interaction's area.
    /// Override this method to implement custom behavior when the player approaches.
    /// </summary>
    protected virtual void OnPlayerEntered()
    {
        // Default implementation does nothing
        // Override in subclasses to add custom behavior
    }

    /// <summary>
    /// Callback when the player exits this interaction's area.
    /// Override this method to implement custom behavior when the player leaves.
    /// </summary>
    protected virtual void OnPlayerExited()
    {
        // Default implementation does nothing
        // Override in subclasses to add custom behavior
    }
}
