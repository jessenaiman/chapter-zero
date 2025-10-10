using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// A* pathfinding implementation for grid-based movement.
/// The Pathfinder handles finding paths between cells on the gameboard, taking into account
/// blocked cells and other obstacles. It uses the A* algorithm to find the shortest path
/// between two points.
/// </summary>
public partial class Pathfinder : RefCounted
{
    // Dictionary to store points: key = point ID, value = PointData
    private Dictionary<int, PointData> points = new Dictionary<int, PointData>();

    // Dictionary to store connections between points: key = point ID, value = list of connected point IDs
    private Dictionary<int, List<int>> connections = new Dictionary<int, List<int>>();

    /// <summary>
    /// Point data structure
    /// </summary>
    private class PointData
    {
        public Vector2I Position { get; set; }
        public float GScore { get; set; } = float.PositiveInfinity;
        public float FScore { get; set; } = float.PositiveInfinity;
        public bool Disabled { get; set; } = false;
    }

    /// <summary>
    /// Check if the pathfinder has a specific cell
    /// </summary>
    public bool HasCell(Vector2I cell)
    {
        return points.Values.Any(p => p.Position == cell);
    }

    /// <summary>
    /// Check if the pathfinder has a point with the given ID
    /// </summary>
    public bool HasPoint(int pointId)
    {
        return points.ContainsKey(pointId);
    }

    /// <summary>
    /// Add a point to the pathfinder
    /// </summary>
    public void AddPoint(int pointId, Vector2I position)
    {
        if (!points.ContainsKey(pointId))
        {
            points[pointId] = new PointData { Position = position };
            connections[pointId] = new List<int>();
        }
    }

    /// <summary>
    /// Remove a point from the pathfinder
    /// </summary>
    public void RemovePoint(int pointId)
    {
        if (points.ContainsKey(pointId))
        {
            points.Remove(pointId);

            // Remove all connections to this point
            foreach (var connectionsList in connections.Values)
            {
                connectionsList.RemoveAll(id => id == pointId);
            }

            if (connections.ContainsKey(pointId))
            {
                connections.Remove(pointId);
            }
        }
    }

    /// <summary>
    /// Connect two points in the pathfinder
    /// </summary>
    public void ConnectPoints(int pointId1, int pointId2)
    {
        if (points.ContainsKey(pointId1) && points.ContainsKey(pointId2))
        {
            if (!connections[pointId1].Contains(pointId2))
            {
                connections[pointId1].Add(pointId2);
            }
            if (!connections[pointId2].Contains(pointId1))
            {
                connections[pointId2].Add(pointId1);
            }
        }
    }

    /// <summary>
    /// Disconnect two points in the pathfinder
    /// </summary>
    public void DisconnectPoints(int pointId1, int pointId2)
    {
        if (connections.ContainsKey(pointId1))
        {
            connections[pointId1].Remove(pointId2);
        }
        if (connections.ContainsKey(pointId2))
        {
            connections[pointId2].Remove(pointId1);
        }
    }

    /// <summary>
    /// Set whether a point is disabled
    /// </summary>
    public void SetPointDisabled(int pointId, bool disabled = true)
    {
        if (points.ContainsKey(pointId))
        {
            points[pointId].Disabled = disabled;
        }
    }

    /// <summary>
    /// Get whether a point is disabled
    /// </summary>
    public bool IsPointDisabled(int pointId)
    {
        if (points.ContainsKey(pointId))
        {
            return points[pointId].Disabled;
        }
        return false;
    }

    /// <summary>
    /// Get path to a cell using A* algorithm
    /// </summary>
    public List<Vector2I> GetPathToCell(Vector2I startCell, Vector2I endCell)
    {
        var startPoint = points.Values.FirstOrDefault(p => p.Position == startCell);
        var endPoint = points.Values.FirstOrDefault(p => p.Position == endCell);

        if (startPoint == null || endPoint == null)
        {
            return new List<Vector2I>();
        }

        var startPointId = points.First(kvp => kvp.Value == startPoint).Key;
        var endPointId = points.First(kvp => kvp.Value == endPoint).Key;

        return GetPath(startPointId, endPointId);
    }

