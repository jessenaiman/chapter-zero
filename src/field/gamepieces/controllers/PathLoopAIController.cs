
// <copyright file="PathLoopAIController.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using System.Collections.ObjectModel;
using Godot;

namespace OmegaSpiral.Source.Scripts.Field.gamepieces.Controllers;
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
    public Line2D? PathToFollow
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

    private global::OmegaSpiral.Source.Scripts.Field.gameboard.Gameboard? gameboard;

    private global::OmegaSpiral.Source.Scripts.Field.gameboard.Gameboard? GetGameboard()
    {
        if (this.gameboard == null || !IsInstanceValid(this.gameboard))
        {
            this.gameboard = this.GetNodeOrNull<global::OmegaSpiral.Source.Scripts.Field.gameboard.Gameboard>("/root/Gameboard");
        }

        return this.gameboard;
    }

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
            this.timer = new Godot.Timer();
            this.timer.OneShot = true;
            this.timer.Timeout += this.OnTimerTimeout;
            this.AddChild(this.timer);

            if (!this.FindMovePath())
            {
                GD.PushWarning($"{this.Name}: No valid loop path could be generated. The controller will remain idle until a path is available.");
            }

            this.timer.Start();
        }

        base._Ready();
    }

    /// <summary>
    /// Gets or sets a value indicating whether override the active state setter to handle timer pausing.
    /// </summary>
    private bool _isControllerActive;

    /// <summary>
    /// Gets or sets a value indicating whether the controller is active, handling timer pausing.
    /// </summary>
    public bool IsControllerActive
    {
        get => _isControllerActive;
        set
        {
            if (value != _isControllerActive)
            {
                _isControllerActive = value;

                // Pause/unpause the timer when controller state changes
                if (this.timer != null)
                {
                    this.timer.Paused = !_isControllerActive;
                }

                // If not paused and timer is stopped, continue movement
                if (_isControllerActive && this.timer?.IsStopped() == true)
                {
                    this.MoveToNextWaypoint();
                }
            }
        }
    }

    private void OnTimerTimeout()
    {
        this.MoveToNextWaypoint();
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
        var pathDefinition = this.PathToFollow;
        if (!this.ValidatePathDefinition(pathDefinition))
        {
            return false;
        }

        var board = this.GetGameboard();
        if (board == null)
        {
            GD.PushError($"{this.Name}: Unable to resolve Gameboard when finding move path.");
            return false;
        }

        // Use a local mutable list to build the path
        var pathList = new List<Vector2I>();

        // Add the first cell to the path
        this.startCell = board.PixelToCell(pathDefinition!.GetPointPosition(0) + this.pathOrigin);
        pathList.Add(this.startCell);

        // Create a looping path from the points specified by PathToFollow
        if (!this.BuildPathFromPoints(board, pathDefinition!, pathList))
        {
            return false;
        }

        // Connect the ending and starting cells to complete the loop
        if (!this.BuildClosingLoopPath(board, pathDefinition!, pathList))
        {
            return false;
        }

        // Set the path using MoveAlongPath
        this.MoveAlongPath(new ReadOnlyCollection<Vector2I>(pathList));
        this.currentWaypointIndex = -1;

        return true;
    }

    /// <summary>
    /// Validates that the path definition is valid.
    /// </summary>
    private bool ValidatePathDefinition(Line2D? pathDefinition)
    {
        if (pathDefinition == null || pathDefinition.GetPointCount() <= 1)
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// Builds the main path segments from the path definition points.
    /// </summary>
    private bool BuildPathFromPoints(global::OmegaSpiral.Source.Scripts.Field.gameboard.Gameboard board, Line2D pathDefinition, List<Vector2I> pathList)
    {
        for (int i = 1; i < pathDefinition.GetPointCount(); i++)
        {
            var source = board.PixelToCell(pathDefinition.GetPointPosition(i - 1) + this.pathOrigin);
            var target = board.PixelToCell(pathDefinition.GetPointPosition(i) + this.pathOrigin);

            var pathSegment = board.PathFinder.GetPathToCell(source, target);

            if (pathSegment == null || pathSegment.Count == 0)
            {
                GD.PushError($"{this.Name}: Failed to find path between cells {source} and {target}");
                return false;
            }

            // Avoid duplicating the overlap cell between segments.
            if (pathList.Count > 0 && pathSegment.Count > 0 && pathSegment.ElementAt(0) == pathList[^1])
            {
                pathSegment = pathSegment.Skip(1).ToList();
            }

            foreach (var cell in pathSegment)
            {
                pathList.Add(cell);
            }
        }

        return true;
    }

    /// <summary>
    /// Builds the closing path segment that connects the end back to the beginning.
    /// </summary>
    private bool BuildClosingLoopPath(global::OmegaSpiral.Source.Scripts.Field.gameboard.Gameboard board, Line2D pathDefinition, List<Vector2I> pathList)
    {
        // Connect the ending and starting cells to complete the loop
        var lastPos = pathDefinition.GetPointPosition(pathDefinition.GetPointCount() - 1) + this.pathOrigin;
        var lastCell = board.PixelToCell(lastPos);
        var firstCell = board.PixelToCell(pathDefinition.GetPointPosition(0) + this.pathOrigin);

        // If we've made it this far there must be a path between the first and last cell
        if (lastCell == firstCell)
        {
            return true;
        }

        var closingPath = board.PathFinder.GetPathToCell(lastCell, firstCell);
        if (closingPath == null || closingPath.Count == 0)
        {
            GD.PushError($"{this.Name}: Failed to close loop between {lastCell} and {firstCell}");
            return false;
        }

        if (closingPath.Count > 0 && closingPath.ElementAt(0) == pathList[^1])
        {
            closingPath = closingPath.Skip(1).ToList();
        }

        foreach (var cell in closingPath)
        {
            pathList.Add(cell);
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

        if (this.IsActive && this.MovePath.Count != 0)
        {
            var nextIndex = this.GetNextWaypointIndex();
            var nextCell = this.MovePath[nextIndex];

            // If the next waypoint is blocked, restart the timer and try again later
            if (this.Gamepiece.CanMoveToCell(nextCell))
            {
                this.currentWaypointIndex = nextIndex;

                var board = this.GetGameboard();
                if (board != null)
                {
                    var destination = board.CellToPixel(this.MovePath[this.currentWaypointIndex]);
                    distanceToPoint = this.Gamepiece.Position.DistanceTo(destination);
                }

                this.Gamepiece.MoveToCell(nextCell);

                // GamepieceRegistry.MoveGamepiece(Gamepiece, MovePath[currentWaypointIndex]);
            }
            else
            {
                this.timer?.Start();
            }
        }

        return distanceToPoint;
    }

    /// <summary>
    /// Override the gamepiece arriving callback to handle path following.
    /// </summary>
    /// <param name="excessDistance"></param>
    protected override void OnGamepieceArriving(float excessDistance)
    {
        if (this.MovePath.Count != 0 && this.IsActive)
        {
            // Fast gamepieces could jump several waypoints at once, so check which waypoint is next
            while (this.MovePath.Count != 0 && excessDistance > 0.0f)
            {
                var nextIndex = this.GetNextWaypointIndex();
                if ((this.currentWaypointIndex >= 0 && this.MovePath[this.currentWaypointIndex] == this.startCell) ||
                    !this.Gamepiece.CanMoveToCell(this.MovePath[nextIndex]))
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
        this.timer?.Start();
    }

    /// <summary>
    /// Called when the pathfinder changes (cells added/removed).
    /// </summary>
    /// <param name="added"></param>
    /// <param name="removed"></param>
    private void OnPathfinderChanged(Godot.Collections.Array added, Godot.Collections.Array removed)
    {
        _ = added;
        _ = removed;

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
#pragma warning restore CA1502, CA1505
