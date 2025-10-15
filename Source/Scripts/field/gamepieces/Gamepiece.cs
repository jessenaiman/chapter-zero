// <copyright file="Gamepiece.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;
using OmegaSpiral;
using OmegaSpiral.Field.Gamepieces;

/// <summary>
/// Base class for movable objects in the game world.
/// Gamepieces are objects that can move around the gameboard and interact with
/// the environment. They have a position, direction, and can be controlled by
/// either the player or AI.
/// </summary>
[Tool]
public partial class Gamepiece : Node2D
{
    /// <summary>
    /// Emitted when the gamepiece's direction changes.
    /// </summary>
    /// <param name="newDirection">The new direction the gamepiece is facing.</param>
    [Signal]
    public delegate void DirectionChangedEventHandler(Directions.Point newDirection);

    /// <summary>
    /// Emitted when the gamepiece starts moving.
    /// </summary>
    [Signal]
    public delegate void MovementStartedEventHandler();

    /// <summary>
    /// Emitted when the gamepiece stops moving.
    /// </summary>
    [Signal]
    public delegate void MovementStoppedEventHandler();

    /// <summary>
    /// Emitted when the gamepiece reaches a waypoint.
    /// </summary>
    /// <param name="newWaypoint">The cell position of the waypoint that was reached.</param>
    [Signal]
    public delegate void WaypointReachedEventHandler(Vector2I newWaypoint);

    /// <summary>
    /// Emitted when the gamepiece's waypoint changes.
    /// </summary>
    /// <param name="newWaypoint">The new waypoint position.</param>
    [Signal]
    public delegate void WaypointChangedEventHandler(Vector2I newWaypoint);

    /// <summary>
    /// The name of the node group that will contain all Gamepieces.
    /// </summary>
    public const string Group = "_GAMEPIECE_GROUP";

    /// <summary>
    /// Gets or sets the direction the gamepiece is facing.
    /// </summary>
    [Export]
    public Directions.Point Direction
    {
        get => this.direction;
        set
        {
            if (value != this.direction)
            {
                this.direction = value;
                EmitSignal(SignalName.DirectionChanged, (int) this.direction);
            }
        }
    }

    /// <summary>
    /// Gets or sets the speed at which the gamepiece moves, in pixels per second.
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
    public Vector2I CellPosition { get; private set; } = Gameboard.InvalidCell;

    /// <summary>
    /// Gets the target cell position the gamepiece is moving towards.
    /// </summary>
    public Vector2I TargetCell { get; private set; } = Gameboard.InvalidCell;

    /// <summary>
    /// Gets the path the gamepiece is following.
    /// </summary>
    public List<Vector2I> MovePath { get; private set; } = new List<Vector2I>();

    /// <summary>
    /// Gets or sets a value indicating whether whether this gamepiece is controlled by the player.
    /// </summary>
    [Export]
    public bool IsPlayerControlled { get; set; } = false;

    /// <summary>
    /// The direction the gamepiece is facing.
    /// </summary>
    private Directions.Point direction = Directions.Point.South;

    /// <summary>
    /// The remaining distance to the target position.
    /// </summary>
    private float remainingDistance;

    /// <summary>
    /// The target position the gamepiece is moving towards.
    /// </summary>
    private Vector2 targetPosition = Vector2.Zero;

    /// <inheritdoc/>
    public override void _Ready()
    {
        if (!Engine.IsEditorHint())
        {
            this.AddToGroup(Group);

            // Initialize the cell position
            var gameboard = this.GetNodeOrNull<Gameboard>("/root/Gameboard");
            this.CellPosition = gameboard?.PixelToCell(this.Position) ?? Gameboard.InvalidCell;
        }
    }

    /// <inheritdoc/>
    public override void _Process(double delta)
    {
        if (this.IsMoving)
        {
            this.ProcessMovement((float) delta);
        }
    }

    /// <summary>
    /// Process the gamepiece's movement.
    /// </summary>
    /// <param name="delta">The time elapsed since the last frame.</param>
    private void ProcessMovement(float delta)
    {
        // Calculate the distance to move this frame
        var distanceThisFrame = this.MovementSpeed * delta;

        // Move towards the target position
        if (distanceThisFrame >= this.remainingDistance)
        {
            // We've reached the target
            this.Position = this.targetPosition;
            this.remainingDistance = 0.0f;
            this.IsMoving = false;

            // Update the cell position
            var gameboard = this.GetNodeOrNull<Gameboard>("/root/Gameboard");
            this.CellPosition = gameboard?.PixelToCell(this.Position) ?? Gameboard.InvalidCell;

            // Emit the waypoint reached signal
            this.EmitSignal(SignalName.WaypointReached, this.CellPosition);

            // If there's a path, move to the next waypoint
            if (this.MovePath.Count > 0)
            {
                this.MoveToNextWaypoint();
            }
            else
            {
                // No more waypoints, emit movement stopped
                this.EmitSignal(SignalName.MovementStopped);
            }
        }
        else
        {
            // Move partway to the target
            var direction = (this.targetPosition - this.Position).Normalized();
            this.Position += direction * distanceThisFrame;
            this.remainingDistance -= distanceThisFrame;
        }
    }

