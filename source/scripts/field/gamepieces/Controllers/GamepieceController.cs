
// <copyright file="GamepieceController.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using Godot;
using OmegaSpiral.Source.Scripts.Field.gamepieces;
using OmegaSpiral.Source.Scripts.Field.gameboard;

namespace OmegaSpiral.Source.Scripts.Field.gamepieces.Controllers;
/// <summary>
/// Base class for controllers that manage Gamepiece movement and interaction.
/// GamepieceControllers handle the logic for moving Gamepieces around the gameboard
/// and responding to player input or AI decisions. They manage movement paths,
/// collision detection, and interaction with other game objects.
/// </summary>
[GlobalClass]
[Tool]
public partial class GamepieceController : Node
{
    /// <summary>
    /// The Gamepiece that this controller manages.
    /// </summary>
    private Gamepiece? gamepiece;

    /// <summary>
    /// Whether this controller is currently active.
    /// </summary>
    private bool isActive = true;

    /// <summary>
    /// The path the gamepiece is following.
    /// </summary>
    private List<Vector2I> movePath = new List<Vector2I>();

    /// <summary>
    /// Emitted when the controller starts moving the gamepiece.
    /// </summary>
    [Signal]
    public delegate void MovementStartedEventHandler();

    /// <summary>
    /// Emitted when the controller stops moving the gamepiece.
    /// </summary>
    [Signal]
    public delegate void MovementStoppedEventHandler();

    /// <summary>
    /// Emitted when the gamepiece reaches a waypoint.
    /// </summary>
    /// <param name="newWaypoint">The waypoint cell position that was reached.</param>
    [Signal]
    public delegate void WaypointReachedEventHandler(Vector2I newWaypoint);

    /// <summary>
    /// Emitted when the gamepiece's waypoint changes.
    /// </summary>
    /// <param name="newWaypoint">The new waypoint cell position.</param>
    [Signal]
    public delegate void WaypointChangedEventHandler(Vector2I newWaypoint);