    /// <summary>
    /// Get path between two points using A* algorithm
    /// </summary>
    public List<Vector2I> GetPath(int startId, int endId)
    {
        // Reset scores
        foreach (var point in points.Values)
        {
            point.GScore = float.PositiveInfinity;
            point.FScore = float.PositiveInfinity;
        }

        if (!points.ContainsKey(startId) || !points.ContainsKey(endId))
        {
            return new List<Vector2I>();
        }

        var start = points[startId];
        var end = points[endId];

        start.GScore = 0;
        start.FScore = HeuristicCostEstimate(start.Position, end.Position);

        var openSet = new PriorityQueue<int, float>();
        var cameFrom = new Dictionary<int, int>();
        var openSetHash = new HashSet<int> { startId };

        openSet.Enqueue(startId, start.FScore);

        while (openSet.Count > 0)
        {
            var currentId = openSet.Dequeue();
            openSetHash.Remove(currentId);

            if (currentId == endId)
            {
                return ReconstructPath(cameFrom, currentId);
            }

            var current = points[currentId];
            if (current.Disabled)
                continue;

            foreach (var neighborId in connections.GetValueOrDefault(currentId, new List<int>()))
            {
                if (!points.ContainsKey(neighborId))
                    continue;

                var neighbor = points[neighborId];
                if (neighbor.Disabled)
                    continue;

                var tentativeGScore = current.GScore + 1; // Distance between neighbors is always 1 in grid

                if (tentativeGScore < neighbor.GScore)
                {
                    cameFrom[neighborId] = currentId;
                    neighbor.GScore = tentativeGScore;
                    neighbor.FScore = neighbor.GScore + HeuristicCostEstimate(neighbor.Position, end.Position);

                    if (!openSetHash.Contains(neighborId))
                    {
                        openSet.Enqueue(neighborId, neighbor.FScore);
                        openSetHash.Add(neighborId);
                    }
                }
            }
        }

        // No path found
        return new List<Vector2I>();
    }

    /// <summary>
    /// Heuristic cost estimate between two positions (Manhattan distance)
    /// </summary>
    private float HeuristicCostEstimate(Vector2I a, Vector2I b)
    {
        return Mathf.Abs(a.X - b.X) + Mathf.Abs(a.Y - b.Y);
    }

    /// <summary>
    /// Reconstruct path from cameFrom dictionary
    /// </summary>
    private List<Vector2I> ReconstructPath(Dictionary<int, int> cameFrom, int currentId)
    {
        var path = new List<Vector2I> { points[currentId].Position };

        while (cameFrom.ContainsKey(currentId))
        {
            currentId = cameFrom[currentId];
            path.Add(points[currentId].Position);
        }

        path.Reverse();
        return path;
    }

    /// <summary>
    /// Get cells to adjacent cell
    /// </summary>
    public List<Vector2I> GetPathCellsToAdjacentCell(Vector2I sourceCell, Vector2I targetCell)
    {
        // Find adjacent cells to the target
        var adjacentCells = GetAdjacentCells(targetCell);
        var shortestPath = new List<Vector2I>();
        var shortestLength = int.MaxValue;

        foreach (var adjacentCell in adjacentCells)
        {
            if (adjacentCell == sourceCell)
                continue;

            var path = GetPathToCell(sourceCell, adjacentCell);
            if (path.Count > 0 && path.Count < shortestLength)
            {
                shortestPath = path;
                shortestLength = path.Count;
            }
        }

        return shortestPath;
    }

    /// <summary>
    /// Get adjacent cells to a given cell
    /// </summary>
    private List<Vector2I> GetAdjacentCells(Vector2I cell)
    {
        var neighbors = new List<Vector2I>();
        var directions = new Vector2I[] {
            Vector2I.Up, Vector2I.Right, Vector2I.Down, Vector2I.Left,
            Vector2I.One, Vector2I.One * -1, new Vector2I(1, -1), new Vector2I(-1, 1)
        };

        foreach (var direction in directions)
        {
            var neighbor = cell + direction;
            if (HasCell(neighbor))
            {
                neighbors.Add(neighbor);
            }
        }

        return neighbors;
    }
}