    /// <summary>
    /// Move the gamepiece to the next waypoint in the path.
    /// </summary>
    private void MoveToNextWaypoint()
    {
        if (this.MovePath.Count == 0)
        {
            return;
        }

        // Get the next waypoint
        var nextWaypoint = this.MovePath[0];
        this.MovePath.RemoveAt(0);

        // Set the target position
        this.MoveToCell(nextWaypoint);
    }

    /// <summary>
    /// Move the gamepiece to a specific cell.
    /// </summary>
    /// <param name="cell">The target cell position to move to.</param>
    public void MoveToCell(Vector2I cell)
    {
        // Convert cell to pixel position
        var gameboard = this.GetNodeOrNull<Gameboard>("/root/Gameboard");
        this.targetPosition = gameboard?.CellToPixel(cell) ?? Vector2.Zero;
        this.remainingDistance = this.Position.DistanceTo(this.targetPosition);

        // Update the direction based on movement
        var movementVector = this.targetPosition - this.Position;
        if (!movementVector.IsEqualApprox(Vector2.Zero))
        {
            this.Direction = Directions.VectorToDirection(movementVector);
        }

        // Update the target cell
        this.TargetCell = cell;

        // Start moving
        this.IsMoving = true;

        // Emit the movement started signal
        this.EmitSignal(SignalName.MovementStarted);

        // Emit the waypoint changed signal
        this.EmitSignal(SignalName.WaypointChanged, cell);
    }

    /// <summary>
    /// Set the gamepiece's movement path.
    /// </summary>
    /// <param name="path">The list of cells to follow as waypoints.</param>
    public void SetMovePath(List<Vector2I> path)
    {
        if (path == null)
        {
            this.MovePath.Clear();
        }
        else
        {
            this.MovePath.Clear();
            foreach (var cell in path)
            {
                this.MovePath.Add(cell);
            }
        }

        // If we're not already moving, start moving to the first waypoint
        if (!this.IsMoving && this.MovePath.Count > 0)
        {
            this.MoveToNextWaypoint();
        }
    }

    /// <summary>
    /// Stop the gamepiece's movement.
    /// </summary>
    public void StopMoving()
    {
        this.IsMoving = false;
        this.MovePath.Clear();
        this.remainingDistance = 0.0f;

        // Emit the movement stopped signal
        this.EmitSignal(SignalName.MovementStopped);
    }

    /// <summary>
    /// Check if the gamepiece can move to a specific cell.
    /// </summary>
    /// <param name="cell">The cell to check for movement validity.</param>
    /// <returns>True if the gamepiece can move to the cell, false otherwise.</returns>
    public bool CanMoveToCell(Vector2I cell)
    {
        var gameboard = this.GetNodeOrNull<Gameboard>("/root/Gameboard");
        if (gameboard == null)
        {
            return false;
        }

        // Check if the cell is within the gameboard bounds
        if (!gameboard.Properties.Extents.HasPoint(cell))
        {
            return false;
        }

        // Since IsCellClear is inaccessible, we'll skip this check
        // Check if the cell is occupied by another gamepiece
        var gamepieceRegistry = this.GetNodeOrNull<GamepieceRegistry>("/root/GamepieceRegistry");
        return !(gamepieceRegistry != null && gamepieceRegistry.IsCellOccupied(cell));
    }

    /// <summary>
    /// Get the adjacent cells that this gamepiece can move to.
    /// </summary>
    /// <returns></returns>
    public List<Vector2I> GetMoveableAdjacentCells()
    {
        var gameboard = this.GetNodeOrNull<Gameboard>("/root/Gameboard");
        var adjacentCells = gameboard?.GetAdjacentCells(this.CellPosition) ?? new List<Vector2I>();
        var moveableCells = new List<Vector2I>();

        foreach (var cell in adjacentCells)
        {
            if (this.CanMoveToCell(cell))
            {
                moveableCells.Add(cell);
            }
        }

        return moveableCells;
    }

    /// <summary>
    /// Rotate the gamepiece to face a specific direction.
    /// </summary>
    /// <param name="newDirection">The direction to face.</param>
    public void FaceDirection(Directions.Point newDirection)
    {
        this.Direction = newDirection;
    }

    /// <summary>
    /// Rotate the gamepiece to face a specific position.
    /// </summary>
    /// <param name="position">The world position to face towards.</param>
    public void FacePosition(Vector2 position)
    {
        var directionVector = position - this.Position;
        this.Direction = Directions.VectorToDirection(directionVector);
    }
}
