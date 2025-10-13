// <copyright file="PathLoopAIController.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;

/// <summary>
/// AI controller that moves a gamepiece along a looping path defined by a Line2D.
/// The controller will continuously follow the path, waiting between loops and when paths are blocked.
/// </summary>
[Tool]
public partial class PathLoopAIController : GamepieceController
{
    /// <summary>
    /// Gets or sets the Line2D that defines the path points for the gamepiece to follow.
    /// </summary>
    [Export]
    public Line2D PathToFollow
    {
        get => this.pathToFollow;
        set
        {
            this.pathToFollow = value;
            this.UpdateConfigurationWarnings();
        }
    }

    private Line2D? pathToFollow;

    private int currentWaypointIndex = -1;
    private Vector2 pathOrigin = Vector2.Zero;
    private Vector2I startCell = Vector2I.Zero;

    /// <summary>
    /// Timer that controls waiting between path loops and when paths are blocked.
    /// </summary>
    private Godot.Timer? timer;

    /// <inheritdoc/>
    public override void _Ready()
    {
        if (!Engine.IsEditorHint())
        {
            if (this.PathToFollow != null)
            {
                this.PathToFollow.Hide();
            }

            this.pathOrigin = this.Gamepiece?.Position ?? Vector2.Zero;

            // Connect to pathfinder changes to update path when needed
            // Gameboard.PathfinderChanged += OnPathfinderChanged;

            // Initialize timer
            this.timer = new Timer();
            this.timer.OneShot = true;
            this.timer.Timeout += MoveToNextWaypoint;
            this.AddChild(this.timer);

            this.timer.Start();
        }

        base._Ready();
    }

    /// <summary>
    /// Gets or sets a value indicating whether override the active state setter to handle timer pausing.
    /// </summary>
    public bool IsControllerActive
    {
        get => isActive;
        set
        {
            if (value != isActive)
            {
                isActive = value;

                // Pause/unpause the timer when controller state changes
                if (this.timer != null)
                {
                    this.timer.Paused = !isActive;
                }

                // If not paused and timer is stopped, continue movement
                if (isActive && this.timer?.IsStopped() == true)
                {
                    this.MoveToNextWaypoint();
                }
            }
        }
    }

    /// <summary>
    /// Get configuration warnings for the editor.
    /// </summary>
    /// <returns></returns>
    public override string[] _GetConfigurationWarnings()
    {
        var warnings = new List<string>();

        if (this.PathToFollow == null)
        {
            warnings.Add("The path loop controller needs a valid Line2D to follow!");
        }

        return warnings.ToArray();
    }

    /// <summary>
    /// Convert the Line2D path into a series of cells for the gamepiece to move to.
    /// Creates a looping path from the points specified by PathToFollow.
    /// </summary>
    /// <returns>True if a valid path was found, false otherwise.</returns>
    private bool FindMovePath()
    {
        this.MovePath.Clear();

        // A path needs at least two points
        if (this.PathToFollow?.GetPointCount() <= 1)
        {
            return false;
        }

        // Add the first cell to the path
        this.startCell = Gameboard.PixelToCell(this.PathToFollow.GetPointPosition(0) + this.pathOrigin);

        // Create a looping path from the points specified by PathToFollow
        for (int i = 1; i < this.PathToFollow.GetPointCount(); i++)
        {
            var source = Gameboard.PixelToCell(this.PathToFollow.GetPointPosition(i - 1) + this.pathOrigin);
            var target = Gameboard.PixelToCell(this.PathToFollow.GetPointPosition(i) + this.pathOrigin);

            var pathSegment = Gameboard.Pathfinder.GetPathToCell(
                source,
                target,
                Pathfinder.Flags.AllowSourceOccupant | Pathfinder.Flags.AllowTargetOccupant);

            if (!pathSegment.Any())
            {
                GD.PushError($"{this.Name}: Failed to find path between cells {source} and {target}");
                return false;
            }

            this.MovePath.AddRange(pathSegment);
        }

        // Connect the ending and starting cells to complete the loop
        var lastPos = this.PathToFollow.GetPointPosition(this.PathToFollow.GetPointCount() - 1) + this.pathOrigin;
        var lastCell = Gameboard.PixelToCell(lastPos);
        var firstCell = Gameboard.PixelToCell(this.PathToFollow.GetPointPosition(0) + this.pathOrigin);

        // If we've made it this far there must be a path between the first and last cell
        if (lastCell != firstCell)
        {
            var closingPath = Gameboard.Pathfinder.GetPathToCell(
                lastCell,
                firstCell,
                Pathfinder.Flags.AllowSourceOccupant | Pathfinder.Flags.AllowTargetOccupant);
            this.MovePath.AddRange(closingPath);
        }

        return true;
    }

