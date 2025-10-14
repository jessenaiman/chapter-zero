// <copyright file="Pathfinder.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

/// <summary>
/// A* pathfinding implementation for grid-based movement.
/// The Pathfinder handles finding paths between cells on the gameboard, taking into account
/// blocked cells and other obstacles. It uses the A* algorithm to find the shortest path
/// between two points.
/// </summary>
public partial class Pathfinder : RefCounted
{
    /// <summary>
    /// Dictionary to store points in the pathfinder.
    /// Key is the point ID, value is the <see cref="PointData"/> for that point.
    /// </summary>
    private Dictionary<int, PointData> points = new Dictionary<int, PointData>();

    /// <summary>
    /// Dictionary to store connections between points.
    /// Key is the point ID, value is a list of connected point IDs.
    /// </summary>
    private Dictionary<int, List<int>> connections = new Dictionary<int, List<int>>();

    /// <summary>
    /// Check if the pathfinder has a specific cell.
    /// </summary>
    /// <param name="cell">The cell position to check for.</param>
    /// <returns>True if the cell exists in the pathfinder, false otherwise.</returns>
    public bool HasCell(Vector2I cell)
    {
        return this.points.Values.Any(p => p.Position == cell);
    }

    /// <summary>
    /// Check if the pathfinder has a point with the given ID.
    /// </summary>
    /// <param name="pointId">The point ID to check for.</param>
    /// <returns>True if the point exists in the pathfinder, false otherwise.</returns>
    public bool HasPoint(int pointId)
    {
        return this.points.ContainsKey(pointId);
    }

    /// <summary>
    /// Add a point to the pathfinder.
    /// </summary>
    /// <param name="pointId">The unique identifier for the point.</param>
    /// <param name="position">The position of the point in the grid.</param>
    public void AddPoint(int pointId, Vector2I position)
    {
        if (!this.points.ContainsKey(pointId))
        {
            this.points[pointId] = new PointData { Position = position };
            this.connections[pointId] = new List<int>();
        }
    }

    /// <summary>
    /// Remove a point from the pathfinder.
    /// </summary>
    /// <param name="pointId">The unique identifier of the point to remove.</param>
    public void RemovePoint(int pointId)
    {
        if (this.points.ContainsKey(pointId))
        {
            this.points.Remove(pointId);

            // Remove all connections to this point
            foreach (var connectionsList in this.connections.Values)
            {
                connectionsList.RemoveAll(id => id == pointId);
            }

            if (this.connections.ContainsKey(pointId))
            {
                this.connections.Remove(pointId);
            }
        }
    }

    /// <summary>
    /// Connect two points in the pathfinder.
    /// </summary>
    /// <param name="pointId1">The unique identifier of the first point.</param>
    /// <param name="pointId2">The unique identifier of the second point.</param>
    public void ConnectPoints(int pointId1, int pointId2)
    {
        if (this.points.ContainsKey(pointId1) && this.points.ContainsKey(pointId2))
        {
            if (!this.connections[pointId1].Contains(pointId2))
            {
                this.connections[pointId1].Add(pointId2);
            }

            if (!this.connections[pointId2].Contains(pointId1))
            {
                this.connections[pointId2].Add(pointId1);
            }
        }
    }

    /// <summary>
    /// Disconnect two points in the pathfinder.
    /// </summary>
    /// <param name="pointId1">The unique identifier of the first point.</param>
    /// <param name="pointId2">The unique identifier of the second point.</param>
    public void DisconnectPoints(int pointId1, int pointId2)
    {
        if (this.connections.ContainsKey(pointId1))
        {
            this.connections[pointId1].Remove(pointId2);
        }

        if (this.connections.ContainsKey(pointId2))
        {
            this.connections[pointId2].Remove(pointId1);
        }
    }

    /// <summary>
    /// Set whether a point is disabled.
    /// </summary>
    /// <param name="pointId">The unique identifier of the point.</param>
    /// <param name="disabled">True to disable the point, false to enable it. Default is true.</param>
    public void SetPointDisabled(int pointId, bool disabled = true)
    {
        if (this.points.ContainsKey(pointId))
        {
            this.points[pointId].Disabled = disabled;
        }
    }

    /// <summary>
    /// Get whether a point is disabled.
    /// </summary>
    /// <param name="pointId">The unique identifier of the point.</param>
    /// <returns>True if the point is disabled, false otherwise.</returns>
    public bool IsPointDisabled(int pointId)
    {
        if (this.points.ContainsKey(pointId))
        {
            return this.points[pointId].Disabled;
        }

        return false;
    }

