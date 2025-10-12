using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

/// <summary>
/// AI controller that moves a gamepiece along a looping path defined by a Line2D.
/// The controller will continuously follow the path, waiting between loops and when paths are blocked.
/// </summary>
[Tool]
public partial class PathLoopAIController : GamepieceController
{
    /// <summary>
    /// The Line2D that defines the path points for the gamepiece to follow.
    /// </summary>
    [Export]
    public Line2D PathToFollow
    {
        get => pathToFollow;
        set
        {
            pathToFollow = value;
            UpdateConfigurationWarnings();
        }
    }

    private Line2D pathToFollow;

    private int currentWaypointIndex = -1;
    private Vector2 pathOrigin = Vector2.Zero;
    private Vector2I startCell = Vector2I.Zero;

    /// <summary>
    /// Timer that controls waiting between path loops and when paths are blocked.
    /// </summary>
    private Godot.Timer timer;

    public override void _Ready()
    {
        if (!Engine.IsEditorHint())
        {
            if (PathToFollow != null)
            {
                PathToFollow.Hide();
            }

            pathOrigin = Gamepiece?.Position ?? Vector2.Zero;

            // Connect to pathfinder changes to update path when needed
            // Gameboard.PathfinderChanged += OnPathfinderChanged;

            // Initialize timer
            timer = new Timer();
            timer.OneShot = true;
            timer.Timeout += MoveToNextWaypoint;
            AddChild(timer);

            timer.Start();
        }

        base._Ready();
    }

    /// <summary>
    /// Override the active state setter to handle timer pausing.
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
                if (timer != null)
                {
                    timer.Paused = !isActive;
                }

                // If not paused and timer is stopped, continue movement
                if (isActive && timer?.IsStopped() == true)
                {
                    MoveToNextWaypoint();
                }
            }
        }
    }

    /// <summary>
    /// Get configuration warnings for the editor.
    /// </summary>
    public override string[] _GetConfigurationWarnings()
    {
        var warnings = new List<string>();

        if (PathToFollow == null)
        {
            warnings.Add("The path loop controller needs a valid Line2D to follow!");
        }

        return warnings.ToArray();
    }

    /// <summary>
    /// Convert the Line2D path into a series of cells for the gamepiece to move to.
    /// Creates a looping path from the points specified by PathToFollow.
    /// </summary>
    /// <returns>True if a valid path was found, false otherwise</returns>
    private bool FindMovePath()
    {
        MovePath.Clear();

        // A path needs at least two points
        if (PathToFollow?.GetPointCount() <= 1)
        {
            return false;
        }

        // Add the first cell to the path
        startCell = Gameboard.PixelToCell(PathToFollow.GetPointPosition(0) + pathOrigin);

        // Create a looping path from the points specified by PathToFollow
        for (int i = 1; i < PathToFollow.GetPointCount(); i++)
        {
            var source = Gameboard.PixelToCell(PathToFollow.GetPointPosition(i - 1) + pathOrigin);
            var target = Gameboard.PixelToCell(PathToFollow.GetPointPosition(i) + pathOrigin);

            var pathSegment = Gameboard.Pathfinder.GetPathToCell(
                source,
                target,
                Pathfinder.Flags.AllowSourceOccupant | Pathfinder.Flags.AllowTargetOccupant
            );

            if (!pathSegment.Any())
            {
                GD.PushError($"{Name}: Failed to find path between cells {source} and {target}");
                return false;
            }

            MovePath.AddRange(pathSegment);
        }

        // Connect the ending and starting cells to complete the loop
        var lastPos = PathToFollow.GetPointPosition(PathToFollow.GetPointCount() - 1) + pathOrigin;
        var lastCell = Gameboard.PixelToCell(lastPos);
        var firstCell = Gameboard.PixelToCell(PathToFollow.GetPointPosition(0) + pathOrigin);

        // If we've made it this far there must be a path between the first and last cell
        if (lastCell != firstCell)
        {
            var closingPath = Gameboard.Pathfinder.GetPathToCell(
                lastCell,
                firstCell,
                Pathfinder.Flags.AllowSourceOccupant | Pathfinder.Flags.AllowTargetOccupant
            );
            MovePath.AddRange(closingPath);
        }

        return true;
    }

    /// <summary>
    /// Get the next waypoint index, wrapping around to 0 if at the end.
    /// </summary>
    private int GetNextWaypointIndex()
    {
        var nextIndex = currentWaypointIndex + 1;
        if (nextIndex >= MovePath.Count)
        {
            nextIndex = 0;
        }
        return nextIndex;
    }

    /// <summary>
    /// Move to the next waypoint in the path.
    /// </summary>
    /// <returns>The distance to the waypoint</returns>
    private float MoveToNextWaypoint()
    {
        float distanceToPoint = 0.0f;

        if (IsActive && MovePath.Any())
        {
            var nextIndex = GetNextWaypointIndex();

            // If the next waypoint is blocked, restart the timer and try again later
            if (Gameboard.Pathfinder.CanMoveTo(MovePath[nextIndex]))
            {
                currentWaypointIndex = nextIndex;

                var destination = Gameboard.CellToPixel(MovePath[currentWaypointIndex]);
                distanceToPoint = Gamepiece.Position.DistanceTo(destination);
                Gamepiece.MoveTo(destination);

                // GamepieceRegistry.MoveGamepiece(Gamepiece, MovePath[currentWaypointIndex]);
            }
            else
            {
                timer.Start();
            }
        }

        return distanceToPoint;
    }

    /// <summary>
    /// Override the gamepiece arriving callback to handle path following.
    /// </summary>
    protected override void OnGamepieceArriving(float excessDistance)
    {
        if (MovePath.Any() && IsActive)
        {
            // Fast gamepieces could jump several waypoints at once, so check which waypoint is next
            while (MovePath.Any() && excessDistance > 0.0f)
            {
                if (MovePath[currentWaypointIndex] == startCell ||
                    !Gameboard.Pathfinder.CanMoveTo(MovePath[GetNextWaypointIndex()]))
                {
                    return;
                }

                var distanceToWaypoint = MoveToNextWaypoint();
                excessDistance -= distanceToWaypoint;
            }
        }
    }

    /// <summary>
    /// Override the gamepiece arrived callback to restart the timer.
    /// </summary>
    protected override void OnGamepieceArrived()
    {
        timer.Start();
    }

    /// <summary>
    /// Called when the pathfinder changes (cells added/removed).
    /// </summary>
    private void OnPathfinderChanged(List<Vector2I> added, List<Vector2I> removed)
    {
        // Log an error if the path described by PathToFollow cannot be traversed
        if (!FindMovePath())
        {
            GD.PrintErr($"Failed to find a path to follow for '{Gamepiece?.Name}'!");
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
        if (timer != null)
        {
            timer.Paused = !IsActive;
        }

        // If not paused and timer is stopped, continue movement
        if (IsActive && timer?.IsStopped() == true)
        {
            MoveToNextWaypoint();
        }
    }
}
