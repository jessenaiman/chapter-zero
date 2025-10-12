using Godot;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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
    [Signal]
    public delegate void DirectionChangedEventHandler(Directions.Points newDirection);

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
    [Signal]
    public delegate void WaypointReachedEventHandler(Vector2I newWaypoint);

    /// <summary>
    /// Emitted when the gamepiece's waypoint changes.
    /// </summary>
    [Signal]
    public delegate void WaypointChangedEventHandler(Vector2I newWaypoint);

    /// <summary>
    /// The name of the node group that will contain all Gamepieces.
    /// </summary>
    public const string Group = "_GAMEPIECE_GROUP";

    /// <summary>
    /// The direction the gamepiece is facing.
    /// </summary>
    [Export]
    public Directions.Points Direction
    {
        get => direction;
        set
        {
            if (value != direction)
            {
                direction = value;
                EmitSignal(SignalName.DirectionChanged, direction);
            }
        }
    }

    /// <summary>
    /// The speed at which the gamepiece moves, in pixels per second.
    /// </summary>
    [Export]
    public float MovementSpeed { get; set; } = 100.0f;

    /// <summary>
    /// Whether the gamepiece is currently moving.
    /// </summary>
    public bool IsMoving { get; private set; } = false;

    /// <summary>
    /// The current cell position of the gamepiece.
    /// </summary>
    public Vector2I CellPosition { get; private set; } = Gameboard.InvalidCell;

    /// <summary>
    /// The target cell position the gamepiece is moving towards.
    /// </summary>
    public Vector2I TargetCell { get; private set; } = Gameboard.InvalidCell;

    /// <summary>
    /// The path the gamepiece is following.
    /// </summary>
    public List<Vector2I> MovePath { get; private set; } = new List<Vector2I>();

    /// <summary>
    /// Whether this gamepiece is controlled by the player.
    /// </summary>
    [Export]
    public bool IsPlayerControlled { get; set; } = false;

    /// <summary>
    /// The direction the gamepiece is facing.
    /// </summary>
    private Directions.Points direction = Directions.Points.Down;

    /// <summary>
    /// The remaining distance to the target position.
    /// </summary>
    private float remainingDistance = 0.0f;

    /// <summary>
    /// The target position the gamepiece is moving towards.
    /// </summary>
    private Vector2 targetPosition = Vector2.Zero;

    public override void _Ready()
    {
        if (!Engine.IsEditorHint())
        {
            AddToGroup(Group);

            // Initialize the cell position
            CellPosition = Gameboard.PixelToCell(Position);
        }
    }

    public override void _Process(double delta)
    {
        if (IsMoving)
        {
            ProcessMovement((float)delta);
        }
    }

    /// <summary>
    /// Process the gamepiece's movement.
    /// </summary>
    private void ProcessMovement(float delta)
    {
        // Calculate the distance to move this frame
        var distanceThisFrame = MovementSpeed * delta;

        // Move towards the target position
        if (distanceThisFrame >= remainingDistance)
        {
            // We've reached the target
            Position = targetPosition;
            remainingDistance = 0.0f;
            IsMoving = false;

            // Update the cell position
            CellPosition = Gameboard.PixelToCell(Position);

            // Emit the waypoint reached signal
            EmitSignal(SignalName.WaypointReached, CellPosition);

            // If there's a path, move to the next waypoint
            if (MovePath.Count > 0)
            {
                MoveToNextWaypoint();
            }
            else
            {
                // No more waypoints, emit movement stopped
                EmitSignal(SignalName.MovementStopped);
            }
        }
        else
        {
            // Move partway to the target
            var direction = (targetPosition - Position).Normalized();
            Position += direction * distanceThisFrame;
            remainingDistance -= distanceThisFrame;
        }
    }

    /// <summary>
    /// Move the gamepiece to the next waypoint in the path.
    /// </summary>
    private void MoveToNextWaypoint()
    {
        if (MovePath.Count == 0)
        {
            return;
        }

        // Get the next waypoint
        var nextWaypoint = MovePath[0];
        MovePath.RemoveAt(0);

        // Set the target position
        MoveToCell(nextWaypoint);
    }

    /// <summary>
    /// Move the gamepiece to a specific cell.
    /// </summary>
    public void MoveToCell(Vector2I cell)
    {
        // Convert cell to pixel position
        targetPosition = Gameboard.CellToPixel(cell);
        remainingDistance = Position.DistanceTo(targetPosition);

        // Update the direction based on movement
        var movementVector = targetPosition - Position;
        if (!movementVector.IsEqualApprox(Vector2.Zero))
        {
            Direction = Directions.VectorToDirection(movementVector);
        }

        // Update the target cell
        TargetCell = cell;

        // Start moving
        IsMoving = true;

        // Emit the movement started signal
        EmitSignal(SignalName.MovementStarted);

        // Emit the waypoint changed signal
        EmitSignal(SignalName.WaypointChanged, cell);
    }

    /// <summary>
    /// Set the gamepiece's movement path.
    /// </summary>
    public void SetMovePath(List<Vector2I> path)
    {
        if (path == null)
        {
            MovePath.Clear();
        }
        else
        {
            MovePath = new List<Vector2I>(path);
        }

        // If we're not already moving, start moving to the first waypoint
        if (!IsMoving && MovePath.Count > 0)
        {
            MoveToNextWaypoint();
        }
    }

    /// <summary>
    /// Stop the gamepiece's movement.
    /// </summary>
    public void StopMoving()
    {
        IsMoving = false;
        MovePath.Clear();
        remainingDistance = 0.0f;

        // Emit the movement stopped signal
        EmitSignal(SignalName.MovementStopped);
    }

    /// <summary>
    /// Check if the gamepiece can move to a specific cell.
    /// </summary>
    public bool CanMoveToCell(Vector2I cell)
    {
        // Check if the cell is within the gameboard bounds
        if (!Gameboard.Properties.Extents.HasPoint(cell))
        {
            return false;
        }

        // Check if the cell is blocked
        if (!Gameboard.IsCellClear(cell))
        {
            return false;
        }

        // Check if the cell is occupied by another gamepiece
        if (GamepieceRegistry.Instance.IsCellOccupied(cell))
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// Get the adjacent cells that this gamepiece can move to.
    /// </summary>
    public List<Vector2I> GetMoveableAdjacentCells()
    {
        var adjacentCells = Gameboard.GetAdjacentCells(CellPosition);
        var moveableCells = new List<Vector2I>();

        foreach (var cell in adjacentCells)
        {
            if (CanMoveToCell(cell))
            {
                moveableCells.Add(cell);
            }
        }

        return moveableCells;
    }

    /// <summary>
    /// Rotate the gamepiece to face a specific direction.
    /// </summary>
    public void FaceDirection(Directions.Points newDirection)
    {
        Direction = newDirection;
    }

    /// <summary>
    /// Rotate the gamepiece to face a specific position.
    /// </summary>
    public void FacePosition(Vector2 position)
    {
        var directionVector = position - Position;
        Direction = Directions.VectorToDirection(directionVector);
    }
}