    /// <summary>
    /// Get the next waypoint index, wrapping around to 0 if at the end.
    /// </summary>
    private int GetNextWaypointIndex()
    {
        var nextIndex = this.currentWaypointIndex + 1;
        if (nextIndex >= this.MovePath.Count)
        {
            nextIndex = 0;
        }

        return nextIndex;
    }

    /// <summary>
    /// Move to the next waypoint in the path.
    /// </summary>
    /// <returns>The distance to the waypoint.</returns>
    private float MoveToNextWaypoint()
    {
        float distanceToPoint = 0.0f;

        if (this.IsActive && this.MovePath.Any())
        {
            var nextIndex = this.GetNextWaypointIndex();

            // If the next waypoint is blocked, restart the timer and try again later
            if (Gameboard.Pathfinder.CanMoveTo(this.MovePath[nextIndex]))
            {
                this.currentWaypointIndex = nextIndex;

                var destination = Gameboard.CellToPixel(this.MovePath[this.currentWaypointIndex]);
                distanceToPoint = this.Gamepiece.Position.DistanceTo(destination);
                this.Gamepiece.MoveTo(destination);

                // GamepieceRegistry.MoveGamepiece(Gamepiece, MovePath[currentWaypointIndex]);
            }
            else
            {
                this.timer.Start();
            }
        }

        return distanceToPoint;
    }

    /// <summary>
    /// Override the gamepiece arriving callback to handle path following.
    /// </summary>
    protected override void OnGamepieceArriving(float excessDistance)
    {
        if (this.MovePath.Any() && this.IsActive)
        {
            // Fast gamepieces could jump several waypoints at once, so check which waypoint is next
            while (this.MovePath.Any() && excessDistance > 0.0f)
            {
                if (this.MovePath[this.currentWaypointIndex] == this.startCell ||
                    !Gameboard.Pathfinder.CanMoveTo(this.MovePath[this.GetNextWaypointIndex()]))
                {
                    return;
                }

                var distanceToWaypoint = this.MoveToNextWaypoint();
                excessDistance -= distanceToWaypoint;
            }
        }
    }

    /// <summary>
    /// Override the gamepiece arrived callback to restart the timer.
    /// </summary>
    protected override void OnGamepieceArrived()
    {
        this.timer.Start();
    }

    /// <summary>
    /// Called when the pathfinder changes (cells added/removed).
    /// </summary>
    private void OnPathfinderChanged(List<Vector2I> added, List<Vector2I> removed)
    {
        // Log an error if the path described by PathToFollow cannot be traversed
        if (!this.FindMovePath())
        {
            GD.PrintErr($"Failed to find a path to follow for '{this.Gamepiece?.Name}'!");
            return;
        }
    }

    /// <summary>
    /// Override the active state changed callback to handle timer pausing.
    /// </summary>
    protected override void OnActiveStateChanged()
    {
        base.OnActiveStateChanged();

        // Pause/unpause the timer when controller state changes
        if (this.timer != null)
        {
            this.timer.Paused = !this.IsActive;
        }

        // If not paused and timer is stopped, continue movement
        if (this.IsActive && this.timer?.IsStopped() == true)
        {
            this.MoveToNextWaypoint();
        }
    }
}