    /// <summary>
    /// Get path to a cell using A* algorithm.
    /// </summary>
    /// <param name="startCell">The starting cell position.</param>
    /// <param name="endCell">The target cell position.</param>
    /// <returns>A list of cell positions representing the path from start to end cell, or empty list if no path found.</returns>
    public List<Vector2I> GetPathToCell(Vector2I startCell, Vector2I endCell)
    {
        var startPoint = this.points.Values.FirstOrDefault(p => p.Position == startCell);
        var endPoint = this.points.Values.FirstOrDefault(p => p.Position == endCell);

        if (startPoint == null || endPoint == null)
        {
            return new List<Vector2I>();
        }

        var startPointId = this.points.First(kvp => kvp.Value == startPoint).Key;
        var endPointId = this.points.First(kvp => kvp.Value == endPoint).Key;

        return this.GetPath(startPointId, endPointId);
    }

    /// <summary>
    /// Get path between two points using A* algorithm.
    /// </summary>
    /// <param name="startId">The unique identifier of the starting point.</param>
    /// <param name="endId">The unique identifier of the target point.</param>
    /// <returns>A list of cell positions representing the path from start to end point, or empty list if no path found.</returns>
    public List<Vector2I> GetPath(int startId, int endId)
    {
        // Reset scores
        foreach (var point in this.points.Values)
        {
            point.GScore = float.PositiveInfinity;
            point.FScore = float.PositiveInfinity;
        }

        if (!this.points.ContainsKey(startId) || !this.points.ContainsKey(endId))
        {
            return new List<Vector2I>();
        }

        var start = this.points[startId];
        var end = this.points[endId];

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
                return this.ReconstructPath(cameFrom, currentId);
            }

            var current = this.points[currentId];
            if (current.Disabled)
            {
                continue;
            }

            foreach (var neighborId in this.connections.GetValueOrDefault(currentId, new List<int>()))
            {
                if (!this.points.ContainsKey(neighborId))
                {
                    continue;
                }

                var neighbor = this.points[neighborId];
                if (neighbor.Disabled)
                {
                    continue;
                }

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
    /// Get cells to adjacent cell.
    /// </summary>
    /// <param name="sourceCell">The source cell position.</param>
    /// <param name="targetCell">The target cell position to find adjacent cells for.</param>
    /// <returns>A list of cell positions representing the shortest path from source to an adjacent cell of the target, or empty list if no path found.</returns>
    public List<Vector2I> GetPathCellsToAdjacentCell(Vector2I sourceCell, Vector2I targetCell)
    {
        // Find adjacent cells to the target
        var adjacentCells = this.GetAdjacentCells(targetCell);
        var shortestPath = new List<Vector2I>();
        var shortestLength = int.MaxValue;

        foreach (var adjacentCell in adjacentCells)
        {
            if (adjacentCell == sourceCell)
            {
                continue;
            }

            var path = this.GetPathToCell(sourceCell, adjacentCell);
            if (path.Count > 0 && path.Count < shortestLength)
            {
                shortestPath = path;
                shortestLength = path.Count;
            }
        }

        return shortestPath;
    }

    /// <summary>
    /// Heuristic cost estimate between two positions (Manhattan distance).
    /// </summary>
    /// <param name="a">The first position.</param>
    /// <param name="b">The second position.</param>
    private static float HeuristicCostEstimate(Vector2I a, Vector2I b)
    {
        return Mathf.Abs(a.X - b.X) + Mathf.Abs(a.Y - b.Y);
    }

    /// <summary>
    /// Get adjacent cells to a given cell.
    /// </summary>
    /// <param name="cell">The cell position to find adjacent cells for.</param>
    private List<Vector2I> GetAdjacentCells(Vector2I cell)
    {
        var neighbors = new List<Vector2I>();
        var directions = new Vector2I[]
        {
            Vector2I.Up, Vector2I.Right, Vector2I.Down, Vector2I.Left,
            Vector2I.One, Vector2I.One * -1, new Vector2I(1, -1), new Vector2I(-1, 1),
        };

        foreach (var direction in directions)
        {
            var neighbor = cell + direction;
            if (this.HasCell(neighbor))
            {
                neighbors.Add(neighbor);
            }
        }

        return neighbors;
    }

    /// <summary>
    /// Reconstruct path from cameFrom dictionary.
    /// </summary>
    /// <param name="cameFrom">A dictionary mapping each point ID to its predecessor in the path.</param>
    /// <param name="currentId">The point ID to start reconstructing the path from (typically the end point).</param>
    private List<Vector2I> ReconstructPath(Dictionary<int, int> cameFrom, int currentId)
    {
        var path = new List<Vector2I> { this.points[currentId].Position };

        while (cameFrom.ContainsKey(currentId))
        {
            currentId = cameFrom[currentId];
            path.Add(this.points[currentId].Position);
        }

        path.Reverse();
        return path;
    }

    /// <summary>
    /// Point data structure.
    /// </summary>
    private class PointData
    {
        public Vector2I Position { get; set; }

        public float GScore { get; set; } = float.PositiveInfinity;

        public float FScore { get; set; } = float.PositiveInfinity;

        public bool Disabled { get; set; }
    }
}