    /// <summary>
    /// Gets or sets the Gamepiece that this controller manages.
    /// </summary>
    public Gamepiece Gamepiece
    {
        get
        {
            if (this.gamepiece == null && !Engine.IsEditorHint())
            {
                this.gamepiece = this.GetParent<Gamepiece>();
            }

            return this.gamepiece!;
        }

        set
        {
            if (this.gamepiece != value)
            {
                // Disconnect from the old gamepiece
                if (this.gamepiece != null)
                {
                    this.gamepiece.WaypointChanged -= this.OnGamepieceWaypointChanged;
                    this.gamepiece.MovementStarted -= this.OnGamepieceMovementStarted;
                    this.gamepiece.MovementStopped -= this.OnGamepieceMovementStopped;
                }

                this.gamepiece = value;

                // Connect to the new gamepiece
                if (this.gamepiece != null)
                {
                    this.gamepiece.WaypointChanged += this.OnGamepieceWaypointChanged;
                    this.gamepiece.MovementStarted += this.OnGamepieceMovementStarted;
                    this.gamepiece.MovementStopped += this.OnGamepieceMovementStopped;
                }
            }
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether whether this controller is currently active and controlling the gamepiece.
    /// </summary>
    [Export]
    public bool IsActive
    {
        get => this.isActive;
        set
        {
            if (value != this.isActive)
            {
                this.isActive = value;
                this.OnActiveStateChanged();
            }
        }
    }

    /// <summary>
    /// Gets or sets the movement speed of the gamepiece, in pixels per second.
    /// </summary>
    [Export]
    public float MovementSpeed { get; set; } = 100.0f;

    /// <summary>
    /// Gets a value indicating whether whether the gamepiece is currently moving.
    /// </summary>
    public bool IsMoving { get; private set; }

    /// <summary>
    /// Gets the current cell position of the gamepiece.
    /// </summary>
    public Vector2I CellPosition { get; private set; } = global::OmegaSpiral.Source.Scripts.Field.gameboard.Gameboard.InvalidCell;

    /// <summary>
    /// Gets the target cell position the gamepiece is moving towards.
    /// </summary>
    public Vector2I TargetCell { get; private set; } = global::OmegaSpiral.Source.Scripts.Field.gameboard.Gameboard.InvalidCell;

    /// <summary>
    /// Gets or sets a value indicating whether whether this controller is controlled by the player.
    /// </summary>
    [Export]
    public bool IsPlayerControlled { get; set; } = false;

    /// <summary>
    /// Gets the path the gamepiece is following.
    /// </summary>
    public List<Vector2I> MovePath => this.movePath;

    /// <inheritdoc/>
    public override void _Ready()
    {
        if (!Engine.IsEditorHint())
        {
            // Get the gamepiece from the parent node
            this.Gamepiece = this.GetParent<Gamepiece>();

            if (this.Gamepiece != null)
            {
                // Initialize the cell position
                this.CellPosition = this.Gamepiece.CellPosition;

                // Connect to gamepiece signals
                this.Gamepiece.WaypointChanged += this.OnGamepieceWaypointChanged;
                this.Gamepiece.MovementStarted += this.OnGamepieceMovementStarted;
                this.Gamepiece.MovementStopped += this.OnGamepieceMovementStopped;
            }
        }
    }

    /// <summary>
    /// Move the gamepiece along a path.
    /// </summary>
    /// <param name="path">The list of cell positions representing the path to follow.</param>
    public virtual void MoveAlongPath(List<Vector2I> path)
    {
        if (path == null || path.Count == 0)
        {
            return;
        }

        this.SetMovePath(new List<Vector2I>(path));
        this.IsMoving = true;

        // Move the gamepiece to the first waypoint
        var firstWaypoint = this.MovePath[0];
        this.RemoveFirstWaypoint();
        this.Gamepiece.MoveToCell(firstWaypoint);

        this.EmitSignal(SignalName.MovementStarted);
    }

    /// <summary>
    /// Stop the gamepiece's movement.
    /// </summary>
    public virtual void StopMoving()
    {
        this.IsMoving = false;
        this.ClearMovePath();

        if (this.Gamepiece != null)
        {
            this.Gamepiece.StopMoving();
        }

        this.EmitSignal(SignalName.MovementStopped);
    }

    /// <inheritdoc/>
    public override void _Process(double delta)
    {
        if (!Engine.IsEditorHint() && this.IsActive)
        {
            this.ProcessController((float) delta);
        }
    }

    /// <summary>
    /// Move the gamepiece to a specific cell.
    /// </summary>
    /// <param name="cell">The target cell position to move to.</param>
    public virtual void MoveToCell(Vector2I cell)
    {
        if (this.Gamepiece != null)
        {
            this.Gamepiece.MoveToCell(cell);
        }
    }

    /// <summary>
    /// Move the gamepiece towards a pressed key direction.
    /// </summary>
    /// <param name="direction">The direction vector of the key press.</param>
    public virtual void MoveToPressedKey(Vector2 direction)
    {
        if (this.Gamepiece == null || !this.IsActive)
        {
            return;
        }

        var sourceCell = this.Gamepiece.CellPosition;
        var targetCell = sourceCell + new Vector2I((int) direction.X, (int) direction.Y);

        // Try to get a path to the destination (will fail if cell is occupied)
        var gameboard = this.GetNode<global::OmegaSpiral.Source.Scripts.Field.gameboard.Gameboard>("/root/Gameboard");
        var newPath = gameboard?.PathFinder?.GetPathToCell(sourceCell, targetCell) ?? new List<Vector2I>();

        // Path is invalid. Bump animation?
        if (newPath.Count < 1)
        {
            this.Gamepiece.Direction = Directions.AngleToDirection(direction.Angle());
        }
        else
        {
            this.SetMovePath(new List<Vector2I>(newPath));
        }
    }

    /// <summary>
    /// Process the controller's logic.
    /// Override this method to implement custom controller behavior.
    /// </summary>
    /// <param name="delta">The time elapsed since the last frame in seconds.</param>
    protected virtual void ProcessController(float delta)
    {
        // Default implementation does nothing
        // Override in subclasses to add custom behavior
    }

    /// <summary>
    /// Callback when the controller's active state changes.
    /// Override this method to implement custom behavior when the controller is activated or deactivated.
    /// </summary>
    protected virtual void OnActiveStateChanged()
    {
        // Default implementation does nothing
        // Override in subclasses to add custom behavior
    }

    /// <summary>
    /// Callback when the move path changes.
    /// Override this method to implement custom behavior when the move path is updated.
    /// </summary>
    protected virtual void OnMovePathChanged()
    {
        // Default implementation does nothing
        // Override in subclasses to add custom behavior
    }

    /// <summary>
    /// Callback when the gamepiece reaches a waypoint.
    /// Override this method to implement custom behavior when the gamepiece reaches a waypoint.
    /// </summary>
    /// <param name="newWaypoint">The waypoint cell position that was reached.</param>
    protected virtual void OnGamepieceWaypointReached(Vector2I newWaypoint)
    {
        // Update the cell position
        this.CellPosition = newWaypoint;

        // If there are more waypoints in the path, move to the next one
        if (this.MovePath.Count > 0)
        {
            var nextWaypoint = this.MovePath[0];
            this.RemoveFirstWaypoint();
            this.Gamepiece.MoveToCell(nextWaypoint);
        }
        else
        {
            // No more waypoints, stop moving
            this.IsMoving = false;
        }

        this.EmitSignal(SignalName.WaypointReached, newWaypoint);
    }

    /// <summary>
    /// Callback when the gamepiece's waypoint changes.
    /// </summary>
    /// <param name="newWaypoint">The new waypoint cell position.</param>
    private void OnGamepieceWaypointChanged(Vector2I newWaypoint)
    {
        this.CellPosition = newWaypoint;
        this.EmitSignal(SignalName.WaypointChanged, newWaypoint);
    }

    /// <summary>
    /// Callback when the gamepiece starts moving.
    /// </summary>
    private void OnGamepieceMovementStarted()
    {
        this.IsMoving = true;
        this.EmitSignal(SignalName.MovementStarted);
    }

    /// <summary>
    /// Callback when the gamepiece stops moving.
    /// </summary>
    private void OnGamepieceMovementStopped()
    {
        this.IsMoving = false;
        this.EmitSignal(SignalName.MovementStopped);
    }

    /// <summary>
    /// Callback when the gamepiece is arriving at a waypoint.
    /// Override this method to implement custom behavior when the gamepiece is arriving.
    /// </summary>
    /// <param name="excessDistance">The excess distance beyond the waypoint.</param>
    protected virtual void OnGamepieceArriving(float excessDistance)
    {
        // Default implementation does nothing
        // Override in subclasses to add custom behavior
    }

    /// <summary>
    /// Callback when the gamepiece has arrived at a waypoint.
    /// Override this method to implement custom behavior when the gamepiece arrives.
    /// </summary>
    protected virtual void OnGamepieceArrived()
    {
        // Default implementation does nothing
        // Override in subclasses to add custom behavior
    }

    /// <summary>
    /// Sets the move path for the gamepiece.
    /// </summary>
    /// <param name="path">The list of cell positions representing the path to follow.</param>
    private void SetMovePath(List<Vector2I> path)
    {
        this.movePath = path ?? new List<Vector2I>();
        this.OnMovePathChanged();
    }

    /// <summary>
    /// Removes the first waypoint from the move path.
    /// </summary>
    private void RemoveFirstWaypoint()
    {
        if (this.movePath.Count > 0)
        {
            this.movePath.RemoveAt(0);
        }
    }

    /// <summary>
    /// Clears the move path.
    /// </summary>
    private void ClearMovePath()
    {
        this.movePath.Clear();
    }
}
