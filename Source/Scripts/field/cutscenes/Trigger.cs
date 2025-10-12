using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// A <see cref="Cutscene"/> that triggers on collision with a <see cref="Gamepiece"/>'s collision shapes.
///
/// A Gamepiece with collision shapes on a layer monitored by the Trigger may activate the Trigger.
/// Triggers typically wait for <see cref="Gamepiece.Arriving"/> before being run, but that behaviour may
/// be overridden in derived Triggers by modifying <see cref="_OnAreaEntered(Area2D)"/>.
/// </summary>
[Tool]
public partial class Trigger : Cutscene
{
    /// <summary>
    /// Emitted when a <see cref="Gamepiece"/> begins moving to the cell occupied by the Trigger.
    /// </summary>
    [Signal]
    public delegate void GamepieceEnteredEventHandler(Gamepiece gamepiece);

    /// <summary>
    /// Emitted when a <see cref="Gamepiece"/> begins moving away from the cell occupied by the Trigger.
    /// </summary>
    [Signal]
    public delegate void GamepieceExitedEventHandler(Gamepiece gamepiece);

    /// <summary>
    /// Emitted when a <see cref="Gamepiece"/> is finishing moving to the cell occupied by the Trigger.
    /// </summary>
    [Signal]
    public delegate void TriggeredEventHandler(Gamepiece gamepiece);

    private bool _isActive = true;
    /// <summary>
    /// An active Trigger may be run, whereas one that is inactive may only be run
    /// directly through code via the <see cref="Cutscene.Run"/> method.
    /// </summary>
    [Export]
    public bool IsActive
    {
        get => _isActive;
        set
        {
            _isActive = value;

            if (!Engine.IsEditorHint())
            {
                if (!IsInsideTree())
                {
                    // We'll set the value and wait for the node to be ready
                    _isActive = value;
                    return;
                }

                // We use "Visible Collision Shapes" to debug positions on the gameboard, so we'll want
                // to change the state of child collision shapes. These could be either CollisionShape2Ds
                // or CollisionPolygon2Ds.
                // Note that we only want to disable the collision shapes of objects that are actually
                // connected to this Interaction.
                var connections = GetIncomingConnections();
                foreach (var connection in connections)
                {
                    var callable = (Callable)connection["callable"];
                    if (callable.Method == "_on_area_entered")
                    {
                        var connectedArea = ((Signal)connection["signal"]).GetOwner() as Area2D;
                        if (connectedArea != null)
                        {
                            foreach (var node in connectedArea.FindChildren("*", "CollisionShape2D"))
                            {
                                (node as CollisionShape2D).SetDisabled(!_isActive);
                            }
                            foreach (var node in connectedArea.FindChildren("*", "CollisionPolygon2D"))
                            {
                                (node as CollisionPolygon2D).SetDisabled(!_isActive);
                            }
                        }
                    }
                }
            }
        }
    }

    public override void _Ready()
    {
        if (!Engine.IsEditorHint())
        {
            // FieldEvents.input_paused.connect(_on_input_paused) - we'll need to implement this when FieldEvents is available
        }
    }

    // Ensure that something is connected to _on_area_entered, which the Trigger requires.
    // If nothing is connected, issue a configuration warning.
    public string[] GetConfigurationWarnings()
    {
        var warnings = new List<string>();
        var hasAreaEnteredBindings = false;

        var connections = GetIncomingConnections();
        foreach (var connection in connections)
        {
            var callable = (Callable)connection["callable"];
            if (callable.Method == "_on_area_entered")
            {
                hasAreaEnteredBindings = true;
            }
        }

        if (!hasAreaEnteredBindings)
        {
            warnings.Add("This object does not have a CollisionObject2D's signals connected to " +
                "this Trigger's _on_area_entered method. The Trigger will never be triggered!");
        }
        return warnings.ToArray();
    }

    // Pause any collision objects that would normally send signals regarding interactions.
    // This will automatically accept or ignore currently overlapping areas.
    private void OnInputPaused(bool isPaused)
    {
        var connections = GetIncomingConnections();
        foreach (var connection in connections)
        {
            // Note that we only want to check _on_area_entered, since _on_area_exited will clean up any
            // lingering references once the Area2Ds are 'shut off' (i.e. not monitoring/monitorable).
            var callable = (Callable)connection["callable"];
            if (callable.Method == "_on_area_entered")
            {
                var connectedArea = ((Signal)connection["signal"]).GetOwner() as Area2D;
                if (connectedArea != null)
                {
                    connectedArea.SetMonitoring(!isPaused);
                    connectedArea.SetMonitorable(!isPaused);
                }
            }
        }
    }

    // Register the colliding gamepiece and wait for it to finish moving before running the trigger.
    protected void _OnAreaEntered(Area2D area)
    {
        var gamepiece = area.Owner as Gamepiece;

        // Check to make sure that the gamepiece is moving before connecting to its 'arriving'
        // signal. This catches edge cases where the Trigger is unpaused while a colliding object
        // is standing on top of it (which would mean that _on_gamepiece_arrived would trigger once
        // the gamepiece moves OFF of it. Which is bad.).
        if (gamepiece != null && gamepiece.IsMoving())
        {
            EmitSignal(SignalName.GamepieceEntered, gamepiece);
            gamepiece.Arrived += (Gamepiece gp) => _OnGamepieceArrived(gp);

            // Triggers need to block input early. Otherwise, if waiting until the gamepiece has arrived,
            // there's a chance that the player's controller may have received the gamepiece.arrived
            // signal first and continue moving the gamepiece.
            // await GetTree().ProcessFrame;
            _is_cutscene_in_progress = true;
        }
    }

    protected void _OnAreaExited(Area2D area)
    {
        var gamepiece = area.Owner as Gamepiece;
        if (gamepiece != null)
        {
            EmitSignal(SignalName.GamepieceExited, gamepiece);
        }
    }

    protected void _OnGamepieceArrived(Gamepiece gamepiece)
    {
        EmitSignal(SignalName.Triggered, gamepiece);
        Run();
    }
}
