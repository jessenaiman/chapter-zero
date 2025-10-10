using Godot;
using System;
using System.Threading.Tasks;

/// <summary>
/// A trigger that activates when the player enters its area.
/// Triggers are invisible areas that automatically activate when the player (or another object)
/// enters them. They can be used to trigger events, start dialogues, change scenes, or any
/// other game mechanic that should happen automatically when the player reaches a certain location.
/// </summary>
[GlobalClass]
public partial class Trigger : Area2D
{
    /// <summary>
    /// Emitted when the trigger is activated by an object entering its area.
    /// </summary>
    [Signal]
    public delegate void TriggeredEventHandler(Node2D triggeringObject);

    /// <summary>
    /// Emitted when an object enters the trigger's area.
    /// </summary>
    [Signal]
    public delegate void EnteredEventHandler(Node2D enteringObject);

    /// <summary>
    /// Emitted when an object exits the trigger's area.
    /// </summary>
    [Signal]
    public delegate void ExitedEventHandler(Node2D exitingObject);

    /// <summary>
    /// Whether this trigger is currently active and can be triggered.
    /// </summary>
    [Export]
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Whether this trigger should be destroyed after being triggered once.
    /// </summary>
    [Export]
    public bool IsOneShot { get; set; } = false;

    /// <summary>
    /// Whether this trigger requires a specific group to trigger it.
    /// If empty, any object can trigger it.
    /// </summary>
    [Export]
    public string RequiredGroup { get; set; } = "";

    /// <summary>
    /// Delay in seconds before the trigger activates after an object enters.
    /// </summary>
    [Export]
    public float ActivationDelay { get; set; } = 0.0f;

    /// <summary>
    /// Whether the trigger should only activate once per object.
    /// </summary>
    [Export]
    public bool ActivateOncePerObject { get; set; } = false;

    /// <summary>
    /// List of objects that have already triggered this trigger (when ActivateOncePerObject is true).
    /// </summary>
    private Godot.Collections.Array<Node2D> triggeredObjects = new Godot.Collections.Array<Node2D>();

    public override void _Ready()
    {
        base._Ready();

        // Connect to area events
        AreaEntered += OnAreaEntered;
        AreaExited += OnAreaExited;

        // Connect to body events (for physics-based triggers)
        BodyEntered += OnBodyEntered;
        BodyExited += OnBodyExited;
    }

    /// <summary>
    /// Activate the trigger's logic.
    /// This method is called when the trigger is activated by an object entering its area.
    /// Override this method to implement custom trigger behavior.
    /// </summary>
    public virtual async void Activate(Node2D triggeringObject)
    {
        if (!IsActive)
        {
            return;
        }

        // Check if this object has already triggered this trigger (when ActivateOncePerObject is true)
        if (ActivateOncePerObject && triggeredObjects.Contains(triggeringObject))
        {
            return;
        }

        // Add the object to the triggered objects list
        if (ActivateOncePerObject)
        {
            triggeredObjects.Add(triggeringObject);
        }

        // Emit the triggered signal
        EmitSignal(SignalName.Triggered, triggeringObject);

        // Handle activation delay
        if (ActivationDelay > 0.0f)
        {
            await Task.Delay(TimeSpan.FromSeconds(ActivationDelay));
        }

        // Handle one-shot triggers
        if (IsOneShot)
        {
            IsActive = false;
            // Optionally remove the trigger from the scene
            // QueueFree();
        }
    }

    /// <summary>
    /// Check if an object can trigger this trigger based on group requirements.
    /// </summary>
    public bool CanBeTriggeredBy(Node2D obj)
    {
        if (obj == null)
        {
            return false;
        }

        // If no specific group is required, any object can trigger it
        if (string.IsNullOrEmpty(RequiredGroup))
        {
            return true;
        }

        // Check if the object is in the required group
        return obj.IsInGroup(RequiredGroup);
    }

    /// <summary>
    /// Callback when another area enters this trigger's area.
    /// </summary>
    private void OnAreaEntered(Area2D area)
    {
        // Check if this trigger is active
        if (!IsActive)
        {
            return;
        }

        // Check if the area can trigger this trigger
        if (!CanBeTriggeredBy(area))
        {
            return;
        }

        // Emit the entered signal
        EmitSignal(SignalName.Entered, area);

        // Activate the trigger
        Activate(area);
    }

    /// <summary>
    /// Callback when another area exits this trigger's area.
    /// </summary>
    private void OnAreaExited(Area2D area)
    {
        // Check if this trigger is active
        if (!IsActive)
        {
            return;
        }

        // Check if the area can trigger this trigger
        if (!CanBeTriggeredBy(area))
        {
            return;
        }

        // Emit the exited signal
        EmitSignal(SignalName.Exited, area);
    }

    /// <summary>
    /// Callback when a physics body enters this trigger's area.
    /// </summary>
    private void OnBodyEntered(PhysicsBody2D body)
    {
        // Check if this trigger is active
        if (!IsActive)
        {
            return;
        }

        // Check if the body can trigger this trigger
        if (!CanBeTriggeredBy(body))
        {
            return;
        }

        // Emit the entered signal
        EmitSignal(SignalName.Entered, body);

        // Activate the trigger
        Activate(body);
    }

    /// <summary>
    /// Callback when a physics body exits this trigger's area.
    /// </summary>
    private void OnBodyExited(PhysicsBody2D body)
    {
        // Check if this trigger is active
        if (!IsActive)
        {
            return;
        }

        // Check if the body can trigger this trigger
        if (!CanBeTriggeredBy(body))
        {
            return;
        }

        // Emit the exited signal
        EmitSignal(SignalName.Exited, body);
    }
}
